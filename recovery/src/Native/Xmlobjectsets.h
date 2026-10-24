#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlobjectsetsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlobjectsetsNode> children;};
unsigned int XmlobjectsetsTypeId(const std::string& name);
Relo::Chunk CompileXmlobjectsets(const XmlobjectsetsNode& input,bool bigEndian);
}}
