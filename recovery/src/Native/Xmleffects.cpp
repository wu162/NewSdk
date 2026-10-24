#include "Xmleffects.h"
#include <cstring>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
namespace {
using Relo::U32;
struct FXListData {
    U32 reserved;
    float cullTracking;
    int cullTrackingMin, cullTrackingMax;
    U32 count;
    void** nuggets;
    unsigned char playEvenIfShrouded, tailorable, padding[2];
};
static_assert(sizeof(FXListData) == 28, "FXList original x86 ABI");
void SwapWord(void* field) {
    U32 x; std::memcpy(&x, field, 4);
    x = (x << 24) | ((x << 8) & 0x00ff0000u) | ((x >> 8) & 0x0000ff00u) | (x >> 24);
    std::memcpy(field, &x, 4);
}
float Time(const std::string& text) {
    // Initialize(float*,char*,Time): first strstr("ms"), then strstr("s").
    std::size_t cut = text.find("ms");
    float scale = 0.001f;
    if (cut == std::string::npos) { cut = text.find("s"); scale = 1.0f; }
    float value = ParseFloat(text.substr(0, cut));
    return static_cast<float>(static_cast<double>(value) * static_cast<double>(scale));
}
}
Relo::Chunk CompileXmleffectsFXList(const XmleffectsFXListInput& input, bool bigEndian) {
    Relo::Tracker tracker;
    FXListData* root = nullptr;
    tracker.Push(reinterpret_cast<void**>(&root), sizeof(FXListData), 1);
    root->playEvenIfShrouded = ParseBoolean(input.playEvenIfShrouded);
    root->tailorable = ParseBoolean(input.tailorable);
    if (input.hasCullTracking) root->cullTracking = Time(input.cullTracking);
    if (input.hasCullTrackingMin) root->cullTrackingMin = ParseSigned(input.cullTrackingMin);
    if (input.hasCullTrackingMax) root->cullTrackingMax = ParseSigned(input.cullTrackingMax);
    if (bigEndian) {
        SwapWord(&root->cullTracking); SwapWord(&root->cullTrackingMin);
        SwapWord(&root->cullTrackingMax); SwapWord(&root->count);
    }
    // XmlCompiler FinalizeTracker uses absoluteInstanceOffset=0 and preserves
    // all three arrays. It does not prepend a Tokenizer procedure table.
    return tracker.MakeRelocatable(bigEndian, 0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
