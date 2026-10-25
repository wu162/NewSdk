# PC MPEG acceptance boundary

Scope: `AudioFileMP3Passthrough`, Win32 only. Production recovers identification
and `SIMEX_info` header metadata, without decoding MPEG samples. It never loads
the original AudioCompiler as a runtime backend. Accepted input is copied
verbatim, including metadata and trailing bytes. The unused NLayer source
dependency has been removed.

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
  with three prefix lengths. Identification and header metadata extraction are separate.

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
The EOF fix changes `FindFirstFrame`; the separate info-contract correction is described below.

## SIMEX_info does not decode the payload

The native method metadata maps `SIMEX_info` to RVA `0x25490`. That dispatcher
uses `0xfd16c + 34*40 = 0xfd6bc`, whose stored function pointer is
`0x1002f8c0`, for MPEG format34. The entire `0x2f8c0..0x2fa7d` callback allocates
SINFO, runs the seven-header identifier and scans successive headers, seeking
over each declared payload. It accumulates frame sample counts, writes the last
parser channel/rate fields and returns1. It does not call a PCM decoder.

The format34 sample-read callback is a separate pointer at `0xfd6c0`, pointing
to `0x2fa80`. The error text "Error decoding MPEG Layer 123 data." belongs to
`0x3bc00`, reached from the sample-conversion dispatcher at `0x40809`, rather
than the passthrough's info callback. Exact dispatch values and complete relevant
function disassemblies are preserved in [mpeg-info-disassembly.txt](mpeg-info-disassembly.txt).

The generator executes the actual public `0x25490` dispatch, native info callback
and original SINFO constructors on 324 initialized cases: two channel modes,
three frame counts, nine constant/seeded payload patterns, two prefix lengths
and three final-payload lengths. Only bounded file I/O, allocation and memset
are hooked. Entering a PCM decoder/sample-read path is a hard failure. All 324
original calls completed successfully locally, including all-255 payloads.

Run `40242571724`, source `40ebb670`, independently exposed the prior mismatch:
all-255 payloads were accepted by the original and rejected by NLayer; five
magic-only prefixes were also accepted by the original. The recovery now uses
the original header-info contract instead of imposing full-stream PCM decoding.
This is not a payload-specific exception or a replacement MPEG decoder.

## Enclosing importer priority

The WAVE identifier at `0x370d0` requires RIFF/WAVE plus a `fmt ` prefix with
format1 or `0xfffe` before returning score100. Its chunk skip uses raw byte
counts. The AIFF/AIFC identifier at `0x38010` requires at least50 file bytes;
FORM/AIFF returns score100, while AIFC additionally requires a COMM compression
of NONE or ADP4 and rounds skipped chunk sizes to even lengths. These bounded
recognizers outrank score75 raw MPEG. A magic string alone does not suffice.
OggS/fLaC/SCHl prefix-only rejection had no original-code evidence and is removed.

The bounded partial-metadata extension follows actual read dependencies: WAVE
needs the chunk header and two initialized format bytes (10 bytes total), not
all24 bytes requested by the identifier. AIFC needs22 initialized COMM body bytes,
not the complete declared length. The generator executes both original driver
identifiers on72 full/partial-format cases, twice with distinct stack fill bytes
(0 and0xa5), and requires identical decisions across stack fills. Its expected
score100/score0 decisions verify the recovered bounded priority. An additional
102 plugin fixtures cover partial WAVE format1/EXTENSIBLE/float/MPEG with declared
16/32-byte bodies and actual2/3/15-byte bodies, plus AIFC NONE/ADP4/sowt with
22/24/64-byte declarations and22/23/24 actual bytes. Earlier JUNK chunks contain
complete MPEG streams, so incorrect container rejection/acceptance is visible.

The recovered recognizers never read beyond the available file. Truncated
enclosing metadata that makes the original examine uninitialized bytes remains
outside the deterministic contract; universal malformed-container equivalence
is not claimed.

## Decoder acceptance probe

`AudioMpegBoundaryOracle.cs` creates 271 fixtures from existing mono MPEG1 and
stereo MPEG2 Layer3 fixtures: plain files; junk/ID3 prefixes and suffixes;
one/six/seven/eight-frame boundaries with missing or partial final payload;
broken synchronization; five constant and four seeded payload patterns; enclosing-format signatures;
concatenated channel layouts; empty/junk input; and invalid final headers with
both channel modes. Twenty-four long-frame prefix cases exercise whole-scan EOF
termination before later valid audio. The final-header group distinguishes
metadata mutation from identification. Thirty-six additional enclosing cases
exercise WAVE format1/EXTENSIBLE/float/MPEG, odd skipped chunks, RIFF other type,
AIFF type priority, AIFC NONE/ADP4/sowt, skipped odd chunks and absent fmt/COMM.

Each fixture exercises Win32 `ProcessInstance` and the direct handler with both
streaming flags (1,084 complete records). Tests record all three buffers, error
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

The header-info contract is independent of MPEG sample decoding. Universal
malformed-container/native-memory behavior equivalence is not claimed. Undefined
native memory behavior is not a compatibility target. These component probes
do not establish SDK or complete Upheaval integration.
