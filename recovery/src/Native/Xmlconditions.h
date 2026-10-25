#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlconditionsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlconditionsNode> children;};
unsigned int XmlconditionsTypeId(const std::string& name);
Relo::Chunk CompileXmlconditions(const XmlconditionsNode& input,bool bigEndian);
}}
