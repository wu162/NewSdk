# PC MPEG acceptance boundary

Scope: `AudioFileMP3Passthrough`, Win32 only. Production uses the recovered
header scanner followed by statically included NLayer decoding; it never loads
the original AudioCompiler as a runtime backend. Accepted input is copied
verbatim, including metadata and trailing bytes.

## Deterministic original header contract

Original DLL SHA256:
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.

`GenerateAudioMpegHeaderOracle.py` executes the original pure x86 header routine
at RVA `0x44d40` in Unicorn. Each call receives an initialized 32-byte SINFO
record, a valid stack and x87 control word `0x037f`; no file importer, allocator
or uninitialized memory is involved. The generator verifies the DLL fingerprint,
preferred base and normal return. Its 32,783 records cover the complete
version/layer/bitrate/frequency/mode/padding product with four ignored-field
patterns, eleven individual sync-bit failures and four sentinel headers.
Expected payload lengths and channel mutations come from original instructions,
not a second bitrate formula.

- Reserved version/layer/bitrate/frequency fields return zero. Channel metadata
  is written only after the earlier validation branches succeed.
- Free-format bitrate0 returns zero **after** channel metadata changes. This
  mutation is intentional, rather than a recovered parser cleanup opportunity.
- The identifier at RVA `0x2f6f0` requires seven successive parsable headers.
  The seventh header needs four bytes; its payload need not fit during
  identification. Fifteen candidate tests cover zero/one/six/seven/eight headers
  with three prefix lengths. Identification and decoding acceptance are separate.

The original machine-code generator completed locally with all 32,783 records;
a local formula cross-check found no difference. Actual Debug/Release candidate
assembly acceptance requires the exact Actions receipt, rather than that local
formula check.

## Entire-scan EOF termination

Original RVA `0x2f6f0` reads four bytes at each successive candidate-frame
position. At `0x2f7ab`, a short read sets a flag which terminates the **entire**
scanner at `0x2f7bf..0x2f7c3`, rather than merely discarding that candidate. An
earlier plausible frame whose declared span seeks beyond EOF therefore prevents
discovery of a later seven-header chain. For example, prefix `ffe4e800` describes
a 2,880-byte MPEG2.5 LayerII frame; prepending it to the 1,462-byte mono fixture
makes the original identifier return -1 even though a complete valid stream
begins at offset4.

The generator now executes that original scanner with hooks implementing only
bounded-file tell/read/absolute-seek and the four-byte endian helper. It never
hooks the MPEG parser or scanner branch decisions. Its 441 independent scanner
records cover seven-header thresholds, partial final payloads, three prefix
lengths and 132 preceding-header variants. Local replay found 54 mismatches in
the previous recovered scanner and none after preserving whole-scan EOF exit.
The fix changes `FindFirstFrame` only; NLayer decoding is unchanged.

## Decoder acceptance probe

`AudioMpegBoundaryOracle.cs` creates 119 fixtures from existing mono MPEG1 and
stereo MPEG2 Layer3 fixtures: plain files; junk/ID3 prefixes and suffixes;
one/six/seven/eight-frame boundaries with missing or partial final payload;
broken synchronization; zero/255 payload; enclosing-format signatures;
concatenated channel layouts; empty/junk input; and invalid final headers with
both channel modes. Twenty-four long-frame prefix cases exercise whole-scan EOF
termination before later valid audio. The final-header group distinguishes
metadata mutation from decode success.

Each fixture exercises Win32 `ProcessInstance` and the direct handler with both
streaming flags (476 complete records). Tests record all three buffers, error
type/message and CustomData. Accepted assets must copy the entire input exactly;
the two plain valid controls must succeed. Each fixture runs in a separate x86
process with a 15-second bound, making native crash/timeout an explicit case
result. Recovered hosts exclude the original DLL and verify that it was not
loaded.

`TestAudioMpegBoundary.ps1 -OriginalOnly` collects original behavior without
claiming candidate equivalence. Full mode also generates the native header
matrix, tests actual Debug/Release candidate assemblies and compares complete
per-case transcripts. Evidence directory:
`recovery/evidence/audio/mpeg-boundary-validation/<runId>-<attempt>/`.

Exploratory cases must be assessed by coherent semantic groups before expanding
support. Ordinary MP3 success does not imply universal damaged/special MPEG
equivalence. Undefined native memory behavior is not a compatibility target.
This probe does not establish SDK or complete Upheaval integration.
