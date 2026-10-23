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
struct VeteranProjectileInput { std::map<std::string, std::string> attributes; };
struct WeaponNuggetInput {
    std::uint32_t typeId = 0;
    bool hasAttackOffset = false, hasBarrel = false;
    std::map<std::string, std::string> attackOffset;
    std::string barrel;
    std::vector<VeteranProjectileInput> veterans;
    std::map<std::string, std::string> attributes;
    bool hasKillFilter = false, hasColor = false;
    ObjectFilterInput killFilter;
    std::map<std::string, std::string> color;
    bool hasSpawnTemplate = false, hasTargetFilter = false, hasDamageFilter = false;
    std::string spawnTemplate;
    ObjectFilterInput targetFilter, damageFilter;
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


