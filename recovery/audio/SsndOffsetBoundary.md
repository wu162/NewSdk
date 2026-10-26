# SSND offset: bounded native-compatible input window

Original DLL SHA256:
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
PC / Win32 only. Original-first evidence passed at source
`49b1842e85bef9f7b06c74df5dc4a32781a4413a`, run
[40242356374](https://github.com/wu162/NewSdk/actions/runs/40242356374):
2704 complete records and1352 shifted-vs-zero comparisons. Its precise
`ssnd-offset-validation/40242356374-1/validation.json` status is
`original-evidence-only`, with no candidate configurations and SDK false.
The overall run failed in an unrelated MPEG fixture tool; this is a
successful offset substep, not an overall run success. Candidate validation
is still pending until a separate Debug/Release receipt exists.

## Original instruction contract

The native importer retains raw SSND size at385ea..385f9. At388f8..38949 it
corrects COMM's frame count to
`floor((rawSsndSize - 8) / (channels * (containerBits / 8)))`.
The SSND reader389a0..38bbb reads its eight-byte prefix and adds the first
BE32 word (offset) to the absolute source position at38b26..38b46. It forwards
the previously corrected count to the common reader at38b70..38b88 without
subtracting offset. The second word(blockSize) remains ignored. Source:
[ssnd-offset-disassembly.txt](ssnd-offset-disassembly.txt), with frame-count
instructions in[pcm-input-disassembly.txt](pcm-input-disassembly.txt).

Subtracting offset before calculating frames would implement different AIFF
semantics. Blindly preserving native reads can instead consume padding,
following chunk headers or uninitialized bytes after EOF.

## Deterministic domain

Let L=rawSsndSize-8, C=channels, W=containerBits/8, F=floor(L/(C*W)).
Offset does not change F.

- Integral AIFF/AIFC NONE consumes F*C*W bytes starting at offset.
- EA raw ADP4 consumes ceil(F*C/2) bytes, using recovered mono high/low or
  stereo high-left/low-right nibble order. COMM container width still controls
  F. This is not Apple ima4 packet decoding.
- Require offset+consumed<=L in widened arithmetic before casting offset to
  int or allocating samples. Pad bytes and following chunks cannot satisfy
  this bound, even if physically present and initialized.
- Retain all other format, frame-count, loop and single-frame safety checks.
  Offset changes the source byte origin, not decoded loop frame numbering.

For PCM, nonzero offsets fit only in L's remainder modulo frame size; mono
PCM8 has no safe nonzero offset. ADP4 permits larger offsets because nibble
consumption is smaller than the container-width-derived frame count.
Out-of-chunk sample windows stay explicitly unsupported. No zero filling,
clamping or guessed frame truncation is used.

## Complete component/plugin batch

`TestAudioAiffOffset.ps1 -OriginalOnly` collects original plugin evidence;
full mode then builds Debug/Release recovered hosts without the original
AudioCompiler DLL. `AudioAiffOffsetCases` covers AIFF PCM, AIFC NONE,
AIFC ADP4 and existing AIFF forward sustain loops; widths8/16/24,
mono/stereo;7/129/2041 frames and4096 for loops. Cases use a partial-frame
tail, blockSize0/ffffffff, and zero/first/intermediate/maximum safe offsets.
Each has an offset0 companion with the consumed window shifted left while
preserving SSND length/metadata.

Every host must produce1352 exact shifted-vs-zero three-buffer/CustomData
comparisons and2704 complete plugin records across NONE/XAS RAM/streamed.
Original imports must succeed with output; matching exceptions cannot pass.
Fixed subtitles prevent filenames from affecting buffers. Recovered and
original complete transcripts must also match byte for byte.

The reader oracle requires338 sample/frame/channel/rate/immutability checks
and480 explicit refusals: one byte beyond maximum, offset at payload end,
uint.MaxValue, and a valid initialized following JUNK chunk that must not
become sound. The old SSND blockSize oracle retains168 refusals using clear
out-of-chunk offsets, rather than incorrectly rejecting every ADP4 offset1.

The caller must run affected existing format/AIFC/ADP4/SSND regressions because
the common input reader changes. Numerical kernels do not change. This does
not establish production SDK/full Upheaval acceptance, and adds no non-PC
support or additional loop/MPEG metadata semantics.

## Native ADP4 byte-consumption proof

`GenerateAudioAiffOffsetReadOracle.py` executes the original0x3b970 mono
and0x3bab0 stereo stream readers, retaining the original0x48500 nibble
decoder, with Unicorn2.1.4. Only the file-read callback is modeled; it supplies
exactly the requested initialized bytes. For72 cases (2 channels options,
12 frame counts spanning the512/1024 block boundaries,3 byte patterns),
the total is ceil(F/2) mono or F stereo bytes, with read blocks no larger
than512. Native sample-byte loads are checked to stay inside the initialized
read buffer, all2*F*C output bytes must be written, and planar output guards
and cdecl return-stack balance are checked. The extraction passed locally;
[original read requests](../evidence/audio/native-ssnd-offset/original-adp4-read-requests.tsv)
preserve the original fingerprint and raw request sequences. This machine-code
evidence substantiates the byte bound; it does not substitute for the real
Windows plugin or SDK batch.
