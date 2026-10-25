#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlchatcolorsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlchatcolorsNode> children;};
unsigned int XmlchatcolorsTypeId(const std::string& name);
Relo::Chunk CompileXmlchatcolors(const XmlchatcolorsNode& input,bool bigEndian);
}}
