#pragma once
#include "Relocation.h"
#include "ScalarParsing.h"
#include <string>
#include <vector>
#include "ResourceMarshaling.h"
#include "ObjectFilter.h"
namespace Recovered { namespace Native {
enum XmleffectsFieldKind { EffectFloat, EffectUnsigned, EffectHash, EffectReference, EffectString, EffectTime, EffectAngle, EffectBoolean, EffectEnum, EffectPercentage, EffectSigned };
struct XmleffectsFieldInput {
    unsigned int offset;
    XmleffectsFieldKind kind;
    std::string text, enumType;
};
struct XmleffectsNuggetInput {
    unsigned int typeId = 0, size = 0;
    std::vector<XmleffectsFieldInput> fields;
    std::vector<std::string> disabled;
    bool hasModelConditions[4] = {false,false,false,false};
    std::string modelConditions[4];
    bool hasFilters[2] = {false,false};
    ObjectFilterInput filters[2]; // Secondary @24, Source @28, in allocation order.
    bool hasTargetOffset = false, hasPackedColor = false;
    std::string targetOffset[3], rgba[4];
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

