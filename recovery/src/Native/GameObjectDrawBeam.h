// Generated original beam Draw group and complete FXShader28 dependency; blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue beamWeaponSlotTypeNames[] = {
    { "NO_WEAPON", 5 },
    { "PRIMARY_WEAPON", 0 },
    { "QUATERNARY_WEAPON", 3 },
    { "QUINARY_WEAPON", 4 },
    { "SECONDARY_WEAPON", 1 },
    { "TERTIARY_WEAPON", 2 },
};
const AIEnum beamWeaponSlotType = { beamWeaponSlotTypeNames, 6u, 5, 0u };
const ModuleToken beamDrawModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &dieModuleData, 0u, 0u },
};
const ModuleLayout beamDrawModuleData = { beamDrawModuleDataTokens, 1u };
const ModuleField beamColor4fFields[] = {
    { "r", 0u, ModuleValue::Float, nullptr, nullptr },
    { "g", 4u, ModuleValue::Float, nullptr, nullptr },
    { "b", 8u, ModuleValue::Float, nullptr, nullptr },
    { "a", 12u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleToken beamColor4fTokens[] = {
    { 568241586u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 1645004052u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 1370364569u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1214652683u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout beamColor4f = { beamColor4fTokens, 4u };
const ModuleField beamFXShaderConstantFields[] = {
    { "Name", 4u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken beamFXShaderConstantTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 1501410244u, 438907818u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout beamFXShaderConstant = { beamFXShaderConstantTokens, 2u };
const ModuleToken beamFXShaderConstantBoolTokens[] = {
    { 1u, 388283141u, 0u, &beamFXShaderConstant, 0u, 0u },
    { 3893002744u, 2011428087u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout beamFXShaderConstantBool = { beamFXShaderConstantBoolTokens, 2u };
const ModuleToken beamFXShaderConstantIntTokens[] = {
    { 1u, 388283141u, 0u, &beamFXShaderConstant, 0u, 0u },
    { 3893002744u, 602296504u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout beamFXShaderConstantInt = { beamFXShaderConstantIntTokens, 2u };
const ModuleToken beamFXShaderConstantTextureTokens[] = {
    { 1u, 388283141u, 0u, &beamFXShaderConstant, 0u, 0u },
    { 3893002744u, 3182837526u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout beamFXShaderConstantTexture = { beamFXShaderConstantTextureTokens, 2u };
const ModuleToken beamFXShaderConstantFloatTokens[] = {
    { 1u, 388283141u, 0u, &beamFXShaderConstant, 0u, 0u },
    { 3893002744u, 1683514611u, 12u, nullptr, 3u, 0u },
};
const ModuleLayout beamFXShaderConstantFloat = { beamFXShaderConstantFloatTokens, 2u };
const ModuleToken beamParticleSystemListTokens[] = {
    { 117547473u, 1139920438u, 0u, nullptr, 3u, 0u },
};
const ModuleLayout beamParticleSystemList = { beamParticleSystemListTokens, 1u };
const ModuleField beamFXShaderMaterialFields[] = {
    { "ShaderName", 0u, ModuleValue::String, nullptr, nullptr },
    { "TechniqueName", 8u, ModuleValue::String, "", nullptr },
    { "TechniqueIndex", 24u, ModuleValue::Byte, "0", nullptr },
};
const ModuleToken beamFXShaderMaterialTokens[] = {
    { 3679191380u, 438907818u, 0u, nullptr, 0u, 0u },
    { 1287768679u, 438907818u, 8u, nullptr, 0u, 0u },
    { 2050609923u, 814836630u, 24u, nullptr, 0u, 0u },
    { 1723017645u, 428092207u, 16u, nullptr, 10u, 0u },
};
const ModuleLayout beamFXShaderMaterial = { beamFXShaderMaterialTokens, 4u };
const ModuleField beamW3DLaserDrawModuleDataFields[] = {
    { "UseDistortionShader", 108u, ModuleValue::Boolean, "false", nullptr },
    { "Texture1_UTile", 8u, ModuleValue::Float, "1.0", nullptr },
    { "Texture1_VTile", 12u, ModuleValue::Float, "1.0", nullptr },
    { "Texture2_UTile", 16u, ModuleValue::Float, "1.0", nullptr },
    { "Texture2_VTile", 20u, ModuleValue::Float, "1.0", nullptr },
    { "Texture1_UScrollRate", 24u, ModuleValue::Float, "0.0", nullptr },
    { "Texture1_VScrollRate", 28u, ModuleValue::Float, "0.0", nullptr },
    { "Texture2_UScrollRate", 32u, ModuleValue::Float, "0.0", nullptr },
    { "Texture2_VScrollRate", 36u, ModuleValue::Float, "0.0", nullptr },
    { "Texture1_NumFrames", 40u, ModuleValue::Signed, "1", nullptr },
    { "Texture1_FrameRate", 44u, ModuleValue::Float, "30.0", nullptr },
    { "Texture2_NumFrames", 48u, ModuleValue::Signed, "1", nullptr },
    { "Texture2_FrameRate", 52u, ModuleValue::Float, "30.0", nullptr },
    { "LaserWidth", 56u, ModuleValue::Float, "5.0", nullptr },
    { "FadeInTime", 60u, ModuleValue::Time, "0s", nullptr },
    { "FadeOutTime", 64u, ModuleValue::Time, "0s", nullptr },
    { "WeaponSlotID", 68u, ModuleValue::Signed, "-1", nullptr },
    { "LaserStateID", 72u, ModuleValue::Signed, "0", nullptr },
};
const ModuleToken beamW3DLaserDrawModuleDataTokens[] = {
    { 1u, 402003752u, 0u, &beamDrawModuleData, 0u, 0u },
    { 1165348214u, 2011428087u, 108u, nullptr, 0u, 0u },
    { 420094846u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 4217272356u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2927321232u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3316353959u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 3285489758u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 620972938u, 1683514611u, 28u, nullptr, 0u, 0u },
    { 2877213179u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 554611614u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 4275014247u, 602296504u, 40u, nullptr, 0u, 0u },
    { 2704775550u, 1683514611u, 44u, nullptr, 0u, 0u },
    { 1629048240u, 602296504u, 48u, nullptr, 0u, 0u },
    { 3547179922u, 1683514611u, 52u, nullptr, 0u, 0u },
    { 1129558406u, 1683514611u, 56u, nullptr, 0u, 0u },
    { 4289579894u, 2575722984u, 60u, nullptr, 0u, 0u },
    { 155167005u, 2575722984u, 64u, nullptr, 0u, 0u },
    { 1579523250u, 602296504u, 68u, nullptr, 0u, 0u },
    { 943702378u, 602296504u, 72u, nullptr, 0u, 0u },
    { 3431444789u, 3354017012u, 76u, &beamFXShaderMaterial, 0u, 0u },
    { 3933465695u, 1049596812u, 104u, &lifecycleObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout beamW3DLaserDrawModuleData = { beamW3DLaserDrawModuleDataTokens, 21u };
const ModuleField beamW3DTracerDrawModuleDataFields[] = {
    { "MinLength", 8u, ModuleValue::Float, nullptr, nullptr },
    { "MaxLength", 12u, ModuleValue::Float, nullptr, nullptr },
    { "Width", 16u, ModuleValue::Float, nullptr, nullptr },
    { "MinSpeed", 20u, ModuleValue::Float, nullptr, nullptr },
    { "MaxSpeed", 24u, ModuleValue::Float, nullptr, nullptr },
    { "SweepSpeed", 28u, ModuleValue::Float, "1.0", nullptr },
    { "SpreadAngle", 32u, ModuleValue::Float, nullptr, nullptr },
    { "MinTracersPerFrame", 36u, ModuleValue::Float, nullptr, nullptr },
    { "MaxTracersPerFrame", 40u, ModuleValue::Float, nullptr, nullptr },
    { "FrameLifeTime", 44u, ModuleValue::Float, nullptr, nullptr },
    { "WeaponSlotType", 48u, ModuleValue::Enum, nullptr, &beamWeaponSlotType },
    { "WeaponSlotID", 52u, ModuleValue::Signed, "1", nullptr },
    { "Texture", 56u, ModuleValue::Reference, nullptr, nullptr },
    { "UseAdditiveBlending", 108u, ModuleValue::Boolean, "false", nullptr },
    { "TracerHitFx", 60u, ModuleValue::Reference, nullptr, nullptr },
    { "TracerEmitFx", 64u, ModuleValue::Reference, nullptr, nullptr },
    { "TextureIndex", 68u, ModuleValue::Unsigned, nullptr, nullptr },
};
const ModuleToken beamW3DTracerDrawModuleDataTokens[] = {
    { 1u, 402003752u, 0u, &dieModuleData, 0u, 0u },
    { 3831657100u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1039421983u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2801412506u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 808441066u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 1030106716u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 4123209693u, 1683514611u, 28u, nullptr, 0u, 0u },
    { 2765449531u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 2367118512u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 808677309u, 1683514611u, 40u, nullptr, 0u, 0u },
    { 3162487636u, 1683514611u, 44u, nullptr, 0u, 0u },
    { 407422281u, 407422281u, 48u, nullptr, 0u, 0u },
    { 1579523250u, 602296504u, 52u, nullptr, 0u, 0u },
    { 568797146u, 3182837526u, 56u, nullptr, 0u, 0u },
    { 178560385u, 2011428087u, 108u, nullptr, 0u, 0u },
    { 550660024u, 219975299u, 60u, nullptr, 0u, 0u },
    { 3819471697u, 219975299u, 64u, nullptr, 0u, 0u },
    { 4174938475u, 602296504u, 68u, nullptr, 0u, 0u },
    { 3033318555u, 380498768u, 72u, &beamColor4f, 0u, 0u },
    { 2913514849u, 380498768u, 88u, &beamColor4f, 0u, 0u },
    { 3933465695u, 1049596812u, 104u, &lifecycleObjectStatusValidationDataType, 4u, 0u },
};
const ModuleLayout beamW3DTracerDrawModuleData = { beamW3DTracerDrawModuleDataTokens, 21u };
const ModuleField beamW3DStreamDrawModuleDataFields[] = {
    { "UseDistortionShader", 72u, ModuleValue::Boolean, "false", nullptr },
    { "Velocity", 8u, ModuleValue::Float, "200.0", nullptr },
    { "ArcHeightFactor", 12u, ModuleValue::Float, "0.0", nullptr },
    { "UVWorldSize", 16u, ModuleValue::Float, "10.0", nullptr },
    { "NumTubeSides", 20u, ModuleValue::Signed, "8", nullptr },
    { "TubeRadius", 24u, ModuleValue::Float, "1.0", nullptr },
    { "HitFx", 28u, ModuleValue::Reference, nullptr, nullptr },
    { "WeaponSlotID", 32u, ModuleValue::Signed, "-1", nullptr },
    { "StreamStateID", 36u, ModuleValue::Signed, "0", nullptr },
};
const ModuleToken beamW3DStreamDrawModuleDataTokens[] = {
    { 1u, 402003752u, 0u, &beamDrawModuleData, 0u, 0u },
    { 1165348214u, 2011428087u, 72u, nullptr, 0u, 0u },
    { 2054604302u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1054640522u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 178396806u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 1545968965u, 602296504u, 20u, nullptr, 0u, 0u },
    { 774727683u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 635676468u, 219975299u, 28u, nullptr, 0u, 0u },
    { 1579523250u, 602296504u, 32u, nullptr, 0u, 0u },
    { 2417861220u, 602296504u, 36u, nullptr, 0u, 0u },
    { 3431444789u, 3354017012u, 40u, &beamFXShaderMaterial, 0u, 0u },
    { 19305773u, 2993638615u, 68u, &beamParticleSystemList, 4u, 0u },
};
const ModuleLayout beamW3DStreamDrawModuleData = { beamW3DStreamDrawModuleDataTokens, 12u };
const ModuleFieldSet beamFieldSets[] = {
    { beamW3DLaserDrawModuleDataFields, 18u },
    { beamW3DTracerDrawModuleDataFields, 17u },
    { beamW3DStreamDrawModuleDataFields, 9u },
};
const ModuleChoice beamChoices[] = {
    { 803073101u, &beamW3DLaserDrawModuleData, 112u, 283u },
    { 1409388128u, &beamW3DTracerDrawModuleData, 112u, 284u },
    { 1610744897u, &beamW3DStreamDrawModuleData, 76u, 285u },
};
