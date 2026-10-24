#include "Xmllocomotor.h"
#include "ScalarParsing.h"
#include "EnumBitset.h"
#include "ResourceMarshaling.h"
#include <cstring>
#include <stdexcept>
#ifdef _MANAGED
#pragma managed(push, off)
#endif

namespace Recovered { namespace Native { namespace Xmllocomotor {
static_assert(sizeof(void*) == 4, "LocomotorTemplate requires x86");
const Field Fields[] = {
#include "XmllocomotorFields.inc"
};
const unsigned FieldCount = sizeof(Fields)/sizeof(Fields[0]);
const Field JetFields[] = {
    { "Options", 0, Kind::JetOptions, nullptr },
    { "AttackPathStartRunDistance", 4, Kind::Float, nullptr },
    { "AttackPathClimbDistance", 8, Kind::Float, nullptr },
    { "AttackPathDiveDistanceStart", 12, Kind::Float, nullptr },
    { "AttackPathDiveDistanceEnd", 16, Kind::Float, nullptr },
};
const unsigned JetFieldCount = sizeof(JetFields)/sizeof(JetFields[0]);
namespace {
using Relo::U32;
void Swap(void* p) {
    U32 v; std::memcpy(&v, p, 4);
    v = (v << 24) | ((v << 8) & 0x00ff0000u) | ((v >> 8) & 0x0000ff00u) | (v >> 24);
    std::memcpy(p, &v, 4);
}
template<class T> void Write(void* p, T value) { std::memcpy(p, &value, sizeof(value)); }
float Unit(const std::string& text, bool time) {
    // Original DataInitializers.Initialize, tokens 0x0600001A/1B:
    // ordered strstr, truncate at the first matching unit, float -> double
    // multiply by a float constant -> float; no case folding.
    const char* raw = text.c_str();
    const char* suffix = std::strstr(raw, time ? "ms" : "r");
    float factor = time ? 0.001f : 1.0f;
    if (!suffix) {
        suffix = std::strstr(raw, time ? "s" : "d");
        factor = suffix ? (time ? 1.0f : 0.01745329238474369f) : 1.0f;
    }
    const float value = ParseFloat(suffix ? std::string(raw, suffix) : std::string(raw));
    return static_cast<float>(static_cast<double>(value) * static_cast<double>(factor));
}
const char* AppearanceNames[] = { "TWO_LEGS", "FOUR_WHEELS", "HOVER", "HOVER_TANK", "WINGS", "HORDE", "HUGE_TWO_LEGS", "TREADS", "SHIP", "OTHER" };
const char* LocoZNames[] = { "NO_MOTIVE_FORCE", "SEA_LEVEL", "SURFACE_RELATIVE_HEIGHT_ALLOW_ZERO_HEIGHT", "SURFACE_RELATIVE_HEIGHT", "ABSOLUTE_HEIGHT", "FIXED_SURFACE_RELATIVE_HEIGHT", "FIXED_ABSOLUTE_HEIGHT", "RELATIVE_TO_GROUND_AND_BUILDINGS", "SMOOTH_RELATIVE_TO_HIGHEST_LAYER", "FLOATING_Z", "SCALING_WALLS", "DO_NOT_MODIFY_HEIGHT", "SEA_LEVEL_SMOOTH_Z" };
const char* LocoFNames[] = { "NO_FORMATION", "CAVALRY_1", "CAVALRY_2", "CAVALRY_3", "MELEE_1", "MELEE_2", "MELEE_3", "RANGED_1", "RANGED_2", "RANGED_3", "ARTILLERY_1", "ARTILLERY_2", "ARTILLERY_3" };
template<std::size_t N> U32 Enum(const std::string& text, const char* const (&names)[N], U32 fallback) {
    for (U32 i = 0; i < N; ++i) if (text == names[i]) return i;
    return fallback;
}
const EnumNameValue surfaceNames[] = {
    { "GROUND", 0 },
    { "WATER", 1 },
    { "CLIFF", 2 },
    { "AIR", 3 },
    { "RUBBLE", 4 },
    { "OBSTACLE", 5 },
    { "IMPASSABLE", 6 },
    { "DEEP_WATER", 7 },
    { "WALL_RAILING", 8 },
    { "CRUSHABLE_OBSTACLE", 9 },
    { "CRUSHABLE_WALL", 10 },
};
const EnumNameValue jetNames[] = { { "NO_CIRCLE_WHILE_USING_SPECIALPOWER", 0 } };
void Apply(const std::map<std::string,std::string>& values, const Field* fields,
    unsigned count, unsigned char* root, Relo::Tracker& data) {
    for (unsigned i = 0; i < count; ++i) {
        const Field& field = fields[i];
        auto at = values.find(field.name);
        if (at == values.end() && !field.fallback) continue;
        const std::string text = at == values.end() ? std::string(field.fallback) : at->second;
        void* p = root + field.offset;
        switch (field.kind) {
        case Kind::Float: Write(p, ParseFloat(text)); break;
        case Kind::Percent: Write(p, ParsePercentage(text)); break;
        case Kind::Time: Write(p, Unit(text, true)); break;
        case Kind::Angle: Write(p, Unit(text, false)); break;
        case Kind::Boolean: Write(p, static_cast<unsigned char>(ParseBoolean(text))); break;
        case Kind::Signed: Write(p, ParseSigned(text)); break;
        case Kind::Surface:
            MarshalEnumBitset(text, surfaceNames, sizeof(surfaceNames)/sizeof(surfaceNames[0]), 3, 11, static_cast<U32*>(p)); break;
        case Kind::JetOptions: MarshalEnumBitset(text, jetNames, 1, 0, 1, static_cast<U32*>(p)); break;
        case Kind::Behavior: Write(p, Enum(text, LocoZNames, 4)); break;
        case Kind::Appearance: Write(p, Enum(text, AppearanceNames, 1)); break;
        case Kind::Formation: Write(p, Enum(text, LocoFNames, 10)); break;
        case Kind::Reference: MarshalReference(data, p, text); break;
        case Kind::Model: case Kind::Status: {
            const bool model = field.kind == Kind::Model;
            U32* words = static_cast<U32*>(data.Push(reinterpret_cast<void**>(p), model ? 60 : 28, 1));
            if (model) MarshalModelCondition(text, words); else MarshalObjectStatus(text, words);
            data.Pop(); break;
        }
        default: throw std::logic_error("Unsupported locomotor field kind");
        }
    }
}
void Endian(unsigned char* root, const Field* fields, unsigned count) {
    for (unsigned i = 0; i < count; ++i) {
        const Field& f = fields[i];
        if (f.kind == Kind::Boolean || f.kind == Kind::Reference) continue;
        if (f.kind == Kind::Model || f.kind == Kind::Status) {
            U32* words = *reinterpret_cast<U32**>(root + f.offset);
            if (words) for (unsigned j = 0; j < (f.kind == Kind::Model ? 15u : 7u); ++j) Swap(words+j);
        } else Swap(root+f.offset);
    }
}
}
Relo::Chunk Compile(const Input& input, bool bigEndian) {
    Relo::Tracker data;
    unsigned char* root = nullptr;
    data.Push(reinterpret_cast<void**>(&root), 396, 1);
    Apply(input.attributes, Fields, FieldCount, root, data);
    if (input.hasBounce) {
        void* reference = data.Push(reinterpret_cast<void**>(root+360), 4, 1);
        MarshalReference(data, reference, input.bounce);
        data.Pop();
    }
    unsigned char* jet = nullptr;
    if (input.hasJet) {
        jet = static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+364), 20, 1));
        Apply(input.jetAttributes, JetFields, JetFieldCount, jet, data);
        data.Pop();
    }
    if (bigEndian) {
        Endian(root, Fields, FieldCount);
        if (jet) Endian(jet, JetFields, JetFieldCount);
    }
    // XmlCompiler FinalizeTracker is one MakeRelocatable(endian, 0).
    // Keep imports: both direct FX and nested Bounce references can populate it.
    return data.MakeRelocatable(bigEndian, 0);
}
}}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
