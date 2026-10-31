# Built-in PC texture compiler

`Recovered.Compiler.Texture.Plugin` is compiled into `NewModCompiler.exe`. It
implements `Texture`, `OnDemandTexture` and `TerrainTextureAtlas` without loading
`BinaryAssetBuilder.TextureCompiler.dll` or any replacement codec DLL. The S3
native implementation is linked into the same EXE and called through
`recovery/src/Bridge/TextureCompiler.cpp`. Texture work remains serialized because
the public S3 implementation contains process-wide mutable encoder state.

Common input/output coverage:

| Input | Handling |
| --- | --- |
| DDS, including uppercase `.DDS` | Complete bytes copied without recompression, preserving existing cube/volume DDS |
| TGA | Recovered raw/RLE true-color, grayscale and indexed decoders |
| PNG, BMP, DIB | Recovered direct-color decoders and managed BI_RGB indexed BMP/DIB (1/4/8-bit); other standard PNG/BMP variants use Windows GDI+ |
| JPEG, GIF, TIFF | Windows GDI+ decoder; GIF/TIFF use the first frame |
| PPM | Managed P3/P6 reader, including 8/16-bit big-endian P6 samples |

Raster outputs support `A8R8G8B8`, `X8R8G8B8`, `R5G6B5`, `DXT1`, `DXT1A`,
`DXT3` and `DXT5`, with optional mipmaps. The existing alpha inference is retained:
default DXT1 becomes DXT3 for constant/nonopaque or one-bit alpha, or DXT5 for
other alpha. NPOT raster images are resized to power-of-two dimensions by the
recovered resampler. `VolumeTexture` accepts a horizontal strip of square slices.
Raster `CubeTexture` reports the existing error; prebuilt cube DDS is preserved.

The reference raw DDS writers, triangle filtering, atlas packing, buffer
relocations and OnDemand cdata layout are retained. The public S3 source retains
DXT3/5 float and partial-block handling. Managed DXT1/DXT1A provide deterministic,
standard BC1 output for common opaque and alpha inputs, including blocks the
reference recovery branch did not support. These new BC1 bytes and GDI+/PPM
expanded inputs do not claim byte identity with the legacy native compiler.
Plugin version `4300` and new processing hashes invalidate old texture cache
entries while leaving runtime type hashes and asset layouts unchanged.

Source origin: [`recovery/texturecompiler-source` at
`b30dffac5aeb027cf37a6dac0cd47beafe9fbea0`](https://github.com/wu162/NewSdk/tree/b30dffac5aeb027cf37a6dac0cd47beafe9fbea0/src/TextureCompiler).
The recovered decoder/raw/mip/packer/catalog source was copied from its
`Managed` directory. `Plugin.Metadata.cs`, `Plugin.Dds.cs`, `Plugin.Images.cs`,
`TextureImageLoader.cs`, `TextureDdsEncoder.cs` and the S3 call boundary were
adapted for this monolithic host. `TextureBlockCompression.cs` is the independent
managed BC1 encoder; `TextureIndexedBmpDecoder.cs` decodes standard indexed
BMP/DIB palette, packed indices, padded rows and top-down/bottom-up storage. Public S3 upstream notices, fingerprints and compatibility
patch notes are preserved in `recovery/src/Native/Texture/PublicS3`.

Build the S3 native translation unit with `/arch:SSE2 /fp:strict`. Windows x86
Actions must validate the linked plugin, common formats and the existing legacy
comparison/SDK checks; a successful source build alone is not a byte-comparison
receipt. JPEG output depends on the Windows GDI+ codec and may differ from the
reference IJG codec. No original-DLL fallback is present.
