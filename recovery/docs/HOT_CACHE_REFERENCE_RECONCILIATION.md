# 热缓存参考文档与当前源码对照

参考资料为用户上传的 `NewSdk_hot_cache_session_summary_2026-10-30.md`，其调查固定提交是 `5c1843aea5f9b0cdcbe04325b251a8a944ed0fae`。上传参考文件 SHA256 为 `a29ceb7751aeae6b8175ace5ffc58d283ce3693e61c392e583476ef730650055`。它用于提供机制和真实编辑方式，历史计划不替代本轮任务，也不把定性发生频率换算成百分比。当前对照以本轮第二组 `d3b3150f` 为基础，随后组合了新FLA基线 `074bf768`（组合 `e2dba3c3`）；实现和验收状态见[性能说明](COMPILER_COLD_HOT_OPTIMIZATION.md)。

## 仍然成立的核心判断

**整份声明文档前端重处理，不等于文档内所有资产调用编译插件。** 当前 [DocumentProcessor.ProcessDocumentContents](../src/Managed/Core/BinaryAssetBuilder.Core/DocumentProcessor.cs) 对 Loaded 文档仍执行表达式、继承、XSD、实例哈希和引用处理；[AssetDeclarationDocument.ProcessInstances](../src/Managed/Core/BinaryAssetBuilder.Core/AssetDeclarationDocument.cs) 随后按每个资产的可用位置决定是否编译。本轮优化降低这些必要前端工作的重复成本，未将“后端缓存命中”解释为可以跳过 XSD 或继承检查。

| 参考文档判断 | 当前源码核对 | 本轮关联 |
| --- | --- | --- |
| 局部 XML、外部素材、缺失资产可让整份文档 Load | `Reinitialize` 仍依次检查文档哈希、依赖文件、输出可用性、TypeHash、ProcessingHash | 哈希固定缓冲和 Schema 分类缓存同时帮助冷态和这种热后重处理 |
| `instance` Include 把整个父文件登记为依赖 | `GatherUnvalidatedIncludes` 仍将其逻辑路径加入 `DependentFiles`；`UpdateDocumentHashes` 仍写入解析后路径哈希与文件哈希 | 父无关兄弟变化仍可能扩大子文档前端范围；本轮未直接删除这项依赖 |
| 为取得父 XML 可触发 `PrevalidatedLoad` | `ReloadIfRequired` 对未加载的已完成文档仍走此路径；随后仍可能执行表达式、继承、XSD 和实例哈希；`ProcessInstances` 对 `ReloadedForInheritance` 返回 | 第二组同时降低其两次 XML 哈希/引用分类成本，未声称只读取一个父节点 |
| Define 变化按实际使用的值传播 | `ExpressionEvaluator.LookupDefine` 只记录实际查到的跨文档 Define 名/值；`ValidateCachedDefines` 逐个比较当前求值 | 已有精确到文档使用集合的机制，不应以“全局 Define 文件任何变化”强制全图失效 |
| 仅 SessionCache 元数据丢失可大量解析、零后端编译 | 原中间 `.asset` / 原生资产缓存仍可命中 | 单独测量元数据重建，不能把它当作完整无缓存冷编译 |
| 最终发布文件缺失不必重解析/编译 | 链接、发布恢复与 BIG 正常打包仍是独立路径 | 按 XML/后端/输出阶段分别解释，BIG/SKUDEF 包缓存不在本轮恢复范围 |

## 已过时或应收窄的说法

参考文档第4.1节把应用层“运行文件身份、SDK 配置、部分选项”等整份包身份列为会话失效条件。这不再准确描述当前入口：当前 [LegacyCacheStore.CanLoad](../src/App/Application/LegacyCacheStore.cs) 只检查原 XML/Deflate 会话是否存在；此前 `session.identity`、全文件 SHA256 收据和重复输入边界防御已被删除。实际 Schema 内容身份、会话格式/处理器版本、类型/插件版本及文件内容身份仍由 Core 分层检查。不能据旧包身份流程重新引入热路径全树哈希。同期FLA能力另有UI来源目录及编译器版本依赖（uidir/uiv），属于实际UI输入层，不等于恢复旧全运行文件身份封包。

参考文档第5.2节提到每个顶层流新建 SessionCache。本轮 [CoreStreamCompiler](../src/App/Application/CoreStreamCompiler.cs) 已在成功保存后复用请求内 `savedSessionCache`。但它仍为每流创建 DocumentProcessor，并调用 `InitializeCache` 重置文档、实例、定义和 Include 的瞬态状态。因此跨流 DOM 保留尚未实现；“复用会话元数据”不能写成“任意父 XML 已跨流保留”。冻结、失败和部分保存也不授权这条内存复用路径。

