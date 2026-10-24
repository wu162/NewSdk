# SpecialPower公共父与Radar子类

本组2026-10-24实现，待完整Windows矩阵验收。此前193种已验收；本组3种未验收，不能提前计数。SDK未运行，原路由保留。

| 类型 | TypeId | native | kind |
|---|---:|---:|---:|
| SpecialPowerModuleData | 3472757011 | 452 | 162 |
| RadarHackSpecialPowerModuleData | 3201673786 | 464 | 163 |
| RadarJamSpecialPowerModuleData | 3870511801 | 460 | 164 |

证据固定原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；`evidence/gameobject-modules/special-power-base-source/manifest.json`完整78方法、body含尾LF的SHA256。生成器核对原blob、每个方法及三个PE枚举全表，导出模式只用于本地，不代替Windows完整检查。

父Marshal000：34根属性依原schema顺序，尾部bool436–448，padding449–451为零；WeatherDuration为ceil(float(double(0.005f)*double(ParseFloat)))帧数，GiveLevels有符号，BurnDecayModifier无符号。OnTriggerRechargeSpecialPower是小写hash弱typed-ID，其余SpecialPower/PlayerTech/FilterAsset均强引用；默认CanAffectObjectFilter资源不含反斜杠时不产生引用值。单枚举SetModelCondition未知233、Weather未知1、Dist未知0；Disabled12未知bit0、默认HELD mask8、负词XOR、ALL分支不前进。

三个真正inline Filter112固定@96/@208/@320，缺省不Marshal但完整Token表仍输出；显式节点复用完整ObjectFilter，包括可空状态位与IncludeThing/ExcludeThing列表。RequirementsFilterMPSkirmish为StringList8@32。InitiateSound为nullable8@432，内部Sound再nullable强引用4@0、EvaEvent为inline强引用@4。原Endian001不调用Sound endian，指针及强引用交Tracker；不能按大小统一交换。Token011为39字段加sentinel，声音025为2字段加sentinel，Sound Token指向nullable值而不是指针字段。父Token链为Behavior→Module，两Radar外层仍各有独立SpecialPower父表。

源码：GameObjectSpecialPowerBase.h（元数据）、GameObjectSpecialPowerBaseRuntime.h（运行）、GenerateSpecialPowerBaseFields.py（指纹/生成检查）。高kind入口独立早返回，Bridge三个Filter走严格ReadDependencyFilter。差分：GameObjectSpecialPowerBaseCases.cs，随原完整矩阵执行，保留三缓冲区、全部既有样本、平台0/1/2/99、两优化模式、独立默认/大小/Token/声音分配/枚举边界与拒绝，覆盖schema顺序不变性、列表0/1/2/27/28/29/65及混合模块。其他126已知Marshal仍拒绝，58 Token-only fallback保留。

验收结果由本组最终sourceCommit/runId补齐，未经成功不能宣称正式DLL或SDK支持。

## 首批验收

源码`8bf01e126379fa964fa04d469774daabed594e34` / GameObject run40096181697 success，基础run40096181708 success。原日志两模式各396080 fixtures、1188240三缓冲区比较、14392拒绝、1104不变性；平台0/1/2/99，78方法/原PE三表默认完整检查成功。两模式合计2376480次缓冲区比较。此时193→196；后续派生组独立等待验收。
