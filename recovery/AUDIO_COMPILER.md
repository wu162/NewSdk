# AudioCompiler 当前状态与交接

更新：2026-10-26（北京时间）。分支 `recovery/audio-compiler-source`，源码 `src/AudioCompiler`。
**完整 DLL 尚未恢复；最大缺口仍是 XMA 数值编码核心。** 验收必须区分独立组件、实际插件调用、专用 SDK MOD 和完整真实 MOD。

## 已支持范围

| 能力 | 当前范围 |
|---|---|
| PathMusicMap / Track / Event | MPF、MUS、PATH header、weak reference、runtime、重定位及 CustomData |
| MP3 passthrough | 原七连续 MPEG 帧识别/头解析、原文件传递；静态编入 MIT NLayer 作解码验证，特殊/损坏流不承诺普遍等价 |
| WAVE | 单/双声道 PCM8/16/24；EXTENSIBLE 按容器整数位宽解码，原版忽略 cbSize/validBits/mask/GUID，**不是新增 float 解码器** |
| AIFF / AIFC | signed8/BE16/BE24 AIFF；AIFC/NONE 的 COMM 名称/FVER/块顺序已恢复；raw ADP4 文件输入已通过组件/插件差分，SDK尚未验收 |
| 编码/容器 | NONE/PCM16、XAS v1，RAM/streamed SNR/SNS；编码算法独立恢复，无原目标 DLL 运行后端 |
| Loop | 单 forward WAVE smpl；AIFF 恰一个 INST/MARK、两个唯一匹配 marker、sustain mode1/release mode0，允许块/marker逆序 |
| 重采样 | filter120 已接入，至少两个源帧；保留原短输入及 loop 缩放规则 |

五个 BAB IAssetBuilderPlugin 路由、命名空间/程序集版本及类型元数据保留；不承诺原 C++ 指针 helper 的二进制 ABI。工程 net472、无签名，Core 仍引用 SDK 基线；NLayer 没有独立运行时 DLL。

构建：`dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release`。

## 本轮已取得的验证

完整 sourceCommit/runId 以对应收据为准；文档提交不冒充新运行。

