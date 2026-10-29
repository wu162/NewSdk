# Schema/文件身份/Texture 四项优化：完整 Windows 验收通过，短 benchmark 待跑

工作分支 `recovery/legacy-sdk-cache`，未合入其他分支，禁止强推。

本轮生产与测试来源 `639a5ff73e1df8a12ec0fa77e5b4324186bae8fb`；[完整 Windows 运行 40878062262](https://github.com/wu162/NewSdk/actions/runs/40878062262) success。Debug/Release 的生产组合、Core/后端/应用、原版基线、无部署清单 Texture 载荷差分、真实 CLI/冷进程/取消、构建缓存、输入根、Audio/W3X、会话缓存冷热/失效/缺损修复、BIG/协议、完整 mymod 原版差分及五文件发行包全部通过。FileHashItem 宿主用例确认独占锁定下热/持久 SHA 收据可复用，以及等长恢复 mtime 的变化文件一次读取同时获得 FastHash/SHA256。

实现：删除配置加载与每流的重复 Schema 检查，一个构建请求共享编译后的 SchemaSet 与目录身份快照；FileHashItem 从既有文件身份取得存在性、时间戳和可用 SHA 收据，变化时合并两种 Hash 读取；Texture 运行时部署审核和测试宿主的重复清单审核已删除，构建时 PE 验收、类型/模块归属和原版载荷差分保留。独立 Core 的显式 Schema 重载仍支持可变宿主。详见 [缓存实现说明](docs/LEGACY_INCREMENTAL_CACHE.md)。

首轮 `de95cc20` / [40876849281](https://github.com/wu162/NewSdk/actions/runs/40876849281) 保留 failure 原终态：Texture 插件已完成处理，测试宿主退场审核再次读取已删除的 patch 清单而失败；其余独立 Core/CLI/缓存/输入根/BIG 组通过。修复仅涉及测试宿主重复审核，并已在上述完整成功运行统一重验。

接下来只用成功运行 40878062262 的 Release 执行新旧 SDK 有界对比：3 个同物理输入 workload、6 种缓存模式、每种冷态 1 次/热态 2 次，单进程 20 秒、测量总预算 120 秒；不重建生产，不把历史运行的绝对耗时差当作本轮收益。24 项 benchmark 预检及生产源码/依赖闭包校验已通过。原始验收归档、benchmark 报告及摘要待归档，尚未宣称 benchmark 已通过。

前序 BIG 包缓存移除里程碑：`7fca1e4f` / [40872280934](https://github.com/wu162/NewSdk/actions/runs/40872280934)；StableSort/身份缓存/LogicCommandSet 原版差分里程碑：`ae4654a8` / [40868109538](https://github.com/wu162/NewSdk/actions/runs/40868109538)。BIG/SKUDEF 继续每次正常打包，通用未变发布与失败/取消回滚保留。
