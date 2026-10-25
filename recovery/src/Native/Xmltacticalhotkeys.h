#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmltacticalhotkeysNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmltacticalhotkeysNode> children;};
unsigned int XmltacticalhotkeysTypeId(const std::string& name);
Relo::Chunk CompileXmltacticalhotkeys(const XmltacticalhotkeysNode& input,bool bigEndian);
}}
