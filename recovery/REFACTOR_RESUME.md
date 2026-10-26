# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理。本轮按用户授权最多 5 个子代理推进；未合入其他分支。

## 当前状态（先读这里即可）

- **P4 已通过；P5 后端/真实 App 定向验收已过，生产 CLI 完整组未过，默认 guard 已恢复。** 生产 build 仍返回 BACKEND_NOT_INTEGRATED，不能宣称完整 MOD 编译器可用。
- `62b7766` XML Debug/Release 各63组及原始 transcript/生产宿主身份已核对；`9cd2cb8` 的有效非XML组件证据按原输入/范围复用，不重复完整 SDK。
- `817b127e` 两配置五 fixture、原 BAB 两 MOD 四流逐字节对照、真实 App 协议（每配置15 reports/489 events）、Texture与P2均通过并核对原始收据；不是完整 G3/SDK。
- `6767870a` Debug 14个真实 CLI case 已过（14独立PID、单进程job/no-child、冷分组、内外MOD、实际CTRL_BREAK/exit6/retry）；独立原版对照失败：bin/imp/relo相同，manifest 源文件表旧ANSI为?? MOD绝对路径，新版为DATA:mod.xml。Release CLI/CLI协议未验，不能归一化字节掩盖差异。
- 最后已将 Program/Help/后端doctor/负例部署探针/构建收据回退到已保存守卫基点，候选实现仍保留在上述 git 提交可恢复；Unicode W、Trace scope、状态隔离、日志桥与测试准备保留。
- 集中路径审计发现新的高影响缺陷（目录前缀/尾斜杠、另一个XInclude ANSI接口、日志junction/hardlink输入覆盖等），详见 PATH_AUDIT.md。尚未修完，因此即便部分CLI case通过也不能默认启用。

## 下一步（按顺序）

1. 先读 PATH_AUDIT.md，集中修根边界/安全相对路径、XInclude Unicode、空/null搜索路径、日志物理别名和诊断文件冲突；保留中文/空格用例，先补必要 Windows 路径矩阵。
2. 保留已有原始证据并按实际输入闭包复用。817的强前置与676的Debug部分CLI结果分开，不能把缺失的Release/CLI协议补成通过。
3. 为原版严格对照新增同内容ASCII companion，继续验证Unicode新路径语义；不要删除Unicode测试或改写manifest。修复后恢复候选并跑真实生产入口两配置、原版/冷进程/取消/日志故障及协议；完整验收前保留guard。
4. 缓存、正式输出发布、完整业务G3/SDK、P6/P7仍未完成。当前仅Win32无缓存loose隔离staging，旧不支持参数明确拒绝。
5. 工具先轻量编译再长链接；运行允许清单二进制已加入未来产物归档，但自动下载复用尚未实现。CLI-only闭包守卫拒绝生产/SDK/其他fixture变化，不以跳过回归优化耗时。

## 按任务读取，禁止启动时递归展开链接

| 问题 | 对应入口（除工作流外相对 recovery） |
| --- | --- |
| 失败/工作流 | `.github/workflows/refactor-native.yml`、`refactor-xml-product.yml`、`refactor-guards.yml` + 对应脚本 |
| 宿主身份 | `scripts/BuildCompilerValidationHost.ps1`、`scripts/CompilerValidation.Common.ps1`、`tests/CompilerValidationEntry.cpp` |
| 生产部署/管理行为 | `scripts/TestCompiler.ps1`、`scripts/TestCompilerFoundation.ps1` |
| Tokenizer/音频 | `scripts/TestCompilerDifferential.ps1`、`TestCompilerGameObject.ps1`、`TestCompilerWeapon.ps1`、`TestCompilerAudio.ps1` |
| W3X/贴图 | `scripts/TestW3xProduct.ps1`、`scripts/CheckW3xCaptureInputs.py`、`scripts/TestCompilerTexture.ps1` |
| XML 全矩阵/smoke/收据 | `scripts/TestCompilerXmlMatrices.ps1`、`scripts/PrepareCompilerXmlMatrices.py`、`scripts/CheckCompilerXmlEvidence.py`、`scripts/TestCompilerXmlSmoke.ps1` |
| P5 生命周期/路径 | `P5_LIFECYCLE_AUDIT.md`、`PATH_AUDIT.md`，以源码和实际作业为准 |
| 阶段定义/历史/效率 | `NewSdk_ModCompiler_Refactor_Plan.md` P4/P5；`REFACTOR_HANDOFF_ARCHIVE_20261026.md` 按需；`EFFICIENCY.md` |

## 不可丢失的边界

Windows x86 / .NET Framework 4.8；恢复业务实现物理链接到 EXE，保留必要 ANTLR。
生产 EXE 不含测试入口；独立宿主复用生产对象，分别记录两份 EXE 身份；不得改写 TLS/IAT 或放回业务 DLL 绕过加载限制。
资源、签名、路由边界与贴图固定原件审计保持原标准。详细约束按相关任务从历史快照读取。
不在文档新增运行编号、指纹或产物链接；从分支 Actions 查询原始证据。

## 上下文预算与下次交接

- 首轮只读根 AGENTS.md + 本页，再查询分支头/运行摘要；不要默认加载恢复线 RESUME、全量 STATUS、计划全文、历史交接或整份 PR diff。
- 工具结果先解析/筛选再输出；提交仅摘要和相关文件，日志先尾部 60–100 行；文件按函数/行段读取，缺证据再扩展。
- 每次只展开当前阻塞所需 1–3 个文件；技术细节不重复粘贴到入口。
- 本页保持约 60 行以内：覆盖更新当前状态、下一步和必要边界，禁止追加流水账；历史保存在 git 或按需文档。
- 中断前及时保存检查点，明确已提交/未验证/验证中/已验证。不能恢复旧会话未提交内容时直说，不推断其已经保存。
