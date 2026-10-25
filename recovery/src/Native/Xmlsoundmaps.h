#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlsoundmapsNode { std::string name,value; std::map<std::string,std::string> attributes; std::vector<XmlsoundmapsNode> children; };
unsigned int XmlsoundmapsTypeId(const std::string& name);
Relo::Chunk CompileXmlsoundmaps(const XmlsoundmapsNode& input,bool bigEndian);
}}
