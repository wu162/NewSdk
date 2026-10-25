# W3X mesh vertex builder

Source: `evidence/w3x/runs/40162934653-1/decompiled/-Module-.cs`, original
DLL SHA-256 `bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1`.
The corresponding `original.il` is preserved in that run's `raw-listings.zip`.

| Method | Token | Restored interface |
| --- | --- | --- |
| DetermineVertexDataContents | 0x060000C9 | DetermineVertexDataContents |
| DetermineVertexDataLayout | 0x060000CA | DetermineVertexDataLayout |
| ComputeBoneMappingTable | 0x060000CB | ComputeBoneMappingTable |
| BuildVertexDeclaration | 0x060000CD | BuildVertexDeclaration |
| BuildVertexData | 0x060000CE | BuildMeshVertexData |

The C++11 interface in `src/Native/W3x/MeshBuilder.h` accepts typed, already
unmarshaled pipeline lists. It is not the original x86 EASTL or managed ABI.
The result contains the complete interleaved vertex bytes, declaration, count,
stride, and forward bone map. `MeshSerialization.h/.cpp` separately serializes
the isolated 28-byte W3DMeshVertexData root through the shared Tracker. Full
mesh XML marshaling and Plugin.HandleMesh remain separate integration work;
these pure functions do not enable that plugin entry point.

## Source contracts

Mappings retain original input order: all position streams, all normal streams,
color, tangent/binormal only when both exist, all texture streams, bone-index
streams, then bone-weight streams. Missing colors use `(1,1,1,1)` with source
pitch zero. Bone stream emission uses `min(position-stream count, bone-stream
count)`, not vertex count. A single emitted bone stream has no weight mapping.
All source influence lists, including lists not emitted, contribute to a sorted
unique ushort bone map; the inverse map has `maxBone+1` entries and zero gaps.

| Usage | Windows format / bytes | Xbox360 and PS3 format / bytes |
| --- | --- | --- |
| position | 3f32 / 12 | 4f16 / 8 |
| normal, tangent, binormal | 3f32 / 12 | 3s11n / 4 |
| texture | 2f32 / 8 | 2f16 / 4 |
| color | 4u8n / 4 | 4u8n / 4 |
| bone index | 4u8 / 1 per source stream | 4u8 / 1 per source stream |
| bone weight | 4u8n / 1 per source stream | 4u8n / 1 per source stream |

The declaration format strings were checked by dereferencing `s_ElementInfo`
static table pointers into the original PE, rather than assuming D3D names.
Name characters are checked against `RNA.ElementNameToCharCode`: p/n/g/b/t/c/w/i.
Groups align to four bytes only after the last mapping with the same usage.
Xbox360 index and weight offsets use `stride-2*usageIndex+3`; PS3 uses forward
byte order. Declaration collapses each packed group into one field, preserves
the first source usage index, and rounds Xbox group offset down to four.
Formatting is `%c%X:%02X:%s`, with single spaces and uppercase hex indices.

The vertex blob is initially all zero, including packing padding. Converters
run in mapping order. When weights are present, exactly two weight mappings
are required; after both conversions, the second byte is overwritten with
`255-firstByte`. This ordering and the byte-pointer offsets were independently
checked against BuildVertexData IL offsets 0x0194–0x021B. Nonzero input second
weight is still converted before that overwrite, so its conversion rejection
must not be skipped.

## Boundaries and verification

Unknown platform is explicitly rejected, rather than emulating the original
null handler dereference. Typed streams shorter than the first position stream
are rejected to avoid original out-of-bounds reads; extra elements are ignored
for vertex conversion, but extra bone entries still enter the map. Output size
must fit uint32 and declaration must fit the original 512-byte local buffer.
These are defensive typed-interface boundaries, not original exception ABI
claims. Converter nonfinite/overflow restrictions are documented separately.
No unsupported input is silently forwarded to the original DLL.

`tests/W3xMeshBuilderTests.cpp` passes g++ C++11 `-Wall -Wextra -Werror` with
`-O0` and `-O2`: 37 assertions per mode. Coverage includes a complete Windows
golden vertex buffer, all-stream declarations/layouts on all three platforms,
default colors, ignored unpaired tangents, sorted bone maps and ushort maximum,
Xbox reversed packing, complementary weights, min-stream selection, zero
padding, malformed short streams, unknown platform, and declaration overflow.
These are independent layout/golden checks, not original-DLL dynamic
comparison or SDK validation. No SDK execution was performed.

Cross-review of MeshConverters against original source/selected IL checked UV
R4 timing, half thresholds/masked shifts, color R4 multiply and add, blend
weight double multiply/add without intermediate R4, packed direction wrap and
endian output, and ushort inverse-map lookup followed by byte truncation.
Build without fast-math or contraction that changes those expression steps.

`tests/W3xMeshBuilderOracle.cpp` adds a Windows x86 CLR4 original-DLL oracle.
It builds raw pipeline list layouts, invokes the original Push (0x06),
BuildVertexData (0xCE), MakeRelocatable (0x38), and cleanup (0x35/0x37) methods
by metadata token, compares count/stride/vertex bytes/declaration/bone maps,
and compares all three buffers of the isolated 28-byte vertex component
against CompileMeshVertexData. The original Tracker begins in its zeroed
pre-allocation constructor state, then the original Push allocates its root.
1296 fixture combinations span all three platforms, one/two position streams,
zero/one/two normal streams, both/unpaired/no tangent lists, default/explicit
colors, zero/one/three texture streams, and zero/one/two bone streams with
sparse IDs including 65535. This oracle does not constitute complete
HandleMesh, formal plugin integration, or SDK validation. The oracle passed both Windows modes; the later complete mesh component and
original-absent replay also passed. Exact acceptance anchors and limits are
maintained in [RESUME.md](RESUME.md).
