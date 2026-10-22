# WeaponTemplate supported subset: scalars, resources, status, filters and scatter

The supported entry point is `Recovered.WeaponCompiler.ProcessInstance` in
`src/Bridge/WeaponCompiler.cpp`; the native implementation is
`src/Native/WeaponTemplate.h` / `.cpp`. It uses the previously validated
`ScalarParsing`, `DataInitializers`, `XmlNodeAdapter` and `Relo::Tracker`,
plus the shared `ResourceMarshaling.h` / `.cpp` helpers.
There is no original compiler forwarding. This component references SDK Core
for `InstanceDeclaration` / `AssetBuffer`, and does not replace either compiler.

## Implemented scope

91 root attributes: the original 77 scalar/bitset attributes plus 12 string,
resource-reference and typed-ID attributes, plus the two ObjectStatus pointer masks. Scalar coverage includes floats, angles, times, percentages, signed integers,
36 byte-sized Boolean fields, five scalar enum groups, and five bitset attributes
(including both anti-mask attributes).
The explicit `WeaponField` table lists every supported field, default text,
byte offset, procedure name/type hash and token treatment in original order.
`WeaponValue::Absent` marks 13 child/list fields. Ten are supported
as child blocks (four durations, two voice overrides, one AI hint and three filters); their
names remain rejected as root attributes. IncompatibleAttributeModifier is supported as a repeated child list. The three
ObjectFilter pointer blocks are also supported. ScatterRadiusVsType is supported as a repeated 176-byte inline-filter/model-mask list.
Nuggets supports the common base and nine damage-family branches described in
`WEAPON_NUGGETS.md`; the other 26 dispatch branches reject explicitly. Unsupported input,
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



## ObjectStatus masks and shared enum bitsets

`EnumBitset.h` / `.cpp` contains the shared text-bitset helper and all 223
ObjectStatus name/ordinal pairs, read directly from the shipped PE enumMap.
The five existing root bitset attributes now use the same helper. No original
DLL is called by the recovered implementation. The direct managed helper
`WeaponCompiler.MarshalObjectStatus(text, initialSevenWords)` exposes inline
accumulation for differential testing and reuse, without resetting initial bits.

| Attribute | Root pointer | Allocation | Procedure-table treatment |
|---|---:|---:|---|
| RequiredFiringObjectStatus | 136 | 28 bytes | Member offset to mask payload (slot 41), no nested table |
| ForbiddenFiringObjectStatus | 140 | 28 bytes | Member offset to mask payload (slot 42), no nested table |

Absent attributes leave null pointers. Present empty/whitespace text still
allocates seven zero words. Allocation follows root Marshal order, after the
three firing-audio pointers and before PreAttackWeapon. Each present mask adds
28 instance bytes and one root pointer relocation; it adds no token-table bytes.
The seven words are endian-corrected independently. Bit 0 is the least significant
bit of word 0; named values span 0 through 222.

Enum names are case-sensitive. Unknown names (including NONE, bare signs and
malformed tokens) select ordinal 198, ABSORBED_DAMAGE. Leading '+' ORs a bit;
leading '-' XORs it, rather than clearing. Splitting uses C whitespace (space,
TAB, CR, LF, vertical tab and form feed), and stops at the first NUL. ALL alone
is case-insensitive, ignores its sign and fills all seven words with FFFFFFFF,
including reserved bit 223. The original ALL branch fails to advance the token
cursor, so later tokens never affect the result. This behavior is retained.

The oracle checks every name, each pointer, positive/negative signs, duplicates,
XOR twice, lower-case fallback, word boundaries, all names together, ALL and
malformed tokens. Independent assertions verify all seven words, pointer targets,
member bookmarks and relocation order/sentinel. All 128 optional-block subsets
also run with both masks and nonempty resource pointers. A direct reflection
oracle additionally compares zero, patterned and all-set initial masks, raw
control characters, embedded NUL and ANSI conversion with the original helper.

## IncompatibleAttributeModifier reference list

Each repeated `<IncompatibleAttributeModifier>name\index</IncompatibleAttributeModifier>`
child contributes one four-byte prepared reference in source order. Empty values
are real list entries. Attributes and nested elements are explicitly rejected by
the partial adapter. The existing ANSI/prepared-reference helper parses content;
no asset resolution or original compiler forwarding occurs here.

The root stores count at 284 and payload pointer at 288. The nonempty payload is
allocated last, after AI hint and duration allocations. The list procedure table
has count + 1 entries: count header hashes 1820487795 / 1564774360, followed by
name hash 2974916843, zero type hash and a direct member bookmark for each payload
word. There is no list-table sentinel. Relative to an otherwise empty list,
each entry adds 12 token bytes and 4 data bytes. The payload pointer adds one
relocation; public ImportsData remains empty as in original Tokenizer.

Differential fixtures cover lengths 0 through representative boundaries up to
257, repeated values, malformed/signed/overflow reference suffixes, CDATA and
interleaved children, and combinations with status/audio/AI/duration allocations.
Independent assertions check count, order, token records, values and relocation.
The exact validated source and run remain in `evidence/weapon/validation.json`.

## Three ObjectFilter blocks

`ObjectFilter.h` / `.cpp` implements the shared 112-byte ObjectFilter layout,
marshaling, tokenization and endian correction. WeaponTemplate allocates
SurpriseAttackObjectFilter, CombinedAttackObjectFilter and HitStoredObjectFilter
through root pointers 264/268/272, in that order, after WeaponAiHintInfo and before
the modifier list. Duplicate XML blocks use the first matching child.

