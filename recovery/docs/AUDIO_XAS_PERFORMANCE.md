# XAS 数组区间编码

基线：`45d485d829170dc4fda3b3ce63b6af2759fc7ba0`（`recovery/legacy-sdk-cache`）。实现分支：`perf/audio-xas-ranges`。本批只修改 Audio 的三个编码器；没有修改其它插件或执行完整 SDK 验证。

- XAS 按输入帧偏移推进，删除 pending、GetRange/RemoveRange 和完整块 PCM 数组。
- 预先计算所有 76×声道块、8 字节分块头和末尾填充，一次分配最终 CustomData。streamed 原来对齐时仍追加 64 字节的规则保留。
- 128 帧块直接写入输出偏移。每次编码只用一个 float[30] 工作区，每组 30 个源样本只做一次原样 F() 转换，四预测器共用；原 F() 精度边界和量化运算顺序保留。
- 四组头和交错 nibble 直接写入最终位置，删除 byte[19]、byte[][] 和块输出数组。只有每段末尾不足 128 帧时分配填充块。
- Loop 直接读取原数组的 Intro/Loop 区间，各段分别结束、填充和计算 streamed 偏移，丢弃 inclusive loopEnd 之后的源样本但保留原 runtime 帧数。
- 共享 PCM 写入器提供相同区间接口，消除 Loop 的非 XAS 整段复制；普通 PCM 复用这条写入路径，格式不变。

## 验证方法

集中复用 TestAudioPlugin（pcm/xas/loops/aiffloops/multichannel/multichannel-aiff/resample-full）、TestAudioPcm 两种 codec 和 TestAudioXasPrecision。Windows .NET Framework Debug/Release x86 执行原 DLL 差分，不替换原版预期、不放宽断言。新增工作流不包含 SDK 步骤。

性能程序从上述固定提交提取三个基线源码，与候选在同一进程、同一运行时、相同输入下比较。先检查 3280 个 PCM/XAS、1/2/4/6 声道、rate400/431/22050/48000/96000、128 帧和 streamed 分块边界、Loop 区间的完整输出及 metadata；再测 30 秒/120 秒 RAM、120 秒 streamed 和 Loop、600 秒 streamed、30 秒六声道。48kHz、确定性 signed16 随机信号，非整块末尾；Loop 使用非整块 Intro 并舍弃尾部 7 帧。每项三轮交替先后次序，每轮检查完整 HeaderData/CustomData。

Windows 计量使用 net472 x86 Release 和 AppDomain.MonitoringTotalAllocatedMemorySize，记录 Stopwatch 耗时与三代 GC 次数。输入生成、字节比较和每轮测量前主动 GC 不计入编码区间；AppDomain 分配计数包含区间内运行时分配。单线程、workstation GC、禁用 tiered compilation。原始每轮数据归档为 CSV/JSON，不以 GC 后存活内存代替累计分配量。

## 当前状态

实现已完成；本地 .NET 8 Linux 预检通过 3280 个前后边界比较、现有 262 个原机器码 XAS 块差分。Windows 原版差分和同机 CLR x86 长音频计量待集中运行；在成功前不宣称整批验收完成。Linux 预检不能替代 Windows x86 浮点验收。
