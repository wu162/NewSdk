#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlmoviearchiveNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlmoviearchiveNode> children;};
unsigned int XmlmoviearchiveTypeId(const std::string& name);
Relo::Chunk CompileXmlmoviearchive(const XmlmoviearchiveNode& input,bool bigEndian);
}}
