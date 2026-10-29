# 缓存性能基准

`BenchmarkCompilerCache.py` 使用已验收的 Win32 Release 五文件包，测量独立进程从启动到退出的墙钟时间、CPU 时间、峰值工作集，以及实际缓存命中、XML 解析、文档复用、资产编译计数和持久缓存大小。只测 loose/base 编译；下载、输入准备、字节校验及统计不计入单次进程耗时。

## 规模与时间限制

- 完整仓库 `mymod` 输入副本：保留全部输入，选择 base，关闭打包。
- `upheaval-armor`：固定历史 Upheaval 的 11 个原始 `Armor.xml`、12 个 ArmorTemplate，另生成 includes 入口；原文件字节不改。它是独立类型子集，不代表完整 Upheaval，也不覆盖纹理、Audio 或 GameObject 的性能。
- `texture-16`：16 个独立 Texture 共享生成的 256×256 TGA，属于合成样例，用于测量 SDK 明确启用资产缓存的类型。
- 每进程最多 30 秒，整组预算 180 秒；准备步骤最多 2 分钟，整个 Windows job 最多 8 分钟。超时和预算耗尽保留证据、返回失败，不把部分结果标为完整成绩。
- 固定生产来源 `39039961847cd1ca9cb93743fd51741a4e5c9f49`、成功运行 `40814401237`、归档 SHA256 `6c395456add02dec40c7759e9cc5a26d34ea353f5e3ee588a12a78df3bfab044`。不重新编译生产代码、不运行完整 SDK 回归。

## 比较维度

| 模式 | 资产 bc | 会话 sc | 压缩 csc |
| --- | --- | --- | --- |
| none | false | false | false |
| asset | true | false | false |
| session-xml | false | true | false |
| session-deflate | false | true | true |
| both-xml | true | true | false |
| both-deflate | true | true | true |

每个 workload 的无缓存运行 3 次；其他各模式先跑 1 次空缓存，再跑 2 次热缓存，共 54 个新进程。各模式使用独立目录，轮次之间反转执行顺序。无缓存及资产缓存单独开启时，每次使用新的 `/iod`，使资产命中确实来自资产缓存。会话模式保留自己的会话及中间结果。其余选项固定 `/psc:true /fsc:false /atc:false`，保持原默认排序、预编译引用和慢清理策略。

原 SDK 的 GameObject、WeaponTemplate、ArmorTemplate 和默认插件未启用 `useBuildCache`，因此前两个 workload 的 bc-only 命中/未命中均为 0，会明确标为类型不适用。准备后的 SDK 配置及实际声明类型提供这一判断依据；不将零活动算成缓存复用。`texture-16` 热资产缓存必须真实命中，热会话必须复用文档且不重新编译实例。

“冷”仅指编译器缓存为空；不清空 OS 文件页缓存。进程启动、JIT、链接、发布及日志成本均计入墙钟时间。样本较少，只报告中位数和 min–max；冷热结果不宣称统计显著。加速比为同 workload 的无缓存墙钟中位数除以对应行中位数，低于 1 表示更慢。缓存大小包括持久会话、资产、中间及 CDATA 状态，不含正式发布文件。

开始计量前先运行独立 `doctor` 启动诊断，其耗时单列并计入总预算，不计入构建样本；所有样本因此共用已加载的 EXE/OS 页缓存状态。工作流使用与原验收相同的 x86 MSVC 环境；启动器禁用无人值守 Windows 错误对话框，运行库错误必须退出或触发原有超时上限。

每个有效样本必须成功发布，实际缓存开关及计数正确，全部输出成员、长度和 SHA256 与无缓存一致，运行前后的五文件运行库、SDK Schema/配置及 MOD 输入不变。失败样本保留原记录，不纳入成绩。

## 重复运行

准备器核对完整归档哈希、成功发行收据、内层 ZIP、精确五文件及 Texture 与 EXE 绑定，并拒绝生产、SDK 或构建输入已变化的复用。

```powershell
python recovery/scripts/PrepareCompilerCacheBenchmark.py --repo . --output "$env:RUNNER_TEMP/cb"
python recovery/scripts/BenchmarkCompilerCache.py --preparation "$env:RUNNER_TEMP/cb/preparation.json" --sdk-root "$PWD/SDK" --output recovery/evidence/refactor/cache-benchmark/local --timeout-seconds 30 --budget-seconds 180 --hot-runs 2
```

准备器可传 `--archive <已下载的外层ZIP>`；省略时使用已配置的 GitHub CLI 下载固定产物。输出目录必须不存在。运行需 Windows、Python 3.11+，以及已验收五文件包现有的系统运行库依赖。原始 JSON、CSV、Markdown 及各次请求/报告/日志都保留；不上传重复的工作目录和运行库。

工作流为 `.github/workflows/compiler-cache-benchmark.yml`；仅显式 `[cache-benchmark]` 提交或手动触发才执行，不会触发生产重编译。最新实测结果将记录在本页及对应运行的原始产物中。

首轮[运行 40819603709](https://github.com/wu162/NewSdk/actions/runs/40819603709) 的 failure 保留：18.6 秒后因 mymod 的热资产缓存活动为 0 而拒绝了成绩。该结果暴露了基准对类型适用性的错误假设；后续依据原 SDK 的类型配置分类，并增加明确适用资产缓存的 Texture 样例，生产行为未修改。

[运行 40819993842](https://github.com/wu162/NewSdk/actions/runs/40819993842) 在首次 mymod 进程启动 30 秒后超时，stdout/stderr 均为空，未取得任何有效样本，保留 failure。不能将它计为编译慢或缓存收益；后续补齐原验收 x86 环境并加入显式启动诊断，未延长时间上限。
