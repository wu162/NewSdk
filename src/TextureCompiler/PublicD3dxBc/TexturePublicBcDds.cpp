#define TEXTURE_PUBLIC_BC_BUILD
#define NOMINMAX
#include <Windows.h>
#include "TexturePublicBcApi.h"
#include "TexturePublicBcResize.h"
#include "../Native/TextureNativeLoaderApi.h"
#include "../Native/TextureNativeResourceApi.h"
#include "../Native/TextureNativeSurfaceApi.h"
#include <algorithm>
#include <cmath>
#include <cstring>
#include <new>
#include <vector>

namespace {
using U = std::uint32_t;
constexpr std::int32_t Invalid = static_cast<std::int32_t>(0x8876086cu);
constexpr std::int32_t InvalidData = static_cast<std::int32_t>(0x80004005u);
constexpr std::int32_t Unsupported = static_cast<std::int32_t>(0x80004001u);
constexpr std::int32_t OutOfMemory = static_cast<std::int32_t>(0x8007000eu);
U Power(U n) { U value = 4; while (value < n) value <<= 1; return value; }
U Read(const unsigned char* p, U at) { U n; std::memcpy(&n, p + at, 4); return n; }
struct Resource {
    void* p = nullptr;
    ~Resource() { if (p) TextureResourceRelease(p); }
    void* detach() { auto result = p; p = nullptr; return result; }
};
struct Lock {
    void* resource; U level; bool active = false;
    Recovered::TextureCompiler::NativeSurfaceAbi::LockedRect rect{};
    Lock(void* p, U n) : resource(p), level(n) {}
    ~Lock() { if (active) TextureResourceUnlockRect(resource, level); }
    std::int32_t acquire() {
        auto status = TextureSurfaceLockRect(resource, level, &rect, nullptr, 0);
        active = status >= 0; return status;
    }
};
float Store(double value) { volatile float result = static_cast<float>(value); return result; }

// The public BC decoder supplies normalized float pixels. The old BOX row
// path performs three ordered float stores then a quarter store, including
// duplicate samples on a collapsed axis. Never quantize to RGBA8 between mips.
std::vector<float> Box(const std::vector<float>& source, U sw, U sh) {
    const U w = std::max(1u, sw >> 1), h = std::max(1u, sh >> 1);
    std::vector<float> result(std::size_t(w) * h * 4);
    for (U y = 0; y < h; ++y) for (U x = 0; x < w; ++x) for (U c = 0; c < 4; ++c) {
        const U x0 = sw > 1 ? x * 2 : 0, x1 = std::min(sw - 1, x0 + 1);
        const U y0 = sh > 1 ? y * 2 : 0, y1 = std::min(sh - 1, y0 + 1);
        const float a = Store(static_cast<double>(source[(std::size_t(y0) * sw + x1) * 4 + c]) + source[(std::size_t(y0) * sw + x0) * 4 + c]);
        const float b = Store(static_cast<double>(a) + source[(std::size_t(y1) * sw + x0) * 4 + c]);
        const float d = Store(static_cast<double>(b) + source[(std::size_t(y1) * sw + x1) * 4 + c]);
        result[(std::size_t(y) * w + x) * 4 + c] = Store(static_cast<double>(d) * 0.25);
    }
    return result;
}
}

extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcDecodeResizeDdsImage(
    const void* blocks, std::int32_t blockPitch, U sw, U sh, U format, float* output,
    std::int32_t outputPitch, U tw, U th) {
    const U blockSize = format == 18 ? 8 : format == 19 || format == 20 ? 16 : 0;
    if (!blocks || !output || !blockSize || !sw || !sh || !tw || !th || sw > 8192 || sh > 8192 || tw > 8192 || th > 8192 ||
        std::uint64_t(sw) * sh >= 16777216 || std::uint64_t(tw) * th >= 16777216 ||
        blockPitch < static_cast<std::int32_t>(((sw + 3) / 4) * blockSize) || outputPitch < static_cast<std::int32_t>(tw * 16) ||
        (outputPitch & 3) || std::uint64_t(blockPitch) * ((sh + 3) / 4) > 0x7fffffffu || std::uint64_t(outputPitch) * th > 0x7fffffffu) return Invalid;
    if (sw == tw && sh == th) return TexturePublicBcDecodeDdsImage(blocks, blockPitch, sw, sh, format, output, outputPitch);
    try {
        std::vector<float> pixels(std::size_t(sw) * sh * 4), resized(std::size_t(tw) * th * 4);
        const auto status = TexturePublicBcDecodeDdsImage(blocks, blockPitch, sw, sh, format, pixels.data(), sw * 16);
        if (status < 0) return status;
        Recovered::TextureCompiler::PublicD3dxBc::ResizeRgbaFloat(pixels.data(), sw, sh, resized.data(), tw, th);
        for (U y = 0; y < th; ++y) std::memcpy(reinterpret_cast<unsigned char*>(output) + std::size_t(y) * outputPitch, resized.data() + std::size_t(y) * tw * 4, tw * 16);
        return 0;
    } catch (const std::bad_alloc&) { return OutOfMemory; }
}

extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcCreateFromDds(
    const void* data, U byteCount, U flags, float threshold, U edgePolicy, void** output) {
    if (!data || !output || (flags != 0x80000000u && (flags & ~0x70000u)) || !std::isfinite(threshold) || threshold < 0 || threshold > 1 || edgePolicy > 1) return Invalid;
    // No partial output ownership is transferred on any failure.
    try {
        const auto bytes = static_cast<const unsigned char*>(data);
        if (byteCount < 128 || Read(bytes, 0) != 0x20534444 || Read(bytes, 4) != 124 || Read(bytes, 76) != 32) return InvalidData;
        if (!(Read(bytes, 80) & 4)) return Unsupported;
        const U fourcc = Read(bytes, 84), format = fourcc == 0x31545844 ? 18 : fourcc == 0x33545844 ? 19 : fourcc == 0x35545844 ? 20 : 0;
        const U width = Read(bytes, 16), height = Read(bytes, 12), provided = std::max(1u, Read(bytes, 28));
        if (!format || !width || !height || width > 8192 || height > 8192 ||
            (Read(bytes, 112) & 0xfe00) || Read(bytes, 24) > 1) return Unsupported;
        const U targetWidth = Power(width), targetHeight = Power(height);
        if (std::uint64_t(targetWidth) * targetHeight >= 16777216) return Unsupported;
        U levels = 1; for (U n = std::max(targetWidth, targetHeight); n > 1; n >>= 1) ++levels;
        U sourceLevels = 1; for (U n = std::max(width, height); n > 1; n >>= 1) ++sourceLevels;
        if (provided > sourceLevels) return Unsupported;
        const bool defaults = flags == 0x80000000u;
        // Default file upload is triangle+dither (80004); default POT mip
        // generation is BOX5 without dither. Do not apply one policy to both.
        const U uploadFlags = defaults ? 0x30000u : flags, mipFlags = defaults ? 0u : flags;
        const U blockSize = format == 18 ? 8 : 16;
        // Check every provided level's storage before allocating a resource.
        std::uint64_t required = 128; U w = width, h = height;
        for (U level = 0; level < provided; ++level) {
            required += std::uint64_t((w + 3) / 4) * ((h + 3) / 4) * blockSize;
            w = std::max(1u, w >> 1); h = std::max(1u, h >> 1);
        }
        if (required > byteCount) return InvalidData;
        Resource resource; resource.p = TextureCreateCpuTexture(targetWidth, targetHeight, levels, 0x1a200040u | format);
        if (!resource.p) return OutOfMemory;
        U offset = 128; w = width; h = height;
        for (U level = 0; level < provided; ++level) {
            const U pitch = ((w + 3) / 4) * blockSize, rows = (h + 3) / 4;
            const U tw = std::max(1u, targetWidth >> level), th = std::max(1u, targetHeight >> level);
            Lock target(resource.p, level); auto status = target.acquire(); if (status < 0) return status;
            if (w == tw && h == th && w >= 4 && h >= 4) {
                for (U row = 0; row < rows; ++row)
                    std::memcpy(static_cast<unsigned char*>(target.rect.bits) + std::size_t(row) * target.rect.pitch, bytes + offset + std::size_t(row) * pitch, pitch);
            } else {
                std::vector<float> pixels(std::size_t(tw) * th * 4);
                status = TexturePublicBcDecodeResizeDdsImage(bytes + offset, pitch, w, h, format, pixels.data(), tw * 16, tw, th); if (status < 0) return status;
                status = TexturePublicBcEncodeDdsImage(target.rect.bits, target.rect.pitch, tw, th, format, pixels.data(), tw * 16, uploadFlags, threshold, edgePolicy); if (status < 0) return status;
            }
            offset += pitch * rows; w = std::max(1u, w >> 1); h = std::max(1u, h >> 1);
        }
        for (U level = provided; level < levels; ++level) {
            const U sw = std::max(1u, targetWidth >> (level - 1)), sh = std::max(1u, targetHeight >> (level - 1));
            std::vector<float> pixels(std::size_t(sw) * sh * 4);
            {
                Lock previous(resource.p, level - 1); auto status = previous.acquire(); if (status < 0) return status;
                status = TexturePublicBcDecodeDdsImage(previous.rect.bits, previous.rect.pitch, sw, sh, format, pixels.data(), sw * 16); if (status < 0) return status;
            }
            auto filtered = Box(pixels, sw, sh);
            Lock target(resource.p, level); auto status = target.acquire(); if (status < 0) return status;
            const U tw = std::max(1u, sw >> 1), th = std::max(1u, sh >> 1);
            status = TexturePublicBcEncodeDdsImage(target.rect.bits, target.rect.pitch, tw, th, format, filtered.data(), tw * 16, mipFlags, threshold, edgePolicy); if (status < 0) return status;
        }
        *output = resource.detach(); return 0;
    } catch (const std::bad_alloc&) { return OutOfMemory; }
}

