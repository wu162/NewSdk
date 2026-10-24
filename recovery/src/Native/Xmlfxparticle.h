#pragma once
#include "Relocation.h"
#include "ScalarParsing.h"
#include <string>
#include <vector>
#include "ResourceMarshaling.h"
namespace Recovered { namespace Native {
enum XmlfxparticleFieldKind { ParticleFloat, ParticleUnsigned, ParticleSigned, ParticleHash, ParticleReference, ParticleString, ParticleTime, ParticleAngle, ParticleBoolean, ParticleEnum, ParticlePercentage };
struct XmlfxparticleFieldInput {
    unsigned int offset;
    XmlfxparticleFieldKind kind;
    std::string text, enumType;
};
struct XmlfxparticleNuggetInput {
    unsigned int typeId = 0, size = 0;
    std::vector<XmlfxparticleFieldInput> fields;
    std::vector<std::string> disabled;
    bool hasRange = false;
    std::string rangeLow = "SCORCH_1", rangeHigh = "SCORCH_4";
};
}}

namespace Recovered { namespace Native {
struct XmlfxparticleRandomInput {
    bool present = false;
    std::string type = "UNIFORM", low, high;
};
struct XmlfxparticleAlphaInput {
    XmlfxparticleRandomInput value;
    std::string relativeAge, frame = "1000";
};
struct XmlfxparticleColorInput {
    std::string relativeAge, frame = "1000", rgb[3];
};
struct XmlfxparticleModuleRandom { unsigned int offset; XmlfxparticleRandomInput input; };
struct XmlfxparticleLightningSetInput { XmlfxparticleRandomInput random[6]; };
struct XmlfxparticleObjectSetInput {
    bool hasRenderGroup = false;
    std::string renderGroup, numObjects = "0", percent = "0.0";
};
struct XmlfxparticleModuleInput {
    bool present = false;
    unsigned int typeId = 0, size = 0;
    std::vector<XmlfxparticleFieldInput> fields;
    std::vector<XmlfxparticleModuleRandom> random;
    std::vector<XmlfxparticleLightningSetInput> lightningSets;
    std::vector<XmlfxparticleObjectSetInput> objectSets;
};
struct XmlfxparticleParticleInput {
    std::string priority = "ULTRA_HIGH_ONLY", shader = "ADDITIVE", type = "PARTICLE";
    std::string references[3], typedIds[2], lifetime = "0", sortLevel = "0", flags[5];
    std::string offset[3];
    XmlfxparticleRandomInput random[6], colorScale;
    XmlfxparticleModuleInput modules[5]; // Physics, Draw, Volume, Velocity, Update
    std::vector<XmlfxparticleModuleInput> events;
    bool hasEmitterSound = false;
    bool hasAlphas = false, hasColors = false, hasWind = false;
    std::vector<XmlfxparticleAlphaInput> alphas;
    std::vector<XmlfxparticleColorInput> colors;
    std::string houseColor = "false", saturation = "1.0", wind[12];
};
Relo::Chunk CompileXmlfxparticleParticle(const XmlfxparticleParticleInput& input, bool bigEndian);
}}

