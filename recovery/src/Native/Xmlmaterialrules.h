#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlmaterialrulesNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlmaterialrulesNode> children;};
unsigned int XmlmaterialrulesTypeId(const std::string& name);
Relo::Chunk CompileXmlmaterialrules(const XmlmaterialrulesNode& input,bool bigEndian);
}}
