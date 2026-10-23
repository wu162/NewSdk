# 下一完整组：Body全部九种XSD类型

状态：本页是原源码调查，**其余七种Body尚未实现/差分**。当前仅InactiveBody、PropBody及公共BodyModuleData已随31类型公共组通过。源Tokenizer blob仍为`6e40a9ed6c9f961d294d49768d911e6a6f354619`；`GAMEOBJECT_MODULES.md`和source-inventory.json给出精确方法入口。

## 完整范围与原方法

| XSD元素 / 类型去掉ModuleData后缀 | TypeId | nativeSize | Marshal包装 | Token包装 | endian包装 |
|---|---:|---:|---:|---:|---:|
| ActiveBody | 1510882715 | 76 | 62310 | 103509 | 65022 |
| ShieldBody | 3545651287 | 108 | 61454 | 102795 | 64241 |
| ImmortalBody | 2296991610 | 76 | 61837 | 103104 | 64596 |
| HighlanderBody | 1863294802 | 76 | 60763 | 102156 | 63560 |
| InactiveBody（已完成） | 690179694 | 8 | 61872 | 103147 | 64616 |
| PropBody（已完成） | 4287119435 | 8 | 60845 | 102239 | 63635 |
| ManagerBody | 1905798828 | 76 | 62839 | 103966 | 65552 |
| ManagedActiveBody | 3993857585 | 76 | 60508 | 101927 | 63329 |
| ManagedImmortalBody | 3834377004 | 76 | 62690 | 103839 | 65426 |

五个76字节派生类的Marshal包装直接调用ActiveBody Marshal，并写自己的TypeId；**Token继承链并不全部相同**：Immortal/Highlander/ManagedActive→Active，Manager→Immortal，ManagedImmortal→ManagedActive。要按原Token方法生成表，不能仅用相同nativeSize推定完全相同Token。

## ActiveBody依赖组

原Marshal18095，endian紧随其后；Token81673起（18记录：基类+16字段/子表+sentinel）。分配76字节，共用ModuleData前8字节。

| 字段 | 偏移 | 原Marshal默认/规则 |
|---|---:|---|
| MaxHealth | 8 | float，0 |
| MaxHealthDamagedPercentage | 12 | 66.66666，经`float(atof)*0.01f` |
| MaxHealthReallyDamagedPercentage | 16 | 33.33333，同上 |
| InitialHealth | 20 | float，-1.0 |
| DodgePercent | 24 | float，0；**虽然名字含Percent，原代码不乘0.01** |
| EnteringDamagedTransitionTime | 28 | Time，0s |
| EnteringReallyDamagedTransitionTime | 32 | Time，0s |
| GrabObject | 36 | 内联8字节String，缺失无分配 |
| HealingBuffFX | 44 | inline resource word，缺失无引用 |
| DamagedAttributeModifier | 48 | inline resource word |
| ReallyDamagedAttributeModifier | 52 | inline resource word |
| CheerRadius | 56 | float，200.0 |
| BurningDeathFX | 60 | inline resource word |
| DamageRangeModifier | 64 | count+pointer，12字节条目 |
| RemoveUpgradesOnDeath | 72 | bool，false |
| BurningDeathBehavior | 73 | bool，false |

Marshal的查找次序与上表偏移排序不完全相同：两个bool先于BurningDeathFX，然后DamageRangeModifier列表，最后ModuleData id。分配上GrabObject先于列表；资源word不当作字符串分配。

ActiveBodyDamageRangeModifier原Marshal18073、列表Marshal53895、列表endian53937、Token81652、列表Token100007。条目12字节：Min@0、Max@4均`float(atof)*0.01f`且无原lookup默认；AttributeModifier资源word@8。完整保留列表0/1/多/长与resource边界。Token为3字段+sentinel。还须完整核对endian helper与资源包装再实现，不能把本调查当成已通过结果。

## ShieldBody依赖组

原Marshal18278/endian18312；原包装分配108字节。扩展字段：ShieldEnabledFX@76、ShieldDepleteFX@80、ShieldRechargeEndFX@84、ShieldTakeDamageFX@88均inline引用；ShieldAmount@92为float，原lookup无默认；ShieldArmor@96为**可空引用对象指针**；ShieldRechargeIdleTime@100为Time默认0s；ShieldActiveModelCondition@104为ModelConditionFlagType默认INVALID。

关键分配顺序：Shield自有字段先Marshal，**ShieldArmor可空对象先分配**，之后调用ActiveBody，才分配GrabObject及DamageRangeModifier列表。不能先公共基类后扩展。ModelCondition的scalar unknown/default用原helper与PE枚举表核对，不按bitset处理。

## 实施和验收

- 按七个剩余Body、全部Active/Shield字段、DamageRangeModifier完整依赖一次实现/统一矩阵。原模块描述目前固定8字节分配，`FindModule`+ModuleChoice只针对已完成公共类型；不能仅把新TypeId加到choices就当成支持。要扩展native size/字段编译和桥接字段白名单，并更新unsupported矩阵。
- 复用scalar、string、resource、relocation与已有ModelCondition源证据；需逐项核对helper，包括百分比舍入、可空资源、数组分配/Token/endian。原源码查完后按每个实际TypeId差分，不以ActiveBody通过替代其他派生Token链。
- 同时覆盖全部六容器、七种实际TypeId、缺省/显式0/空引用、所有字段边界、混合模块列表、XML顺序不变性；保留已完成31类型和其余未支持类型拒绝。
- 整组一次/Od、/O2、0/1/2/99、三缓冲区逐字节差分与独立布局/Token/sentinel。真实SDK继续累计，未正式GameObject路由启用前不宣称集成完成。
