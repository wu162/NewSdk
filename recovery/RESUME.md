# AudioCompiler 当前交接

更新：2026-10-26（北京时间）。目标分支 `recovery/audio-compiler-source`；产品源码 `src/AudioCompiler`。先读 AGENTS.md、EFFICIENCY.md；禁止强推。

## 当前目标：PC / Win32

用户已明确不需要 Xbox 平台。Xbox/XMA 专用恢复源码、测试、工作流及恢复规划已移除；完整 XMA 编码不再是项目待办或音频完成门槛。历史提交和运行中的其他平台结果只作为历史证据，不扩大当前支持范围。

当前 PC 能力：PathMusic、MP3 passthrough、AudioFile；PCM8/16/24 WAVE/AIFF、WAVE EXTENSIBLE 整数容器、AIFC/NONE、EA raw ADP4、NONE/PCM16 与 XAS v1 输出、filter120 重采样、已界定的 forward loop。SSND blockSize 任意 uint32 被忽略，非零 offset 仍拒绝。

ADP4/SSND 已过真实插件 ProcessInstance 三缓冲区/CustomData 差分，**SDK 留待里程碑合批**。已有 EXTENSIBLE 与 AIFC/NONE 两个各120资产专用 SDK 批次通过；它们不是完整 Upheaval 验收，不能覆盖后续新增输入路径。

## 下一轮

1. 先读 [AUDIO_COMPILER](AUDIO_COMPILER.md) 的 PC 剩余边界及 [输入元数据](audio/InputMetadataBoundary.md)，按完整能力组决定是否扩展循环/元数据接受范围；原版不支持及未初始化内存行为不列为必补功能。
2. 将 ADP4、SSND 等已积累输入能力合成一次最小合法 PC 音频 MOD，做 SDK 原/恢复版对照。不要重复运行未变且已有有效证据的组件。
3. PC 范围满足后处理 `compiler-source` 合并、最终 exe 集成及完整 Upheaval 里程碑验收；目前生产 SDK DLL/config 未替换，最终 exe 未接入。

本次删除后验证已通过：源码 `4f2c03c5f7510e71655987189a6b9ecd4a636d80` / [40240685038](https://github.com/wu162/NewSdk/actions/runs/40240685038)，产品源码树 `38a94a66e7cca464c465c5947fc428315d217ec9`。每种 Debug/Release：元数据73,861项、保留路由2,899项及8项单帧安全拒绝、AIFC1,080项、ADP4 3,180项、SSND2,436项、Xbox排除检查8项；PCM/XAS组件及整工程构建通过。SDK运行0次。保留路由矩阵包含现有PC/PS3及适用unknown检查；Xbox已排除。

精确收据见[验收索引](evidence/audio/acceptance-current.json)；PC剩余边界及后续集成见[AUDIO_COMPILER](AUDIO_COMPILER.md)。历史SDK成功仍只对应原提交，不代表本次清理后的SDK验收。

**用户要求：不要频繁触发完整 SDK 真实验证。** 组件/插件差分优先，实际依赖变化组完整验证，SDK 按更大里程碑显式合批。

其他子系统历史按[整理前导航](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/RESUME.md)按需读取，不将旧整树 SDK 证明套到当前源码。
