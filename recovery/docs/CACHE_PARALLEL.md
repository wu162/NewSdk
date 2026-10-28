# 资产动作缓存与跨文档后端并行

更新：2026-10-28（北京时间）。成果分支 `feature/cache-parallel`，基线 `compiler-main@e7396d3b`。本页只记录当前工作；旧发行证据见 [COMPILER_RELEASE](../COMPILER_RELEASE.md)。**生产主线与完整 Windows 回归已有通过收据；最新批次和精确范围见下文。未据此声明整份方案全部覆盖或完整 MOD 加速。**

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
| W3DAnimation/W3DMesh | 是 | 估算展开≤64MiB时 | 冻结 XML/命名空间，native scratch/output 独占，初始化与 trace 已源码审计；真实重叠已在双动画用例验证；Mesh 保持源码重入认证，未单独宣称双 Mesh 重叠实测 |
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

已通过的完整批次：`125902cc5b8b0c3df3434b24db574970e2e022d5`，[40629580163](https://github.com/wu162/NewSdk/actions/runs/40629580163)，**success**。binarySourceCommit、testSourceCommit、packagingSourceCommit 均为该提交；Debug/Release 均全新编译。包含应用/Core、schema 默认属性、真实读锁及父目录换名拒绝、旧位置授权、重复include/继承、双native在途取消与真实worker异常安全汇合、CLI/jobs/协议、精确缓存/损坏/事务、输入根、Audio PCM/XAS/CDATA、W3X动画/网格缓存与双动画跨文档重叠、旧session、原BAB/BIG、完整未精简mymod与最终精确五文件ZIP独立运行。

原始归档 artifact `12791073520`，SHA256 `44d237d777bbbe6d8730beab34b1aff60c9510e2b646e8dcf48adc549f00d4fa`。已下载并核验，12组收据路径及哈希、运行文件、ZIP与逐用例数据集中在 [机器可读证据](CACHE_PARALLEL_EVIDENCE.json)。发行与源码来源分开记录，不用文档提交冒充二进制来源，不复用不匹配旧二进制。

最终Core原始收据确认：Debug/Release的取消与native异常两种场景，Dispose前均仍有1个真实后端调用在途，返回后active=0，两个调用区间都有结束；取消发生时确实观察到2个实际调用同时在途，失败/取消均未发布文档完成或最终manifest。完整W3X无缓存对照还记录了跨文档最大实际并发2，乱序结束后输出与串行逐字节一致。

本轮全部完整运行均来自成果分支。早期显式workflow_dispatch，加入push过滤后由提交标记 `[refactor-core-lifecycle]` 触发；最终运行event=push、cli_only=false。`75118632` 的采样频率限制也已独立完整通过，最终收据包含该生产改动及后续native异常测试，不需要拿早期通过范围代替它。

失败记录保留原终态，均未放宽断言或超时：

| runId | 确认原因与修复 |
| --- | --- |
| [40625998371](https://github.com/wu162/NewSdk/actions/runs/40625998371) | 私有候选名重复完整Key/GUID导致Win32路径过长；改为短序号，后续全组通过 |
| [40626352606](https://github.com/wu162/NewSdk/actions/runs/40626352606) | GameObject夹具缺必需Body，原始XSD异常已核实；修正真实夹具并增加长链接前预检，后续普通缓存矩阵通过 |
| [40626782705](https://github.com/wu162/NewSdk/actions/runs/40626782705) | 旧大帧W3X串行控制 `control.json` 明确 timedOut=true、180秒超时；实际压缩O(n²)成本使夹具过重。改4096/3072帧并保留真实重叠、字节对照与原超时，后续完整组通过 |

## 单样本计量与实际边界

以下为同一成功运行的 Release 小夹具，固定运行文件与输入族，顺序启动独立CLI进程；“空缓存”指编译器动作缓存为空，OS文件缓存未清空。仅提供原始耗时与调用数，不推出全MOD加速倍率；完整机器/OS缓存控制及重复采样尚未做。

| 夹具 | 状态 | 后端调用 | hit/miss | elapsedMs |
| --- | --- | ---: | ---: | ---: |
| Audio PCM/XAS | 空动作缓存 | 2 | 0/2 | 429 |
| Audio PCM/XAS | 热缓存 | 0 | 2/0 | 412 |
| Audio PCM/XAS | 单资产修改 | 1 | 1/1 | 421 |
| Audio PCM/XAS | 仅Additional修改 | 0 | 2/0 | 409 |
| W3X | 空动作缓存 | 3 | 0/3 | 490 |
| W3X | 热缓存 | 0 | 3/0 | 432 |
| W3X | 单资产修改 | 1 | 2/1 | 435 |
| W3X | 仅Additional修改 | 0 | 3/0 | 448 |

小夹具热运行仍有约13MiB输入保护/校验哈希读取，前端、启动、全根保护和链接继续执行；零后端调用不等于只重打包。混合Texture用例的热缓存仍有1次Texture调用；其余认证资产8hit/0miss，修改Weapon/GameObject/Armor等为7hit/1miss，含Texture总调用2。

仍保留的范围与下一步：

- Texture、未闭合/警告/非ASCII native读取、超大准备态继续兼容执行；XML/Audio并行尚未开启。资格表区分源码认证与实测，未把双动画重叠扩称每个W3X子类型实测。
- 各stream全根保护扫描尚未合并；已有按需恢复和扫描内工具摘要共享。跨stream再复用摘要须有稳定字节版本证明，不能用mtime/size替代。
- 尚未形成细分全部miss原因或真实大型MOD的受控冷/热/单改/封装性能基准；先用现有报告定位剩余成本，再扩并行或减少扫描。

已完成本轮有界实施与验收；后续从上述明确保留范围选择下一组，不把本轮描述为完整方案全部完成。续接只需读本页、[REFACTOR_RESUME](../REFACTOR_RESUME.md)和上述最新运行；按失败步骤取原始日志，不重新全仓调研。成果在 [draft PR #17](https://github.com/wu162/NewSdk/pull/17)，目标compiler-main；禁止强推，未经用户明确批准不合入。全部编译/测试在GitHub Actions；文档整理不触发Windows构建。

## 后续验证效率要求

本轮正确性门槛已有通过证据，但必要测试分批补充、过重动画夹具及重复完整构建降低了执行效率。用户再次要求提高效率，后续统一按 [EFFICIENCY的批次规则](../EFFICIENCY.md#验证批次与触发决策) 执行：先准备完整能力组与必要回归，在CI长链接前完成夹具预检；检查点普通推送不自动触发全量；失败集中定位和修复。保留严格字节对照、真实调用重叠、取消/异常、事务和适用SDK/发行门槛，不能靠删断言或放宽超时减少失败。

现有 `scripts/DownloadCompilerCliReuse.py` 固定绑定历史源码/运行/归档，不能仅设置 `cli_only=true` 就复用本页的新二进制。仅测试/控制器改动时，先核对现有入口能否以最小适配支持本页的成功产物：必须核验生产闭包、编译配置/工具链、归档和运行文件哈希，并分别记录binarySourceCommit/testSourceCommit；原有来源校验应改为准确核验两者，不得直接删除。无法安全复用时，明确具体缺口，集中准备后重建一次，避免每补一个测试就重编。

普通并行验收的4096/3072帧是已通过的真实工作负载，保留原180秒门槛与后端区间重叠断言；不要恢复旧32768/24576帧规模作为常规串行控制。独立取消/异常用例的更重第二任务用于证明仍在途调用被安全汇合，不能为压缩耗时破坏该性质。计量采用上文原始报告，不将小夹具、空动作缓存或单样本耗时泛化成完整MOD性能。
