# 编译器恢复：当前续接入口

更新：2026-10-24（北京时间）。wu162/NewSdk，分支 recovery/compiler-source。先读 AGENTS.md、EFFICIENCY.md、本文件；按本轮范围读取源码/测试/类型文档，历史 STATUS 按需。

## 已验收128/322已知Marshal模块

本轮完整接入 WeaponSetUpdate：四槽、72字节 Sorted WeaponData、完整 TurretAI/Chooser/VisionRange/inline ObjectFilter。源码 **45625616880eb075147618813786d27794ee38c2**，GameObject run **40071300506 success**；基础 run **40071300482 success**、WeaponTemplate run **40071300487 success**。每 /Od、/O2：**238272 fixtures、714816三缓冲区逐字节比较、11196拒绝、472不变性**，平台0/1/2/99；终态及validation.json两模式摘要已核对。细节见 GAMEOBJECT_WEAPON_SET.md。

Tracker.ExchangePointers 既有实现已符合原000/001：仅在当前栈顶block内，按elementSize调整pointer/reference/member三组bookmark的**offset**，不改value。原AlterBookmark并不同时交换来源和目标；此前“源/目标”措辞有歧义，已纠正。排序先保留XML生成引用的顺序，获取原EASTL排列，再通过Tracker交换bookmark并交换72字节记录；没有提前重排XML。

**194种已知Marshal仍拒绝**（含本轮Crate检查点，在验收前不计完成）；58 Token-only按未知Marshal回退。完整GameObject、两份完整替代DLL、正式GameObject插件尚未完成。SDK本轮0次，原路由保留。

## 治疗/维修四模块已验收

源码 **3b2cb3d2687d06b92417c3af56f6656b45e7cc63** / run **40072240199 success**，基础run40072240244 success。每/Od、/O2：**251332 fixtures、753996三缓冲区比较、11316拒绝、592不变性**，四平台含99；validation.json、两模式实际日志及终态已核对。完整AutoHeal/RepairUpdate/InitiateRepairDie/HealCrate及Upgrade/DieMux/Crate/Filter依赖见GAMEOBJECT_HEALING.md。首轮仅遗漏拒绝数组末尾AutoHeal ID，已修复清单并完整通过。该最终矩阵同时包含WeaponSet炮塔Chooser+多个视觉过滤器+乱序Weapon组合。

## 当前检查点：完整Crate八模块验证中

完整CrateCollide/Money/Unit/Veterancy/Attribute/Random/Salvage/Spawn八模块及必要依赖已实现，待本检查点完整矩阵成功后才能计入支持数。见GAMEOBJECT_CRATE.md、GenerateCrateFields.py、GameObjectCrate.h、GameObjectCrateCases.cs。原82方法/PE枚举已导出，源码1fe0f0469d20a583ecd34f14eac995db81650b58/run40072161495 success；只是取证。先核实本检查点Actions终态与validation.json的sourceCommit/runId。CrateList是重复文本强引用子节点，不能当字符串属性列表；RangeOfEffect unsigned、Chance普通float等按原方法而非名字解释。SDK继续0次，原路由保持。

## 执行规则

默认单代理，约45分钟有效实现；每约10–15分钟保存有意义检查点并普通推送。按完整能力组统一验证，不降低原DLL核对、三缓冲区差分、拒绝/不变性、四平台及正式SDK门槛。使用GitHub插件访问仓库，不重复尝试命令行clone；Actions通常需2分30秒以上，以实际终态为准。直接写代码更快时省略无助正确性的中间步骤。禁止强推，不新建无必要分支；并发更新先读取新头并保留Actions证据。

SDK跨阶段累计批次，原路由保持；正式启用前仍需XSD、同输入原/恢复、真实调用、四产物及适用无原DLL重放。完整mymod里程碑门槛保留。优化只省重复调查/上下文/无意义重跑，没有账号额度计量，不能量化节省。
