#include "MeshSerialization.h"
#include <cstring>
#include <limits>
#include <stdexcept>
namespace Recovered { namespace W3x {
namespace {
void Write(unsigned char* p, std::uint32_t v, unsigned size, bool be) {
    for (unsigned i = 0; i < size; ++i) p[i] = static_cast<unsigned char>(v >> (8 * (be ? size - i - 1 : i)));
}
}
Relo::Chunk CompileMeshVertexData(const MeshPipelineData& input, int platform) {
    if (sizeof(void*) != 4) throw std::runtime_error("W3X Tracker serialization requires an x86 process");
    MeshVertexData data = BuildMeshVertexData(input, platform);
    if (data.vertices.size() > std::numeric_limits<std::uint32_t>::max() ||
        data.declaration.size() >= std::numeric_limits<std::uint32_t>::max() ||
        data.boneMapping.size() > std::numeric_limits<std::uint32_t>::max() / 2)
        throw std::length_error("W3X mesh vertex data exceeds tracker limits");
    const bool be = platform != 0;
    Relo::Tracker tracker;
    void* rootPointer = nullptr;
    unsigned char* root = static_cast<unsigned char*>(tracker.Push(&rootPointer, 28, 1));
    Write(root, data.vertexCount, 4, be);
    Write(root + 4, data.vertexStride, 4, be);
    unsigned char* vertices = static_cast<unsigned char*>(tracker.Push(
        reinterpret_cast<void**>(root + 8), 1, static_cast<std::uint32_t>(data.vertices.size())));
    if (!data.vertices.empty()) std::memcpy(vertices, data.vertices.data(), data.vertices.size());
    tracker.Pop();
    // BuildVertexDeclaration writes its length directly and performs no endian
    // correction, even though its enclosing scalar fields are target-endian.
    Write(root + 12, static_cast<std::uint32_t>(data.declaration.size()), 4, false);
    char* declaration = static_cast<char*>(tracker.Push(reinterpret_cast<void**>(root + 16), 1,
        static_cast<std::uint32_t>(data.declaration.size() + 1)));
    std::memcpy(declaration, data.declaration.c_str(), data.declaration.size() + 1);
    tracker.Pop();
    Write(root + 20, static_cast<std::uint32_t>(data.boneMapping.size()), 4, be);
    unsigned char* bones = static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(root + 24), 2,
        static_cast<std::uint32_t>(data.boneMapping.size())));
    for (std::size_t i = 0; i < data.boneMapping.size(); ++i) Write(bones + i * 2, data.boneMapping[i], 2, be);
    tracker.Pop();
    return tracker.MakeRelocatable(be, 0);
}
}}
