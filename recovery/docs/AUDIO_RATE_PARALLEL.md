# AudioFile 显式采样率并行

基于 `recovery/legacy-sdk-cache` 的 `5c1843ae`，开发分支 `feature/audio-sample-rate-parallel` / [PR #25](https://github.com/wu162/NewSdk/pull/25)。功能及 Windows 统一验收已通过，PR 待审；禁止强推，合入须经用户明确批准。

`PCSampleRate` 不再直接导致 owner 回退。资格仍要求精确内建 `PcmWaveAudioCompiler`，NONE/XAS 或原默认策略、Win32 稳定只读输入租约和私有 CDATA；EALAYER3 诊断、注入编码器和其它未认证类型保持原边界。原编码器、重采样数值顺序、拒绝行为、类型/版本哈希、缓存身份均不变。

重采样的 profile、系数、FFT、导数、源/目标样本及循环点都是每次调用私有数据。共享数据只有只读表；压缩警告路径仍被资格门槛排除，没有新增 ProcessInstance 全局锁。

准备阶段只通过已有租约读取 WAV `fmt ` / AIFF、AIFC `COMM` 的源采样率，并跳过样本数据，不复制/解码/哈希整个输入。扫描与 importer 一致地处理字节序、奇数 chunk padding 和最后一个格式 chunk。显式目标值与编码器共用原 C 整数解析，含十六/八进制及截断语义。沿用输入字节 ×64 的 importer/encoder 预算，实际转换再按 `ceil(target/source)` 的至少一倍计入展开并增加 1 MiB FFT 工作空间；不变采样率不追加。不能读取格式或预算不足走原 owner 路径，租约释放；无显式采样率及 MP3 的预算不变。

统一验证范围：

- 生产模块宿主：32 个 WAV/AIFF 用例，显式同率/升降采样和十六进制目标，1/2/4/6 声道、PCM8/16/24、NONE/XAS、RAM/streamed、WAV/AIFF 循环，逐一对比 owner/worker 三缓冲及 CDATA，核验真实线程、重叠及 DOM 不变。
- 资源边界：两种容器的 400→96000，16 MiB 拒绝、128 MiB 接纳，拒绝后可重新写入源文件；同率 400 在 1 MiB 内接纳。
- 既有64生产缓存/jobs用例加入4个显式采样率 WAV/AIFF，冷态要求8个 AudioFile 全在 worker，冷热切换、单资产编辑、失败/部分保存/重试与独立串行控制继续逐字节比较。
- 原版 AudioFile 既有 rates/formats/loops/aiffloops/resample-full/multichannel/multichannel-aiff 完整组差分和既有拒绝检查，Debug/Release 重建。
- 原有完整 Core 生命周期、mymod 原 SDK 四流及 BIG/SKUDEF、五文件包和完整 Upheaval 串并行验收保持。

Linux fixture、原SDK聚合XSD、YAML及差异预检通过；正式结果来自下面的新 Win32 运行。

## 验证结果（2026-10-30）

源码/二进制/测试来源 `8faba4e9f7efe470b95fdc7f2be43ed069e3a670`，[运行41028084011](https://github.com/wu162/NewSdk/actions/runs/41028084011) 整体 **success**，所有适用步骤success；历史CLI二进制复用步骤按配置skipped，实际Debug/Release均重新构建。

- 每配置32个新增WAV/AIFF worker用例的三缓冲及CDATA逐字节一致，8组均实测峰值4；DOM不变、私有输出路径、输入租约和400→96000预算边界通过。
- 每配置原版音频完整差分10106条：rates2、formats1200、loops216、aiffloops200、resample-full424、multichannel1728、multichannel-aiff6336；另外8条原单帧未定义导入仍明确拒绝，未放宽原检查。
- 64个生产缓存/jobs用例全部通过，包括4个新增显式采样率输入、冷态8个AudioFile全worker、热态零准备/编译、单资产修改、失败/部分保存/重试。
- Core生命周期、真实CLI/冷进程/CtrlBreak、Texture、原BAB输入根、构建/会话缓存及Audio/W3X重放、BIG/APT/maps/Additional、完整mymod原SDK四流及BIG/SKUDEF、独立五文件包全部通过。
- 完整未精简Upheaval的22个jobs1/4/auto冷/热/修改样本全部0错误，发布输出与独立串行控制逐字节相同。它不量化本次新增资格的收益，原因见下文。

[原始artifact12966923428](https://github.com/wu162/NewSdk/actions/runs/41028084011/artifacts/12966923428) 已实际下载：250607352字节，SHA256 `bd9981b61f677b122e6956f2d3da8a7e8eea99476ebf6c9ee9888ad01aa9cc02`，与GitHub digest匹配。Debug/Release两份ZIP内各五个文件已逐一复算，与发布收据及实际生产bin相同。[摘要、API终态和选定原收据](../evidence/refactor/audio-rate-parallel/41028084011-1) 已归档；后续仅文档/证据更新，没有重编译。未新增游戏内验收。

完整 Upheaval 历史输入 `5551fc26a6f96b62613443e0d0fa2958c14cbce0` 的14份音频文档共1289个 AudioFile，均没有显式 PCSampleRate。其全MOD测量属于整体回归，不能用来量化本次新增显式采样率并行的收益。新资格由专项worker与生产缓存矩阵证明。
