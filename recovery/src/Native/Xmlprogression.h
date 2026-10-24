#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlprogressionNode { std::string name,text; std::map<std::string,std::string> attributes; std::vector<XmlprogressionNode> children; };
Relo::Chunk CompileXmlprogression(const XmlprogressionNode& input,bool bigEndian);
unsigned int XmlprogressionTypeId(const std::string& name);
}}

