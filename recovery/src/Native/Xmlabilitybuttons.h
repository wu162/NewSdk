#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlabilitybuttonsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlabilitybuttonsNode> children;};
unsigned int XmlabilitybuttonsTypeId(const std::string& name);
Relo::Chunk CompileXmlabilitybuttons(const XmlabilitybuttonsNode& input,bool bigEndian);
}}
