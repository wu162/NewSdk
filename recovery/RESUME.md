# AudioCompiler 当前交接

2026-10-26（北京时间）；目标 `recovery/audio-compiler-source`；仅 PC / Win32。先读 AGENTS.md、EFFICIENCY.md、[AUDIO_COMPILER.md](AUDIO_COMPILER.md)。禁止强推，非PC不再恢复。

## 本轮已完成并通过

最终源码 `2d64684742f9b19808d8fd721fbe31a549907c37`，源码树 `ee6074a096795aa66b682fcb363e360feabe4d2f`，运行 [40244825458](https://github.com/wu162/NewSdk/actions/runs/40244825458) attempt1 **success**：D/R loop4224、optional3888、COMM7776、offset2704、MPEG1084及旧受影响回归/整工程构建通过；新增1820资产SDK的实际三缓冲区、四产物及CustomData一致，恢复侧删除原AudioCompiler DLL。

前一完整批 `bb4a225` / [40243817293](https://github.com/wu162/NewSdk/actions/runs/40243817293) success：4488资产SDK已通过。其精确旧source/run保持不变；新COMM边界和offset组合由1820新批覆盖，未重复旧4488。详见[覆盖审计](audio/CommSdkCoverageAudit.md)和[验收索引](evidence/audio/acceptance-current.json)。

修复循环元数据、signed COMM过滤、SSND有界offset、WAVE无padding lookup、MPEG EOF/真实容器识别。原MP3透传SIMEX_info只查头信息，不解PCM；错误增加的NLayer解码门槛和19个未引用文件已删除。MARK-only原未初始化mode、单端匹配和越界输入继续明确拒绝。

## 下一轮

用户要求先交接，本轮不再启动新实现/新验证。无需重跑上述成功组。

1. 启动一次完整固定Upheaval音频替换门槛：`audio-upheaval-validation.yml`（提交标记`[audio-input-upheaval]`，监听该工作流/专属脚本变更）；`TestAudioUpheavalSdk.ps1`。工作流已核对当前源码树与1820 SDK收据后才允许运行。保留194个.asset输入，核对实际AudioFile调用。全部Upheaval为WAV，不能代替已独立通过的AIFF SDK。
2. 通过后合入`compiler-source`。合并时保留其各子系统导航，不能简单把Audio专用RESUME覆盖通用导航；SDK及shared recovery/src/Native/Core/W3X树与目标b122a54完全相同，不重跑未变子系统。
3. 最终exe集成尚未完成，属于后续重构；已查`refactor/compiler-exe-integration`的46b8785仅有联合SDK基线准备，尚无最终exe工程。

完整Upheaval未启动；compiler-source未合入；SDK/tools原DLL/config未替换；不冒充最终exe集成。详细当前范围、拒绝边界和分组证据只维护在AUDIO_COMPILER及其链接。
