#include "ScalarParsing.h"
#include <cstdlib>
#include <cctype>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
static_assert(sizeof(long) == 4, "Recovered CRT integer parsing requires Windows 32-bit long");
bool ParseBoolean(const std::string& text) {
    return !text.empty() && (text[0] == '1' || text[0] == 't' || text[0] == 'T');
}
std::uint32_t ParseUnsigned(const std::string& text) {
    const char* at = text.c_str();
    while (std::isspace(static_cast<unsigned char>(*at))) ++at;
    const bool negative = *at == '-';
    if (*at == '-' || *at == '+') {
        ++at;
        if (*at == '+' || *at == '-' || std::isspace(static_cast<unsigned char>(*at))) return 0;
    }
    // VC8 saturates magnitude before applying the unsigned negation, even on
    // overflow. Current UCRT instead returns ULONG_MAX for negative overflow.
    const std::uint32_t magnitude = static_cast<std::uint32_t>(std::strtoul(at, nullptr, 0));
    return negative ? 0u - magnitude : magnitude;
}
std::int32_t ParseSigned(const std::string& text) {
    return static_cast<std::int32_t>(std::strtol(text.c_str(), nullptr, 0));
}
float ParseFloat(const std::string& text) {
    // VC8 accepts decimal numbers and D exponents, but not UCRT's newer
    // hexadecimal floats, INF or NaN spellings. Isolate the legacy numeric
    // prefix before calling the current CRT for correctly rounded conversion.
    std::size_t at = 0;
    while (at < text.size() && std::isspace(static_cast<unsigned char>(text[at]))) ++at;
    const std::size_t start = at;
    if (at < text.size() && (text[at] == '+' || text[at] == '-')) ++at;
    bool digits = false;
    while (at < text.size() && text[at] >= '0' && text[at] <= '9') { ++at; digits = true; }
    if (at < text.size() && text[at] == '.') {
        ++at;
        while (at < text.size() && text[at] >= '0' && text[at] <= '9') { ++at; digits = true; }
    }
    if (!digits) return 0.0f;
    const std::size_t mantissaEnd = at;
    if (at < text.size() && (text[at] == 'e' || text[at] == 'E' || text[at] == 'd' || text[at] == 'D')) {
        ++at;
        if (at < text.size() && (text[at] == '+' || text[at] == '-')) ++at;
        const std::size_t exponentStart = at;
        while (at < text.size() && text[at] >= '0' && text[at] <= '9') ++at;
        if (at == exponentStart) at = mantissaEnd;
    }
    std::string number = text.substr(start, at - start);
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
