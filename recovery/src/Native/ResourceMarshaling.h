#pragma once
#include "Relocation.h"
#include <string>
namespace Recovered { namespace Native {
// The caller must distinguish absent XML values from present empty strings.
void MarshalString(Relo::Tracker& tracker, void* location, const std::string& text);
void MarshalReference(Relo::Tracker& tracker, void* location, const std::string& text);
std::uint32_t HashAssetId(const std::string& text);
}}
