# 完整SlavedUpdate模块

TypeId3370197599，native112字节。原Marshal19690、Endian19814、Token87708；精确签名/原行号/SHA256见evidence/gameobject-modules/slaved-source/manifest.json，共32方法，固定原blob6e40a9ed6c9f961d294d49768d911e6a6f354619。GenerateSlavedFields.py读取原PE枚举表并校验全部字段次序/fallback、原Token表和scalar helper绑定。源生成run40047463132 success；运行验收以RESUME.md与gameobject-root/validation.json为准。

| 字段 | 偏移 | 解析 | 缺失默认 |
|---|---:|---|---|
| LeashRange | 8 | Signed | 0 |
| GuardMaxRange | 12 | Signed | 0 |
| GuardWanderRange | 16 | Signed | 0 |
| AttackRange | 20 | Signed | 0 |
| AttackWanderRange | 24 | Signed | 0 |
| ScoutRange | 28 | Signed | 0 |
| ScoutWanderRange | 32 | Signed | 0 |
| DistToTargetToGrantRangeBonus | 36 | Signed | 0 |
| RepairRange | 40 | Signed | 0 |
| RepairMinAltitude | 44 | Float | 0 |
| RepairMaxAltitude | 48 | Float | 0 |
| RepairWhenHealthBelowPercentage | 52 | Signed | 0 |
| RepairRatePerSecond | 56 | Float | 0 |
| MinReadyFrames | 60 | Signed | 0 |
| MaxReadyFrames | 64 | Signed | 0 |
| MinWeldFrames | 68 | Signed | 0 |
| MaxWeldFrames | 72 | Signed | 0 |
| WeldingSysId | 76 | Reference | 保持零/null |
| WeldingFXBone | 80 | String | 保持零/null |
| StayOnSameLayerAsMaster | 108 | Boolean | false |
| DieOnMastersDeath | 109 | Boolean | false |
| DieOnMastersDeathDamageType | 88 | Enum | UNRESISTABLE |
| DieOnMastersDeathType | 92 | Enum | NORMAL |
| FadeOutRange | 96 | Signed | 0 |
| FadeTime | 100 | Unsigned | 0 |
| MarkUnselectable | 110 | Boolean | false |
| UseSlaverAsControlForEvaObjectSightedEvents | 111 | Boolean | false |
| GuardPosOffset子节点 | 104 | 可空Vector3指针，12字节x/y/z普通float默认0.0 | null |

## 必须保留的区别

- 前9种Range及RepairWhenHealthBelowPercentage、四种Ready/Weld Frames、FadeOutRange均signed/base0；超出int32按原strtol饱和。Percentage字段37%存整数37，既不是float也不除100。
- RepairMinAltitude、RepairMaxAltitude、RepairRatePerSecond是普通float；FadeTime是unsigned/base0，不进行时间单位转换。
- WeldingSysId为强FXParticleSystem资源引用；WeldingFXBone为8字节String头，缺失保持null；字符串内容按原Relo分配。WeldingFXBone在GuardPosOffset之前分配，XML属性/坐标次序不会改变结果。
- StayOnSameLayerAsMaster@108、DieOnMastersDeath@109、MarkUnselectable@110、UseSlaverAsControlForEvaObjectSightedEvents@111为四个连续bool，各默认false，不按32位交换。
- DieOnMastersDeathDamageType是39项scalar DamageType，默认UNRESISTABLE=6、未知38；DieOnMastersDeathType是34项scalar DeathType，默认NORMAL=1、未知33。ALL也只是对应scalar值，不能替换为位集。
- Token保留Update→Contain→Behavior→ModuleData父链；实际Marshal末尾直接调用ModuleData。共29个记录再加全零sentinel。
- 原FadeOutRange/FadeTime/两bool的Token通过scalar模板helper绑定，指针算术使用int/uint单位4、bool单位1；不能把未找到byte*表达式的偏移一律视作0。
- GuardPosOffset Token调用可空Vector3 helper，非空直接bookmark坐标数据本身，缺失写null；它不是递归Vector3子表，也不是指针成员地址。

测试覆盖所有27属性、完整39/34枚举、大小写/未知/空白、signed/unsigned溢出/base0/尾缀、float特殊值、引用导入边界、字符串长度/Unicode、坐标每个分量和可空状态、0/1/2/27/28/29/65个模块、与Production/Create混合依赖、六容器、独立布局/默认/父链/Token/sentinel/分配次序和拒绝矩阵。完整/Od、/O2、0/1/2/99三缓冲区逐字节验证。SDK未运行，仍为组件。
