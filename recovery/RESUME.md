# 热缓存性能优化：续接入口

当前工作分支为 `recovery/legacy-sdk-cache`，本轮直接在该分支开发；旧 PR #20 已合入。用户本轮要求按需加载插件、取消 Schema 目录全扫、四流/SessionCache/发布无变化时跳过重复 I/O，并精简重复防御检查；完成后必须与老 SDK 在同一输入下比较热缓存性能。

本轮最新生产源码 `98f1a2ea`，完整 Windows 验收 [40844460739](https://github.com/wu162/NewSdk/actions/runs/40844460739) 进行中，尚未标为通过。旧版成功验收和基准只说明历史状态，不能替代本轮结果。独立应用 Debug/Release 在前两个检查点已通过；基准准备器的 24 个轻量检查已通过。下一步跟进最新完整生产运行，集中修复失败，然后以成功运行的精确 Release 跑 `compiler-sdk-benchmark.yml` 的 `accepted_run` 配对矩阵，并保存原始输出一致性与热态耗时。

实现入口：`BuiltinCompilers`、`CoreStreamCompiler`、`SchemaSet`、`SessionCache`、`OutputManager`、`BinaryAsset`、`LinkedOutputCache`、`LegacyCacheStore`、`FileHashCache`、`ArtifactPublisher`。合同与新验收记录见[缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)，性能结果见 [SDK 对照](docs/SDK_BENCHMARK.md)。禁止强推；不将本次分支自行合并至其他分支。
