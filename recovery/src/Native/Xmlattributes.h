#pragma once
#include "Relocation.h"
#include <string>
#include <vector>
namespace Recovered { namespace Native {
Relo::Chunk CompileXmlattributesLogicCommandSet(const std::vector<std::string>& commands, bool bigEndian);
}}
