#pragma once
#include <cstdint>
#ifdef RECOVERED_TEXTURE_NATIVE_SURFACE_API_BUILD
#define RECOVERED_TEXTURE_NATIVE_SURFACE_API __declspec(dllexport)
#else
#define RECOVERED_TEXTURE_NATIVE_SURFACE_API __declspec(dllimport)
#endif
namespace Recovered { namespace TextureCompiler { namespace NativeSurfaceAbi {
struct Rect{std::int32_t left,top,right,bottom;};
struct LockedRect{std::int32_t pitch;void* bits;};
// Original PC texture header, resource type3, all four dimensional encodings.
// Valid pointers and bounded native header dimensions/levels are required.
using LayoutFn=void (__stdcall *)(const void*,std::uint32_t,std::uint32_t,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,void**);
using LockFn=std::int32_t (__stdcall *)(void*,std::uint32_t,LockedRect*,const Rect*,std::uint32_t);
}}}
extern "C" RECOVERED_TEXTURE_NATIVE_SURFACE_API void __stdcall TextureSurfaceGetLayout(const void*,std::uint32_t,std::uint32_t,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,std::uint32_t*,void**);
extern "C" RECOVERED_TEXTURE_NATIVE_SURFACE_API std::int32_t __stdcall TextureSurfaceLockRect(void*,std::uint32_t,Recovered::TextureCompiler::NativeSurfaceAbi::LockedRect*,const Recovered::TextureCompiler::NativeSurfaceAbi::Rect*,std::uint32_t);
