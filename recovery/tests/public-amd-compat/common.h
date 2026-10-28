#pragma once
// Standalone ABI shim for the upstream's retained legacy CPU branch.
// No codec algorithm is implemented here. Avoid the application's UI/GPU framework headers.
#include <cstdio>
#include <cassert>
#include <cmath>
#include <cfloat>
#include <cstring>
#include <cstdlib>
namespace CMP {}
typedef unsigned char BYTE;
typedef unsigned long DWORD;
typedef unsigned char CMP_BYTE;
typedef signed char CMP_SBYTE;
typedef unsigned short CMP_WORD;
typedef unsigned long CMP_DWORD;
#define CMP_UNUSED(x) ((void)(x))
