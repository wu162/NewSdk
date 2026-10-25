# Single-frame SIMEX import boundary

2026-10-25 (Beijing). Original input is
`SDK/tools/BinaryAssetBuilder.AudioCompiler.dll`, SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses below are RVAs. The accompanying
[single-frame-import-disassembly.txt](single-frame-import-disassembly.txt)
was decoded from that PE. This boundary concerns the original file importer,
not a different interpolation algorithm or a single-frame restriction of the
recovered numerical kernel.

## Observed full-plugin discrepancy

The original-only run `40199000936` and full-plugin differential run
`40208009068` both contain the WAVE PCM16 fixtures with one source frame,
source rate400 and requested rate96000. Their file sample is24000 in each
channel. The first decoded PCM16 output sample differs between the original
runs:

| Original run | Mono first sample | Stereo first sample |
|---|---:|---:|
| `40199000936` | 30 | 30 |
| `40208009068` | -4422 | 0 |

The recovered result begins with23399 after ordinary PCM encoding. That is
consistent with the original numerical kernel's first signed16 output23400,
followed by the established `truncate(sample / 32768 * 32767)` encoder step.
The differing original plugin samples alone demonstrate an unstable outcome;
they do not establish the cause without the instruction chain below. The
eight affected full-plugin records cover mono/stereo, PCM/XAS and RAM/streamed
output. Original complete transcripts and CustomData remain evidence; they
must not be replaced by a synthetic expected output or silently omitted.

The nearby two-source-frame fixtures use a different signal (constant8192),
so their success was not, by itself, proof of a special single-frame branch.
The import investigation independently establishes that branch.

## Original allocation and omitted-write chain

1. The WAVE read callback at `0x375e0` calls `0x26c10` at `0x37645`.
   `0x26c25..0x26c38` allocates `SSOUND.frameCount * 2` bytes for each
   signed16 channel and stores the returned pointer at `SSOUND+0x68+channel*4`.
   It does not initialize the sample payload.
2. The allocator `0x39630` calls the original `MSVCR80.dll!malloc` IAT entry
   `0xab180` at `0x39650`. It adds its own header/trailer guard words and
   returns allocation+8; it never clears the sample bytes between the guards.
3. At `0x377c1` the WAVE callback loads `SSOUND+0x218` (source frame count).
   It pushes that count at `0x377ce`, then pushes zero as the starting frame
   at `0x377d2`, before calling importer dispatcher `0x404e0` at `0x377df`.
   These are dispatcher arguments5=count and4=start.
4. `0x4050a..0x40518` computes `start + count - 1` in EBP. At `0x4051a`,
   only a positive result continues into seeking and conversion. Otherwise
   `0x4051c..0x40523` immediately returns success1. Thus start0/count1
   returns before any source seek, read, or destination write. Its newly
   allocated planar sample remains uninitialized.
5. On the ordinary PCM16 path, dispatch to the real reader `0x3afa0`
   happens only later at `0x40608..0x4061a`. The early return bypasses it.
   The original filter120 subsequently consumes the planar sample pointer;
   its source is therefore allocator residue, not the file's24000 sample.

A direct execution check using the pinned original-machine-code harness set
only the dispatcher's already-initialized flag (`RVA0x12a528`) and invoked
`0x404e0` with start0/count1 and a prefilled64-byte destination guard. It
returned EAX1 and left all64 destination bytes unchanged. This is a targeted
confirmation of the early-return path; it is not full-plugin or recovered
candidate acceptance. The static branch is before the format dispatch and is
shared by the generic import dispatcher; AIFF single-frame input must not be
presumed safe merely because the observed failing fixtures were WAVE16.

`SIMEX_filterssound` at `0x29fb0` merely dispatches to the registered callback;
it has no single-frame bypass that would explain the phenomenon. The WAVE
metadata parser likewise computes the ordinary frame count from its data
length and channel/width fields without padding a one-frame file into two.

## Numerical kernel versus file-import policy

Direct original `0x42480` execution with an explicitly initialized signed16
source `[24000]` and the actual400→96000 profile produces240 frames, beginning
`23400,23399,23396,23392,23386,23378,23368,23357`. Its complete little-endian
signed16 output SHA256 is
`f59c83c3baf9136e4e4a3d3e71422cdbec316fbc1261abfc719dbea485e22756`.
This is original kernel evidence with known input memory. Existing standalone
kernel differential coverage also includes initialized one-frame inputs near
ordinary rate ratios. A deterministic initialized source and an uninitialized
file-import allocation are distinct input contracts.

The recovered file route therefore explicitly rejects **a changed requested
rate when the imported source has exactly one frame**. It does not attempt to
reproduce allocator residue, replace it with zero, or invent a padding rule.
Unchanged-rate handling and the standalone numerical kernel remain separate.
This refusal is a declared candidate safety policy, not a claim that the
original DLL rejects the file or a claim of byte equivalence in that domain.

The full-plugin acceptance is reported as448 successful byte-for-byte
comparisons plus8 explicit safety refusals, with all8 original output records
retained. Those counts become acceptance only after the corresponding final
sourceCommit/runId receipt actually passes; this investigation document does
not certify a run or convert an earlier failure into success.

## Final plugin acceptance

Source `684e3310004853455dd0b5b4e53287afb067e93c`, run40209451010
attempt1: Debug/Release each448 defined resampling records are byte-identical,
and each8 single-frame candidate errors have the exact required type/message
and no CustomData. Every original single-frame record remains in the full
transcript and in `original-single-frame-evidence.txt`. No original buffer
was silently discarded or represented as a successful candidate encoding.
