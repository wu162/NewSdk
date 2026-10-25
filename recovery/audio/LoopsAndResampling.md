# Loop and resampling investigation

2026-10-25. Evidence-only continuation of [NativeAudit.md](NativeAudit.md).
The examined original `SDK/tools/BinaryAssetBuilder.AudioCompiler.dll` has SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses below are RVAs; the preferred image base is `0x10000000`.
[Original instruction excerpts](loops-resampling-disassembly.txt) preserve the
branches supporting the conclusions. No new runtime support or acceptance is
claimed by this document.

## Original structures and dispatch

`SINFO` is 136 bytes, beginning with an `SSOUND*`; `SSOUND` is 1440 bytes.
The current managed IL (`Plugin.ProcessAudioFileInstance`, RVA `0x5974`) supplies
the processing policy before invoking the native writer.

| SSOUND offset | Meaning supported by original uses |
|---|---|
| `0x68 + channel*4` | Per-channel signed-16 sample pointer used by resampler |
| `0x1f5` | Requested output sample representation |
| `0x1f6` | Channel count |
| `0x206` | Play-location flags; plugin selects RAM `0x800` or streamed `0x1000` |
| `0x214` | Sample rate |
| `0x218` | Source sample frames |
| `0x21c` | Loop start; negative means absent |
| `0x220` | Inclusive loop end |
| `0x250 + group*4` | Auxiliary marker-group pointers, rescaled during resampling |
| `0x570` | Number of auxiliary marker groups |

SIMEX format table at `0xfd160` has 40-byte entries. Format **0** is Apple
AIFF/AIFC (metadata callback `0x37f80`, identifier `aiff` at `0xb88b0`), while
format **1** is Microsoft WAVE (metadata callback `0x37040`, identifier `wave`
at `0xb8754`). Both IDs are accepted by the managed plugin. The order of a
generic phrase such as "WAVE and AIFF" must not be used to infer their IDs.

## WAVE loop import and output

The WAVE importer at `0x37210` searches for `smpl` (`0x373e3` calls the chunk
lookup with the string at `0xb879c`). It reads the 44-byte chunk header at
`0x3741a`; when its loop count is positive (`0x37431`..`0x37445`), it reads
the first 24-byte loop record (`0x37447`..`0x37457`). It copies that record's
start and end to `SSOUND+0x21c` and `SSOUND+0x220` at `0x3746e` and `0x37483`.
The end value is copied unchanged and is inclusive. The observed code reads
only the first loop record; the policy for other sampler fields requires
original-plugin probes before promising compatibility.

The AIFF marker parser uses big-endian marker positions, assigning loop start
at `0x387a2` and loop end at `0x387cc`. The latter subtracts one from the marker
position, unlike WAVE's inclusive `smpl` end. Resolving the sustain/release
marker selection and all chunk-presence rules still needs AIFF input probes.

The SNR header writer is `0x2cb50`. After the usual 35 bits of version, codec,
channels, rate and storage, its loop fields are:

1. One `loopPresent` bit computed as `loopStart >= 0` (`0x2cc5c`..`0x2cc70`).
2. A 29-bit frame count: `loopEnd + 1` when a loop exists, otherwise the source
   frame count (`0x2cc75`..`0x2cc92`).
3. A 32-bit loop-start frame index when a loop exists (`0x2cc97`..`0x2cca2`).
4. For streamed loop output, another 32-bit value supplied by the caller
   (`0x2cca7`..`0x2ccc1`). It is the byte offset of the loop segment in the SNS data: intro
   segment length, including block headers and padding (see below).

Loop support is **not** just setting those header bits. The writer's source
extent is `loopEnd+1` when the start is valid (`0x2e105`..`0x2e139`), discarding
samples after the loop end. RAM and streamed paths then split or flush at
loop boundaries (`0x2e26a`..`0x2e41d`), with additional codec-specific handling.
In particular, the streamed branch can end an encoder segment at the loop
start (`0x2e3f3`..`0x2e403`) and encode `end-start+1` frames for its loop segment
(`0x2e405`..`0x2e410`). The intro length is captured as the output position after the intro segment
minus its initial position at `0x2e49a`..`0x2e4ab`; start zero forces that
offset to zero at `0x2e4af`..`0x2e4bc`. The caller passes it to the header
writer at `0x2e7be`..`0x2e7c9`. Implementing only metadata would produce
incorrect CustomData. The existing segment encoder can be composed for
this path, pending the full original-plugin loop differential.

