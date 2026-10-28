# Upheaval 新旧编译器 benchmark

## 跨 Actions 步骤的缓存环境修正

生产 `CacheProcessContract` 将整个子进程环境纳入缓存命名空间。首轮8318eb71框架直接继承Actions环境，其 `GITHUB_ACTION`、命令文件等随步骤变化，导致缓存阶段的备份在后续增量/恢复步骤中全部miss。因此首轮首次增量不是纯文件修改测量，新版损坏缓存也未证明实际命中被损坏的命名空间；这两类结论撤回，原记录保留。首轮同一步骤内的BC冷/热对照，以及各自初始化冷/热的SC和打包，不受此跨步骤问题影响。

`compiler_environment` 对两侧使用相同的显式Windows运行环境白名单，排除CI临时元数据、凭据及调试变量；只保存环境摘要，不写出环境值。每个阶段必须与初始化的环境摘要相同。首次增量修改前和每次缓存破坏前，额外运行基线验证：新版必须真实hits>0、misses=0，旧版必须有cache-copy消息，禁止把其他命名空间的重建误判成缓存失效或完整性恢复。

`suite=cache-followup` 或提交标记 `[upheaval-cache-followup]` 仅重测受影响的XML、DDS、WAV和恢复；每组各两台独立Windows VM、单独prime并跨步骤证明命中，增量仍各3对。组间VM独立、最多四个job同时运行；不会在同一VM上并行计时。既有有效缓存/SC和直接打包不重复测量。原失败终态和混淆数据不删除，不进入新正式增量统计。报告汇总器会自动排除缺少固定环境证据的旧增量组，以及含额外收集复制的旧打包组，并列出排除原因；旧新版恢复观察若缺少预先命中证明，也会明确标注不能接受为缓存完整性结论。

## 扩展矩阵：缓存、增量、恢复、会话边界及打包

用户于2026-10-28要求补齐缓存及其他指标。工作流手动选择 `suite=expanded`，或以 `[upheaval-expanded]` 标记提交，即在两台独立 `windows-2022` runner VM 上执行新旧配对；原clean组不重复启动。两台VM先各完成一对无缓存预热和既有完整输出指纹验证。

| 实验 | 每台 VM 的正式次数 | 门槛 |
| --- | ---: | --- |
| 缓存关闭 / BC首次冷建 / BC无改动热建 | 各5对 | 同一轮配对；冷热必须有真实缓存文件，新侧要求真实miss/hit计数 |
| XML / DDS / WAV内容修改 | 每类首次修改、修改后热建各3对 | 长度及mtime保持；每类先运行新旧无缓存oracle，要求输出确实变化且两侧一致 |
| 缺失/破损缓存、失败构建后重试 | 每种各侧一次 | 记录自动恢复、明确拒绝、错误输出、手动恢复及重试；失败操作不进入速度比 |
| 完整MOD会话缓存 | 各侧能力检查 | 新侧预期明确拒绝含Texture的 `/sc:true`；保留旧侧完整冷/热结果，不虚构新侧速度 |
| 实际地图元数据的SC冷/热子集 | 各5对 | 明确是原MOD的部分入口；新侧热建必须记录sessionPackageHits |
| 编译→LoD→Additional→BIG/SKUDEF | 预热后3对 | 新旧完整成员及SKUDEF身份对照，计完整流水线wall time |

旧SDK工具复制到临时目录，仅调整配置的SDK/Schema搜索路径及CLR4/legacy-v2启动策略以适配runner；原EXE/DLL未修改。不据此推断CLR2机器上的表现。

两种缓存存储永不跨编译器共享。BC单独测试清除旧侧intermediate及linked输出，保留其资产缓存；SC测试保留所需intermediate/session状态。增量测试每次恢复到同一份已验证的基线缓存，避免前一次修改污染下一次样本。修改仅发生在临时副本，最后核对SDK、完整MOD和两侧运行文件恢复原样。

汇总按VM分层重采样配对数据，保留各VM结果、CPU/峰值工作集、命令、缓存计数、产物指纹及失败观察；不会把两台同规格VM说成不同物理硬件覆盖。CI包含预热、正确性oracle及故障注入，耗时可能超过一小时；缓存阶段完成后先上传轻量checkpoint，整个任务最终同时上传轻量报告及完整证据。异常恢复行为和不支持的SC组单列，不能冒充性能成绩；LoD原工具若出现原始成员差异也不放宽校验。

扩展脚本是 `expanded.py`，分阶段命令由工作流运行；`aggregate.py` 生成跨VM的分层配对统计。已有无缓存结果仍保留原 source/run，新增矩阵以各自新终态为准。

打包阶段的旧BAB直接写入 `package/input/data`，利用原MOD的Data搜索根自然生成APT及地图子目录，不人为增加一次松散产物搬运。手动选择 `suite=packaging`，或提交标记 `[upheaval-package]`，仅在两台VM重新校验无缓存指纹并运行打包组，不重复缓存/增量矩阵。首轮扩展运行8318eb71中的打包路径包含测试收集复制，因此正式完整出包比较采用此直接输出版本的独立新运行；前轮缓存与增量数据保持原源码身份。

Windows托管镜像可能使用相同的hostname，不能用hostname判定是否独立机器。汇总使用GitHub分配的runner名称及jobs API中的唯一runner_id核实对应测量job。手动 `expanded_report_run_id` 或 `[upheaval-reaggregate]` 仅校验已有轻量artifact的原始SHA256、source/run与runner元数据后重汇总，不重新编译，不改写原运行终态。只读报告任务使用独立并发组，不挤掉正在排队的测量任务。原8318eb71与c35ed91d测量代码使用旧hostname门槛，其有效测量数据需由新报告校验器重新汇总；原始门槛错误仍保留。

