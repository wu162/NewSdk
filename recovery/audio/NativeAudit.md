# AudioCompiler native backend audit

This audit uses the checked-in original `SDK/tools/BinaryAssetBuilder.AudioCompiler.dll`, SHA256 `a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`. Addresses below are RVAs; PE image base is `0x10000000`. It does not rely on another conversation's local artifacts.

## Original execution path

`Plugin.ProcessAudioFileInstance` is managed IL at RVA `0x5974`. Its imported helpers call statically linked SIMEX and RenderWare AudioCore implementations. No external audio codec DLL appears in the PE import table. The original plugin accepts SIMEX format IDs 0 and 1 (WAVE and AIFF), and creates output format 39 (SNR/SNS).

| Function | RVA |
|---|---:|
| `SIMEX_id` | `0x25210` |
| `SIMEX_open` | `0x25370` |
| `SIMEX_info` | `0x25490` |
| `SIMEX_read` | `0x25500` |
| `SIMEX_create` | `0x25640` |
| `SIMEX_write` | `0x256f0` |
| `SIMEX_getsamplerepname` | `0x26160` |
| format metadata lookup | `0x25190` |
| SNR/SNS format 39 writer | `0x2de20` |
| SNR bit-header writer | `0x2cb50` |
| MSB bit append | `0x2c6a0` |
| SNR/SNS data-block writer | `0x2d4d0` |
| PCM16BE encoder constructor | `0xf5b0` |
| PCM16BE float-to-int encoder | `0xf600` |
| unsigned PCM8 (`P8U0`) decoder | `0x10390` |
| XAS1 four-subblock rearrangement | `0xf690` |
| XAS1 predictor and quantizer | `0xf700` |
| XAS1 streaming buffer | `0xfa60` |
| XAS1 partial-block flush | `0xfba0` |
| XAS1 encoder constructor | `0xfc40` |

The SIMEX dispatch table starts at RVA `0xfd160`, with 40-byte entries. Within each entry, offsets 0/4/12/16/24/28 identify metadata, format identification, open, info, create and write callbacks. Format 39's write callback is `0x2de20`.

## Policy recovered from managed IL

`src/AudioCompiler/Native/AudioPolicy.cs` reproduces selection of SIMEX sample representation IDs. PC defaults to XAS1 (ID 29) when the input directory ends in `\\sounds`, otherwise PCM16BE (1). Xenon defaults to EA-XMA (28), PS3 defaults to XAS1 (29). Explicit EALAYER3 produces the original warning and falls back to PCM on PC/Xenon or XAS1 on PS3. This original SDK cannot perform EALAYER3 encoding, so implementing MP3 encoding is not required for equivalence to this DLL.

The native SNR writer maps IDs to codec registry identifiers: PCM16BE 1 → `P6B0`; EA-XMA 28 → `EXm0`; XAS1 29 → `Xas1`; EALayer3 30/31/32 → `EL31`/`L32P`/`L32S`. The plugin does not select IDs 30–32. Quality is checked only for representation 28 or 31. An explicit requested sample rate is validated in [400,96000] only when it differs from the source rate, then invokes SIMEX filter 120.

## PCM16BE and SNR/SNS

The implemented component `src/AudioCompiler/Native/Pcm16Encoder.cs` is a managed replacement for native code and does not call the original DLL. Its current input entry accepts mono/stereo PCM8/PCM16 RIFF/WAVE, rejects loop metadata, and does not resample. Unsupported cases remain explicit exceptions.

SNR's eight-byte header is MSB-first: version 4 bits (0), codec 4 (PCM=2/XAS1=4), channels-minus-one 6, sample rate 18, storage 2 (RAM=0/stream=1), loop-present 1, frame count 29. Each data block has a BE32 size including the 8-byte block header and a BE32 frame count. RAM uses a single data block without the final high bit. Streamed output sets `0x80000000` in the final block and pads the final block to a 64-byte file boundary. The native branch adds a full 64 padding bytes when the unpadded length is already aligned.

PCM input samples normalize as `signed16 / 32768.0f`. The encoder uses `truncate(floatSample * 32767.0f)`, then writes big-endian signed16. A byte-preserving PCM pass-through would therefore be incorrect: input `[0,1,-1,32767,-32768,0x1234]` becomes `[0,0,0,32766,-32767,4659]`.

