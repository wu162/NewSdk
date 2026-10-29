# 热缓存、StableSort 与插件校验：续接入口

工作分支仍为 `recovery/legacy-sdk-cache`，禁止强推，未合入其他分支。

用户第 1–7 项及第 8 项有界清理已经实现并通过完整 Windows 验收。生产二进制与测试来源均为 `ae4654a892446724bf870348e56f2e8072fafa58`；[40868109538](https://github.com/wu162/NewSdk/actions/runs/40868109538) success，Debug/Release、Core/后端/应用、真实 CLI/冷进程/取消、资产与会话缓存冷热/失效/缺损修复、Texture/Audio/W3X、BIG/协议、完整 mymod 原版差分及五文件发行包全部通过。[原始收据与失败记录摘要](evidence/refactor/hot-cache-second/40868109538-1/summary.json)，[实现说明](docs/LEGACY_INCREMENTAL_CACHE.md)。原归档已下载，复算摘要与 GitHub 元数据及上传日志一致。

文件身份优先，Schema 跨进程复用既有哈希收据；两配置热态为零 XML 解析、零资产编译、零完整文件 SHA256 读取，837 次哈希收据复用、2 个链接流复用，`publicationUnchanged=true`。StableSort 每配置 66 组原算法顺序对照通过；1024 独立资产样本的 schema 比较器调用 523,776→1,023。只报告排序辅助函数样本，未声明端到端 SDK 提速比例；基础流前缀、文化相等键、类型哈希碰撞沿用原规则。

第 8 项先清理已核实的 LogicCommandSet 重复结构遍历：Core 校验完成后提供当前 XML 的只读收据，加载新 XML/校验失败时撤销；独立未校验入口维持原拒绝合同。编译器恢复范围、布局、语义和外部载荷校验继续保留，未笼统删除其它组件检查。真实 Core 已验证三缓冲区相等及非法 XSD 在插件调用前拒绝。

用户已明确第 9 项为移除整个 BIG/SKUDEF 包缓存。本提交删除包指纹、`package|v1` 收据读写和旧包复用；每次从本轮四流/CDATAs 执行 LoD、Additional 与 MakeBig，通用未变发布跳过和失败/取消回滚保留。附带删除仅供包缓存使用的 Published 标志及 TryHash 查询。新增重复打包、忽略/清退旧收据与包阶段 staging 来源检查。**本提交生产/测试已变，Windows 验证中；上面的成功结果仅代表移除前版本。**

前六项独立里程碑 `22133880` / [40863166008](https://github.com/wu162/NewSdk/actions/runs/40863166008) 也已完整通过。Schema 时钟刻度用例及 StableSort 合成类型哈希碰撞的早期失败原终态均保留在摘要中，没有放宽顺序或原字节门槛。
