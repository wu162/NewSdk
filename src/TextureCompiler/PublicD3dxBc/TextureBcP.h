#pragma once
#define NOMINMAX
#include <Windows.h>
#include <algorithm>
#include <cassert>
#include <cfloat>
#include <cmath>
#include <cstddef>
#include <cstdint>
#include <cstring>
// The historical PC decoder uses scalar floating-point arithmetic. Keep the
// public implementation on DirectXMath's scalar path for this first adapter.
#ifndef _XM_NO_INTRINSICS_
#define _XM_NO_INTRINSICS_
#endif
#include <DirectXMath.h>
#include <DirectXPackedVector.h>
