# TGA decoder subset and original-load oracle

`src/TextureCompiler/Managed/TextureTgaDecoder.cs` is an independently written CPU decoder for a narrow TGA subset. It is opt-in through `TexturePacker(TextureTgaDecoder.Load)` and is not installed as the default loader. Acceptance requires the exact Windows original-load differential receipt; a successful source build or `--probe` run is not acceptance.

The candidate supports true-color image types 2 and 10 (uncompressed and RLE), 24-bit BGR and 32-bit BGRA storage, all four combinations of horizontal/vertical file direction, image IDs, raw/RLE packets, and 128-pixel run boundaries. The returned `TexturePacker.Image` uses a top-left origin and little-endian packed RGBA words. Original probe 40194142937-1 establishes the alpha rule: 32-bit images whose entire alpha channel is zero are converted to alpha 255 and report `HasAlpha=false`. Other 32-bit images, including entirely opaque alpha 255, preserve their bytes and report `HasAlpha=true`. 24-bit images synthesize alpha 255 and report false. Descriptor attribute counts 0 and 8 follow this same tested rule.

Explicitly rejected inputs include mapped/grayscale/15-/16-bit TGA, interleaving, nonzero origin/palette declarations, unsupported attribute counts, TGA 2.0 extension/developer metadata, unexplained trailing bytes, zero dimensions and images at or above the 64 MiB processing bound. Truncated IDs, headers, pixels and RLE packets, and runs exceeding the image are rejected explicitly. Rejection of inputs outside this subset does not claim original rejection behavior.

