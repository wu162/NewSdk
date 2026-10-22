#include "../Native/Relocation.h"
using namespace System;
using namespace System::Runtime::InteropServices;
namespace Recovered {
public ref class RelocationChunk {
public:
    array<Byte>^ Instance;
    array<Byte>^ Relocations;
    array<Byte>^ Imports;
};
public ref class RelocationTracker sealed {
    Relo::Tracker* tracker;
    Relo::Chunk* chunk;
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        auto out = gcnew array<Byte>(static_cast<int>(source.size()));
        if (out->Length) Marshal::Copy(IntPtr(const_cast<unsigned char*>(source.data())),out,0,out->Length);
        return out;
    }
public:
    RelocationTracker() : tracker(new Relo::Tracker()), chunk(new Relo::Chunk()) {}
    ~RelocationTracker() { this->!RelocationTracker(); }
    !RelocationTracker() { delete chunk; chunk = nullptr; delete tracker; tracker = nullptr; }
    IntPtr Push(IntPtr location, UInt32 size, UInt32 count) {
        return IntPtr(tracker->Push(static_cast<void**>(location.ToPointer()),size,count));
    }
    void Pop() { tracker->Pop(); }
    void AddReference(IntPtr location, UInt32 value) { tracker->AddReference(location.ToPointer(),value); }
    bool GetMemberOffset(IntPtr member, UInt32% offset) {
        Relo::U32 value = offset; bool found = tracker->GetMemberOffset(member.ToPointer(),value); offset = value; return found;
    }
    void BookmarkMemberOffset(IntPtr member, IntPtr offset) { tracker->BookmarkMemberOffset(member.ToPointer(),static_cast<void**>(offset.ToPointer())); }
    bool ExchangePointers(IntPtr a, IntPtr b) { return tracker->ExchangePointers(a.ToPointer(),b.ToPointer()); }
    void MakeRelocatable(bool bigEndian, UInt32 absolute) { *chunk = tracker->MakeRelocatable(bigEndian,absolute); }
    void FixupMemberOffsets(RelocationTracker^ owner, bool bigEndian) { tracker->FixupMemberOffsets(*owner->tracker,*chunk,bigEndian); }
    RelocationChunk^ Snapshot() {
        auto out = gcnew RelocationChunk(); out->Instance = Copy(chunk->instance);
        out->Relocations = Copy(chunk->relocations); out->Imports = Copy(chunk->imports); return out;
    }
};
}
