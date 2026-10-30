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
