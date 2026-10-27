# Build monitoring and economy settings components

Complete LocalBuildListMonitor, BuildSystemSettings, GiftManagerSettings and MpGameRules roots, including their complete ObjectFilter and SkirmishStartCash dependencies. All are original registered global assets; component-only and SDK-pending.

Original fixed Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

## Original evidence

| Root | Plugin HandleType line | TypeId | TypeHash | Root bytes |
| --- | ---: | ---: | ---: | ---: |
| LocalBuildListMonitor | 7213 | 315614191 | 3578044627 | 24 |
| BuildSystemSettings | 28544 | 3974617950 | 4161179766 | 36 |
| GiftManagerSettings | 27810 | 1047799468 | 3376391493 | 120 |
| MpGameRules | 30922 | 741706624 | 886642248 | 24 |

Module Local Marshal/Endian30420/30480 (`evidence/xml-source/parts/part-0060.cs.txt`), Gift.ObjectGiftSettings/Gift Marshal52850/52874 (`part-0105.cs.txt`), Build Marshal/Endian54026/54077 (`part-0108.cs.txt`), SkirmishStartCash Marshal/Endian58884/58919 and MpGameRules Marshal58959 (`part-0117.cs.txt`). Constructors94613/111994/112631/114633 allocate exact24/120/36/24 roots. Gift constructor crosses the part0223→0224 boundary; allocation120 was checked in the complete original method. Geometry enum helper77022 (`part-0154.cs.txt`) maps SPHERE0/BOX1/CYLINDER2/RING3, unknown1; fixed map at `enums/GeometryType.json`.

Schemas are AssetTypeLocalBuildListMonitor.xsd, System/BuildSystemSettings.xsd, System/GiftManagerSettings.xsd and Shell/MpGameRules.xsd. Full field names/default strings are checked against original Module C-string data and methods.

## Complete binary layouts

- LocalBuildListMonitor24: reserved0; TimeBetweenUpdates float seconds+4 default0.25s; unsigned UpdatesToSkipAfterStart+8 default4; UpdatesToSkipAfterSaveFileLoad+12 default2; UpdatesToSkipAfterTargetPlayerChanges+16 default2; nullable ProducerObjectFilter112 pointer+20. Empty present filter allocates112 and applies RuleNONE3; absence leaves null.
- BuildSystemSettings36: float AllowedHeightVariationForBuildingPlacement+4 default3.0; RotationalSnapIterations+8 default4.0; BuildabilityDisplayRadius+12 default90.0; Geometry BuildabilityDisplayShape+16 defaultCYLINDER2; float BuildabilityDecalMargin+20 default0.142857; BuildabilityDecalSize+24 default30.0; percentage BuildabilityDecalRadiusPercent+28 default18.5185×0.01f; byte BuildabilityUsesStaticDecalSize+32 defaulttrue; padding33–35 zero. Unknown geometry mapsBOX1.
- GiftManagerSettings120: reserved word; inline ObjectGiftSettings at+4, containing ObjectFilter112 at+4 and byte AllowTransferToSkirmishPlayer at+116 with padding117–119. Absent ObjectGiftSettings remains zero. Present ObjectGiftSettings applies true byte even if AllowObjectFilter absent; present empty AllowObjectFilter applies RuleNONE3. The original Plugin endian-corrects only the inline ObjectFilter, leaving boolean byte unchanged.
- MpGameRules24: reserved word; signed NumMinutesBeforePlayersCanTransferMoney+4 default5; inline SkirmishStartCash16 at+8. Cash fields LoCash+8, HiCash+12, ChoiceStepAmount+16 absent0; DefaultChoiceIndex+20 default1 only if cash node is present. Absent cash leaves all16 bytes zero.

Existing recovered ObjectFilter is reused unchanged, with original first-NUL C-string values supplied by this adapter. All enum/bitset/status/Thing list fields are included; child allocation order status28, excluded status28, IncludeThing hashes, ExcludeThing hashes. All scalar words/filter contents swap on non-Win32 including99, while bool bytes stay in place. These four roots create no imported references.

## Implementation and full matrix

`src/Native/Xmlbuildeconomy.{h,cpp}`, `src/Bridge/XmlbuildeconomyCompiler.cpp`, `catalog/XmlbuildeconomyTypes.json`, `tests/XmlbuildeconomyOracle.cs`, `scripts/TestXmlbuildeconomy.ps1`.

Original ProcessInstance plus direct CompileXml, all three arrays, /Od and /O2, platforms0/1/2/99, fixed original DLL fingerprint. Every field receives scalar parser boundaries/default/empty/NUL cases; all Geometry values and fallback/case; all284 KindOf and223 ObjectStatus names, signs/NUL, full Rule/Alignment/Relationship vocabularies on both nullable and inline filters; status presence, Thing lists0–129 and multilingual/NUL names. Independent complete defaults, all offsets, bool/padding, present/absent distinctions,204-byte full Local and188-byte full Gift allocation layouts, status bits/targets, Thing counts/targets, parent-owner relocation order and complete Mp/Build ABI are asserted. Unsupported root/qualified attrs/namespace/duplicate singletons/nested text/unknown children/TypeId mismatch reject explicitly.

Status: implementation/matrix prepared; integrating session records Windows acceptance. No SDK real compilation or active route/Plugin changes.
