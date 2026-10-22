#include "DataInitializers.h"
#include <cstdlib>
#include <cstring>
#include <string>

#ifdef _MANAGED
#pragma managed(push, off)
#endif

namespace Recovered { namespace Native {
    namespace {
        float InitializeWithUnit(const char* text, const char* const* units,
            const float* scales, unsigned int unitCount)
        {
            const char* suffix = nullptr;
            float scale = 1.0f;
            // Original uses strstr, in table order, rather than parsing a suffix
            // or selecting whichever unit occurs earliest in the input.
            for (unsigned int i = 0; i < unitCount; ++i) {
                suffix = std::strstr(text, units[i]);
                if (suffix != nullptr) {
                    scale = scales[i];
                    break;
                }
            }
            std::string number(text);
            if (suffix != nullptr) {
                number.resize(static_cast<std::size_t>(suffix - text));
            }
            // The SDK's VC8 CRT accepts a Fortran D exponent (2d1 == 20).
            // Current UCRT atof does not. Preserve that observed behavior;
            // unit removal above must happen first (2d == two degrees).
            for (char& character : number) {
                if (character == 'd') character = 'e';
                else if (character == 'D') character = 'E';
            }
            // Preserve both float-rounding points in the original IL:
            // atof(double) -> float; then double(float)*double(float) -> float.
            const float value = static_cast<float>(std::atof(number.c_str()));
            return static_cast<float>(static_cast<double>(value) * scale);
        }
    }

    float InitializeTime(const char* text)
    {
        const char* const units[] = { "ms", "s" };
        const float scales[] = { 0.001f, 1.0f };
        return InitializeWithUnit(text, units, scales, 2);
    }

    float InitializeAngle(const char* text)
    {
        const char* const units[] = { "r", "d" };
        // The IL constant is pi rounded to float, divided by 180.0f.
        const float scales[] = { 1.0f, 3.1415927410125732421875f / 180.0f };
        return InitializeWithUnit(text, units, scales, 2);
    }
} }

#ifdef _MANAGED
#pragma managed(pop)
#endif
