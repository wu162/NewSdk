#pragma once
#include "Relocation.h"
#include <map>
#include <string>
namespace Recovered { namespace Native {
enum class GameObjectValue { Float, Signed, Unsigned, Short, Byte, Boolean, Time, Angle, String, StringList, Bits, Enum, AssetId, Reference, DurationDefault };
struct GameObjectField { const char* name; unsigned offset; GameObjectValue kind; const char* fallback; int defaultValue; };
const GameObjectField* GameObjectFields();
unsigned GameObjectFieldCount();
bool GameObjectLeafAttributeSupported(const std::string& name, const std::string& attribute);
struct GameObjectChild {
    std::string name, text;
    std::map<std::string,std::string> attributes;
};
// Restricted component; unsupported attributes and child blocks are rejected.
Relo::Chunk CompileGameObject(const std::map<std::string,std::string>& attributes, const std::vector<GameObjectChild>& children, bool bigEndian);
}}

