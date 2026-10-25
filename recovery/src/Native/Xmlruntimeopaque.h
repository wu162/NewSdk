#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlruntimeopaqueNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlruntimeopaqueNode> children;};
unsigned int XmlruntimeopaqueTypeId(const std::string& name);
Relo::Chunk CompileXmlruntimeopaque(const XmlruntimeopaqueNode& input,bool bigEndian);
}}
