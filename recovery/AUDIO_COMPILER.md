# AudioCompiler 源码恢复（阶段验收通过，仍有未恢复能力）

目标：从 `recovery/compiler-source` 的 `b122a54abe4405516b021e425b8fb9235d914a0e` 新建 `recovery/audio-compiler-source`。
原 DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
代码按本轮用户要求放在仓库根目录 `src/AudioCompiler`；原版 SDK/tools 二进制与配置不修改。

## 实现范围

- `Plugin` 实现 BAB `IAssetBuilderPlugin` 五个路由，保留原命名空间、程序集版本和类型/处理元数据。生产接入只需接口，不承诺原 C++ 指针辅助方法的二进制 ABI。
- `PathMusicMap`：读取 MPF 字节并生成 runtime、重定位。
- `PathMusicTrack`：复制 MUS 到 CustomData，解析 PATH_TRACK_ header handle，生成 runtime 与字符串分配。
- `PathMusicEvent`：解析 PATH_EVENT_ header、weak reference、IsXferable，生成 runtime。
- `AudioFileMP3Passthrough`：恢复原生七连续 MPEG 帧识别和头解析；MIT NLayer 源码作为实际 MPEG 解码验证后端。文件原样传递，生成 subtitle/channels/streamed runtime。第三方代码静态编入目标程序集，不使用独立 NLayer DLL。
- `AudioFile`：默认 `PcmWaveAudioCompiler` 后端支持单/双声道 PCM8/16/24 RIFF/WAVE、标准 PCM WAVE EXTENSIBLE 子集、AIFF signed8/BE16/BE24、NONE 和 XAS v1、RAM/streamed SNR/SNS 容器；合法单 forward WAVE smpl loop 和保守 AIFF forward sustain loop 已恢复；恢复平台策略、默认 subtitle、runtime 布局、重定位和 CustomData。SIMEX filter120 重采样已接入，限定至少两个源帧，保留原短输入和 loop 缩放规则。
- `Pcm16Encoder` 与 `Xas1Encoder` 从原 DLL 算法独立重建，不读取/加载原目标 DLL 作为运行后端。

明确未覆盖：AIFC、AIFF release/pingpong/额外MARK行为、非标准 WAVE EXTENSIBLE 元数据、PCM32/IEEE float WAVE、XMA、单帧源音频的采样率转换。原 DLL 本身也不接受 PCM32/IEEE float WAVE，不将其误记为待恢复的原能力。上述输入显式拒绝，不静默使用不同编码格式。MP3 采用公开解码后端，损坏/特殊 MPEG 流的接受行为不保证普遍等同原 RenderWare decoder；必须按实测样本描述兼容性，不能称原 native decoder 全量还原。

## 构建

`dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release`。
工程目标 net472（为公开 MPEG 后端提供兼容运行时），无签名，Core 引用保留原 SDK 基线。代码可直接纳入将来的单 exe；此独立工程用于阶段验证，不代表已修改生产 SDK。

## 第1阶段联合验收（2026-10-25）

