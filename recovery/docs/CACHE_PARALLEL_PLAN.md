# NewSdk 缓存与并行编译：GPT 实施任务书

> **给执行 GPT：请直接按本文修改生产代码、接入真实构建链、完成验证并交付成果。不要再次扩展全局研究或只提交方案、空接口和测试脚手架。**
>
> 原则：**效率优先，正确性是底线；先解决主要重复工作，再加速剩余计算。**
>
> 文档日期：2026-10-28。调研基线：`wu162/NewSdk` 的 `compiler-main`，提交 `e7396d3b6fb302682d3610ebbc6e99ce87eb867f`。执行时以最新远程头和实际源码为准，只补查相关差异。
>
> 本文已包含目标、产品模式、代码入口、实现顺序、验收和交付要求，不需要此前聊天或 PDF。本文描述待实施工作，不表示代码或性能已验证。

## 1. 要完成的任务

仓库：<https://github.com/wu162/NewSdk>。开发基线与最终 PR 目标分支：`compiler-main`。

围绕现有前端、后端和链接器，完成以下主线：

1. **资产级增量缓存。** 改一个 Weapon、AudioFile 或 W3X 时，未变化且已认证的资产继续复用后端结果；普通 XML、GameObject、Weapon、Armor 必须进入缓存覆盖目标。
2. **减少重复 I/O。** 新缓存按需读取和物化结果；把全根指纹与资产复用身份分离，在保留输入保护的前提下减少跨 stream 重复盘点、哈希与整树复制。
3. **跨文档的后端并行。** 同一 stream 中独立、经过认证的资产可并发执行；前端语义、验证、提交和最终链接保持正确、确定。
4. **保留产品合同。** 输出格式、CDATA、引用/schema 检查、取消、诊断协议、发布回滚、现有适用 SDK/发行验收不得因优化而削弱。

资产缓存和并行共用一套执行边界；缓存资格与并行资格分别认证。所有阶段都是验收节点，完成后继续推进依赖满足的工作，不因完成一个小子集就结束整轮。

### 本轮不设为前置条件

不先建设完整持久化语义图、Parse/Prepare 持久化缓存、通用 IR、全局 CAS 去重、复杂 GC、动态内存预测器、多进程 worker、远程缓存或新基准平台。保留现有前端和链接算法；排序、压缩及容器微优化按计量后移。

保留当前 Win32、进程与部署合同；不扩展平台，不重新引入已取消的 AssetMerge/WBData 工作。Texture 未闭合的缓存与并行能力继续走兼容路径，不因此阻塞其他资产。

## 2. 启动步骤与工作方式

按以下顺序开始，随后进入编码：

1. 读 `AGENTS.md`、`recovery/EFFICIENCY.md`、`README.md`、`recovery/REFACTOR_RESUME.md`，核对远程 `compiler-main` 和已有验证来源。交接描述与实际提交不一致时，以源码和有效收据为准。
2. 从核对后的远程头创建成果分支 `feature/cache-parallel`。若已经存在本任务的成果分支，核实后续接；同名分支属于其他工作时使用不冲突的新名称，不覆盖别人工作。成果集中在首次确定的成果分支。
3. 只读第 4 节所列接入点及当前阻塞需要的相关文件，不重做全仓和全网调研。行为未知时用定向探针确认，同时推进不依赖它的部分。
4. 记录现有工具/输入身份，复用已有报告和计数位置补充必要计量；不要先开发独立性能平台，也不要等全量基准齐备才开始改造。
5. 以完整能力组接入生产。核心循环由主集成者维护，存储、后端适配和验证按文件边界、当轮授权及可用并发容量分工；主集成者持续集成。

常规实现选择自行解决。按用户当轮时限或仓库阶段约定推进；保存有意义检查点并继续工作。禁止强推；创建 PR 后不自动合入，合入仍需用户明确批准。[R1]

## 3. 固定产品模式，避免新旧缓存混用

新增应用层参数 `--jobs=N`，默认 `1`，只接受正整数。沿用现有参数解析、项目预设优先级和错误协议，不建立第二套解析器。实际并发可因后端许可与内存预算小于 N；不得按巨大 N 直接预分配线程或队列。

保留 `/bc`、`/sc` 现行默认值：均关闭。首版执行行为固定如下：

