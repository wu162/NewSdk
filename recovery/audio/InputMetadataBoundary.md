# WAVE extensible metadata boundary

The recovered importer follows the original SIMEX integer interpretation,
not the WAVE extensible GUID's requested interpretation. All addresses are
RVAs in the original AudioCompiler DLL, SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.

## Native contract

- Identifier `371ce..371f6` accepts format tags 1 and fffe.
- Open callback `37335..37369` seeks back to the fmt header and reads 48
  bytes: eight-byte chunk header plus the 40-byte WAVEFORMATEXTENSIBLE prefix.
- The remaining callback through `375d9` selects mono/stereo integral
  container widths 8/16/24. It does not inspect cbSize, validBits,
  channelMask or any subFormat GUID byte.
- Therefore a float GUID with an integral 16-bit payload still decodes that
  payload as signed16; no float interpretation or channel remapping occurs.
  Ordinary IEEE-float tag3 and PCM32 remain unsupported.

The candidate now accepts a bounded fmt chunk of at least40 bytes and ignores
those extension fields, preserving the existing valid container/rate/channel,
frame alignment and PCM8/16/24 restrictions. A short fmt prefix is a separate
candidate safety refusal. This change removes six conservative refusals from
the prior canonical-only subset; it does not add a new float decoder.

## Verification batch

`TestAudioInputMetadata.ps1` executes original `formats` first, then both
Debug/Release recovered hosts with the original target DLL absent, followed
by a dedicated real SDK batch.

- Existing480 format records remain; 1920 additional records are generated
  from20 metadata variants,3 widths,2 channels,2 frame counts and8 supported
  codec/platform/storage combinations. Every plugin fixture must emit a
  buffer and CustomData before byte comparison can pass.
- Variants: cbSize0/1/21/23/65535; validBits0/1/bits-1/bits+1/65535;
  masks1/4/ffffffff; float/zero/ones/modified-tail GUID; simultaneous garbage;
  fmt sizes42/64 with extra bytes.
- Candidate reader checks also compare120 altered imports against canonical
  imports, including every decoded float sample bit, channels and rate;
 15 remaining malformed/unsupported input refusals stay separate from parity.
- `TestAudioSdk.ps1 -InputMetadataOnly` generates120 XSD-valid AudioFile assets
  using float GUID, partial validBits, nonstandard mask, simultaneous garbage
  and a64-byte fmt chunk, covering all integral widths/channels and PC
  NONE/XAS RAM/streamed. It requires120 actual calls per side, four final
  MOD products and120 CustomData files equal; recovered SDK has no original
  AudioCompiler DLL.

Accepted source `5d56d52ff11510c7e93f83b47d08b1def5c99d27`, run
[40232917060](https://github.com/wu162/NewSdk/actions/runs/40232917060),
attempt1: Debug/Release each2400 exact plugin records and120 sample-bit
invariants passed; dedicated SDK120 actual calls/four products/120 CustomData
matched with the original target DLL absent in the recovered SDK. Exact
receipts are `evidence/audio/plugin-validation/40232917060-1/validation.json`
and `sdk-validation/40232917060-1/validation.json`.
Do not rewrite old canonical-only results as this new batch. The SDK profile
is a dedicated metadata MOD, not full Upheaval.

## Remaining format work

AIFC/NONE subsequently passed its own exact batch; see
[AifcNoneBoundary](AifcNoneBoundary.md). ADP4, additional AIFF marker/loop
structures and nonzero AIFF SSND offset/block size remain outside the current
input acceptance. [Adp4Boundary](Adp4Boundary.md) distinguishes original-only
probe and standalone decoder progress from future file-import support.
