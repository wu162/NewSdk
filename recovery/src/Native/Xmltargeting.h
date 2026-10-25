#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmltargetingNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmltargetingNode> children;};
unsigned int XmltargetingTypeId(const std::string& name);
Relo::Chunk CompileXmltargeting(const XmltargetingNode& input,bool bigEndian);
}}
