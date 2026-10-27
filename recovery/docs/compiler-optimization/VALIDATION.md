# 编译器优化：验收与产物身份

所有编译、测试和微基准只在 GitHub Actions 执行；本地只编辑、静态检查和读取已有证据。当前实现合同见 [CONTRACTS](CONTRACTS.md)，续接见 [HANDOFF](HANDOFF.md)。前五轮原始验证、失败与旧产物哈希完整保存在 [历史归档](history/VALIDATION_ROUNDS_1_5.md)，不把旧成功冒充本轮结果。

## 当前完整接受点

| 身份 | 值 |
| --- | --- |
| 生产 / 首次测试 / 打包源码 | `64bfcc80cec7860e69f0ffbc5cb0e04c69baec87` |
| 完整运行 | [40524897974](https://github.com/wu162/NewSdk/actions/runs/40524897974)，attempt 1，整体 success |
| 生产范围 | Debug/Release 各 30 组 Core fixtures；原 DLL；backend/Upheaval/98 资产链三组独立原 BAB；协议、Texture、实际 CLI、全部缓存/输入根、完整 mymod/BIG、独立五文件 ZIP |
| 完整 archive | [`12744579204`](https://github.com/wu162/NewSdk/actions/runs/40524897974/artifacts/12744579204)，111102909字节，官方SHA256 `75e6e675bfcc42c47e90a11cc28a7c23d2b0119bf085f1fa3980576a7384e270` |
| 后续严格复用 / 原 ZIP 导出 | 工具/测试源码 `2bd197f64b5ef1bd9476ac4198e1c3934ed13897` / [40528794946](https://github.com/wu162/NewSdk/actions/runs/40528794946) **进行中**；binarySource仍64bf，sourceRun=40524897974。Ubuntu控制器合同已过，完整复用运行尚不标记接受。 |

生产源码、测试源码、archive、运行文件与 ZIP 身份分别核对。生产 inputs/SDK/schema/recipe/工具链变化必须重建；只允许明确测试/工具/Markdown 的后续变化复用，依赖闭包与祖先关系仍须通过控制器。原 ZIP 只复制，不重打包。

原ZIP小包正由上述严格复用运行导出；完成并核验后更新本节的Debug/Release链接、字节数和SHA。

完整archive仅核对官方元数据，本地未下载大包；严格复用job负责实际archive digest/size、依赖闭包和原ZIP身份验证。最终targeted小证据已本地核验，见下。

## 第六轮实现与回归

- 外部 Cache 物化在原 copy 成功 chunks 上增量 SHA 与规范 1 MiB legacy FastHash，首次 `.asset` 不再额外读取；完整成功才 pin 当前路径并比较真实已消费 handle 身份。失败回滚新增所有权，late fallback 立即关闭未转交 spool；Local 可变输出、CDATA 原 File.Copy、缓存修复和真实 CommitFromCache 取消保留。
- 有效 pin 下原 FileInfo keepalive 失败是实际生产交互，新增 attribute-only、identity 复核的时间戳更新；保留旧 access→write→creation 顺序、只读/未捕获 fallback 与内容写/删除拒绝。
- exact DOM 表达式遍历保留原节点/错误/Define 轨迹；原进程 40 组 DOM corpus 覆盖混合 text/CDATA/comment/PI/entity、custom getter 和 sibling 增删移动，真实 Core parser/traversal 开关组合四输出相同。
- XML hash 的缓冲与 ASCII 尝试均已撤回，当前生产是原 writer；947 行原 DLL 确定性语料保留，512 UTF-16 分块、重复 Final/后续 Write 与确定性 ANSI 边界通过。原进程和恢复版各自产生 raw UTF-16 输入，避免 JSON/UTF8 传输修复 unpaired surrogate；多 pair 造成旧 native 未初始化读取的范围不列为稳定 oracle。
- 符号索引只替代同一次输出绑定的 exact 表查找，真实 98 资产强/弱引用链在两文化、symbol×membership 四组合保持四输出与独立原 BAB 相同。公共 shadow 日志逐 JSON 比较 task/query/parentQuery/provider/order/选择，不省略逻辑查询及影子事件，仅替换候选表查找实现。
- NodeJoiner schema 前序集只存单次插入所需标量，支持标准小组、派生/预算回退和 per-owner 实际计量；公开原 DLL XML/helper 差分保留。
- 私有 saved metadata 共享只以 positive ownership 放行，Current 恢复/recapture/动态 legacy/未知 BinaryFormatter/派生及别名合同通过。其公开 CacheFromDocument 当前无生产调用，组件结果不当作 CLI 收益。

最终 targeted artifact `12743682844` 已下载核对官方 digest、字节数、source/run 与 controller 30/30 结果：542094 字节，SHA256 `a23077b3c842a93289cca003bee44409b0f563faf71248991c53b1cdcbe61139`。原 hash oracle 与两配置 currentLegacy 各947行通过且 `optimizationWithdrawn=true`；表达式40原DOM用例、两个文化的symbol×membership真实链、Upheaval四组合均通过。BinaryMaterialization两配置12组、TransferBuffers两配置3组的 result/runtime-log 哈希与 controller 一致。

## 实际计量

以下均来自最终源码 `64bfcc80` /运行40524897974，同一配置内交错7轮，单位ms，优化/回退中位数：

| 公共组件范围 | Debug | Release |
| --- | --- | --- |
| 表达式遍历769节点、1281属性，100次；含实际遍历，排除fixture构图/比较 | 10.3039 / 13.5721 | 9.9147 / 13.5792 |
| NodeJoiner完整merge，256旧子节点+64插入；排除parse/Validate/XML比较 | 4.8839 / 32.4956 | 4.8350 / 31.3033 |
| Symbol 2048声明、8192次public查询；含scope资格检查、建索引与调用 | 1.7384 / 2.1074 | 1.6288 / 1.9668 |
| Metadata 32×4记录、20次saved传递；不计XML/Current/身份枚举，无生产调用 | 0.0057 / 0.3814 | 0.0052 / 0.3770 |

真实公开Core外部cache三流各配置实际记录：首次tee一次/1048692字节，额外asset captureRead=0，后两次copyReadHits=2，metadata touches=3/失败0；独立public Commit的asset+CDATA touches=2且内容写/删除仍拒绝。取消attempt=1/rollback=1，未进入20次重试。Local无输出排除条件下attempts=0，删除坏文件/修复/新流重试与四输出合同保留；late fallback在scope仍活跃时已立即关闭私有spool。


计时是同 runner/进程内交错样本，无速度断言、P95 或显著性承诺；不包含范围外的 XML 解析/检查时会逐项说明。DOM OuterXml 长度是内容规模，不是精确 writer 输入或字节量。所有 read/tee/hit/capture 计数按 CONTRACTS 理解，不称物理 I/O、完整内存峰值或完整 CLI 提速。

首次接受候选 `30d2ae96` 的早期独立证据仍保留：targeted artifact `12739912354`，472492 字节，SHA256 `6c0b167fc8f0b4fa9c154c16301c679b07b255492d1bceea2aad9159b563e908`。其中宽 schema 256 子节点/64 插入的 7 轮公共 NodeJoiner 中位数 Debug 4.9739/31.8176 ms、Release 4.8192/30.9398 ms（优化/旧算法），只包括 merge；metadata 32×4 记录/20 次公共传递为 0.0061/0.3843、0.0052/0.3691 ms，保留 records 128/2688，不外推生产调用或峰值。

`6ea8ff16` 的早期 targeted artifact `12740316547`：499463 字节，SHA256 `3e64c1b241c20f224d8efc1db0cc75b1d7b27d578f5f81f16d9863069e218ee7`，双配置各 29 组与收据匹配。表达式公共 769 节点/1281 属性×100 次、7 轮中位 Debug 7.477/10.2352 ms、Release 7.0201/9.8991 ms。该次 Cache 首遍仍额外读取 1048692 字节，不冒充后来同流结果。

## 本轮运行与保留失败

| 源码 | Actions | 原终态 / 范围 |
| --- | --- | --- |
| `7664aa36` | [40514462818](https://github.com/wu162/NewSdk/actions/runs/40514462818) | failure；廉价预检错误地给 App 依赖探针只提供 Foundation，未进入长 Native 链接。 |
| `30d2ae96` | [40514912497](https://github.com/wu162/NewSdk/actions/runs/40514912497) | success；双配置 28 fixtures、完整 SDK/缓存/BIG/ZIP；未覆盖后续生产组。 |
| `6ea8ff16` | [40516481094](https://github.com/wu162/NewSdk/actions/runs/40516481094) | success；双配置 29 fixtures、原 DLL/BAB、全部矩阵与 ZIP；Cache 首遍仍多读一次。 |
| `d4c52eb5` | [40519394846](https://github.com/wu162/NewSdk/actions/runs/40519394846) | failure；前置编译捕获 owner probe 平台 enum 命名空间错误。 |
| `13ca8bd5` | [40519455441](https://github.com/wu162/NewSdk/actions/runs/40519455441) | failure；同一新增 probe 又缺 Settings/PluginDescriptor 命名空间。原失败 archive `12741721538`，SHA `554621afc9204e742b63685f5a6d340d881c2de8d9290a8c4f6d74437d80bcd1`。 |
| `9154da61` | [40520099176](https://github.com/wu162/NewSdk/actions/runs/40520099176) | failure；owner probe 未初始化 Settings.Current，另有有效 cache pin 下 TouchCache 失败。 |
| `4682184b` | [40520247547](https://github.com/wu162/NewSdk/actions/runs/40520247547) | failure；两配置 29/30 通过，仅实际 TouchCache 断言失败。hash 原 DLL 919 行通过，但 CPU 计量回归。 |
| `6ac8bc41` | [40521031829](https://github.com/wu162/NewSdk/actions/runs/40521031829) | failure；两配置仍仅 TouchCache 失败，其余同流/短块/身份/取消/late spool 回滚合同通过。原 targeted `12741008520`，539873B，SHA `43f136f8f21d60ed83e3f45c082a798e71a76e5b3de7d31b854f897b75b4abb5`。 |
| `abb72795` | [40523178582](https://github.com/wu162/NewSdk/actions/runs/40523178582) | success；双配置各30及完整矩阵/ZIP，Cache 同流与 metadata touch 通过。ASCII hash 的功能差分通过但性能收益不稳，随后撤回。targeted `12742099696`，543920B，SHA `601118f54e8e714914270439345da1f5ef41cc64d4d365ce38a783b5688ec143`。 |
| `64bfcc80cec7860e69f0ffbc5cb0e04c69baec87` | [40524897974](https://github.com/wu162/NewSdk/actions/runs/40524897974) | success；生产 HashingWriter 恢复原实现，947 原 DLL corpus 和最终完整矩阵/ZIP 通过。 |

HashingWriter 初版没有 CPU 收益：`4682184b /40520247547` 中 Release 的 mixed DOM OuterXml 2516 字符×512 为 6.5425/5.7944 ms，148895 字符×32 为 19.5179/18.7273 ms，小 274 字符为 1.3922/1.3758 ms（新/旧）。919 行原语义通过不能替代性能收益；随后 `abb72795 /40523178582` 的 ASCII 专门路径仍在 Debug 小/中/大、Release 小/中及 mixed 多组回归，仅 Release 大 ASCII 约 2.5% 小幅改善；最终恢复原 HashingWriter，未把推测的分配减少当作 CPU 收益。

失败 archive 与独立成功范围按各自 source/run 保存，未取消或改写原终态。基础 `30d2ae96 /40514912497` 和 `6ea8ff16 /40516481094` 均完整 success，但不覆盖后来新增生产对象。

## 效率与续接

按用户约110分钟目标推进，使用2个子代理；10个完整候选中4个success、6个failure，另有工具合同与严格复用运行。完成四处实际生产快路径和W06私有saved共享；没有完整CLI前后计量，不给端到端提速百分比。

效率评为中等：前置预检挡住了3次长Native链接，但新增owner探针的命名空间与Settings.Current未一次核齐，出现4次可避免准备错误。真实cache元数据交互和late spool预算缺陷按原断言修复；两次hash尝试测得不稳定CPU收益后撤回，没有拿推测的分配减少抵消回归。没有可靠逐项工时记录，不编造实现/等待比例。下一轮应一次核齐fixture依赖/全局初始化/规范路径，再批量前置编译，继续让调查和工具服务完整生产功能。

本轮结束只更新当前接受点和未决工作；后续完整语义结果、Texture/其它读取闭包、新 bundle 缓存与并行见短交接。不把本轮局部优化写成这些能力已完成，也不自行合并 PR。
