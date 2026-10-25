#include "MeshAsset.h"
#include <cstring>
#include <limits>
#include <stdexcept>

namespace Recovered { namespace W3x {
namespace {
void U32(unsigned char* p,std::uint32_t v,bool be) {
    for(unsigned i=0;i<4;++i) p[i]=static_cast<unsigned char>(v>>((be?3-i:i)*8));
}
void F32(unsigned char* p,float value,bool be) {
    std::uint32_t bits;std::memcpy(&bits,&value,4);U32(p,bits,be);
}
void V3(unsigned char* p,const MeshVector3& value,bool be) {
    for(unsigned i=0;i<3;++i) F32(p+i*4,value[i],be);
}
std::uint32_t Count(std::size_t value,unsigned itemSize) {
    if(value>(std::numeric_limits<std::uint32_t>::max()-3u)/itemSize)
        throw std::length_error("W3X mesh list exceeds tracker allocation limit");
    return static_cast<std::uint32_t>(value);
}
void UIntList(Relo::Tracker& tracker,unsigned char* list,
              const std::vector<std::uint32_t>& input,bool be) {
    const std::uint32_t count=Count(input.size(),4);
    U32(list,count,be);
    // Marshaler lists allocate only for nonzero count, unlike the custom
    // BuildVertexData output lists, which always allocate their pointer slots.
    if(!count) return;
    unsigned char* data=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(list+4),4,count));
    for(std::size_t i=0;i<input.size();++i) U32(data+i*4,input[i],be);
    tracker.Pop();
}
void Triangles(Relo::Tracker& tracker,unsigned char* root,const std::vector<MeshTriangle>& input,bool be) {
    const std::uint32_t count=Count(input.size(),24);
    U32(root+52,count,be);
    if(!count) return;
    unsigned char* triangles=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(root+56),24,count));
    for(std::size_t i=0;i<input.size();++i) {
        unsigned char* triangle=triangles+i*24;
        UIntList(tracker,triangle,input[i].vertices,be);
        V3(triangle+8,input[i].normal,be);
        F32(triangle+20,input[i].distance,be);
    }
    tracker.Pop();
}
void AabTree(Relo::Tracker& tracker,unsigned char* root,const MeshAabTree& input,bool be) {
    unsigned char* tree=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(root+88),16,1));
    UIntList(tracker,tree,input.polyIndices,be);
    const std::uint32_t count=Count(input.nodes.size(),32);
    U32(tree+8,count,be);
    if(count) {
        unsigned char* nodes=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(tree+12),32,count));
        for(std::size_t i=0;i<input.nodes.size();++i) {
            unsigned char* node=nodes+i*32;
            const MeshAabNode& src=input.nodes[i];
            V3(node,src.min,be);V3(node+12,src.max,be);
            // Source looks up Children then Polys separately; preserve both
            // flags for raw-source inputs, although valid XSD uses a choice.
            if(src.hasChildren) {
                unsigned char* children=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(node+24),8,1));
                U32(children,src.front,be);U32(children+4,src.back,be);tracker.Pop();
            }
            if(src.hasPolys) {
                unsigned char* polys=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(node+28),8,1));
                U32(polys,src.polyBegin,be);U32(polys+4,src.polyCount,be);tracker.Pop();
            }
        }
        tracker.Pop();
    }
    tracker.Pop();
}
void VertexData(Relo::Tracker& tracker,unsigned char* root,const MeshVertexData& input,bool be) {
    unsigned char* output=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(root+4),28,1));
    U32(output,input.vertexCount,be);U32(output+4,input.vertexStride,be);
    unsigned char* vertices=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(output+8),1,Count(input.vertices.size(),1)));
    if(!input.vertices.empty()) std::memcpy(vertices,input.vertices.data(),input.vertices.size());
    tracker.Pop();
    // Original CD writes declaration length directly, including on consoles.
    U32(output+12,Count(input.declaration.size(),1),false);
    const std::uint32_t textCount=Count(input.declaration.size()+1,1);
    char* declaration=static_cast<char*>(tracker.Push(reinterpret_cast<void**>(output+16),1,textCount));
    std::memcpy(declaration,input.declaration.c_str(),input.declaration.size()+1);tracker.Pop();
    U32(output+20,Count(input.boneMapping.size(),2),be);
    unsigned char* bones=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(output+24),2,Count(input.boneMapping.size(),2)));
    for(std::size_t i=0;i<input.boneMapping.size();++i) {
        const std::uint16_t bone=input.boneMapping[i];
        bones[i*2+(be?1:0)]=static_cast<unsigned char>(bone);
        bones[i*2+(be?0:1)]=static_cast<unsigned char>(bone>>8);
    }
    tracker.Pop();tracker.Pop();
}
}
Relo::Chunk CompileMeshAsset(const MeshAssetInput& input,int platform) {
    if(sizeof(void*)!=4) throw std::runtime_error("W3X mesh serialization requires x86 Tracker ABI");
    if(platform<0 || platform>2) throw std::invalid_argument("unknown W3X mesh platform");
    const bool be=platform!=0;
    // Original creates a separate temporary pipeline tracker, then marshals
    // the W3DMesh before constructing its vertex output. Building the pure
    // value first adds no output allocations and avoids partial output on error.
    MeshVertexData vertexData=BuildMeshVertexData(input.pipeline,platform);
    Relo::Tracker tracker;
    void* pointer=0;
    unsigned char* root=static_cast<unsigned char*>(tracker.Push(&pointer,96,1));
    // Offset0 is not written by the original W3DMesh Marshal or EndianCorrect.
    U32(root+8,input.geometryType,be);
    V3(root+12,input.boundingMin,be);V3(root+24,input.boundingMax,be);
    F32(root+36,input.sphereRadius,be);V3(root+40,input.sphereCenter,be);
    root[92]=input.hidden?1:0;root[93]=input.castShadow?1:0;root[94]=input.sortLevel;
    // Exact original Marshal allocation order: triangles, shader, optional AAB
    // tree; then HandleMesh adds the custom vertex-data block and its children.
    Triangles(tracker,root,input.triangles,be);
    if(input.hasShader) WriteShaderMaterial(tracker,root+60,input.shader,be);
    if(input.hasAabTree) AabTree(tracker,root,input.aabTree,be);
    VertexData(tracker,root,vertexData,be);
    tracker.Pop();
    return tracker.MakeRelocatable(be,0);
}
} }
