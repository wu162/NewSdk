# Upheaval 修复选择性合入核对

目标：`recovery/compiler-source`，原头 `ddf7ff6dac82b8f3e5331899d493e3847f46efb3`。来源：验证分支固定提交 [`068643219fded09a05fb7b75c6507b769c430316`](https://github.com/wu162/NewSdk/tree/068643219fded09a05fb7b75c6507b769c430316)。本次保留来源为合并提交第二父提交，以保留原验证提交的可追溯祖先关系；最终树仅引入下列修复、回归、审计、文档和精简成功证据。未引入 SDK/Mods 中的整套素材、公开用例输入或大型轨迹；后续需要这些输入时从固定来源显式导入。

## 实现与回归

| 实现 | 修复 |
| --- | --- |
| Bridge/GameObjectCompiler.cpp | 接纳 GameDependencyType、ArmorTemplateSet、LocomotorSet 的流程 id 与 ObjectVisionRangeItem 的继承身份 id；错误诊断报告节点路径 |
| Native/Xmlai.cpp | 按 XSD 允许嵌套 Strategic/Budget/BuildState 和 BuildDelayRange 的身份标记，不以独立根登记代替继承规则 |
| Native/Xmlpowers.cpp | GameDependency 的 id 属于合法流程元数据，不增加虚构二进制字段 |
| Native/Xmlprogression.cpp | AISpecialPowerInfo 接纳合法 id，Fields 参数改名 allowIdentityId |

6个对应测试文件与原始成功来源逐blob一致；包含合法嵌套id、三缓冲区原DLL差分、流程id不改变输出、ObjectFilter.id仍改变真实哈希、外来命名空间id继续拒绝等断言。TestUpheavalGameObjectSets.ps1可独立运行目标组。AuditSchemaIdentityMetadata.py及SchemaIdentityMetadata.json核对821份XSD，本地重新检查通过：22种显式ID、6种嵌套身份类型。

TestUpheavalSdk.ps1保留原版先通过的硬门槛；复现前必须从固定来源取回完整SDK/Mods/Upheaval目录（包括17资产PublicCases），不可用缺输入的目标分支直接宣称SDK通过。完整输入、配置、DLL/输入/输出指纹、插件轨迹与二进制输出都在固定验证提交及对应Actions产物中。

## 证据与合入范围

合入后 recovery/src 与 recovery/tests 两个完整子树必须分别等于来源树 `ca162af8d8dd70b5e9183348cdb8e1234a0641c5`、`a7d3c4516c84d1d70a7677cb75d5c0bc6958ce49`；原SDK除Mods外所有子树/文件也与来源一致。receipt记录具体blob、旧sourceCommit/runId及范围；不存在把历史验收改成合入提交新跑成功的声明。

- AI组件：运行40175816790；powers组件：40177066058；ID审计与progression /Od、/O2四平台差分：40180066600。
- 原Upheaval三阶段：40178403660；新增17资产：40182719547。后者原版先通过，10次进程成功、20组四产物逐字节相同，17资产在最终manifest，16个非Weapon各有实际恢复插件调用。
- 成功组件日志和validation.json一并保留；SDK仅复制成功收据及最近的GameObject定向组件证据。大量轨迹和媒体留在来源提交；[根因](ID_METADATA_ROOT_CAUSE.md)、[联合SDK](UPHEAVAL_SDK.md)、[公开源码经验](PUBLIC_MOD_CASES.md)均指向该来源。

本次没有手工重复已通过且未变化的组件/SDK批次，现有分支工作流触发规则保持原样。现有自动检查若触发，按原规则执行；不会关闭回归。Lua运行、实际游戏行为、后处理与BIG打包不在历史SDK验收范围。
