// Generated from pinned original Dynamics Draw methods; source blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue drawDynamicsDynamicsCollisionTypeNames[] = {
    { "INTERCOLLIDING", 1 },
    { "NONCOLLIDING", 3 },
    { "NONINTERCOLLIDING", 0 },
    { "OMNICOLLIDING", 2 },
};
const AIEnum drawDynamicsDynamicsCollisionType = { drawDynamicsDynamicsCollisionTypeNames, 4u, 1, 0u };
const EnumNameValue drawDynamicsIntuitiveCollisionTypeNames[] = {
    { "COLLIDES_WITH_EVERYTHING", 2 },
    { "COLLIDES_WITH_GROUND_AND_VEHICLES", 1 },
    { "COLLIDES_WITH_GROUND_ONLY", 0 },
};
const AIEnum drawDynamicsIntuitiveCollisionType = { drawDynamicsIntuitiveCollisionTypeNames, 3u, 2, 0u };
const EnumNameValue drawDynamicsDynamicsDrawPriorityNames[] = {
    { "MUST_BE_DRAWN", 2 },
    { "REMOVABLE", 0 },
    { "REMOVE_ONLY_IF_NEEDED", 1 },
};
const AIEnum drawDynamicsDynamicsDrawPriority = { drawDynamicsDynamicsDrawPriorityNames, 3u, 2, 0u };
const EnumNameValue drawDynamicsEffectsLODTypeNames[] = {
    { "HIGH", 3 },
    { "LOW", 1 },
    { "MEDIUM", 2 },
    { "ULTRA_HIGH", 4 },
    { "VERY_LOW", 0 },
};
const AIEnum drawDynamicsEffectsLODType = { drawDynamicsEffectsLODTypeNames, 5u, 3, 0u };
const EnumNameValue drawDynamicsDynamicsContactTagNames[] = {
    { "DEBRIS", 7 },
    { "GROUND", 1 },
    { "NONE", 0 },
    { "PROP", 4 },
    { "TREE", 6 },
    { "VEHICLE", 3 },
    { "WALL", 5 },
    { "WATER", 2 },
};
const AIEnum drawDynamicsDynamicsContactTag = { drawDynamicsDynamicsContactTagNames, 8u, 7, 0u };
const ModuleToken drawDynamicsDynamicsShapeTypeTokens[] = {
    { 2267787263u, 691437687u, 0u, nullptr, 2u, 0u },
    { 3046282137u, 2236188103u, 4u, nullptr, 2u, 0u },
};
const ModuleLayout drawDynamicsDynamicsShapeType = { drawDynamicsDynamicsShapeTypeTokens, 2u };
const ModuleField drawDynamicsDynamicsSphereShapeTypeFields[] = {
    { "Radius", 8u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleToken drawDynamicsDynamicsSphereShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &drawDynamicsDynamicsShapeType, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout drawDynamicsDynamicsSphereShapeType = { drawDynamicsDynamicsSphereShapeTypeTokens, 2u };
const ModuleField drawDynamicsDynamicsCapsuleShapeTypeFields[] = {
    { "Radius", 8u, ModuleValue::Float, nullptr, nullptr },
    { "HalfLength", 12u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleToken drawDynamicsDynamicsCapsuleShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &drawDynamicsDynamicsShapeType, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2268209495u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout drawDynamicsDynamicsCapsuleShapeType = { drawDynamicsDynamicsCapsuleShapeTypeTokens, 3u };
const ModuleField drawDynamicsDynamicsBoxShapeTypeFields[] = {
    { "HalfSizeX", 8u, ModuleValue::Float, nullptr, nullptr },
    { "HalfSizeY", 12u, ModuleValue::Float, nullptr, nullptr },
    { "HalfSizeZ", 16u, ModuleValue::Float, nullptr, nullptr },
    { "EdgeRadius", 20u, ModuleValue::Float, "0", nullptr },
};
const ModuleToken drawDynamicsDynamicsBoxShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &drawDynamicsDynamicsShapeType, 0u, 0u },
    { 3283477156u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3335082463u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 3231876940u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 2494874265u, 1683514611u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout drawDynamicsDynamicsBoxShapeType = { drawDynamicsDynamicsBoxShapeTypeTokens, 5u };
const ModuleField drawDynamicsDynamicsCylinderShapeTypeFields[] = {
    { "Radius", 8u, ModuleValue::Float, nullptr, nullptr },
    { "HalfLength", 12u, ModuleValue::Float, nullptr, nullptr },
    { "EdgeRadius", 16u, ModuleValue::Float, "0", nullptr },
};
const ModuleToken drawDynamicsDynamicsCylinderShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &drawDynamicsDynamicsShapeType, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2268209495u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2494874265u, 1683514611u, 16u, nullptr, 0u, 0u },
};
const ModuleLayout drawDynamicsDynamicsCylinderShapeType = { drawDynamicsDynamicsCylinderShapeTypeTokens, 4u };
const ModuleField drawDynamicsDynamicsTriangleShapeTypeFields[] = {
    { "X1", 8u, ModuleValue::Float, nullptr, nullptr },
    { "Y1", 12u, ModuleValue::Float, nullptr, nullptr },
    { "Z1", 16u, ModuleValue::Float, nullptr, nullptr },
    { "X2", 20u, ModuleValue::Float, nullptr, nullptr },
    { "Y2", 24u, ModuleValue::Float, nullptr, nullptr },
    { "Z2", 28u, ModuleValue::Float, nullptr, nullptr },
    { "X3", 32u, ModuleValue::Float, nullptr, nullptr },
    { "Y3", 36u, ModuleValue::Float, nullptr, nullptr },
    { "Z3", 40u, ModuleValue::Float, nullptr, nullptr },
    { "EdgeRadius", 44u, ModuleValue::Float, "0", nullptr },
};
const ModuleToken drawDynamicsDynamicsTriangleShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &drawDynamicsDynamicsShapeType, 0u, 0u },
    { 3356871300u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3465944601u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2534701849u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 1662605289u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 1419179897u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 1105583844u, 1683514611u, 28u, nullptr, 0u, 0u },
    { 1586316556u, 1683514611u, 32u, nullptr, 0u, 0u },
    { 2570922351u, 1683514611u, 36u, nullptr, 0u, 0u },
    { 2210293783u, 1683514611u, 40u, nullptr, 0u, 0u },
    { 2494874265u, 1683514611u, 44u, nullptr, 0u, 0u },
};
const ModuleLayout drawDynamicsDynamicsTriangleShapeType = { drawDynamicsDynamicsTriangleShapeTypeTokens, 11u };
const ModuleField drawDynamicsDynamicsVolumeTypeFields[] = {
    { "Mass", 0u, ModuleValue::Float, "0", nullptr },
    { "Bounciness", 4u, ModuleValue::Percent, "30%", nullptr },
    { "Spinniness", 8u, ModuleValue::Float, "2.0", nullptr },
    { "StaticFriction", 12u, ModuleValue::Float, "0.6", nullptr },
    { "KineticFriction", 16u, ModuleValue::Float, "0.5", nullptr },
    { "AngularDrag", 20u, ModuleValue::Float, "0.1", nullptr },
    { "LinearDrag", 24u, ModuleValue::Float, "0.1", nullptr },
    { "ReportContacts", 72u, ModuleValue::Boolean, "false", nullptr },
    { "ContactTag", 28u, ModuleValue::Enum, "NONE", &drawDynamicsDynamicsContactTag },
};
const ModuleToken drawDynamicsDynamicsVolumeTypeTokens[] = {
    { 3289575800u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 3989063176u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 4165278293u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1077291918u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 4228715191u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3366782081u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 3319205883u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 249076369u, 2011428087u, 72u, nullptr, 0u, 0u },
    { 1042572641u, 2464367332u, 28u, nullptr, 0u, 0u },
    { 3222474882u, 2550196117u, 32u, &drawDynamicsDynamicsSphereShapeType, 1u, 12u },
    { 2241289593u, 4160064679u, 40u, &drawDynamicsDynamicsCapsuleShapeType, 1u, 16u },
    { 1212192353u, 1376476976u, 48u, &drawDynamicsDynamicsBoxShapeType, 1u, 24u },
    { 3480105696u, 939838314u, 56u, &drawDynamicsDynamicsCylinderShapeType, 1u, 20u },
    { 1533886466u, 458429666u, 64u, &drawDynamicsDynamicsTriangleShapeType, 1u, 48u },
};
const ModuleLayout drawDynamicsDynamicsVolumeType = { drawDynamicsDynamicsVolumeTypeTokens, 14u };
const ModuleField drawDynamicsDynamicsBoneVolumeTypeFields[] = {
    { "BoneName", 76u, ModuleValue::String, nullptr, nullptr },
};
const ModuleToken drawDynamicsDynamicsBoneVolumeTypeTokens[] = {
    { 1u, 2525245349u, 0u, &drawDynamicsDynamicsVolumeType, 0u, 0u },
    { 4048210012u, 438907818u, 76u, nullptr, 0u, 0u },
};
const ModuleLayout drawDynamicsDynamicsBoneVolumeType = { drawDynamicsDynamicsBoneVolumeTypeTokens, 2u };
const ModuleToken drawDynamicsDynamicsBoneVolumeSetTypeTokens[] = {
    { 3812826366u, 4174443494u, 0u, &drawDynamicsDynamicsBoneVolumeType, 1u, 84u },
};
const ModuleLayout drawDynamicsDynamicsBoneVolumeSetType = { drawDynamicsDynamicsBoneVolumeSetTypeTokens, 1u };
const ModuleField drawDynamicsDynamicsLifetimeFields[] = {
    { "Delay", 0u, ModuleValue::Time, "0s", nullptr },
    { "FadeTime", 4u, ModuleValue::Time, nullptr, nullptr },
};
const ModuleToken drawDynamicsDynamicsLifetimeTokens[] = {
    { 1718388562u, 2575722984u, 0u, nullptr, 0u, 0u },
    { 1730190031u, 2575722984u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout drawDynamicsDynamicsLifetime = { drawDynamicsDynamicsLifetimeTokens, 2u };
const ModuleField drawDynamicsW3DDynamicsDrawModuleDataFields[] = {
    { "Collision", 216u, ModuleValue::Enum, nullptr, &drawDynamicsDynamicsCollisionType },
    { "IntuitiveCollision", 220u, ModuleValue::Enum, "COLLIDES_WITH_GROUND_ONLY", &drawDynamicsIntuitiveCollisionType },
    { "DrawPriority", 224u, ModuleValue::Enum, "MUST_BE_DRAWN", &drawDynamicsDynamicsDrawPriority },
    { "LODPriority", 228u, ModuleValue::Enum, "VERY_LOW", &drawDynamicsEffectsLODType },
    { "Explodiness", 232u, ModuleValue::Float, "0", nullptr },
    { "FlingPerturbation", 236u, ModuleValue::Float, "0", nullptr },
    { "InitiallyActive", 248u, ModuleValue::Boolean, "true", nullptr },
};
const ModuleToken drawDynamicsW3DDynamicsDrawModuleDataTokens[] = {
    { 1u, 1322199627u, 0u, &drawMainW3DScriptedModelDrawModuleData, 0u, 0u },
    { 4239356069u, 1231950365u, 216u, nullptr, 0u, 0u },
    { 2536790540u, 3459026409u, 220u, nullptr, 0u, 0u },
    { 234120523u, 1098088443u, 224u, nullptr, 0u, 0u },
    { 3373463952u, 4091321714u, 228u, nullptr, 0u, 0u },
    { 2003221980u, 1683514611u, 232u, nullptr, 0u, 0u },
    { 2550281427u, 1683514611u, 236u, nullptr, 0u, 0u },
    { 1454433568u, 2011428087u, 248u, nullptr, 0u, 0u },
    { 3436108774u, 689013301u, 240u, &drawDynamicsDynamicsBoneVolumeSetType, 4u, 0u },
    { 1411793767u, 703544202u, 244u, &drawDynamicsDynamicsLifetime, 4u, 0u },
};
const ModuleLayout drawDynamicsW3DDynamicsDrawModuleData = { drawDynamicsW3DDynamicsDrawModuleDataTokens, 10u };
struct DrawDynamicsSchema;
struct DrawDynamicsChild {const char* name;unsigned offset,mode;const DrawDynamicsSchema* nested;unsigned size;};
struct DrawDynamicsSchema {ModuleFieldSet fields;const DrawDynamicsChild* children;unsigned count;const DrawDynamicsSchema* parent;};
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsShapeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsSphereShapeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsCapsuleShapeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsBoxShapeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsCylinderShapeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsTriangleShapeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsVolumeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsBoneVolumeType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsBoneVolumeSetType;
extern const DrawDynamicsSchema drawDynamicsSchemaDynamicsLifetime;
extern const DrawDynamicsSchema drawDynamicsSchemaW3DDynamicsDrawModuleData;
const DrawDynamicsChild drawDynamicsDynamicsShapeTypeChildren[] = {
    { "Translation", 0u, 2u, nullptr, 12u },
    { "Rotation", 4u, 2u, nullptr, 16u },
};
const DrawDynamicsSchema drawDynamicsSchemaDynamicsShapeType = { { nullptr, 0u }, drawDynamicsDynamicsShapeTypeChildren, 2u, nullptr };
const DrawDynamicsSchema drawDynamicsSchemaDynamicsSphereShapeType = { { drawDynamicsDynamicsSphereShapeTypeFields, 1u }, nullptr, 0u, &drawDynamicsSchemaDynamicsShapeType };
const DrawDynamicsSchema drawDynamicsSchemaDynamicsCapsuleShapeType = { { drawDynamicsDynamicsCapsuleShapeTypeFields, 2u }, nullptr, 0u, &drawDynamicsSchemaDynamicsShapeType };
const DrawDynamicsSchema drawDynamicsSchemaDynamicsBoxShapeType = { { drawDynamicsDynamicsBoxShapeTypeFields, 4u }, nullptr, 0u, &drawDynamicsSchemaDynamicsShapeType };
const DrawDynamicsSchema drawDynamicsSchemaDynamicsCylinderShapeType = { { drawDynamicsDynamicsCylinderShapeTypeFields, 3u }, nullptr, 0u, &drawDynamicsSchemaDynamicsShapeType };
const DrawDynamicsSchema drawDynamicsSchemaDynamicsTriangleShapeType = { { drawDynamicsDynamicsTriangleShapeTypeFields, 10u }, nullptr, 0u, &drawDynamicsSchemaDynamicsShapeType };
const DrawDynamicsChild drawDynamicsDynamicsVolumeTypeChildren[] = {
    { "Sphere", 32u, 1u, &drawDynamicsSchemaDynamicsSphereShapeType, 12u },
    { "Capsule", 40u, 1u, &drawDynamicsSchemaDynamicsCapsuleShapeType, 16u },
    { "Box", 48u, 1u, &drawDynamicsSchemaDynamicsBoxShapeType, 24u },
    { "Cylinder", 56u, 1u, &drawDynamicsSchemaDynamicsCylinderShapeType, 20u },
    { "Triangle", 64u, 1u, &drawDynamicsSchemaDynamicsTriangleShapeType, 48u },
};
const DrawDynamicsSchema drawDynamicsSchemaDynamicsVolumeType = { { drawDynamicsDynamicsVolumeTypeFields, 9u }, drawDynamicsDynamicsVolumeTypeChildren, 5u, nullptr };
const DrawDynamicsSchema drawDynamicsSchemaDynamicsBoneVolumeType = { { drawDynamicsDynamicsBoneVolumeTypeFields, 1u }, nullptr, 0u, &drawDynamicsSchemaDynamicsVolumeType };
const DrawDynamicsChild drawDynamicsDynamicsBoneVolumeSetTypeChildren[] = {
    { "BoneVolume", 0u, 1u, &drawDynamicsSchemaDynamicsBoneVolumeType, 84u },
};
const DrawDynamicsSchema drawDynamicsSchemaDynamicsBoneVolumeSetType = { { nullptr, 0u }, drawDynamicsDynamicsBoneVolumeSetTypeChildren, 1u, nullptr };
const DrawDynamicsSchema drawDynamicsSchemaDynamicsLifetime = { { drawDynamicsDynamicsLifetimeFields, 2u }, nullptr, 0u, nullptr };
const DrawDynamicsChild drawDynamicsW3DDynamicsDrawModuleDataChildren[] = {
    { "BoneVolumes", 240u, 4u, &drawDynamicsSchemaDynamicsBoneVolumeSetType, 8u },
    { "Lifetime", 244u, 4u, &drawDynamicsSchemaDynamicsLifetime, 8u },
};
const DrawDynamicsSchema drawDynamicsSchemaW3DDynamicsDrawModuleData = { { drawDynamicsW3DDynamicsDrawModuleDataFields, 7u }, drawDynamicsW3DDynamicsDrawModuleDataChildren, 2u, nullptr };
