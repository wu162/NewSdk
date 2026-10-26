# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理。本轮按用户授权最多 5 个子代理推进；未合入其他分支。

## 当前状态（先读这里即可）

- **P4 已通过，P5 首批后端验证通过，真实 CLI 候选已接入待验收。** 候选尚不代表正式可用；若真实入口验收未过，结束前恢复默认 guard。完整 P5/G3/SDK 未完成。
- `62b7766` XML 专项已终态成功并核对原始收据：Debug/Release 各完整 63 组、四平台 0/1/2/99、原有布局/拒绝及三缓冲契约全部通过；126 份原版 transcript 的摘要、序号、平台、终态与调用数均匹配。
- 同次部署、XML 联合 smoke、生产/宿主两份 EXE、95 项非入口链接输入及配置/ANTLR 记录一致。标准 artifact 未含 EXE/object/runtime 原件，不将收据核对说成重新哈希这些二进制。
- `9cd2cb8` 已核对的 Foundation、Armor/Hash、GameObject、Weapon、音频 13 组、W3X capture、贴图旧探针及完整生产对象贴图继续按原提交/范围复用；至 `62b7766` 生产/链接/宿主输入未变，未重复 SDK。
- 新增 `CheckCompilerXmlEvidence.py` 只读核对归档收据和原始日志，已对真实 XML artifact 通过；16 项临时 Git 合成证据回归通过。Windows 源码检出为 CRLF，仅已知源码接受 Git 原字节或纯 LF→CRLF 两种候选，原 DLL 仍严格固定原字节；不证明证据可复用到当前 HEAD。
- `6bc692af` 完整新生产组合、Debug/Release 四类 lifecycle/backend fixture、当前 EXE 绑定贴图实际 payload/部署拒绝均通过并核对原始身份；原项目断言已证实是 fixture 混合斜杠缓存查询，保留成功断言并补独立流证明。
- 实验后端已恢复原 XML 默认 hash/version 且未知路由仍拒绝，验证独立 manifest/Armor 字节布局；报告/Hash/Tracer/Settings、主错误及租约恢复已有定向证据。该首批未验生产入口；不把小 Armor fixture 当完整 G3/SDK。
- 后续 Debug 前四 fixture 与两配置 P2（各 310 断言）、部署/贴图成功；App 路径测试先暴露过长测试 workspace，再暴露 ShPath 默认 ANSI 将中文流名变为 `?` 的生产缺陷，App/原 BAB/协议仍未完整通过。
- 已只修 Core 实际使用的 PathCanonicalizeW/PathRemoveFileSpecW，保留中文用例和 MAX_PATH 限制；显式 Trace scope 也不再受旧 BabEnableTrace 环境静默绕过。两修复须新组合验收。
- 候选接入 monolithic build 与准确只读 doctor；原有日志桥/状态隔离/无缓存能力不变。专项将直接运行干净生产 EXE，核对冷进程/原 BAB 四流、SDK 内外中文 MOD、真实 CTRL_BREAK 及单进程 job 限制，不通过则回退入口。仅隔离 staging，缓存、发布、完整业务覆盖仍未验收。

## 下一步（按顺序，先取证再修改）

1. 查当前分支最新**实际执行** Core lifecycle：分别核对五 fixture、原 BAB、App 协议、真实生产 CLI/冷进程/CTRL_BREAK、CLI 协议、Texture、P2 两配置；工作流总体状态不能替代每项实际结果，失败只读相关日志。
2. 首批四 fixture 有效证据保留原范围，第二批新增处理器状态与真实 App/原版对照仍待验。完成并验证生命周期基础组：真实 Core include/继承/项目配置失败后重试、递归早退与循环拒绝；Hash/Tracer 原状态恢复；listener 和报告 writer 故障后资源释放。每个测试通过生产对象宿主，不放回业务 DLL 或 stub Core。
3. 核对候选入口验收/是否回退；后端 `src/App/Application/Model.cs` 的 `IStreamCompiler` → `src/Managed/Core/BinaryAssetBuilder.Core/DocumentProcessor.cs:ProcessDocument`。旧 `DoBuildData` 只参考初始化顺序，不复制进程退出假设；每流新 Settings/cache/processor，失败 cache 丢弃，顺序租约和全局状态恢复明确。
4. 保持显式内建 compiler/verifier、SDK schema/搜索根和隔离输出，错误/取消进入统一事件报告；默认入口必须有真实生产进程证据，不因候选源码存在而宣称启用成功。完整 G3、多流与冷进程字节对照、缓存失效/取消后复用及 SDK 验收仍未完成。
5. 复用匹配输入的原始证据；P5 改动后的生产组合须另行验证，不能把 `62b7766` 的 EXE 身份冒用给新组合。无需重复无变化的完整 SDK 基线。

## 按任务读取，禁止启动时递归展开链接

| 问题 | 对应入口（除工作流外相对 recovery） |
| --- | --- |
| 失败/工作流 | `.github/workflows/refactor-native.yml`、`refactor-xml-product.yml`、`refactor-guards.yml` + 对应脚本 |
| 宿主身份 | `scripts/BuildCompilerValidationHost.ps1`、`scripts/CompilerValidation.Common.ps1`、`tests/CompilerValidationEntry.cpp` |
| 生产部署/管理行为 | `scripts/TestCompiler.ps1`、`scripts/TestCompilerFoundation.ps1` |
| Tokenizer/音频 | `scripts/TestCompilerDifferential.ps1`、`TestCompilerGameObject.ps1`、`TestCompilerWeapon.ps1`、`TestCompilerAudio.ps1` |
| W3X/贴图 | `scripts/TestW3xProduct.ps1`、`scripts/CheckW3xCaptureInputs.py`、`scripts/TestCompilerTexture.ps1` |
| XML 全矩阵/smoke/收据 | `scripts/TestCompilerXmlMatrices.ps1`、`scripts/PrepareCompilerXmlMatrices.py`、`scripts/CheckCompilerXmlEvidence.py`、`scripts/TestCompilerXmlSmoke.ps1` |
| P5 生命周期 | `P5_LIFECYCLE_AUDIT.md`，完成情况以源码和实际作业为准 |
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
