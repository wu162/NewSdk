#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlenvironmentNode {
    std::string name,value;
    std::map<std::string,std::string> attributes;
    std::vector<XmlenvironmentNode> children;
};
unsigned int XmlenvironmentTypeId(const std::string& name);
Relo::Chunk CompileXmlenvironment(const XmlenvironmentNode& input,bool bigEndian);
}}
