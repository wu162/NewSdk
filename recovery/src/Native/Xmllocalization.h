#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmllocalizationNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmllocalizationNode> children;};
unsigned int XmllocalizationTypeId(const std::string& name);
Relo::Chunk CompileXmllocalization(const XmllocalizationNode& input,bool bigEndian);
}}
