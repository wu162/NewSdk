#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlradarNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlradarNode> children;};
unsigned int XmlradarTypeId(const std::string& name);
Relo::Chunk CompileXmlradar(const XmlradarNode& input,bool bigEndian);
}}
