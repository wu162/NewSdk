#define RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API_BUILD
#include "TextureNativeDescriptorApi.h"
#include <algorithm>
#include <cstring>
using namespace Recovered::TextureCompiler::NativeDescriptorAbi;
namespace {
using U=std::uint32_t;
static_assert(sizeof(LevelDesc)==32&&sizeof(VolumeDesc)==28&&sizeof(TextureDesc)==60,"Original descriptor layouts");
U Load(const void* p,U offset){U n;std::memcpy(&n,static_cast<const unsigned char*>(p)+offset,4);return n;}
constexpr U Bits[]={1,1,8,16,16,16,32,32,8,8,16,16,16,32,32,16,32,32,4,8,8,64,32,32,16,32,64,16,32,64,16,32,64,32,64,128,32,64,128,8,16,16,32,8,8,16,16,16,32,8,32,4,8,8,32,32,32,96,4,4,4,4,32,32};
U Shift(U n,U count){return n<<(count&31);}
U Log(U n){U zeros=0;--n;while(zeros<32&&!(n&0x80000000u)){n<<=1;++zeros;}return 32-zeros;}
U Round(U n,U multiple){return (n+multiple-1)&~(multiple-1);}
void Blocks(U f,U& x,U& y){if(f==11||f==12||f==40){x=2;y=1;}else if(f==39||f==44){x=4;y=1;}else if((f>=18&&f<=20)||f==49||(f>=51&&f<=53)||(f>=58&&f<=61)){x=y=4;}else{x=y=1;}}
struct Dimensions{U w,h,d;};
Dimensions Base(const void* p){const U add=1+((Load(p,0x28)>>30)&2),n=Load(p,0x24),dim=(Load(p,0x30)>>9)&3;if(!dim)return {(n&0xffffff)+add,1,1};if(dim==2)return {(n&0x7ff)+add,((n>>11)&0x7ff)+add,(n>>22)+add};return {(n&0x1fff)+add,((n>>13)&0x1fff)+add,1};}
Dimensions Logical(const void* p,U level){auto d=Base(p);if(!level)return d;const U pad=(Load(p,0x28)>>31)*2;d.w=std::max((d.w-pad)>>(level&31),1u)+pad;d.h=std::max((d.h-pad)>>(level&31),1u)+pad;d.d=((Load(p,0x30)>>9)&3)==2?std::max((d.d-pad)>>(level&31),1u)+pad:1;return d;}
U Kind(const void* p){const U dim=(Load(p,0x30)>>9)&3;return dim==0?20:dim==2?17:dim==3?18:(Load(p,0x20)&0x400)?19:3;}
U Format(const void* p){const U a=Load(p,0x1c),b=Load(p,0x20);return (((((Load(p,0x28)&0x1fff)<<10)|(a&0x3fc))<<7)|((a>>23)&0x100)|(b&0xff));}
struct Storage{U w,h;bool tiled,packed;};
Storage AlignedStorage(const void* p,U level){
 const U format=Load(p,0x20)&63,dim=(Load(p,0x30)>>9)&3,border=Load(p,0x28)>>31,pad=border*2;const bool tiled=(Load(p,0x1c)>>31)!=0,packing=(Load(p,0x30)&0x800)!=0;
 const bool baseLevel=level==0;const auto base=Base(p);const U ex=Log(base.w-pad)+border,ey=Log(base.h-pad)+border;U w,h,d=1;bool tail;
 if(!level){w=Shift(1,ex);h=Shift(1,ey);tail=packing&&(w<=16||h<=16);if(!tail){w=base.w;h=base.h;}}
 else{
  const auto minimum=static_cast<std::int32_t>(std::min(ex,ey))-4;const U packedLevel=minimum>0?static_cast<U>(minimum):0;
  tail=packing&&level>=packedLevel;if(tail)level=packedLevel;
  const U ez=dim==2?Log(base.d-pad)+border:0;
  auto exponent=[level](U e){const auto n=static_cast<std::int32_t>(e-level);return n>0?static_cast<U>(n):0;};
  w=Shift(1,exponent(ex));h=Shift(1,exponent(ey));d=Shift(1,exponent(ez));
 }
 U bx,by;Blocks(format,bx,by);U mx=32;const U bytes=(bx*by*Bits[format])>>3;if(!tiled&&bytes)mx=std::max(32u,256/bytes);
 w=Round(w,bx*mx);h=Round(h,by*(dim?32u:1u));
 if(baseLevel)w=((Load(p,0x1c)>>22)&0x1ff)<<5;
 return {w,h,tiled,tail};
}
}
extern "C" RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API void __stdcall TextureGetLevelDesc(const void* p,U level,LevelDesc* output){const auto d=Logical(p,level);*output={Format(p),Kind(p),0,0,0,0,d.w,d.h};}
extern "C" RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API void __stdcall TextureGetVolumeLevelDesc(const void* p,U level,VolumeDesc* output){const auto d=Logical(p,level);*output={Format(p),Kind(p),0,0,d.w,d.h,d.d};}
extern "C" RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API void __stdcall TextureXGGetTextureDesc(const void* p,U level,TextureDesc* output){
 const auto d=Logical(p,level);const auto storage=AlignedStorage(p,level);const U kind=Kind(p),fmt=Format(p),bits=Bits[fmt&63];U bx,by;Blocks(fmt&63,bx,by);
 U height=storage.h;
 if(!level&&((Load(p,0x30)&0x600)==0x200)&&!storage.tiled&&!(Load(p,0x20)&0x400)&&!(Load(p,0x30)&0xfffff800u)&&!(Load(p,0x28)&0x80000000u))height=Round(Base(p).h,by);
 const U row=(bits*storage.w)>>3;U slice=row*height;
 if((storage.tiled||level>1||kind!=3)&&kind!=17)slice=Round(slice,4096);
 U exponent=(Load(p,0x28)>>13)&63;if(exponent&32)exponent|=0xffffffc0u;
 const U depth=kind==19?Load(p,0x24)>>26:kind==17?d.d:1;
 *output={kind,d.w,kind==20?1u:d.h,depth,fmt,row*by,slice,bits,(d.w+bx-1)/bx,((kind==20?1u:d.h)+by-1)/by,depth,(bits*bx*by)>>3,exponent,(storage.tiled?1u:0u)|(storage.packed?2u:0u)|((Load(p,0x28)&0x80000000u)?8u:0u),0};
}
