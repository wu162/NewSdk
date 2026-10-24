#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmldamagefxNode {
    std::string name,value;
    std::map<std::string,std::string> attributes;
    std::vector<XmldamagefxNode> children;
};
unsigned int XmldamagefxTypeId(const std::string& name);
Relo::Chunk CompileXmldamagefx(const XmldamagefxNode& input,bool bigEndian);
}}
