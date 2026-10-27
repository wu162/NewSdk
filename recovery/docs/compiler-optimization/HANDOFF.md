# 编译器优化：当前交接

更新：2026-10-27，北京时间；本轮窗口约14:01—15:01，使用2个子代理。
成果分支：`feature/compiler-optimization`，起点 `compiler-main@e7396d3`。
执行约定与方案读取索引：[README.md](README.md)。

## 当前状态：阶段 A 首批成果完整验收通过

- W01：三项风险已在真实 Core 复现、修复并通过双配置严格回归：覆盖前 Define 导入值比较、每流重绑引用/闭包、按 checksum+预期长度统一恢复链接三文件组。未宣称 CLI 边界已复现。
- W02 部分：按流输出 8 个阶段的调用/失败次数及单调时钟耗时；与 legacy MetricManager 独立，快照不随后续工作变动。
- W03 部分：新文件/时间戳变化合并 SHA256 + legacy FastHash 读取，暖路径保留 SHA-only；同时间戳内容变化仍重算。保留原一 MiB 分块、持久化字段与 Reset 内容复核；尚无跨流共享快照。
- W08 部分：NodeJoiner 调用内有序子节点索引，小列表保留扫描；原 DLL 的80个三方差分与隔离/归属检查通过。尚未实现 AST 缓存或引用索引。
- W04 部分：每 SchemaSet 代复用自有只读引用描述，保留声明上下文、实际类型、注解优先级和完整 schema 内容复核；提供回退与 build/hit 计数，不宣称跨流共享 schema。
- 已接入现有 Core lifecycle Actions 的新分支和新增 fixtures，Core 结束后提前上传小型证据；保留原生产、原 BAB、缓存、mymod、BIG 与独立发行 ZIP 验收。

## 验证与下一步

本地只编辑和静态阅读；所有测试/编译通过 GitHub Actions。
首组失败与改进依据留在 [VALIDATION.md](VALIDATION.md)，不在本文件累加日志或历史。
生产/测试源码 `75601909b0e874bc24786f1d6dd3d8c64eb1aee6` / [运行40461583024](https://github.com/wu162/NewSdk/actions/runs/40461583024) 整体 success：生产、两配置各13个 Core fixtures、原 BAB、协议、Texture、CLI、全部缓存、输入根、完整 mymod/BIG 和五文件 ZIP 独立验收均通过。后续提交仅文档，不改二进制来源。
提前上传 artifact `12714883356` 的 W01 严格回归、原 DLL 隔离对照、schema 与摘要均通过。微基准及已知边界见 [VALIDATION.md](VALIDATION.md)，不报告完整项目提速。
下一步：先补 W02 的 schema 准备/原始解析/外层成本区间，在已有多流 CLI business 样本记录重复准备量，再推进 W03 跨流物理输入快照或 W04 自有只读元数据共享及 W05 影子依赖。完整 mymod 本次仅1流，不能用来证明跨流收益。当前每流重绑不等于 W06 不可变结果分离或并行完成。
后续成果仍集中本分支；[PR #15](https://github.com/wu162/NewSdk/pull/15) 为草稿，不自行合入。每轮60分钟、通常2个子代理（最多3个）；只改文档不重编译。
