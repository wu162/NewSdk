# 资产动作缓存与跨文档后端并行

更新：2026-10-28（北京时间）。成果分支 `feature/cache-parallel`，基线 `compiler-main@e7396d3b`。本页只记录当前工作；旧发行证据见 [COMPILER_RELEASE](../COMPILER_RELEASE.md)。**当前源码已接生产，尚待本轮完整 CI 验收，不声明性能加速或全矩阵通过。**

用户提供的[原始实施方案](CACHE_PARALLEL_PLAN.md)保留作目标与验收参考；当前状态只看本页收据，不能把方案要求当作已完成能力。日常续接无需重读整份方案。

## 先查什么

| 问题 | 最短入口 |
| --- | --- |
| 使用、参数和项目优先级 | [使用说明第6节](../COMPILER_USAGE.md#6-缓存与资源根)、`src/App/Cli/Options.cs` |
| 模式路由、保护扫描、缓存终态 | `src/App/Application/CoreStreamCompiler.cs` |
| 准备态、读取闭包和资格 | `src/App/Application/PreparedBackend.cs` |
| 排队、真实调用区间、结果与有序提交 | `src/App/Application/AssetTaskExecution.cs` |
| 磁盘完整性、pending、修复与回滚 | `src/App/Application/ActionAssetCache.cs` |
| 旧位置绕过与生命周期 | `src/Managed/Core/BinaryAssetBuilder.Core/{AssetDeclarationDocument,BinaryAsset,OutputManager,DocumentProcessor}.cs` |
| 逐资产失效、CDATA 与真实并行证据 | `scripts/TestCompilerBuildCache.ps1`、`scripts/TestCompilerBuildCachePayloads.ps1` |
| 输入冻结/Win32读租约证明 | `tests/CompilerPreparedBackend.cs`、`scripts/TestCompilerCoreLifecycle.ps1` |
| 总集成和部署门槛 | [Core lifecycle workflow](../../.github/workflows/refactor-core-lifecycle.yml) |

表中的源码路径相对 `recovery/`。定点使用 `rg -n 'TryPrepare|WriteIdentity|Enqueue|CommitOne|AuthorizeExecutionResult' recovery/src`；不要重新加载历史状态全集。续接从本页末尾运行收据与真实失败开始。

## 产品模式

| 参数 | 实际路径 |
| --- | --- |
| sc=false, bc=false, jobs=1 | AssetTasks.v1 串行，无持久动作缓存 |
| sc=false, bc=false, jobs>1 | 同一执行器，仅认证后端可并发，无持久动作缓存 |
| sc=false, bc=true | `.cache/action-cache-v1`，强 Key 按需恢复完整结果，命中仍过 verifier |
| sc=true, jobs=1 | 原 SessionPackage 及其 bc 组合，全根保护和拒绝行为保留 |
| sc=true, jobs>1 | 参数/预检报 `CORE_SESSION_PARALLEL_UNSUPPORTED` |

默认 sc/bc=false、jobs=1。jobs 只接受正整数，巨大值不会直接创建线程或等量队列；当前 W3X 并发最多2，过大展开任务转 owner 串行。缓存资格和并行资格分别判断，其他后端执行前排空在途队列。

## 源码资格表（实测范围由下节收据决定）

| 族/类型 | CanCache | CanRunParallel | 实际合同与边界 |
| --- | --- | --- | --- |
| 普通 XML、GameObject、WeaponTemplate、ArmorTemplate | 是 | 否 | 私有有效 XML（含 schema 默认属性）、独占命名空间、类型/schema/引用兼容身份；完整三缓冲区。其他 XML 特例见下一行 |
| AptAptData/AptConstData/AptDatData/AptGeometryData、TestTexture | 条件成立时 | 否 | 对全部真实外部读取持 Win32 FileShare.Read 租约并 SHA256；native 窄路径暂限 ASCII。缺失/不可读/非 ASCII 回兼容实际执行 |
| W3DAnimation/W3DMesh | 是 | 估算展开≤64MiB时 | 冻结 XML/命名空间，native scratch/output 独占，初始化与 trace 已源码审计；待真实跨文档重叠证据 |
| AudioFile 默认/NONE/XAS、MP3 passthrough、PathMusicMap | 条件成立时 | 否 | 存在且可读的真实输入持续持租约，三缓冲区、完整应有 CDATA；非法后端输出不得成为候选 |
| PathMusicTrack/Event | 条件成立时 | 否 | 全部实际 File/header 持租约、header 非零 handle；保留语义路径，缺失/警告路径实际执行 |
| Audio EALAYER3 或未闭合/警告路径 | 否 | 否 | 保留原诊断与执行，不消除进程一次性警告 |
| Texture/OnDemandTexture/TerrainTextureAtlas | 否 | 否 | 兼容 owner 调用并设在途屏障；混合 Texture session 原拒绝保留 |

不存在“同族一个用例通过即整个族实测通过”的推断。以上资格由后端真实读取审计决定，不按 fixture ID 白名单。

## 身份、事务与资源

强 Key 包括版本、实际工具/配置/schema/进程输入、有效后端参数、冻结消费输入、兼容引用元数据和完整外部读取摘要。jobs、TaskId、发现序号、worker 和私有输出目录不进入 Key。旧游戏 FileBase 保留格式用途，不能授权新路径复用；相同旧命名身份、不同语义结果明确拒绝。

全根盘点仍在原保护时点运行，保障适用 resolver 边界及发布前变化检查。新工具身份复用该扫描已有文件 SHA256，不再独立重复盘点工具树；MOD/Additional 普通内容不进入工具身份。当前尚未合并逐 stream 的全根保护扫描，报告 `hashReadBytes` 含保护扫描和后端外部输入哈希成本，不能把此实现称为严格文件系统快照。

存储按 ActionKey 读取与物化单条完整结果；不恢复历史整树，不导入旧缓存。候选和发布产物独立复制；Commit 可失败安装并保留 pending，必要报告/终态/sink 完成后 Complete 才移除排除标记。失败撤销安装及修复替换，遗留 pending 继续 miss。根写锁、4096条/4GiB 默认容量和成功后的整条清理已接入。

owner 保持前端顺序，worker 仅拿独占后端输入；结果按发现顺序验证和提交，文档最后任务提交后才完成/释放，再汇合链接。准备/在途/待提交受数量与256MiB保守预算限制，W3X展开按帧/通道和节点估计；取消停止派发并等待不可中断 native 安全返回。极大单任务采用串行，不承诺自动内存预测。

准备前先无序列化遍历 DOM 估算私有表示成本，再做队列预算准入；超过128MiB的准备估算不克隆、不缓存，直接使用 owner 兼容调用。256MiB是并发任务的保守预算；单个超大兼容任务沿用原执行峰值，不承诺通用硬内存上限。

报告新增 executionEngine/cacheMode/requestedJobs/backendCalls/maxBackendConcurrency、backendCallsByType、最多512条 backendIntervals、hashReadBytes/restoredBytes/writtenBytes/cacheCopyBytes。hashReadBytes记录保护盘点、后端外部输入与store payload校验的文件SHA256读取；cacheCopyBytes记录store实际拷贝，不代表整个进程全部I/O。peakSampledPrivateBytes来自owner/worker边界采样，不等于OS连续峰值。最大并发来自真实调用区间；缓存命中、排队长度与请求 N 均不能代替此证据。

## 当前收据与续接

| sourceCommit | runId / 状态 | 范围 |
| --- | --- | --- |
| b9a5f2686fb6dc54aa8fb76b9b5cb8be8e9b60b2 | [40625998371](https://github.com/wu162/NewSdk/actions/runs/40625998371)，failure | App Debug/Release 发现 ActionAssetCache.Stage 私有候选路径过长；未进入生产验收。失败终态保留 |
| cb22761562cbe9a9d12c50591e448d65a31cc883 | [40626352606](https://github.com/wu162/NewSdk/actions/runs/40626352606)，进行中且cache step失败 | 生产构建、Core、原BAB、Texture、CLI/协议、输入根、Audio/CDATA/W3X含真实并行、旧session与session协议已success；普通缓存fixture的GameObject缺Body已在后续测试修复，待终态原始日志确认 |
| d7143a505e4579f2b96a947e8aa7c8c559e8a5f2 | [40626782705](https://github.com/wu162/NewSdk/actions/runs/40626782705)，进行中 | 普通action-cache完整矩阵、输入根、生产/Core/CLI/协议等局部success；后续payload/session/SDK/发行仍须终态 |
| 79270b094a94de185f2bdaf88a593188db056837 | [40627506002](https://github.com/wu162/NewSdk/actions/runs/40627506002)，push触发、进行中 | 新分支已加入push过滤，显式完整标记，不复用旧二进制；含pending重复include/Reset修复、准备预算前置、计量及小帧并行fixture |

所有编译和测试均在 GitHub Actions。运行中不计作通过；二进制和测试来源以具体 run 的归档 build/validation JSON 及运行文件 SHA256 为准，文档提交不冒充二进制来源。尚无固定机器/输入可比的冷、热、单改、Additional 性能数据，暂不报告加速倍数。

下一步：跟进最新完整运行到终态，按失败步骤取日志集中修复，保留严格差分和 session 矩阵。生产输入再变更时重建；独立成功组只在闭包与证据匹配时复用。最后更新此表的最终 sourceCommit、原始 artifact、通过范围和真实阻塞，再准备指向 compiler-main 的 PR；未验收用 draft，未经用户明确批准不合入。
