// Adapted from Microsoft DirectXTex filters.h / DirectXTexResize.cpp.
// Copyright (c) Microsoft Corporation. Licensed under the MIT License.
// Float-IO specialization of the accepted PublicDirectXTex adaptation.
// See ../PublicDirectXTex/upstream/LICENSE and local PROVENANCE.json.
#include "TexturePublicBcResize.h"
#include <algorithm>
#include <array>
#include <cassert>
#include <cmath>
#include <cstring>
#include <vector>

namespace Recovered { namespace TextureCompiler { namespace PublicD3dxBc {
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


}

// DirectXTex ResizeTriangleFilter's scatter, active-row counts, completed-row
// write and row reuse, with normalized float IO instead of DXGI/DirectXMath.
void ResizeRgbaFloat(const float* source, U sourceWidth, U sourceHeight,
    float* target, U targetWidth, U targetHeight) {
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
            std::memcpy(pixel.data(), source + (std::size_t(y) * sourceWidth + x) * 4, sizeof(pixel));
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
                // Match the actual D3DX surface-memory float consumer's row
                // store, after all triangle contributions. Its normalized
                // file path saturates channels here, including a constant
                // alpha plane whose accumulated weights exceed one by ulps.
                // Do not clamp individual taps or partially accumulated rows.
                for (auto& pixel : rowAcc.scanline) for (auto& channel : pixel)
                    channel = std::min(1.f, std::max(0.f, channel));
                std::memcpy(target + std::size_t(to.u) * targetWidth * 4, rowAcc.scanline.data(), std::size_t(targetWidth) * sizeof(Pixel));
                rowFree.push_back(&rowAcc);
            }
        }
    }
}
}}}
