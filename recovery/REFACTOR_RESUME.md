# MOD 编译器：结果主线与续接入口

更新：2026-10-28（北京时间）。用户指定 `compiler-main` 为重构结果及今后开发主线。旧 `refactor/modcompiler-p0-p3` 保留历史，PR #13 已关闭且未合并，不再合回旧主线。开发主线约定不代表已修改 GitHub 网页默认分支。
禁止强推；今后其他合并仍须用户批准；子代理数量由当轮用户指定。

## 当前任务：资产动作缓存与跨文档并行（2026-10-28）

成果分支 `feature/cache-parallel`，基线 `compiler-main@e7396d3b`，成果为 [draft PR #17](https://github.com/wu162/NewSdk/pull/17)。已接入生产统一资产任务、强ActionKey、按需完整结果及可回滚store、jobs与有界W3X worker；旧session隔离、Texture保留兼容。准确能力、源码入口、计量、sourceCommit/runId/归档哈希与剩余范围只看 [CACHE_PARALLEL](docs/CACHE_PARALLEL.md)，无需重读全量历史。

`125902cc` 的 [40629580163](https://github.com/wu162/NewSdk/actions/runs/40629580163) 整体success：Debug/Release完整生产、精确缓存、CDATA、跨文档实际重叠、读租约、旧来源授权、重复include/继承、native在途取消/异常安全汇合、旧session、完整mymod、五文件ZIP独立运行。该push运行已包含采样频率优化和native异常用例；原始归档、两配置运行文件/ZIP哈希及计量均已核验，见专项记录。主线发行不自动替换。

本轮 XML/Audio 并行已在 `d7ecbebf` 的 [40647141375](https://github.com/wu162/NewSdk/actions/runs/40647141375) 完整通过 Debug/Release、精确缓存/CDATA、实际重叠、完整 mymod 与五文件 ZIP。覆盖 APT 四根、Xmlaudio 七根、内建 AudioFile 与 MP3 passthrough；未知注入编码器拒绝认证，重采样/PathMusic 保持 owner。新增成功归档 payload 重放工具已通过 [40648208910](https://github.com/wu162/NewSdk/actions/runs/40648208910) 的拒绝合同测试，真实归档重放 [40649079387](https://github.com/wu162/NewSdk/actions/runs/40649079387) 已成功，运行文件哈希与原二进制一致，binary/test 来源分别记录。

按用户关于机器资源的最新反馈，生产调度已改为 `MachineResources.v1`：jobs/CPU/Win32可用物理和虚拟地址空间共同准入，单任务和准备态预算由总预算推导；默认jobs=1保留，显式jobs=N不再统一硬限2 worker。`c2a2cec6`的 [40650040607](https://github.com/wu162/NewSdk/actions/runs/40650040607) 完整success：两配置均在4核机器得到4路真实重叠、约512MiB预算；压力下8→1、安全汇合、超预算APT owner、小任务worker、完整mymod及五文件ZIP全部通过。原始归档12800535841和运行文件/ZIP哈希已核验，精确来源见专项JSON。首跑40649384165因测试路径大小写误判failure，c2仅修测试、未改生产或放宽断言；失败原终态保留。现payload复用不覆盖Core宿主与完整SDK/发行链，本次集中重建完成验收。

本轮前两组已完整通过：6ed的[40656205838](https://github.com/wu162/NewSdk/actions/runs/40656205838)与810的[40657123355](https://github.com/wu162/NewSdk/actions/runs/40657123355)，原始归档、12组收据及两配置五文件ZIP均核验。新增工具/Schema/runtime摘要读租约、十二根Xmlenvironment并行和准确miss/bypass报告；十二根都实际使用worker，实际Weather/W3X重叠，不夸大每根双同类实测。

当前源码`129d9639`正在[40660066519](https://github.com/wu162/NewSdk/actions/runs/40660066519)完整验收稳定SDK摘要共享，排除当前MOD子树和SDKAdditional，保留全根枚举/鲜读及512句柄上限。新增真实SDKArt Texture只读兼容与未消费SageXml账本，未宣称此来源通过。

真实MOD计量[40659913327](https://github.com/wu162/NewSdk/actions/runs/40659913327)success，binary810/test25df，严格复用成功归档、未重编。原mymod7文件982257字节，47资产热全部命中且零backend；热仍约230MB哈希读取，3.1–3.3秒，比无缓存0.76–0.95秒更慢，不能宣称加速。该计量不覆盖129d；下步用129d成功产物核对相同输入的读量变化，准确数据见CACHE_PARALLEL。

后续优先按计量减少跨stream扫描或扩展剩余已认证并行，保留现有强身份、读租约、失败事务与完整SDK门槛。编译/测试全部GitHub Actions；通常两个子代理、最多三个由本轮用户授权。禁止强推；目标compiler-main，未经用户批准不合入。

执行要求（2026-10-28用户再次确认）：提高效率且守住正确性底线，按 [EFFICIENCY](EFFICIENCY.md#验证批次与触发决策) 集中实现、预检和验收。下一轮先列齐必要回归、核验可复用二进制；不要重复本轮零散补测试后多次启动完整构建的方式。仅文档变化不重跑已通过的生产/SDK矩阵；测试-only 变化使用严格来源重放并取得新结果，生产闭包变化统一重建。缓存专项记录列出了现有复用入口的具体限制。

## 已合入的默认构建与参数改进

PR [#14](https://github.com/wu162/NewSdk/pull/14) 已合入 `compiler-main@e7396d3b`（旧交接写待合入已过时）。生产/测试源码 `8b3e146c` 的 [运行40446975355](https://github.com/wu162/NewSdk/actions/runs/40446975355) 完整 mymod、默认 BIG/SKUDEF 和五文件 ZIP 验收属于基线证据，不能替代本轮新生产源码验收。详见 [默认构建验证](docs/BUILD_DEFAULTS_VALIDATION.md)。

## 最新旧参数兼容验收（2026-10-27）

`57354fbc013369eac1ad4c6e3346134d81a68a6c` 已恢复 ss/pc/slowclean 默认 true，并支持独立 true/false；缓存仍默认关闭。[运行40440661815](https://github.com/wu162/NewSdk/actions/runs/40440661815) 整体 success：完整未精简 mymod 的两配置 MOD 四文件逐字节相同、无额外引用流；真实 MOD 的 BIG 12成员与原 LoD/MakeBig 内容一致，SKUDEF 对照通过。应用/Core/原BAB/Texture/CLI/全部缓存/P7及五文件ZIP独立运行也均通过。发行下载见 COMPILER_RELEASE.md，失败历史和新收据见 [SDK_SCRIPT_VALIDATION.md](docs/SDK_SCRIPT_VALIDATION.md)。原失败40439297653不改写，本轮未降低断言；没有完整游戏数据或游戏内运行的新验收。

## 已完成的发行范围

- Win32 单进程核心编译、MOD目录入口、base/apt/maps 计划、日志/JSONL/报告、默认 BIG/SKUDEF 与五文件发行包已验收。恢复业务代码进入 EXE；纯 C# MakeBig 已内建，不启动 BAB/MakeBig worker。
- 最新结果在新主线重新编译 Debug/Release，应用、Core九组、原BAB基线、Texture、实际CLI、输入根、全部缓存、BIG原工具差分和独立ZIP运行均通过。准确源码/运行/产物身份见 COMPILER_RELEASE.md，不用文档头提交冒充二进制来源。
- 新增 mod-version、game-version、skudef-name 及项目预设/命令行优先级已通过；默认1.0/1.12保持旧脚本行为。布尔开关使用等号，帮助示例已修正。必要旧参数及拒绝范围见 CLI_COMPATIBILITY.md。
- `/bc`、`/sc` 默认关闭，可独立启用；会话缓存恢复实际文档和intermediate/CDATA，再重新链接输出。两配置各55用例、冷热/失效、缺损修复、失败取消和事务已验证。混合Texture仍明确拒绝 `/sc:true`。
- 默认base包含低/中LoD；APT版本标记为原脚本空格+CRLF。既有主线发行中Additional显式开启（本轮PR改为默认自动包含），SDK先、MOD后覆盖并排除.asset，已通过原CopyFolders/MakeBig全内容对照。
- 可选中立修复默认关闭；已集成HashFix→Resolver→LoD，在私有包工作区处理，缺完整参考明确拒绝。验收范围为组件差分及生产零插入链，未泛称完整游戏中立资产插入。
- 原LoD已知越界明确采用确定性修复合同：完整逻辑表复制并保留NUL，不复刻越界内存。28–32字节边界专项重复验证通过；原工具异常差分/失败证据保留，不能改称原始字节相等。
- Debug/Release正式包精确五运行文件，独立解压后doctor、小MOD、版本/包名、BIG、Additional、中立零插入、CDATA、回滚/取消/重试和协议全部通过。依赖.NET4.8、x86 VC80/VC与DirectX，不能推断裸Windows无前置要求。
- 本轮完整未精简 mymod 已按旧脚本非缓存参数在新 Debug/Release 构建上重跑并通过严格四文件与真实 BIG 验收；历史 XML/非 XML 差分仍保留原 source/run 范围。

## 已取消与未覆盖范围

- 用户取消AssetMerge集成，不是发行必做项。用户取消WBData下载/提取/读取器及依赖其真实数据的验收，不再作为重构完成条件；保留已有可选修复和缺输入拒绝，不能把取消标记为验收通过。
- 完整UI/runtime、中文纹理路径、混合Texture会话缓存仍有未覆盖范围。压缩/部分/冻结缓存、旧缓存目录覆盖及替代输出等未开放参数明确拒绝；这些不自动扩大为当前收尾任务。

## 后续工作方式

此前重构工具交付已完成；本轮缓存与并行状态以前文及 CACHE_PARALLEL 为准。后续继续以 compiler-main 为入口，不再准备合回旧分支。
生产输入改变时成组重建受影响回归；只有文档改变时不重编译。旧固定归档重放器仍绑定其历史源，不能直接用于当前新生产源码。新发行走主工作流的完整构建与发布门槛。

## 按任务读取

| 事项 | 入口 |
| --- | --- |
| 使用与文档分类 | `recovery/COMPILER_USAGE.md`、`recovery/README.md`、`recovery/docs/README.md` |
| 当前发行与参数 | `recovery/COMPILER_RELEASE.md`、`recovery/CLI_COMPATIBILITY.md` |
| 缓存与事务 | `recovery/P5_LIFECYCLE_AUDIT.md`、`recovery/src/App/Application/CoreStreamCompiler.cs`、`SessionPackage.cs`、`BuildAssetCache.cs` |
| BIG与LoD | `recovery/P7_PACKAGING.md`、`recovery/P7_LOD_ORIGINAL_TAIL.md`、`recovery/scripts/TestCompilerBig.ps1` |
| 执行与验证 | `recovery/EFFICIENCY.md`、`.github/workflows/refactor-core-lifecycle.yml`、`recovery/scripts/PublishCompiler.ps1` |

效率优先，正确性是底线；保留原始差分和失败终态，复用有效证据。长任务每约10–15分钟保存检查点，低频跟进Actions到终态；交接直接说明新增能力、验证、剩余范围和下一步。
