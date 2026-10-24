# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，最终源码分支`recovery/compiler-source`。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md和本文件；本轮用户允许3子代理并行、约1小时工作，明确可同时恢复多个GameObject模块并建议拆分长源码。仓库通过GitHub插件访问，禁止强推，保留并发提交与Actions证据。

## 当前已验收158/322已知Marshal模块

此前155种基础上完成SpawnBehavior、OCLUpdate、OCLMonitorUpdate完整能力组，文档`GAMEOBJECT_SPAWN_UPDATE.md`。有效源码`3f0f0e017380dac81a210a0a8d4055da6d4de5ad`，GameObject run **40088179574 success**，基础run **40088179650 success**。/Od、/O2每模式328144 fixtures、984432三缓冲区比较、12328拒绝、856不变性；平台0/1/2/99，两模式合计1968864次缓冲区比较。validation.json、实际两模式日志及sourceCommit/runId/终态均已核对。

164种其他已知Marshal仍拒绝，58种Token-only未知Marshal回退保留。完整GameObject、两份完整替代DLL及正式GameObject插件尚未完成；本轮SDK0次，原路由保留，组件成功不冒充SDK验收。

## 本轮后续能力组（源码检查点不代表验收）

正在连续实现SlaveWatcherBehavior（既有SpawnCrate只增强回归，不重复恢复或计数）、QueueDock/ParkingPlaceBehavior、BuildingBehavior/StructureUnpackUpdate/StructureSellUpdate，及GameObject.cpp机械拆分。本地已准备9种新增源码，完整通过后应为164/322、拒绝158；当前最终整批验证中。最终支持计数须以对应源码的完整矩阵成功更新；未取得成功前不要把这些组计入158。

SpawnCrate/SlaveWatcher原44方法在`evidence/gameobject-modules/spawn-crate-watcher-source`，Queue/Parking原47方法在`queue-parking-source`，建筑组31方法在`building-structure-source`。已核对原Marshal/Token/endian及必要helper。PE枚举取证在`gameobject-root/spawn-update-enums.txt`：OCLMonitor MASTER0/KILL_WHEN_RELEASED1/RELEASE_WHEN_MASTER_CHANGES_TARGET2，CrateFlags NONE0/DELETE_EXPIRED_CRATES1（默认mask2），不能猜bit0。

首组Spawn/OCL原61方法来源固定Tokenizer blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。原LOGICFRAMES_PER_MSEC_REAL为0.005f，OCL MinDelay1000转换5帧，不是30。SpawnTemplate是typed-ID列表，Die44→SpawnTemplate→Upgrade依赖按schema分配。OCL两种Marshal直接ModuleData但Token有独立Update→Contain→Behavior→Module父链。其余关键合同见类型文档。

## 执行与验收

每10–15分钟保存有意义检查点，标明未验证/验证中；按完整能力组连续实现，整批准备好后统一完整差分。保持/Od、/O2、四平台含99、三缓冲区逐字节比较、独立布局与拒绝边界，不减少原回归。拆分后的代码需完整矩阵通过；本地文本展开等价检查仅为静态证据。

真实SDK继续跨阶段按大里程碑集中运行，正式启用前XSD、同输入原/恢复、真实调用、四产物、适用无原DLL重放及完整mymod门槛不降低。历史STATUS按需读取，不全量加载旧交接。原DLL二进制不能通过文本插件取回；新增枚举使用现有测试作业内只读PE探针，无需修改云端源码导出工作流。
