#pragma once
#include "Relocation.h"
#include "ScalarParsing.h"
#include <string>
namespace Recovered { namespace Native {
struct XmleffectsFXListInput {
    std::string playEvenIfShrouded, tailorable, cullTracking, cullTrackingMin, cullTrackingMax;
    bool hasCullTracking = false, hasCullTrackingMin = false, hasCullTrackingMax = false;
};
Relo::Chunk CompileXmleffectsFXList(const XmleffectsFXListInput& input, bool bigEndian);
}}
