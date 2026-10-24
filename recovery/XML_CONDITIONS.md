# Gameplay conditions component recovery

Component-only scope: four original registered XmlCompiler entry points. SDK integration and real SDK compilation remain pending; this round does not change the main Plugin or SDK routing.

| Entry | TypeId | TypeHash | Root bytes | Global schema asset |
| --- | ---: | ---: | ---: | --- |
| BaseConditionData | 3899256902 | 2796907385 | 12 | no |
| ObjectConditionData | 519519951 | 3621799508 | 16 | no |
| UpgradeConditionData | 76901015 | 2863617245 | 20 | no |
| GameplayConditionData | 2309012900 | 3176926574 | 44 | yes |

The first three roots are real independent original Plugin entries despite not being global CnC3Types.xsd assets. Their first word is Count, default 1, rather than the reserved BaseAsset word. GameplayConditionData has the reserved word and an inline ObjectConditionData at +20.

## Evidence and complete behavior

Original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, fixed original DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- `xml-source/parts/part-0083.cs.txt`: original Marshal/EndianCorrect DieMuxDataType (Module line41658), BaseConditionData (41784), ObjectConditionData (41842), GameplayConditionData (41897); Gameplay endian continues in part0084. Root tracker constructors at96979/97070/97161/97252 prove12/16/20/44 byte allocation.
- `parts/part-0140.cs.txt` line70220: optional UpgradeConditionData allocation20, UpgradeRequired list first, then inherited Base. Original Plugin HandleType UpgradeConditionData around line10223 separately implements the same root behavior.
- `parts/part-0127.cs.txt` line63989 and part0128: UpgradeRequired list4 stride and per-child typed asset hash; part0175 line87615 proves C-string truncation, lowercase conversion, FastHash rather than an import reference.
- `parts/part-0170.cs.txt` line85247: VeterancyLevels whitespace token list,8-byte ReloString entries, each string separately allocated. DeathType34-bit parsing at85306; GameplayCondition3-bit at85431. GameplayMode4-bit at85550 and GameDifficulty4-bit at85669 in part0171. Fallbacks are DeathType33, GameplayCondition1, GameplayMode0, Difficulty3. Exact original PE tables are retained under `xml-source/enums/{DeathType,GameplayConditionType,GameplayModeType,GameDifficulty}.json`.
- Schema `SDK/Schemas/xsd/AssetTypeGameplayCondition.xsd` and `Includes/DieMux.xsd` are cross-checks; defaults and allocation order follow original methods, not schema presence constraints.

Base: Count@0 signed CRT integer default1; nullable112-byte ObjectFilter@4; Unique byte@8 defaultfalse. Object: nullable44-byte DieMuxData@12 is allocated before the inherited ObjectFilter regardless of XML element order. Upgrade: count@12/pointer@16 typed weak hash list is allocated before the inherited filter.

Gameplay: condition bits@4 defaultempty; optional float TimeReq pointer@8; allowed mode bits@12 default SKIRMISH RANKED_ONLINE (10); difficulty bits@16 default all four (15); inline PrimaryObject@20; optional Object16@36; optional Upgrade20@40. Missing inline PrimaryObject remains zero; present-empty PrimaryObject applies Count1. Time pointer is allocated first, then PrimaryObject suballocations, RequiredObjectCondition and its suballocations, RequiredUpgradeCondition and its suballocations.

DieMux44: VeterancyLevels list@0; optional ObjectStatus223-bit pointers@8/@12; DamageAmountRequired@16 default-1; MinKillerAngle@20 default1d; MaxKillerAngle@24 default-1d; DeathTypes34bits@28; DeathTypesForbidden34bits@36. Present-empty statuses allocate28-byte zero blocks. Angle/time use the existing proven float rounding and unit initializers.

All ObjectFilter fields and nested IncludeThing/ExcludeThing weak hashes are supported through the existing independent native helper. Attributes and thing text are truncated at the original IValue C-string boundary before helper calls. TypeId remains BAB metadata; unknown/qualified attributes, foreign namespaces, unsupported children, text in structured nodes, and duplicate singleton nodes are explicitly rejected.

## Verification entry points

`TestXmlconditions.ps1` builds native/bridge sources for x86 /Od and /O2, then runs original ProcessInstance versus recovered ProcessInstance and direct CompileXml, all three arrays, on platforms0/1/2/99. Oracle: `XmlconditionsOracle.cs`. Native: `Xmlconditions.cpp`, PE-derived enum table `XmlconditionsEnums.inc`; public routes: `XmlconditionsCompiler.cpp`; catalog: `XmlconditionsTypes.json`.

The full matrix covers every field, all Gameplay/Death enum words with casing/sign/ALL/NUL boundaries, six ObjectFilter attachment paths with every KindOf/ObjectStatus word, DieMux status words on all three attachment paths, scalar and CRT overflow boundaries, unit parsing, all weak-hash text boundaries, metadata, string/list growth including16/32/257/1024, duplicate/unknown rejection, and independent fixed ABI assertions for roots, present/absent inline defaults, allocation order, bitmasks, angles, byte booleans and relocation owner/target ordering. Full /Od/O2 original-DLL differential evidence is produced by the parent integration run; no success is claimed here before that run finishes.

Local preflight: native syntax checked with g++ after removing only the x86 pointer-size assertion in a temporary copy; the real source retains that assertion. Linux lacks32-bit C++ headers and cannot execute the original Windows DLL. This preflight does not replace the Windows matrix.
