#define TEXTURE_PUBLIC_BC_BUILD
#include "TextureBcP.h"
#include "BC.h"
#include "TexturePublicBcApi.h"

namespace {
using U = std::uint32_t;
constexpr std::int32_t Invalid = static_cast<std::int32_t>(0x8876086cu);
U BlockSize(U format) { return format == 18 ? 8 : format == 19 || format == 20 ? 16 : 0; }
bool Encoding(U flags, float threshold) {
    return !(flags & ~0x70000u) && std::isfinite(threshold) && threshold >= 0 && threshold <= 1;
}
bool Pixels(const float* rgba) {
    for (U i = 0; i < 64; ++i) if (!std::isfinite(rgba[i]) || rgba[i] < 0 || rgba[i] > 1) return false;
    return true;
}
void Decode(const void* block, U format, float* rgba) {
    DirectX::XMVECTOR color[16];
    auto source = static_cast<const std::uint8_t*>(block);
    if (format == 18) DirectX::D3DXDecodeBC1(color, source);
    else if (format == 19) DirectX::D3DXDecodeBC2(color, source);
    else DirectX::D3DXDecodeBC3(color, source);
    for (U i = 0; i < 16; ++i) {
        DirectX::XMFLOAT4 pixel; DirectX::XMStoreFloat4(&pixel, color[i]);
        std::memcpy(rgba + i * 4, &pixel, sizeof(pixel));
    }
}
void Encode(void* block, U format, const float* rgba, U flags, float threshold) {
    DirectX::XMVECTOR color[16];
    for (U i = 0; i < 16; ++i) {
        DirectX::XMFLOAT4 pixel; std::memcpy(&pixel, rgba + i * 4, sizeof(pixel));
        color[i] = DirectX::XMLoadFloat4(&pixel);
    }
    auto target = static_cast<std::uint8_t*>(block);
    if (format == 18) DirectX::D3DXEncodeBC1(target, color, threshold, flags);
    else if (format == 19) DirectX::D3DXEncodeBC2(target, color, flags);
    else DirectX::D3DXEncodeBC3(target, color, flags);
}
bool Layout(U w, U h, U format, std::int32_t blockPitch, std::int32_t rgbaPitch) {
    return w && h && w <= 8192 && h <= 8192 && std::uint64_t(w) * h < 16777216 &&
        BlockSize(format) && blockPitch >= static_cast<std::int32_t>(((w + 3) / 4) * BlockSize(format)) &&
        rgbaPitch >= static_cast<std::int32_t>(w * 16) && !(rgbaPitch & 3) &&
        std::uint64_t(blockPitch) * ((h + 3) / 4) <= 0x7fffffffu && std::uint64_t(rgbaPitch) * h <= 0x7fffffffu;
}
}
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcDecodeBlock(const void* block, U format, float* rgba) {
    if (!block || !rgba || !BlockSize(format)) return Invalid;
    Decode(block, format, rgba); return 0;
}
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcEncodeBlock(void* block, U format, const float* rgba, U flags, float threshold) {
    if (!block || !rgba || !BlockSize(format) || !Encoding(flags, threshold) || !Pixels(rgba)) return Invalid;
    Encode(block, format, rgba, flags, threshold); return 0;
}
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcDecodeImage(const void* blocks, std::int32_t blockPitch, U width, U height, U format, float* rgba, std::int32_t rgbaPitch) {
    if (!blocks || !rgba || !Layout(width, height, format, blockPitch, rgbaPitch)) return Invalid;
    auto input = static_cast<const unsigned char*>(blocks); auto output = reinterpret_cast<unsigned char*>(rgba);
    for (U by = 0; by < height; by += 4) for (U bx = 0; bx < width; bx += 4) {
        float pixels[64]; Decode(input + std::size_t(by / 4) * blockPitch + bx / 4 * BlockSize(format), format, pixels);
        for (U y = 0; y < std::min(4u, height - by); ++y)
            std::memcpy(output + std::size_t(by + y) * rgbaPitch + bx * 16, pixels + y * 16, std::min(4u, width - bx) * 16);
    }
    return 0;
}
extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcEncodeImage(void* blocks, std::int32_t blockPitch, U width, U height, U format, const float* rgba, std::int32_t rgbaPitch, U flags, float threshold, U edgePolicy) {
    if (!blocks || !rgba || !Layout(width, height, format, blockPitch, rgbaPitch) || !Encoding(flags, threshold) || edgePolicy > 1) return Invalid;
    auto input = reinterpret_cast<const unsigned char*>(rgba); auto output = static_cast<unsigned char*>(blocks);
    // Validate the complete input before writing the first destination block.
    for (U y = 0; y < height; ++y) for (U x = 0; x < width * 4; ++x) {
        float v; std::memcpy(&v, input + std::size_t(y) * rgbaPitch + x * 4, 4);
        if (!std::isfinite(v) || v < 0 || v > 1) return Invalid;
    }
    for (U by = 0; by < height; by += 4) for (U bx = 0; bx < width; bx += 4) {
        float pixels[64];
        for (U y = 0; y < 4; ++y) for (U x = 0; x < 4; ++x) {
            const U sx = edgePolicy ? std::min(width - 1, bx + x) : (bx + x) % width;
            const U sy = edgePolicy ? std::min(height - 1, by + y) : (by + y) % height;
            std::memcpy(pixels + (y * 4 + x) * 4, input + std::size_t(sy) * rgbaPitch + sx * 16, 16);
        }
        Encode(output + std::size_t(by / 4) * blockPitch + bx / 4 * BlockSize(format), format, pixels, flags, threshold);
    }
    return 0;
}
