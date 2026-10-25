#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlvisualsystemsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlvisualsystemsNode> children;};
unsigned int XmlvisualsystemsTypeId(const std::string& name);
Relo::Chunk CompileXmlvisualsystems(const XmlvisualsystemsNode& input,bool bigEndian);
}}
