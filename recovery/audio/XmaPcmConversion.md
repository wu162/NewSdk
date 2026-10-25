# XMA PCM preconversion

Original `0xe9b0..0xea7f` (207 bytes, excluding final padding); DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
The argument order is `destination, floatSource, channels, sourceStride, frames`,
StdCall `ret 0x14`. Mono and stereo use the same sample operation; stride is in
float elements, not bytes. Stereo writes two adjacent samples per output frame.

The exact sample operation is SSE `MULSS` by the binary32 constant `32767`
(`0xac86c = 00feff46`), physical store to binary32, `FLD`, masked `FISTP int32`,
then only the low 16 bits are written. With nearest/even control this differs
from the ordinary PCM encoder's truncation. There is no clipping: out-of-int16
results wrap, and masked NaN/infinity/int32-overflow conversion stores integer
indefinite (`80000000`), whose low word is zero. Raw sample 24000/32768 becomes
23999 after this operation.

`XmaPcmConverter` supports the bounded mono/stereo, nonnegative stride/count
array domain, including overlapping and repeated source frames, zero frames,
and nonfinite samples under the original default rounding contract. It safely
rejects invalid channels, array bounds, negative stride/count and output-count
overflow before allocating or writing. It does not expose arbitrary FPU rounding
modes or native negative-stride pointer walks.

`TestAudioXmaPcm.ps1` generates golden outputs on the actual pinned Windows x86
CPU and compares isolated Debug/Release source builds without the original DLL.
The 124 cases include every normalized signed16 value (mono and stereo), random
float bit patterns, NaN/infinity/denormal/overflow values, quantization half-way
neighbors and offset/stride/frame-count combinations. All source bits, destination
prefix/suffix guards and untouched output tails are checked. Nine candidate safety
refusals are separate from the native-equivalence records.

Status at implementation checkpoint: Windows validation pending. No XMA encoder
or product route is enabled by this component. Exact sourceCommit/runId belongs
in the run receipt after verification, never inferred from this audit note.