Unsigned PCM8 input uses the original registry ID `P8U0` (entry at RVA `0xf9448`), whose decoder at `0x10390` zero-extends each byte, subtracts 128, converts to float and multiplies by the float constant at `0xac984` = 0.0078125 (exactly 1/128). The original instructions at `0x10450` and `0x104d0` establish normalization as `(unsignedByte - 128) / 128.0f`. Relevant disassembly is preserved in `pcm8-decoder-disassembly.txt`. This extends input normalization only; PCM16BE/XAS1 output encoders do not change.

The block limit is computed using x87 extended arithmetic as `truncate(2040 / bytesPerSecond * sampleRate)`, not by simplifying that expression. At 22050 Hz this yields 1019 mono PCM frames or 509 stereo frames. `NativeBlockFrames` emulates 64-bit significand rounding using decimal arithmetic and the exact binary32 bitrate value. The original encoded payload buffers are sized for this calculation.

Original PCM evidence: `recovery/evidence/audio/plugin-validation/40192722185-1/pcm-original.txt` and four adjacent WAV files. There are sixteen cases: mono/stereo, 3/20001 frames, RAM/stream, PC/Xenon NONE. An independent Python replay of the recovered arithmetic/container matched all sixteen HeaderData/CustomData outputs. The C# implementation still requires the recorded Windows component validation before acceptance is claimed.

## XAS1 source reconstruction

`src/AudioCompiler/Native/Xas1Encoder.cs` independently translates the original native routines. Each 128-frame channel block produces 76 bytes: four 4-byte group headers followed by fifteen rows with one byte per group. Each group holds two 12-bit PCM histories and thirty ADPCM nibbles. Predictor pairs in encoder sign convention are `(0,0)`, `(-0.9375,0)`, `(-1.796875,0.8125)`, `(-1.53125,0.859375)`. The predictor is chosen by minimum maximum residual, with the original filter-zero early exit at residual ≤7. Quantization uses the original float operations, integer truncation, shift search, 0x800 rounding, signed16 saturation and feedback. Partial final blocks repeat the last frame, rather than zero-padding the source.

Original plugin evidence `plugin-validation/40193025288-1/xas-original.txt` contains twelve cases: mono/stereo, 128/129/4096 frames, RAM/streamed at 22050 Hz. An independent Python replay matched all twelve outputs byte-for-byte. This does not replace the candidate C# Debug/Release runs.

`recovery/scripts/GenerateAudioXasNativeOracle.py` maps the original PE at its preferred base and executes the pure x86 routine at RVA `0xf700` under Unicorn 2.1.4. It does not import the candidate algorithm. The checked-in `native-xas/original-blocks.tsv` contains 262 input/output cases with exact float32 input samples and original native output bytes. Cases cover mono/stereo constant signals, five sine frequencies, ramps and seeded random samples, exercising all four predictors and all thirteen quantizer shifts (0–12). The generator pins the original DLL SHA256, Unicorn version, image base, x86 mode and FPU control word; its output also records the Python and pefile versions. Original 128-frame plugin payloads in four RAM/streamed mono/stereo cases also match direct native emulation. This is a component oracle, not a claim that Unicorn runs the original SDK or the full mixed-mode DLL.

Public format cross-checks were read after the independent implementation: vgmstream `src/meta/ea_eaac.c` (blob `7853f94e6fd2095f44801bfe9eadbc245594e2e8`) confirms header fields and codec IDs; `src/coding/ea_xas_decoder.c` (blob `4de6c42dd1a2112a50d4a44882fb69bf910ea321`) confirms 76-byte layout and four predictor pairs. No code from those files or the unlicensed `lgdel/eaxas` project was copied into the implementation.

## Unrecovered native scope

AIFF importing, WAVE formats beyond the explicit PCM8/PCM16 entry, loop marker semantics, automatic channel remapping, SIMEX resampling filter 120, EA-XMA, and generic SIMEX filters remain outside this source component. They must not silently fall back to approximate behavior. Metadata and PathMusic/MP3 components have separate scopes and evidence.