| 参数组合 | 必须执行的行为 |
| --- | --- |
| `/sc:false /bc:false`，jobs=1 | 新共用执行器串行；禁用持久动作缓存读写；作为新路径无缓存对照 |
| `/sc:false /bc:false`，jobs>1 | 新执行器并行；禁用持久缓存，用于证明真实后端并发 |
| `/sc:false /bc:true`，jobs≥1 | 新 ActionCache；所有普通资产结果复用来源经过动作身份授权 |
| `/sc:true`，jobs=1 | 保留原 session 行为及其原有 `/bc` 组合路径、全根保护和既有拒绝行为 |
| `/sc:true`，jobs>1 | 参数/预检阶段明确拒绝，诊断 `CORE_SESSION_PARALLEL_UNSUPPORTED`；不偷偷关闭 session 或回退单线程 |
| 未指定缓存和 jobs | 缓存关闭、jobs=1，保持默认功能语义 |

`/sc:true` 的兼容路径不计作新动作缓存或新并行已生效；暂不为完成主线重写 session。一个语义前端可使用不同复用策略，不能复制两套前端长期维护。

同步修改帮助、项目预设、有效选项报告和测试。报告明确执行引擎/缓存模式、requestedJobs、实际后端调用数及最大实际并发；保留已有字段含义，不把 session 复用记成新资产 hit。必要时同步更新协议 schema。[R2]

## 4. 生产代码接入地图

以下路径都相对仓库根目录。行号仅作调研定位，执行时按方法核对。

| 文件/区域 | 要修改的职责 |
| --- | --- |
| `recovery/src/Managed/Core/BinaryAssetBuilder.Core/AssetDeclarationDocument.cs`：ProcessInstances、CompileInstance | 统一结果复用决策，抽取 Prepare / Execute / Commit，命中仍经过 verifier |
| 同目录 `OutputManager.cs`：GetBinaryAsset | 把返回的已有 BinaryAsset 与经过验证的动作来源关联 |
| 同目录 `BinaryAsset.cs`：GetLocation、Commit | 禁止旧 Output/Local/Cache 优先级绕过新身份；正确物化完整结果 |
| 同目录 `DocumentProcessor.cs`：ProcessDocumentContents | 保留前端语义，调整跨文档任务、完成状态、XML 释放与链接的汇合点 |
| `recovery/src/App/Application/CoreStreamCompiler.cs` | 模式路由、任务执行器、缓存/并发上下文与计量 |
| 同目录 `CacheInputSnapshot.cs`、`BuildAssetCache.cs`、`SessionPackage.cs` | 分离动作身份和输入保护；复用事务机制，保持旧 session 隔离 |
| 同目录 `BuildSession.cs` | 接入候选、可回滚安装、成功完成和异常清理 |
| `recovery/src/App/Cli/Options.cs`、`Application/Model.cs`及现有帮助/协议位置 | jobs、有效模式、预设和报告 |
| `recovery/src/Plugin/W3x/NativePlugin.cs`、`recovery/src/Native/W3x/` | W3X 冻结输入、重入与初始化审计、真实并行 |
| `recovery/src/Managed/Audio/`及 XML 各后端 adapter | 完整读取/CDATA/元数据合同，分别认证缓存与并行 |
| `recovery/scripts/`现有缓存、CLI、Core、BIG、SDK 脚本 | 扩展对照和失效用例，复用原比较与运行控制 |

### 必须先处理的已知陷阱

当前 ProcessInstances 在调用 CompileInstance 之前就调用 GetLocation；OutputManager 以旧 FileBase 查找对象；BinaryAsset 选位置时 Output 可优先于新装入的 Memory，Commit 还会再次选择位置。

**新缓存必须接在 ProcessInstances 的结果来源决策处，并覆盖物化与提交。只包装 plugin.ProcessInstance 不算完成。**[R3]

## 5. 能力组 A：完整资产任务与精确增量缓存

### A1. 抽取职责，先以 jobs=1 接入生产

保留现有表达式、继承、schema、路径、引用解析、字符串哈希注册和链接行为。前端每次正常执行，先省掉后端重复计算。

建立以下最小数据合同；类名可沿用现有风格，不要求新建框架或运行时程序集：

