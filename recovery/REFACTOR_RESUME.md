# MOD 编译器重构：最小续接入口

更新：2026-10-27（北京时间）。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在本分支普通提交；禁止强推、未经用户批准合入；子代理数量以本轮用户指示为准。

## 当前状态

- **P4 已通过；P5 未完成。** `/sc` 生产候选已接入，App 两配置已通过；当前完整生产构建与实际会话缓存矩阵仍待终态，不标通过。
- `/bc:true` 既有资产缓存与 `/dr /data /art /audio` 输入根组已通过两配置；本轮生产输入变化后正在集中重建并复验受影响组，历史成功范围仍按原二进制/测试来源记录。
- `/sc:true` 每流还原独立包的 session.xml/intermediate/cdata，以 fresh SessionCache.LoadCache→InitializeCache→新 processor 处理，重新生成四输出；不共享 Complete 文档。`/bc` 与 `/sc` 可独立启用，默认均关闭。
- 会话包有完整清单/SHA256、布局/XML检查；坏包、结构或版本失效整包冷编译并可逆修复，失败恢复原条目。缓存与发布、必需报告/终态/日志共同确认，pending 遗留始终 miss，回滚失败保留位置。
- 报告实际文档复用、XML解析、实例编译及会话包hit/miss，不能把目录命中当真实复用。三类混合 Texture 暂明确拒绝 `/sc`，`/sc:false` 保留编译；Texture 仍不授权资产缓存复用。
- 首轮 App 两配置通过，但生产链接 MSVC C1001，未产生可复用 EXE；已等价移除 App 新增异常过滤器，修复是否有效以新生产构建终态为准，不能复用失败产物。
- 实际 `/sc` CLI 组及链接前 fixture 预检已准备：Include/XInclude、定义/表达式、继承、同 mtime、多入口/双MOD、损坏修复、失败取消收尾重试、Audio CDATA/W3X、原BAB四输出和 `/sc+/bc` 独立计数。未执行范围不标完成。
- 完整未精简 mymod 的原 BAB 先行及两配置生产对照已接入，缓存组成功后集中运行；当前 mymod apt/maps 无入口，明确记录空集合，不伪造完整 UI/runtime。
- P6 最小五文件 ZIP 发布脚本已实现准备，独立解压后 doctor/真实小MOD/协议/原始四输出对照尚待 Windows；不把准备脚本当正式发行包验收。
- 既有 Core/原 BAB、APT/MapMetaData 和三份真实 Upheaval UI 106文件/103依赖闭包证据仅按未变化范围复用。UI Texture 是内联 DDS，不是 cdata；中文纹理仍受旧混合 DLL LoadImageA/D3DX 限制，不重复无依据路径审查。
- XML63组及非XML历史证据按原 sourceCommit/输入范围复用，不能冒充当前生产重测。完整 P5/G3、完整 SDK、完整 Unicode 纹理、BIG/skudef 与正式发行包均未宣称通过。

## 下一步（按顺序）

1. 跟进当前 Windows 生产组合至终态，集中修复实际失败；生产输入改变就重建受影响组，禁止盲重试或用旧二进制掩盖新代码。只改测试时须先验证完整生产闭包与归档身份，分别记录二进制/测试来源。
2. 完成实际 `/sc` 冷/热、失效、事务和原始输出矩阵；缓存组稳定后取得完整 mymod 原版/生产 SDK 对照，再核对 P5/G3 完成条件。
3. 依赖满足就运行 `PublishCompiler.ps1` 的正式最小包及独立目录验收，随后推进 P7 必要后处理/BIG/skudef。还有本轮时间则继续下一项，不因保存检查点提前结束。
4. `PrepareCompilerSessionReplay.py` 与其工作流当前仅未启用准备，仍钉住没有生产产物的失败源并保持空归档pin；不能直接触发。若确需重放，必须先按实际成功生产范围重设完整source/run/archive/失败集并审阅输入闭包；否则清理该准备。

## 按任务读取

| 事项 | 入口 |
| --- | --- |
| P5 缓存与事务 | `recovery/P5_LIFECYCLE_AUDIT.md`、`recovery/src/App/Application/CoreStreamCompiler.cs`、`SessionPackage.cs`、`BuildAssetCache.cs` |
| Windows 受影响组 | `.github/workflows/refactor-core-lifecycle.yml`、`recovery/scripts/TestCompilerSessionCache.ps1`、`TestCompilerSdk.ps1` |
| P6 正式最小包 | `recovery/scripts/PublishCompiler.ps1`、`recovery/COMPILER_RELEASE.md` |
| 既有路径/阶段/效率 | `recovery/PATH_AUDIT.md`、`recovery/NewSdk_ModCompiler_Refactor_Plan.md` P5/G3/P6/P7、`recovery/EFFICIENCY.md` |

## 不可丢失的执行约束

效率优先，正确性是底线；已明确行为成组实现并接入生产，调查、工具和文档只服务当前交付。先廉价预检，独立组集中验收，严格复用未变化证据，保留原始差分及SDK门槛。
Windows x86 / .NET Framework 4.8；恢复业务代码物理链接 EXE，保留必要 ANTLR。生产 EXE 不含测试入口；独立宿主复用生产对象，分别绑定两份 EXE 身份；不得放回业务 DLL 或改写 TLS/IAT 绕过限制。
一次启动持续推进本轮已授权工作，低频跟进 Actions 到终态；不让用户反复启动。每约10–15分钟保存有意义检查点，未验证不能标完成。
交接在对话中直接说明新增能力、验证、真正阻塞和下一步。入口约60行，不追加流水账、运行编号/指纹/产物链接；原始证据从分支 Actions 查询。
