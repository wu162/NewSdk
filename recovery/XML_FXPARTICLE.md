# FXParticleSystemTemplate XmlCompiler component

Status: implementation and oracle ready; Windows verification pending.
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
Event list header@156 remains zero in the supported empty-Event boundary.
All data uses a single Tracker::MakeRelocatable(bigEndian,0), retaining all three arrays.
Platform0 is little endian;1/2/99 big endian. Packed bools stay bytes.
Reference imports retain their actual allocated field offsets. Typed IDs use the shared hash.

## Supported original TypeId dispatch

Routing uses numeric TypeId, never element names, and deliberately allows original
cross-category routing. The same supported TypeIds can appear in any singleton wrapper.

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

Every field in these 25 types is implemented, including all Volume/Velocity variants,
RandCoord3D, nested ranges/vectors, Physics flags and DrawSwarm texture imports.
Missing/zero/unrecognized TypeId produces the original four-byte zero-type fallback,
provided the module has only the TypeId attribute and no children.
Empty singleton wrappers leave a null pointer. Unknown enum fallbacks are:
Priority6, Shader1, Type2, WindMotion3, WavefrontShape1; names are case sensitive.
Namespaced enum tables were read from original static-data/fields-0030.json;
stored sorted name/value pairs were combined with exact shipped schema name sets.
This avoids the manifest exporter omitting namespaced enum maps.

## Explicit refusal boundary

Nonempty Event is unsupported. Known unsupported module TypeIds are:
FXParticleEventBase=2693838876, FXParticleUpdateDefault=2933826200, FXParticleUpdateRenderObject=234581578, FXParticleDrawGpu=2248017887, FXParticleDrawLightning=3036920908, FXParticleDrawRenderObject=3408220251, FXParticleEventCollision=1075723135, FXParticleEventLife=3465979181.
They are explicitly refused rather than emitted as an unknown fallback.
Unknown or namespaced attributes, foreign/nested/unknown child content, duplicate
singletons, multiple module children and wrong root/types are refused.
Some malformed inputs are ignored by the original; these guards are an explicit
component boundary and not a claim of identical malformed-XML permissiveness.
BAB schema preprocessing, inheritance and real SDK integration are outside this component.

## Validation

Run recovery/scripts/TestXmlfxparticle.ps1 in Windows VS2022 x86 devshell.
The script is self-contained against the fixed baseline runtime sources, requiring
no Xmleffects files or Draft files. Builds /Od and /O2, Framework-v4 x86 host, legacy
CLR activation config. Tests original ProcessInstance on platforms0/1/2/99; every
accepted fixture compares InstanceData/RelocationData/ImportsData byte for byte.
Covers every root attribute, each enum, all supported module fields/IDs, missing/0/
unknown dispatch, all five wrappers and cross-category routing, ranges/vectors,
null/empty lists, 0/1/2/27/28/29/64/257 keyframes, imports and explicit rejection cases.
Independent checks cover176-byte root, exact module sizes/IDs, block targets, counts,
relocation/import offsets and sentinels, padding and the empty Event header.

Outputs recovery/evidence/xml-fxparticle/validation.json with succeeded/sourceCommit/runId,
per-mode result JSON and Od/O2 build/host/oracle logs. Logs include fixture, three-array,
layout and refusal counts for all four platforms. Success is set only after both matrices.
Initial implementation reuses and corrects the independently supplied disabled draft
from effects commit baca81ec04503d2eabeca1be28f41e49e62b9277; no Draft files are merged.
Windows and SDK validation are not claimed by this checkpoint.
