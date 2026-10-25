# EA-XMA integral PCM window leaf group

2026-10-26 (Beijing). Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
**This group does not enable XMA encoding or recover the outer window scheduler.**

## Actual encoding dependency and complete scope

The source-processing path identified from the pinned PE is
`648b6 -> 63ec2 -> 62fda -> 60fb7 -> 8c373 -> 8bfb0 -> 97ccb`.
At `97d97`, native `97ccb` calls `[core+18e08]`; the initialization write
at `673af` assigns `98267` to that slot. `98267` handles old/new source
blocks, leading/trailing unwindowed ranges and windowed ranges. For PCM16,
its indirect inner leaf `[core+18e1c]` is assigned `98010` at `672f2`.
For PCM20/24 it directly calls the corresponding integral leaves.

This batch recovers all six integral input window/copy leaves used by that
scheduler, rather than just one sample format or one recurrence step:

| Native range [start,end) | Bytes / instructions | Input / operation |
|---|---:|---|
| `98010..980da` | 202 / 80 | signed16 window |
| `97da4..97e7d` | 217 / 85 | signed20 stored in signed24, arithmetic shift4, window |
| `97edd..97fb3` | 214 / 84 | signed24 window |
| `980da..9812a` | 80 / 37 | signed16 unwindowed copy |
| `97e7d..97edd` | 96 / 42 | signed20 unwindowed copy |
| `97fb3..98010` | 93 / 41 | signed24 unwindowed copy |

Total 902 bytes / 369 instructions, with no calls from any of the six leaves.
Their native pointer ABI is replaced by bounded managed buffers and cursors in
`src/AudioCompiler/Native/XmaPcmWindow.cs`. The window leaves use 15 stack
arguments (`ret3c`), the copy leaves ten (`ret28`); unused native fields are
not assigned invented meanings.

## Recurrence and exact floating boundaries

For each sample, the window leaf writes `float(sample * w)`, then computes:

- `newW = float(q * factor + previousW)`;
- `newQ = float(previousQ - w * factor)`;
- `previousW = oldW`, `previousQ = oldQ`, `w = newW`, `q = newQ`.

Each native `float(...)` is a real x87 `FSTP binary32`. The multiply and
add/subtract within each recurrence use the original 53-bit x87 precision,
with no binary32 rounding between them. The candidate explicitly stores
binary32 via `BitConverter` at those boundaries; it does not rely on a
Release JIT keeping a C# float temporary in binary32.

Signed16 uses two bytes per input item. Signed20 and signed24 use three;
only signed20 performs arithmetic shift4. Source strides count interleaved
items, so actual byte advancement is stride times item size. The native
position, source cursor and output cursor are advanced together; all four
recurrence words are returned even for an empty interval. Unwindowed copy
leaves leave recurrence state alone.

Defined candidate policy rejects null/undersized arrays, invalid formats,
nonpositive stride, negative/reversed positions, invalid source/output slices,
nonfinite input factor/state and aliased recurrence/output arrays. These are
separate safety policies, not original malformed-pointer equivalence claims.
The parent scheduler's valid nonnegative forward intervals are the recovered
entry domain; reverse/native-undefined cursor layouts are not accepted.

## Verification entry and status

`recovery/scripts/TestAudioXmaWindow.ps1` first runs original Windows x86 CPU,
then isolated candidate Debug/Release. The original host invokes all six native
functions with guarded input/state/output allocations and a pinned DLL/load
path. It compares 6,888 cases: 6,720 window cases and 168 copy cases,
covering all formats, stride1/2, zero/extreme/impulse/deterministic random PCM,
counts0/1/2/3/31/128/257, four recurrence initializations, ten factor bit
patterns (including signed zero, +/-2 and near2). Each comparison includes
all output float bits, all four final recurrence bits, all three cursors,
source immutability and untouched output prefix/suffix. Twelve candidate
refusal cases are counted separately from equivalence.

Both hosts require actual physical x87 CW027f and verify restoration of the
previous physical control word. Candidate runs check original target DLL
absence on disk and in loaded modules. Receipts are stored under
`recovery/evidence/audio/xma-window-validation/<runId>-<attempt>` with exact
sourceCommit/runId. Take final pass/fail from that receipt; this source note
does not predeclare a Windows run passed.

Local preflight ran original machine code in Unicorn against an independent
Python recurrence for 45 combinations, each31 samples, with complete state
and float-bit agreement. This only catches implementation/ABI mistakes;
it is not a substitute for real CPU or candidate Debug/Release acceptance.

The next integration work is the entire `98267` scheduler and the downstream
actual transform/quantization/entropy core. This leaf group alone does not
recover those paths, supply production XMA output, or justify an SDK success
claim. See [XmaCodecCore](XmaCodecCore.md) for the previously recovered noise
pair, and [XmaNextBatch](XmaNextBatch.md) for the larger codec milestones.
