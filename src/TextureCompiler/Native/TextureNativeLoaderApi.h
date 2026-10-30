#pragma once
#include <cstdint>
#ifdef RECOVERED_TEXTURE_NATIVE_LOADER_API_BUILD
#define RECOVERED_TEXTURE_NATIVE_LOADER_API __declspec(dllexport)
#else
#define RECOVERED_TEXTURE_NATIVE_LOADER_API __declspec(dllimport)
#endif
namespace Recovered { namespace TextureCompiler { namespace NativeLoaderAbi {
using CreateFn=void* (__stdcall *)(std::uint32_t,std::uint32_t,std::uint32_t,std::uint32_t);
using LoadFn=std::int32_t (__stdcall *)(void*,const char*,void**);
}}}
// CPU, linear, ordinary 2D PC resources. levels=0 requests the complete chain.
// Returned object owns its VM allocations and is released with TextureResourceRelease.
// Unsupported formats/dimensions/level counts return nullptr before allocating.
extern "C" RECOVERED_TEXTURE_NATIVE_LOADER_API void* __stdcall TextureCreateCpuTexture(std::uint32_t width,std::uint32_t height,std::uint32_t levels,std::uint32_t format);
// PC D3DX default file contract, with explicit E_NOTIMPL outside restored codecs.
// device is the non-rendering type1 device; no rendering state is modified.
extern "C" RECOVERED_TEXTURE_NATIVE_LOADER_API std::int32_t __stdcall TextureCreateTextureFromFileA(void* device,const char* path,void** output);
