# TextureCompiler recovery

Implementation source lives in this directory. Original DLL, IL/decompiler exports,
component oracles and immutable receipts live in `SDK/tools` and `recovery`.

Build managed code on Windows with .NET Framework 4.8 targeting support:

```powershell
dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release
# For CI, prepare a complete batch and commit with [texture-test].
# SDK milestones use [texture-sdk]; see recovery/TEXTURE_TESTING.md.
```

The test script additionally needs x86 MSVC (`/clr`) and .NET Framework csc.exe.
The production SDK DLL/configuration is not replaced by this project.

## Target platforms

Per the user's 2026-10-25 instruction, recovery and final acceptance target
Win32/PC and PlayStation 3 only. Xbox 360 / XenonBundler and Xbox-specific
surface/header/tiling conversion are excluded, not pending completion.
Historical evidence and platform metadata are retained. Shared XGraphics-named
image routines still needed by PC/PS3 remain in scope.

## Reconstructed scope

- Plugin metadata and managed platform/hash state.
- Alpha classification, image validation and format inference.
- Complete original 1644-byte compression option initialization; this constructs
  options and does not implement the proprietary compressor.
- In-memory terrain atlas layout, wrapping borders, normal alpha handling and
  original EASTL metadata ordering; file decoding is an explicit loader boundary.
- Opt-in TGA24/32 raw/row-contained RLE decoder with alpha/orientation rules
  and validated bounded standard/legacy-DevIL metadata dialects; all three real
  main-branch TGA fixtures match original complete pixels.
- Native SimpleBuffer, byte reversal and two constant-return D3D facade functions.
- Texture and OnDemandTexture DDS passthrough for PC/PS3/unknown platforms,
  four-byte block padding, endian relocations and separate `.cdata` serialization.

The recovered managed library does not load the original TextureCompiler DLL.
The differential tests deliberately load it as an independent oracle.

## Remaining work

JPEG, ICC/APNG, NPOT D3DX resampling and other image profiles outside the declared TGA/PNG/BMP subsets; DXT compression and unrecovered resampling paths.
Complete PC/PS3 plugin native lifecycle is not implemented. Xbox 360/Xenon
conversion is excluded from the recovery target. TerrainTextureAtlas is not yet wired to a complete codec
pipeline. Unsupported entry paths throw; there is no original-DLL fallback.

Dedicated real SDK MOD checks are now recorded in recovery/TEXTURECOMPILER.md; production SDK files remain unchanged. See
`recovery/TEXTURE_CONTRACT.md`, `TEXTURE_IMAGE.md`, `TEXTURE_PACKER.md`,
`TEXTURE_NATIVE.md` and the latest exact validation receipt before relying on a
component's acceptance status.

## Second recovery stage

The actual plugin now routes supported TGA groups through three exact raw DDS formats; Win32 GenerateMipMaps=true uses the independently validated triangle mip encoder. Volume strip/mip storage helpers and additional indexed/gray/16-bit TGA formats are available. Dedicated real SDK builds cover 18 assets with and without mips in original/recovered/no-original-target-DLL configurations; see recovery/TEXTURECOMPILER.md for immutable receipts and the workflow evidence-push limitation. PS3 POT automatic cubic resizing is accepted. DXT compression, NPOT resampling and full terrain-atlas compilation remain pending; Xbox 360/Xenon recovery is excluded.

## PNG/BMP and validation batches

POT grayscale/RGB/RGBA PNG, including original RGB16 packing and exact gamma rounding, and BI_RGB 16/24/32 BMP now route through the actual PC/PS3 raw/mip/volume plugin. Original indexed and bitfields rejection behavior is retained. See recovery/TEXTURE_PNG_BMP.md for exact scope and source/run anchors. The production decoder uses recovered C# plus DeflateStream; System.Drawing is fixture-only. Ordinary checkpoints do not start Windows tests; recovery/TEXTURE_TESTING.md describes the one-workflow, one-Windows-job milestone policy. Final RGB16 SDK acceptance is pending collection.
