# 当前：已按用户要求回退本轮编辑优化，保留上一轮冷热成果

仅回退 `e2dba3c3→8e8cf766` 的instance依赖收窄、父DOM快照和引用处理优化。五个Core文件恢复为 `e2dba3c321504752438b4e940d651bbfcdb58fff`；完整生产/SDK/构建输入闭包与该已验收提交一致，复用 [41169755141](https://github.com/wu162/NewSdk/actions/runs/41169755141)，没有重建二进制或重跑完整MOD。此前会话/Schema共享、XML哈希缓冲及冷热性能成果保留。v19会话由恢复后的v18逻辑安全判失效并重建。

同步移除仅属于本轮的三个专用探针和修复工作流/脚本，恢复原失效断言；通用Core/CLI/SDK门槛、后续完整Upheaval比较脚本和全部历史证据保留。检查：生产闭包零差异、变动Python/YAML语法、专用接口清理、diff检查通过。后述8e8及性能结果均为历史状态，不代表当前代码。新并行方案尚未实现。PR #38仍待审，禁止强推和未经批准合并。

# 历史：完整 Upheaval 大幅编辑热缓存比较已完成，未证明普遍提速

用户追加的大型MOD八类真实大改已完成：[41181143063](https://github.com/wu162/NewSdk/actions/runs/41181143063) A/B均success，计量源 `a4682714`，生产仍为已验收 `8e8cf766`，基线 `e2dba3c3`，没有重建。完整原MOD保留，SDK父本地覆盖与8个生命周期补充资产明确标注。每场景off三对、detailed单列；272/272构建0错误/1859原警告、每代全发布原字节相同、192次恢复/热检查0解析0编译。sourceCommit/runId、实际输入、生产树和运行五文件身份通过核对，两个新归档各核验一次；既有非法拒绝/失败取消/SDK门槛按未变二进制复用。

无关父兄弟解析16→1，三对均快，中位少2.47%；有效父解析16/编译304不变，中位少4.29%但逐对方向混合。GameObject少0.19%、Weapon多0.74%、Define多2.15%（解析16→12）、增删重命名少0.35%、80媒体多2.08%、32W3X多1.93%。纯热无普遍改善。引用阶段单次detailed减少工作，但所有真实修改均无snapshot hits/reloadAvoided；Define继承重载0→1。本轮未证明普遍编辑提速或完整MOD父快照收益，不追加测试改善数字。[完整结果/原始三对/边界/证据](docs/UPHEAVAL_LARGE_EDIT_COMPARISON.md)。下一步优先查真实MOD快照为何未命中及Define新增重载，再按Schema/会话/输出与整文档前端热点推进。未新增生产代码；PR #38 未合并，禁止强推。

# 上轮：缓存后编辑的依赖/父快照/引用优化已验收，PR #38 待审

`perf/compiler-cold-hot-cache` / [PR #38](https://github.com/wu162/NewSdk/pull/38)，未合并。生产二进制 `8e8cf766`，后续仅测试/文档。组合验收 [41178094135](https://github.com/wu162/NewSdk/actions/runs/41178094135) success：原D/R与12既有Core/CLI/取消/缓存等有效组复用，Include D/R各140检查/4组、完整mymod原SDK、五文件ZIP、Upheaval22全通过；父快照/引用D/R成功结果按实际输入闭包复用。原失败终态和修复来源保留。

实际父实例/祖先/文件物理闭包替代instance整文件内容依赖（XInclude/真实文件仍保留）；旧v18相关文档安全重载、写v19；请求内成功原节点快照和引用DOM/Schema/类型对/路径复用均已落地。无关父兄弟2→1解析，受控父快照3→2解析/1→0继承重载、后端不变；快照仍保留整父DOM，局部XML仍按整文档前端。

同机编辑配对 [41178837554](https://github.com/wu162/NewSdk/actions/runs/41178837554) 80/80、每代原字节及再热0/0通过。off各三对：有效父518.893→511.192ms（减少1.48%），无关兄弟490.765→493.781ms（增加0.61%）；小负载纯热n12增加2.52%。**没有稳定端到端编辑提速结论**，不以解析减少代替总耗时收益，不与旧完整MOD纯热数字混算。必要验收已完成，不再扩大测试。

[本轮实现/数据/验收/未测](docs/HOT_CACHE_EDIT_OPTIMIZATION.md)、[下一步入口](docs/HOT_CACHE_REFERENCE_RECONCILIATION.md)。后续优先实例级表达式/继承/引用阶段复用与实际XSD/启动成本，父DOM持有成本另做证据；完整XSD、失败/取消和跨流清理保留。禁止强推，未经用户明确批准不合并。

# 上轮：冷/热缓存性能优化已验收，PR #38 待审

分支 `perf/compiler-cold-hot-cache` / [PR #38](https://github.com/wu162/NewSdk/pull/38)。已同步目标FLA基线 `074bf768`，组合源码 `e2dba3c3`，[Windows 41169755141](https://github.com/wu162/NewSdk/actions/runs/41169755141) 全success：Debug/Release、真实FLA/AS、Core/CLI/取消、64worker/缓存、完整mymod原BAB、五文件ZIP及22个完整Upheaval样本。原归档、十运行文件和两个ZIP已下载核验。

最新UI基线同机jobs4：完整MOD冷36.723→33.501秒(-8.78%)、热2.273→1.691秒(-25.63%)，24/24样本通过；扩容+49152Armor正式off冷70.806→61.259秒(-13.48%)、热10.099→6.594秒(-34.71%)。扩容全部18正式+2补充通过，随后基线detailed在720秒总预算耗尽时终止，运行failed保留，候选detailed未测；不把整组标通过。38进程增量诊断通过，含未使用Define与仅丢会话元数据。全部已完成对照产物原字节一致，冷n=3/热n=6、OS页缓存未控。

[实现/全部原始计量与限制](docs/COMPILER_COLD_HOT_OPTIMIZATION.md)、[深入源码优先级](docs/COMPILER_CACHE_SOURCE_ANALYSIS.md)、[上传热缓存文档对照](docs/HOT_CACHE_REFERENCE_RECONCILIATION.md)。共享schema/成功会话、原512字符/ANSI哈希缓冲、Schema事实缓存和纯查询/纯写索引精简已落地；继承粒度、父DOM与稳定排序的后续边界另列。未经用户批准不合并，禁止强推。

下一轮优先优化修改后的前端范围：收窄instance依赖→请求内父实例快照→引用处理；[待办与执行节奏](docs/HOT_CACHE_REFERENCE_RECONCILIATION.md)、[可直接使用的提示词](docs/HOT_CACHE_NEXT_ROUND_PROMPT.md)。多数有效时间用于成组实现，中途短场景、收口集中验收；必要检查通过后停止扩测，复用有效证据，压缩重复归档。无修改纯热态收益不代表编辑收益，现有增量诊断尚无稳定多样本提速结论。

# 当前：FLA / AS 直接 MOD 四流插件（验收通过，PR #36 待审）

用户指定 `recovery/legacy-sdk-cache` / `665468cb`；成果在 `feat/fla-direct-mod`，
[PR #36](https://github.com/wu162/NewSdk/pull/36)。生产/测试源码 `75d29895`，
[Windows 41165126334](https://github.com/wu162/NewSdk/actions/runs/41165126334) 整体 success：
Debug/Release 新构建、精确五文件部署、两份原附件（3 / 62 资产）、混合 MOD 与裸 FLA
自动发现、30 个真实 CLI 源依赖/缓存/失败清理重试用例、原 Core/CLI/会话缓存及完整
mymod 原 SDK 差分、两个独立 ZIP 通过。归档、十个运行文件及两个 ZIP 已下载复算。

支持二进制 MX2004 / Flash8 FLA 和 AS1/AS2，直接进入 Core 多资产链接；旧二进制
APT 及其 worker 资格保留。三个固定 UI 库嵌入 EXE。操作、边界及失败合同见
[FLA UI 文档](docs/FLA_UI_COMPILER.md)，[验收身份及原收据](evidence/ui/fla-validation/41165126334-1/summary.json)。
首轮按钮兼容问题和错误测试预期的失败记录保留；未新增游戏内 UI 播放验收。
未经用户批准不合并，禁止强推。以下为历史交接。

# 当前：PR #33 组合验收通过，已获用户合入批准

已同步 `recovery/legacy-sdk-cache` 的 `0d7b685c`（含已合入 PR #32、#34），只解决续接文档冲突，保留两边生产实现。组合源码 `1a3e4bc6`，[Windows 41152199159](https://github.com/wu162/NewSdk/actions/runs/41152199159) 整体 success：Debug/Release 新构建、Core/CLI/取消、64 worker/cache、完整缓存/载荷/输入根/BIG、完整 mymod 原 SDK、五文件 ZIP 及 Upheaval22样本原字节通过。归档已下载复算，十个运行文件/两个 ZIP 身份已核对。

20个插件生产文件及63组 XML 组件依赖未变，复用下方独立组件矩阵；新组合产品没有冒用旧 EXE。新增组合收据与依赖身份见 [验收目录](evidence/refactor/tokenizer-native-stage2/41152199159/archive.json)，详细来源见 [第二阶段报告](docs/TOKENIZER_NATIVE_STAGE2.md)。用户已明确批准 PR #33 合入，无需再次确认；禁止强推。

# 当前：六项插件原生优化已验收，PR #33 待审

分支 `perf/gameobject-weapon-plugins` / [PR #33](https://github.com/wu162/NewSdk/pull/33)，最终生产源码 `d60904bc`。透明属性查找、member 地址索引、mask 范围解析、标量默认值预解码、Draw 子节点分组及最终缓冲直写均已接入。以 [第二阶段报告](docs/TOKENIZER_NATIVE_STAGE2.md) 为准。

[七类组件 41146357838](https://github.com/wu162/NewSdk/actions/runs/41146357838) success，原 DLL/前后组件大小端三缓冲一致、真实退出0。相对上一阶段，Schema 默认 GO/Weapon 中位耗时减少25.14%/26.95%，32 Nuggets 减少57.37%；是插件负载，不是完整 MOD 收益。

[最终产品 41146357892](https://github.com/wu162/NewSdk/actions/runs/41146357892) 整体 success：Debug/Release、GO/Weapon 完整四平台原字节/tokenizer、Core/CLI/取消/缓存/64 worker矩阵、完整 mymod 原 SDK、五文件包、Upheaval22样本通过。十个运行文件和两个ZIP身份已下载核对。

[共享 XML63 41145352024](https://github.com/wu162/NewSdk/actions/runs/41145352024) 的路由及完整矩阵步骤 success（1,918,904样本/11,064,384比较）；后续重复产品检查停止，整体 cancelled。仅复用其独立 XML 范围，d457f99a→d60904bc 的63组依赖闭包和实际工具链/托管宿主身份未变；最终产品另取上面的完整成功运行。

[完整 MOD 三轮配对 41148441137](https://github.com/wu162/NewSdk/actions/runs/41148441137) success，全部发布字节相同、六次0错误/1859警告；中位34.766→34.659秒（减少0.31%，基本持平），第二对稍慢。原始证据均已下载复算，较慢样本保留。未经批准不合并，禁止强推。

# 当前：GameObject / Weapon 插件编译优化（验收通过，PR #33 待审）

用户指定 `recovery/legacy-sdk-cache` / `8f854606`，成果在 `perf/gameobject-weapon-plugins` / [PR #33](https://github.com/wu162/NewSdk/pull/33)。移动嵌套输入树、只读模块索引、Weapon 单遍读取和枚举表复用已接入；仅五个插件 Bridge/Native 生产文件改变。原生 InitOnce 修复混合运行时关闭，失败均保留。

组件源码 `25aedf8b`，[41138099602](https://github.com/wu162/NewSdk/actions/runs/41138099602) success；同生产源码的完整产品 `9f9d1767`，[41138338003](https://github.com/wu162/NewSdk/actions/runs/41138338003) success：Debug/Release、完整原字节/tokenizer、64 worker/cache、Core/CLI/缓存/取消、完整 mymod 原 SDK、两个五文件 ZIP、完整 Upheaval 22 样本通过。归档、十个运行文件及 ZIP 已下载复算。

同 VM 完整 Upheaval 三轮前后配对 [41141062587](https://github.com/wu162/NewSdk/actions/runs/41141062587) success，全部发布字节相同；中位数 22.185→21.638 秒，耗时减少 2.47%，第三对较慢及 OS 页缓存边界保留。插件合成负载中空 GameObject 慢约 6%，复杂 GameObject/Weapon 改善；详见[实现与计量](docs/TOKENIZER_PLUGIN_PERFORMANCE.md)。

追加的[深入源码研究](docs/TOKENIZER_PLUGIN_NEXT_OPTIMIZATIONS.md)保留原型与 Linux 算法数据；六个方向随后已实施，当前状态以上方第二阶段入口为准。未经用户批准不合并，禁止强推。
# 当前：RA3 EALayer3 V1（Windows 验收通过，PR #34 已合入）

用户指定基线 `recovery/legacy-sdk-cache` / `eb503513`；开发分支 `feature/ra3-ealayer3`，
[PR #34](https://github.com/wu162/NewSdk/pull/34)。源码/二进制 `4ddac274`，
[Windows 41149785200](https://github.com/wu162/NewSdk/actions/runs/41149785200) **success**：
176 实际编码/88 候选解码、jobs1/4、原有格式 D/R 各521原字节、正式 BAB 44资产、
正常产品 D/R、热态0编译/修改WAV仅1编译/独立重建、完整 mymod/LoD/BIG/SKUDEF通过。

EALAYER3 选择真实 EL31/V1b/SNR5，复用选定 sagetools/LAME3.96 源码，保留现有
输入/重采样/Loop/资产/SessionCache/worker；旧 AudioFile processing hash 已更新。
1/2/4/6声道、六种16–48kHz MPEG采样率、0–100质量、Loop、RAM/streamed通过；
8/11.025/12kHz、V2及游戏内播放未完成。原插件随附库 codec31 实测为 V2；显式 V1
LSF 参照不可用的25条数据明确保留，只有4条可用 V1 参照计为波形对照通过。

范围/来源/限制见 [audio/EaLayer3.md](audio/EaLayer3.md)，原收据和下载复算身份见
[ealayer3 summary](evidence/audio/ealayer3-validation/41149785200-1/summary.json)。
许可只在源码保留，不嵌入或加入部署包。后续提交仅文档/收据；禁止强推，未经用户批准不合并。

# 当前：L8 热缓存 Benchmark（Common 通过，PR #32 已合入）

分支 `benchmark/l8-hot-cache`，基于 `recovery/legacy-sdk-cache` / `8f854606`；[PR #32](https://github.com/wu162/NewSdk/pull/32)。Benchmark 脚本/工作流已实施，未修改生产算法。Windows/Linux 各 33 项预检通过；[Common 41139062989](https://github.com/wu162/NewSdk/actions/runs/41139062989) success，测量源 `a286062f`、Release 源 `cc10391b`，整个 Windows job 18 分 20 秒，C00–C12 全部完成，75/75 正式样本 wall/elapsed 均 >8 秒，另有 3 个独立 detailed 样本。实际纯热态约 9.34 秒，未达 10–14 秒软目标，未把调参点当成绩。详见 [报告/原始计时/范围](docs/HOT_CACHE_BENCHMARK.md)。旧失败/预算耗尽结果保留，不作跨 VM A/B。Full 未运行，Environment 配置受集成权限限制，且 28 个扩展条件的双重复理论下限 >22.4 分钟、超过其 16 分钟阶段预算，不能声称整组完成。禁止强推，未经用户批准不合并。

# 当前：AudioFile 全设置并行与低预算（组合验收通过）

[PR #30](https://github.com/wu162/NewSdk/pull/30) 已获用户批准合入 `recovery/legacy-sdk-cache`。成果在 `perf/audiofile-parallel`，现已同步目标分支 `d85b8c8c`（已合入的 PR #28）。保留其重复状态清理和预算估算，适配 AudioFile 测试；显式采样率及压缩诊断可并行，外部输入预算由 64 倍降至 4 倍，压缩提示进程内仅一次。

组合源码/二进制/测试来源 `cc10391be67dda57c7efa7fc06256b99e9372d9a`，[41068958541](https://github.com/wu162/NewSdk/actions/runs/41068958541) **success**：Debug/Release 新构建，八种音频设置 worker 三缓冲/CDATA、64 生产缓存/jobs 用例、CLI/CtrlBreak、全部缓存组、完整 mymod 原 SDK 和五文件包通过；完整 Upheaval 22 样本原字节一致，均 0 错误/1859 原警告。新归档、两个发布 ZIP/十个运行文件均已下载复算；14 份原收据身份见[组合验收摘要](evidence/refactor/audiofile-parallel/41068958541-1/summary.json)。独立 AudioFile 分支的 `6c2f0869`/41058436061 证据作为历史保留。后续只补文档/证据，无新游戏内验收。详见[实现与验证范围](docs/AUDIOFILE_PARALLEL.md)。禁止强推。

# 当前：冗余安全清理已合入（PR #28）

`cleanup/redundant-defenses` 基于刷新后的 `b89fbadb`，生产/测试源码 `ecd2bf1d`。[Windows 41062149232](https://github.com/wu162/NewSdk/actions/runs/41062149232) 全部 success：Debug/Release、Core/CLI、SessionCache 108×2、两类原缓存/导入、Audio/Texture/W3X、BIG/SKUDEF/完整 mymod 原字节、并行/热态依赖/协议与 40 次配对输出均通过。后续仅文档/证据。

已删除 CacheInputBoundary、运行目录身份扫描、session.identity、.file-identities.json/SHA 收据、ANTLR 固定审核、诊断句柄固定及重复后端状态。原缓存语义、实际依赖、ChangeTime、窄 XSD 约束失效及必要 worker 预算保留；没有新侧车/发布事务。混合冷 1010.75→969.61 ms、热 652.69→634.27 ms；准备 5.29→7.79 ms、目录查询各 +3 的不利数据也保留。

源码依据、完整计量/范围和原始失败见 [DIRECT_OUTPUT](docs/DIRECT_OUTPUT.md)。[PR #28](https://github.com/wu162/NewSdk/pull/28) 已合入；未强推或修改授权/合入规则。无正确性阻塞问题，准备阶段及目录查询仍可继续精简。

以下为历史交接，不代表本轮实现或验收结果。

# 当前：插件重复 schema 校验已移除（验收通过，PR #26 待审）

用户指定基线 `recovery/legacy-sdk-cache` / `5c1843ae`；成果在 `refactor/remove-plugin-schema-validation` / [PR #26](https://github.com/wu162/NewSdk/pull/26)。删除 XML、GameObject、Weapon、Effects/Particle、Locomotor 及生成代码中的属性/子节点白名单、文本、单例、结构和命名空间校验及专用遍历/数组分配。Core 统一 XSD、依赖元数据及类型路由继续工作；Audio/W3X 的编码器数据检查保留。

生产源码验收入口 `e3272ffd`；最新 `822007a5` 仅补齐测试源清单/流程和旧断言，生产源码、SDK、构建目录和类型目录的 Git tree 均相同。Core 的专门用例验证非法 XSD 在派发前失败，直接插件调用不重复校验。组件拒绝用例使用统一 SDK schema 测试边界，原始字节与 ABI 断言继续保留。

- [Core 41037021490](https://github.com/wu162/NewSdk/actions/runs/41037021490) **success**：Debug/Release、CLI、缓存、完整 mymod 原 SDK 差分、独立五文件包和 Upheaval 22 样本对照通过。
- [XML 41037565236](https://github.com/wu162/NewSdk/actions/runs/41037565236) 的 XML 步骤及原收据 **passed**：Debug/Release 各 63 组、四平台，合计 1,918,904 样本/11,064,384 缓冲区比较。该运行整体 **failure** 来自随后 GameObject/Weapon 测试源清单未登记共享助手；此部分已在下面的独立运行修复验收，XML 成功范围及输入未变。
- [GameObject/Weapon 41042830679](https://github.com/wu162/NewSdk/actions/runs/41042830679) **success**：完整原版字节/tokenizer 矩阵、Debug/Release、完整既有样本集及隔离运行通过；该专项复用上面的独立 XML 成功范围。

三份归档已实际下载并复算 SHA256；原收据、来源、运行文件身份及复用边界见 [验收摘要](evidence/refactor/plugin-schema-removal/summary.json)。本地路由审计、26 项组选择测试、测试源清单及两个 UIConfig 生成器精确复现通过。未经用户批准不合并，禁止强推。

# 当前：有界热缓存扩展与失效/依赖计数（已验证，PR #23 待审）

分支 `test/hot-cache-coverage`，基于 `8a6b7a70`。扩展三档 Armor 规模（32/256/2048 资产、4/32/128 文档），中档混合 Texture/PCM Audio/W3X，代表性缓存模式、压缩、jobs、BIG、等长同mtime编辑与输出/会话修复。单进程15秒、整轮120秒，不做全维度笛卡尔积。

summary/detailed 新增按流聚合的 `performance.counters`：实际缓存失效原因、唯一失效文档和原 self-instance 数、Include 传播、定义/继承/文件依赖检查及传播重载；off 不建计数字典，不追加文件读取或依赖扫描。见 [性能说明](docs/PERFORMANCE.md)。[PR #23](https://github.com/wu162/NewSdk/pull/23)，禁止强推，未经批准不合并。

生产二进制/测试来源 `91b040d2`，[41010073140](https://github.com/wu162/NewSdk/actions/runs/41010073140) success：重建Release、App合同、本地及Windows五项Python预检、原14样本（9.98秒）及扩展56样本（42.85秒）通过。全输出与每代独立无缓存原字节相同，定义修改实际记录 contentChanged→definesChanged→inheritanceChanged，3失效文档/2依赖传播文档；Include最大深度3，访问与去重文档分别统计，修改后热态零失效/解析/编译。五运行文件和原归档SHA256已下载复算，69份报告Schema通过。[原收据与摘要](evidence/refactor/hot-cache/41010073140-1)。未新增Debug、完整MOD或游戏内验收；后续仅文档/证据更新。

首轮 `90e81710` / [41009339596](https://github.com/wu162/NewSdk/actions/runs/41009339596) 已保留失败：App、当前Release、原14样本通过，扩展48样本的字节检查通过后因继承用例误要求晚于文件依赖检查的原因而拒绝。实际输出相等、直接失效1文档并传播至第2文档；控制器已按真实先后顺序修复，并分离定义文件以独立触发定义与继承链。原收据及归档复算身份在 [证据目录](evidence/refactor/hot-cache/41009339596-1)。生产源码未随测试修复改变。

# 当前：W3DAnimation 压缩优化（功能及真实通道计量已通过）

分支 `optimize/w3d-animation-compression` / [PR #22](https://github.com/wu162/NewSdk/pull/22)，基于 `recovery/legacy-sdk-cache` 的 `436115db`。Timecoded 邻接索引/局部最小堆、AdaptiveDelta 有界搜索/常量快路/步长复用、生产逐帧去拷贝已完成，精度及输出不变。

生产/功能验收源码 `4e9c9a27`，Windows [41003661518](https://github.com/wu162/NewSdk/actions/runs/41003661518) success：Od/O2原版组件、正式插件、13376新旧完整输出、Debug/Release产品完整三缓冲区重放及真实CLI缓存矩阵通过。第一轮漏建验证宿主的失败已保留并修复。

真实源计量源码 `12848f8e`，[41004166786](https://github.com/wu162/NewSdk/actions/runs/41004166786) success：194动画/7390通道/468946展开帧，同VM三对中位数25.161→8.449秒，约2.98倍；选中编码全部逐字节相同。它是压缩阶段，不能当作完整MOD收益。生产源码及既有功能验收输入随后未变；分别保留功能与计量来源。

完整未精简Upheaval端到端对照 [41004795406](https://github.com/wu162/NewSdk/actions/runs/41004795406) success：jobs4/双缓存关闭/全新输出，两对中位数33.832→29.616秒，减少12.46%；全部BIG/SKUDEF实际字节相同，四次均0错误及相同1859警告。仅两样本/实现、未控OS页缓存，不作通用保证。原始收据、完整输出对照及artifact哈希已下载核对并归档；[详细说明](docs/W3X_COMPRESSION_PERFORMANCE.md)。禁止强推，未经用户批准不合并。

# 当前：AudioFile XAS 性能优化（已验收，PR 待审）

基于 `recovery/legacy-sdk-cache` 的 `45d485d8`，成果在 `perf/audio-xas-ranges` / [PR #21](https://github.com/wu162/NewSdk/pull/21)。六项优化一次完成：原数组偏移读取、最终数组直写、完整128帧块零复制、Loop区间独立分段、复用float[30]、四预测器共用一次转换。只修改 Audio 三个编码器，未修改其它插件。

源码/二进制/测试来源 `b8e37a2e957f7a2465ad303110b92eec3269df4d`；后续只有结果归档。Windows [40996371215](https://github.com/wu162/NewSdk/actions/runs/40996371215) success：Debug/Release x86 各8924原版完整输出一致、既有拒绝和块/精度矩阵通过；同机3280边界+21长音频完整输出对照通过。未执行完整SDK。完整ZIP已下载复算SHA256，原版输出压缩保留于仓库。

同机CLR x86三轮中位数：120秒双声道RAM 46.243→2.788秒，分配336.078→6.523MiB，GC33/3/1→0/0/0；Loop RAM约16倍。600秒streamed分配1081.368→32.744MiB、GC184/2/0→0/0/0，耗时13.629→13.936秒（增加2.3%，如实保留）。[完整方法和计量](docs/AUDIO_XAS_PERFORMANCE.md)。禁止强推；未经用户批准不合并。

## 上一批：Tokenizer 并行与输入准备去拷贝（已验收）

成果分支 `recovery/legacy-sdk-cache`；生产源码/二进制/测试来源 `5be40ac421fa4562654d34cc3d62050eca7a389e`。本轮基于 `fbf51cee`，后续提交仅归档文档和证据。禁止强推；未合并其它主线，未发布 Release。

已开放精确内建 GameObject、WeaponTemplate、ArmorTemplate worker 资格；按 Core 生命周期直接删除 XML DOM、NameTable、namespace bindings、句柄和引用集合深拷贝。普通任务使用原声明，CDATA 任务只浅拷贝状态以隔离输出路径。删除调度器重复整树估算和无文件任务的租约集合。三种可选 trace 共享索引写入已同步，旧缓存/哈希及 owner 验证提交/失败取消 join 保留。

统一新验收 [40983673225](https://github.com/wu162/NewSdk/actions/runs/40983673225) **success**：Debug/Release 完整构建、三 tokenizer×四平台 worker/trace/三缓冲、XML284×四平台、64生产缓存/jobs用例、原BAB、CtrlBreak、缓存冷热/失效/重试、完整未裁剪mymod原SDK四流/BIG/Additional和五文件包全部通过，没有复用旧二进制。原归档8b4be284…已实际下载复算匹配。

完整 Upheaval 同VM 22样本全部0错误、1859原警告、输出原字节一致。默认冷jobs1/4/auto为73.489/43.611/43.632秒，jobs4比jobs1耗时减少40.7%；无缓存冷70.480/41.610/41.716秒。三类实际调用121/108/12全部取得worker资格，未知插件owner回退从旧记录358降至117；10个大任务预算回退保留。准备阶段本轮jobs4/auto为2.126/2.029秒；旧4.561/4.710秒来自不同VM，不作为严格前后性能对照。热态零解析/编译/准备，单动画修改仅编译1资产并与独立串行重建一致。

[源码依据和完整新结果](docs/TOKENIZER_PARALLEL_INPUT.md)；[新收据摘要](evidence/refactor/tokenizer-parallel/40983673225-1/summary.json)；[计量](evidence/refactor/tokenizer-parallel/40983673225-1/measurement.md)。冷每档一次/热两次，OS缓存未控；不宣称游戏内验收。旧实现与证据保留于[历史迁移记录](docs/BACKEND_PARALLEL_MIGRATION.md)。
