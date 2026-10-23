# GameObject recovery entry

Status: original-DLL characterization is being established; no recovered GameObject compiler exists yet. Weapon's recovered compiler must never be registered for GameObject.

Evidence: Tokenizer `-Module-.cs`, blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`.

| Original method | Source lines | Observed contract |
| --- | --- | --- |
| GameObject tracker constructor | 74590–74680 | Native root allocation 592 bytes |
| Marshal(INode*, GameObject*, Tracker*) | 50072–50627 | Root fields, optional child blocks, module/list dependencies |
| EndianCorrect(GameObject*, EndianHelper*) | begins 50628 | Must map fully before implementation |
| Tokenize(void**, GameObject*, Tracker*) | 97296–97814 | Root procedure table 127 records including sentinel; first member KindOf at root+4 |
| Tokenizer.Plugin metadata | GetExtendedTypeInformation case 2486173485 | TypeHash 3415017856; ProcessingHash=TypeHash+version, tokenized=true |

`tests/GameObjectBaseline.cs` calls only the original Tokenizer and records three buffers for defaults and 16 simple scalar fields, across Win32/Xbox360/PS3/99. It verifies deterministic replay and independent bindings at native offsets. It derives the native root location from KindOf's first bookmark, rather than guessing that the token table is always the same size. Characterization success is not recovered compiler success and not an SDK integration result.

The first scalar set: FenceWidth@84, FenceXOffset@88, RemoveTerrainRadius@92, EmotionRange@96, FactoryExitWidth@108, FactoryExtraBibWidth@112, BountyValue@132 (int), BuildTime@136, EnergyProduction@140 (int), EnergyBonus@144 (int), VoicePriority@168 (int), CampnessValueRadius@184, Scale@188, HealthBoxHeightOffset@192, LiveCameraFXPitch@196, FormationWidth@200 (int). Other entries are float. Values -17/0/23 expose zero, sign, and endian differences; this is a starting baseline, not a complete parsing matrix.

Next implementation stage (~35 minutes): map all default allocations and token children before implementing any root-only subset; default GameObject invokes nested/list helpers even when XML omits them. Reuse existing relocation/scalar/string/enum helpers only after checking exact representation. Implement the coherent root scalar/string group with explicit rejection of unsupported blocks, then /Od and /O2 full three-buffer differentials on four targets plus independent layouts and refusal tests. Do not ship a frozen original buffer as recovered logic. GameObject remains on the original SDK route until actual schema-valid mymod integration and original-free replay pass.
