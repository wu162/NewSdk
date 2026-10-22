# WeaponTemplate scalar, resource and optional-block subset

The supported entry point is `Recovered.WeaponCompiler.ProcessInstance` in
`src/Bridge/WeaponCompiler.cpp`; the native implementation is
`src/Native/WeaponTemplate.h` / `.cpp`. It uses the previously validated
`ScalarParsing`, `DataInitializers`, `XmlNodeAdapter` and `Relo::Tracker`,
plus the shared `ResourceMarshaling.h` / `.cpp` helpers.
There is no original compiler forwarding. This component references SDK Core
for `InstanceDeclaration` / `AssetBuffer`, and does not replace either compiler.

## Implemented scope

89 root attributes: the original 77 scalar/bitset attributes plus 12 string,
resource-reference and typed-ID attributes. Scalar coverage includes floats, angles, times, percentages, signed integers,
36 byte-sized Boolean fields, five scalar enum groups, and five bitset attributes
(including both anti-mask attributes).
The explicit `WeaponField` table lists every supported field, default text,
byte offset, procedure name/type hash and token treatment in original order.
`WeaponValue::Absent` now marks 15 child/list/status fields. Seven are supported
as child blocks (four durations, two voice overrides and one AI hint); their
names remain rejected as root attributes. The remaining eight fields must be
absent: RequiredFiringObjectStatus, ForbiddenFiringObjectStatus, Nuggets,
SurpriseAttackObjectFilter, CombinedAttackObjectFilter, HitStoredObjectFilter,
ScatterRadiusVsType and IncompatibleAttributeModifier. Unsupported input,
including an explicit empty value, is rejected.
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
zero durations and no other pointer allocations emit no relocation table.
Tracker rewrites live pointers to serialized offsets; child scalar words are
endian-corrected separately.

## Strings, resources and the additional optional blocks

| Root attributes | Original representation |
|---|---|
| PreferredTargetBone | Inline length/pointer pair at 76/80, ANSI bytes plus NUL in a separately padded allocation |
| FireSound, FireSoundPerClip, FiringLoopSound | Optional four-byte reference allocation, pointers at 84/88/92 |
| FireFX, FireVeteranFX, FireFlankFX, PreAttackFX | Inline references at 96/100/104/108 |
| SingleAmmoReloadedNotFullSound, ClipReloadedSound | Inline references at 188/192 |
| ProjectileStreamName | Lowercased ANSI typed-ID hash at 216; not a resource import |
| PreAttackWeapon | Optional four-byte reference allocation, pointer at 224 |

A present empty string is different from an absent value: PreferredTargetBone
still allocates a NUL and pointer-valued references still allocate a word.
BAB supplies prepared reference text `name\index`. The original marshaler
finds the first backslash and parses the suffix with decimal VC8 `atoi`, then
records the reference. No separator leaves the reference word zero; it does
not resolve the name inside the compiler. Relocation writes `index + 1` modulo
2^32, including negative and saturated signed-overflow results. The shared
tracker can produce an import table, but **Tokenizer's ProcessInstance discards
ImportsData**, even for nonempty references. Preserving this is deliberate.
`HashAssetId` reconstructs the original FastHash function, including signed
byte behavior for remainder bytes; C-locale ASCII case folding is preserved.
The managed bridge uses ANSI conversion, as original Value.StringToHGlobalAnsi.

`OverrideVoiceAttackSound` and `OverrideVoiceEnterStateAttackSound` allocate
8-byte SoundOrEvaEvent blocks through root pointers 228/232. Optional Sound
allocates a four-byte reference word; EvaEvent is inline at +4. Their nested
procedure tables have three entries (36 bytes) including sentinel.

`WeaponAiHintInfo` allocates 12 bytes through root pointer 252: optional
MaxSpeedOfTarget float pointer at +0, inline UseAsWarheadForDamageCalculations
reference at +4, Boolean IsAntiGarrisonWeapon/UseLongLockOnTimeCode bytes at
+8/+9. A present MaxSpeedOfTarget (even empty) allocates four bytes. Its
procedure table has five entries (60 bytes) including sentinel. Missing Boolean
attributes default to false. Padding stays zero, and Boolean bytes are never
word-swapped. All these blocks preserve the first duplicate child and original
allocation order regardless of XML order.

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
Final resource/seven-block validation source is
`bef3220408380d609d7a145c4c40171caba63ee8`, run
https://github.com/wu162/NewSdk/actions/runs/39787342909 (success). Each /Od
and /O2 build passes 5,492 fixtures, 16,476 buffer comparisons and 26 rejection
checks; both modes together total 32,952 buffer comparisons. The optimized
component artifact is `recovered-weapon-component-x86`.

The AI warhead XML attribute is `UseAsWarheadForDamageCalculations` (plural);
read full raw string data rather than its truncated mangled identifier.

Additional fixtures cover explicit-empty strings, ANSI conversion, length
boundaries, typed-ID hash remainders, prepared resource references and malformed
indexes, nested voice/AI blocks, pointer-target allocation and relocation
assertions. All 128 subsets of the seven optional blocks are combined with
nonempty root resources and reverse XML child order.

Next: recover the two ObjectStatus masks, filters, lists and polymorphic Nuggets. A complete
WeaponTemplate, GameObject, XmlCompiler serializers, plugin assembly identity,
and a MOD build using replacement DLLs remain outstanding.

