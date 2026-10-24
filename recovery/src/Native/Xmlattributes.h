#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
typedef std::map<std::string, std::string> XmlattributesValues;
struct XmlattributesInput {
    XmlattributesValues attributes;
    std::vector<XmlattributesValues> modifiers;
    bool hasDependency = false;
    XmlattributesValues dependency;
    bool hasFilter = false;
    XmlattributesValues filter;
    std::vector<std::string> filterLists[2];
    std::vector<std::string> dependencyLists[3];
};
Relo::Chunk CompileXmlattributesLogicCommandSet(const std::vector<std::string>& commands, bool bigEndian);
Relo::Chunk CompileXmlattributesAttributeModifier(const XmlattributesInput& input, bool bigEndian);
Relo::Chunk CompileXmlattributesUpgradeTemplate(const XmlattributesInput& input, bool bigEndian);
}}