## SIMEX filter 120: actual rate conversion

`SIMEX_filterabout` is at `0x29f60`; `SIMEX_filterssound` is at `0x29fb0`.
The callback table begins at `0x127640`, so filter 120 occupies `0x127820`.
Its registration at `0x42c50` sets that slot to **`0x429a0`** and describes it
as `SIMEX_FILTER_RESAMPLE`, `Resample`, switch `rs`, one rate parameter.
Do not confuse it with filter **220**, `SIMEX_FILTER_RESAMPFILTER`, whose
callback is `0x42740` and which reads a textual filter profile.

The managed caller validates a changed requested rate in `[400,96000]`,
constructs 56-byte `SIMEXFILTERPARAM` records using the filter metadata, writes
the requested integer rate at each record's offset 0, and calls filter 120.
It reads back the actual resulting rate and warns if it differs from the
request. Unchanged rates bypass this filter entirely.

The original callback first computes the binary32 ratio `newRate / oldRate`.
It has a significant short-input branch: if binary32 `oldFrames*ratio <= 1`,
it changes only the sample rate, preserves the original sample pointers and
frame count, and returns success (`0x429d9`..`0x429fa`). Otherwise, when rates
differ, it resamples each signed-16 channel separately via `0x42480`.

For the normal path, the new frame count is
`truncate(binary32(binary32(oldFrames) * ratio))`; it is not a rounded rational
rescaling. The final sample rate is the exact requested integer. If the old
loop end is positive, both loop indices become
`truncate(binary32(binary32(index)*ratio + 0.5f))` (`0x42b4d`..`0x42b81`).
Auxiliary marker positions undergo the same operation (`0x42b87`..`0x42be9`).
Finally an end index greater than or equal to the resulting frame count is
clamped to `newFrames-1` (`0x42bfb`..`0x42c0c`). These rules include asymmetry
for end zero and for the short-input branch; do not replace them with a
general-purpose resampling library's metadata behavior.

The kernel `0x42480` is not simple linear interpolation. Its dependency chain
includes coefficient setup `0x41a90`, source-window fill `0x41f40`, FFT/filter
processing `0x41fa0` and `0x42250`, FFT state constructor `0x4ec40`, transform
`0x4eca0`, and teardown `0x4ec80`. It uses 8192-point working buffers and cubic
interpolation of filtered samples (`0x42631`..`0x426ba`). Output converts via
x87 `fistp` to signed16 and handles the `0x8000` overflow result with the float
sign (`0x426c0`..`0x426df`). A replacement must preserve intermediate float
rounding, FFT operation ordering, and the original FPU rounding mode.

Filter setup at `0x42a06`..`0x42ab6` distinguishes upsampling, downsampling by
at most two, and downsampling by more than two; it uses 256 as a setup value,
pass-band constants 0.5 and binary32 0.95, and different filter shape codes
3/4. These constants alone do not specify the full filter response.

## Recommended next implementation batch

- Recover no-resampling PCM loop support first, with original/candidate input
  fixtures covering zero/nonzero starts, inclusive ends, trailing source
  samples, block-boundary starts, unaligned XAS starts, RAM/streamed output,
  and mono/stereo. Retain full runtime, relocation, imports and CustomData
  comparison; inspect the streamed extra header field before implementation.
- Add AIFF sustain/release loop fixtures separately; do not apply WAVE end
  semantics to AIFF marker positions.
- Recover filter 120 and all of the kernel dependency chain as one batch.
  Generate native-machine-code oracles for coefficient/filter kernels, then
  compare the actual original AudioFile route at up/downsample ratios and
  lengths around `oldFrames*ratio == 1`, fractional frame-count truncation,
  impulses, DC, alternating full scale, sine and noise, and loops/markers.
- Complete a real SDK batch only after the affected component differential
  passes. Investigation addresses or isolated native emulation do not count
  as plugin or SDK acceptance.

The accompanying `Native/LoopPcmEncoder.cs` candidate composes existing PCM/XAS
segment encoding with the recovered loop header and truncation rules. It is
not accepted until the 216-fixture `AudioLoopProbeCases.cs` original-plugin
matrix passes. Resampling remains unimplemented. Production SDK binaries
and configuration were not modified by this investigation.
