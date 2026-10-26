# PC 4/6-channel recovery boundary

Status on2026-10-26: **WAVE4/6 complete group accepted**.
Source `ed1a7bcbec4a7a89d08369b9668129c8c554f8ec`, tree `a8e47a2e737ca7e35a486ec17e5eaaa913c8678e`,
run [40250750964](https://github.com/wu162/NewSdk/actions/runs/40250750964) attempt1 success,
evidence commit `7eba9f108008ae424b18d10f56bdbcb6ff5487fd`. Debug/Release1728 original/plugin
comparisons and affected regressions pass; dedicated real SDK executes exactly
the same1728 unique combinations, with equal full buffers,1728 CustomData
files and four products. The recovered SDK has no original target DLL.
Independent public ReadWave/EncodeWave/EncodeSamples checks reuse64 original
records (128 encoder comparisons), plus36 reader and15 direct-channel refusals.

Original-only WAVE discovery40249960201 (source48fa1f4d) succeeded96cases/
192 processes, confirming maps below for all integral widths and output modes.
AIFF/AIFC NONE remain mono/stereo in restored production. Their original-only
run40251368891 confirms identity ordering; see
[separate native audit](MultichannelAiffNativeAudit.md). ADP4>2 is unsupported
by the original importer and remains rejected.

## Implemented WAVE group

| File | Implemented WAVE change |
|---|---|
| `src/AudioCompiler/AudioPcmInputReader.cs` | WAVE guard admits4/6; WAVE-specific remap once after import; AIFF/AIFC and ADP4 stay1/2 |
| `src/AudioCompiler/AudioResampling.cs` | Admit 4/6 at interleaved input guard; retain exact signed16 and frame/loop safety checks |
| `src/AudioCompiler/Native/Pcm16Encoder.cs` | Both ReadWave and EncodeSamples guards; standalone ReadWave must perform the same WAVE remap as the production reader |
| `src/AudioCompiler/Native/Xas1Encoder.cs` | Both EncodeBlock128 and EncodeSamples guards; retain precision/finite sample checks |
| `src/AudioCompiler/Native/LoopPcmEncoder.cs` | Admit 4/6 at frame-interleaved guard; keep inclusive loop and segment behavior |

These are seven guards across five files. An eighth mono/stereo guard in
`Native/Adp4Decoder.cs` is **not** a simple admission change: the decoder has
only left/right predictor/index state, stereo frame*2 indexing and one packed
byte per stereo frame. Its multichannel nibble/state layout needs separate
original evidence. Do not generalize its stereo else branch.

The regular PCM reader is sample-linear with frameSize=channels*width.
PCM encoding, loop slice offsets and payload lengths already use channels.
XAS iterates channels, writes76 bytes per channel per128-frame block, buffers
channels*128 floats, and repeats every channel of the last frame for padding.
The resampler extracts each channel to its own short[] and runs the existing
single-channel kernel. Loop points remain frame coordinates. SNR encodes
channels-minus-one in six bits and AssetBuffer uses a channel byte. These
observations support code reuse; none substitutes for differential evidence.
Stream block sizes depend on bitrate*channels and must be compared anew.
MPEG passthrough derives1/2 from MPEG channel mode and needs no expansion.

## WAVE channel ordering: avoid the inverse-mapping trap

Focused external reference: `jonwil/sagetools` main,
`simex/source/cmn/swave.cpp` blob
`bd998c2f1a3e7502fe06c845593ad87ab7e53015`; its
`SIMEXI_remapWaveToITU` changes **destination track pointers before import**.
`ssamples.cpp` then writes file channel c into ptracks[c][frame], and swave
restores the original SSOUND pointer structure. Therefore directly copying
the pointer permutation as managed output order gives the wrong6-channel
mapping. With zero-based file channels:

| Channels | Import destination pointer permutation | Canonical output takes file source channels |
|---|---|---|
| 4 | [0,1,3,2] | [0,1,3,2] |
| 6 | [0,2,1,5,3,4] | [0,2,1,4,5,3] |

Original-DLL probes with distinct channel constants/impulses confirmed this
direction for the target binary. Apply once on WAVE import, before encoding;
resampling each independent channel commutes with this permutation. Do not
apply WAVE ordering to AIFF/AIFC NONE without target evidence. Preserve the
recovered WAVE extensible policy: validBits, mask and GUID do not reinterpret
integer container samples. The target may have noChannelReordering behavior;
only the actual production caller's default is in scope.

External XAS reference `audiocore/source/encoders/encxas1.cpp` blob
`9111dd829205a6afa959f15d861e3fc0251b36ef` uses per-channel EncodeBlock loops,
76-byte interleaving and channel-scaled residue/padding. It supports the
parameterized-layout audit, not acceptance of the restored target binary.

## Complete capability inventory (WAVE accepted; AIFF/AIFC next)

The list below includes the next AIFF/AIFC group; current run40250750964
covers WAVE only. Do not infer AIFF/AIFC4/6 acceptance from its WAVE result.

Use distinct per-channel values and asymmetric impulses/ramp/random samples,
so identical content cannot hide swapped channels. All supported combinations
need original-vs-restored three-buffer and CustomData equality in Debug and
Release, with explicit actual coverage counts and source/run anchors:

- Channels4 and6; retain affected mono/stereo regression evidence.
- WAVE PCM8/16/24, WAVE extensible including ignored extension metadata,
  AIFF PCM8/16/24 and AIFC NONE; keep ADP4 greater than2 rejected because the original importer does not dispatch it.
- PCM16 and XAS; RAM and streamed containers; same-rate, upsample and
  downsample paths, including rate-only resampler paths and loop rescaling.
- No loop, loopStart0 and nonzero intro; inclusive loop end and discarded
  trailing samples; WAVE and AIFF metadata semantics already supported.
- Lengths around127/128/129 XAS frames, full and partial final blocks, and
  lengths crossing actual4/6-channel PCM/XAS streaming block limits; verify
  terminal padding, headers, per-channel order and loop byte offsets.
- Invalid loop/source windows retain existing refusal behavior;3/5/8 are
  not silently enabled. Single-frame uninitialized imports remain refused.

After the component group passes, run a dedicated real BAB SDK MOD with both
new channel counts, all accepted container families, codec/stream/loop/rate
routes and actual handler coverage; compare four products, all CustomData,
actual three buffers and original-Audio absence. Original and restored sides
must consume identical inputs. Keep the old component source2d6468 /
run40244825458 and full Audio-only Upheaval sourcec078bce6 /
run40248486242 immutable; neither proves the new code. The accepted Upheaval
corpus is mono WAV and cannot supply this multichannel integration gate.

## Accepted matrix and next group

The measured WAVE matrix is exactly1728 combinations: channels4/6, bits8/16/24,
standard/extensible, frames127/128/129/4096, no-loop/start0 at every length,
intro129 at4096, target rates22050/11025/44100/8000, NONE/XAS, RAM/streamed.
Distinct per-channel composite signals cover order and interleave. The older
mono/stereo and XAS precision groups passed with this exact new source.
The source/run above owns new SDK acceptance. Historical1820/4488/full-Upheaval
receipts keep their old source scopes; none is relabeled as a new execution.

AIFF/AIFC NONE4/6 need the next complete group. Their raw origin ordering is
already proven by192cases/384 original invocations and80496 decoded PCM samples.
Do not apply the WAVE map to these containers or claim restored support early.
