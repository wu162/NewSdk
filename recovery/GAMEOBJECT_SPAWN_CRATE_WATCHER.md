# GameObject SlaveWatcher新增与既存SpawnCrate回归

2026-10-24。本组新增SlaveWatcher已通过最终整批差分；既存SpawnCrate仅强化回归。SDK未运行，原路由保留。**本轮只新增SlaveWatcherBehavior。SpawnCrateUpdate已由既存crateChoices kind103完整支持，本轮只加强其回归，不重复实现、分派或计数。**

| 类型 | TypeId | native |
|---|---:|---:|
| SpawnCrateUpdate（既存kind103，回归加强） | 1244839822 | 48 |
| SlaveWatcherBehavior | 3148647452 | 20 |

来源为原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`，完整44方法见`evidence/gameobject-modules/spawn-crate-watcher-source/manifest.json`，含完整Marshal、Token、polymorphic wrappers与必要ObjectFilter/引用/helper。SlaveWatcher没有独立Endian overload，必须保留其原polymorphic wrapper内联实现。`GenerateSlaveWatcherFields.py --check`核对来源及新增SlaveWatcher字段/Token表；SpawnCrate继续使用既存GameObjectCrate.h及kind103，不创建第二个同TypeId的choice。

既存SpawnCrate回归合同：unsigned MaxCrates@8默认20，Time CreateFrequency@12默认30s、CrateLifetime@16默认120s，三个float@20/@24/@28默认50/30/300，Flags@32默认DELETE_EXPIRED_CRATES。Flags为完整32位存储、2位枚举，未知index1；完整表由原PE只读探针取得（source3f0f0e/run40088179574）：NONE=0、DELETE_EXPIRED_CRATES=1，因此默认mask2。PE SHA256为`ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603`。CrateList列表头@36，每项inline4字节强引用，不额外分配每项；先分配完整引用数组再注册各项，最后nullable BlockingUnits ObjectFilter112@44。endian交换根标量、Flags、listCount及完整Filter；引用和指针由公共处理保留原合同。上述样例在原kind103上运行，不让新增choice遮盖旧实现。

SlaveWatcher GrantUpgrade/RemoveUpgrade为强引用@8/@12，ShareUpgrades/LetSlaveLive为bool@16/@17，padding@18/@19保持零。原endian wrapper仅交换ModuleData的@0/@4，不能把两个引用当普通word交换。两个模块的Marshal直接ModuleData，Token仍保留完整Update→Contain→Behavior→Module父链；各10/5项加sentinel。

测试`GameObjectSpawnCrateWatcherCases.cs`保留所有旧矩阵，并覆盖完整标量/时间/引用/布尔/枚举词、默认/显式空/未知/ALL/正负词、列表0/1/2/27/28/29/65/129项、nullable Filter及完整过滤器、schema顺序与拒绝边界。最终须核对/Od、/O2、平台0/1/2/99、三缓冲区逐字节比较和实际sourceCommit/runId。

同时对GameObject实现作机械拆分：通用模块字段运行逻辑、各能力组Marshal/endian、分派与Token、公开输入校验分别移至`GameObjectModuleFieldsRuntime.h`、`GameObjectModuleFamiliesRuntime.h`、`GameObjectModuleDispatchRuntime.h`、`GameObjectModuleSupport.h`。均在原位置include，保留命名空间、定义顺序和同一编译单元；本地逐字节展开比较与拆分前一致，最终仍由完整差分验收。

## 本轮最终整批验收

最终整批源码`922f44602683fc8a048d99fa2eb8ce8887ee0d72`；GameObject run **40089743442 success**，基础run **40089743457 success**。/Od、/O2每模式335812 fixtures、1007436三缓冲区逐字节比较、12596拒绝、892不变性，平台0/1/2/99；两模式合计2014872缓冲区比较。validation.json、实际两模式日志及sourceCommit/runId/终态已逐项核对。

9种实际新增模块与源码拆分均包含在该矩阵中，总数155→164/322，158种其他已知Marshal仍拒绝。SpawnCrate沿用既存kind103，仅强化回归，不重复计数。本轮SDK0次，原路由保留；组件成功不代表完整GameObject、正式插件或SDK集成完成。
