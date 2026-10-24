# 治疗/维修完整四模块（已通过组件完整差分）

AutoHealBehaviorModuleData(TypeId4227225901,112)、RepairUpdateModuleData(170624835,44)、InitiateRepairDieModuleData(3337877037,32)、HealCrateCollideModuleData(2207967292,116)。源码3b2cb3d2687d06b92417c3af56f6656b45e7cc63 / run40072240199 success；每/Od、/O2：251332 fixtures、753996三缓冲区逐字节比较、11316拒绝、592不变性，四平台0/1/2/99。validation.json和两模式实际日志已核对。首轮未移除拒绝数组末尾AutoHeal编号导致测试清单失败；已修复并完整重跑，不改Marshal或降低比较。最新锚点见RESUME.md。

原blob6e40a9ed6c9f961d294d49768d911e6a6f354619；healing-source86精确方法及PE enum-source来自源码89368ce680cb7c575a9628a432a0ab65dee076e4/run40071459215 success。GenerateHealingFields.py对全部86签名/bodySHA及CI原blob/PE枚举核对，派生字段/Token/hash/offset/sentinel。共享Upgrade/DieMux/validation/ObjectFilter的既有完整实现复用，未修改共享解析、Tracker或枚举逻辑。

AutoHeal继承完整Upgrade36；HealingAmount@36与Radius@48均signed整数，两个Time@40/44；KindOf284位@52，FX refs@88/92，Time@96，nullable ObjectStatusValidation56@100，bool@104–111。先自身/validation再Upgrade CustomAnim、TriggeredBy、ConflictsWith，保持分配顺序。

RepairUpdate44：RepopThreshold普通float@8默认0.2；HealAmountPerSecond unsigned@12默认10；inline PercentOfBuildCostToRebuildInfo16@16，四percent默认50，仅显式子节点应用；inline SelfAudio8@32两强引用；bool@40/41默认true/false。缺失inline子节点保持0，不等价显式空。

InitiateRepairDie32：Time@8，FX ref@12，Options1位@16（unknown回退0）；ModelCondition enum@20默认INVALID=-1，unknown回退233；Time@24；nullable完整DieMux44@28。该类型Token父表是UpdateModuleData，以原方法为准，不按名字改成DieModuleData。

HealCrate116：Range普通float@108默认75；nullable ObjectFilter112@112先分配；再完整CrateCollide108。Crate父KindOf/KindOfNot284位@8/44，FX@80、typedGameObject@84、普通float@88/92、Audio ref@96（默认CratePickup）、bool@100–104（single-player-human默认true）。CrateCollide的独立TypeId仍拒绝，此处仅作为必要完整父依赖，不额外声称支持。

GameObjectHealingCases.cs覆盖四模块/六容器、全字段边界、全部KindOf/ModelCondition/DeathType枚举及未知行为、nullable/inline空与缺失、完整Upgrade列表与DieMux/Filter依赖、独立尺寸/偏移/默认/掩码/引用/sentinel、拒绝及XML次序不变性。受影响完整GameObject /Od、/O2、平台0/1/2/99及三缓冲区逐字节比较需全部成功；本次也包含WeaponSet完整组合场景。组件通过不代表SDK；SDK保持原路由。
