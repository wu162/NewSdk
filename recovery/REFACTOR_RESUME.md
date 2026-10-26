# MOD 编译器重构：最小续接入口

更新：2026-10-26。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在当前分支普通提交；禁止强推、未经批准合入；默认单代理；历史轮次的 3 个子代理授权不自动沿用，数量以用户本轮指示为准；未合入其他分支。

## 当前状态（先读这里即可）

- **P4 已通过；P5 真实后端、路径、生产 CLI 定向组已通过，完整 P5/G3/SDK 未完成。** 生产入口保留实验性 Win32 loose/no-cache 后端；不能宣称完整 MOD 编译器或正式发布流程可用。
- 本轮完整 Windows 候选两配置均通过：App 各355断言、Core 全7组（路径组各130断言，含产物收集根/后代 junction、精确 cdata 布局、同名目录、Unicode/空格/#/% XInclude）、原版后端对照、真实 App 协议、当前 EXE 绑定 Texture 部署/payload。
- 真实 CLI 各25个独立 PID 已过：单进程 job/no-child、冷分组、内外 MOD、实际 CTRL_BREAK/exit6/retry、真实日志锁/exit7/同路径重试、参数拒绝与协议。每配置22 reports/912 events；独立原版严格比较原始四输出，未归一化字节。
- 真实业务新增12个文件型 APT 资产、3个 MapMetaData 资产；组合与三组冷进程输出一致，APT/maps 独立当前原 BAB 对照已过。这不是 APT 格式解析或完整 UI/SDK 验收。
- 三份真实 Upheaval UI 两配置均通过：固定祖先原 blob 的106文件/103依赖/100资产闭包，3流组合与各UI冷进程一致；当前原 BAB 逐流原始四输出严格相同。两份 Texture 按固定原DLL正确ID/Hash及内联DDS布局核验（不是cdata），未删纹理或归一化产物；不代表运行时UI/完整SDK。
- 根/路径、日志原子替换与活动文件保护、晚期失败撤销产物、.build junction 与既有 staging 所有权修复均已取得上述完整候选的原始收据；已核对归档 SHA、14份 fixture 的结果/日志/EXE 和两份协议原始文件哈希，详见 PATH_AUDIT.md。
- `62b7766` XML 两配置各63组、`9cd2cb8` 非XML组件证据仅按原输入/范围复用，不将旧生产/宿主身份冒充当前重测；本轮未重复完整 SDK。

## 下一步（按顺序；2026-10-26 用户纠偏）

1. **主线优先完成功能：先接入事务式产物发布。** 发布组件已提交且独立 Windows publisher contracts 已通过，但尚未接入 CLI/BuildSession 最终收尾；完成成功提交、失败/取消回滚和日志收尾的生命周期衔接，不能把组件通过当成正式发布完成。
2. **按完整发布能力组统一验证。** 覆盖首次发布、替换已有输出、文件占用、取消、晚期失败、回滚及重试，核对返回码、日志和磁盘产物一致性；通过后推进缓存能力组，不以新增边界审查代替剩余功能实现。
3. **Unicode 失败保留为明确待办，先按失败日志判断阻塞范围。** 独立 SDK/MOD/runtime/output 根路径工作流失败，Windows 自动下载路径及 Unicode 整组未验收；只定位具体失败步骤。若影响当前功能正确性或文件安全则优先修复，否则记录限制并继续独立功能；不得宣称原混合 Texture DLL 全 Unicode 已支持。
4. **路径审查收敛为现有回归。** 产物收集布局、junction、staging 所有权及活动文件保护已有验证；保留这些测试和真实资产差分。仅在新失败、相关实现变更或具体未覆盖风险出现时补充定向调查；相关测试通过且无未解决的具体风险即收尾，不再默认扩展特殊路径排列、重复审计已核验归档或全面重查路径。
5. **集中验收，复用有效证据。** 发布与缓存等相关改动完成后集中进行完整 P5/G3/SDK 验收，再推进 P6/P7；不得删减既定验收或改写历史 sourceCommit。验证等待期间推进依赖满足的实现/测试准备，不空等、不为填时间增加审查。旧不支持参数继续明确拒绝，当前 CLI 输出仍仅隔离 staging。

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

- 首轮只读根 AGENTS.md + 本页，再查询分支头/运行摘要；不要默认加载恢复线 RESUME、全量 STATUS、计划全文、历史交接或整份 PR diff。
- 工具结果先解析/筛选再输出；提交仅摘要和相关文件，日志先尾部 60–100 行；文件按函数/行段读取，缺证据再扩展。
- 每次只展开当前阻塞所需 1–3 个文件；技术细节不重复粘贴到入口。
- 本页保持约 60 行以内：覆盖更新当前状态、下一步和必要边界，禁止追加流水账；历史保存在 git 或按需文档。
- 中断前及时保存检查点，明确已提交/未验证/验证中/已验证。不能恢复旧会话未提交内容时直说，不推断其已经保存。
