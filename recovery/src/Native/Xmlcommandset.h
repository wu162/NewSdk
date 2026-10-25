#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlcommandsetNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlcommandsetNode> children;};
unsigned int XmlcommandsetTypeId(const std::string& name);
Relo::Chunk CompileXmlcommandset(const XmlcommandsetNode& input,bool bigEndian);
}}
