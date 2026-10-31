# 编译器冷、热缓存性能优化

本轮从 `recovery/legacy-sdk-cache` / `665468cb` 拉出 `perf/compiler-cold-hot-cache`，范围是编译器冷缓存与热缓存的主要重复工作。当前候选生产源码为 `4d732a1407a3547b5f47a3d7e0f37b76de747b56`；Windows 验收与同机性能结果尚未完成，本文不预报加速比例。

## 优先级与实现

| 优先级 | 问题与改动 | 保留的行为 |
| --- | --- | --- |
| 首要：冷态 XML 校验 | 每份 XML 不再把已编译的共享 `XmlSchemaSet` 合并进另一个集合，改为直接绑定本次请求的共享 Schema 快照，避免重复合并/编译。 | 每份 XML 仍执行完整 XSD 校验；非法输入仍在插件派发前拒绝。Schema 身份及变化失效继续生效。 |
| 首要：多流会话加载 | 一条流成功保存会话后，同次请求的后续流复用已保存的 `SessionCache` 对象，避免反复解压、解析整个会话文件。每流仍调用 `InitializeCache`。 | 只复用成功保存状态；冻结、失败、部分保存及请求结束均不把未完成的内存状态交给下一流。文件、Schema、处理器版本及依赖检查保留。 |
| 次要：资产与实例查询 | `KeyedCollection2` 的查询索引改为哈希字典；`OutputManager` 的资产、旧资产及基础流身份查询改用 Ordinal 哈希索引。 | 集合遍历继续使用独立 `Items`；实际输出仍经 `OutputInstances` 与原排序规则生成。完整资产身份用于相等判断，哈希碰撞不合并不同身份。 |

会话对象复用必须重现反序列化后的瞬态状态。`AssetDeclarationDocument.ResetState` 同时清理保存实例的当前状态、Define 的原始表达式/所属文档以及 Include 的文档指针；已持久化的 Define 计算值保留，实例重新通过原有惰性初始化恢复。这样不会把上一流的已解析引用、XML 或 CDATA 输出路径带入下一流。

生产入口分别为 [XML 校验与实例输出](../src/Managed/Core/BinaryAssetBuilder.Core/AssetDeclarationDocument.cs)、[请求内会话复用](../src/App/Application/CoreStreamCompiler.cs)、[实例索引](../src/Managed/Core/BinaryAssetBuilder.Core/KeyedCollection2.cs)及[资产索引](../src/Managed/Core/BinaryAssetBuilder.Core/OutputManager.cs)。本轮没有改写排序算法，也未恢复 BIG/SKUDEF 包缓存；包仍每次正常生成。

## 历史证据的使用范围

