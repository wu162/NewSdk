# 原 SDK 缓存：行为与实现

本页记录缓存恢复与热态优化的行为、代码分工和验收范围；当前分支、准确源码及结果见 [续接入口](../RESUME.md)。旧阶段交接与验收在后文保留。

## 本轮热态优化（2026-10-29，已验收）

直接开发于 `recovery/legacy-sdk-cache`，生产与测试源码 `84261243`；完整构建/生产差分 [40845943601](https://github.com/wu162/NewSdk/actions/runs/40845943601) success。独立应用、完整生产矩阵、mymod 原始字节差分和独立五文件 ZIP 均通过；[原始收据摘要](../evidence/refactor/hot-cache/40845943601-1/summary.json) 保存归档哈希及来源。两配置热态 `xmlParse=0`、`instanceCompile=0`、`fileHashReads=0`、`fileHashBytesRead=0`、`publicationUnchanged=true`，会话/CDATAs 写时间保持不变。新旧 SDK 配对 [40848391545](https://github.com/wu162/NewSdk/actions/runs/40848391545) 也已 success，具体计量见 [SDK 对照](SDK_BENCHMARK.md)。

- 生产插件按实际请求的类型创建；缓存版本从原 Win32 元数据取得，不为汇总版本加载 Texture/W3X/Audio 等无关实现。doctor 和显式全路由目录检查继续完整初始化；跨流只重新初始化已经加载的实现。
- 会话 Schema 身份来自 `SchemaSet` 已读取的实际 XSD include 闭包及其内容哈希，不再递归枚举整个 `Schemas`。同一构建请求内复用已编译 Schema；未参与闭包的孤立文件不再导致冷失效。
- Core 原 manifest/type/stream checksum 确认四流未变化后，BuildSession 直接接受缓存文件的内部引用。未变化的一代不创建 staging 四流、候选目录、硬链接或快照，也不做链接目录交换；缺失组仍重建，移除的 include 通过当代完整成员表清理。直接调用后端的现有物理输出合同保留。
- 会话直接只读载入已确认的 XML/Deflate，避免复制到 staging 再读取；Core 跟踪实际文档、文件、编译器版本和最终 stream hints 变化。同格式且未变化时不序列化、不建立 LegacyCacheStore 候选、不比较或安装会话。CDATA 只按实际使用的资产定位和引用，避免整树复制。压缩切换、原格式导入、部分/冻结及失败取消合同保留。
- 发布以产物列表及其内容身份定位上一代收据，再检查已知文件/目录的 FileId、ChangeTime、mtime 和长度。正常热态不递归枚举 `published`、不重复读文件内容或大小、不创建发布候选和目录交换；真实内容/成员变化仍走边界检查与回滚事务。
- Core 输入 SHA256 接入同一文件身份缓存，避免下游再次全读未变化输入。删除生产内部产物的重复祖先检查、属性遍历和发布前后重复整树检查；外部输入/输出重叠、重解析点边界、路径合同、锁和真实失败回滚保留。

本轮新增少量关键检查：虚拟四流复用不落盘也不替换旧文件，发布收据识别嵌套新增文件和等长/恢复 mtime 编辑，真实热态会话及 CDATA 不重写。完整原版字节差分和正式 ZIP 门槛继续保留。历史验收见后文，不将其作为本轮新代码的结果。

## 上一轮链接与发布复用（历史，2026-10-29）

实现分支 `codex/legacy-cache-completion`，[PR #20](https://github.com/wu162/NewSdk/pull/20)；该阶段完整生产源码 `4ad5d0d9`，统一验收 [40836177604](https://github.com/wu162/NewSdk/actions/runs/40836177604) 已 success，完整 mymod 原版字节差分及独立五文件 ZIP 同时通过。[原收据核对摘要](../evidence/refactor/performance-implementation/40836177604-1/summary.json) 记录下载归档 SHA256、来源、两配置零热态 SHA 读取与实际 BIG 复用；旧验收不替代本批结果。

- `LinkedOutputCache` 在输出根 `.linked-streams` 保留四流，按实际创建的 `OutputManager` 懒恢复完整四文件，沿用原 manifest/type/stream checksum 决定重写。删除的 include 不会混入输出；该能力独立于 `/sc`、`/bc`，BIG 与 loose 共用。
- 私有四流快照优先硬链接，不支持时复制；原链接器通过 unlink、临时文件和 rename 更新四流。公开文件及恢复的 CDATA 使用独立存储。会话、安装与回滚备份改为移动，CDATA 相同则不重新安装。
- `FileIdentity` 用 Windows 批量目录记录的卷标识、FileId、ChangeTime、mtime 和大小替代每进程 826 文件的内容 SHA256。等长且恢复 mtime 的写入仍由 ChangeTime 识别，替换由 FileId 识别。
- `FileHashCache` 将已确认收据保存在 `.file-identities.json`，仅保留本轮使用项，内容不变时不重写。公开文件、运行库与 Additional 的键包含 ChangeTime；编译器私有且只以临时文件替换的四流按 inode/mtime/大小复用，硬链接计数变化不触发重读。CDATA 使用完整身份键。报告 `cache.fileHashReads/fileHashReuses/fileHashBytesRead` 给出实际读取量。
- 发布核对精确文件集合和已确认身份，未变时不创建 `.publish-*`、不复制、不交换目录。报告保持 `publicationStatus=PublishedDirectory`，新增 `publicationUnchanged=true`；公开文件被修改、缺失或多出文件时仍执行完整事务。
- BIG 的输入指纹包含当前四流/CDATAs、Additional 实际覆盖后的内容、任务布局、版本/SKUDEF 选项及宿主模块身份。相同时直接引用既有 BIG/SKUDEF，跳过 LoD、Additional 快照和 MakeBig；公开包仍通过包含 ChangeTime 的身份确认，实际覆盖 warning 与空目录碰撞保持。显式 neutral repair 继续使用实际参考输入链，不复用该包指纹。
- 可信内部收据贯通 BuildSession、打包和发布，不重复哈希；Texture 的 EXE/DLL 校验也使用文件身份缓存。删去内置路由启动时逐项自审，插件版本按每个实际插件计算一次并按原路由数累加。
- 全部生产源码已无 `Flush(true)`。JSONL/文本以 64 KiB 字符缓冲，控制边界、warning/error 即时刷新，正常事件按 250 ms 批量刷新；普通 trace 不再逐条同步写 stderr，仍完整保留文件。

必需日志/报告失败、取消和真正的替换故障继续撤销未确认产物与链接缓存，`/psc` 语义保留。首轮 `40832233489` 的三个 `publication-fault` 失败保留：旧 fixture 以“锁住未变文件”要求失败，现已新增该场景的成功断言，并用额外旧文件强制真实替换后保留原失败及重试断言。

验证集中于已有完整生产矩阵，另增加少量关键组件/行为断言。生产会话用例检查原链接器四项 up-to-date、原始字节、发布时间及无 CDATA 热态 `fileHashReads=0`，Debug/Release 原收据均确认 2 个复用流、18 次哈希收据命中、0 次完整文件 SHA256 读取、0 字节 SHA256 输入，且 `publicationUnchanged=true`。BIG 热态包阶段事件直接引用 published 中的两文件。配对性能运行 `40837902446` 已通过，准确数据与小样本边界见 [SDK 对照](SDK_BENCHMARK.md)。

## 选项与目录

| 选项 | 默认值 | 行为 |
| --- | --- | --- |
| `/sc`、`/bc` | true | 会话复用与原类型/实例哈希资产缓存，独立开关 |
| `/csc` | true | 原 Deflate 会话格式；false 使用 XML。旧 defaultscript 显式传 `/csc:false` |
| `/psc` | true | 中止后保留已完成的可复用会话工作 |
| `/fsc` | false | 读取但不更新会话，不冻结资产或中间文件 |
| `/atc` | false | 复用中间结果时也进入原资产缓存文件 touch 路径 |
| `/scd`、`/scp` | 输出根 | 原会话目录及别名 |
| `/bcd`、`/bcp` | 输出根 `cache` | 原资产目录及别名 |
| `/iod` | 输出根 | 中间目录 |

SDK 的 `buildCache`、`buildCacheRoot` 继续生效，显式命令行/项目设置优先。相关参数可保存在项目预设；报告中的 `sdkConfig` 来源已加入协议 Schema。

## 生产路径

- `CoreStreamCompiler` 接回原 Core 的文档、文件依赖、定义、继承和逐资产复用。不再以整个 SDK/MOD 内容清单作为缓存命名空间；外部资源根保留原解析语义。
- Texture、OnDemandTexture、TerrainTextureAtlas 使用原描述符的会话及资产缓存。测试从固定原版 DLL 读取类型身份和 `HasCustomData`，以原版四流及完整 CDATA 字节对照验收。
- `LegacyCacheStore` 读写 `BinaryAssetBuilder.SessionCache.xml[.deflate]`，保留原资产哈希布局，支持无新侧车的旧目录导入。新宿主的 CDATA 状态位于会话根 `.NewModCompiler/cdata`；恢复时只带入当前流，避免无关流数据混入输出。
- Schema、宿主和解析设置的身份侧车用于会话保守冷失效。`CacheInputSnapshot`、`SessionPackage`、`BuildAssetCache` 旧包装组件仍有独立测试，但不再承担当前生产缓存路径。

## 失败与确认

最终 MOD 始终在独立 staging 中生成并发布；部分缓存保存不改变失败/取消终态，也不发布部分 MOD。

`/psc:true` 保存已完成的可复用会话工作；`/psc:false` 的会话安装跟随发布和必需输出结果提交，可回滚。`/bc` 保留原生共享资产的独立写入语义，即使会话不保存，中止前完成的资产仍可供重试使用。

## 验证入口

- `tests/App/LegacyCacheStoreTests.cs`：原目录导入、格式选择、CDATA、身份失效、部分确认与替换回滚。
- `scripts/TestCompilerBuildCache.ps1`、`TestCompilerBuildCachePayloads.ps1`：真实资产命中/失效、共享、失败重试、Audio/W3X 原始输出。
- `scripts/TestCompilerSessionCache.ps1`：会话冷热/依赖失效、缺损修复、取消/失败、三种纹理、全部高级模式与原版目录互操作。
- `tests/CompilerLegacyCacheMetadata.cs`、`scripts/TestCompilerBackendBaseline.ps1`：原 DLL 元数据、原 BAB 字节差分及缓存导出/导入证据。

## 原缓存恢复旧验收（39039961，2026-10-29）

[运行 40814401237](https://github.com/wu162/NewSdk/actions/runs/40814401237) 整体 success；二进制及测试源码均为 `39039961847cd1ca9cb93743fd51741a4e5c9f49`，Debug/Release 均重新构建。[状态与日志摘要](../evidence/refactor/legacy-cache/40814401237-1/summary.json) 保留运行、任务、产物及来源，原始收据位于该运行的归档内。

下列用例名来自 `TestCompilerSessionCache.ps1`，两配置会话协议各检查 101 份报告、4030 条事件，错误为 0。

| 范围 | 已通过的直接用例及断言 |
| --- | --- |
| 三种纹理 | 各类型 `*-session-cold/hot/invalidated`、`*-changed-hot`、`*-build-cold/hot`；同长度/mtime 图像修改失效，修改后对照原 BAB 四流及完整 CDATA |
| 原默认值、压缩切换 | `original-defaults-cold/hot`、`original-xml-switch`；实际 Deflate、文档复用、XML 替换且无旧格式歧义 |
| 冻结/解冻 | `original-freeze-edit/repeat`、`original-unfreeze/hot`；修改输出正确，冻结会话字节不变，解冻后更新可复用 |
| touch | `original-touch-disabled/enabled`；检查实际资产缓存文件 mtime |
| 独立目录及别名 | `original-cache-directories`、`original-cache-directory-aliases`；`/scd /bcd /iod` 与 `/scp /bcp` 共享真实缓存 |
| SDK 配置及 CLI 优先级 | `original-sdk-cache-config`、`original-sdk-cache-config-cli-override`；`buildCache`、`buildCacheRoot`、报告来源 |
| 原 BAB 导入 | `original-cache-export` → `original-bab-cache-import`；导入旧 XML 会话及中间资产，文档复用且无资产编译 |
| 部分失败保存 | `original-partial-failure/retry`、`original-no-partial-failure/retry`；psc true 保存完成文档，psc false 不保存会话但保留独立资产供重试 |
| 真实取消与重试 | `actual-cancel`、`retry-after-actual-cancel`；CtrlBreak、退出码 6、无部分发布、psc false 的原确认会话不变 |

Texture CDATA 数量由固定原 DLL 的 `HasCustomData` 确定，并非每种纹理都产生 CDATA。`/psc:true` 完成文档后遭 CtrlBreak 的持久化、原 BAB 导出的 Deflate 直接导入，未有独立生产用例；不能将其他组合的成功改写为这两个组合已独立验收。

同次运行还通过完整未精简 mymod 的原 BAB 四流（两配置差异均为 0）、原 LoD/MakeBig 及 SKUDEF 差分，以及独立五文件 ZIP 的生产 BIG/协议门槛。mymod 本身无 APT/maps 输入，两组覆盖来自独立生产用例；完整游戏数据、WBData、游戏内运行、完整 UI/runtime 和中文纹理路径未新增验收。

以上为原缓存恢复阶段的历史记录。当时续接未更改生产输入，沿用运行 40814401237，且曾记录下载 HTTP 403；本轮性能实现另以顶部的 40836177604 验收，并已通过 GitHub 产物工具取得新归档、在本地复算 SHA256 和核对原始收据。
