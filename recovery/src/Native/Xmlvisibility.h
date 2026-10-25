#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlvisibilityNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlvisibilityNode> children;};
unsigned int XmlvisibilityTypeId(const std::string& name);
Relo::Chunk CompileXmlvisibility(const XmlvisibilityNode& input,bool bigEndian);
}}
