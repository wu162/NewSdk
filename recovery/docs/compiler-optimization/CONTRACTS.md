# 当前实现合同与诊断开关

本页说明实现边界；验收状态以 [HANDOFF](HANDOFF.md) 和 [VALIDATION](VALIDATION.md) 的准确 sourceCommit/runId 为准。[PLAN](PLAN.md) 是设计材料，未完成提案不能用来授权强缓存命中或并行。

2026-10-28：当前完整接受 `71b5a0a4 / 40598874896`；`e4169602`两fixture失败，修正真实事件arm及text/typed DOM证据的`6db109af / 40602153873`正在验证。来源与终态集中于 VALIDATION。

## 工作计量

`coreWork` 保存实际调用次数、抛异常次数和单调时钟区间：rawParse、definitions、expressions、inheritance、validation、assetProcessing、referenceBinding、manifest、link。rawParse 包含 resolver/reader 初始化和 XInclude；assetProcessing 包含准备与缓存工作；返回失败状态而未抛异常不计 failedCalls。嵌套区间不可相加求总 wall time。

`outerStages` 记录 settings、providerPreparation、cachePreparation、schemaPreparation、sessionPreparation、processDocument、artifactCollection、cacheValidation、cacheStage、cleanup 的实际区间。缓存关闭时仍有 cachePreparation 空检查；cleanup 计实际释放的 scope，不包含失败后目录删除。schemaPreparation 内分 schemaRead、schemaCompile、schemaMetadata、schemaContentCheck；schemaMetadata 再分 helper、构建与恢复。

顶层 `applicationWork` 记录规划、backend 初始化/逐流编译/释放、artifact 校验、BIG、发布 Prepare/Commit 和缓存 Commit。`captureBoundary=beforeInitialReport` 不含初次报告写入、终态事件/日志关闭以及最后 Complete/回滚成本，不能把差额全部归因于 schema。

`schemaInputs`、`inputDigests`、`physicalInputs` 是逻辑读取/共享计量，不是 OS 物理磁盘 I/O；同一路径不同配置可以重复计数。`inputDigests` 汇总本流 SessionCache 已登记且按对象去重的 FileHashItem，包含会话初始化复核，不包含 schema 专用摘要、外层输入快照或绕过 FileHashItem 的插件读取。

局部报告还有 expressionPrograms、expressionTraversal、nodeJoinerPredecessors、symbolLookup、stableSortMembership、baseManifest、transferBuffers。只按其字段说明已执行的工作，不用索引命中数或组件微基准推导完整 CLI 提速。

## 正内容封存与查询复核

PhysicalInputSnapshot 由本次构建拥有，实际正内容使用只读文件句柄和祖先目录防替换句柄；XML/XInclude/schema 消费封存流，原路径消费者仍受 pins 保护。默认预算：32 MiB 内存内容、512 MiB 私有磁盘内容、4096 文件路径、4096 目录 pin。预算不包含所有托管对象/传输 scratch，不能称 RAM 峰值上限。超限、不支持路径、generated output 和 opaque Texture 各有回退原因。

大内容保留独占私有文件，仅暴露独立只读游标；只清理自己创建且为空的 scratch。`activeFilePins` 为 unique 内容 owner，`activeAliasPins` 为额外名称 handle，`activeReadHandles` 是二者之和。硬链接别名各自保留 handle，不能凭一个 inode handle 宣称所有名字不可 unlink。同目录未消费文件仍可编辑。

已接入 source、XInclude、schema、已登记 FileReference 原路径资源（包括 APT DataBlob/Audio/header）、precompiled/root 与递归 patch manifest、发现的 StringHashes.xml。输入代结束前强校验 SHA 与身份；终验复用 64 KiB buffer，在各次同步 Read 前后和 EOF 检查 token，不承诺中断底层 OS Read。

BuildSession 在 provider Dispose 后、发布前强复核，pins 保留至发布/缓存确认和必需 sink 关闭后。直接 Core 失败会结束该代，旧代未发布结果必须丢弃；同 backend 重试建立新代并强比较 Initialize 配置，配置变化需重新 Initialize。`beforeInitialReport` 快照中 pins 仍活跃，Run 返回后应全部释放。

resolverCandidates 只记录原 resolver/manifest/asset 存在查询已执行的布尔答案，不缓存答案、不改变原探测顺序；发布前逐路径复核，代内冲突或变化拒绝该代结果。默认 8192 路径/1 Mi UTF-16 字符，丢弃与不完整明确记录。它不是原子路径快照、查询锁或完整闭包；正内容身份仍靠 pins/SHA，原 `/bc`、`/sc` 全根验证保留。