源码 `ad37841d5bf7fba3cbdede2bdae60da2b49d9639`，运行 [40197878572](https://github.com/wu162/NewSdk/actions/runs/40197878572) attempt1 **整体成功**。实际收据均绑定该源码，不将后续文档提交冒充新运行。

- Debug/Release 各1001条完整插件记录逐字节一致：business239、PCM16、XAS12、defaults4、rates2、main真实音频8、PCM8 24、formats480、loops216；比较InstanceData、RelocationData、ImportsData和完整CustomData，恢复宿主无原目标DLL。
- Debug/Release 各1008条原native block-size标量样本（PCM/XAS各504）和262条XAS编码块样本通过；独立工程两配置构建通过。候选输入拒绝19项、loop结构/拒绝14项通过，不把安全拒绝样本算为原版业务等价。
- 真实SDK从EALAModStudio case5 BAB基础数据流程执行，原版/恢复版使用同一临时SDK根、同一完整XSD合法输入。28次实际handler调用的三缓冲区/CustomData相同；四份MOD产物和26份CustomData相同；恢复侧删除原AudioCompiler DLL。覆盖全部五个路由、main两份PCM8真实音频、AIFF16/WAVE24/extensible24和WAVE loop。此为专用音频MOD验收，非完整Upheaval全量编译。
- 收据：`evidence/audio/plugin-validation/40197878572-1/validation.json`、`pcm-validation`、`xas-validation`和`sdk-validation`同运行目录；SDK `calls-validation.log`明确28次调用，`Original-command.json`/`Recovered-command.json`保留命令及原DLL存在状态。

关键修复：CLR下x87实际为53位精度，上轮64位模拟在400/431Hz块大小差一帧；PCM24真实SIMEX导入路径先丢弃低8位转signed16，并非直接使用P2L0/P2B0 float decoder；AIFF SSND帧数使用原始chunk长度，排除padding。原始指令、1008条标量oracle及差分失败/成功锚点均保留。详细见 `audio/PcmInputAudit.md`、`audio/NativeAudit.md`、`audio/LoopsAndResampling.md`。

## 第2阶段 AIFF sustain loop 验收

源码 `78f819c1f061a4ba27d1d58145839517da1b72a9`，运行 [40198458731](https://github.com/wu162/NewSdk/actions/runs/40198458731) attempt1整体成功。Debug/Release各480条受影响format记录和200条新增AIFF loop记录相同，另16条AIFF reader安全检查通过；SDK专用4资产MOD原/恢复版4次实际调用、四产物和4份CustomData相同，恢复侧删除原目标DLL。未变PCM/XAS矩阵复用上节精确锚点，没有重复触发。

AIFF支持恰一个标准INST和一个MARK、两个唯一匹配marker、sustain playMode1、release playMode0的子集；end marker.position减1成为闭区间结束，允许MARK/INST和marker顺序颠倒。其余结构显式拒绝，不称完整AIFF元数据恢复。最新运行收据位于 `evidence/audio/plugin-validation/40198458731-1` 与 `sdk-validation/40198458731-1`。

## 独立 FFT 组件验收（不是重采样支持）

`src/AudioCompiler/Native/ResamplerFft.cs` 恢复原 `0x4eca0` 的order1..13、正/反方向子集；26对原x87 stage常量静态编入，不加载原目标DLL。源码 `69915484a322542ddf5a8041609b396e40c29a86`，运行[40199685628](https://github.com/wu162/NewSdk/actions/runs/40199685628) attempt1成功：C# Debug/Release各78条原机器码oracle的完整real/imag float bits相同，9项安全检查通过。收据 `evidence/audio/fft-validation/40199685628-1/validation.json` 明确 `runtimeResamplingValidated=false`、`sdkValidated=false`。未重新触发已验收音频/SDK矩阵。

该历史FFT阶段当时未接到AudioFile重采样路径。该阶段后续须完成系数生成、窗口/滤波调度、导数和插值及完整原插件差分，才可启用重采样；不得把FFT通过写成音频重采样已通过。实际Windows x86 CPU复核也已通过：源码 `eb68933006f54681f33b38baa196e9ff6273f938`，运行[40199843730](https://github.com/wu162/NewSdk/actions/runs/40199843730) attempt1，原DLL实际CPU与同一基准78组逐位一致，记录原CRT控制字53位最近舍入。该运行只复核原CPU，未重跑未变C#组件；C#验收继续绑定前述6991548/40199685628。

## 尚未恢复能力的原版样本

源码 `1c0457599f2a78082a283aade83a19e3f60a430e`，运行 [40199000936](https://github.com/wu162/NewSdk/actions/runs/40199000936) attempt1收取原版证据成功：112条重采样和8条Xenon XMA记录，XMA八条均实际产出buffer/CustomData，并非缺外部编码DLL导致必然失败。保存完整源WAV、原版三缓冲区/CustomData、专用脚本及精确native函数证据。收据状态是 `original-evidence-only`，不含candidate配置、不是恢复版验收；当时重采样/XMA仍明确拒绝，当前重采样状态见下节。

这批原版证据继续保留；现在只复用其输入和历史语义，不把它改写为候选验收。XMA完整编码器仍未恢复，见 `audio/XmaNextBatch.md`。

## 本轮重采样与 XAS 精度验收

- 独立重采样核：源码 `25aab8fe5a6b9e762293f9ace71c3edc59e36e36`，运行 [40207847717](https://github.com/wu162/NewSdk/actions/runs/40207847717) 成功。原Windows x86 CPU直接提取512个binary64窗口常数；Debug/Release各258组逐byte相同（48 source、16 filter、96 derivative、18 coefficient、80 whole-kernel）及5项安全拒绝。
- 插件/XAS/SDK最终联合批次：源码 `684e3310004853455dd0b5b4e53287afb067e93c`，运行 [40209451010](https://github.com/wu162/NewSdk/actions/runs/40209451010)。Debug/Release各1392条已定义插件记录逐byte一致；另8条单帧原未初始化导入保留原记录并要求候选明确拒绝。XAS block全264组及两项精度回归通过；真实SDK 38次调用、四产物及36份CustomData一致，恢复侧删除原AudioCompiler DLL。
- SDK是EALAModStudio case5 BAB基础数据流程的专用联合音频MOD，包含既有路由、真实PCM8、WAVE/AIFF/loop及新增六个重采样资产；不是完整Upheaval验收。
- XMA整数包结构独立组件：源码 `96f72d037b840ad09590688e8d4ec1f998446d8d`，运行 [40209977895](https://github.com/wu162/NewSdk/actions/runs/40209977895)。真实原CPU、Debug/Release各1025组逐byte相同，含8份真实EA XMA输出块（16个frame/scan case）与6项候选安全拒绝。此为packet结构恢复，不是编码器验收，representation28仍明确拒绝。

重要边界与根因：

1. Unicorn FCOS内部调用宿主cos，模拟器与真实CPU窗口195项binary64不同。最终源码使用原Windows x86 CPU常数，并以原CPU权威输出作为258组golden；不是按模拟器结果放松比较。
2. 原SIMEX `0x404e0` 对start0/count1成功返回但完全不写malloc样本。单帧旧内存不是可复现音频定义，不能照抄垃圾值。详见 [SingleFrameImportBoundary](audio/SingleFrameImportBoundary.md)。数值核对显式初始化的一帧输入仍有组件证明，不能与文件导入混淆。
3. XAS旧Release在一个量化阈值保留扩展临时精度，已按原SSE每步binary32修复。具体原32short、首个量化分叉、原/旧Release字节及264组回归见 [XasPrecision](audio/XasPrecision.md)。
4. 无新音频运行时依赖原DLL；原生产SDK/tools DLL/config仍不改。当前只在独立测试SDK副本启用恢复插件。


## 历史阶段证据（不扩大旧运行的范围）

- 原始 ILSpy 9.1.0.7988 全工程/IL/CLR 清单：`evidence/audio`，提取运行 `40192090985`，源码 `b957b1977fc1350d1ef366ef1a1727fab04e0b71`。
- 元数据：源码 `3ecb7ad3fed175d71675c9a527954364e04c7141`，运行 `40192722185`。Debug/Release 各 86,171 条完全一致；恢复宿主不含原 AudioCompiler DLL。记录保留原精确锚点，不将新实现冒充该次验证。
- PathMusic：源码 `ee7c278e7ede19675f5c9fabaa93166f53ae6959`，运行 `40193025288`，Debug/Release 各 95 条业务/解析记录一致。该同次运行 PCM/XAS 旧候选因 RAM 块结束标志失败，不把整体运行称成功；已保留原失败证据，修复后的编码器须重新验证。
- 编码组件：源码 `046734825aa84b8dbeda7343ecea3a66ba054342`，运行 `40193438615`：Debug/Release 各16条PCM和12条XAS HeaderData/CustomData完全一致；standalone工程Debug/Release构建成功。同次完整插件测试因legacy csc不支持NLayer部分现代语法而未执行；已改用通过构建的dotnet/Roslyn路径，修复尚待后续运行确认，不把整体运行称成功。
- 上轮检查点新增PCM8、完整插件矩阵、262组原native XAS样本及SDK脚本，当时尚未验收；本轮Windows/SDK成功结果见上文，独立Python/Unicorn核对不冒充C#或SDK验收。
- `main` 固定提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf` 的1289个WAV逐个解析均为PCM8/mono/22050Hz且无smpl；取脚步和环境音两个原文件到 `fixtures/audio/main`，具体源路径/blob/SHA256见provenance.json。未复制整套MOD。当时真实音频组件及SDK矩阵待运行，现已纳入第1阶段成功锚点。
- MP3公开NLayer后端、PCM8、AudioFile路由和400/431Hz边界在上轮尚未验收，本轮均按上文第1阶段矩阵取得完整差分。仍不扩大到特殊/损坏MPEG流的普遍等价。
- 此前真实SDK未验收；本轮两批SDK成功锚点独立列在上文，不能以组件成功替代。

## 公开实现审查

- NLayer：上游 `https://github.com/naudio/NLayer`，固定源码提交和本地等价兼容修改详见 `../src/AudioCompiler/ThirdParty/NLayer/PROVENANCE.md`，MIT 原许可证一并保留。
- vgmstream：`src/meta/ea_eaac.c`（SNR/SNS 容器）和相关 XAS decoder 可交叉核对格式；项目 COPYING 为保留声明的宽松许可。当前未复制该项目源码到产物。
- lgdel/eaxas：公开 XAS encoder 与 XasWriter 证明可参考的编码方向；检查时没有 LICENSE，未直接复制。恢复核心仍从本仓库原 DLL native 指令推导并通过原版输出对照。

## 当前下一步

重采样已完成本轮验收，不重复触发未变成功矩阵。XMA已恢复闭合273指令整数packet辅助组，但其数值编码核心仍未恢复；下一轮从 [XmaNextBatch](audio/XmaNextBatch.md) 的真实CPU原始codec输出收取及 `589d0 -> 59a60 -> 5d180 -> 659b8/6978f` 续接。AIFC、额外loop/marker及特殊/损坏MP3兼容边界仍未扩大。

公开实现已实际读取并交叉核对；minidx9 xma2defs、FFmpeg、vgmstream的精确blob与EA差异见 [PublicXmaReferences](audio/PublicXmaReferences.md)。没有把公开解析器当成原编码器的替代验收。

本轮未触发GameObject/XmlCompiler等无关矩阵；未变FFT和block-size标量复用旧锚点。XAS实际依赖改变后，重跑完整受影响插件组和SDK，而非沿用旧成功。没有账号额度计量，不能量化额度节省。生产SDK未替换，本分支未合入compiler-source。

最终文档提交前核对：SDK验收源码684e331与证据头7429062的整个`src`树及`SDK`树完全相同，后续只改测试工具/收据/文档。`src` tree=`1347625bc33cfa93286af3a119ab070372b4b55f`，`SDK` tree=`1f2d71a20ce8bf7d9343c7a2246c10275abe28f9`。完整compare接口仅返回前300文件，未用该截断列表证明“无源码变化”，而是直接比较根tree的子树SHA。汇总入口见`evidence/audio/acceptance-current.json`；各旧sourceCommit/runId继续原样保留。
