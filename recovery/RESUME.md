# 编译器恢复：当前续接入口

更新：2026-10-23。仓库 `wu162/NewSdk`，分支 `recovery/compiler-source`。
先读本文件，再按本轮范围打开源码/测试。`STATUS.md` 是历史记录；旧章节的“下一轮”不代表当前进度。

## 持续规则

- 目标：恢复 SDK 自带 XmlCompiler / Tokenizer 的完整 C# 与 C++/CLI 逻辑；目前尚未完成两份替代 DLL。
- 本仓库操作已持续授权；禁止任何强推。基于最新分支头提交，保留并发 Actions 的证据提交。
- 每轮约 35 分钟有效工作，完成实现、验证、提交和交接；不拆微任务调度器、不空等凑时长。问答和规则修改无需延长。
- 额度优化遵循 EFFICIENCY.md：先读短入口和本轮必要证据，复用可追溯结论；禁止通过减测、抽样替代完整回归或省略原 DLL 核对来省额度。
- 真实 SDK 验证攒够字段/类型后集中运行。Weapon 提交消息 `[sdk-probe]` 才触发该步骤；普通修复只跑组件差分。
- 新增 mymod XML 必须符合 SDK/Schemas/xsd 的完整 XSD，并接入实际 Mod.xml。保持未实现分支显式拒绝。

## 当前能力与验证边界

ArmorTemplate：正式 `Recovered.Compiler.dll` 插件已通过真实 SDK 配置路由、原/恢复版四份产物逐字节比较、三个实际调用和无原 DLL 重放。

WeaponTemplate：根标量/资源、可选块、ObjectFilter、状态位、ScatterRadiusVsType、共用 Nugget 基类和 ScalarInfo 已恢复；Nugget 显式分派 **32/35** 已恢复。
- 九个 Damage 家族。
- Projectile / ScatterProjectile / WeaponOCL / SeedTiberium；SpecialPower / InformTarget / ContainedObjectAttack / SlavesAttack / ReportWeaponFiredStat。
- Circular / Lift / Linear / Stream / Laser / TerrainCrater；Attach / Spend / SecondaryDamage / Strip / Suppression / LuaEvent。
- 本轮新增 Paralyze / InformationWarfare。布局和覆盖表见 WEAPON_NUGGETS.md。

Weapon 仍由原 SDK Tokenizer 处理。组件差分通过不等于正式 Weapon 插件已接入；GameObject 与其余 XmlCompiler 序列化仍未恢复。

## 精确验证锚点

最新组件源码：`bd474c8cfec499be91ade4af040279e86010c5cf`。
组件运行：https://github.com/wu162/NewSdk/actions/runs/39892423711
已完成 success。每种 /Od、/O2：57,288 个 ProcessInstance 样例、171,864 次三缓冲区比较、850 次显式拒绝；2,070 次 ObjectStatus 与 4,179 次 ModelCondition 直接比较。两模式合计 343,728 次缓冲区比较。四个平台含 unknown 99。
基础 catalog/time-angle 回归同样通过：https://github.com/wu162/NewSdk/actions/runs/39892423774 。

独立的真实 SDK 批次源码：`83ec946e687569fb883bda0798ceb6de46a93cc4`。
- Weapon 批次：https://github.com/wu162/NewSdk/actions/runs/39891922522，success。
- 31 个合法武器覆盖前 30 分支及一个含 12 个 Nugget 的混合列表。完整 XSD、三个非法样例拒绝、实际 SDK 编译和所有恢复 InstanceData 在 mod.bin 中逐字节匹配均通过。
- Armor 回归：https://github.com/wu162/NewSdk/actions/runs/39891922459，success；四份完整产物一致、三次实际调用及隔离重放通过。
- Paralyze / InformationWarfare 未在该 SDK 批次中；它们不触发第二次真实 SDK 验证。
- SDK 批次证据独立保存在 evidence/weapon/sdk-batch-validation.json 和 sdk-batch-logs.zip；后续组件结果不能被当成该批次的 SDK 验证。

本轮源码修复：SecondaryDamage 的 PercentDamageToContained procedure name 从误抄的 4033350053 改为原始 signed -261607243 对应的 **4033360053**。加入独立 key/type/member 断言；全组件 /Od、/O2 回归通过。

## 下一轮约 35 分钟

恢复剩余 MetaImpact、TintObjects、AttributeModifier，按依赖成组推进。原始 Tokenizer `-Module-.cs`：

| 分支 | 原始结构字节数 | Marshal 行 | Tokenize 行 | 关键依赖 |
|---|---:|---:|---:|---|
| MetaImpact | 96 | 16328 | 79149 | optional KillObjectFilter，标量/时间/角度/Boolean |
| TintObjects | 72 | 16018 | 79012 | 内嵌 RGBColor、时间与频率 |
| AttributeModifier | 64 | 16805 | 79356 | 两个 modifier 引用、FX、角度与 AntiCategories 位集 |

行号仅导航，必须核对方法名及原始指针偏移。复用 WeaponNugget.cpp 的 SimpleSpec 只适合布局吻合的标量路径；复杂嵌套结构独立恢复。扩展原 ProcessInstance 三缓冲区差分、四个平台、/Od 与 /O2。

35 个分支齐全后仍须恢复 unknown-TypeId 行为、BAB 给根/非多态节点注入的 TypeId 注解与预处理边界，再实现正式 Weapon 插件路由和一批合法 SDK 样例；不能直接宣称完整替代 DLL。

## 代码与证据入口

`src/Native/WeaponNugget.*`、`src/Bridge/WeaponCompiler.cpp`；测试为 tests/Weapon*Oracle.cs 的 partial class；构建脚本 scripts/TestWeapon.ps1。枚举从原始 PE 由 scripts/ExtractWeaponEvidence.py 提取。当前新增测试在 WeaponEffectOracle.cs。

本轮已证明当前会话与工具仍可继续；无法通过仓库代码重置账号的模型使用额度。简短入口用于减少历史交接加载，避免上下文压力；不要声称已解除服务端额度。
