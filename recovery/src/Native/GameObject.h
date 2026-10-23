#pragma once
#include "Relocation.h"
#include <map>
#include <string>
#include <stdexcept>
namespace Recovered { namespace Native {
class GameObjectFormatError : public std::runtime_error {
public:
    using std::runtime_error::runtime_error;
};
enum class GameObjectValue { Float, Signed, Unsigned, Short, Byte, Boolean, Time, Angle, String, StringList, Bits, Enum, AssetId, Reference, DurationDefault };
struct GameObjectField { const char* name; unsigned offset; GameObjectValue kind; const char* fallback; int defaultValue; };
const GameObjectField* GameObjectFields();
unsigned GameObjectFieldCount();
bool GameObjectLeafAttributeSupported(const std::string& name, const std::string& attribute);
bool GameObjectAIAttributeSupported(const std::string& name, const std::string& attribute);
unsigned GameObjectSoundSelectorKind(const std::string& typeId);
bool GameObjectModuleAttributeSupported(const std::string& typeId, const std::string& attribute);
bool GameObjectModuleNestedAttributeSupported(const std::string& typeId, const std::string& name, const std::string& attribute);
bool GameObjectModuleNestedSupported(const std::string& typeId, const std::string& name);
bool GameObjectModuleChildSupported(const std::string& parent, const std::string& child);
struct GameObjectChild {
    std::string name, text;
    std::map<std::string,std::string> attributes;
    std::vector<GameObjectChild> children;
};
// Restricted component; unsupported attributes and child blocks are rejected.
Relo::Chunk CompileGameObject(const std::map<std::string,std::string>& attributes, const std::vector<GameObjectChild>& children, bool bigEndian);
}}



