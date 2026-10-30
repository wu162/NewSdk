# Public D3DX BC1/2/3 continuation candidate

Fixed source: Microsoft DirectXTex `ec66b78b37389e8373a8e14ccc3cd29be94146b3`.
Original BC.cpp/BC.h and MIT notice are under `upstream/`. All source/adapter
fingerprints are in PROVENANCE.json. BC.cpp replaces its private whole-library precompiled header with minimal
`TextureBcP.h`. After original diagnostic 165184da/41007602862, compatibility
patches restore BC2/3 three-color decode, A4 expansion through byte17/255, the
old OptimizeAlpha reversed gradient and pD8 last coefficient8/7 (original RVA
2851b/table9d250). The wrapper unpremultiplies BC2/3 via a stored float reciprocal
then stored products/clamp, following every applicable captured original RGB
channel. BC3 alpha interpolation now explicitly stores the right weighted product
before its extended left multiply/add, matching all8192 original alpha-channel
bit patterns in the immutable41007602862 capture by numeric analysis. This
latest source patch is not yet Actions-validated and cannot inherit the prior
source commit's diagnostic result. The build explicitly selects scalar DirectXMath and x86
x87 strict arithmetic. Windows SDK DirectXMath is a build prerequisite.

The isolated DLL exposes actual public D3DX BC1/2/3 encoders and decoders via
normalized-float block and image interfaces. Image IO preserves caller pitches,
crops partial decode blocks, and offers explicit wrap/clamp partial-block
packing for later original-contract comparison. All encode input is validated
before the first output write. There is no original DLL, recovered codec
callback, gold lookup or input-label dispatch.

An isolated `TexturePublicBcCreateFromDds` consumer now constructs owned PC
resources for ordinary POT DXT1/3/5 bases with both axes >=4. It checks all
provided storage, copies complete blocks, decodes/re-encodes provided small
tails, and generates each missing mip by decoding the ACTUAL previous stored
compressed level, applying ordered float BOX and re-encoding. Lock/resource
RAII preserves ownership on failure. Encoder flags/threshold/partial policy
remain explicit until original mapping is proved. Small base axes and NPOT
remain explicit boundaries; no approximated clamp/resize is enabled.

The actual compressed-to-raw consumer now uses public BC decode and the
accepted native float-row conversion, preserving source endian/swizzle and
rect/pitch/guard semantics. The diagnostic compares both masked LoadImageA and
physical-endian RGBA for EVERY mip plus all native storage; it still does not
provide formal NoOriginal acceptance.

This is a component-unvalidated continuation checkpoint, **not restored DDS
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

Initial diagnostic 41007602862 completed with identical D/R captures (62,109,936
bytes), 1536 blocks and168 DDS, original repeats stable. flags0 matched all
DXT1/DXT3 encodes; DXT5 alpha had504 block differences. BC1 float decode matched
all512; BC2/3 had511 differences each, now under compatibility adaptation.
Its success proves comparison completed only; no policy passed the whole DDS
consumer. The updated actual-copy consumer requires a new diagnostic before
any claim. No production loader or accepted NPOT/BMP input is changed.

Diagnostic d50b04a9/41009041890 completed D/R with identical89,979,984-byte
captures and original repeats stable. All1536 complete BC1/2/3 encodes match
under one fixed flags0 policy (not just alpha subblocks). Actual-copy and full
DDS remain mismatched (best flags0/wrap86 of168); decode had976 block bit
differences, including A4 byte normalization and BC3 alpha interpolation.
The latest unvalidated source now forces the exact stored float byte inverse
0.0039215688593685627 with an explicit store before palette/unpremultiply;
this avoids x87 retaining the 1.f/255.f division until multiplication. Together
with the right-product store, it needs current-source Actions verification.
No extra diagnostic or final matrix was triggered for the saved precision fix.


2026-10-30 continuation: ce16e990/41010488699 verifies all1536 complete XG
float decode blocks and all1536 flags0 encode blocks in D/R, with identical
capture3c56bb03...; full DDS still86 wrap mismatches. New unvalidated adaptation
separates file-codec A4/15 and weighted-sum BC3 alpha from XG byte-normalized
alpha, and includes the original D3DX premultiply wrapper on file encodes.
The original static global at RVA b8914 is1: D3DX decodes unpremultiply and
encodes premultiply (28385/4c901), independently of XG's flags0x12.
BC1 file encoding uses the original0.5 alpha threshold. The diagnostic retains
all old XG and full-resource comparisons and adds1536 complete direct file
codec comparisons (including original repeated no-dither/dither outputs) to
locate any remaining general-rule differences. This checkpoint is unvalidated;
no production backend or acceptance boundary changes.


2026-10-30 second implementation checkpoint (unvalidated): direct file codec
2b79fee9/41011190751 matches all1536 decode and no-dither encode blocks; dither
has6 colour differences (all in premultiplied uniform planes), with D/R
identical53e01cd9... captures. Full DDS flags0 improves141/168. The current
source adds the original x87 toward-zero phases around file colour quantization,
565 endpoint packing and index/error diffusion (28e46/2842b; caller CW restored).
It also separates default file upload RGB/alpha dither from BOX mip generation.

The isolated default file ABI now consumes small/NPOT DDS: base axes clamp4 and
round upward to POT; each provided level is decoded with the file codec and
resized using the accepted upstream triangle specialization, complete aligned
blocks remain verbatim, and missing levels decode the actual preceding stored
blocks before BOX/encode. New float-IO triangle files derive from the unchanged
accepted PublicDirectXTex source, recorded in PROVENANCE.json. The diagnostic
adds all provided-level counts for30 small/NPOT/POT shapes through the actual
file ABI. Its result remains pending; production compressed loading still rejects.
XG compressed-to-raw now honors its explicit flag2 unpremultiplication bit.


2904039f/41011903071 stopped in Debug on the5x3 default file input before
completing the expanded matrix. The complete preceding file block captures
show0 decode/no-dither/dither differences: the6 grey cases are fixed by the
rounding-phase adaptation. This is not a passed matrix. The failure exposed
triangle alpha accumulating a few ulps above1; current file encoding retains
those finite floats as original D3DX does, with safe integer-conversion bounds,
instead of clamping or rejecting an otherwise valid file. The current source
also applies original toward-zero phases to file BC1/2 alpha dither and BC3
A8 quantization, endpoint packing and bitmap diffusion. Full resource and
NoOriginal acceptance remain pending; no production route is changed.
