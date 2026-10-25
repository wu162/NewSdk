#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlgamescriptsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlgamescriptsNode> children;};
unsigned int XmlgamescriptsTypeId(const std::string& name);
Relo::Chunk CompileXmlgamescripts(const XmlgamescriptsNode& input,bool bigEndian);
}}
