# MOD 编译器：结果主线与续接入口

更新：2026-10-27（北京时间）。用户指定 `compiler-main` 为重构结果及今后开发主线。旧 `refactor/modcompiler-p0-p3` 保留历史，PR #13 已关闭且未合并，不再合回旧主线。开发主线约定不代表已修改 GitHub 网页默认分支。
禁止强推；今后其他合并仍须用户批准；子代理数量由当轮用户指定。

## 当前任务：编译器性能优化

2026-10-27 从 `compiler-main@e7396d3` 新建 `feature/compiler-optimization`。按用户最新要求每轮最多120分钟，效率优先、正确性是底线，通常2个子代理、最多3个；全部编译/测试通过 GitHub Actions。先读 [当前短交接](docs/compiler-optimization/HANDOFF.md)，再按 [方案索引](docs/compiler-optimization/README.md) 读取所需章节。完整用户方案只保存一份，阶段结果与未决工作由短交接覆盖更新。

首批优化与三项真实 Core 风险修复已通过[完整运行40461583024](https://github.com/wu162/NewSdk/actions/runs/40461583024)，生产源码 `75601909`。第二轮的 schema/解析/外层区间与构建内自有继承/依赖元数据共享已由源码 `4a128696` / [完整运行40466010643](https://github.com/wu162/NewSdk/actions/runs/40466010643) 整体成功验收，含全部缓存、完整mymod/BIG与独立五文件ZIP；三流元数据表1次构建+2次恢复，每流读取/Compile仍保留，不报告完整构建提速。范围、失败记录、计量与下一步见短交接和[验证记录](docs/compiler-optimization/VALIDATION.md)。[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，后续成果继续集中原优化分支。

第三轮硬链接热修 `c65761fe` / [40479845571](https://github.com/wu162/NewSdk/actions/runs/40479845571) 已整体success，含双配置17个Core fixtures、CLI/全部缓存、完整mymod/BIG和独立五文件ZIP。第四轮接入manifest封存/已消费查询复核、Define/继承/符号轨迹、每流私有绑定及exact表达式程序；`25dde80d` / [40486677963](https://github.com/wu162/NewSdk/actions/runs/40486677963) 双配置20个组件通过，但随后应用协议测试失败已修，尚非整体验收。追加只读文化复用与每DP传输缓冲正在统一验证；准确source/run和未完边界见短交接，不用旧成功接受新源。

## 默认构建与参数改进（PR #14，已合入）

本轮从 `compiler-main` 的 `933cf4a2` 创建 `feature/compiler-build-defaults`，PR [#14](https://github.com/wu162/NewSdk/pull/14)，已于2026-10-27合入 `compiler-main@e7396d3`。生产/测试源码 `8b3e146cb5220e830ff6a0ce4718f47af052f166`；[验收运行40446975355](https://github.com/wu162/NewSdk/actions/runs/40446975355) 的完整 mymod 原 BAB 流、含 Additional 的默认 BIG 与 SKUDEF 差分、全部受影响回归及独立五文件 ZIP 验收均已通过，运行整体 success。

标准构建默认自动发现 base/APT/全局元数据并输出 BIG/SKUDEF，Additional 默认包含、可关闭且覆盖警告；开发模式默认 loose，高级兼容入口保留。`global-metadata` 兼容 `maps`，项目可保存 SDK/输出路径和常用设置。显式部分编译/空可选组行为与打包机制保留，缓存参数/默认值/实现未改。最新使用方式见 [COMPILER_USAGE.md](COMPILER_USAGE.md)，来源与范围见 [BUILD_DEFAULTS_VALIDATION.md](docs/BUILD_DEFAULTS_VALIDATION.md)。该功能已在主线；新的优化工作从上面的独立分支续接，发行下载身份仍以 COMPILER_RELEASE.md 为准。

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
