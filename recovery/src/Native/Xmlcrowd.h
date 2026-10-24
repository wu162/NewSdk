#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlcrowdNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlcrowdNode> children;};
unsigned int XmlcrowdTypeId(const std::string& name);
Relo::Chunk CompileXmlcrowd(const XmlcrowdNode& input,bool bigEndian);
}}
