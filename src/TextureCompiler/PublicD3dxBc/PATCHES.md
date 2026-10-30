# Public D3DX BC1/2/3 continuation candidate

Fixed source: Microsoft DirectXTex `ec66b78b37389e8373a8e14ccc3cd29be94146b3`.
Original BC.cpp/BC.h and MIT notice are under `upstream/`. All source/adapter
fingerprints are in PROVENANCE.json. BC.cpp's only source patch replaces its
private whole-library precompiled header with the minimal `TextureBcP.h`;
BC.h is unmodified. The build explicitly selects scalar DirectXMath and x86
x87 strict arithmetic. Windows SDK DirectXMath is a build prerequisite.

The isolated DLL exposes actual public D3DX BC1/2/3 encoders and decoders via
normalized-float block and image interfaces. Image IO preserves caller pitches,
crops partial decode blocks, and offers explicit wrap/clamp partial-block
packing for later original-contract comparison. All encode input is validated
before the first output write. There is no original DLL, recovered codec
callback, gold lookup or input-label dispatch.

This is an unbuilt/unvalidated continuation checkpoint, **not restored DDS
support**. The current accepted/pending native loader continues to reject
compressed files. No production module calls this candidate. No new validation
matrix was launched for this checkpoint.

Next compatibility work must map the original XG flags/threshold, retain
original decode/store precision, compare endpoint/index ties, and consume this
core from the complete compressed DDS loader including provided small tails,
missing mips, and actual compressed-to-raw CopySurface. Use the immutable
`native-loader/40999035840-1/captures.zip` as diagnostic evidence: the complete
105 successful compressed resources are in Debug.unsupported-original and
Release.unsupported-original (not business acceptance gold). In those traces
the 1x1 DDS resources have a 4x4 native base and three mip levels; decoding the
input header alone loses that resource contract. Base axes clamp to at least
four before rounding. Do not treat original compressed probe bytes as stable
until repeats and all actually consumed output are verified.

Only complete original/gold byte equality, D/R, independent NoOriginal and
the complete affected native consumer chain may justify merge. SDK activation
remains a separate integration gate. First-candidate mismatch requires general
compatibility changes; it is not a reason to abandon public-source reuse.