| 合同 | 必需内容 |
| --- | --- |
| PreparedAsset | 后端实际消费的独占准备态、type/schema/namespace 信息、引用和局部索引、兼容元数据、有效参数、完整外部读取身份 |
| CompileResult | 三缓冲区、完整 CDATA 集合及存在性、允许的元数据变化、后端额外注册/诊断、ResultDigest |
| ExecutionContext / 任务信封 | TaskId、使用位置、源位置/诊断归属、提交顺序、私有 scratch、取消、计量；与语义摘要分开 |

owner 做 Prepare；Execute 可在 owner 或认证 worker 上执行。owner 先在私有 staging 安装完整结果，并应用允许的元数据变化与必要副作用，再执行现有 verifier，成功后调用 BinaryAsset.Commit。缓存命中与真实执行使用相同顺序；失败不得使候选缓存成为可见成功结果。已有前端注册仍由前端完成，不把整个 Core 重写成事件重放系统。

后端输入使用专属表示，保留真实需要的信息；不能用只有 OuterXml 的 DTO 假装覆盖所有后端。[R4]

### A2. 强 Key 与来源授权

使用版本化、字段边界明确的 SHA-256：

```text
ActionKey = H(
  keyFormat,
  actualBackendAndToolchainIdentity,
  targetAndEffectiveOptions,
  preparedBackendInput,
  compatibilityAndReferenceMetadata,
  completeExternalReadIdentity,
  relevantPathAndProcessIdentity
)
```

执行规则：

1. 对输入与结果合同已认证的类型，在本轮前端处理完成后、选择复用来源之前计算完整 ActionKey；同一 build 的工具链指纹共享计算。未认证类型只记录 ExecutionId，按已认证的执行方式实际调用后端；ExecutionId 不得授权跨执行复用，旧 Output/Local/Cache 同名不能让它跳过执行。不得用全根指纹或不完整 Key 将未认证类型伪装为已认证。
2. 结果来源记录已验证 ActionKey，或仅限本次任务消费的 ExecutionId，以及 ResultDigest 和来源类别。OutputManager 返回已有对象、GetLocation 和 Commit 使用结果前均须满足相应身份合同；私有 staging 中存在同名文件也不自动可信。只允许完整 ActionKey 授权新路径的跨执行复用。
3. TaskId、StreamId、发现序号、worker、计时、临时输出位置和 jobs 不进入语义 Key；后端确实写入产物的路径仍进入 Key。
4. 旧 TypeId/TypeHash/InstanceId/InstanceHash 和 FileBase 保留为游戏格式身份，不作为新强缓存身份。
5. 旧缓存不直接导入新 store。合法外部流/base-patch 是显式输入，保持原语义，不作为普通缓存 hit。
6. 同名但不同动作身份不得未经验证直接复用。允许的覆盖按既有规则显式执行；同一命名空间中须同时保留且内容不同的结果发生旧身份冲突时明确报错。不同 Key 可能产生相同内容，不仅凭 Key 不同就报冲突。
7. 同一强 Key 若被执行得到不同语义结果摘要，按缺失依赖/非确定性处理，不默默替换可信结果。

初版保守覆盖实际 EXE/native/config/schema 及尚不能排除的相关进程输入。ReferencedFiles 只是线索，不是读取闭包完整的证明。不要删除未证明无关的输入，也不要把全根指纹重新塞回每个资产 Key。[R5]

### A3. 保证摘要与消费的输入一致

XML/W3X 对后端实际消费的冻结表示求摘要。Audio 等直接读文件的后端，采用经验证的冻结字节适配，或经 Windows 共享语义验证的受控读取/句柄策略，保证 hash 后不会消费另一个版本。

仅“先 hash，再让 native 任意重开路径”不合格；前后根扫描不能替代这一要求。后端自己做路径查找、缺失探测或 header 读取时，将结果纳入合同。

PathMusic 的路径字符串会进入产物，语义路径与快照存储路径必须分开。未知读取或副作用的类型继续原执行且不缓存，注明具体阻塞。不要为此先虚拟化整个文件系统。[R6]

### A4. 简单 store 与明确事务时序

新增独立版本目录，例如 `.cache/action-cache-v1`，以 ActionKey 对应完整 manifest 和 payload；所有成员校验长度、摘要、路径边界与应有/合法缺失状态。命中只读取和物化所需条目，不恢复整棵历史缓存树。

