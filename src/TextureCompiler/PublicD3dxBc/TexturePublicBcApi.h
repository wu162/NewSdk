#pragma once
#include <cstdint>
#include "../Native/TextureNativeCopyApi.h"
#ifdef TEXTURE_PUBLIC_BC_BUILD
#define TEXTURE_PUBLIC_BC_API __declspec(dllexport)
#else
#define TEXTURE_PUBLIC_BC_API __declspec(dllimport)
#endif
// Isolated PC candidate, not the production XGCopySurface backend.
// format is the existing PC base format: 18=DXT1,19=DXT3,20=DXT5.
// Pixel IO is normalized float RGBA, retaining precision for DDS/mip consumers.
// Flags use public DirectXTex bits 0x10000/0x20000/0x40000; mapping the original
// XG filter flags and validating exact original output remain required.
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcDecodeBlock(
    const void* block, std::uint32_t format, float* rgba);
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcEncodeBlock(
    void* block, std::uint32_t format, const float* rgba, std::uint32_t flags, float threshold);
// Image variants consume/produce complete compressed blocks with caller-owned
// pitches. Decode crops partial blocks; encode fills partial blocks by the
// explicit edge policy (0=wrap logical image,1=clamp), never hidden gold rules.
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcDecodeImage(
    const void* blocks, std::int32_t blockPitch, std::uint32_t width, std::uint32_t height,
    std::uint32_t format, float* rgba, std::int32_t rgbaPitch);
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcEncodeImage(
    void* blocks, std::int32_t blockPitch, std::uint32_t width, std::uint32_t height,
    std::uint32_t format, const float* rgba, std::int32_t rgbaPitch,
    std::uint32_t flags, float threshold, std::uint32_t edgePolicy);
// Experimental owned-resource DXT1/3/5 DDS consumer. Base axes round up
// to POT and clamp to4;0x80000000 selects actual file/mip defaults. Not the accepted default
// file loader. Copies complete provided blocks, re-encodes provided partial
// tails and recursively filters/re-encodes missing mips from stored blocks.
// Release a successful resource with TextureResourceRelease.
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcCreateFromDds(
    const void* data, std::uint32_t byteCount, std::uint32_t flags,
    float threshold, std::uint32_t edgePolicy, void** output);
// Experimental actual LoadImageA consumer: public BC decode -> normalized
// float native row conversion, with source endian and swizzle preserved.
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcCopySurface(
    void*, std::int32_t, std::uint32_t, std::uint32_t, std::uint32_t,
    const Recovered::TextureCompiler::NativeCopyAbi::Point*, const void*, std::int32_t,
    std::uint32_t, const Recovered::TextureCompiler::NativeCopyAbi::Rect*, std::uint32_t, float);

// D3DX file/mip rows use A4/15 and weighted-sum BC3 alpha, then the
// original stored-reciprocal unpremultiplication. D3DX encode premultiplies.
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcDecodeDdsImage(
    const void*, std::int32_t, std::uint32_t, std::uint32_t, std::uint32_t,
    float*, std::int32_t);
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcEncodeDdsImage(
    void*, std::int32_t, std::uint32_t, std::uint32_t, std::uint32_t,
    const float*, std::int32_t, std::uint32_t, float, std::uint32_t);

extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcDecodeStoredImage(
    const void*, std::int32_t, std::uint32_t, std::uint32_t, std::uint32_t,
    float*, std::int32_t, std::uint32_t);
// Isolated real file entry; compressed DDS uses only this public BC candidate.
// Other accepted formats delegate to the unchanged accepted native loader.
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcCreateTextureFromFileA(
    void*, const char*, void**);
