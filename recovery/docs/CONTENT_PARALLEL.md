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

报告的 `contentExecution` 包含实际组数、Owner 线程、各流 Compile 单调时钟区间和共享预算峰值。
缓存根计数是各组之和，`cache.groups` 保留原组报告；全构建资源峰值以
`contentExecution.resources` 为准。沿用低内存单个大资产的独占回退，单项可能超过估算预算，
但不允许其他在途结果同时占用；这不是严格进程内存上限。

## 验证状态

本地静态编译已通过 App（net48/C#7.3）和完整托管闭包；可移植预算/进度/性能及
Core 哈希/trace/上下文合同通过。Linux 预检不代表 Windows 原生验收。

`content-parallel.yml` 构建一次当前 Release 和同对象验证宿主，运行现有 Core/CLI 合同，
并通过 `TestContentParallel.ps1` 覆盖七种非空组合、APT/maps 多入口、冷热、jobs1/4
四流与 BIG/SKUDEF 原字节、实际 Compile 区间重叠、缺组缓存、进度开关、失败/取消/重试，
以及一对完整历史 Upheaval base 编译。显式跨组 DAG 的就绪调度由 App 受控后端合同覆盖。
当前 Windows 验收待执行，不将预检计为真实 SDK 或原版差分通过。
