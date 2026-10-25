#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlmapmetadataNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlmapmetadataNode> children;};
unsigned int XmlmapmetadataTypeId(const std::string& name);
Relo::Chunk CompileXmlmapmetadata(const XmlmapmetadataNode& input,bool bigEndian);
}}
