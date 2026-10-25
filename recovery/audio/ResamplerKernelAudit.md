# Filter 120 resampler kernel: remaining recovery work

2026-10-25. This supplements [LoopsAndResampling.md](LoopsAndResampling.md).
The examined original DLL is `SDK/tools/BinaryAssetBuilder.AudioCompiler.dll`,
SHA256 `a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
Addresses below are RVAs. [Instruction evidence](resampler-kernel-disassembly.txt)
is decoded directly from that PE; no candidate algorithm or public resampling
library was used as evidence.

## Current recovered state (2026-10-25)

The six remaining routines are now implemented in `ResamplerCoefficients.cs`,
`ResamplerFilter.cs` and `ResamplerScheduler.cs`, alongside the accepted FFT.
Source `25aab8fe5a6b9e762293f9ace71c3edc59e36e36`, run40207847717:
original Windows x86 CPU window512 exact binary64 constants and Debug/Release
258 exact-byte cases pass. The authoritative golden comes from original real
CPU execution, not Unicorn's host-libm FCOS. The two windows differ in195
binary64 values even though the sampled final coefficient/output floats agree.
The actual Windows CPU window SHA256 is
`605c69534025d105b254107d80806e5e6b07bd8feb36ae0de3cadfc6a9813d87`.

AudioFile integration and XAS precision final acceptance: source
`684e3310004853455dd0b5b4e53287afb067e93c`, run40209451010; detailed scope and single-frame import
boundary are in [AUDIO_COMPILER](../AUDIO_COMPILER.md),
[SingleFrameImportBoundary](SingleFrameImportBoundary.md) and
[XasPrecision](XasPrecision.md). The original kernel itself has defined explicit
one-frame-array tests; the uninitialized file importer is a separate issue.

## Historical pre-implementation audit

**At the time of the following audit general resampling was unimplemented.** It cannot be recovered and
validated safely in the remaining 15-minute implementation window. The small
policy wrapper is understood, but its ordinary output path depends on a
frequency-domain filter and exact float interpolation. A different resampling
library, nearest-neighbor copying, linear interpolation, or changing only the
rate would not reproduce the original bytes.

## Minimum ordinary-path dependency set

| Original routine | Instructions in recorded range | Required behavior |
|---|---:|---|
| `0x429a0..0x42c43` | 165 | Filter 120 callback, setup profile, per-channel allocation, output/loop/marker counts |
| `0x42480..0x4273b` | 194 | Window scheduler, filter delay, fractional phase, Hermite interpolation, int16 conversion |
| `0x41a90..0x41f3a` | 351 | Sample frequency response from profile, transform, finite support/window, optional inversion, transform back |
| `0x41f40..0x41f93` | 32 | Signed16 source windows with zero outside source bounds and integer factor scaling |
| `0x41fa0..0x4224a` | 203 | Transform/filter processing, frequency-domain coefficient multiplication and transform back |
| `0x42250..0x42311` | 60 | Centered derivatives of filtered output for interpolation |
| `0x4eca0..0x4f3ca` | 492 | Bit-reversal, staged complex FFT, twiddle recurrence, direction-dependent normalization |

These seven recorded ranges contain **1497 instructions**, before the small
FFT allocator/destructor, optional progress callbacks, or a differential test
host. The remaining work is concentrated here; it does not require recovering
generic SIMEX format dispatch, the unrelated filter 220 text-profile parser at
`0x42320`, or EA-XMA to implement this one input/output operation.

`0x4ec40` allocates separate real/imaginary arrays of `4*(1<<order)` bytes;
`0x4ec80` frees them. These can become managed arrays without preserving native
allocation internals. The kernel uses order 13 (8192 floats per array) and also
allocates two 32768-byte work buffers. Native allocation/progress functions
`0x39630`, `0x39690`, `0x26fd0` and `0x27030` are host services, rather than
additional signal-processing algorithms; their error boundaries still need
an explicit managed policy.

## Established arithmetic and boundary contracts

The callback builds the profile directly. Its first fields are support setting
256, inversion flag 0, a factor 1 or 2, a float phase/rate parameter, gain 1,
and three or four frequency/amplitude breakpoints. Upsampling, downsampling
by at most two, and downsampling by more than two take different branches
(`0x42a13..0x42ab6`). Constants `0.5f` and the exact binary32 value
`0.949999988079071f` must be preserved. Those values alone do not specify the
complete filter.

`0x41a90` linearly samples the breakpoint frequency response into an 8192-entry
symmetric spectrum, invokes the original transform, windows/truncates its
finite-support impulse response, optionally inverts it, then invokes the
transform again to obtain the coefficients consumed by `0x41fa0`. The window
uses original x87 `FCOS` at `0x41d1c`; the constant at `0xb94dc` is **binary32
pi = 3.1415927410125732**, rather than a newly selected double-precision pi.
A direct sinc/FIR implementation would change both coefficient generation
and accumulation order.

`0x41f40` uses signed16 planar source samples, fills out-of-range positions with
zero, and multiplies the float source value by its integer factor. This is
not the XAS final-block rule that repeats the last source frame. The kernel
starts its source position at the negative filter support, and advances
windows using a usable span derived from 8192, support and factor
(`0x424da..0x42502`). Thus the FIR delay and zero-padding belong to the same
scheduler. Do not repair delay afterward by dropping an arbitrary number of
samples or make the last sample repeat.

`0x42250` is specifically a derivative pass:

`derivative[i] = binary32(binary32(filtered[i+1] - filtered[i-1]) * 0.5f)`.

It is not another FFT or a generic interpolation-library call. At
`0x42631..0x426ba`, the kernel chooses the floor of the float phase using an
x87 integer conversion plus the fractional sign correction, then interpolates
between adjacent filtered samples with those derivatives. Algebraically, for
`p0, p1, m0, m1` and fractional phase `t`, the cubic coefficients are
`a = 2*(p0-p1)+m1+m0`, `b = (p1-p0)-m0-a`, and the value is
`((a*t+b)*t+m0)*t+p0`. The instruction evidence, not an algebraic rewrite,
is authoritative for each binary32 operation's ordering. Phase increment is
`binary32(1/ratioParameter)`; phase is accumulated with repeated binary32
additions (`0x42700`), not recomputed as `outputIndex/ratio`.

The final sample is rounded by x87 `FISTP` to signed16 (`0x426c4`), using the
caller's rounding control; it is **not** truncated like `Pcm16Encoder`'s final
float-to-PCM output stage. Result `0x8000` triggers the native sign-based clamp
at `0x426cd..0x426da`: positive overflow becomes 32767, negative overflow
becomes -32768. The resulting signed16 samples subsequently pass through the
normal PCM/XAS output encoder, so bypassing this intermediate int16 stage
would produce different bytes.

The ordinary callback's output frame count, loop/marker rescaling and clamp
rules, and its rate-only short-input branch are documented in
[LoopsAndResampling.md](LoopsAndResampling.md). The callback must remain paired
with this kernel; matching just metadata does not constitute audio equality.

## FFT numerical details still requiring implementation and oracles

The FFT at `0x4eca0` is self-contained in the original PE, with no external
codec calls. It uses a bit-reversal table at `0xfe978` (256 32-bit entries),
x87-generated stage twiddles and SSE binary32 butterflies. Direction selects
exact double constants `+2*pi` or `-2*pi` at `0xb9980/0xb9988`. The stage
recurrence uses x87 `FSIN` (`0x4eef3`, `0x4eefd`) and converts the resulting
coefficients to float before the butterfly loops. Direction 1 also multiplies
both arrays by a float `1/N` (`0x4f2b9..0x4f3c0`). A generic FFT may use a
different direction convention, scaling location, twiddle constants, or
butterfly order; visual/audio similarity does not establish byte equivalence.

The known CLR precision mode is x87 53-bit (`0x027f`), as independently exposed
by the 400/431 Hz block-size differential. The resampler additionally uses
nearest-integer `FIST/FISTP`, so a native oracle must explicitly record both
precision and rounding bits. Managed `Math.Sin/Math.Cos` cannot be presumed
to reproduce x87 `FSIN/FCOS` exactly. One possible recovery strategy is to
extract immutable native stage-twiddle and fixed-support window constants
under the recorded control word, and preserve all subsequent operation order
in managed source; that strategy still needs original-plugin comparisons.

## Next complete implementation batch

The practical work is one kernel-recovery batch, expected to require a full
implementation session plus actual Windows validation time, rather than a
15-minute addition. A reasonable sequence is:

1. Recover and test the self-contained FFT, then profile-to-coefficient setup.
   Native-machine-code oracles must retain exact input float bits, direction,
   order, output real/imaginary bits and FPU control word. Start with orders
   used by the kernel, zeros, impulses, DC, alternating signs and seeded noise.
2. Recover window scheduling, filtered output and derivative arrays. Compare
   intermediate arrays around negative source indices, the first usable
   sample, each usable-span boundary and the final zero-padded window.
3. Recover Hermite phase progression and signed16 rounding/clipping. Compare
   original kernel output for impulses, constant values, alternating extrema,
   sine and noise, upsampling and both downsampling profile branches, integer
   and fractional output lengths, and half-integer rounding neighborhoods.
4. Compare the full original AudioFile route in Debug/Release for mono/stereo,
   PCM8/16/24 WAVE/AIFF inputs, PCM/XAS output and RAM/streamed storage. Include
   lengths on both sides of `binary32(frames*ratio) <= 1`, loop start/end
   rescaling and clamping, and output lengths crossing SNR block boundaries.
   Finish with actual SDK calls and all output comparisons for that batch.

No resampler runtime code was added in this audit. Unsupported rate changes
continue to fail explicitly. Existing loop and no-resampling format acceptance
must retain their original sourceCommit/runId receipts; this document supplies
investigation evidence only.


## Separate FFT component candidate

`src/AudioCompiler/Native/ResamplerFft.cs` now independently translates the
bit-reversal, radix-2 butterfly operation order, stage-twiddle recurrence and
direction-dependent normalization for orders **1..13** and directions **0/1**.
It is not called by `PcmWaveAudioCompiler`, does not change the explicit rate
conversion rejection, and does not claim the full resampling kernel or the
native FFT's behavior outside this declared subset.

`GenerateAudioFftOracle.py` executes the original `0x4eca0` code under pinned
Unicorn 2.1.4 with x87 control word `0x027f`, captures all 26 stage coefficient
pairs, and preserves exact binary32 input/output arrays for 78 fixtures:
13 orders, both directions, impulse/DC/seeded complex noise. Native coefficient
float bits are embedded statically in the component, so candidate execution
does not read or load the original DLL. `native-fft/original.tsv.gz` and
`native-fft/twiddles.tsv` regenerate byte-for-byte deterministically. An
independent Python binary32 replay of the recovered operation sequence matched
all 78 original-machine-code outputs bit-for-bit.

The C# implementation uses a non-inlined float argument boundary after each
arithmetic operation to prevent legacy x86 extended-register temporaries;
future simplification or FMA/vectorization must rerun the exact-bit oracle.
`AudioResamplerFftOracle.cs` compares every real/imaginary output bit and checks
all order/direction/pattern coverage, plus nine separate candidate safety
rejections. Its Windows Debug/Release component acceptance passed at source `69915484a322542ddf5a8041609b396e40c29a86`, run `40199685628` attempt1: 78 exact-bit comparisons and nine safety checks per configuration. This result supplements, rather than substitutes for, original-plugin resampling integration. This component
still requires integration with coefficient generation and scheduling, then
an original-plugin rate-conversion and actual SDK differential.

The FFT is now a verified separate dependency. The six other ordinary-path ranges above contain 1005 original instructions. They were the next complete batch at this earlier FFT-only checkpoint; their current recovery and acceptance are recorded at the top of this document.

## Real original Windows CPU cross-check

Source `eb68933006f54681f33b38baa196e9ff6273f938`, run `40199843730` attempt1: `AudioNativeFftOracle` loaded the pinned original mixed-mode DLL on Windows x86, verified its mapped identity/path, selected 53-bit nearest rounding through the original CRT IAT, and invoked native RVA0x4eca0 via ThisCall. All 78 original real/imaginary array bit patterns matched the Unicorn baseline exactly. This validates the emulator-derived constants and oracle on the real CPU as well. The separate original CPU receipt has no candidate modes and does not replace the C# Debug/Release anchor `69915484a322542ddf5a8041609b396e40c29a86` / `40199685628`. Full resampling and SDK integration remain explicitly unimplemented.
