# AudioFile 显式采样率并行

基于 `recovery/legacy-sdk-cache` 的 `5c1843ae`，开发分支 `feature/audio-sample-rate-parallel`。当前代码已实现，Windows 统一验收待执行；禁止强推，合入须经 PR 和用户明确批准。

`PCSampleRate` 不再直接导致 owner 回退。资格仍要求精确内建 `PcmWaveAudioCompiler`，NONE/XAS 或原默认策略、Win32 稳定只读输入租约和私有 CDATA；EALAYER3 诊断、注入编码器和其它未认证类型保持原边界。原编码器、重采样数值顺序、拒绝行为、类型/版本哈希、缓存身份均不变。

重采样的 profile、系数、FFT、导数、源/目标样本及循环点都是每次调用私有数据。共享数据只有只读表；压缩警告路径仍被资格门槛排除，没有新增 ProcessInstance 全局锁。

准备阶段只通过已有租约读取 WAV `fmt ` / AIFF、AIFC `COMM` 的源采样率，并跳过样本数据，不复制/解码/哈希整个输入。扫描与 importer 一致地处理字节序、奇数 chunk padding 和最后一个格式 chunk。显式目标值与编码器共用原 C 整数解析，含十六/八进制及截断语义。沿用输入字节 ×64 的 importer/encoder 预算，实际转换再按 `ceil(target/source)` 的至少一倍计入展开并增加 1 MiB FFT 工作空间；不变采样率不追加。不能读取格式或预算不足走原 owner 路径，租约释放；无显式采样率及 MP3 的预算不变。

统一验证准备：

- 生产模块宿主：32 个 WAV/AIFF 用例，显式同率/升降采样和十六进制目标，1/2/4/6 声道、PCM8/16/24、NONE/XAS、RAM/streamed、WAV/AIFF 循环，逐一对比 owner/worker 三缓冲及 CDATA，核验真实线程、重叠及 DOM 不变。
- 资源边界：两种容器的 400→96000，16 MiB 拒绝、128 MiB 接纳，拒绝后可重新写入源文件；同率 400 在 1 MiB 内接纳。
- 既有64生产缓存/jobs用例加入4个显式采样率 WAV/AIFF，冷态要求8个 AudioFile 全在 worker，冷热切换、单资产编辑、失败/部分保存/重试与独立串行控制继续逐字节比较。
- 原版 AudioFile 既有 rates/formats/loops/aiffloops/resample-full/multichannel/multichannel-aiff 完整组差分和既有拒绝检查，Debug/Release 重建。
- 原有完整 Core 生命周期、mymod 原 SDK 四流及 BIG/SKUDEF、五文件包和完整 Upheaval 串并行验收保持。

Linux 只能完成轻量预检，不能替代 Win32 混合模块、原 DLL 和 SDK 验收。验证终态及来源随后登记。
