# RA3 EALayer3 V1

实现分支：`feature/ra3-ealayer3`，基线 `recovery/legacy-sdk-cache` / `eb503513`。
当前为实现后的验证阶段；下表是实现范围，正式通过范围以本文末尾的 Windows 收据为准。

## 编码与来源

`AudioFile PCCompression="EALAYER3"` 现在选择 SIMEX representation 31，输出 SNR
codec **5 / EL31 / EALayer3 V1b**，不再输出 PCM 降级警告或 PCM 替代数据。
V2 PCM/spike、TW 的旧 EALayer3、Xbox/XMA 不在实现中。

选取 sagetools `49373be5e922e8d7ae0b2caa2c74fa6c64d6cf4c` 的
`mp3toea`、LAME 3.96 和起始混合所需 V1 解码器依赖闭包；编码调度按
`encealayer31.cpp` 的算法接入。包含 MPEG reservoir 展开、每 576 帧 granule、
声道对交错、1105 帧延迟补偿、47 帧原始 PCM/解码结果 crossfade、末尾补齐。
仅复用必要音频源码，没有引入 SIMEX、外部转换程序或第三方工程/工具链。

参考与验证：

- RA3 插件 `jonwil/ealayer3compilerplugin` / `ed407302ae63cd6ba849af586ef765c03ca008b3`：
  codec31、质量默认 75/范围 0–100、RAM/streamed 的 CustomData 及运行时元数据合同。
- `jonwil/snrtool` / `34308631b0bf94a57d1d254b651ee191346377ab`：
  representation31 → SNR5、流式块、下一 64 字节边界填充和循环段。
- `jonwil/lame-3.96` / `ed9d6f623109775b89159ed506c20afb1e8f20b4`：核对 LAME 来源。
- vgmstream r2168：独立解码 EL31b。Windows ZIP SHA256
  `692d45070852a35c1fa42726aa3c9de528e44eec925d37d99b1074256b99a229`。

许可证/版权保留在第三方源码及 `Native/Audio/EaLayer3/NOTICE.md`、COPYING 文件。
按用户要求，不嵌入 EXE/DLL，也不向运行时部署包增加协议文件。
GPL/LGPL 的源码分发条件仍适用，分发二进制时应标明匹配的公开源码版本。

## 接入与边界

沿用 `PcmWaveAudioCompiler`、现有 WAVE/AIFF 读取、filter120 重采样、Loop 读取、
`PcmEncodingResult`、`AssetBuffer`、字幕及 CustomData 输出；没有平行资产管线。

- 输入沿用 integral WAVE/EXTENSIBLE、AIFF/AIFC NONE 的既有解析能力。编码器接收
  1/2/4/6 声道浮点 PCM，4/6 声道以两个/三个独立 stereo 编码器交错。
  WAVE4/6 原有重排仅做一次，AIFF 保持原序。
- 采样率：16000、22050、24000、32000、44100、48000 Hz。`PCSampleRate` 先走
  既有重采样。其它输出率明确拒绝，不自动改率或降级 PCM。
  所选上游 V1 启动解码器的 MPEG2.5 `sfreq` 分支没有初始化，未将
  8000/11025/12000 标为支持；更高/任意非 MPEG 采样率亦不标为支持。
- `PCQuality` 按 RA3 插件的十进制/SageInt 解析，默认 75，范围 0–100；映射
  到 LAME `VBR_q=int((100-quality)*9/100)`，VBR mtrh、算法 quality2、joint stereo。
- RAM：SNR 头及帧块放 CustomData。streamed：SNR 放运行时 HeaderData、SNS
  放 CustomData，独立段的结束标记和下一 64 字节边界填充包含在块尺寸中。
  RAM 单段必须符合 SNR 24 位块长度；超限明确提示使用 streamed。
- Loop：保持既有 WAVE inclusive、AIFF end-marker exclusive 的转换；在重采样后
  分别编码 intro/loop、各自补偿延迟，streamed SNR 写入真实 loop byte offset。
  SNR 在 loopEnd+1 结束，运行时 NumberOfSamples 保持原输入/重采样后的总帧数。

每个任务/段独立持有 LAME、reservoir、granule 队列及启动解码器。固定 LAME 表只
初始化一次；上游合成 scratch 改为调用内数组，Huffman count descriptors 改为实例字段。
没有 AudioFile 全局锁、外部进程、独立音频缓存或输入 SHA256。

正常 `BuildCompiler.ps1` 将选定原生源码直接链接进 EXE，仍为原五文件运行时。
独立 AudioCompiler csproj 自动构建、复制并 publish `NewSdk.EaLayer3.dll`；机器需
正常 Windows MSVC x86 构建工具，不要求用户另装编码器。

缓存：仅 AudioCatalog 插件版本 548405252→548405253、AudioFile ProcessingHash
2414318214→2414318215。原 TypeHash/SessionCache18/DocumentProcessor11 不改。
旧 AudioFile 缓存（包括旧 EALAYER3→PCM）通过现有 processing hash 比较失效；
这会使旧 NONE/XAS AudioFile 也首次重编译。其它格式的编码算法和数据合同不改。

## 验证

入口 `.github/workflows/audio-ealayer3.yml`：先 Debug/Release 真实插件调用、jobs1/4
完整缓冲区及 CustomData 比较、vgmstream 解码/波形/声道顺序/长度/Loop，再运行
jonwil RA3 插件格式/解码信号对照、既有 NONE/XAS/MP3/PathMusic 原字节回归。
专项通过后才运行正式 BAB 加载新 DLL、正常产品 D/R 构建、生产 CLI 缓存/并行/音频
修改/独立重编译，最后集中执行一次现有完整 mymod SDK 验收。

当前 Windows 验证仍在进行。早期失败保留于 Actions：输出路径引号、测试宿主路径/
CLR 配置、上游静态 synthesis scratch 导致的并行差异，以及 AIFF 测试 end marker
误用 inclusive 值。没有降低字节相等断言，没有把仅能解码标为游戏内播放通过。
游戏内运行未执行。
