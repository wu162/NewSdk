#pragma once
#include "ObjectFilter.h"
#include <map>
#include <string>
namespace Recovered { namespace Native {
struct NuggetScalarInput {
    std::string scalar = "100%";
    bool hasFilter = false;
    ObjectFilterInput filter;
    std::vector<std::string> requiredUpgrades;
};
struct WeaponNuggetInput {
    std::uint32_t typeId = 0;
    std::map<std::string, std::string> attributes;
    bool hasSpecialFilter = false;
    ObjectFilterInput specialFilter;
    std::vector<std::string> requiredUpgrades, forbiddenUpgrades;
    std::vector<NuggetScalarInput> damageScalars;
};
struct WeaponNuggetList { std::uint32_t count; void** elements; };
void MarshalWeaponNuggets(const std::vector<WeaponNuggetInput>& input, WeaponNuggetList* list, Relo::Tracker& data);
void TokenizeWeaponNuggets(WeaponNuggetList* list, void** table, Relo::Tracker& tokens);
void EndianWeaponNuggets(WeaponNuggetList* list);
}}
