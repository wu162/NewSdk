# AudioCompiler 当前交接

2026-10-26（北京时间）；目标 `recovery/audio-compiler-source`；仅 PC / Win32。先读 AGENTS.md、EFFICIENCY.md、[AUDIO_COMPILER.md](AUDIO_COMPILER.md)。禁止强推，非PC不再恢复。

已验收 `bb4a225` / [40243817293](https://github.com/wu162/NewSdk/actions/runs/40243817293)：循环元数据、有界SSND offset、原MPEG header/info完整组及旧受影响回归D/R通过；4488资产专用SDK的三缓冲区、四产物、CustomData一致，恢复侧无原AudioCompiler DLL。源码已移除透传流程不需要的NLayer解码限制/源码。

最终候选 `2d64684742f9b19808d8fd721fbe31a549907c37` / [40244825458](https://github.com/wu162/NewSdk/actions/runs/40244825458) **验证中**：COMM signed声明范围过滤7776记录、MPEG1084记录及72容器oracle，旧受影响组完整复验；全成功才跑新增1820资产SDK。不把旧4488收据改为本次新运行。

接下来：收取该终态并更新[索引](evidence/audio/acceptance-current.json)；通过后启动一次完整Upheaval音频替换门槛（`audio-upheaval-validation.yml` / `TestAudioUpheavalSdk.ps1`），然后合入compiler-source。最后exe工程尚未建立，不冒充exe集成完成。SDK/tools原DLL/config仍为基线。

详细支持范围、安全拒绝、固定输入及精确证据只维护在AUDIO_COMPILER和其分组链接。完整Upheaval全WAV，不能替代COMM/AIFF SDK。SDK继续里程碑合批，不重复未变成功范围。
