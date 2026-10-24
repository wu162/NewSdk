#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlmusicscriptNode{std::string name,text;std::map<std::string,std::string> attributes;std::vector<XmlmusicscriptNode> children;};
Relo::Chunk CompileXmlmusicscript(const XmlmusicscriptNode& input,bool bigEndian);
unsigned int XmlmusicscriptTypeId(const std::string& name);
}}
