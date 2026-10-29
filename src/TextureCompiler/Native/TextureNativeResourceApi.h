#pragma once
#include <cstdint>
// Raw PC resources use the original header/refcount layout and Win32 ownership:
// GlobalAlloc object; VirtualAlloc storage (tagged pointers at original offsets).
// Supported final destruction: types 0..8 except GPU-backed standalone type4.
// Type9/custom GPU contexts are explicitly rejected at final release (UINT_MAX).
// Pointer validity, acyclic parent ownership and allocation pairing are required.
namespace Recovered { namespace TextureCompiler { namespace NativeResourceAbi {
using RefFn=std::uint32_t (__stdcall *)(void*);
using UnlockFn=std::int32_t (__stdcall *)(void*,std::uint32_t);
}}}
extern "C" std::uint32_t __stdcall TextureResourceAddRef(void* resource);
extern "C" std::uint32_t __stdcall TextureResourceRelease(void* resource);
extern "C" std::int32_t __stdcall TextureResourceUnlockRect(void* texture,std::uint32_t level);
