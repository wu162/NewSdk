#include "XmleffectsEnums.h"
#include <stdexcept>
namespace Recovered { namespace Native {
unsigned int XmleffectsEnum(const std::string& name, const std::string& value) {
    if(name == "DistributionType") {
        if(value == "CONSTANT") return 0u;
        if(value == "GAUSSIAN") return 2u;
        if(value == "HIGH_BIAS") return 5u;
        if(value == "LOW_BIAS") return 4u;
        if(value == "TRIANGULAR") return 3u;
        if(value == "UNIFORM") return 1u;
        return 0u;
    }
    if(name == "WeatherType") {
        if(value == "CLOUDY") return 1u;
        if(value == "CLOUDYRAINY") return 3u;
        if(value == "INVALID") return 6u;
        if(value == "NORMAL") return 0u;
        if(value == "RAINY") return 2u;
        if(value == "SNOWY") return 5u;
        if(value == "SUNNY") return 4u;
        return 1u;
    }
    if(name == "DisabledType") {
        if(value == "DEFAULT") return 0u;
        if(value == "EMP") return 2u;
        if(value == "FREEFALL") return 7u;
        if(value == "HELD") return 3u;
        if(value == "INFILTRATED") return 11u;
        if(value == "PARALYZED") return 4u;
        if(value == "SCRIPT_DISABLED") return 9u;
        if(value == "SCRIPT_UNDERPOWERED") return 10u;
        if(value == "TEMPORARILY_BUSY") return 8u;
        if(value == "UNDERPOWERED") return 6u;
        if(value == "UNMANNED") return 5u;
        if(value == "USER_PARALYZED") return 1u;
        return 0u;
    }
    if(name == "ViewShakeType") {
        if(value == "CINE_EXTREME") return 4u;
        if(value == "CINE_INSANE") return 5u;
        if(value == "NORMAL") return 1u;
        if(value == "SEVERE") return 3u;
        if(value == "STRONG") return 2u;
        if(value == "SUBTLE") return 0u;
        return 4u;
    }
    if(name == "TerrainScorchType") {
        if(value == "RANDOM") return 4294967295u;
        if(value == "SCORCH_1") return 0u;
        if(value == "SCORCH_10") return 9u;
        if(value == "SCORCH_11") return 10u;
        if(value == "SCORCH_12") return 11u;
        if(value == "SCORCH_13") return 12u;
        if(value == "SCORCH_14") return 13u;
        if(value == "SCORCH_15") return 14u;
        if(value == "SCORCH_16") return 15u;
        if(value == "SCORCH_2") return 1u;
        if(value == "SCORCH_3") return 2u;
        if(value == "SCORCH_4") return 3u;
        if(value == "SCORCH_5") return 4u;
        if(value == "SCORCH_6") return 5u;
        if(value == "SCORCH_7") return 6u;
        if(value == "SCORCH_8") return 7u;
        if(value == "SCORCH_9") return 8u;
        if(value == "TERRAIN_MAPPED") return 16u;
        return 4294967295u;
    }
    throw std::invalid_argument("effects enum table not yet recovered: "+name);
}
}}
