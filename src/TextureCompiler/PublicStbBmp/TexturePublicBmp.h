#pragma once
#include <cstdint>
#include <vector>
namespace Recovered { namespace TextureCompiler { namespace PublicStbBmp {
struct Image {
    std::uint32_t width = 0, height = 0;
    bool alpha = false;
    std::vector<unsigned char> bgra;
};
// Bounded BI_RGB 24/32 storage; preserves the physical all-zero alpha plane.
std::int32_t Decode(const std::vector<unsigned char>& bytes, Image& image);
}}}
