# 原 SDK 完整缓存恢复：交接

## 目标与位置

恢复原 SDK 的完整缓存能力，包括默认行为、三种纹理、XML/Deflate、部分/冻结会话、touch、原缓存目录及共享资产。不能以部分增量缓存代替完成。

- 分支：`recovery/legacy-sdk-cache`；基于 `compiler-main` 的 `e7396d3b6fb302682d3610ebbc6e99ce87eb867f`。
- 实现及测试源码：`39039961847cd1ca9cb93743fd51741a4e5c9f49`。后续纯文档提交不改变该验证来源。
- [PR #19](https://github.com/wu162/NewSdk/pull/19)：草稿，未合并。
- 当前整体状态：已接入生产路径，完整验收尚未完成。

## 验证快照

核对时间：2026-10-29 12:05（北京时间）。[运行 40814401237](https://github.com/wu162/NewSdk/actions/runs/40814401237)，对应上述源码，两种配置均由本次运行重建。此表是快照，续接时先查运行的最新终态。

| 范围 | 快照状态 |
| --- | --- |
| Debug/Release 应用、Foundation、完整构建与部署检查 | 通过 |
| Core/后端/应用管线、原 BAB 基线、Texture 组件差分 | 通过 |
| 实际 CLI、冷进程、CtrlBreak 取消与报告协议 | 通过 |
| 资产缓存冷热/失效/重试、外部输入根原版差分、Audio CDATA/W3X 重放 | 通过 |
| 会话缓存矩阵：纹理三路、高级模式、原 BAB 缓存导入等 | 运行中，未验收 |
| 会话协议、后续整体验收门槛及独立 ZIP | 尚未完成 |

早期增量子集的成功运行不能替代本次完整缓存验收。

## 从哪里继续

| 内容 | 入口 |
| --- | --- |
| 原缓存接线、选项应用、失败/冻结/touch 行为 | `src/App/Application/CoreStreamCompiler.cs` |
| 原格式会话、CDATA 恢复、部分保存与提交/回滚 | `src/App/Application/LegacyCacheStore.cs` |
| 默认值、目录别名、项目预设 | `src/App/Cli/Options.cs` |
| 生产模式矩阵与原版互操作 | `scripts/TestCompilerSessionCache.ps1`、`scripts/TestCompilerBackendBaseline.ps1` |
| 原版纹理元数据探针 | `tests/CompilerLegacyCacheMetadata.cs` |
| 流程及报告契约 | `../.github/workflows/refactor-core-lifecycle.yml`、`protocol/build-report.schema.json` |

路径相对本目录。行为边界见 [缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。

## 待办

1. 跟进运行 `40814401237` 到终态；失败时先定位失败步骤及原始收据，不重做已通过的调查。
2. 确认三种纹理的原版四流及完整 CDATA 对照、压缩切换、冻结/解冻、部分失败恢复、touch、目录别名、SDK 配置优先级、原 BAB 缓存导入均通过。
3. 完成该运行后续协议、完整 mymod 原版差分及独立 ZIP 门槛。所有相关门槛通过后，记录准确源码/运行/产物来源，更新交接和 PR 状态。

保留失败记录，不降低原版字节差分与取消/失败断言；禁止强推，合入仍需用户明确批准。
