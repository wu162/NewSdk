# MOD 编译器：当前续接入口

更新：2026-10-28（北京时间）。开发主线为 `compiler-main`；当前任务在 `feature/compiler-optimization`，成果继续集中该分支。[PR #15](https://github.com/wu162/NewSdk/pull/15) 保持草稿，合并须用户明确批准；禁止任何强推。GitHub 网页默认分支以仓库实际设置为准。

## 继续性能优化

1. 读仓库 [AGENTS.md](../AGENTS.md)、[当前短交接](docs/compiler-optimization/HANDOFF.md)。
2. 按 [优化索引](docs/compiler-optimization/README.md) 选择源码和方案章节；当前实现边界查 [CONTRACTS](docs/compiler-optimization/CONTRACTS.md)，完整用户方案仅保留 [PLAN](docs/compiler-optimization/PLAN.md) 一份。
3. 按 [EFFICIENCY](EFFICIENCY.md) 推进完整能力组。通常 2 个子代理、最多 3 个；编译、测试和微基准全部使用 GitHub Actions。每约 10–15 分钟检查并普通推送有意义的检查点，未验证成果明确标记。

最新完整接受的生产、首次测试及打包来源为 **`71b5a0a4f55e530eccc9703eff2f3c3a397dd910` / [40598874896](https://github.com/wu162/NewSdk/actions/runs/40598874896)**，整体success。双配置各30组、原DLL/BAB、完整CLI/cache/SDK/mymod/BIG及五文件ZIP通过。

用户要求先交接，已暂停新增实现/运行。后续生产候选`6db109af / 40602153873`的Core双配置各29/30成功，唯一失败是Metadata探针错误要求Reader恢复丢失Validity；实测原版/恢复版保留，严格原DLL比较已过。交接时其独立CLI/cache/SDK仍在运行，未取消；下一轮先查终态并按[短交接](docs/compiler-optimization/HANDOFF.md)纠正期待、用Actions重验，不能把候选标完整接受或repin到失败来源。

本轮已接受live继承/栈安全、外部Cache查询/自身写删/别名、摘要读取器收尾、Texture/Builtin owner门禁及实际消费的能力声明。候选的Define索引、真实DOM回调/原DLL差分、XML/text资源、摘要回退取消和Chunk池已通过组件门槛，等待整批接受。完整ASR/Define环境、Texture及parse=text外部输入闭包、新bundle和并行仍未完成。精确来源、失败原因、已核证据与下一步见[HANDOFF](docs/compiler-optimization/HANDOFF.md)、[VALIDATION](docs/compiler-optimization/VALIDATION.md)。没有可靠完整CLI提速结论。

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
