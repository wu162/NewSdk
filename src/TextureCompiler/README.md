# TextureCompiler recovery — Win32 / PC only

Build on Windows with .NET Framework 4.8 targeting support:

```powershell
dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release
```

The plugin accepts only Win32. Non-PC initialization is rejected. PS3-only mip/cubic encoders and their test/workflow entrypoints have been removed. Original SDK binaries, platform enum definitions and immutable historical evidence remain unchanged.

Implemented PC scope: metadata, image rules/options, in-memory atlas packing, native buffer helpers, DDS passthrough, declared POT TGA/PNG/BMP subsets, three raw DDS formats, full 2D and volume mips, Texture/OnDemandTexture buffers and cdata. No original TextureCompiler DLL fallback.

Remaining PC work: DXT compression, NPOT resampling, remaining image profiles, complete TerrainTextureAtlas pipeline and required PC native lifecycle. Production SDK DLL/configuration is not replaced. See [current handoff](../../recovery/TEXTURECOMPILER.md) for scope and exact validation status; [testing policy](../../recovery/TEXTURE_TESTING.md) keeps one batched workflow and at most one Windows job. Historical acceptance anchors do not certify this cleanup until its affected PC validation finishes.
