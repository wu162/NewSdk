#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlbuildeconomyNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlbuildeconomyNode> children;};
unsigned int XmlbuildeconomyTypeId(const std::string& name);
Relo::Chunk CompileXmlbuildeconomy(const XmlbuildeconomyNode& input,bool bigEndian);
}}
