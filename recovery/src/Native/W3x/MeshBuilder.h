#pragma once
#include <array>
#include <cstdint>
#include <string>
#include <vector>
#include "MeshConverters.h"

namespace Recovered { namespace W3x {
// Typed input, not the original x86 C++/CLI object ABI. The vectors represent
// the unmarshaled W3DMeshPipelineVertexData lists in their original order.
using MeshVector2 = std::array<float, 2>;
using MeshVector3 = std::array<float, 3>;
using MeshVector4 = std::array<float, 4>;
struct MeshBoneInfluence {
    float weight;
    std::uint16_t bone;
    std::uint16_t padding;
};
struct MeshPipelineData {
    std::vector<std::vector<MeshVector3> > vertices, normals;
    std::vector<MeshVector3> tangents, binormals;
    std::vector<MeshVector4> colors;
    std::vector<std::vector<MeshVector2> > texCoords;
    std::vector<std::vector<MeshBoneInfluence> > boneInfluences;
};
struct MeshDataMapping {
    std::uint32_t usage, usageIndex;
    const void* source;
    std::uint32_t sourcePitch, destinationOffset;
};
struct MeshVertexData {
    std::uint32_t vertexCount, vertexStride;
    std::vector<unsigned char> vertices;
    std::string declaration;
    std::vector<std::uint16_t> boneMapping;
};
// Platform numbers are the original BAB TargetPlatform values 0/1/2.
// Unknown platforms are explicitly rejected rather than reproducing a null
// handler dereference. All inputs must remain alive while mappings are used.
std::vector<MeshDataMapping> DetermineVertexDataContents(const MeshPipelineData&);
std::uint32_t DetermineVertexDataLayout(std::vector<MeshDataMapping>&, int platform);
void ComputeBoneMappingTable(const MeshPipelineData&, std::vector<std::uint16_t>& forward,
                             std::vector<std::uint16_t>& inverse);
std::string BuildVertexDeclaration(const std::vector<MeshDataMapping>&, int platform);
MeshVertexData BuildMeshVertexData(const MeshPipelineData&, int platform);
} }
