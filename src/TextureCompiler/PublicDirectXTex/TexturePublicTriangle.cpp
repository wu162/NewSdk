// Adapted from Microsoft DirectXTex filters.h / DirectXTexResize.cpp.
// Copyright (c) Microsoft Corporation. Licensed under the MIT License.
// See upstream/LICENSE, PROVENANCE.json and PATCHES.md in this directory.
#include "TexturePublicTriangle.h"
#include <algorithm>
#include <array>
#include <cassert>
#include <cmath>
#include <cstring>
#include <vector>

namespace Recovered { namespace TextureCompiler { namespace PublicDirectXTex {
namespace {
using U = std::uint32_t;
using Pixel = std::array<float, 4>;
struct FilterTo { U u; float weight; };
using Filter = std::vector<std::vector<FilterTo>>;
struct TriangleRow { U remaining = 0; std::vector<Pixel> scanline; };
constexpr float TF_EPSILON = 0.00001f;

// The old D3DX x87 filter stores these temporaries as float, while its
// intervening arithmetic remains extended. Do not use float SSE arithmetic
// or DirectXMath multiply/add here: it changes weights and final byte ties.
float Store(double value) { volatile float result = static_cast<float>(value); return result; }
float Truncate(double value) {
    float result = Store(value);
    if (static_cast<double>(result) > value) {
        U bits; std::memcpy(&bits, &result, 4); --bits; std::memcpy(&result, &bits, 4);
    }
    return result;
}

// DirectX::Filters::CreateTriangleFilter, specialized to wrap, with bounded
// vectors replacing its variable-sized allocation. The two half-kernels,
// clipped destination cells, merging and influence order are upstream's.
Filter CreateTriangleFilter(U source, U dest) {
    const float scale = Store(static_cast<double>(dest) / source);
    const float scaleInv = Store(0.5 / static_cast<double>(scale));
    Filter result(source);
    U accumU = 0;
    float accumWeight = 0;
    for (U u = 0; u < source; ++u) {
        auto& from = result[u];
        for (U j = 0; j < 2; ++j) {
            const float src = Store(static_cast<double>(u + j) - 0.5);
            const float destMin = Store(static_cast<double>(src) * scale);
            const float destMax = Store(static_cast<double>(destMin) + scale);
            for (int k = static_cast<int>(std::floor(destMin)); static_cast<double>(k) < destMax; ++k) {
                float d0 = static_cast<float>(k), d1 = static_cast<float>(k + 1);
                const U u0 = static_cast<U>(k < 0 ? k + static_cast<int>(dest) :
                    k >= static_cast<int>(dest) ? k - static_cast<int>(dest) : k);
                if (u0 != accumU) {
                    if (accumWeight > TF_EPSILON) from.push_back({accumU, accumWeight});
                    accumWeight = 0; accumU = u0;
                }
                if (d0 < destMin) d0 = destMin;
                if (d1 > destMax) d1 = destMax;
                const float weight = Store((static_cast<double>(d0) + d1) * scaleInv - src);
                accumWeight = Store(static_cast<double>(accumWeight) +
                    (static_cast<double>(d1) - d0) * (j ? 1.0 - static_cast<double>(weight) : static_cast<double>(weight)));
            }
        }
        if (accumWeight > TF_EPSILON) from.push_back({accumU, accumWeight});
        accumWeight = 0;
    }
    return result;
}

void StoreScanline(unsigned char* target, const std::vector<Pixel>& row, U y, bool alpha, bool luminance) {
    // D3DX RGBA8 row writer: ordered dither, serpentine X lookup, toward-zero
    // float stores. X8 packing has no alpha mask, so its unused byte is zero.
    constexpr float dither[] = {31.f/32,15.f/32,27.f/32,11.f/32,7.f/32,23.f/32,3.f/32,19.f/32,
        25.f/32,9.f/32,29.f/32,13.f/32,1.f/32,17.f/32,5.f/32,21.f/32};
    const U width = static_cast<U>(row.size());
    for (U x = 0; x < width; ++x) {
        const double bias = dither[(y & 3) * 4 + (((y & 1) ? width - 1 - x : x) & 3)];
        if (luminance) {
            // D3DX L8 writer first stores its weighted RGB luminance toward
            // zero, even when all three source channels came from one byte.
            const float value = Truncate(static_cast<double>(row[x][1]) * 0.715399980545044 +
                static_cast<double>(row[x][0]) * 0.21250000596046448 +
                static_cast<double>(row[x][2]) * 0.072099998593330383);
            const float scaled = Truncate(std::max(0.0, std::min(1.0, static_cast<double>(value))) * 255.0);
            target[x] = static_cast<unsigned char>(std::min(255, std::max(0,
                static_cast<int>(Truncate(static_cast<double>(scaled) + bias)))));
            continue;
        }
        for (U c = 0; c < 4; ++c) {
            if (c == 3 && !alpha) { target[x * 4 + c] = 0; continue; }
            const double value = std::max(0.0, std::min(1.0, static_cast<double>(row[x][c])));
            const float scaled = Truncate(value * 255.0);
            target[x * 4 + c] = static_cast<unsigned char>(std::min(255, std::max(0,
                static_cast<int>(Truncate(static_cast<double>(scaled) + bias)))));
        }
    }
}

// DirectXTex ResizeTriangleFilter's scatter, active-row counts, completed-row
// write and row reuse, with scalar BGRA8 IO instead of DXGI/DirectXMath.
void Resize(const unsigned char* source, U sourcePitch, U sourceWidth, U sourceHeight,
    unsigned char* target, U targetPitch, U targetWidth, U targetHeight, bool alpha, bool luminance) {
    assert(source && target && sourceWidth && sourceHeight && targetWidth && targetHeight);
    const Filter tfX = CreateTriangleFilter(sourceWidth, targetWidth);
    const Filter tfY = CreateTriangleFilter(sourceHeight, targetHeight);
    std::vector<TriangleRow> rowActive(targetHeight);
    std::vector<TriangleRow*> rowFree;
    for (const auto& yFrom : tfY) for (const auto& to : yFrom) ++rowActive[to.u].remaining;
    for (U y = 0; y < sourceHeight; ++y) {
        const auto& yFrom = tfY[y];
        for (const auto& to : yFrom) {
            auto& rowAcc = rowActive[to.u];
            if (rowAcc.scanline.empty()) {
                if (!rowFree.empty()) { rowAcc.scanline.swap(rowFree.back()->scanline); rowFree.pop_back(); }
                else rowAcc.scanline.resize(targetWidth);
                std::fill(rowAcc.scanline.begin(), rowAcc.scanline.end(), Pixel{});
            }
        }
        for (U x = 0; x < sourceWidth; ++x) {
            Pixel pixel;
            for (U c = 0; c < 4; ++c) pixel[c] = c == 3 && !alpha ? 1.f :
                Store(source[y * sourcePitch + (luminance ? x : x * 4 + c)] * 0.0039215688593685627);
            for (const auto& vy : yFrom) for (const auto& vx : tfX[x]) {
                const float weight = Store(static_cast<double>(vy.weight) * vx.weight);
                auto& acc = rowActive[vy.u].scanline[vx.u];
                for (U c = 0; c < 4; ++c) acc[c] = Store(static_cast<double>(acc[c]) + static_cast<double>(pixel[c]) * weight);
            }
        }
        for (const auto& to : yFrom) {
            auto& rowAcc = rowActive[to.u];
            assert(rowAcc.remaining);
            if (!--rowAcc.remaining) {
                StoreScanline(target + to.u * targetPitch, rowAcc.scanline, to.u, alpha, luminance);
                rowFree.push_back(&rowAcc);
            }
        }
    }
}
}
void ResizeBgra8(const unsigned char* source, U sourcePitch, U sourceWidth, U sourceHeight,
    unsigned char* target, U targetPitch, U targetWidth, U targetHeight, bool alpha) {
    Resize(source, sourcePitch, sourceWidth, sourceHeight, target, targetPitch, targetWidth, targetHeight, alpha, false);
}
void ResizeL8(const unsigned char* source, U sourcePitch, U sourceWidth, U sourceHeight,
    unsigned char* target, U targetPitch, U targetWidth, U targetHeight) {
    Resize(source, sourcePitch, sourceWidth, sourceHeight, target, targetPitch, targetWidth, targetHeight, false, true);
}
}}}
