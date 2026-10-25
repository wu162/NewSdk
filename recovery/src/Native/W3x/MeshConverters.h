#pragma once
#include <cstddef>
#include <cstdint>

namespace Recovered { namespace W3x {
// A recovered algorithm interface, not the original C++/CLI ABI.
enum class MeshConversion {
    CopyTexCoord, CompressTexCoord, Vector4ToUByte4N, Vector3ToHalf4,
    ConvertBlendIndex, ConvertBlendWeight, CompressNormalizedDirection,
    CopyVector3
};
struct ConverterContext {
    bool bigEndian;
    const std::uint16_t* inverseBoneMapping;
    std::size_t inverseBoneMappingCount;
    ConverterContext() : bigEndian(false), inverseBoneMapping(0), inverseBoneMappingCount(0) {}
};
// Preserves the original IL algorithm, including masked shift counts and its
// non-IEEE overflow result. Do not substitute a standard float16 converter.
std::uint16_t MeshFloatToHalf(float value);
void ConvertMeshStream(MeshConversion kind, int count,
    const void* src, std::ptrdiff_t srcPitch, void* dst, std::ptrdiff_t dstPitch,
    const ConverterContext& context);
}}
