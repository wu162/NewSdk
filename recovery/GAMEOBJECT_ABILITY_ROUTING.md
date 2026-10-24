# 完整SpecialAbility模块路由与雷达过滤列表

2026-10-25。最终共同组件验收见RESUME.md，SDK尚未运行。两个native均248字节，完整父SpecialAbility240；自身只有List8@240，无自身根属性。

| 类型 | TypeId | 子节点 | 每项大小 |
|---|---:|---|---:|
| ActivateModuleSpecialAbilityUpdateModuleData | 3442719982 | SpecialPowerModules，可重复结构 | 12 |
| RadarScanMapSpecialAbilityUpdateModuleData | 3660533477 | IntersectionFlagsFilter，可重复结构 | 128 |

SpecialPowerModuleInfo12：ModuleId为typed Module ID@0（无默认，ASCII小写后hash）；TriggerAtTargetPosition bool8默认true；TriggerType单枚举4默认TARGET_POSITION=1，未知SELF_POSITION=0，TARGET_OBJECT=2；padding9–11。不能混用ActivateModuleSpecialPower的ActivateInfo16，后者多一个Deactivate字段且拥有自身继承Token层。

Intersection128：ObjectStatus为inline223/28字节@0；ModelCondition为inline456/60字节@28；KindOf为inline284/36字节@88；UseLocalPlayerFilter bool124默认true；padding125–127。三个掩码无默认，未知bit分别198/233/149；ModelCondition的INVALID=-1忽略。ALL覆盖完整7/15/9个word（含末word padding），普通负词XOR。不是ObjectFilter112，不使用IncludeThing等子节点。

自身结构列表先分配，再完整父String8/Anim12。Token列表为count头+每项独立结构表及sentinel，元素非多态指针。Endian逐项交换全部掩码word或ModuleId/TriggerType，然后list count，再完整父；bool与padding不交换。

固定原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；ability-routing-source/manifest.json保留26完整方法（含列表helpers和两种元素三方法中实际存在的方法）。ExportAbilityRoutingMethods.py从固定原源码重取；GenerateAbilityRoutingFields.py核验方法指纹、属性/默认值、两个wrapper native248/父240、列表stride12/128、Token与sentinel；公共枚举继续由完整矩阵原PE检查覆盖。

GameObjectAbilityRoutingCases.cs覆盖完整父字段/Anim、元素全部字段和三个全名称位集、正负/未知/ALL边界、0/1/2/27/28/29/65列表/模块、独立完整默认/padding/分配/原Token键、typed ID大小写、拒绝和不变性，保留/Od、/O2及平台0/1/2/99全部旧回归。实现与运行时分别位于GameObjectAbilityRouting.h和GameObjectAbilityRoutingRuntime.h。
