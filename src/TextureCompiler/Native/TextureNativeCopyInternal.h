#pragma once
#include "TextureNativeCopyApi.h"

// Private entry points for embedding the accepted public BC decoder. The
// compressed bridge must call raw conversion directly, not re-enter dispatch.
extern "C" std::int32_t __stdcall TextureCopySurfaceUncompressed(
    void*, std::int32_t, std::uint32_t, std::uint32_t, std::uint32_t,
    const Recovered::TextureCompiler::NativeCopyAbi::Point*, const void*,
    std::int32_t, std::uint32_t,
    const Recovered::TextureCompiler::NativeCopyAbi::Rect*, std::uint32_t, float);
extern "C" std::int32_t __stdcall TexturePublicBcCopySurface(
    void*, std::int32_t, std::uint32_t, std::uint32_t, std::uint32_t,
    const Recovered::TextureCompiler::NativeCopyAbi::Point*, const void*,
    std::int32_t, std::uint32_t,
    const Recovered::TextureCompiler::NativeCopyAbi::Rect*, std::uint32_t, float);
