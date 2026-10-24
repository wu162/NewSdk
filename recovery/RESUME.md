# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md、STAGE_DURATION.md及本文件；用户最新允许约1小时实现阶段，覆盖旧45分钟约定。

## 已验收144/322已知Marshal模块

本轮完整AI Update八模块已验收，见GAMEOBJECT_AI_UPDATE.md。有效源码c7788592d20f1c4fbdc63f422b647595b840253d：GameObject run40077851777 success，基础run40077851766 success。每/Od、/O2：300060 fixtures、900180三缓冲区比较、11896拒绝、808不变性，平台0/1/2/99；两模式1800360次比较。validation.json和实际两模式日志/sourceCommit/runId已核对。178种其他已知Marshal仍拒绝，58 Token-only保持未知Marshal回退。

AIUpdate/Jet/Horde/SupplyTruck/Transport/Animal/Wander/DeployStyle全部字段、UnitChooser40和完整BaseChooser/VisionRange/Filter依赖已恢复。AI chooser指针@68、bool@72/@73；UnitChooser先强引用@36再BaseChooser。Horde/Transport保留独立Token父层。AutoAcquire未知词设bit4（ATTACK_BUILDINGS），不是忽略；StateMachine未知回退0、LocomotorSet回退12。

## 当前实现批次：经济/采集七模块，未验收

整组DockUpdate28、OreNode72、OreMiner20、SupplyCenterDock36、SupplyWarehouseDock36、GatherSlaves20、GatherEject172已接入并准备完整测试，尚不增加144计数。固定原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619的75个方法和枚举在recovery/evidence/gameobject-modules/economy-source；GenerateEconomyFields.py本地证据校验通过，Windows还需原源码/PE核对与/Od、/O2四平台完整差分。

Dock含nullable Filter@20；OreMiner含nullable ObjectStatusValidation@16；GatherEject是inline Vector3@36/@48及完整inline ObjectFilter@60。OreNode的四个Animation是单枚举，未知回退233；不是模型状态bitset。SupplyWarehouse StartingBoxes是signed，GatherSlaves Amount是unsigned。具体合同在生成器/原方法，最终结果以validation sourceCommit/runId及实际终态为准。

## 执行规则与已知限制

默认单代理、完整能力组连续实现，准备好后统一完整差分；每约10–15分钟保存有意义检查点，未经验证不标成功。禁止强推；提交前读新头，保留并发Actions证据。仓库访问用GitHub插件；Actions一般至少约2分30秒，以实际终态为准。

本轮SDK0次，原路由保留。完整GameObject、两份完整替代DLL及正式GameObject插件未完成；组件验收不冒充SDK验收。SDK按大里程碑集中运行，正式启用前仍保留XSD/同输入原恢复/真实调用/四产物/适用无原DLL重放门槛及完整mymod里程碑。
