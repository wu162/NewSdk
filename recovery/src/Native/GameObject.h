#pragma once
#include "Relocation.h"
#include <map>
#include <string>
namespace Recovered { namespace Native {
enum class GameObjectValue { Float, Signed, Unsigned, Short, Byte, Boolean, Time, Angle, String, Enum, AssetId, Reference, DurationDefault };
struct GameObjectField { const char* name; unsigned offset; GameObjectValue kind; const char* fallback; int defaultValue; };
const GameObjectField* GameObjectFields();
unsigned GameObjectFieldCount();
// Root-only component. Unsupported attributes and every child block are rejected.
Relo::Chunk CompileGameObject(const std::map<std::string,std::string>& attributes, bool bigEndian);
}}
