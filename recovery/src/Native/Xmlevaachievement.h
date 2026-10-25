#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlevaachievementNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlevaachievementNode> children;};
unsigned int XmlevaachievementTypeId(const std::string& name);
Relo::Chunk CompileXmlevaachievement(const XmlevaachievementNode& input,bool bigEndian);
}}
