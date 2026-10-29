# 原 SDK 缓存能力恢复（实现与验证中）

分支 `recovery/legacy-sdk-cache`，基于 `compiler-main` 的 `e7396d3b`。用户于 2026-10-29 明确要求恢复原来的完整缓存能力；早期仅覆盖增量缓存的范围已被替换，不能将该子集当作交付完成。

按原 `Settings`、`BinaryAssetBuilder.DoBuildData/InitializeSessionCache` 和 `BinaryAsset` 恢复：

- `/sc`、`/bc`、`/psc`、`/csc` 默认 true；`/fsc`、`/atc` 默认 false。旧 defaultscript 显式传入 `/csc:false`，这与 EXE 本身的默认值不同。
- Core 逐文档、依赖、定义、继承及逐资产检查复用，不再将整个 SDK/MOD 内容清单用作缓存命名空间。已有文件修改、新增及删除均交回原解析/依赖判断。
- Texture、OnDemandTexture、TerrainTextureAtlas 使用原 SDK 描述符的资产缓存能力，并接入会话缓存；必须以真实纹理数据和旧版输出验证，不能仅删除拒绝条件便算完成。
- `/csc` 选择原 XML 或 Deflate 格式；`/psc` 在中止后保留已完成的可复用会话工作；`/fsc` 读取但不更新会话；`/atc` 进入原 Core 的缓存文件 touch 路径。
- `/scd`、`/scp` 与 `/bcd`、`/bcp` 及 `/iod` 支持独立目录。默认会话文件位于输出根 `BinaryAssetBuilder.SessionCache.xml[.deflate]`，默认资产目录为输出根 `cache`，资产使用原类型/实例哈希布局，允许读取旧 SDK 格式。
- 新宿主仍在独立 staging 生成最终 MOD 并发布。CDATA 闭包保存在会话根 `.NewModCompiler/cdata`，无新侧车的旧目录可直接导入；Schema、宿主和解析设置身份侧车用于保守冷失效。缓存根外资源搜索恢复原解析契约。

`/psc:false` 的会话确认仍跟随最终发布和必需输出结果；`/psc:true` 明确允许在失败/取消后保留已完成缓存，失败终态与未发布的 MOD 不改变。`/bc` 使用原生可共享缓存的独立资产写入语义。

已有增量、原版字节差分、失败/取消、损坏修复与原回归保留并改为针对原语义检查。另补默认值、原目录导入、压缩切换、冻结、部分恢复、touch、纹理三路及共享缓存测试。Windows Debug/Release 和完整 SDK 验收须取得新运行结果后才标记完成。
