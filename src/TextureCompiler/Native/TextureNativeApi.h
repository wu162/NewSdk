#pragma once
#include <cstdint>
// Returned ASCII strings, including trailing spaces, are immutable static storage.
// Borrowed pointers remain valid until this module unloads; callers must not free them.
extern "C" const char* __cdecl TextureGetErrorString(std::int32_t code);
