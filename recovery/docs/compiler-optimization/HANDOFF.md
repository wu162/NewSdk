# 编译器优化：当前短交接

更新：2026-10-28（北京时间）。用户要求先交接，已停止新增实现/验证。成果集中`feature/compiler-optimization`；[PR #15](https://github.com/wu162/NewSdk/pull/15)保持草稿，不自行合并、不强推。通常2个子代理、最多3个；所有编译/测试/微基准只用Actions。[合同](CONTRACTS.md)、[详细证据](VALIDATION.md)。

## 接受点与当前源码

- **最新完整接受：`71b5a0a4f55e530eccc9703eff2f3c3a397dd910` / [40598874896](https://github.com/wu162/NewSdk/actions/runs/40598874896)，整体success。** 双配置各30组、原DLL/独立原BAB、协议/Texture/真实CLI、全部cache/输入根、完整mymod/BIG和五文件ZIP通过。本轮此前四组完整运行也success。
- **后续生产候选：`6db109aff316e21258485f49eaf361d96c790467` / [40602153873](https://github.com/wu162/NewSdk/actions/runs/40602153873)。** Core双配置各29/30通过，唯一失败为Metadata新增恢复原语探针的错误期待；整个候选不能标完整接受。交接时其独立CLI/cache/SDK组仍在运行，未取消，后续先查终态。
- 上一候选`e4169602 / 40600682680`已整体failure：DOM事件未arm、失败DOM空根两个fixture错误。本候选已实证修好这两项；历史失败保留。
- 严格targeted/CLI复用工具仍pin`64bf/40524897974`，不能覆盖当前生产差异；当前候选失败，**不要repin到它或用旧对象跳过新生产验证**。包的官方身份、已核Core小证据与历史Release下载见VALIDATION。

## 下一轮先做

1. 读AGENTS、本页及VALIDATION，查`40602153873`终态；原/恢复证据已下载到`work/core-probes-40602153873.zip`（artifact`12778904826`，SHA256`0ed021507a6310e8cc1adde1879c6b981c16de3c8d627d6046bae9adc9aaf94e`，官方/本地一致）。
2. 修`recovery/tests/CompilerDocumentMetadata.cs`约317行的错误期待。现在要求Reader恢复后没有Valid节点，实测validated-Core为9→9、processed-Core为0→0、prefix/entity为4→4。改成守住实际保留行为的正断言，保留严格原DLL逐行比较、默认Specified/BaseURI/DOM隔离和schema别名/环境缺失门槛；不要只删失败断言。
3. 仅修测试后用Actions集中重验。当前生产仍缺完整接受；现有复用控制器只接受完整success来源，不能把29项成功冒充整批接受。最终success后再更新targeted pin及runner对应literal、核原ZIP身份、同步PR/短交接，不合并。

## 本轮能力与实际证明

| 范围 | 状态 |
| --- | --- |
| live继承/DFS | 已完整接受：按引用visiting/complete、4096前向链、拒环/修复重试、legacy顺序与schema默认来源；不从零hash判断阶段，不授权语义缓存。 |
| 外部Cache | 已完整接受：header封存游标、实际存在查询/终验、自身写删及失败修复、case/junction有界别名撤销；已有冲突保留，Local/Output/staging原角色不变。 |
| 读取器/插件 | 已完整接受：APM reader确定收尾、真实SHA故障/同item重试、Texture/Builtin所属Thread门禁；保留原字节及关闭错误。 |
| 能力声明 | 已完整接受：实际路由immutable标量，消费于legacy缓存只否决、session拒绝与opaque输入原因；未知不增长map，强bundle/并行始终false。 |
| Define/资源候选 | 当前双配置已通过：52/52 Define回调真实changed、40 traversal中16 live-sibling真实mutated及严格原差分；有界final-expression索引、成功/失败finally释放；XML/text获取/关闭、三文件立即exclusive/修复重读/事件退订、摘要回退取消、8MiB idle Chunk池。仍受整批接受门槛约束。 |

Define组件7轮4096读的wall中位数：Release默认4.8171/回退7.8925ms（Debug4.8930/7.9456ms），只代表组件。此前实际三流小CLI成对计量没有可靠端到端提速证据，不外推完整MOD收益。

## 明确剩余边界

- 完整ASR/Define环境仍未冻结。三行原DLL实证推翻了“Reader必丢Validity”“Clone必改变默认Specified/legacy hash”的先前推导；当前实际Reader/Clone保留这些项目。仍实证存在XmlSchema对象别名与IsMixed共享突变、Schemas数量815→0/815→0/1→0；saved恢复也缺DOM阶段/完整Define输入，不能跳过PrevalidatedLoad重算门槛。
- parse=text仍走原GetResource，绕过Core resolver Paths/PhysicalInputSnapshot；外部file/http文本的legacy缓存覆盖尚未证明。Texture隐藏闭包、新强键bundle与并行未完成，`completeClosure=false`，不由LegacyAllowed或部分轨迹放行强缓存。
