#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmluiconfigNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmluiconfigNode> children;};
unsigned int XmluiconfigTypeId(const std::string& name);
Relo::Chunk CompileXmluiconfig(const XmluiconfigNode& input,bool bigEndian);
}}
