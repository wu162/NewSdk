# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md和本文件；用户最新允许约1小时实现阶段，覆盖旧45分钟引用。历史STATUS按需，不全量加载大型反编译文件。

## 已验收155/322已知Marshal模块

本轮136→155，新增三个完整能力组共19种模块及必要依赖：

| 完整组 | 新增 | 文档 |
|---|---:|---|
| AIUpdate/Jet/Horde/SupplyTruck/Transport/Animal/Wander/DeployStyle | 8 | GAMEOBJECT_AI_UPDATE.md |
| Dock/OreNode/OreMiner/SupplyCenterDock/SupplyWarehouseDock/GatherSlaves/GatherEject | 7 | GAMEOBJECT_ECONOMY.md |
| AssistedPlacementInfo/AssistedPlacement/AttackAssistResponse/SpecialPowerAssistResponse | 4 | GAMEOBJECT_ASSIST.md |

最终有效组件源码 **16a37d05276242c29eddaee4726c6ed92603dac0**：GameObject run **40078985685 success**，基础run **40078985621 success**。每/Od、/O2：**318592 fixtures、955776三缓冲区逐字节比较、12160拒绝、844不变性**，平台0/1/2/99；两模式合计**1911552**次缓冲区比较。validation.json、两模式实际日志、sourceCommit/runId及实际终态已逐项核对。中间AI最终源码c7788592d20f1c4fbdc63f422b647595b840253d / GameObject40077851777、基础40077851766均success；最终矩阵包含全部AI回归。

**167种其他已知Marshal仍明确拒绝**；58 Token-only保持未知Marshal回退。完整GameObject、两份完整替代DLL及正式GameObject插件未完成。SDK本轮**0次**，原路由保留；组件验收不冒充SDK/完整DLL验收。

## 新组关键合同与已纠正边界

AI chooser是nullable指针@68，StandGround/FadeOnPortals bool@72/@73。UnitChooser完整40字节，先TargetingCompareList强引用@36再BaseChooser36/VisionRange/Filter，不能替换为TurretChooser44。Horde/Transport保留独立Token父层。AutoAcquire未知词设置bit4（ATTACK_BUILDINGS），StateMachine未知回退0、LocomotorSet回退12。

Dock的nullable Filter@20；OreMiner的nullable ObjectStatusValidation56@16，ForbiddenStatus在该结构@0、RequiredStatus在@28。GatherEject为inline Vector3@36/@48及完整inline ObjectFilter@60。OreNode四个Animation是单枚举，未知/显式空回退233，ALL值454；不是模型状态bitset。SupplyWarehouse StartingBoxes是signed，GatherSlaves Amount是unsigned。

HintObjectMapping20含nullable PlayerTemplate typed-ID@0、弱HintDisplayObject@4、float HintOpacity@8、nullable Filter@12、bool@16。Info36按HintObject列表/逐项依赖→nullable PlacementRestriction8及Filter→inline SuggestedOffset的schema顺序处理。新NullableAssetId分配4字节，Token指向该值、endian交换值。PlacementHint默认NONE设置bit0，未知词设置bit3；WeaponSlotType未知回退5。63个assist原方法含手动补齐的HintObjectMapping endian062，原列表通过typed局部变量直接调用它，不能漏掉。

原Tracker/排序/解析/资源/位集实现复用，未改坏ExchangePointers/AlterBookmark；旧WeaponSet排序与720排列证据继续有效。本轮失败已按原方法/PE集中修正，未放宽比较或拒绝：AI测试容器误用、AutoAcquire未知bit4，以及经济组两个独立断言偏移/单枚举ALL值错误。详细来源、方法指纹和边界见各组文档。

## 下一完整组：Spawn/OCL Update（三种仍拒绝）

先读 **GAMEOBJECT_SPAWN_UPDATE_NEXT.md**。固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619的61个精确方法已提取并逐项核对，位于evidence/gameobject-modules/spawn-update-source。SpawnBehavior92、OCLUpdate28、OCLMonitorUpdate40；尚未实现/计数。enum-status.json明确要求补齐DamageType、DeathType、OCLMonitorOption原PE表；取证不代表支持。

下一轮先核对manifest.signature/bodySha、完整Marshal/Token/endian及所有必要helper；特别检查非显式cast的typed局部变量调用。可在准备好的相关批次TestGameObject.ps1中附带只读PE探针（InspectAssistEnums.py为示例），不必修改云端工作流。先读当前源文件再决定是否能复用已有枚举证据，不猜数值。整组三类型及依赖连续实现、测试后统一完整矩阵。

## 执行规则与工具限制

默认单代理、约1小时有效实现；每约10–15分钟检查并普通推送有意义检查点，未经验证不标成功。整组准备好后统一完整差分；不空等、不重复同输入验证凑时长。禁止强推，不新建无必要分支；并发更新先读新头，保留Actions证据。

仓库访问使用GitHub插件，勿反复clone。大文件fetch_file可能返回空内容；必要时按已知blob SHA用fetch_blob，把原文保存在本地/工具状态，只向模型展示必要方法。核对原blob指纹，避免写文件时额外换行改变SHA。原DLL二进制不能靠当前文本插件取回，本轮Windows原PE检查和只读枚举探针已提供所需证据。未做额度计量，不能量化节省；本轮复用了公共实现和旧证据、未运行SDK、后两组修正与辅助整组合并最终验证。

自动审批曾拒绝扩展源码导出工作流及修改AGENTS的交接提交；二者均未生效。AGENTS远程保持原文，用户1小时约定记录于STAGE_DURATION.md和EFFICIENCY.md；新增原方法在本地只读提取，PE探针在现有测试脚本内完成，无需该工作流变更。

SDK继续按大里程碑集中运行；正式启用前XSD、同输入原/恢复、真实调用、四产物、适用无原DLL重放门槛及完整mymod里程碑不降低。
