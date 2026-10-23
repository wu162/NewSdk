# SkirmishAIInformation：下一完整能力组的源码调查

状态：**尚未实现，未进行此组原 DLL 差分，不能作为支持声明。** 2026-10-23 在完成29类GameObject子节点后预先整理，供下一轮直接定位；当前恢复组件仍显式拒绝SkirmishAIInformation。

证据：原Tokenizer `-Module-.cs` blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`，布局/调用次序来自该源码，枚举表来自原PE与源码映射。XSD表只记录声明契约；最终缺省、未知值、拒绝与输出仍须原版探针及完整矩阵验证。不要用下表的XSD缺省代替Marshal取值规则。

## 必须一起恢复的依赖

| 结构 | 大小与位置 | Marshal / endian / Tokenize |
|---|---|---|
| SkirmishAIInformation | 88字节；GameObject指针@460；wrapper66069 | 49752 / 49891 / 97167（25项+sentinel） |
| AIContextualPower / AISpecialPowerInfo | 172字节条目；列表头@60；list65746 | 25254 / 25393 / 80506（30项+sentinel） |
| AIBaseSelectionCriteria | 20字节；两可空指针@68/@72 | 24697 / 24729 / 80181（5项+sentinel） |
| AISpecialPowerWeaponData | 28字节；special power指针@160；wrapper56011 | 24780 / 24816 / 80210（10项+sentinel） |
| ObjectFilter | 复用已验证112字节实现；root DodgeWithUnits@76、special power ObjectFilter@156 | 现有ObjectFilter.{h,cpp}；仍需整合分配/Token顺序回归 |

根分配顺序：默认/标量与可空override → AIContextualPower全部条目（每条含自身可空值/资源/过滤器/武器）→ ConstructionBaseSelectionCriteria → ResourceNodeBaseClosenessTestSelectionCriteria → DodgeWithUnits。不能只放开88字节外壳或只支持没有AIContextualPower的空对象。

## SkirmishAIInformation

`SDK/Schemas/xsd/AssetTypeGameObject.xsd`；原Marshal从49752行起。偏移均为字节，*表示可空指针，字符串等结构不能当成普通单word。

| 属性 | 原布局偏移 | XSD类型 | XSD缺省/约束 |
|---|---|---|---|
| BaseBuildingLocation | 0 | SkirmishAIBaseLocation | `FRONT` |
| AdditionalRotation | 4 | Angle | `0r` |
| BaseBuildingMaxNudges | 8 | SageInt | `3` |
| BlockIfUnpackTargetBuildingCannotBeRebuilt | 80 | SageBool | `true` |
| UnitBuilderStandardCombatUnit | 81 | xs:boolean | `false` |
| TargetDuringCrush | 82 | xs:boolean | `false` |
| CrushSpeedBonus | 12 | SageReal | `0.0` |
| OutOfRangePenalty | 16 | SageReal | `1.3` |
| ContainedMembersProvideWeapon | 83 | xs:boolean | `false` |
| ConquerMetricsOverrideDPS | 20* | xs:float | optional |
| ConquerMetricsOverrideDamageType | 24 | DamageType | `UNDEFINED` |
| ConquerMetricsOverrideAntiMask | 28 | WeaponAntiBitFlags | `ANTI_GROUND ANTI_WATER` |
| ConquerMetricsOverrideForbiddenAntiMask | 32 | WeaponAntiBitFlags | 空字符串 |
| ConquerMetricsOverrideDefendingAntiMask | 36* | WeaponAntiBitFlags | optional |
| ConquerMetricsDpsBonusAgainstDefenses | 40 | xs:float | `1.0` |
| NearResourceNodeType | 44 | EconomyResourceType | `ORE` |
| DodgeSafetyDistance | 48 | SageReal | `300.0` |
| OverrideInnerDistanceToTarget | 52* | SageReal | optional |
| OverrideOuterDistanceToTarget | 56* | SageReal | optional |
| CanFireUpCliffs | 84 | SageBool | `false` |
| DoesSecondaryDamage | 85 | SageBool | `false` |

## AISpecialPowerInfo

`SDK/Schemas/xsd/SkirmishAI/AssetTypeAISpecialPower.xsd`；原Marshal从25254行起。偏移均为字节，*表示可空指针，字符串等结构不能当成普通单word。

| 属性 | 原布局偏移 | XSD类型 | XSD缺省/约束 |
|---|---|---|---|
| id | metadata only | Poid | 省略时无声明缺省 |
| Heuristic | 0 | AISpecialPowerHeuristic | required |
| Manager | 4 | AISpecialPowerManager | required |
| Distance | 8 | xs:float | `100.0` |
| HealthFraction | 12 | xs:float | `0.0` |
| TargetObjectInclude | 16 | KindOfBitFlags | 省略时无声明缺省 |
| TargetObjectExclude | 52 | KindOfBitFlags | 省略时无声明缺省 |
| UseOnlyWhenPowerAdvantageIsAbove | 88* | SageReal | optional |
| NeverUseWhileRetreating | 164 | SageBool | `false` |
| NeverUseWhilePullingBack | 165 | SageBool | `false` |
| InvulnerabilityDamageLookAheadTime | 92 | Time | `1s` |
| InvulnerabilityDamageLookBehindTime | 96 | Time | `1s` |
| InvulnerabilityDamageFractionWhenOutOfRangeOfTarget | 100 | SageReal | `.3` |
| InvulnerabilityDamageFractionWhenInRangeOfTarget | 104 | SageReal | `.6` |
| InvulnerabilityAvoidUseIfAngleToNextPathNodeIsMoreThan | 108 | Angle | `360d` |
| MaxTargetSpeed | 112 | SageReal | `99999.0` |
| RequireKillTargets | 166 | SageBool | `true` |
| MinKillValue | 116 | SageReal | `0.0` |
| MinTargetsHit | 120 | SageInt | `1` |
| MinEnemiesNearby | 124 | SageInt | `1` |
| NextLogicCommand | 128* | LogicCommandRef | optional |
| UseInMicroManagerState | 132 | MicroManagerStateList | 空字符串 |
| UseNearTacticalTarget | 167 | SageBool | `false` |
| UseWhenOutOfAmmo | 168 | SageBool | `false` |
| UpdateObject | 136* | GameObjectRef | optional |
| SearchRadius | 140 | SageReal | `150.0` |
| Location | 144 | AITargetLocationBitFlags | `Water Land` |
| MaxUpdatables | 148* | SageInt | optional |
| UseToCounterGarrison | 152* | SageBool | optional |

此Marshal方法没有对`id`作字段写入；它是XSD元数据，不能凭属性顺序插入原生字节。原Token只有28个有效属性字段及2子对象。`NextLogicCommand`和`UpdateObject`是可空引用对象，不是内联资源word；`UseToCounterGarrison`是可空bool，不能合并到普通尾部bool。

## AISpecialPowerWeaponData

`SDK/Schemas/xsd/SkirmishAI/AssetTypeAISpecialPower.xsd`；原Marshal从24780行起。偏移均为字节，*表示可空指针，字符串等结构不能当成普通单word。

| 属性 | 原布局偏移 | XSD类型 | XSD缺省/约束 |
|---|---|---|---|
| Weapon | 0 | WeaponRef | required |
| IsDisablingAttack | 24 | SageBool | `false` |
| AffectsFriendlies | 25 | SageBool | `false` |
| AffectsSelf | 26 | SageBool | `false` |
| TotalValueGreaterThan | 4 | SageInt | `0` |
| AoeAttackPowerLocationPreference | 8 | AIAoeAttackPowerLocationPreference | `PREFER_NEAREST` |
| AoeAttackPowerBlockAreaAfterActivationTime | 12 | Time | `0s` |
| AoeAttackEmanationLocation | 16 | AIAoeAttackPowerEmanationLocation | `EMANATE_FROM_SELF` |
| ToggleRangeHysteresis | 20 | SageReal | `0.0` |
| ToggleShortRangeTargetForcesTarget | 27 | SageBool | `false` |

## AIBaseSelectionCriteria

`SDK/Schemas/xsd/SkirmishAI/AIBaseSelectionCriteria.xsd`；原Marshal从24697行起。偏移均为字节，*表示可空指针，字符串等结构不能当成普通单word。

| 属性 | 原布局偏移 | XSD类型 | XSD缺省/约束 |
|---|---|---|---|
| RequiredBaseTypes | 0 | AIBaseTypeBitFlags | 空字符串 |
| ExcludedBaseTypes | 4 | AIBaseTypeBitFlags | 空字符串 |
| PreferredBaseTypes | 8 | AIBaseTypeBitFlags | 空字符串 |
| PreferredNotBaseTypes | 12 | AIBaseTypeBitFlags | 空字符串 |
| SortOrder | 16 | AIBaseSortingOrder | `PREFER_NEWEST` |

## 枚举与位集调查入口

| 类型 | 原映射数量 | 原标量unknown | helper起始行 |
|---|---|---|---|
| SkirmishAIBaseLocation | 7 | 2 | 65712 |
| DamageType | 39 | 38 | 51393 |
| EconomyResourceType | 4 | 3 | 65735 |
| AISpecialPowerHeuristic | 25 | 4 | 56048 |
| AISpecialPowerManager | 4 | 3 | 56059 |
| AIBaseSortingOrder | 7 | 6 | 55924 |
| AIAoeAttackPowerLocationPreference | 3 | 1 | 55957 |
| AIAoeAttackPowerEmanationLocation | 2 | 0 | 55968 |

WpnAntiT为14位、MicroManagerState为10位、AITargetLocationType为2位、AIBaseType为10位（原名字表有11项，`PRIMARY_TYPE_MAX=2`与`CAPTURED=2`为同值名称）。四个原位集helper的unknown、ALL、负号XOR、越界与padding已在末节补齐源码对照，尚待恢复与差分；不能按枚举表长度猜bitCount。KindOf可复用当前原始284位表/解析器，但必须新加AISpecialPowerInfo内联include/exclude的独立布局与差分。

## 整组测试门槛

- 根21属性、special power有效28属性、weapon10属性、base selection5属性全部覆盖；所有枚举名及空/unknown/大小写/边界；id/TypeId仅按原输入链规则处理。
- 明确区分缺失可空值与显式0、false、空字符串；尤其float*、bool*、nullable enum-bitset、AssetReference*。测试所有资源索引边界及插入排序带来的重定位顺序。
- 全部列表空/单/多/长，两个base子对象和两个ObjectFilter，嵌套SpecialPowerWeapon，schema顺序分配与XML顺序不变性；混合已支持GameObject组。
- 非命名空间TypeId允许，外来命名空间/未知字段/未知结构/重复单例/结构文本显式拒绝。
- 全组准备后统一/Od、/O2、0/1/2/99、三缓冲区差分、独立布局/Token/sentinel和拒绝矩阵。当前没有此组成功结果；不以源码调查代替原版差分。真实SDK继续按EFFICIENCY.md累计。

## 后续位集helper源码核对补充

已完整读取并对照四个118行原始helper，规范化类型、名字表数量、bitCount、unknown常量与IL标签后控制流相同。此结论来自源码结构比较，仍不是恢复实现或新增原DLL差分结果。

| 位集 | helper行 | 名字数 / bitCount | unknown置位 |
|---|---|---|---|
| WpnAntiT | 68313 | 14 /14 | 7 |
| MicroManagerState | 70165 | 10 /10 | 0 |
| AITargetLocationType | 70046 | 2 /2 | 1 |
| AIBaseType | 69927 | 11 /10 | 2 |

这些helper均先按原词分隔符解析，普通名称大小写敏感；`+`为OR、`-`为XOR。`ALL`大小写不敏感（去掉前导符号后判断），直接把整个32位word设为0xffffffff，包含padding；`-ALL`同样设满而不是清空。ALL分支不推进token指针，因此剩余循环反复处理ALL而不再处理后续token；新测试必须包括 `ALL -UNKNOWN`、`-ALL`、重复负号XOR及AIBaseType同值名称。不是bitCount范围掩码。已知枚举值若为-1或>=bitCount则跳过；unknown落入表中fallback位。

可复用现有MarshalEnumBitset逻辑，但仍要为本组各字段补齐新布局/可空包装、XOR/ALL/padding的独立断言及完整差分，不能仅凭控制流相同省略验证。
