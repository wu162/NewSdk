#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlaiNode {
    std::string name, value;
    std::map<std::string,std::string> attributes;
    std::vector<XmlaiNode> children;
};
Relo::Chunk CompileXmlai(const XmlaiNode& node, bool bigEndian);
unsigned int XmlaiTypeId(const std::string& name);
}}
