#pragma once
#include "Relocation.h"
#include <map>
#include <string>

namespace Recovered { namespace Native { namespace Xmllocomotor {
enum class Kind { Float, Percent, Time, Angle, Boolean, Signed, Surface,
    Behavior, Appearance, Formation, Model, Status, Reference, JetOptions };
struct Field { const char* name; unsigned offset; Kind kind; const char* fallback; };
extern const Field Fields[];
extern const unsigned FieldCount;
extern const Field JetFields[];
extern const unsigned JetFieldCount;
struct Input {
    std::map<std::string, std::string> attributes, jetAttributes;
    bool hasBounce = false, hasJet = false;
    std::string bounce;
};
Relo::Chunk Compile(const Input& input, bool bigEndian);
}}}
