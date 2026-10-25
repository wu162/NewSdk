// Complete motion/cleanup contracts; original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.
const ModuleField motionCleanupRunOffMapBehaviorModuleDataFields[] = {
    { "RequiresSpecificTrigger", 16u, ModuleValue::Boolean, "true", nullptr },
    { "RunOffMapWaypointName", 8u, ModuleValue::String, nullptr, nullptr },
    { "DieOnMap", 17u, ModuleValue::Boolean, "false", nullptr },
    { "FlyingOffMap", 18u, ModuleValue::Boolean, "false", nullptr },
};
const ModuleField motionCleanupDestroyObjectsUpdateModuleDataFields[] = {
    { "Radius", 8u, ModuleValue::Float, "0.0", nullptr },
};
const ModuleField motionCleanupGrapplingHookUpdateModuleDataFields[] = {
    { "PullRate", 8u, ModuleValue::Float, nullptr, nullptr },
    { "MinBounceDelay", 12u, ModuleValue::Time, "0.75s", nullptr },
    { "MaxBounceDelay", 16u, ModuleValue::Time, "1.5s", nullptr },
    { "BouncePercentage", 20u, ModuleValue::Percent, "20%", nullptr },
    { "CollidingPullRateFactor", 24u, ModuleValue::Percent, "0%", nullptr },
    { "SingleFireWeapon", 28u, ModuleValue::Reference, nullptr, nullptr },
    { "ContinuousFireWeapon", 32u, ModuleValue::Reference, nullptr, nullptr },
    { "ContinuousFireWeaponBlocked", 36u, ModuleValue::Reference, nullptr, nullptr },
    { "ContinuousFireWeaponDelay", 40u, ModuleValue::Time, "0.5s", nullptr },
};
const ModuleFieldSet motionCleanupFieldSets[] = {
    { motionCleanupRunOffMapBehaviorModuleDataFields, 4u },
    { motionCleanupDestroyObjectsUpdateModuleDataFields, 1u },
    { motionCleanupGrapplingHookUpdateModuleDataFields, 9u },
};
const ModuleToken motionCleanupRunOffMapBehaviorModuleDataTokens[] = {
    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },
    { 3961999930u, 2011428087u, 16u, nullptr, 0u, 0u },
    { 1602620932u, 2153588212u, 8u, nullptr, 0u, 0u },
    { 2532629860u, 2011428087u, 17u, nullptr, 0u, 0u },
    { 1903657264u, 2011428087u, 18u, nullptr, 0u, 0u },
};
const ModuleLayout motionCleanupRunOffMapBehaviorModuleData = { motionCleanupRunOffMapBehaviorModuleDataTokens, 5u };
const ModuleToken motionCleanupDestroyObjectsUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1328662213u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 3832456114u, 3832456114u, 12u, nullptr, 8u, 0u },
};
const ModuleLayout motionCleanupDestroyObjectsUpdateModuleData = { motionCleanupDestroyObjectsUpdateModuleDataTokens, 3u };
const ModuleToken motionCleanupGrapplingHookUpdateModuleDataTokens[] = {
    { 1u, 4043318533u, 0u, &spawnUpdateModuleData, 0u, 0u },
    { 1492103970u, 1683514611u, 8u, nullptr, 0u, 0u },
    { 2689377628u, 2575722984u, 12u, nullptr, 0u, 0u },
    { 3312148179u, 2575722984u, 16u, nullptr, 0u, 0u },
    { 1124229537u, 1683514611u, 20u, nullptr, 0u, 0u },
    { 712500653u, 1683514611u, 24u, nullptr, 0u, 0u },
    { 1961592837u, 3494305058u, 28u, nullptr, 0u, 0u },
    { 3378371528u, 3494305058u, 32u, nullptr, 0u, 0u },
    { 1878558908u, 3494305058u, 36u, nullptr, 0u, 0u },
    { 159484150u, 2575722984u, 40u, nullptr, 0u, 0u },
    { 3933465695u, 1049596812u, 44u, &lifecycleObjectStatusValidationDataType, 4u, 0u },
    { 543013345u, 3832456114u, 48u, nullptr, 7u, 0u },
};
const ModuleLayout motionCleanupGrapplingHookUpdateModuleData = { motionCleanupGrapplingHookUpdateModuleDataTokens, 12u };
const ModuleChoice motionCleanupChoices[] = {
    { 2272972461u, &motionCleanupRunOffMapBehaviorModuleData, 20u, 159u },
    { 2082975044u, &motionCleanupDestroyObjectsUpdateModuleData, 124u, 160u },
    { 426756072u, &motionCleanupGrapplingHookUpdateModuleData, 52u, 161u },
};