| ObjectFilter member | Offset | Representation |
|---|---:|---|
| id | 0 | Lowercased ANSI typed-ID FastHash, zero when absent |
| Rule | 4 | Enum; NONE defaults to 3, unknown text falls back to ALL (1) |
| Relationship | 8 | Four named bits in one word; empty by default; unknown token selects ALLIES (0) |
| Alignment | 12 | Enum; NONE defaults to 0, unknown text falls back to EVIL (1) |
| Include / Exclude | 16 / 52 | Two inline 284-bit KindOf masks, nine words each |
| StatusBitFlags / StatusBitFlagsExclude | 88 / 92 | Optional pointers to 28-byte ObjectStatus masks |
| IncludeThing | 96 / 100 | Count/pointer pair for repeated four-byte GameObject typed IDs |
| ExcludeThing | 104 / 108 | Count/pointer pair for repeated four-byte GameObject typed IDs |

All 284 KindOf names/ordinals and the three small enum maps come from the original
PE. KindOf unknown-token fallback is 149 (ACTIVATE_AFTER_UNPACK); ALL fills all
nine words, including the four padding bits. Signs and ALL cursor behavior reuse
the validated enum-bitset helper. These are compiler serialization semantics,
not runtime target-selection behavior.

The filter procedure table has ten fields plus sentinel (132 bytes). The two
thing lists each have a count header even when empty, adding 24 token bytes.
An otherwise empty filter adds 156 token bytes and 112 data bytes. Status payloads
use direct member bookmarks; thing entries use name hash 2974916843, zero type
hash and direct member bookmarks to typed IDs. No nested procedure table is
created for a status mask or typed-ID word. All scalar/mask words and typed-ID
arrays are endian-corrected before relocation, while host pointers remain intact.

The bridge accepts only the eight listed attributes and repeated IncludeThing /
ExcludeThing children. Other attributes, namespaced attributes, unknown children,
thing attributes and nested thing elements are rejected explicitly. Every name
and sign/toggle/case fallback, enum values, default combinations, duplicate blocks,
thing lists through 257 and the seven optional-block combinations are compared
with original ProcessInstance. Independent assertions check empty-block sizes,
pointer allocation order, defaults, nested token offsets and relocation tables.

The previously unsupported ScatterRadiusVsType is not a simple radius list: its original element is 176
bytes (radius + 456-bit ModelCondition mask + inline 112-byte ObjectFilter).
The complete ModelCondition map and inline-filter integration are now recovered
and validated as described below. Nuggets remains unsupported.


## ModelCondition and ScatterRadiusVsType

The scatter list uses a count/pointer at root offsets 276/280. Each element is
176 bytes: Radius float at 0; 15 inline ModelCondition words at 4; a complete
112-byte inline ObjectFilter at 64. The list payload is allocated after the three
root filters and before IncompatibleAttributeModifier. Per-element filter status
payloads and typed-ID lists use the same validated shared helpers.

An absent Filter leaves all 112 inline bytes zero. A present empty Filter applies
the normal defaults, including Rule NONE = 3. Tokenization always emits the inline
filter table, even when the XML Filter was absent. Repeated Filter children use
the first one; repeated ScatterRadiusVsType children remain ordered list entries.

The original PE ModelCondition enum map has 457 name/value pairs: INVALID = -1
and ordinals 0 through 455. ALL has ordinal 454 in the map, but the text parser
handles case-insensitive ALL specially and fills all 15 words, including 24
padding bits. INVALID is skipped, unknown names fall back to ABOUT_TO_HIT (233),
minus XORs, and ALL retains the already-characterized token cursor behavior.
`MarshalModelCondition` reuses the shared parser with these exact parameters;
its managed bridge supports seeded 15-word masks for direct original-helper tests.

The scatter element procedure table has three members plus sentinel (48 bytes):
Radius hashes 1328662213/1683514611, RequiredModelConditions 1022003432/1742454729,
and Filter -2055623642/-462511182. Each element unconditionally adds the 156-byte
ObjectFilter token structure. The list has a count header and one 12-byte entry
per element; no list sentinel. With no extra filter payloads or other optionals,
n elements produce 1296 + 216*n token bytes and 1624 + 392*n instance bytes,
with one root payload relocation plus sentinel when nonempty. Both endian modes
swap all 15 model words and inline/nested filter data, preserving live pointers
until the tracker rewrites them.

`tests/WeaponScatterOracle.cs` is a second partial class in the existing original
ProcessInstance oracle. It covers all 456 names, INVALID, signs/toggles, lower-case
fallback, ALL/padding, word boundaries, raw seeded helper calls, list lengths
through 257, absent/empty/duplicate filters, nested status/thing lists and 128
combinations of earlier optionals/resources/status/filters/modifier references.
Independent assertions check radius bits, mask words, inline-filter defaults,
procedure offsets, list count/order, total size and relocation/sentinel.
Actual validation/source/run is recorded in STATUS.md and evidence/weapon.

At that historical scatter milestone Nuggets remained unsupported; the continuation
below supersedes that boundary. WeaponTemplate is not yet switched to the recovered
SDK plugin. Full recovery of WeaponTemplate is still required before that switch.


## Nugget damage family continuation

Nuggets is now partially supported, rather than wholly absent. The common base,
ScalarInfo, polymorphic list/token representation and all nine DamageNugget-family branches are
recovered and differentially validated. See `WEAPON_NUGGETS.md` for exact type
IDs, sizes, fields, evidence and the remaining 26-branch boundary. The new
`RecoveryWeaponNuggetProbe.xml` is included in real mymod; XSD validation and an
original SDK build verify all nine reconstructed InstanceData payloads. That
probe retains original Weapon routing. Full recovered-Weapon SDK integration
and complete replacement DLLs remain outstanding.
