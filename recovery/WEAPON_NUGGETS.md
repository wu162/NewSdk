# WeaponEffectNugget recovery

`src/Native/WeaponNugget.h` / `.cpp` recover the common Nugget path and a coherent
damage family plus the reference, activation and scalar families below. `WeaponTemplate` delegates Nugget allocation, tokenization and
endian correction to that module. The C++/CLI adapter reads the XML and the
original SDK-injected TypeId; it never calls the original compiler. The original
compiler is loaded only by differential tests and the explicitly original-SDK
integration probe.

## Supported family

| XML element | TypeId | Instance bytes | Extra fields above DamageNugget |
|---|---:|---:|---|
| DamageNugget | 3237297817 | 152 | DamageNugget fields and ScalarInfo list |
| DOTNugget | 4022030886 | 164 | DamageInterval, DamageDuration, RemoveIfHealed |
| GrapplingHookNugget | 1093138400 | 152 | No extra data; separate inheritance token table |
| LeechPercentMaxHealthDamageNugget | 921679887 | 156 | PercentMaxHealthToTake |
| LeechHealthDamageNugget | 4012276286 | 156 | PercentDamageToHeal |
| LineDamageNugget | 4021248832 | 176 | OffsetAngle, LineWidth, LineLengthLeadIn/Out, UseDynamicLineLength, OverShootDistance |
| DamageAndSpawnNugget | 3266020104 | 220 | SpawnProbability, inline model mask, SpawnTemplate typed ID |
| FireOnObjectsNugget | 2014632688 | 164 | Weapon reference, two Booleans and optional Filter pointer |
| DamageContainedNugget | 2459653016 | 272 | MaxUnitsToDamage, WindowBlastFX reference, inline DamageObjectFilter |

The original pointer dispatcher has 35 explicit branches; all 35 are implemented.
Zero/unknown TypeIds, unknown attributes and unimplemented children remain rejected
until the original fallback and BAB preprocessing boundary are separately recovered.
An empty Nuggets container now serializes correctly, but does not establish full
Nugget support. A matching XML element name without its prepared TypeId is not
sufficient for the direct component API. Actual BAB preprocessing supplies it.
Dispatch follows TypeId rather than the spelling of the child element, as in the
original; a direct differential fixture verifies this explicitly.


### Non-Damage branches (40-byte WeaponEffectNugget base)

| XML element | Instance bytes | Derived fields |
|---|---:|---|
| ProjectileNugget | 68 | Two references, slot override, optional Vector3, VeteranProjectile list, optional barrel int |
| ScatterProjectileNugget | 48 | ScatterMin / ScatterMax |
| WeaponOCLNugget | 48 | WeaponOCL reference, TargetAsPrimaryObject |
| SeedTiberiumNugget | 56 | WeaponOCL parent, FieldAmount / SpawnedInFieldBonus |
| SpecialPowerNugget | 44 | SpecialPowerTemplate reference |
| InformTargetNugget | 48 | Min/MaxTimeToImpactFudgeFactor |
| ContainedObjectAttackNugget / SlavesAttackNugget / ReportWeaponFiredStatNugget | 40 each | Common base only |
| ActivateCircularDamageNugget / ActivateLiftObjectNugget / ActivateLinearDamageNugget | 44 each | Lifetime |
| ActivateStreamNugget | 48 | Lifetime, StreamId |
| ActivateLaserNugget | 56 | Lifetime, LaserId, HitGroundFX, OverShootDistance |
| TerrainCraterNugget | 48 | Depth, Lift percentage |
| AttachNugget | 44 | AttachModuleId typed ID |
| SpendStolenTiberiumNugget | 44 | AmountToSpend |
| SecondaryDamageNugget | 52 | Amount, DamageTaperOff, PercentDamageToContained |
| StripMaxHealthPercentNugget | 56 | AmountToStrip, DamageType, DeathType, DamageFXType |
| SuppressionNugget | 60 | Suppression, DurationSeconds, TaperOff, Arc, ArcInverted |
| LuaEventNugget | 52 | Allocated EventName string and three Boolean bytes |
| ParalyzeNugget | 60 | EffectArc, DurationSeconds, two ParalyzeEffectType enums and ParalyzeFX reference |
| InformationWarfareNugget | 52 | Inline seven-bit InfoWar mask, RadarJamRadius and RadarJamDuration |
| MetaImpactNugget | 96 | Shock-wave scalars, five Boolean bytes and optional KillObjectFilter |
| TintObjectsNugget | 72 | Three durations, frequency/amplitude and inline RGBColor |
| AttributeModifierNugget | 64 | Two modifier references, optional DamageFXType, arc, AntiCategories and AntiFX |

