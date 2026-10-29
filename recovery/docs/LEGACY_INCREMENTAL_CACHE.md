# 旧 SDK 增量缓存恢复

开发分支：`recovery/legacy-sdk-cache`，基于 `compiler-main` 的 `e7396d3b`。

`/sc:true` 恢复上次成功会话的文档、intermediate 和 CDATA，再由原 Core 检查文档内容、XInclude、文件依赖、定义、继承和插件身份。修改 MOD 中已有文件不再把整个会话切到冷命名空间；无关文档可继续复用，链接输出仍重新生成。`/bc:true` 同样保留上次资产集合，资产是否命中仍由 Core 的类型、实例和依赖哈希判断。两个开关仍可独立使用，默认关闭。

输入清单同时产生两种身份，文件只读取一次：完整内容指纹用于编译前后输入稳定性检查；增量命名空间忽略 MOD 文件内容，保留文件/目录名称和布局、请求/任务/参数、SDK 其余内容、运行文件与进程解析合同。MOD 位于 SDK 内时，两次枚举采用相同的排除规则。新增、删除、重命名输入保守地冷失效，以覆盖搜索路径中新出现的候选；SDK、Schema、配置、运行实现或解析合同变化也冷失效。没有删除原内容 SHA-256 检查，也不直接信任旧 BAB 缓存目录。

每次成功编译保存最新缓存代际，避免同一命名空间永久停留在首次编译。替换前保留旧条目，发布、必需报告或事件终态失败时恢复；新条目确认前保留 durable pending 标记。完整清单、字节哈希、缺损冷重建和锁仍有效。会话恢复计数表示已恢复旧包，不表示所有文档均命中；`documentReuse`、`xmlParse`、`instanceCompile` 才反映实际增量工作。

验证入口：`CacheInputSnapshotTests`、`BuildAssetCacheTests`、`SessionPackageTests` 和生产 `TestCompilerSessionCache.ps1`；后者覆盖原有冷热/失效、原 BAB 字节差分、Audio/W3X/CDATA、损坏修复和失败取消，并增加无关内容编辑、受影响/未受影响流的同时存在及更新后的热复用。由 Core lifecycle 工作流重建 Debug/Release 和运行完整受影响回归。验证结果取得前不标记为验收完成。

本轮没有开放混合 Texture 会话缓存、压缩/部分/冻结模式和旧缓存目录覆盖，也没有修改缓存默认开关。新增模式需另行证明依赖闭包和旧版行为。
