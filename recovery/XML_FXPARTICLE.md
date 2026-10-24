# FXParticleSystemTemplate XmlCompiler component

Latest acceptance (2026-10-24): source `aac2279c99abf4c55d7f4d682b3e68bbe3a77107`, component run [40091218825](https://github.com/wu162/NewSdk/actions/runs/40091218825), /Od and /O2, platforms 0/1/2/99. Fourteen-type Reduced and FullMymod SDK acceptance is recorded in [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md), run [40091218796](https://github.com/wu162/NewSdk/actions/runs/40091218796). Full replacement of all XmlCompiler types remains outside this acceptance.
Fixed base commit: ba7a3d6845be5e124d08320cb2c56fda552d9fbe.
Public interface: Recovered::FXParticleSystemTemplateCompiler::CompileXml(XmlNode^, XmlNamespaceManager^, bool)
and ProcessInstance(InstanceDeclaration^, TargetPlatform); TypeId 686292351.
No original compiler is called by the recovered implementation.

## Evidence and layout

Original Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea,
SHA256 be379d88db4c9f6de9cb4844bfc86a58d880d7a88a6b28cc76ba1c1bfc0fc6c1.
Read-only bounded evidence on recovery/xmlcompiler-priority:
roots/FXParticleSystemTemplate.cs.txt (Marshal 24310–24632, endian 24634–24729,
Tracker constructor 90882–90971), matching .static.json, parts/part-0043..0048,
part-0123..0125, and original methods.json/types.json from the baseline.
Original DLL SHA256 d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6.
The host checks this exact fingerprint.

Root is 176 bytes, with no Tokenizer header. Root attributes:
Priority@4 default ULTRA_HIGH_ONLY, Shader@8 ADDITIVE, Type@12 PARTICLE,
ParticleTexture/Drawable direct references@16/20, SlaveSystem/PerParticleAttachedSystem
typed hashes@24/28, SystemLifetime/SortLevel unsigned@32/36 default 0,
EmitterSound nullable reference pointer@40, five packed bools@168..172 default false.
Bool names are IsOneShot, IsGroundAligned, IsEmitAboveGroundOnly,
IsParticleUpTowardsEmitter and UseMaximumHeight. id/inheritFrom are ignored metadata.
Reserved @0 and tail padding @173..175 remain zero.
EmitterSound is allocated even for a present empty attribute; absent means no block.

Complete inline children: SlavePosOffset Vector3@44; Lifetime/Size/StartSizeRate/
BurstDelay/BurstCount/InitialDelay random variables@56/68/80/92/104/116.
Random variables serialize distribution@0, Low@4, High@8 (12 bytes).
Present nodes default to UNIFORM=1; absent nodes remain zero.
Distribution fallback is 0; exact PE map retained.

Alphas@128 points to an 8-byte list header; each Alpha is 20 bytes:
random value@0, RelativeAge percentage@12, Frame unsigned@16 default 1000.
Colors@132 points to 28 bytes: saturation@0 default1, Color list header@4,
ColorScale random@12, house-color bool@24. Each Color item is20 bytes:
RelativeAge@0, Frame@4, nested lower-case r/g/b float channels@8/12/16.
Wind@136 is48 bytes with all 12 attributes/defaults from original Marshal.
List allocation order, null versus empty, imports and relocation sentinels are preserved.

Module pointers Physics/Draw/Volume/Velocity/Update are@140/144/148/152/164.
Event has inline count@156 and pointer-array@160. Empty/absent Event is zero/null. Nonempty Event first allocates all four-byte pointer slots, then each module in document order. Allocation order is Physics, Draw, Volume, Velocity, Event, Update.
All data uses a single Tracker::MakeRelocatable(bigEndian,0), retaining all three arrays.
Platform0 is little endian;1/2/99 big endian. Packed bools stay bytes.
Reference imports retain their actual allocated field offsets. Typed IDs use the shared hash.

## Supported original TypeId dispatch

Numeric TypeId selects the implementation, independently of element name. All 33 known types can appear in any singleton wrapper or Event list, including cross-category routing.

| Type | TypeId | Size |
|---|---:|---:|
| FXParticlePhysicsBase | 877888681 | 4 |
| FXParticleDrawBase | 3201797789 | 4 |
| FXParticleDrawStreak | 3809157760 | 4 |
| FXParticleEmissionVelocityBase | 2268785311 | 4 |
| FXParticleUpdateBase | 1903056374 | 4 |
| FXParticleEmissionVolumeBase | 612354849 | 8 |
| FXParticleEmissionVolumePoint | 3543124331 | 8 |
| FXParticleEmissionVolumeLightning | 1423381183 | 8 |
| FXParticleDrawLightSource | 1373300932 | 8 |
| FXParticleDrawTrail | 1865622437 | 24 |
| FXParticleDrawSwarm | 2381754661 | 24 |
| FXParticleDefaultPhysics | 3110487039 | 36 |
| FXParticleSwarmPhysics | 1976375475 | 8 |
| FXParticleEmissionVolumeSphere | 3992468329 | 12 |
| FXParticleEmissionVolumeBox | 1242273524 | 20 |
| FXParticleEmissionVolumeCylinder | 1552576278 | 32 |
| FXParticleEmissionVolumeSpline | 795809322 | 32 |
| FXParticleEmissionVolumeLine | 228810652 | 32 |
| FXParticleEmissionVolumeTerrainFire | 3772792373 | 48 |
| FXParticleEmissionVelocitySphere | 2343798068 | 16 |
| FXParticleEmissionVelocityHemisphere | 3082112962 | 16 |
| FXParticleEmissionVelocityOutward | 3476018911 | 28 |
| FXParticleEmissionVelocityCylinder | 4058790702 | 28 |
| FXParticleEmissionVelocityOrtho | 3880943874 | 40 |
| FXParticleEmissionVelocityWavefront | 3975650247 | 68 |
| FXParticleEventBase | 2693838876 | 12 |
| FXParticleEventCollision | 1075723135 | 28 |
| FXParticleEventLife | 3465979181 | 24 |
| FXParticleUpdateDefault | 2933826200 | 104 |
| FXParticleUpdateRenderObject | 234581578 | 200 |
| FXParticleDrawGpu | 2248017887 | 60 |
| FXParticleDrawLightning | 3036920908 | 36 |
| FXParticleDrawRenderObject | 3408220251 | 20 |

All fields of these 33 types are implemented. Missing/zero/unknown module TypeId emits four zero bytes only when there are no unsupported payload attributes or children. Empty singleton wrappers remain null. Unknown enum fallbacks are Priority6, Shader1, Type2, WindMotion3, WavefrontShape1, Rotation0, GeometryType2; names are case sensitive. Original namespaced maps come from static-data/fields-0030.json and exact shipped schema names.

## Completed layouts and source boundaries

EventBase has EventFX reference@4, PerParticle byte@8, KillAfterEvent byte@9 and padding@10..11. Collision adds HeightOffset random@12 and OrientFXToTerrain byte@24 (default false), padding@25..27. Life adds EventTime random@12. Original Marshal methods are 23359/23369/23399 (part-0046), allocation methods 61822/61951/61975 (part-0123), endian methods 62152/62294/62325 (part-0124); list methods are 62669/62730/83030 (parts-0125/0166). Base reference fields retain import offsets; they are not additionally swapped by the module endian methods.

UpdateDefault stores Rotation@4 and eight random variables@8..103: SizeRate, SizeRateDamping, AngleZ, AngularRateZ, AngularDamping, AngleXY, AngularRateXY, AngularDampingXY. SizeRateDamping and AngularDampingXY initialize to UNIFORM/1/1, including partial-node fallback. UpdateRenderObject has Rotation@4, seven angular random variables@8..91, then RandCoord3D StartSize@92, SizeRate@128, SizeDamping@164.

DrawGpu has Shader string@4, signed frame fields@12/16/20/24, DetailTexture reference@28, SpeedMultiplier@32, GeometryType@36, four MiscValue floats@40..52 and bools@56/57 (EnableSoftParticles defaults true). Default Shader is GPUParticle.fx. DrawLightning has StartPoint@4, EndPoint@16 and RandomSet count@28/pointer@32; each item is 72 bytes containing six random variables. DrawRenderObject has SinkRate@4, ObjectSet count@8/pointer@12 and bools@16/17; each 16-byte item contains nullable RenderGroup string@0, signed NumObjects@8, percentage Percent@12. A missing RenderGroup allocates nothing; a present empty one allocates a NUL block. List item arrays are allocated contiguously before per-item strings.

String lengths follow native ANSI conversion, not UTF-8. Original Value constructor at 118053 (part-0236) uses Marshal.StringToHGlobalAnsi; original String Marshal at 60055/60257 (part-0120) consumes NUL-terminated bytes. Counts exclude the first NUL, payloads include it, and Tracker aligns allocations to four bytes. The added Unicode/combining/surrogate/embedded-NUL tests independently derive ANSI byte lengths; those extra cases are outside the first accepted matrix anchor.

Module parsing is split into Bridge/XmlfxparticleModuleHelpers.inc and XmlfxparticleModuleCases.inc. Nested native allocation is in Native/XmlfxparticleModuleLists.inc. The partial oracle includes XmlfxparticleModuleCases.cs and XmlfxparticleEventCases.cs. The implementation retains the same single-Tracker three-buffer contract.

## Component and SDK boundaries

Unknown or qualified attributes, foreign/unknown child content, duplicate singletons, multiple singleton-wrapper modules and wrong roots/types remain explicit refusals. BAB attaches unnamespaced TypeId to every named schema type (Core AssetDeclarationDocument.ValidateInstances 1213–1235); structural children ignore this metadata as their original Marshal methods do. Optional root TypeId must parse to 686292351; decimal/hexadecimal matches are accepted, mismatches are explicitly refused although the original root ignores that attribute. Qualified TypeId remains refused.

EventBase and unknown-type fallback are component capabilities; they are not a claim that those forms satisfy the shipped XSD. BAB preprocessing, inheritance and dependency resolution remain external. Some malformed inputs are ignored by the original, so component guards do not claim identical malformed-XML permissiveness.

Recovered.XmlPriority.Plugin explicitly registers all fourteen priority roots with original XmlCompiler metadata (XMLCOMPILER_METADATA.md). The final SDK batch uses eighteen legal probes and both Reduced and FullMymod profiles, preserving all seven shipped includes. It does not replace all 301 XmlCompiler types. SDK originals remain unchanged; the recovered implementation never calls them.

## Validation

Run recovery/scripts/TestXmlfxparticle.ps1 under Windows VS2022 x86. It builds /Od and /O2 with CLR4 legacy activation, runs original ProcessInstance on platforms0/1/2/99, and compares InstanceData, RelocationData and ImportsData byte for byte for every accepted fixture. Existing samples and refusal assertions are retained. Added coverage includes all 33 types in singleton/Event cross-category routes, every added field, nested lists/strings, 0/1/2/27/28/29/64/257 mixed Event lists, multiple imports, unknown fallback and metadata boundaries. Independent layout checks cover root/module sizes, padding, counts, allocation order, pointers, relocation/import offsets and sentinels.

The first expanded matrix succeeded at source 77231712d8a56bd385044a9eb9c08ae871dea227, run40089824762. Each mode completed 13792 fixtures, 41376 three-buffer comparisons, 364 refusals and 13792 independent layouts across four platforms. Both modes total 27584 fixtures and 82752 array comparisons. Evidence is recovery/evidence/xml-fxparticle/validation.json plus per-mode result and build/oracle logs. This anchor does not validate later test/code changes.

Run40090297467 (source44fc877dd1f88edec6dc174de03d521f5c577fd4) did not establish SDK acceptance: BAB inserted root TypeId, triggering a component refusal. The follow-up accepts structural TypeId metadata and records failed-input XML/error files separately from successful calls.tsv entries. The final exact-source anchors above now establish the post-fix matrix, Unicode/NUL cases, same-input SDK four-artifact comparisons in both profiles, actual-call trace replay and original-free isolated replay.
