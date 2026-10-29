# TextureCompiler recovery — Win32 / PC only

Build on Windows with .NET Framework 4.8 targeting support:

```powershell
dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release
```

To build the complete PC plugin package, use an x86 MSVC developer shell with
the .NET 8 SDK and .NET Framework 4.8 targeting support:

```powershell
./recovery/scripts/BuildTextureCompiler.ps1 -Destination ./work/texture-pc-package -Configuration Release
```

Use a new, empty output directory. The package contains the managed plugin,
PublicS3 and separate IJG6b JPEG/PPM native backends, the two independent colour/alpha API helpers,
source/runtime fingerprints, build logs and upstream notices/provenance. It
does not change the source SDK. Configure the existing host with the plugin
type recorded in `manifest.json`; its README lists deployment prerequisites.
The dedicated BMP SDK validates this actual package, separately from a build
succeeding. Current acceptance status remains in the recovery handoff.

The plugin accepts only Win32. Non-PC initialization is rejected. PS3-only mip/cubic encoders and their test/workflow entrypoints have been removed. Original SDK binaries, platform enum definitions and immutable historical evidence remain unchanged.

Implemented PC scope: metadata, image rules/options, full TerrainTextureAtlas packing/DXT5+RGB555 fixed-five-level DDS/runtime and relocation, native buffer helpers, DDS passthrough, declared POT/NPOT TGA/PNG/BMP/JPEG subsets, three raw DDS formats, DXT3/5 and bounded transparent-block DXT1A compression, full 2D and volume mips, Texture/OnDemandTexture buffers and cdata. No original TextureCompiler DLL fallback.

Transparent-block DXT1A accepts only Production=71 inputs with at least one actual sample at alpha <= 0.5 in every encoded block, including every mip level and volume slice. Padding holes do not initialize this contract. All-transparent blocks, partial mapping, original single-colour level4 results, endpoint ordering and index remapping are preserved. Blocks without an actual transparent sample are explicitly rejected. Original image-loader alpha rules still apply (in particular, an all-zero TGA alpha channel becomes opaque). Exact current component and dedicated 320-asset SDK anchors are recorded in [the bounded DXT1A acceptance](../../recovery/TEXTURE_DXT1A_TRANSPARENT.md).

Current PC boundaries: default DXT1/opaque DXT1A are demonstrably stack-dependent in the original (three-process3840-DDS probe); undeclared image/metadata profiles, native pointer ABI and production installation remain separate. Current managed pipeline resource ownership is statically audited. Complete unmodified mymod and an overlay using all150 DDS+7 TGA repository art passed three-mode full-output/trace acceptance at5a1b8e1d/run40657712486 (384 actual texture calls per overlay mode). See recovery/TEXTURE_MYMOD_ACCEPTANCE.md and TEXTURE_PC_LIFECYCLE.md. Atlas entry Debug/Release (48 cases each) and its dedicated 16-asset SDK three-mode MOD passed at source be520b8e / run 40607538234; this is not complete production MOD validation. Production SDK DLL/configuration is not replaced. See [current handoff](../../recovery/TEXTURECOMPILER.md) for scope and exact validation status; [testing policy](../../recovery/TEXTURE_TESTING.md) keeps one batched workflow and at most one Windows job. PC cleanup is validated: affected components and both 600-asset PC SDK profiles passed at source 0ae0efdb / run 40241188546 (overall failure due to missing MSVC in base only); base passed after x86 toolchain setup at source 7786834f / run 40241581283. The retry reused successful components and did not rerun SDK. See the handoff for exact anchors and receipts.

Public-source JPEG: pinned IJG6b with IFAST/fancy and original grayscale L8 NPOT storage passed full component acceptance at699479a9/run40626161229 and3216-asset three-mode real SDK at e8835cfe/run40628208308. The original32 public DXT/STB variants remain failed historical candidates on validation/texture-public-codecs. Subsequent adapted S3 source is now the accepted full PC DXT3/5 colour/alpha backend:705af338/run40649825971 passed complete affected components and867e7d38/run40650835181 passed all9 SDK profiles. See recovery/TEXTURE_PUBLIC_S3.md and its acceptance record.

In an x86 MSVC GitHub Actions shell, build the new required DXT native DLL with `./recovery/scripts/BuildTexturePublicS3.ps1 -Destination <native-output> -Configuration Release`. Ship `Recovered.TexturePublicS3.dll` beside the managed plugin for these PC DXT paths. Consumer test/SDK scripts build and copy it explicitly. Existing independent recovered colour/alpha APIs retain their own helpers, but the new complete/float/partial DXT route does not call them.

On validation/texture-public-ppm, P3/P6 is routed through the adapted pinned IJG
reader in Recovered.TexturePpm.dll. Its independent 8438-input D/R full-gold
component passed at 782f6b97/run40837034188; actual plugin consumers and the
expanded six-DLL package are still under validation. The dedicated PcPpm SDK
is prepared for 15712 real assets per mode but has not run. This branch is not
merged into recovery/texturecompiler-source until full consumers/SDK pass.
