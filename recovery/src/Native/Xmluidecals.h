#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmluidecalsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmluidecalsNode> children;};
unsigned int XmluidecalsTypeId(const std::string& name);
Relo::Chunk CompileXmluidecals(const XmluidecalsNode& input,bool bigEndian);
}}
