#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlrenderdeviceNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlrenderdeviceNode> children;};
unsigned int XmlrenderdeviceTypeId(const std::string& name);
Relo::Chunk CompileXmlrenderdevice(const XmlrenderdeviceNode& input,bool bigEndian);
}}
