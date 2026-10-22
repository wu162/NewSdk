#pragma once
#include <cstdint>
#include <string>
namespace Recovered { namespace Native {
bool ParseBoolean(const std::string& text);
std::uint32_t ParseUnsigned(const std::string& text);
std::int32_t ParseSigned(const std::string& text);
float ParseFloat(const std::string& text);
float ParsePercentage(const std::string& text);
}}
