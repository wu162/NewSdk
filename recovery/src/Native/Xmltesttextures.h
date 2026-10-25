#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmltesttexturesNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmltesttexturesNode> children;};
unsigned int XmltesttexturesTypeId(const std::string& name);
Relo::Chunk CompileXmltesttextures(const XmltesttexturesNode& input,bool bigEndian);
}}
