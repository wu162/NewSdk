#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlhotkeymapsNode {
    std::string name,value;
    std::map<std::string,std::string> attributes;
    std::vector<XmlhotkeymapsNode> children;
};
unsigned int XmlhotkeymapsTypeId(const std::string& name);
Relo::Chunk CompileXmlhotkeymaps(const XmlhotkeymapsNode& input,bool bigEndian);
}}
