#include "ResourceMarshaling.h"
#include <cstring>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
using U32 = std::uint32_t;
namespace {
U32 Decimal(const char* text) {
    while (*text == ' ' || (*text >= '\t' && *text <= '\r')) ++text;
    bool negative = *text == '-';
    if (*text == '-' || *text == '+') ++text;
    U32 value = 0, limit = negative ? 0x80000000u : 0x7FFFFFFFu;
    while (*text >= '0' && *text <= '9') {
        U32 digit = static_cast<U32>(*text++ - '0');
        value = value > (limit-digit)/10 ? limit : value*10+digit;
    }
    return negative ? 0u-value : value;
}
U32 Short(const unsigned char* p) { return p[0] | (U32(p[1]) << 8); }
U32 SignedByte(unsigned char c) { return static_cast<U32>(static_cast<std::int8_t>(c)); }
}
void MarshalString(Relo::Tracker& tracker, void* location, const std::string& text) {
    U32 size = static_cast<U32>(std::strlen(text.c_str()));
    std::memcpy(location, &size, 4);
    void* bytes = tracker.Push(reinterpret_cast<void**>(static_cast<unsigned char*>(location)+4), 1, size+1);
    std::memcpy(bytes, text.c_str(), size+1);
    tracker.Pop();
}
void MarshalReference(Relo::Tracker& tracker, void* location, const std::string& text) {
    // BAB has already resolved the resource name; the suffix is a decimal index.
    const char* separator = std::strchr(text.c_str(), '\\');
    if (separator) tracker.AddReference(location, Decimal(separator+1));
}
U32 HashAssetId(const std::string& text) {
    std::string lower(text.c_str());
    for (char& c : lower) if (c >= 'A' && c <= 'Z') c = static_cast<char>(c-'A'+'a');
    U32 size = static_cast<U32>(lower.size()), hash = size;
    if (!size) return 0;
    auto p = reinterpret_cast<const unsigned char*>(lower.data());
    for (U32 n = size/4; n; --n, p += 4) {
        hash += Short(p); hash ^= (Short(p+2) ^ (hash << 5)) << 11; hash += hash >> 11;
    }
    switch (size & 3) {
    case 3: hash += Short(p); hash ^= ((SignedByte(p[2]) << 2) ^ hash) << 16; hash += hash >> 11; break;
    case 2: hash += Short(p); hash ^= hash << 11; hash += hash >> 17; break;
    case 1: hash += SignedByte(*p); hash ^= hash << 10; hash += hash >> 1; break;
    }
    hash ^= hash << 3; hash += hash >> 5; hash ^= hash << 2; hash += hash >> 15;
    return (hash << 10) ^ hash;
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
