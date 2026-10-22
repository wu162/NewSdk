#include "Relocation.h"
#include <algorithm>
#include <cstring>
#include <stdexcept>

namespace Recovered { namespace Relo {
namespace {
void Write(std::vector<unsigned char>& bytes, U32 at, U32 value, bool big) {
    for (U32 i = 0; i < 4; ++i) bytes.at(at + i) = static_cast<unsigned char>(value >> ((big ? 3-i : i)*8));
}
void Append(std::vector<unsigned char>& bytes, U32 value, bool big) {
    U32 at = static_cast<U32>(bytes.size()); bytes.resize(at + 4); Write(bytes, at, value, big);
}
}
U32 Tracker::Allocate(U32 count, U32 elementSize) {
    U32 size = (count * elementSize + 3u) & ~3u;
    Block b; b.data.reset(new unsigned char[size]()); b.size = size; b.elementSize = elementSize;
    blocks_.push_back(std::move(b)); size_ += size;
    return static_cast<U32>(blocks_.size()-1);
}
Tracker::Bookmark Tracker::At(const void* location, U32 value) const {
    U32 block = stack_.back();
    return {block, static_cast<U32>(static_cast<const unsigned char*>(location)-blocks_[block].data.get()), value};
}
void* Tracker::Push(void** location, U32 objectSize, U32 count) {
    bool root = stack_.empty();
    U32 parent = root ? 0 : stack_.back();
    U32 target = Allocate(count, objectSize); stack_.push_back(target);
    if (root) return *location = blocks_[target].data.get();
    if (!location) return nullptr; // allocation and stack push still occurred
    pointers_.push_back({parent, static_cast<U32>(reinterpret_cast<unsigned char*>(location)-blocks_[parent].data.get()), target});
    return *location = blocks_[target].data.get();
}
void Tracker::Pop() { stack_.pop_back(); }
void Tracker::AddReference(void* location, U32 value) {
    references_.push_back(At(location, value)); std::memcpy(location, &value, 4);
}
bool Tracker::GetMemberOffset(const void* member, U32& offset) const {
    U32 base = 0; auto address = reinterpret_cast<std::uintptr_t>(member);
    for (const auto& b : blocks_) {
        auto start = reinterpret_cast<std::uintptr_t>(b.data.get());
        if (address >= start && address-start < b.size) { offset = base + static_cast<U32>(address-start); return true; }
        base += b.size;
    }
    return false; // preserve the caller's output on failure
}
void Tracker::BookmarkMemberOffset(const void* member, void** offset) {
    auto b = At(offset, 0); members_.push_back({b.block, b.offset, member});
}
bool Tracker::ExchangePointers(void* a, void* b) {
    U32 current = stack_.back(); auto& block = blocks_[current];
    auto start = reinterpret_cast<std::uintptr_t>(block.data.get());
    auto aa = reinterpret_cast<std::uintptr_t>(a), bb = reinterpret_cast<std::uintptr_t>(b);
    if (aa < start || aa-start >= block.size || bb < start || bb-start >= block.size) return false;
    U32 x = static_cast<U32>(aa-start), y = static_cast<U32>(bb-start);
    auto alter = [&](U32& offset) {
        if (offset >= x && offset < x + block.elementSize) offset = offset-x+y;
        else if (offset >= y && offset < y + block.elementSize) offset = offset-y+x;
    };
    for (auto& p : pointers_) if (p.block == current) alter(p.offset);
    for (auto& p : references_) if (p.block == current) alter(p.offset);
    for (auto& p : members_) if (p.block == current) alter(p.offset);
    return true; // only bookmarks move; caller exchanges object bytes
}
bool Tracker::Less(const Bookmark& a, const Bookmark& b) {
    return a.block < b.block || (a.block == b.block && a.value < b.value);
}
void Tracker::Quick(Bookmark* first, Bookmark* last, int depth) {
    while (last-first > 28 && depth > 0) {
        auto a = first, b = first+(last-first)/2, c = last-1;
        auto median = Less(*a,*b) ? (Less(*b,*c) ? b : (Less(*a,*c) ? c : a))
                                  : (Less(*a,*c) ? a : (Less(*b,*c) ? c : b));
        Bookmark pivot = *median;
        auto lo = first, hi = last;
        for (;;) {
            while (Less(*lo,pivot)) ++lo;
            do { --hi; } while (Less(pivot,*hi));
            if (lo >= hi) break;
            std::swap(*lo,*hi); ++lo;
        }
        --depth; Quick(lo,last,depth); last = lo;
    }
    if (depth == 0) { std::make_heap(first,last,Less); std::sort_heap(first,last,Less); }
}
void Tracker::Sort(std::vector<Bookmark>& values) {
    if (values.empty()) return;
    int bits = 0; for (auto n = values.size(); n; n >>= 1) ++bits;
    Quick(values.data(),values.data()+values.size(),bits*2-2);
    // Equivalent to the original guarded first-28 insertion pass followed by
    // its sentinel-based unguarded pass, without reading before the range.
    for (size_t i = 1; i < values.size(); ++i) {
        auto v = values[i]; size_t j = i;
        while (j && Less(v,values[j-1])) { values[j] = values[j-1]; --j; }
        values[j] = v;
    }
}
Chunk Tracker::MakeRelocatable(bool big, U32 absolute) {
    Chunk out; out.instance.resize(size_); blockOffsets_.clear();
    U32 base = 0;
    for (const auto& block : blocks_) {
        blockOffsets_.push_back(base);
        if (block.size) std::memcpy(out.instance.data()+base,block.data.get(),block.size);
        base += block.size;
    }
    Sort(pointers_); Sort(references_);
    for (const auto& p : pointers_) {
        U32 at = blockOffsets_[p.block]+p.offset;
        Append(out.relocations,at+absolute,big);
        Write(out.instance,at,blockOffsets_[p.value]+absolute,big);
    }
    if (!pointers_.empty()) Append(out.relocations,~0u,big);
    for (const auto& p : references_) {
        U32 at = blockOffsets_[p.block]+p.offset;
        Append(out.imports,at+absolute,big); Write(out.instance,at,p.value+1,big);
    }
    if (!references_.empty()) Append(out.imports,~0u,big);
    return out;
}
void Tracker::FixupMemberOffsets(const Tracker& owner, Chunk& chunk, bool big) const {
    for (const auto& m : members_) {
        U32 offset;
        if (!owner.GetMemberOffset(m.member,offset)) throw std::invalid_argument("Member outside data owner");
        Write(chunk.instance,blockOffsets_.at(m.block)+m.offset,static_cast<U32>(chunk.instance.size())+offset,big);
    }
}
}}
