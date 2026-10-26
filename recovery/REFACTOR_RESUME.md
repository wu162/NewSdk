# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理。本轮按用户授权最多 3 个子代理推进；未合入其他分支。

## 当前状态（先读这里即可）

- **P4 已通过；P5 真实后端、路径、生产 CLI 定向组已通过，完整 P5/G3/SDK 未完成。** 生产入口保留实验性 Win32 loose/no-cache 后端；不能宣称完整 MOD 编译器或正式发布流程可用。
- 本轮完整 Windows 候选两配置均通过：App 各355断言、Core 全7组（含 Unicode/空格/#/% XInclude、根边界、空搜索路径）、原版后端对照、真实 App 协议、当前 EXE 绑定 Texture 部署/payload。
- 真实 CLI 各22个独立 PID 已过：单进程 job/no-child、冷分组、内外 MOD、实际 CTRL_BREAK/exit6/retry、真实日志锁/exit7/同路径重试、参数拒绝与协议。每配置19 reports/609 events；独立原版严格比较原始四输出，未归一化字节。
- 真实业务新增12个文件型 APT 资产、3个 MapMetaData 资产；组合与三组冷进程输出一致，APT/maps 独立当前原 BAB 对照已过。这不是 APT 格式解析或完整 UI/SDK 验收。
- 真实 Upheaval CommandListBox UI 两配置均已通过：固定祖先按原 blob 导出 wrapper/子XML/APT/CONST/DAT 完整5文件、4依赖闭包，生产 apt-only 单流及当前原 BAB 原始四输出严格一致；另两份含纹理的 UI 未在此范围。
- 根/路径、日志原子替换与活动文件保护、晚期失败撤销产物、.build junction 与既有 staging 所有权修复均已取得上述完整候选的原始收据；已核对归档 SHA、14份 fixture 的结果/日志/EXE 和两份协议原始文件哈希，详见 PATH_AUDIT.md。
- `62b7766` XML 两配置各63组、`9cd2cb8` 非XML组件证据仅按原输入/范围复用，不将旧生产/宿主身份冒充当前重测；本轮未重复完整 SDK。

## 下一步（按顺序）

1. 继续下一完整能力组，复用本轮原始成功证据及实际输入闭包；本轮 UI 扩展已成功并核对原始归档，不能把历史 sourceCommit 的收据改标为新提交重测。
2. 另两份真实 UI 含纹理 CustomData；先扩展精确输出允许清单及原版比较器，再接入，不能删除 Texture 迁就现有四流断言。原混合 Texture DLL 的完整 Unicode 路径仍未验。
3. 缓存、正式输出发布、完整业务 G3/SDK、P6/P7 仍未完成；旧不支持参数继续明确拒绝，当前输出仅隔离 staging。
4. CLI helper 已提前廉价编译；稀疏检出仅排除约2GiB历史 evidence，保留所有源码/SDK/Git历史与完整验证。新增 PrepareCompilerCliReuse.py 及16个负例/闭包测试，已用两份真实成功归档核验，只提取两配置各3运行文件，保留历史来源、candidateValidated=false；尚未接入 Actions 自动下载/执行。旧 CLI-only 锚点须更新且调用严格闭包，不允许任意 workflow/build/env 变更。

## 按任务读取，禁止启动时递归展开链接

| 问题 | 对应入口（除工作流外相对 recovery） |
| --- | --- |
| 失败/工作流 | `.github/workflows/refactor-core-lifecycle.yml`、`refactor-native.yml`、`refactor-xml-product.yml`、`refactor-guards.yml` + 对应脚本 |
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
