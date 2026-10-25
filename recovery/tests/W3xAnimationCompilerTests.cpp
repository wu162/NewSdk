#include "../src/Native/W3x/AnimationPipeline.h"
#include <cassert>
#include <iostream>
#include <stdexcept>
using namespace Recovered::W3x;
static void Word(std::vector<unsigned char>& bytes, unsigned at, std::uint32_t value, bool big) {
    for (unsigned i = 0; i < 4; ++i) bytes.at(at+i) = static_cast<unsigned char>(value >> (8*(big ? 3-i : i)));
}
static std::vector<unsigned char> Words(const std::vector<std::uint32_t>& values, bool big) {
    std::vector<unsigned char> out(values.size()*4);
    for (unsigned i = 0; i < values.size(); ++i) Word(out,i*4,values[i],big);
    return out;
}
int main() {
    if (sizeof(void*) != 4) {
        bool rejected=false;
        try { CompileAnimation(AnimationInput(), 0); } catch (const std::logic_error&) { rejected=true; }
        assert(rejected);
        std::cout << "PASS W3X animation compiler rejects non-x86 tracker host\n";
        return 0;
    }
    for (int platform = 0; platform < 3; ++platform) {
        for (unsigned hierarchy : {0u,7u}) {
        bool big = platform != 0;
        AnimationInput empty;
        empty.numFrames = 1; empty.frameRate = 30; empty.hierarchy = hierarchy;
        Recovered::Relo::Chunk noChannels = CompileAnimation(empty, platform);
        std::vector<unsigned char> expected = Words({0,hierarchy+1,1,30,0,24},big);
        assert(noChannels.instance == expected);
        assert(noChannels.relocations == Words({20,0xffffffffu},big));
        assert(noChannels.imports == Words({4,0xffffffffu},big));

        AnimationInput single = empty;
        AnimationInputChannel scalar; scalar.type=4;
        // No authored frames: the expanded Visibility default is 1.0f.
        single.channels.push_back(scalar);
        Recovered::Relo::Chunk oneChannel = CompileAnimation(single, platform);
        expected.assign(72,0);
        Word(expected,4,hierarchy+1,big); Word(expected,8,1,big); Word(expected,12,30,big);
        Word(expected,16,1,big); Word(expected,20,24,big); Word(expected,24,28,big);
        Word(expected,28,1624821609u,big); Word(expected,32,4,big);
        Word(expected,36,1,big); Word(expected,40,1,big);
        Word(expected,48,1,big); Word(expected,52,68,big);
        Word(expected,56,1,big); Word(expected,60,64,big);
        Word(expected,64,0x3f800000u,big);
        assert(oneChannel.instance == expected);
        assert(oneChannel.relocations == Words({20,24,60,52,0xffffffffu},big));
        assert(oneChannel.imports == Words({4,0xffffffffu},big));
    }
    }
    std::cout << "PASS W3X animation compiler independent three-buffer layout\n";
}
