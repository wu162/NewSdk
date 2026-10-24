#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlplayersNode {
    std::string name,value;
    std::map<std::string,std::string> attributes;
    std::vector<XmlplayersNode> children;
};
Relo::Chunk CompileXmlplayers(const XmlplayersNode& node,bool bigEndian);
unsigned XmlplayersTypeId(const std::string& name);
}}
