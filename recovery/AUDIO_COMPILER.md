# AudioCompiler 当前状态

分支 `recovery/audio-compiler-source`；源码 `src/AudioCompiler`。2026-10-26（北京时间）。
**仅支持 PC / Win32。** 所有非 Win32 目标在元数据、初始化、直接编译和编码策略入口明确拒绝；非 PC 实现、平台路由、专用字节序分支及恢复规划均已移除。输入/编码格式自身要求的字节序处理保留。精确历史锚点集中在[验收索引](evidence/audio/acceptance-current.json)。

## PC 产品能力

| 范围 | 当前支持与边界 |
|---|---|
| 五个 BAB 路由 | PathMusicMap/Track/Event、AudioFileMP3Passthrough、AudioFile；runtime/重定位/CustomData |
| WAVE | mono/stereo PCM8/16/24；EXTENSIBLE 按整数容器位宽解码，原版忽略 cbSize/validBits/mask/GUID，并非 float 解码 |
| AIFF/AIFC | AIFF signed8/BE16/BE24；AIFC/NONE 和 EA raw ADP4（非标准 Apple ima4）；SSND blockSize 任意 uint32 被忽略，offset 仍要求0 |
| 编码/重采样 | NONE/PCM16、XAS v1、RAM/streamed SNR/SNS；filter120 及原短输入/loop 缩放，至少两个源帧 |
| Loop | 单 forward WAVE smpl；AIFF 一个 INST/MARK、两个唯一匹配 marker、sustain mode1/release mode0 子集，允许块/marker 逆序 |
| MP3 | 原七连续 MPEG 帧识别/头解析、原文件传递；静态编入 MIT NLayer 解码验证，特殊/损坏流不承诺普遍等价 |

构建：`dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release`。
工程 net472、无签名，Core 仍引用 SDK 基线；NLayer 无独立运行时 DLL。不承诺原 C++ 指针 helper 的二进制 ABI。生产源码没有加载原 AudioCompiler 作为运行后端；原 DLL 调用及 native 插桩仅在测试宿主。

## 当前验证

源码 `376243310ad44a164c7ac29b100e5489c5d93928` / [40241451234](https://github.com/wu162/NewSdk/actions/runs/40241451234) 已通过 Win32-only 合批验证，源码树 `44d8089ff03c9af29c6e6cb12245eff8ba0c25c2`。每种 Debug/Release：元数据12,311项、PC插件路由2,175项及8项单帧安全拒绝、AIFC720项、ADP4 2,136项、SSND1,680项、非PC拒绝48项；PCM/XAS组件和整工程构建通过。仅对照Win32输出，其他平台只验证拒绝。SDK运行0次。 历史混合平台结果不作为当前源码的新验证。

## 历史 PC 验证

既有结果必须按原 sourceCommit/runId 复用，不能写成删除代码后的新运行。历史矩阵包含其他平台时，PC 子集以原收据的具体记录为准；历史总数不是当前 PC 专用矩阵数量。

| 完整能力组 | 历史运行 / 结果 |
|---|---|
| EXTENSIBLE 元数据 | 40232917060：每配置2400插件记录、120资产 PC 专用 SDK |
| AIFC/NONE | 40233228823：每配置1440插件记录、60样本检查/10拒绝，旧2600回归、120资产 PC 专用 SDK |
| raw ADP4 | 40233712043：decoder 每配置18243组/11拒绝；40234514000：文件每配置4224插件记录、144样本检查/12拒绝，旧4040回归 |
| SSND blockSize | 40236037500：每配置3192插件记录，2736非零与零值闭环、168样本检查/168 offset拒绝，AIFF loop200回归 |
| 删除前整工程构建 | 40237397076：默认 Debug/Release 通过，source `ec1e34657edb8395570d8e325fe81dc8ab762c71`，旧源码树 `aaac794cc64e04b9f68e202a53d9ae245f5d6ace`；不作为删除后树的构建证明 |

两个 SDK 批次走 EALAModStudio case5 BAB 基础数据流程，原/恢复版使用同一合法输入和临时 SDK 根，实际三缓冲区、四产物及 CustomData 一致，恢复侧删除原目标 DLL。**这是最小专用 PC 音频 MOD，不是完整 Upheaval。** ADP4/SSND 新增路径仅组件/插件验收，SDK 尚未运行。

40234514000 整体因另一个独立探针工具失败，但 ADP4 收据成功；不得将局部成功写成整体运行成功。当前不再维护已取消平台的探针或其待办。

## PC 还缺什么

1. 额外/重复/不匹配 AIFF MARK、多 INST/MARK、AIFC loop 元数据、WAVE 多 smpl/多 loop/nonforward 的接受行为。release/pingpong 原版只表现为忽略/不安装 forward loop，不能暗示原版具备完整高级循环。
2. 非零 SSND offset 的有界兼容：原 importer 的长度修正可能读出声明的 sound 范围，需先建立已初始化、不会越界的确定契约，不能照搬未定义行为。
3. 特殊/损坏 MPEG 的接受/拒绝兼容。当前 NLayer 与原 RenderWare 在这些输入上尚无普遍等价证明。
4. ADP4/SSND 等能力的 PC SDK 合批验收，最终 exe 集成、合入 `compiler-source` 和完整 Upheaval 验证。

PCM32/IEEE-float WAVE、sowt 等原路径本就拒绝的格式不列作待恢复原能力；单帧 ADP4 文件/重采样为原未初始化内存安全拒绝。已取消平台的编码器、数值分支和状态驱动不再属于剩余工作。

**不要频繁触发完整 SDK 真实验证。** 先累计组件/插件差分，实际依赖变化组完整验证，SDK 按更大里程碑显式合批。未变组复用精确旧锚点；文档提交不冒充新运行。

## 按需阅读

[输入元数据](audio/InputMetadataBoundary.md) · [AIFC/NONE](audio/AifcNoneBoundary.md) · [ADP4](audio/Adp4Boundary.md) · [SSND](audio/SsndBlockSize.md)

[重采样](audio/ResamplerKernelAudit.md)、[单帧边界](audio/SingleFrameImportBoundary.md)、[XAS 精度](audio/XasPrecision.md)保留原 CPU 常数权威与 x87/SSE 精度要求；[NLayer 来源](../src/AudioCompiler/ThirdParty/NLayer/PROVENANCE.md)保留来源边界。

[整理前历史](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/AUDIO_COMPILER.md)只适用于相应固定提交，不能证明当前整树已 SDK 验收。原 DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