已实际下载并复算当前生产基线 [41152199159](https://github.com/wu162/NewSdk/actions/runs/41152199159) 的归档，SHA256 为 `2ba381a049201eacf2de5ba6668e9f6061d81ccdf1eb878bda3af8d8d593138b`，五个 Release 文件也与原收据一致。[完整 Upheaval、jobs4 冷态 detailed](../evidence/refactor/cache-paired/baseline-41152199159/diagnostic-cold-4.json)中，报告总耗时 34.718 秒，925 份 XML/3354 次编译，base `core.xml.validate` 自耗时 **14.064 秒**（910 份文档）；GameObject 插件累计仅 0.302 秒。Audio/Animation 后端累计工作分别为 17.089/12.636 秒，可与前端或其他 worker 重叠，不能相加为总耗时。这份最近证据比旧 13.48 秒记录更直接支持本轮的冷态优先级，但仍不作为跨 VM 收益对照。

同一归档的[纯热态 detailed](../evidence/refactor/cache-paired/baseline-41152199159/defaults-hot-diagnostic-4.json)报告总耗时 2.210 秒，零解析/编译；五流 `cache.load` 合计约 0.812 秒，base 引用阶段约 0.307 秒。它说明原始完整 MOD 与下面 L8 扩容负载的规模差异，不将两者绝对值混用。

[L8 41139062989](https://github.com/wu162/NewSdk/actions/runs/41139062989) 的三个 `detailed` 样本是 **C08/C09/C12 修改后的增量构建**。其五流 `cache.load` 合计约 2.9–3.0 秒，base 的 `core.output.references` 约 2.46–2.58 秒，缓存文档重初始化 self 约 1.18–1.27 秒。计时来自旧二进制 `cc10391b`、扩容负载和另一台 VM；它们支持调查方向，不能解释为纯热态的阶段拆分，也不能与本轮结果直接计算收益。原始数据见 [L8 phases.csv](../evidence/refactor/l8-hot-cache/41139062989-1/phases.csv)及[负载/采样说明](HOT_CACHE_BENCHMARK.md)。

本轮新增批量计时，将 XML 校验分为 Schema 绑定、实际 XSD 校验、实例校验及文档哈希，将输出引用阶段分为引用闭包、资产可用性及排序。保留原上层计时，不把嵌套阶段相加当作总耗时；正常吞吐不逐资产计时。

## 同机测量方法

[已登记的 Benchmark 入口](../../.github/workflows/compiler-cache-benchmark.yml)在指定 `candidate_run_id` 后调用[配对工作流](../../.github/workflows/compiler-cache-paired.yml)和 [MeasureCompilerCachePaired.py](../scripts/MeasureCompilerCachePaired.py)，只有指定源码的 Core lifecycle 运行完整成功后才接受其候选 Release。基线复用 [41152199159](https://github.com/wu162/NewSdk/actions/runs/41152199159) 的已验收 Release，二进制源码为 `1a3e4bc6`；脚本核对它与本轮起点 `665468cb` 的生产输入闭包未变，并固定校验五个运行文件身份。

- 同一 Windows VM、同一 SDK、同一份完整历史 Upheaval 输入；默认不扩容。可另跑可达 Armor 分区扩容，但各负载分别报告。
- 默认三组独立冷态配对，交替基线/候选先后顺序；每个全新输出/编译器缓存目录之后各跑两个全新热态进程。两个热态样本共用同一冷态命名空间，并非独立冷态重复。
- 正式中位数只包含 `--performance off`：每实现冷态 n=3、热态 n=6。另有每实现一次双缓存关闭冷态，以及单独的 `detailed` 冷/热归因，均不混入正式中位数。
- 冷态指编译器缓存与输出命名空间初始不存在；不控制操作系统页缓存。同一冷进程的后续流可以复用此前已完成流的文档。
- 每次进程计时后比较全部实际发布文件原始字节；热态还要求 XML 解析和后端编译为零、缓存文档复用为正。记录外部 wall、报告 elapsed、诊断数、缓存计数、机器与运行文件身份。
- 工作流最长 15 分钟，测量脚本预算 720 秒、单进程最多 180 秒；超时与部分数据保留。小样本仅描述该机器和负载，不作通用速度承诺。

性能配对不替代生产验收；生命周期、非法 XSD、保存会话重入、碰撞/顺序、冷热/失效/冻结/取消、完整 mymod 原版字节及五文件包等仍以本轮 Windows 实际收据为准。

## 本轮结果待填

截至本初稿：

| 项目 | 当前状态 |
| --- | --- |
| 候选源码 | `4d732a1407a3547b5f47a3d7e0f37b76de747b56` |
| Windows Core/CLI/缓存/SDK/ZIP 验收 | [41165490728](https://github.com/wu162/NewSdk/actions/runs/41165490728) 正在运行，尚未验收通过 |
| 首轮运行 | [41165381586](https://github.com/wu162/NewSdk/actions/runs/41165381586) 尚无最终状态；源码缺少本轮随后补齐的会话瞬态重置，不可当作最终候选验收 |
| 同机冷/热配对运行及原始归档 | 待上述候选成功后取得并核验 |
| 冷态/热态中位数、全部原始样本与较慢数据 | 待填，当前不报告收益 |
| 新增 detailed 分段归因及覆盖限制 | 待填 |

后续应在此更新实际终态、源码/二进制/测试身份、下载归档与发布文件校验结果。失败与旧运行保留，不能把运行中或部分成功写成整体验收通过。未经用户明确批准不得合并，禁止强推。
