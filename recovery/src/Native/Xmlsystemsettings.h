#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlsystemsettingsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlsystemsettingsNode> children;};
unsigned int XmlsystemsettingsTypeId(const std::string& name);
Relo::Chunk CompileXmlsystemsettings(const XmlsystemsettingsNode& input,bool bigEndian);
}}
