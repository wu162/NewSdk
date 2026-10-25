#include "../src/Native/W3x/MeshBuilder.h"
#include <cstring>
#include <iostream>
#include <stdexcept>
using namespace Recovered::W3x;
namespace {
unsigned assertions=0;
void Check(bool value,const char* message) { ++assertions; if(!value) throw std::runtime_error(message); }
template<class F> void Reject(F f,const char* message) {
    bool rejected=false; try {f();} catch(const std::exception&) {rejected=true;} Check(rejected,message);
}
MeshPipelineData Base() {
    MeshPipelineData p;
    p.vertices.push_back(std::vector<MeshVector3>{MeshVector3{{1,2,3}},MeshVector3{{4,5,6}}});
    return p;
}
void Basic() {
    MeshPipelineData p=Base();
    MeshVertexData out=BuildMeshVertexData(p,0);
    Check(out.vertexCount==2 && out.vertexStride==16,"Win basic count/stride");
    Check(out.declaration=="p0:00:3f32 c0:0C:4u8n","Win basic declaration");
    const unsigned char golden[]={0,0,128,63,0,0,0,64,0,0,64,64,255,255,255,255,
                                  0,0,128,64,0,0,160,64,0,0,192,64,255,255,255,255};
    Check(out.vertices==std::vector<unsigned char>(golden,golden+sizeof(golden)),"Win complete golden vertex bytes");
    Check(out.boneMapping.empty(),"no bone map");
    for(int platform=1;platform<=2;++platform) {
        MeshVertexData consoleOut=BuildMeshVertexData(p,platform);
        Check(consoleOut.vertexCount==2 && consoleOut.vertexStride==12,"console basic count/stride");
        Check(consoleOut.declaration=="p0:00:4f16 c0:08:4u8n","console basic declaration");
        Check(consoleOut.vertices[8]==255 && consoleOut.vertices[9]==255 &&
              consoleOut.vertices[10]==255 && consoleOut.vertices[11]==255,"console default color");
    }
    // A lone tangent or binormal stream has no mapping in the original.
    p.tangents.push_back(MeshVector3{{0,1,0}});
    Check(BuildMeshVertexData(p,0).vertices==out.vertices,"unpaired tangent ignored");
}
void AllStreams() {
    MeshPipelineData p=Base();
    p.vertices.push_back(p.vertices[0]);
    p.normals.assign(2,std::vector<MeshVector3>(2,MeshVector3{{0,0,1}}));
    p.tangents.assign(2,MeshVector3{{1,0,0}});
    p.binormals.assign(2,MeshVector3{{0,1,0}});
    p.colors.assign(2,MeshVector4{{1,0,0,1}});
    p.texCoords.assign(2,std::vector<MeshVector2>(2,MeshVector2{{0.5f,1}}));
    p.boneInfluences.push_back(std::vector<MeshBoneInfluence>{MeshBoneInfluence{0.25f,9,0},MeshBoneInfluence{0.5f,2,0}});
    p.boneInfluences.push_back(std::vector<MeshBoneInfluence>{MeshBoneInfluence{0.99f,2,0},MeshBoneInfluence{0.1f,9,0}});
    std::vector<std::uint16_t> fwd,inv;
    ComputeBoneMappingTable(p,fwd,inv);
    Check(fwd==std::vector<std::uint16_t>({2,9}),"bone map sorted unique");
    Check(inv.size()==10 && inv[2]==0 && inv[9]==1 && inv[8]==0,"inverse gaps zero");
    MeshVertexData win=BuildMeshVertexData(p,0);
    Check(win.vertexStride==100,"Win full stride");
    Check(win.declaration=="p0:00:3f32 p1:0C:3f32 n0:18:3f32 n1:24:3f32 c0:30:4u8n g0:34:3f32 b0:40:3f32 t0:4C:2f32 t1:54:2f32 i0:5C:4u8 w0:60:4u8n","Win full declaration");
    Check(win.vertices[92]==1 && win.vertices[93]==0 && win.vertices[94]==0 && win.vertices[95]==0,"Win index data/padding");
    Check(win.vertices[96]==64 && win.vertices[97]==191 && win.vertices[98]==0 && win.vertices[99]==0,"Win complementary weight overrides second source");
    Check(win.vertices[196]==128 && win.vertices[197]==127,"Win second complementary weight");
    for(int platform=1;platform<=2;++platform) {
        MeshVertexData out=BuildMeshVertexData(p,platform);
        Check(out.vertexStride==52,"console full stride");
        Check(out.declaration=="p0:00:4f16 p1:08:4f16 n0:10:3s11n n1:14:3s11n c0:18:4u8n g0:1C:3s11n b0:20:3s11n t0:24:2f16 t1:28:2f16 i0:2C:4u8 w0:30:4u8n","console full declaration");
        const unsigned i=platform==1?47:44,w=platform==1?51:48;
        const int next=platform==1?-1:1;
        Check(out.vertices[i]==1 && out.vertices[i+next]==0,"console bone remap");
        Check(out.vertices[w]==64 && out.vertices[w+next]==191,"console weight complement");
    }
    // Content selection uses min(position-stream count,bone-stream count),
    // whereas mapping-table collection includes every source influence.
    p.vertices.resize(1);
    MeshVertexData one=BuildMeshVertexData(p,0);
    Check(one.boneMapping==fwd,"unused second bone stream still collected");
    Check(one.declaration.find("w0:")==std::string::npos,"one position stream omits blend weights");
    Check(one.declaration.find("i0:")!=std::string::npos,"one position stream retains indices");
}
void Boundaries() {
    MeshPipelineData p=Base();
    Reject([&] {BuildMeshVertexData(p,99);},"unknown platform rejected");
    MeshPipelineData empty;
    Reject([&] {BuildMeshVertexData(empty,0);},"missing position list rejected");
    p.normals.push_back(std::vector<MeshVector3>(1));
    Reject([&] {BuildMeshVertexData(p,0);},"short normal list rejected");
    p=Base(); p.texCoords.assign(60,std::vector<MeshVector2>(2,MeshVector2{{0,0}}));
    Reject([&] {BuildMeshVertexData(p,0);},"declaration original buffer overflow rejected");
    p=Base(); p.boneInfluences.push_back(std::vector<MeshBoneInfluence>{{1,65535,0},{1,0,0}});
    std::vector<std::uint16_t> fwd,inv;
    ComputeBoneMappingTable(p,fwd,inv);
    Check(fwd.size()==2 && fwd[1]==65535 && inv.size()==65536 && inv[65535]==1,"full ushort bone range");
    p.boneInfluences.clear(); ComputeBoneMappingTable(p,fwd,inv);
    Check(fwd.empty() && inv.empty(),"clear previous bone maps");
    std::vector<MeshDataMapping> mappings;
    Check(DetermineVertexDataLayout(mappings,0)==0 && BuildVertexDeclaration(mappings,2).empty(),"empty mappings");
    MeshDataMapping bad={8,0,0,0,0}; mappings.push_back(bad);
    Reject([&] {DetermineVertexDataLayout(mappings,0);},"invalid usage rejected");
}
}
int main() {
    try {Basic();AllStreams();Boundaries();std::cout<<"PASS W3x mesh builder "<<assertions<<" assertions\n";return 0;}
    catch(const std::exception& e) {std::cerr<<e.what()<<"\n";return 1;}
}
