# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。wu162/NewSdk，分支recovery/compiler-source。先读AGENTS.md、EFFICIENCY.md、本文件；历史STATUS按需，不全量加载原反编译文件。

## 已验收123/322已知Marshal模块

本阶段112→123，新增11种完整模块及必要依赖：

| 完整组 | 新增 | 文档 |
|---|---:|---|
| Projectile/Dynamics/Freefall/EnableDynamicsCollide | 4 | GAMEOBJECT_MOTION.md |
| Physics/BezierProjectileBehavior/Missile | 3 | GAMEOBJECT_FLIGHT.md |
| DamageStateList及受伤/碰撞/死亡触发开火 | 4 | GAMEOBJECT_REACTIVE.md |

最终GameObject有效源码 **3968ee7499a924ef068f8ef0132d62927c2bd168**：run **40069379207 success**；基础run **40069379228 success**。每/Od、/O2：**220032 fixtures、660096三缓冲区逐字节比较、10820拒绝、460不变性**；平台0/1/2/99，两模式合计1320192次缓冲区比较。validation.json与两模式日志、sourceCommit/runId及实际终态已核对。中间Motion源码c77679fc/run40053555512、Flight源码3af11bee/run40068382383均成功，最终矩阵包含全部。

**199种已知Marshal仍明确拒绝**，包含WeaponSetUpdate；58 Token-only仍按未知Marshal回退。完整GameObject、两份完整替代DLL及正式GameObject插件未完成。SDK本阶段**0次**，原路由保留；组件结果不冒充SDK/完整DLL验收。此前Receiver/雷达可见性/直接开火/Beam等组继续保留，语义详见对应GAMEOBJECT_*.md。

Motion与Reactive各有一次新增独立断言预期错误（ALL不前移词指针；空Flags没有token应为0），原/恢复字节一致，已据原方法修正，最终完整矩阵重新取得成功。没有放宽字节比较或减少平台。

## 下一完整组：WeaponSetUpdate（未支持）

读 **GAMEOBJECT_WEAPON_SET_NEXT.md**。完整四种槽位、WeaponData、TurretAI、可空目标选择器、ObjectVisionRange/ObjectFilter均须恢复，不能只启用空槽。原weapon-state-source143方法、PE枚举已导出；weapon-slot-sort-source23精确方法包括完整EASTL排序及Tracker.ExchangePointers/AlterBookmark，按signature/bodySha定位，不沿用旧文件编号。导出器已修正按平衡大括号截断void/非void方法，避免后续MarshalAs属性污染方法SHA；23方法本地指纹检查通过。

原版排序/引用配对探针：源码72b573545f0d8b88f6057d0e245bdb6a5d419f72，run40069703938 success；720组，四槽/四平台/长度0/1/2/27/28/29/65/129/257/五模式。permutations.tsv含重复key的实际原索引次序与三缓冲区指纹；不用重新运行相同探针。

Native/WeaponSlotOrder.h/.cpp已恢复排序目标排列，本地-O0 ASan/UBSan与-O2均匹配720原版排列，另1024有符号键检查。TestWeaponSlotOrder.ps1复用TSV运行Windows x86 /Od、/O2，结果在weapon-slot-order/validation.json；首次两模式通过但保存证据遇并发推送拒绝；已加入有限pull/rebase/push重试，最终源码e5ba9e6858c0bbb20016ec65f8beeba72aafb5f2、run40070276350 success；实际validation.json和两模式日志已核对。此helper未接入GameObject，不计WeaponSetUpdate支持。额外WeaponTemplate组件run40070108591及基础run40070108589均success（源码b2d59f1d）；没有SDK步骤。已核对3968ee74至最终文档前分支头的差异，GameObject运行时代码/生成器/测试未变，不重跑同输入矩阵。

下一步先恢复Tracker交换：a/b必须在当前栈顶block，按elementSize映射三组bookmark，再交换72字节记录；不得提前排序XML或只memcpy。然后完整接入四槽与炮塔依赖，补排序/重复key/强引用/重定位/Token与所有属性、nullable/列表、拒绝/不变性，统一完整GameObject及适用共享回归。SDK保持原路由，正式启用前门槛不降。

## 执行规则与用户提示

默认单代理，约45分钟有效实现阶段；每约10–15分钟普通推送检查点，未验证不标完成。整组实现与测试准备好后统一受影响完整矩阵；不空等、重复调查或重复同输入验证凑时长。禁止强推，不新建无必要分支；并发更新先读新头、重建树保留Actions证据。SDK跨阶段攒够能力再集中验证；正式启用前XSD、同输入原/恢复、真实调用、四产物和适用无原DLL重放门槛不降低。

用户明确提示已纳入EFFICIENCY.md：**命令行无法克隆，使用GitHub插件；Actions一般需要2分30秒以上**（一般参考，不是固定下限，最终看实际终态）；**直接写代码还原更快就省略不必要步骤，效率优先，但严格守住正确性底线**。本阶段复用既有解析/Upgrade/DieMux及固定证据；没有重复克隆或整库下载。无账号额度计量，不能量化节省。
