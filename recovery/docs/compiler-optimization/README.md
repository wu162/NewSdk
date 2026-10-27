# 编译器性能优化

实施分支：`feature/compiler-optimization`；起点：`compiler-main@e7396d3b6fb302682d3610ebbc6e99ce87eb867f`。[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，合并须用户明确批准；禁止强推。

继续任务先读 [HANDOFF](HANDOFF.md)。它只保留当前接受点和下一步；源码、二进制、测试及原 ZIP 身份集中在 [VALIDATION](VALIDATION.md)。当前实现边界与诊断开关查 [CONTRACTS](CONTRACTS.md)。[PLAN](PLAN.md) 原样保存用户的 Final v3 方案，提案不等于已上线能力；历史证据查[前五轮](history/VALIDATION_ROUNDS_1_5.md)和[第六轮](history/VALIDATION_ROUND_6.md)。

## 按问题进入

源码根目录：`recovery/src/Managed/Core/BinaryAssetBuilder.Core/` 和 `recovery/src/App/Application/`；探针：`recovery/tests/`。

| 问题 | 方案章节 | 当前实现入口 |
| --- | --- | --- |
| 已确认 Core 风险 W01 | 3、5.3、17 | `CompilerCoreLifecycle.cs` 的 Define、跨流绑定、linked side-file 探针 |
| 实际工作计量 W02 | 18 | `DocumentProcessor.GetWorkMetrics`、`CoreStreamCompiler.CacheReport` |
| 内容封存与物化 W03 | 8.1、9.6、10.1 | `PhysicalInputSnapshot.cs`、`FileHashItem.cs`、`BinaryAsset.cs`；完整闭包仍未完成 |
| Schema 元数据 W04 | 4.3、10.1 | `SchemaSet.cs`；每流仍读取并 Compile 自己的 schema |
| 影子依赖 W05 | 6、7.3–7.4 | `DependencyObservationLog.cs`；观测不能授权强缓存复用 |
| 声明与每流绑定 W06 | 3.7、7.6 | `AssetDeclarationDocument.cs`、`InstanceDeclaration.cs`；完整语义结果未完成 |
| 新缓存 W07 | 8–9、16–17 | `BuildAssetCache.cs`、`SessionPackage.cs`；未满足完整合同前不开放新 bundle 命中 |
| XML 局部热点 W08 | 10.2–10.3、17.1 | `NodeJoiner.cs`、表达式程序/遍历、`CompilerSymbolLookup.cs`、StableSort 局部索引；XML 哈希保持旧实现 |
| 状态与并行 W09–10 | 11–14 | 每DP传输buffer、摘要读取所有权及Texture/Builtin owner门禁；能力声明实际消费仍待本轮验收，未开放worker |

## 工作与验收

- 以本轮用户时限和 [AGENTS](../../../AGENTS.md)、[EFFICIENCY](../../EFFICIENCY.md) 为准；通常 2 个子代理、最多 3 个。成果普通提交到本分支，10–15 分钟保存检查点。
- 编译、测试和微基准只通过 [Core lifecycle 工作流](../../../.github/workflows/refactor-core-lifecycle.yml) 执行，完整候选使用 `[refactor-core-lifecycle]` 标记。Core-only 探针先对真实 Foundation 预编译，App 依赖组仍在完整生产宿主上编译运行。
- 生产输入变化必须重建；只改允许的测试/工具/Markdown 时可使用严格来源复用，并分别记录 binarySource、testSource、sourceRun。未验证检查点不标完成，原失败终态保留。
- 保持 Win32/.NET4.8、插件 ABI、legacy 摘要/顺序、缓存默认关闭、Texture 缓存限制、原 DLL/BAB 与四输出字节差分，以及适用的缓存/SDK/ZIP 门槛。
- 组件耗时与实际工作量分别报告，不由局部微基准推导完整构建提速。新合同、下载身份和剩余范围只在上述入口维护，避免各入口重复历轮流水。
