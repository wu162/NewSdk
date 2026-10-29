#define RECOVERED_TEXTURE_NATIVE_SURFACE_API_BUILD
#include "TextureNativeSurfaceApi.h"
#include <Windows.h>
#include <algorithm>
#include <cstring>
using namespace Recovered::TextureCompiler::NativeSurfaceAbi;
static_assert(sizeof(void*)==4&&sizeof(LockedRect)==8,"Native surface ABI requires x86");
namespace {
using U=std::uint32_t;
U Load(const void* p,U offset){U n;std::memcpy(&n,static_cast<const unsigned char*>(p)+offset,4);return n;}
void Store(void* p,U offset,U n){std::memcpy(static_cast<unsigned char*>(p)+offset,&n,4);}
// Fixed format metadata at original RVA98230, first byte of each 2-byte row.
// These are bits per texel, not encoded image outputs or sample dispatches.
constexpr U Bits[]={1,1,8,16,16,16,32,32,8,8,16,16,16,32,32,16,32,32,4,8,8,64,32,32,16,32,64,16,32,64,16,32,64,32,64,128,32,64,128,8,16,16,32,8,8,16,16,16,32,8,32,4,8,8,32,32,32,96,4,4,4,4,32,32};
U Shl(U n,U shift){return n<<(shift&31);}
U Shr(U n,U shift){return n>>(shift&31);}
U LeadingZeros(U n){U result=0;while(result<32&&!(n&0x80000000u)){n<<=1;++result;}return result;}
U CeilLog(U n){return 32-LeadingZeros(n-1);}
U Round(U n,U alignment){return (n+alignment-1)&~(alignment-1);}
U Page(U n){return Round(n,4096);}
void Blocks(U f,U& x,U& y){
 if(f==11||f==12||f==40){x=2;y=1;}
 else if(f==39||f==44){x=4;y=1;}
 else if((f>=18&&f<=20)||f==49||(f>=51&&f<=53)||(f>=58&&f<=61)){x=4;y=4;}
 else{x=1;y=1;}
}
struct Dimensions{U w,h,d;};
Dimensions Decode(const void* p){
 const U add=1+((Load(p,0x28)>>30)&2),packed=Load(p,0x24),dim=(Load(p,0x30)>>9)&3;
 if(!dim)return {(packed&0xffffff)+add,1,1};
 if(dim==2)return {(packed&0x7ff)+add,((packed>>11)&0x7ff)+add,(packed>>22)+add};
 return {(packed&0x1fff)+add,((packed>>13)&0x1fff)+add,1};
}
void Align(Dimensions& d,U bits,U format,U dimension,bool tiled){
 U bx,by;Blocks(format,bx,by);U widthMultiple=32;
 const U bytes=(bx*by*bits)>>3;
 if(!tiled&&bytes)widthMultiple=std::max(32u,256/bytes);
 d.w=Round(d.w,bx*widthMultiple);d.h=Round(d.h,by*(dimension?32u:1u));d.d=Round(d.d,dimension==2?4u:1u);
}
U PackedOffset(U level,Dimensions d,U row,U slice,U format){
 U bx,by;Blocks(format,bx,by);const U wx=CeilLog(d.w),hy=CeilLog(d.h),dz=CeilLog(d.d);
 U offsets[3]={0,0,0};const U axis=hy<wx?1:0;
 if(level<3)offsets[axis]=16>>level;
 else{
  const U other=axis^1;const U sizes[2]={Shl(1,wx),Shl(1,hy)};
  const U position=Shr(sizes[other],level-2);offsets[other]=position;
  if(position<4)offsets[2]=4*std::max(dz-level,1u);
 }
 return ((bx*Bits[format]*offsets[0])>>3)+offsets[1]*row+offsets[2]*slice;
}
struct Layout{U row,slice,total,packed,w,h,d,address;};
Layout Compute(const void* p,U face,U level){
 const U h0=Load(p,0x1c),h1=Load(p,0x20),h3=Load(p,0x28),h4=Load(p,0x2c),h5=Load(p,0x30);
 const U dimension=(h5>>9)&3,format=h1&63,bits=Bits[format],border=h3>>31,pad=border*2;
 const bool tiled=(h0>>31)!=0,packed=(h5&0x800)!=0;
 const Dimensions logical=Decode(p);
 const Dimensions power={Shl(1,CeilLog(logical.w-pad)+border),Shl(1,CeilLog(logical.h-pad)+border),dimension==2?Shl(1,CeilLog(logical.d-pad)+border):1u};
 const bool basePacked=packed&&(power.w<=16||power.h<=16);
 U address=(level&&!basePacked?h5:h1)&~0xfffu;
 Dimensions storage{};Dimensions result=logical;U row=0,packedOffset=0;
 if(!level&&!basePacked){
  storage=logical;Align(storage,bits,format,dimension,tiled);
  if(!tiled&&!(h1&0x400)&&!packed&&dimension==1&&!(h4&0x3c0)&&!border){U bx,by;Blocks(format,bx,by);storage.h=Round(logical.h,by);}
  row=dimension?((((h0>>22)&0x1ff)*bits)<<5)>>3:(storage.w*bits)>>3;
 }else{
  result.w=std::max(Shr(logical.w-pad,level),1u)+pad;result.h=std::max(Shr(logical.h-pad,level),1u)+pad;
  result.d=dimension==2?std::max(Shr(logical.d-pad,level),1u)+pad:1u;
  U faces=1;
  if(dimension==3||(dimension==1&&(h1&0x400))){const U multiple=Shl(1,((h1>>10)&1)*2);faces=((Load(p,0x24)>>26)+multiple)&~(multiple-1);}
  U ex=31-LeadingZeros(power.w),ey=31-LeadingZeros(power.h),ez=31-LeadingZeros(power.d);
  std::int32_t step=static_cast<std::int32_t>(level);
  if(basePacked){++ex;++ey;++ez;}else --step;
  if(step<0){row=0;storage={power.w,power.h,power.d};}
  for(;step>=0;--step){
   if(ex)--ex;if(ey)--ey;if(ez)--ez;
   const Dimensions raw={Shl(1,ex),Shl(1,ey),Shl(1,ez)};
   storage=raw;Align(storage,bits,format,dimension,tiled);row=(storage.w*bits)>>3;
   if(packed&&(raw.w<=16||raw.h<=16)){
    packedOffset=PackedOffset(static_cast<U>(step),raw,row,row*storage.h,format);address+=packedOffset;break;
   }
   if(step>0){U bytes=row*storage.h;bytes=dimension==2?Page(bytes*storage.d):Page(bytes)*storage.d;address+=bytes*faces;}
  }
 }
 const U plane=row*storage.h;
 address+=Page(plane)*face;
 const U slice=dimension==2?plane:Page(plane);
 const U total=dimension==2?Page(plane*storage.d):slice*storage.d;
 return {row,slice,total,packedOffset,result.w,result.h,result.d,address};
}
void LockState(void* p,U level,U flags,const Layout& l){
 const U first=Load(p,0x20)&~0xfffu,second=Load(p,0x30)&~0xfffu;
 flags|=2;
 if(((flags&0x10)||(Load(p,0)&0x200000))&&!(flags&1)){
  const U base=level&&second?second:first;const U offset=level&&second?0x18:0x14;
  const U start=(l.address-l.packed)-(base&~127u),old=Load(p,offset);
  const U end=std::max((start+l.total+127)>>7,old&0xffff);
  const U begin=std::min(start>>7,old>>16);
  Store(p,offset,(begin<<16)|end);
 }
 InterlockedExchangeAdd(reinterpret_cast<volatile LONG*>(p),256);
}
}
extern "C" RECOVERED_TEXTURE_NATIVE_SURFACE_API void __stdcall TextureSurfaceGetLayout(const void* p,U face,U level,U* row,U* slice,U* total,U* packed,U* w,U* h,U* d,void** address){
 const auto l=Compute(p,face,level);*row=l.row;*slice=l.slice;*total=l.total;*packed=l.packed;*w=l.w;*h=l.h;*d=l.d;*address=reinterpret_cast<void*>(static_cast<std::uintptr_t>(l.address));
}
extern "C" RECOVERED_TEXTURE_NATIVE_SURFACE_API std::int32_t __stdcall TextureSurfaceLockRect(void* p,U level,LockedRect* output,const Rect* rect,U flags){
 const auto l=Compute(p,0,level);U bx,by;Blocks(Load(p,0x20)&63,bx,by);const U row=l.row*by;
 LockState(p,level,flags,l);
 U address=l.address;
 if(rect)address+=static_cast<U>(rect->top)*row+(static_cast<U>(rect->left)*Bits[Load(p,0x20)&63]>>3);
 output->pitch=static_cast<std::int32_t>(row);output->bits=reinterpret_cast<void*>(static_cast<std::uintptr_t>(address));return 0;
}
