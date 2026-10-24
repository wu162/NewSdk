#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlemotionsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlemotionsNode> children;};
unsigned int XmlemotionsTypeId(const std::string& name);
Relo::Chunk CompileXmlemotions(const XmlemotionsNode& input,bool bigEndian);
}}
