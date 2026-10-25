# Raw DDS encoder recovery (no-mipmap component accepted)

Original input SHA256: `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`.
`nvDDS::nvDXTcompress(RGBAImage const&,...)` is token `060000dc`, RVA `52b60`.
RVA `52950` is the separate `RGBAMipMappedVolumeMap` overload.

Current source is `src/TextureCompiler/Managed/TextureRawDdsEncoder.cs`.
Target initial scope is uncompressed 2D A8R8G8B8, X8R8G8B8 and R5G6B5 with
mipmap generation disabled. Input is tightly packed RGBA. No original DLL or
external compressor is called by recovered encoding. Mipmap requests are
explicitly rejected until native filtering is recovered and accepted.

Original header writer native RVA `56490` emits magic separately then a zeroed
124-byte header. Its no-mipmap flags are `0x81007` (LINEARSIZE, not PITCH), mip
count zero and caps `0x1000`. Native pixel-format jump table RVA `56708` selects:

| Option format | PF pointer RVA | Flags | Bits | R/G/B/A masks |
| --- | --- | --- | --- | --- |
| 6 R5G6B5 | a3024 | 40 | 16 | f800 / 07e0 / 001f / 0 |
| 7 A8R8G8B8 | a2f44 | 41 | 32 | ff0000 / ff00 / ff / ff000000 |
| 25 X8R8G8B8 | a2f64 | 40 | 32 | ff0000 / ff00 / ff / 0 |

`recovery/tests/TextureRawDdsOracle.cs` verifies the immutable original DLL hash,
uses its original pure GetNVCompressionOptions, then invokes the real RGBAImage
native compressor via a cdecl delegate. The callback captures every output byte
and five-field MIPMapData when present. The initial matrix includes both mip
settings, three raw formats, nine positive power-of-two dimensions including
1D tails, and five pixel patterns: 270 fixtures. This original evidence host is
kept immutable. The separate `TextureRawDdsDifferential.cs` compares recovered
no-mipmap output. Mipmap fixtures provide original evidence only.

Original evidence run [40196625641](https://github.com/wu162/NewSdk/actions/runs/40196625641)
succeeded with 270 fixtures. Transcript is preserved in
`evidence/texture/raw-dds-probe/40196625641-1/transcript.zip`. It confirms
A8R8G8B8 writes BGRA, X8R8G8B8 writes BGR followed by **zero**, and R5G6B5
quantizes each input channel to its field maximum by nearest-value rounding:
`(channel * max + 127) / 255`, with max=31/63/31. Bit truncation does not match.
All 135 no-mip fixture payloads (30,050 bytes) were independently checked against
that formula locally. Windows run
[40197368508](https://github.com/wu162/NewSdk/actions/runs/40197368508) attempt 1
then accepted the actual recovered source at commit
`3b7eac919422044cbb7e8deebd9381d809f14527`: Debug and Release each passed 135
fixtures and 48,157 checks. Receipt is
`evidence/texture/stage2/Raw/40197368508-1/validation.json`.
This is raw encoder component acceptance, not SDK integration acceptance.

Differential mode only invokes 135 original no-mip fixtures, compares all DDS
bytes and confirms input preservation. It also checks 12 explicit adapter
rejections, including a mipmap=true request. It does not repeat the already
captured original mipmap=true matrix or label that evidence recovered support.


## Mipmap investigation boundary

Original captures show each mip level is independently resampled from the base
image. The triangle kernel excludes out-of-range taps and renormalizes the
remaining weights; e.g. 4-to-2 first output has weights 3/7,3/7,1/7, while
4-to-1 uses 5/24,7/24,7/24,5/24. A mathematical reconstruction still differs at
float32 half-value rounding boundaries. These findings are evidence for further
work, not an accepted mipmap implementation. The encoder continues to reject
all mipmap=true requests explicitly.

`TextureMipFloatProbe.cs` is an isolated test-only probe of native
FloatToUnsigned RVA5bc90. It guards original SHA/opcodes/relocated constants,
saves/restores all x87/SSE state and GPRs/EFLAGS around a float-bit capture
callback, executes the original quantizer instructions, restores the hook in
finally, and requires every captured DDS byte and callback record to match the
immutable 270-fixture transcript. Its traces are only usable after that entire
comparison passes. The probe is never linked into recovered source.
