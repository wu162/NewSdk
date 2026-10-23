# 编译器恢复：当前续接入口

更新：2026-10-23。仓库 `wu162/NewSdk`，分支 `recovery/compiler-source`。
先读本文件及 AGENTS.md / EFFICIENCY.md，再按本轮范围读取源码与证据。STATUS.md 保存历史，旧章节的“下一轮”不是当前计划。

## 持续规则

- 目标是完整恢复 XmlCompiler / Tokenizer 的 C# 与 C++/CLI；两份完整替代 DLL 尚未完成。
- 仓库操作持续授权，禁止任何强推。保留并发 Actions 证据提交；API 更新也只能普通快进。
- 实现阶段按约 35 分钟有效工作量；先完成子集就保存，再继续依赖满足的工作。首个可复现成果及时提交/推送，每约 10–15 分钟检查未保存成果。禁止空等或重复分析凑时长。
- 正确性优先：保持原 DLL 核对、/Od 与 /O2、四平台含 99、三缓冲区比较、独立布局和拒绝测试。只减少重复读取/重复调查，不减测、不放宽断言。默认单代理。
- 输出先筛选，大型反编译按方法展开。通常一次约 150 行或 8 KB；这是展示目标而非证据上限。不能把写规则当作已证明额度降低。
- 真实 SDK 可跨多个实现阶段集中验证，不要求每轮执行；可在临时副本中注释/移除无关 MOD 输入，保留依赖，原/恢复版使用相同输入。精简覆盖须记录，关键里程碑保留完整 mymod 验收，详见 EFFICIENCY.md。Armor 仅显式 [sdk-armor] 或手动批次运行。真实 SDK 按类型/字段批次集中验证。Weapon `[sdk-probe]` 为原路由 payload 批次，`[sdk-plugin]` 为正式恢复插件对照；普通修复只跑组件。输入链/路由变化后必须集成验证。
- mymod 源码须符合 SDK/Schemas/xsd 完整 XSD 并接入 Mod.xml。保留未支持输入的显式拒绝。

## 当前已完成：Armor 与 Weapon 逐类型正式接入验证

ArmorTemplate：此前正式插件、真实 SDK 配置路由、四份完整产物比较、实际调用及无原 DLL 重放已通过。本轮 mymod 变更后的 Armor 回归也通过。

WeaponTemplate：91 根属性、可选块、三种 ObjectFilter、状态位、ScatterRadiusVsType、共用 Nugget/ScalarInfo、全部 **35/35 显式 Nugget 分支**，以及 missing/zero/unknown TypeId 默认路径已恢复。

本轮关键细节：未知对象分配 40 字节，仅 Marshal 共用基类，输出 TypeId 清零；Tokenize 只有 12 字节包装记录且子表偏移为零，不能生成基类 Token 子表；endian 仅处理共用基类。所有非多态节点允许 BAB 注入的无命名空间 TypeId，Nugget 项仍按它分派。未知字段/结构及外来命名空间继续拒绝。

正式插件：`Recovered.Weapon.Plugin, Recovered.WeaponPlugin`，依赖 `Recovered.Weapon.dll`，不调用原编译器。`scripts/SetWeaponPlugin.ps1` 可逆、幂等切换 WeaponTemplate。默认检入的 SDK 配置仍是原路由；已在隔离 SDK 副本中实际完成恢复插件路由编译。其他类型仍需原插件。

## Weapon 精确验证锚点

源码：`22c12f53c2f3c886cdb63d212ab10bc57cf715e0`。
运行：https://github.com/wu162/NewSdk/actions/runs/39896743313 ，success。
每种 /Od、/O2：66,152 个 fixture、793,824 次缓冲区比较、1,002 次拒绝检查、2,070 次 ObjectStatus 和 4,179 次 ModelCondition 直接比较；含 Win32、Xbox360、PS3、99。两模式合计 **1,587,648 次缓冲区比较**。整套矩阵同时覆盖非多态 TypeId 注解和正式插件元数据/入口。

