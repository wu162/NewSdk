# TextureCompiler recovery

Implementation source lives in this directory. Original DLL, IL/decompiler exports,
component oracles and immutable receipts live in `SDK/tools` and `recovery`.

Build managed code on Windows with .NET Framework 4.8 targeting support:

```powershell
dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release
./recovery/scripts/TestTexture.ps1
```

The test script additionally needs x86 MSVC (`/clr`) and .NET Framework csc.exe.
The production SDK DLL/configuration is not replaced by this project.

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

Full `LoadImageA` decoding/conversion beyond the declared TGA subset, nvDDS compression/mipmap/resizing,
XenonBundler/XGraphics texture conversion and complete plugin native lifecycle
are not implemented. TerrainTextureAtlas is not yet wired to a complete codec
pipeline. Unsupported entry paths throw; there is no original-DLL fallback.

No full SDK MOD compilation or production replacement is claimed. See
`recovery/TEXTURE_CONTRACT.md`, `TEXTURE_IMAGE.md`, `TEXTURE_PACKER.md`,
`TEXTURE_NATIVE.md` and the latest exact validation receipt before relying on a
component's acceptance status.
