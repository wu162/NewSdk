# 原 SDK 缓存：行为与实现

本页记录当前缓存恢复的行为、代码分工和验收范围；分支、准确源码及待办见 [交接](../REFACTOR_RESUME.md)。运行 `40814401237` 的全部生产验收门槛已通过，具体覆盖边界见下文。

## 链接与发布复用（2026-10-29）

新增实现位于 `codex/legacy-cache-completion`，首次实现检查点 `f44b49a2`；Windows 生产验收正在运行 [40832233489](https://github.com/wu162/NewSdk/actions/runs/40832233489)。旧验收不能代替本次结果。

- `LinkedOutputCache` 将已完成的四流保存在输出根 `.linked-streams`。Core 仅在实际创建某个流的 `OutputManager` 时恢复该流的完整四文件，继续使用原有 manifest/type/stream checksum 判断是否重写。删除的 include 不会通过整目录恢复混入本次输出。该能力独立于 `/sc`、`/bc`，BIG 与 loose 共用；冻结会话不冻结链接输出。
- 私有四流快照使用硬链接，文件系统不支持时退回复制。原链接器以临时文件替换二进制，并通过 rename 恢复未变 manifest；不会就地改写快照。已发布文件和恢复的 CDATA 使用独立存储，后续编辑不影响缓存。
- 发布先比较精确文件集合、大小和 SHA256。与本轮已采集内容身份一致时，不创建 `.publish-*`，不复制、不交换目录；报告保持 `publicationStatus=PublishedDirectory`，新增 `publicationUnchanged=true`。公开文件损坏、缺失或多出文件时仍生成完整候选并执行原事务。
- Core 产物身份只采集一次，内部 `Verified` 状态供 BuildSession/打包/发布复用，不进入协议。移除重复的输出 SHA256、已接收产物的打包后再次校验、发布副本再次哈希和逐文件 `Flush(true)`；会话身份仍覆盖文件内容，但不再逐文件重走祖先检查。
- 会话序列化文件、安装及备份使用移动；CDATA 先建立私有快照，安装时跳过字节相同的文件。取消、编译/报告/必需日志失败仍撤销未确认的最终产物和链接缓存；`/psc` 的既有部分保存语义保留。

测试入口：`LinkedOutputCacheTests`、`ArtifactPublisherTests`、`SessionPublicationTests`、`LegacyCacheStoreTests`；生产会话矩阵增加无缓存热链接、缺失链接组重建及三种纹理热链接，检查原链接器四项 up-to-date 日志、原始字节与发布文件时间。

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

## 已通过的生产验收（2026-10-29）

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

本次续接未更改生产、测试或构建输入，沿用上述成功运行。归档元数据及上传日志 SHA256 一致，但本环境下载 HTTP 403，未在本地打开原始收据或复算 ZIP 哈希；这项访问限制单独记录在摘要中。
