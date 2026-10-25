# TextureCompiler 验证触发规则

2026-10-26（北京时间），用户反馈“每次触发的测试个数太多了”。本规则优先于旧文档关于每次相关提交自动启动多条 Texture 工作流的安排。断言、Debug/Release、原版差分及 SDK 验收标准不变。

- **普通检查点提交不启动 Windows 验证。** 整个能力批次准备好后，在阶段提交消息加 `[texture-test]`。自动验证统一由 `texture-validation.yml` 承担：一个 Linux 选择作业、至多一个 Windows 作业，选中组顺序执行，不再按组/配置创建矩阵作业。
- `[texture-sdk]` 显式纳入受影响的四个 PNG/BMP SDK profile；普通 `[texture-test]` 不启动 SDK。同样已成功且输入未变的 SDK profile 保留原 sourceCommit/runId，不重复运行。
- `[texture-plan]` 仅运行选择器及其单元检查并上传计划，不启动 Windows。手动入口可指定 force_groups、run_sdk 和 plan_only。
- `TextureTestGroups.json` 明列实际消费者依赖。选择器要求完整成功的 Debug/Release 证据（SDK 则要求 Original/Recovered/NoOriginal 三组成功）、有效祖先 sourceCommit 和 runId，并比较该祖先与当前版本的依赖并集；探针、失败、不完整或非祖先记录不能跳过。原 SDK 工具 DLL 变化使相关组失效；新增/修改但未登记的实现文件按保守方式选组。
- 新的 helper 或消费关系必须更新依赖表。已登记组改依赖时同时核对验收版本的旧依赖，不能靠删除路径隐藏变化。测试数量选择不改变选中组内部覆盖范围。
- 原独立 Texture 组件、入口、SDK 与取证工作流保留手动入口，已从 push 自动触发迁出，功能由新入口集中调度。保留历史运行和证据；不取消、删除失败记录。
- 每个实际组仍生成自身收据；batch/selection.json 区分本轮执行与沿用结果，沿用记录保留原 sourceCommit/runId，不写成当前源码新跑成功。失败组保存证据并继续完成本批其他已选组，最终 batch 必须失败。

当前依赖表按 TGA 入口与 PNG/BMP 入口分别登记：只改 PNG 解码分支不会重跑只使用 TGA 的旧入口；修改共用 ImageLoader 或插件入口时才扩展到其实际消费者。生产 SDK 配置及 DLL 仍不替换。
