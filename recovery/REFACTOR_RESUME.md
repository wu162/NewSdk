# MOD 编译器：结果主线与续接入口

更新：2026-10-27（北京时间）。用户指定 `compiler-main` 为重构结果及今后开发主线。旧 `refactor/modcompiler-p0-p3` 保留历史，PR #13 已关闭且未合并，不再合回旧主线。开发主线约定不代表已修改 GitHub 网页默认分支。
禁止强推；今后其他合并仍须用户批准；子代理数量由当轮用户指定。

## Upheaval 编译速度比较（2026-10-28 北京时间，已验证）

独立分支 `benchmark/upheaval-compiler-speed`，[PR #16](https://github.com/wu162/NewSdk/pull/16)。[实测运行40604851281](https://github.com/wu162/NewSdk/actions/runs/40604851281) 全部成功：完整 Upheaval 五入口、1 对预热后 10 对交替正式样本，新 Release 平均 **74.002秒**，旧 SDK **88.918秒**，耗时减少 **16.77%**，速度比 **1.202×**（配对95% CI **1.199–1.204×**）。22 次构建各1311文件/1291CDATA全部身份一致，SDK/MOD/运行文件未变；仅clean compile/link，不含LoD/BIG/SKUDEF或增量缓存。

详见 [完整结果与来源](docs/UPHEAVAL_BENCHMARK.md)、[持久计量JSON](docs/UPHEAVAL_BENCHMARK_RESULT.json) 和 [复跑方法](benchmarks/upheaval/README.md)。二进制源码 `8b3e146cb5220e830ff6a0ce4718f47af052f166` / 成功运行40446975355，测试源码 `c98507a0bdc90f7a5dbdbe37353a17ecba765fc2`，未重建或修改生产代码。[40608627111](https://github.com/wu162/NewSdk/actions/runs/40608627111) 已验证原始归档并只读提取约2MiB轻量报告，未重新编译；以后正常运行也同时上传轻量包，手动 `report_run_id` 仅提取已有成功结果。早期40604428912/40604660397的输入预检失败保留，未混入统计。当前任务已完成，后续按用户审查决定是否合入，禁止强推或自行合并。

## 扩展缓存与完整打包矩阵（验证中）

独立分支及[PR#16](https://github.com/wu162/NewSdk/pull/16)继续，暂保持draft；生产编译器未改。缓存、SC正式测量来自[40720756791](https://github.com/wu162/NewSdk/actions/runs/40720756791)，测试源码8318eb71，两VM各5对。无缓存旧82.342秒/新68.594秒，BC冷建旧83.804秒/新88.947秒，BC热建旧24.149秒/新45.804秒。**新版无缓存优势不能泛化到缓存体验。** 两VM缓存checkpoint及终态summary的原始SHA已核对。

原407207两VM已终态，所有输入/运行文件哈希未变，缓存/增量/SC/包成员门槛执行完；但旧SDK被破坏的最大Texture缓存返回成功且产物字节错误，最终正确性门槛失败，不能写整轮通过。完整新SC两VM明确CORE_SESSION_TEXTURE_UNSUPPORTED；老完整SC冷/热各一次观察89.051/4.658秒、74.178/3.673秒。原地图元数据SC子集各5对，合计10对：冷旧0.598秒/新3.342秒，热旧0.501秒/新3.262秒；不冒充完整MOD支持。

首轮框架直接继承Actions环境，而生产缓存键覆盖整个子进程环境；跨步骤临时变量变化导致基线备份在后续新侧增量/恢复中全部miss。首轮首次增量及新版缓存损坏的因果结论不接受，原数据保留。同一步骤的BC冷热及各自初始化的SC/打包不受影响。源码41cea8d84a7efafc631de20aae636b991d5d656b固定两侧Windows运行环境白名单，逐阶段摘要一致校验，修改/破坏前强制真实热命中证明；[40737740303](https://github.com/wu162/NewSdk/actions/runs/40737740303)仅补测XML/DDS/WAV和恢复，每组各两VM、最多四个job独立并行。尚未结束，不用首轮混淆成绩填补正式结果。

直接输出打包首次运行[40725824993](https://github.com/wu162/NewSdk/actions/runs/40725824993)、源码c35ed91d在初始化失败：PowerShell把单字符串splat拆成字符，未进入计时。现改成显式原生参数调用并重新启动，仅基线+打包两VM，不重复有效缓存/SC。原8318的额外收集复制打包只能作次要记录，不能冒充正式直接输出时间。

报告器用GitHub jobs runner_id/runner_name校验独立VM，不用托管镜像的相同hostname；原hostname汇总错误及原始失败终态保留。当前11项本地轻量测试通过；新汇总器明确排除缺少固定环境证据的旧增量组、额外复制的旧打包组并标注新版旧恢复试验无命名空间证明，准备只读重汇总，无需重编译有效组。后续先查上述补测及新的直接打包终态，下载轻量artifact并核对SHA、source/run/job身份，再更新[扩展报告](docs/UPHEAVAL_EXPANDED.md)、持久JSON及PR；不得自行合并。

## 默认构建与参数改进（PR #14，待合入）

本轮从 `compiler-main` 的 `933cf4a2` 创建 `feature/compiler-build-defaults`，PR [#14](https://github.com/wu162/NewSdk/pull/14)，尚未合并。生产/测试源码 `8b3e146cb5220e830ff6a0ce4718f47af052f166`；[验收运行40446975355](https://github.com/wu162/NewSdk/actions/runs/40446975355) 的完整 mymod 原 BAB 流、含 Additional 的默认 BIG 与 SKUDEF 差分、全部受影响回归及独立五文件 ZIP 验收均已通过，运行整体 success。

标准构建默认自动发现 base/APT/全局元数据并输出 BIG/SKUDEF，Additional 默认包含、可关闭且覆盖警告；开发模式默认 loose，高级兼容入口保留。`global-metadata` 兼容 `maps`，项目可保存 SDK/输出路径和常用设置。显式部分编译/空可选组行为与打包机制保留，缓存参数/默认值/实现未改。最新使用方式见 [COMPILER_USAGE.md](COMPILER_USAGE.md)，来源与范围见 [BUILD_DEFAULTS_VALIDATION.md](docs/BUILD_DEFAULTS_VALIDATION.md)。下次以该 PR 分支续接，用户批准后才能合入 `compiler-main`；主线现有发行下载不因此自动替换。

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

本次选定范围的收尾与工具交付已完成。后续按用户新需求选择上述未覆盖能力推进；继续以 compiler-main 为入口，不再准备合回旧分支。
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