同次正式 SDK 批次：38 个合法 mymod 武器，覆盖全部 35 分支、完整 35 项混合列表和继承版本；完整 XSD 及三个非法样例拒绝通过；原/恢复版 mod.bin、mod.manifest、mod.imp、mod.relo 全部逐字节一致；38 个实际调用的三缓冲区原版重放通过，移除两份原编译器 DLL 后重放也通过。实际 SDK 为 Win32；其他平台由组件矩阵验证。

证据：`evidence/weapon/validation.json`、`formal-sdk.json`、`formal-sdk-output.zip`、`oracle-logs.zip`，sourceCommit/runId 和两模式摘要已核对。正式构建两份程序集在该 run 的 recovered-weapon-component-x86 artifact。
基础回归：https://github.com/wu162/NewSdk/actions/runs/39896743345 。
Armor SDK 回归：https://github.com/wu162/NewSdk/actions/runs/39896743435 ，success。

旧 `sdk-batch-validation.json` 是此前原路由 31 样例批次的独立历史证据，不等同于新的正式路由结果。详见 WEAPON_TEMPLATE.md / WEAPON_NUGGETS.md / WEAPON_INPUT_PIPELINE.md。

## 当前下一目标：GameObject，尚未实现恢复编译器

已建立原 DLL 基线：196 个样例、588 次重复三缓冲区比较，覆盖四平台、默认布局及 16 个标量字段。源码 `a2bca36c1b72b09eea4a0cacfef1c4b7e7724ea6`，运行 39897613031 success；对应基础回归 39897613102 success。

原始证据映射：592 字节根对象；根 Token 表 126 字段加结束记录；23 个成员书签、101 个辅助入口、2 个空值分支。ReplaceModule@496 和 InheritableModule@500 的 Tokenize 原方法没有非空处理，不得自行补成普通子表。

追加检查点 `46b0ec5020b3911ec0b0403a3d60f63426590673` 将全部原方法字段与四平台实际 Token 表关联，验证 126 个名称、全部 name/type hash 和默认书签/空指针/空列表关系；本地及 Windows 全量检查均通过，运行 https://github.com/wu162/NewSdk/actions/runs/39897995324 success，validation.json 的 sourceCommit/runId 及三条 PASS 摘要已核对。默认根偏移 1620，InstanceData 2212，RelocationData/ImportsData 为空；八个空列表依次生成 count Token：Browser、ArmorSet、LocomotorSet、EquivalentTo、AI、UpgradeCameo、ProjectedBuildabilityInfo、DisplayUpgrade。

入口：GAMEOBJECT.md、scripts/ExtractGameObjectEvidence.py、VerifyGameObjectEvidence.py、TestGameObjectBaseline.ps1、tests/GameObjectBaseline.cs。自动保存 `evidence/gameobject/source-layout.json` 和 `original-baseline.zip`，包含全部源字段/原辅助方法标识/原始缓冲区/名称表。

## 下一轮约 35 分钟

1. 先核对 GameObject 最新检查点的 Windows 结果，再读取 GAMEOBJECT.md 和 source-layout.json；不要重扫两份大型反编译文件。
2. 按原 Marshal/Tokenize/endian 恢复根标量/字符串及默认列表的一个完整可差分子集。先映射默认值与所有默认辅助调用；无 XML 子节点不等于无 Token/分配。禁止拿冻结的原始二进制当恢复逻辑。
3. 原始方法：Tokenizer `-Module-.cs` Marshal 50072–50627、endian 50628–50911、Tokenize 97296–97814、构造器 74590–74680；blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`。原 GameObject TypeId=2486173485、TypeHash=3415017856。先复用现有 relocation/scalar/enum/string 模块，但逐一核实实际表示和缺省。
4. 新恢复逻辑保持未支持块显式拒绝；建立 /Od、/O2 四平台完整三缓冲区矩阵和独立布局/拒绝门槛。攒够类型/字段后再做合法 mymod SDK 批次，GameObject 继续由原插件处理。

本轮复用了同一份原模块与组件构建产物完成正式 SDK 验证，没有为各字段分别做真实 SDK 构建；不能量化账号额度节省，也不能通过仓库重置模型使用额度。