ParalyzeEffectType comes from the original PE: EMP=0, USER_PARALYZE=1,
UNMANNED=2, HELD=3, NONE=4; unknown names fall back to EMP. InfoWar has
seven bits, NONE=0 through RESET_RADAR_ALL=6. Its default NONE sets bit zero;
ALL fills all 32 bits, including padding. Existing sign/XOR/ALL-cursor behavior
is preserved. Unknown InfoWar names select bit zero.

These branches marshal derived allocations before their common base. Their token
tables preserve original per-member keys and offsets. Reference words are handled
by Tracker; Boolean bytes and string pointers are not byte-swapped as scalars.
LuaEvent swaps the string length only. Seed retains the WeaponOCL inheritance
token frame. These branches never use the 152-byte Damage prefix.

## Layout and allocation order

WeaponTemplate stores the list count/pointer at offsets 256/260. A nonempty list
allocates an array of four-byte pointers followed by separately allocated derived
instances. Each instance begins with the original TypeId word. The common base is
40 bytes:

| Offset | Member |
|---:|---|
| 0 / 4 | TypeId / PartitionFilterTestType |
| 8 / 12 | Optional 28-byte ObjectStatus / 60-byte ModelCondition pointers |
| 16 / 20 | Radius / optional 112-byte SpecialObjectFilter pointer |
| 24 / 28 | RequiredUpgrade count / typed-ID payload pointer |
| 32 / 36 | ForbiddenUpgrade count / typed-ID payload pointer |

DamageNugget extends that base with numeric/enum/reference fields at 40–107,
inline 28-byte InvalidTargetStatus at 108, DamageScalarDetails count/pointer at
136/140, and eight individual Boolean bytes at 144–151. Boolean padding and all
original default values are retained. DamageType unknown fallback is 38,
DeathType 33, DamageFXType 35 and DamageSubType 1; the default named values are
respectively UNDEFINED=14, NORMAL=1, UNDEFINED=112 and NORMAL=0. The partition
unknown fallback is 0, while its default EXTENTS_3D is 4.

A ScalarInfo item is 124 bytes: percentage Scalar at 0, inline ObjectFilter at 4,
and RequiredUpgrade count/pointer at 116/120. Absent Filter preserves zero bytes;
explicit empty Filter runs ObjectFilter defaults. The source XSD requires a
Filter; the direct original-ProcessInstance oracle also characterizes malformed
missing-Filter input separately. RequiredUpgrade and ForbiddenUpgrade store
lowercased ANSI typed-ID hashes, not prepared external-reference indexes.

Original derived Marshal executes before base Marshal: DamageScalarDetails and
its nested allocations precede common forbidden-mask, SpecialObjectFilter and
upgrade-list allocations. DOT/line/leech scalar extensions execute before their
DamageNugget parent. Derived TypeId is written after the inherited Marshal chain.
This ordering is preserved because it changes buffer and relocation offsets.

DamageAndSpawn adds SpawnProbability at 152, 15 inline model-mask words at 156,
and SpawnTemplate typed GameObject ID at 216. FireOnObjects adds Weapon prepared
reference at 152, an optional ObjectFilter pointer at 156, and FireOnVictimObject /
VictimMustBeAlive bytes at 160/161. Its extra filter is allocated before parent
DamageScalarDetails and base allocations. First SpawnTemplate / Filter children
win; unsupported use of those children on other types rejects explicitly. Fire
reference words are rewritten by Tracker, while SpawnTemplate is a numeric typed
ID and is endian-swapped. Neither representation is inferred from the XML name.

DamageContained adds MaxUnitsToDamage at 152, WindowBlastFX reference at 156,
and an inline 112-byte DamageObjectFilter at 160. Its filter is marshaled and
tokenized unconditionally before the Damage parent, including when absent in
direct component input. Its derived table has five entries plus the nested
156-byte filter table. XSD requires DamageObjectFilter in legal SDK sources.
The base DamageNugget and all eight XSD-derived members are now supported; this
does not complete the other WeaponEffectNugget families.

## Token and endian representation

Each Nugget list entry points to a single procedure record with marker 2 and the
actual derived TypeId, then to its derived table. Inheritance uses a marker-1
record naming the parent type. Grappling has its own two-entry inheritance table
even though its instance size equals DamageNugget. The common table has nine
entries including sentinel; DamageNugget has 29; ScalarInfo has four. Upgrade,
ScalarInfo and Nugget lists have a count header and entry records, without a list
sentinel. Nested filters reuse the existing complete ObjectFilter helper.

