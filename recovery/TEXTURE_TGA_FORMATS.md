# Additional TGA format group

This is a separate, initially unaccepted implementation/validation group. Existing `TextureTgaDecoder.cs` and its 1,675-input accepted raw24/raw32/metadata group are unchanged. New code is `src/TextureCompiler/Managed/TextureTgaAdditionalFormats.cs`; no original-DLL delegation, default-loader wiring, Plugin changes, or workflow changes are made here.

The candidate implements packed true-color 16-bit conversion, 8-bit grayscale and 16-bit grayscale/alpha, and mapped TGA with 8/16-bit pixel indices and 15/16/24/32-bit palette entries. Raw and row-contained RLE packets, four directions, IDs and palette starting indices are handled. Footer/extension, interleaving and nonzero coordinate metadata remain outside this new group until independently checked. Candidate scaling/alpha/palette policies are provisional until the exact original LoadImageA probe establishes them.

`recovery/tests/TextureTgaFormatsOracle.cs` reuses the accepted original-loader test technique: guarded original DLL SHA, original VC8 malloc/free thunks, original Initialize, 24-byte nvImage ctor/dtor and original module LoadImageA, with full pixels/dimensions/hasAlpha and rejection type/message/ErrorCode comparisons. It does not run the existing raw24/raw32 group.

The independent matrix has exactly **4,040 fixtures**: 4,032 matrix fixtures plus 8 exhaustive packed-color fixtures. The base matrix uses sizes 1×1, 4×2, 8×4; raw/RLE; four directions; four alpha patterns; ID lengths 0/7. For each combination it exercises true16 attributes 0/1, gray8, gray16 attributes 0/8, and four palette entry depths × two index depths × palette first index 0/5. Palette entries are structurally valid and input indices stay within the declared four-entry range. RLE packets stay within scanlines so this group isolates format semantics rather than repeating the already investigated cross-row issue. Eight additional 256×256 raw16 fixtures contain every one of the 65,536 possible packed 16-bit words, under attributes 0/1 and all four directions, to exhaust color expansion and the alpha bit. True-color depth 15 is not enabled by CanDecode; palette depth 15 remains in the tested palette matrix.

Build x86 CLR4 with `/unsafe /r:Recovered.BinaryAssetBuilder.TextureCompiler.dll`, legacy-v2 activation and unchanged original support DLLs alongside the host. Run:

```
TextureTgaFormatsOracle.exe original.dll transcript.txt fixtures-dir --probe
```

`--probe` records all differences without acceptance. `--strict` compares every input and fails the first mismatch, including unexpected acceptance/rejection; it cannot turn unsupported originals into pixel successes. Full matrix counts remain enforced in either mode. Source compile, independent local checks or probe exit zero are not original-behavior acceptance.

Original DLL SHA256 remains `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`. Precise original results, accepted scope and sourceCommit/runId will be recorded here after Windows evidence is collected. No full SDK or non-TGA image-decoding claim is made.
