#include "TextureNativeRaster.h"
#include <limits>
#include <new>
#include <utility>

// These are the existing accepted wrappers around pinned public IJG6b and
// adapted rdppm.c. BuildTextureNativeLoader links their C objects and jpeg.lib
// statically; no original compiler or separate decoder DLL is called.
extern "C" int __cdecl TextureDecodeJpeg(const unsigned char*, int,
    unsigned char*, int, int*, int*, int*);
extern "C" int __cdecl TextureDecodePpm(const unsigned char*, int,
    unsigned char*, int, int*, int*);

namespace Recovered { namespace TextureCompiler { namespace NativeRaster {
namespace {
constexpr std::int32_t InvalidData = static_cast<std::int32_t>(0x80004005u);
constexpr std::int32_t Unsupported = static_cast<std::int32_t>(0x80004001u);
constexpr std::int32_t OutOfMemory = static_cast<std::int32_t>(0x8007000eu);
}

std::int32_t Decode(const std::vector<unsigned char>& bytes, Image& output) {
    if (bytes.size() < 2) return Unsupported;
    const bool jpeg = bytes[0] == 0xff && bytes[1] == 0xd8;
    const bool ppm = bytes[0] == 'P' &&
        (bytes[1] == '2' || bytes[1] == '3' || bytes[1] == '5' || bytes[1] == '6');
    if (!jpeg && !ppm) return Unsupported;
    if (bytes.size() > static_cast<std::size_t>(std::numeric_limits<int>::max())) return InvalidData;
    // The original parser only supports RGB P3/P6. Do not add standard PGM
    // support merely because the retained public source also describes it.
    if (ppm && (bytes[1] == '2' || bytes[1] == '5')) return InvalidData;
    try {
        const auto count = static_cast<int>(bytes.size());
        int width = 0, height = 0, gray = 0;
        const int header = jpeg ? TextureDecodeJpeg(bytes.data(), count, nullptr, 0, &width, &height, &gray) :
            TextureDecodePpm(bytes.data(), count, nullptr, 0, &width, &height);
        if (!header || width <= 0 || height <= 0 || width > 8192 || height > 8192 ||
            std::uint64_t(width) * height >= 16777216u) return InvalidData;
        const auto pixels = static_cast<std::size_t>(width) * height;
        std::vector<unsigned char> rgba(pixels * 4);
        int decodedWidth = 0, decodedHeight = 0, decodedGray = 0;
        const int decoded = jpeg ? TextureDecodeJpeg(bytes.data(), count, rgba.data(), static_cast<int>(rgba.size()),
            &decodedWidth, &decodedHeight, &decodedGray) :
            TextureDecodePpm(bytes.data(), count, rgba.data(), static_cast<int>(rgba.size()), &decodedWidth, &decodedHeight);
        if (!decoded || decodedWidth != width || decodedHeight != height || (jpeg && decodedGray != gray)) return InvalidData;

        Image image;
        image.width = static_cast<std::uint32_t>(width);
        image.height = static_cast<std::uint32_t>(height);
        image.format = jpeg && gray ? 0x28000002u : 0x28280086u;
        if (jpeg && gray) {
            image.pixels.resize(pixels);
            for (std::size_t pixel = 0; pixel < pixels; ++pixel) image.pixels[pixel] = rgba[pixel * 4];
        } else {
            for (std::size_t pixel = 0; pixel < pixels; ++pixel) {
                const auto at = pixel * 4;
                std::swap(rgba[at], rgba[at + 2]);
                // Original JPEG RGB row writer (RVA14bce) leaves X=0,
                // while the PPM packed row writer (RVA15463) forces X=FF.
                if (jpeg) rgba[at + 3] = 0;
            }
            image.pixels = std::move(rgba);
        }
        output = std::move(image);
        return 0;
    } catch (const std::bad_alloc&) { return OutOfMemory; }
}
}}}
