# MOD 编译器：结果主线与续接入口

更新：2026-10-28（北京时间）。用户指定 `compiler-main` 为重构结果及今后开发主线。旧 `refactor/modcompiler-p0-p3` 保留历史，PR #13 已关闭且未合并，不再合回旧主线。开发主线约定不代表已修改 GitHub 网页默认分支。
禁止强推；今后其他合并仍须用户批准；子代理数量由当轮用户指定。

## 当前任务：资产动作缓存与跨文档并行（2026-10-28）

最新用户要求：XML按完整后端模块批量开放，避免逐类型复制夹具/全量编译。当前289条内建XML路由已完成63模块源码审计，284纯metadata+4既有APT闭合读取共288条资格已接生产（TestTexture仍owner），**批量组件/集成CI待执行**。未知新增type默认owner，通用4平台worker/owner三缓冲或拒绝一致矩阵复用复杂历史差分，详见专项及XML_BACKEND_REENTRY.json。Upheaval在专用test/upheaval-cache-parallel-20261028，仅显式低频单批测量，普通push只廉价guards；先等完整成功二进制，不以失败归档开始大型编译。

成果集中 `feature/cache-parallel`，基线 `compiler-main@e7396d3b`，[draft PR #17](https://github.com/wu162/NewSdk/pull/17)；未经用户批准不合入。源码入口、准确资格和原始证据只看 [CACHE_PARALLEL](docs/CACHE_PARALLEL.md) 及其JSON，无需重读历史。

本轮继续（2026-10-28，约80分钟、通常两个子代理最多三个）：已准备独立 `preparationRejectedReasons`、完整九个元数据后端族共24类并行资格（SDK公开19根）及双 Mesh 真实区间验收；**新源码/测试尚未在 Actions 验证**，不得用下方129d收据代替。本批生产闭包改变，首批b467d708在40670721531验收；本批统一新建Debug/Release并跑完整验收。用户指出逐类型开放太细，收束这批后按模块合同成组认证/复用测试，优先专用测试分支的低频Upheaval实际计量，准确状态见专项。

当前生产源码 `129d9639` 的[40660066519](https://github.com/wu162/NewSdk/actions/runs/40660066519)已完整success，原始归档/12组收据/两配置五文件ZIP已核验：稳定SDK/runtime文件按512-largest持续读租约和128位身份复核复用摘要，排除当前MOD完整子树与SDKAdditional；目录仍首尾全枚举，MOD/Additional仍鲜读。预检/生产/Core/真实SDKArt Texture+SageXml账本/精确缓存/旧session/完整mymod/BIG/独立ZIP全部通过。原包12805100025，精确SHA见专项。

先前诊断来源：`810dd2ad` / [40657123355](https://github.com/wu162/NewSdk/actions/runs/40657123355)完整success，原包12803193116、12组收据和两配置运行文件/五文件ZIP均核验。包括原强ActionKey/完整三缓冲区及CDATA/回滚store/旧session隔离、MachineResources.v1按CPU和Win32物理/虚拟内存准入及降额、十二根Xmlenvironment worker、准确miss/bypass报告。冷8次缺失、损坏和pending各仅1次对应miss、其余7命中，修复后恢复全热；完整未精简mymod/原BAB/BIG/独立ZIP保留。十二根都使用worker，实际Weather与W3X重叠，不夸大每根双同类实测。

真实MOD计量[40659913327](https://github.com/wu162/NewSdk/actions/runs/40659913327)success，binary810/test25df，严格复用成功归档未重编。完整MOD7文件982257字节，真实SDK14715文件125887928字节，47资产热全部命中/零backend；热仍约230MB哈希读取、3.1–3.3秒，比无缓存0.76–0.95秒更慢。它不覆盖129d SDK扩展；相同输入新计量[40662537517](https://github.com/wu162/NewSdk/actions/runs/40662537517)已success，binary129d/teste259：完整SDK/MOD字节、参数与跨版本四输出均核对一致，两配置每次热构建少78862090字节哈希读取（约75.2MiB），47hit/0backend不变。新热中位数Debug2545ms/Release2463ms，仍比新无缓存763/668ms慢；不宣称此小MOD有缓存净收益或通用加速。两生产归档与两重放归档均已核验，完整来源见专项JSON。

主线继续资产动作缓存与跨文档并行的功能完善，包括细分prepare拒绝原因、更多已认证后端与双Mesh实测。小MOD热缓存慢于无缓存的问题已单列为次要小任务，见[专项记录](docs/CACHE_PARALLEL.md)“小 MOD 热缓存开销”；有余量时处理，不作为下一轮默认主线或主线验收阻塞项。Texture、重采样/PathMusic等继续owner，混合Texture session仍拒绝。默认bc/sc=false、jobs1保留，默认发行不随本分支替换。

按 [EFFICIENCY](EFFICIENCY.md) 集中实现/预检/完整验收，全部编译测试在Actions。仅测试改变先核验复用，分别记录binary/test来源；文档不重编，每10–15分钟检查点普通推送。用户本轮要求约70分钟、通常两个子代理最多三个，未自动变成以后每轮固定约定。

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
