# AIFC/ADP4 raw-nibble boundary

Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses are RVAs. The AudioFile reader continues to reject ADP4 until
its complete import/frame-count/initialization behavior has been established
and plugin/SDK acceptance obtained. No Apple ima4 packet decoder is implied.

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
zero-initialized raw mono/stereo sequence API. It is not wired to AudioFile.
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
Use actual `sourceCommit/runId` receipts before declaring this component
accepted. Production import integration and SDK acceptance remain separate.
