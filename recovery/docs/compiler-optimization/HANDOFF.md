# 编译器优化：当前短交接

更新：2026-10-28（北京时间）。成果集中 `feature/compiler-optimization`；[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，不强推、不自行合并。通常2个子代理、最多3个；所有编译/测试/微基准只用Actions。导航：[README](README.md)、[CONTRACTS](CONTRACTS.md)、[VALIDATION](VALIDATION.md)。

## 最新接受点与候选

- **完整接受：`5727b05b234c130a5c26e5c465a31f8ff42baf22` / [40593235622](https://github.com/wu162/NewSdk/actions/runs/40593235622)，整体 success。** Debug/Release 各30组 Core、原 DLL/三组原 BAB、协议、Texture、实际 CLI、全部缓存/输入根、完整 mymod/BIG 和独立五文件 ZIP 通过。原 archive 与已核下载的 Core/CLI 小证据身份见 VALIDATION。
- **后续仍在验证：** `c3ef5344 / 40594750754` 的 cache 大小写/junction 别名与9行原 DLL 继承差分；`07ff6a3b / 40595750142` 的摘要读取器所有权。它们尚不能继承上一个接受点的生产结论。
- Texture/Builtin owner-thread 合同组随本候选进入完整验收；精确运行状态由 VALIDATION 更新。旧严格复用工具仍 pin `64bf/40524897974`，不能用于跳过本轮生产变化；最后完整接受后再集中 repin。

## 本轮新增

| 能力 | 实际边界 |
| --- | --- |
| 继承阶段 | 显式 DFS、按引用 active 集与 live DOM readiness，拒环且支持4096前向链；原 lookup/merge/hash、重复调用、schema 默认值/SchemaInfo/来源和主错误顺序保留。readiness 不从 legacy hash 推断、不从 saved 声明恢复，不授权语义缓存。 |
| 外部 Cache | 恢复 header 复用已封存来源；原坏长度删除/拒绝保留。记录实际存在查询并终验，成功自身写删推进预期；可选写失败撤销对应证明并 incomplete，已有冲突保留。真实 cold→warm→warm 四输出与取消通过。 |
| 别名候选 | 后续候选使用同 leaf 有界桶、case/junction 保守证明撤销；native parent 查询每次16/每代256，失败或预算不足 incomplete，不把路径直接大小写合并。 |
| 摘要读取器 | 后续候选为构造/读取/消费者异常及提前退出确定收尾；pending I/O 未完成的 buffer 不归池。保留同 Chunk、实际短块、活 FileSize 与 EOF 形态；不宣称正常 APM 事件泄漏或底层 OS read 可取消。 |
| 插件生命周期 | 待验收组在 Texture 活跃入口和 Builtin 聚合 Registry/ReInitialize/Dispose 检查 owner，拒绝发生在 native/聚合终态之前。不是所有插件已可重入，也未开放并行。 |

既有 exact DOM/表达式程序、符号/NodeJoiner/manifest/StableSort 局部索引、schema 自有元数据及 saved 声明隔离保持；详细边界集中于 CONTRACTS，不把历史组件收益当完整 CLI 收益。`CacheFromDocument` 元数据共享仍无生产调用收益证据。

## 计量与未完成

第一次实际 CLI 同 Release EXE/三流小输入、各一次 warmup 后交错7对，全部十二输出逐次字节相同。默认/诊断回退的 wall 中位数 **1255.3308/1234.7419ms**，CPU **1218.75/1187.5ms**；未观察到这份小用例端到端提速，不宣称显著回归或完整 MOD 收益，原样本/CPU粒度/计时边界见 VALIDATION。

1. 完整 AssetSemanticResult、processed XML/default/type/source 冻结与 Define 环境仍未完成；不要用 CloneNode、旧 XML hash 或部分轨迹放行强缓存。
2. Texture 隐藏资源仍未闭合；BinaryAsset Local/Output 可变角色和 App staging 排除继续保留，`completeClosure=false`。原字段只证明本代内容，不证明旧 cache 编码语义。
3. 新强键 bundle 缓存、完整插件资源/状态合同和并行仍未完成。继续保留原字节、拒绝、冷热/持久缓存、失败取消/事务、完整 mymod/BIG、独立 ZIP 门槛。
4. 跟完全部当前 Actions，再按生产/测试/打包来源和原 ZIP 精确成员核验更新短交接；有可靠完整对象后才复用。普通推送本分支，合入须用户批准。