从 `compiler-main` 独立分支测试**完整、未精简的历史 Upheaval**。当前主线已经不跟踪该 MOD，因此从初始 SDK 提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf` 用 `git archive` 提取到临时目录，固定 tree `5551fc26a6f96b62613443e0d0fa2958c14cbce0`，共 2723 文件，不修改当前 SDK。新旧编译器共同使用这份外部 MOD；完整输入哈希另存 `mod-inputs.json`。修改 benchmark 文件并普通推送自动触发；合入后也可手动 `workflow_dispatch`。默认 1 对预热 + 10 对正式样本，旧→新、新→旧交替顺序，同一台 `windows-2022` runner 串行执行。分支并发排队，不取消已有实验；最长 240 分钟。单个编译进程 1200 秒超时会终止进程树，失败证据仍上传。

## 原clean组测量合同

- 对比原 SDK `BinaryAssetBuilder.exe` 与正式 Release `NewModCompiler.exe` 的完整 MOD **编译/链接耗时**。自动选择 `Data/Mod.xml`、APT 和地图元数据入口；不注释引用、不替换资产、不用小 fixture 冒充真实 MOD。
- 两侧使用相同 SDK/MOD、Schema、资源搜索路径、Win32、`pc/ss/slowclean=true`，所有编译缓存关闭，每次新进程、全新输出/intermediate/hash/cache 目录。旧侧保留原配置中的 LowLOD/MediumLOD 表及所有插件路由。
- 旧侧每入口启动 BAB，累加所有进程耗时；新侧一次正式 CLI 编译所有入口。使用 Python `perf_counter`，从创建进程前计到退出后；包含编译器启动和正式 CLI 正常事务/日志开销。复制、参数准备、哈希及产物比较不计时。旧侧额外脚本循环开销不计入，是保守比较。
- 预热只暖 OS 文件缓存和依赖读取；后续每个编译器进程都是新进程，不能称为热 JIT。无管理员清页缓存，也不把该组叫作冷磁盘或缓存增量编译测试。
- 双方都不计 LoD 后处理、Additional、BIG/SKUDEF 打包、GUI、游戏启动。最终报告只能说明完整 Upheaval 的 clean compile/link，不能外推端到端发行、所有 MOD 或增量开发速度。
- 每次计时后核对完整输出集合，manifest/bin/imp/relo 和全部 CDATA 的长度及 SHA256 均须与首份输出一致；所有预热及正式样本都必须通过。不匹配或进程失败时保留日志和差异输出，整轮失败，不发布速度结论。首对原始输出保存在 artifact，后续保留每个文件的清单。
- 不删异常值，报告均值、中位数、标准差、CV、p95、全部原始样本，以及旧/新均值比和配对 bootstrap 95% 区间（10000 次，固定 seed）。区间含 1 时不宜宣称已分辨出速度提升。共享 runner 的噪声、单 runner/单 MOD 和有限样本均限制结论。
- 原始进程 CPU 秒、峰值工作集辅助解释；旧侧 CPU 求和、工作集取各进程峰值最大值。这不是整棵进程树的资源计量，不作为主要胜负指标。

方法参照 [hyperfine](https://github.com/sharkdp/hyperfine) 的预热/多次运行/准备步骤/统计，以及 [Google Benchmark](https://github.com/google/benchmark/blob/main/docs/user_guide.md) 的重复样本与聚合报告；这里直接测试 Windows 真实编译命令，没有引入微基准框架或声称它们给本结果背书。

## 二进制与结果来源

复用已完整通过新默认参数、完整 mymod/原 BAB、Core/CLI/缓存/Texture/BIG/独立 ZIP 验收的 [运行 40446975355](https://github.com/wu162/NewSdk/actions/runs/40446975355)。生产源码固定 `8b3e146cb5220e830ff6a0ce4718f47af052f166`，artifact `12708969379`，归档 SHA256 固定在 `prepare.py`。`compiler-main` 初始头 `e7396d3b6fb302682d3610ebbc6e99ce87eb867f` 与该二进制源仅文档有差异。

准备器要求生产源为当前提交祖先，生产源码、SDK、构建脚本、probes/catalog 完全未变；仅准许文档及本 benchmark 目录/专属工作流差异。校验 Actions 成功终态、原始 artifact 元数据与归档哈希、五文件正式 Release ZIP、Texture 与 EXE 的绑定。来源分别记为 `binarySourceCommit` 与 `testSourceCommit`，不把历史二进制冒充新构建。不重跑生产代码未变的既有全回归，也不修改其门槛。

本机 Linux 仅执行轻量测试；真实 Windows 编译结果以本分支 Actions 的 `validation.json`、`summary.md` 和原始日志为准。SDK 和运行文件前后完整哈希核对。产物自动保留30天。整轮只有 `succeeded=true` 才通过全部门槛；故障注入导致负向正确性发现时，仍保留失败终态。报告可单列独立通过、产物身份一致且样本完整的性能组，但不得把它们写成整轮验收通过。

```powershell
python recovery/benchmarks/upheaval/prepare.py recovery/artifacts/upheaval-runtime
python recovery/benchmarks/upheaval/benchmark.py --runtime recovery/artifacts/upheaval-runtime/bin --output recovery/artifacts/upheaval-benchmark --runs 10 --warmups 1
```

准备器需要 `gh`、`GH_TOKEN` 和完整 git 历史；测量需要 Windows、原 SDK 及正式包已有的 .NET/VC/DirectX 前置依赖。输出目录必须不存在，避免覆盖历史结果。正式结果自动出现在 Actions Summary，并上传 raw logs、每次命令/计量/输出清单、输入清单、首对原始流和来源收据。
