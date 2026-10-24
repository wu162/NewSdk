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
