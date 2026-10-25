# AudioCompiler 续接入口

更新：2026-10-26（北京时间）。目标 `recovery/audio-compiler-source`，源码根目录 `src/AudioCompiler`。先读 AGENTS.md、EFFICIENCY.md；禁止强推。

## 已验证状态

- 五路由、PCM/XAS、受限 WAVE/AIFF loop、filter120 重采样沿用原精确验收。
- **新增 WAVE EXTENSIBLE 忽略扩展元数据**：`5d56d52ff11510c7e93f83b47d08b1def5c99d27` / [40232917060](https://github.com/wu162/NewSdk/actions/runs/40232917060)，Debug/Release 各2400插件记录，专用 SDK120次调用、四产物/120份 CustomData 一致。
- **新增 AIFC/NONE**：`3863a632faac79ef586569e134b7eb46014fab94` / [40233228823](https://github.com/wu162/NewSdk/actions/runs/40233228823)，各1440插件记录、60样本位检查/10拒绝，旧2600记录回归及专用 SDK120次调用通过。
- **XMA 仍不能编码**：整数 packet 组之外，PCM预转换/输出重写/PRNG-noise 已验收于40232917060；六个 integral 加窗/复制叶函数各6888组已验收于 `579c95661d90f067ab414c4133f033c2571b96e1` / [40233712043](https://github.com/wu162/NewSdk/actions/runs/40233712043)。representation28仍拒绝。
- 同运行40233712043：纯原生跳板111次实际调用确认 `97d97 -> 98267`，原始三返回输出不变；长/真实音频8案例各重复2次，121个packet、4个多block案例；ADP4独立解码各18243组及原插件96条双进程取证通过，**文件入口尚未接入**。

## 当前工作与下一步

XMA EA/WAVE/seek元数据组正在以独立原CPU+Debug/Release矩阵验收；98267 integral加窗调度及真实参数/源数据捕获正在推进。以对应最新运行收据为准，未有成功收据不得宣称完成。细节见 [AUDIO_COMPILER](AUDIO_COMPILER.md)、[XmaNextBatch](audio/XmaNextBatch.md)、[XmaPcmWindow](audio/XmaPcmWindow.md)。

后续先完成调度真实输入回放，再进入实际transform/量化/bitstream核心。旧外围managed-VEH探针40232992739失败，不能用作已验证取证工具；新的纯原生journal只证明97d97路径，不冒充外围3个site全覆盖。

**用户最新要求：不要频繁触发完整SDK真实验证。** 后续优先组件/插件差分及定向原版取证，SDK按更大里程碑合批、显式启用；当前工作流默认components不运行SDK。上述两个SDK是最小专用音频MOD，不是完整Upheaval。未替换生产SDK DLL/config，未合入compiler-source。

[验收索引](evidence/audio/acceptance-current.json)尚保留上一阶段精确锚点，本轮新增收据见上述运行目录，收尾时统一更新；不得把历史sourceCommit/runId改成本轮文档提交。其他子系统按[整理前导航](https://github.com/wu162/NewSdk/blob/36880397ce00f18690edf21e78bc68b39736eac4/recovery/RESUME.md)按需读取。
