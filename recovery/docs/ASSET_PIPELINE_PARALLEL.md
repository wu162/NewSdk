# 资产全流程并行

实现分支：`feat/asset-pipeline-parallel`。方案与源码基线：
`1810fff87727b70779f31328740bb0c2133157d2`（`recovery/legacy-sdk-cache`）。

当前是集成检查点，**尚未通过 Windows 完整构建和真实 SDK 验收**。
本地 Roslyn 对 Foundation、App（原生桥接使用明确的类型检查桩）及关键测试的
编译检查不代表原生编码器、最终 EXE 或 MOD 输出通过。

## 执行与所有权

- Owner 完成 Include、表达式、继承和 XSD，提取 Schema 的惰性元数据。
  每个资产的原始 XML Hash、引用解析及引用尾 Hash 进入纯计算任务；同一文档的
  只读批次全部结束后，Owner 按原次序改写 DOM、登记字符串 Hash 和文件依赖。
- `HashingWriter` 继续使用原 512 字符分组和 ANSI 算法；引用尾仍 Hash
  `MemoryStream.GetBuffer()` 的容量范围。没有整文档 XML 深拷贝。
- `OutputManager` 以完整 FileBase 保持唯一 `BinaryAsset`；探测对象仅在首次
  注册前绑定实际声明，注册后不替换。调度器保留物理任务，重复请求各自验证、
  订阅完成，只编码、写入、计数一次。
- 已认证的验证器、插件和独立文件 IO 在 Worker 执行。验证在编码之前，
  线程本地诊断和纹理消息由 Owner 在原文档上下文回放。未知验证器/插件留在
  Owner 兼容路径；FLA 已展开缓冲直接交给独立 IO，不重新编码。
- `PrepareCommit` 在 Owner 捕获路径和状态；`BinaryAssetCommitWork.ExecuteIO`
  写入私有结果；`PublishCommit` 在 Owner 发布 Header、availability 和自定义
  输出回调。复制缓冲由工作私有持有，没有全局复制锁。
- 外部输入租约和私有 `.cdata` 持有到本资产 IO 结束。共享 BuildCache 文件先
  写入唯一临时文件，再发布最终名称，避免不同输出流看到半文件。

## 调度和文档版本

CPU、IO 和在途内存分别计量。完成的编码任务立即释放 CPU 名额，结果缓冲仍
计入内存；缓存物理 IO 不占编码名额。Owner 只收割已完成任务，资源不足时等待
能释放相应资源的任意任务，没有 FIFO 队头或 committedSequence。

每个文档版本、每个输出流维护独立订阅和只读输入引用。稳定 Include 复用已
发布前端；需要重新加载时创建新的文档和 DOM，SessionCache 的请求内替换不改
磁盘格式。旧版本完成时不能覆盖新版本。只有相关工作成功，Owner 才能执行
MakeComplete、MakeCacheable、缓存保存和 Reset。

输出屏障等待对应 OutputManager 的全部订阅。引用闭包、排序、Manifest、
LinkStream 和 BIG/SKUDEF 沿用最终输出阶段及原格式；失败回收所有已启动任务，
不为未完成版本写入成功缓存，不新增全流程回滚事务。

## 验证与计量

关键测试覆盖慢首任务与快后任务、同 FileBase 独立验证和唯一物理工作、CPU 满载
时缓存 IO、文档引用计数、失败/取消后的任务与租约回收，以及不同命名空间的
并发验证和 Owner 诊断回放。原 Core、CLI、缓存、SDK 和取消门槛继续保留。

统一 Windows 入口为 `refactor-core-lifecycle.yml`，最终代码提交标记
`[refactor-core-lifecycle]`；该运行新构建 Debug/Release 并执行既有完整验证。
通过后复用候选二进制，对基线验收运行 `41192817627` 做完整 Upheaval 冷/热
原字节与耗时配对比较。来源以实际 sourceCommit/runId 为准。

报告在 `cache.parallel` 中分别输出 XML 前端/纯计算、验证、编码、资产 IO、
Owner 确认、CPU/内存资源等待及 IO 排队耗时，另含 CPU/IO/Owner 待收割和在途
结果字节峰值。各 Worker 累计耗时可重叠，不作为端到端墙钟耗时相加。
最终结果和实测限制取得后再更新本页，不预设提升百分比。
