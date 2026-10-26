# AIFF/AIFC NONE4/6 acceptance

2026-10-26. PC / Win32 only. Source `7abc0505f6340504f32767b86de7a34b04ac3367`, AudioCompiler tree
`8bbe47d084d879b38397bc45e4493965640731ec`; [run 40257973827](https://github.com/wu162/NewSdk/actions/runs/40257973827), attempt1 success.
Evidence commit `47823e9451018654e61237ef32cfde36d463f22e`. All source/run anchors below refer to this run.

## Implementation

`AudioPcmInputReader` now admits integral AIFF and AIFC/NONE channels4/6.
Channels remain in source order; WAVE's existing permutation stays WAVE-only.
Native importer/read wrapper RVAs381c0..3895e/389a0..38bbb and the original
192-case probe establish this distinction; see [native audit](MultichannelAiffNativeAudit.md).
ADP4 channels greater than2 are refused before sample allocation or decoder invocation,
matching representation13's missing native generic callback at RVAfdfd4.
No codec, resampler, SNR/SNS layout or SDK production configuration was changed.

## Complete component group

Debug and Release each compare6336 complete ProcessInstance three-buffer and
CustomData records with the original DLL, with the original absent from the recovered host:

| Group | Records per configuration | Coverage |
|---|---:|---|
| Baseline |1728|AIFF/AIFC,4/6,8/16/24,127/128/129/4096 frames, no loop/start0/intro129,22050/11025/44100/8000Hz,NONE/XAS,RAM/stream|
| COMM/marker |3456|18 signed declaration/duplicate/last-COMM variants, actual and canonical, both resampling directions, all containers/channels/widths/codecs/storage modes|
| SSND offset |1152|129 frames,offset0/1/frameSize-1,blockSize0/uint.MaxValue, actual and shifted canonical, both resampling directions, all containers/channels/widths/codecs/storage modes|

Independent reader checks per configuration:7740 exact source-order integer samples,
72 canonical sample/metadata/immutability windows,432 independent marker expectations,
108 safety refusals (one-past/end/uint.MaxValue offsets, partial marker pairs,
single-frame resampling). Separate complete WAVE regression retains64 original direct
cases/128 encoder comparisons/15 invalid-channel checks; updated format oracle has42
refusals covering3/5/8-channel WAVE/AIFF/AIFC NONE and4/6-channel ADP4.
All affected mono/stereo AIFC,ADP4,loop/optional metadata,COMM,SSND blockSize/offset,
XAS precision and full-project Debug/Release build gates passed.

## Real SDK

Profile `create-multichannel-aiff` compiles the same6336 assets after original full
CnC3Types schema validation. Each side has6336 unique actual handler callbacks and
6336 CustomData files; all three buffers, CustomData and four final products match.
Original-first execution uses one disposable SDK root, identical inputs/commands,
non-target DLL/BAB/config fingerprints; the recovered side deletes the original
AudioCompiler DLL. Repository SDK/tools/config remain unchanged.

Receipts: `recovery/evidence/audio/multichannel-aiff-validation/40257973827-1/validation.json`
and `recovery/evidence/audio/sdk-validation/40257973827-1/validation.json`.
The mono WAVE full-Upheaval run40248486242 and earlier WAVE/PC SDK receipts keep
only their original source/input scope; they are not relabeled as current-source runs.

## Completion boundary

No implementation task remains in the agreed PC compiler scope. Continue to refuse
unsafe native reads, MARK-only uninitialized mode, partial marker pairs, out-of-range
loops/SSND windows and single-frame ADP4/import-resampling. Unaudited malformed
containers do not carry universal equivalence guarantees. Original pointer-helper
binary ABI, final EXE integration, refactoring and non-PC platforms are outside scope.

本轮按完整能力组合批执行一次新增范围SDK；复用完整Upheaval原验收，避免重复完整MOD运行。没有账号额度对照数据，不量化额度节省。
