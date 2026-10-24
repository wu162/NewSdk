#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlpathmusicNode{std::string name,text;std::map<std::string,std::string> attributes;std::vector<XmlpathmusicNode> children;};
Relo::Chunk CompileXmlpathmusic(const XmlpathmusicNode& input,bool bigEndian);
unsigned int XmlpathmusicTypeId(const std::string& name);
}}
