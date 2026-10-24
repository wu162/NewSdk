#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlredalertNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlredalertNode> children;};
unsigned int XmlredalertTypeId(const std::string& name);
Relo::Chunk CompileXmlredalert(const XmlredalertNode& input,bool bigEndian);
}}