直接复用/提取现有 pending、owner、替换备份和回滚机制，固定如下行为：

| 状态/时点 | 必须满足 |
| --- | --- |
| 执行与恢复 | 只写本构建私有 staging；新结果写候选目录，本构建可内部使用 |
| CommitCache | 完成可失败的安装与校验；条目保留 pending 排除标记；修复替换保留旧版本用于回滚 |
| 报告、终态事件、必要 sink 完成前 | 其他构建不能将 pending 条目视为成功缓存 |
| 全部必要输出成功后 CompleteCache | 完成可见性处理；沿用现有保守完成语义，不在终态之后新增必须成功的安装步骤 |
| 编译、取消、发布、报告或 sink 失败 | 撤销本次安装并恢复本次替换，不能删除其他成功构建的数据 |
| 崩溃或完成清理失败 | 遗留 pending 继续 miss，可后续清理；不因缓存清理失败伪造构建失败或暴露未确认结果 |

普通损坏条目按同一事务修复，不能提前破坏旧已确认状态。初版采用明确的缓存根写入所有权和锁；缓存条目不与可写发布输出共享可被修改的字节。先实现完整条目清理/容量控制，延后全局 blob 去重、数据库和复杂 GC。[R7]

### A5. 后端覆盖要求

目标是覆盖当前已支持、输入与结果可闭合的生产后端族，不把整个族的审计缩成一个简单白名单样例。

- XML、GameObject、Weapon、Armor：成组接入数值修改复用，不能仅翻转旧 UseBuildCache 标志。
- W3X：覆盖已支持的动画/网格编译路径，保留原格式和三缓冲区合同。
- Audio：连同实际读取、CDATA、header/路径和允许副作用一起接入；PCM/XAS 等主用路径提供真实证据。
- Texture：保留已知兼容边界；未认证不缓存，不阻止同 stream 其他已认证资产命中。

提交实际能力表，逐族/必要时逐类型列出 CanCache、CanRunParallel、读取与输出合同、证据、剩余阻塞。某一子类型通过不能宣称整个后端族完成；未知类型不能成为停下其他已知能力的理由。

**A 组完成条件：**新模式真实进入生产构建；普通 XML 数值、W3X 与 Audio/CDATA 的代表修改通过精确失效和独立无缓存对照；其他已认证资产不调用后端；旧位置复用无法绕过强 Key；损坏与失败事务通过。

## 6. 能力组 B：同组消除主要 I/O 放大

与 A 配合推进，避免另起一套输入系统：

1. 全根指纹不再决定新资产条目的身份；保留其适用输入变化保护及 resolver 边界。
2. 新缓存按需校验、按需恢复；不复制无关历史资产。
3. 共享一个 build 中的工具链身份、输入清单和有稳定字节版本证明的摘要。
4. 合并逐 stream 扫描时单独验证检查时点：输入根与输出根仍隔离，适用读取完成后、发布前检查仍有效。
5. 不用 mtime/size 替代内容身份，不引入 watcher/USN，不把前后扫描称为严格快照。

扫描合并尚有阻塞时先保留原检查时点，继续交付精确资产身份和按需恢复；记录剩余 hash/read/copy 成本及阻塞，不借此重建全语义图。[R8]

只改“未被编译动作读取、仅作为包成员”的 Additional 文件，在相关资产全部已可缓存的样例中应为零后端重编。首版允许前端与链接继续运行；链接缓存完成后才可宣称只重打包。

## 7. 能力组 C：跨文档、有界后端并行

A 的任务合同稳定后即可并行推进 C，不必等所有后端缓存覆盖结束。

### C1. 调整生命周期

沿现有前端合法顺序生成独占 PreparedAsset，使同一 stream 的资产可跨文档进入队列。禁止“每个文档启动任务后立即全等完”作为跨文档并行的完成证明。

在需要物理结果的输出准备、链接和文档完成/保存之前汇合。正确延后 MakeComplete、MakeCacheable、SaveDocumentToCache 及非继承 XML 释放；保留仍被继承/语义消费者使用的源数据。[R4]

不要把普通运行时资产引用一律改成等待目标 payload 的调度依赖。只有已证明后端只消费稳定标识/局部槽位时才允许独立执行；继承、宏和真正内容读取保留实际依赖，引用合法性仍验证。

