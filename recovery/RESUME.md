# Schema、文件身份与 Texture 优化：完成并验收

工作分支仍为 `recovery/legacy-sdk-cache`，禁止强推，未合入其他分支。

本轮四项均已实现：删除重复 Schema 检查；一个请求的全部流复用编译后的 SchemaSet 和文件身份快照；FileHashItem 从既有身份取得存在性、时间戳及 SHA 收据，变化文件一次读取同时计算 FastHash/SHA256；删除 Texture 运行时部署审核及测试宿主重复清单审核，构建时 PE 验收、类型/模块归属及载荷差分保留。独立 Core 的显式 Schema 重载仍支持可变宿主。详见 [实现说明](docs/LEGACY_INCREMENTAL_CACHE.md)。

生产与测试来源 `639a5ff73e1df8a12ec0fa77e5b4324186bae8fb`；[完整 Windows 运行 40878062262](https://github.com/wu162/NewSdk/actions/runs/40878062262) success。Debug/Release 生产组合、Core/后端/应用、原版基线、无部署清单 Texture、真实 CLI/冷进程/取消、构建/会话缓存冷热/失效/修复、输入根、Audio/W3X、BIG/协议、完整 mymod 原版差分和五文件 ZIP 全部通过。[原始验收收据与归档摘要](evidence/refactor/schema-filehash/40878062262-1/summary.json) 已下载并复算归档 SHA256。

新旧 SDK 短 benchmark 也 success：[40880915003](https://github.com/wu162/NewSdk/actions/runs/40880915003)，测量脚本来源 `0c6ea99fe2973fe3e3e0232ab9e1e3e9f7ea9fc6`，二进制来自上述完整成功运行。实际 49.06 秒，108 次测量/54 对；3 个同物理输入 workload、6 种缓存模式，冷态 1 次/热态 2 次；流和 CDATA 的成员、长度、SHA256 及原始字节全部相等。单进程 20 秒/总预算 120 秒。[完整结果](evidence/refactor/sdk-benchmark/40880915003-1/comparison.md)、[CSV](evidence/refactor/sdk-benchmark/40880915003-1/comparison.csv)、[含来源与限制的摘要](evidence/refactor/sdk-benchmark/40880915003-1/summary.json) 已归档并复算原 ZIP SHA256。

默认 both-deflate 热态中位数（老→新）：mymod 316.98→328.16 ms（新慢 3.5%）；Upheaval Armor 子集 317.69→295.75 ms（新快 6.9%）；16 个合成纹理 328.27→335.51 ms（新慢 2.2%）。这三组新 SDK 热态均为零 XML 解析、零资产编译、零完整文件哈希读取。仅新鲜进程 base 编译；小样本、同 VM 配对且 OS 页缓存未控制，新 CLI 包含报告和发布成本；不涵盖完整 Upheaval/BIG 或跨流整体耗时，不声明普遍提速，也不将不同 VM 的历史绝对时间当作优化收益。

首轮 `de95cc20` / [40876849281](https://github.com/wu162/NewSdk/actions/runs/40876849281) 保留 failure 原终态：纹理处理已通过，测试宿主退场审核重复读取已删除清单而失败。修复后在上述成功运行完整重验。此前 BIG 包缓存移除 `7fca1e4f` / [40872280934](https://github.com/wu162/NewSdk/actions/runs/40872280934) 及 StableSort/身份缓存里程碑 `ae4654a8` / [40868109538](https://github.com/wu162/NewSdk/actions/runs/40868109538) 仍保留；BIG/SKUDEF 每次正常打包，通用未变发布和失败/取消回滚继续保留。
