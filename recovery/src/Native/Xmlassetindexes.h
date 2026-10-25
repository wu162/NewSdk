#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlassetindexesNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlassetindexesNode> children;};
unsigned int XmlassetindexesTypeId(const std::string& name);
Relo::Chunk CompileXmlassetindexes(const XmlassetindexesNode& input,bool bigEndian);
}}
