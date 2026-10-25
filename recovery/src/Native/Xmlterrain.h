#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlterrainNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlterrainNode> children;};
unsigned int XmlterrainTypeId(const std::string& name);
Relo::Chunk CompileXmlterrain(const XmlterrainNode& input,bool bigEndian);
}}
