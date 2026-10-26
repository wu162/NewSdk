# AudioCompiler 当前状态

审查分支 `integration/audio-win32-sdk`，PR#10目标 `recovery/compiler-source`（等待用户批准）；源码 `src/AudioCompiler`；仅 PC / Win32。
2026-10-26（北京时间）。非 Win32 在元数据、初始化、直接编译和编码策略入口明确拒绝。
生产源码不加载原 AudioCompiler；原 DLL 仅作测试 oracle。SDK/tools 原 DLL/config 未替换。

## 本轮新增 WAVE4/6 已验收

源码 `ed1a7bcbec4a7a89d08369b9668129c8c554f8ec`，音频树 `a8e47a2e737ca7e35a486ec17e5eaaa913c8678e`；[运行40250750964](https://github.com/wu162/NewSdk/actions/runs/40250750964) attempt1 **success**，证据提交 `7eba9f108008ae424b18d10f56bdbcb6ff5487fd`。

D/R各1728个WAVE多声道原版差分通过：4/6×8/16/24×WAVE/EXTENSIBLE×PCM/XAS×RAM/stream、127/128/129/4096帧、四采样率、无loop/start0/长intro；全部受影响旧组通过。每配置36拒绝、64个原版公开入口案例/128次直接比较、15个非法声道检查通过。真实SDK同1728组合全部实际回调，四产物、1728份CustomData及三缓冲区一致，恢复侧无原AudioCompiler DLL。仅临时SDK替换，仓库SDK/tools/config不变。

WAVE重排实证：4ch取源[0,1,3,2]，6ch取源[0,2,1,4,5,3]，导入时只执行一次。AIFF/AIFC NONE仍仅1/2声道；其4/6原版探针192案例/384进程已通过并确认保持原序，留下一完整组。ADP4>2在原native也不支持，继续拒绝。

PR [#10](https://github.com/wu162/NewSdk/pull/10)供用户审查；未经用户明确批准不得合并。下面历史SDK保持原source/run，不冒充新源码重跑。

## 当前验收

- **已通过**：`bb4a225` / [40243817293](https://github.com/wu162/NewSdk/actions/runs/40243817293)。Debug/Release 每配置：loop4224、optional3888、offset2704、MPEG676、旧插件2175、AIFC720、ADP42136、SSND1680；整工程构建通过。MPEG 原机器码32783 header /441 identifier /324 info另有15识别边界。
- **已通过 SDK**：同源码、同运行的4488资产专用 PC 音频 MOD，原/恢复版实际三缓冲区、四产物及4488份CustomData一致，恢复侧删除原目标 DLL。该批不是完整 Upheaval。
- **最终候选已通过**：`2d64684742f9b19808d8fd721fbe31a549907c37` / [40244825458](https://github.com/wu162/NewSdk/actions/runs/40244825458) attempt1 success。D/R loop4224、optional3888、COMM7776、offset2704、MPEG1084及旧受影响完整回归、整工程构建均通过；原机器码32783 header/441 identifier/324 info/72容器及15识别边界通过。
- **新增SDK已通过**：同最终源码、同运行的1820资产专用PC MOD（COMM1728、MPEG44、ADP4 offset组合48），原/恢复版实际三缓冲区、四产物、1820份CustomData一致；恢复侧删除原目标DLL。前一4488保留原锚点，不冒充本次重跑。

- **完整固定 Upheaval 音频替换已通过**：源码提交 `c078bce6e5e28866dfd3c25acabda731203b2556` / [40248486242](https://github.com/wu162/NewSdk/actions/runs/40248486242) attempt1 success；生产音频树仍为 `ee6074a096795aa66b682fcb363e360feabe4d2f`。固定输入 `068643219fded09a05fb7b75c6507b769c430316`，2738文件、194个.asset保留；原/恢复侧 case3/4/5 共10次BAB调用全部exit0，20份产物全部一致，各侧1293份CustomData、1289次音频回调，1281个required声明与1281个actual unique资产匹配，恢复侧原AudioCompiler缺席。收据 `evidence/audio/upheaval-sdk-validation/40248486242-1/validation.json`，提交 `ef87339d3f76848687b714a394a9e1ff00ad1923`。只替换AudioCompiler，其他插件仍为原SDK；该WAV mono语料不证明4/6声道，也不替代AIFF/AIFC专用SDK。

精确收据、配置、源码树和历史锚点见[验收索引](evidence/audio/acceptance-current.json)。

## PC 能力与边界

| 范围 | 当前实现 |
|---|---|
| 五个 BAB 路由 | PathMusicMap/Track/Event、AudioFileMP3Passthrough、AudioFile；三缓冲区和CustomData |
| 输入 | WAVE1/2/4/6、AIFF1/2声道 PCM8/16/24；EXTENSIBLE按整数容器位宽；AIFC/NONE及EA raw ADP4（非Apple ima4） |
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

1. 下一组为AIFF/AIFC NONE4/6：原版已确认原序，恢复版尚未启用。完整loop/COMM/SSND、重采样、流式分块差分和新SDK仍需完成；见[原生审计与原版实证](audio/MultichannelAiffNativeAudit.md)。多声道ADP4不是缺失功能，原native明确拒绝。
2. PR[#10](https://github.com/wu162/NewSdk/pull/10)尚未合并。保留compiler-source通用导航、SDK及共享源码；必须由用户明确批准后才能合并。

[音频合入审查](audio/AudioIntegrationReview.md)与[WAVE多声道验收](audio/MultichannelBoundary.md)记录范围和精确证据。

继续明确拒绝：MARK-only原未初始化mode、单端匹配、无效loop范围、结构损坏/marker超限，以及超出SSND的offset。
尚未界定的畸形容器识别组合不承诺普遍等价。PCM32/IEEE-float、sowt等原路径本就拒绝的格式不是待恢复能力；单帧ADP4/import-resample原未初始化行为保持安全拒绝。
非PC编码器、分支与规划不属于待办。不要频繁触发完整SDK；完整能力组先组件差分，SDK按实际新增范围合批，未变范围保留原sourceCommit/runId。

## 按需阅读

[Loop](audio/LoopMetadataBoundary.md) · [COMM](audio/AiffMarkerRangeBoundary.md) · [Offset](audio/SsndOffsetBoundary.md) · [MPEG](audio/MpegBoundary.md) · [SDK覆盖审计](audio/CommSdkCoverageAudit.md)

[输入元数据](audio/InputMetadataBoundary.md) · [AIFC](audio/AifcNoneBoundary.md) · [ADP4](audio/Adp4Boundary.md) · [SSND blockSize](audio/SsndBlockSize.md)

[重采样](audio/ResamplerKernelAudit.md) · [单帧安全](audio/SingleFrameImportBoundary.md) · [XAS精度](audio/XasPrecision.md)。原DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