公开 MOD 的175/234个 AudioFile、106个 APT 声明等组织方式仍是合理场景来源，但不是当前实际构建入口、修改频率或耗时倍数的证明。必须核对计划选中的流、可达文档和真实后端类型。

## 按收益依据排序的下一步

### 1. 先降低被迫重处理时的共同成本：本轮已实施

第一轮候选完整 MOD 的实例校验自耗时约7.45秒、实际XSD约5.54秒、解析约2.82秒，显示后端只编译少量资产时仍可能有明显前端成本。第二组的固定512字符哈希缓冲、Schema事实分类缓存，以及删除只写的 FileConfigs，服务于文档所描述的热后编辑场景；这些阶段耗时不是可直接全部消除的额度。新实现仍以新的 Windows 原字节/缓存门槛和增量配对结果为准。

### 2. 区分 instance 父文件的“路径身份”与“实际继承内容”：需完整依赖合同

当前 `DependentFiles` 是没有来源类别的字符串列表：同一路径可以来自 instance Include、XInclude 或文件引用。直接删除 instance 父路径的哈希会同时失去路径遮蔽、移动及其他来源依赖保护。父文件改动后仍须处理父文档，保留其非法XSD、重复Define、循环/缺失引用的拒绝行为。

可研究将 instance 依赖的内容判定延后到 `ProcessIncludedDocuments` 完成父文档处理后，再以实际使用父实例的身份/展开哈希判定子文档是否需要重载；父物理路径身份仍应保留。必须覆盖父自身的 Define、XInclude、继承链、外部文件绑定及搜索根变化，不能仅比较父声明的原始文本或 `PrevalidationXmlHash`。

旧 `LastState` 没有记录每条文件依赖的来源和被用父实例闭包。稳健方案需要可回退的元数据扩展/迁移，或证明能从旧记录无歧义恢复来源；这不是仅删一条列表元素的优化。应先用“父有效字段改”和“父无关兄弟改”配对，分别记录重解析文档、继承重载、编译实例与阶段耗时，再决定是否承担格式/互操作成本。本轮未修改此判定。

### 3. 为继承保留窄的只读 DOM 快照：优先请求内，需证明复用身份

当前已完成的父文档可能有元数据但缺少这次需要的 XML。可研究只对真正被请求的 inheritable 实例保留读取视图，减少再次取得父 DOM 时重做整文档前端。候选快照必须与本次 Schema、父文件/依赖、实际 Define 值、配置和已完成状态一致；取消/失败不得发布快照。

不能直接保留上一流的 `InstanceDeclaration._Current`：其中含引用索引、CDATA路径和所属文档。也不能假定 `CloneNode` 自动保留原PSVI/默认属性与类型信息；继承使用的是处理后的节点，缓存应保持相同阶段。请求内方案可避免旧XML缓存格式变化；跨进程持久化则增加原BAB格式和版本迁移成本。需先量化 `cache.reload.inheritanceTarget` 的实际次数和父文档前端时间。

### 4. Define 保持文档精确使用集合，进一步下沉到实例需新证据

当前机制已经避免对从未使用的全局Define重载文档；不应先引入全图 Define 指纹。进一步优化可研究每实例的外部Define读集合与依赖定义的求值闭包，让同文档未受影响兄弟避免部分前端工作。但表达式、继承、XSD默认值、引用重写及整个文档的唯一性约束相互关联，不能在二进制命中后直接复用任意旧节点。

`InplaceLoad` 本身不创建全新 CurrentState，而是重新读取/收集声明；旧 `UsedDefines` 会随新查找更新。是否存在“不再使用的旧项导致额外失效”需要构造多代表达式/继承/Define变化验证，当前不能仅凭字典保留就声称一定有可复现回归。若要清理，必须在完整重处理成功时提交新读集合，失败时不能丢掉旧缓存的有效依赖。

## 验证应回答的问题

增量性能至少区分：大文档单个实例编辑、父实际继承字段编辑、instance父无关兄弟编辑、实际使用/未使用Define编辑，以及只丢Session元数据。各代比较同机基线/候选发布原字节，修改后重新变热应零解析/零编译；保留等长同mtime修改和缺失/删除路径检查。

优先看 `xmlParse`、`cache.invalidation.*`、`cache.reload.inheritanceTarget`、`dependency.*` 与 `core.xml.*`，再看 `instanceCompile` / `core.compiler.<Type>`。短合成场景说明机制和定向成本，完整 MOD 的吞吐另报；两者不互相冒充。文档未测量场景保持未测状态，不用单一热态中位数推断所有日常编辑收益。
