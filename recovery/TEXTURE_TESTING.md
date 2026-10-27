# TextureCompiler 验证触发规则

2026-10-26（北京时间），用户反馈“每次触发的测试个数太多了”。本规则优先于旧文档关于每次相关提交自动启动多条 Texture 工作流的安排。后续仅 Win32/PC；保留 PC 的全部输入、断言、Debug/Release、原版差分及 SDK 三模式，移除非 PC 平台轴和专属组。

- **普通检查点提交不启动 Windows 验证。** 整个能力批次准备好后，在阶段提交消息加 `[texture-test]`。自动验证统一由 `texture-validation.yml` 承担：一个 Linux 选择作业、至多一个 Windows 作业，选中组顺序执行，不再按组/配置创建矩阵作业。
- `[texture-sdk]` 显式纳入受影响的两个 PC PNG/BMP SDK profile（PcNoMip、PcMips）；普通 `[texture-test]` 不启动 SDK。同样已成功且输入未变的 SDK profile 保留原 sourceCommit/runId，不重复运行。
- `[texture-plan]` 仅运行选择器及其单元检查并上传计划，不启动 Windows。手动入口可指定 force_groups、run_sdk 和 plan_only。
- `TextureTestGroups.json` 明列实际消费者依赖。选择器要求完整成功的 Debug/Release 证据（SDK 则要求 Original/Recovered/NoOriginal 三组成功）、有效祖先 sourceCommit 和 runId，并比较该祖先与当前版本的依赖并集；探针、失败、不完整或非祖先记录不能跳过。原 SDK 工具 DLL 变化使相关组失效；新增/修改但未登记的实现文件按保守方式选组。
- 新的 helper 或消费关系必须更新依赖表。已登记组改依赖时同时核对验收版本的旧依赖，不能靠删除路径隐藏变化。测试数量选择不改变选中组内部覆盖范围。
- 原独立 Texture 组件、入口、SDK 与取证工作流保留手动入口，已从 push 自动触发迁出，功能由新入口集中调度。保留历史运行和证据；不取消、删除失败记录。
- 每个实际组仍生成自身收据；batch/selection.json 区分本轮执行与沿用结果，沿用记录保留原 sourceCommit/runId，不写成当前源码新跑成功。失败组保存证据并继续完成本批其他已选组，最终 batch 必须失败。

当前依赖表按 TGA 入口与 PNG/BMP 入口分别登记：只改 PNG 解码分支不会重跑只使用 TGA 的旧入口；修改共用 ImageLoader 或插件入口时才扩展到其实际消费者。生产 SDK 配置及 DLL 仍不替换。

## 取证入口补充

[texture-dxt-evidence] 显式选择 dxt-symbols，用同一个集中入口采集参考库 COFF 符号；该组 manual_only/evidence_only，普通回归与 SDK 不会自动选它。收据为 probe-only，绝不能作为 DXT 恢复验收或业务组件成功证据。40236386719 实测只选此组，复用全部20个已验收组件组；13项选择器检查包含旧依赖删除、实际非祖先提交、SDK完整三模式及取证非自动触发边界。

[texture-dxt-blocks] 显式选择 dxt-blocks 采集原版小块样本；同样 manual_only/evidence_only，普通回归和SDK不自动执行。40237236370只运行此组，20个已验收组件组全部复用；其probe-only收据不能作为DXT实现完成证据。


## PC 唯一目标

PS3 mip/cubic/resize 与其 SDK/取证入口已删除，Xbox360 与 unknown 平台不进入业务差分。非 PC Initialize/ReInitialize 的明确拒绝由 metadata 组检查。历史 PC/PS3 收据原样保留，但选择器须对实际变更重新验收 PC 消费组。修改仅平台专属文件时不扩展到其他编译器；公共 PC 依赖按原规则选组。

`dxt3-alpha` 是自动组件组，只验证 DXT3 显式 alpha 子组件，不能作为完整 DXT 压缩器或 SDK 验收。其实现尚无业务消费者，依赖仅登记本组；后续接入任何压缩入口时，必须同时扩展对应消费者依赖及完整差分。

`dxt5-alpha` 的完整门槛包括 Debug/Release 自有 native x87 helper 构建与 PE 检查、原量化器端点/索引/误差位模式、实际 DDS 的8字节alpha、归一化float组件及独立 NoOriginal重放。新增消费者必须登记 `src/TextureCompiler/Dxt/**` 及托管alpha入口依赖，并复制自有native DLL；目前没有插件或SDK消费者，不能借此宣称完整DXT已恢复。`dxt5-fast-alpha` 只接受质量68低层state；`-InspectDdsEntry` 保留原版未初始化levels的失败探针，不作成功验收。`dxt-decompile`/`dxt-colour-functions` 是手动取证组，部分导出或超时不充当业务验收。
