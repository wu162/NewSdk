#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmluisettingsNode {
    std::string name,value;
    std::map<std::string,std::string> attributes;
    std::vector<XmluisettingsNode> children;
};
unsigned int XmluisettingsTypeId(const std::string& name);
Relo::Chunk CompileXmluisettings(const XmluisettingsNode& input,bool bigEndian);
}}

