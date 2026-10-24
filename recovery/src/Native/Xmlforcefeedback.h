#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlforcefeedbackNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlforcefeedbackNode> children;};
unsigned int XmlforcefeedbackTypeId(const std::string& name);
Relo::Chunk CompileXmlforcefeedback(const XmlforcefeedbackNode& input,bool bigEndian);
}}
