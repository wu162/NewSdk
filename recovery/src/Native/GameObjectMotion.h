// Generated original motion contracts; blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const EnumNameValue motionProjectileStateMachineTypeNames[] = {
    { "BalloonProjectile", 2 },
    { "BezierSplineProjectile", 3 },
    { "BombProjectile", 0 },
    { "StraightLineProjectile", 1 },
};
const AIEnum motionProjectileStateMachineTypeEnum = { motionProjectileStateMachineTypeNames, 4u, 2, 0u };
const EnumNameValue motionDynamicsMotionTypeNames[] = {
    { "SIMULATED", 1 },
    { "STATIC", 2 },
    { "TRACKED", 0 },
};
const AIEnum motionDynamicsMotionTypeEnum = { motionDynamicsMotionTypeNames, 3u, 1, 0u };
const EnumNameValue motionDynamicsCollisionTypeNames[] = {
    { "INTERCOLLIDING", 1 },
    { "NONCOLLIDING", 3 },
    { "NONINTERCOLLIDING", 0 },
    { "OMNICOLLIDING", 2 },
};
const AIEnum motionDynamicsCollisionTypeEnum = { motionDynamicsCollisionTypeNames, 4u, 1, 0u };
const EnumNameValue motionIntuitiveCollisionTypeNames[] = {
    { "COLLIDES_WITH_EVERYTHING", 2 },
    { "COLLIDES_WITH_GROUND_AND_VEHICLES", 1 },
    { "COLLIDES_WITH_GROUND_ONLY", 0 },
};
const AIEnum motionIntuitiveCollisionTypeEnum = { motionIntuitiveCollisionTypeNames, 3u, 2, 0u };
const EnumNameValue motionDynamicsContactTagNames[] = {
    { "DEBRIS", 7 },
    { "GROUND", 1 },
    { "NONE", 0 },
    { "PROP", 4 },
    { "TREE", 6 },
    { "VEHICLE", 3 },
    { "WALL", 5 },
    { "WATER", 2 },
};
const AIEnum motionDynamicsContactTagEnum = { motionDynamicsContactTagNames, 8u, 7, 0u };
const EnumNameValue motionDynamicsDrawPriorityNames[] = {
    { "MUST_BE_DRAWN", 2 },
    { "REMOVABLE", 0 },
    { "REMOVE_ONLY_IF_NEEDED", 1 },
};
const AIEnum motionDynamicsDrawPriorityEnum = { motionDynamicsDrawPriorityNames, 3u, 2, 0u };
const EnumNameValue motionProjectileUpdateOptionsNames[] = {
    { "LAUNCH_AT_TARGET_POS", 0 },
};
const AIEnum motionProjectileUpdateOptionsEnum = { motionProjectileUpdateOptionsNames, 1u, 0, 1u };
const ModuleField motionProjectileUpdateModuleDataFields[] = {
    { "StateMachine", 8u, ModuleValue::Enum, nullptr, &motionProjectileStateMachineTypeEnum },
    { "MinDetonationHeight", 12u, ModuleValue::Float, nullptr, nullptr },
    { "MinTargetDistance", 16u, ModuleValue::Float, nullptr, nullptr },
    { "Flags", 20u, ModuleValue::ReplaceOptions, nullptr, &motionProjectileUpdateOptionsEnum },
};
const ModuleField motionDynamicsUpdateModuleDataFields[] = {
    { "Motion", 8u, ModuleValue::Enum, nullptr, &motionDynamicsMotionTypeEnum },
    { "Collision", 12u, ModuleValue::Enum, nullptr, &motionDynamicsCollisionTypeEnum },
    { "IntuitiveCollision", 16u, ModuleValue::Enum, "COLLIDES_WITH_GROUND_ONLY", &motionIntuitiveCollisionTypeEnum },
    { "ContactTag", 20u, ModuleValue::Enum, "NONE", &motionDynamicsContactTagEnum },
    { "DrawPriority", 24u, ModuleValue::Enum, "REMOVABLE", &motionDynamicsDrawPriorityEnum },
};
const ModuleField motionFreefallUpdateModuleDataFields[] = {
    { "ModelConditionsToSet", 8u, ModuleValue::ModelCondition, nullptr, nullptr },
    { "InitialVelocity", 68u, ModuleValue::Float, "0", nullptr },
    { "Acceleration", 72u, ModuleValue::Float, "1", nullptr },
    { "TerminalVelocity", 76u, ModuleValue::Float, "15", nullptr },
};
const ModuleField motionEnableDynamicsCollideModuleDataFields[] = {
    { "Weight", 8u, ModuleValue::Float, "0.1", nullptr },
};
const ModuleField motionProjectileMoveInStraightLineDataFields[] = {
    { "MuzzleVelocity", 4u, ModuleValue::Float, nullptr, nullptr },
    { "Acceleration", 8u, ModuleValue::Float, nullptr, nullptr },
    { "TerminalVelocity", 12u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleField motionProjectileMoveToGroundDataFields[] = {
    { "InitialSpeed", 4u, ModuleValue::Float, "0", nullptr },
    { "Acceleration", 8u, ModuleValue::Float, nullptr, nullptr },
    { "TerminalVelocity", 12u, ModuleValue::Float, nullptr, nullptr },
    { "RotationSpeedPerSecond", 16u, ModuleValue::Angle, "0d", nullptr },
    { "TerminalRotation", 20u, ModuleValue::Angle, "90d", nullptr },
    { "ForwardVelocityDamping", 24u, ModuleValue::Percent, "100%", nullptr },
};
const ModuleField motionBezierSplineProjectileDataFields[] = {
    { "Velocity", 4u, ModuleValue::Float, nullptr, nullptr },
    { "Range", 8u, ModuleValue::Float, nullptr, nullptr },
    { "Gravity", 12u, ModuleValue::Float, "1.0", nullptr },
};
const ModuleField motionDynamicsVolumeTypeFields[] = {
    { "Mass", 0u, ModuleValue::Float, "0", nullptr },
    { "Bounciness", 4u, ModuleValue::Percent, "30%", nullptr },
    { "Spinniness", 8u, ModuleValue::Float, "2.0", nullptr },
    { "StaticFriction", 12u, ModuleValue::Float, "0.6", nullptr },
    { "KineticFriction", 16u, ModuleValue::Float, "0.5", nullptr },
    { "AngularDrag", 20u, ModuleValue::Float, "0.1", nullptr },
    { "LinearDrag", 24u, ModuleValue::Float, "0.1", nullptr },
    { "ReportContacts", 72u, ModuleValue::Boolean, "false", nullptr },
    { "ContactTag", 28u, ModuleValue::Enum, "NONE", &motionDynamicsContactTagEnum },
};
const ModuleField motionDynamicsSphereShapeTypeFields[] = {
    { "Radius", 8u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleField motionDynamicsCapsuleShapeTypeFields[] = {
    { "Radius", 8u, ModuleValue::Float, nullptr, nullptr },
    { "HalfLength", 12u, ModuleValue::Float, nullptr, nullptr },
};
const ModuleField motionDynamicsBoxShapeTypeFields[] = {
    { "HalfSizeX", 8u, ModuleValue::Float, nullptr, nullptr },
    { "HalfSizeY", 12u, ModuleValue::Float, nullptr, nullptr },
    { "HalfSizeZ", 16u, ModuleValue::Float, nullptr, nullptr },
    { "EdgeRadius", 20u, ModuleValue::Float, "0", nullptr },
};
const ModuleField motionDynamicsCylinderShapeTypeFields[] = {
    { "Radius", 8u, ModuleValue::Float, nullptr, nullptr },
    { "HalfLength", 12u, ModuleValue::Float, nullptr, nullptr },
    { "EdgeRadius", 16u, ModuleValue::Float, "0", nullptr },
};
const ModuleField motionDynamicsTriangleShapeTypeFields[] = {
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
const ModuleField motionVector3Fields[] = {
    { "x", 0u, ModuleValue::Float, "0.0", nullptr },
    { "y", 4u, ModuleValue::Float, "0.0", nullptr },
    { "z", 8u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleField motionQuaternionFields[] = {
    { "x", 0u, ModuleValue::Float, "0.0", nullptr },
    { "y", 4u, ModuleValue::Float, "0.0", nullptr },
    { "z", 8u, ModuleValue::Float, "0.0", nullptr },
    { "w", 12u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleFieldSet motionFieldSets[] = {
    { motionProjectileUpdateModuleDataFields, 4u },
    { motionDynamicsUpdateModuleDataFields, 5u },
    { motionFreefallUpdateModuleDataFields, 4u },
    { motionEnableDynamicsCollideModuleDataFields, 1u },
    { motionProjectileMoveInStraightLineDataFields, 3u },
    { motionProjectileMoveToGroundDataFields, 6u },
    { motionBezierSplineProjectileDataFields, 3u },
    { motionDynamicsVolumeTypeFields, 9u },
    { motionDynamicsSphereShapeTypeFields, 1u },
    { motionDynamicsCapsuleShapeTypeFields, 2u },
    { motionDynamicsBoxShapeTypeFields, 4u },
    { motionDynamicsCylinderShapeTypeFields, 3u },
    { motionDynamicsTriangleShapeTypeFields, 10u },
    { motionVector3Fields, 3u },
    { motionQuaternionFields, 4u },
};
const ModuleToken motionModuleDataTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
    { 2363267224u, 3186653541u, 4u, nullptr, 0u, 0u },
};
const ModuleLayout motionModuleData = { motionModuleDataTokens, 2u };
const ModuleToken motionBehaviorModuleDataTokens[] = {
    { 1u, 3613234082u, 0u, &motionModuleData, 0u, 0u },
};
const ModuleLayout motionBehaviorModuleData = { motionBehaviorModuleDataTokens, 1u };
const ModuleToken motionContainModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &motionBehaviorModuleData, 0u, 0u },
};
const ModuleLayout motionContainModuleData = { motionContainModuleDataTokens, 1u };
const ModuleToken motionUpdateModuleDataTokens[] = {
    { 1u, 687874333u, 0u, &motionContainModuleData, 0u, 0u },
};
const ModuleLayout motionUpdateModuleData = { motionUpdateModuleDataTokens, 1u };
const ModuleToken motionProjectileUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &motionUpdateModuleData, 0u, 0u },
    { 1939012404u, 243927557u, 8u, nullptr, 0u, 0u },
    { 2848808553u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2444801719u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 4081386890u, 3506386483u, 20u, nullptr, 0u, 0u },
    { 2697616422u, 2670309790u, 24u, nullptr, 9u, 0u },
};
const ModuleLayout motionProjectileUpdateModuleData = { motionProjectileUpdateModuleDataTokens, 6u };
const ModuleToken motionDynamicsShapeTypeTokens[] = {
    { 2267787263u, 691437687u, 0u, nullptr, 2u, 0u },
    { 3046282137u, 2236188103u, 4u, nullptr, 2u, 0u },
};
const ModuleLayout motionDynamicsShapeType = { motionDynamicsShapeTypeTokens, 2u };
const ModuleToken motionDynamicsSphereShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &motionDynamicsShapeType, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout motionDynamicsSphereShapeType = { motionDynamicsSphereShapeTypeTokens, 2u };
const ModuleToken motionDynamicsCapsuleShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &motionDynamicsShapeType, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2268209495u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout motionDynamicsCapsuleShapeType = { motionDynamicsCapsuleShapeTypeTokens, 3u };
const ModuleToken motionDynamicsBoxShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &motionDynamicsShapeType, 0u, 0u },
    { 3283477156u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3335082463u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 3231876940u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 2494874265u, 1683514611u, 20u, nullptr, 0u, 0u },
};
const ModuleLayout motionDynamicsBoxShapeType = { motionDynamicsBoxShapeTypeTokens, 5u };
const ModuleToken motionDynamicsCylinderShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &motionDynamicsShapeType, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2268209495u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 2494874265u, 1683514611u, 16u, nullptr, 0u, 0u },
};
const ModuleLayout motionDynamicsCylinderShapeType = { motionDynamicsCylinderShapeTypeTokens, 4u };
const ModuleToken motionDynamicsTriangleShapeTypeTokens[] = {
    { 1u, 977547402u, 0u, &motionDynamicsShapeType, 0u, 0u },
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
const ModuleLayout motionDynamicsTriangleShapeType = { motionDynamicsTriangleShapeTypeTokens, 11u };
const ModuleToken motionDynamicsVolumeTypeTokens[] = {
    { 3289575800u, 1683514611u, 0u, nullptr, 0u, 0u },
    { 3989063176u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 4165278293u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 1077291918u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 4228715191u, 1683514611u, 16u, nullptr, 0u, 0u },
    { 3366782081u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 3319205883u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 249076369u, 2011428087u, 72u, nullptr, 0u, 0u },
    { 1042572641u, 2464367332u, 28u, nullptr, 0u, 0u },
    { 3222474882u, 2550196117u, 32u, &motionDynamicsSphereShapeType, 1u, 12u },
    { 2241289593u, 4160064679u, 40u, &motionDynamicsCapsuleShapeType, 1u, 16u },
    { 1212192353u, 1376476976u, 48u, &motionDynamicsBoxShapeType, 1u, 24u },
    { 3480105696u, 939838314u, 56u, &motionDynamicsCylinderShapeType, 1u, 20u },
    { 1533886466u, 458429666u, 64u, &motionDynamicsTriangleShapeType, 1u, 48u },
};
const ModuleLayout motionDynamicsVolumeType = { motionDynamicsVolumeTypeTokens, 14u };
const ModuleToken motionDynamicsUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &motionUpdateModuleData, 0u, 0u },
    { 3980050261u, 2744843040u, 8u, nullptr, 0u, 0u },
    { 4239356069u, 1231950365u, 12u, nullptr, 0u, 0u },
    { 2536790540u, 3459026409u, 16u, nullptr, 0u, 0u },
    { 1042572641u, 2464367332u, 20u, nullptr, 0u, 0u },
    { 234120523u, 1098088443u, 24u, nullptr, 0u, 0u },
    { 3707251233u, 2525245349u, 28u, &motionDynamicsVolumeType, 4u, 0u },
};
const ModuleLayout motionDynamicsUpdateModuleData = { motionDynamicsUpdateModuleDataTokens, 7u };
const ModuleToken motionFreefallUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &motionUpdateModuleData, 0u, 0u },
    { 814327707u, 1742454729u, 8u, nullptr, 0u, 0u },
    { 2128225161u, 1683514611u, 68u, nullptr, 0u, 0u },
    { 16211304u, 1683514611u, 72u, nullptr, 0u, 0u },
    { 3334390368u, 1683514611u, 76u, nullptr, 0u, 0u },
};
const ModuleLayout motionFreefallUpdateModuleData = { motionFreefallUpdateModuleDataTokens, 5u };
const ModuleToken motionCollideModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &motionBehaviorModuleData, 0u, 0u },
};
const ModuleLayout motionCollideModuleData = { motionCollideModuleDataTokens, 1u };
const ModuleToken motionEnableDynamicsCollideModuleDataTokens[] = {
    { 1u, 1894105347u, 0u, &motionCollideModuleData, 0u, 0u },
    { 2520901945u, 1683514611u, 8u, nullptr, 0u, 0u },
};
const ModuleLayout motionEnableDynamicsCollideModuleData = { motionEnableDynamicsCollideModuleDataTokens, 2u };
const ModuleToken motionProjectileMoveBaseTokens[] = {
    { 430981792u, 602296504u, 0u, nullptr, 0u, 0u },
};
const ModuleLayout motionProjectileMoveBase = { motionProjectileMoveBaseTokens, 1u };
const ModuleToken motionProjectileMoveInStraightLineDataTokens[] = {
    { 1u, 2882646154u, 0u, &motionProjectileMoveBase, 0u, 0u },
    { 3523397774u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 16211304u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3334390368u, 1683514611u, 12u, nullptr, 0u, 0u },
};
const ModuleLayout motionProjectileMoveInStraightLineData = { motionProjectileMoveInStraightLineDataTokens, 4u };
const ModuleToken motionProjectileMoveToGroundDataTokens[] = {
    { 1u, 2882646154u, 0u, &motionProjectileMoveBase, 0u, 0u },
    { 2170864342u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 16211304u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3334390368u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 3200860086u, 1310124621u, 16u, nullptr, 0u, 0u },
    { 1969522213u, 1310124621u, 20u, nullptr, 0u, 0u },
    { 257248090u, 1683514611u, 24u, nullptr, 0u, 0u },
};
const ModuleLayout motionProjectileMoveToGroundData = { motionProjectileMoveToGroundDataTokens, 7u };
const ModuleToken motionBezierSplineProjectileDataTokens[] = {
    { 1u, 2882646154u, 0u, &motionProjectileMoveBase, 0u, 0u },
    { 2054604302u, 1683514611u, 4u, nullptr, 0u, 0u },
    { 3426209446u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3459874901u, 1683514611u, 12u, nullptr, 0u, 0u },
    { 373209091u, 751542556u, 16u, nullptr, 3u, 0u },
};
const ModuleLayout motionBezierSplineProjectileData = { motionBezierSplineProjectileDataTokens, 5u };
const ModuleToken motionBalloonProjectileMoveToGroundDataTokens[] = {
    { 1u, 2882646154u, 0u, &motionProjectileMoveBase, 0u, 0u },
};
const ModuleLayout motionBalloonProjectileMoveToGroundData = { motionBalloonProjectileMoveToGroundDataTokens, 1u };
const ModuleChoice motionChoices[] = {
    { 290244143u, &motionProjectileUpdateModuleData, 28u, 80u },
    { 358254523u, &motionDynamicsUpdateModuleData, 32u, 81u },
    { 4230098692u, &motionFreefallUpdateModuleData, 80u, 82u },
    { 4235893941u, &motionEnableDynamicsCollideModuleData, 12u, 83u },
};
const ModuleChoice motionMoveChoices[] = {
    { 2240593884u, &motionProjectileMoveInStraightLineData, 16u, 0u },
    { 2272689654u, &motionProjectileMoveToGroundData, 28u, 0u },
    { 3012512132u, &motionBezierSplineProjectileData, 24u, 0u },
    { 442194537u, &motionBalloonProjectileMoveToGroundData, 4u, 0u },
};
