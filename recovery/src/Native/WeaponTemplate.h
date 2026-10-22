#pragma once
#include "Relocation.h"
#include "ObjectFilter.h"
#include <map>
#include <string>
namespace Recovered { namespace Native {
enum class WeaponValue { Float, Angle, Time, Percentage, Integer, Boolean, Enum, Bits, String, Reference, ReferencePointer, StatusPointer, AssetId, Absent };
enum class WeaponToken { Member, Pointer, List };
struct WeaponField {
    const char* name;
    const char* fallback;
    std::uint32_t offset, nameHash, typeHash;
    WeaponValue value;
    WeaponToken token;
    const char* enumGroup;
};
const WeaponField* WeaponFields();
unsigned WeaponFieldCount();
using WeaponOptionalInput = std::map<std::string, std::string>;
struct WeaponDurationInput { std::string minSeconds, maxSeconds; };
Relo::Chunk CompileWeapon(const std::map<std::string, std::string>& attributes,
    const std::map<std::string, WeaponDurationInput>& durations,
    const std::map<std::string, WeaponOptionalInput>& optionalBlocks,
    const std::vector<std::string>& incompatibleModifiers,
    const std::map<std::string, ObjectFilterInput>& filters, bool bigEndian);
}}


