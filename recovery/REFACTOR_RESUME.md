# 原 SDK 完整缓存恢复：交接

## 目标与位置

恢复原 SDK 的完整缓存能力，包括默认行为、三种纹理、XML/Deflate、部分/冻结会话、touch、原缓存目录及共享资产。不能以部分增量缓存代替完成。

- 分支：`recovery/legacy-sdk-cache`；基于 `compiler-main` 的 `e7396d3b6fb302682d3610ebbc6e99ce87eb867f`。
- 实现及测试源码：`39039961847cd1ca9cb93743fd51741a4e5c9f49`。后续纯文档提交不改变该验证来源。
- [PR #19](https://github.com/wu162/NewSdk/pull/19)：验收通过，待审查，未合并。
- 当前整体状态：交接列出的生产缓存矩阵、完整 mymod 原版差分和独立五文件 ZIP 门槛均已通过；覆盖边界见下文。

## 验证快照

核对时间：2026-10-29 12:17（北京时间）。[运行 40814401237](https://github.com/wu162/NewSdk/actions/runs/40814401237) 已于 12:08 完成，整体及全部验收步骤均为 success；对应上述源码，Debug/Release 均由该运行重建。本次续接仅更新文档及状态摘要，复用这次成功结果，没有重复运行长测试。[核对摘要](evidence/refactor/legacy-cache/40814401237-1/summary.json)。

| 范围 | 快照状态 |
| --- | --- |
| Debug/Release 应用、Foundation、完整构建与部署检查 | 通过 |
| Core/后端/应用管线、原 BAB 基线、Texture 组件差分 | 通过 |
| 实际 CLI、冷进程、CtrlBreak 取消与报告协议 | 通过 |
| 资产缓存冷热/失效/重试、外部输入根原版差分、Audio CDATA/W3X 重放 | 通过 |
| 会话缓存矩阵：纹理三路、高级模式、原 BAB XML 缓存导入等 | 通过 |
| 会话协议、BIG/Additional/中立零插入及协议 | 通过 |
| 完整未精简 mymod 原版四流及 BIG/SKUDEF 差分 | Debug/Release 均通过，四流差异为 0 |
| 独立五文件 ZIP 部署、生产 BIG 与协议 | 通过 |

早期增量子集的成功运行不能替代本次完整缓存验收。

## 产物与覆盖边界

[原始产物](https://github.com/wu162/NewSdk/actions/runs/40814401237/artifacts/12871674302) 中的 Release ZIP 为 `artifacts/final-release/40814401237-1/NewModCompiler-Win32-Release.zip`，Debug 同目录。外层归档 SHA256 `6c395456add02dec40c7759e9cc5a26d34ea353f5e3ee588a12a78df3bfab044`，来自 GitHub 元数据及上传日志，两者一致。本环境下载返回 HTTP 403，未在本地重新计算归档哈希或打开内层收据；本次核对依据成功步骤、原始任务日志和产物元数据。

部分保存直接验证 XML 失败后完成文档的保存及重试；真实 CtrlBreak 用例使用 `/psc:false`。原 BAB 目录导入直接验证 XML，新宿主另行验证 Deflate 冷热及切换；未增加原 BAB Deflate 导入或 `/psc:true` 取消后持久化的独立生产证明。完整游戏数据、WBData、游戏内运行和中文纹理路径未新增验收。用例映射见[缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。

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

## 下一步

交接中的验收跟进和状态整理已完成，PR 等待用户审查。若修改生产或测试输入，按受影响范围重新验证；仅文档更新不触发 Windows/SDK 构建。用户明确批准合入后才可合并至 `compiler-main`。

保留失败记录，不降低原版字节差分与取消/失败断言；禁止强推，合入仍需用户明确批准。
