#define RECOVERED_TEXTURE_NATIVE_LOADER_API_BUILD
#include "TextureNativeLoaderApi.h"
#include "TextureNativeSurfaceApi.h"
#include "TextureNativeResourceApi.h"
#include "TextureNativeCopyApi.h"
#include "../PublicDirectXTex/TexturePublicTriangle.h"
#include "../PublicStbBmp/TexturePublicBmp.h"
#define NOMINMAX
#include <Windows.h>
#include <algorithm>
#include <cstring>
#include <limits>
#include <new>
#include <vector>
static_assert(sizeof(void*)==4,"PC file/resource ABI requires x86");
namespace {
using U=std::uint32_t;
constexpr std::int32_t Invalid=static_cast<std::int32_t>(0x8876086cu),BadFile=static_cast<std::int32_t>(0x88760b59u),InvalidData=static_cast<std::int32_t>(0x80004005u),Unsupported=static_cast<std::int32_t>(0x80004001u),OutOfMemory=static_cast<std::int32_t>(0x8007000eu);
constexpr U Bits[]={1,1,8,16,16,16,32,32,8,8,16,16,16,32,32,16,32,32,4,8,8,64,32,32,16,32,64,16,32,64,16,32,64,32,64,128,32,64,128,8,16,16,32,8,8,16,16,16,32,8,32,4,8,8,32,32,32,96,4,4,4,4,32,32};
U Load(const void* p,U offset){U n;std::memcpy(&n,static_cast<const unsigned char*>(p)+offset,4);return n;}
void Store(void* p,U offset,U n){std::memcpy(static_cast<unsigned char*>(p)+offset,&n,4);}
U Address(const void* p){return static_cast<U>(reinterpret_cast<std::uintptr_t>(p));}
U Round(U n,U a){return (n+a-1)&~(a-1);}
U Log(U n){U e=0;for(U p=1;p<n;p<<=1)++e;return e;}
void Blocks(U f,U& x,U& y){if(f==11||f==12||f==40){x=2;y=1;}else if(f==39||f==44){x=4;y=1;}else if((f>=18&&f<=20)||f==49||(f>=51&&f<=53)||(f>=58&&f<=61)){x=y=4;}else{x=y=1;}}
struct Size{U width,height,bytes;};
Size Aligned(U w,U h,U f){U bx,by;Blocks(f,bx,by);const U bytes=bx*by*Bits[f]/8,multiple=bytes?std::max(32u,256/bytes):32u;w=Round(w,bx*multiple);h=Round(h,by*32);return {w,h,Round(w*Bits[f]/8*h,4096)};}
bool Format(U f){return (f&63)>=2&&!(f&0xc0000100u);}
struct File{HANDLE value=INVALID_HANDLE_VALUE;~File(){if(value!=INVALID_HANDLE_VALUE)CloseHandle(value);}};
std::int32_t ReadFileBytes(const char* path,std::vector<unsigned char>& bytes){
 if(!path)return Invalid;
 File file;file.value=CreateFileA(path,GENERIC_READ,FILE_SHARE_READ,nullptr,OPEN_EXISTING,FILE_FLAG_RANDOM_ACCESS,nullptr);
 if(file.value==INVALID_HANDLE_VALUE)return BadFile;
 LARGE_INTEGER size{};if(!GetFileSizeEx(file.value,&size)||size.QuadPart<0||size.QuadPart>0x10000000)return BadFile;
 bytes.resize(static_cast<std::size_t>(size.QuadPart));DWORD count=0;
 if(!ReadFile(file.value,bytes.data(),static_cast<DWORD>(bytes.size()),&count,nullptr)||count!=bytes.size())return BadFile;
 return 0;
}
struct Resource{void* p=nullptr;~Resource(){if(p)TextureResourceRelease(p);}void* detach(){auto value=p;p=nullptr;return value;}};
std::int32_t LoadDds(const std::vector<unsigned char>& bytes,void** output);
std::int32_t LoadTga(const std::vector<unsigned char>& bytes,void** output);
std::int32_t LoadBmp(const std::vector<unsigned char>& bytes,void** output);
}
extern "C" RECOVERED_TEXTURE_NATIVE_LOADER_API void* __stdcall TextureCreateCpuTexture(U width,U height,U levels,U format){
 if(!width||!height||width>8192||height>8192||!Format(format))return nullptr;
 U maximum=1;for(U n=std::max(width,height);n>1;n>>=1)++maximum;if(!levels)levels=maximum;if(levels>maximum||levels>16)return nullptr;
 // RVA c9f6: ordinary 2D resources have one face, packed tails iff levels>1.
 const U f=format&63,ex=Log(width),ey=Log(height);const bool packed=levels>1,basePacked=packed&&(width<=16||height<=16);
 const auto base=Aligned(basePacked?1u<<ex:width,basePacked?1u<<ey:height,f);
 U secondaryBytes=0,x=ex,y=ey;
 for(U level=1;level<levels;++level){if(packed&&(x<=4||y<=4))break;if(x)--x;if(y)--y;secondaryBytes+=Aligned(1u<<x,1u<<y,f).bytes;}
 // All computations are bounded by 8192 dimensions and 128 bits/texel.
 auto p=GlobalAlloc(GMEM_ZEROINIT,52);if(!p)return nullptr;
 U primaryBytes=base.bytes;if(!packed){U bx,by;Blocks(f,bx,by);primaryBytes=Round(base.width*Bits[f]/8*Round(height,by),4096);}
 auto primary=VirtualAlloc(nullptr,primaryBytes,MEM_COMMIT,PAGE_READWRITE);if(!primary){GlobalFree(p);return nullptr;}
 void* secondary=nullptr;if(secondaryBytes){secondary=VirtualAlloc(nullptr,secondaryBytes,MEM_COMMIT,PAGE_READWRITE);if(!secondary){VirtualFree(primary,0,MEM_RELEASE);GlobalFree(p);return nullptr;}}
 Store(p,0,0x100003);Store(p,4,1);Store(p,0x14,0xffff0000u);Store(p,0x18,0xffff0000u);
 Store(p,0x1c,2|((base.width>>5)<<22)|((format>>7)&0x3fc)|((format&0x100)<<23));
 Store(p,0x20,Address(primary)|(format&255));Store(p,0x24,(width-1)|((height-1)<<13));
 Store(p,0x28,(format>>17)&0x1fff);Store(p,0x2c,(levels-1)<<6);Store(p,0x30,Address(secondary)|0x200|(packed?0x800:0));
 return p;
}
extern "C" RECOVERED_TEXTURE_NATIVE_LOADER_API std::int32_t __stdcall TextureCreateTextureFromFileA(void* device,const char* path,void** output){
 if(!output)return Invalid;
 // The file-open wrapper returns before touching output on an open/read error.
 if(!device||!path)return Invalid;
 try{std::vector<unsigned char> bytes;auto status=ReadFileBytes(path,bytes);if(status<0)return status;if(bytes.size()>=4&&Load(bytes.data(),0)==0x20534444)return LoadDds(bytes,output);if(bytes.size()>=2&&bytes[0]=='B'&&bytes[1]=='M')return LoadBmp(bytes,output);return LoadTga(bytes,output);}
 catch(const std::bad_alloc&){return OutOfMemory;}
}
namespace {
float FloatStore(double value){volatile float result=static_cast<float>(value);return result;}
float TruncateFloat(double value){float rounded=FloatStore(value);U bits;std::memcpy(&bits,&rounded,4);if(static_cast<double>(rounded)>value){--bits;std::memcpy(&rounded,&bits,4);}return rounded;}
unsigned char GenericBoxByte(unsigned char tl,unsigned char tr,unsigned char bl,unsigned char br){
 // RVA17825 fast byte BOX requires BOTH source axes>=2. A collapsed axis
 // uses the normalized float row interface, duplicates that axis, performs
 // three sequential float32 adds, then a float32 quarter store (17a89..b1e).
 // The RGBA8 row writer1cfd0 uses x87 toward-zero stores before adding .5.
 constexpr double inverse=0.0039215688593685627;
 const float a=FloatStore(tl*inverse),b=FloatStore(tr*inverse),c=FloatStore(bl*inverse),d=FloatStore(br*inverse);
 const float first=FloatStore(static_cast<double>(b)+a),second=FloatStore(static_cast<double>(c)+first),third=FloatStore(static_cast<double>(d)+second);
 const float average=FloatStore(static_cast<double>(third)*0.25);
 const float scaled=TruncateFloat(static_cast<double>(average)*255.0);
 return static_cast<unsigned char>(std::min(255,std::max(0,static_cast<int>(TruncateFloat(static_cast<double>(scaled)+0.5)))));
}
std::int32_t UploadRawLevel(void* resource,U level,const unsigned char* source,U sourcePitch,U sw,U sh,U tw,U th,U format){
 Recovered::TextureCompiler::NativeSurfaceAbi::LockedRect locked{};
 auto status=TextureSurfaceLockRect(resource,level,&locked,nullptr,0);if(status<0)return status;
 try{
  if(sw==tw&&sh==th)status=TextureCopySurface(locked.bits,locked.pitch,tw,th,format&~0x1c0u,nullptr,source,sourcePitch,format&~0x1c0u,nullptr,0,0.0f);
  else Recovered::TextureCompiler::PublicDirectXTex::ResizeBgra8(source,sourcePitch,sw,sh,static_cast<unsigned char*>(locked.bits),locked.pitch,tw,th,((format>>27)&7)!=5);
 }catch(...){TextureResourceUnlockRect(resource,level);throw;}
 TextureResourceUnlockRect(resource,level);return status;
}
std::int32_t GenerateRawMips(void* resource,U first,U levels,U width,U height,U format){
 for(U level=first;level<levels;++level){
  const U sw=std::max(1u,width>>(level-1)),sh=std::max(1u,height>>(level-1)),w=std::max(1u,sw>>1),h=std::max(1u,sh>>1),nx=sw>1?2:1,ny=sh>1?2:1,count=nx*ny;
  Recovered::TextureCompiler::NativeSurfaceAbi::LockedRect source{},target{};
  TextureSurfaceLockRect(resource,level-1,&source,nullptr,1);TextureSurfaceLockRect(resource,level,&target,nullptr,0);
  const auto input=static_cast<const unsigned char*>(source.bits);auto output=static_cast<unsigned char*>(target.bits);
  for(U y=0;y<h;++y)for(U x=0;x<w;++x)for(U c=0;c<4;++c){
   U sum=0;for(U dy=0;dy<ny;++dy)for(U dx=0;dx<nx;++dx)sum+=input[(y*ny+dy)*source.pitch+(x*nx+dx)*4+c];
   // The two-axis byte BOX fast path rounds half up. X8's
   // unused byte is zero in generated levels, independently of base padding.
   if(c==3&&((format>>27)&7)==5)output[y*target.pitch+x*4+c]=0;
   else if(sw>=2&&sh>=2)output[y*target.pitch+x*4+c]=static_cast<unsigned char>((sum+2)/4);
   else output[y*target.pitch+x*4+c]=GenericBoxByte(input[(y*ny)*source.pitch+(x*nx)*4+c],input[(y*ny)*source.pitch+(x*nx+nx-1)*4+c],input[(y*ny+ny-1)*source.pitch+(x*nx)*4+c],input[(y*ny+ny-1)*source.pitch+(x*nx+nx-1)*4+c]);
  }
  TextureResourceUnlockRect(resource,level);TextureResourceUnlockRect(resource,level-1);
 }
 return 0;
}
std::int32_t LoadDds(const std::vector<unsigned char>& bytes,void** output){
 if(bytes.size()<128||Load(bytes.data(),4)!=124||Load(bytes.data(),76)!=32)return InvalidData;
 const U flags=Load(bytes.data(),80),bits=Load(bytes.data(),88),r=Load(bytes.data(),92),g=Load(bytes.data(),96),b=Load(bytes.data(),100),a=Load(bytes.data(),104);
 U format=0;
 // Original D3DX re-encodes even PROVIDED 2x2/1x1 compressed tail mips.
 // The recovered copy ABI has no compression backend. Preserve that explicit
 // boundary instead of returning a superficially correct but different block.
 if(flags&4)return Unsupported;
 else if((flags&0x40)&&bits==32&&r==0xff0000&&g==0xff00&&b==0xff&&(a==0xff000000||a==0))format=a?0x18280086:0x28280086;
 else return Unsupported;
 const U width=Load(bytes.data(),16),height=Load(bytes.data(),12),sourceLevels=std::max(1u,Load(bytes.data(),28));
 if(!width||!height||width>8192||height>8192)return Unsupported;
 const U targetWidth=1u<<Log(width),targetHeight=1u<<Log(height);
 if((targetWidth!=width||targetHeight!=height)&&std::uint64_t(targetWidth)*targetHeight>=16777216)return Unsupported;
 // D3DXCreateTextureFromFileA requests ordinary 2D even when caps2 carries
 // volume metadata. The native parser's multi-slice/cube paths are separate.
 if((Load(bytes.data(),112)&0xfe00)||Load(bytes.data(),24)>1)return Unsupported;
 const U levels=Log(std::max(targetWidth,targetHeight))+1;
 U sourceMaximum=1;for(U n=std::max(width,height);n>1;n>>=1)++sourceMaximum;
 if(sourceLevels>sourceMaximum)return Unsupported;
 Resource resource;resource.p=TextureCreateCpuTexture(targetWidth,targetHeight,levels,format);if(!resource.p)return OutOfMemory;
 U offset=128;const U f=format&63;U bx,by;Blocks(f,bx,by);U w=width,h=height,tw=targetWidth,th=targetHeight;
 for(U level=0;level<sourceLevels;++level){
  const U rowBytes=((w+bx-1)/bx)*(Bits[f]*bx*by/8),rows=(h+by-1)/by,levelBytes=rowBytes*rows;
  if(offset>bytes.size()||levelBytes>bytes.size()-offset)return InvalidData;
  // CPU file uploads keep the file's little-endian representation. The
  // descriptor still carries the original 0x40/0x80 endian tags. LoadImageA
  // clears bits 0x1c0 before its flags=0x12 logical conversion (IL token17).
  const auto status=UploadRawLevel(resource.p,level,bytes.data()+offset,rowBytes,w,h,tw,th,format);if(status<0)return status;
  offset+=levelBytes;w=std::max(1u,w>>1);h=std::max(1u,h>>1);tw=std::max(1u,tw>>1);th=std::max(1u,th>>1);
 }
 if(sourceLevels<levels)GenerateRawMips(resource.p,sourceLevels,levels,targetWidth,targetHeight,format);
 *output=resource.detach();return 0;
}
}
namespace {
bool Text(const std::vector<unsigned char>& data,U at,const char* text,U length){return at<=data.size()&&length<=data.size()-at&&std::memcmp(data.data()+at,text,length)==0;}
U Word(const std::vector<unsigned char>& data,U at){return data[at]|(U(data[at+1])<<8);}
bool TgaTail(const std::vector<unsigned char>& data,U start){
 if(start==data.size())return true;
 // Ordinary footer/extension frames. Rendering metadata is ignored by the
 // original PC decoder; validate referenced storage, never apply correction.
 struct Region{U start,end;};std::vector<Region> regions;
 auto add=[&](U at,std::uint64_t count,U limit){if(!at&&!count)return true;if(at<start||at>limit||count>limit-at)return false;regions.push_back({at,at+static_cast<U>(count)});return true;};
 if(data.size()>=26&&Text(data,static_cast<U>(data.size()-18),"TRUEVISION-XFILE.\0",18)){
  const U footer=static_cast<U>(data.size()-26),extension=Load(data.data(),footer),developer=Load(data.data(),footer+4);
  bool valid=true;
  if(extension){
   valid=add(extension,495,footer);
   if(valid){
    valid=Word(data,extension)==495&&data[extension+494]<=4;
    // The bounded DevIL dialect is handled below, with its shifted fields.
    if(Text(data,extension+438,"Developer's Image Library (DevIL)",32))valid=false;
    if(valid){
     const U correction=Load(data.data(),extension+482),stamp=Load(data.data(),extension+486),scanlines=Load(data.data(),extension+490);
     if(correction)valid=add(correction,2048,footer);
     if(valid&&scanlines){const U height=Word(data,14);valid=add(scanlines,std::uint64_t(height)*4,footer);if(valid)for(U row=0;row<height;++row){const U offset=Load(data.data(),scanlines+row*4);if(offset<18u+data[0]||offset>=start){valid=false;break;}}}
     if(valid&&stamp){valid=add(stamp,2,footer);if(valid){const U w=data[stamp],h=data[stamp+1];valid=w&&h&&w<=64&&h<=64&&add(stamp,2+std::uint64_t(w)*h*(data[16]/8),footer);}}
    }
   }
  }
  if(valid&&developer){valid=add(developer,2,footer);if(valid){const U count=Word(data,developer);valid=add(developer,2+std::uint64_t(count)*10,footer);for(U i=0;valid&&i<count;++i){const U at=developer+2+i*10;valid=add(Load(data.data(),at+2),Load(data.data(),at+6),footer);}}}
  if(valid){add(footer,26,static_cast<U>(data.size()));std::sort(regions.begin(),regions.end(),[](Region a,Region b){return a.start<b.start;});U covered=start;for(auto r:regions){if(r.start>covered){valid=false;break;}covered=std::max(covered,r.end);}if(valid&&covered==data.size())return true;}
 }
 // Legacy concatenated footers, including the 507-byte DevIL extension that
 // still declares 495, and the historical omitted final footer NUL.
 U at=start;
 while(at<data.size()){
  const U extension=at;bool hasExtension=false,devil=false;
  if(data.size()-at>=2&&Word(data,at)==495){
   hasExtension=true;devil=Text(data,at+438,"Developer's Image Library (DevIL)",32);const U count=devil?507:495;
   if(count>data.size()-at)return false;
   if(devil){for(U i=379;i<391;++i)if(data[at+i])return false;for(U i=482;i<506;++i)if(data[at+i])return false;if(data[at+506]!=3)return false;}
   else for(U i=470;i<495;++i)if(data[at+i])return false;
   at+=count;
  }
  if(data.size()-at<25||Load(data.data(),at+4)||!Text(data,at+8,"TRUEVISION-XFILE.\0",17))return false;
  const U pointer=Load(data.data(),at);if(pointer&&(!hasExtension||pointer!=extension))return false;
  at+=25;if(at<data.size()&&data[at]==0)++at;else if(!(devil&&at==data.size()))return false;
 }
 return true;
}
std::int32_t LoadRawImage(const std::vector<unsigned char>& pixels,U width,U height,bool alpha,void** output){
 const U targetWidth=1u<<Log(width),targetHeight=1u<<Log(height);
 if(std::uint64_t(targetWidth)*targetHeight>=16777216)return Unsupported;
 const U format=alpha?0x18280086:0x28280086,levels=Log(std::max(targetWidth,targetHeight))+1;
 Resource resource;resource.p=TextureCreateCpuTexture(targetWidth,targetHeight,levels,format);if(!resource.p)return OutOfMemory;
 const auto status=UploadRawLevel(resource.p,0,pixels.data(),width*4,width,height,targetWidth,targetHeight,format);if(status<0)return status;
 GenerateRawMips(resource.p,1,levels,targetWidth,targetHeight,format);*output=resource.detach();return 0;
}
std::int32_t LoadBmp(const std::vector<unsigned char>& bytes,void** output){
 Recovered::TextureCompiler::PublicStbBmp::Image image;
 const auto status=Recovered::TextureCompiler::PublicStbBmp::Decode(bytes,image);if(status<0)return status;
 return LoadRawImage(image.bgra,image.width,image.height,image.alpha,output);
}
std::int32_t LoadTga(const std::vector<unsigned char>& bytes,void** output){
 if(bytes.size()<18||bytes[1]||(bytes[2]!=2&&bytes[2]!=10)||(bytes[16]!=24&&bytes[16]!=32))return Unsupported;
 for(U at=3;at<8;++at)if(bytes[at])return Unsupported;
 const U width=Word(bytes,12),height=Word(bytes,14),depth=bytes[16],descriptor=bytes[17],attributes=descriptor&15;
 if((descriptor&0xc0)||(depth==24&&attributes)||(depth==32&&attributes!=0&&attributes!=8))return Unsupported;
 if(!width||!height)return InvalidData;
 if(width>8192||height>8192)return Unsupported;
 const U targetWidth=1u<<Log(width),targetHeight=1u<<Log(height);
 if(std::uint64_t(targetWidth)*targetHeight>=16777216)return Unsupported;
 const U count=width*height,stride=depth/8;U offset=18+bytes[0];if(offset>bytes.size())return InvalidData;
 std::vector<unsigned char> pixels(count*4);U position=0;
 while(position<count){
  if(bytes[2]==10&&offset==bytes.size())return InvalidData;
  const U packet=bytes[2]==10?bytes[offset++]:0,length=bytes[2]==10?(packet&127)+1:count-position;const bool repeat=bytes[2]==10&&(packet&128);
  if(length>count-position||(bytes[2]==10&&length>width-position%width))return InvalidData;
  if(std::uint64_t(repeat?1:length)*stride>bytes.size()-offset)return InvalidData;
  for(U i=0;i<length;++i){
   const U x=descriptor&0x10?width-1-position%width:position%width,y=descriptor&0x20?position/width:height-1-position/width;
   auto target=pixels.data()+(y*width+x)*4;std::memcpy(target,bytes.data()+offset,stride);if(stride==3)target[3]=255;
   if(!repeat)offset+=stride;++position;
  }
  if(repeat)offset+=stride;
 }
 if(!TgaTail(bytes,offset))return Unsupported;
 bool alpha=false;if(depth==32)for(U pixel=0;pixel<count;++pixel)if(pixels[pixel*4+3]){alpha=true;break;}
 // Original TGA parser changes an entirely zero alpha plane to X8. The
 // stored base bytes stay zero; only the logical alpha selector becomes one.
 return LoadRawImage(pixels,width,height,alpha,output);
}
}
