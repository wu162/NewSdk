# AIFF/AIFC multichannel native audit

2026-10-26. Read-only target audit and original-only probe preparation. No
candidate AIFF/AIFC4/6 support or SDK acceptance follows from this document.
Target DLL SHA256:
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All offsets below are RVAs, image base0x10000000. Extracted directly from the
repository SDK DLL with pefile/Capstone; existing
`pcm-input-disassembly.txt` and `ssnd-offset-disassembly.txt` retain the
corresponding importer/read-wrapper ranges.

## AIFF/AIFC NONE: source-order inference

- Open/importer `381c0..3895e`; COMM `3836d..3845e` reads BE channels,
  declared frame count, integral width and rate. PCM8 selects representation2,
  PCM16 representation1, PCM24 representation21. AIFC `3845e..3848c` changes
  to representation13 only for ADP4; NONE retains integral PCM.
- Read wrapper `389a0..38bbb` initializes decoder state and locates SSND.
  At `38b48`, it takes the original track array with `lea edx,[eax+0x68]`.
  At `38b66` it reads numchannels from sound+0x1f6. The unpermuted track array,
  channel count and selected representation are passed to `404e0` at `38b88`.
- This wrapper contains no WAVE-style destination-track pointer permutation.
  Common dispatcher `405c3..405d1` sends signed16BE representation1 directly
  to `3adc0`; the other integral representations use their own conversion
  paths. There is no observed AIFF-specific reordering step here.

Thus the static chain supports **keeping AIFF/AIFC NONE source channel
order**, unlike WAVE. This is an inference, not proof that every4/6 importer,
codec and stream combination succeeds or is byte-equivalent. Distinct-channel
constant/impulse original ProcessInstance probes must confirm the output.
Do not reuse WAVE's `[0,2,1,4,5,3]` six-channel mapping on AIFF by analogy.

Jonwil/sagetools main `49373be5e922e8d7ae0b2caa2c74fa6c64d6cf4c`, tree
`ed215aa64d2803b440f2e59d365331d50e515ed5`, contains swave/smpeg/ssamples
but no AIFF importer in its examined tree. Its WAVE source cannot establish
AIFF ordering or acceptance for this target.

## ADP4 beyond stereo: native rejection chain

At `40727` the dispatcher recognizes representation13:

| Channels | Branch |
|---|---|
| 1 | `4072c..4073e` calls mono reader `3b970` |
| 2 | `4074b..40761` calls stereo reader `3bab0` |
| Other | `4074e` jumps to generic callback lookup `40840` |

The generic callback table at RVA`fdfa0`, entry13 (`fdfd4`), contains0 in the
target DLL. The initialization calls at `404e9..404f8` install allocator hooks,
not an ADP4 multichannel importer. The missing callback reaches `4087c`, which
reports string RVA`b9330`: "Unsupported sample import format." `4088c` clears
eax and the dispatcher returns0. This provides a concrete reason to retain
ADP4>2 rejection rather than treating it as an unimplemented general layout.

Stereo reader `3bb40..3bb69` takes state[0] and state[1] at `[ebp]` and
`[ebp+4]`, decodes high/low nibbles via `48500`, and writes track[0]/track[1]
at `[ebx]` and `[ebx+4]`. It is not a channels loop. The final-block path
`3bb90..3bbb9` repeats those same two states/tracks.

Important existing exception: `40518..40523` returns success before importing
when start0/count1 yields end0. Its samples are uninitialized. This is not
multichannel support and remains an explicit candidate safety refusal.
A separate ADP4>2 ProcessInstance rejection probe may document the managed
error presentation later; it is unnecessary for enabling AIFF/AIFC NONE and
must not be inferred as positive capability.

## Prepared bounded original-only discovery

- Host: `recovery/tests/AudioMultichannelAiffOriginalProbe.cs`.
- Runner: `recovery/scripts/TestAudioMultichannelAiffOriginal.ps1`.
- Workflow: `.github/workflows/audio-multichannel-aiff-original.yml`.
- Branch: `validation/audio-upheaval-win32`; explicit commit marker
  `[audio-multichannel-aiff-original]`; no production changes or candidate run.
- Default192 cases:2 containers (AIFF/AIFC NONE),4 channel counts (1/2 controls,
  4/6 targets),3 widths (8/16/24),2 signals,2 codecs,2 stream modes,129 frames.
  Each case executes in two independent x86 processes:384 total invocations.
- Records preserve complete instance/relocation/import buffers and CustomData,
  metadata, managed errors, exit codes, timeout state and repeat-byte identity.
  DLL fingerprint is pinned; per-process timeout30 seconds; full input bytes
  and SHA256 fingerprints are saved. Discovery errors/crashes remain evidence.
- Receipt states originalOnly=true, candidateValidated=false and
  sdkValidated=false. A completed discovery job does not mean every case
  succeeded: inspect completeCases, stableCases, controlsComplete and individual
  records before deriving the next implementation group.

No CI result is claimed at preparation time. The caller must append exact
source/run/attempt and observed source-order results from the real receipt.

## Original ProcessInstance confirmation

Run [40251368891](https://github.com/wu162/NewSdk/actions/runs/40251368891),
source `0d744a716df7b487816dc0fadf4f12dc82c5b66c`, attempt1 completed:
192 cases /384 isolated processes all succeeded and repeated byte-identically.
All192 fixture hashes and384 record hashes were checked. Decoding96 NONE
cases twice compared80496 PCM samples exactly: both AIFF and AIFC/NONE keep
source order,4ch [0,1,2,3],6ch [0,1,2,3,4,5], for8/16/24-bit,
constant/impulse, RAM/streamed. XAS outcomes were successful and stable; this
audit's independent sample decoder covers NONE only.

Receipt at commit `5f2574fc7f60b5b63db4100f7b86e9e832efe15d`:
`recovery/evidence/audio/multichannel-aiff-original/40251368891-1/validation.json`.
See [mapping audit](multichannel-aiff-original-audit.json). Reproduce from
repository root with
`python recovery/scripts/AuditAudioAiffMultichannelOriginal.py recovery/evidence/audio/multichannel-aiff-original/40251368891-1 /tmp/aiff-mapping-audit.json`.

This confirms original behavior only. The restored reader still refuses
AIFF/AIFC channels4/6. The next complete implementation group must separately
cover NONE/XAS, loop/COMM/SSND behavior, resampling, streaming boundaries,
Debug/Release full buffers and a real SDK milestone. ADP4 greater than2
remains an original unsupported format, not a missing recovery feature.
