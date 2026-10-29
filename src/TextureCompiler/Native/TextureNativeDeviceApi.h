#pragma once
#include <cstdint>
// Independent PC static-library ABI, x86/stdcall. No system D3D COM device.
// Supported factory domain: type != 2 (the SDK plugin uses type 1).
// Type 2 rendering/default-state initialization is explicitly unsupported.
namespace Recovered { namespace TextureCompiler { namespace NativeDeviceAbi {
constexpr std::uint32_t ObjectBytes = 0x5500;
constexpr std::uint32_t CommandBytes = 0x12c0;
using CreateFn = std::int32_t (__stdcall *)(std::uint32_t, std::uint32_t, void*, std::uint32_t, void*, void**);
using ReleaseFn = std::uint32_t (__stdcall *)(void*);
}}}
extern "C" std::int32_t __stdcall TextureCreateDevice(std::uint32_t adapter, std::uint32_t type, void* window, std::uint32_t flags, void* parameters, void** output);
extern "C" std::uint32_t __stdcall TextureDeviceRelease(void* device);
