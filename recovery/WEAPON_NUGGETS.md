# WeaponEffectNugget recovery

`src/Native/WeaponNugget.h` / `.cpp` recover the common Nugget path and a coherent
damage family. `WeaponTemplate` delegates Nugget allocation, tokenization and
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

The original pointer dispatcher has 35 explicit branches. The other 26 branches,
zero/unknown TypeIds, unknown attributes and unimplemented children are rejected.
An empty Nuggets container now serializes correctly, but does not establish full
Nugget support. A matching XML element name without its prepared TypeId is not
sufficient for the direct component API. Actual BAB preprocessing supplies it.
Dispatch follows TypeId rather than the spelling of the child element, as in the
original; a direct differential fixture verifies this explicitly.

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
| AttachNugget | 576040660 | Explicitly rejected |
| ParalyzeNuggetType | 169470683 | Explicitly rejected |
| ContainedObjectAttackNuggetType | 609705483 | Explicitly rejected |
| InformationWarfareNuggetType | 978137461 | Explicitly rejected |
| LeechPercentMaxHealthDamageNuggetType | 921679887 | Recovered |
| GrapplingHookNuggetType | 1093138400 | Recovered |
| SpecialPowerNuggetType | 1542018985 | Explicitly rejected |
| InformTargetNuggetType | 1115743736 | Explicitly rejected |
| StripMaxHealthPercentNuggetType | 1622083673 | Explicitly rejected |
| SeedTiberiumNuggetType | 1757028425 | Explicitly rejected |
| ReportWeaponFiredStatNuggetType | 1675169590 | Explicitly rejected |
| LuaEventNuggetType | 2010867179 | Explicitly rejected |
| ActivateStreamNuggetType | 2159708756 | Explicitly rejected |
| FireOnObjectsNuggetType | 2014632688 | Recovered |
| ProjectileNuggetType | 2278014412 | Explicitly rejected |
| DamageContainedNuggetType | 2459653016 | Recovered |
| ActivateCircularDamageNuggetType | 2360033115 | Explicitly rejected |
| SecondaryDamageNuggetType | 2475108259 | Explicitly rejected |
| MetaImpactNuggetType | 2940373499 | Explicitly rejected |
| ActivateLiftObjectNuggetType | 2769610658 | Explicitly rejected |
| SlavesAttackNuggetType | 3111574559 | Explicitly rejected |
| DamageAndSpawnNuggetType | 3266020104 | Recovered |
| DamageNuggetType | 3237297817 | Recovered |
| WeaponOCLNuggetType | 3322313791 | Explicitly rejected |
| TintObjectsNuggetType | 3665090361 | Explicitly rejected |
| TerrainCraterNuggetType | 3520092115 | Explicitly rejected |
| ActivateLinearDamageNuggetType | 3694898303 | Explicitly rejected |
| ActivateLaserNuggetType | 3915396258 | Explicitly rejected |
| AttributeModifierNuggetType | 3821195611 | Explicitly rejected |
| LeechHealthDamageNuggetType | 4012276286 | Recovered |
| DOTNuggetType | 4022030886 | Recovered |
| LineDamageNuggetType | 4021248832 | Recovered |
| SuppressionNuggetType | 4104564135 | Explicitly rejected |
| SpendStolenTiberiumNuggetType | 4286253161 | Explicitly rejected |
| ScatterProjectileNuggetType | 4131529406 | Explicitly rejected |

WeaponTemplate remains a partial type until the remaining branches and any unknown-polymorphic fallback boundary are deliberately recovered. Its SDK route is retained on the original plugin.
