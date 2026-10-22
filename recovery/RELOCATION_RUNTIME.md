# Recovered relocation runtime

`src/Native/Relocation.h` and `.cpp` reconstruct the relocation algorithms used by
the two original plugins. `src/Bridge/RelocationBridge.cpp` provides a C++/CLI
ownership/interop surface. It does not load or forward to either original DLL.
The bridge is an integration component, not a replacement compiler plugin.

## Evidence and original x86 layout

The source of truth is the committed `<Module>` IL, method metadata and mechanical
C# rendering from these exact DLLs. Tokenizer method tokens:

| Operation | Token |
|---|---|
| Chunk destruction / allocation | `06000007` / `06000008` |
| Push / GetMemberOffset / ExchangePointers | `06000009` / `0600000A` / `0600000B` |
| FixupMemberOffsets | `0600000C` |
| Tracker construction / destruction | `0600000D` / `0600000E` |
| MakeRelocatable / Allocate / AddReference | `0600000F` / `06000010` / `06000011` |
| AlterBookmark / BookmarkCompare | `060002F9` / `060002FA` |
| BookmarkMemberOffset | `0600081A` |

XmlCompiler has Chunk destruction/allocation at `06000007`/`06000008`, Push at
`06000009`, ExchangePointers at `0600000A`, tracker construction/destruction at
`0600000B`/`0600000C`, MakeRelocatable/Allocate/AddReference at
`0600000D`/`0600000E`/`0600000F`, and AlterBookmark/BookmarkCompare at
`0600036B`/`0600036C`. It has **no** GetMemberOffset, BookmarkMemberOffset or
FixupMemberOffsets global methods. Those are needed for Tokenizer's procedure
tables, not XmlCompiler's ordinary data serialization.

| Original Tracker byte offset | Meaning |
|---|---|
| 0 | Sum of aligned block sizes, uint32 |
| 4 | EASTL stack of active block indices, 16 bytes |
| 20 | EASTL vector of blocks, 16 bytes |
| 36 | EASTL vector of pointer bookmarks, 16 bytes |
| 52 | EASTL vector of external-reference bookmarks, 16 bytes |
| 68 | EASTL vector of member-offset bookmarks, 16 bytes |
| 84 | Pointer to flattened block-offset array |

A block is `{data pointer, aligned byte size, element size}` (12 bytes).
A bookmark is `{owning block index, field byte offset, target block / reference
index / member pointer}` (12 bytes). Chunk is six uint32 words:
`{instance pointer, instance size, relocation pointer, relocation size,
imports pointer, imports size}` (24 bytes).

The recovered class uses STL containers and RAII, not the original VC8 EASTL
container ABI. Never cast an original Tracker pointer to the recovered class.
Pointers held in asset structures and all serialized offsets remain 32 bits;
the supported integration target is Windows x86.

## Behavior that must survive serializer integration

* Allocate zeroes `(count * elementSize + 3) & ~3` bytes and appends a new block;
  it never merges blocks. Each block is independently padded to four bytes.
* Push creates and activates a block. A first/root Push writes the caller's root
  pointer without creating a relocation. A nested Push bookmarks a field in the
  previously active block. A nested null pointer-location still creates and
  activates a block, but returns null. Pop is inlined in the originals; its
  operation is simply removing the active stack entry.
* AddReference records the active-block field and writes its original value.
  MakeRelocatable writes `value + 1`, including uint32 wraparound. Import records
  identify the field locations, not the reference values.
* MakeRelocatable concatenates blocks in allocation order. Pointer fields and
  relocation/import record locations include `absoluteInstanceOffset`.
  Nonempty relocation/import arrays end with `FFFFFFFF`; an empty array is
  length zero, not a sentinel-only array. Ordinary scalar endianness is the
  serializer's responsibility; this runtime converts the offsets it writes.
* BookmarkCompare orders by **block index, then bookmark value**, not field
  offset. Equal values are possible. The original EASTL median partition,
  28-entry threshold, insertion pass and heap child preference are reconstructed
  rather than delegated to an implementation-dependent STL sort.
* GetMemberOffset scans allocations including alignment padding and leaves its
  output unchanged on a miss. Member fixup runs after flattening the token table
  and writes `token table byte size + data-owner member offset` in target endian.
  It does not add a relocation entry.
* ExchangePointers changes bookmark field offsets within the current block's
  element-size ranges. It does **not** exchange bytes or change target pointers;
  a sorting caller must move the actual elements separately.

## Validation and boundaries

`scripts/TestRelocation.ps1` builds the native/C++/CLI component and the x86 C#
oracle on Windows. The oracle loads each original DLL in a separate process and
calls its real global methods using reflection and boxed native pointers.
It compares instance, relocation and import arrays byte for byte for generated
allocation/reference graphs, absolute bases 0 and 1260, both endian modes,
equal-key sorting around 28 entries and through 257 entries, pointer/reference
exchange, and Tokenizer's cross-tracker member fixup and member lookup.
Explicit expected offsets additionally test block padding and lookup failure.

The XML-only test cases explicitly report the absence of token-table methods;
they are not silently emulated with recovered code on the oracle side. See
`evidence/relocation` for actual counts, exit codes and compiler output.

These tests are runtime tests, **not MOD compilation with replacement DLLs**.
The allocator's CRT internals, original container capacities and exception ABI
are not reproduced. Invalid pointers, stack underflow, out-of-memory behavior
and overflowing allocation sizes are outside the validated serializer contract.
Unresolved member fixup throws explicitly instead of emitting an uninitialized
offset as the original unchecked path could do. No support for x64 integration
is claimed. Full Node/XML marshaling, per-type serializers and Plugin entry
points still need reconstruction.
