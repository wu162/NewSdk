#define TEXTURE_PUBLIC_BC_BUILD
#include "TexturePublicBcApi.h"
#include <cmath>
#include <cstring>
#include <new>
#include <vector>

using namespace Recovered::TextureCompiler::NativeCopyAbi;
namespace {
using U = std::uint32_t;
constexpr std::int32_t Unsupported = static_cast<std::int32_t>(0x80004001u);
constexpr std::int32_t Invalid = static_cast<std::int32_t>(0x8876086cu);
constexpr std::int32_t OutOfMemory = static_cast<std::int32_t>(0x8007000eu);
bool Compressed(U f) { return (f & 63) >= 18 && (f & 63) <= 20; }
}
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcCopySurface(
    void* destination, std::int32_t destinationPitch, U width, U height, U destinationFormat,
    const Point* point, const void* source, std::int32_t sourcePitch, U sourceFormat,
    const Rect* rectangle, U flags, float threshold) {
    // Existing raw-row and exact block-identity semantics are reused as their
    // accepted host conversion; all compressed decoding below is public BC.
    if (!Compressed(sourceFormat) || Compressed(destinationFormat))
        return TextureCopySurface(destination, destinationPitch, width, height, destinationFormat, point, source, sourcePitch, sourceFormat, rectangle, flags, threshold);
    if (!source || !destination || !std::isfinite(threshold) || threshold < 0 || threshold > 1) return Invalid;
    if (sourcePitch < 0 || destinationPitch < 0) return OutOfMemory;
    if (sourceFormat & ~0x3ffc00ffu) return Unsupported;
    if (!width || !height || width > 8192 || height > 8192) return Unsupported;
    const Point origin = point ? *point : Point{0, 0};
    const Rect rect = rectangle ? *rectangle : Rect{0, 0, static_cast<std::int32_t>(width), static_cast<std::int32_t>(height)};
    if (origin.x < 0 || origin.y < 0 || rect.left < 0 || rect.top < 0 || rect.right < rect.left || rect.bottom < rect.top) return Unsupported;
    const U left = rect.left, top = rect.top, right = rect.right, bottom = rect.bottom, x = origin.x, y = origin.y;
    if (right > width || bottom > height || x > width || y > height || right-left > width-x || bottom-top > height-y) return Unsupported;
    if (left == right || top == bottom) return 0;
    if (std::uint64_t(right) * bottom >= 16777216) return Unsupported;
    const U format = sourceFormat & 63, blockSize = format == 18 ? 8 : 16;
    const U rowBytes = ((right + 3) / 4) * blockSize, rows = (bottom + 3) / 4;
    if (sourcePitch < static_cast<std::int32_t>(rowBytes) || std::uint64_t(sourcePitch) * rows > 0x7fffffffu) return Unsupported;
    const U endian = (sourceFormat >> 6) & 3;
    constexpr U xorMask[] = {0, 1, 3, 2};
    try {
        // Compressed rows always contain complete 8/16-byte blocks, so every
        // endian unit is complete; no undefined raw-row tail is introduced.
        std::vector<unsigned char> little(std::size_t(rowBytes) * rows);
        const auto input = static_cast<const unsigned char*>(source);
        for (U row = 0; row < rows; ++row) for (U byte = 0; byte < rowBytes; ++byte)
            little[std::size_t(row) * rowBytes + byte] = input[std::size_t(row) * sourcePitch + (byte ^ xorMask[endian])];
        std::vector<float> pixels(std::size_t(right) * bottom * 4);
        const auto status = TexturePublicBcDecodeImage(little.data(), rowBytes, right, bottom, format, pixels.data(), right * 16);
        if (status < 0) return status;
        const U floatSourceFormat = (sourceFormat & 0x3ffc0000u) | 0x0002aa26u;
        return TextureCopySurface(destination, destinationPitch, width, height, destinationFormat, point,
            pixels.data(), right * 16, floatSourceFormat, rectangle, flags, threshold);
    } catch (const std::bad_alloc&) { return OutOfMemory; }
}