A simple list containing n default DamageNuggets produces 1296+516*n token bytes,
1624+672*n total instance bytes, and n+1 pointer relocation locations plus a
sentinel for nonempty lists. Independent oracle assertions check these sizes,
list order, pointer targets, defaults, tags, relocation lengths and endpoints.
Additional assertions cover each derived type's full instance/token size, leech
percentage defaults, DOT Boolean bytes and line padding.

Endian conversion swaps numeric/enum/mask words, upgrade IDs and list counts.
It traverses live Nugget pointers before the root's generic field conversion;
the root count is excluded from the generic swap to avoid converting it twice.
Pointer/reference values are rewritten by the shared tracker. No native ABI from
another game or SDK version is assumed.

## Verification

Latest 32-branch source: `bd474c8cfec499be91ade4af040279e86010c5cf`,
[run 39892423711](https://github.com/wu162/NewSdk/actions/runs/39892423711), success.
Each /Od and /O2 passes 57,288 fixtures, 171,864 buffer comparisons and 850
explicit rejection checks; four platforms include unknown 99. The SDK batch
[run 39891922522](https://github.com/wu162/NewSdk/actions/runs/39891922522) separately
passes 31 legal weapons covering the prior 30 branches plus a mixed list.
Paralyze/InformationWarfare were not in that SDK batch; keep the two scopes distinct.

Current validation: read `RESUME.md` and the pinned `evidence/weapon/validation.json`.
The paragraphs below retain the historical nine-branch milestone; the coverage
table and non-Damage section above describe the current 32 branches.


Run `scripts/TestWeapon.ps1` in the x86 VS2022 developer environment. It builds
both /Od and /O2 and compiles three partial oracle files: WeaponOracle,
WeaponScatterOracle and WeaponNuggetOracle. The actual original ProcessInstance
is called for every accepted fixture, and all three public buffers are compared.
The oracle covers all five new enum maps, every shared/derived attribute, legacy
numeric/unit/Boolean inputs, empty and nonempty references, pointer combinations,
upgrade and ScalarInfo lists through 257, duplicate-filter semantics, every
unimplemented dispatch branch, and 128 combinations with existing root fields,
optional blocks, filters, scatter and modifier references on all four platform IDs.

`SDK/Mods/mymod/Data/RecoveryWeaponNuggetProbe.xml` adds nine legal weapons and is
included from the real Mod.xml entry. It supplies required DOT fields and valid
ScalarInfo.Filter children, preserving existing sample sources.
`TestWeaponSdkProbe.ps1` validates these raw files against the full CnC3Types.xsd,
compiles actual mymod basic data through the original SDK with fresh output, and
requires the nine recovered component InstanceData payloads to occur in the
resulting mod.bin. Its expected-component input adds only the nine known
polymorphic TypeIds to raw XML; it is not a reconstruction of all BAB preprocessing.
This probe retains original WeaponTemplate routing. It establishes legal SDK
source/output agreement for these nine assets, not a recovered Weapon SDK plugin
build. Direct differential tests separately check all three buffers.

Final nine-type source is `f552f2195ebac59858a9c8d6db92194fa8756881`;
[run 39863931585](https://github.com/wu162/NewSdk/actions/runs/39863931585)
completed successfully. Each /Od and /O2 passed 47,512 fixtures, 142,536
three-buffer comparisons and 190 rejection checks. All nine raw SDK probes
passed full XSD validation and matched original SDK InstanceData.

The workflow persists source/run, mode/scope and SDK-probe status in
`evidence/weapon/validation.json`. The evidence ZIP contains SDK logs, all four
original MOD outputs and component inputs/buffers. Check the pinned current
validation before claiming success. Armor-only recovered-plugin integration
continues separately and also runs when the mymod entry changes.

## Original evidence

All code is based on this repository's Tokenizer PE/IL. Entry points in
`decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs` include common Marshal
15406, pointer dispatch 15499, ScalarInfo Marshal 15339, DamageNugget Marshal
16961, line/leech/DOT Marshal 17219–17390, allocation wrappers around 52878–52979,
FireOnObjects / Spawn Marshal 17392 / 17427 and Tokenize 79667 / 79706,
DamageContained Marshal / Tokenize 17498 / 79731,
common Tokenize 78605, Damage Tokenize 79450, derived token tables 79571–79666,
ScalarInfo list Marshal 53395 and Nugget list Tokenize 99182.
`ExtractWeaponEvidence.py` now extracts the original partition/damage/death/FX/
subtype enum maps too; no enum order is guessed from XSD or another SDK.

## Remaining dispatch coverage

| Original native type | TypeId | State |
|---|---:|---|
| AttachNugget | 576040660 | Recovered |
| ParalyzeNuggetType | 169470683 | Recovered |
| ContainedObjectAttackNuggetType | 609705483 | Recovered |
| InformationWarfareNuggetType | 978137461 | Recovered |
| LeechPercentMaxHealthDamageNuggetType | 921679887 | Recovered |
| GrapplingHookNuggetType | 1093138400 | Recovered |
| SpecialPowerNuggetType | 1542018985 | Recovered |
| InformTargetNuggetType | 1115743736 | Recovered |
| StripMaxHealthPercentNuggetType | 1622083673 | Recovered |
| SeedTiberiumNuggetType | 1757028425 | Recovered |
| ReportWeaponFiredStatNuggetType | 1675169590 | Recovered |
| LuaEventNuggetType | 2010867179 | Recovered |
| ActivateStreamNuggetType | 2159708756 | Recovered |
| FireOnObjectsNuggetType | 2014632688 | Recovered |
| ProjectileNuggetType | 2278014412 | Recovered |
| DamageContainedNuggetType | 2459653016 | Recovered |
| ActivateCircularDamageNuggetType | 2360033115 | Recovered |
| SecondaryDamageNuggetType | 2475108259 | Recovered |
| MetaImpactNuggetType | 2940373499 | Explicitly rejected |
| ActivateLiftObjectNuggetType | 2769610658 | Recovered |
| SlavesAttackNuggetType | 3111574559 | Recovered |
| DamageAndSpawnNuggetType | 3266020104 | Recovered |
| DamageNuggetType | 3237297817 | Recovered |
| WeaponOCLNuggetType | 3322313791 | Recovered |
| TintObjectsNuggetType | 3665090361 | Explicitly rejected |
| TerrainCraterNuggetType | 3520092115 | Recovered |
| ActivateLinearDamageNuggetType | 3694898303 | Recovered |
| ActivateLaserNuggetType | 3915396258 | Recovered |
| AttributeModifierNuggetType | 3821195611 | Explicitly rejected |
| LeechHealthDamageNuggetType | 4012276286 | Recovered |
| DOTNuggetType | 4022030886 | Recovered |
| LineDamageNuggetType | 4021248832 | Recovered |
| SuppressionNuggetType | 4104564135 | Recovered |
| SpendStolenTiberiumNuggetType | 4286253161 | Recovered |
| ScatterProjectileNuggetType | 4131529406 | Recovered |

WeaponTemplate remains a partial type until the remaining branches and any unknown-polymorphic fallback boundary are deliberately recovered. Its SDK route is retained on the original plugin.



## Final three explicit branches (2026-10-23)

Original evidence: Tokenizer `-Module-.cs` blob
`6e40a9ed6c9f961d294d49768d911e6a6f354619`, read at repository source
`5b48971641c5cceea41ffb9621963ba53f71df97`. Method names and actual pointer
arithmetic, not the separate TypeHash constants, determine dispatch and layout.

| Type / TypeId | Marshal / endian | Tokenize | Allocation |
|---|---|---|---|
| MetaImpact / 2940373499 | 16328–16544 | 79149–79239 | 52684–52690: 96 bytes |
| TintObjects / 3665090361 | 16018–16130 | 79012–79048 | 52795–52801: 72 bytes |
| AttributeModifier / 3821195611 | 16805–16864 | 79356–79392 | 52803–52809: 64 bytes |

MetaImpact: floats at 40/44/48/52/56/60/64/68/72/76/80, respectively
ShockWaveAmount=0, ShockWaveRadius=0, ShockWaveArc=360d,
ShockWaveTaperOff=0, ShockWaveSpeed=0, ShockWaveZMult=1,
CyclonicFactor=0, ShockwaveDelaySeconds=0s, Suppression=0,
ShockWaveClearWaveMult=2, ShockWaveClearFlingHeight=100.
The exact lower-case `w` in ShockwaveDelaySeconds is intentional.
KillObjectFilter is a nullable 112-byte allocation through offset 84, marshaled
before base allocations. Boolean bytes at 88–92 are ShockWaveArcInverted,
InvertShockWave, FlipDirection, OnlyWhenJustDied, ShockWaveClearRadius, all false;
93–95 remain zero padding. The derived procedure table has 19 entries including
base and terminator; the filter procedure stays null when absent.

TintObjects: PreColorTime=2s at 40, PostColorTime=2s at 44,
SustainedColorTime=1s at 48, Frequency=1.0 at 52, Amplitude=1.0 at 56.
Color is always inline at 60 (R), 64 (G), 68 (B). Missing Color/components remain
zero; no 255 normalization or clamping. RGBColor Marshal/endian is at
13320–13378, Tokenize at 76714–76734: a four-entry nested table is always emitted,
including when Color is absent. Scalar members swap on big endian.

AttributeModifier: references at 40/44 are AttributeModifierName/OwnerName;
DamageFXType at 48 has no fallback attribute value, so absence remains zero
(GDI_CANNON), unlike DamageNugget's UNDEFINED default. An explicitly supplied
unknown name uses the existing original enum fallback ALIEN_BOMB=35.
DamageArc at 52 defaults to 360d; AntiCategories at 56 is a 15-bit inline mask,
absent zero; AntiFX is a reference at 60. Reference slots are not scalar-swapped.
The enum map is extracted from the original PE by ExtractWeaponEvidence.py;
its text parser at 67837–67961 uses unknown-name fallback BUFF=7, minus as XOR,
and sign-insensitive ALL fills all 32 bits and stops advancing the token pointer.

The implementation shares explicit per-member specifications but gives nested
Color/Filter their own marshaling, token and endian paths. Tests in
WeaponFinalNuggetOracle.cs add independent size/key/type/offset/default assertions,
all new attributes and RGB components across numeric/reference edge values,
first duplicate child selection, filter/list/pointer combinations, all category
names/sign pairs plus deterministic random sequences, mixed recovered families,
and unsupported-child/attribute rejections. The prior three TypeId rejection cases
are replaced by genuinely unknown IDs; zero/missing/unknown rejection remains.

Validation passed for source `0d4c9932dd4c7bec1af88f37a527033afad80b9c`:
[run 39894482358](https://github.com/wu162/NewSdk/actions/runs/39894482358),
completed success. Each /Od and /O2 mode ran 64,392 ProcessInstance fixtures,
193,176 three-buffer comparisons, 1,002 explicit rejections, 2,070 direct
ObjectStatus comparisons and 4,179 direct ModelCondition comparisons, across
Win32, Xbox360, PlayStation3 and unknown platform 99. The original procedure
keys/types for all 29 new members were also compared against their decompiled
Tokenize methods, and all 15 category entries were extracted from original PE.
The unchanged primitive regression also passed in run 39894482472.
The last successful SDK batch covered only the first 30 branches; all five added
since that batch remain outside real SDK integration evidence. All 35 explicit
branches does not establish a complete Weapon plugin or replacement compiler DLL:
unknown-TypeId behavior and BAB TypeId/preprocessing remain open.


## Unknown polymorphic branch (2026-10-23, validation pending)

Original evidence is Tokenizer `-Module-.cs` blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`. Pointer Marshal (15499–15623) reads `TypeId` with default `0` through `strtoul(...,0)`. Unknown helper (52980–52986) Pushes **40 bytes**, marshals WeaponEffectNugget common members (15406–15448), then writes tag **zero**, not the unknown input value. Root type hash 3351432196 is also not a known polymorphic tag.

Pointer Tokenize (78676–78796) allocates a single 12-byte `{name=2,type=outputTag,offset=0}` record. Its switch has **no default and no case zero**: unknown output keeps offset zero, with **no common-base token subtree**. Common-base optional objects and upgrade lists still exist in InstanceData and RelocationData; they must not be dropped merely because Tokenize does not expose them. Pointer endian dispatch (15624–15742), default helper (53210–53213), and base endian (15449–15498) convert common members and pointed-to data only. Never cast this 40-byte fallback to the 152-byte Damage layout.

Implementation recognizes all 35 known IDs and otherwise validates the supported common-base surface, allocates 40 bytes, sets output tag zero, emits the wrapper alone, and runs EndianBase. Derived/unknown fields and foreign namespaces remain explicitly rejected. This strict subset intentionally does not silently ignore arbitrary derived input as raw Marshal might. Missing, zero, decimal/hex/prefix/trailing and malformed TypeId strings use the already recovered unsigned parser.

Tests: UnknownNuggets in WeaponNuggetOracle.cs; every platform including 99, both /Od and /O2, all three buffers. Cartesian TypeId/base fields/filter/upgrade cases; unknown/known mixed lists; independent allocation, output tag, wrapper offset, radius, and relocation checks at 1/2/7/31 entries. Existing full matrix remains enabled. Component results do not establish BAB preprocessing or formal Weapon plugin integration.
