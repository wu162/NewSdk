# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md及本文件；旧STATUS按需查，不全量加载。

## 已验收94/322已知Marshal模块

新增完整TechUpgradeReceiver（GAMEOBJECT_TECH_RECEIVER.md）。有效源码fa250611af72d2639c141a532df858a9837bd0b8：GameObject run40048856796 success、基础run40048856747 success。每/Od、/O2：185152 fixtures、555456三缓冲区比较、9612拒绝、352不变性；平台0/1/2/99。远端validation.json的sourceCommit/runId与终态已核对。两模式合计1110912次缓冲区比较。

此前完整93模块（Slaved/Upgrade/Production/Create等）的回归均保留；根范围88属性/38类子节点。228种已知Marshal仍拒绝，58 Token-only沿原未知Marshal回退。SDK本阶段0次；原路由保留，完整GameObject、两份完整替代DLL及正式GameObject插件未完成。

## 当前检查点：八种雷达/可见性/共享控制模块（待统一验证）

GenerateVisionFields.py及GameObjectVision.h新增CaptureBehavior、UnifiedControlBehavior、RadarUpdate、UnitVisionBehavior、SelectiveVisibilityBehavior、InvisibilityBehavior、InvisibilityBroadcastUpdate、UnifiedShroudRevealBehavior，全部字段、父链、嵌套filter/validation、Token/Endian。Beacon/RadarMarker此前已完成，本次源码导出包含二者供核对，不重复计数。

本地已核对原导出87方法body SHA256、枚举证据及生成一致性；提交将触发TestGameObject.ps1的原blob/PE校验和完整/Od、/O2四平台差分。未取得该源码run的最终成功前不得计为102或声称支持，检查完整validation.json及双模式终态；失败集中修复。GAMEOBJECT_VISION.md记录实现契约。

## 关键路径及持续规则

原blob6e40a9ed6c9f961d294d49768d911e6a6f354619；对应evidence/gameobject-modules/*-source/manifest.json存精确签名、行号与SHA256。各Generate*Fields.py接入TestGameObject.ps1，原DLL逐字节差分、独立布局/Token/sentinel/拒绝矩阵保留。

默认单代理，约45分钟有效实现阶段；完整能力组实现后统一验收，中断检查点每约10–15分钟普通推送。不能用空等、重复读取、反复运行凑时长；不强推、不新建无必要分支。并发Actions会写证据，更新ref前读新头重建树，保留所有并发内容。

SDK跨阶段累计降频；组件成功不冒充集成。适用XSD、同输入原/恢复、真实调用、四产物及适用无原DLL重放门槛不降低。本轮未改变SDK路由与输入，不重跑既有Weapon/Armor SDK。

Git命令行无凭据，使用GitHub插件。大原文件按ExportLifecycleMethods.py+sound-selector-source.yml导出有界方法；fetch_file对巨大原文件行范围本环境返回空，不能把空结果当无实现。原证据相同可按signature/bodySha复用，仅下载新增方法；不重复全量导出下载。
