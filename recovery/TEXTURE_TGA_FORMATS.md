# Additional TGA format group

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

This independent group leaves the accepted raw24/raw32/metadata `TextureTgaDecoder.cs` and all 1,675 earlier cases unchanged. New implementation: `src/TextureCompiler/Managed/TextureTgaAdditionalFormats.cs`; new differential: `recovery/tests/TextureTgaFormatsOracle.cs`. No original-DLL delegation, Plugin/default-loader wiring, or workflow changes are made here.

## Original behavior established by probe

Original probe `40197368504-1` ran Debug and Release against 4,040 fixtures. It is evidence gathering, not acceptance: the initial candidate matched 320 cases and differed in 3,720; original rejected 1,920. Full immutable original pixels and failure signatures are in `recovery/evidence/texture/stage2/Formats/40197368504-1/transcripts.zip`.

The complete data establishes:

- True-color **16-bit** is supported. Original 5→8 channel conversion is `(value * 255 + 15) / 31`, verified against all 65,536 packed words under attributes 0/1 and all four directions. Alpha is the high bit, then the same all-zero-image alpha normalization as the earlier true-color group.
- **8-bit grayscale** is supported with intensity copied into all RGB channels, alpha 255 and `HasAlpha=false`.
- **16-bit grayscale/alpha is rejected** by original D3DX with `BinaryAssetBuilderException`, ErrorCode `BadTextureFile`, message `D3DX cannot load the texture from disk.` The recovered implementation preserves that rejection.
- Mapped TGA supports **8-bit indices**, palette first indices 0/5 in the probe, and 15/16/24/32-bit palette entries. All **16-bit index** cases are rejected by original D3DX with the same BadTextureFile failure and are preserved as negatives.
- The original palette path swaps red/blue relative to the true-color path. Packed palette entries use **bit replication** for 5→8 channels (`(v << 3) | (v >> 2)`), while direct true-color16 uses the rounded mapping above. Original output is preserved exactly.
- Palette alpha has two stages: an entirely zero-alpha palette is first made opaque; afterward the selected pixels are checked for all-zero alpha. Mapped images normally report `HasAlpha=true`, including 15-/24-bit opaque palettes and palettes whose zero alpha was normalized. A mixed-alpha palette whose selected pixels are all zero-alpha becomes opaque and reports false. This differs from determining `HasAlpha` solely from palette entry depth.

An independent local reconstruction model replayed every immutable original record after applying these rules: **2,120 full pixel/alpha matches + 1,920 exact-format rejections, zero model differences**. This supports implementing the correction; Windows execution of the actual corrected C# source remains the required acceptance.

## Corrected implementation scope

Supports raw and row-contained RLE for true-color16, gray8, and mapped8 with 15/16/24/32 palette entries. Handles four directions, image IDs and bounded first/count ranges within the 256-entry index space. Preserves original explicit rejection for gray16/index16. Truecolor attributes are limited to 0/1; gray/mapped attributes to 0. Interleaving, nonzero coordinate metadata, unmapped truecolor15, footer/extension and other descriptor metadata remain explicit unsupported boundaries in this additional group. Earlier raw24/raw32 metadata support is unchanged.

The source does not copy a third-party decoder. Pixel and alpha policies derive from the original DLL evidence. Original SHA256 remains `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`.

## Final independent matrix

All original 4,040 cases remain. The corrected matrix totals **4,534 fixtures**:

- 4,032 base combinations: sizes 1×1/4×2/8×4, raw/RLE, four directions, four alpha patterns, IDs 0/7; true16 attributes 0/1, gray8, gray16 attributes 0/8, four palette depths × 8/16-bit indices × first indices 0/5.
- 8 raw16 exhaustive images: all 65,536 packed words, attributes 0/1 and all four directions.
- 12 grayscale exhaustive images: every gray8 intensity under four directions; every 16-bit grayscale/alpha word under attributes 0/8 and four directions (original rejection negatives).
- 384 exhaustive packed-palette images: all 32,768 RGB555 words in 128 palette15 blocks and all 65,536 ARGB1555 words in 256 palette16 blocks, each 256-entry palette addressed by all 256 indices.
- 2 full-byte palettes: 24-bit RGB and 32-bit RGBA byte/alpha ranges across 256 entries.
- 96 upper-index-boundary cases: palette first/count 250/4, 253/3 and 255/1, all palette depths, raw/RLE and all four directions.

Expected strict accounting is **2,606 complete pixel/alpha matches + 1,928 matching original rejections**, no unsupported-original-success exclusions and no dropped fixtures. New upper palette/index and exhaustive cases require the new strict receipt; original probe acceptance is not reused for those added cases.

## Build and execution

Build x86 CLR4 `/unsafe /r:Recovered.BinaryAssetBuilder.TextureCompiler.dll`, legacy-v2 activation and unchanged original support DLLs beside the host. The host guards original SHA/allocator thunk instructions, uses original VC8 malloc/free, original Initialize, real nvImage ctor/dtor and LoadImageA, then compares every output pixel and alpha or the full rejection type/message/ErrorCode.

`TextureTgaFormatsOracle.exe original.dll transcript.txt fixtures-dir --probe|--strict`

Probe records differences without acceptance. Strict fails any unexpected acceptance/rejection/pixel/alpha result and hard-checks total 4,534 and exact 2,606/1,928 accounting. It never calls rejections successful image decodes and never reruns the old 1,675 group. No SDK integration is implied.