## 可修复缓存物化

BinaryAsset 新封存仅用于有效的外部 legacy Cache 来源。Local 属于可变输出，始终保留原复制、删除、坏输入拒绝和修复重试；App 的 staging cache 仍被输出根排除，不宣称 App `/bc` 已利用此共享。

原 CopyAsset 的 Read、短块拒绝、输出 Write/Flush、临时文件和错误顺序保留。header identity 与完整声明长度合格后，同步把已经成功写出的实际 chunks tee 到有界私有存储；SHA 增量计算，legacy FastHash 独立按规范 1 MiB 块累计，不能依赖 FileStream 每次填满 buffer。完整复制后才 pin 父目录和当前路径，复核已消费 handle 的 native identity/length；不读取输出 `.tmp` 冒充来源。

整个 asset/CDATA 物化成功才保留新 captures；任一失败回滚本次新增 entry、alias 和目录 pin，先前成功 owner 保留。Complete 后立即释放未转交的 spool/hash 资源，晚期预算回退不能把临时文件留到 CDATA 消费之后。取消经真实 CommitFromCache 继续传播，不能被网络重试 catch 转成 InternalError。

重复 `.asset` 读取可使用已封存游标。CDATA 保留原 File.Copy 及其文件属性行为，首次仍可能额外读取封存；时间戳触碰使用经过身份复核的 attribute-only handle，内容写入与路径替换继续拒绝。此组 `completeClosure=false`：其它 BinaryAsset 读取、网络 cache 查询闭包及 Texture 隐藏资源仍未完成；内容 SHA 只证明本代消费内容，不证明旧 cache 的编码语义正确。

| binaryMaterialization 字段 | 实际含义 |
| --- | --- |
| attempts / commits / rollbacks | 物化 scope 的尝试、成功与回滚；成功不保证每个来源都通过封存预算 |
| captureReadsStarted / captureReadsCompleted / captureReadBytes | 额外封存读取的累计工作，含回滚；同流 `.asset` 首遍不再额外读取 |
| teeStarted / teeCompleted / teeBytes | 实际接收到的成功原 copy chunks，累计值包含后续回滚 |
| copyReadHits | 已打开封存游标的次数，不保证后续 copy 成功 |
| cacheHeaderReadHits | 恢复 header 已打开封存游标的次数；不代表另一次完整物化或新 capture |
| 顶层 captures / capturedBytes | 当前保留的 snapshot entries；回滚恢复规模，不能当全部读取量 |

恢复 header 只复用已经封存的 Cache 来源，不为尚未验证的坏缓存新建 pin；坏长度删除、Local/Output 的可变修复路径继续执行原逻辑。网络 cache 存在查询在原调用之后记录，保留原 `&&` 短路，使用独立的 8192 路径/1 Mi UTF-16 字符预算并在 VerifyUnchanged 终验。成功的本进程 File.Copy/File.Delete 推进对应预期状态，已有外部冲突不被清除。实际开始而失败的可选 cache 写可能留下部分目标，因此撤销该路径的查询证明并永久报告本代 incomplete；后续真实查询可重建存在基线，不能因此授权强缓存。内容封存、预算回退、输出根排除及 Texture 未闭合边界不变。新增取消点在可选 cache 发布和 keepalive 路径继续传播，不落入原 best-effort catch。

## 摘要读取与插件所属线程

AsynchronousFileReader拥有排队、已出队待完成和当前Chunk；正常EOF及提前Dispose/构造读取消费者故障均收尾，EndRead最多一次，只有完成已证实的buffer才能归池，不确定BeginRead失败的数组放弃归池。Dispose可等待pending I/O，不承诺取消OS读取；清理错误不替换消费者主异常。保留原同Chunk复用、实际短块、live FileSize、EOF非null但Data=null/BytesRead=0形态。正常Framework EndRead本来关闭内部等待事件，不能称为修复正常路径事件泄漏。

Texture活跃方法在native前检查创建它的Thread对象；live Dispose先检查owner再关闭，关闭后保持ODE优先及重复Dispose无害。Builtin聚合Registry/ReInitialize/活跃Dispose同样先检查owner；错误线程Dispose不能先置终态而使owner无法回收native provider。这不是所有插件可重入或允许并行的证明。

能力声明候选为实际注册路由保存独立immutable标量，不保存plugin、metadata、schema或native引用；未知查询不增长字典。当前消费仅为legacy build-cache否决、session复用拒绝和opaque输入原因。已允许的非Texture类型仍须原whole-root验证，不能把原UseBuildCache=false改为true。ConservativeCoreBoundary不是exact闭包；Texture/未知输入跟踪为Unknown，StrongBundleAllowed/ParallelAllowed始终false，环境仅记录已知最低host并明确不完整。能力声明组完整Actions已success。

