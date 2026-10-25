#include "../src/Native/W3x/Shader.h"
#include <cassert>
#include <cstring>
#include <iostream>
using namespace Recovered::W3x;
static unsigned Word(const std::vector<unsigned char>& b,unsigned at,bool be) {
    unsigned v=0;for(unsigned i=0;i<4;++i) v|=unsigned(b.at(at+i))<<((be?3-i:i)*8);return v;
}
int main() {
    if(sizeof(void*)!=4) {std::cerr<<"W3X Shader tests require x86\n";return 2;}
    for(int endian=0;endian<2;++endian) {
        bool be=endian!=0;
        ShaderMaterialInput material;
        Recovered::Relo::Chunk empty=CompileShaderMaterial(material,be);
        assert(empty.instance.size()==32 && empty.imports.empty());
        assert(Word(empty.instance,0,be)==0 && Word(empty.instance,4,be)==0);
        assert(Word(empty.instance,8,be)==0 && Word(empty.instance,12,be)==28);
        assert(empty.instance[28]==0); // present default empty TechniqueName
        material.hasShaderName=true;material.shaderName="test.fx";
        material.techniqueName="T";material.techniqueIndex=255;
        const unsigned types[]={2750958653u,2300057986u,2777716390u,3741900581u,99u};
        for(unsigned i=0;i<5;++i) {
            ShaderConstantInput c;c.typeId=types[i];c.hasName=true;c.name="N";
            c.booleanValue=true;c.integerValue=-123;c.textureValue="Texture\\17";
            c.floatValues={1.f,-2.f};material.constants.push_back(c);
        }
        Recovered::Relo::Chunk chunk=CompileShaderMaterial(material,be);
        const std::vector<unsigned char>& data=chunk.instance;
        assert(Word(data,0,be)==7 && Word(data,8,be)==1);
        assert(data[24]==255 && Word(data,16,be)==5);
        unsigned slots=Word(data,20,be);
        for(unsigned i=0;i<5;++i) {
            unsigned c=Word(data,slots+i*4,be);
            assert(Word(data,c,be)==(i==4?0:types[i]));
            assert(Word(data,c+4,be)==1);
            unsigned name=Word(data,c+8,be);
            assert(data[name]=='N' && data[name+1]==0);
            if(i==0) assert(data[c+12]==1);
            if(i==1) assert(Word(data,c+12,be)==unsigned(-123));
            if(i==2) {
                assert(Word(data,c+12,be)==18);
                assert(chunk.imports.size()==8 && Word(chunk.imports,0,be)==c+12);
                assert(Word(chunk.imports,4,be)==0xffffffffu);
            }
            if(i==3) {
                assert(Word(data,c+12,be)==2);
                unsigned values=Word(data,c+16,be);
                assert(Word(data,values,be)==0x3f800000u);
                assert(Word(data,values+4,be)==0xc0000000u);
                assert(values<name); // derived list allocated before base Name
            }
        }
    }
    std::cout<<"PASS W3X Shader independent layout checks\n";
}
