# GameObject Tiberium资源组

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；66个精确模块/helper/wrapper方法与指纹见 `evidence/gameobject-modules/tiberium-source/manifest.json`。`GenerateTiberiumFields.py` 核对完整字段、Token与来源；实现分别在metadata头及独立 `GameObjectTiberiumRuntime.h`。无新PE表。

| 完整类型 | TypeId | native | kind | 父合同 |
|---|---:|---:|---:|---|
| TiberiumGrowthModBehavior | 2923073644 | 60 | 147 | Marshal/Token均Die |
| TiberiumCrystalBehavior | 254628242 | 32 | 148 | Marshal Module / Token Update |
| TiberiumFieldBehavior | 1808399630 | 76 | 149 | Marshal/Token均Update |

按TypeId全ModuleChoice元数据去重。GrowthMod Green/BlueTiberiumMultiplier为float@52/@56，均1.0；保留完整Die52父层，DieMuxData为inline44@8及其全部状态/Death位集/字符串列表依赖。先写本层两值，再处理原父层；endian本层及父层各一次。

Crystal NumBoxes/ValuePerBox/GrowthStages为unsigned@8/@12/@16，默认0/0/1；TimeBetweenGrowthStages/GrowthTimePerStage为Time@20/@24，默认5s/3s；RadarColor为packed UInt@28，仅RadarColor子节点存在才处理。原ColorTag大写R/G/B默认0、A默认255，每通道strtoul后截断为byte（不是clamp），按ARGB打包。缺省node保持word0；显式空node为FF000000；R=300取44，-1取255。Token仅bookmark完整颜色word，不产生颜色子表；endian交换该word。不能与EMP小写Color3f混用。

Field：Damage/HealUpdateFrequencySeconds为Time@8/@12，默认0.67s/0.60s；Radius/DamageRadiusAdd/HealRadiusAdd float@16/@20/@24，默认0/20/20；FieldResolution/MaxFieldMoney/StartingFieldMoney为signed@28/@32/@36，默认40/0/0；ThingToSpawn为弱GameObject typed-ID@40无默认；SpawnOffset/ClusterScaleFactor/ClusterPowerFactor float@44/@48/@52，默认0.0/1.0/1.0；CrystalGrowthRate signed@56=0（非时间），LifetimeFieldMoney unsigned@60=0；DamageFilter/HealFilter nullable112@64/@68；KillWhenEmptyAndFinishedSpawning/AllowStartingFieldCrystalsToGrow bool@72/@73=false，padding保持零。

Field固定先根属性，再DamageFilter→HealFilter分配，原XML顺序不改变buffer布局；两个Filter Token均为可空完整表(mode7)，endian递归处理Filter。Bridge kind149对两路径使用严格ReadDependencyFilter；Growth只允许DieMuxData，Crystal只允许RadarColor及大写RGBA，拒绝未知属性、重复节点、外来namespace和不适用嵌套。

测试 `GameObjectTiberiumCases.cs` 覆盖signed/unsigned边界、弱ID、时间、完整Die父与过滤器、packed颜色/缺省差异、schema分配顺序及拒绝。验收仍区分组件、SDK与完整替代DLL。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
