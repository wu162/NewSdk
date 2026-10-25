#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmlcampaignNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmlcampaignNode> children;};
unsigned int XmlcampaignTypeId(const std::string& name);
Relo::Chunk CompileXmlcampaign(const XmlcampaignNode& input,bool bigEndian);
}}
