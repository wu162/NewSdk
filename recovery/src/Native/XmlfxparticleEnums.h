#pragma once
#include <cstring>
#include <stdexcept>
#include <string>
namespace Recovered { namespace Native {
inline unsigned int XmlfxparticleEnum(const std::string& type,const std::string& text) {
    if(type=="DistributionType") {
        if(text=="CONSTANT") return 0u;
        if(text=="GAUSSIAN") return 2u;
        if(text=="HIGH_BIAS") return 5u;
        if(text=="LOW_BIAS") return 4u;
        if(text=="TRIANGULAR") return 3u;
        if(text=="UNIFORM") return 1u;
        return 0u;
    }
    if(type=="FXParticleSystem_Priority") {
        if(text=="ALWAYS_RENDER") return 6u;
        if(text=="HIGH_OR_ABOVE") return 2u;
        if(text=="HIGH_OR_BELOW") return 8u;
        if(text=="INVALID_PRIORITY") return 0u;
        if(text=="LOW_OR_ABOVE") return 4u;
        if(text=="LOW_OR_BELOW") return 10u;
        if(text=="MEDIUM_OR_ABOVE") return 3u;
        if(text=="MEDIUM_OR_BELOW") return 9u;
        if(text=="ULTRA_HIGH_ONLY") return 1u;
        if(text=="ULTRA_HIGH_OR_BELOW") return 7u;
        if(text=="VERY_LOW_ONLY") return 11u;
        if(text=="VERY_LOW_OR_ABOVE") return 5u;
        return 6u;
    }
    if(type=="FXParticleSystem_ShaderType") {
        if(text=="ADDITIVE") return 1u;
        if(text=="ADDITIVE_ALPHA_TEST") return 2u;
        if(text=="ADDITIVE_NO_DEPTH_TEST") return 6u;
        if(text=="ALPHA") return 3u;
        if(text=="ALPHA_NO_DEPTH_TEST") return 7u;
        if(text=="ALPHA_TEST") return 4u;
        if(text=="INVALID_SHADER") return 0u;
        if(text=="MULTIPLY") return 5u;
        if(text=="W3D_ALPHA") return 8u;
        return 1u;
    }
    if(type=="FXParticleSystem_Type") {
        if(text=="DRAWABLE") return 2u;
        if(text=="GPU_PARTICLE") return 5u;
        if(text=="GPU_TERRAINFIRE") return 6u;
        if(text=="INVALID_TYPE") return 0u;
        if(text=="PARTICLE") return 1u;
        if(text=="STREAK") return 3u;
        if(text=="SWARM") return 7u;
        if(text=="TRAIL") return 8u;
        if(text=="VOLUME_PARTICLE") return 4u;
        if(text=="WAVE_PARTICLE") return 9u;
        return 2u;
    }
    if(type=="FXParticleSystem_WindMotion") {
        if(text=="CIRCULAR") return 3u;
        if(text=="INVALID") return 0u;
        if(text=="NOT_USED") return 1u;
        if(text=="PING_PONG") return 2u;
        return 3u;
    }
    if(type=="FXParticleSystem_WavefrontShape") {
        if(text=="CIRCLE") return 1u;
        if(text=="INVALID") return 0u;
        if(text=="LINE") return 2u;
        return 1u;
    }
    throw std::invalid_argument("Unknown particle enum table");
}
}}
