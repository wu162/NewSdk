#include "../../src/TextureCompiler/Native/TextureNativeLoaderApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeDeviceApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeResourceApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeSurfaceApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeDescriptorApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeCopyApi.h"
#define NOMINMAX
#include <Windows.h>
#include <algorithm>
#include <array>
#include <cstring>
#include <fstream>
#include <iostream>
#include <iterator>
#include <stdexcept>
#include <string>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Cryptography;
#pragma managed(push,off)
namespace {
using namespace Recovered::TextureCompiler;using U=std::uint32_t;
using OriginalCreate=void* (__stdcall *)(U,U,U,U,U,U,U,U);
struct Api{NativeDeviceAbi::CreateFn device;NativeDeviceAbi::ReleaseFn deviceRelease;NativeResourceAbi::RefFn add,release;NativeResourceAbi::UnlockFn unlock;NativeSurfaceAbi::LayoutFn layout;NativeSurfaceAbi::LockFn lock;NativeDescriptorAbi::LevelFn level;NativeDescriptorAbi::TextureFn describe;NativeCopyAbi::CopyFn copy;NativeLoaderAbi::CreateFn create;NativeLoaderAbi::LoadFn load;OriginalCreate originalCreate;bool isolated=false;};
U constructorCases=0,fileCases=0,copyCases=0,rejections=0,mismatches=0,compressedFiles=0,bcFileCases=0,npotCases=0,bmpCases=0;std::string current,failurePath;
U rasterDecoded=0,rasterRejected=0;
void Check(bool c,const char* why){if(!c)throw std::runtime_error(current+": "+why);}
void Isolation(const Api& api){if(api.isolated){Check(!GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"),"Original loaded during loader replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file during loader replay");}}
U Address(const void* p){return static_cast<U>(reinterpret_cast<std::uintptr_t>(p));}
U Load(const void* p,U at){U n;std::memcpy(&n,static_cast<const unsigned char*>(p)+at,4);return n;}
void Store(void* p,U at,U n){std::memcpy(static_cast<unsigned char*>(p)+at,&n,4);}
Api Original(unsigned char* p){return {reinterpret_cast<NativeDeviceAbi::CreateFn>(p+0x1c10),reinterpret_cast<NativeDeviceAbi::ReleaseFn>(p+0x1ad0),reinterpret_cast<NativeResourceAbi::RefFn>(p+0xc599),reinterpret_cast<NativeResourceAbi::RefFn>(p+0x1940),reinterpret_cast<NativeResourceAbi::UnlockFn>(p+0x1980),reinterpret_cast<NativeSurfaceAbi::LayoutFn>(p+0xd24b),reinterpret_cast<NativeSurfaceAbi::LockFn>(p+0x1950),reinterpret_cast<NativeDescriptorAbi::LevelFn>(p+0xdb60),reinterpret_cast<NativeDescriptorAbi::TextureFn>(p+0x3df34),reinterpret_cast<NativeCopyAbi::CopyFn>(p+0x3ccc0),nullptr,reinterpret_cast<NativeLoaderAbi::LoadFn>(p+0x13c82),reinterpret_cast<OriginalCreate>(p+0xdc78),false};}
Api Recovered(const std::string& directory,std::vector<HMODULE>& modules,bool isolated){
 auto module=[&](const char* name){auto m=LoadLibraryA((directory+"/Recovered.TextureNative"+name+".dll").c_str());Check(m!=nullptr,"Native loader dependency load");modules.push_back(m);return m;};
 auto device=module("Device"),resource=module("Resource"),surface=module("Surface"),descriptor=module("Descriptor"),copy=module("Copy"),loader=module("Loader");
 Api a{reinterpret_cast<NativeDeviceAbi::CreateFn>(GetProcAddress(device,"TextureCreateDevice")),reinterpret_cast<NativeDeviceAbi::ReleaseFn>(GetProcAddress(device,"TextureDeviceRelease")),reinterpret_cast<NativeResourceAbi::RefFn>(GetProcAddress(resource,"TextureResourceAddRef")),reinterpret_cast<NativeResourceAbi::RefFn>(GetProcAddress(resource,"TextureResourceRelease")),reinterpret_cast<NativeResourceAbi::UnlockFn>(GetProcAddress(resource,"TextureResourceUnlockRect")),reinterpret_cast<NativeSurfaceAbi::LayoutFn>(GetProcAddress(surface,"TextureSurfaceGetLayout")),reinterpret_cast<NativeSurfaceAbi::LockFn>(GetProcAddress(surface,"TextureSurfaceLockRect")),reinterpret_cast<NativeDescriptorAbi::LevelFn>(GetProcAddress(descriptor,"TextureGetLevelDesc")),reinterpret_cast<NativeDescriptorAbi::TextureFn>(GetProcAddress(descriptor,"TextureXGGetTextureDesc")),reinterpret_cast<NativeCopyAbi::CopyFn>(GetProcAddress(copy,"TextureCopySurface")),reinterpret_cast<NativeLoaderAbi::CreateFn>(GetProcAddress(loader,"TextureCreateCpuTexture")),reinterpret_cast<NativeLoaderAbi::LoadFn>(GetProcAddress(loader,"TextureCreateTextureFromFileA")),nullptr,isolated};
 Check(a.device&&a.deviceRelease&&a.add&&a.release&&a.unlock&&a.layout&&a.lock&&a.level&&a.describe&&a.copy&&a.create&&a.load,"Missing native loader ABI export");return a;
}
struct Trace{std::vector<unsigned char> data;void number(U n){for(U i=0;i<4;++i)data.push_back(static_cast<unsigned char>(n>>(8*i)));}void add(const void* p,std::size_t count){auto b=static_cast<const unsigned char*>(p);data.insert(data.end(),b,b+count);}void name(const std::string& text){number(static_cast<U>(text.size()));add(text.data(),text.size());}};
void Save(const std::string& path,const std::vector<unsigned char>& bytes){std::ofstream file(path,std::ios::binary);file.write(reinterpret_cast<const char*>(bytes.data()),bytes.size());Check(file.good(),"Cannot write input/capture");}
std::vector<unsigned char> Read(const std::string& path){std::ifstream file(path,std::ios::binary);Check(file.is_open(),"Missing input/capture");return {(std::istreambuf_iterator<char>(file)),std::istreambuf_iterator<char>()};}
struct Storage{void* primary;void* secondary;U psize,ssize;};
U Region(void* p){if(!p)return 0;MEMORY_BASIC_INFORMATION info{};Check(VirtualQuery(p,&info,sizeof(info))==sizeof(info)&&info.BaseAddress==p&&info.AllocationBase==p&&info.State==MEM_COMMIT&&info.RegionSize<=0x10000000,"Texture allocation pairing/bounds");return static_cast<U>(info.RegionSize);}
Storage Owned(void* p){auto primary=reinterpret_cast<void*>(Load(p,0x20)&~0xfffu),secondary=reinterpret_cast<void*>(Load(p,0x30)&~0xfffu);Check(primary&&primary!=secondary,"Owned nonaliased allocations");return {primary,secondary,Region(primary),Region(secondary)};}
U Normalize(const Storage& s,const void* pointer){U p=Address(pointer);if(p>=Address(s.primary)&&p-Address(s.primary)<s.psize)return 0x10000000u+p-Address(s.primary);if(s.secondary&&p>=Address(s.secondary)&&p-Address(s.secondary)<s.ssize)return 0x20000000u+p-Address(s.secondary);throw std::runtime_error(current+": Level address outside owned allocations");}
void Header(Trace& trace,const void* p,const Storage& s){std::array<unsigned char,52> h;std::memcpy(h.data(),p,52);Store(h.data(),0x20,(Load(p,0x20)&0xfff)|0x10000000);Store(h.data(),0x30,(Load(p,0x30)&0xfff)|(s.secondary?0x20000000:0));trace.add(h.data(),h.size());}
void Release(Api api,void* p,const Storage& s){Isolation(api);Check(api.release(p)==0,"Final loader resource release");Isolation(api);for(auto address:{s.primary,s.secondary})if(address){MEMORY_BASIC_INFORMATION info{};Check(VirtualQuery(address,&info,sizeof(info))==sizeof(info)&&info.State==MEM_FREE,"Loader allocation not freed");}}
Trace Consume(Api api,void* p,bool pixels){
 Trace t;Isolation(api);Check(p&&(Load(p,0)&15)==3&&Load(p,4)==1,"Created resource header/refcount");auto s=Owned(p);t.number(s.psize);t.number(s.ssize);Header(t,p,s);
 Check(api.add(p)==2&&api.release(p)==1,"Created resource add/release ownership");Isolation(api);
 const U levels=((Load(p,0x2c)>>6)&15)+1;t.number(levels);
 for(U level=0;level<levels;++level){
  NativeDescriptorAbi::LevelDesc desc{};NativeDescriptorAbi::TextureDesc texture{};api.level(p,level,&desc);api.describe(p,level,&texture);Isolation(api);t.add(&desc,sizeof(desc));t.add(&texture,sizeof(texture));
  std::array<U,7> layout{};void* address=nullptr;api.layout(p,0,level,&layout[0],&layout[1],&layout[2],&layout[3],&layout[4],&layout[5],&layout[6],&address);Isolation(api);t.add(layout.data(),sizeof(layout));t.number(Normalize(s,address));
  for(U mode=0;mode<3;++mode){
   const U format=desc.format&63;const bool compressed=format>=18&&format<=20;
   const U cropX=mode==1&&desc.width>=8?4:0,cropY=mode==1&&desc.height>=8?4:0;
   // Original LockRect offsets BC rectangles in raw pitch/bit units, not
   // ordinary texels: left16/top1 encodes the same logical block crop4/4.
   // The independent surface matrix retains arbitrary raw rect ABI checks.
   NativeSurfaceAbi::LockedRect locked{};NativeSurfaceAbi::Rect rect{static_cast<std::int32_t>(compressed?cropX*4:cropX),static_cast<std::int32_t>(compressed?cropY/4:cropY),static_cast<std::int32_t>(desc.width),static_cast<std::int32_t>(desc.height)};
   Check(api.lock(p,level,&locked,mode==1?&rect:nullptr,mode==2?1:mode==1?0x10:0)==0,"Loaded texture lock");Isolation(api);t.number(locked.pitch);t.number(Normalize(s,locked.bits));Header(t,p,s);
   if(pixels&&((desc.format&63)==2||(desc.format&63)==6||((desc.format&63)>=18&&(desc.format&63)<=20))){
    const U w=desc.width-cropX,h=desc.height-cropY,pitch=w*4+7;
    const U rows=compressed?(h+3)/4:h,rowBytes=compressed?((w+3)/4)*(format==18?8:16):w*(format==2?1:4);
    Check(locked.pitch>=static_cast<std::int32_t>(rowBytes),"Loaded copy source row exceeds pitch");
    const std::uint64_t start=Address(locked.bits),span=std::uint64_t(rows-1)*locked.pitch+rowBytes;
    const auto contained=[&](void* base,U bytes){return base&&start>=Address(base)&&start+span<=std::uint64_t(Address(base))+bytes;};
    Check(contained(s.primary,s.psize)||contained(s.secondary,s.ssize),"Loaded copy source span outside owned VM");
    // Both the exact managed LoadImageA source-format mask and the physical
    // descriptor/endian view are compared, with actual flags0x12/threshold0.
    for(U mask:{~0x1c0u,0xffffffffu}){
     std::vector<unsigned char> rgba(pitch*h+64,0xa5);Check(api.copy(rgba.data()+32,pitch,w,h,0x1a200006,nullptr,locked.bits,locked.pitch,desc.format&mask,nullptr,0x12,0.0f)==0,"Loaded texture actual LoadImageA conversion");Isolation(api);
     for(U i=0;i<32;++i)Check(rgba[i]==0xa5&&rgba[rgba.size()-1-i]==0xa5,"Loaded copy guard");for(U y=0;y<h;++y)for(U x=w*4;x<pitch;++x)Check(rgba[32+y*pitch+x]==0xa5,"Loaded copy row padding");
     t.number(static_cast<U>(rgba.size()));t.add(rgba.data(),rgba.size());++copyCases;
    }
   }
   Check(api.unlock(p,level)==0,"Loaded texture unlock");Isolation(api);Header(t,p,s);
  }
 }
 // Compare all allocation bytes, including untouched zero pages, row padding,
 // packed-tail gaps and both base/secondary storage, not just visible RGBA.
 t.add(s.primary,s.psize);if(s.secondary)t.add(s.secondary,s.ssize);Release(api,p,s);return t;
}
void Equal(const Trace& actual,const Trace& expected){if(actual.data!=expected.data){
 ++mismatches;std::size_t at=0;while(at<std::min(actual.data.size(),expected.data.size())&&actual.data[at]==expected.data[at])++at;
 if(mismatches<=30){std::cerr<<current<<" first difference="<<at<<" actualSize="<<actual.data.size()<<" originalSize="<<expected.data.size();if(at<actual.data.size()&&at<expected.data.size())std::cerr<<" actualByte="<<U(actual.data[at])<<" originalByte="<<U(expected.data[at]);std::cerr<<"\n";}
 if(mismatches<=16){Save(failurePath+"-"+std::to_string(mismatches)+".actual",actual.data);Save(failurePath+"-"+std::to_string(mismatches)+".expected",expected.data);}
 }}
// Stream complete records rather than retaining two growing complete captures
// in a 32-bit process. Every byte/name/record and final EOF is still checked.
struct Gold{
 std::ofstream capture;std::ifstream replay;std::uint64_t bytes=0;bool recording=false;
 void open(const std::string& path,bool original){recording=original;if(original){capture.open(path,std::ios::binary);Check(capture.is_open(),"Cannot create loader capture");}else{replay.open(path,std::ios::binary);Check(replay.is_open(),"Missing loader capture");}}
 void write(const void* p,std::size_t count){capture.write(static_cast<const char*>(p),static_cast<std::streamsize>(count));Check(capture.good(),"Loader capture write failed");}
 U number(){U n=0;replay.read(reinterpret_cast<char*>(&n),4);Check(replay.good(),"Truncated loader capture number");return n;}
 void record(Trace actual,const Trace* original){
  bytes+=8+current.size()+actual.data.size();
  if(original){Equal(actual,*original);U length=static_cast<U>(current.size());write(&length,4);write(current.data(),current.size());length=static_cast<U>(original->data.size());write(&length,4);write(original->data.data(),original->data.size());}
  else{U length=number();Check(length==current.size(),"Loader gold name length");std::string name(length,'\0');replay.read(&name[0],length);Check(replay.good()&&name==current,"Loader gold label");length=number();Trace expected;expected.data.resize(length);replay.read(reinterpret_cast<char*>(expected.data.data()),length);Check(replay.good(),"Truncated loader gold record");Equal(actual,expected);}
 }
 void finish(){if(recording){capture.flush();Check(capture.good(),"Loader capture flush failed");capture.close();}else Check(replay.peek()==std::char_traits<char>::eof(),"Trailing loader capture bytes");}
};
void* Construct(Api api,U w,U h,U levels,U format){Isolation(api);auto p=api.originalCreate?api.originalCreate(w,h,1,levels,0,format,1,3):api.create(w,h,levels,format);Isolation(api);return p;}
void Constructors(Api recovered,Api original,bool hasOriginal,Gold& gold){
 const U formats[]={2,3,4,5,6,7,8,9,10,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,41,42,43,45,46,47,48,50,54,55,56,57,62,63};
 const std::array<U,2> sizes[]={{{1,1}},{{2,4}},{{4,2}},{{8,32}},{{16,64}},{{32,16}},{{32,32}},{{64,128}},{{128,64}},{{3,7}},{{17,35}},{{65,33}}};
 for(U base:formats)for(U endian:{0u,0x40u,0x80u,0xc0u})for(auto size:sizes)for(U levels:{0u,1u,2u}){
  if(levels==2&&size[0]==1&&size[1]==1)continue;
  U format=0x1a200000|endian|base;
  current="constructor-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-f"+std::to_string(format)+"-levels"+std::to_string(levels);
  auto actual=Consume(recovered,Construct(recovered,size[0],size[1],levels,format),false);Trace expected;if(hasOriginal)expected=Consume(original,Construct(original,size[0],size[1],levels,format),false);gold.record(std::move(actual),hasOriginal?&expected:nullptr);++constructorCases;
 }
 for(auto size:{std::array<U,2>{0,1},std::array<U,2>{1,0},std::array<U,2>{8193,1},std::array<U,2>{1,8193}})Check(recovered.create(size[0],size[1],0,0x18280086)==nullptr,"Unsupported constructor dimensions must reject");
 Check(!recovered.create(8,8,17,0x18280086)&&!recovered.create(8,8,0,0x18280186)&&!recovered.create(8,8,0,0x1a200001),"Unsupported constructor levels/tiled/sub-byte must reject");Isolation(recovered);
}
std::vector<unsigned char> Tga(U w,U h,U depth,U origin,bool rle,U pattern,U tail){
 std::vector<unsigned char> b(18);b[0]=3;b[2]=rle?10:2;b[12]=static_cast<unsigned char>(w);b[13]=static_cast<unsigned char>(w>>8);b[14]=static_cast<unsigned char>(h);b[15]=static_cast<unsigned char>(h>>8);b[16]=static_cast<unsigned char>(depth);b[17]=static_cast<unsigned char>(origin|(depth==32?8:0));b.insert(b.end(),{'i','d','!'});
 for(U y=0;y<h;++y)for(U x=0;x<w;++x){if(rle)b.push_back(pattern==0?0x80:0);for(U c=0;c<depth/8;++c)b.push_back(static_cast<unsigned char>(pattern>=4?pattern-4:pattern==0?0:pattern==1?255:pattern==2?((x*37+y*61+c*83)&255):((x^y^(c*31))*17)&255));}
 if(tail){const U extension=static_cast<U>(b.size());if(tail>=2){b.resize(b.size()+495);b[extension]=0xef;b[extension+1]=1;b[extension+494]=3;if(tail==3){b[extension+478]=22;b[extension+480]=10;}}
  U footer=static_cast<U>(b.size());b.resize(b.size()+26);Store(b.data(),footer,tail>=2?extension:0);std::memcpy(b.data()+footer+8,"TRUEVISION-XFILE.\0",18);
 }
 return b;
}
std::vector<unsigned char> Dds(U w,U h,U format,U provided,U pattern){
 std::vector<unsigned char> b(128);Store(b.data(),0,0x20534444);Store(b.data(),4,124);Store(b.data(),8,0x2100f);Store(b.data(),12,h);Store(b.data(),16,w);Store(b.data(),28,provided);Store(b.data(),76,32);Store(b.data(),108,provided>1?0x401008:0x1000);
 bool compressed=format<=3;Store(b.data(),80,compressed?4:format==4?0x41:0x40);if(compressed)Store(b.data(),84,format==1?0x31545844:format==2?0x33545844:0x35545844);else{Store(b.data(),88,32);Store(b.data(),92,0xff0000);Store(b.data(),96,0xff00);Store(b.data(),100,0xff);Store(b.data(),104,format==4?0xff000000:0);}
 for(U level=0;level<provided;++level){U count=compressed?((w+3)/4)*((h+3)/4)*(format==1?8:16):w*h*4;for(U i=0;i<count;++i)b.push_back(static_cast<unsigned char>(pattern>=4?pattern-4:pattern==0?0:pattern==1?255:(i*53+level*71+(i>>3)*29)&255));w=std::max(1u,w>>1);h=std::max(1u,h>>1);}
 return b;
}
std::vector<unsigned char> Bmp(U w,U h,U depth,bool top,U pattern,U dib,U colors=0,U gap=0,U profile=0){
 const bool core=dib==12;const U header=14+dib,stride=(w*(depth/8)+3)&~3u,start=header+colors*4,raster=stride*h;
 // D3DX's pre-expansion DWORD alpha scan also visits byte3 of BGR24
 // DWORDs. Explicit zero trailing storage bounds that scan for every shape;
 // it is file data, not dependence on original heap contents past EOF.
 std::vector<unsigned char> b(start+gap+raster+(profile?17:0)+(depth==24?w:0));b[0]='B';b[1]='M';Store(b.data(),2,static_cast<U>(b.size()));Store(b.data(),10,start+gap);Store(b.data(),14,dib);
 if(core){b[18]=static_cast<unsigned char>(w);b[19]=static_cast<unsigned char>(w>>8);b[20]=static_cast<unsigned char>(h);b[21]=static_cast<unsigned char>(h>>8);b[22]=1;b[24]=static_cast<unsigned char>(depth);}
 else{Store(b.data(),18,w);Store(b.data(),22,top?0u-h:h);b[26]=1;b[28]=static_cast<unsigned char>(depth);Store(b.data(),34,raster);Store(b.data(),46,colors);}
 for(U i=0;i<colors*4;++i)b[header+i]=static_cast<unsigned char>(i*71+19);
 // Deliberately make the extra gap/raster distinguishable. D3DX consumes at
 // start rather than bfOffBits, including the first gap bytes as pixels.
 for(U i=0;i<gap+raster;++i)b[start+i]=static_cast<unsigned char>(pattern>=4?pattern-4:pattern==0?0:pattern==1?255:pattern==2?(i*53+(i>>3)*29)&255:((i^31)*17)&255);
 if(profile){const U at=start+gap+raster;Store(b.data(),126,at-14);Store(b.data(),130,17);for(U i=0;i<17;++i)b[at+i]=static_cast<unsigned char>(i*41+7);}
 return b;
}
void FileCase(Api recovered,Api original,bool hasOriginal,Gold& gold,void* rd,void* od,const std::string& path){
 auto input=Read(path);void* p=nullptr;Isolation(recovered);const auto status=recovered.load(rd,path.c_str(),&p);Isolation(recovered);Trace actual;actual.number(static_cast<U>(status));
 const bool compressed=input.size()>=128&&Load(input.data(),0)==0x20534444&&(Load(input.data(),80)&4)&&(Load(input.data(),84)==0x31545844||Load(input.data(),84)==0x33545844||Load(input.data(),84)==0x35545844);
 if(compressed)++compressedFiles;
 if(status>=0){auto trace=Consume(recovered,p,true);actual.add(trace.data.data(),trace.data.size());}else Check(!p,"Rejected load returned a resource");
 Trace expected;if(hasOriginal){p=nullptr;const auto result=original.load(od,path.c_str(),&p);Check(result==0&&p,"Declared supported original file must load successfully");expected.number(static_cast<U>(result));if(result>=0){auto trace=Consume(original,p,true);expected.add(trace.data.data(),trace.data.size());}else Check(!p,"Rejected original returned resource");}
 Check(Read(path)==input,"File loader modified source bytes");gold.record(std::move(actual),hasOriginal?&expected:nullptr);++fileCases;
}
void FileOutcomeCase(Api recovered,Api original,bool hasOriginal,Gold& gold,void* rd,void* od,const std::string& path){
 const auto input=Read(path);void* p=nullptr;Isolation(recovered);
 const auto status=recovered.load(rd,path.c_str(),&p);Isolation(recovered);Trace actual;actual.number(static_cast<U>(status));
 if(status>=0){Check(status==0&&p,"Successful raster load must transfer a resource");auto trace=Consume(recovered,p,true);actual.add(trace.data.data(),trace.data.size());++rasterDecoded;}
 else{Check(!p,"Rejected raster load transferred a resource");++rasterRejected;}
 Trace expected;
 if(hasOriginal){
  p=nullptr;const auto result=original.load(od,path.c_str(),&p);expected.number(static_cast<U>(result));
  if(result>=0){Check(result==0&&p,"Successful original raster load must transfer a resource");auto trace=Consume(original,p,true);expected.add(trace.data.data(),trace.data.size());}
  else Check(!p,"Rejected original raster load transferred a resource");
 }
 Check(Read(path)==input,"Raster loader modified immutable source bytes");Isolation(recovered);
 gold.record(std::move(actual),hasOriginal?&expected:nullptr);++fileCases;
}
std::vector<std::string> RasterNames(const std::string& directory,const char* extension){
 WIN32_FIND_DATAA data{};const auto search=FindFirstFileA((directory+"/*"+extension).c_str(),&data);
 Check(search!=INVALID_HANDLE_VALUE,"Missing declared raster corpus");std::vector<std::string> result;
 do{if(!(data.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY)){Check(std::string(data.cFileName)!="encoded.jpg","Temporary encoder output must not enter immutable corpus");result.push_back(data.cFileName);}}while(FindNextFileA(search,&data));
 const auto error=GetLastError();FindClose(search);Check(error==ERROR_NO_MORE_FILES,"Raster corpus enumeration failed");
 std::sort(result.begin(),result.end());return result;
}
void RunRaster(unsigned char* original,const std::string& libs,const std::string& corpus,const std::string& prefix){
 failurePath=prefix;std::vector<HMODULE> modules;auto recovered=Recovered(libs,modules,!original);Api baseline{};if(original)baseline=Original(original);
 Gold gold;gold.open(prefix+".capture",original!=nullptr);
 void* rd=nullptr;void* od=nullptr;Check(recovered.device(0,1,nullptr,0,nullptr,&rd)==0&&rd,"Raster recovered device");
 if(original)Check(baseline.device(0,1,nullptr,0,nullptr,&od)==0&&od,"Raster original device");
 struct Corpus{const char* name;const char* extension;U count,decoded,rejected;};
 bool outcomeCountsMatch=true;
 for(const auto& source:{Corpus{"ppm",".ppm",8438,6615,1823},Corpus{"jpeg",".jpg",1284,1280,4},Corpus{"jpeg-metadata",".jpg",4000,1440,2560}}){
  const auto directory=corpus+"/"+source.name;const auto names=RasterNames(directory,source.extension);
  Check(names.size()==source.count,"Complete immutable raster source count");const U decoded=rasterDecoded,rejected=rasterRejected;
  for(const auto& name:names){current=std::string(source.name)+"/"+name;FileOutcomeCase(recovered,baseline,original!=nullptr,gold,rd,od,directory+"/"+name);}
  outcomeCountsMatch=outcomeCountsMatch&&rasterDecoded-decoded==source.decoded&&rasterRejected-rejected==source.rejected;
  std::cerr<<"COMPLETE corpus="<<source.name<<" files="<<names.size()<<" decoded="<<rasterDecoded-decoded<<" rejected="<<rasterRejected-rejected<<" mismatches="<<mismatches<<"\n";
 }
 gold.finish();Check(outcomeCountsMatch&&fileCases==13722&&rasterDecoded==9335&&rasterRejected==4387&&constructorCases==0,"Complete JPEG/PPM raster outcome matrix");
 Check(recovered.deviceRelease(rd)==0,"Raster recovered device release");if(original)Check(baseline.deviceRelease(od)==0,"Raster original device release");Isolation(recovered);
 if(mismatches)throw std::runtime_error("Complete raster mismatches="+std::to_string(mismatches));for(auto module:modules)FreeLibrary(module);
 std::cout<<"PASS PC native raster "<<(original?"differential":"NoOriginal")<<" fileCases="<<fileCases<<" decoded="<<rasterDecoded<<" rejected="<<rasterRejected<<" rgbaCopies="<<copyCases<<" completeBytes="<<gold.bytes<<"; complete HRESULTs, headers/VM/all mips/three locks, L8+X8 masked/physical RGBA, source invariance, resource ownership and per-call isolation; immutable known-safe original inputs only; not base regression, production package or SDK acceptance\n";
}
void Files(Api recovered,Api original,bool hasOriginal,Gold& gold,const std::string& fixtures,const std::string& scratch){
 void* rd=nullptr;void* od=nullptr;Check(recovered.device(0,1,nullptr,0,nullptr,&rd)==0&&rd,"Loader recovered device");if(hasOriginal)Check(original.device(0,1,nullptr,0,nullptr,&od)==0&&od,"Loader original device");
 const std::array<U,2> sizes[]={{{1,1}},{{1,8}},{{8,1}},{{2,4}},{{4,2}},{{8,8}},{{16,32}},{{32,16}},{{32,32}},{{64,64}}};
 for(auto size:sizes)for(U depth:{24u,32u})for(U origin:{0u,0x10u,0x20u,0x30u})for(bool rle:{false,true})for(U pattern:{0u,1u,2u,3u})for(U tail:{0u,1u,2u,3u}){
  current="tga-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-depth"+std::to_string(depth)+"-origin"+std::to_string(origin)+"-rle"+std::to_string(rle)+"-pattern"+std::to_string(pattern)+"-tail"+std::to_string(tail);const auto path=scratch+"/input.tga";Save(path,Tga(size[0],size[1],depth,origin,rle,pattern,tail));FileCase(recovered,original,hasOriginal,gold,rd,od,path);
 }
 for(auto size:sizes)for(U format:{1u,2u,3u,4u,5u})for(U pattern:{0u,1u,2u}){
  U levels=1;for(U n=std::max(size[0],size[1]);n>1;n>>=1)++levels;
  for(U provided:{1u,levels}){if(format<=3&&provided!=levels)continue;current="dds-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-format"+std::to_string(format)+"-pattern"+std::to_string(pattern)+"-provided"+std::to_string(provided);const auto path=scratch+"/input.dds";Save(path,Dds(size[0],size[1],format,provided,pattern));FileCase(recovered,original,hasOriginal,gold,rd,od,path);}
 }
 // Public DirectXTex adaptation: preserve every old case above and compare
 // complete new resources, including X8 physical bytes, all provided levels,
 // missing BOX levels, packed tails and the actual masked LoadImageA copy.
 const std::array<U,2> npotSizes[]={{{3,1}},{{1,3}},{{3,2}},{{2,3}},{{3,3}},{{5,3}},{{3,5}},{{7,9}},{{9,7}},{{17,35}},{{35,17}},{{31,65}},{{65,31}},{{127,5}},{{5,127}},{{129,1}},{{1,129}}};
 for(auto size:npotSizes){
  for(U depth:{24u,32u})for(U origin:{0u,0x10u,0x20u,0x30u})for(bool rle:{false,true})for(U pattern:{0u,1u,2u,3u}){
   current="npot-tga-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-depth"+std::to_string(depth)+"-origin"+std::to_string(origin)+"-rle"+std::to_string(rle)+"-pattern"+std::to_string(pattern);
   const auto path=scratch+"/npot.tga";Save(path,Tga(size[0],size[1],depth,origin,rle,pattern,0));FileCase(recovered,original,hasOriginal,gold,rd,od,path);++npotCases;
  }
  U sourceLevels=1;for(U n=std::max(size[0],size[1]);n>1;n>>=1)++sourceLevels;
  for(U format:{4u,5u})for(U pattern:{0u,1u,2u})for(U provided=1;provided<=sourceLevels;++provided){
   current="npot-dds-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-format"+std::to_string(format)+"-pattern"+std::to_string(pattern)+"-provided"+std::to_string(provided);
   const auto path=scratch+"/npot.dds";Save(path,Dds(size[0],size[1],format,provided,pattern));FileCase(recovered,original,hasOriginal,gold,rd,od,path);++npotCases;
  }
 }
 // All byte quantization values, two transposed shapes and every existing
 // source level count. Uniform planes expose reciprocal/float/dither ties.
 for(auto size:{std::array<U,2>{3,5},std::array<U,2>{5,3}})for(U value=0;value<256;++value){
  for(U depth:{24u,32u}){
   current="npot-uniform-tga-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-depth"+std::to_string(depth)+"-byte"+std::to_string(value);
   const auto path=scratch+"/uniform.tga";Save(path,Tga(size[0],size[1],depth,0x20,false,value+4,0));FileCase(recovered,original,hasOriginal,gold,rd,od,path);++npotCases;
  }
  for(U format:{4u,5u})for(U provided=1;provided<=3;++provided){
   current="npot-uniform-dds-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-format"+std::to_string(format)+"-provided"+std::to_string(provided)+"-byte"+std::to_string(value);
   const auto path=scratch+"/uniform.dds";Save(path,Dds(size[0],size[1],format,provided,value+4));FileCase(recovered,original,hasOriginal,gold,rd,od,path);++npotCases;
  }
 }
 current="npot-previous-rejected-tga-3x4";Save(scratch+"/previous-npot.tga",Tga(3,4,32,0,false,2,0));FileCase(recovered,original,hasOriginal,gold,rd,od,scratch+"/previous-npot.tga");++npotCases;
 auto bmpCase=[&](U w,U h,U depth,bool top,U pattern,U dib,U colors,U gap,U profile){
  current="bmp-"+std::to_string(w)+"x"+std::to_string(h)+"-depth"+std::to_string(depth)+"-top"+std::to_string(top)+"-pattern"+std::to_string(pattern)+"-dib"+std::to_string(dib)+"-colors"+std::to_string(colors)+"-gap"+std::to_string(gap)+"-profile"+std::to_string(profile);
  const auto path=scratch+"/input.bmp";Save(path,Bmp(w,h,depth,top,pattern,dib,colors,gap,profile));FileCase(recovered,original,hasOriginal,gold,rd,od,path);++bmpCases;
 };
 for(const auto& shapes:{std::vector<std::array<U,2>>(std::begin(sizes),std::end(sizes)),std::vector<std::array<U,2>>(std::begin(npotSizes),std::end(npotSizes))})for(auto size:shapes)
  for(U dib:{12u,40u,52u,56u,64u,108u,124u})for(U depth:{24u,32u})for(bool top:{false,true})for(U pattern:{0u,1u,2u,3u}){
   if(dib==12&&top)continue;bmpCase(size[0],size[1],depth,top,pattern,dib,0,0,0);
  }
 for(auto size:{std::array<U,2>{4,8},std::array<U,2>{3,5},std::array<U,2>{35,17}}){
  for(U dib:{40u,52u,56u,64u,108u,124u})for(U depth:{24u,32u})for(bool top:{false,true})for(U pattern:{0u,1u,2u,3u})for(U colors:{1u,4u})for(U gap:{0u,7u})bmpCase(size[0],size[1],depth,top,pattern,dib,colors,gap,0);
  for(U depth:{24u,32u})for(U pattern:{0u,1u,2u,3u})for(U gap:{1u,7u})bmpCase(size[0],size[1],depth,false,pattern,12,0,gap,0);
  for(U depth:{24u,32u})for(bool top:{false,true})for(U pattern:{0u,1u,2u,3u})for(U gap:{0u,7u})bmpCase(size[0],size[1],depth,top,pattern,124,4,gap,1);
 }
 for(auto size:{std::array<U,2>{3,5},std::array<U,2>{5,3}})for(U value=0;value<256;++value)for(U depth:{24u,32u})for(bool top:{false,true})bmpCase(size[0],size[1],depth,top,value+4,40,0,0,0);
 for(const auto* name:{"AULegionnaireInfantry.dds","EXJapanArrow.dds","FXAlliedFutureLaser.dds","FXYurikoShieldFadeLUT.dds","JUArcherInfantry.dds","PackedImages_097_EP1.dds","SUMortarCycle.dds","apt_tacticalhudbuildqueuepage_1.tga","apt_tacticalhudmaincommandbar_1.tga","map_mp_2_feasel10_ep1_art.tga"}){current=std::string("real-")+name;FileCase(recovered,original,hasOriginal,gold,rd,od,fixtures+"/"+name);}
 // Both formerly unsupported real files now execute the full successful
 // original/actual/no-original resource and every mip-copy comparison.
 for(const auto* name:{"LURomania_vol.dds","Target_Japan_ArcherBarrage.dds"}){current=std::string("real-")+name;FileCase(recovered,original,hasOriginal,gold,rd,od,fixtures+"/"+name);}
 // Exact accepted public-BC corpus through the FORMAL file and copy exports.
 const std::array<U,2> bcSizes[]={{{1,1}},{{1,8}},{{8,1}},{{2,4}},{{4,2}},{{2,2}},{{3,1}},{{1,3}},{{3,2}},{{2,3}},{{3,3}},{{5,3}},{{3,5}},{{7,9}},{{9,7}},{{17,35}},{{35,17}},{{31,65}},{{65,31}},{{127,5}},{{5,127}},{{129,1}},{{1,129}},{{4,4}},{{4,8}},{{8,4}},{{8,8}},{{16,32}},{{32,16}},{{64,64}}};
 for(auto size:bcSizes)for(U format:{1u,2u,3u})for(U pattern:{0u,1u,2u,31u}){
  U levels=1;for(U n=std::max(size[0],size[1]);n>1;n>>=1)++levels;
  for(U provided=1;provided<=levels;++provided){
   current="formal-bc-dds-"+std::to_string(size[0])+"x"+std::to_string(size[1])+"-f"+std::to_string(format)+"-p"+std::to_string(pattern)+"-levels"+std::to_string(provided);
   const auto path=scratch+"/bc.dds";Save(path,Dds(size[0],size[1],format,provided,pattern));FileCase(recovered,original,hasOriginal,gold,rd,od,path);++bcFileCases;
  }
 }
 current="missing-file";void* p=nullptr;const auto missing=scratch+"/absent.tga";const auto missingStatus=recovered.load(rd,missing.c_str(),&p);Check(missingStatus==static_cast<std::int32_t>(0x88760b59u)&&!p,"Missing-file HRESULT/output");if(hasOriginal){p=nullptr;Check(original.load(od,missing.c_str(),&p)==missingStatus&&!p,"Original missing-file HRESULT/output");}
 // Deliberately unsupported codecs, tiled/native dimensions and
 // malformed metadata are only sent to recovered, never unsafe original paths.
 U rejectionIndex=0;
 for(auto bytes:{std::vector<unsigned char>{'P','6','\n'},Tga(8193,1,32,0,false,2,0),Tga(4,4,32,0,false,2,0)}){
  current="rejected-input-"+std::to_string(rejectionIndex++);if(rejectionIndex==3)bytes.resize(20);const auto path=scratch+"/invalid.bin";Save(path,bytes);p=nullptr;
  Check(recovered.load(rd,path.c_str(),&p)<0&&!p,"Unsupported/truncated source must reject without resource");Isolation(recovered);++rejections;
 }
 for(U mode=0;mode<8;++mode){
  current="rejected-bmp-"+std::to_string(mode);auto bytes=Bmp(3,5,32,false,2,mode==6?124:40);
  if(mode==0)bytes.resize(22);else if(mode==1)Store(bytes.data(),30,3);else if(mode==2)bytes[28]=8;else if(mode==3)Store(bytes.data(),10,0);else if(mode==4)Store(bytes.data(),46,0xffffffffu);else if(mode==5)bytes.resize(60);else if(mode==6)Store(bytes.data(),126,124);else{bytes=Bmp(4,4,24,false,0,40);bytes.resize(54+48);Store(bytes.data(),2,static_cast<U>(bytes.size()));}
  const auto path=scratch+"/invalid.bmp";Save(path,bytes);p=nullptr;Check(recovered.load(rd,path.c_str(),&p)<0&&!p,"Unsupported/malformed BMP must not transfer resource");Isolation(recovered);++rejections;
 }
 for(U variant=0;variant<8;++variant){
  current="rejected-bc-"+std::to_string(variant);auto bytes=Dds(4,4,3,3,2);
  if(variant==0)bytes.resize(127);else if(variant==1)bytes.pop_back();else if(variant==2)Store(bytes.data(),84,0x30315844);else if(variant==3)Store(bytes.data(),112,0x200);else if(variant==4)Store(bytes.data(),24,2);else if(variant==5)Store(bytes.data(),16,0);else if(variant==6)Store(bytes.data(),12,0);else Store(bytes.data(),28,16);
  const auto path=scratch+"/invalid.dds";Save(path,bytes);p=reinterpret_cast<void*>(0x13579);Isolation(recovered);Check(recovered.load(rd,path.c_str(),&p)<0&&p==reinterpret_cast<void*>(0x13579),"Rejected BC must preserve borrowed output ownership");Isolation(recovered);Check(Read(path)==bytes,"Rejected BC changed source");++rejections;
 }
 current="invalid-loader-pointers";Check(recovered.load(rd,nullptr,&p)<0&&recovered.load(rd,missing.c_str(),nullptr)<0&&recovered.load(nullptr,missing.c_str(),&p)<0,"Invalid loader arguments must reject");Isolation(recovered);
 Check(recovered.deviceRelease(rd)==0,"Loader device release");if(hasOriginal)Check(original.deviceRelease(od)==0,"Original loader device release");Isolation(recovered);
}
void Run(unsigned char* original,const std::string& libs,const std::string& fixtures,const std::string& prefix){
 failurePath=prefix;std::vector<HMODULE> modules;auto recovered=Recovered(libs,modules,!original);Api baseline{};if(original)baseline=Original(original);Gold gold;gold.open(prefix+".capture",original!=nullptr);
 Constructors(recovered,baseline,original!=nullptr,gold);Files(recovered,baseline,original!=nullptr,gold,fixtures,prefix+"-inputs");gold.finish();std::cerr<<"COMPLETE constructorCases="<<constructorCases<<" fileCases="<<fileCases<<" mismatches="<<mismatches<<"\n";Check(constructorCases==6860&&fileCases==16180&&npotCases==5665&&bmpCases==6152&&rejections==19&&compressedFiles==1679&&bcFileCases==1572,"Complete declared constructor/file/rejection matrix count");if(mismatches)throw std::runtime_error("Complete loader mismatches="+std::to_string(mismatches)+" constructorCases="+std::to_string(constructorCases)+" fileCases="+std::to_string(fileCases));for(auto m:modules)FreeLibrary(m);
 std::cout<<"PASS PC native loader "<<(original?"differential":"NoOriginal")<<" constructorCases="<<constructorCases<<" fileCases="<<fileCases<<" npotCases="<<npotCases<<" bmpCases="<<bmpCases<<" rgbaCopies="<<copyCases<<" supportedFiles="<<fileCases<<" supportedCompressedFiles="<<compressedFiles<<" formalBcCases="<<bcFileCases<<" explicitRejections="<<rejections<<" completeBytes="<<gold.bytes<<"; complete headers/VM/padding/packed mips/three locks, exact LoadImageA mask+flags and physical endian views, add/release/final storage ownership; not production SDK or mixed-mode identity\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 try{
  if(IntPtr::Size!=4||(args->Length!=4&&args->Length!=5))throw gcnew Exception("Usage: x86 original.dll|isolated native-dir fixtures capture-prefix [raster-corpus]");unsigned char* original=nullptr;
  if(args[0]!="isolated"){auto sha=SHA256::Create();auto digest=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;if(digest!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original loader DLL fingerprint");auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto h=Marshal::GetHINSTANCE(assembly->ManifestModule);if(h==IntPtr::Zero||h==IntPtr(-1))throw gcnew Exception("Original loader module handle");original=static_cast<unsigned char*>(h.ToPointer());}
  Directory::CreateDirectory(Path::GetFullPath(args[3])+"-inputs");auto libs=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[1]));auto fixtures=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));auto prefix=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[3]));
  if(args->Length==5){auto corpus=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[4]));RunRaster(original,static_cast<const char*>(libs.ToPointer()),static_cast<const char*>(corpus.ToPointer()),static_cast<const char*>(prefix.ToPointer()));Marshal::FreeHGlobal(corpus);}
  else Run(original,static_cast<const char*>(libs.ToPointer()),static_cast<const char*>(fixtures.ToPointer()),static_cast<const char*>(prefix.ToPointer()));
  Marshal::FreeHGlobal(libs);Marshal::FreeHGlobal(fixtures);Marshal::FreeHGlobal(prefix);return 0;
 }catch(const std::exception& e){std::cerr<<current<<": "<<e.what()<<" constructors="<<constructorCases<<" files="<<fileCases<<" mismatches="<<mismatches<<"\n";}catch(Exception^ e){Console::Error->WriteLine(e);std::cerr<<"FAILED current="<<current<<" constructors="<<constructorCases<<" files="<<fileCases<<" mismatches="<<mismatches<<"\n";}return 1;
}