后续资源组（随`6db109af`重验）把Chunk空闲池限制为8个1MiB数组，Dispose原子取走数组并最多归还一次；这是idle保留量，不是live buffer/进程RAM上限，也不是开放Core并行。摘要预算/禁用snapshot/暖SHA/诊断回退路径在实际块边界检查当前input scope的token；取消不发布部分SHA或成功计量，同item仍可重试。同步read与pending APM收尾仍可能等待OS，不承诺abort底层读取。

同组Core XML加载拥有外层XIncludingReader与base reader局部引用，失败后退订原DOM事件，保留解析阶段原XmlFormatting/SchemaValidation包装和主异常。Mvp.Close按current→父stack顺序尽量全部关闭、保留首close错误与closed root NameTable；custom resolver获取后未转交的Stream/TextReader/XmlReader及构造/infoset/XPointer失败也有确定所有权，失败撤回自己的parent frame，保留fallback行为。实际资源探针不冒充SDK输出oracle；整组仍须完整四输出/CLI/SDK验收。

## Define最终表达式查找（后续候选）

只在一次ProcessExpressions中对实际DefinitionSet建立有界key→live Definition索引，值/Document继续实时读取；EvaluateDefinitions安装顺序不变。默认16—65536条目，key合计1048576 UTF-16字符，实际读取达到max(32,entryCount/4)才建索引；不是RAM峰值预算。

仅exact DefinitionSet、默认string comparer、标准CultureInfo/CompareInfo可入场。持有原SortedDictionary enumerator作版本哨兵，EOF后也继续MoveNext检查版本；base-cast赋值、删重加同count、Current/table/culture身份变化、重入均永久legacy。建索引前验证当前文化下实际key严格有序，不修复旧文化构建的树所给出的legacy查询结果。scope在成功/异常finally释放，DP只保存标量工作快照；`canAuthorizeReuse=false`，没有冻结Define环境或完整AssetSemanticResult。

52行原DLL同源语料、4文化等价/Unicode键、变更/重绑/重入/错误、预算和真实XML四输出矩阵随`6db109af`重验，强制NodeChanged/NodeInserted回调实际发生。`DisableDefineLookupIndex`进入真实CLI成对fallback；组件计量与完整CLI计量分别报告。

## 构建内 schema 元数据

每次构建持有独立、单 entry 的 MetadataCache，只共享自有的有序继承名和资产依赖计数；保存/恢复均复制，调用方的 StringCollection/IDictionary 仍私有。每流完整读取 schema 闭包、SHA256、Compile 自己的 XmlSchemaSet，并绑定本流 helpers/hashable 类型；不共享 Framework schema 对象或跨流引用描述。

键含实现版本、规范根路径、读取顺序/实际路径/逐文件 SHA、countDependencies、当前及比较文化、StringHashBinDescriptors。只有成功构建后发布；不同根、同大小同时间戳内容变化、模式或文化保守失效。builds/hits 是本 SchemaSet 生命周期计数，不是整构建次数。

## 声明、文档与每流绑定

根文档私有 StreamBindings 保存本次强引用出现顺序、weak 规则和依赖闭包，manifest/排序/资产报告消费当前 root 结果。成功 PrepareOutputInstances 后发布只读标量快照；它只证明绑定准备完成，不证明后续 manifest/link/发布已提交。执行期仍有 legacy live Handle 语义。

声明快照按 reference identity 复制 exact-base Handle 图，直接保留 32-bit ids/hash、重复顺序和跨 own/inherit/strong/weak 的别名。派生 Handle/list 整图回退；null 只保留 in-memory 行为，不新增 null XML roundtrip。文档快照隔离声明 wrapper、Define/include/output/UsedDefine 记录，保留路径、顺序、别名与来源引用；恢复 Define.Document 不写穿其它文档。派生声明/InstanceSet 和派生 metadata 分别保守回退。

只有实际 exact 值复制或 XML 新建提供 positive ownership 时，私有 saved 声明/元数据可共享；Current 恢复仍复制。未知 BinaryFormatter 来源无共享证明；动态启用旧生命周期时先 detach，再暴露给可变 Current 并撤销证明。`CacheFromDocument` 的 saved-to-saved 共享已通过公共 API 合同，但当前无生产调用，不报告 CLI 收益。DOM、SchemaInfo、完整 Define 环境和 AssetSemanticResult 尚未冻结。

