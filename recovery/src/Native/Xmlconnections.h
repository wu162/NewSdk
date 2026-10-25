#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlconnectionsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlconnectionsNode> children;};
unsigned int XmlconnectionsTypeId(const std::string& name);
Relo::Chunk CompileXmlconnections(const XmlconnectionsNode& input,bool bigEndian);
}}
