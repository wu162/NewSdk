# WeaponTemplate scalar and duration subset

The supported entry point is `Recovered.WeaponCompiler.ProcessInstance` in
`src/Bridge/WeaponCompiler.cpp`; the native implementation is
`src/Native/WeaponTemplate.h` / `.cpp`. It uses the previously validated
`ScalarParsing`, `DataInitializers`, `XmlNodeAdapter` and `Relo::Tracker`.
There is no original compiler forwarding. This component references SDK Core
for `InstanceDeclaration` / `AssetBuffer`, and does not replace either compiler.

## Implemented scope

77 root attributes: floats, angles, times, percentages, signed integers,
36 byte-sized Boolean fields, five scalar enum groups, and five bitset attributes
(including both anti-mask attributes).
The explicit `WeaponField` table lists every supported field, default text,
byte offset, procedure name/type hash and token treatment in original order.
`WeaponValue::Absent` marks 27 non-scalar fields. Four are now supported as
RangeDuration child elements; the other 23 must be absent. All 27 names are
still rejected as root attributes. Unsupported root input, including an empty string,
is rejected.
Other child elements, unknown attributes and namespaced attributes are rejected
by the partial component, except namespace declarations and the root `id`.
This deliberately narrows the supported input domain instead of claiming
full original-plugin acceptance of unknown XML.

The serialized root occupies 328 bytes. Its first word is reserved. Scalar
and pointer/list fields occupy bytes 4–291; Boolean fields occupy bytes
292–327. The main procedure table contains 105 entries including the sentinel.
Empty Nuggets, ScatterRadiusVsType and IncompatibleAttributeModifier lists
each emit a 12-byte count table even with no payload. For the scalar-only subset,
the token prefix is 1,296 bytes and InstanceData is 1,624 bytes; RelocationData
and ImportsData are empty. Member offsets include the token prefix.
Win32 emits little endian; all other platform IDs emit big endian.

Four optional child blocks now serialize real allocations and relocations:
`PreAttackDelay`, `FiringDuration`, `CoolDownDelayBetweenShots`, `ClipReloadTime`.
Each has two float fields `MinSeconds` / `MaxSeconds`, both defaulting to `0s`.
Their root pointers occupy bytes 236, 240, 244 and 248. Each present block adds
8 data bytes and a 36-byte procedure table (two members plus sentinel), hence
44 InstanceData bytes. Allocation order follows the original Marshal order,
independent of XML child order. Duplicate children use the first matching node.
Unknown duration attributes and nested elements are explicitly rejected.
`n` present durations emit `n` pointer relocation locations plus FFFFFFFF;
zero durations emit no relocation table. Live root pointers are preserved
until Tracker fixup; child scalar words are endian-corrected separately.

## Original evidence and non-obvious behavior

Use committed Tokenizer `-Module-.cs`: WeaponTemplate Marshal starts at line
17530 and Tokenize at 79756. Nested generic Tokenize functions near 99090–99265
show the difference between inline member offsets, absent pointers and empty
list count tables. Static raw-data fields contain full names even where the
mangled field name truncates them.

`scripts/ExtractWeaponEvidence.py` reads the original PE section table and
resolves enum-map string addresses directly from original DLL bytes. It does
not load the DLL. `evidence/weapon/enums.json` records all nine maps used by
this subset, including the exact value for every name.

* Enums use case-sensitive names and the original per-type unknown fallback.
* Flags `NONE` is enum index zero and emits bit zero (value 1), not an empty set.
* A `-NAME` token toggles its bit with XOR; repeated removal can set it again.
* Case-insensitive `ALL` emits FFFFFFFF and consumes the remaining logical
  iterations without advancing the original token cursor; later tokens do
  not remove bits. This behavior is preserved rather than normalized.
* Angle/time conversion and VC8 scalar grammar reuse the validated helpers.
* The root's Boolean bytes are never word-swapped.

## Validation and remaining work

`./recovery/scripts/TestWeapon.ps1` builds native C++ plus the C++/CLI bridge
for x86 with `/Od` and `/O2`. `tests/WeaponOracle.cs` calls the actual original
Tokenizer Plugin.ProcessInstance for every accepted fixture, compares all
three buffers, asserts independent size/member/list offsets, and verifies
unsupported inputs raise NotSupportedException. It exercises each supported
attribute independently, all named enum values, case/unknown/default handling,
bitset additions/toggles/ALL/mixed tokens, numeric boundaries, mixed fields,
each duration block, missing/default duration attributes, all four blocks in
reverse XML order, duplicate durations and independent relocation assertions
on Win32, Xbox360, PlayStation3 and unknown platform 99. Actual result counts,
source commit and Actions run are recorded in STATUS.md and validation.json.

Next: recover references/strings and related shared helpers, remaining
optional blocks, then lists/filters and polymorphic Nuggets. A complete
WeaponTemplate, GameObject, XmlCompiler serializers, plugin assembly identity,
and a MOD build using replacement DLLs remain outstanding.
