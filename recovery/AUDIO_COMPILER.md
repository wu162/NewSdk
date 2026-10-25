# AudioCompiler 当前状态与交接

更新：2026-10-26（北京时间）。分支 `recovery/audio-compiler-source`，源码 `src/AudioCompiler`。
**阶段验收通过，完整 DLL 尚未恢复；最大缺口是 XMA 编码核心。**

## 已支持

| 路由/能力 | 当前范围 |
|---|---|
| PathMusicMap / Track / Event | MPF、MUS、PATH header、weak reference、runtime、重定位及 CustomData |
| AudioFileMP3Passthrough | 原七连续 MPEG 帧识别/头解析，文件原样传递；静态编入 MIT NLayer 作解码验证 |
| AudioFile 输入 | 单/双声道 PCM8/16/24 RIFF/WAVE、标准 PCM WAVE EXTENSIBLE 子集、AIFF signed8/BE16/BE24 |
| 编码与容器 | NONE/PCM16、XAS v1，RAM/streamed SNR/SNS；编码算法独立重建，运行时不加载原目标 DLL |
| Loop | 合法单 forward WAVE smpl；AIFF 恰一个 INST/MARK、两个唯一匹配 marker、sustain mode1/release mode0；允许块/marker 逆序 |
| 重采样 | SIMEX filter120 已接入，至少两个源帧；保留原短输入及 loop 缩放规则 |
| XMA 辅助组件 | XmaPacketBits / XmaPacketScanner 的 count/bit/frame/scan 整数组；**不能编码 XMA** |

保留五个 BAB IAssetBuilderPlugin 路由、原命名空间/程序集版本及类型处理元数据；不承诺原 C++ 指针辅助方法的二进制 ABI。工程 net472、无签名，Core 仍引用 SDK 基线；NLayer 无独立运行时 DLL。

构建：`dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release`。

## 还差什么

| 优先级 | 缺口 | 下一步/完成门槛 |
|---|---|---|
| 1 | **完整 XMA 编码器** | 当前 representation28 拒绝。先取得真实 CPU 的原始 codec 输出，再恢复数值核心、packet 重写和插件接入；最后组件差分及真实 SDK 验收。见 [XmaNextBatch](audio/XmaNextBatch.md) |
| 2 | AIFC、AIFF release/pingpong/额外 MARK、额外 loop 结构、非标准 WAVE EXTENSIBLE 元数据 | 先按原版探针确认接受范围，再整组恢复并验收；当前显式拒绝，不以替代格式静默输出 |
| 3 | 特殊/损坏 MPEG 流兼容性 | 当前公开 NLayer 后端不能证明与原 RenderWare decoder 普遍等价；需扩大原版接受/拒绝差分，按样本界定兼容范围 |
| 集成 | 生产 SDK/最终 exe、完整真实 MOD | 当前仅测试副本启用恢复插件；未替换生产 DLL/config、未合入 compiler-source、未完成完整 Upheaval 编译 |

**不列为原能力缺失**：原 DLL 本身拒绝 PCM32/IEEE float WAVE；单帧源音频变采样率时，原导入器成功返回却不写 malloc 样本，恢复版明确安全拒绝，不拟合垃圾内存。显式初始化的一帧数值核测试与文件导入是不同边界。

## 当前验收依据

汇总及精确收据路径：[acceptance-current.json](evidence/audio/acceptance-current.json)。以下均为 attempt1，历史结果继续绑定各自源码，不冒充本次文档提交新运行。

| 范围 | sourceCommit | runId / 结果 |
|---|---|---|
| filter120 重采样核 | `25aab8fe5a6b9e762293f9ace71c3edc59e36e36` | [40207847717](https://github.com/wu162/NewSdk/actions/runs/40207847717)：原 x86 CPU 512个 binary64 窗口常数；Debug/Release 各258组、5安全检查 |
| 插件、重采样、XAS 精度及联合 SDK | `684e3310004853455dd0b5b4e53287afb067e93c` | [40209451010](https://github.com/wu162/NewSdk/actions/runs/40209451010)：Debug/Release 各1392条（含448条重采样）、另8条单帧安全拒绝；XAS264组；SDK38次调用、四产物/36份 CustomData 相同 |
| XMA 整数 packet 组件 | `96f72d037b840ad09590688e8d4ec1f998446d8d` | [40209977895](https://github.com/wu162/NewSdk/actions/runs/40209977895)：原 CPU 与 Debug/Release 各1025组、6安全拒绝，含8份 EA 输出块；raw codec packets=0 |

SDK 验收使用相同合法输入、临时 SDK 根，走 EALAModStudio case5 BAB 基础数据流程，比较实际调用的三缓冲区/CustomData及四产物，恢复侧删除原 AudioCompiler DLL。**这是专用联合音频 MOD，不是完整 Upheaval。**

联合 SDK 验收源码与整理前分支头的 `src`、`SDK` 子树相同：
`src=1347625bc33cfa93286af3a119ab070372b4b55f`；
`SDK=1f2d71a20ce8bf7d9343c7a2246c10275abe28f9`。
此说明不把其他测试工具或文档变化算为新验收；后续有实际依赖变化，须重跑受影响完整组。

## 调查入口与必须保留的结论

- [ResamplerKernelAudit](audio/ResamplerKernelAudit.md)：Unicorn FCOS 调宿主 cos，曾有195个 double 常数不同；最终使用真实原 CPU 常数及输出。
- [SingleFrameImportBoundary](audio/SingleFrameImportBoundary.md)：原 `0x404e0` 的 start0/count1 不写样本；安全拒绝不能算业务等价。
- [XasPrecision](audio/XasPrecision.md)：按原 SSE 每步 binary32 修复 Release 跨量化阈值问题。
- [PcmInputAudit](audio/PcmInputAudit.md)、[NativeAudit](audio/NativeAudit.md)、[LoopsAndResampling](audio/LoopsAndResampling.md)：PCM24 先丢低8位转 signed16、AIFF SSND 不计 padding、原 CLR/x87 为53位精度及 loop 语义。
- [XmaAudit](audio/XmaAudit.md)、[XmaNextBatch](audio/XmaNextBatch.md)、[PublicXmaReferences](audio/PublicXmaReferences.md)：XMA 静态链接核心及下一批恢复入口；公开解析器不能代替原编码器验收。
- NLayer 固定来源及修改：[PROVENANCE](../src/AudioCompiler/ThirdParty/NLayer/PROVENANCE.md)。vgmstream/eaxas 等仅作格式/算法交叉核对，未直接复制到产物。

## 历史记录按需读取

[整理前完整阶段记录](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/AUDIO_COMPILER.md)保留全部成功/失败与源码锚点，原证据未删除。常用旧运行：元数据40192722185（各86171条）、首轮联合40197878572（28次 SDK 调用）、AIFF loop40198458731（4次 SDK 调用）、FFT40199685628及原 CPU 复核40199843730。FFT历史阶段本身不证明插件重采样；当前插件重采样以40209451010为准。

原始提取与审计在 `evidence/audio`；原 DLL SHA256：
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
后续只运行实际受影响的完整验证组，复用未变组精确锚点，不重复无关 GameObject/XmlCompiler 测试。
