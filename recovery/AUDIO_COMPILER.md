# AudioCompiler 源码恢复（阶段交接，未完整验收）

目标：从 `recovery/compiler-source` 的 `b122a54abe4405516b021e425b8fb9235d914a0e` 新建 `recovery/audio-compiler-source`。
原 DLL SHA256：`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`。
代码按本轮用户要求放在仓库根目录 `src/AudioCompiler`；原版 SDK/tools 二进制与配置不修改。

## 实现范围

- `Plugin` 实现 BAB `IAssetBuilderPlugin` 五个路由，保留原命名空间、程序集版本和类型/处理元数据。生产接入只需接口，不承诺原 C++ 指针辅助方法的二进制 ABI。
- `PathMusicMap`：读取 MPF 字节并生成 runtime、重定位。
- `PathMusicTrack`：复制 MUS 到 CustomData，解析 PATH_TRACK_ header handle，生成 runtime 与字符串分配。
- `PathMusicEvent`：解析 PATH_EVENT_ header、weak reference、IsXferable，生成 runtime。
- `AudioFileMP3Passthrough`：恢复原生七连续 MPEG 帧识别和头解析；MIT NLayer 源码作为实际 MPEG 解码验证后端。文件原样传递，生成 subtitle/channels/streamed runtime。第三方代码静态编入目标程序集，不使用独立 NLayer DLL。
- `AudioFile`：默认 `PcmWaveAudioCompiler` 后端支持单/双声道 PCM8/16 RIFF/WAVE、NONE 和 XAS v1、RAM/streamed SNR/SNS 容器；恢复平台策略、默认 subtitle、runtime 布局、重定位和 CustomData。
- `Pcm16Encoder` 与 `Xas1Encoder` 从原 DLL 算法独立重建，不读取/加载原目标 DLL 作为运行后端。

明确未覆盖：AIFF、PCM8/16 之外的 WAV sample format、loop 元数据、重采样、XMA。上述输入显式拒绝，不静默使用不同编码格式。MP3 采用公开解码后端，损坏/特殊 MPEG 流的接受行为不保证普遍等同原 RenderWare decoder；必须按实测样本描述兼容性，不能称原 native decoder 全量还原。

## 构建

`dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release`。
工程目标 net472（为公开 MPEG 后端提供兼容运行时），无签名，Core 引用保留原 SDK 基线。代码可直接纳入将来的单 exe；此独立工程用于阶段验证，不代表已修改生产 SDK。

## 已有证据

- 原始 ILSpy 9.1.0.7988 全工程/IL/CLR 清单：`evidence/audio`，提取运行 `40192090985`，源码 `b957b1977fc1350d1ef366ef1a1727fab04e0b71`。
- 元数据：源码 `3ecb7ad3fed175d71675c9a527954364e04c7141`，运行 `40192722185`。Debug/Release 各 86,171 条完全一致；恢复宿主不含原 AudioCompiler DLL。记录保留原精确锚点，不将新实现冒充该次验证。
- PathMusic：源码 `ee7c278e7ede19675f5c9fabaa93166f53ae6959`，运行 `40193025288`，Debug/Release 各 95 条业务/解析记录一致。该同次运行 PCM/XAS 旧候选因 RAM 块结束标志失败，不把整体运行称成功；已保留原失败证据，修复后的编码器须重新验证。
- 编码组件：源码 `046734825aa84b8dbeda7343ecea3a66ba054342`，运行 `40193438615`：Debug/Release 各16条PCM和12条XAS HeaderData/CustomData完全一致；standalone工程Debug/Release构建成功。同次完整插件测试因legacy csc不支持NLayer部分现代语法而未执行；已改用通过构建的dotnet/Roslyn路径，修复尚待后续运行确认，不把整体运行称成功。
- 当前检查点新增PCM8、完整插件7组业务矩阵、262组原native指令XAS样本（涵盖全部4预测器和shift0..12）及SDK对照脚本，**新增内容尚未取得最终Windows成功收据**。独立Python/Unicorn核对不冒充C#或SDK验收。
- `main` 固定提交 `bacfc69fc3e71c8116d1f22b933b8b6173170ecf` 的1289个WAV逐个解析均为PCM8/mono/22050Hz且无smpl；取脚步和环境音两个原文件到 `fixtures/audio/main`，具体源路径/blob/SHA256见provenance.json。未复制整套MOD。真实音频组件及SDK矩阵待运行。
- MP3公开NLayer替代后端、PCM8输入、AudioFile完整路由、400/431Hz块大小边界均仍待最终差分。不能把先前仅PCM16静态编码器的成功结果扩大到这些新增能力。
- 真实 SDK 集成尚未验收，不能以组件成功替代。

## 公开实现审查

- NLayer：上游 `https://github.com/naudio/NLayer`，固定源码提交和本地等价兼容修改详见 `../src/AudioCompiler/ThirdParty/NLayer/PROVENANCE.md`，MIT 原许可证一并保留。
- vgmstream：`src/meta/ea_eaac.c`（SNR/SNS 容器）和相关 XAS decoder 可交叉核对格式；项目 COPYING 为保留声明的宽松许可。当前未复制该项目源码到产物。
- lgdel/eaxas：公开 XAS encoder 与 XasWriter 证明可参考的编码方向；检查时没有 LICENSE，未直接复制。恢复核心仍从本仓库原 DLL native 指令推导并通过原版输出对照。

## 后续

本轮达到1小时时限停止新增实现。下一轮先运行最新检查点的Windows完整差分；定位真实差异后重跑受影响组。真实 SDK 使用同一临时根顺序对照，避免 PathMusic 绝对路径产生假差异。完成后更新本文件与 RESUME，保留未支持格式和公开后端边界。
