#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlcontentNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlcontentNode> children;};
unsigned int XmlcontentTypeId(const std::string& name);
Relo::Chunk CompileXmlcontent(const XmlcontentNode& input,bool bigEndian);
}}
