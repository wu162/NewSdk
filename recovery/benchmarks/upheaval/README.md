# Upheaval 新旧编译器 benchmark

从 `compiler-main` 独立分支测试**完整、未精简的历史 Upheaval**。当前主线已经不跟踪该 MOD，因此从初始 SDK 提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf` 用 `git archive` 提取到临时目录，固定 tree `5551fc26a6f96b62613443e0d0fa2958c14cbce0`，共 2723 文件，不修改当前 SDK。新旧编译器共同使用这份外部 MOD；完整输入哈希另存 `mod-inputs.json`。修改 benchmark 文件并普通推送自动触发；合入后也可手动 `workflow_dispatch`。默认 1 对预热 + 10 对正式样本，旧→新、新→旧交替顺序，同一台 `windows-2022` runner 串行执行。分支并发排队，不取消已有实验；最长 240 分钟。单个编译进程 1200 秒超时会终止进程树，失败证据仍上传。

## 测量合同

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

本机 Linux 仅执行轻量测试；真实 Windows 编译结果以本分支 Actions 的 `validation.json`、`summary.md` 和原始日志为准。SDK 和运行文件前后完整哈希核对。产物自动保留 30 天，未取得 `succeeded=true` 不算完成 benchmark。

```powershell
python recovery/benchmarks/upheaval/prepare.py recovery/artifacts/upheaval-runtime
python recovery/benchmarks/upheaval/benchmark.py --runtime recovery/artifacts/upheaval-runtime/bin --output recovery/artifacts/upheaval-benchmark --runs 10 --warmups 1
```

准备器需要 `gh`、`GH_TOKEN` 和完整 git 历史；测量需要 Windows、原 SDK 及正式包已有的 .NET/VC/DirectX 前置依赖。输出目录必须不存在，避免覆盖历史结果。正式结果自动出现在 Actions Summary，并上传 raw logs、每次命令/计量/输出清单、输入清单、首对原始流和来源收据。
