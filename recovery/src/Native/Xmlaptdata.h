#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlaptdataNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlaptdataNode> children;};
unsigned int XmlaptdataTypeId(const std::string& name);
Relo::Chunk CompileXmlaptdata(const XmlaptdataNode& input,bool bigEndian);
}}
