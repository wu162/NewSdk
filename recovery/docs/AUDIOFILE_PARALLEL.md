# AudioFile 并行与预算

基线：`recovery/legacy-sdk-cache` / `b89fbadb`。实现分支：`perf/audiofile-parallel`。

内建恢复版 AudioFile 的并行资格不再受 `PCSampleRate` 和 `PCCompression` 属性限制。默认压缩、NONE、XAS、EALAYER3，以及显式原采样率和真正重采样均可进入 worker。编码器中的不支持输入/参数仍按原路径报错；未知或注入的插件不获得内建实现的资格。

AudioFile 外部文件的调度预算由 `文件长度 × 64 + XML 估算` 改为 `文件长度 × 4 + XML 估算`，文件部分降低 93.75%，与 MP3 passthrough 的现有倍率一致。2 MiB 输入约计 8 MiB，原来约 128 MiB。这个值是调度启发式估算，尤其高倍重采样的实际峰值可能更高；它不代表实测内存或吞吐提升。jobs、CPU、Win32 内存观测、单任务及总在途预算继续控制并发量。

采样转换的 scratch/输出均属于单次调用，系数表只读。EALAYER3 的进程级压缩提示改用原子首次标记，多个 worker 只输出一条完整提示。CDATA 私有路径、输入只读租约、有序验证提交和失败/取消 join 不变。

验证沿用 Core 生命周期完整工作流。组件波扩充为八个音频资产，比较串行与 worker 的三缓冲和 CDATA，覆盖四种压缩设置、升/降采样、streamed/RAM，并检查并发首次压缩提示仅一次。资格用例验证显式同采样率以及新预算的允许/拒绝边界。生产混合夹具增加同样八种设置，要求所有 AudioFile 均在 worker 执行，并继续比较 jobs 1/4/auto、冷热缓存、修改、失败重试的原字节输出。

本地 Python 语法、夹具生成及 SDK aggregate XSD 已通过。Windows 新构建与完整生产回归待执行；旧结果不作为本次验收。
