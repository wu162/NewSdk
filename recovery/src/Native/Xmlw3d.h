#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct Xmlw3dNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<Xmlw3dNode> children;};
unsigned int Xmlw3dTypeId(const std::string& name);
Relo::Chunk CompileXmlw3d(const Xmlw3dNode& input,bool bigEndian);
}}
