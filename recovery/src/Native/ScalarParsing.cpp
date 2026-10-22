#include "ScalarParsing.h"
#include <cstdlib>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
static_assert(sizeof(long) == 4, "Recovered CRT integer parsing requires Windows 32-bit long");
bool ParseBoolean(const std::string& text) {
    return !text.empty() && (text[0] == '1' || text[0] == 't' || text[0] == 'T');
}
std::uint32_t ParseUnsigned(const std::string& text) {
    return static_cast<std::uint32_t>(std::strtoul(text.c_str(), nullptr, 0));
}
std::int32_t ParseSigned(const std::string& text) {
    return static_cast<std::int32_t>(std::strtol(text.c_str(), nullptr, 0));
}
float ParseFloat(const std::string& text) {
    std::string number(text);
    // The VC8 CRT recognizes D exponents; the current UCRT does not.
    for (char& c : number) { if (c == 'd') c = 'e'; else if (c == 'D') c = 'E'; }
    return static_cast<float>(std::atof(number.c_str()));
}
float ParsePercentage(const std::string& text) {
    const float value = ParseFloat(text);
    return static_cast<float>(static_cast<double>(value) * 0.01f);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