### C2. 调度与认证

先用全局 jobs、后端/重任务 pool、待执行输入和待提交结果的数量/字节预算实现有界调度，不建设自动学习预测器。

- 首先认证 W3X；复用其现有独占输入/输出边界及已安全的 trace 逻辑，确认 native 共享状态与初始化。
- 按计量扩展 XML、Audio；缓存资格与并行资格分别开启。
- worker 使用显式输入与上下文，不持有共享 BinaryAsset，不依赖 ThreadStatic、Settings 或 Core owner scope 自动传播。
- owner 按兼容顺序验证、提交、生成引用表与链接，不能按完成先后决定输出。
- Texture 等兼容调用留在 owner。若其与在途 worker 的共享状态/副作用尚未排除，先排空相关队列或设互斥屏障；主线程执行不自动等于隔离。
- 取消后停止派发，等待不可中断 native 调用安全返回；不用强制线程终止。
- 内存预算覆盖准备、执行、完成未提交全生命周期。W3X 展开量不能只由源文件大小估算；大资源使用保守 pool/串行策略。[R9]

**C 组完成条件：**同一 stream 至少两个独立、跨文档资产，在关闭缓存或全新 store 下，真实后端调用区间发生重叠；输出与串行一致；乱序完成、异常、取消及队列预算通过。jobs>1、排队数>1 或热缓存通过均不算真实并行证据。

## 8. 验证：使用现有工具，整组取得证据

### 8.1 直接扩展的入口

| 现有入口 | 用途 |
| --- | --- |
| `recovery/scripts/TestCompilerBuildCache.ps1` | 生产 CLI、缓存状态、取消、发布故障和模式矩阵 |
| `recovery/scripts/TestCompilerBuildCachePayloads.ps1` | W3X/Audio、三缓冲区、完整 CDATA、等长等 mtime 修改 |
| `recovery/scripts/TestCompilerSessionCache.ps1` | 原 session/组合合同、define/继承/include、损坏与事务回归 |
| `recovery/scripts/TestCompilerBackendBaseline.ps1` | 独立原版输出对照与来源检查 |
| `recovery/scripts/TestCompilerCoreLifecycle.ps1`、`TestCompilerCli.ps1` | owner、生命周期、CLI/协议与部署合同 |
| `recovery/scripts/TestCompilerBig.ps1`、`TestCompilerSdk.ps1` | 包内容、Additional、完整 mymod/适用 SDK 里程碑 |
| `SelectXmlComponentGroups.py` | 按真实依赖选择 XML 受影响组，保留有效祖先证据 |

优先扩展现有脚本，可增加必要 fixture 和小型辅助方法，不另造一套通用验证框架。

### 8.2 必须通过的验收矩阵

| 用例 | 必须断言的结果 |
| --- | --- |
| 基线等价 | 旧串行无缓存、新 jobs=1、新 jobs>1；四文件、CDATA、BIG 成员内容满足既有严格合同 |
| 精确增量 | 改一个 Weapon、WAV、W3X；实际消费者重编，未变化资产命中，与独立无缓存结果一致 |
| 输入闭包 | 同大小同 mtime、define/继承/XInclude/schema、工具实现、header、路径根顺序变化正确处理 |
| 缺失与覆盖 | 新增高优先级 MOD 资源、引用目标删除/换类型不被旧结果掩盖 |
| 复用绕过 | 故意放置旧 Output/Local/Cache 或旧 session 内容，证明不能绕过新身份；命中仍走 verifier |
| 完整结果 | 缺 CDATA、损坏条目、错误元数据安全 miss/修复，不误报成功命中 |
| 事务 | 编译失败、真实取消、发布/报告/sink 故障和崩溃 pending 均不暴露半成品；重试可成功 |
| 真实并行 | 同 stream 跨文档 cold miss 的实际调用重叠，乱序完成后输出一致 |
| 模式合同 | 第 3 节所有组合及 jobs 非法值；原 session Texture 拒绝等既有行为保留 |
| 资源与部署 | 准备/执行/待提交均有界；不额外启动产品子进程；现有发行与独立运行门槛保留 |

代表用例必须含普通 XML/Weapon/Armor/GameObject、W3X 动画与网格、Audio 主用编码和 CDATA；这是验收覆盖，不能将实现范围缩成只识别这些 fixture ID。

