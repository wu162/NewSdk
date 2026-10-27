# 编译器优化：按需读取

实施分支：`feature/compiler-optimization`；固定基线：`compiler-main@e7396d3b6fb302682d3610ebbc6e99ce87eb867f`。
[PLAN.md](PLAN.md) 原样保存用户提供的 Final v3 设计，属于设计与证据材料；执行范围、时限、代理数量和操作权限以用户当轮要求及仓库约定为准。

续接先读 [HANDOFF.md](HANDOFF.md)，再按下表选择方案章节和源码。不要每轮载入全方案、全部历史交接或反编译目录。方案中的“待验证”不等于已复现，接口与参数提案不等于已上线。

| 当前问题 | 方案章节 | 主要源码/证据入口 |
| --- | --- | --- |
| 顺序与放行 | 1、19 | 本页、HANDOFF.md |
| Core 三项风险 W01 | 3、5.3、17 | CompilerCoreLifecycle.cs 的 DefineOverrideProbe / CrossStreamBindingProbe / LinkedSideFileProbe |
| 工作计量 W02 | 18 | DocumentProcessor.GetWorkMetrics、CoreStreamCompiler.CacheReport.coreWork |
| 摘要读取 W03 | 8.1、9.6、10.1 | FileHashItem.cs、CompilerInputDigests.cs |
| schema 元数据 W04 | 4.3、10.1 | SchemaSet.GetReferenceDescriptor、CompilerSchemaDescriptors.cs；保留闭包内容校验 |
| 影子依赖 W05 | 6、7.3—7.4 | AssetDeclarationDocument.cs；先记录，再授权跳过 |
| 每流绑定 W06 | 3.7、7.6 | InstanceDeclaration.cs、AssetDeclarationDocument.cs |
| 新缓存 W07 | 8—9、16—17 | BuildAssetCache.cs、SessionPackage.cs；未满足完整合同前不开放 |
| 局部索引 W08 | 10.2—10.3、17.1 | NodeJoiner.cs、CompilerNodeJoiner.cs |
| 状态与并行 W09—10 | 11—14 | CoreStreamCompiler.cs；先隔离再并行 |

源码位于 `recovery/src/Managed/Core/BinaryAssetBuilder.Core/` 和 `recovery/src/App/Application/`，探针位于 `recovery/tests/`。验证入口：`.github/workflows/refactor-core-lifecycle.yml`，提交标记 `[refactor-core-lifecycle]`；全部编译、测试与微基准在 GitHub Actions 执行。

## 本轮合同

- 每轮最多 120 分钟，尽量跑满时间持续推进并交接；效率优先、正确性是底线，完整能力组完成后继续有价值且依赖满足的已授权工作，不空等凑时长；通常 2 个子代理，最多 3 个。所有成果普通提交到本页指定的新分支；不强推、不自行合入主线。
- HANDOFF 只保留当前结果、准确 sourceCommit/runId、未决事项和下一步，目标不超过 120 行；替换旧状态，历史由 git 保留。原方案只保存一份，不往交接里复制章节和运行日志。
- 不改变 Win32/.NET4.8、插件 ABI、legacy 摘要/输出顺序、缓存默认关闭和 Texture 缓存边界。不将微基准当作完整构建提速。
- 试验能力提供 AppContext 回退开关：`NewSdk.Compiler.DisableSinglePassDigest`、`NewSdk.Compiler.DisableNodeJoinerIndex`、`NewSdk.Compiler.DisableSchemaReferenceDescriptors` 为 true 时分别恢复历史双遍摘要、线性子节点匹配和每次重算 schema 引用描述。`NewSdk.Compiler.DisableSharedSchemaMetadata` 为 true 时关闭构建内继承/依赖元数据共享，仍逐流构建私有表。可在 EXE.config 的 `runtime/AppContextSwitchOverrides` 配置；不新增 CLI 参数。

## 当前计量边界

`coreWork` 按任务保存实际调用次数、抛异常次数和单调时钟区间：rawParse、definitions、expressions、inheritance、validation、assetProcessing、referenceBinding、manifest、link。rawParse 包含 resolver/reader 初始化与 XInclude 展开；assetProcessing 包含资产准备/缓存等工作，不等于纯插件编码。返回失败状态而没有抛异常的调用，不计入 failedCalls。

`coreWork[].outerStages` 记录 settings、providerPreparation、cachePreparation、schemaPreparation、sessionPreparation、processDocument、artifactCollection、cacheValidation、cacheStage、cleanup 中实际执行的区间；cachePreparation 在缓存关闭时仍有空检查开销。cleanup 计每个实际释放的 scope，尚不包含失败后删除工作目录。`schemaPreparation` 是 SchemaSet 生命周期快照，细分 schemaRead、schemaCompile、schemaMetadata 和需要时的 schemaContentCheck；schemaMetadata 内再分 schemaHelpers、schemaMetadataBuild 或 schemaMetadataRestore。`schemaInputs` 记录成功读取文件次数、成功读取总字节和内容复核文件次数，不代表唯一文件或物理磁盘 I/O。

