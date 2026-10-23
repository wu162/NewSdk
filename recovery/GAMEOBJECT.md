# GameObject recovery entry

Status: original-DLL characterization passed (196 fixtures, 588 repeated three-buffer comparisons; source 10fcc4cb7d95cd1f3bb30ec5778f7eab1d9f56c0, run 39897241362); no recovered GameObject compiler exists yet. Weapon's recovered compiler must never be registered for GameObject.

Evidence: Tokenizer `-Module-.cs`, blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`.

| Original method | Source lines | Observed contract |
| --- | --- | --- |
| GameObject tracker constructor | 74590–74680 | Native root allocation 592 bytes |
| Marshal(INode*, GameObject*, Tracker*) | 50072–50627 | Root fields, optional child blocks, module/list dependencies |
| EndianCorrect(GameObject*, EndianHelper*) | 50628–50911 | Must map fully before implementation |
| Tokenize(void**, GameObject*, Tracker*) | 97296–97814 | Root procedure table 127 records including sentinel; first member KindOf at root+4 |
| Tokenizer.Plugin metadata | GetExtendedTypeInformation case 2486173485 | TypeHash 3415017856; ProcessingHash=TypeHash+version, tokenized=true |

`tests/GameObjectBaseline.cs` calls only the original Tokenizer and records three buffers for defaults and 16 simple scalar fields, across Win32/Xbox360/PS3/99. It verifies deterministic replay and independent bindings at native offsets. It derives the native root location from KindOf's first bookmark, rather than guessing that the token table is always the same size. Characterization success is not recovered compiler success and not an SDK integration result.

The first scalar set: FenceWidth@84, FenceXOffset@88, RemoveTerrainRadius@92, EmotionRange@96, FactoryExitWidth@108, FactoryExtraBibWidth@112, BountyValue@132 (int), BuildTime@136, EnergyProduction@140 (int), EnergyBonus@144 (int), VoicePriority@168 (int), CampnessValueRadius@184, Scale@188, HealthBoxHeightOffset@192, LiveCameraFXPitch@196, FormationWidth@200 (int). Other entries are float. Values -17/0/23 expose zero, sign, and endian differences; this is a starting baseline, not a complete parsing matrix.

Next implementation stage (~35 minutes): map all default allocations and token children before implementing any root-only subset; default GameObject invokes nested/list helpers even when XML omits them. Reuse existing relocation/scalar/string/enum helpers only after checking exact representation. Implement the coherent root scalar/string group with explicit rejection of unsupported blocks, then /Od and /O2 full three-buffer differentials on four targets plus independent layouts and refusal tests. Do not ship a frozen original buffer as recovered logic. GameObject remains on the original SDK route until actual schema-valid mymod integration and original-free replay pass.

## Full token inventory (additional verification pending)

ExtractGameObjectEvidence.py parses all 126 field records plus the sentinel, preserving original numeric name/type hashes, native offsets, helper identifiers, method source ranges, and explicit classification. It fails on unparsed records rather than silently leaving gaps. Runtime characterization additionally resolves token name hashes through the original HashProvider against names in AssetTypeGameObject.xsd; unresolved names remain visible.

Records 109 and 110 refer to ReplaceModule@496 and InheritableModule@500. Their original root Tokenize bodies only set offset zero when the pointer is null; no non-null handler is present in that decompilation. Do not substitute a normal nested procedure table. Confirm BAB preprocessing and helper Marshal/endian before deciding whether these inputs belong in the recovered plugin surface. This is an observed source gap/stub, not proof that arbitrary non-null nodes are safe to accept.
