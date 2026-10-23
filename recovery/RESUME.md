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

## GameObject 当前：88根属性、29类子节点完整组件差分通过

本轮新增六类根子节点：CrusherInfo（含完整ExtraCrushLevels）、可重复ProjectedBuildabilityInfo（RadiusY可空、三类位集、AllowedObjectFilter）、完整原始Geometry（PlacementShape/Shape/ContactPoint与Vector2/Vector3）、AudioArrayVoice（三类完整列表）、AudioArraySound、EvaEventArray。此前23类及全部根属性继续通过。

关键边界：Crusher默认MinCrushVelocityPercent为0.01f；必须保留原拼写PrimaryBuidability；Geometry与GeometryInfo是不同结构。NamedEntry Name采用大小写敏感FastHash，TargetObject采用小写化typed-ID哈希；缺失Sound是null，显式空Sound仍分配引用word。全部位集、列表、可空值、Token、endian与原版逐字节一致。

原指针StringList仍只接受XSD合法文本（原版结果为空列表），非空Item拒绝；未知输入、外来命名空间、重复单例继续拒绝。共享relocation/scalar/resource/bitset/ObjectFilter实现未修改。**仍不是完整GameObject编译器，没有正式GameObject插件；本轮真实SDK运行0次、保持原路由。**

## GameObject最终精确验证锚点

源码：`d27f0b932b4855d708cf0f09095c0b50026251e4`。
组件：https://github.com/wu162/NewSdk/actions/runs/39956084622 ，success。
每种/Od、/O2：**44,980 fixtures、134,940次三缓冲区比较、796次拒绝检查、136次独立不变性比较**；两模式合计**269,880次缓冲区比较**，含Win32、Xbox360、PS3、99。
基础回归：https://github.com/wu162/NewSdk/actions/runs/39956084413 ，success，源码相同。
已读取终态及 `evidence/gameobject-root/validation.json`，sourceCommit/runId/modes/平台已核对。组件artifact：recovered-gameobject-component-x86；两模式原始摘要在对应oracle日志，保留Actions证据提交。

本轮前序完整组锚点（历史）：Crusher/Projected源码4a8989d232432330a230cdcb58f0e7bbd7e6462e / run39935810403；Geometry源码140963c2f2be6c1091ce506c5130205b77fe072b / run39955059001。最终覆盖以上全部能力，不需对未变化源码重复运行。

源码：`src/Native/GameObject.{h,cpp}`、`GameObjectFields.h`、`src/Bridge/GameObjectCompiler.cpp`。生成器 `scripts/GenerateGameObjectFields.py` 保持原源码 blob 指纹及生成一致性检查。测试 `tests/GameObject{Oracle,Cases,KindOf}.cs` / `scripts/TestGameObject.ps1`。具体源方法、大小、偏移和限制见 `GAMEOBJECT.md`。

## 下一轮约45分钟

1. 直接按 `GAMEOBJECT_AI.md` 的完整依赖组推进SkirmishAIInformation：88字节根、172字节AISpecialPowerInfo列表、两组20字节AIBaseSelectionCriteria、28字节AISpecialPowerWeaponData及ObjectFilter。该文档已整理原Marshal/endian/Token方法、65项XSD声明（id仅元数据，其余64项字段）、字节偏移与枚举入口；**只是源码调查，尚未实现或原DLL差分**。下一轮仍需核对实际方法及未知helper，不能把调查表当验证结果。
2. SkirmishAIInformation、全部多态模块、ReplaceModule/InheritableModule仍未完成，继续显式拒绝；不只放开AI外壳。
3. GameObject能力累计后集中准备合法mymod样例及正式插件，四产物对照、实际三缓冲区及无原DLL重放门槛不降低。普通组件组不频繁跑SDK。
4. 本次及以后约45分钟有效工作量；沿用及时检查点/普通推送与禁止强推。本轮直接更新现有分支，没有创建新分支。中途用户续接前存在外部暂停，暂停不计有效工作量。

本轮复用已加载原模块与既有共享实现，按完整组验证，未反复读取Weapon历史或触发真实SDK；没有账号额度计量，不能量化节省。

## 2026-10-23 当前检查点：AI已验证，空Module标记验证中

AI完整依赖组已通过：源码986bb898dacfbed2d6f92074f3e49f9aef421719；run39959718372 success。每种/Od、/O2为60,272 fixtures、180,816三缓冲区比较、964拒绝、184独立不变性比较，四平台含99。见GAMEOBJECT_AI.md。

本提交继续完整恢复XSD空结构ReplaceModule/InheritableModule：原方法66174/66183各Push1字节、tracker按4字节对齐；root@496/@500，Token不生成子表。新增原版差分、独立指针/填充/relocation/零Token、混合顺序及拒绝边界。**空Module标记组尚待本提交Actions完整矩阵，不声明通过。SDK未运行，原路由保留。**

下一步：检查本提交GameObject与基础回归Actions终态并核对证据；全部通过后更新当前入口，继续GameObject多态模块完整能力组。保留所有证据提交，禁止强推。