顶层 `applicationWork` 记录规划、backend 初始化/逐流编译/释放、artifact 校验、BIG 包装、发布 Prepare/Commit 及缓存 Commit。`captureBoundary=beforeInitialReport` 明确快照在初次写报告之前；未覆盖初次报告写入、终态事件/日志关闭及最后 Complete/回滚成本，不把差额全部归因给 schema。嵌套区间重叠，不能相加求总 wall time；旧顶层 elapsedMs 与 stages 合同保留。

FileHashItem 的 DigestReadCount / DigestBytesRead 是每项成功完成的逻辑摘要读取计数，不是操作系统物理磁盘 I/O。新文件、已知时间戳变化使用单遍双摘要；可能未变的输入保留 SHA-only 检查，同时间戳内容变化走保守重算。首轮试验中全量双摘要的暖路径有额外 CPU 成本，因此没有保留该策略。已接入跨流正内容 PhysicalInputSnapshot；是否验收通过以短交接的准确source/run为准，不能以mtime/size代替内容复核。

`coreWork[].inputDigests` 汇总本流 SessionCache 已登记 FileHashItem 的成功摘要 reads / bytes，items 为去除对象重复后的项数；包含会话初始化内容复核，同一路径不同配置可以重复计数。不含 schema 专用摘要、插件绕开 FileHashItem 的读取和外层输入快照/发布校验。

## 构建内 schema 元数据共享边界

CoreStreamCompiler 持有每次构建独立、单 entry 的 MetadataCache，Dispose 时释放。共享值只有自有的有序继承类型名和资产依赖计数，保存与恢复均复制；调用方仍拥有可变 StringCollection/IDictionary，不会把一流的修改泄漏到下一流。每流仍完整读取 schema 闭包、SHA256、编译自己的 XmlSchemaSet 并绑定本流 helper/hashable 类型。引用描述仍属于本流 Framework 对象，不跨流复用。

键包含实现版本、规范根路径、读取顺序中的实际路径及每文件内容 SHA256、countDependencies、当前文化/比较文化与 StringHashBinDescriptors。仅成功构建表后发布快照；不同根、内容（含同大小/同时间戳）、模式、文化或描述符保守失效。`coreWork[].schemaMetadata` 的 enabled/builds/hits 是本 SchemaSet 生命周期计数。此为 W04 子组，不代表共享整个 SchemaSnapshot，也未完成 W03 输入封存或 W05 影子依赖。

真实多流入口仍为现有 CLI business-combined 与三个独立单组；`business-work-metrics.json` 分别记录源码/二进制来源、实际 schema 读/编译/表构建及恢复次数和区间，保留原 BAB 与四输出字节比较。生效与验收状态以 HANDOFF/VALIDATION 的准确 sourceCommit/runId 为准。

## 正内容封存、影子查询与基准匹配边界

`PhysicalInputSnapshot`仅封存实际接入的正内容，文件只读句柄和祖先目录防替换句柄保证原路径消费者期间内容不变，XML/XInclude/schema消费封存流。默认预算为32MiB内存、512MiB私有磁盘、4096文件路径与4096目录pin；预算、不支持路径、generated output及opaque Texture等回退有独立reason计数。大文件副本保持独占handle，读取只暴露独立只读游标；只清理自己创建的空scratch，不递归删未知目录。

`cache.physicalInputs`按snapshotId记录captures/hits、capturedBytes、memory/diskBytes、verificationReads/Bytes、active pins与bypassReasons。activeFilePins为unique内容owner数，activeAliasPins为额外名称handle数，activeReadHandles为二者合计；hardlink名称各自保留handle，不能只凭一个inode handle宣称所有别名不可unlink。它是当前generation的逻辑原文件读取/共享计量，不是物理磁盘I/O；beforeInitialReport快照中pins仍活跃，真实Run返回后应全部释放。封存文件同目录中的其它未消费文件仍可编辑；不长锁整个SDK内容。

BuildSession在provider Dispose后、发布前强复核捕获文件内容与身份，pins保留至publication/cache确认、必需报告/事件sink关闭后才释放。直接Core失败尝试结束整个封存generation，允许原同backend修复重试；下一次Compile新建snapshot并强比较Initialize配置原始SHA，配置改动须重新Initialize新backend。失败generation先前产生但未发布的所有结果失效，BuildSession原合同会丢弃其全部artifacts；直接Core调用方也应丢弃失败代结果。不能在已成功且仍可发布的同代中解pin后继续消费修改内容。

