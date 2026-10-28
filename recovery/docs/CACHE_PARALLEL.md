# 资产动作缓存与跨文档后端并行

更新：2026-10-28（北京时间）。成果分支 `feature/cache-parallel`，基线 `compiler-main@e7396d3b`。本页只记录当前工作；旧发行证据见 [COMPILER_RELEASE](../COMPILER_RELEASE.md)。**生产主线与完整 Windows 回归已有通过收据；最新批次和精确范围见下文。未据此声明整份方案全部覆盖或完整 MOD 加速。**

用户提供的[原始实施方案](CACHE_PARALLEL_PLAN.md)保留作目标与验收参考；当前状态只看本页收据，不能把方案要求当作已完成能力。日常续接无需重读整份方案。

## 最新已核验阶段与当前批量

`c807fd1bd13f902bd659d2112dfd3c91fcf66c71` / [40674077574](https://github.com/wu162/NewSdk/actions/runs/40674077574)已完整success；原始归档12810219503、SHA256 `c2a3f2bca01fd804c34a37e34e5822725b9fb36b1512ab3685f920bddaa6ec42`、12组收据及两配置运行文件/独立五文件ZIP已核验。该阶段是24类资格/19个公开根及拒绝原因/哈希开销改进，**不能冒充后续288路由批量结果**。双Mesh4096/4顶点在两配置均为两独立worker且区间重叠、max2、全部串并行/冷热单改原字节相等；九族19个新公开根均实际worker，观察到的与W3X重叠子集逐族保留。11种可构造拒绝原因、部分租约释放/零预算owner、完整mymod/原BAB/BIG/旧session/独立ZIP通过。Debug ZIP `3bcd6cd33a8cb11857713c28756f6feeba9391e030277ec737ebe01142f3275b`，Release `2afad7b0f26e5e3a8df591efa20a1385758511fd5611136bd930514d0181b69f`。

全模块闭合表生产源码 `ac22f7770c7172d1dcb00699387b32bf12be46b7` 正在 [40676722714](https://github.com/wu162/NewSdk/actions/runs/40676722714) 集中验收，当前Core四平台批量矩阵已通过，完整验收仍须等待终态。原发行不随分支替换。证据见JSON的 `preparedMetadataAndInventoryStage` 与后续新收据。

## 模块批量开放（2026-10-28，最新批次验证待执行）

用户再次明确指出逐类型开放太慢，因此改为完整模块审计+一套通用矩阵，不再为每个类型复制测试类。当前内建 XML 路由共289条：已对63个模块的外读、共享写/诊断、初始化表与数字属性驱动分配进行成组审计，完整依据和逐模块/逐类型清单见 [XML_BACKEND_REENTRY.json](XML_BACKEND_REENTRY.json)。其中284条纯元数据由闭合的编译期模块表认证，APT四条继续已有闭合读取资格；只有TestTexture因真实外读及文件长度驱动分配保持owner。TestTextureCollection是纯引用数组，单独按实际读合同认证，不按族名推断。

生产 `XmlBackendCertification` 不枚举live routes后自动开放，新/未知类型默认owner。沿用独占DOM/namespace/引用、原始三缓冲、机器预算及取消汇合；XML里的runtime容量只写标量，审计未发现NumFrames式额外编译展开。Audio插件的重采样/PathMusic与Texture限制不改变。Xmlw3d的Hierarchy/Container/Runtime/CollisionBox也属于纯元数据，生产报告新增显式`backendOwnerThreadId`，不再拿Hierarchy的人为串行作worker判断。

新增一个共享的批量组件矩阵：全部284条已审计元数据路由×四平台含99，私有最小DOM先执行有界worker波次，再与owner的完整三缓冲或同型同文拒绝结果对照，原owner DOM/namespace突变不影响冻结输入；最小输入被拒绝的条目明确记录，不冒充复杂有效输入验收或SDK全局可达。原有复杂原DLL差分证据、真实19公开根/原始流、Mesh/Audio/APT实际重叠、取消/异常及完整SDK/发行门槛全部保留。这批改变生产并行资格，集中新建并验收；尚未在CI运行的部分不得标完成。

## 当前实现检查点（2026-10-28，验证待执行）

本轮准备态拒绝诊断保留旧 `actionMissReasons` 与 `cacheBypassReasons` 语义，新增独立 `preparationRejectedReasons`，仅记录准备器实际返回false的首个门禁；缓存关闭时也记录，不把成功准备后的worker额度不足或排队算为拒绝。13种源码门禁，Windows夹具可构造11种，保留部分外读失败释放租约、成功重试清空原因与零准备预算owner输出对照；不捏造罕见header重开失败实测。

完整 Xmltargeting 五类、Xmlvisibility 两根、Xmlradar 两根已加入源码并行资格，四平台含99的准备态三缓冲/冻结对照与生产八根worker、冷热、等长mtime单改已接入本批。TargetingBaseCompare没有SDK全局元素，只声明组件资格，不修改Schema扩大范围。新增独立双Mesh（修正为4096与4顶点）的串并行真实区间、冷/热与单改原字节对照，保持原180秒门槛和机器资源准入，不靠sleep/派发钩子证明重叠。

Radar生产保留完整Texture/PackedTextureImage强引用及ObjectFilter弱名闭包，热缓存预期仍有1次Texture owner调用；Visibility保留flags/时间/inline filter；Targeting单改交换已有两引用，避免修改依赖资产。新增9类源码资格、8个SDK公开根，准备态组件覆盖完整9类。

首次本轮运行 `b467d708` / [40670721531](https://github.com/wu162/NewSdk/actions/runs/40670721531) 保留failure原终态：唯一失败是新增双Mesh夹具。原始归档12809127829、SHA256 `e69979291b815d2d44c734774dd9b00834331ecf5a9b713fb09de59496ac069f`已核；Debug actual report中requested/effective/minimum jobs均2、总预算536838144/单任务268419072不变，8192顶点Mesh线程1(owner)、4顶点Mesh线程7(worker)、2调用/max1，未触发准备拒绝。该夹具经保守PreparedBytes估算超过单任务worker门槛，兼容owner行为正确；修为4096/4顶点，保留180秒/双worker区间重叠/串并行及冷热单改原字节断言，不改生产限额。原应用/Core/原因组/三族准备态与真实worker、原BAB、Texture、CLI、普通缓存、input roots及旧session/BIG独立组通过；未执行的完整mymod与独立ZIP不冒称通过。

测试修复不改变c7生产闭包；但失败批次缺完整mymod/独立final-release收据，现成功归档重放器拒绝失败源且不覆盖Core/SDK/发行完整重放，故这次集中重建一次并重新完成完整门槛，未强行复用不完整证据。后续不得再为零散测试触发全构建。

首批9类生产/诊断闭包 `b467d708` 正在[40670721531](https://github.com/wu162/NewSdk/actions/runs/40670721531)集中验收。后续同一批已准备 Conditions4类（公开1根）、Objectsets3类（公开2根）、Renderdevice2根、UIimages3根、Healthbars2根、Unitoverlay1根；合计本轮24类源码资格、19个SDK公开根。新增族保持真实强引用与非空filter/string/relocation/import覆盖，未把嵌套类型充作公开资产。

本轮另对次要小任务做两项有界实现：仅当当前open句柄的长度无法进入已满保留集且path没有既有entry时跳过FileIdInfo查询，仍持拒写句柄完整鲜读SHA；所有既有entry仍强制128位ID核对。单Capture内独占复用SHA256对象，不跨线程或Capture共享。新增实际API尝试/鲜读但免身份查询的 `inputIdentityQueries/inputIdentitySkippedFiles`，App cap1矩阵保留同长mtime修改、最大文件替换、释放与fresh指纹/字节账本对照。尚无新端到端计量，不宣称小MOD问题已解决。

用户进一步要求减少逐类型细拆和大型MOD测试频率：本批收束后不再追加小XML组，后续按共享后端/状态合同成批认证与复用夹具；专用测试分支只手动低频测量完整历史Upheaval，先廉价预检、严格复用已验收运行文件，不把未运行计量当收益。

本批因生产闭包改变，统一新建Debug/Release并保留完整Core、真实SDK/原BAB、缓存/旧session、完整mymod、BIG与五文件ZIP门槛。此检查点**未完成Actions验收**；下节收据只适用于各自历史生产闭包。按EFFICIENCY继续成组实现与集中验证；不替换默认发行，不合并PR。

## 本轮验收（2026-10-28）

`6ed2cda8` / [40656205838](https://github.com/wu162/NewSdk/actions/runs/40656205838) 首批完整验收已success，原始归档、12组收据及两配置五文件ZIP已核验：

- `CacheInputLeaseSet` 在构建内持续持最多512个工具/Schema/runtime文件的只读拒写拒删句柄；每次盘点仍重新打开当前路径，用 Win32 `FileIdInfo` 的64位volume与128位文件ID比对后才复用 SHA256。保留较大文件；不支持可靠身份或超上限时重新哈希。每stream首尾完整枚举不变，MOD/Additional/普通SDK资产仍逐次重哈希，session路径保持旧行为。工具输入在租约存续期不可改写，Core销毁释放所有租约；不等同于完整目录快照或硬内存保证。
- 新增完整 Xmlenvironment 十二根的源码重入资格：Weather、WaterTransparency、WaterSetting、Environment、LightingSettings、WaterTextureList、TerrainParticleMapping、BounceKickTerrainMapping、ShadowMap、OrdersManagerSettings、TerrainVisualSettings、CameraSettings。独占桥接树、局部Builder/Tracker、InitOnce只读表、无外部读取；worker仍须通过原机器准入。准备四平台三缓冲区差分及生产冷/热/单改/串并行原字节对照；实际重叠只按收据记录观察到的根。
- 新报告 `inputInventoryCaptures`、`inputHashReusedBytes`、`inputHashLeaseFiles`；`hashReadBytes`继续只计实际读取。读量减少不等于已证明完整MOD耗时倍率。

第二组已验收 `actionMissReasons` 与 `cacheBypassReasons`：前者只记录真实认证查询最终未命中的原因，之和严格等于既有 `actionMisses/misses`，不统计Stage/Commit/Prune内部完整性读取；后者互斥记录 `cache-disabled`、`not-prepared`、`base-patch-source`，不扩大旧miss总数。查询理由为 `entry-not-found`、`entry-invalidated`、`entry-pending`、`invalid-manifest`、`invalid-payload`、`entry-unavailable`、`invalid-result-contract`。`not-prepared`不猜测预算/插件/外读等具体拒绝原因；报告也不把键缺失猜成源码/schema/工具的某种变化。低层与生产CLI已准备损坏/pending/修复后热命中的精确计数及原始字节对照。第二组生产闭包已统一新建验收，未用首批二进制冒充。`8f0d228a`仅增强重放拒绝合同和逐根worker线程证据，未改生产源码；其guards [40656585109](https://github.com/wu162/NewSdk/actions/runs/40656585109) success，新增逐根worker断言已在810完整批次通过。

第三组 `129d9639` / [40660066519](https://github.com/wu162/NewSdk/actions/runs/40660066519) 已完整success并核验原始产物，继续减少SDK重复哈希：有界租约资格扩至稳定SDK文件，排除当前MOD完整子树（包括MOD嵌于SDK/Art等任意路径）和SDK/Additional；所有目录仍首尾完整枚举，MOD与Additional继续按字节重新哈希，工具身份范围不扩大。最多512句柄及当前路径128位身份复核/最大文件优先/失败取消释放合同不变。新增真实SDK/Art Texture原生只读兼容、原等长mtime像素失效与原BAB字节对照，以及1MiB未消费SageXml文件的四次盘点完整账本和缓存命中证明；App补嵌套MOD、Additional可写、释放后修改与容量退化。该组改变生产闭包，已新建Debug/Release并完成完整验收，未复用810二进制冒充。1MiB SageXml文件在四次盘点的总账本精确增加4194304字节，复用增加3143382字节；持租约SDKTexture保持8hit/0miss/1次owner后端，像素等长mtime变化与原BAB原字节对照通过。

第二组 `810dd2ad` / [40657123355](https://github.com/wu162/NewSdk/actions/runs/40657123355) 已整体success，原始归档 `12803193116`，API SHA256 `eeaabe66deb0c2e50fcd1650e7fc355bf901b62da189f2c67cf5f50c2c8bd32f`，本地原始归档SHA、12组收据、运行文件与两配置ZIP已核验。其完整payload+可选真实MOD计量重放 [40659913327](https://github.com/wu162/NewSdk/actions/runs/40659913327) 已success，binarySource=810dd2ad、testSource=25dfd24d；原包12802884504/SHA256 `dc349be5072d68bd6ec92e78c378b3fc71743d684c3cc24c2dbf27e9e3463148`已核验，不能据此宣称第三组SDK扩展性能。

## 先查什么

| 问题 | 最短入口 |
| --- | --- |
| 使用、参数和项目优先级 | [使用说明第6节](../COMPILER_USAGE.md#6-缓存与资源根)、`src/App/Cli/Options.cs` |
| 模式路由、保护扫描、缓存终态 | `src/App/Application/CoreStreamCompiler.cs` |
| CPU、Win32 地址空间与动态内存准入 | `src/App/Application/BackendResourcePolicy.cs`、`AssetTaskExecution.cs` |
| 准备态、读取闭包和资格 | `src/App/Application/PreparedBackend.cs` |
| 排队、真实调用区间、结果与有序提交 | `src/App/Application/AssetTaskExecution.cs` |
| 磁盘完整性、pending、修复与回滚 | `src/App/Application/ActionAssetCache.cs` |
| 旧位置绕过与生命周期 | `src/Managed/Core/BinaryAssetBuilder.Core/{AssetDeclarationDocument,BinaryAsset,OutputManager,DocumentProcessor}.cs` |
| 逐资产失效、CDATA 与真实并行证据 | `scripts/TestCompilerBuildCache.ps1`、`scripts/TestCompilerBuildCachePayloads.ps1` |
| 输入冻结/Win32读租约证明 | `tests/CompilerPreparedBackend.cs`、`scripts/TestCompilerCoreLifecycle.ps1` |
| 总集成和部署门槛 | [Core lifecycle workflow](../../.github/workflows/refactor-core-lifecycle.yml) |

表中的源码路径相对 `recovery/`。定点使用 `rg -n 'TryPrepare|WriteIdentity|Enqueue|CommitOne|AuthorizeExecutionResult' recovery/src`；不要重新加载历史状态全集。续接从本页末尾运行收据与真实失败开始。

## 产品模式

| 参数 | 实际路径 |
| --- | --- |
| sc=false, bc=false, jobs=1 | AssetTasks.v1 串行，无持久动作缓存 |
| sc=false, bc=false, jobs>1 | 同一执行器，仅认证后端可并发，无持久动作缓存 |
| sc=false, bc=true | `.cache/action-cache-v1`，强 Key 按需恢复完整结果，命中仍过 verifier |
| sc=true, jobs=1 | 原 SessionPackage 及其 bc 组合，全根保护和拒绝行为保留 |
| sc=true, jobs>1 | 参数/预检报 `CORE_SESSION_PARALLEL_UNSUPPORTED` |

默认 sc/bc=false、jobs=1。jobs 只接受正整数，巨大值不会直接创建线程或等量队列；`MachineResources.v1` 根据用户上限、CPU 和当前 Win32 进程可用内存决定准入容量，超预算任务转 owner 串行。缓存资格、后端重入认证和当前资源准入分别判断；未认证后端执行前排空在途队列。本轮 XML/Audio 及机器资源策略已通过下方最新完整验收；历史收据保留各自范围。

## 源码资格表（实测范围由下节收据决定）

| 族/类型 | CanCache | CanRunParallel | 实际合同与边界 |
| --- | --- | --- | --- |
| GameObject、WeaponTemplate、ArmorTemplate | 是 | 否 | 私有有效XML、类型/schema/引用强身份及完整三缓冲；非本次XML模块表族仍保留owner |
| 全部内建XML元数据闭合集 | 是 | 是（源码资格；批量运行见最新收据） | 284条/62个metadata族；非全局类型保持组件范围。APT四条另有闭合读取；TestTexture继续owner；新未知TypeId不自动加入 |
| AptAptData/AptConstData/AptDatData/AptGeometryData | 条件成立时 | 是 | 对全部真实外部读取持 Win32 FileShare.Read 租约并 SHA256；native 窄路径暂限 ASCII。私有 bridge 树和 Relo::Tracker；缺失/不可读/非 ASCII 回兼容实际执行 |
| AudioEventLimitGroup/AudioLOD/AudioSettings/Video/VideoEventList/MicrophoneSettings/BootupDisplaySequence | 是 | 是 | Xmlaudio 的七根共用局部 Builder/Tracker 和只读字段/枚举表；不读取外部文件，不注册全局字符串哈希；可选 XML trace 已有锁 |
| TestTexture | 条件成立时 | 否 | 全部真实外读持 Win32 租约并 SHA256；未扩展其并行资格 |
| Weather/WaterTransparency/WaterSetting/Environment/LightingSettings/WaterTextureList/TerrainParticleMapping/BounceKickTerrainMapping/ShadowMap/OrdersManagerSettings/TerrainVisualSettings/CameraSettings | 是 | 是 | Xmlenvironment完整十二根；独占树及局部Builder/Tracker、只读布局/枚举、无外部读取。四平台含99的三缓冲区已对照；十二根均真实worker执行，实际与W3X重叠子集仅Weather |
| W3DAnimation/W3DMesh | 是 | 是 | 冻结 XML/命名空间，native scratch/output 独占，初始化与 trace 已源码审计；旧固定预算下真实重叠已在双动画用例验证，MachineResources.v1完整生产矩阵已重验；Mesh 保持源码重入认证，未单独宣称双 Mesh 重叠实测 |
| AudioFile 默认/NONE/XAS | 条件成立时 | 无 PCSampleRate 时 | 必须为内建精确 PcmWaveAudioCompiler；存在且可读的真实输入持续持租约，编码 scratch 和 CDATA 独占。注入其他 IAudioFileCompiler 不认证缓存或并行；有 PCSampleRate 保持串行，不影响已认证缓存资格 |
| MP3 passthrough | 条件成立时 | 是 | 真实输入持续持租约，输出 CDATA 独占；不解码音频，非法结果不得成为候选 |
| PathMusicMap | 条件成立时 | 否 | 真实输入持租约；I/O fallback 可能发出 Core scoped Tracer 诊断，保留 owner 执行 |
| PathMusicTrack/Event | 条件成立时 | 否 | 全部实际 File/header 持租约、header 非零 handle；保留语义路径，缺失/警告路径实际执行 |
| Audio EALAYER3 或未闭合/警告路径 | 否 | 否 | 保留原诊断与执行，不消除进程一次性警告 |
| Texture/OnDemandTexture/TerrainTextureAtlas | 否 | 否 | 兼容 owner 调用并设在途屏障；混合 Texture session 原拒绝保留 |

不存在“同族一个用例通过即整个族实测通过”的推断。以上资格由后端真实读取审计决定，不按 fixture ID 白名单。`CanRunParallel` 表示后端重入合同已认证，不表示该次任务一定进入 worker；执行器另外按 `PreparedBytes`、可用槽位和当前机器预算准入。

## 身份、事务与资源

强 Key 包括版本、实际工具/配置/schema/进程输入、有效后端参数、冻结消费输入、兼容引用元数据和完整外部读取摘要。jobs、TaskId、发现序号、worker 和私有输出目录不进入 Key。旧游戏 FileBase 保留格式用途，不能授权新路径复用；相同旧命名身份、不同语义结果明确拒绝。

全根盘点仍在原保护时点运行，保障适用 resolver 边界及发布前变化检查。新工具身份复用该扫描已有文件 SHA256，不再独立重复盘点工具树；MOD/Additional 普通内容不进入工具身份。当前尚未合并逐 stream 的全根保护扫描，报告 `hashReadBytes` 含保护扫描和后端外部输入哈希成本，不能把此实现称为严格文件系统快照。

存储按 ActionKey 读取与物化单条完整结果；不恢复历史整树，不导入旧缓存。候选和发布产物独立复制；Commit 可失败安装并保留 pending，必要报告/终态/sink 完成后 Complete 才移除排除标记。失败撤销安装及修复替换，遗留 pending 继续 miss。根写锁、4096条/4GiB 默认容量和成功后的整条清理已接入。

owner 保持前端顺序，worker 仅拿独占后端输入；结果按发现顺序验证和提交，文档最后任务提交后才完成/释放，再汇合链接。取消停止派发并等待不可中断 native 安全返回。新生产路径使用 `MachineResources.v1`，不再固定为两个任务、256MiB 总预算或64MiB 单任务上限；旧运行的固定限制仍属于其历史证据。

每个 stream 固定初次 CPU 上限，并通过 `GlobalMemoryStatusEx` 同时读取 availablePhysical、availableVirtual、totalVirtual。预留 `max(128MiB,totalVirtual/4)`，从 `min(availablePhysical,availableVirtual)` 扣除后得到非负 usable；总预算为 `min(usable/2,totalVirtual/4)`。容量由 requestedJobs、CPU、`总预算/16MiB` 和32个槽位的防御上限共同限制，最低为串行1；没有可用预算时不虚构最低可用内存。32是限制 Win32 线程栈及调度开销的防御顶，不是默认线程数，也不能只按64位宿主机物理 RAM 放大 x86 进程。

owner 在任务准入处最多每100ms重新采样一次；本 stream 的预算和容量只保留已观测较小值，不因后续瞬时回升扩容。单任务 worker 上限与准备态克隆上限均为当时总预算的一半；排队前同时核对已保留的准备态、在途及待提交结果之和。内存查询失败明确退化为串行，使用16MiB保守预算并报告 `resourceMemoryQuerySucceeded=false`，不能把该回退值称为实测可用内存。

`PreparedBytes` 保留后端成本估算：W3X 按帧/通道和节点展开；APT 与 MP3 passthrough 在准备态上另加全部租约输入长度4倍；无 PCSampleRate 的 AudioFile 加64倍，保守覆盖解码 float 样本、loop 片段、编码列表和 CDATA；Xmlaudio 七根使用准备态估算。长度求和与乘法 checked，溢出视为超预算。Audio 显式采样率和 PathMusic scoped Tracer 路径仍保持 owner。准备前无序列化遍历 DOM；超过机器准备预算时不克隆、不缓存，直接 owner 兼容调用。超大任务的 owner 路径沿用原执行峰值；这些估算和采样不承诺进程通用硬内存上限。

报告的 `requestedJobs` 是用户请求上限；`effectiveJobs` 是各 stream 初始准入容量的最大值，`minimumEffectiveJobs` 是运行中观测的最低准入容量；两者均不等于实际同时执行的后端数。`taskMemoryBudgetBytes`、`minimumTaskMemoryBudgetBytes` 分别保留初始最高和观测最低总预算，`perTaskMemoryLimitBytes` 对应最低预算的一半，另有 `logicalProcessorCount`、`resourcePolicy` 和查询状态。真实并发只看 `maxBackendConcurrency` 和最多512条 `backendIntervals`。报告还保留 executionEngine/cacheMode/backendCalls、backendCallsByType、hashReadBytes/restoredBytes/writtenBytes/cacheCopyBytes；hashReadBytes含保护盘点、外部输入与store payload校验的文件SHA256读取，cacheCopyBytes只含store拷贝。peakSampledPrivateBytes来自边界采样，不等于OS连续峰值。缓存命中、排队长度和请求 N 都不能代替真实调用重叠。

## 当前收据与续接

最新已验收批次：`129d9639df7379dfe7898c5037afc32501eb0cd0`，[40660066519](https://github.com/wu162/NewSdk/actions/runs/40660066519)，**success**；binary/test/packagingSource均为129d。两配置新构建通过完整应用/Core、稳定SDK输入共享/嵌套MOD/Additional保护、真实SDKArt Texture与SageXml账本、精确缓存/CDATA、十二根环境XML worker及Weather/W3X真实重叠、取消/异常、原BAB、旧session、完整未精简mymod、BIG及五文件ZIP。原始归档12805100025、SHA256 `cebe5b0798c9ec2719ca0743e8702575ec44962ec47023bc6d94374d51b1af47`；12组收据和运行文件/ZIP已独立核验。Debug ZIP `52029ecd2c4574fa64e10a73c65ed0a5f7c2aef4808c1115de2c9eadfcaf8cac`，Release `415ad9c27043d498a0a0dd8d1ab4f66fb5d6b378931073983ff026e4b4ceb240`。精确来源、区间、SDK读取账本和原因计数见[机器证据](CACHE_PARALLEL_EVIDENCE.json)。旧810计量不能冒称该二进制成本。

两配置冷缓存8次`entry-not-found`，热缓存8hit/0miss且只保留Texture一次owner调用；损坏与pending修复分别7hit/1miss且仅一个对应原因，修复后恢复8hit/0miss。原因总和严格保留旧miss计数；每个修复用例八产物及原BAB对照均原字节一致。环境十二根均实际在worker上运行；观测到的跨家族重叠子集两配置均仅Weather，不扩大为每根双同类重叠。

资源自适应里程碑：`c2a2cec616ff625ce37f4751084d3b82626f98f4`，[40650040607](https://github.com/wu162/NewSdk/actions/runs/40650040607)，**success**；binarySourceCommit、testSourceCommit、packagingSourceCommit 均为该提交。Debug/Release 全新编译，通过应用/Core、资源纯计算/实际准入、压力降额、超大单任务 owner 与小任务 worker、原有取消/异常安全汇合、原BAB/Texture/CLI/协议、精确缓存/CDATA、XML/Audio/W3X真实并行、旧session、完整未精简mymod、BIG与精确五文件ZIP独立运行。

两配置均在逻辑CPU=4、requestedJobs=4的真实机器上得到 effectiveJobs=4、actualMaximumConcurrency=4，四个独立AudioFile后端区间确实重叠，串行/并行原始输出一致；总预算536838144字节（约512MiB），并非宿主机RAM的任意比例放大。压力注入发生时仍有1个真实调用在途，额度从8收紧至1、预算降为16MiB，3个调用安全完成且四输出逐字节相同。44MiB总预算/22MiB单任务用例确认8MiB APT（估算至少32MiB）在owner执行，小APT仍使用worker。内存估算和准入采样不等于进程硬内存保证。

原始归档 artifact `12800535841`，SHA256 `20ac889a7d3180034d8761fce4eb56ee09a0aa5901d4551cc6edfaa85a82d5b7`，已下载并核验12组收据、运行文件、两配置ZIP及其内部精确五文件。Debug ZIP SHA256 `b3736967c80193932545892f9c5ea2f72c825db41ee7ac7ad05cacba1955fac2`；Release `b4a98fc66cb2625cd002d9ef9db789968ee92042cac80cc78219d636ac0cebbc`。完整来源、区间和逐用例计量见[机器可读证据](CACHE_PARALLEL_EVIDENCE.json)。文档提交不冒充二进制来源，主线发行不自动替换。

新增 XML/Audio 组已通过完整批次：`d7ecbebf884f03b1b682bee7eecfe5db9207b8dc`，[40647141375](https://github.com/wu162/NewSdk/actions/runs/40647141375)，**success**。binarySourceCommit、testSourceCommit、packagingSourceCommit 均为该提交；Debug/Release 全新编译，保留当时最多2任务、256MiB总预算、64MiB单任务及128MiB准备态的初始保守限额。此成功不等于新 `MachineResources.v1` 自适应验收。

本批包含11根 XML 的源码资格，其中七根 Xmlaudio 通过独占准备态与 owner 三缓冲区逐字节对照；Audio 未知注入编码器拒绝、显式采样率串行及大输入预算边界通过。两配置均证明独立文档的 AudioFile 和 AptAptData 在无持久缓存时真实后端区间重叠、最大并发2，串行/并行输出逐字节相同；冷/热/单改继续覆盖完整 CDATA。MP3 mono/stereo 的 jobs1/jobs2、完整 CDATA 原字节及冷热缓存对照也通过；不将这些样例扩大为每个新增子类型的实际重叠实测。既有 W3X、读租约、旧来源授权、重复include/继承、取消/异常安全汇合、CLI/协议、缓存/损坏/事务、旧session、原BAB/BIG、完整未精简 mymod 及精确五文件 ZIP 独立运行均通过。

原始归档 artifact `12799061630`，SHA256 `3b4711b1e15283998b0173dbc9d88d002cd2890fcb4928f471cc774a414e7da0`，已下载核验12组收据、运行文件和两配置 ZIP。Debug ZIP 为 `fa423e26a51885fab908494a41fb88dd1605508052723f0bf2df56383bc61dd9`，Release 为 `9d64db7f60d85f7031c22850db58e8e9c53c4157682bea701b912dfef4246292`。逐用例区间、计量和完整身份见[机器可读证据](CACHE_PARALLEL_EVIDENCE.json)。Core 原始收据仍确认取消/异常时 Dispose 前有真实调用在途、返回后 active=0，两条区间结束，失败不发布完成状态。

资源组首次运行 [40649384165](https://github.com/wu162/NewSdk/actions/runs/40649384165) 保留 **failure** 原终态。原始日志确认两配置均仅 CompilerPreparedBackend 的 SingleTaskAdmission 路径识别断言失败（timeout=false、exit=1）：Core将物理include路径小写化，而测试按`One.xml`大小写匹配。`c2a2cec6`仅改为OrdinalIgnoreCase，owner/worker断言未放宽，生产闭包与`0f5388a4`相同，后续完整批次通过。现有复用入口仅覆盖payload，没有Core validation host及完整SDK/发行重放链；最小适配成本高于一次集中重建，本次因此集中完整重验，没有强行复用。

成功归档重放入口的 guards [40648208910](https://github.com/wu162/NewSdk/actions/runs/40648208910) 与 [40649384198](https://github.com/wu162/NewSdk/actions/runs/40649384198) 已 success。真实 payload 重放 [40649079387](https://github.com/wu162/NewSdk/actions/runs/40649079387) 也 success，分别记录 binarySourceCommit=`d7ecbebf884f03b1b682bee7eecfe5db9207b8dc`、testSourceCommit=`0e0edc16d7e5c7229b8afc4ee8d159dc73648c0b`；原始收据已核验，重放 artifact `12799251466`、SHA256 `dbeb151510c476f53d02fe6f5036565c5d84bbb11314f72b467a3f70c96a6891`。它验证自适应前 XML/Audio 闭包的测试复用，不是 `0f5388a4` 或 `c2a2cec6` 的生产验收。

旧 W3X 里程碑保留为历史来源：`125902cc5b8b0c3df3434b24db574970e2e022d5` / [40629580163](https://github.com/wu162/NewSdk/actions/runs/40629580163) success，artifact `12791073520`，SHA256 `44d237d777bbbe6d8730beab34b1aff60c9510e2b646e8dcf48adc549f00d4fa`。其取消/异常、完整 SDK 和独立 ZIP 范围不扩展为后续源码结果。主线发行不自动替换。

失败记录保留原终态，均未放宽断言或超时：

| runId | 确认原因与修复 |
| --- | --- |
| [40625998371](https://github.com/wu162/NewSdk/actions/runs/40625998371) | 私有候选名重复完整Key/GUID导致Win32路径过长；改为短序号，后续全组通过 |
| [40626352606](https://github.com/wu162/NewSdk/actions/runs/40626352606) | GameObject夹具缺必需Body，原始XSD异常已核实；修正真实夹具并增加长链接前预检，后续普通缓存矩阵通过 |
| [40626782705](https://github.com/wu162/NewSdk/actions/runs/40626782705) | 旧大帧W3X串行控制 `control.json` 明确 timedOut=true、180秒超时；实际压缩O(n²)成本使夹具过重。改4096/3072帧并保留真实重叠、字节对照与原超时，后续完整组通过 |

## 单样本计量与实际边界

以下为 `d7ecbebf` / `40647141375` 同一成功运行的 Release 小夹具，使用当时固定保守限额，固定运行文件与输入族，顺序启动独立CLI进程；“空缓存”指编译器动作缓存为空，OS文件缓存未清空。表中空缓存、单改和Additional使用jobs2，热缓存使用jobs1，精确参数保留在原报告。仅提供原始耗时与调用数，不推出全MOD加速倍率，也不作为自适应源码的性能结果；完整机器/OS缓存控制及重复采样尚未做。

| 夹具 | 状态 | 后端调用 | hit/miss | elapsedMs |
| --- | --- | ---: | ---: | ---: |
| Audio PCM/XAS | 空动作缓存 | 2 | 0/2 | 569 |
| Audio PCM/XAS | 热缓存 | 0 | 2/0 | 571 |
| Audio PCM/XAS | 单资产修改 | 1 | 1/1 | 589 |
| Audio PCM/XAS | 仅Additional修改 | 0 | 2/0 | 558 |
| W3X | 空动作缓存 | 3 | 0/3 | 634 |
| W3X | 热缓存 | 0 | 3/0 | 595 |
| W3X | 单资产修改 | 1 | 2/1 | 603 |
| W3X | 仅Additional修改 | 0 | 3/0 | 591 |

小夹具热运行仍有约13MiB输入保护/校验哈希读取，前端、启动、全根保护和链接继续执行；零后端调用不等于只重打包。同批 Release 双文档 streamed XAS 串行/并行分别539/500ms、APT分别611/609ms，均有实际区间重叠和原始字节对照；这两个单次计量只证明当前样例，不推导机器资源策略或大型 MOD 的速度收益。既有混合Texture验收保留其真实调用和其他认证资产命中合同，未因此开放Texture缓存。

仍保留的范围与下一步：

- Texture、未闭合/警告/非ASCII native读取、超大准备态继续兼容执行；新增11根XML和两类Audio已有初始保守限额的完整通过证据，MachineResources.v1自适应组已有最新完整通过收据，其余XML及Audio重采样/PathMusic仍串行。资格表区分源码认证与实测，未把样例重叠扩称每个W3X或新增XML/Audio子类型实测。
- 各stream全根保护扫描尚未合并；已有按需恢复和扫描内工具摘要共享。同一构建内稳定SDK/runtime文件已使用受512句柄限制的持续只读租约及128位文件身份复核来复用摘要；当前MOD子树和SDKAdditional保持鲜读。目录枚举成本仍在，跨构建仍需首次读全输入，不能用mtime/size替代。
- 尚未形成细分全部miss原因或真实大型MOD的受控冷/热/单改/封装性能基准；性能成本按下文次要小任务有界处理，不据此将调优升级为主线。

XML/Audio 初始组已完成有界实施与验收，机器资源组也已完成最新完整验收；后续从下列真实剩余范围继续。续接只需读本页、[REFACTOR_RESUME](../REFACTOR_RESUME.md)和相应最新运行；按失败步骤取原始日志，不重新全仓调研。成果在 [draft PR #17](https://github.com/wu162/NewSdk/pull/17)，目标compiler-main；禁止强推，未经用户明确批准不合入。全部编译/测试在GitHub Actions；文档整理不触发Windows构建。

## 可选真实仓库 MOD 计量

`refactor-cache-payload-replay.yml` 的 `repository_mod_metrics=true` 在完整payload/协议重放之外追加计量，默认关闭，不增加普通生产矩阵负载。必须显式指定已完整成功的生产 source/run/attempt/artifact/SHA256，并通过既有祖先、完整闭包、构建/链接配方、API成功范围和运行文件校验；不能用较旧生产源码的归档验证新生产输入。

每配置使用原仓库 `SDK/Mods/mymod` 的未精简副本与真实完整SDK，固定development/loose/base/jobs=1、sc=false、Additional关闭及原资源根顺序。运行无缓存、空动作缓存、三次热运行、私有副本仅Additional变化；保留六次CLI原始报告/事件和严格四文件逐字节对照。SDK全部文件、完整MOD和五个运行文件均保存实际字节指纹并在结束时复核，准备/哈希/归档成本不混入CLI计时。报告分别记录产品与控制器耗时、真实后端调用、hash/read/copy、预算与边界采样内存。

仓库MOD仅7文件约996KiB，这不是大型MOD基准；OS文件缓存未控制，“空缓存”仅指动作缓存为空。Additional不进入loose输出，此例只证明编译命中与流字节不变，不是封装性能或封装字节差分。保留未认证owner调用的实际计数，不靠要求所有热运行零调用掩盖未开放资格；不按耗时阈值放行、不推断通用加速倍率。`92b7925e`的[guards 40658122856](https://github.com/wu162/NewSdk/actions/runs/40658122856)已success，真实重放[40659913327](https://github.com/wu162/NewSdk/actions/runs/40659913327)已success并核验原包、两配置52报告/52事件文件及12次真实MOD四输出原字节。SDK实际14715文件/125887928字节，MOD7文件/982257字节；三次热与Additional-only均47hit/0miss/0backend。

该来源仍为810（仅工具摘要共享）。Debug无缓存945ms、空动作缓存3506ms、热3303/3224/3253ms；Release756ms、3278ms、热3101/3168/3121ms。热hashRead分别231979083/229537867字节，复用37516937/35075721字节。这个小MOD缓存热运行比关闭缓存慢，扫描成本仍大；不能隐藏该结果或称为通用加速。129d相同输入计量[40662537517](https://github.com/wu162/NewSdk/actions/runs/40662537517)已success，binary129d/teste259；原包12803958986、SHA256 `387a7189e4a0a2dcb025b76df007c2cc4afcbff160c9b894e20abbaddb58c026`已核验。两归档SDK全部14715文件/MOD全部7文件的path/bytes/SHA、工具链、测量脚本及控制器源码/编译配方均相同；控制器每次新编，EXE哈希分列，未把非确定性二进制强当相同。规范化临时根后的实际参数和effective options相同，六用例各四输出跨两个生产二进制均逐字节一致。

在这些条件下，两配置每次热构建的hashRead均精确少78862090字节（约75.2MiB）：Debug231979083→153116993，Release229537867→150675777；reuse分别37516937→116379027、35075721→113937811。仍为47hit/0miss/0backend、两次完整盘点、512句柄。新热原始report时间Debug2477/2545/2550ms，Release2423/2463/2503ms；旧/新热中位数分别3253→2545、3121→2463ms，仅保存本次观测，不因OS缓存/机器调度未控制而推出速度倍率。新无缓存763/668ms，仍比热缓存更快；此小MOD仍没有缓存净收益，首次全根哈希和目录元数据成本继续存在。

## 小 MOD 热缓存开销（次要小任务）

状态：待办。用户于2026-10-28明确要求将此问题作为小任务记录，**不作为缓存与并行主线**。有余量时集中处理，不挤占主线功能推进，不作为主线或现有PR验收的新增阻塞项。

- 问题基线：129d的同输入计量40662537517中，47资产热缓存全部命中且零后端调用，但仍哈希读取约151–153MB；Debug/Release热中位数2545/2463ms，无缓存763/668ms。原始来源、完整输入指纹和限制见前节及JSON，不把命中率或读取量下降直接当作加速。
- 目标与范围：降低小MOD热缓存相对无缓存的额外开销，优先按计量选择目录枚举、非保留文件身份查询、首次/重复哈希中的一至两个具体瓶颈，做有界改进；跨构建摘要复用只有取得可靠字节版本证明后才考虑，不凭mtime/size授权。避免为此启动全局重构、长期调优或扩大压力负载。
- 正确性底线：保留强身份、全根变化检测、嵌套MOD与Additional鲜读、同长度同时间修改拒绝、取消/异常安全汇合、事务回滚及原始输出逐字节一致，不以关闭保护换取速度。
- 验收：固定SDK/MOD实际字节、参数和配置，分别记录二进制与测试来源，比较无缓存、空动作缓存及重复热运行的端到端耗时与扫描/查询/读取成本；保留跨版本四输出字节对照及受影响完整回归。仅减少读取量不足以标记“问题已解决”；未见端到端收益则如实记录并暂停，不继续挤占主线，也不承诺所有小MOD启用缓存都更快。编译测试遵循EFFICIENCY在Actions集中执行，能严格复用二进制时不重编。

## 后续验证效率要求

本轮正确性门槛已有通过证据，但必要测试分批补充、过重动画夹具及重复完整构建降低了执行效率。用户再次要求提高效率，后续统一按 [EFFICIENCY的批次规则](../EFFICIENCY.md#验证批次与触发决策) 执行：先准备完整能力组与必要回归，在CI长链接前完成夹具预检；检查点普通推送不自动触发全量；失败集中定位和修复。保留严格字节对照、真实调用重叠、取消/异常、事务和适用SDK/发行门槛，不能靠删断言或放宽超时减少失败。

现有 `scripts/DownloadCompilerCliReuse.py` 固定绑定历史源码/运行/归档，不能仅设置 `cli_only=true` 就复用本页的新二进制。仅测试/控制器改动时，先核对现有入口能否以最小适配支持本页的成功产物：必须核验生产闭包、编译配置/工具链、归档和运行文件哈希，并分别记录binarySourceCommit/testSourceCommit；原有来源校验应改为准确核验两者，不得直接删除。无法安全复用时，明确具体缺口，集中准备后重建一次，避免每补一个测试就重编。

普通并行验收的4096/3072帧是已通过的真实工作负载，保留原180秒门槛与后端区间重叠断言；不要恢复旧32768/24576帧规模作为常规串行控制。独立取消/异常用例的更重第二任务用于证明仍在途调用被安全汇合，不能为压缩耗时破坏该性质。计量采用上文原始报告，不将小夹具、空动作缓存或单样本耗时泛化成完整MOD性能。
