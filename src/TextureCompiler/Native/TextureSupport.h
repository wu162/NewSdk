#pragma once
#include <cstdint>

namespace Recovered { namespace TextureCompiler {
// The original x86 layout is { pointer at 0, size at 4, capacity at 8 }.
// Host-native pointers deliberately allow portable behavior tests on x64.
struct SimpleBuffer {
    std::uint8_t* data;
    std::uint32_t size;
    std::uint32_t capacity;
};
SimpleBuffer* ConstructBuffer(SimpleBuffer* self, std::uint32_t capacity);
void ResetBuffer(SimpleBuffer* self);
void AppendBuffer(SimpleBuffer* self, const void* bytes, std::uint32_t count);
// Destructor body only. Object storage belongs to the caller.
void DestroyBuffer(SimpleBuffer* self);
std::uint32_t ReverseEndianness(std::uint32_t value);
// These are the original static-library facade's constant-return functions;
// they do not create or release a system Direct3D COM object.
void* Direct3DCreate9(std::uint32_t sdkVersion);
std::uint32_t Direct3DRelease();
}}
