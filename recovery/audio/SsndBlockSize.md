# SSND blockSize: ignored metadata component batch

Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses are RVAs. This batch adds arbitrary uint32 SSND blockSize to
the already supported AIFF/AIFC integral and raw-ADP4 input domains. Nonzero
SSND **offset remains explicitly refused**. No SDK is run for this batch.

## Original instruction contract

The AIFF/AIFC read wrapper `389a0..38bbb` scans for SSND, reads its first eight
body bytes at `38b0a..38b17`, then decodes only the first BE32 word(offset)
at `38b26..38b32`. It adds that offset to the current absolute file position
at `38b35..38b46`, builds the source-offset array and calls the common reader
`404e0` at `38b88`. The second body word(blockSize) is not read or interpreted.
The open callback's raw frame-count correction at `388f8..38949` also does
not use blockSize. Therefore it is an opaque ignored uint32, not a request
for block-aligned audio rearrangement.

Nonzero offset is different: the read wrapper skips it, while the importer
still derives frame count from the entire raw SSND payload length. This can
cause reads beyond the declared sound bytes. Do not implement a standard
AIFF offset interpretation or fit uninitialized samples to that behavior;
keep the refusal until a separate bounded original contract is established.

## Complete original-first plugin matrix

`TestAudioAiffSsnd.ps1` runs the original matrix first and requires every
record to produce buffers and CustomData. It then executes Debug/Release
candidates in original-DLL-free hosts. `AudioAiffSsndOracle` compares each
nonzero case against its matching zero-blockSize canonical **within the same
original or recovered host**, as well as the full original/candidate transcript.
An explicit fixed SubtitleStringName prevents different fixture filenames
from obscuring the exact canonical buffer comparison.

- AIFF PCM,AIFC NONE,AIFC ADP4:3 widths*2 channels*3 frame counts*
  7 block values*4 PC codec/storage combinations =1512 records.
- Existing AIFF forward sustain loops:3 widths*2 channels*7 block values*
  PC NONE/XAS RAM/streamed =168 records.
- Total1680 PC records,240 zero canonical rows and1440 nonzero-vs-zero exact
  comparisons per host. Values are0,1,2,4,512,4096,ffffffff.
- Separate reader oracle:168 exact decoded-sample/metadata invariants and
  168 explicit nonzero-offset refusals, covering all four input kinds.
- The affected old AIFF-loop200-record group runs with its full existing
  reader rejection checks. The two former nonzero-blockSize refusals are
  replaced with actual truncated SSND fixed-prefix refusals; nonzero offset
  checks remain. WAVE,MP3,FFT and other unchanged codec matrices are not rerun.

The new matrix includes each common SSND consumer's canonical0 behavior, so
the added metadata domain is tested alongside its known baseline, including
loops and ADP4. Numerical encoders, resampler and nibble decoder are unchanged.
Original runtime DWORD fields, full CustomData and all three buffers must
match; merely accepting the file cannot pass.

## Historical accepted component/plugin batch

Source `d3f96009e64b44c4d9482421cc5922b3309320bc`, run
[40236037500](https://github.com/wu162/NewSdk/actions/runs/40236037500),
attempt1 overall succeeded. Debug/Release each3192 exact plugin records,
2736 nonzero-vs-zero canonical comparisons,168 sample/metadata invariants
and168 nonzero-offset refusals passed; the affected AIFF-loop200-record
regression and existing reader safety checks also passed.
Exact receipts are `evidence/audio/ssnd-block-validation/40236037500-1/validation.json`
and `evidence/audio/plugin-validation/40236037500-1/validation.json`.
SDK was not run; this is component/plugin acceptance, not production SDK or
full MOD integration acceptance.
