#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <vector>
namespace Recovered { namespace Native {
struct XmldynamicsNode {std::string name,value;std::map<std::string,std::string> attributes;std::vector<XmldynamicsNode> children;};
unsigned int XmldynamicsTypeId(const std::string& name);
Relo::Chunk CompileXmldynamics(const XmldynamicsNode& input,bool bigEndian);
}}
