#define RECOVERED_TEXTURE_NATIVE_COPY_API_BUILD
#include "TextureNativeCopyApi.h"
#include <cmath>
#include <cstddef>
#include <cstring>
using namespace Recovered::TextureCompiler::NativeCopyAbi;
namespace {
using U=std::uint32_t;
constexpr std::int32_t Unsupported=static_cast<std::int32_t>(0x80004001u);
struct Format {U id,bytes,wordBytes,offset[4],mask[4],shift[4];bool floating;};
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
 {62,4,4,{0,0,0,0},{0xffu,0xff00u,0xff0000u,0xff000000u},{0,8,16,24}},
 {27,2,2,{0,0,0,0},{0xffffu,0x0u,0x0u,0x0u},{0,0,0,0},true},
 {28,4,4,{0,0,0,0},{0xffffu,0xffff0000u,0x0u,0x0u},{0,16,0,0},true},
 {29,8,2,{0,1,2,3},{0xffffu,0xffffu,0xffffu,0xffffu},{0,0,0,0},true},
 {30,2,2,{0,0,0,0},{0xffffu,0x0u,0x0u,0x0u},{0,0,0,0},true},
 {31,4,4,{0,0,0,0},{0xffffu,0xffff0000u,0x0u,0x0u},{0,16,0,0},true},
 {32,8,2,{0,1,2,3},{0xffffu,0xffffu,0xffffu,0xffffu},{0,0,0,0},true},
 {36,4,4,{0,0,0,0},{0xffffffffu,0x0u,0x0u,0x0u},{0,0,0,0},true},
 {37,8,4,{0,1,0,0},{0xffffffffu,0xffffffffu,0x0u,0x0u},{0,0,0,0},true},
 {38,16,4,{0,1,2,3},{0xffffffffu,0xffffffffu,0xffffffffu,0xffffffffu},{0,0,0,0},true},
 {57,12,4,{0,1,2,0},{0xffffffffu,0xffffffffu,0xffffffffu,0x0u},{0,0,0,0},true}
};
const Format* Find(U full){
 // Normalized/integer/float, unsigned/signed/biased channels; no gamma.
 if(full&~0x3ffffeffu)return nullptr;
 const Format* result=nullptr;
 for(const auto& f:Formats)if(f.id==(full&63))result=&f;
 if(!result)return nullptr;
 for(U c=0;c<4;++c){
  const U type=(full>>(9+2*c))&3;
  if(((full>>(18+3*c))&7)>5||type==3)return nullptr;
  // Signed/biased one-bit normalization has a zero denominator in the original.
  if(type&&(result->mask[c]>>result->shift[c])==1)return nullptr;
 }
 return result;
}
float MultiplyAdd(float value,float scale,float bias){
 // Match the original x87 multiply/add followed by an explicit float32 store.
 volatile float result=static_cast<float>(static_cast<double>(value)*scale+bias);
 return result;
}
float Normalize(U value,U maximum,U type,bool integer){
 if(!maximum)return 0;
 const U midpoint=(maximum>>1)+1;
 if(type==1)value^=midpoint;
 const float offset=type?-static_cast<float>(midpoint):0;
 volatile float scale=integer?1:static_cast<float>(1.0/static_cast<double>(type?midpoint-1:maximum));
 const float bias=MultiplyAdd(offset,scale,0);
 const float result=MultiplyAdd(static_cast<float>(value),scale,bias);
 // Source normalized signed/biased values clamp the asymmetric minimum to -1.
 return !integer&&type&&result<-1?-1:result;
}
U Quantize(float value,U maximum,U type,bool integer,float threshold){
 const U midpoint=(maximum>>1)+1;
 const float minimum=type?(integer?-static_cast<float>(midpoint-1):-1):0;
 const float upper=integer?static_cast<float>(type?midpoint-1:maximum):1;
 value=value<minimum?minimum:value>upper?upper:value;
 const float scale=integer?1:static_cast<float>(type?midpoint-1:maximum);
 const float bias=type?static_cast<float>(midpoint):0;
 // XGCopySurface adds the one-bit alpha reference to the destination bias first.
 volatile float combinedBias=maximum==1?static_cast<float>(static_cast<double>(bias)+static_cast<float>(0.5-static_cast<double>(threshold))):bias;
 volatile float scaled=MultiplyAdd(value,scale,combinedBias);
 // Original uses truncation with a sign-selected half, not ties-to-even.
 volatile float rounded=static_cast<float>(static_cast<double>(scaled)+(scaled<0?-0.5:0.5));
 return static_cast<U>(static_cast<std::int32_t>(rounded))^(type==1?midpoint:0);
}

