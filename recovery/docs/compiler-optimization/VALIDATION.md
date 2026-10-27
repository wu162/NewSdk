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


## 第三轮：正内容封存、resolver影子记录与基准匹配索引

执行从北京时间15:56起，按用户最新要求推进接近90分钟，2个子代理。全部编译、测试仅在Actions，本地仅编辑、静态检查和读取既有证据。

| 构建/测试源码 | Actions | 结果 |
| --- | --- | --- |
| `21cd74f98b6dd01d3d41d02e730d34de44eac4b2` | [40472165263](https://github.com/wu162/NewSdk/actions/runs/40472165263) | failure；CLI收据新加重复PowerShell key，预检拒绝，未进入长Native构建；普通修复保留原终态。 |
| `0df7c92f4fb54d238b3dae588787f56bdb9cc230` | [40472198342](https://github.com/wu162/NewSdk/actions/runs/40472198342) | failure；旧Backend逐流reads>0假设、同backend失败修复被旧pin阻挡、ApplicationBuild漏System.Xml引用，以及真实CLI/BIG取消被ReadingSchema包装。无result的编译失败也按失败处理。各缓存/payload等独立成功不替代完整验收。 |
| `b88240bc389f945191695118631e6abdef4d7d3e` | [40472999771](https://github.com/wu162/NewSdk/actions/runs/40472999771) | failure；新增W05专用用例双配置204检查/6组独立成功，仍受上述旧失败影响。 |
| `68f38abb1ff649857ff786aa8a6bcf657553b8da` | [40476134103](https://github.com/wu162/NewSdk/actions/runs/40476134103) | 整体success；生产双配置、各17个Core fixtures、原BAB、协议、Texture、实际CLI/CTRL_BREAK、全部缓存、输入根、完整mymod/BIG和独立五文件ZIP全部通过。 |

取消修复保留OperationCanceledException和原token，schema及XInclude不再把它转成ReadingSchema/resource错误或选择xi:fallback；真实CLI与BIG的CTRL_BREAK均重验exit6。失败Compile结束整个封存generation，原same-backend输入修复/重试合同恢复；新owner的snapshotId明确变代，重试强比较Initialize配置SHA。Backend计量要求零逐流摘要读必须由实际sealed hits解释、同代单调且跨代重置、旧快照不变，未简单删掉读工作断言。

最新targeted artifact `12720638360`，SHA256 `02ccaf49d791ccac27329571b21866df6bd130bc987fb486d26b5117e7463cca`。完整validation逐项核对17/17、结果文件与构建日志，双配置均通过；PhysicalInputs100检查/5组，DependencyObservations204/6组，BaseManifestIndex38例/135检查/4组，ApplicationBuild4155检查/9组（含真实native APT DataBlob与XInclude发布边界），BackendLifecycle8010/6组及InputRoots80/6组均通过。

### 实际三流的正内容工作量

Debug/Release的business收据均succeeded=true，binarySourceCommit=sourceCommit=68f38abb。每配置实际capture822次/1,529,741字节，sealed hits1633次；发布前强复核822次/1,529,741字节，bypass0、cleanupFailures0。beforeInitialReport快照仍持822文件pin及47目录路径pin；实际Run返回后的释放由应用成功/取消/失败/重试用例验证。

schema逻辑消费仍为2445次/4,547,652字节并逐流Compile，后续读取由封存字节满足；capture/verify是原文件逻辑I/O，不等同物理磁盘读，不能跨CI求提速。新快照单遍强/legacy摘要保持0、1及1MiB±边界原算法差分；同时间同长度的新generation变化、有限内存/磁盘/文件预算、不支持路径回退及取消已验证。positiveInputsOnly不表示全部依赖闭包，opaque Texture、precompiled/reference manifest及未接入查询仍保守。

W05真实business每配置记录48次resolve、candidateChecks0、dropped0。这些FileReference是直接相对文件名，按旧resolver本就不做候选Exists；新观察器不额外探测。多根/postfix/wildcard/缺失与候选新增false→true等负查询由专用fixture验证，影子记录canAuthorizeReuse=false，不授权跳过或缓存命中。

### 基准manifest子组的原DLL三方对照

原SDK Core DLL SHA256 `b36d5015f97532457d64072034f9f5c629a4c060a2d34c49c4f89fd6e6f79e47`。原DLL在可卸载AppDomain中，38例默认索引/禁用索引/原SDK结果、顺序、错误及异常部分状态一致；卸载后宿主ownership门槛通过。含重复Handle/base、patched首项保留、空/非法候选/别名列表、真实trace修改集合或Handle及异常等边界。

512项倒序组两配置相同：默认512次索引查找、5121次秩操作，旧candidate/removal两类线性扫描各131,328次均降为0。List2内部数据搬移仍保留；任何实际trace回调后当前remove与后续lookup全部legacy，默认/tl3的放行范围保守。该fixture验证base初始化helper，不扩大为完整patch布局、CLI新patch功能或端到端提速。

### 完整发行身份

完整artifact `12720508892`，104,702,620字节，下载归档SHA256 `7b5e0c78444ac6937f9adfd1328f93bdcb60a53ec45be19dca40bb14070186d9` 与GitHub一致。完整mymod、BIG及release收据均success且source/run一致，两个manifest的binarySourceCommit/packagingSourceCommit均为68f38abb。

| ZIP | SHA256 |
| --- | --- |
| NewModCompiler-Win32-Debug.zip | `4deb40b17e63193f1840641998bb6a500567d35cbbb1c884dc974878255f840b` |
| NewModCompiler-Win32-Release.zip | `4a579c09d991fe3ccbc6936ce0768b5b8ad4b080968aabd043865d07277d0a62` |

两份ZIP均精确五运行文件，下载后哈希与manifest相同。独立解压后的doctor、小MOD原始输出、BIG/Additional/LoD/MakeBig、CDATA及事务回滚/取消验收通过；完整mymod另有同run严格收据，不宣称新增游戏内/G3或中文Texture验收。

### 测试补充与严格复用：发现硬链接路径缺口

补测源码`4a67f452b41d182e4c9c10a9d8c2930b1bcc8ea9` / [40478217177](https://github.com/wu162/NewSdk/actions/runs/40478217177)仅改测试/controller与严格复用工具，生产源码/SDK/recipes未变。新补同卷hardlink/大小写alias身份共享、同字节不同inode不得合并，以及派生Equals主动改list/抛异常的旧固定扫描count与partial-state。二进制来自上述完整success的68f38abb。首个短运行40478217177复用门槛通过，但两新增fixture编译失败（未确定赋值/局部变量重名），其他15项通过；原失败保留，普通修复为55f1a519。

复用拒绝非祖先、src/SDK/recipe/其他test改动，核对固定run/attempt/repository/branch/workflow、archive SHA/size、安全ZIP、双配置生产与validation EXE/runtime/Texture绑定及仅入口对象不同的生产清单。原receipts不改写，reuse.json和validation分开记录binary/test/sourceRun身份。未来生产改动不能直接沿用本固定归档。


修正补测源码 `55f1a519c8a140f5c2988c138f15bc8854d68905` / [40479104375](https://github.com/wu162/NewSdk/actions/runs/40479104375) 整体failure：双配置各16/17，所有fixture构建已成功，每项结果文件/来源逐一核对。BaseManifestIndex新增派生Equals改list/抛错与null-final显式断言通过，双配置40例/147检查/5组；实际保持原Remove固定32次检查及异常先final.Add后移除的部分状态。

唯一新真实失败是hardlink alias unlink：同文件ID的capture/hash共享正确、两个名字写入均被阻断，但第二名称可被File.Delete删除。原实现identity命中后释放别名handle；同inode的第一handle不阻止另一硬链接目录项unlink。父目录pin不阻子名称替换，末端Verify不足以保护此前native原路径消费窗口。不能删除/放宽该断言，也不能把68完整成功外推成硬链接闭包已通过。证据artifact `12722940233`，SHA256 `a2cb2bd385bf1d5f25ada550d65094d1a99e9aa1adbb069580647a4be8ea4e5b`；初次编译失败补测artifact `12722045816`，SHA256 `b84ea2f03293facec30ad7d3312ee5630a9869423cd317d7e39efdd8c727dbbc`。

最新生产修复 `c65761fed6eed5ad71512e1cea2271eb49a4163f` 保留每个新alias路径的只读/禁delete句柄，仅共享immutable内容及双摘要；Dispose逐一关闭全部alias handles，仍受路径预算约束。activeFilePins保持unique内容owner口径，新增activeAliasPins/activeReadHandles区分实际资源数。该修复已独立静态复核，不等于运行验收。

生产输入已改变，停止沿用68二进制复用，已触发[完整重建40479845571](https://github.com/wu162/NewSdk/actions/runs/40479845571)。本轮接近90分钟交接时仍在运行，未标通过；下一轮首先跟进此run到终态，核对hardlink unlink/其它17个fixture及全部CLI/缓存/mymod/BIG/ZIP，再更新准确二进制来源。原68发行包带已知hardlink alias边界，不把其成功当本修复的验收。

## 第四轮：manifest、消费轨迹、每流绑定与表达式准备

北京时间17:39开始；用户最新要求每轮最多120分钟、尽量跑满持续推进，通常2个子代理、最多3个。所有编译、测试和微基准均在Actions，本地仅编辑/静态检查及读取已生成证据。

先核实上一轮热修：生产/测试/打包 `c65761fed6eed5ad71512e1cea2271eb49a4163f` / [40479845571](https://github.com/wu162/NewSdk/actions/runs/40479845571) 已整体success，包含17个Core fixtures、全部CLI/缓存/输入根、完整mymod/BIG和独立五文件ZIP。下载核验targeted artifact `12722102881`，SHA256 `dc4f5d4fc3a76ffc7aafe569ca136e881fe1eca1ca38cfecd28956225d56eae8`，source/run匹配；双配置PhysicalInputs各111检查，hardlink alias unlink阻断且独立inode不误合并。完整artifact `12723670985` 的GitHub digest为 `d004d4ae92f91e934ec7a1f0056a18e62f5af1ad85d30dfd56be8a0537592af3`；本轮未下载该大包，不冒称独立校验ZIP字节。

| 生产/测试来源 | Actions | 状态与保留失败 |
| --- | --- | --- |
| `88e114bc0c1623dd5be707a3b583189ee78b9473` | [40483925423](https://github.com/wu162/NewSdk/actions/runs/40483925423) | failure：Foundation审计拒绝新增Utility.Asset.Update回调签名；修复精确列出Asset.Update和Manifest.Load两种新增重载，旧全部public/family签名、flags/custom modifiers门槛保留。 |
| `cff094f1067c38465b9c00f777297953e45ee623` | [40484533010](https://github.com/wu162/NewSdk/actions/runs/40484533010) | failure：双配置18个Core fixtures仅ManifestInputs的测试配置默认BuildCache=true/未给缓存目录，OutputManager构造Path.Combine(null,platform)拒绝；其余17通过。原生产与断言不改，f001仅显式关闭该fixture未用缓存。未冒充完整SDK/ZIP验收。 |
| `6e54d06f40a7d8e7c5bb08938b5029f2db2403dc` | [40485040370](https://github.com/wu162/NewSdk/actions/runs/40485040370) | 双配置新增W06 StreamBindings各42检查/3组通过，19 fixtures仅上述已知manifest测试配置失败；继续收集其它独立矩阵，不作为整体成功。 |
| `25dde80d4c9355fa67e64679bf1dedd47e5ee65c` | [40486677963](https://github.com/wu162/NewSdk/actions/runs/40486677963) | 双配置20个Core fixtures全部通过，含修好的manifest与Expr各1279检查/5组；其后应用协议校验失败。新候选一致性测试写入非协议effectiveOptions.Source临时标签，已改用实际Options.Parse接收/data参数，不扩大schema枚举。当前后续完整重验仍必需。 |

已下载核对targeted：cff artifact `12725295399` / SHA256 `f95e7d07d5d59d9e5b243f83f99eea515e0f3291f3e743be2848484d694ec031`；6e artifact `12724588122` / `0daa7f5ba386f04d887fc0ce6a2b83688ae6c55815c56504d517b38b5db34f00`；25d artifact `12724659580` / `97a7199e613fb94ac2518ba59fa3318cdbde1f8424c6f5c62154422b0f38e60d`。各收据source/run与原始归档匹配，只接受各自成功组件范围。

W03新增两个Core入口及Utility递归patch/StringHashes封存与实际发现查询复核；不新增resolve时探测，不复用答案。存在性检查只证明复核时点的已登记候选，预算/路径/输出根回退明确，不替代全根缓存校验或Texture闭包。生产Utility原签名和fallback保留，追加三条回调签名经独立显式审计。

W05记录实际绑定/值/来源/安装epoch及原符号候选/选择/歧义，继承记录实际消费阶段。标量事件有界，canAuthorizeReuse=false，legacy XML hash不是强缓存放行证据。W06执行期map归root私有，强引用重复位置、weak规则、合法引用环、错误与排序保留；成功Prepare的公开快照不再持有可变Core对象。尚未完成整个不可变AssetSemanticResult或并行隔离。

W08 exact程序只在完整原parser求值与AsString成功后发布，暖路重新执行当前Define/文化转换与原运算。25d原SDK DLL隔离进程264行、四文化cold/warm/fallback值/错误码/消息/parser诊断/UsedDefines全部相同；当前Define零值、缺失和未求值失败的首错读取前缀不变，失败不入缓存。独立entry/text/node预算压力、depth回退、跨DP隔离和真实XML四输出差分通过。后续共享文化与微基准属于新源，不能用25d组件结果冒称该新组已经通过。

### 只读文化与传输缓冲的首次整组结果

`9fce00c4e785a874607ff78f7eedc639f68dd17e` / [40488513976](https://github.com/wu162/NewSdk/actions/runs/40488513976) 双配置21个Core fixtures中20项通过，ExpressionPrograms各12421检查，新增五开关组合原DLL差分及文化ownership均通过。唯一目标组失败为TransferBuffers：default/fallback各自生成AssetHeader，32字节中只设置7个DWORD而保留原未初始化padding，两份独立源导致完整.tmp SHA不可直接相等。每个copy自身source==destination及原尾块拒绝已通过；修复让两路逐案例复用同一份原始source并校验其完整SHA和所有header字段，保留完整.tmp长度/SHA强差分，生产格式未改，不忽略padding换通过。

下载核验targeted artifact `12727940311` / SHA256 `a71985151b308c2f738c34345629febca3319f1b0aa7ee48d43c0b9ca15d65d3`，binary/test/source/run均9fce/40488513976。以下只接受独立成功的表达式组件，不作为该run整体验收或新缓冲组成功。

同runner/同进程，各路径先预热30次，7轮轮换顺序，每轮每路径500次，所有实际LookupDefine/结果检查保留。单位ms，中位数[最小,最大]；表达式`=round(($I+3)*2/3)+(($I*2)>=10)`，每次结果8。原始21行样本见上述result.json。

| 配置 | program共享文化 | parser共享文化 | program独立文化 |
| --- | --- | --- | --- |
| Debug | 1.0535 [1.0157,1.0582] | 40.1146 [39.8454,43.0562] | 1.6057 [1.5840,3.1085] |
| Release | 1.0431 [0.9898,1.0589] | 39.5572 [39.2252,40.4235] | 1.6714 [1.6643,1.7772] |

每program batch实际parserCalls=0/hits=500，parser batch为500/0，测量不包含报告序列化；只是重复表达式组件的工作与耗时，不报告完整CLI提速、P95、物理I/O或统计显著性。25d真实三流业务输入当时无表达式，记录parser/build/hit全0；后续业务fixture加入三个相同Define求值，仍先做原XSD预检和原BAB四输出对照，验证真实CLI中1次parse/build+2次语义重放，不能把原0次称作已优化的业务收益。

当前追加批次还包括：64KiB复用buffer的终验SHA及确定性read/EOF取消、强内容复核后才复核候选存在性；声明Handle保存/恢复两侧值图隔离（无DOM/Define或整个LastState不可变承诺）。这些新源须完整重建，未取得新source/run整体成功前不标验收完成。

### 第四轮最终完整生产验收

`eb7ed6d07679be91c768fb145d4ca85e29720f65` / [40490423682](https://github.com/wu162/NewSdk/actions/runs/40490423682) 保留failure：双配置22 fixtures仅DeclarationSnapshots的derived-list回退检查失败，其他21成功。保存时派生列表回退被复制为base List2，恢复时二次分类误判为可clone；生产修复为LastState/Current运行时legacy来源标记，跨保存/恢复/再次保存传播，XML不新增字段，强断言及recapture检查保留。该失败targeted artifact `12728742768` 已下载核验SHA256 `9527721e05f82a9c3df7927cbcda3a7fa1977b025bb175a7440dbb2877358f0d`。

最终生产/首次测试/打包源码 **`60abfeff415e8b1c5bc0b26bf1457f13754c6d75` / [40491888271](https://github.com/wu162/NewSdk/actions/runs/40491888271) 整体success**：Debug/Release各22个Core fixtures、原SDK DLL/原BAB、协议、Texture、真实CLI、build/session cache和输入根矩阵、完整mymod、BIG及独立五文件ZIP全部通过。原异常/失败记录保留，没有删除样本、关闭回归或放宽断言。

已下载核验targeted artifact `12727804602` / SHA256 `85d8ae1f8975192f010295385d8fc2ff465ebe35b154dcf2907abfe52eaa030c`，收据source/run均为本成功身份，双配置22/22；PhysicalInputs各179检查、TransferBuffers各189、DeclarationSnapshots各46、ExpressionPrograms各12421。取消mid-read/EOF是在实际private SHA helper上的确定性stream用例，真实Snapshot整体采用预取消+有效token重试，不冒称向真实磁盘读中注入取消。

CLI artifact `12729275483` / SHA256 `132359b543b49e1212cdf41fbfc2062039aa6b8f57487f4d55dea0f463c07b2c` 已下载核验。两配置business-work-metrics均succeeded=true且binary/source=60：base三次相同Define为parserCalls1/builds1/hits2，APT/maps无表达式均0，原BAB与独立冷组四输出相等。三流copyRequests/allocations均0，linkRequests依次5/24/6、各linkAllocations1/65536字节、legacy请求0；只说明当前owner分配，不报告完整内存峰值或端到端提速。原schema逐流读/Compile与构建内元数据共享门槛保留。

完整生产artifact `12729367281`（106,765,851字节），GitHub SHA256 `3300f351b4dfff1d89a4d86d7dcb349ad1d2f7566c932fa01ff11e8b384f65c9`。该完整archive随后由严格复用/导出Actions下载并按digest/size逐字节核对；本地只下载其小证据包和导出原ZIP，不冒称本地下载过106MB原archive。

### 当前接受点的严格复用与原包导出

工具源码 `b549357fa32d2f73d51ae8f0d24c8035abf53681` / [40492634684](https://github.com/wu162/NewSdk/actions/runs/40492634684) 先仅运行控制器合同：12项真实临时git闭包测试+14项导出拒绝/传输测试成功，runtime job未请求；当时未更新旧68 pin。小证据 `12727624888` 已下载核验SHA256 `0078f4bd0017fb4c517dbefecca9914136685a4a5d5bb7af9fddee9f2a0f51ab`。

确认60完整终态success后，工具/当前探针源码 **`443dc88152606a5a41475ff0a5820fc958e1a003` / [40494473681](https://github.com/wu162/NewSdk/actions/runs/40494473681) 整体success**。Ubuntu及Windows控制器合同通过，原SOURCE/RUN固定接受点更新为60/40491888271，仓库/分支/workflow/attempt、完整archive身份、production/validation共同对象、EXE/Texture绑定与实际依赖闭包全部严格核对。精确22 fixture及指定工具/Markdown白名单之外、生产/SDK/recipe、删除/rename/typechange、非祖先、staged/unstaged及含ignored的未跟踪生产输入拒绝。

该run未重新编译生产：binarySourceCommit=60、testSourceCommit=443、sourceRunId=40491888271；当前22个fixtures在原生产对象上重新编译运行，双配置22/22。复用证据artifact `12730467093` 已下载核验SHA256 `1a424fcd046905a83141dcbf2a5d8676cc2df79d35b4f3c6999f9025c726d07c`，工具/测试身份和12+14项合同结果一致。

Export再次实时确认完整接受metadata，从同一校验过的原archive读取发布receipt/manifest/ZIP，与staged product runtime的精确五文件逐字节核对，只复制原ZIP与manifest；rebuildPerformed=false、zipBytesRepacked=false。原发布manifest的binarySource与packagingSource仍为60，443只记在export/targeted测试收据，candidateValidated=false不表示重新验收候选生产代码。

| 模式 | 原ZIP字节数 | 原ZIP SHA256 | 导出artifact |
| --- | --- | --- | --- |
| Debug | 2,269,828 | `cd30de97f6e38f33f278fc8a77ecc5ca61226d2ee32e1a51015d16e6aeb00b99` | [12730631821](https://github.com/wu162/NewSdk/actions/runs/40494473681/artifacts/12730631821) |
| Release | 1,767,446 | `651084b2296449b99027ef59e126152bb48b42f2609be0012266bb850a6713f1` | [12730207255](https://github.com/wu162/NewSdk/actions/runs/40494473681/artifacts/12730207255) |

两份导出小artifact均已本地下载复核外层SHA（Debug `c1895d09e11ea4fa8467a41ffd2b154bfaefab2ffcdbc169a236b750b2792eb0`，Release `18d18a83624f58e9b9a3d828b23f68c08bd6334c1ab263413aa4cc308f823fad`）、原内层ZIP、manifest及五个成员的长度/SHA、binary/test/sourceRun身份。这里只读取已生成字节和收据；实际EXE doctor、输出、BIG与事务运行仍全部来自Actions验收。

本轮未完成整个不可变AssetSemanticResult、AssetDeclarationDocument.LastState/SelfInstances生命周期、所有隐藏文件读取/集合查询、W07完整bundle缓存或W09—10并行合同。绑定Prepare快照不是事务接受凭据；null未新增XML roundtrip支持。缓存默认、/bc /sc全根保护、Texture限制、Win32/.NET4.8与前置运行条件未改变。完整mymod仍一流，不外推全游戏/G3/中文Texture/游戏内新验收。
