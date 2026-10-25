#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlhealthbarsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlhealthbarsNode> children;};
unsigned int XmlhealthbarsTypeId(const std::string& name);
Relo::Chunk CompileXmlhealthbars(const XmlhealthbarsNode& input,bool bigEndian);
}}
