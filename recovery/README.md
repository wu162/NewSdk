# 编译器文档入口

本分支 `feature/cache-parallel` 从 `compiler-main` 分出，专门执行 [缓存与并行计划](docs/CACHE_PARALLEL_PLAN.md)。继续专项先读 [简短交接](REFACTOR_RESUME.md) 和 [执行规则](EFFICIENCY.md)，再按需读计划章节；`compiler-main` 为最终 PR 目标。首次使用工具请读使用说明。历史组件记录用于追溯，旧“下一步”不代表当前待办。

## 使用与交付

| 需要做什么 | 文档 |
| --- | --- |
| 安装、编译 MOD、打包、缓存、排错 | [新编译器使用说明](COMPILER_USAGE.md) |
| 查看版本、下载和发行验证 | [发行说明](COMPILER_RELEASE.md) |
| 查询支持的命令行与旧参数 | [CLI 兼容范围](CLI_COMPATIBILITY.md) |
| 查询 BIG、Additional、中立修复布局 | [打包合同](P7_PACKAGING.md) |

## 开发与维护

| 事项 | 文档 |
| --- | --- |
| 当前状态与后续工作 | [REFACTOR_RESUME](REFACTOR_RESUME.md) |
| 工作规则 | [EFFICIENCY](EFFICIENCY.md)、[阶段时长](STAGE_DURATION.md) |
| 缓存与并行的当前能力、源码入口、CI 与下一步 | [缓存并行记录](docs/CACHE_PARALLEL.md) |
| 旧缓存与事务参考 | [生命周期记录](P5_LIFECYCLE_AUDIT.md) |
| 路径约束 | [PATH_AUDIT](PATH_AUDIT.md) |
| MakeBig 来源与恢复 | [MAKEBIG_RECOVERY](MAKEBIG_RECOVERY.md) |
| LoD 原工具缺陷与修复合同 | [LoD 边界说明](P7_LOD_ORIGINAL_TAIL.md) |
| 事件与最终报告 | [protocol](protocol/README.md) |

## 原实现与历史资料

[分类索引](docs/README.md) 集中列出 GameObject、XML、Weapon、Armor、基础库和原始流程资料。152份类型/历史记录已从根目录移入分类目录，内容、原提交与 Actions 证据保留，文档相对链接已调整。

`src/` 是恢复实现，`scripts/` 是构建及验证入口，`tests/` 是测试；`decompiled/`、`evidence/`、`preparation/` 是相应反编译、证据和准备资料。不要将准备目录或历史成功范围等同于当前产品已支持能力。
