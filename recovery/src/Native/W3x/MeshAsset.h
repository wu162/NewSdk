#pragma once
#include "MeshBuilder.h"
#include "Shader.h"
#include "../Relocation.h"

namespace Recovered { namespace W3x {
struct MeshTriangle {
    std::vector<std::uint32_t> vertices;
    MeshVector3 normal;
    float distance;
    MeshTriangle() : normal{{0,0,0}}, distance(0) {}
};
struct MeshAabNode {
    MeshVector3 min,max;
    bool hasChildren,hasPolys;
    std::uint32_t front,back,polyBegin,polyCount;
    MeshAabNode() : min{{0,0,0}},max{{0,0,0}},hasChildren(false),hasPolys(false),
        front(0),back(0),polyBegin(0),polyCount(0) {}
};
struct MeshAabTree {
    std::vector<std::uint32_t> polyIndices;
    std::vector<MeshAabNode> nodes;
};
struct MeshAssetInput {
    MeshPipelineData pipeline;
    std::uint32_t geometryType;
    bool hidden,castShadow;
    std::uint8_t sortLevel;
    MeshVector3 boundingMin,boundingMax,sphereCenter;
    float sphereRadius;
    std::vector<MeshTriangle> triangles;
    bool hasShader;
    ShaderMaterialInput shader;
    bool hasAabTree;
    MeshAabTree aabTree;
    MeshAssetInput() : geometryType(0),hidden(false),castShadow(false),sortLevel(0),
        boundingMin{{0,0,0}},boundingMax{{0,0,0}},sphereCenter{{0,0,0}},sphereRadius(0),
        hasShader(false),hasAabTree(false) {}
};
// Compiles the complete 96-byte W3DMesh root and child blocks. It expects
// already parsed/validated input; it is not the managed InstanceDeclaration ABI.
Relo::Chunk CompileMeshAsset(const MeshAssetInput&,int platform);
} }
