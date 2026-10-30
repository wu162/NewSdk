#define RECOVERED_TEXTURE_NATIVE_COPY_API_BUILD
#include "TextureNativeCopyApi.h"
#include "TextureNativeCopyInternal.h"
#include <cstddef>
#include <cstring>
using namespace Recovered::TextureCompiler::NativeCopyAbi;
namespace {
using U=std::uint32_t;
constexpr U Bits[]={1,1,8,16,16,16,32,32,8,8,16,16,16,32,32,16,32,32,4,8,8,64,32,32,16,32,64,16,32,64,16,32,64,32,64,128,32,64,128,8,16,16,32,8,8,16,16,16,32,8,32,4,8,8,32,32,32,96,4,4,4,4,32,32};
void Blocks(U f,U& x,U& y){if(f==11||f==12||f==40){x=2;y=1;}else if(f==39||f==44){x=4;y=1;}else if((f>=18&&f<=20)||f==49||(f>=51&&f<=53)||(f>=58&&f<=61)){x=y=4;}else{x=y=1;}}
constexpr std::int32_t Unsupported=static_cast<std::int32_t>(0x80004001u);
}
extern "C" RECOVERED_TEXTURE_NATIVE_COPY_API std::int32_t __stdcall TextureCopySurface(void* destination,std::int32_t destinationPitch,U width,U height,U destinationFormat,const Point* point,const void* source,std::int32_t sourcePitch,U sourceFormat,const Rect* rectangle,U flags,float threshold){
 // Limit the new conversion route to the exact LoadImageA contract accepted
 // by native-bc-files: both descriptor-endian and masked source views, full
 // locked rows, RGBA8 target, flags0x12 and threshold0. Other conversions keep
 // the existing raw/identity boundaries until separately verified.
 const U format=sourceFormat&63;
 if(format>=18&&format<=20&&destinationFormat==0x1a200006u&&flags==0x12&&threshold==0&&!point&&!rectangle)
  return TexturePublicBcCopySurface(destination,destinationPitch,width,height,destinationFormat,point,source,sourcePitch,sourceFormat,rectangle,flags,threshold);
 return TextureCopySurfaceUncompressed(destination,destinationPitch,width,height,destinationFormat,point,source,sourcePitch,sourceFormat,rectangle,flags,threshold);
}
extern "C" RECOVERED_TEXTURE_NATIVE_COPY_API std::int32_t __stdcall TextureCopySurfaceIdentity(void* destination,std::int32_t destinationPitch,U width,U height,U destinationFormat,const Point* point,const void* source,std::int32_t sourcePitch,U sourceFormat,const Rect* rectangle,U flags,float){
 const U format=destinationFormat&63;
 if(format<2||(sourceFormat&63)<2)return Unsupported;
 if(destinationFormat!=sourceFormat||((flags^(flags>>1))&5))return Unsupported;
 const Point origin=point?*point:Point{0,0};const Rect rect=rectangle?*rectangle:Rect{0,0,static_cast<std::int32_t>(width),static_cast<std::int32_t>(height)};
 if(origin.x<0||origin.y<0||rect.left<0||rect.top<0||rect.right<rect.left||rect.bottom<rect.top)return Unsupported;
 const U left=static_cast<U>(rect.left),top=static_cast<U>(rect.top),right=static_cast<U>(rect.right),bottom=static_cast<U>(rect.bottom),x=static_cast<U>(origin.x),y=static_cast<U>(origin.y);
 const U columns=right-left,rows=bottom-top;
 if(right>width||bottom>height||x>width||y>height||columns>width-x||rows>height-y)return Unsupported;
 U bx,by;Blocks(format,bx,by);
 if((x&(bx-1))||(y&(by-1))||(left&(bx-1))||(top&(by-1))||((right&(bx-1))&&right!=width)||((bottom&(by-1))&&bottom!=height))return Unsupported;
 const U bytesPerBlock=Bits[format]*bx*by>>3;
 const U copyBytes=((columns+bx-1)/bx)*bytesPerBlock,blockRows=(rows+by-1)/by;
 auto dst=static_cast<unsigned char*>(destination)+static_cast<std::ptrdiff_t>(y/by)*destinationPitch+(x/bx)*bytesPerBlock;
 auto src=static_cast<const unsigned char*>(source)+static_cast<std::ptrdiff_t>(top/by)*sourcePitch+(left/bx)*bytesPerBlock;
 for(U row=0;row<blockRows;++row){
  std::memcpy(dst,src,copyBytes);
  // Do not form a pointer before the allocation after the final negative row.
  if(row+1<blockRows){dst+=destinationPitch;src+=sourcePitch;}
 }
 return 0;
}
