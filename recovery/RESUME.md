# AudioCompiler 当前交接

2026-10-26（北京时间）；目标 `recovery/audio-compiler-source`；仅 PC / Win32。先读 AGENTS.md、EFFICIENCY.md、[AUDIO_COMPILER.md](AUDIO_COMPILER.md)。禁止强推，非PC不再恢复。

## 本轮已完成并通过

最终源码 `2d64684742f9b19808d8fd721fbe31a549907c37`，源码树 `ee6074a096795aa66b682fcb363e360feabe4d2f`，运行 [40244825458](https://github.com/wu162/NewSdk/actions/runs/40244825458) attempt1 **success**：D/R loop4224、optional3888、COMM7776、offset2704、MPEG1084及旧受影响回归/整工程构建通过；新增1820资产SDK的实际三缓冲区、四产物及CustomData一致，恢复侧删除原AudioCompiler DLL。

前一完整批 `bb4a225` / [40243817293](https://github.com/wu162/NewSdk/actions/runs/40243817293) success：4488资产SDK已通过。其精确旧source/run保持不变；新COMM边界和offset组合由1820新批覆盖，未重复旧4488。详见[覆盖审计](audio/CommSdkCoverageAudit.md)和[验收索引](evidence/audio/acceptance-current.json)。

修复循环元数据、signed COMM过滤、SSND有界offset、WAVE无padding lookup、MPEG EOF/真实容器识别。原MP3透传SIMEX_info只查头信息，不解PCM；错误增加的NLayer解码门槛和19个未引用文件已删除。MARK-only原未初始化mode、单端匹配和越界输入继续明确拒绝。

## 本轮完整 Upheaval 结果

源码 `c078bce6e5e28866dfd3c25acabda731203b2556` / [40248486242](https://github.com/wu162/NewSdk/actions/runs/40248486242) attempt1 **success**，音频树仍为`ee6074a096795aa66b682fcb363e360feabe4d2f`。固定输入`068643219fded09a05fb7b75c6507b769c430316`保留194个.asset；case3/4/5原/恢复共10次BAB调用全部exit0，20份产物一致；各侧1293份CustomData、1289次音频回调，1281 required与1281 actual unique资产匹配，恢复侧原AudioCompiler缺席。收据`evidence/audio/upheaval-sdk-validation/40248486242-1/validation.json`（提交`ef87339d3f76848687b714a394a9e1ff00ad1923`）。其他插件使用原SDK；本WAV mono语料不能代替AIFF/AIFC边界SDK或多声道验收，仅证明上述音频替换范围。

## 下一步与边界

1. 生产当前仅mono/stereo；4/6声道是新确认待恢复的完整能力组，正在原DLL隔离探针阶段，尚未实现；见[多声道边界](audio/MultichannelBoundary.md)。完整组需覆盖WAVE重排、PCM/XAS、RAM/stream、loop和重采样；3/5/8与多声道ADP4不默认支持。变更源码后必须保留旧成功scope并取得新验收。
2. 合入`compiler-source`前再读目标头。保留其通用子系统导航，不能用本Audio专用RESUME覆盖；已读b122a54与成功测试分支的SDK及shared recovery/src树相同，未变子系统不重复运行。
3. [音频合入审查](audio/AudioIntegrationReview.md)记录固定SDK/共享树、原版与恢复版实际覆盖和通用导航保留要求。

完整Upheaval音频门槛已通过；compiler-source尚未合入；SDK/tools原DLL/config未替换；任务尚未全部完成。范围及限制见[AUDIO_COMPILER.md](AUDIO_COMPILER.md)和[验收索引](evidence/audio/acceptance-current.json)。