U HalfBitsToFloat(U half){
 U mantissa=half&1023;std::int32_t exponent;
 if(half&0x7c00)exponent=(half>>10)&31;
 else if(mantissa){exponent=1;do{mantissa<<=1;--exponent;}while(!(mantissa&1024));mantissa&=1023;}
 else exponent=-112;
 // The original s10e5 layout uses exponent31 for finite values, not IEEE Inf/NaN.
 return ((half&0x8000)<<16)|(mantissa<<13)|(static_cast<U>(exponent+112)<<23);
}
float CanonicalFloat(U bits){
 if((bits&0x7fffffff)>0x7f800000u)bits=0xff800000u; // Original min(-Inf, NaN) selects -Inf.
 if(!(bits&0x7fffffff))bits=0; // x87 value*1 + positive zero.
 float value;std::memcpy(&value,&bits,4);return value;
}
U FloatToHalfBits(float value){
 U bits;std::memcpy(&bits,&value,4);U magnitude=bits&0x7fffffff,sign=(bits>>16)&0x8000;
 if(magnitude>0x47ffefff)return sign|0x7fff;
 if(magnitude<0x38800000){
  const U exponent=magnitude>>23;
  // Match the original x86 masked shift, including tiny/subnormal float inputs.
  magnitude=((magnitude&0x7fffff)|0x800000)>>((113-exponent)&31);
 }else magnitude-=0x38000000;
 return sign|((magnitude+0xfff+((magnitude>>13)&1))>>13);
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
 const U sourceEndian=(sourceFormat>>6)&3,destinationEndian=(destinationFormat>>6)&3;
 const U sourceUnit=sourceEndian==0?1:sourceEndian==1?2:4,destinationUnit=destinationEndian==0?1:destinationEndian==1?2:4;
 // The original row swap truncates incomplete units, leaving an uninitialized tail.
 // Accept only complete endian units; do not freeze that undefined tail as gold.
 if((columns*srcFormat->bytes)%sourceUnit||(columns*dstFormat->bytes)%destinationUnit)return Unsupported;
 constexpr U xorMask[]={0,1,3,2};
 for(U row=0;row<rows;++row){
  auto dst=static_cast<unsigned char*>(destination)+static_cast<std::ptrdiff_t>(y+row)*destinationPitch+x*dstFormat->bytes;
  auto src=static_cast<const unsigned char*>(source)+static_cast<std::ptrdiff_t>(top+row)*sourcePitch+left*srcFormat->bytes;
  for(U column=0;column<columns;++column){
   unsigned char decoded[16],encoded[16]={};
   auto sourceRow=src-static_cast<std::ptrdiff_t>(column)*srcFormat->bytes;
   for(U byte=0;byte<srcFormat->bytes;++byte)decoded[byte]=sourceRow[(column*srcFormat->bytes+byte)^xorMask[sourceEndian]];
   float physical[6]={0,0,0,0,0,1},logical[4],stored[4]={};
   for(U c=0;c<4;++c){U word=0;std::memcpy(&word,decoded+srcFormat->offset[c]*srcFormat->wordBytes,srcFormat->wordBytes);U raw=(word&srcFormat->mask[c])>>srcFormat->shift[c];physical[c]=srcFormat->floating?CanonicalFloat((srcFormat->mask[c]>>srcFormat->shift[c])==0xffff?HalfBitsToFloat(raw):raw):Normalize(raw,srcFormat->mask[c]>>srcFormat->shift[c],(sourceFormat>>(9+2*c))&3,(sourceFormat&0x20000)!=0);}
   for(U c=0;c<4;++c)logical[c]=physical[(sourceFormat>>(18+3*c))&7];
   // Inversion writes R, G, B, A in order: duplicate destinations are last-wins.
   for(U c=0;c<4;++c){U selector=(destinationFormat>>(18+3*c))&7;if(selector<4)stored[selector]=logical[c];}
   for(U c=0;c<4;++c){U maximum=dstFormat->mask[c]>>dstFormat->shift[c];if(!maximum)continue;U word=0;auto address=encoded+dstFormat->offset[c]*dstFormat->wordBytes;std::memcpy(&word,address,dstFormat->wordBytes);U raw;if(dstFormat->floating){float value=stored[c]==0?0:stored[c];if(maximum==0xffff)raw=FloatToHalfBits(value);else std::memcpy(&raw,&value,4);}else raw=Quantize(stored[c],maximum,(destinationFormat>>(9+2*c))&3,(destinationFormat&0x20000)!=0,threshold);word|=(raw<<dstFormat->shift[c])&dstFormat->mask[c];std::memcpy(address,&word,dstFormat->wordBytes);}
   auto destinationRow=dst-static_cast<std::ptrdiff_t>(column)*dstFormat->bytes;
   for(U byte=0;byte<dstFormat->bytes;++byte)destinationRow[(column*dstFormat->bytes+byte)^xorMask[destinationEndian]]=encoded[byte];
   if(column+1<columns){dst+=dstFormat->bytes;src+=srcFormat->bytes;}
  }
 }
 return 0;
}
