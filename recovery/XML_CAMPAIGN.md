# Campaign, missions and tutorial components

Five complete original registered global XmlCompiler entries. Component-only; SDK pending, no main SDK Plugin routing change or real SDK compilation.

| Entry | TypeId | TypeHash | Root bytes | Complete scope |
| --- | ---: | ---: | ---: | --- |
| CampaignTemplate | 1482556238 | 3502529123 | 84 | two String8, Mission strong list4, FinalMovie28, AlternateFinalMovie28 |
| CampaignEnumerator | 731899443 | 118801026 | 12 | Campaign strong list count@4/pointer@8 |
| MissionTemplate | 3477794083 | 3533302842 | 148 | eight String8@4..60, five unsigned victories@68..84, Prerequisite list@88, Objective@96, BonusObjective@104, AICommander@112, IntroMovie28@120 |
| MissionHotSpotSettings | 3175516876 | 4275530952 | 16 | WaypointMarker strongref@4, DecalTemplate inline list8@8 of RadiusDecalComponent64 |
| TutorialMapData | 3408790761 | 2508525962 | 12 | Lesson list@4, Lesson24 three String8 Map/Summary/ImageName |

Nested CampaignMovie28: ImageRef strongref@0, FileName String8@4, ScreenName@12, Description@20. CampaignAICommander24: Portrait strongref@0, Personality String8@4, Description@12, Faction enum@20. Missing nested nodes remain zero, presentempty strings allocate padded bytes.

## Original evidence and semantic boundaries

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; fixed DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Part0062 lines31246/31272: CampaignMovie full Marshal/EndianCorrect;31303/31324: Commander full Marshal/EndianCorrect;31357/31409: CampaignTemplate;31442 through part0063: MissionTemplate complete Marshal/EndianCorrect.
- Mission processes all eight strings, five victory unsigned values, then Prerequisite signed list4, Objective String8 list, BonusObjective String8 list, Commander24 list, then inline IntroMovie suballocations. XML order does not reorder these allocations.
- Part0132 line66092 `Marshal<CampaignAICommander,3>` delegates directly to the ordinary list; part0169 line84911 allocates24byte records for the full actual count. Mission EndianCorrect in part0063 also uses the ordinary count-based list. It does not truncate or reject above schema3 commanders. Objective/BonusObjective likewise have no schema5 truncation.
- `PrerequisiteTemplate` is schema pipelineOnly. The original Mission Marshal does not query it, allocate it or create imports. This component explicitly recognizes/validates the leaf and ignores its text, preserving actual original component output; this is not a general unknown-field fallback. BAB preprocessing responsibility remains separate.
- Original Plugin CampaignEnumerator HandleType around7758 marshals Campaign strongrefs list@4 and swaps count before tracker finalization.
- Part0101 lines50751/50776: complete Lesson24 three String8 Marshal/EndianCorrect. Part0152 lines76379/76421: Lesson list. Original Plugin TutorialMapData HandleType around25665 routes Lesson list@4.
- Part0107 line53660: Hotspot complete WaypointMarker and inline DecalTemplate list8. Part0053 lines26539/26650: RadiusDecalComponent64 complete Marshal/EndianCorrect; part0128 line64165: complete64byte component list.
- RadiusDecal64 is Texture strong@0, Texture2@4, ShadowType@8, OpacityMin percentage@12 and OpacityMax@16 (getter defaults `1.0` mean float0.01, not1), ten floats@20..56 (MinRadius@36/MaxRadius@40 default1, remaining0), packed Color@60. Presentempty Color applies alpha255; absent Color stays0. PE enum ShadowType has unknownfallback3; Faction unknownfallback0. Tables retained under `xml-source/enums/ShadowType.json` and `FactionType.json`; lookup uses exact case-sensitive C-strings.
- Root tracker constructors94886/94977/95068/110083/112358 and original decompiled sequential layouts confirm all root/helper sizes. Schema cross-checks: AssetTypeCampaignManager.xsd, AssetTypeTutorialData.xsd, System/MissionHotSpotSettings.xsd and Includes/Decal.xsd. Original Marshal semantics control component defaults and allocation.

Strictly rejects unknown/qualified attrs, unknown/foreign children, unsupported structured text and duplicate movie/decal/color singletons. C-string boundaries, CRT signed/unsigned/floats, percentages, strong reference index+1 and ownerblock/value bookmark ordering use the existing proven native helpers.

## Validation

`Xmlcampaign.{h,cpp}`, public routes `XmlcampaignCompiler.cpp`, catalog `XmlcampaignTypes.json`, oracle `XmlcampaignOracle.cs`, runner `TestXmlcampaign.ps1`.

Runner retains x86 /Od and /O2; platforms0/1/2/99; original ProcessInstance versus restored ProcessInstance/direct CompileXml; all three exact arrays. Oracle covers every root/nested field, PE enums and casing/sign/NUL/fallbacks, every victory and signed prerequisite CRT boundary, all strings including Unicode/empty/NUL and long65535, all reference boundaries, radius percentages/floats/packed channels, all lists through1024 (including commander4+ and objectives6+), pipelineOnly ignored behavior, metadata and strict rejection, plus fixed independent ABI assertions for148/84/16/12 roots,28/24/64 helpers, source allocation ordering, values, bytes and complete import/relocation arrays.

Local native g++ syntax preflight passes after removing only the pointer-size assertion in a temporary copy; production source retains x86 enforcement. Parent Windows differential supplies final evidence. This document does not claim unrun component acceptance or SDK support.
