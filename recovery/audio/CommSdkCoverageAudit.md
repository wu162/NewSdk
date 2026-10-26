# COMM / SSND / loop coverage audit

Scope: fixture sources at `bb4a225`, not a new validation receipt. The4488
asset SDK batch running at that source must retain its own sourceCommit/runId.
This audit classifies whether the later **selected-marker signed COMM range
filter** changes those inputs. It does not extend the batch's coverage to
new marker/declaration combinations or claim that later code was rerun.

## SDK4488 classification

| Generated group | Assets | Source declaration / SSND frames | MARK behavior |
|---|---:|---|---|
| ADP4 lengths/patterns |432|declared F+99; actual floor((F*frameSize+delta)/frameSize)|No INST/MARK|
| ADP4 requested rates |48|declared228, actual129|No INST/MARK|
| SSND kinds0/1/2 |288|declared=actual129|No INST/MARK|
| SSND kind3 |96|declared=actual4096|Selected129/4095 valid|
| Boundary WAVE metadata + rates |256|No COMM|WAVE smpl, unrelated to AIFF filtering|
| Boundary AIFF/AIFC/ADP4 metadata + rates |1152|declared=actual257|All selected positions within declaration|
| Optional metadata + rates |864|declared=actual257|INST-only288; other MARK-bearing576|
| Boundary offsets kinds0/1/2 |1240|declared=actual F, partial-frame tail does not add a frame|No INST/MARK|
| Boundary offsets kind3 |112|declared=actual4096, including nonzero offsets|Selected129/4095 valid|
| Total |4488|256 WAVE;480 mismatched no-MARK;3752 equal COMM/SSND|1936 AIFF-family assets contain MARK|

Of the optional864 assets,192 (`aux-outside`/`aux-negative`, including
requested rates) carry out-of-range/negative **unselected IDs99/100**.
Selected IDs7/11 remain17/223. Their previous bb4a225 implementation already
ignored auxiliary positions. These cases do not test skipping an out-of-range
selected duplicate. Other optional cases have no selected pair, empty MARK,
or no MARK. Resampling occurs after metadata resolution; requested output
rates do not turn these into mismatched source-COMM fixtures.

For every selected marker in this batch, eligibility remains unchanged if
only the native signed declaration predicate is added. No existing4488
fixture selects a positive position above COMM or a negative position, and
no MARK-bearing fixture has COMM different from SSND-derived frame count.
Consequently the batch is unaffected by that narrow predicate change, but
its receipt cannot be relabeled as SDK coverage for the new declaration domain.

Sources: `AudioPcInputSdkCases` (4488) and `AudioPcBoundarySdkCases` (3624),
plus their exact original fixture writers. Fixed source blobs checked:

| File in recovery/tests | bb4a225 blob |
|---|---|
| AudioPcInputSdkCases.cs |bb28bde6797bdf630e5b05ba85a9367b5ccac17d|
| AudioPcBoundarySdkCases.cs |41169904d03cca3c12815196f4eae9f2ead079fd|
| AudioLoopMetadataCases.cs |f8d6dcd7650d41cac80faa87e1afe7ec4cb552be|
| AudioLoopMetadataOptionalCases.cs |6b64e3221f8458b4c2a3bb2c770b7b91a5cbaaa4|
| AudioAiffOffsetCases.cs |4210f12c73f8bd40e84430bd2102b316ce42f7d6|
| AudioAiffLoopCases.cs |bd9ae8fc5cabdcd2f437dc496d4ab595b326194f|
| AudioAiffSsndCases.cs |360614c1c9782881ad228cf9e0054a7fe1e86406|

## Offset component and additional SDK needs

The2704-record offset component is2480 no-loop records plus224 loop records
(including canonical companions). All use COMM equal to SSND-derived frames.
The224 loop records and112 actual SDK offset-loop assets use COMM4096;
appending frameSize-1 tail bytes preserves actual4096. Offset changes only
byte origin, never loop frame numbering or marker eligibility. Their selected
positions remain129/4095. No offset fixture exercises the new COMM boundary.

Existing old `AudioAiffLoopCases` includes one selected end-marker4096 equal
to COMM4096, so the full plugin regression already protects the inclusive
upper bound. SDK4488's AIFF selected markers do not reach their COMM endpoint.

New SDK coverage must explicitly exercise the new accepted source domain:
COMM below SSND actual frames, zero/negative signed declarations, out-of-range
selected duplicates that must not overwrite valid earlier positions, and
selected positions exactly equal to COMM. Use bounded initialized original
cases with complete selected pairs or the established initialized no-loop
domain; partial pairs or output ranges reading unwritten memory must not be
turned into supported fixtures. If offset composition is claimed for the new
domain, include a bounded offset loop whose selected duplicates cross COMM
while SSND frame count remains unchanged. Do not repeat all4488 unchanged
assets merely to add these samples; preserve exact old anchors and perform
one appropriate new integration milestone after component acceptance.

## Fixed complete Upheaval input

The `068643219fded09a05fb7b75c6507b769c430316` Audio tree is
`5e404d23791791d6026d7209486e181cfd6c969f`, identical to the Audio tree at
`bacfc69fc3e71c8116d1f22b933b8b6173170ecf` referenced by the existing
`fixtures/audio/main/provenance.json`. Its1289 media paths all end in.wav.
The14 dedicated AudioFile.xml files contain1289 AudioFile declarations,
all with.wav File attributes; none declares AIFF/AIFC input. No whole-media
download was performed for this audit.

The existing provenance's single1289-entry format bucket is
`(1, 1, 22050, 8, False)`. Both retained real fixtures are RIFF/WAVE with only
fmt/data chunks, and both blob IDs match the fixed tree:
`real-0.wav`891a6beaac5ea0b5bd74e77905a1d2c68f4d4e40;
`real-1.wav`2d52c8cad34c6d80d194a47afc5bda4af7570d5e.
Thus this fixed real-audio input supplies no AIFF COMM-boundary coverage;
a complete Upheaval success cannot substitute for the dedicated new AIFF
integration cases. Actual whole-MOD handler traces remain the runtime coverage
record, independently of this static classification.
