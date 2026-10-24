# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。wu162/NewSdk，最终源码分支`recovery/compiler-source`。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md和本文件；约1小时有效工作规则覆盖旧45分钟引用。本轮用户允许3子代理、同时恢复多个GameObject模块并要求注意源码拆分。仓库访问通过GitHub插件；禁止强推，保留并发提交与Actions证据。

## 当前轮次进行中（2026-10-24 北京时间19:34起，目标约20:34）

用户要求本轮按实际约1小时持续推进，最多3子代理。首批经济/电力4种、墙桥/放置3种、战斗响应3种完整源码及测试已准备并提交验证；未取得本批实际成功前仍按164验收数。当前源码沿用拆分，新增组运行实现另存各组Runtime.h。后续EMP/Suppression等连续推进，不因一组完成提前结束。

## 已验收164/322已知Marshal模块

本轮155→164，实际新增9种完整模块：

| 完整组 | 新增类型 | 文档 |
|---|---|---|
| Spawn/OCL | SpawnBehavior、OCLUpdate、OCLMonitorUpdate | GAMEOBJECT_SPAWN_UPDATE.md |
| SlaveWatcher | SlaveWatcherBehavior（既存SpawnCrate仅强化回归） | GAMEOBJECT_SPAWN_CRATE_WATCHER.md |
| Queue/Parking | QueueDock、ParkingPlaceBehavior | GAMEOBJECT_QUEUE_PARKING.md |
| Building/Structure | BuildingBehavior、StructureUnpackUpdate、StructureSellUpdate | GAMEOBJECT_BUILDING_STRUCTURE.md |

最终整批源码`922f44602683fc8a048d99fa2eb8ce8887ee0d72`；GameObject run **40089743442 success**，基础run **40089743457 success**。/Od、/O2每模式335812 fixtures、1007436三缓冲区逐字节比较、12596拒绝、892不变性，平台0/1/2/99；两模式合计2014872缓冲区比较。validation.json、实际两模式日志及sourceCommit/runId/终态已逐项核对。

158种其他已知Marshal仍明确拒绝；58种Token-only未知Marshal回退保留。完整GameObject、两份完整替代DLL、正式GameObject插件仍未完成。本轮SDK0次，原路由保留；组件验收不冒充SDK/完整DLL验收。首组独立锚点3f0f0e017380dac81a210a0a8d4055da6d4de5ad/GameObject40088179574/基础40088179650也均success，最终矩阵已包含该组全部回归，不重复运行相同源码验证。

## 源码已拆分

`GameObject.cpp`约15万→68070字符/1007行。原位置include的四个文件保持同一编译单元及定义顺序：

- `GameObjectModuleFieldsRuntime.h`：FindModule与通用字段解析/endian。
- `GameObjectModuleFamiliesRuntime.h`：各能力组Marshal/endian与分配。
- `GameObjectModuleDispatchRuntime.h`：多态分派、Token与模块列表。
- `GameObjectModuleSupport.h`：公开字段/递归节点输入校验。

机械拆分时原位展开逐字节等于拆分前源码，独立代理复核通过；最终整批矩阵同时验收拆分与新增模块。后续修改先定位相应Runtime/Support及能力组元数据头，勿把所有代码堆回主cpp。Parking的CanParkHereFilter在Bridge通过kind129专用路径进入原严格ObjectFilter读取器。

## 必须保留的新增合同

原Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；各组61/44/47/31原方法指纹、完整Marshal/Token/endian及实际helper已核对。SpawnCrate1244839822早已在crateChoices/kind103支持，本轮没有新建分派或重复计数；后续候选按ID核对所有ModuleChoice头，不能只搜cpp名字。

Spawn DieMux44→typed GameObject SpawnTemplate列表→Upgrade依赖按schema分配；Damage双word默认ALL。OCL两种Marshal直接ModuleData，Token却保留Update→Contain→Behavior→Module父层。原LOGICFRAMES_PER_MSEC_REAL=0.005f，1000ms转换5帧。OCLMonitor未知bit1；Damage未知bit38，Death未知bit33，ALL覆盖完整存储word。

QueueDock Token父Create（非Update），五个默认空String仍各分配NUL；Parking属性依赖→nullableFilter112@44→nullableVector3@48按schema分配，Vector Token仅bookmark值（mode2），不额外生成Vector表。Building的String无默认，缺省不分配/显式空分配；三建筑类型Token父Update。Unpack nullable音效@12独立分配4字节、Token mode2；模块endian不交换引用值，但最终由TrackerFinalize按平台写入，不能断言最终little-endian。单ModelCondition未知233/ALL454，不能当位集。

## 下一完整组：经济与电力（四种仍拒绝）

先读`GAMEOBJECT_ECONOMY_POWER_NEXT.md`。PowerUpdate24、PlayerPowerManagerBehavior16、AutoDepositUpdate48、DistributedMoneyBehavior20，已按ID对旧全表及本轮新增去重。固定原blob46精确方法和完整helper闭包已提取、指纹核对，见`evidence/gameobject-modules/economy-power-source`，尚未实现或计数。

先用原PE取完整PowerUpdateType2、AutoDepositFlagsType2、MoneyTransactionCategory30表；`enum-status.json`明确列出未知回退及原source静态数据，不能猜名称顺序。可复用已有TestGameObject作业内只读探针，无需修改云端工作流。注意Power名为Duration仍Time float、Distributed默认百分比0.01与Create Token父链、AutoDeposit三份MoneyTransaction列表的schema分配顺序及两unsigned字段。详细布局和必要原helper见NEXT，不以摘要代替原方法。

## 执行与集成门槛

按完整能力组连续实现和统一完整矩阵；每10–15分钟检查并保存有意义检查点，未验证不标支持。保留/Od、/O2、四平台含99、三缓冲区逐字节、独立布局与拒绝，不减少旧回归。Actions通常至少约2分30秒，查实际终态/sourceCommit/runId再验收，不高频轮询。

SDK继续跨阶段按大里程碑集中运行，正式启用前XSD、同输入原/恢复、真实调用、四产物、适用无原DLL重放及完整mymod门槛不降低。历史STATUS按需读取。本轮复用已有枚举和公共实现、分组统一验证并未运行SDK；没有账号额度计量，无法量化节省。
