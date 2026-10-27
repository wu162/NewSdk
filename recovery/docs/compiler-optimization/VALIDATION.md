# 阶段 A 验证记录

所有编译、测试和微基准只在 GitHub Actions 执行。本地只编辑、静态检查和读取 Actions 证据。产品来源和文档提交分开记录，不以文档分支头冒充二进制来源。

| 生产/测试源码 | Actions | 状态与范围 |
| --- | --- | --- |
| `18ae05cace7099bbd65fa6632dcfd7dd383bdded` | [40459453132](https://github.com/wu162/NewSdk/actions/runs/40459453132) | 整体 failure；完整生产、摘要、既有 Core、Texture、CLI、输入根、各缓存和 BIG 独立组通过。新 NodeJoiner 差分本身通过，但原 DLL 进入宿主触发归属检查，导致完整 mymod/最终 ZIP 未执行。失败不改写。 |
| `75601909b0e874bc24786f1d6dd3d8c64eb1aee6` | [40461583024](https://github.com/wu162/NewSdk/actions/runs/40461583024) | 整体 success：完整生产、两配置各13个 Core fixtures、原 BAB、协议、Texture、CLI、全部缓存、输入根、完整 mymod 四文件/实际 BIG 原工具差分和最终五文件 ZIP 独立解压运行验收通过。 |
| `ed364467a114a841c4ae3b2ba87629c4c4e851f3` | [40465129025](https://github.com/wu162/NewSdk/actions/runs/40465129025) | 整体 failure；新增 application 计量断言误要求零输入可选组存在 backendCompile 阶段。CLI/协议、全部缓存、输入根、Texture及BIG独立范围成功；mymod与最终ZIP因门槛跳过。失败不改写。 |
| `4da2282f99cae1a35e5c4cd1dfe64023b27c410c` | [40465759765](https://github.com/wu162/NewSdk/actions/runs/40465759765) | 整体 failure；W04 metadata专用fixture及其余独立范围通过，仅同一零输入测试断言失败，mymod与ZIP因门槛跳过。失败历史不改写。 |
| `4a128696c2de04b5151c4dc9f1ac3c35661e7407` | [40466010643](https://github.com/wu162/NewSdk/actions/runs/40466010643) | 整体 success：零任务断言改为实际调用数；生产双配置、各14个Core fixtures、原BAB、协议、Texture、实际CLI、全部缓存、输入根、完整mymod/BIG及独立五文件ZIP全部通过。 |

首轮证据 artifact：`12714925627`，SHA256 `9672e7e862791bac1ecc472c4e0efdadf1055f3d029405b83cfb798109fae593`。风险观察来自其中 Debug/Release `CompilerCoreLifecycle/result.json`，两配置一致。

第二轮提前上传探针 artifact：`12714883356`，SHA256 `47ada239c76a49c849c84a1f91e4b71525e09dabbba710ed3defe5e931f65e38`。收据 succeeded=true；两配置 W01 严格回归全部 equivalent、未变化 override 解析0；NodeJoiner 原 DLL / 默认索引 / 禁用索引差分与归属门槛通过；schema、摘要及生产报告集成通过。

完整成功证据 artifact：`12715561263`（`refactor-core-lifecycle-40461583024-1`），SHA256 `c7e29041e529b068219e4d720e41192becbc61054e8d39f32a51bd98eb5a1dbc`。`artifacts/final-release/40461583024-1/` 含 Debug/Release ZIP；两份 manifest 的 binarySourceCommit 与 packagingSourceCommit 均为上表成功源码。后续文档提交不改变该二进制来源。

| ZIP | SHA256 |
| --- | --- |
| NewModCompiler-Win32-Debug.zip | `7a3c970475cb2f4ff3c195c8e61189d44b2446aee89f0b6dc0ce536a9bd0c0e8` |
| NewModCompiler-Win32-Release.zip | `e494ac0f42f6fc05c963e67790fa7609c9642f5d753d2283a688f70356c4f6f2` |

## 确认的问题与有意修复

这些是在公共 Core 边界确认的结果；不将其扩大为已确认的 CLI 故障。外层 CLI 使用独立流工作区与整输入缓存身份，单独验收。

| 探针 | 首轮真实结果 | 新修复及严格门槛 |
| --- | --- | --- |
| 外部 A=10，父级 `A=$A`，外部改20 | 暖恢复仍为10，冷构建为20，四文件不一致 | 按本地定义安装前的导入值验证 UsedDefines；冷暖值和四文件一致。 |
| 父级 `A=$A+1`，输入未变 | 暖输出正确，但重复解析1份文档 | 同上；未变暖路径解析0，外部改20时结果21。 |
| 两流共享弱引用，第二流新增 Tentative Upgrade | 暖流仅1资产，冷流2资产 | 每流从声明引用重建绑定和闭包；第二流包含 Upgrade，与冷流四文件一致。 |
| 保留有效 .bin，分别删除/截断 .imp/.relo | 六种损坏均恢复为空payload，原payload各8字节 | 按当前有序资产集合验证三文件 checksum 与预期长度，任一损坏重建整组；六种情况均与完整冷构建相同。 |

链接长度/头检查不等于强 payload 校验：长度和 checksum 头均保持时的任意 payload 篡改仍须后续完整 LinkPlan/CAS 凭据。绑定修复针对串行 Core，不代表已完成 W06 不可变结果或并行安全。

## 优化证据与边界

- NodeJoiner 最终80个原 SDK DLL XML/错误差分及卸载 AppDomain 后的宿主 ownership 门槛通过；该 fixture 累计 destination Attributes 访问从549,919降到13,952，禁用索引恢复549,919。此为匹配工作量，不是构建耗时。
- 首轮对4,194,335字节输入交错5次测量，OS页缓存状态 unknown。全量双摘要暖路径中位数：Debug 6.065ms 对 SHA-only 4.849ms；Release 5.754ms 对4.692ms。因额外 CPU 成本，最终策略保留可能未变输入的 SHA-only 检查。
- 首轮冷摘要模拟遗漏历史第二遍中的 SHA256，其耗时不作为最终比较数据。后续模拟已补齐，与新文件、普通变更、暖输入和同时间戳内容变化的正确性探针一起重验。
- schema 描述以当前成功加载代、声明对象和实际类型为边界；有 fallback、上下文隔离、重载失败和同时间同大小内容变化探针。尚无跨流共享 SchemaSnapshot。
- 暂无完整编译器前后 wall-time 对照，不报告端到端提速百分比。首批计量当时未覆盖解析、schema 准备和发布的独立区间；第二轮扩展见下，完整边界见 README。

第二轮修正后摘要微基准，同一 runner/进程内交错5次，输入4,194,335字节，OS页缓存 unknown。单位 ms，数值为中位数 [最小, 最大]；新文件指新的 FileHashItem 观察，不代表冷磁盘。

| 配置 | 历史新文件双遍 | 新策略新文件单遍 | 历史暖 SHA-only | 新策略暖 SHA-only |
| --- | --- | --- | --- | --- |
| Debug | 12.765 [12.639,15.268] | 7.113 [7.034,7.173] | 5.602 [5.557,5.703] | 5.549 [5.521,5.589] |
| Release | 11.953 [11.879,12.287] | 6.619 [6.581,7.043] | 5.399 [5.337,6.708] | 5.535 [5.428,5.694] |

逻辑摘要读取：新文件/已知时间变化从2遍减到1遍；暖输入保持1遍；同时间内容变化保留2遍安全重算。暖耗时两配置有小幅双向波动，不将5个样本解释为统计显著性、P95或全项目性能结论。

## 下一轮可用的真实成本入口

成功运行的 `evidence/refactor/sdk-mymod/40461583024-1/validation.json` 保存完整报告。完整 mymod 两配置均为1个 Core 流、11份 XML 解析、47次资产编译，逻辑摘要13次/86,747字节；同代 schema 描述854次构建、6,860次命中。该样本不足以证明跨流复用收益，后续应结合已有多流 CLI business fixture。

| 当前一次运行 | 报告总 wall ms | assetProcessing ms | validation ms | inheritance ms |
| --- | --- | --- | --- | --- |
| Debug | 845 | 202.127 | 63.706 | 15.346 |
| Release | 713 | 162.036 | 56.252 | 13.694 |

这只说明当前运行的成本，不与旧分支或其他 CI 机器相除求提速。这是首轮八阶段的历史读数，当时尚未覆盖 schema 准备、原始 XML 解析、外层初始化与发布，不把差额全部归因给 schema。第二轮已扩展这些区间；当前下一步以短交接为准。


## 第二轮：W02 成本区间与 W04 构建内元数据

完整成功源码 `4a128696c2de04b5151c4dc9f1ac3c35661e7407` / 40466010643。提前 targeted artifact `12716528944`，SHA256 `e7bec169a810740e985b2e9cc164435891f4f795d21d70fe76f18ee056d1001f`；下载后复核归档 SHA 与收据 source/run 一致。Debug/Release 各14 fixtures 全部通过：Core lifecycle 各15组、解析/schema失败计数和不可变快照、ApplicationBuild零任务与失败取消重试、原有缓存边界与原DLL差分均保留。完整验收随后由同run成功完成；本小包只代表其中targeted范围。

第一批 targeted 失败原因仅为新增 application 测试在空可选组调用 Single 查找不存在的 backendCompile；已有零输入不编译合同正确。修复要求阶段至多1条，调用数之和等于真实 StreamStarted 数（含0），没有放宽生产拒绝或字节断言。初始失败 artifact `12716646191` 的 SHA256 为 `866203ff6b1c5e67adedf554ebfbbd178ee74f9fcbe543f740f35c46a3340ab0`。4da 的新增 metadata 专用 fixture 已独立成功，后续由4a完整重验。

W02 当前新增 rawParse、schema 读/Compile/元数据/内容复核及 outerStages；顶层 applicationWork 快照明确停在 beforeInitialReport。报告/终态日志关闭、Complete及后续回滚仍不纳入本快照；嵌套区间不能相加。schema成功逻辑读取与digest读取分开记录，不等同物理磁盘 I/O。

W04 仅共享构建内自有继承/依赖快照，存储与恢复双向深拷贝。专用 fixture 双配置各91项检查/3组：真实SDK完整表与顺序cold/warm/fallback相等；私有Framework和可变集合隔离；同大小同时间戳include变更、失败不发布/修复命中、countDependencies/文化/root/hashable配置/Clear全部通过。每次仍完整读取815文件、1,515,884字节并Compile，故未实现schema读取或Framework编译复用。

以下为最终候选同一runner/进程内交错3轮的schemaMetadata区间，单位ms，中位数 [最小, 最大]；此区间包含helper绑定、key/查找及冷构建保存或命中恢复，warm表示构建内metadata命中，不表示物理磁盘冷暖。OS页缓存unknown。

| 配置 | cold（新缓存） | warm（恢复） | fallback（禁用共享） |
| --- | --- | --- | --- |
| Debug | 5.540 [3.862,13.208] | 0.988 [0.984,0.993] | 12.458 [3.098,22.917] |
| Release | 4.849 [3.945,13.819] | 0.957 [0.930,0.997] | 3.018 [2.852,14.395] |

完整schema准备中位数cold/warm/fallback分别为Debug 191.345/187.655/185.008ms、Release 154.057/146.906/153.545ms。波动明显，样本少，不能据此报告完整构建提速或P95；确定性成果是命中不再遍历元数据表。冷路径保存及key计算有成本，单流不受益；可用DisableSharedSchemaMetadata回退。


### 三流真实 CLI 基线（共享前，独立成功范围）

W02 `ed364467` / 40465129025 的失败终态不作为完整验收；其中独立 CLI 与原 BAB 严格差分成功，`business-work-metrics.json` 两配置 succeeded=true，sourceCommit=binarySourceCommit=ed364467。最终 artifact `12716599360`，下载归档 SHA256 `f78a9b1514ee4f1254db09dbd34d543f28134ce698da5e7e2c234d3c6a55e238` 与 GitHub 一致。

同一CLI进程的base/APT/maps三流：每配置schemaRead3、schemaCompile3、rawParse3；每流815文件/1,515,884字节，合计2445次逻辑文件读取/4,547,652字节。各独立单组也保留同样计量及严格四输出差分。

| 配置 | report/controller wall ms | schemaRead合计ms | schemaCompile合计ms | schemaMetadata合计ms | outer schemaPreparation合计ms |
| --- | --- | --- | --- | --- | --- |
| Debug | 1095 / 1234 | 528.623 | 103.189 | 28.846 | 668.179 |
| Release | 1088 / 1198 | 537.153 | 113.240 | 28.392 | 685.823 |

schemaMetadata首流约21ms（含首次/JIT），后两流各约4ms。区间嵌套不能相加；controller含进程审计开销，report停在输出最终关闭前；OS页缓存unknown。此为一次基线，不与其他run的时间相除求提速。主导的schema读取/Compile在本轮W04后仍保留，后续应继续处理完整输入封存合同，而不是仅按mtime/size共享摘要。


### 最终三流与完整发行验收

`4a128696` / 40466010643 的 Debug/Release `business-work-metrics.json` 均 succeeded=true，sourceCommit=binarySourceCommit=本轮成功源码。三流均schemaRead3/Compile3/rawParse3、逻辑读2445次/4,547,652字节；metadata为base build1/hit0、APT build0/hit1、maps build0/hit1，合计build1/hit2。三个fresh单组各build1/hit0；原BAB和四输出严格差分均保留并通过。确定性成果是少两次元数据表遍历，未减少schema读/Compile。

完整 artifact `12716723779`（103,927,379字节），下载归档 SHA256 `6ddce03ae8f8313e8d9c5f73e04c70bf33e871a353241079b6a1d9bd89abec1f` 与GitHub一致。完整mymod、实际BIG及release收据均succeeded=true，source/run均为本轮成功身份；两个manifest的binarySourceCommit与packagingSourceCommit同为4a128696。后续文档头不改变来源。

| ZIP | SHA256 |
| --- | --- |
| NewModCompiler-Win32-Debug.zip | `8780c5a1d69d1e29103f298f40afc60b0de17682f853d4d8d708c496463e9751` |
| NewModCompiler-Win32-Release.zip | `939a7c096cb98a1d8a9a55ec788c2d5704e7d44dbabff1bfa661b2965c44399b` |

两ZIP各精确五个运行文件，归档哈希与manifest一致；Actions独立解压后doctor、小MOD与source-runtime原始输出、真实BIG/Additional/LoD/MakeBig、CDATA及回滚/取消验收通过。完整mymod另有同run收据，不把小MOD ZIP验收扩大为完整游戏/G3或中文Texture新验收。
