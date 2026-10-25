# Signal transform: 0x97b00

This closes the complete 459-byte, 165-instruction `97b00..97ccb` wrapper for the recovered table-backed DCT domain. It includes folding, DCT dispatch, leading/trailing band masks and the optional 256-float gain curve. It does not recover quantization, entropy coding or the public XMA encoding route.

## Native contract

`stdcall(core*, channel*, optionalShift*)`, `ret 0x0c`, HRESULT zero. The actual direct call is `8c0af -> 97b00`, returning at `8c0b4`. `core[0]` is the backend and `channel[0]` the channel parameters. The source at channel+48 contains 2N floats; channel+50 points to N output floats. Signed short parameters+7c and +7e are respectively folding and DCT lengths; the recovered domain requires both equal N in {64,128,256,512,1024,2048}.

| Location | Meaning |
|---|---|
| backend+3c | signed codec version |
| backend+10c / +110 | leading zero count / legacy upper limit |
| backend+1f0 / +204 | DCT / FFT callback (a81db / a7de3) |
| core+75d0 / +18c64 | tail flag / float comparison value |
| core+75d4 | retained fraction |
| core+18e24 / +18e30 | gain curve enabled / float[256] pointer |

No core, backend, parameters, source or gain-curve fields are written. The output and the optional shift destination are written; shift becomes zero. Parameters+78/+94 and backend+1b8/+1d0 are forwarded as arguments that the recovered DCT does not use.

## Numerical order

For h=N/2 and i in [0,h), fold[i]=F32(source[3h-1-i]+source[3h+i]), fold[h+i]=F32(source[N-1-i]-source[i]). DCT-IV runs in place with scale 1 and the original forward FFT callback. The leading mask writes positive zero to [0,leading).

The fraction product is rounded to binary32 before helper `79490` adds/subtracts 0.5 and helper `586a0` truncates to integer (half away from zero). For version <=2, the trailing cutoff is the minimum of that integer and the legacy upper limit. For newer versions, the upper limit starts at N; if the tail flag is nonzero and the comparison float is ordered and less than 1, it becomes N+trunc((-9*N)/100). In particular N=64 gives 59, not 58. `FLD1; FCOMP [core+18c64]; FNSTSW AX; TEST AH,41; JNE skip` excludes equality and unordered comparisons. The trailing mask writes positive zeros up to N. Finally the enabled gain curve multiplies each output by gain[floor(i*256/N)] and stores binary32, preserving signed zeros from the multiplication.

The managed API normalizes the flag/comparison pair to `reduceTail`. `Fold` is also exposed for checking captured DCT inputs. Physical binary32 stores use the same explicit store barrier as the accepted DCT/FFT. Caller input and weights remain unchanged even if a safe input check or a nonfinite intermediate is rejected.

## Validation

`TestAudioXmaSignal.ps1` builds only this component and its recovered DCT/FFT dependencies. The existing DCT/FFT matrices are not repeated. Its new 5,760 cases cover six sizes, four signed versions, ten fractions including half-integer rounding ties and adjacent floats, two tail policies, four gain curves (including negative and signed-zero values), three input patterns, varying leading/legacy masks. The original host calls actual original CPU code with original DCT/FFT; independent Debug and Release hosts require every spectrum bit, guards, HRESULT and shift to match and assert all native state/input/weights are unchanged. Thirteen candidate safety refusals are separate from equivalence acceptance.

The original DLL is pinned to SHA256 `a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`; original and candidate hosts require physical x87 CW `027f` and restore the previous word. Candidate hosts reject presence of the original module or file. Evidence lives under `xma-signal-validation` with source commit and Actions run identity. CPU acceptance remains pending until a successful receipt exists. Local independent original-instruction emulation/model checks passed 24 cases with the tail clamp active at fraction 1; this is preliminary checking, not CPU/C# acceptance.

The supported domain requires finite source and fraction, nonnegative fraction, leading count in [0,N], nonnegative legacy limit when version <=2 (newer versions ignore it), and finite 256-entry weights when enabled. Unrepresentable integer cutoff and nonfinite DCT intermediates are explicit safe refusals, not claims about unsafe original behavior.

Actual capture replay accepts `-SnapshotsDirectory .../signals`. Each record verifies input/output/optional weights and the 12-byte argument/96-byte raw-fields fingerprints; all masks, lengths, callback targets and optional shift are cross-checked against raw captured fields. Original CPU replay and recovered Debug/Release must match the actual captured complete output. `-RealOnly` suppresses synthetic rows for a future unchanged-source replay; the final legacy-validation correction requires the full 5,760-case matrix plus actual captures. Version 99 fixtures now carry legacy limit -1 to directly exercise its ignored field; the negative-limit refusal uses version 2.
