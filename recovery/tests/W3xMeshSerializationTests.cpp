#include "../src/Native/W3x/MeshSerialization.h"
#include <iostream>
#include <stdexcept>
using namespace Recovered::W3x;
static unsigned Word(const std::vector<unsigned char>& p, unsigned at, bool be) {
    unsigned v = 0; for (unsigned i = 0; i < 4; ++i) v |= unsigned(p.at(at+i)) << (8 * (be ? 3-i : i)); return v;
}
static void Check(bool condition) { if (!condition) throw std::runtime_error("Mesh serialization assertion"); }
int main() {
    if (sizeof(void*) != 4) { std::cout << "SKIP W3X mesh serialization requires x86\n"; return 0; }
    try {
        MeshPipelineData input;
        input.vertices.push_back(std::vector<MeshVector3>(1, MeshVector3{{1,2,3}}));
        for (int platform = 0; platform != 3; ++platform) {
            const bool be = platform != 0;
            auto expected = BuildMeshVertexData(input, platform);
            auto chunk = CompileMeshVertexData(input, platform);
            Check(Word(chunk.instance, 0, be) == 1);
            Check(Word(chunk.instance, 4, be) == expected.vertexStride);
            Check(Word(chunk.instance, 8, be) == 28);
            Check(Word(chunk.instance, 12, false) == expected.declaration.size());
            auto pointer = Word(chunk.instance, 16, be);
            Check(std::string(reinterpret_cast<const char*>(chunk.instance.data()+pointer)) == expected.declaration);
            Check(Word(chunk.instance, 20, be) == 0);
            Check(chunk.relocations.size() == 16 && Word(chunk.relocations, 12, be) == 0xffffffffu);
            Check(Word(chunk.relocations, 0, be) == 8 && Word(chunk.relocations, 4, be) == 16 && Word(chunk.relocations, 8, be) == 24);
            Check(chunk.imports.empty());
        }
        input.boneInfluences.push_back(std::vector<MeshBoneInfluence>(1, MeshBoneInfluence{1.f, 37, 0}));
        for (int platform = 0; platform != 3; ++platform) {
            const bool be = platform != 0;
            auto chunk = CompileMeshVertexData(input, platform);
            const unsigned declaration = Word(chunk.instance, 16, be);
            const unsigned length = Word(chunk.instance, 12, false);
            const unsigned bones = Word(chunk.instance, 24, be);
            Check(bones == ((declaration + length + 1 + 3) & ~3u));
            Check(Word(chunk.instance, 20, be) == 1);
            Check(chunk.instance.at(bones + (be ? 1 : 0)) == 37);
            Check(chunk.instance.at(bones + (be ? 0 : 1)) == 0);
            Check(chunk.imports.empty());
        }
        std::cout << "PASS W3X mesh vertex serialization platforms=0,1,2\n";
        return 0;
    } catch (const std::exception& error) { std::cerr << error.what() << '\n'; return 1; }
}
