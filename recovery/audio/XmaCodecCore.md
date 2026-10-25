# EA-XMA codec core: numeric helper batch

2026-10-26 (Beijing). Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses are RVAs. **No XMA encoding route is enabled.**

## Closed numeric group implemented

`src/AudioCompiler/Native/XmaNoiseGenerator.cs` implements the complete pair
below, excluding the native pointer ABI and allocation hazards. It is a
numeric noise/state helper, not the transform/quantizer/entropy encoder.

| Function [start,end) | Bytes / instructions | Native contract |
|---|---:|---|
| `64cd5..64cff` | 42 / 14 | One pointer to two int32 words; returns difference and mutates both words; `ret 4` |
| `65a0b..65a54` | 73 / 27 | Outer-state pointer and output pointer; `ret 8`; calls only `64cd5` |

The outer-state first word points to backend configuration/state `config`.
The fill reads `int32 config+104` as count, `float config+128` as gain, and
passes **config+210**, not outer-state+210, to the PRNG. Its two words are
previous transformed value at +210 and uint32 seed at +214. A minimal fill
oracle therefore needs 0x218 bytes for config, plus an outer pointer. Count
zero performs no writes or state transitions.

The state transition is all wrapping int32 arithmetic:

1. `seed = seed * 1664525 + 1013904223` modulo 2^32.
2. `value = arithmetic_shift_right(int32(seed), 2)`.
3. `value += arithmetic_shift_right(value, 2)` modulo 2^32.
4. Return `value - previous` modulo 2^32; store `previous = value`.

The fill executes `FILD int32`, `FMUL float[config+128]`,
`FMUL double[c58e0]`, `FSTP float`. The double is exactly 2^-29,
bits `3e20000000000000`. The original CLR uses x87 precision 53-bit nearest
(physical CW 027f). **Do not insert binary32 rounding between the two
multiplies.** The managed implementation uses a double product and one final
float conversion. Candidate finite-gain bounds are explicit; NaN/infinity,
negative counts and invalid output slices are safe policy refusals rather
than claims about the original's malformed-input behavior.

Actual call edge: `6978f -> 67057`; initialization at `67259..6728d` calls
`65a0b` once per channel when config+28 is nonzero, config+11c is 1 and
uint16 config+54 is positive. Destination pointers come from core+7688 in
4-byte channel steps. The same backend PRNG state is shared across those
calls. Calling it "noise initialization" describes the generated differenced
LCG samples and call site; no unproven flag names or psychoacoustic role are
assigned.

## Validation and current status

`recovery/scripts/TestAudioXmaNoise.ps1` runs actual original x86 CPU to
produce `original.tsv.gz`, then builds an isolated candidate library and
compares Debug/Release independently. There are 6,360 complete cases:
200 state-only sequences and 6,160 fills (8 seeds, 5 previous states,
22 finite gain bit patterns, 7 counts). Gain patterns include positive and
negative zero, smallest/largest subnormal, minimum normal, maximum finite,
the actual backend default gain 0.04 (binary32 bits 3d23d70a, installed
at 8e2b7 from d548c), and neighboring values around 1. Output is compared by exact bytes,
including untouched prefix/suffix; both PRNG words and unrelated state/config
bytes are checked. Eight managed safety refusals are counted separately.

The host pins original DLL SHA/load path, actually reads physical x87 CW,
requires 027f throughout, and restores the previous FP controls. Candidate
runs check original target DLL absence both on disk and in loaded native
modules. Scope flags explicitly leave XMA encoding, SDK and plugin routing
false. Evidence destination is
`recovery/evidence/audio/xma-noise-validation/<runId>-<attempt>`.

Local preflight: 880 original-machine-code Unicorn/Python combinations with
257 samples each agreed in state and float bits after correcting the oracle
pointer layout. This is a sanity check only, **not real CPU or C# acceptance**.
Final Windows status/sourceCommit/runId must be taken from the actual receipt;
this source note does not predeclare the batch passed.

## Actual deeper encode/flush path confirmed

- `59070 -> 5d5b0` passes wrapper this in ECX and five stack arguments
  (`5d5b0..5d6bf`, final ret14). It calls **648b6**, whose full range
  `648b6..64cd5` contains 1,055 bytes / 352 instructions and **14 stack
  arguments, ret38**. The latter enters `63ec2` for source work and `728e3`
  for packet production, or `74f20` under the special three-flag branch.
- `59070 -> 5d520` is the flush path. `5d520..5d5ac` calls `60136`,
  whose range `60136..6026e` contains 312 bytes / 110 instructions,
  seven stack parameters and ret1c. Its packet-production edge is also
  `728e3` or `74f20` under the special flags.
- `6978f` calls `673dc`, `6592c`, allocates a 0x310-byte backend via
  `8e274`, then calls `65a54`, `68ad0`, `67297`, `65e19`, `67057`,
  `69094` and optional `675ce`. These callees have additional allocation,
  table and transform dependencies; this batch does not recover them.

Next actual codec milestone remains reproducible raw `589d0` output before
`ee00` rewriting, then live indirect targets and a complete transform /
quantization / entropy group. The numeric pair above removes one closed
helper dependency but does not materially establish complete encoder scope.
See [XmaNextBatch](XmaNextBatch.md) and [XmaAudit](XmaAudit.md).