仅对新 ActionCache 模式，将旧“任意修改后全 cold”的统计预期改成精确失效断言。旧 `/sc:true, jobs=1` 路径保留全根保护对应的冷热、失效、组合统计及 Texture 拒绝断言；不能把新缓存预期套到兼容路径。字节比较、拒绝行为和失败回滚要求均不降低。并行证据来自真实后端调用区间，不是队列长度。

### 8.3 构建与 CI 的明确安排

先做廉价预检：脚本语法、fixture/schema、修改 token、等长/mtime 条件及小样例。完整能力组准备好后集中构建、集中收集独立失败、统一修复；公共产物或输入身份损坏时停止依赖组。

使用 `.github/workflows/refactor-core-lifecycle.yml` 的实际构建和验证链；后端变化还须覆盖相应组件回归。**调研基线的 push 分支过滤不自动包含新的实施分支。** 在成果分支显式 `workflow_dispatch`，生产改动使用 `cli_only=false`；或按需要最小调整触发范围。不能只加提交标记就假定已触发。[R10]

生产输入改变必须成批重建并跑完整受影响回归。仅测试/控制器/文档变化且生产闭包、工具、配置和有效证据均未变时才复用二进制；分别记录 binarySourceCommit、testSourceCommit、runId、原始归档及运行文件哈希。修改后的测试仍需新结果。

SDK 验证集中到生产路由/输入/调度正式启用的里程碑，保留完整 mymod；允许按仓库规则精简探针输入，但小样例不能冒称完整 MOD。不得关闭回归、skip-ci、复用不匹配证据或把运行中写成通过。[R1]

## 9. 性能记录与后续推进

在现有报告中记录：端到端与阶段耗时、按族的真实后端调用数、hit/miss 原因、hash/read/copy bytes、峰值 private bytes、最大真实后端并发。

固定工具/输入/机器，区分以下四类：空编译器缓存的冷构建、完全热构建、单资产修改、仅 Additional 修改；记录 OS 文件缓存条件。复用相同计量口径，必要时少量重复，不用一个热缓存数字代表冷编译加速。

A/B/C 是默认主线，基于证据调整次序：

- 局部修改仍重做大量后端：补齐动作缓存覆盖或修复过宽 Key。
- 冷构建后端占主导：提前或扩展认证后端并行。
- 热构建扫描/恢复占主导：优先去重读取和按需物化。
- 前端占主导：再做解析/语义准备复用及依赖证明。
- 链接/封装占主导：再做完整 stream、后处理、BIG/SKUDEF 缓存。

已知且依赖满足的下一项继续实施，不把计量变成无限调查；没有可比数据时如实记录，不写加速倍数。

## 10. 阻塞、保存与最终交付

遇到阻塞按以下规则处理：

- 某后端输入或副作用不闭合：明确保留原路径，记录具体类型、问题和解除条件，继续其他已知能力；不能泛称整个族已完成。
- native 重入未证明：保留串行，继续缓存和其他可并行后端；不能用一把锁冒称该后端已并行。
- 本地缺 Windows/SDK：继续源码与廉价检查，使用已授权 Windows CI；无法取得证据时保存成果、提交 draft PR，标明待验收。
- 测试失败：定位真实失败并修复，保留失败终态；不得删断言或反复盲重试换通过。
- 到用户时限或需中断：按仓库规则保存、普通推送和交接；不把未验收结果写成完成。下次从同一成果分支继续。

有意义进展及时提交；长任务按仓库约定检查未推送成果。临时分支/代理成果及时汇回确定的成果分支，禁止强推。

最终应交付：

1. 接入生产路径的源码、参数/协议更新和必要测试。
2. 一张真实能力矩阵：类型/模式、CanCache、CanRunParallel、输入/结果合同、证据与剩余阻塞。
3. 冷/热/单修改/封装修改的可比计量，或明确未能测量的原因。
4. 实际 CI/SDK 运行链接及 sourceCommit/runId/产物身份；区分新验收与复用证据。
5. 更新 `recovery/COMPILER_USAGE.md`、`recovery/REFACTOR_RESUME.md`；新建或复用一份简短缓存并行验证记录，写清当前分支、最后通过范围、阻塞及下一步。
6. 面向 `compiler-main` 的 PR：说明问题、改动、实际行为和验证；待验收用 draft。**未经用户明确批准不合入。**

