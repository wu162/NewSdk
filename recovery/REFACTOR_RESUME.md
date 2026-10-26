# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理；历史轮次的 3 个子代理授权不自动沿用，数量以用户本轮指示为准；未合入其他分支。

## 当前状态（先读这里即可）

- **P4 已通过；P5 事务式发布已接入，当前真实 CLI 发布整组仍在修复验证，完整 P5/G3/SDK 未完成。** 生产入口保留实验性 Win32 loose/no-cache 后端；不能宣称完整 MOD 编译器或正式发行包可用。
- 发布采用 `OutputRoot/published` 整代替换，必需报告/终态/日志收尾失败回滚；空可选集合不替换上一代。Debug/Release App 各569断言（发布生命周期147项）已过，含RollbackFailed备份保留。两配置 Core全7组/原版对照/协议/Texture已过；真实CLI的Debug整组已过，Release同目录取消在旧日志替换处失败，最终观察器修复正在Windows整组复验。
- 新增未接入的 SDK/MOD/runtime 内容快照及27项合同断言，明确不能授权缓存复用；生产缓存仍关闭。旧缓存的 Complete 文档、资产来源、mtime失效和事务收尾接入仍待实现。
- 前次完整 Windows 候选两配置均通过：App 各355断言、Core 全7组（路径组各130断言，含产物收集根/后代 junction、精确 cdata 布局、同名目录、Unicode/空格/#/% XInclude）、原版后端对照、真实 App 协议、当前 EXE 绑定 Texture 部署/payload。
- 前次真实 CLI 各25个独立 PID 已过：单进程 job/no-child、冷分组、内外 MOD、实际 CTRL_BREAK/exit6/retry、真实日志锁/exit7/同路径重试、参数拒绝与协议。每配置22 reports/912 events；独立原版严格比较原始四输出，未归一化字节。
- 真实业务新增12个文件型 APT 资产、3个 MapMetaData 资产；组合与三组冷进程输出一致，APT/maps 独立当前原 BAB 对照已过。这不是 APT 格式解析或完整 UI/SDK 验收。
- 三份真实 Upheaval UI 两配置均通过：固定祖先原 blob 的106文件/103依赖/100资产闭包，3流组合与各UI冷进程一致；当前原 BAB 逐流原始四输出严格相同。两份 Texture 按固定原DLL正确ID/Hash及内联DDS布局核验（不是cdata），未删纹理或归一化产物；不代表运行时UI/完整SDK。
- 根/路径、日志原子替换与活动文件保护、晚期失败撤销产物、.build junction 与既有 staging 所有权修复均已取得上述完整候选的原始收据；已核对归档 SHA、14份 fixture 的结果/日志/EXE 和两份协议原始文件哈希，详见 PATH_AUDIT.md。
- `62b7766` XML 两配置各63组、`9cd2cb8` 非XML组件证据仅按原输入/范围复用，不将旧生产/宿主身份冒充当前重测；这些历史证据不冒充当前重测；本轮未重复完整 SDK。

## 下一步（按顺序；2026-10-26 用户纠偏）

1. **先收尾本轮真实发布整组验证。** 先查分支最新 Core lifecycle 作业终态；当前控制器已按PID/buildId/sequence隔离旧事件，且用Created/Renamed观察新日志发布后再读取，避免参与替换竞态。保留前两次失败，不盲重试；首次/替换/占用/取消/回滚/重试和CLI协议两配置均过才标为生产整组通过。晚期输出/回滚失败的注入仍是App受控组，不冒充真实CLI注入。
2. **随后推进实际缓存能力。** 参见 P5_LIFECYCLE_AUDIT.md 的具体接入顺序；新命名空间、内容失效、缓存文档/资产来源和事务确认须一并处理。不得只跨流共享旧 SessionCache 或把输入快照当成可用缓存；保留无/冷/热、依赖变化、连续MOD、失败/取消后重试及原始四输出差分。
3. **Unicode 已定位为旧混合 Texture DLL 的 ANSI 加载限制。** Windows 下载/准备已通过；Debug ASCII 三UI成功，中文组合根路径在 LoadImageA/D3DX 失败，exit4且空产物，输入未变；Release/协议未执行，未做单根归因。记录限制继续独立功能，不重复下载器审查，不宣称完整Unicode纹理支持。
4. **路径审查保持现有回归。** 产物布局、junction、staging所有权和活动文件保护已有验证。仅新失败、相关实现变更或具体未覆盖风险才补定向调查；测试通过且无具体风险即收尾，不默认扩展路径排列或重审归档。
5. **集中完成P5/G3/SDK，再推进P6/P7。** 发布/缓存相关能力完成后集中验收，复用未变化的有效证据；不删减既定验收，不改写历史sourceCommit，不重复全SDK凑时长。旧不支持参数继续明确拒绝；事务只支持loose产物，不代表BIG/skudef或正式发行包完成。

CLI-only 历史运行文件下载/准备器及轻量 guards 已有通过证据，ZIP 原始路径校验已补充修复；复用须保留历史二进制来源与当前测试来源，candidateValidated=false，不能冒充当前生产重测。三 UI 整组已有通过及归档核对证据，除相关变更或具体失败外不重复展开。

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

- 用户最新澄清：**一次启动后持续推进并低频跟进已有 Actions，不要因等待而提前结束，让用户反复启动任务。** 有独立工作先做；没有也可以合理等待，直到取得结果。及时保存检查点，但保存不等于结束任务；不重复启动已有验证，不把运行中当作通过。

- 首轮只读根 AGENTS.md + 本页，再查询分支头/运行摘要；不要默认加载恢复线 RESUME、全量 STATUS、计划全文、历史交接或整份 PR diff。
- 工具结果先解析/筛选再输出；提交仅摘要和相关文件，日志先尾部 60–100 行；文件按函数/行段读取，缺证据再扩展。
- 每次只展开当前阻塞所需 1–3 个文件；技术细节不重复粘贴到入口。
- 本页保持约 60 行以内：覆盖更新当前状态、下一步和必要边界，禁止追加流水账；历史保存在 git 或按需文档。
- 中断前及时保存检查点，明确已提交/未验证/验证中/已验证。不能恢复旧会话未提交内容时直说，不推断其已经保存。
