# EALA.Hash 交接：源码恢复与原版差分完成

2026-10-26。工作分支 `recovery/eala-hash-source`，目标 `recovery/compiler-source`，PR #12 等用户批准后合入。
纯 C# 源码 `recovery/src/Managed/Hash`，全部六个公开重载和可构造类恢复。
源码 `e300225c`；Windows run `40262767940` attempt1 success，D/R各84380行转录与原版一致。
非法越界长度恢复侧明确拒绝，原版该输入是未定义内存读取。详细行为、原IL与公开实现对比见 [HASH.md](../../HASH.md)。
未替换SDK/tools或改变Core引用，未做真实SDK集成验收。本任务没有剩余源码实现待办。

---

# AudioCompiler 交接：PC 功能已验收，等待 PR 审查

2026-10-26（北京时间）。唯一工作与交付分支 `recovery/audio-compiler-source`。
仅PC/Win32；不负责最终EXE或重构。禁止强推；任何合入必须先PR并由用户明确批准。

最后一组AIFF/AIFC NONE4/6已完成。源码 `7abc0505f6340504f32767b86de7a34b04ac3367`，音频树 `8bbe47d084d879b38397bc45e4493965640731ec`；
[运行40257973827](https://github.com/wu162/NewSdk/actions/runs/40257973827) attempt1 success，证据提交 `47823e9451018654e61237ef32cfde36d463f22e`。
D/R各6336项三缓冲区/CustomData差分、7740源序样本、72窗口、432marker独立检查、108安全拒绝通过；
全部受影响回归及D/R完整工程构建通过。真实SDK同6336资产全部实际回调，四产物/CustomData一致，恢复侧原音频DLL缺席。
细节与准确范围见[AIFF验收](../../audio/MultichannelAiffBoundary.md)及[验收索引](../../evidence/audio/acceptance-current.json)。

当前支持WAVE/AIFF/AIFC NONE的PCM8/16/24、1/2/4/6声道；raw ADP4仅1/2。
WAVE4/6导入重排，AIFF/AIFC保持源序。PCM/XAS、RAM/stream、loop、filter120重采样、MPEG信息/透传及五个BAB路由已验收。
多声道ADP4原native不支持；非PC、未定义原生读取、单端marker、越界loop/SSND及单帧ADP4/import-resample继续拒绝。

约定PC功能范围内无剩余实现待办。下一步审查[PR #11](https://github.com/wu162/NewSdk/pull/11)，目标 `recovery/compiler-source`；未获用户明确批准不得合入。
SDK/tools/config未替换。完整固定Upheaval仍保留source`c078bce6`/run40248486242原锚点，
不冒充本轮完整MOD重跑；本轮另有6336资产专用SDK。旧WAVE/1820/4488等证据保持原范围。