## 继承阶段执行

PrevalidationReady 属于 InstanceDeclaration 当前 live DOM 的非序列化阶段状态，不从 legacy hash 推断，也不随 saved 声明恢复。每次 XmlNode 赋值含 null 都撤销它并递增 revision；实际 hash 成功且 revision 未变时才发布，保留 custom WriteTo 的异常及赋值回调。它不证明 DOM 不可变、不代表完整验证成功，也不授权语义缓存。

同文档继承使用显式 DFS frame 与按引用身份的 active visiting 集，空间随当前深度增长，拒绝环且不消耗 CLR 递归栈。查找、source 返回边界、merge/hash、顶层重复处理和错误顺序继续执行；无继承的顶层路径不分配图或 frame。跨文档仍直接消费原 processed DOM 的 SchemaInfo、默认值与来源，没有用 CloneNode/OuterXml 替代完整结果。完整 AssetSemanticResult 与 Define 环境冻结仍未完成。

## 影子语义轨迹

DependencyObservationLog 保留原候选探测、Define 绑定/值/epoch/read-site、同文档 prevalidation 与跨文档 processed 继承来源、精确/派生符号候选及选择/歧义。只存 immutable scalar/string，保留 task/query/parentQuery/sequence；每次查询仍实际执行，`canAuthorizeReuse=false`。

默认 8192 条、1,048,576 UTF-16 字符；不是 RAM/UTF8 字节预算。超限整条丢弃、计数继续，truncated/dropped 明确，不能由旧 XML hash 或部分轨迹放行强缓存。

## 表达式与 XML 哈希

exact 表达式程序由 DP 弱生命周期拥有有界 LRU；第一次仍由原 ANTLR parser 按原序求值，整 expr/AsString 成功才发布不可变语法。命中重做 Value 工厂、运算、当前 Define 读取和文化转换，不缓存值/绑定，不新增短路。默认 128 entries、262144 UTF-16 字符、32768 节点；单表达式 16384 字符/4096 节点/depth128，超限走原 parser。固定 en-US 只读文化共享不改变原环境文化参与的转换。

表达式遍历对 exact Framework DOM 使用 live FirstChild/NextSibling，保留移除/移动当前 sibling 后的原枚举行为；exact 单 XmlText 属性只读 Value 一次。派生/custom DOM 与属性聚合的派生 text/entity 保留 legacy getter/遍历；前序 ordinal、错误顺序和 Define 观测不变。

HashingWriter 保持原生产实现：每个 512 UTF-16 字符块和最后不足块、原拼接/字符串及 ANSI 调用，GetFinalHash 只读状态，可重复调用并继续 Write。本轮缓冲与 ASCII 直接字节哈希尝试因 CPU 收益不稳定已撤回，947 行原 DLL 确定性语料保留为后续回归。

原 ANSI wrapper 的 UTF-16 Length/Marshal 边界保留，不作 Unicode 规范化。多 surrogate pair 引起 ANSI 收缩后读取未初始化 native 字节的旧范围不能作为稳定跨进程 oracle；确定性差分覆盖分隔 unpaired、512 边界拆 pair 和至多单 pair/NUL 边界，不泛称任意 Unicode 原字节验收。

## 局部索引与传输 buffer

| 实现 | 资格与顺序 | 回退/计量边界 |
| --- | --- | --- |
| NodeJoiner child id | 有序重复桶，id 跨 node name/namespace 保留原规则；Name 匹配始终线性 | Prefix setter 未必发事件；相关外部 DOM 变化永久 legacy，成功/异常均退订，保留 512 项热点门槛 |
| NodeJoiner predecessor | 单次插入扫描前建立标量前序名称集；标准节点、至少 8 子项/16 schema 项，4096 入场预算；包括原先“先比 Name 再比 target identity”规则 | 每次插入重新建，DOM 回调可改 schema；不跨插入缓存 schema。DP 报告实际 builds/entries/indexQueries/linearQueries |
| Symbol lookup | 仅同步输出绑定，exact 图/标准 comparer，检查实际 legacy table key 的完整 `(TypeId,InstanceId)` 严格有序；不从可替换的 live value Handle 重建 key | 16–65536 entries、有界引用检查；Trace 回调、派生 schema 查询、ISessionCache、MissingReferences 外部边界后永久 legacy。location/skip/derived 顺序和影子事件保留，scope 不逃逸 |
| StableSort membership | 根私有 bindings、exact 图/标准 schema 字典/文化，以完整 ID 索引已有依赖 Contains | 小于 16 项、1024 集合/65536 依赖/4096 声明预算或 custom/callback 走原路；闭包构造、原 int XOR 碰撞错误、schema 初排及插序不变 |
| Base manifest | 有序重复队列与 Fenwick 查当前秩，仍即时 List2.RemoveAt | 小组/派生/非法/别名列表及 Trace 回调保守回退；不计 List2 搬移，不宣称整排序复杂度下降 |
| 每 DP copy/link scratch | 懒分配并复用 1 MiB copy、64 KiB link 数组 | copy 锁、循环/短块/错误保留；无 owner 或关闭时走懒分配静态 fallback。计量是分配/请求，不是完整内存峰值；未放开并行 |

