#pragma once
#include <cstdint>

namespace Recovered { namespace TextureCompiler { namespace PublicDirectXTex {
// PC D3DX default wrapped triangle, BGRA8 storage. Call only for a size
// conversion; the loader's unchanged-size path preserves the physical X byte.
void ResizeBgra8(const unsigned char* source, std::uint32_t sourcePitch,
    std::uint32_t sourceWidth, std::uint32_t sourceHeight,
    unsigned char* target, std::uint32_t targetPitch,
    std::uint32_t targetWidth, std::uint32_t targetHeight, bool alpha);
// JPEG grayscale keeps its original L8 storage through filtering. The same
// public triangle kernel feeds the original luminance row store.
void ResizeL8(const unsigned char* source, std::uint32_t sourcePitch,
    std::uint32_t sourceWidth, std::uint32_t sourceHeight,
    unsigned char* target, std::uint32_t targetPitch,
    std::uint32_t targetWidth, std::uint32_t targetHeight);
}}}