| 范围 | sourceCommit / runId | 已验证结果 |
|---|---|---|
| EXTENSIBLE 元数据、XMA PCM/rewriter/noise | `5d56d52ff11510c7e93f83b47d08b1def5c99d27` / [40232917060](https://github.com/wu162/NewSdk/actions/runs/40232917060) | 输入各2400条、SDK120次调用；XMA预转换124例/142193样本、rewriter801例含8真实闭环、noise6360例，均Debug/Release |
| AIFC/NONE | `3863a632faac79ef586569e134b7eb46014fab94` / [40233228823](https://github.com/wu162/NewSdk/actions/runs/40233228823) | 各1440插件记录、60 sample-bit检查/10拒绝；旧2600回归及SDK120次调用 |
| 六个 XMA integral window/copy 叶函数 | `579c95661d90f067ab414c4133f033c2571b96e1` / [40233712043](https://github.com/wu162/NewSdk/actions/runs/40233712043) | 各6888组、12拒绝；无原 DLL 的隔离候选宿主 |
| ADP4 独立 decoder | 同上579c956 / 40233712043 | 各18243组、11拒绝；原插件96例双进程相同；尚不等于ADP4文件/SDK验收 |
| XMA EA/WAVE/BE seek 元数据 | `1ebd94a9c61dc4c44ea66784075f43062840ad41` / [40233897932](https://github.com/wu162/NewSdk/actions/runs/40233897932) | 各384个合成state、15拒绝；原合成state边界保留；5495da2/40234514000另取得8个真实state与原分配闭环 |
| 原始 codec 输出 | `cd66d9a2c821bc41f1f975aac8ac0a5bac6a6499` / [40232525056](https://github.com/wu162/NewSdk/actions/runs/40232525056) | 8例各两次、三返回分配逐byte一致，捕获ee00重写前的raw packet |
| 长/真实音频、实际window目标 | 579c956 / 40233712043 | 8例16次原编码，121 packet、4个多block案例；纯native journal111次实际调用确认97d97→98267，三原始输出SHA不变 |

两个 SDK 批次均用相同合法输入和临时 SDK 根，走 EALAModStudio case5 BAB 基础数据，实际三缓冲区/CustomData、四产物相同，恢复侧删除原 AudioCompiler DLL。它们是**最小专用音频 MOD，不是完整 Upheaval**。

XMA 所有组件仍未组成完整编码器，representation28继续明确拒绝。新增源 `5495da2d703ca429434de369f5414cd45e4de819` / 40234514000 的ADP4文件各4224条、旧4040回归，以及8个真实metadata state均通过；该运行整体因独立snapshot工具导出名错误失败，不能称整体成功。该工具修复后 `de20ba24dc6941c56ee27ade3c897ece6a22b17e` / 40234805270 取得111真实window快照及各3240+111调度回放成功。FFT192组也已独立通过；DCT和raw packet构造正在整组验证。

## 还差什么

1. **完整 XMA 编码器**：加窗调度/真实输入回放与table-backed FFT已通过；继续DCT、后续分析/量化/bitstream，再连接预转换、元数据和packet封装，最后整组插件及必要SDK里程碑验收。见 [XmaNextBatch](audio/XmaNextBatch.md)。
2. 更多 AIFF loop/marker、非零 SSND offset等仍在当前边界外；blockSize忽略语义正在新组件矩阵验收。ADP4文件已取得组件差分但SDK留待里程碑。不要把标准 Apple ima4 包当成原 EA raw ADP4。
3. 特殊/损坏 MPEG 流的原版接受/拒绝兼容，当前 NLayer 后端不能证明 RenderWare decoder 全量等价。
4. 最终集成：未替换生产SDK DLL/config，未合入compiler-source、未接入最终exe、未完成完整Upheaval验证。当前ADP4新增路径尚无SDK验收。

原 DLL 本身拒绝 PCM32/IEEE-float WAVE，不将其记作待恢复原能力。原SIMEX单帧文件导入可能成功却不写malloc样本；当前单帧重采样/ADP4文件输入安全拒绝，不拟合旧堆内容。显式初始化的单帧数值组件是另一边界。

## 续接规则与证据入口

**用户要求不要频繁触发完整SDK真实验证。** 先累计组件/插件差分与定向原版取证，真实SDK按更大里程碑合批且显式启用；当前工作流默认components/all只运行独立组件，不隐式启动SDK。未变成功组复用精确锚点，只对实际依赖变更组运行完整必要矩阵。不得把组件支持写成SDK已验收。

- [验收索引](evidence/audio/acceptance-current.json)；本轮新运行收据在 `evidence/audio/<group>/<runId>-1`，最终同步索引时不改旧锚点。
- [输入元数据](audio/InputMetadataBoundary.md)、[AIFC/NONE](audio/AifcNoneBoundary.md)、[ADP4](audio/Adp4Boundary.md)。
- [XMA预转换](audio/XmaPcmConversion.md)、[数值noise](audio/XmaCodecCore.md)、[window叶函数](audio/XmaPcmWindow.md)、[window调度](audio/XmaPcmWindowScheduler.md)、[metadata](audio/XmaCodecMetadata.md)。
- [重采样](audio/ResamplerKernelAudit.md)、[单帧边界](audio/SingleFrameImportBoundary.md)、[XAS精度](audio/XasPrecision.md)：真实原CPU常数权威；x87中间53位与SSE逐步binary32不可混用。
- [NLayer来源](../src/AudioCompiler/ThirdParty/NLayer/PROVENANCE.md)、[公开XMA交叉核对](audio/PublicXmaReferences.md)。公开decoder/parser不能代替原编码器验收。

[整理前阶段历史](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/AUDIO_COMPILER.md)保留旧成功/失败及完整SHA；其“src整树1347625与SDK验收相同”只适用当时固定提交，不能证明本轮当前整树已SDK验收。
原 DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
