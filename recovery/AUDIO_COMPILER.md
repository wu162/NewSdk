# AudioCompiler 当前状态

目标 `recovery/audio-compiler-source`；源码 `src/AudioCompiler`；仅 PC / Win32。
2026-10-26（北京时间）。非 Win32 在元数据、初始化、直接编译和编码策略入口明确拒绝。
生产源码不加载原 AudioCompiler；原 DLL 仅作测试 oracle。SDK/tools 原 DLL/config 未替换。

## 当前验收

- **已通过**：`bb4a225` / [40243817293](https://github.com/wu162/NewSdk/actions/runs/40243817293)。Debug/Release 每配置：loop4224、optional3888、offset2704、MPEG676、旧插件2175、AIFC720、ADP42136、SSND1680；整工程构建通过。MPEG 原机器码32783 header /441 identifier /324 info另有15识别边界。
- **已通过 SDK**：同源码、同运行的4488资产专用 PC 音频 MOD，原/恢复版实际三缓冲区、四产物及4488份CustomData一致，恢复侧删除原目标 DLL。该批不是完整 Upheaval。
- **最终候选已通过**：`2d64684742f9b19808d8fd721fbe31a549907c37` / [40244825458](https://github.com/wu162/NewSdk/actions/runs/40244825458) attempt1 success。D/R loop4224、optional3888、COMM7776、offset2704、MPEG1084及旧受影响完整回归、整工程构建均通过；原机器码32783 header/441 identifier/324 info/72容器及15识别边界通过。
- **新增SDK已通过**：同最终源码、同运行的1820资产专用PC MOD（COMM1728、MPEG44、ADP4 offset组合48），原/恢复版实际三缓冲区、四产物、1820份CustomData一致；恢复侧删除原目标DLL。前一4488保留原锚点，不冒充本次重跑。

精确收据、配置、源码树和历史锚点见[验收索引](evidence/audio/acceptance-current.json)。

## PC 能力与边界

| 范围 | 当前实现 |
|---|---|
| 五个 BAB 路由 | PathMusicMap/Track/Event、AudioFileMP3Passthrough、AudioFile；三缓冲区和CustomData |
| 输入 | mono/stereo PCM8/16/24 WAVE/AIFF；EXTENSIBLE按整数容器位宽；AIFC/NONE及EA raw ADP4（非Apple ima4） |
| 编码 | NONE/PCM16、XAS v1；RAM/streamed SNR/SNS；filter120重采样，至少两个源帧 |
| WAVE loop | 原无padding查找的首smpl/首条range；type等被忽略，仍是普通forward loop，非高级pingpong播放 |
| AIFF/AIFC loop | 多INST/MARK、额外/重复marker；INST-only/空MARK/双端均不匹配为无loop；release忽略；mode0/2无forward loop |
| COMM过滤 | 用最后COMM的signed声明帧数先筛marker，负值/超声明位置跳过，之后才按SSND长度修正actual frames |
| SSND | 任意uint32 blockSize忽略；非零offset仅接受完整采样窗口在声明sound范围内，保留原raw-length帧数 |
| MP3透传 | 原七帧识别、EOF终止、MPEG头信息与有界WAVE/AIFF/AIFC优先级；按原SIMEX_info仅查信息，不解PCM；原文件逐字节复制 |

原SIMEX_info不执行PCM解码，因此已删除错误增加的全流解码门槛及全部未引用NLayer源码；不再规划恢复一个透传流程不需要的RenderWare解码器。
工程net472、无签名；Core仍引用SDK基线。构建：`dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release`。
不承诺原C++指针helper二进制ABI。

## 尚未完成

用户要求先交接，本轮停止新运行。既有两个SDK批次及最终组件均已终态成功，不重复触发。

1. 完整固定Upheaval/PublicCases的音频替换验收（AptUI/全局/基础数据，其他插件用原SDK）。入口 `TestAudioUpheavalSdk.ps1`，工作流已准备源码树/SDK收据门槛，保留194个预编译.asset输入并检查真实AudioFile调用。完整Upheaval全部WAV，不替代AIFF声明边界SDK。
2. 合入`compiler-source`；合并时保留其通用续接导航。SDK及shared recovery/src源码树均与目标b122a54相同，不重跑未变编译器。
3. 最终exe集成。已查`refactor/compiler-exe-integration`的46b8785仅有联合基线准备，尚无可接入的最终exe工程。

继续明确拒绝：MARK-only原未初始化mode、单端匹配、无效loop范围、结构损坏/marker超限，以及超出SSND的offset。
尚未界定的畸形容器识别组合不承诺普遍等价。PCM32/IEEE-float、sowt等原路径本就拒绝的格式不是待恢复能力；单帧ADP4/import-resample原未初始化行为保持安全拒绝。
非PC编码器、分支与规划不属于待办。不要频繁触发完整SDK；完整能力组先组件差分，SDK按实际新增范围合批，未变范围保留原sourceCommit/runId。

## 按需阅读

[Loop](audio/LoopMetadataBoundary.md) · [COMM](audio/AiffMarkerRangeBoundary.md) · [Offset](audio/SsndOffsetBoundary.md) · [MPEG](audio/MpegBoundary.md) · [SDK覆盖审计](audio/CommSdkCoverageAudit.md)

[输入元数据](audio/InputMetadataBoundary.md) · [AIFC](audio/AifcNoneBoundary.md) · [ADP4](audio/Adp4Boundary.md) · [SSND blockSize](audio/SsndBlockSize.md)

[重采样](audio/ResamplerKernelAudit.md) · [单帧安全](audio/SingleFrameImportBoundary.md) · [XAS精度](audio/XasPrecision.md)。原DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
