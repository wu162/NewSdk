// Evidence-only harness. No codec implementation and no original binary patch.
// A completed scratch frame controls otherwise unspecified incoming stack bytes
// before the immutable original nvDXTcompress body is called normally.
#include <cstdint>
#include <intrin.h>
using Writer=int (__cdecl *)(void*,unsigned int,void*,void*);
using Compressor=int (__cdecl *)(void*,void*,Writer,void*);
__declspec(noinline) static void PrepareStack(std::uint32_t fill) {
    volatile std::uint32_t scratch[32768];
    for(unsigned int i=0;i<32768;i++)scratch[i]=fill;
    _ReadWriteBarrier();
}
extern "C" __declspec(dllexport) int __cdecl TextureBoundaryCompress(
    void* image,void* options,Compressor compressor,Writer writer,std::uint32_t fill) {
    PrepareStack(fill);
    return compressor(image,options,writer,nullptr);
}
