# GameObject 经济与电力

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；46个精确原方法、literal、尾LF body SHA256与helper闭包见 `evidence/gameobject-modules/economy-power-source/manifest.json`。字段/Token由 `GenerateEconomyPowerFields.py` 对照原方法再生；实现位于对应metadata头和独立 `GameObjectEconomyPowerRuntime.h`。

| 完整类型 | TypeId | native | kind |
|---|---:|---:|---:|
| PowerUpdate | 4100895620 | 24 | 133 |
| PlayerPowerManagerBehavior | 771889415 | 16 | 134 |
| AutoDepositUpdate | 1300449569 | 48 | 135 |
| DistributedMoneyBehavior | 1023139366 | 20 | 136 |

已按TypeId全ModuleChoice元数据去重。四者Marshal直接ModuleData；前三者Token父Update，DistributedMoney父Create，不得按Behavior名称推断父层。

- Power：UpdatePeriod Time@8=1s、ReloadTime Time@12=0s、Duration Time@16=0s、Type单枚举@20=INVALID。Duration仍是TimeTag float，不是逻辑帧整数。
- PlayerPowerManager：SpecialPowerTemplate强引用@8无默认，OnAvailableEvaEvent强引用@12默认空；模块endian只交换ModuleData两字，引用最终平台字节由TrackerFinalize处理。
- AutoDeposit：DepositInterval Time@8=0.0s，Flags位集@12=NONE，TransactionCategory单枚举@16=AUTO_DEPOSIT；InitialCaptureBonus/Deposit/ResourceCap三列表头@20/@28/@36；GiveNoXP/OnlyWhenGarrisoned bool@44/@45=false。
- DistributedMoney：Capacity unsigned@8=1、OnDieSpawnPercentage Percent@12=1.0（最终0.01）、RedistributeMoneyOnDeathType单DeathType@16=SUICIDED，未知33。

三列表按schema次序分配，不按XML顺序。MoneyTransaction为8字节，Account unsigned@0、Amount unsigned@4，均无fallback；每列表一次分配8*count，每项生成两字段Token表。endian交换两值、列表count，指针不交换。

原PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。只读probe取得完整2/2/30表，保存在 `GameObjectEconomyPowerEnums.h`；精确probe运行锚点由主代理补齐。PowerType BACKUP_GENERATOR=0、INVALID=1，未知回退0；AutoDepositFlags NONE=0、ACTIVE_WHEN_REPAIRING=1，未知bit1、ALL覆盖整word，因此默认NONE的mask为1；MoneyCategory AUTO_DEPOSIT=4，未知22（AI_MONEY_LENDER）。不能把单枚举回退值和位索引混为一谈。

测试 `GameObjectEconomyPowerCases.cs` 覆盖完整枚举词、单值/位集边界、unsigned金额、三列表次序及旧回归。最终仍须/Od、/O2、平台0/1/2/99、三缓冲区逐字节、独立布局、拒绝与不变性；组件成功不能冒充SDK或完整GameObject支持。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
