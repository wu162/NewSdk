#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlarmyopeningNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlarmyopeningNode> children;};
unsigned int XmlarmyopeningTypeId(const std::string& name);
Relo::Chunk CompileXmlarmyopening(const XmlarmyopeningNode& input,bool bigEndian);
}}
