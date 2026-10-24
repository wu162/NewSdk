#pragma once
#include <cstdint>
#include <vector>
namespace Recovered { namespace Native {
// Original EASTL permutation, including equal-key order. Preparation only:
// WeaponSetUpdate must also exchange Tracker bookmarks before moving records.
std::vector<std::uint32_t> WeaponSlotOrder(const std::vector<int>& ordering);
}}
