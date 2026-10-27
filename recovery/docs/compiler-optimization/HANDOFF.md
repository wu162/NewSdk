# 编译器优化：当前交接

更新：2026-10-27，北京时间；本轮开始约 14:01，计划约 15:01 交接。
成果分支：`feature/compiler-optimization`，起点 `compiler-main@e7396d3`。
执行约定与方案读取索引：[README.md](README.md)。

## 当前状态：生产与 Core 探针通过，整轮验收进行中

- W01：首组双配置真实 Core 探针确认三项风险，已准备修复与严格回归：覆盖前 Define 导入值比较、每流重绑引用/闭包、按 checksum+预期长度统一恢复链接三文件组。未宣称 CLI 边界已复现。
- W02 部分：按流输出 8 个阶段的调用/失败次数及单调时钟耗时；与 legacy MetricManager 独立，快照不随后续工作变动。
- W03 部分：新文件/时间戳变化合并 SHA256 + legacy FastHash 读取，暖路径保留 SHA-only；同时间戳内容变化仍重算。保留原一 MiB 分块、持久化字段与 Reset 内容复核；尚无跨流共享快照。
- W08 部分：NodeJoiner 调用内有序子节点索引，小列表保留扫描；准备原 SDK DLL 差分和逻辑工作量比较。尚未实现 AST 缓存或引用索引。
- W04 部分：每 SchemaSet 代复用自有只读引用描述，保留声明上下文、实际类型、注解优先级和完整 schema 内容复核；提供回退与 build/hit 计数，不宣称跨流共享 schema。
- 已接入现有 Core lifecycle Actions 的新分支和新增 fixtures；保留原生产、原 BAB、缓存、mymod、BIG 与独立发行 ZIP 验收。下一轮在 Core 步骤结束时提前上传小型探针证据。

## 验证与下一步

本地只编辑和静态阅读；所有测试/编译通过 GitHub Actions。
首组源码 `18ae05cace7099bbd65fa6632dcfd7dd383bdded` / [运行40459453132](https://github.com/wu162/NewSdk/actions/runs/40459453132) 整体 failure：生产、摘要、旧Core/CLI/缓存/Texture/BIG组通过；NodeJoiner 80个原DLL差分通过但直接加载原Core触发宿主 ownership 拒绝，测试已改独立可卸载 AppDomain，归属断言保留。完整 mymod/最终 ZIP 因前置失败未执行。
当前生产/测试源码 `75601909b0e874bc24786f1d6dd3d8c64eb1aee6` / [运行40461583024](https://github.com/wu162/NewSdk/actions/runs/40461583024)：生产与两配置各13个 Core fixtures、原 BAB 基线和协议通过；后续组进行中，不标整轮成功。
提前上传 artifact `12714883356` 的 W01 严格回归、原 DLL 隔离对照、schema 与摘要均通过。微基准及已知边界见 [VALIDATION.md](VALIDATION.md)，不报告完整项目提速。
先完成当前批次差分与暖路径成本评估；若有新修复，集中重新验证受影响组。W04—W10 后续按依赖推进，不提前开放未经验证的新缓存或并行。
