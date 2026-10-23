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

## GameObject 当前：88 根属性、23 类子节点已恢复，SDK 尚未接入

独立 x86 C++/CLI 组件 `Recovered.GameObject.dll` 支持全部 88 根属性，以及此前16类子节点加本轮7类：GameDependency、ObjectResourceInfo、BuildFadeInOnCreateList、BuildVariations、AutoResolveArmor、AutoResolveWeapon、AutoResolveInfo。

- GameDependency 完整包含两组可空 ModelCondition 位集、一组可空 ObjectStatus 位集、RequiredObject / ForbiddenUpgrade / NeededUpgrade 三组 ID 列表及可选完整 ObjectFilter；复用未改动的共享位集/过滤器实现。
- AutoResolveArmor / Weapon 包含资源引用及 RequiredUpgrade / ExcludedUpgrade 全部列表依赖；Info 包含全部六属性（字符串、ModuleData ID、两资源引用）。
- ObjectResourceInfo 完整支持 BuildCost / MoneyTransaction 列表，含两个 uint32、原分配、嵌套 Token 和 endian。
- **两个指针 StringList 只支持 XSD 合法的文本输入。原版实测将这类文本保留为显式空列表，不能按空白分词。** 原 helper 66048 → Node.GetChildNodes(null) 75998/76050 选择元素；BAB ValidateInstances 仅注入 TypeId、不展开文本。测试读取实际 StringList XSD、验证文本并注入注解，再原版差分；另外刻画原非空 `<Item>` 路径并证实它不符 XSD，因此恢复组件继续显式拒绝这种输入。详见 GAMEOBJECT.md。尚未运行真实 GameObject SDK 批次，不能将源码输入链分析称为 SDK 集成验证。

旧16类：DisplayName、DisplayNameTransformed、Buildable、ThingClass、DisplayColor、EquivalentTo、UpgradeCameo、DisplayUpgrade、Flammability、LiveCameraOffset、ArmorSet、LocomotorSet、FormationPreviewDecal、FormationPreviewItemDecal、ShadowInfo、VisionInfo。原默认值/布局、duration 边界、KindOf 284全值矩阵及所有既有拒绝检查继续保留。所有非多态节点支持 BAB 无命名空间 TypeId；未知字段、结构、外来命名空间和重复单例仍拒绝。ReplaceModule / InheritableModule 非空路径仍拒绝。

**仍不是完整 GameObject 编译器，没有正式 GameObject 插件；SDK 保持原路由。** 本轮真实 SDK 运行0次，共享 relocation / scalar / resource / enum-bitset / ObjectFilter 实现均未修改。TestGameObject.ps1 新增链接 ObjectFilter.cpp，工作流路径过滤新增 ObjectFilter；权限和原有证据自动推送步骤没有改变。

## GameObject 精确验证锚点

源码：`074d267b2835be89ade3944c1cbe6d2f94b3793a`。
完整组件：https://github.com/wu162/NewSdk/actions/runs/39928834132 ，success。
每种 /Od、/O2：**34,052 fixtures、102,156 次三缓冲区比较、492 次拒绝检查、124 次独立不变性比较**；两模式合计 **204,312 次缓冲区比较**，含 Win32、Xbox360、PS3、99。
新增 GameDependency 全部457个 ModelCondition 名称和223个 ObjectStatus 名称、符号/XOR/ALL、可空数组、所有三组列表及完整嵌套过滤器；其他新组包括全部依赖、独立布局/Token/分配断言和原版指针列表刻画。
基础回归：https://github.com/wu162/NewSdk/actions/runs/39928834050 ，success，源码一致。
证据：`recovery/evidence/gameobject-root/validation.json` 和两模式日志，sourceCommit/runId已核对；组件 artifact 为 `recovered-gameobject-component-x86`。旧 `evidence/gameobject` 只作原版刻画，不能混同当前组件或 SDK 成功。

源码：`src/Native/GameObject.{h,cpp}`、`GameObjectFields.h`、`src/Bridge/GameObjectCompiler.cpp`。生成器 `scripts/GenerateGameObjectFields.py` 保持原源码 blob 指纹及生成一致性检查。测试 `tests/GameObject{Oracle,Cases,KindOf}.cs` / `scripts/TestGameObject.ps1`。具体源方法、大小、偏移和限制见 `GAMEOBJECT.md`。

## 下一轮约45分钟

1. 先核对本节最终锚点，无需重读 Weapon 或历史 STATUS。
2. 建议连贯组：CrusherInfo + ExtraCrushLevels，以及 ProjectedBuildabilityInfo + 全部位集/AllowedObjectFilter。先查齐公共契约再实现整组，不逐字段触发全矩阵。
3. SkirmishAIInformation 需要连同可空 override、AIContextualPower、两个 base-selection 子对象等完整依赖另成组；不要只放开外壳。
4. Geometry、音频数组、全部多态模块、Replace/Inheritable 仍未完成，继续显式拒绝。
5. GameObject 能力累计完成后再集中制作合法 mymod 样例及正式插件路由，四产物对照、实际三缓冲区重放、无原 DLL 重放门槛不降低。

本轮按三个完整能力组推进并保留工作分支检查点和并发证据，未为单字段拆 CI。真实 SDK 零次；未重复调查 Weapon 实现，仅读取依赖所需的共享过滤器入口。没有账号额度计量，不能量化节省。

