# XAS binary32 quantization regression

2026-10-25 (Beijing). Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
Original group encoder: RVA `0xf700`. This change restores its individual
SSE `mulss/addss/subss` binary32 boundaries in `Xas1Encoder.EncodeGroup`.

The full resampling plugin batch exposed an optimized x86 CLR difference.
Run `40208638429`, source `fc36e7ca101d9154e9774b9aeeccdd850d8dc007`,
passed all 448 defined records in Debug; Release differed in four XAS records:
8191-frame mono/stereo sine, 22050 -> 44100, RAM/stream. PCM output matched.
No comparison tolerance was introduced and the failing records were retained.

The mono RAM CustomData differs in eight bytes at offsets
3696, 3700, 3704, 3708, 3712, 3716, 3732 and 3736. Extracting output block48,
group0 gives:

- Original: `c2f536fc464666576676766765665555344241`
- Old Release: `c2f536fc464666576757577576565555344322`

Executing original resampler machine code on the pinned WAV, then original
`0xf700`, reproduces the original bytes. A strict per-operation binary32
replay agrees on every group. Keeping extended intermediate values instead
reproduces the actual old Release bytes exactly. The first threshold crossing
is audio frame6155: original scaled residual `26623.8125` truncates to26623,
while extended evaluation gives approximately26624.088657 and truncates to26624.
Adding0x800 and clearing the low12 bits then yields different quantizers.
The original and saved input WAV both have Git blob
`2163442d63de29c6ed004b7e1b567aebdc37114c`.

A non-inlined float call boundary now stores binary32 after each original
multiply, add and subtraction, including initial sample/history preparation
and reconstruction feedback. It does not depend on whether the x86 JIT spills
a local value to memory. `AudioXasPrecisionFixture.cs` preserves the original
32 signed16 samples; mono/stereo repetitions form two new128-frame cases.
The original PE generates their complete block outputs independently.

`GenerateAudioXasNativeOracle.py --include-precision` retains the original262
rows and appends these two cases. `AudioXasBlockOracle ... precision` requires
all264, original fingerprint/FPU, all predictor/shift coverage and both exact
signed16 regression inputs/outputs. Old two-argument use still requires262.

Accepted at source `684e3310004853455dd0b5b4e53287afb067e93c`,
run [40209451010](https://github.com/wu162/NewSdk/actions/runs/40209451010),
attempt1: Debug/Release full264 block matrix passes. The eight affected
plugin groups have1392 exact records per configuration, including all448
defined resampling fixtures and8 separately asserted single-frame safety
refusals. The original/original-free recovered combined SDK MOD passes38
actual plugin calls, four output products and36 CustomData files. Receipts
are under `../evidence/audio/xas-precision-validation/40209451010-1`,
`plugin-validation/40209451010-1` and `sdk-validation/40209451010-1`.
Unchanged PCM scalar/FFT/unrelated compiler matrices reuse earlier exact anchors.
