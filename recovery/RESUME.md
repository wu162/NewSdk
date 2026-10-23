# 编译器恢复：当前续接入口

更新：2026-10-23。仓库 `wu162/NewSdk`，分支 `recovery/compiler-source`。
先读本文件，再按本轮范围打开源码/测试。`STATUS.md` 是历史记录；旧章节的“下一轮”不代表当前进度。

## 持续规则

- 目标：恢复 SDK 自带 XmlCompiler / Tokenizer 的完整 C# 与 C++/CLI 逻辑；目前尚未完成两份替代 DLL。
- 本仓库操作已持续授权；禁止任何强推。基于最新分支头提交，保留并发 Actions 的证据提交。
- 实现任务按约 35 分钟有效工作量推进并记录阶段时间；提前完成一组先保存，再继续下一项已授权工作，不因子集完成提前收尾。首个可复现阶段及时 commit/push，长任务每约 10–15 分钟检查未推送成果，长验证/复杂调查前保存并标注验证状态。禁止空等凑时长；问答和规则修改无需延长。
- 额度优化遵循 EFFICIENCY.md：先筛选再输出目录/API/大型源码；通常每次展示约 150 行或 8 KB 内，必要证据按方法展开。复用可追溯结论，禁止减测或省略原 DLL 核对。上轮未证明额度下降，不能用规则修改冒充实际节省。
- 真实 SDK 验证攒够字段/类型后集中运行。Weapon 提交消息 `[sdk-probe]` 才触发该步骤；普通修复只跑组件差分。
- 新增 mymod XML 必须符合 SDK/Schemas/xsd 的完整 XSD，并接入实际 Mod.xml。保持未实现分支显式拒绝。

## 本轮检查点（验证中）

2026-10-23：开始 unknown-TypeId 分配、Tokenize、endian 的默认分支恢复。已核对原 blob 的 Marshal/Tokenize/endian：未知对象分配 40 字节，共用基类、输出 tag 清零；Tokenize 无 default，不生成基类 token 子表，仅保留 12 字节包装且 offset=0。已添加完整差分扩展及独立布局断言；本检查点尚未通过新的 Windows 差分，不能引用下方旧锚点作为新代码成功。下一步检查 Actions，修复任何失败，再调查 BAB TypeId 注入边界。

## 当前能力与验证边界

ArmorTemplate：正式 `Recovered.Compiler.dll` 插件已通过真实 SDK 配置路由、原/恢复版四份产物逐字节比较、三个实际调用和无原 DLL 重放。

WeaponTemplate：根标量/资源、可选块、ObjectFilter、状态位、ScatterRadiusVsType、共用 Nugget 基类和 ScalarInfo 已恢复；Nugget 显式分派 **35/35** 已恢复。
- 九个 Damage 家族。
- Projectile / ScatterProjectile / WeaponOCL / SeedTiberium；SpecialPower / InformTarget / ContainedObjectAttack / SlavesAttack / ReportWeaponFiredStat。
- Circular / Lift / Linear / Stream / Laser / TerrainCrater；Attach / Spend / SecondaryDamage / Strip / Suppression / LuaEvent。
- Paralyze / InformationWarfare；本轮新增 MetaImpact / TintObjects / AttributeModifier。布局和覆盖表见 WEAPON_NUGGETS.md。

Weapon 仍由原 SDK Tokenizer 处理。组件差分通过不等于正式 Weapon 插件已接入；GameObject 与其余 XmlCompiler 序列化仍未恢复。

## 精确验证锚点

最新组件源码：`0d4c9932dd4c7bec1af88f37a527033afad80b9c`。
组件运行：https://github.com/wu162/NewSdk/actions/runs/39894482358
已完成 success。每种 /Od、/O2：64,392 个 ProcessInstance 样例、193,176 次三缓冲区比较、1,002 次显式拒绝；2,070 次 ObjectStatus 与 4,179 次 ModelCondition 直接比较。两模式合计 386,352 次缓冲区比较。四个平台含 unknown 99。validation.json 的 sourceCommit/runId 已核对；两模式日志总结相同。
基础 catalog/time-angle 回归同样通过：https://github.com/wu162/NewSdk/actions/runs/39894482472 。

