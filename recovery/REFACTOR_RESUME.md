# 原 SDK 完整缓存恢复：交接

本轮性能实现以 [RESUME](RESUME.md) 为当前入口：`codex/legacy-cache-completion` / PR #20，完整生产源码 `4ad5d0d9`，最终验收运行 `40836177604` 和同输入配对基准 `40837902446` 均已 success；当前实现与证据以 RESUME 为准。下面保留原缓存恢复及优化前证据；不能用旧源码通过结果代替新文件身份、哈希收据、BIG/发布复用和缓冲日志的验收。

## 目标与位置

恢复原 SDK 的完整缓存能力，包括默认行为、三种纹理、XML/Deflate、部分/冻结会话、touch、原缓存目录及共享资产。不能以部分增量缓存代替完成。

- 分支：`recovery/legacy-sdk-cache`；基于 `compiler-main` 的 `e7396d3b6fb302682d3610ebbc6e99ce87eb867f`。
- 实现及测试源码：`39039961847cd1ca9cb93743fd51741a4e5c9f49`。后续纯文档提交不改变该验证来源。
- [PR #19](https://github.com/wu162/NewSdk/pull/19)：验收通过，待审查，未合并。
- 当前整体状态：交接列出的生产缓存矩阵、完整 mymod 原版差分和独立五文件 ZIP 门槛均已通过；覆盖边界见下文。

## 验证快照

核对时间：2026-10-29 12:17（北京时间），归档及性能结果于 12:55 后补核。[运行 40814401237](https://github.com/wu162/NewSdk/actions/runs/40814401237) 已于 12:08 完成，整体及全部验收步骤均为 success；对应上述源码，Debug/Release 均由该运行重建。生产及原验收输入未变，完整结果复用该运行，没有重复运行长测试；新增性能基准单独记录来源。[核对摘要](evidence/refactor/legacy-cache/40814401237-1/summary.json)。

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

[原始产物](https://github.com/wu162/NewSdk/actions/runs/40814401237/artifacts/12871674302) 中的 Release ZIP 为 `artifacts/final-release/40814401237-1/NewModCompiler-Win32-Release.zip`，Debug 同目录。外层归档 SHA256 `6c395456add02dec40c7759e9cc5a26d34ea353f5e3ee588a12a78df3bfab044`；GitHub 元数据、上传日志及性能基准续接时本地重新计算均一致。最初 CLI 下载 HTTP 403 的记录保留；后续通过 GitHub 产物下载工具取得原始 ZIP，已读取会话、完整 mymod、发行原收据，并校验 Release 五文件及 Texture 绑定。

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

交接中的验收跟进和状态整理已完成，PR 等待用户审查。用户追加的有界性能基准见[说明](docs/CACHE_BENCHMARK.md)：复用原验收 Release，生产输入及原验收测试不变；benchmark 自己的源码/运行身份单独记录。若修改生产或原验收测试输入，按受影响范围重新验证；仅文档更新不触发 Windows/SDK 构建。用户明确批准合入后才可合并至 `compiler-main`。

性能运行 `40820325416` 已 success，基准源码 `c39ad93eb5df7f7b7cd78f7028155d14a5094f2b`，54 样本约 39.6 秒（含单列启动诊断），全部输出一致。mymod 默认热态约减少 19% 耗时，合成纹理资产热态约减少 35%；Upheaval Armor 小子集会话热态略慢。未测完整 Upheaval、BIG 或输入编辑的性能；保留首轮适用性假设失败及一次启动超时，均不冒充有效成绩。

新增原 SDK 同输入配对性能[运行 40823369828](https://github.com/wu162/NewSdk/actions/runs/40823369828) 已 success，基准源码 `1c7ff21f098e972105c410eb586c4f6c8ba32d29`；54 对/108 次计量及两次预热约 91.2 秒，两方实际共用输入、Schema、cwd 和有序资源根，输出逐字节一致。默认热态新版比原版多约 25%～29% 端到端耗时；新版报告/事务发布也在计量内，未作纯 Core 分段归因。明细及原版无会话 `StringHashes.xml` 状态隔离见[对照报告](docs/SDK_BENCHMARK.md)，未重新编译生产。

随后[原因分析](docs/CACHE_PERFORMANCE_ANALYSIS.md)复算已有阶段和旧日志，并取得只读 I/O 诊断 `40826280008`（源码 `f7d69ff6f2aeefae1a32642d9854de853a95b82f`）：身份工作涉及 826 文件/6.66 MB，独立稳定复现约 59.5 ms，非生产方法直接计时；publish 本身仅 11–30.5 ms。缓存复用成功，旧热链接跳过而新 staging 重新生成并三次哈希/复制；优化优先级及未量化部分见报告。原反射诊断的 CLR 加载失败保留，生产未改。

保留失败记录，不降低原版字节差分与取消/失败断言；禁止强推，合入仍需用户明确批准。
