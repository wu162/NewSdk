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

## GameObject 当前：88 根属性与 16 类子节点已恢复，尚未接入 SDK

独立 x86 C++/CLI 组件 `Recovered.GameObject.dll` 已恢复 GameObject XSD 直接声明的 **88/88 根属性**，包括此前剩余的 `EvaEventSecondDamageFarFromFirstTimeoutMS` 覆盖。原 scale=0.004999999888241291f，保留 atof→float、乘法→float、ceil→float 再转 uint 的行为；默认155；200.00002仍为1帧、200.00004为2帧，负数/指数/溢出和相邻float已直接差分。

已恢复子节点共16类：DisplayName、DisplayNameTransformed、Buildable、ThingClass、DisplayColor、EquivalentTo、UpgradeCameo、DisplayUpgrade、Flammability、LiveCameraOffset、ArmorSet、LocomotorSet、FormationPreviewDecal、FormationPreviewItemDecal、ShadowInfo、VisionInfo。重复列表、嵌套 Token、原分配顺序、字节序和默认值均覆盖。ArmorSet全部23位，LocomotorSet全部19个枚举项，ShadowType全部7值及未知回退已测。单例重复、未知结构/属性、外来命名空间等继续拒绝；UpgradeCameo/DisplayUpgrade保持XSD上限5/2。

**仍不是完整 GameObject 编译器，没有正式 GameObject 插件；SDK保持原路由。** 其余子节点/模块仍拒绝，本轮真实SDK运行0次。未修改共享relocation、scalar/resource、enum-bitset逻辑。

## GameObject 精确验证锚点

源码：`a115ef671b3c4a7a18a11bc720418b4c68e789f4`。
完整组件运行：https://github.com/wu162/NewSdk/actions/runs/39924338693 ，success。
每种 /Od、/O2：**23,240 fixtures、69,720次三缓冲区比较、224次拒绝检查、16次额外不变性比较**；两模式合计 **139,440次缓冲区比较**。含Win32、Xbox360、PS3、99，保留原全部根属性/284个KindOf全值矩阵，并新增完整子节点/混合输入和独立布局断言。
基础回归：https://github.com/wu162/NewSdk/actions/runs/39924338637 。第2次作业尝试已success，head_sha与上述源码一致；首次编译/差分已成功，但保存证据与GameObject作业发生普通push竞态，定向重跑后普通推送成功。基础回归41次浮点位比较、1,232条类型记录比较通过。

证据：`recovery/evidence/gameobject-root/validation.json` 与两模式日志，sourceCommit/runId已核对。该run的 `recovered-gameobject-component-x86` artifact已生成；DLL不是完整替代原Tokenizer。布局与原方法来源见 `recovery/GAMEOBJECT.md`。

实现：`src/Native/GameObject.{h,cpp}`、`GameObjectFields.h`、`src/Bridge/GameObjectCompiler.cpp`；生成器 `scripts/GenerateGameObjectFields.py`，验证 `scripts/TestGameObject.ps1`、`tests/GameObject{Oracle,Cases,KindOf}.cs`。视觉叶子的Token键/偏移/顺序从原方法生成并对照可读字段元数据，仍规范化CRLF检查原源码指纹，无实例缓冲区快照。

无子节点时仍为592字节root、126字段+结束记录、90成员书签/28空偏移/8空列表，默认root=1620、InstanceData=2212。非空列表、结构Token与字符串会改变前缀/分配；不能固定非默认root位置。ReplaceModule@496 / InheritableModule@500原非空Token路径仍缺失，继续拒绝。

## 下一轮约35分钟

按完整能力组连续推进；未完成检查点放工作分支，准备好后普通合入目标分支统一跑完整矩阵，保留并发证据。当前默认单代理和SDK降频规则不变。

1. 先核对本节最终锚点；无需重扫Weapon、历史STATUS或整个大型反编译文件。
2. 建议下一连贯组：BuildFadeInOnCreateList / BuildVariations 两个指针列表，以及 AutoResolveArmor / AutoResolveWeapon / AutoResolveInfo 家族，跟进完整子依赖。不要每个字段单独全量CI。
3. **输入链注意：** Includes/Base.xsd的StringList是xs:list，但原指针列表helper66048调用Node.GetChildNodes(null)，实现75998选的是元素子节点；它没有走Browser的空白分词重载68660。先刻画BAB预处理后实际输入与原版行为，不能按文本空白分词猜写。
4. GameDependency/ObjectResourceInfo、SkirmishAIInformation、CrusherInfo、ProjectedBuildabilityInfo、Geometry、音频数组、全部多态模块及Replace/Inheritable仍未恢复。逐组完成Marshal/Tokenize/endian/分配和拒绝契约后统一验证。
5. 继续累计GameObject能力，之后集中制作合法mymod样例与正式插件路由；正式宣称接入前仍需四产物对照、实际三缓冲区重放及无原DLL重放。组件成功不能代替SDK验证。

本轮通过GitHub插件读取与普通快进保存阶段成果；按四个连贯能力组推进，未为单个字段/枚举值拆CI；仅一次新独立断言的float边界预期修正后复验。真实SDK零次，不重复调查Weapon/共享模块。没有账号额度计量，不能量化节省。
