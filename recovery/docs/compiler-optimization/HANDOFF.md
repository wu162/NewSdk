# 编译器优化：当前短交接

更新：2026-10-28（北京时间）。成果集中 `feature/compiler-optimization`；[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，不强推、不自行合并。通常2个子代理、最多3个；所有编译/测试/微基准只用Actions。导航：[README](README.md)、[CONTRACTS](CONTRACTS.md)、[VALIDATION](VALIDATION.md)。

## 最新接受点与候选

- **完整接受：`15bcd14c97c475d6922cb367989a4fc04d8ba7bb` / [40596715818](https://github.com/wu162/NewSdk/actions/runs/40596715818)，整体 success。** Debug/Release 各30组、原DLL/独立原BAB、协议、Texture、实际CLI/cache/输入根、完整mymod/BIG及五文件ZIP通过。此前`5727b05b`、`c3ef5344`、`07ff6a3b`三组完整运行也全部success。
- **后续验证中：`71b5a0a4 / 40598874896`**，路由能力声明实际消费于legacy缓存否决、会话拒绝与输入跟踪；追加6行独立原reader/digest语料。不能继承旧生产结论。
- **后续未验证：** `b7fe0363`保存Chunk idle池8MiB上限/原子归还，等待与下组统一验证。完整语义结果/Define环境仍未冻结；下一组只在依赖满足时推进。
- 独立Release包已下载核对官方transport SHA、原ZIP、精确五成员及Texture→EXE绑定；Debug当前只有官方身份。完整下载身份见VALIDATION。严格复用仍pin`64bf/40524897974`，最后完整接受后再集中更新。

## 本轮新增

| 能力 | 实际边界 |
| --- | --- |
| 继承阶段 | 显式 DFS、按引用 active 集与 live DOM readiness，拒环且支持4096前向链；原 lookup/merge/hash、重复调用、schema 默认值/SchemaInfo/来源和主错误顺序保留。readiness 不从 legacy hash 推断、不从 saved 声明恢复，不授权语义缓存。 |
| 外部 Cache | 恢复 header 复用已封存来源；原坏长度删除/拒绝保留。记录实际存在查询并终验，成功自身写删推进预期；可选写失败撤销对应证明并 incomplete，已有冲突保留。真实 cold→warm→warm 四输出与取消通过。 |
| Cache别名 | 已接受组使用同 leaf 有界桶、case/junction 保守证明撤销；native parent 查询每次16/每代256，失败或预算不足 incomplete，不把路径直接大小写合并。 |
| 摘要读取器 | 已接受组为构造/读取/消费者异常及提前退出确定收尾；pending I/O 未完成的 buffer 不归池。保留同 Chunk、实际短块、活 FileSize 与 EOF 形态；不宣称正常 APM 事件泄漏或底层 OS read 可取消。 |
| 插件生命周期 | 已接受组在 Texture 活跃入口和 Builtin 聚合 Registry/ReInitialize/Dispose 检查 owner，拒绝发生在 native/聚合终态之前。不是所有插件已可重入，也未开放并行。 |

既有 exact DOM/表达式程序、符号/NodeJoiner/manifest/StableSort 局部索引、schema 自有元数据及 saved 声明隔离保持；详细边界集中于 CONTRACTS，不把历史组件收益当完整 CLI 收益。`CacheFromDocument` 元数据共享仍无生产调用收益证据。

## 计量与未完成

第一次实际 CLI 同 Release EXE/三流小输入、各一次 warmup 后交错7对，全部十二输出逐次字节相同。默认/诊断回退的 wall 中位数 **1255.3308/1234.7419ms**，CPU **1218.75/1187.5ms**；未观察到这份小用例端到端提速，不宣称显著回归或完整 MOD 收益，原样本/CPU粒度/计时边界见 VALIDATION。

1. 完整 AssetSemanticResult、processed XML/default/type/source 冻结与 Define 环境仍未完成；不要用 CloneNode、旧 XML hash 或部分轨迹放行强缓存。
2. Texture 隐藏资源仍未闭合；BinaryAsset Local/Output 可变角色和 App staging 排除继续保留，`completeClosure=false`。原字段只证明本代内容，不证明旧 cache 编码语义。
3. 新强键 bundle 缓存、完整插件资源/状态合同和并行仍未完成。继续保留原字节、拒绝、冷热/持久缓存、失败取消/事务、完整 mymod/BIG、独立 ZIP 门槛。
4. 跟完能力声明候选及下一组Actions，再更新最终source/run及严格复用pin。新生产输入必须完整重建；测试/工具仅在闭包不变且严格校验后复用。普通推送本分支，合入须用户批准。
