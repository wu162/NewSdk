#pragma once
#include "Relocation.h"
#include <string>
#include <map>
#include <vector>
namespace Recovered { namespace Native {
struct XmlpowersNode {
    std::string name, text;
    std::map<std::string,std::string> attributes;
    std::vector<XmlpowersNode> children;
};
Relo::Chunk CompileXmlpowers(const XmlpowersNode& input, bool bigEndian);
}}
