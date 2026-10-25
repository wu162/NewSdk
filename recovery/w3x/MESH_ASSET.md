# Complete W3DMesh envelope source recovery

`src/Native/W3x/MeshAsset.h/.cpp` implements the typed 96-byte W3DMesh root,
Triangle lists and their individual uint index lists, optional AABTree and
optional Children/Polys blocks, the embedded 28-byte ShaderMaterial, and the
custom 28-byte vertex-data component. `CompileMeshAsset` returns the shared
Tracker's instance/relocation/import buffers. It requires an x86 host; x64
hosts explicitly reject instead of corrupting adjacent 32-bit pointer slots.

Source evidence is the immutable W3X baseline `runs/40162934653-1`, original
DLL SHA-256 `bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`.
The implementation was checked against Plugin.HandleMesh, Marshal(W3DMesh*)
RVA 0x81FC, EndianCorrect(W3DMesh*) RVA 0x4D38, and the corresponding Triangle,
Sphere, BoxMinMax, AABTree and AABTree.Node methods. Original raw IL remains in
`raw-listings.zip`; the mechanical C# method bodies are in `decompiled/-Module-.cs`.

| Root offset | Size | Content |
| --- | --- | --- |
| 0 | 4 | untouched zero word |
| 4 | 4 | custom vertex-data pointer |
| 8 | 4 | geometry enum |
| 12 | 24 | box min/max vectors |
| 36 | 16 | sphere radius then center |
| 52 | 8 | triangle count/pointer |
| 60 | 28 | inline ShaderMaterial |
| 88 | 4 | optional AABTree pointer |
| 92 | 1 | Hidden |
| 93 | 1 | CastShadow |
| 94 | 1 | low byte of SortLevel |
| 95 | 1 | untouched padding |

A Triangle is 24 bytes: uint index count/pointer at 0/4, normal vector at 8,
and distance at 20. AABTree is 16 bytes: polygon-index count/pointer at 0/4,
node count/pointer at 8/12. An AABTree.Node is 32 bytes: min/max at 0/12,
optional Children pointer at 24 and optional Polys pointer at 28. Children
contains Front/Back uints; Polys contains Begin/Count uints. These references
are stored as given, without inventing tree validation or remapping.

Output allocation order matches the original: root, triangle array and each
triangle's index array, shader strings/constants, optional tree root, polygon
array, node array and each node's Children then Polys blocks, finally the
custom vertex component followed by its blob, declaration and bone map.
Empty ordinary marshaled lists allocate no pointer block; the custom vertex
output's empty bone map still allocates/bookmarks its pointer slot. Absent
FXShader leaves all 28 inline bytes zero; an existing node uses the Shader
writer's TechniqueName default. The typed hasShader/hasAabTree flags preserve
those distinctions independently of list emptiness.

All envelope uint/float scalars and list counts are target endian. Float
EndianCorrect uses integer byte swaps, so envelope serialization preserves
float bits rather than applying the converter-specific floating-store NaN
behavior. Vertex declaration length retains the original CD method's host
little-endian write even on consoles. Existing Shader and vertex algorithms
retain their independently documented conversion/rejection behavior.

The bridge parses the original MeshGeometryType table (CameraAligned=2,
CameraOriented=3, Normal=0, Skin=1); an absent attribute leaves the zeroed enum,
while an explicitly unrecognized value uses original fallback 2. The typed
serializer accepts the already parsed uint and does not redo XML lexing.

`tests/W3xMeshAssetTests.cpp` has deterministic three-platform envelope/layout
checks for root attributes and bounds, two triangles with separate index lists,
a three-node tree containing Children and Polys branches, embedded vertices,
zero padding, pointer offsets, target endian, the empty imports buffer, and all
13 pointer relocations plus sentinel. It specifically checks root relocation
order 56,88,4, which follows allocation targets rather than ascending field
offsets. Linux g++ C++11 -Wall/-Wextra/-Werror -O2 compilation and the explicit
x64 ABI rejection pass; actual x86 assertions require the Windows runner.
The local environment lacks the 32-bit standard C++ headers, so no local x86
execution is claimed. Full original ProcessInstance mesh three-buffer
acceptance must be taken from the immutable Windows evidence, not this source
review or the presence of tests. SDK validation was not run.
