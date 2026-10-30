# stb_image BMP native adapter

Source: `nothings/stb` commit `003501f6d7a8668ce8b6d875e34dd97ad0d5d5b5`.
The complete original `stb_image.h`, including its MIT/Unlicense notices, is
retained under `upstream/`. Source and adapted SHA256 values are recorded in
`PROVENANCE.json`; the full native acceptance preflight checks every file.

The adapted header adds one general policy condition to stb's BMP all-zero
alpha fixup. `STBI_BMP_PRESERVE_ZERO_ALPHA=1` keeps physical alpha0 for the old
D3DX X8 base-level resource. This changes no color decoding, scanline padding
or orientation code. The implementation builds BMP only, without stdio/HDR or
linear conversion, and has no original DLL/recovered decoder callbacks.

The host adapter supports BI_RGB24/32 from core12, INFO40, V2-52, V3-56,
OS2-64, V4-108 and V5-124. It validates original bounds, `biClrUsed` RGBQUADs,
declared offsets and bounded V5 post-raster profiles, then normalizes the
container to INFO40 for actual stb pixel decoding. D3DX consumes raster bytes
immediately after the true DIB plus color table, ignoring the declared file
gap, rendering fields and profile colors. The wrapper preserves that general
rule and uses stb's pixel extraction, channel decoding, row padding and flip.

stb produces RGBA; the host swaps R/B for native BGRA storage. BMP24 selects
A8; BMP32 selects A8 if any alpha byte is nonzero, otherwise X8 while retaining
the base's alpha0 bytes. Shared DirectXTex triangle and existing BOX generation
handle NPOT and missing mips, with explicit X8 packing. Other bit depths,
bitfields, compression and unbounded metadata remain explicitly unsupported.

Full native headers, both allocations, every mip/lock and actual LoadImageA
conversion require original differential D/R plus independent NoOriginal
replay before merge. The production six-DLL package and SDK remain separate.
