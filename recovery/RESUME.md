# 本轮四项优化：实现完成，Windows 验收待跑

工作分支仍为 `recovery/legacy-sdk-cache`；本轮未合入其他分支。

2026-10-29：已实现请求内 SchemaSet 跨流复用，删除每流 Schema 闭包复查与配置加载中的重复存在性检查；Schema 加载复用既有目录身份快照，不再在完成时重扫。FileHashItem 由宿主一次提供存在性、时间戳和已有 SHA256 收据，变化输入的 FastHash/SHA256 合并为一次读取，等长恢复 mtime、缺失/重建及旧会话升级合同保留。Texture 运行时部署审核已删除，不再读取 patch 清单、哈希 EXE/DLL 或枚举旧程序集；构建时 mixed PE 验收和原版纹理载荷差分继续保留。

已完成差异检查及 24 项 benchmark 脚本预检。生产二进制改变，以下历史验收不能替代本轮结果；本轮须集中运行完整 Core lifecycle（含 FileHashItem 宿主收据/单次读取、跨流 Schema 及无部署清单纹理用例），通过后使用该次 Release 做有严格时间预算的新旧 SDK benchmark。尚未宣称生产或 benchmark 通过。

用户第 1–7 项及第 8 项有界清理已经实现并通过完整 Windows 验收。生产二进制与测试来源均为 `ae4654a892446724bf870348e56f2e8072fafa58`；[40868109538](https://github.com/wu162/NewSdk/actions/runs/40868109538) success，Debug/Release、Core/后端/应用、真实 CLI/冷进程/取消、资产与会话缓存冷热/失效/缺损修复、Texture/Audio/W3X、BIG/协议、完整 mymod 原版差分及五文件发行包全部通过。[原始收据与失败记录摘要](evidence/refactor/hot-cache-second/40868109538-1/summary.json)，[实现说明](docs/LEGACY_INCREMENTAL_CACHE.md)。原归档已下载，复算摘要与 GitHub 元数据及上传日志一致。

文件身份优先，Schema 跨进程复用既有哈希收据；两配置热态为零 XML 解析、零资产编译、零完整文件 SHA256 读取，837 次哈希收据复用、2 个链接流复用，`publicationUnchanged=true`。StableSort 每配置 66 组原算法顺序对照通过；1024 独立资产样本的 schema 比较器调用 523,776→1,023。只报告排序辅助函数样本，未声明端到端 SDK 提速比例；基础流前缀、文化相等键、类型哈希碰撞沿用原规则。

第 8 项先清理已核实的 LogicCommandSet 重复结构遍历：Core 校验完成后提供当前 XML 的只读收据，加载新 XML/校验失败时撤销；独立未校验入口维持原拒绝合同。编译器恢复范围、布局、语义和外部载荷校验继续保留，未笼统删除其它组件检查。真实 Core 已验证三缓冲区相等及非法 XSD 在插件调用前拒绝。

用户已明确第 9 项为移除整个 BIG/SKUDEF 包缓存。生产与测试来源均为 `7fca1e4f38c304bfc107e5301b1b72b70cbfacea`；[40872280934](https://github.com/wu162/NewSdk/actions/runs/40872280934) 完整 Windows 验收 success，前述生产矩阵、完整 mymod 原版差分和五文件发行包均再次通过。[本次验收摘要](evidence/refactor/big-cache-removal/40872280934-1/summary.json) 已核对原归档摘要。包指纹、`package|v1` 收据读写和旧包复用已删除；每次从本轮四流/CDATAs 执行 LoD、Additional 与 MakeBig，通用未变发布跳过和失败/取消回滚保留。仅供包缓存使用的 Published 标志及 TryHash 查询一并删除。应用测试通过重复打包、忽略/清退损坏旧收据及未变发布；生产 BIG 每配置 23 个用例、13 组原版对比通过，14 个成功打包用例的原事件均确认两产物来自本轮 staging。

前六项独立里程碑 `22133880` / [40863166008](https://github.com/wu162/NewSdk/actions/runs/40863166008) 也已完整通过。Schema 时钟刻度用例及 StableSort 合成类型哈希碰撞的早期失败原终态均保留在摘要中，没有放宽顺序或原字节门槛。
