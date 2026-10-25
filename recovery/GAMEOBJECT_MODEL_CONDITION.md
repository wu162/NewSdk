# GameObject 模型状态监控与随机条件

状态：本轮最终完整组件矩阵已验收通过；精确源码/运行/计数见文末。本页不代表完整GameObject、完整替代DLL或SDK集成完成。

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；42个精确模块、Create/Update父层、StringList/Model位集/引用/Time helper指纹见 `evidence/gameobject-modules/model-condition-source/manifest.json`。`GenerateModelConditionFields.py` 再生独立metadata头，三种通用字段处理直接在 `GameObjectModuleDispatchRuntime.h` 分派，三ID按全ModuleChoice去重。

| 完整类型 | TypeId | native | kind | Token父 |
|---|---:|---:|---:|---|
| MonitorSpecialPowerTimerUpdate | 2376148578 | 72 | 156 | Update |
| RandomModelConditionBehavior | 3721928425 | 16 | 157 | Create |
| MonitorConditionUpdate | 267133387 | 84 | 158 | Update |

三者Marshal均直接ModuleData。PowerTimer：SpecialPowerTemplate强引用@8，ReadyCondition为60字节ModelCondition位集@12，均无默认。

RandomModel：Conditions为StringList8@8，无默认，保留完整字符串列表分配、逐字符串与mode6/stride8 Token，不是模型位集或枚举列表。必须保留Create父表。

MonitorCondition：ModelConditionFlags为60字节Model位集@8；OCL强引用@68；PreDelay/PostDelay Time float@72/@76；ActionsOnTriggered为1位、完整word存储的位集@80；全部无fallback。

Model位集复用完整原表：有效范围0..455；INVALID=-1被原helper明确忽略，不置bit31、不影响已有mask；未知词设置bit233；ALL走特殊分支将完整15word置FFFFFFFF，不能只设置表值454那一位。普通已到达的负词是XOR，但不能把此规则泛化为ALL之后仍会消费后续词。单ModelCondition枚举的回退合同不可替代此位集。

只读原PE probe取得MonitorConditionActionType唯一映射DUMP_ORE=0；原PE SHA256 `ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。unknown是bit0，ALL设置整个word；完整表应随 `GameObjectModelConditionEnums.h` 与实际probe sourceCommit/runId追溯，不猜名称或填dummy表。

**原ALL循环非推进合同：** 精确方法 `Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AMonitorConditionActionType_002C1_003E(sbyte* text, BitFlags_003C1_002Cenum_0020SageBinaryData_003A_003AMonitorConditionActionType_003E* target, Tracker* tracker)`，原blob行73104，对应 `model-condition-source/036.txt`，body SHA256 `56463e826838b3912fb33ff1ae1ea1dd11878a090edfcef4804f6cd039f1eaa6`。命中ALL时写 `*(int*)target=-1` 后直接跳 `IL_012e`，只执行 `num2--` 并continue；普通词才经过 `IL_012a` 的 `ptr=(int)num3+ptr`。因此ALL后剩余迭代继续读取同一个ALL，后续文本不被消费，`ALL -DUMP_ORE` 的精确原结果仍是FFFFFFFF，不是FFFFFFFE。恢复实现与三缓冲区差分无变化；独立golden必须保留此原版行为，而非按通常位集直觉推导。

endian交换每位集全部存储word、Time与StringList length/count；强引用最终由TrackerFinalize按平台写index+1，GameObject最终imports为空。三者无子节点，错误嵌套仍拒绝，高kind校验提前return。测试 `GameObjectModelConditionCases.cs` 覆盖完整Model词表/INVALID/ALL/未知/正负词、Action完整词、引用/StringList/Time和父链，保留完整矩阵及独立布局。


## 本轮最终整批验收

有效验收提交`284599bfd958c0f62d40ce1b72d27dd70a568bcc`；GameObject run **40094269582 success**，同提交基础run **40094269592 success**。/Od、/O2每模式**381140 fixtures、1143420三缓冲区逐字节比较、13884拒绝、1060不变性**；平台0/1/2/99，两模式合计**2286840**次缓冲区比较。实际两模式日志、validation.json、sourceCommit/runId及运行终态均已逐项核对。

本轮实际29新，164→193/322；129种其他已知Marshal继续明确拒绝，58 Token-only未知Marshal回退保留。本轮SDK0次，原路由保留。
