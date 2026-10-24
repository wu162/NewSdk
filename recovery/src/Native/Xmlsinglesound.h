#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlsinglesoundNode{std::string name,text;std::map<std::string,std::string> attributes;std::vector<XmlsinglesoundNode> children;};
Relo::Chunk CompileXmlsinglesound(const XmlsinglesoundNode& input,bool bigEndian);
unsigned int XmlsinglesoundTypeId(const std::string& name);
}}
