#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlgamemapNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlgamemapNode> children;};
unsigned int XmlgamemapTypeId(const std::string& name);
Relo::Chunk CompileXmlgamemap(const XmlgamemapNode& input,bool bigEndian);
}}
