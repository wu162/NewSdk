#include "Shader.h"
#include "../ResourceMarshaling.h"
#include <cstring>
#include <limits>
#include <stdexcept>

namespace Recovered { namespace W3x {
namespace {
const std::uint32_t BoolType=2750958653u,IntType=2300057986u,
                    TextureType=2777716390u,FloatType=3741900581u;
void Word(unsigned char* p,std::uint32_t value,bool bigEndian) {
    for(unsigned i=0;i<4;++i)
        p[bigEndian?3-i:i]=static_cast<unsigned char>(value>>(i*8));
}
void WriteString(Relo::Tracker& tracker,unsigned char* destination,
                 const std::string& value,bool bigEndian) {
    if(value.size()>=std::numeric_limits<std::uint32_t>::max())
        throw std::length_error("W3X Shader string exceeds tracker range");
    Native::MarshalString(tracker,destination,value);
    std::uint32_t length;
    std::memcpy(&length,destination,4);
    Word(destination,length,bigEndian);
}
void WriteConstant(Relo::Tracker& tracker,void** slot,
                   const ShaderConstantInput& input,bool bigEndian) {
    const bool isBool=input.typeId==BoolType, isInt=input.typeId==IntType,
               isTexture=input.typeId==TextureType,isFloat=input.typeId==FloatType;
    const std::uint32_t size=isFloat?20u:(isBool||isInt||isTexture)?16u:12u;
    unsigned char* root=static_cast<unsigned char*>(tracker.Push(slot,size,1));
    // In the original all derived values are marshaled BEFORE the base Name.
    if(isBool) root[12]=input.booleanValue?1:0;
    else if(isInt) Word(root+12,static_cast<std::uint32_t>(input.integerValue),bigEndian);
    else if(isTexture) Native::MarshalReference(tracker,root+12,input.textureValue);
    else if(isFloat) {
        if(input.floatValues.size()>std::numeric_limits<std::uint32_t>::max()/4u)
            throw std::length_error("W3X Shader float list exceeds tracker range");
        const std::uint32_t count=static_cast<std::uint32_t>(input.floatValues.size());
        Word(root+12,count,bigEndian);
        if(count) {
            unsigned char* values=static_cast<unsigned char*>(tracker.Push(
                reinterpret_cast<void**>(root+16),4,count));
            for(std::size_t i=0;i<input.floatValues.size();++i) {
                std::uint32_t bits;
                std::memcpy(&bits,&input.floatValues[i],4);
                // Original EndianCorrect<float list> stores uint32 bits. It
                // does not call the float-returning EndianToPlatform helper.
                Word(values+i*4,bits,bigEndian);
            }
            tracker.Pop();
        }
    }
    if(input.hasName) WriteString(tracker,root+4,input.name,bigEndian);
    const std::uint32_t type=isBool||isInt||isTexture||isFloat?input.typeId:0u;
    Word(root,type,bigEndian);
    tracker.Pop();
}
}
void WriteShaderMaterial(Relo::Tracker& tracker,unsigned char* root,
                         const ShaderMaterialInput& input,bool bigEndian) {
    if(sizeof(void*)!=4)
        throw std::logic_error("W3X shader tracker serialization requires an x86 host");
    if(!root) throw std::invalid_argument("W3X shader requires an allocated destination");
    if(input.hasShaderName) WriteString(tracker,root,input.shaderName,bigEndian);
    WriteString(tracker,root+8,input.techniqueName,bigEndian);
    root[24]=input.techniqueIndex;
    if(input.constants.size()>std::numeric_limits<std::uint32_t>::max()/4u)
        throw std::length_error("W3X Shader constant list exceeds tracker range");
    const std::uint32_t count=static_cast<std::uint32_t>(input.constants.size());
    Word(root+16,count,bigEndian);
    if(count) {
        unsigned char* slots=static_cast<unsigned char*>(tracker.Push(
            reinterpret_cast<void**>(root+20),4,count));
        for(std::size_t i=0;i<input.constants.size();++i)
            WriteConstant(tracker,reinterpret_cast<void**>(slots+i*4),input.constants[i],bigEndian);
        tracker.Pop();
    }
}
Relo::Chunk CompileShaderMaterial(const ShaderMaterialInput& input,bool bigEndian) {
    if(sizeof(void*)!=4)
        throw std::logic_error("W3X shader tracker serialization requires an x86 host");
    Relo::Tracker tracker;
    unsigned char* root=nullptr;
    tracker.Push(reinterpret_cast<void**>(&root),28,1);
    WriteShaderMaterial(tracker,root,input,bigEndian);
    tracker.Pop();
    return tracker.MakeRelocatable(bigEndian,0);
}
}}
