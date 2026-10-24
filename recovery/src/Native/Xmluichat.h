#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmluichatNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmluichatNode> children;};
unsigned int XmluichatTypeId(const std::string& name);
Relo::Chunk CompileXmluichat(const XmluichatNode& input,bool bigEndian);
}}
