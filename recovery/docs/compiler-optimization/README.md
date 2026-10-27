# 编译器优化：按需读取

实施分支：`feature/compiler-optimization`；固定基线：`compiler-main@e7396d3b6fb302682d3610ebbc6e99ce87eb867f`。
[PLAN.md](PLAN.md) 原样保存用户提供的 Final v3 设计，属于设计与证据材料；执行范围、时限、代理数量和操作权限以用户当轮要求及仓库约定为准。

续接先读 [HANDOFF.md](HANDOFF.md)，再按下表选择方案章节和源码。不要每轮载入全方案、全部历史交接或反编译目录。方案中的“待验证”不等于已复现，接口与参数提案不等于已上线。

| 当前问题 | 方案章节 | 主要源码/证据入口 |
| --- | --- | --- |
| 顺序与放行 | 1、19 | 本页、HANDOFF.md |
| Core 三项风险 W01 | 3、5.3、17 | CompilerCoreLifecycle.cs 的 OptimizationRiskProbes |
| 工作计量 W02 | 18 | DocumentProcessor.GetWorkMetrics、CoreStreamCompiler.CacheReport.coreWork |
| 摘要读取 W03 | 8.1、9.6、10.1 | FileHashItem.cs、CompilerInputDigests.cs |
| schema 元数据 W04 | 4.3、10.1 | SchemaSet.cs；复用前保留闭包内容校验 |
| 影子依赖 W05 | 6、7.3—7.4 | AssetDeclarationDocument.cs；先记录，再授权跳过 |
| 每流绑定 W06 | 3.7、7.6 | InstanceDeclaration.cs、AssetDeclarationDocument.cs |
| 新缓存 W07 | 8—9、16—17 | BuildAssetCache.cs、SessionPackage.cs；未满足完整合同前不开放 |
| 局部索引 W08 | 10.2—10.3、17.1 | NodeJoiner.cs、CompilerNodeJoiner.cs |
| 状态与并行 W09—10 | 11—14 | CoreStreamCompiler.cs；先隔离再并行 |

源码位于 `recovery/src/Managed/Core/BinaryAssetBuilder.Core/` 和 `recovery/src/App/Application/`，探针位于 `recovery/tests/`。验证入口：`.github/workflows/refactor-core-lifecycle.yml`，提交标记 `[refactor-core-lifecycle]`；全部编译、测试与微基准在 GitHub Actions 执行。

## 本轮合同

- 每轮执行 60 分钟后交接；通常 2 个子代理，最多 3 个。所有成果普通提交到本页指定的新分支；不强推、不自行合入主线。
- HANDOFF 只保留当前结果、准确 sourceCommit/runId、未决事项和下一步，目标不超过 120 行；替换旧状态，历史由 git 保留。原方案只保存一份，不往交接里复制章节和运行日志。
- 不改变 Win32/.NET4.8、插件 ABI、legacy 摘要/输出顺序、缓存默认关闭和 Texture 缓存边界。不将微基准当作完整构建提速。
- 试验能力提供 AppContext 回退开关：`NewSdk.Compiler.DisableSinglePassDigest`、`NewSdk.Compiler.DisableNodeJoinerIndex` 为 true 时分别恢复历史双遍摘要和线性子节点匹配。可在 EXE.config 的 `runtime/AppContextSwitchOverrides` 配置；不新增 CLI 参数。

## 当前计量边界

`coreWork` 按任务保存各阶段的调用次数、失败次数和单调时钟区间：definitions、expressions、inheritance、validation、assetProcessing、referenceBinding、manifest、link。assetProcessing 包含资产准备/缓存等工作，不等于纯插件编码；这些区间不代表全构建成本，未计入解析、schema 准备、I/O 排队及发布成本的独立区间。

FileHashItem 的 DigestReadCount / DigestBytesRead 是每项成功完成的逻辑摘要读取计数，不是操作系统物理磁盘 I/O。当前只合并单项同次观察的双摘要，不等于完成跨流共享 PhysicalInputSnapshot。暖内容验证同时计算 FastHash 的 CPU 成本须在微基准中与原 SHA-only 暖路径一起报告。
