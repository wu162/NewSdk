#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlmouseNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlmouseNode> children;};
unsigned int XmlmouseTypeId(const std::string& name);
Relo::Chunk CompileXmlmouse(const XmlmouseNode& input,bool bigEndian);
}}
