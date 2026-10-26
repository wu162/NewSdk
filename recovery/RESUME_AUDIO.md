# AudioCompiler 当前交接

2026-10-26（北京时间）。仅PC/Win32；最终职责为Audio恢复及音频SDK验收。先读AGENTS.md、EFFICIENCY.md、[AUDIO_COMPILER.md](AUDIO_COMPILER.md)。禁止强推；合入必须先PR并经用户明确批准。

## 本轮新增已通过

WAVE4/6源码 `ed1a7bcbec4a7a89d08369b9668129c8c554f8ec`，源码树 `a8e47a2e737ca7e35a486ec17e5eaaa913c8678e`，[运行40250750964](https://github.com/wu162/NewSdk/actions/runs/40250750964) attempt1 success；证据提交 `7eba9f108008ae424b18d10f56bdbcb6ff5487fd`。D/R各1728多声道三缓冲区/CustomData差分、全部受影响旧组、36拒绝边界、64公开入口原版案例/128直接比较、15非法声道检查及整工程构建通过。真实SDK同1728完整组合均实际编译，四产物和1728份CustomData一致，恢复侧原音频DLL缺席。WAVE4取源[0,1,3,2]、6取源[0,2,1,4,5,3]，仅导入时一次重排。

当前支持：WAVE PCM8/16/24含EXTENSIBLE的1/2/4/6声道；AIFF/AIFC NONE及raw ADP4仍1/2；PCM/XAS、RAM/stream、loop、filter120重采样及MPEG头信息/透传。非Win32、3/5/8、未定义原生输入等明确拒绝。

## 本轮完整 Upheaval 结果（保留旧源码锚点）

源码`c078bce6e5e28866dfd3c25acabda731203b2556` / [40248486242](https://github.com/wu162/NewSdk/actions/runs/40248486242) success，音频树`ee6074a096795aa66b682fcb363e360feabe4d2f`。固定输入`068643219fded09a05fb7b75c6507b769c430316`保留194个.asset；原/恢复共10次BAB调用exit0，20份产物、各侧1293份CustomData一致；1281唯一音频资产全部覆盖、每侧1289回调三缓冲区一致。仅替换音频，其他插件原SDK；语料mono WAV。不重标为本轮a8e47a源码新跑成功，新源码另有上面的1728 SDK。旧组件/1820 SDK source2d6468/run40244825458、4488 SDK sourcebb4a225/run40243817293继续保留其精确范围。

## 下一组与审批状态

1. PR[#10](https://github.com/wu162/NewSdk/pull/10)等待用户明确批准，未合并。PR分支保留compiler-source通用RESUME，本音频入口为RESUME_AUDIO.md；共享源码及SDK树不变。不得自行点击或API合并。
2. 下一实现组AIFF/AIFC NONE4/6：原版[40251368891](https://github.com/wu162/NewSdk/actions/runs/40251368891)的192案例/384进程全稳定成功；80496个PCM样本实证保持源序。恢复版仍拒绝，需完整格式/loop/COMM/SSND/resample/stream差分和新SDK；见[audit](audio/MultichannelAiffNativeAudit.md)。ADP4>2原native明确拒绝，不列缺失功能。
3. 保留MARK-only未初始化mode、单端marker、越界loop/SSND offset、单帧ADP4/import-resample等安全拒绝。不重复触发未变已验收组；SDK继续按完整能力组集中运行。

精确收据见[验收索引](evidence/audio/acceptance-current.json)；源码与运行锚点不得交叉套用。仓库SDK/tools/config未替换。所有已触发运行均已收取终态，没有未完成的验证作业。
