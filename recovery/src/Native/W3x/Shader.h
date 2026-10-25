#pragma once
#include "../Relocation.h"
#include <cstdint>
#include <string>
#include <vector>
namespace Recovered { namespace W3x {
struct ShaderConstantInput {
    std::uint32_t typeId=0;
    bool hasName=false;
    std::string name;
    bool booleanValue=false;
    std::int32_t integerValue=0;
    // BAB-resolved texture text; only a backslash followed by an atoi index
    // creates an import. Missing/empty/unresolved text creates no import.
    std::string textureValue;
    std::vector<float> floatValues;
};
struct ShaderMaterialInput {
    bool hasShaderName=false;
    std::string shaderName;
    // Getter default for an existing FXShader node is present empty string.
    std::string techniqueName;
    std::uint8_t techniqueIndex=0;
    std::vector<ShaderConstantInput> constants;
};
// Writes an already allocated, initially zero 28-byte embedded material.
// Call only when FXShader XML node exists (absent node has no default strings).
// Allocates child strings/constants using the caller's active Tracker block.
// x86 only, matching the shared Tracker pointer-slot ABI.
void WriteShaderMaterial(Relo::Tracker& tracker,unsigned char* destination,
                         const ShaderMaterialInput& input,bool bigEndian);
// Isolated material root for original Marshal/EndianCorrect three-buffer tests.
Relo::Chunk CompileShaderMaterial(const ShaderMaterialInput& input,bool bigEndian);
}}
