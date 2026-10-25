# AudioCompiler 当前状态

分支 `recovery/audio-compiler-source`；源码 `src/AudioCompiler`。2026-10-26（北京时间）。
**本轮能力已取得对应验收，完整DLL/XMA编码器仍未恢复。** 精确锚点集中在[验收索引](evidence/audio/acceptance-current.json)，避免在多份文档复制完整历史。

## 产品能力

| 范围 | 当前支持与边界 |
|---|---|
| 五个BAB路由 | PathMusicMap/Track/Event、AudioFileMP3Passthrough、AudioFile；runtime/重定位/CustomData |
| WAVE | mono/stereo PCM8/16/24；EXTENSIBLE按整数容器位宽解码，原版忽略cbSize/validBits/mask/GUID，并非新增float解码 |
| AIFF/AIFC | AIFF signed8/BE16/BE24；AIFC/NONE和EA raw ADP4（非标准Apple ima4）；SSND blockSize任意uint32被忽略，offset仍要求0 |
| 编码/重采样 | NONE/PCM16、XAS v1、RAM/streamed SNR/SNS；filter120及原短输入/loop缩放，至少两个源帧 |
| Loop | 单forward WAVE smpl；AIFF一个INST/MARK、两个唯一匹配marker、sustain mode1/release mode0子集，允许块/marker逆序 |
| MP3 | 原七连续MPEG帧识别/头解析、原文件传递；静态编入MIT NLayer解码验证，特殊/损坏流不承诺普遍等价 |
| XMA | 已恢复多个独立结构/数值组件，**尚不能编码XMA，representation28明确拒绝** |

构建：`dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release`。
工程net472、无签名，Core仍引用SDK基线；NLayer无独立运行时DLL。不承诺原C++指针helper的二进制ABI。生产源码没有加载原AudioCompiler作为运行后端；原DLL调用和native插桩仅在测试宿主。

## 本轮验证摘要

各数值/结构候选均比较实际原Windows x86 CPU、Debug/Release完整位模式，原DLL从恢复宿主排除；拒绝检查另计。sourceCommit全文及收据路径见索引。

| 完整能力组 | 运行 / 结果 |
|---|---|
| EXTENSIBLE元数据 | 40232917060：各2400插件记录、120资产专用SDK |
| AIFC/NONE | 40233228823：各1440插件记录、60样本检查/10拒绝，旧2600回归、120资产专用SDK |
| raw ADP4 | 40233712043：decoder各18243组/11拒绝；40234514000：文件各4224插件记录、144样本检查/12拒绝，旧4040回归 |
| SSND blockSize | 40236037500：各3192插件记录，含2736非零/零值闭环、168样本检查/168 offset拒绝，AIFF loop200回归 |
| XMA预转换/重写/noise | 40232917060：124例/142193样本、801重写例含8真实闭环、6360 noise例 |
| XMA window/调度 | 40233712043：六叶各6888组；40234805270：调度各3240 synthetic＋111实际输入，15拒绝 |
| XMA FFT/DCT | 40235031462：order0..15双向192组/6拒绝；40236037500：N64..2048的DCT各360＋111真实输入输出，9拒绝 |
| XMA signal/谱门限 | 40236818983：完整97b00 wrapper各5760＋111真实输入输出，13拒绝；40237077516：独立855ef各2400组/11拒绝 |
| XMA元数据 | 40233897932：384 synthetic；40234514000：8真实state的EA/seek与原codec分配闭环，WAVE52为原真实state重放 |
| XMA raw packet | 40236037500：1106例（1098 synthetic＋8真实payload）/15拒绝；40236621021：92真实state仅header63/next29，padding0、实际bitsplice状态未覆盖 |
| 整工程构建 | 40237397076：默认Debug/Release通过；源码树`aaac794cc64e04b9f68e202a53d9ae245f5d6ace`与当前产品源码相同 |

原版取证另保留40232525056的8例/16次raw编码、40233712043的长/真实音频8例/16次编码（121packet、4多block案例）。window/DCT/signal插桩都要求原三份最终返回分配与未插桩原版一致。

两个SDK批次走EALAModStudio case5 BAB基础数据流程，原/恢复版同一合法输入和临时SDK根，实际三缓冲区、四产物及CustomData一致，恢复侧删除原目标DLL。**这是最小专用音频MOD，不是完整Upheaval。** ADP4/SSND新增路径仅组件/插件验收，SDK尚未运行。

40234514000整体因独立snapshot工具导出名错误失败，但ADP4及metadata收据成功；修复后40234805270整体通过。旧managed-VEH外围探针40232992739失败保留，不能作可用基线。不得将局部成功写成整体运行成功。

## 还缺什么

1. **完整XMA**：初始化与状态驱动、channel分析/量化/entropy、连续编码和flush；完整`5e8f0`帧appender、FIFO `60136→728e3`、`70fd6`缓冲回收、`710a2`码率时钟等编排。已恢复helpers及transform不等于整个编码链；下一入口见[XmaAfterTransform](audio/XmaAfterTransform.md)。
2. generic reader和超出table-backed域的数值分支；当前window仅integral61/78/94，FFT order0..15、DCT N64..2048，不扩大到CRT trig或8/float输入分支。
3. 额外/重复/不匹配AIFF MARK、多INST/MARK、AIFC loop元数据、WAVE多smpl/多loop/nonforward的接受行为；release/pingpong原版只表现为忽略/不安装forward loop，不能暗示原版具备完整高级循环。非零SSND offset需先界定不会读出sound范围的确定行为。
4. 特殊/损坏MPEG的接受/拒绝兼容。PCM32/IEEE-float WAVE、sowt等原路径本就拒绝的格式不列作待恢复原能力；单帧ADP4文件/重采样为原未初始化内存安全拒绝。
5. ADP4/SSND等SDK里程碑、最终exe集成、合入compiler-source和完整Upheaval验证。

**不要频繁触发完整SDK真实验证。** 先累计组件/插件差分，实际依赖变更组完整验证，SDK按更大里程碑显式合批。工作流默认components/all不运行SDK。未变组复用精确旧锚点，文档提交不冒充新运行。

## 按需阅读

[输入元数据](audio/InputMetadataBoundary.md) · [AIFC/NONE](audio/AifcNoneBoundary.md) · [ADP4](audio/Adp4Boundary.md) · [SSND](audio/SsndBlockSize.md) · [XMA导航](audio/XmaNextBatch.md) · [Signal](audio/XmaSignalTransform.md) · [Gate](audio/XmaSpectralGate.md) · [下一分析链](audio/XmaAfterTransform.md)

[重采样](audio/ResamplerKernelAudit.md)、[单帧边界](audio/SingleFrameImportBoundary.md)、[XAS精度](audio/XasPrecision.md)保留原CPU常数权威与x87/SSE精度要求；[NLayer来源](../src/AudioCompiler/ThirdParty/NLayer/PROVENANCE.md)、[公开XMA核对](audio/PublicXmaReferences.md)保留来源边界。

[整理前历史](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/AUDIO_COMPILER.md)的src整树1347625相同证明只适用于当时固定提交，不能证明当前整树已SDK验收。原DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
