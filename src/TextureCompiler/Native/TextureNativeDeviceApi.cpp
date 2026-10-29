#include "TextureNativeDeviceApi.h"
#include <Windows.h>
#include <cstring>
using namespace Recovered::TextureCompiler::NativeDeviceAbi;
static_assert(sizeof(void*) == 4, "Original device ABI requires x86");
namespace {
void Store(void* p, std::int32_t offset, std::uint32_t value) {
    std::memcpy(static_cast<unsigned char*>(p) + offset, &value, sizeof(value));
}
std::uint32_t Load(const void* p, std::int32_t offset) {
    std::uint32_t value;
    std::memcpy(&value, static_cast<const unsigned char*>(p) + offset, sizeof(value));
    return value;
}
void FreeObject(void* device) {
    GlobalFree(reinterpret_cast<HGLOBAL>(static_cast<std::uintptr_t>(Load(device, -4))));
}
}
extern "C" std::int32_t __stdcall TextureCreateDevice(std::uint32_t, std::uint32_t type, void*, std::uint32_t, void*, void** output) {
    *output = nullptr;
    // A real rendering device has a separate state/dispatch initialization chain.
    // Never return a partially initialized type-2 object as a successful device.
    if (type == 2) return static_cast<std::int32_t>(0x80004001u); // E_NOTIMPL
    // RVA e044/e06e: zero GlobalAlloc(size+alignment), align (base+alignment+3)
    // down, preserve allocator base in the DWORD immediately before the object.
    auto base = static_cast<unsigned char*>(GlobalAlloc(GMEM_ZEROINIT, ObjectBytes + 128));
    if (!base) return static_cast<std::int32_t>(0x8007000eu);
    const auto aligned = (reinterpret_cast<std::uintptr_t>(base) + 128 + 3) & ~std::uintptr_t(127);
    auto device = reinterpret_cast<void*>(aligned);
    Store(device, -4, static_cast<std::uint32_t>(reinterpret_cast<std::uintptr_t>(base)));
    std::memset(device, 0, ObjectBytes);
    // RVA 115fe: complete initialization for the SDK's non-rendering device.
    Store(device, 0x3c, 1);
    Store(device, 0x2a80, 0x1ddd4000);
    Store(device, 0x2a84, 0x20000);
    Store(device, 0x2a88, GetCurrentThreadId());
    Store(device, 0x2a8c, 0);
    auto commands = VirtualAlloc(nullptr, CommandBytes, MEM_COMMIT, PAGE_READWRITE);
    Store(device, 0x3864, static_cast<std::uint32_t>(reinterpret_cast<std::uintptr_t>(commands)));
    if (!commands) {
        FreeObject(device);
        return static_cast<std::int32_t>(0x8007000eu);
    }
    *output = device;
    return 0;
}
extern "C" std::uint32_t __stdcall TextureDeviceRelease(void* device) {
    const auto count = Load(device, 0x3c);
    if (count != 1) {
        Store(device, 0x3c, count - 1);
        return count - 1;
    }
    // RVA df8a/1169a: a factory-created type!=2 device has no bound resources,
    // no current rendering-device global and no secondary command allocation.
    // Resource-populated/type-2 objects are outside this ABI's supported domain.
    const auto commands = Load(device, 0x3864);
    if (commands) VirtualFree(reinterpret_cast<void*>(static_cast<std::uintptr_t>(commands)), 0, MEM_RELEASE);
    FreeObject(device);
    return 0;
}
