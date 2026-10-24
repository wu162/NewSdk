// Handoff snapshot: FXParticle root/input draft, excluded from active FXList build.
#if 0
#pragma once
#include "Relocation.h"
#include "ScalarParsing.h"
#include <string>
#include <vector>
#include "ResourceMarshaling.h"
namespace Recovered { namespace Native {
enum XmleffectsFieldKind { EffectFloat, EffectUnsigned, EffectHash, EffectReference, EffectString, EffectTime, EffectAngle, EffectBoolean, EffectEnum, EffectPercentage };
struct XmleffectsFieldInput {
    unsigned int offset;
    XmleffectsFieldKind kind;
    std::string text, enumType;
};
struct XmleffectsNuggetInput {
    unsigned int typeId = 0, size = 0;
    std::vector<XmleffectsFieldInput> fields;
    std::vector<std::string> disabled;
    bool hasRange = false;
    std::string rangeLow = "SCORCH_1", rangeHigh = "SCORCH_4";
};
void MarshalXmleffectsNugget(Relo::Tracker& tracker, void** pointer, const XmleffectsNuggetInput& input, bool bigEndian);
struct XmleffectsFXListInput {
    std::string playEvenIfShrouded, tailorable, cullTracking, cullTrackingMin, cullTrackingMax;
    std::vector<XmleffectsNuggetInput> nuggets;
    bool hasCullTracking = false, hasCullTrackingMin = false, hasCullTrackingMax = false;
};
Relo::Chunk CompileXmleffectsFXList(const XmleffectsFXListInput& input, bool bigEndian);
}}

namespace Recovered { namespace Native {
struct XmleffectsRandomInput {
    bool present = false;
    std::string type = "UNIFORM", low, high;
};
struct XmleffectsAlphaInput {
    XmleffectsRandomInput value;
    std::string relativeAge, frame = "1000";
};
struct XmleffectsColorInput {
    std::string relativeAge, frame = "1000", rgb[3];
};
struct XmleffectsModuleRandom { unsigned int offset; XmleffectsRandomInput input; };
struct XmleffectsModuleInput {
    bool present = false;
    unsigned int typeId = 0, size = 0;
    std::vector<XmleffectsFieldInput> fields;
    std::vector<XmleffectsModuleRandom> random;
};
struct XmleffectsParticleInput {
    std::string priority = "ULTRA_HIGH_ONLY", shader = "ADDITIVE", type = "PARTICLE";
    std::string references[3], typedIds[2], lifetime = "0", sortLevel = "0", flags[5];
    std::string offset[3];
    XmleffectsRandomInput random[6], colorScale;
    XmleffectsModuleInput modules[2]; // Volume@148, Velocity@152
    bool hasAlphas = false, hasColors = false, hasWind = false;
    std::vector<XmleffectsAlphaInput> alphas;
    std::vector<XmleffectsColorInput> colors;
    std::string houseColor = "false", saturation = "1.0", wind[12];
};
Relo::Chunk CompileXmleffectsParticle(const XmleffectsParticleInput& input, bool bigEndian);
}}

#endif
