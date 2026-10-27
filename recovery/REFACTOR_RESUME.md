# MOD 编译器：当前续接入口

更新：2026-10-28（北京时间）。开发主线为 `compiler-main`；当前任务在 `feature/compiler-optimization`，成果继续集中该分支。[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，合并须用户明确批准；禁止任何强推。GitHub 网页默认分支以仓库实际设置为准。

## 继续性能优化

1. 读仓库 [AGENTS.md](../AGENTS.md)、[当前短交接](docs/compiler-optimization/HANDOFF.md)。
2. 按 [优化索引](docs/compiler-optimization/README.md) 选择源码和方案章节；当前实现边界查 [CONTRACTS](docs/compiler-optimization/CONTRACTS.md)，完整用户方案仅保留 [PLAN](docs/compiler-optimization/PLAN.md) 一份。
3. 按 [EFFICIENCY](EFFICIENCY.md) 推进完整能力组。通常 2 个子代理、最多 3 个；编译、测试和微基准全部使用 GitHub Actions。每约 10–15 分钟检查并普通推送有意义的检查点，未验证成果明确标记。

最新完整接受的生产、首次测试及打包来源为 **`71b5a0a4f55e530eccc9703eff2f3c3a397dd910` / [40598874896](https://github.com/wu162/NewSdk/actions/runs/40598874896)**，整体 success。双配置各30组、原DLL/独立原BAB、协议、Texture、CLI/cache/输入根、完整mymod/BIG和五文件ZIP通过。本轮前五组完整success；第六组Core有两fixture失败，修复与text/原语证据集中`6db109af / 40602153873`验证中，不能继承旧生产结论。精确来源、产物身份和下一组范围见[短交接](docs/compiler-optimization/HANDOFF.md)与[验证记录](docs/compiler-optimization/VALIDATION.md)。

本轮新增live继承阶段/栈安全、外部Cache查询/自身写删与case/junction别名、摘要读取确定收尾、Texture/Builtin所属线程门禁。能力声明下一组实际收紧缓存资格，没有授权强缓存或并行。实际三流小CLI默认/回退成对计量未观察到端到端提速；局部组件收益不能替代完整构建对照。

下一步仍以完整语义结果与 Define 环境为主线；Texture 隐藏闭包、新强键 bundle 缓存和并行尚未完成。Local/Output 的可变修复和 App staging 排除继续保留；不能靠旧 XML hash 或部分依赖放行强缓存命中。

## 已交付产品与边界

`compiler-main` 已交付 Win32/.NET4.8 单进程编译、MOD 目录入口、base/APT/全局元数据计划、日志/JSONL/报告、默认 BIG/SKUDEF、Additional、内建 MakeBig 和五文件运行包。PR #14 的默认构建/参数改进已合入，来源见 [BUILD_DEFAULTS_VALIDATION](docs/BUILD_DEFAULTS_VALIDATION.md)。使用和正式发行身份分别以 [使用说明](COMPILER_USAGE.md)、[发行说明](COMPILER_RELEASE.md) 为准。

- `/bc`、`/sc` 默认关闭；混合 Texture 会话缓存仍明确拒绝。缓存冷热、失效、缺损修复、失败取消与事务门槛保留。
- 中立修复默认关闭，已验收组件差分和生产零插入链；完整游戏数据/游戏内运行、完整 UI/runtime 和中文纹理路径仍有未覆盖范围。
- LoD 原工具越界采用完整表复制并保留 NUL 的确定性修复，不能把该边界称为原始越界字节相等。
- AssetMerge 集成、WBData 下载/提取/读取器及依赖其真实数据的验收已按用户要求取消，不列为本次必做项。

## 按任务读取

| 事项 | 入口 |
| --- | --- |
| 使用、参数、下载 | [COMPILER_USAGE](COMPILER_USAGE.md)、[CLI_COMPATIBILITY](CLI_COMPATIBILITY.md)、[COMPILER_RELEASE](COMPILER_RELEASE.md) |
| 优化状态、合同、证据 | [HANDOFF](docs/compiler-optimization/HANDOFF.md)、[CONTRACTS](docs/compiler-optimization/CONTRACTS.md)、[VALIDATION](docs/compiler-optimization/VALIDATION.md) |
| 缓存与事务 | [P5_LIFECYCLE_AUDIT](P5_LIFECYCLE_AUDIT.md)、`src/App/Application/CoreStreamCompiler.cs`、`SessionPackage.cs`、`BuildAssetCache.cs` |
| BIG、Additional、LoD | [P7_PACKAGING](P7_PACKAGING.md)、[P7_LOD_ORIGINAL_TAIL](P7_LOD_ORIGINAL_TAIL.md) |
| 构建与验收 | [工作流](../.github/workflows/refactor-core-lifecycle.yml)、[PublishCompiler](scripts/PublishCompiler.ps1) |
| 类型恢复与历史记录 | [分类索引](docs/README.md)、[优化前五轮归档](docs/compiler-optimization/history/VALIDATION_ROUNDS_1_5.md) |

生产输入变化时集中重建和回归；只改文档/测试时按来源、依赖闭包和产物哈希复用已通过对象。旧交接的“下一轮”只作历史，当前待办始终由短交接覆盖更新。
