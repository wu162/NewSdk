# MOD 编译器重构：最小续接入口

更新：2026-10-27（北京时间）。分支 `refactor/modcompiler-p0-p3`；草稿 PR #13 → `recovery/compiler-source`。
只在本分支普通提交；禁止强推、未经用户批准合入；子代理数量以本轮用户指示为准。

## 当前状态

- **P4 已通过；Win32 loose 核心 P5/G3 与 P6 最小发行包已验收，P7 未完成。** 阶段依据是已接受生产版本和明确输入范围，不代表后续候选已重验，也不泛称完整 UI/Unicode Texture/全部后处理已支持。
- `/sc:true` 已真实接入并通过两配置：独立包 session.xml/intermediate/cdata → fresh LoadCache→InitializeCache→新 processor → 重建四输出。热样例实际文档复用且 XML解析/实例编译为零；未用目录命中代替实际复用。
- `/bc`、`/sc` 默认关闭，可独立启用；命名空间覆盖输入/运行文件/参数/进程合同，实际解析边界限制根。`/dr /data /art /audio` 输入根已通过原版对照。压缩/部分/冻结 session、旧缓存目录覆盖及其他未实现选项仍明确拒绝。
- 会话包有完整清单/SHA256、布局/XML检查；坏包/结构/版本失效整包冷编译并可逆修复。缓存与发布、必需报告/终态/日志共同确认，pending 遗留始终 miss，回滚失败保留位置。
- 三类混合 Texture 暂明确拒绝 `/sc`，`/sc:false` 仍可编译；不授权未经证明的 Texture 缓存复用。中文纹理受旧混合 DLL LoadImageA/D3DX 限制，不能泛称完整 Unicode Texture 已支持。
- 当前 `/sc` 两配置各55用例与协议、生产组合/路由、App、Core九组、Texture、真实CLI、`/bc`、输入根、Audio CDATA/W3X均有成功证据。原Core Unicode组因反射fixture没绑processor失败；修正owner并保留全部断言后，精确归档host定向重验及原BAB基线已通过，原失败终态保留。
- 原Core App协议步骤曾被整体失败跳过，已对固定原归档60份原始报告/事件文件补做协议校验，两配置各15报告/511事件，零错误；只补协议，不重跑成功业务组。
- 完整未精简 mymod 原 BAB 先行与两配置生产四输出/cdata对照已通过。mymod apt/maps无入口，明确记录空集合；另有既有真实APT/MapMetaData及三份Upheaval UI完整依赖闭包证据，仍不代替完整 UI/runtime。
- P6 Debug/Release 正式 ZIP 已通过：精确五运行文件、独立解压后源bin/包bin doctor与真实小MOD、四输出逐字节及协议检查。系统仍需.NET4.8/x86 VC80/现有VC与DirectX；包证据不推断裸Windows无前置要求。
- 本轮首次生产链接C1001已通过等价移除App新增异常过滤器解决；生产重建成功。其后仅测试/发布工具变更严格复用同一已接受EXE与host，分别记录二进制/测试来源，不冒充当前重编译。
- P7 内建 BIG/skudef 的 apt/maps 主生产组已通过两配置：按游戏 Data/AptUI、Data/AdditionalMaps 布局精确消费本次产物；base BIG 依赖尚未恢复的 LoDStreamBuilder，必须明确拒绝，loose不受影响。旧SDK原命令实证发现版本标记应为`20 0d 0a`，旧候选两字节不符，包刷新已阻止。源码/断言/oracle已修为三字节，尚未重建，不能标完成。
- XML63组/非XML历史差分按原sourceCommit与未变化输入复用；不重复完整SDK或无依据路径排列。BIG完整base后处理、完整Unicode纹理、完整UI/runtime仍未完成。

## 下一步（按顺序）

1. **先重建/验收已写入源码的APT三字节版本修正**，保留原Windows `20 0d 0a` 实证与原主组成功范围。随后将发布准备器固定到修正后的真实成功source/run/archive，并同步旧命令期望为三字节，刷新含BIG发行包。旧二字节候选不得发布；本轮未再触发新重建。
2. 按原脚本必要顺序恢复/整合 base 所需 LoDStreamBuilder；需联合时保持 HashFix→ModAssetResolver→LoD。AssetMerge只在MOD自带预编译资产合并开启时执行；不能把raw BIG称为完整可用base包。
3. 后续候选稳定后刷新P6正式包；已有包属于已验收版本，不自动包含后续候选。还有本轮时间就推进下一项依赖满足的能力，不因检查点提前结束。
4. 固定归档重放入口已启用为精确partial-Core恢复，不再钉无产物源。只在实际测试修复需要时使用，校验完整生产闭包/原始归档/成功范围，保留原失败终态与二进制/测试双来源；不重复已过SC。

## 按任务读取

| 事项 | 入口 |
| --- | --- |
| P5 缓存与事务 | `recovery/P5_LIFECYCLE_AUDIT.md`、`recovery/src/App/Application/CoreStreamCompiler.cs`、`SessionPackage.cs`、`BuildAssetCache.cs` |
| Windows 受影响组 | `.github/workflows/refactor-core-lifecycle.yml`、`recovery/scripts/TestCompilerSessionCache.ps1`、`TestCompilerSdk.ps1`、`TestCompilerUnicodeReplay.ps1` |
| P7 BIG合同/限制 | `recovery/P7_PACKAGING.md`、`recovery/scripts/TestCompilerBig.ps1` |
| P6 正式最小包 | `recovery/scripts/PublishCompiler.ps1`、`recovery/COMPILER_RELEASE.md` |
| 既有路径/阶段/效率 | `recovery/PATH_AUDIT.md`、`recovery/NewSdk_ModCompiler_Refactor_Plan.md` P5/G3/P6/P7、`recovery/EFFICIENCY.md` |

## 不可丢失的执行约束

效率优先，正确性是底线；已明确行为成组实现并接入生产，调查、工具和文档只服务当前交付。先廉价预检，独立组集中验收，严格复用未变化证据，保留原始差分及SDK门槛。
Windows x86 / .NET Framework 4.8；恢复业务代码物理链接 EXE，保留必要 ANTLR。生产 EXE 不含测试入口；独立宿主复用生产对象，分别绑定两份 EXE 身份；不得放回业务 DLL 或改写 TLS/IAT 绕过限制。
一次启动持续推进本轮已授权工作，低频跟进 Actions 到终态；不让用户反复启动。每约10–15分钟保存有意义检查点，未验证不能标完成。
交接在对话中直接说明新增能力、验证、真正阻塞和下一步。入口约60行，不追加流水账、运行编号/指纹/产物链接；原始证据从分支 Actions 查询。
