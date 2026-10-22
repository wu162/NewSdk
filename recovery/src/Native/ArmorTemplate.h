#pragma once
#include "Relocation.h"
#include "ScalarParsing.h"
#include <string>
#include <vector>

namespace Recovered { namespace Native {
struct ArmorEntryInput {
    bool hasDamage = false;
    std::string damage, percent;
    bool hasPercent = false;
};
struct ArmorInput {
    std::string percentages[5];
    std::vector<ArmorEntryInput> entries;
};
// The order is the shipped SDK DamageDef.xsd order, validated against every
// original enum result. Unknown text is 38; a missing attribute remains zero.
unsigned int ParseDamageType(const std::string& text);
Relo::Chunk CompileArmor(const ArmorInput& input, bool bigEndian);
}}
