# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理。本轮按用户授权最多 5 个子代理推进 P4，未合入其他分支。

## 当前状态（先读这里即可）

- **P4 仍差 XML 完整矩阵，P5 未接入。** 生产 `build` 仍返回 `BACKEND_NOT_INTEGRATED`（退出码 3），不能宣称可构建真实 MOD。
- `9cd2cb8` 原始收据已核对：生产部署、Foundation 行为、Armor/Hash、GameObject、Weapon、音频 13 组、W3X capture 重放、贴图旧探针及完整生产对象贴图、XML 联合 smoke，均为 Debug/Release 成功；sourceCommit、配置、两份 EXE、链接输入清单一致。该作业红状态来自后述已修复的测试工具问题，不代表这些组件失败。
- 宿主 schema=2，绑定 config/ANTLR 哈希并拒绝陈旧目录。独立 Windows guard 已通过，编码修复后的后续 guard 也通过；W3X 仅在双端依赖闭包不变时复用历史 capture，未重跑 SDK。
- `c603bcd` 已修 Framework csc transport 源路径。XML 专项实际通过 **Debug 29/63 组、Release 0/63**，在 Locomotor `PrefixNavigation` 的“原版未知 TypeId 应返回 null”用例中，测试 transport 未编码空结果导致 EOF；不是已证实的业务算法差异。
- 本轮最后已补 nullable AssetBuffer 帧及空/非空/非法 tag 回归，并补异常时原版进程日志保存；两分支 Core stub 本地语义/往返测试通过，**Windows 全矩阵复验未完成**。transport 已变，29 组旧通过不得标成修复后版本通过，需全 63 组两配置复验。
- XML 专项入口 `.github/workflows/refactor-xml-product.yml`，消息 `[refactor-xml-product]`；它构建完整生产组合，只验部署、XML smoke/全部 63 组，不重复其他组件或 SDK。XML job 90 分钟、单组 900 秒；全 Native job 120 分钟，避免完整矩阵被旧 60 分钟上限截断。结束时以最新实际运行状态为准，不把提交/启动当通过。

## 下一步（按顺序，先取证再修改）

1. 查本分支 `refactor-native.yml`、`refactor-xml-product.yml`、`refactor-guards.yml` 最近**实际执行**运行，匹配各自实现/修复提交；只输出 SHA、状态、失败步骤。未终结时不要重复相同输入；旧 Native 的 guard 编码/路径红状态须结合已修复专项证据判断，勿因此重复全矩阵。
2. 优先只读最新 XML 的 `validation.json`、生产/宿主 `build.json`，核对 sourceCommit、配置、两份 EXE 身份和运行文件；失败只取相关组日志尾部。`9cd2cb8` 已核对成功项无相关变化时直接复用；工作流成功不自动代表 P4/G3/SDK 验收。
3. 集中修实际失败；完整 XML 需全部 63 组、两配置、四平台及原有布局/拒绝矩阵，不以联合 smoke 替代。已知 Locomotor/FXParticle 为单三缓冲比较，按原组契约验收。
4. 生产对象贴图 payload/部署拒绝已核对成功；保留固定原件及 PE 审计，验证宿主不得笼统放行业务 DLL。P4 门槛满足后才推进 P5。
5. 复用匹配输入的原始证据；链接/宿主变化的新组合须验证，无需重复完整 SDK 基线。
6. P5 只读定位已完成，未实现：`src/App/Application/Model.cs` 的 `IStreamCompiler` 接 `src/Managed/Core/BinaryAssetBuilder.Core/DocumentProcessor.cs` 的 `ProcessDocument`；旧初始化顺序见 `decompiled/BinaryAssetBuilder/BinaryAssetBuilder/BinaryAssetBuilder.cs` 的 `DoBuildData`。先处理 Settings/HashProvider 静态状态、异常后 document/stream 栈、配置恢复、Tracer 委托还原和 Metrics listener 累积，再做多流/失败后复用验收，不能只把 CLI 的 null 换成后端。

## 按任务读取，禁止启动时递归展开链接

| 问题 | 对应入口（除工作流外相对 recovery） |
| --- | --- |
| 失败/工作流 | `.github/workflows/refactor-native.yml`、`refactor-xml-product.yml`、`refactor-guards.yml` + 对应脚本 |
| 宿主身份 | `scripts/BuildCompilerValidationHost.ps1`、`scripts/CompilerValidation.Common.ps1`、`tests/CompilerValidationEntry.cpp` |
| 生产部署/管理行为 | `scripts/TestCompiler.ps1`、`scripts/TestCompilerFoundation.ps1` |
| Tokenizer/音频 | `scripts/TestCompilerDifferential.ps1`、`TestCompilerGameObject.ps1`、`TestCompilerWeapon.ps1`、`TestCompilerAudio.ps1` |
| W3X/贴图 | `scripts/TestW3xProduct.ps1`、`scripts/CheckW3xCaptureInputs.py`、`scripts/TestCompilerTexture.ps1` |
| XML 全矩阵/smoke | `scripts/TestCompilerXmlMatrices.ps1`、`scripts/PrepareCompilerXmlMatrices.py`、`scripts/TestCompilerXmlSmoke.ps1` |
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
