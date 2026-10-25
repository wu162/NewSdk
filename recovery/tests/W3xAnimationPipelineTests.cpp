#include "../src/Native/W3x/AnimationPipeline.h"
#include <cassert>
#include <stdexcept>
#include <iostream>
using namespace Recovered::W3x;
template<class F> static void Reject(F f) { bool rejected = false; try { f(); } catch (const std::invalid_argument&) { rejected = true; } assert(rejected); }
int main() {
    AnimationInput input;
    input.numFrames = 5;
    input.frameRate = 30;
    input.hierarchy = 0x1234;
    AnimationInputChannel scalar;
    scalar.type = 4;
    scalar.pivot = 57;
    scalar.firstFrame = 2;
    AnimationInputFrame frame;
    frame.values.push_back(.25f);
    frame.binaryMove = true;
    scalar.frames.push_back(frame);
    input.channels.push_back(scalar);
    for (int platform = 0; platform < 3; ++platform) {
        AnimationPrepared result = PrepareAnimation(input, platform);
        assert(result.hierarchy == 0x1234 && result.numFrames == 5 && result.frameRate == 30);
        assert(result.bigEndian == (platform != 0));
        assert(result.settings.maxTranslationError == .002f && result.settings.maxRotationError == .003f);
        assert(result.settings.maxAdaptiveDeltaError == .001f);
        assert(result.channels[0].pivot == 57 && result.channels[0].type == 4);
        for (unsigned i = 0; i < 5; ++i) {
            assert(result.channels[0].frames[i].index == i);
            assert(result.channels[0].frames[i].values[0] == (i == 2 ? .25f : 1.f));
            assert(result.channels[0].frames[i].binaryMove == (i == 2));
        }
    }
    Reject([&] { PrepareAnimation(input, 99); });
    input.hasCompressionSettings = true;
    AnimationPrepared explicitSettings = PrepareAnimation(input, 99);
    assert(explicitSettings.bigEndian && explicitSettings.settings.maxTranslationError == .001f);
    assert(explicitSettings.settings.maxRotationError == .01f && explicitSettings.settings.maxAdaptiveDeltaError == .03f);
    input.settings.allowTimecoded = input.settings.allowAdaptiveDelta = false;
    input.settings.forceReductionRate = .25f;
    AnimationPrepared disabled = PrepareAnimation(input, 0);
    assert(disabled.settings.allowTimecoded && !disabled.settings.allowAdaptiveDelta);
    assert(disabled.settings.maxTranslationError == 0.f && disabled.settings.maxRotationError == 0.f && disabled.settings.maxVisibilityError == 0.f);
    assert(disabled.settings.forceReductionRate == 1.f && disabled.settings.maxAdaptiveDeltaError == .03f);
    for (unsigned type = 0; type < 3; ++type) {
        input.channels[0].type = type;
        assert(PrepareAnimation(input, 0).channels[0].frames[0].values[0] == 0.f);
    }
    input.channels[0].type = 3;
    Reject([&] { PrepareAnimation(input, 0); });
    input.channels[0].typeId = 4131577120u;
    input.channels[0].type = 4; // Original permits any usage for quaternion.
    input.channels[0].frames[0].values = { .1f, .2f, .3f, .4f };
    AnimationPrepared quaternion = PrepareAnimation(input, 0);
    assert(quaternion.channels[0].frames[0].values == std::vector<float>({0.f, 0.f, 0.f, 1.f}));
    assert(quaternion.channels[0].frames[2].values == input.channels[0].frames[0].values);
    input.channels[0].firstFrame = 5;
    Reject([&] { PrepareAnimation(input, 0); });
    input.channels[0].firstFrame = 0;
    input.channels[0].frames[0].values.clear();
    Reject([&] { PrepareAnimation(input, 0); });
    input.channels[0].frames.clear();
    input.channels[0].firstFrame = 0xffffffffu;
    assert(PrepareAnimation(input, 0).channels[0].frames.size() == 5);
    input.channels[0].typeId = 0;
    Reject([&] { PrepareAnimation(input, 0); });
    for (int d = 0; d < 2; ++d) for (int t = 0; t < 2; ++t) {
        assert(ChooseAdaptiveDelta(d != 0,t != 0,10,10) == (d != 0));
        assert(ChooseAdaptiveDelta(d != 0,t != 0,10,11) == (d && !t));
        assert(ChooseAdaptiveDelta(d != 0,t != 0,11,10) == (d != 0));
    }
    std::cout << "PASS W3X animation preparation and selection\n";
}
