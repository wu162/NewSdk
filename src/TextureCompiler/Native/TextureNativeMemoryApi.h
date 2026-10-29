#pragma once
#include <cstdint>
// x86 loader ABI. Object storage must come from the paired allocator below if
// Delete is called with bit 0 set. Data and objects never cross CRT ownership.
namespace Recovered { namespace TextureCompiler { namespace NativeAbi {
struct Buffer { std::uint8_t* data;std::uint32_t size,capacity; };
using AllocateFn=void* (__cdecl *)(std::uint32_t);
using FreeFn=void (__cdecl *)(void*);
using ConstructFn=Buffer* (__thiscall *)(Buffer*,std::uint32_t);
using ResetFn=void (__thiscall *)(Buffer*);
using AppendFn=void (__thiscall *)(Buffer*,const void*,std::uint32_t);
using DeleteFn=Buffer* (__thiscall *)(Buffer*,std::uint32_t);
using ReverseFn=std::uint32_t (__cdecl *)(std::uint32_t);
using CreateFn=void* (__stdcall *)(std::uint32_t);
using ReleaseFn=std::uint32_t (__cdecl *)();
}}}
