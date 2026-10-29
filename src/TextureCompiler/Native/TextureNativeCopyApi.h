#pragma once
#include <cstdint>
#ifdef RECOVERED_TEXTURE_NATIVE_COPY_API_BUILD
#define RECOVERED_TEXTURE_NATIVE_COPY_API __declspec(dllexport)
#else
#define RECOVERED_TEXTURE_NATIVE_COPY_API __declspec(dllimport)
#endif
namespace Recovered { namespace TextureCompiler { namespace NativeCopyAbi {
struct Point{std::int32_t x,y;};struct Rect{std::int32_t left,top,right,bottom;};
using CopyFn=std::int32_t (__stdcall *)(void*,std::int32_t,std::uint32_t,std::uint32_t,std::uint32_t,const Point*,const void*,std::int32_t,std::uint32_t,const Rect*,std::uint32_t,float);
}}}
// Original XGCopySurface's same-format, block-aligned row-copy path only.
// Nonoverlapping valid storage and bounded dimensions/strides are required.
// Conversion/filter/unaligned paths return E_NOTIMPL without changing storage.
extern "C" RECOVERED_TEXTURE_NATIVE_COPY_API std::int32_t __stdcall TextureCopySurfaceIdentity(void*,std::int32_t,std::uint32_t,std::uint32_t,std::uint32_t,const Recovered::TextureCompiler::NativeCopyAbi::Point*,const void*,std::int32_t,std::uint32_t,const Recovered::TextureCompiler::NativeCopyAbi::Rect*,std::uint32_t,float);

// Identity plus unsigned-normalized PC format conversion (28 layouts, selectors 0..5).
// Conversion requires flags=0, little endian, finite threshold in [0,1], and
// nonoverlapping valid storage. Other format attributes are explicitly rejected.
extern "C" RECOVERED_TEXTURE_NATIVE_COPY_API std::int32_t __stdcall TextureCopySurface(void*,std::int32_t,std::uint32_t,std::uint32_t,std::uint32_t,const Recovered::TextureCompiler::NativeCopyAbi::Point*,const void*,std::int32_t,std::uint32_t,const Recovered::TextureCompiler::NativeCopyAbi::Rect*,std::uint32_t,float);
