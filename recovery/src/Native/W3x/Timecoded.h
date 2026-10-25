#pragma once
#include <array>
#include <cstdint>
#include <vector>
namespace Recovered { namespace W3x {
struct TimecodedFrame {
    std::uint32_t index;
    std::vector<float> values;
    bool binaryMove;
};
struct TimecodedChannel {
    std::uint32_t type;
    std::uint16_t pivot;
    std::vector<TimecodedFrame> frames;
};
struct TimecodedEncoded {
    // Root pointer slots +24 (time codes) and +32 (values) remain zero.
    // Tracker allocation order: root36, values elementSize4, codes elementSize2.
    std::array<unsigned char,36> root;
    std::vector<unsigned char> values;
    std::vector<unsigned char> codes;
};
TimecodedChannel CompressTimecoded(std::uint32_t type, std::uint16_t pivot,
    const std::vector<TimecodedFrame>& frames, float maxTranslationError,
    float maxRotationError, float maxVisibilityError, float forceReductionRate);
std::uint32_t EstimateTimecodedSize(const TimecodedChannel& channel);
TimecodedEncoded EncodeTimecoded(const TimecodedChannel& channel, bool bigEndian);
// Original CLR x86 conv.u4 reduction target, including negative, overflowing
// and nonfinite rates. Exposed for direct original-runtime boundary probes.
std::uint32_t TimecodedReductionLimit(std::uint32_t sourceCount,float forceRate);
}}