Original evidence: immutable run `40191971356-1`, original DLL SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`. Original module method `06000017` calls the statically linked `D3DXCreateTextureFromFileA`, obtains `XGGetTextureDesc`, derives `hasAlpha` from the loaded texture's format, locks level 0, converts with `XGCopySurface` to format `0x1A200006` and pitch `width * 4`, then unlocks/releases. The native destination image is 24 bytes on x86. Its ctor/dtor are methods `0600002C` / `0600002D`.

`recovery/tests/TextureTgaOracle.cs` guards the original SHA and allocates original structs using the original module's VC8 malloc/free import thunks at RVA `4E74C` / `4E758`, after checking their `FF25` instruction prefixes. It calls original Initialize on Win32, the real image ctor, the real LoadImageA and image dtor, and compares complete pixel arrays, dimensions, and `hasAlpha`. No original image storage is freed using this host's unrelated CRT.

Fixtures: 1/2/4/8 widths and 1/2/4 heights, 24/32 bits, raw/RLE, four directions, four alpha patterns and image-ID lengths 0/7; plus long RLE runs at widths 64/128/256 and all direction/depth combinations; plus 32-bit descriptor attribute count zero. Total 1,608 generated fixtures, including 16 exact empty-footer cases. The original and candidate receive exactly the same generated files.

Build on Windows as x86 CLR4 with `/unsafe /r:Recovered.BinaryAssetBuilder.TextureCompiler.dll`, a legacy-v2-activation exe config, and recovered/support DLLs beside the host. Run:

```
TextureTgaOracle.exe originalTextureCompiler.dll transcript.txt fixtures [--probe|--strict] [real-fixtures-directory]
```

An optional fifth argument recursively adds every real `*.tga` fixture; the explicit fourth argument `--strict` is equivalent to default strict mode. This strict mode rejects every unexpected mismatch/failure. It separately asserts the explicit unsupported profile described below; those entries do not increment pixel-equivalence passes. For evidence gathering only, append `--probe`: it records all mismatches and prints `PROBE`, never `PASS`, and may exit 0 even with differences. The probe is useful for identifying original alpha/orientation policy before claiming support. No SDK or texture-compression acceptance is implied.

Implementation research sources, not claims of binary equivalence:

- [stb_image](https://github.com/nothings/stb/blob/master/stb_image.h), [MIT/Unlicense](https://github.com/nothings/stb/blob/master/LICENSE). Its TGA16 alpha and horizontal-direction behavior differs from the scope intended here.
- [DirectXTex TGA](https://github.com/microsoft/DirectXTex/blob/main/DirectXTex/DirectXTexTGA.cpp), [MIT license](https://github.com/microsoft/DirectXTex/blob/main/LICENSE). Its default all-zero-alpha fixup and TGA extension handling demonstrate why original alpha semantics require explicit comparison.


## Real main-branch inputs and safe footer boundary

The exact checked-in real files are identified by the separate `recovery/tests/fixtures/texture-main/manifest.json`; this decoder change does not modify them. Their observed structures are:

- `map_mp_2_feasel10_ep1_art.tga`: 512×512, 24-bit raw, descriptor 0. Pixel data ends at offset 786450, followed by exactly 26 bytes containing extension offset 0, developer offset 0, and the full `TRUEVISION-XFILE.\0` signature. The decoder accepts this footer and performs no alpha/gamma metadata interpretation.
- `apt_tacticalhudbuildqueuepage_1.tga`: 1024×512, 32-bit raw, descriptor 8, image ID length 1. Bytes after the pixels contain two extension/footer chains; the last footer references a 495-byte extension at offset 2097692. Explicitly unsupported until exact extension semantics are recovered.
- `apt_tacticalhudmaincommandbar_1.tga`: 512×256, 32-bit raw, descriptor 0. Has a 495-byte extension at offset 524306, additional trailing bytes, and an ending `TRUEVISION-XFILE.` signature missing its expected NUL. Explicitly unsupported instead of guessing metadata intent.

A footer is accepted only if all remaining bytes are exactly the 26-byte standard footer, both offsets are zero, and its 18-byte signature matches including NUL. Footer acceptance does not imply support for arbitrary trailing data or TGA 2.0 extensions. Unsupported originals retain complete pixel output and alpha metadata and remain non-accepted as equivalent decodes. Strict mode asserts that exactly the declared boundary entries are rejected and separately reports them as `unsupportedCount`; it rejects unexpected unsupported inputs. Original decoder failures are compared against the candidate exception type/message/ErrorCode and counted as `rejectionMatches`, rather than being skipped.


## Probe-driven alpha and RLE correction

Immutable original probe `40194142937-1` tested all 1,611 inputs in both Debug and Release, with original source anchor `765cf26f418e4fe503603c9c7515ea16c8fc9e67`. It exposed 252 original RLE failures and the all-zero-alpha normalization above. No fixtures were removed for the correction.

The 252 original failures are exactly RLE packets whose repeat run crosses a scanline boundary: 240 main-matrix cases, 8 long-run cases at width 64, and 4 empty-footer cases. Original errors are `BinaryAssetBuilder.BinaryAssetBuilderException`, ErrorCode `BadTextureFile`, message `D3DX cannot load the texture from disk.` The decoder explicitly rejects scanline-crossing packets with that same error. Full independent negative assertions retain all 252 cases.

Two additional scanline-crossing footer fixtures are accepted but misdecoded by the old D3DX loader: `empty-footer-24-rleTrue-direction16` and `empty-footer-24-rleTrue-direction32`. The captured pixels include `FF525545`/`FF564953`, bytes from the `TRUEVISION` footer consumed as pixel data after the old per-row RLE logic discards excess run pixels. These are not promoted as useful supported decoding behavior; they are retained as two explicit unsupported boundaries. Together with the two real extension-bearing images, they make exactly four original-success/candidate-rejected entries.

The strict receipt must therefore distinguish **1,355 complete pixel/alpha matches**, **252 matching original rejection cases**, and **4 declared unsupported original-success cases** for the full 1,611-input profile. It must not call this 1,611 successful image decodes. Running without the real directory retains all 1,608 generated cases, including the same 252 negatives and two unsupported footer cases; pixel matches are then 1,354. Strict mode checks total counts, exception signatures, all boundaries, and every successful pixel/alpha array. The one real image with an empty footer participates in pixel comparison; the other two remain unsupported.
