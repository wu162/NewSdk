#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmluitacticalNode {
    std::string name,value;
    std::map<std::string,std::string> attributes;
    std::vector<XmluitacticalNode> children;
};
unsigned int XmluitacticalTypeId(const std::string& name);
Relo::Chunk CompileXmluitactical(const XmluitacticalNode& input,bool bigEndian);
}}

