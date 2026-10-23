# 编译器恢复：当前续接入口

更新：2026-10-23。仓库 `wu162/NewSdk`，分支 `recovery/compiler-source`。
先读本文件及 AGENTS.md / EFFICIENCY.md，再按本轮范围读取源码与证据。STATUS.md 保存历史，旧章节的“下一轮”不是当前计划。

## 持续规则

- 目标是完整恢复 XmlCompiler / Tokenizer 的 C# 与 C++/CLI；两份完整替代 DLL 尚未完成。
- 仓库操作持续授权，禁止任何强推。保留并发 Actions 证据提交；API 更新也只能普通快进。
- 实现阶段按约 45 分钟有效工作量；先完成子集就保存，再继续依赖满足的工作。首个可复现成果及时提交/推送，每约 10–15 分钟检查未保存成果。禁止空等或重复分析凑时长。
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

## GameObject 当前：26类子节点已验证，音频/EVA完整组验证中

全部88根属性及此前25类子节点已恢复；本轮CrusherInfo/ExtraCrushLevels与可重复ProjectedBuildabilityInfo的验证锚点为 `4a8989d232432330a230cdcb58f0e7bbd7e6462e`，组件39935810403、基础39935810494 success。每模式42,704 fixtures /128,112三缓冲区比较 /572拒绝 /128独立不变性比较。

完整原始Geometry（RotationAnchorOffset、PlacementShape、Shape列表、ContactPoint列表及Vector2/Vector3）亦通过：源码 `140963c2f2be6c1091ce506c5130205b77fe072b`，组件 https://github.com/wu162/NewSdk/actions/runs/39955059001 ，基础 https://github.com/wu162/NewSdk/actions/runs/39955059019 ，均success；每种/Od、/O2为43,624 fixtures、130,872三缓冲区比较、652拒绝、132独立不变性比较；两模式261,744比较，平台0/1/2/99。sourceCommit/runId已核对。Geometry不是后处理GeometryInfo。

本检查点继续实现完整AudioArrayVoice（三类列表，含SoundOrEvaEvent、可空Sound引用、TargetObject与大小写敏感Name哈希）、AudioArraySound、EvaEventArray，将支持面推进至29类子节点。全部枚举、引用、长列表、缺省/未知、独立分配与拒绝测试已准备；本地生成器一致性/原生语法通过，等待本次源码的Windows完整矩阵。**不能将26类的旧成功结果用于本批29类源码。**

仍不是完整GameObject编译器，没有正式插件；SDK运行0次、保持原路由。指针StringList只接受XSD合法文本；非空Item仍拒绝。未知输入、外来命名空间、重复单例继续拒绝。共享实现未修改。

源码：`src/Native/GameObject.{h,cpp}`、`GameObjectFields.h`、`src/Bridge/GameObjectCompiler.cpp`。生成器 `scripts/GenerateGameObjectFields.py` 保持原源码 blob 指纹及生成一致性检查。测试 `tests/GameObject{Oracle,Cases,KindOf}.cs` / `scripts/TestGameObject.ps1`。具体源方法、大小、偏移和限制见 `GAMEOBJECT.md`。

## 下一步

1. 先取得本次音频/EVA源码的完整组件矩阵结果，集中修复差异，核对sourceCommit/runId后更新入口。
2. SkirmishAIInformation必须带齐可空override、AIContextualPower/AISpecialPowerInfo及其ObjectFilter、AISpecialPowerWeaponData、两组AIBaseSelectionCriteria等完整依赖；仅外壳不能宣称支持。其方法49752、AISpecialPowerInfo方法252xx显示依赖较多，需按完整组推进。
3. 全部多态模块、ReplaceModule/InheritableModule尚未完成，继续显式拒绝。Geometry与GeometryInfo是不同结构，本轮只涉及GameObject原始Geometry。
4. GameObject能力累计后集中制作合法mymod样例及正式插件，四产物、实际三缓冲区和无原DLL重放门槛不变。

本次及以后按约45分钟有效工作量推进。中途用户续接前存在外部暂停，不把暂停算作有效工作。直接在现有分支保存检查点，未创建新分支；SDK零次。没有账号额度计量，不能量化节省。
