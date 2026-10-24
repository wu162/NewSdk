#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlprojectilepathNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlprojectilepathNode> children;};
unsigned int XmlprojectilepathTypeId(const std::string& name);
Relo::Chunk CompileXmlprojectilepath(const XmlprojectilepathNode& input,bool bigEndian);
}}
