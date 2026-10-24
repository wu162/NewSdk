# 完整SpecialAbility武器、光束与状态派生

2026-10-24实现；组件验收状态以RESUME.md精确sourceCommit/runId为准，未经完整矩阵成功不计入已验收数量。SDK原路由保留，本组不宣称正式插件或完整DLL替代通过。

固定原Tokenizer源码blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`，32完整方法及直接装饰helper闭包在`evidence/gameobject-modules/ability-weapon-next-source/manifest.json`。`ExportAbilityWeaponMethods.py --check`从固定完整原源码重取逐项核对；`GenerateAbilityWeaponFields.py --check`核对方法指纹、完整属性顺序/默认值、Token字段与native偏移、父层及sentinel，生成`GameObjectAbilityWeapon.h`。复用完整SpecialAbility240、Anim12、WeaponSlotType6、ModelCondition457单枚举和ObjectStatus223合同。

| 类型 | TypeId | native | 父层 |
|---|---:|---:|---|
| WeaponFireSpecialAbilityUpdateModuleData | 507880339 | 272 | SpecialAbility240 |
| DelayedWeaponFireSpecialAbilityUpdateModuleData | 915350037 | 276 | WeaponFire272→SpecialAbility240 |
| BeamSpecialAbilityUpdateModuleData | 2695497697 | 272 | SpecialAbility240 |
| ObjectStatusSpecialAbilityUpdateModuleData | 127626925 | 296 | SpecialAbility240 |

WeaponFire完整11属性：SpecialWeapon强引用240；WhichSpecialWeapon signed244默认0；SkipContinue bool264 false；BusyForDuration Time248默认0s；NeedLivingTargets265、PlayWeaponPreFireFX266、OffsetTargetLocationFromObject267 bool false；WeaponSlotID signed252默认1；WeaponSlotType单枚举256默认PRIMARY_WEAPON（未知5）；DestealthClosestObjectMatchingFilter **bool268** false；ModelConditionsOnUnpack **单枚举260**默认INVALID=-1（未知233），不能当Filter或mask。尾padding269–271为0。Delayed先自身FireDelay Time272默认0s，再完整WeaponFire和完整SpecialAbility。

Beam完整10属性：DamagePerSecond float240无默认；JoinWithOtherBeams bool268 false；原名DissapateWhenTargetDead bool269 true；LineType bool270 false；TargetAttributeModifierAdd强引用244；TargetObjectStatusAdd nullable223位status28指针248；SweepFX强引用252；ReflectorExtendDistance float256默认0；TargetSamePlayerOnly bool271 false；PreferredTargetBone String8@260。nullable status先分配28，再自身String8文本，然后父的SpecialObjectAttachToBone、ContactPointOverride两文本和nullable Anim12，不能按XML顺序分配。

ObjectStatus完整两属性Status1/Status2为inline223位mask28@240/268，之后完整父类。四类自身Token有效数12/2/11/3，各加sentinel；Delayed保留独立WeaponFire父表，后接47字段SpecialAbility表和Update父层。Beam nullable status Token指向实际28字节值，非指针槽；父Anim保留完整子表。强引用由Tracker终结，Endian不交换强引用。

实现入口`GameObjectAbilityWeaponRuntime.h`；分派、属性白名单、递归CustomAnimAndDuration分别接入既有Module运行时。`GameObjectAbilityWeaponCases.cs`覆盖完整自身属性、Delayed全部继承WeaponFire属性、四类全部45父属性、所有模型/状态枚举与边界、Anim三个字段、所有模块容器、0/1/2/27/28/29/65列表、非法大小写/命名空间/未知字段/伪Filter/跨类型属性、独立默认/布局/父层/nullable/字符串/引用/padding以及属性顺序不变性。统一完整旧回归、/Od和/O2、平台0/1/2/99、三公开缓冲区逐字节比较；不以组件成功代替SDK验收。