extern "C" TEXTURE_PUBLIC_BC_API std::int32_t __stdcall TexturePublicBcCreateTextureFromFileA(
    void* device, const char* path, void** output) {
    if (!device || !path || !output) return Invalid;
    struct File { HANDLE p = INVALID_HANDLE_VALUE; ~File() { if (p != INVALID_HANDLE_VALUE) CloseHandle(p); } } file;
    constexpr auto BadFile = static_cast<std::int32_t>(0x88760b59u);
    try {
        file.p = CreateFileA(path, GENERIC_READ, FILE_SHARE_READ, nullptr, OPEN_EXISTING, FILE_FLAG_RANDOM_ACCESS, nullptr);
        if (file.p == INVALID_HANDLE_VALUE) return BadFile;
        LARGE_INTEGER size{};
        if (!GetFileSizeEx(file.p, &size) || size.QuadPart < 0 || size.QuadPart > 0x10000000) return BadFile;
        std::vector<unsigned char> bytes(static_cast<std::size_t>(size.QuadPart)); DWORD count = 0;
        if (!ReadFile(file.p, bytes.data(), static_cast<DWORD>(bytes.size()), &count, nullptr) || count != bytes.size()) return BadFile;
        if (bytes.size() >= 84 && Read(bytes.data(), 0) == 0x20534444 && (Read(bytes.data(), 80) & 4))
            return TexturePublicBcCreateFromDds(bytes.data(), static_cast<U>(bytes.size()), 0x80000000u, 0.5f, 0, output);
        return TextureCreateTextureFromFileA(device, path, output);
    } catch (const std::bad_alloc&) { return OutOfMemory; }
}
