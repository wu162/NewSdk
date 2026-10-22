#include "ArmorTemplate.h"
#include <cstdlib>
#include <cstring>

#ifdef _MANAGED
#pragma managed(push, off)
#endif

namespace Recovered { namespace Native {
namespace {
using Relo::U32;
struct ArmorEntry { U32 damage; float percent; };
struct ArmorData {
    U32 reserved;
    float percentages[5];
    U32 count;
    ArmorEntry* entries;
};
struct Procedure { U32 name, type, offset; };
static_assert(sizeof(ArmorData) == 32, "Armor serialization requires x86");
static_assert(sizeof(ArmorEntry) == 8, "Armor entry ABI");
static_assert(sizeof(Procedure) == 12, "Procedure ABI");
U32 Swap(U32 value) {
    return (value << 24) | ((value << 8) & 0x00ff0000u) |
        ((value >> 8) & 0x0000ff00u) | (value >> 24);
}
void SwapWord(void* field) {
    U32 word; std::memcpy(&word, field, 4); word = Swap(word);
    std::memcpy(field, &word, 4);
}
void Member(Procedure& p, U32 name, U32 type, const void* member, Relo::Tracker& tokens) {
    p.name = name; p.type = type;
    tokens.BookmarkMemberOffset(member, reinterpret_cast<void**>(&p.offset));
}
}

unsigned int ParseDamageType(const std::string& text) {
    static const char* names[] = {
        "FORCE", "CRUSH", "SLASH", "PIERCE", "FLAME", "HEALING",
        "UNRESISTABLE", "WATER", "PENALTY", "FALLING", "BLAST", "REFLECTED",
        "PASSENGER", "MAGIC", "UNDEFINED", "LOGICAL_FIRE", "POISON", "LASER",
        "PLASMA", "FIRE", "ROCKET", "GUN", "CANNON", "GRENADE", "SNIPER",
        "MINE", "TIBERIUM", "STORM_SHIELD", "MELEE", "AUTO_CANNON", "IMPACT",
        "EXPLOSIVE", "CONCUSSIVE", "FLAK", "PRISM", "TESLA", "KI", "RADIATION", "ALL"
    };
    for (U32 i = 0; i < 39; ++i) if (text == names[i]) return i;
    return 38;
}

Relo::Chunk CompileArmor(const ArmorInput& input, bool bigEndian) {
    Relo::Tracker data, tokens;
    ArmorData* root = nullptr;
    data.Push(reinterpret_cast<void**>(&root), sizeof(ArmorData), 1);
    for (U32 i = 0; i < 5; ++i) root->percentages[i] = ParsePercentage(input.percentages[i]);
    root->count = static_cast<U32>(input.entries.size());
    if (root->count) {
        data.Push(reinterpret_cast<void**>(&root->entries), sizeof(ArmorEntry), root->count);
        for (U32 i = 0; i < root->count; ++i) {
            const auto& entry = input.entries[i];
            if (entry.hasDamage) root->entries[i].damage = ParseDamageType(entry.damage);
            if (entry.hasPercent) root->entries[i].percent = ParsePercentage(entry.percent);
        }
        data.Pop();
    }

    Procedure* table = nullptr;
    tokens.Push(reinterpret_cast<void**>(&table), sizeof(Procedure), 7);
    const U32 names[] = { static_cast<U32>(-1538240), static_cast<U32>(-899026358),
        81996879, static_cast<U32>(-2003558244), static_cast<U32>(-1429821187) };
    for (U32 i = 0; i < 5; ++i) Member(table[i], names[i], 1683514611, &root->percentages[i], tokens);
    table[5].name = 735930353; table[5].type = 617967919;
    Procedure* list = static_cast<Procedure*>(tokens.Push(
        reinterpret_cast<void**>(&table[5].offset), sizeof(Procedure), root->count + 1));
    Member(list[0], 1820487795, 1564774360, &root->count, tokens);
    for (U32 i = 0; i < root->count; ++i) {
        list[i + 1].name = static_cast<U32>(-1320050453);
        Procedure* item = static_cast<Procedure*>(tokens.Push(
            reinterpret_cast<void**>(&list[i + 1].offset), sizeof(Procedure), 3));
        Member(item[0], 2083775721, 1179152369, &root->entries[i].damage, tokens);
        Member(item[1], static_cast<U32>(-1382012893), 1683514611, &root->entries[i].percent, tokens);
        tokens.Pop();
    }
    tokens.Pop(); tokens.Pop();

    // Tokenize before converting count/scalars; pointers are handled by Tracker.
    if (bigEndian) {
        for (float& value : root->percentages) SwapWord(&value);
        for (U32 i = 0; i < root->count; ++i) {
            SwapWord(&root->entries[i].damage); SwapWord(&root->entries[i].percent);
        }
        SwapWord(&root->count);
    }
    Relo::Chunk tokenChunk = tokens.MakeRelocatable(bigEndian, 0);
    tokens.FixupMemberOffsets(data, tokenChunk, bigEndian);
    Relo::Chunk result = data.MakeRelocatable(bigEndian, static_cast<U32>(tokenChunk.instance.size()));
    if (bigEndian) for (std::size_t offset = 0; offset < tokenChunk.instance.size(); offset += 12) {
        SwapWord(&tokenChunk.instance[offset]); SwapWord(&tokenChunk.instance[offset + 4]);
    }
    tokenChunk.instance.insert(tokenChunk.instance.end(), result.instance.begin(), result.instance.end());
    result.instance = std::move(tokenChunk.instance);
    // Original Tokenizer HandleType always returns an empty ImportsData array.
    result.imports.clear();
    return result;
}
}}

#ifdef _MANAGED
#pragma managed(pop)
#endif