交接只报告新增可用能力、验证与性能证据、尚未解决的真实阻塞。不要用接口数量、提交数量、文档长度或单个小样例代替完成情况。

## 附：已核实的设计依据与定位

以下链接固定到调研基线，供定点查证；执行时如远程已变化，只补查相关差异，不再次展开全面研究。

- [R1] 仓库效率规则：成组实现、证据复用、SDK 降频与保存要求。
- [R2] CLI 参数/预设实现；公开模式沿用现有解析与报告框架。
- [R3] ProcessInstances；[OutputManager](https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Managed/Core/BinaryAssetBuilder.Core/OutputManager.cs#L177-L207) 与 [BinaryAsset](https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Managed/Core/BinaryAssetBuilder.Core/BinaryAsset.cs#L220-L419) 的复用入口：强 Key 必须控制完整来源选择。
- [R4] DocumentProcessor 的前端处理、链接与完成状态。
- [R5] 验证后的实例/依赖处理；当前缓存类型注册见 `recovery/src/App/Application/BuiltinCompilers.cs`。
- [R6] Audio 的实际读取、CDATA、header 与路径序列化。
- [R7] BuildAssetCache 的 pending/owner/回滚及 [BuildSession 事务](https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/App/Application/BuildSession.cs#L140-L201)。
- [R8] 每 stream 命名空间及 [CacheInputSnapshot 的输入盘点和根边界](https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/App/Application/CacheInputSnapshot.cs#L26-L85)。
- [R9] W3X 编译接口、trace 及 [AnimationPipeline 的帧数据展开](https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Native/W3x/AnimationPipeline.cpp#L56-L73)。
- [R10] Windows 工作流的触发过滤、完整构建和测试链。
- [M1] Upheaval 基础流与资产组织；[M2] 单个 AudioFile.xml 的大量独立声明；[M3] 网上 UpheavalEx 项目流。已有静态研究支持资产粒度与跨文档调度，文件/声明数量不能换算成耗时。
- [W1] ccache：预处理后复用结果；[W2] Bazel：动作输入/完整结果与输入变化；[W3] Ninja：有限并发 pools；[W4] LLVM ThinLTO：协调、并行后端、最终链接与增量缓存。仅借鉴机制，不引入这些工具作为依赖或沿用其加速比例。

[R1]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/EFFICIENCY.md
[R2]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/App/Cli/Options.cs
[R3]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Managed/Core/BinaryAssetBuilder.Core/AssetDeclarationDocument.cs#L1030-L1115
[R4]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Managed/Core/BinaryAssetBuilder.Core/DocumentProcessor.cs#L522-L579
[R5]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Managed/Core/BinaryAssetBuilder.Core/AssetDeclarationDocument.cs#L1194-L1251
[R6]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Managed/Audio/Plugin.cs#L71-L170
[R7]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/App/Application/BuildAssetCache.cs#L106-L189
[R8]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/App/Application/CoreStreamCompiler.cs#L436-L530
[R9]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/recovery/src/Plugin/W3x/NativePlugin.cs#L20-L56
[R10]: https://github.com/wu162/NewSdk/blob/e7396d3b6fb302682d3610ebbc6e99ce87eb867f/.github/workflows/refactor-core-lifecycle.yml
[M1]: https://github.com/wu162/NewSdk/tree/bacfc69fc3e71c8116d1f22b933b8b6173170ecf/SDK/Mods/Upheaval
[M2]: https://github.com/wu162/NewSdk/blob/bacfc69fc3e71c8116d1f22b933b8b6173170ecf/SDK/Mods/Upheaval/Data/Mod/Units/Giga-Fortress/AudioFile.xml#L3-L177
[M3]: https://github.com/xan105/RA3-Mod-UpheavalEx/blob/8711c6111c406c78d1a0d3b055384888b9f08782/Upheaval/mod.babproj#L3-L18
[W1]: https://ccache.dev/manual/4.14.html#_how_ccache_works
[W2]: https://bazel.build/remote/caching
[W3]: https://ninja-build.org/manual.html#ref_pool
[W4]: https://clang.llvm.org/docs/ThinLTO.html

