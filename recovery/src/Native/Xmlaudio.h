#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlaudioNode { std::string name,text;std::map<std::string,std::string> attributes;std::vector<XmlaudioNode> children; };
Relo::Chunk CompileXmlaudio(const XmlaudioNode& input,bool bigEndian);
unsigned int XmlaudioTypeId(const std::string& name);
}}
