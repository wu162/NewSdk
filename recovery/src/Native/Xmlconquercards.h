#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlconquercardsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlconquercardsNode> children;};
unsigned int XmlconquercardsTypeId(const std::string& name);
Relo::Chunk CompileXmlconquercards(const XmlconquercardsNode& input,bool bigEndian);
}}
