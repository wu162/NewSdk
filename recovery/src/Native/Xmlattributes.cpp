#include "Xmlattributes.h"
#include "ResourceMarshaling.h"
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
namespace {
struct LogicCommandSetData { Relo::U32 reserved, count; Relo::U32* commands; };
static_assert(sizeof(LogicCommandSetData) == 12, "LogicCommandSet requires x86");
Relo::U32 Swap(Relo::U32 x) {
    return (x << 24) | ((x << 8) & 0x00ff0000u) | ((x >> 8) & 0x0000ff00u) | (x >> 24);
}
}
Relo::Chunk CompileXmlattributesLogicCommandSet(const std::vector<std::string>& commands, bool bigEndian) {
    Relo::Tracker tracker;
    LogicCommandSetData* root = nullptr;
    tracker.Push(reinterpret_cast<void**>(&root), sizeof(*root), 1);
    root->count = static_cast<Relo::U32>(commands.size());
    if (root->count) {
        tracker.Push(reinterpret_cast<void**>(&root->commands), 4, root->count);
        for (Relo::U32 i = 0; i < root->count; ++i)
            MarshalReference(tracker, &root->commands[i], commands[i]);
        tracker.Pop();
    }
    if (bigEndian) root->count = Swap(root->count);
    // XmlCompiler FinalizeTracker serializes this tracker directly at offset 0.
    // In particular, import bookmarks are retained and there is no token table.
    return tracker.MakeRelocatable(bigEndian, 0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
