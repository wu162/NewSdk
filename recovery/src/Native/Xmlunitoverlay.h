#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlunitoverlayNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlunitoverlayNode> children;};
unsigned int XmlunitoverlayTypeId(const std::string& name);
Relo::Chunk CompileXmlunitoverlay(const XmlunitoverlayNode& input,bool bigEndian);
}}