真实有界补充用例包括四份固定 Upheaval 原 XML 加三份明确 synthetic Include/Define/继承适配器（7 资产，无 Art/Texture/Audio 闭包），以及 96 项 Experience 强依赖链加 2 项 weak Upgrade。保留原字节/闭包预检、冷热/持久会话、默认/回退四输出和独立原 BAB；不能替代完整 Upheaval 或游戏内验收。

## 诊断开关

这些是 EXE.config 的 AppContext 开关，不是 CLI 参数。true 时恢复相应历史算法；`DisableDocumentSnapshots` 和 `DisableStreamBindingResults` 涉及旧可变生命周期，不能把旧共享错误当作应保留的性能行为。

| AppContext 名称（统一前缀 `NewSdk.Compiler.`） | 作用 |
| --- | --- |
| DisableSinglePassDigest | 历史双遍摘要；可能未变输入原本仍保留 SHA-only 检查 |
| DisablePhysicalInputSnapshot | 关闭正内容封存 |
| DisableBinaryMaterializationCapture | 关闭外部 Cache 同流封存与游标复用 |
| DisableSharedSchemaMetadata / DisableSchemaReferenceDescriptors | 每流重建自有元数据 / 每次重算引用描述 |
| DisableDependencyObservations | 关闭有界影子观测 |
| DisableDocumentSnapshots | 旧文档缓存生命周期 |
| DisableDeclarationHandleSnapshots | 旧声明 Handle 共享语义 |
| DisableSharedDocumentDeclarations / DisableSharedDocumentMetadata | 私有 saved 值深复制；仍保留正确的 Current 隔离 |
| DisableStreamBindingResults | 旧跨流可变绑定算法 |
| DisableExpressionPrograms / DisableSharedExpressionCulture | 原 parser / 每 Value 文化对象 |
| DisableExpressionTraversal | 旧 Value 读取和 ChildNodes 遍历 |
| DisableDefineLookupIndex | 最终表达式阶段使用原DefinitionSet查找；不改变Define安装/值读取顺序 |
| DisableNodeJoinerIndex / DisableNodeJoinerPredecessors | id 线性匹配 / schema 前序线性查询 |
| DisableBaseManifestIndex / DisableStableSortMembershipIndex / DisableSymbolLookupIndex | 对应局部查找走原算法 |
| DisableOwnedTransferBuffers | 历史静态传输 buffer |

在临时解压副本的 `<configuration>` 下合并下面设置；已有 runtime 或 AppContextSwitchOverrides 时合并内容，保留其它项，不能重复创建节点。修改后重启进程。

```xml
<runtime>
  <AppContextSwitchOverrides value="NewSdk.Compiler.DisableExpressionTraversal=true;NewSdk.Compiler.DisableSymbolLookupIndex=true" />
</runtime>
```

对比同输入的四输出、原始错误和报告工作量；所有编译、测试与微基准仍只由 Actions 取得。

## 明确剩余的输入与语义边界

parse=text资源组只补GetResource→raw stream→reader→response的确定释放、坏encoding/fallback和主异常保留，仍按原GetResource路由读取，绕过Core FileNameXmlResolver.Paths/PhysicalInputSnapshot。外部file/http text输入的legacy /bc、/sc whole-root覆盖尚未实证；非Texture的LegacyAllowed不是完整闭包证明。后续须真实CLI差分和保守拒绝/跟踪决策，不据此开放强缓存。

完整ASR恢复还有具体门槛：XmlNodeReader→XmlDocument.Load能带入部分PSVI及默认Specified，但新doc的有效性报告状态/Schema环境未完整恢复，XmlSchema对象仍可能别名；saved FromLast也没有DOM阶段和完整Define输入。现有PrevalidatedLoad重新Gather/evaluate/whole Validate及引用/FileReference改写，不能直接以processed DOM绕过。三行原DLL恢复原语用例只解除调查阻塞，生产没有接入半成品ASR，也不授权复用。
