# PC loop metadata acceptance

2026-10-26. Candidate batch on `recovery/audio-compiler-source`; **Windows
recovered differential pending**. Original-only evidence is accepted below. This document does not certify
SDK integration or claim pingpong/release playback support.

Original DLL SHA256:
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
Addresses below are RVAs (image base `0x10000000`). Original instruction
excerpts are in [loop-metadata-disassembly.txt](loop-metadata-disassembly.txt).

## Native selection contract

- WAVE chunk lookup `0x36ed0..0x3703a` seeks from file start and returns the
  first matching `smpl`. Import `0x37431..0x37445` tests whether loop count is
  positive; `0x37447..0x37483` reads only the first 24-byte record and copies
  its start and inclusive end. It never tests record type, ID, fractional
  offset, or play count. Additional records and later sampler chunks cannot
  override that first range. Consequently type1/type2 are accepted as the
  ordinary forward runtime range; this is **not alternating/reverse playback**.
- AIFF and AIFC use the same chunk scan. `0x384a6..0x384fc` reads each INST
  sustain mode/start-ID/end-ID into the same local storage. The last INST
  therefore selects the IDs before marker resolution begins. Release-loop
  fields are not read. Standard bounded 20-byte INST chunks remain required
  by the candidate; arbitrary INST lengths are not part of this batch.
- Every MARK group is retained in encounter order at `0x38516..0x38547`, then
  resolved at `0x3863b..0x388f2`. For mode1, each matching record overwrites
  `SSOUND+0x21c` (start) or `SSOUND+0x220` (end position minus one) at
  `0x38781..0x387cc`. Thus later matching records/groups win. Other sustain
  modes do not install a loop. Unrelated records remain auxiliary markers in
  the original; equivalence of their final encoded output must be proven by
  the plugin differential, not assumed from the parser alone.
- Native marker-group allocation is `0x324` bytes: one count plus 200 pointer
  slots. The candidate bounds each group and the group list to 200. Truncated
  records/names, excessive counts, trailing marker bytes, ambiguous IDs,
  missing instrument/marker chunks, incomplete selected pairs, and invalid
  loop ranges remain explicit refusals. Unsafe original memory behavior is
  outside the compatibility contract.

The importer validates loop metadata before PCM import even when that metadata
installs no loop. `AudioPcmInputReader` is then called with metadata enabled;
its standalone default continues rejecting unapproved loop chunks. This avoids
turning accepted mode0/mode2 or zero-loop `smpl` metadata into a second-stage
rejection.

## Complete differential batch

`recovery/scripts/TestAudioLoopMetadata.ps1` supports `-OriginalOnly` and
writes `evidence/audio/loop-metadata-validation/<runId>-<attempt>/validation.json`.
The default mode requires both original and recovered to emit a real buffer
and CustomData for every fixture, compares all three buffer arrays and full
CustomData, and checks the recovered host has no original target DLL.

`AudioLoopMetadataCases` generates 264 altered inputs and independent canonical
partners. Six width/channel combinations and four codec/storage combinations
are exercised at unchanged rate and 11025 Hz:

- WAVE: canonical, types1/2/ffffffff, multiple records, multiple chunks,
  first zero-count chunk followed by an active one, ignored sampler fields.
- AIFF, AIFC/NONE, raw ADP4: canonical, unrelated records before/after selected
  markers, duplicate start/end/both IDs, multiple MARK, multiple INST, ignored
  release metadata, sustain mode2/mode0, and MARK-before-INST group ordering.

The batch contains 4,224 plugin records per configuration and 2,112 independent
variant-versus-canonical comparisons per side. `AudioLoopMetadataReaderOracle`
adds 528 independent expected-range checks and four malformed/oversized-marker
refusals. Prior loop boundary tests are updated to assert the newly accepted
metadata behavior; their other structural, missing-marker, range and encoder
bounds assertions are retained. Existing loop/format/ADP4/SSND regressions are
still required for this shared importer change.

No SDK run is initiated by this script. Add these paths to an explicitly
selected SDK milestone only after the full affected plugin batch succeeds.

## Original-only evidence

Source `49b1842e85bef9f7b06c74df5dc4a32781a4413a`, run
[40242356374](https://github.com/wu162/NewSdk/actions/runs/40242356374), attempt1:
all 4,224 original plugin records emitted real buffers/CustomData and all
2,112 canonical-pair invariants passed. Receipt:
`evidence/audio/loop-metadata-validation/40242356374-1/validation.json`;
`original.log` ends with `PASS loop metadata complete plugin records: 4224`.
The overall workflow failed in a subsequent independent MPEG probe; this is
a successful original-loop evidence step, not overall workflow or recovered
acceptance. SDK was not run. The independent candidate reader assertions were
added after this original-only source and are not covered by that receipt.
