#include "../src/Native/W3x/MeshAsset.h"
#include <cstring>
#include <iostream>
#include <stdexcept>
using namespace Recovered::W3x;
namespace {
unsigned checks=0;
void Check(bool value,const char* message) {++checks;if(!value) throw std::runtime_error(message);}
std::uint32_t Read(const std::vector<unsigned char>& data,std::size_t at,bool be) {
    std::uint32_t value=0;for(unsigned i=0;i<4;++i)value|=std::uint32_t(data.at(at+i))<<((be?3-i:i)*8);return value;
}
std::uint32_t Bits(float value) {std::uint32_t b;std::memcpy(&b,&value,4);return b;}
MeshAssetInput Fixture() {
    MeshAssetInput input;
    input.pipeline.vertices.push_back(std::vector<MeshVector3>{MeshVector3{{1,2,3}},MeshVector3{{4,5,6}},MeshVector3{{7,8,9}}});
    input.geometryType=3;input.hidden=true;input.castShadow=true;input.sortLevel=32;
    input.boundingMin=MeshVector3{{-1,-2,-3}};input.boundingMax=MeshVector3{{1,2,3}};
    input.sphereRadius=10;input.sphereCenter=MeshVector3{{2,3,4}};
    MeshTriangle triangle;triangle.vertices={0,1,2};triangle.normal=MeshVector3{{0,0,1}};triangle.distance=1.5f;
    input.triangles.push_back(triangle);triangle.vertices={2,1,0};input.triangles.push_back(triangle);
    input.hasAabTree=true;input.aabTree.polyIndices={2,1,0};
    MeshAabNode node;node.min=MeshVector3{{-1,-2,-3}};node.max=MeshVector3{{1,2,3}};
    node.hasChildren=true;node.front=1;node.back=2;input.aabTree.nodes.push_back(node);
    node.hasChildren=false;node.hasPolys=true;node.polyBegin=0;node.polyCount=1;input.aabTree.nodes.push_back(node);
    node.polyBegin=1;node.polyCount=2;input.aabTree.nodes.push_back(node);
    return input;
}
void Test(int platform) {
    const bool be=platform!=0;
    MeshAssetInput input=Fixture();Recovered::Relo::Chunk result=CompileMeshAsset(input,platform);
    const std::vector<unsigned char>& d=result.instance;
    Check(Read(d,0,be)==0,"unused root word remains zero");
    Check(Read(d,8,be)==3,"geometry target endian");
    Check(Read(d,12,be)==Bits(-1) && Read(d,32,be)==Bits(3),"root box bounds");
    Check(Read(d,36,be)==Bits(10) && Read(d,40,be)==Bits(2),"root sphere bounds");
    Check(d[92]==1 && d[93]==1 && d[94]==32 && d[95]==0,"root byte attributes and padding");
    Check(Read(d,52,be)==2 && Read(d,56,be)==96,"triangles contiguous allocation first");
    Check(Read(d,96,be)==3 && Read(d,100,be)==144,"first triangle index allocation");
    Check(Read(d,120,be)==3 && Read(d,124,be)==156,"second triangle index allocation");
    Check(Read(d,144,be)==0 && Read(d,152,be)==2 && Read(d,156,be)==2,"triangle index values");
    Check(Read(d,112,be)==Bits(1) && Read(d,116,be)==Bits(1.5f),"triangle normal and distance endian");
    for(unsigned i=60;i<88;++i)Check(d[i]==0,"missing shader stays zero without default strings");
    Check(Read(d,88,be)==168,"optional tree allocated after triangles");
    Check(Read(d,168,be)==3 && Read(d,172,be)==184,"AAB polygon list");
    Check(Read(d,176,be)==3 && Read(d,180,be)==196,"AAB nodes contiguous allocation");
    Check(Read(d,184,be)==2 && Read(d,192,be)==0,"AAB polygon values");
    Check(Read(d,196,be)==Bits(-1) && Read(d,216,be)==Bits(3),"AAB node bounds endian");
    Check(Read(d,220,be)==292 && Read(d,224,be)==0,"children optional pointer");
    Check(Read(d,292,be)==1 && Read(d,296,be)==2,"children values endian");
    Check(Read(d,252,be)==0 && Read(d,256,be)==300,"polys optional pointer");
    Check(Read(d,300,be)==0 && Read(d,304,be)==1 && Read(d,308,be)==1 && Read(d,312,be)==2,"polys values endian");
    Check(Read(d,4,be)==316,"vertex data allocated after envelope");
    Check(Read(d,316,be)==3 && Read(d,320,be)==(be?12u:16u),"vertex count and stride");
    Check(Read(d,324,be)==344,"vertex buffer allocation");
    const std::string declaration=be?"p0:00:4f16 c0:08:4u8n":"p0:00:3f32 c0:0C:4u8n";
    Check(Read(d,328,false)==declaration.size(),"vertex declaration length stays little endian");
    const std::uint32_t text=Read(d,332,be);
    Check(std::memcmp(d.data()+text,declaration.c_str(),declaration.size()+1)==0,"embedded vertex declaration");
    Check(Read(d,336,be)==0,"empty bone map count");
    Check(result.imports.empty(),"no imports without shader textures");
    Check(result.relocations.size()==56,"thirteen relocations plus sentinel");
    Check(Read(result.relocations,0,be)==56 && Read(result.relocations,4,be)==88 && Read(result.relocations,8,be)==4,
          "root relocation order follows allocation targets rather than numeric field offsets");
    Check(Read(result.relocations,result.relocations.size()-4,be)==0xffffffffu,"relocation sentinel");
}
}
int main() {
    try {
        if(sizeof(void*)!=4) {
            bool rejected=false;try{CompileMeshAsset(Fixture(),0);}catch(const std::runtime_error&){rejected=true;}
            Check(rejected,"x64 serialization must explicitly reject ABI mismatch");
            std::cout<<"PASS W3x mesh asset x64 ABI rejection; x86 envelope assertions require Windows runner\n";
            return 0;
        }
        Test(0);Test(1);Test(2);
        std::cout<<"PASS W3x mesh asset "<<checks<<" assertions; three target platforms\n";return 0;
    }catch(const std::exception& e){std::cerr<<e.what()<<"\n";return 1;}
}
