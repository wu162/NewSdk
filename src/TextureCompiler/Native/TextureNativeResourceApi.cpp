#include "TextureNativeResourceApi.h"
#include <Windows.h>
#include <cstring>
static_assert(sizeof(void*)==4,"PC resource ABI requires x86");
namespace {
std::uint32_t Load(const void* p,std::uint32_t offset){std::uint32_t value;std::memcpy(&value,static_cast<const unsigned char*>(p)+offset,4);return value;}
void Store(void* p,std::uint32_t offset,std::uint32_t value){std::memcpy(static_cast<unsigned char*>(p)+offset,&value,4);}
volatile LONG* AtomicWord(void* p,std::uint32_t offset){return reinterpret_cast<volatile LONG*>(static_cast<unsigned char*>(p)+offset);}
void* Pointer(std::uint32_t p){return reinterpret_cast<void*>(static_cast<std::uintptr_t>(p));}
bool View(std::uint32_t header){return (header&15)==4&&(header&0x40000000u)!=0;}
bool UnsupportedFinal(std::uint32_t header,void* resource){return (header&15)==9||((header&15)==4&&!View(header)&&((header&0x80000000u)!=0||(Load(resource,0x20)&1)!=0));}
}
extern "C" __declspec(dllexport) std::uint32_t __stdcall TextureResourceAddRef(void* resource){
 const auto count=static_cast<std::uint32_t>(InterlockedIncrement(AtomicWord(resource,4)));
 if(count==1&&View(Load(resource,0)))TextureResourceAddRef(Pointer(Load(resource,0x18)));
 return count;
}
extern "C" __declspec(dllexport) std::uint32_t __stdcall TextureResourceRelease(void* resource){
 const auto header=Load(resource,0);
 // These context/GPU destruction algorithms are not part of the restored PC
 // memory-resource group. Preserve the object on explicit unsupported input.
 if(Load(resource,4)==1&&UnsupportedFinal(header,resource))return 0xffffffffu;
 const auto count=static_cast<std::uint32_t>(InterlockedDecrement(AtomicWord(resource,4)));
 if(count)return count;
 if(View(header))TextureResourceRelease(Pointer(Load(resource,0x18)));
 switch(header&15){
  case 1:VirtualFree(Pointer(Load(resource,0x18)&~3u),0,MEM_RELEASE);break;
  case 2:case 7:case 8:VirtualFree(Pointer(Load(resource,0x18)),0,MEM_RELEASE);break;
  case 3:
   VirtualFree(Pointer(Load(resource,0x20)&~0xfffu),0,MEM_RELEASE);
   VirtualFree(Pointer(Load(resource,0x30)&~0xfffu),0,MEM_RELEASE);
   break;
  case 6:VirtualFree(Pointer(Load(resource,0x20)),0,MEM_RELEASE);break;
  // Type4 views borrow parent's storage, ordinary CPU surfaces own no VM here;
  // type0/type5 and unknown default types only own their GlobalAlloc object.
 }
 GlobalFree(resource);
 return 0;
}
extern "C" __declspec(dllexport) std::int32_t __stdcall TextureResourceUnlockRect(void* texture,std::uint32_t){
 const auto secondary=Load(texture,0x30)&~0xfffu;
 const auto previous=static_cast<std::uint32_t>(InterlockedExchangeAdd(AtomicWord(texture,0),-256));
 if((previous&0xf00)==0x100){
  Store(texture,0x14,0xffff0000u);
  if(secondary)Store(texture,0x18,0xffff0000u);
 }
 // The public native wrapper discards the internal result unconditionally.
 return 0;
}
