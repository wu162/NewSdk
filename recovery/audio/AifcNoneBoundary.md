# AIFC/NONE integral input recovery batch

This batch targets FORM/AIFC with compression type `NONE` only. It does not
recover ADP4, little-endian sowt, float formats or additional loop metadata.
All addresses below are RVAs in original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.

## Original instruction contract

- Identifier `380c3..381a9` recognizes FORM/AIFC and scans COMM. At `38171`
  it compares compression type at COMM body+18 with ADP4, then at `38188`
  with NONE. It does not select a general-purpose decoder by FourCC.
- Open callback `382fb..38310` sets its AIFC flag. COMM parsing at
  `3836d..3845e` uses the same BE channel/count/rate/width fields as AIFF;
  signed8/BE16/BE24 map to representation2/1/21.
- `3845e..3848c` changes that representation to13 only for ADP4. NONE keeps
  the integral interpretation. Compression-name Pascal length/content and
  trailing COMM data are not interpreted by the callback.
- FVER has no special branch in `38354..38625`; the generic chunk traversal
  skips it. SSND count correction remains `388f8..38949`, using the raw
  SSND length without even-chunk padding.

This initial candidate batch required the complete fixed22-byte AIFC COMM
prefix and NONE compression. It ignored compressionName/FVER within the
no-loop, SSND offset0/blockSize0, integral mono/stereo domain. Later ADP4
and arbitrary ignored blockSize support are separately recorded below.
No original acceptance is inferred from the file-format specification.

## Reproducible original-first matrix

`TestAudioAifc.ps1 -OriginalOnly` collects720 full original PC plugin records,
requiring each to produce a buffer and CustomData. Ten variants are crossed
with PCM8/16/24, mono/stereo,7/129/2041 frames and all four PC
codec/storage combinations:

- COMM22 minimum, empty compression name, odd/even/255-byte names,
  extra trailing name data;
- FVER before/after COMM, unknown FVER value and SSND-before-COMM order.

The raw outputs and source files are preserved at
`evidence/audio/aifc-validation/<runId>-<attempt>`. Only a successful original
run establishes that these constructed inputs are accepted by the original.
The complete script then compares Debug/Release exact plugin transcripts
without the original DLL in recovered hosts,60 decoded sample-bit invariants,
10 separate candidate safety refusals and the dedicated SDK profile.

`TestAudioSdk.ps1 -AifcOnly` creates120 XSD-valid PC assets from3 widths,
2 channels and5 variants (minimum COMM,255-byte name, extra name data,
unknown FVER, SSND-first), each NONE/XAS and RAM/streamed. It requires120
actual calls per side, four MOD products and120 CustomData files equal,
with the original AudioCompiler DLL absent from the recovered SDK.
The old input consumer regression is `TestAudioPlugin.ps1 -Groups
formats,aiffloops`: all1200 PC WAVE/AIFF/extensible and200 AIFF-loop records.
The numerical encoders/resampler, MP3 and PathMusic logic are unchanged.

## Accepted batch

Source `3863a632faac79ef586569e134b7eb46014fab94`, run
[40233228823](https://github.com/wu162/NewSdk/actions/runs/40233228823),
attempt1 succeeded: Debug/Release each1440 exact AIFC plugin records,
60 sample-bit invariants and10 separate safety refusals; old input regression
formats2400 and aiffloops200 passed. Dedicated SDK120 actual calls per side,
four MOD products and120 CustomData files matched; recovered SDK had no
original AudioCompiler DLL. Receipts are `evidence/audio/aifc-validation/40233228823-1`,
`plugin-validation/40233228823-1` and `sdk-validation/40233228823-1`.
The SDK profile is dedicated AIFC/NONE input, not full Upheaval.

The earlier original-only1440 probe at source
`5d56d52ff11510c7e93f83b47d08b1def5c99d27`, run40232917060, remains an
original-only receipt; do not relabel it candidate or SDK acceptance.
Subsequent [ADP4 component/plugin acceptance](Adp4Boundary.md) and
[arbitrary ignored SSND blockSize acceptance](SsndBlockSize.md) have their own
sourceCommit/runId scopes and no new SDK run. The original SDK receipt above
continues to prove only its exact AIFC/NONE profile. Extra AIFC loop metadata
and nonzero SSND offset remain outside the current accepted input domain.