已接入source、XInclude、schema及已登记FileReference原路径资源（含APT DataBlob/Audio/header），第四轮扩展两个Core入口实际消费的precompiled/base patch root/递归patch manifest及发现的StringHashes.xml；准确验收状态见HANDOFF。Utility保留原重载与共享fallback，生成输出根不锁定；只读manifest元数据不等于封闭其它BinaryAsset缓存/物化读取或Texture隐藏读取。`positiveInputsOnly=true`不代表全部依赖已接入；保留/bc /sc全根复核、Texture缓存拒绝及原字节/失败门槛。回退开关为`NewSdk.Compiler.DisablePhysicalInputSnapshot`。取消在schema、XInclude与manifest包装边界保留原token。

`cache.dependencyObservations`为W05有界影子证据，scope=ResolverCandidatesAndSemanticReads、canAuthorizeReuse=false。保留原resolver候选File.Exists/选择/错误及实际Define绑定/值/安装epoch/read-site、同文档prevalidation或跨文档processed继承消费、精确/派生符号查询与选择/歧义；每次原查询继续执行。事件只保存immutable scalar/string，保留task/query/parentQuery/sequence，不反推旧依赖。默认8192条、1,048,576 UTF-16字符预算，不是RAM或UTF8字节；整条丢弃后计数继续，truncated/dropped明确。DisableDependencyObservations关闭观测；未以legacy XML hash授权强缓存命中。

`cache.physicalInputs.resolverCandidates`另存原resolver及manifest发现/asset存在查询已执行的布尔结果；解析阶段不额外探测、不复用答案，发布前VerifyUnchanged逐路径复核，已观测代内冲突或终验变化拒绝本代结果。最多8192路径/1Mi UTF-16字符，超限、不支持路径及输出根明确incomplete。只证明复核时点的已登记布尔存在性，不是原子快照、路径查询锁、文件身份或完整查询闭包；实际内容身份仍由正内容pins/SHA保障，原缓存全根校验保留。

## 每流绑定与精确表达式准备

W06子组以每根文档私有map保存强引用每次出现/顺序、weak规则和依赖闭包，manifest、排序及资产报告消费当前root结果。成功PrepareOutputInstances后GetStreamBindings发布只读标量快照，失败不发布；执行期仍保留legacy live Handle语义。DisableStreamBindingResults恢复旧共享字段算法。尚未拆出完整不可变AssetSemanticResult，不代表并行安全。

W08 exact程序以DocumentProcessor弱生命周期拥有有界LRU：首遍原ANTLR parser仍按原序实际求值，在成功leaf/运算后观察不可变语法节点，整expr及AsString成功才发布。命中后重做原Value工厂/运算、当前Define读取和文化转换，不缓存值/绑定、不提前吞错或短路。默认128 entry、262144 UTF-16字符（key+leaf文本）、32768节点；单表达式16384字符/4096节点/depth128，超限继续原parser且不入缓存。DisableExpressionPrograms关闭；coreWork[].expressionPrograms记录实际parserCalls/builds/hits/evictions/fallbacks/失败与预算，hits不等于完整构建提速。

固定en-US格式可共享只读CultureInfo，DisableSharedExpressionCulture恢复每Value独立文化对象。此开关不改变原环境文化参与的数字转换；组件原DLL四文化差分、所有开关组合和真实XML四输出验收身份以HANDOFF为准。

W09当前只隔离实际copy/link传输scratch：每DocumentProcessor捕获DisableOwnedTransferBuffers开关，自有1MiB copy及64KiB link数组按实际消费懒分配并复用；无DP或关闭时使用原静态数组，静态fallback同样按需分配。保留copy锁及原循环/短块/错误/临时文件行为。coreWork[].transferBuffers分别计自有allocations/bytes与legacy请求，不把全局fallback分配算进owner。增加每DP持有数组可能改变多流分配量，不能据此声称CPU提速或完整内存峰值下降；原backend租约及Chunk/native/其它全局限制保留，未开放并行。

W08基准manifest索引仅替代既有候选查找与Remove的查找阶段：有序重复队列+Fenwick定位当前秩，保留原base顺序、patched首项及即时List2.RemoveAt。小组、派生/非法候选及别名列表走原路径；实际外部TraceWrite回调后永久退回原查找/移除，以保留回调修改任意Handle或集合的行为。DisableBaseManifestIndex可关闭；`coreWork[].baseManifest`记录doc工作量，不含List2内部数据搬移，不能称整排序O(N+B)或完整CLI提速。默认/tl3有回调时放行范围保守。
