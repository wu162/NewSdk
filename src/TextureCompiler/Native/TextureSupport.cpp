#include "TextureSupport.h"
#include <cstdlib>
#include <cstring>
#include <limits>
#include <new>
#include <stdexcept>

namespace Recovered { namespace TextureCompiler {
SimpleBuffer* ConstructBuffer(SimpleBuffer* self, std::uint32_t capacity) {
    if (!self) throw std::invalid_argument("SimpleBuffer object is null");
    self->size = 0;
    self->capacity = capacity;
    self->data = static_cast<std::uint8_t*>(std::malloc(capacity));
    if (!self->data && capacity) throw std::bad_alloc();
    return self;
}
void ResetBuffer(SimpleBuffer* self) {
    if (!self) throw std::invalid_argument("SimpleBuffer object is null");
    self->size = 0;
}
void AppendBuffer(SimpleBuffer* self, const void* bytes, std::uint32_t count) {
    if (!self || (count && !bytes)) throw std::invalid_argument("SimpleBuffer append has a null argument");
    if (count > std::numeric_limits<std::uint32_t>::max() - self->size)
        throw std::length_error("SimpleBuffer length overflows uint32");
    const std::uint32_t required = self->size + count;
    if (required > self->capacity) {
        // The original loops forever for zero initial capacity, and can wrap
        // the doubling loop. Explicit rejection only covers those undefined /
        // nonterminating inputs; successful-domain capacities double exactly.
        if (!self->capacity) throw std::length_error("Cannot grow a zero-capacity SimpleBuffer");
        std::uint32_t capacity = self->capacity;
        do {
            if (capacity > std::numeric_limits<std::uint32_t>::max() / 2)
                throw std::length_error("SimpleBuffer capacity doubling overflows uint32");
            capacity *= 2;
        } while (capacity < required);
        void* grown = std::realloc(self->data, capacity);
        if (!grown) throw std::bad_alloc();
        self->data = static_cast<std::uint8_t*>(grown);
        self->capacity = capacity;
    }
    // The original memcpy source must be valid and non-overlapping. In
    // particular callers cannot append from old self storage across realloc.
    if (count) std::memcpy(self->data + self->size, bytes, count);
    self->size = required;
}
void DestroyBuffer(SimpleBuffer* self) {
    if (!self) throw std::invalid_argument("SimpleBuffer object is null");
    if (self->data) std::free(self->data);
    self->data = nullptr;
    self->size = 0;
    self->capacity = 0;
}
std::uint32_t ReverseEndianness(std::uint32_t value) {
    return ((value >> 24) & 0xffu) | ((value >> 8) & 0xff00u)
        | ((value << 8) & 0xff0000u) | ((value << 24) & 0xff000000u);
}
void* Direct3DCreate9(std::uint32_t) { return reinterpret_cast<void*>(static_cast<std::uintptr_t>(1)); }
std::uint32_t Direct3DRelease() { return 1; }
}}
