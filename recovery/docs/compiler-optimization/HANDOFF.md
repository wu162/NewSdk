# 编译器优化：当前交接

更新：2026-10-27，北京时间；本轮窗口15:01—16:01，使用2个子代理。
成果分支：`feature/compiler-optimization`，起点 `compiler-main@e7396d3`。
执行约定与方案读取索引：[README.md](README.md)。

## 当前状态：W02 扩展与 W04 构建内元数据共享验证中

- 既有 W01 三项真实 Core 修复、W03 单遍双摘要、W08 NodeJoiner 局部索引、W04 同代引用描述已在源码 `75601909` / [40461583024](https://github.com/wu162/NewSdk/actions/runs/40461583024) 完整验收；准确范围与微基准保留在 [VALIDATION.md](VALIDATION.md)。
- 本轮 W02：补 rawParse、schemaRead/Compile/Metadata/ContentCheck、逻辑 schema 读取与外层 settings/provider/cache/session/清理等区间；顶层 applicationWork 覆盖规划、backend、artifact 校验、包装、发布和缓存 Commit。不可变快照保留失败调用，明确 beforeInitialReport 边界与嵌套关系，不宣称全部 wall 成本已覆盖。
- 本轮 W04 子组：每 build 单 entry MetadataCache，仅保存自有继承/依赖值、双向深拷贝，保留每流独立 XmlSchemaSet 全文读取/Compile/helper 绑定及整文档验证。键含完整有序内容闭包、根路径、依赖模式、文化及 hashable 配置；提供 DisableSharedSchemaMetadata 回退。尚无跨流 Framework schema 或引用描述共享。
- 新专用 fixture 验证完整 SDK 表/顺序 cold-warm-fallback、可变集合隔离、同大小同时间戳 include 变更、失败不发布、模式/文化/根/配置隔离及 Clear；三流 business 继续严格原 BAB 与四输出差分，并记录逐任务读/编译/元数据 build/hit。

## 验证中，不能标完成

所有编译、测试和微基准仅在 Actions；本地只编辑、静态检查及读取证据。

| 源码 | Actions | 当前证据 |
| --- | --- | --- |
| `ed364467a114a841c4ae3b2ba87629c4c4e851f3` | [40465129025](https://github.com/wu162/NewSdk/actions/runs/40465129025) | W02 首批。两配置新增 application fixture 断言误要求空可选组存在 backendCompile 指标而失败；其它独立组继续。失败保留。 |
| `4da2282f99cae1a35e5c4cd1dfe64023b27c410c` | [40465759765](https://github.com/wu162/NewSdk/actions/runs/40465759765) | W04 生产/测试源码；运行中，仍带上述测试断言问题。 |
| `4a128696c2de04b5151c4dc9f1ac3c35661e7407` | [40466010643](https://github.com/wu162/NewSdk/actions/runs/40466010643) | 修正零任务实际调用数断言，生产输入与上一行相同；两配置各14个 targeted fixtures、原 BAB 和协议通过，CLI/缓存/mymod/BIG/ZIP继续验收。 |

继续低频跟进到终态，下载 targeted 小证据及最终 business-work-metrics、mymod/BIG/ZIP 收据，核对 source/run；更新本页与 VALIDATION，不能用文档头替代二进制来源。两次旧运行均保留，不取消或改写。

## 下一步与边界

先以三流实际计量核实 W04 是否减少表遍历及其成本，不以不同 CI runner 时长相除报告提速。W04 本组不减少 schema 读取/Compile，完整 mymod 仍仅1流。
W03 不能仅加 path→digest 字典：Reset 的强内容复核、同时间同长度变更、XML/XInclude/payload 消费同版本、cacheoff 发布前变化拒绝及快照跨 Dispose/发布生命周期必须成组处理；未实现输入封存与跨流读取去重。W05 影子依赖、W06 不可变结果、W09—10 并行尚未完成。
后续成果仍集中本分支；[PR #15](https://github.com/wu162/NewSdk/pull/15) 为草稿，不自行合入。每轮最多60分钟、通常2个子代理（最多3个）；只改文档不重编译。
