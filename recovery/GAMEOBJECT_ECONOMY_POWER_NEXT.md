# 下一完整能力组：经济与电力（四种仍拒绝）

状态：仅原方法取证，尚未实现或计入已验收模块。原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；46个精确模块/helper方法及包含尾LF的body SHA256已核对，见 `evidence/gameobject-modules/economy-power-source/manifest.json`。不要把取证或PE探针成功计为支持。

## 先按真实类型与ID去重

PowerPlantUpdate、GettingBuiltBehavior、RebuildHole在本固定原blob无精确类型/方法，不能按其他游戏/版本的名称猜别名或布局。下列四个实际类型已按ID对现有ModuleChoice元数据核对（本轮9新增源码922f446及其既存基线），均尚无已支持choice，仍在已知Marshal拒绝范围。

| 完整类型 | TypeId | native |
|---|---:|---:|
| PowerUpdateModuleData | 4100895620 | 24 |
| PlayerPowerManagerBehaviorModuleData | 771889415 | 16 |
| AutoDepositUpdateModuleData | 1300449569 | 48 |
| DistributedMoneyBehaviorModuleData | 1023139366 | 20 |

四者Marshal均直接ModuleData。前三者Token父Update；DistributedMoney的Token父Create，不能按Behavior名称替换。PowerUpdate必须用完整精确签名匹配，不能误取CloneStoredObjectsSpecialPowerUpdate或SpecialPowerUpdate的同尾子串。

## 已核对布局和必要依赖

- PowerUpdate：UpdatePeriod Time@8默认1s、ReloadTime Time@12默认0s、Duration Time@16默认0s、单PowerUpdateType@20默认INVALID。名为Duration仍是TimeTag float，不是逻辑帧整数。
- PlayerPowerManager：SpecialPowerTemplate强引用@8无默认；OnAvailableEvaEvent强引用@12默认空字符串。无独立endian overload，polymorphic wrapper内联仅交换ModuleData两字；最终引用字节仍由公共TrackerFinalize处理，不能据模块不swap推断最终little-endian。
- AutoDeposit：DepositInterval Time@8默认0.0s；Flags位集@12默认NONE；TransactionCategory单枚举@16默认AUTO_DEPOSIT；三份MoneyTransaction列表头依次@20 InitialCaptureBonus、@28 Deposit、@36 ResourceCap；GiveNoXP bool@44与OnlyWhenGarrisoned bool@45均false。列表按上述schema顺序分配。MoneyTransaction为8字节（Account unsigned@0、Amount unsigned@4，均无fallback），不是signed金额、资源引用或nullable对象。每列表一次分配8*count，逐项Marshal；Token逐项生成两字段表，endian交换两值及列表count。
- DistributedMoney：Capacity unsigned@8默认1；OnDieSpawnPercentage float@12按原atof转float后乘0.01f，默认文本1.0故结果0.01（不是1.0）；RedistributeMoneyOnDeathType单DeathType@16默认SUICIDED，未知33。复用既有完整DeathType表。不得误用Death bitset。

全部原Marshal/Token/endian、polymorphic wrappers、实际MoneyTransaction列表/标量helper、时间/布尔/引用及各Token父层均在46方法证据内；AutoDeposit的endian列表helper已单独包含，避免只提取Marshal/Token造成遗漏。未发现还缺的typed局部helper；下一轮实现前仍按manifest精确签名重核调用闭包。

## 尚缺PE表和最先验证项

`enum-status.json`保存原source静态table字节、原枚举entry count与未知回退；仍需从相同原PE解码字符串地址取得完整名称/数值表：

| 原PE组 | entries | 原未知行为 |
|---|---:|---|
| PowerUpdateType | 2 | 单值0 |
| AutoDepositFlagsType | 2 | bit index1；ALL整word置FFFFFFFF，负词为XOR |
| MoneyTransactionCategory | 30 | 单值22 |

不要据schema名称顺序补表。可复用 `GenerateUpgradeFields.enum_values` 在已有TestGameObject作业内附带只读探针，随准备好的相关代码批次一次取表；不必新建云端导出工作流。表返回后完整覆盖每个词及未知/空/ALL/正负词边界。优先独立验证DistributedMoney百分比默认0.01、AutoDeposit两unsigned字段及三列表schema分配顺序、Power Duration的Time单位、Distributed Create父链、强引用最终平台字节。

四种及MoneyTransaction依赖作为完整批次连续实现/编写测试，完成后统一 /Od、/O2、平台0/1/2/99、三缓冲区逐字节差分、独立布局与拒绝边界。必须保留其他已知Marshal的明确拒绝，不重复恢复既存模块或扩展正式SDK支持声明；完整GameObject/替代DLL/正式插件仍未完成。

