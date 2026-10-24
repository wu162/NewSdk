# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。仓库wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md及本文件；旧STATUS按需查，不全量加载。

## 已验收102/322已知Marshal模块

先新增完整TechUpgradeReceiver（GAMEOBJECT_TECH_RECEIVER.md），再完成八种雷达/可见性/Capture模块（GAMEOBJECT_VISION.md）。最新已验收源码7cd68875d9fda24b8650676b4794c246dd4f873e：GameObject run40049625447 success，基础run40049625277 success。每/Od、/O2：187612 fixtures、562836三缓冲区比较、9792拒绝、384不变性，平台0/1/2/99。终态及远端validation.json已核对。此前Receiver锚点fa250611/run40048856796仍有效。

全部既有回归保留；根范围88属性/38类子节点。220种已知Marshal仍拒绝，58 Token-only沿原未知Marshal回退。SDK本阶段0次；原路由保留，完整GameObject、两份完整替代DLL及正式GameObject插件未完成。

## 当前检查点：七种开火/伤害跟踪模块（待统一验证）

完整实现ChargeWeaponUpdate、FireWeaponUpdate、AimWeaponBehavior、LaserScatterUpdate、LinearDamageUpdate、SecondaryDamageTracker、TrackDamage及所有Nugget/Vector3/EffectInfo/RGBColor/validation依赖。GenerateWeaponBehaviorFields.py及GameObjectWeaponBehavior.h、测试已接入；GAMEOBJECT_WEAPON_BEHAVIORS.md记录契约。LaserUpdate原已支持，只核对不计新增。

原85方法、PE DisabledType/EffectInfoFlagType、5Hz逻辑帧常量与Token/字段生成已本地核对；提交触发完整Windows原blob/PE校验与/Od、/O2四平台差分。未取得本批源码的完整成功前不得计为109或声称运行支持；失败集中修复并取得最终矩阵。源码调查11模块首次触发150依赖上限，后缩至本连贯组；没有放宽保护限制。ExportLifecycleMethods.py已补齐带点号嵌套类型依赖匹配。

## 关键路径及持续规则

原blob6e40a9ed6c9f961d294d49768d911e6a6f354619；对应evidence/gameobject-modules/*-source/manifest.json存精确签名、行号与SHA256。各Generate*Fields.py接入TestGameObject.ps1，原DLL逐字节差分、独立布局/Token/sentinel/拒绝矩阵保留。

默认单代理，约45分钟有效实现阶段；完整能力组实现后统一验收，中断检查点每约10–15分钟普通推送。不能用空等、重复读取、反复运行凑时长；不强推、不新建无必要分支。并发Actions会写证据，更新ref前读新头重建树，保留所有并发内容。

SDK跨阶段累计降频；组件成功不冒充集成。适用XSD、同输入原/恢复、真实调用、四产物及适用无原DLL重放门槛不降低。本轮未改变SDK路由与输入，不重跑既有Weapon/Armor SDK。

Git命令行无凭据，使用GitHub插件。大原文件按ExportLifecycleMethods.py+sound-selector-source.yml导出有界方法；fetch_file对巨大原文件行范围本环境返回空，不能把空结果当无实现。原证据相同可按signature/bodySha复用，仅下载新增方法；不重复全量导出下载。
