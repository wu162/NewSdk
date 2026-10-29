#define RECOVERED_TEXTURE_NATIVE_COPY_API_BUILD
#include "TextureNativeCopyApi.h"
#include <cmath>
#include <cstddef>
#include <cstring>
using namespace Recovered::TextureCompiler::NativeCopyAbi;
namespace {
using U=std::uint32_t;
constexpr std::int32_t Unsupported=static_cast<std::int32_t>(0x80004001u);
struct Format {U id,bytes,wordBytes,offset[4],mask[4],shift[4];};
// Original 60-byte format descriptors at RVA 0xa0390; layout metadata, not pixel gold.
constexpr Format Formats[]={
 {2,1,1,{0,0,0,0},{0xffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {3,2,2,{0,0,0,0},{0x1fu,0x3e0u,0x7c00u,0x8000u},{0,5,10,15}},
 {4,2,2,{0,0,0,0},{0x1fu,0x7e0u,0xf800u,0x0u},{0,5,11,0}},
 {5,2,2,{0,0,0,0},{0x1fu,0x3e0u,0xfc00u,0x0u},{0,5,10,0}},
 {6,4,4,{0,0,0,0},{0xffu,0xff00u,0xff0000u,0xff000000u},{0,8,16,24}},
 {7,4,4,{0,0,0,0},{0x3ffu,0xffc00u,0x3ff00000u,0xc0000000u},{0,10,20,30}},
 {8,1,1,{0,0,0,0},{0xffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {9,1,1,{0,0,0,0},{0xffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {10,2,2,{0,0,0,0},{0xffu,0xff00u,0x0u,0x0u},{0,8,0,0}},
 {14,4,4,{0,0,0,0},{0xffu,0xff00u,0xff0000u,0xff000000u},{0,8,16,24}},
 {15,2,2,{0,0,0,0},{0xfu,0xf0u,0xf00u,0xf000u},{0,4,8,12}},
 {16,4,4,{0,0,0,0},{0x7ffu,0x3ff800u,0xffc00000u,0x0u},{0,11,22,0}},
 {17,4,4,{0,0,0,0},{0x3ffu,0x1ffc00u,0xffe00000u,0x0u},{0,10,21,0}},
 {24,2,2,{0,0,0,0},{0xffffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {25,4,4,{0,0,0,0},{0xffffu,0xffff0000u,0x0u,0x0u},{0,16,0,0}},
 {26,8,2,{0,1,2,3},{0xffffu,0xffffu,0xffffu,0xffffu},{0,0,0,0}},
 {41,2,2,{0,0,0,0},{0xffffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {42,4,4,{0,0,0,0},{0xffffu,0xffff0000u,0x0u,0x0u},{0,16,0,0}},
 {43,1,1,{0,0,0,0},{0xffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {45,2,2,{0,0,0,0},{0xffffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {46,2,2,{0,0,0,0},{0xffffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {47,2,2,{0,0,0,0},{0xffffu,0x0u,0x0u,0x0u},{0,0,0,0}},
 {48,4,4,{0,0,0,0},{0xffffu,0xffff0000u,0x0u,0x0u},{0,16,0,0}},
 {50,4,4,{0,0,0,0},{0xffu,0xff00u,0xff0000u,0xff000000u},{0,8,16,24}},
 {54,4,4,{0,0,0,0},{0x3ffu,0xffc00u,0x3ff00000u,0xc0000000u},{0,10,20,30}},
 {55,4,4,{0,0,0,0},{0x7ffu,0x3ff800u,0xffc00000u,0x0u},{0,11,22,0}},
 {56,4,4,{0,0,0,0},{0x3ffu,0x1ffc00u,0xffe00000u,0x0u},{0,10,21,0}},
 {62,4,4,{0,0,0,0},{0xffu,0xff00u,0xff0000u,0xff000000u},{0,8,16,24}}
};
const Format* Find(U full){
 // Unsigned normalized, little endian; selectors 0..3, constant zero and one.
 if(full&~0x3ffc003fu)return nullptr;
 for(U c=0;c<4;++c)if(((full>>(18+3*c))&7)>5)return nullptr;
 for(const auto& f:Formats)if(f.id==(full&63))return &f;
 return nullptr;
}
float MultiplyAdd(float value,float scale,float bias){
 // Match the original x87 multiply/add followed by an explicit float32 store.
 volatile float result=static_cast<float>(static_cast<double>(value)*scale+bias);
 return result;
}
float Normalize(U value,U maximum){
 if(!maximum)return 0;
 volatile float reciprocal=static_cast<float>(1.0/static_cast<double>(maximum));
 return MultiplyAdd(static_cast<float>(value),reciprocal,0);
}
U Quantize(float value,U maximum,float threshold){
 float bias=maximum==1?static_cast<float>(0.5-static_cast<double>(threshold)):0;
 volatile float scaled=MultiplyAdd(value,static_cast<float>(maximum),bias);
 volatile float rounded=static_cast<float>(static_cast<double>(scaled)+0.5);
 return static_cast<U>(static_cast<std::int32_t>(rounded));
}
}
extern "C" RECOVERED_TEXTURE_NATIVE_COPY_API std::int32_t __stdcall TextureCopySurface(void* destination,std::int32_t destinationPitch,U width,U height,U destinationFormat,const Point* point,const void* source,std::int32_t sourcePitch,U sourceFormat,const Rect* rectangle,U flags,float threshold){
 if(destinationFormat==sourceFormat&& !((flags^(flags>>1))&5))
  return TextureCopySurfaceIdentity(destination,destinationPitch,width,height,destinationFormat,point,source,sourcePitch,sourceFormat,rectangle,flags,threshold);
 const auto dstFormat=Find(destinationFormat),srcFormat=Find(sourceFormat);
 if(!dstFormat||!srcFormat||flags||!std::isfinite(threshold)||threshold<0||threshold>1)return Unsupported;
 // Original conversion allocates unsigned temporary pitches before visiting pixels.
 // A negative signed row pitch requests at least 2 GiB in this x86 path.
 if(destinationPitch<0||sourcePitch<0)return static_cast<std::int32_t>(0x8007000eu);
 const Point origin=point?*point:Point{0,0};
 const Rect rect=rectangle?*rectangle:Rect{0,0,static_cast<std::int32_t>(width),static_cast<std::int32_t>(height)};
 if(origin.x<0||origin.y<0||rect.left<0||rect.top<0||rect.right<rect.left||rect.bottom<rect.top)return Unsupported;
 const U x=origin.x,y=origin.y,left=rect.left,top=rect.top,right=rect.right,bottom=rect.bottom,columns=right-left,rows=bottom-top;
 if(right>width||bottom>height||x>width||y>height||columns>width-x||rows>height-y)return Unsupported;
 for(U row=0;row<rows;++row){
  auto dst=static_cast<unsigned char*>(destination)+static_cast<std::ptrdiff_t>(y+row)*destinationPitch+x*dstFormat->bytes;
  auto src=static_cast<const unsigned char*>(source)+static_cast<std::ptrdiff_t>(top+row)*sourcePitch+left*srcFormat->bytes;
  for(U column=0;column<columns;++column){
   float physical[6]={0,0,0,0,0,1},logical[4],stored[4]={};
   for(U c=0;c<4;++c){U word=0;std::memcpy(&word,src+srcFormat->offset[c]*srcFormat->wordBytes,srcFormat->wordBytes);physical[c]=Normalize((word&srcFormat->mask[c])>>srcFormat->shift[c],srcFormat->mask[c]>>srcFormat->shift[c]);}
   for(U c=0;c<4;++c)logical[c]=physical[(sourceFormat>>(18+3*c))&7];
   // Inversion writes R, G, B, A in order: duplicate destinations are last-wins.
   for(U c=0;c<4;++c){U selector=(destinationFormat>>(18+3*c))&7;if(selector<4)stored[selector]=logical[c];}
   std::memset(dst,0,dstFormat->bytes);
   for(U c=0;c<4;++c){U maximum=dstFormat->mask[c]>>dstFormat->shift[c];if(!maximum)continue;U word=0;auto address=dst+dstFormat->offset[c]*dstFormat->wordBytes;std::memcpy(&word,address,dstFormat->wordBytes);word|=(Quantize(stored[c],maximum,threshold)<<dstFormat->shift[c])&dstFormat->mask[c];std::memcpy(address,&word,dstFormat->wordBytes);}
   if(column+1<columns){dst+=dstFormat->bytes;src+=srcFormat->bytes;}
  }
 }
 return 0;
}
