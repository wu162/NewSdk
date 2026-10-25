# AudioCompiler 当前交接

更新：2026-10-26（北京时间）。目标分支 `recovery/audio-compiler-source`；产品源码 `src/AudioCompiler`。先读 AGENTS.md、EFFICIENCY.md；禁止强推。

## 本轮结论

- 新增WAVE EXTENSIBLE忽略扩展元数据、AIFC/NONE、raw ADP4文件输入、SSND任意uint32 blockSize忽略语义；非零SSND offset仍拒绝。ADP4/SSND已过真实插件ProcessInstance三缓冲区/CustomData差分，**SDK留待里程碑合批**。
- XMA新增预转换、packet重写/构造、元数据、PRNG/noise、integral window及调度、table-backed FFT/DCT、完整97b00 signal wrapper与855ef谱门限。均有原CPU+Debug/Release逐位证据；window/DCT/signal各有111份实际编码输入重放。
- **完整XMA编码器仍未恢复，representation28继续明确拒绝。** Helpers通过不等于初始化/分析/量化/entropy/连续编码和完整帧编排已恢复。
- 整工程Debug/Release构建通过：`ec1e34657edb8395570d8e325fe81dc8ab762c71` / [40237397076](https://github.com/wu162/NewSdk/actions/runs/40237397076)，产品源码树 `aaac794cc64e04b9f68e202a53d9ae245f5d6ace`。本地整树已核对相同；这只是源码构建，不是SDK/MOD编译。

## 下一轮直接入口

先读 [XmaAfterTransform](audio/XmaAfterTransform.md)：从真实分析/量化/entropy与driver缺口续接。`5e8f0`完整帧appender、输出FIFO/缓冲回收/码率时钟及连续编码/flush仍缺；不要再从已经完成的packet整数helper、window、FFT或DCT重做。

当前能力、构建与输入边界：[AUDIO_COMPILER](AUDIO_COMPILER.md)。完整sourceCommit/runId、数量及每组收据：[acceptance-current.json](evidence/audio/acceptance-current.json)。[XmaNextBatch](audio/XmaNextBatch.md)保留已完成组导航和工具边界。

真实raw packet状态92项仅为header63/next29；padding0及实际bitsplice状态未覆盖。metadata的8真实state可证明EA/seek与原codec分配相同，WAVE52只是原真实state重放。早期managed-VEH外围探针失败保留历史；新的纯native成功不扩大为旧外围3site全覆盖。

**用户最新要求：不要频繁触发完整SDK真实验证。** 后续组件/插件差分优先，实际依赖变更组完整验证；SDK按更大里程碑显式合批。工作流默认components/all不启动SDK。两个早期120资产SDK是最小专用音频MOD，不是完整Upheaval；未替换生产SDK DLL/config，未合入compiler-source，未接入最终exe。

其他子系统历史按[整理前导航](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/RESUME.md)按需读取，不将旧整树SDK证明套到当前源码。
