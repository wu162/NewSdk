#pragma once
#include <cstdint>
namespace Recovered { namespace TextureCompiler { namespace PublicD3dxBc {
// DirectXTex wrapped triangle, retaining every float store of PC D3DX.
void ResizeRgbaFloat(const float*, std::uint32_t, std::uint32_t,
    float*, std::uint32_t, std::uint32_t);
}}}
