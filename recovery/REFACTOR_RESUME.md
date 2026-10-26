# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理。本轮仅快速交接，未继续实现或触发验证。

## 当前状态（先读这里即可）

- **P4 验收未完成，P5 未接入。** 生产 `build` 仍返回 `BACKEND_NOT_INTEGRATED`（退出码 3），不能宣称可构建真实 MOD。
- 本次核对的最新实现为 `0500d4a`；后续文档提交不代表重新验证业务代码。
- `7ace438` 已实现独立验证宿主、生产链接输入清单及 Armor/Hash、GameObject、Weapon 接线。旧交接的“宿主尚未实现”已失效，**不要重复实现**。
- `0500d4a` 已修复 GameObject 缺少 Base.xsd、Weapon 日志参数绑定、贴图预期拒绝退出码泄漏；W3X 历史 capture 下载/重放与 XML smoke 已接入工作流。
- 该实现对应的 native integration 运行在本次交接检查时仍进行中：Foundation 步骤成功，Debug/Release 生产检查进行中，其余组件待执行。**这是时间点快照，续接必须刷新状态。**
- 上一提交记录称生产部署及 Armor/Hash Debug/Release 已过；这里只核对了提交记录，未重新读取原始收据，不把它升级为当前提交通过。

## 下一步（按顺序，先取证再修改）

1. 查本分支 `.github/workflows/refactor-native.yml` 最近运行，匹配上述实现提交；仅输出 SHA、状态和失败步骤。不要因聊天中断另起重复运行。
2. 读取该运行 artifact 中相关 `validation.json`、生产/宿主 `build.json`；失败时只取相关日志尾部。核对 sourceCommit、配置、生产与宿主身份。运行成功也不等于 P4 全部验收。
3. 只修实际失败项；若全部已通过，再推进 XML 受影响组完整边界/拒绝差分。联合 smoke 不能替代各组完整矩阵；P4 门槛满足前不冒进 P5。
4. 复用有效历史证据；链接变化的新组合须验。无需重复完整 SDK 基线，确有必要时集中验证。

## 按任务读取，禁止启动时递归展开链接

| 需要解决的问题 | 只读取对应入口 |
| --- | --- |
| 当前失败/工作流接线 | `.github/workflows/refactor-native.yml` + 失败步骤对应脚本 |
| 宿主身份或输入一致性 | `scripts/BuildCompilerValidationHost.ps1`、`scripts/CompilerValidation.Common.ps1` |
| 生产部署 | `scripts/TestCompiler.ps1`、`tests/CompilerDeployment.cs` |
| 组件失败 | `scripts/TestCompilerDifferential.ps1` / `TestCompilerGameObject.ps1` / `TestCompilerWeapon.ps1` / `TestW3xProduct.ps1` / `TestLegacyTextureAdapter.ps1` 中相关一项 |
| XML 覆盖 | `scripts/TestCompilerXmlSmoke.ps1`、`scripts/PrepareCompilerXmlSmoke.py`；再定位具体组 |
| 阶段验收定义 | `NewSdk_ModCompiler_Refactor_Plan.md` 中 P4/P5 相关段落 |
| 历史实现背景与详细约束 | `REFACTOR_HANDOFF_ARCHIVE_20261026.md`（历史快照，非当前待办） |
| 效率/保存检查点细则 | `EFFICIENCY.md` 对应章节 |

表内路径相对 recovery，工作流路径相对仓库根。

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
