# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md及本文件；旧STATUS按需查，不全量加载。

## 已验收109/322已知Marshal模块

本轮依次完成TechUpgradeReceiver、八种雷达/可见性/Capture、七种直接开火/伤害跟踪。最新已验收源码b57e5a5a1d11ebd3004a6f0b6f0af4bb15c86057：GameObject run40050646715 success，基础run40050646652 success。每/Od、/O2：192100 fixtures、576300三缓冲区比较、10064拒绝、400不变性，平台0/1/2/99。终态及远端validation.json已核对。中间锚点Receiver fa250611/run40048856796；Vision 7cd68875/run40049625447。

全部既有回归保留；根范围88属性/38类子节点。213种已知Marshal仍拒绝，58 Token-only沿原未知Marshal回退。SDK本阶段0次；原路由保留，完整GameObject、两份完整替代DLL及正式GameObject插件未完成。

## 当前检查点：三种完整光束状态（待统一验证）

LaserState、SweepingLaserState、StreamState及双强引用列表、EndOffset可空Vector3、完整ObjectStatusValidation已整组实现；原38方法、PE/字段/Token生成及所有新增测试已本地核对。GenerateBeamStateFields.py及GameObjectBeamState.h接入TestGameObject.ps1，提交触发原blob/PE校验与完整/Od、/O2四平台差分。未取得本批源码完整成功前不得计为112或声称运行支持。契约见GAMEOBJECT_BEAM_STATES.md。

新增的Sweep父类调用顺序、强引用列表Endian及translations不能简化为弱ID；EndOffset Token指向数据pointee。失败集中修复后取最终矩阵，保留失败历史。

源码调查工具ExportLifecycleMethods.py已支持带点号的FireWeaponNugget依赖；已保存5Hz逻辑帧常量、原PE enum表、每个方法SHA256。11模块调查曾触发150依赖上限（run40049720082），收敛为完整直接开火/跟踪组后完成，未放宽保护。

## 关键路径及持续规则

原blob6e40a9ed6c9f961d294d49768d911e6a6f354619；对应evidence/gameobject-modules/*-source/manifest.json存精确签名、行号与SHA256。各Generate*Fields.py接入TestGameObject.ps1，原DLL逐字节差分、独立布局/Token/sentinel/拒绝矩阵保留。

默认单代理，约45分钟有效实现阶段；完整能力组实现后统一验收，中断检查点每约10–15分钟普通推送。不能用空等、重复读取、反复运行凑时长；不强推、不新建无必要分支。并发Actions会写证据，更新ref前读新头重建树，保留所有并发内容。

SDK跨阶段累计降频；组件成功不冒充集成。适用XSD、同输入原/恢复、真实调用、四产物及适用无原DLL重放门槛不降低。本轮未改变SDK路由与输入，不重跑既有Weapon/Armor SDK。

Git命令行无凭据，使用GitHub插件。大原文件按ExportLifecycleMethods.py+sound-selector-source.yml导出有界方法；fetch_file对巨大原文件行范围本环境返回空，不能把空结果当无实现。原证据相同可按signature/bodySha复用，仅下载新增方法；不重复全量导出下载。
