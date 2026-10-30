#pragma once
#include <cstdint>
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
