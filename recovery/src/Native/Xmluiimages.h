#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmluiimagesNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmluiimagesNode> children;};
unsigned int XmluiimagesTypeId(const std::string& name);
Relo::Chunk CompileXmluiimages(const XmluiimagesNode& input,bool bigEndian);
}}
