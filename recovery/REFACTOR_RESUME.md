# MOD 编译器重构：最小续接入口

更新：2026-10-27（北京时间）。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在本分支普通提交；禁止强推、未经用户批准合入；子代理数量以本轮用户指示为准。

## 当前状态

- **P4 已通过；P5 未完成。** 事务式 loose 发布及此前真实 CLI 发布整组已通过。新增 `/bc` 和输入根的本轮范围已通过 Debug/Release；完整会话缓存、SDK/G3 和正式发行包仍未完成。
- `/bc:true` 为真正的 Core 资产缓存：恢复 audio/W3X 可复用，混合 Texture 仍正常编译且不复用；每流 SessionCache 仍冷建。默认缓存关闭，`/sc`、压缩/部分/冻结缓存、旧缓存目录覆盖仍拒绝。
- 新命名空间按完整 SDK/MOD/runtime 内容、任务/参数及进程解析合同隔离；缓存模式实际约束 XML/Include/XInclude/FileReference/Schema 读取根。根外引用明确拒绝，输入快照本身仍不能授权复用。
- 缓存使用短目录 v2 与数字 payload，完整清单/SHA256核验；已确认条目才可复用。与发布一起可逆提交，必需报告/终态/日志失败撤销；pending 遗留始终是 miss，回滚失败输出保留位置。
- `/dr /data /art /audio` 已按原 SettingsLoader 顺序接入：显式列表替换默认，缺省保留 MOD 前置，顺序/空格/`*` 不改。报告含 inputRoots 与来源；CLI 在打开日志/清除报告前预检输入重叠。无缓存允许合法外部根，缓存模式拒绝未覆盖外部根。
- 内容失效保留游戏 FastHash，另用 SHA256 覆盖同长度同 mtime 修改。Windows 新回归发现“删除后同字节同时间重建返回零 Hash”并已修复；原失败及加强回归保留，两配置内容组已复验通过。
- 本轮 App 各667断言、生产组合、Core全10组、Texture、原 BAB、真实 CLI 和输入根整组通过两配置。最终缓存31 reports/1501 events、Audio CDATA/W3X重放12 reports/348 events、输入根7 reports/183 events，均每配置零协议错误。
- 既有 Core 全组、原 BAB、APT/MapMetaData、三份真实 Upheaval UI 106文件/103依赖闭包证据按未变化范围复用。UI Texture 是内联 DDS，不是 cdata；不代表完整 UI/runtime 或 SDK。
- 中文纹理根仍受旧混合 DLL 的 LoadImageA/D3DX 限制；已有 exit4/空产物证据，不重复下载器审查或无依据路径排列。
- XML 63组及非 XML 组件历史证据只按原 sourceCommit/输入范围复用，不冒充当前生产重测；本轮未重复完整 SDK。

## 下一步（按顺序）

1. **本轮资产缓存和输入根已收尾，直接推进 `/sc`。** 复用未变化的生产/Core/Texture/CLI/原版及缓存成功证据；仅相关实现变更、实际失败或具体缺口才补受影响组，不重复相同输入。
2. **实际 `/sc` 会话缓存。** 具体接入在 P5_LIFECYCLE_AUDIT.md：每流 fresh SessionCache.LoadCache→InitializeCache→新 processor，独立包持久 session.xml/intermediate/cdata，重新生成四输出；不得裸共享 Complete 文档或只存 session XML。
3. `/sc` 增加真实文档复用/解析/实例编译计数，坏包/缺资产整包冷失效；三类混合 Texture 暂明确拒绝 `/sc`，保留 `/sc:false` 编译。覆盖 include/XInclude、定义/表达式、继承、同 mtime 变化、连续 MOD、失败取消/收尾失败重试及原始输出对照。
4. 缓存及输入根功能稳定后集中完成完整 P5/G3/SDK，再推进 P6/P7。BIG/skudef、正式发行包、完整 Unicode 纹理和完整 SDK 均不得由本轮小型组推断完成。
5. 最终缓存定向重放已通过：生产源码/SDK树未变，固定原始归档、成功范围及运行文件哈希，分别记录二进制与测试来源，不冒充当前重编译。原组合运行的两次测试工具失败保留；已修复配置匹配及旧进程父PID重用误报，未削单进程/后代拒绝断言。旧 CLI-only 准备器仍钉住旧源，不直接适用当前缓存；窄重放不宣称原失败运行整体成功。

## 按任务读取

| 事项 | 入口 |
| --- | --- |
| P5 缓存/输入根/会话下一组 | `recovery/P5_LIFECYCLE_AUDIT.md`、`src/App/Application/CoreStreamCompiler.cs`、`BuildAssetCache.cs` |
| 当前 Windows 验证 | `.github/workflows/refactor-core-lifecycle.yml`、`TestCompilerCoreLifecycle.ps1`、`TestCompilerBuildCache.ps1`、`TestCompilerInputRoots.ps1`、`TestCompilerBuildCachePayloads.ps1`、`.github/workflows/refactor-cache-replay.yml`、`PrepareCompilerReplay.py`（脚本在 recovery/scripts） |
| 既有路径审计 | `recovery/PATH_AUDIT.md`；只在相关变更、实际失败或具体缺口时补定向测试 |
| 阶段/效率 | `recovery/NewSdk_ModCompiler_Refactor_Plan.md` P5/G3、`recovery/EFFICIENCY.md` |

## 不可丢失的执行约束

Windows x86 / .NET Framework 4.8；恢复业务代码物理链接 EXE，保留必要 ANTLR。生产 EXE 不含测试入口；独立宿主复用生产对象，分别绑定两份 EXE 身份；不得放回业务 DLL 或改写 TLS/IAT 绕过加载限制。
一次启动后持续推进本轮已授权工作；还有时间就推进下一项依赖满足的能力。等待 Actions 可做独立工作或合理等待，低频查询直到终态；保存检查点不等于提前结束，不让用户反复启动。
每次交接在对话中直接说明下一步能力、验证和未完成事项。首次只读 AGENTS.md、本页与分支/运行摘要，再按阻塞读取1–3文件；不默认展开历史与全计划。入口约60行，不追加流水账，不新增运行编号/指纹/产物链接；原始证据从分支 Actions 查询。
