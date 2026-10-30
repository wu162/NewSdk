#pragma once
#include <cstdint>
#include <vector>

namespace Recovered { namespace TextureCompiler { namespace NativeRaster {
// Decoded, unresized PC storage. JPEG gray is one byte per texel (L8);
// color JPEG and PPM are BGRA/X8 with their original physical X byte.
struct Image {
    std::uint32_t width = 0, height = 0, format = 0;
    std::vector<unsigned char> pixels;
};

// Only JPEG and P2/P3/P5/P6 signatures are handled here. PGM is rejected as
// by the original; no output storage is transferred on decode failure.
std::int32_t Decode(const std::vector<unsigned char>& bytes, Image& output);
}}}
