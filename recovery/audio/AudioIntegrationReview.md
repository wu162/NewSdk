> 最新状态：PC功能已完成，最终AIFF/AIFC4/6验收见[AIFF边界](MultichannelAiffBoundary.md)及[续接入口](../RESUME.md)。本页以下保留成果集中操作的历史记录；新PR等待用户明确批准后才能合入。

# AudioCompiler 成果集中核对

按用户2026-10-26指示，统一工作分支为`recovery/audio-compiler-source`；原PR[#10](https://github.com/wu162/NewSdk/pull/10)关闭，不合入`recovery/compiler-source`。此操作只集中已存在的源码、证据及下一组静态准备，保留全部来源提交历史，普通快进更新，禁止强推。

## 保留的验收范围

- WAVE4/6：source`ed1a7bcbec4a7a89d08369b9668129c8c554f8ec`，Audio tree`a8e47a2e737ca7e35a486ec17e5eaaa913c8678e`，run40250750964-1；D/R1728新组合和受影响回归通过，同1728资产真实SDK四产物、CustomData及三缓冲区一致，恢复侧原Audio DLL缺席。
- 固定完整Upheaval音频替换：source`c078bce6e5e28866dfd3c25acabda731203b2556`，tree`ee6074a096795aa66b682fcb363e360feabe4d2f`，run40248486242-1；10次BAB调用exit0，20产物及各侧1293份CustomData一致，1281唯一资产/每侧1289音频回调。仅音频替换，其他插件原SDK；保留原源码与输入范围。
- 旧1820/4488 SDK、WAVE原版探针及AIFF原版192案例/384进程保留原source/run，不改标为集中提交新跑。
- 原音频分支独有run40250230722-1的FFT/PCM/plugin/XAS四个证据子树按原Git SHA完整保留，只作为其原源码的历史证据。

## 树与续接入口

原SDK树`1f2d71a20ce8bf7d9343c7a2246c10275abe28f9`、共享`recovery/src`树`c13c492885fc64bfac0f53562360d706252b6489`保持原样。生产源码、已验收测试和历史证据未改；两份多声道工作流增加原音频分支监听，仍由原有显式标记选择执行，不因集中重复跑SDK。

直接阅读[RESUME.md](../RESUME.md)、[AUDIO_COMPILER.md](../AUDIO_COMPILER.md)及[验收索引](../evidence/audio/acceptance-current.json)。AIFF/AIFC NONE4/6下一组准备集中在[本分支preparation目录](../preparation/audio-aiff-multichannel/NEXT.md)，尚未编译、接线或启用；ADP4>2保持原生不支持的拒绝边界。

完整来源头、证据子树与核对记录见[MERGE_VALIDATION.json](MERGE_VALIDATION.json)。旧辅助分支保留历史，不再作为必须的续接入口。未来合入其他分支仍必须先PR且由用户明确批准。
