#pragma once
#include <cstdint>
#include <memory>
#include <vector>

namespace Recovered { namespace Relo {
using U32 = std::uint32_t;
struct Chunk {
    std::vector<unsigned char> instance, relocations, imports;
};

// Reconstructed semantics, not the VC8 EASTL container ABI. Serialized pointers
// remain 32 bits; callers of Push supply a native pointer location on x86.
class Tracker {
    struct Block {
        std::unique_ptr<unsigned char[]> data;
        U32 size, elementSize;
    };
    struct Bookmark { U32 block, offset, value; };
    struct MemberBookmark { U32 block, offset; const void* member; };
    U32 size_ = 0;
    std::vector<U32> stack_, blockOffsets_;
    std::vector<Block> blocks_;
    std::vector<Bookmark> pointers_, references_;
    std::vector<MemberBookmark> members_;
    Bookmark At(const void* location, U32 value) const;
    static bool Less(const Bookmark& a, const Bookmark& b);
    static void Sort(std::vector<Bookmark>& values);
    static void Quick(Bookmark* first, Bookmark* last, int depth);
public:
    U32 Allocate(U32 count, U32 elementSize);
    void* Push(void** pointerLocation, U32 objectSize, U32 objectCount);
    void Pop();
    void AddReference(void* location, U32 value);
    bool GetMemberOffset(const void* member, U32& offset) const;
    void BookmarkMemberOffset(const void* member, void** offset);
    bool ExchangePointers(void* a, void* b);
    Chunk MakeRelocatable(bool bigEndian, U32 absoluteInstanceOffset);
    void FixupMemberOffsets(const Tracker& owner, Chunk& chunk, bool bigEndian) const;
};
}}
