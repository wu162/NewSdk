# TextureCompiler recovery — Win32 / PC only

Build on Windows with .NET Framework 4.8 targeting support:

```powershell
dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release
```

The plugin accepts only Win32. Non-PC initialization is rejected. PS3-only mip/cubic encoders and their test/workflow entrypoints have been removed. Original SDK binaries, platform enum definitions and immutable historical evidence remain unchanged.

Implemented PC scope: metadata, image rules/options, full TerrainTextureAtlas packing/DXT5+RGB555 fixed-five-level DDS/runtime and relocation, native buffer helpers, DDS passthrough, declared POT/NPOT TGA/PNG/BMP/JPEG subsets, three raw DDS formats, DXT3/5 compression, full 2D and volume mips, Texture/OnDemandTexture buffers and cdata. No original TextureCompiler DLL fallback.

Remaining PC work: default DXT1/1A undefined-state boundary, undeclared JPEG/other image profiles and metadata, required PC native lifecycle audit, and complete production integration. Atlas entry Debug/Release (48 cases each) and its dedicated 16-asset SDK three-mode MOD passed at source be520b8e / run 40607538234; this is not complete production MOD validation. Production SDK DLL/configuration is not replaced. See [current handoff](../../recovery/TEXTURECOMPILER.md) for scope and exact validation status; [testing policy](../../recovery/TEXTURE_TESTING.md) keeps one batched workflow and at most one Windows job. PC cleanup is validated: affected components and both 600-asset PC SDK profiles passed at source 0ae0efdb / run 40241188546 (overall failure due to missing MSVC in base only); base passed after x86 toolchain setup at source 7786834f / run 40241581283. The retry reused successful components and did not rerun SDK. See the handoff for exact anchors and receipts.

Public-source JPEG: pinned IJG6b with IFAST/fancy and original grayscale L8 NPOT storage passed full component acceptance at699479a9/run40626161229 and3216-asset three-mode real SDK at e8835cfe/run40628208308. The original32 public DXT/STB variants remain failed historical candidates on validation/texture-public-codecs. Subsequent adapted S3 source is now the accepted full PC DXT3/5 colour/alpha backend:705af338/run40649825971 passed complete affected components and867e7d38/run40650835181 passed all9 SDK profiles. See recovery/TEXTURE_PUBLIC_S3.md and its acceptance record.

In an x86 MSVC GitHub Actions shell, build the new required DXT native DLL with `./recovery/scripts/BuildTexturePublicS3.ps1 -Destination <native-output> -Configuration Release`. Ship `Recovered.TexturePublicS3.dll` beside the managed plugin for these PC DXT paths. Consumer test/SDK scripts build and copy it explicitly. Existing independent recovered colour/alpha APIs retain their own helpers, but the new complete/float/partial DXT route does not call them.
