# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理。本轮按用户授权最多 5 个子代理推进；未合入其他分支。

## 当前状态（先读这里即可）

- **P4 已通过，P5 生命周期基础组和实验后端已保存，Windows 验证未通过，CLI 尚未接入。** 生产 `build` 仍返回 `BACKEND_NOT_INTEGRATED`（退出码 3），不能宣称可构建真实 MOD。
- `62b7766` XML 专项已终态成功并核对原始收据：Debug/Release 各完整 63 组、四平台 0/1/2/99、原有布局/拒绝及三缓冲契约全部通过；126 份原版 transcript 的摘要、序号、平台、终态与调用数均匹配。
- 同次部署、XML 联合 smoke、生产/宿主两份 EXE、95 项非入口链接输入及配置/ANTLR 记录一致。标准 artifact 未含 EXE/object/runtime 原件，不将收据核对说成重新哈希这些二进制。
- `9cd2cb8` 已核对的 Foundation、Armor/Hash、GameObject、Weapon、音频 13 组、W3X capture、贴图旧探针及完整生产对象贴图继续按原提交/范围复用；至 `62b7766` 生产/链接/宿主输入未变，未重复 SDK。
- 新增 `CheckCompilerXmlEvidence.py` 只读核对归档收据和原始日志，已对真实 XML artifact 通过；16 项临时 Git 合成证据回归通过。Windows 源码检出为 CRLF，仅已知源码接受 Git 原字节或纯 LF→CRLF 两种候选，原 DLL 仍严格固定原字节；不证明证据可复用到当前 HEAD。
- P5 首轮实际构建/部署通过；Core fixture 在项目 Good 配置的缓存状态成功断言处失败（原始工作树已有 `root_G` 四件套，已定位为 fixture 混合斜杠查询缓存漏配，路径规范化及独立资产/流证明修复待新组合验证），后续 Resource/State 及 Release 尚未验收，不能宣称基础组完成。
- 最后检查点补 Hash 输出失败释放句柄、Core metrics 次级异常保留主错误及回归；新增实验 `CoreStreamCompiler.cs`、backend fixture 和绑定新宿主的贴图验证入口，均未取得 Windows 成功证据。CLI 仍传 null。
- 实验后端已补真实 XML 默认插件的 all-types/version 元数据视图，未知路由仍拒绝；新增独立 manifest/Armor 字节及破坏拒绝断言、调用者 AssetReport 作用域恢复、主异常优先和租约修复，均待 Windows。原版 BAB 小型四流基线正在准备；完整参数、缓存、日志和 G3/SDK 未验收。

## 下一步（按顺序，先取证再修改）

1. 先查当前分支最近**实际执行** guard/Core lifecycle，定位项目配置成功证明的失败根因；原始四流已输出，不直接删除断言。新组合已准备显式 `[refactor-core-lifecycle]` 集中运行，必须核对实际终态，不把提交/启动算通过。
2. 最后检查点仅静态 diff 检查通过，未编译新增后端。完成并验证生命周期基础组：真实 Core include/继承/项目配置失败后重试、递归早退与循环拒绝；Hash/Tracer 原状态恢复；listener 和报告 writer 故障后资源释放。每个测试通过生产对象宿主，不放回业务 DLL 或 stub Core。
3. 接入 `src/App/Application/Model.cs` 的 `IStreamCompiler` → `src/Managed/Core/BinaryAssetBuilder.Core/DocumentProcessor.cs:ProcessDocument`。旧 `DoBuildData` 只参考初始化顺序，不复制进程退出假设；每流新 Settings/cache/processor，失败 cache 丢弃，顺序租约和全局状态恢复明确。
4. 后端须显式配置内建 compiler/verifier、SDK schema/搜索根和隔离输出，错误/取消进入统一事件报告；不能只把 CLI 的 null 换成后端。完整 G3、多流与冷进程字节对照、缓存失效/取消后复用及 SDK 验收仍未完成。
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
