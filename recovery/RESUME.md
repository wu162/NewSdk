# AudioCompiler 续接入口

更新：2026-10-26（北京时间）。本分支 `recovery/audio-compiler-source`；源码位于根目录 `src/AudioCompiler`。先读 AGENTS.md、EFFICIENCY.md；禁止强推。

## 当前结论

- 五个插件路由、PCM/XAS、受限 WAVE/AIFF loop、filter120 重采样已取得组件及专用音频 MOD 的真实 SDK 验收。
- **尚未完整恢复 AudioCompiler：XMA 仅完成整数 packet 辅助组件，编码核心仍未恢复，representation28 明确拒绝。**
- AIFC、额外 loop/marker、非标准 WAVE EXTENSIBLE 元数据和特殊/损坏 MP3 的兼容边界尚未补齐。
- 生产 SDK DLL/config 未替换，未合入 compiler-source，未完成完整 Upheaval 验收。

## 下一步与证据

1. 阅读 [AUDIO_COMPILER.md](AUDIO_COMPILER.md)：当前支持范围、剩余工作和精确验收锚点。
2. 按 [XmaNextBatch.md](audio/XmaNextBatch.md) 收取真实 CPU 的原始 codec 输出，再追踪数值核心；整数 packet 组已完成，不再作为下一轮待实现项。
3. 验收入口：[acceptance-current.json](evidence/audio/acceptance-current.json)。SDK 源码 `684e3310004853455dd0b5b4e53287afb067e93c` / 运行 [40209451010](https://github.com/wu162/NewSdk/actions/runs/40209451010)：Debug/Release 各1392条插件记录，38次真实 SDK 调用、四产物及36份 CustomData 一致，恢复侧无原 AudioCompiler DLL。

只验证实际受影响组；未变组复用原 sourceCommit/runId。文档整理不冒充新运行，安全拒绝不算原版业务等价。单帧重采样拒绝源于原导入器未初始化内存，见 [边界说明](audio/SingleFrameImportBoundary.md)。

其他子系统历史交接保留在[整理前 RESUME](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/RESUME.md)，按需读取；不将其中 compiler-source 的阶段完成声明套用到音频。
