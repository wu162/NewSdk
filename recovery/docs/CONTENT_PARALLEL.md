# 单进程按内容分组并行

基线 `recovery/legacy-sdk-cache` / `680c4f0c4439bd7435addebfc57a1a16c3c320e3`。

`BuildSession` 在主线程完成计划、MOD CacheLock 和 SDK 有效配置解析。仅对计划中存在的
base/apt/maps 创建 Owner，分别持有 Core、Schema、插件、会话缓存和性能收集器。
每组顺序执行入口；主调度器按任务依赖就绪派发，不等待另一组整体完成。`--jobs 1`
仅允许一个流运行，其他 jobs 在全构建 CPU/IO/在途结果预算内执行。没有生产子进程。

Core 的 Settings、文档/流栈、哈希表、trace/报告作用域以
`AsyncLocal<CompilationContext>` 隔离，托管 Task 继承上下文。实例仍检查线程亲和及不可重入。
每组使用 `session/<group>`、`build-cache/<group>`、`intermediate/<group>`；
`streams/<group>/<entry>` 和 BAB 缓存格式保持原合同。串行/并行使用相同分区；旧缓存不删除。
组内多入口复用自己的 savedSessionCache。缺失组不会创建或触碰其分区。

Owner 仅向线程安全消息队列写开始/结果/诊断，主线程持有事件 sink、报告与最终发布。
失败/取消停止新任务，并等待已开始的原生编码、IO、Owner Dispose 后返回；失败不打包。
进度计数绑定 streamId，终端保留每秒单行和 `NEWSDK_PROGRESS=off`；JSONL Progress
继续仅计成功完成的流。性能收集器在 Owner 私有，结束后主线程合并。
主线程登记流并确认 StreamStarted 后，Owner 才开始初始化/编译，避免短流计数丢失。
清理异常附在原失败/取消诊断后，每个已开始流只有一次结束事件；空组缓存报告保持对象合同。

报告的 `contentExecution` 包含实际组数、Owner 线程、各流 Compile 单调时钟区间和共享预算峰值。
缓存根计数是各组之和，`cache.groups` 保留原组报告；全构建资源峰值以
`contentExecution.resources` 为准。沿用低内存单个大资产的独占回退，单项可能超过估算预算，
但不允许其他在途结果同时占用；这不是严格进程内存上限。

## 验证与计量

生产/测试 `1b9da37a00beb639c8c56590417ef51bb210c3cb`，
[Windows 41287090604](https://github.com/wu162/NewSdk/actions/runs/41287090604) 整体 **success**。
本轮重新构建 Release 及同生产对象验证宿主，未复用旧 EXE。
App 3268 合同、六组真实 Core/应用合同、58 个 CLI 样本（53 个预期成功、5 个失败/取消边界）
以及全部 58 份报告 Schema 通过。两个真实 Core 的作用域内重叠、实例线程/不可重入约束、
DAG 任务及时就绪、失败/取消与清理异常、短流进度启动竞态均有合同。

CLI 覆盖七种非空组合，各可选组两个入口，jobs1/4 冷热四流和 BIG/SKUDEF 原字节；
另有真实 Flash8 FLA + 文件 APT XML 两入口、实际 MapMetaData 两入口、缺失/空选择、
缓存隔离、进度开关和实际 CtrlBreak/重试。每个生产进程均受单进程 Job 约束，无子进程。
七组合中的 Armor 入口用于调度和缓存矩阵；定向业务不代表完整 APT/Maps 内容验收。

以下为同轮小型调度输入的冷构建进程墙钟，单位 ms；每条件仅一个样本。
计时包含进程启动和初始化；各流 Compile 区间、冷热缓存命中、峰值工作集、
分组耗时和其余全部原始条件在 [summary.json](../evidence/content-parallel/41287090604-1/summary.json)。

| 内容 | Owner 数 | jobs1 | jobs4 | jobs4 跨组 Compile 最大交叠 ms |
| --- | ---: | ---: | ---: | ---: |
| Base | 1 | 870 | 825 | 0 |
| APT | 1 | 906 | 946 | 0 |
| Maps | 1 | 881 | 979 | 0 |
| Base+APT | 2 | 1160 | 1061 | 627.047 |
| Base+Maps | 2 | 1173 | 1000 | 622.399 |
| APT+Maps | 2 | 1282 | 1108 | 673.818 |
| Base+APT+Maps | 3 | 1594 | 1218 | 790.387 |

三组实际峰值 Owner=3；热构建墙钟 1032→852ms，热态 0 解析/0 资产编译、5 次会话命中。
三组冷 jobs4 峰值工作集 77,201,408 字节，CPU/IO 峰值 4/4，结果预算峰值
9,060,928 ≤ 536,838,144 字节；全部样本的共享预算均通过。

完整历史 Upheaval 的 base 流做一对冷构建，jobs1→4 为 53.098→32.521 秒，
峰值工作集 189,177,856→185,536,512 字节，1295 个发布文件/75,093,395 字节原字节相同。
这对样本只有 base，反映同轮 jobs 差异；OS 页缓存未控，没有统计重复，不能据此量化
完整三组 Upheaval 或通用性能收益。没有新 Debug、原 SDK 或游戏运行验收。

最终 ZIP 单次下载，96,706,437 字节，SHA256
`7541c94bc3fb1d43555f6ab58f6b4a84688b1ce29a5a514631dfde7795e4acd6`。
独立核对生产/宿主六运行文件、源码与链接收据，并直接比较归档内 18 对原始输出，
每侧共 1413 文件全部相同。新生产 EXE SHA256
`6df773d0f9101713a53f42b36266c36d43d276513bcf62934626564e8f899680`。
[最终独立复核](../evidence/content-parallel/41287090604-1/archive-verification.json)
保留身份、Schema、资源和逐对字节比较边界。

## 保留的先前记录

- `41285506496`：最后一次取消边界修补引入同名变量，App 编译预检失败；随后修复。
- [41285596210](../evidence/content-parallel/41285596210-1/archive-verification.json)：Release 与四组 Core 通过，旧全局 lease 测试断言阻止继续；已替换为真实并发及实例约束验证。
- [41286126589](../evidence/content-parallel/41286126589-1/archive-verification.json)：新 Release 与六组 Core 通过，CLI 脚本误向 loose 传入 BIG 身份参数，首样本提前拒绝。
- [41286811137](../evidence/content-parallel/41286811137-1/archive-verification.json)：核对闭包后复用该 EXE，58 样本及原始字节通过；独立 Schema 为 57/58，发现空组 cache:null。最终 1b9da37 已修复并重新构建/完整验证。

[PR #48](https://github.com/wu162/NewSdk/pull/48) 待审，未合并。禁止强推。
