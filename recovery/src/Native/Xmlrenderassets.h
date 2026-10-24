#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlrenderassetsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlrenderassetsNode> children;};
unsigned int XmlrenderassetsTypeId(const std::string& name);
Relo::Chunk CompileXmlrenderassets(const XmlrenderassetsNode& input,bool bigEndian);
}}
