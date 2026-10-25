# AIFC/ADP4 raw-nibble boundary

Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses are RVAs. The standalone decoder and original-only plugin
probe and AudioFile ADP4 component/plugin route are accepted at the exact
batches below. SDK integration is deliberately deferred to a larger milestone. No Apple
ima4 packet decoder is implied.

## Actual original decoder chain

AIFC identifier accepts compression ADP4 at `38171..38182`.
The importer switches to representation13 at `38485`.
`404e0` dispatches representation13 via `40727..4076d` to:

| Channels | Native reader | Actual nibble order |
|---|---|---|
| mono | `3b970..3baa7` | high then low;1024-frame read chunks |
| stereo | `3bab0..3bbf9` | high=left,low=right;512-frame read chunks |

These readers consume raw nibbles, with no34-byte Apple ima4 packet header.
Both invoke the closed integer decoder `48500..48584`: predictor at state+0,
step index at+18,89-entry step table at fe340,8-entry adjustment table at fe4a4.
All other state bytes remain untouched by that leaf. It clamps predictor to
signed16 and index0..88; only the low four nibble bits affect the result.

**Importer caveat:** `388f8..38949` still checks declared frame count against
raw SSND length using COMM container bits/8, even when representation13 was
selected. Do not substitute a standard compressed-size formula or assume the
whole physical nibble payload contributes decoded frames.
The shared `404e0` start0/count1 early return also applies to representation13:
file-imported single-frame data remains uninitialized. A future ADP4 file
route must refuse that boundary rather than treating the standalone explicitly
initialized one-frame decode as proof of native file import behavior.

## Original-only plugin probe

`TestAudioAdp4Probe.ps1` creates24 AIFC/ADP4 inputs from mono/stereo,
7/129/2041 declared frames and four byte patterns, then requests PC NONE/XAS,
RAM/streamed:96 complete outputs. Files contain raw legal nibbles, no packet
headers; physical payload length is deliberately channels*2*declaredFrames
with COMM bits16 so the original importer's own count check is satisfied.
This is an explicitly constructed original input domain, not a claim that the
files conform to the Apple ima4 packet specification.

Two independent original processes must produce identical full transcripts.
`frame-counts.tsv` records label/channels/declaredFrames/physicalPayloadBytes/
returnedRuntimeFrames. Source files, exact outputs and original input SHA are
saved under `evidence/audio/adp4-probe/<runId>-<attempt>`.

## Standalone decoder component

`Native/Adp4Decoder.cs` recovers the integer leaf and provides a bounded
zero-initialized raw mono/stereo sequence API. Its original accepted batch
was standalone; subsequent AudioFile wiring requires the separate batch below.
`TestAudioAdp4Decoder.ps1` first runs the real original x86 CPU leaf, then
Debug/Release candidates in original-DLL-free hosts:

-18067 scalar cases:7 signed16 predictor boundaries,all89 indices and29
 nibble encodings including sign-extended high nibbles and ignored high bits;
-176 explicitly initialized stream sequences across mono/stereo,11 frame
 counts through2041,offset0/3,and four patterns; source bytes remain immutable;
-11 separate candidate safety refusals for invalid state/source/bounds.

Native sequence expectations call the original leaf for each nibble using the
separately audited mono/stereo nibble order; they are **not direct native file
reader calls**. The raw sequence API starts predictor/index at0 by contract. A subsequent
static audit traced AIFC read wrapper `389a0..38bbb`: it calls `26c90` at
`38a0c`, whose allocation initialization sets predictor+0 to0 at `26cc8`
and index+18 to0 at `26cca`. This establishes the original zero initial-state
instruction chain; original-only repeated-process plugin probes still guard
against overlooked import state and frame-count behavior.

Local original-machine-code Unicorn execution matched all18067 scalar cases;
that integer-only quick check does not replace real CPU/C# acceptance.
Accepted source `579c95661d90f067ab414c4133f033c2571b96e1`, run
[40233712043](https://github.com/wu162/NewSdk/actions/runs/40233712043),
attempt1: real CPU and Debug/Release18243 exact records,11 safety refusals;
original plugin96 records across two independent processes were identical.
Receipts are `adp4-decoder-validation/40233712043-1` and
`adp4-probe/40233712043-1`. The96 `frame-counts.tsv` rows confirm runtime
frame count from rawSSND/(channels*2) for the original bits16 probe domain.
Those receipts do not prove the later candidate file route or SDK.

## Accepted file-import component batch

The candidate reader selects raw ADP4 for FORM/AIFC COMM typeADP4, normalizes
its decoded signed16 samples by2^-15 and preserves the original frame-count
correction from raw SSND and COMM bits8/16/24. It requires mono/stereo and
at least two imported frames;0-frame and1-frame sources are separate refusals.
This initial file-import batch used SSND blockSize0. Arbitrary ignored
blockSize was subsequently accepted in [SsndBlockSize](SsndBlockSize.md),
source `d3f96009e64b44c4d9482421cc5922b3309320bc` / run40236037500.
Loop metadata and nonzero SSND offset remain outside this ADP4 input domain.

`TestAudioAdp4Plugin.ps1` now runs2136 PC full-plugin records in each original,
Debug and Release host:4032 ordinary cases (3 widths*2 channels*7 frame counts*
4 patterns*3 COMM declarations),144 partial-SSND-length cases and48 actual
filter120 sample-rate conversions. It separately checks144 exact normalized
sample/frame invariants and12 empty/single-frame safety refusals.
The frame boundaries2/7/129/1023/1024/1025/2041 cross both original reader
chunk lengths; COMM declarations are actual/zero/actual+99. SSND tails include
-1,+1,+frameSize+1 byte to verify floor division and padding exclusion.

Affected old inputs must also pass `TestAudioPlugin.ps1 -Groups formats,aiffloops`
and `TestAudioAifc.ps1 -SkipSdk`:2400 formats,200 AIFF loops and1440 AIFC/NONE
records in Debug/Release. The AIFC safety case now explicitly rejects undefined
ADP4 single-frame import rather than treating all ADP4 as unsupported.
No script in this new batch triggers real SDK; report component/plugin status
and **SDK not validated for ADP4** until a later accumulated integration run.

Source `5495da2d703ca429434de369f5414cd45e4de819`, run
[40234514000](https://github.com/wu162/NewSdk/actions/runs/40234514000),
attempt1: Debug/Release each4224 exact ADP4 plugin records,144 sample/frame
invariants and12 safety refusals passed. Affected formats2400,AIFF loops200,
and AIFC/NONE1440 regressions passed in both configurations. Receipts are
`adp4-plugin-validation/40234514000-1`, `plugin-validation/40234514000-1`
and `aifc-validation/40234514000-1`; SDK was not run.
The overall workflow failure was a separate out-of-scope snapshot tool
export-name mismatch; it does not change these input-scope successful receipts. Do not
label the entire multi-scope workflow successful or claim ADP4 SDK acceptance.
