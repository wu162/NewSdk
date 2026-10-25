# Integral PCM input importer recovery

Evidence input: original `SDK/tools/BinaryAssetBuilder.AudioCompiler.dll`, SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
Addresses are RVAs, image base `0x10000000`. Relevant disassembly is preserved in
[`pcm-input-disassembly.txt`](pcm-input-disassembly.txt).

`src/AudioCompiler/AudioPcmInputReader.cs` is the plugin input reader. The older
`Pcm16Encoder.ReadWave` remains the narrow PCM8/16 component entry point so that
its component build and API do not acquire new importer dependencies.

## Recovered input representations

| Input | Native importer | Representation selection | Decoder |
|---|---|---|---|
| WAVE unsigned8 | `0x37210` | `0x37527` → ID11 | `P8U0`, `0x10390` |
| WAVE signed16 LE | `0x37210` | `0x37535` → ID0 | existing PCM16 normalization |
| WAVE signed24 LE | `0x37210` | `0x37571` → ID19 | SIMEX high16 conversion `0x47630` |
| AIFF signed8 | `0x381c0` | `0x3840e` → ID2 | `P8S0`, `0x10520` |
| AIFF signed16 BE | `0x381c0` | `0x3841c` → ID1 | existing PCM16 normalization |
| AIFF signed24 BE | `0x381c0` | `0x3843c` → ID21 | SIMEX high16 conversion `0x47720` |

The AudioCore 24-bit decoders at `0x10670`/`0x10830` move the three bytes into
integer bits31..8, convert to float32 and multiply by `2^-31`. **Those are not
the SIMEX file-import path.** The original/candidate plugin differential exposed
this distinction: input `0x123456` must produce PCM output `0x1233`, not `0x1234`.

SIMEX importer `0x404e0` dispatches through table `0xfdfa0`: representation19
(LE24) → `0x3a9e0`, representation21 (BE24) → `0x3ab80`. They create conversion
objects at `0x476b0`/`0x477b0`, with vtables `0xb9794`/`0xb97a4`. Their actual
read methods `0x47630`/`0x47720` **discard the low byte and store signed16**.
LE reads bytes2/1 into AH/AL; BE reads bytes0/1. Thus the plugin path normalizes
`(signed24 >> 8) / 32768.0f`, which is what the candidate implements.
Signed8 converts the signed byte then multiplies by `0xac984` (`2^-7`).

The WAVE importer accepts format tags 1 and `0xfffe` (`0x37321..37369`), but its
width selection only accepts 8, 16 and 24. This recovery still rejects extensible
WAVE outside the canonical subset described below. IEEE-float format tag3 and
32-bit PCM are not supported by the original width selection either.

### WAVE extensible: actual native rules versus recovered boundary

The identifier `0x371ce..371f6` checks only format tag1/`0xfffe`. When the open
callback sees `0xfffe`, `0x37335..37369` seeks back to the fmt chunk header and
reads 48 bytes (8-byte chunk header plus 40-byte format). The remainder of the
complete callback, through `0x375d9`, never interprets cbSize, validBits,
channelMask or subFormat GUID. It selects an integer decoder solely from the
container bits8/16/24; even a float GUID does not select a float decoder. This
is native instruction evidence, not a standards-derived assumption.

The candidate enables a deliberately narrower, ordinary integer PCM subset:

| Field | Enabled recovery boundary | Native callback |
|---|---|---|
| fmt chunk size | at least40 bytes | attempts a 48-byte read including header |
| cbSize | exactly22 | ignored |
| validBits | equals container bits8/16/24 | ignored |
| GUID | `00000001-0000-0010-8000-00aa00389b71` | ignored |
| mono channelMask | 0 or4 (front center) | ignored |
| stereo channelMask | 0 or3 (front left/right) | ignored |

Other GUIDs, partial validBits, extra extension sizes and nonstandard masks
are explicit refusals despite being tolerated as raw integer PCM by native.
These refusals must not be described as original acceptance parity. The
supported subset follows the proven integer decoder path; it performs no
channel-mask remapping.

## AIFF details preserved

The dispatch table `0xfd160` stores AIFF at format index0 (open wrapper
`0x38980` → importer `0x381c0`) and WAVE at index1 (open `0x37210`).

The native AIFF sample-rate conversion reads only the high32 significand bits,
then shifts right by `0x401e - exponent` (`0x3837f..383ca`). The recovery uses
the same integer conversion within the supported positive-rate range rather
than a general extended-float parser.

The AIFF importer rounds chunk sizes to even for traversal at `0x3833c..38352`.
However, `0x385ea` separately rereads the **raw** SSND size and stores it in
`[esp+0x3c]`. `0x38907` retrieves that raw size (the stack temporarily has eight
argument bytes), and `0x388f8..38949` derives the frame count without padding.
An odd mono8 payload therefore retains its declared7 frames. The first candidate
mistakenly included the padding; full-plugin differential caught this and the
reader now uses unrounded SSND payload size.

Only ordinary uncompressed AIFF with SSND offset0/blockSize0, mono or stereo,
and no INST/MARK metadata is enabled. AIFC, marker/loop behavior, channel
remapping and resampling remain explicit refusals.

## Tests

`AudioInputFormatCases` exercises all four newly enabled combinations (WAVE24,
AIFF8/16/24), mono/stereo, 7/129/2041 frames, RAM/streamed, PC NONE/XAS,
Xenon NONE and PS3 XAS: 192 ordinary-format complete-plugin records per configuration.
Canonical extensible8/16/24 adds 288 records spanning mono/stereo, the same frame
boundaries, both zero/conventional masks and all eight codec/storage/platform
combinations. Every
record must produce a buffer and custom data; comparisons include all three
buffers and all custom bytes. The odd7 AIFF8 case covers padding exclusion.
SDK tests additionally use generated AIFF16 and WAVE24 assets through BAB.

`AudioInputReaderOracle` separately asserts 19 candidate rejection boundaries,
including unsupported formats, loop chunks, malformed alignment/lengths and
nonzero AIFF SSND offsets/block sizes and unrecovered extensible metadata. These safety boundaries are not counted
as original DLL parity. Successful Windows differential and SDK receipt IDs
must be taken from the parent stage's final validation record.
