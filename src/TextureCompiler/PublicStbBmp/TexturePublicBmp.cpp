#define STB_IMAGE_IMPLEMENTATION
#define STB_IMAGE_STATIC
#define STBI_ONLY_BMP
#define STBI_NO_STDIO
#define STBI_NO_HDR
#define STBI_NO_LINEAR
#define STBI_MAX_DIMENSIONS 8192
#define STBI_BMP_PRESERVE_ZERO_ALPHA 1
#include "stb_image.h"
#include "TexturePublicBmp.h"
#include <algorithm>
#include <cstring>
#include <limits>
#include <memory>

namespace Recovered { namespace TextureCompiler { namespace PublicStbBmp {
namespace {
using U = std::uint32_t;
U Read32(const std::vector<unsigned char>& b, U p) { U n; std::memcpy(&n, b.data() + p, 4); return n; }
U Read16(const std::vector<unsigned char>& b, U p) { return b[p] | U(b[p + 1]) << 8; }
void Write(std::vector<unsigned char>& b, U p, U n) { std::memcpy(b.data() + p, &n, 4); }
constexpr std::int32_t Unsupported = static_cast<std::int32_t>(0x80004001u);
constexpr std::int32_t InvalidData = static_cast<std::int32_t>(0x80004005u);
}

std::int32_t Decode(const std::vector<unsigned char>& bytes, Image& image) {
    if (bytes.size() < 18 || bytes[0] != 'B' || bytes[1] != 'M') return InvalidData;
    const U dib = Read32(bytes, 14), headerEnd = 14 + dib;
    if (dib != 12 && dib != 40 && dib != 52 && dib != 56 && dib != 64 && dib != 108 && dib != 124) return Unsupported;
    if (bytes.size() < headerEnd) return InvalidData;
    const bool core = dib == 12;
    const U w = core ? Read16(bytes, 18) : Read32(bytes, 18);
    const std::int32_t signedHeight = core ? static_cast<std::int32_t>(Read16(bytes, 20)) : static_cast<std::int32_t>(Read32(bytes, 22));
    const U planes = Read16(bytes, core ? 22 : 26), depth = Read16(bytes, core ? 24 : 28);
    const U compression = core ? 0 : Read32(bytes, 30), colors = core ? 0 : Read32(bytes, 46), offset = Read32(bytes, 10);
    if (compression || (depth != 24 && depth != 32)) return Unsupported;
    if (planes != 1 || !w || w > 8192 || !signedHeight || signedHeight == std::numeric_limits<std::int32_t>::min()) return InvalidData;
    const U h = signedHeight < 0 ? static_cast<U>(-signedHeight) : static_cast<U>(signedHeight);
    if (h > 8192 || std::uint64_t(w) * h >= 16777216) return Unsupported;
    const U stride = (w * (depth / 8) + 3) & ~3u;
    const std::uint64_t pixelStart = headerEnd + std::uint64_t(colors) * 4, rasterBytes = std::uint64_t(stride) * h;
    if (pixelStart > bytes.size() || offset < pixelStart || offset > bytes.size() ||
        rasterBytes > bytes.size() - offset || rasterBytes > bytes.size() - pixelStart) return InvalidData;
    if (dib == 124) {
        const U profileOffset = Read32(bytes, 126), profileBytes = Read32(bytes, 130);
        if ((!profileOffset) != (!profileBytes)) return InvalidData;
        if (profileBytes) {
            const std::uint64_t start = 14ull + profileOffset;
            if (start < std::uint64_t(offset) + rasterBytes || start + profileBytes > bytes.size()) return InvalidData;
        }
    }
    // Minimal host adaptation for the original D3DX BMP contract: BI_RGB
    // consumes pixels after the real DIB plus biClrUsed RGBQUADs, ignoring
    // bfOffBits and color metadata. Normalize the container, then actually use
    // stb's decoder for pixel extraction, padding, channel IO and orientation.
    std::vector<unsigned char> canonical(54 + static_cast<std::size_t>(rasterBytes));
    canonical[0] = 'B'; canonical[1] = 'M'; Write(canonical, 2, static_cast<U>(canonical.size()));
    Write(canonical, 10, 54); Write(canonical, 14, 40); Write(canonical, 18, w);
    Write(canonical, 22, static_cast<U>(signedHeight)); canonical[26] = 1; canonical[28] = static_cast<unsigned char>(depth);
    std::memcpy(canonical.data() + 54, bytes.data() + static_cast<std::size_t>(pixelStart), static_cast<std::size_t>(rasterBytes));
    int dw = 0, dh = 0, channels = 0;
    std::unique_ptr<stbi_uc, decltype(&stbi_image_free)> decoded(stbi_load_from_memory(canonical.data(), static_cast<int>(canonical.size()), &dw, &dh, &channels, 4), stbi_image_free);
    if (!decoded || dw != static_cast<int>(w) || dh != static_cast<int>(h)) return InvalidData;
    image.width = w; image.height = h; image.alpha = depth == 24;
    image.bgra.assign(decoded.get(), decoded.get() + std::size_t(w) * h * 4);
    for (std::size_t at = 0; at < image.bgra.size(); at += 4) {
        std::swap(image.bgra[at], image.bgra[at + 2]);
        if (depth == 32 && image.bgra[at + 3]) image.alpha = true;
    }
    return 0;
}
}}}
