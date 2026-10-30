# 当前：冗余安全清理，Windows 验证中

`cleanup/redundant-defenses` 基于刷新后的 `b89fbadb`。整批实现和相应测试已完成，尚未通过本轮 Windows 验收。删除机制、保留依据和统一验证入口见 [DIRECT_OUTPUT](docs/DIRECT_OUTPUT.md)。无合并、强推或规则修改。

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