独立的真实 SDK 批次源码：`83ec946e687569fb883bda0798ceb6de46a93cc4`。
- Weapon 批次：https://github.com/wu162/NewSdk/actions/runs/39891922522，success。
- 31 个合法武器覆盖前 30 分支及一个含 12 个 Nugget 的混合列表。完整 XSD、三个非法样例拒绝、实际 SDK 编译和所有恢复 InstanceData 在 mod.bin 中逐字节匹配均通过。
- Armor 回归：https://github.com/wu162/NewSdk/actions/runs/39891922459，success；四份完整产物一致、三次实际调用及隔离重放通过。
- Paralyze / InformationWarfare / MetaImpact / TintObjects / AttributeModifier 未在该 SDK 批次中；本轮未触发新的真实 SDK 验证。
- SDK 批次证据独立保存在 evidence/weapon/sdk-batch-validation.json 和 sdk-batch-logs.zip；后续组件结果不能被当成该批次的 SDK 验证。

本轮实现：MetaImpact 的可选 KillObjectFilter、ShockWave 标量/时间/角度/五个 Boolean；TintObjects 的三段时间、频率/振幅与内嵌 RGBColor；AttributeModifier 的双 modifier 引用、AntiFX、DamageFXType、角度与 AntiCategories 位集。原 PE 枚举提取增加 AttributeModifierCategoryType；保持未知名称 BUFF=7、减号 XOR、ALL 填充与停止推进 token 指针的原行为。DamageFXType 缺省保持零；不能套用 DamageNugget 的 UNDEFINED 缺省。

## 下一轮约 35 分钟

显式分支已齐，但不得直接宣称完整 Weapon 插件或完整替代 DLL。

1. 恢复 unknown-TypeId 分配、Tokenize、endian 行为并扩展完整差分矩阵。目前组件仍对 missing/zero/unknown TypeId 显式拒绝。原 Tokenizer `-Module-.cs` 的 Marshal 分派在 15499–15622，unknown helper 在 52980–52986：分配 40 字节、Marshal 共用基类，再将输出 TypeId 写为零。必须进一步核对 Tokenize/endian 默认分支和所有缓冲区，不能把未知输入原值直接写入输出，也不能只凭这条总结实现。
2. 定位 BAB 给根/非多态节点注入 TypeId 的注解与预处理边界，检查完整 XSD、继承/引用/表达式与命名空间行为。保留不支持的输入显式拒绝；schema 与预处理准备好后再实现正式 Weapon 插件路由。
3. 路由和输入链变更后集中扩展 mymod 合法样例，覆盖全部 35 分支与混合列表，运行真实 SDK 批次。没有这一步不能把组件结果作为 SDK 集成通过。

原方法、偏移、缺省与测试入口详见 WEAPON_NUGGETS.md 的 Final three explicit branches。原 DLL 证据仍是 Tokenizer `-Module-.cs` blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`；TypeHash 静态常量不同于实际 polymorphic dispatch TypeId，不能混用。

## 代码与证据入口

`src/Native/WeaponNugget.*`、`src/Bridge/WeaponCompiler.cpp`；测试为 tests/Weapon*Oracle.cs 的 partial class；构建脚本 scripts/TestWeapon.ps1。枚举从原始 PE 由 scripts/ExtractWeaponEvidence.py 提取。本轮新增测试在 WeaponFinalNuggetOracle.cs；WeaponEffectOracle.cs 继续完整回归。

本轮已证明当前会话与工具仍可继续；无法通过仓库代码重置账号的模型使用额度。简短入口用于减少历史交接加载，避免上下文压力；不要声称已解除服务端额度。

