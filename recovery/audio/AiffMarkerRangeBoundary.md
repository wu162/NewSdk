# Signed COMM/marker range filtering

2026-10-26. Candidate correction, pending its complete original/recovered
matrix. Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`;
all addresses are RVAs. [Instruction evidence](aiff-marker-range-disassembly.txt).

## Original contract and defect corrected

The AIFF/AIFC importer scans all chunks before resolving any MARK group.
Every COMM sets `SSOUND+0x218` from body+2's four bytes at `0x383e1..0x38400`.
A later COMM replaces the earlier count, regardless of MARK/INST ordering.

During MARK resolution, `0x3874e` compares the32-bit position with that
COMM count using signed `jg`; `0x38765..0x38767` then rejects signed-negative
positions. Only a nonnegative position no greater than the signed COMM count
can reach mode/ID matching. Filtering precedes duplicate assignment, so an
out-of-range or negative later record cannot erase an earlier valid match.
A high-bit COMM count is signed-negative here and excludes every position.

Only afterwards, `0x388f8..0x38949` corrects the source frame count from the
raw SSND length. Therefore the accepted sample decoder's usual policy of
ignoring a mismatched COMM frame count must not be applied to marker filtering.
The previous candidate did exactly that: it could install a loop whose marker
positions were inside actual SSND samples but beyond the original COMM count.
This was a real accepted-input miscompilation, not merely missing validation.

The correction retains the last bounded COMM count as signed32, applies the
native signed position tests before each marker's ID match, and leaves valid
previous duplicates unchanged. Both selected IDs absent means the initialized
no-loop state. A partial pair remains a deliberate safety refusal; end-only
metadata changes under filter120 and start-only can drive the writer to a
zero source extent. MARK-only and malformed metadata refusals remain intact.
The same rules apply to AIFF PCM, AIFC/NONE and EA raw ADP4.

## Complete batch

`recovery/scripts/TestAudioAiffMarkerRange.ps1` supports `-OriginalOnly`.
Receipts use `evidence/audio/marker-range-validation/<runId>-<attempt>`.
No SDK invocation is made by this script.

The18 variants cover declared counts0/1/16/223/256/257/300/7fffffff/
80000000/ffffffff; later out-of-range/negative duplicate pairs; invalid pairs
before valid ones; zero and actual-source-end boundaries; and a final COMM
that overrides earlier metadata with zero, a valid count, or a negative count.
Each uses the same initialized sample payload and an independent canonical
input with the expected loop/no-loop outcome. No native fixture contains a
partial pair.

Across AIFF/AIFC NONE/raw ADP4, PCM8/16/24, mono/stereo, NONE/XAS,
RAM/streamed and unchanged/downsample11025/upsample44100 rates, each
configuration must produce7,776 full plugin records and3,888 independent
canonical comparisons. Required buffers and CustomData must actually exist;
matching failures cannot count as acceptance. The recovered host must omit
the original target DLL. The reader oracle adds648 expected-range checks and
72 independent partial-pair refusals, for720 assertions per configuration.

The prior4,224 metadata records and3,888 optional-metadata records must also
be rerun against the corrected source. Prior SDK receipts remain tied to their
own exact source commits and must not be relabeled as this correction's SDK
acceptance.
