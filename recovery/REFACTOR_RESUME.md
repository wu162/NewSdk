# MOD 编译器：结果主线与续接入口

更新：2026-10-28（北京时间）。用户指定 `compiler-main` 为重构结果及今后开发主线。旧 `refactor/modcompiler-p0-p3` 保留历史，PR #13 已关闭且未合并，不再合回旧主线。开发主线约定不代表已修改 GitHub 网页默认分支。
禁止强推；今后其他合并仍须用户批准；子代理数量由当轮用户指定。

## 当前任务：资产动作缓存与跨文档并行（2026-10-28）

最新用户要求：XML按完整后端模块批量开放；大型MOD低频集中测试，运行时继续独立工作。当前289条内建XML路由完成63模块审计，284纯metadata+4既有APT闭合读取共288条资格已接生产，TestTexture仍owner；未知新增type默认owner。闭合表及完整审计见 [CACHE_PARALLEL](docs/CACHE_PARALLEL.md) / XML_BACKEND_REENTRY.json，不再按每type复制夹具。

- 成果分支 `feature/cache-parallel`，[draft PR #17](https://github.com/wu162/NewSdk/pull/17)，未经批准不合入。最新模块批量生产 `ac22f777` / [40676722714](https://github.com/wu162/NewSdk/actions/runs/40676722714) 已完整success并独立核验原包12811797346/12组收据/两配置五文件ZIP；Debug/Release通用284路由×四平台均真实worker且owner三缓冲原字节相同（无拒绝）。生产/SDK/完整mymod/BIG/旧session/取消回滚及独立ZIP全部通过。复杂历史原DLL矩阵和真实公开根/取消/回滚门槛保留，最小DOM矩阵不冒充复杂字段或全部SDK全局根验收。
- 前一阶段 `c807fd1b` / [40674077574](https://github.com/wu162/NewSdk/actions/runs/40674077574) 已完整success，原包12810219503、12组收据、两配置原五文件及独立ZIP均已核验。包含24类资格/19公开根、双Mesh4096/4顶点实际双worker重叠、13种源码prepare拒绝（Windows实测11种）、独立原因计数与零预算owner/部分租约释放；不能据此前一阶段宣称288路由批量已验收。
- 有界开销改进：full bounded保留集之外且没有既有entry的小文件跳过FileIdInfo查询，但仍同一拒写handle鲜读SHA；既有entry继续128位身份核对。每Capture独占复用SHA对象。App容量/等长mtime/替换释放及完整SDK已通过c807；尚无本轮真实MOD端到端新收益，不称小MOD缓存慢的问题已解决。129d历史同输入计量仍见专项JSON。
- 专用完整Upheaval测试分支 `test/upheaval-cache-parallel-bulk` 基于ac22，严格复用已验收原始五文件；[40682631514](https://github.com/wu162/NewSdk/actions/runs/40682631514) 的五入口plan和六次build收据成功，原始2723文件/300900885字节输入未改、每次1311输出原字节相同。同二进制无缓存jobs1→4为88.160s→57.733s（约缩短34.5%），热缓存三次中位73.609s仍慢于无缓存并行；每热次3214hit/0miss但鲜读哈希约3.45GB，下一步调查闭包成本。Actions整体failure来自末尾通用协议检查器误要求Debug/Release布局，保留原终态，仅轻量重放已有归档，勿为入口错误重跑MOD。精确source/归档SHA/边界见 [Upheaval计量](docs/UPHEAVAL_CACHE_PARALLEL.md)。

默认bc/sc=false、jobs1及旧发行不变，Texture、Audio重采样/Audio PathMusic仍owner。本批模块验收已全部完成，不重跑未变生产矩阵或大型MOD；优先用已有计量/小夹具定位缓存闭包成本。禁止强推、禁止未经批准合并。编译/测试全部Actions，文档不重编，依据EFFICIENCY复用有效来源，分别记录binary/test/packaging来源。

交接状态：测试分支 `582946ba` 的轻量归档重放 [40685364366](https://github.com/wu162/NewSdk/actions/runs/40685364366) 已success，guards与原包schema/原字节重放均通过，measure明确skipped；原40682631514的failure终态保留。下一轮按小夹具成组补缓存关键阶段计时；hot后端仅约5.7–6.0s/总73–77s，需调查其余wall，不能通过扩大XML资格或跳过强输入校验盲目优化。测试分支脚本截止/清理/reparse/协议修复均已保存；不将尚未跑到的新超时路径报为已验收。PR #17保持draft，未合入。

本轮B组最终生产`e86958de` / [40690172191](https://github.com/wu162/NewSdk/actions/runs/40690172191)已完整success：聚合`cache.phaseTimings`；共享每Capture目录边界/每SDK文件规范化分类；热恢复后移动已验证私有结果以消除再次复制。强校验、CDATA、首尾盘点及回滚保持；完整Debug/Release/Core/缓存/旧session/SDK/mymod/BIG/独立ZIP均通过。原包12817384085/SHA`accf9436a6343359286c386f31cbfaee3f92e7a492d60513c5951bc485edcee5`、12组收据、build/link身份、两配置五文件ZIP及608份报告计时账本已核验。恢复字节修复及普通/CDATA冷热断言通过；原`3ada12e7`零恢复字节检查点保留且不作完整计量。精确证据在JSON的`sharedInventoryAndPrivateMoveMilestone`。

仅计时基线`062053cb` / [40687271479](https://github.com/wu162/NewSdk/actions/runs/40687271479)已完整success，原包12816553914/SHA`1dedcd81b768cad26183d84aaffbb3b2838000c60b08647d8c3f43db9a1ea234`、12组收据/build-link来源/两配置五文件ZIP及608份报告计时账本已核验。Release小夹具hot1237ms、stream准备812.696ms、盘点208.477ms、后端30.924ms（嵌套不可相加），不能外推完整Upheaval。测试复用入口`d358a656`的guards [40687837253](https://github.com/wu162/NewSdk/actions/runs/40687837253)已success（34测试，replay未运行），不等同B组已验收。计时/边界见[CACHE_PARALLEL](docs/CACHE_PARALLEL.md)，精确新证据在JSON的`phaseTimingBaseline`。本轮未重跑大型Upheaval，未宣称缓存净收益已解决；收束工具工作，优先B组直接消除重复工作与必要验收。

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
