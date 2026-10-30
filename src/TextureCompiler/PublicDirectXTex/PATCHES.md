# DirectXTex PC triangle adaptation

Upstream `microsoft/DirectXTex` at `ec66b78b37389e8373a8e14ccc3cd29be94146b3`.
Unmodified files and MIT notice are retained under `upstream/`; exact original
and adapted file SHA256 values are in `PROVENANCE.json`.

`TexturePublicTriangle.cpp` specializes `CreateTriangleFilter` from `filters.h`
and `ResizeTriangleFilter` from `DirectXTexResize.cpp`. It retains both kernel
halves, wrapped clipped-cell integration, contribution order, scatter, active
row accounting, completed-row writes and row recycling. Bounded vectors replace
the variable-size filter allocation and linked free-row list. DXGI and
DirectXMath IO is replaced by BGRA8 IO for the existing native loader ABI.

Compatibility patches follow original numeric evidence `678db012` /
`40529918922` and the already accepted managed RGBA8 resampler's contract:

- Keep explicit float32 stores around double arithmetic for the old x87
  contribution and scatter operations; no separable intermediate quantization.
- Normalize byte channels with the original stored reciprocal, multiply joint
  XY weights once, then store every accumulated channel as float32.
- Use the old ordered 4x4 serpentine dither and toward-zero float stores in the
  row writer. X8 has no alpha mask; resampling writes its unused byte as zero.
- Keep unchanged-size uploads on the existing physical byte-copy path, retaining
  their unused X8 bytes, and generate missing mips from the uploaded result.

No original DLL or recovered resampler callbacks, output lookup tables or input
labels are used. This candidate needs full native-resource/mip/actual-copy
Debug/Release and NoOriginal differential acceptance before merging. Production
six-DLL packaging and SDK integration are separate gates.
