# NewSdk：缓存与并行优化分支

`feature/cache-parallel` 从 `compiler-main` 分出，专门实施 [缓存与并行优化计划](recovery/docs/CACHE_PARALLEL_PLAN.md)。成果继续提交到本分支，最终通过 [draft PR #17](https://github.com/wu162/NewSdk/pull/17) 面向 `compiler-main` 审查；未经用户明确批准不合入，禁止强推。

## 专项入口

日常续接：先读 `AGENTS.md`、简短交接与执行规则，再按交接链接读取计划章节；不默认加载完整计划和历史记录。

| 需要了解什么 | 文档 |
| --- | --- |
| 目标、范围、产品模式与验收条件 | [CACHE_PARALLEL_PLAN](recovery/docs/CACHE_PARALLEL_PLAN.md) |
| 最新状态、当前阻塞、下一步 | [REFACTOR_RESUME](recovery/REFACTOR_RESUME.md) |
| 能力矩阵、实现与验证记录 | [CACHE_PARALLEL](recovery/docs/CACHE_PARALLEL.md) |
| 完整 Upheaval 计量与限制 | [UPHEAVAL_CACHE_PARALLEL](recovery/docs/UPHEAVAL_CACHE_PARALLEL.md) |
| 成组实现、验证复用与保存规则 | [EFFICIENCY](recovery/EFFICIENCY.md) |

本分支围绕资产级动作缓存、重复输入读取/物化消除、跨文档有界后端并行推进。保留输出字节、CDATA、引用/schema、取消、事务回滚及适用 SDK/发行合同。默认缓存关闭、jobs=1；`/sc:true` 保留兼容路径且要求 jobs=1。具体可缓存与可并行资格以能力矩阵为准。

当前重点是计划 B 组的输入盘点/受控读取重复成本。已有并行收益证据，但热缓存端到端净收益仍未解决；不能把命中率、读取量或小样例结果当作普遍加速。

## 产品与历史参考

使用工具见 [COMPILER_USAGE](recovery/COMPILER_USAGE.md)，参数见 [CLI_COMPATIBILITY](recovery/CLI_COMPATIBILITY.md)，主线发行见 [COMPILER_RELEASE](recovery/COMPILER_RELEASE.md)。专项成果不自动替换主线发行。

[文档索引](recovery/README.md) 保留产品与历史资料入口。旧重构收尾、其他组件恢复和已取消事项不作为本分支待办；只在当前优化需要时查阅相关合同与证据。
