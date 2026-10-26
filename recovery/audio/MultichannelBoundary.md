# PC 4/6-channel recovery boundary

Status on 2026-10-26: **confirmed missing capability group; not implemented**.
Production Audio remains mono/stereo at source tree
`ee6074a096795aa66b682fcb363e360feabe4d2f`. This round preserves that accepted
source for its full pinned Upheaval Audio-only merge. The original managed
`ProcessAudioFileInstance` explicitly recognizes 1/2/4/6 channels; other counts
produce a warning. This management-layer observation alone does not establish
every importer/codec combination. Isolated original-DLL probe CI results and
exact source/run/receipt anchors are to be appended by the SDK probe owner.
No probe result or multichannel acceptance is claimed here.

## Production changes needed as one group

| File | Required change after original evidence |
|---|---|
| `src/AudioCompiler/AudioPcmInputReader.cs` | Integral input guard admits 4/6; WAVE-specific channel remap after import; AIFF/AIFC NONE mapping separately established; ADP4 greater than2 stays explicitly rejected |
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

Original-DLL probes with distinct channel constants/impulses must confirm this
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

## Required complete capability matrix

Use distinct per-channel values and asymmetric impulses/ramp/random samples,
so identical content cannot hide swapped channels. All supported combinations
need original-vs-restored three-buffer and CustomData equality in Debug and
Release, with explicit actual coverage counts and source/run anchors:

- Channels4 and6; retain affected mono/stereo regression evidence.
- WAVE PCM8/16/24, WAVE extensible including ignored extension metadata,
  AIFF PCM8/16/24 and AIFC NONE; keep ADP4 greater than2 rejected unless its
  complete layout is independently recovered.
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

## Isolated original probe results

Pending SDK probe owner: append terminal runId/attempt, source/input hashes,
receipt paths, observed mapping and per-format/codec outcomes. Preserve failures
and unsupported combinations; do not infer every importer from one successful
PCM WAVE sample. No production4/6 implementation is included in this round.
