# 原 SDK 缓存：行为与实现

本页只记录当前缓存恢复的行为和代码分工；分支、准确源码、验证状态及待办统一见 [交接](../REFACTOR_RESUME.md)。已接入不等于完整验收通过。

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

只有交接中对应完整运行及其收据取得成功后，才能将这些能力标为已验收。
