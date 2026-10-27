# Crate碰撞/生成完整八模块（已通过完整组件差分）

本组已验收。最终源码1ef49469cd7a9f290b76952c4040cd2df0a2a147 / run40072750732 success；每/Od、/O2：275032 fixtures、825096三缓冲区逐字节比较、11352拒绝、712不变性，平台0/1/2/99。validation.json与两模式实际日志、sourceCommit/runId及终态已核对。原crate-source82精确方法和PE枚举：源码1fe0f0469d20a583ecd34f14eac995db81650b58 / run40072161495 success；这是取证结果。GenerateCrateFields.py核对所有签名/bodySHA、CI原blob6e40a9ed6c9f961d294d49768d911e6a6f354619及PE枚举，生成完整字段/Token父表/sentinel。当前状态与精确sourceCommit/runId见RESUME.md。

| 模块 | TypeId | native |
|---|---:|---:|
| CrateCollide | 539465258 | 108 |
| MoneyCrateCollide | 4012457028 | 112 |
| UnitCrateCollide | 4002813588 | 116 |
| VeterancyCrateCollide | 1463665155 | 120 |
| AttributeCrateCollide | 1212396263 | 112 |
| RandomCrateCollide | 482350304 | 140 |
| SalvageCrateCollide | 505436482 | 144 |
| SpawnCrateUpdate | 1244839822 | 48 |

前七模块按实际原方法完整包含CrateCollide108父字段，KindOf双284位、typed GameObject、FX/Audio强引用、普通float及packed bool全部保留。MoneyProvided unsigned@108；UnitCount unsigned@108/UnitType typed ID@112；Veterancy RangeOfEffect unsigned@108（不是float）、AffectsUpToLevel signed@112、两bool@116/117。Attribute强引用@108默认AttributeModifier_InvulnerabilityCrate，原包装内联Marshal不得漏掉。

Random七项Chance@108–132为普通float，不是Percent，bool@136；Salvage五float@108–124、两signed资源@128/132、Upgrade强引用@136、bool@140。没有按字段名字猜概率范围，也不额外添加数值限制。

Spawn48：MaxCrates unsigned@8默认20、Time@12/16默认30s/120s、三个普通float@20/24/28默认50/30/300、Flags2位@32默认DELETE_EXPIRED_CRATES，unknown回退1。CrateList是重复XML文本子节点的强引用列表头@36，stride4，**不是属性字符串列表**；先生成完整引用列表，再nullable BlockingUnits ObjectFilter112@44。复用既有ResourceMarshaling/Tracker/ObjectFilter，不修改公共解析或排序。

GameObjectCrateCases.cs覆盖全部八模块/六容器、全字段边界、全部KindOf及Flags未知/ALL行为、reference/typed-ID/packed bool、列表0/1/2/27/28/29/65/129/257含重复引用、nullable过滤器及全部依赖、独立native尺寸/offset/default/sentinel、拒绝、XML及typed-ID大小写不变性。最终已取得/Od、/O2、0/1/2/99全GameObject三缓冲区差分成功，基础run40072750706 success；SDK未运行，原路由保留。
