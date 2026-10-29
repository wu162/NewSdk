# 性能优化与原 SDK 缓存：续接入口

用户本轮要求补齐链接输出复用、无变化发布跳过、文件搬运去重，并反复强调性能优先、代码实现先行、整批统一验证。已按此实现跨进程文件身份/哈希复用、BIG/SKUDEF 输入指纹复用、事件缓冲和同步 trace 清理。

当前分支 `codex/legacy-cache-completion`，生产源码 `4ad5d0d9`；[PR #20](https://github.com/wu162/NewSdk/pull/20) 目标为 `recovery/legacy-sdk-cache`，未合并。最终完整验收 [40836177604](https://github.com/wu162/NewSdk/actions/runs/40836177604) 已 success，包含完整 mymod 原版字节差分与独立五文件 ZIP；两配置热态收据均为 `fileHashReads=0`、`fileHashBytesRead=0`，BIG 直接引用既有公开包。首轮失败、阶段来源与具体代码合同见[缓存说明](docs/LEGACY_INCREMENTAL_CACHE.md)。同输入配对基准 [40837902446](https://github.com/wu162/NewSdk/actions/runs/40837902446) 已 success：复用该精确 Release，54 对/108 次约 83.4 秒，全部原始输出一致。默认热态新/原 SDK 中位数比分别为 mymod 1.067、Armor 子集 1.105、合成纹理 1.085（各边 n=2）；仍有约 6.7%–10.5% 端到端差距，不把不同 VM 的历史绝对毫秒差当作因果收益。[验收摘要](evidence/refactor/performance-implementation/40836177604-1/summary.json)，[基准摘要](evidence/refactor/sdk-benchmark/40837902446-1/summary.json)。

实现入口为 `CoreStreamCompiler`、`LinkedOutputCache`、`FileIdentity`、`FileHashCache`、`ArtifactPublisher`、`BigPackage`、`LegacyCacheStore`；日志与打包 I/O 见 `Diagnostics/Events.cs`、`AdditionalFiles`、`BigArchiveWriter`。旧缓存原始验收与分析记录仍在[交接](REFACTOR_RESUME.md)及对应能力文档；不得用旧二进制结果替代本批生产变化。
