# MOD 编译器：结果主线与续接入口

更新：2026-10-27（北京时间）。用户指定 `compiler-main` 为重构结果及今后开发主线。旧 `refactor/modcompiler-p0-p3` 保留历史，PR #13 已关闭且未合并，不再合回旧主线。开发主线约定不代表已修改 GitHub 网页默认分支。
禁止强推；今后其他合并仍须用户批准；子代理数量由当轮用户指定。

## Upheaval 编译速度比较（2026-10-28 北京时间，已验证）

独立分支 `benchmark/upheaval-compiler-speed`，[PR #16](https://github.com/wu162/NewSdk/pull/16)。[实测运行40604851281](https://github.com/wu162/NewSdk/actions/runs/40604851281) 全部成功：完整 Upheaval 五入口、1 对预热后 10 对交替正式样本，新 Release 平均 **74.002秒**，旧 SDK **88.918秒**，耗时减少 **16.77%**，速度比 **1.202×**（配对95% CI **1.199–1.204×**）。22 次构建各1311文件/1291CDATA全部身份一致，SDK/MOD/运行文件未变；仅clean compile/link，不含LoD/BIG/SKUDEF或增量缓存。

详见 [完整结果与来源](docs/UPHEAVAL_BENCHMARK.md)、[持久计量JSON](docs/UPHEAVAL_BENCHMARK_RESULT.json) 和 [复跑方法](benchmarks/upheaval/README.md)。二进制源码 `8b3e146cb5220e830ff6a0ce4718f47af052f166` / 成功运行40446975355，测试源码 `c98507a0bdc90f7a5dbdbe37353a17ecba765fc2`，未重建或修改生产代码。[40608627111](https://github.com/wu162/NewSdk/actions/runs/40608627111) 已验证原始归档并只读提取约2MiB轻量报告，未重新编译；以后正常运行也同时上传轻量包，手动 `report_run_id` 仅提取已有成功结果。早期40604428912/40604660397的输入预检失败保留，未混入统计。当前任务已完成，后续按用户审查决定是否合入，禁止强推或自行合并。

## 扩展缓存与完整打包矩阵（已完成）

独立分支 `benchmark/upheaval-compiler-speed`，[PR#16](https://github.com/wu162/NewSdk/pull/16)，尚未合入。每场景两台独立Windows VM，整套12个VM作业、12个正式场景、92对计时，另保存430次构建观察。生产编译器/SDK未改，复用既有Release的祖先、闭包、原始包和EXE绑定Texture均核验；各VM的SDK/MOD/运行文件最终字节未变。[扩展报告](docs/UPHEAVAL_EXPANDED.md)、[持久JSON](docs/UPHEAVAL_EXPANDED_RESULT.json)、[复跑入口](benchmarks/upheaval/README.md)已齐备。

| 正式场景 | 旧SDK秒 | 新版秒 | 结论 |
| --- | ---: | ---: | --- |
| 关闭生成缓存 | 82.342 | 68.594 | 新版速度1.2004倍，95%区间1.1957–1.2055 |
| BC首次冷建 | 83.804 | 88.947 | 旧版更快 |
| BC无修改热建 | 24.149 | 45.804 | 旧版约1.90倍速度 |
| 完整直接出包 | 71.556 | 59.348 | 新版速度1.2057倍，95%区间1.1965–1.2150 |

出包组使用不同VM，不能与缓存组绝对均值相减估算开销。XML/DDS/WAV真实同长度同mtime单文件修改，各首次/热建每行6对均通过独立无缓存oracle：新版首次耗时为旧版3.6～3.8倍、热建约1.9倍。修改前新侧命中1617/0，首次变成0/1617，显示整SDK/MOD/runtime内容清单命名空间的失效成本；DDS样本新缓存新增约54.9MB，旧新增692字节。

完整新SC两VM明确CORE_SESSION_TEXTURE_UNSUPPORTED，不计速度比；老完整SC支持，冷/热各一次观察89.051/4.658秒和74.178/3.673秒。地图元数据SC子集每行10对：冷旧0.598秒/新3.342秒，热旧0.501秒/新3.262秒，不能泛化完整MOD。CPU/峰值工作集、各VM计时及BIG1322成员+SKUDEF身份门槛见报告；BIG容器SHA不同，不声称整体字节相等。

固定环境故障补测：旧SDK破损Texture缓存返回成功但产物字节错误，两VM复现；新版拒用损坏缓存后重编译正确输出，但缺失/坏持久条目前后哈希相同，不能称已自动修好。无效XML两侧均失败且确认缓存不变，新侧既有发布字节也不变；恢复健康备份/合法XML后重试均正确。各故障各侧每VM仅一次，不作性能区间；JSON明确measurementComplete=true/allCorrectnessGatesPassed=false，不能写全部正确性通过。

正式来源：[40720756791](https://github.com/wu162/NewSdk/actions/runs/40720756791)/8318eb71的缓存和SC，经[40741441409](https://github.com/wu162/NewSdk/actions/runs/40741441409)/86eb0b25只读验证原summary SHA及runner身份、排除7个无效旧组；[40737740303](https://github.com/wu162/NewSdk/actions/runs/40737740303)/41cea8d8的固定环境增量/恢复；[40741198404](https://github.com/wu162/NewSdk/actions/runs/40741198404)/263fa808直接打包整体成功。三个双VM增量组成功；原运行及恢复组因真实旧SDK错误产物保留失败，报告器正常保存负向发现。11项最新轻量合同检查及直接打包预检通过。

修正记录保留：相同镜像hostname改用GitHub唯一runner_id/name核验；首轮Actions跨步骤环境改变新版缓存键，旧增量/新版恢复因果结论撤回，41cea8d8固定子进程环境并在修改/破坏前证明热命中后重测；40725824993因PowerShell字符串splat初始化失败、零计时，263fa808显式参数后重新验证。不得重用混淆数据、降低断言或重复启动有效组。待用户审查，禁止强推或自行合并。

## 缓存成本定位（诊断已验证）

用户继续追问冷/热建为何更慢。生产EXE/SDK未改，[40778834315](https://github.com/wu162/NewSdk/actions/runs/40778834315)整体成功，测试源d30f36aa13a2839eba5728c99b6e8c5b499fff7b：单Intel WindowsVM，原Release执行完整无缓存/冷/热各一次并核对原Upheaval指纹；未修改原函数源码用CLR4/x86 opt+独立helper重放3次，实际请求/任务及缓存namespace真命中证明、原确认缓存和全部输入不变门槛均通过。它是同源受控成本诊断，不能冒充整进程CPU采样或替换原双VM正式均值。

本机新版无缓存59.905秒、冷75.166秒、热49.148秒。五条流每条前后均完整Capture SDK/MOD/runtime：17443文件×10，哈希4319264120字节；原函数重放合计中位13.192秒（CPU12.563秒）。三APT及地图无缓存载荷却同样付全库扫描，地图从0.252增至3.437秒。热恢复复制/校验1.667秒，命中后重建候选1.091秒、丢弃0.118秒；Commit已有条目仅0.0003秒。冷候选复制/哈希1.149秒，不含插件首次生成工作缓存成本。发布阶段约1.5秒且差异小。

热命中1617项恰对应AudioFile1289、W3DAnimation194、W3DMesh134；仍解析925XML、实际插件编译1737次（含Texture115/OnDemandTexture2与大量XML类型），不能把1737全当贴图，也不能给未消融类型硬分配秒数。优先评估会话级去重扫描/保留统一最终校验，以及全命中无新增资产时避免候选重写；单文件依赖粒度和Texture D3DX闭包须另作正确性审计，不能只关哈希/改mtime键/强开贴图BC。

[诊断报告](docs/UPHEAVAL_CACHE_COST.md)、[持久原始计量](benchmarks/upheaval/results/cache-cost-40778834315.json)含源/运行文件哈希、84次方法观察、逐流事件、编译类型计数及已核验原archive摘要。原尝试40778055207在混合PE不能作普通程序集加载时预检失败、尚无构建计时，失败保留；随后同源helper方案无正式EXE修改。复跑suite=cache-cost只跑定向job，既有92对正式benchmark不重复。尚未优化生产实现，禁止自行合并。

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
