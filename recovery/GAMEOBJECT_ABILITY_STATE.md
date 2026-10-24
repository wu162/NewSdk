# 完整SpecialAbility状态与对象操纵派生

2026-10-25。七类共用完整SpecialAbility240父层；组件最终验收源码/run见RESUME.md，不能把组件通过称为SDK集成。原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；`ability-state-source/manifest.json`归档52完整原方法及直接装饰helper闭包，ExportAbilityStateMethods.py重取核验。GenerateAbilityStateFields.py核验字段顺序/缺省、父层/Token/sentinel、方法指纹，并从SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`原PE读取ReplaceSelfOptions7。

| 类型 | TypeId | native | 完整自身合同 |
|---|---:|---:|---|
| GiveOrRestoreUpgradeSpecialPower | 72607304 | 252 | CommandButton String8@240；UpgradeToGive强Upgrade@248，无默认 |
| ReconstituteStateSpecialAbility | 1067163377 | 260 | UpdateModuleId typed Module ID@240；TimeStepsBack Time244默认0s；LeavePositionFX/ReturnPositionFX/PostReconstituteAttributeModifier强引用248/252/256 |
| DominateEnemySpecialPower | 2603081264 | 260 | DominateRadius float240无默认；TriggerFX/DominatedFX强FX244/248；PermanentlyConvert bool256=false；AttributeModifierAffects nullable Filter112指针252 |
| LevelGrantSpecialPower | 2431669529 | 256 | Experience signed240=0；RadiusEffect float244=0；LevelFX强FX248；AcceptanceFilter nullable Filter112指针252 |
| CloneStoredObjectsSpecialPowerUpdate | 3455002493 | 264 | Min/MaxDestinationRadius float240/244=0；TriggerAttributeModifierOnClones/TriggerFX/TargetFX强引用248/252/256；ClonedObjectLifetime Time260=0s |
| ReplaceSelfSpecialAbility | 2434933121 | 260 | NewObjectUnpackTime Time240=0s；ReplaceOptions bits7@244=DISABLE_DURING_REPLACE；ClearTriggerDistance float248=120；ReplacementTemplate typed GameObject文本列表252 |
| AssembleStructureSpecialAbilityUpdate | 2000137321 | 260 | NewObjectAssemblyTime Time240=0s；BusyDuringAssembly bool256=false；ModelConditionsOnAssembly单枚举244=INVALID；StructureTemplate typed GameObject文本列表248 |

类型名带SpecialPower不意味着继承SpecialPower452：以上七类父层全部是SpecialAbility240。自身String8、nullable Filter或typed列表先于完整父层分配；父两String8再Anim12。两个typed列表每项4字节、ASCII小写后HashAssetId，不是强引用；Token是count头+直接值bookmark，无每项结构表。Filter Token保留完整ObjectFilter结构，强引用Endian由Tracker处理。

ReplaceSelfOptions原PE：COPY_TARGET_OBJECT=0、IGNORE_TERRAIN_RESTRICTIONS=1、COPY_UPGRADES=2、DISABLE_DURING_REPLACE=3、CHECK_BUILD_ASSISTANT=4、DISABLE_NEW_OBJECT_DURING_UNPACK=5、CLEAR_LOCATION=6。未知bit4；普通负词XOR，ALL完整32位覆盖并忽略后续词。Assembly单ModelCondition未知233，不是456位位集。Reconstitute的PostReconstituteAttributeModifier完整拼写来自原始literal，不能使用截断MSVC符号。

GameObjectAbilityStateCases.cs覆盖所有自身字段/完整父字段/父Anim12、默认值、单枚举、typed Module ID大小写、完整Filter、字符串分配、列表0/1/2/27/28/29/65和跨分配模块列表、三缓冲区逐字节差分、独立布局/原Token键/父表/sentinel、拒绝与不变性。保留全部旧回归、/Od和/O2、平台0/1/2/99。实现位于GameObjectAbilityState.h及GameObjectAbilityStateRuntime.h；Bridge只放开已实现结构。
