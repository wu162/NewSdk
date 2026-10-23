# GameObject recovery entry

Status: recovered **root component**, not a complete GameObject compiler or SDK plugin. It supports all 88 attributes declared directly by the GameObject XSD and 16 non-polymorphic child types listed below. Unsupported children, fields and foreign namespaces remain explicitly rejected. SDK GameObject routing remains original.

Implementation: `src/Native/GameObject.{h,cpp}`, source-derived `GameObjectFields.h`, `src/Bridge/GameObjectCompiler.cpp`. Validation: `scripts/TestGameObject.ps1`, `tests/GameObject{Oracle,Cases,KindOf}.cs`, `.github/workflows/recovered-gameobject.yml`. The workflow builds a separate `Recovered.GameObject.dll` on x86 under /Od and /O2; it does not replace either original DLL. Successful builds publish the DLL as `recovered-gameobject-component-x86`.

## Original evidence and recovered contracts

Tokenizer `-Module-.cs`, blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`.

| Original method | Source lines | Contract |
| --- | --- | --- |
| GameObject tracker constructor | 74590–74680 | Zero-initialized 592-byte native root |
| Root Marshal | 50072–50627 | Attribute order, literal defaults, child/list dependencies |
| Root endian correction | 50628–50911 | Four-byte fields, separate ushort fields at 576/578, packed bytes at 580–590 |
| Root Tokenize | 97296–97814 | 126 field records plus zero sentinel |
| Whitespace tokenizer | 12996 onward | C isspace; skips whitespace; stops at NUL; punctuation is retained |
| Browser Marshal | 68660 onward | Root count@40, pointer@44; allocate 8-byte string entries, then each string; runs before Description allocation |
| Browser Tokenize | 98397 onward | Count record plus one record per item; item bookmarks point to string entries |
| KindOf wrapper / bitset Marshal | 51203 / 67066 onward | Nine words; 284 names; unknown name maps to bit 149; minus XORs; ALL fills padding and stops later-token processing |
| Duration Marshal | 55021 onward | float(atof), float(double(scale)*double(value)), ceil, uint conversion |

`GenerateGameObjectFields.py` reads the canonical original source and original PE enum tables. It verifies the source blob hash after normalizing checkout CRLF to LF, resolves literal names against the schema, and emits readable field and token metadata. It does not embed original instance-buffer snapshots. Exact helper overload resolution distinguishes inline member bookmarks from nullable helpers. All 126 generated default token keys, offsets and kinds were independently joined to the captured runtime inventory.

Default root Token behavior: 90 member bookmarks, 28 null offsets, eight empty-list count tables. Default root offset=1620, InstanceData=2212 bytes, relocation/import output empty. The eight inline lists are Browser@40, ArmorSet@412, LocomotorSet@420, EquivalentTo@444, AI@472, UpgradeCameo@536, ProjectedBuildabilityInfo@560 and DisplayUpgrade@568. Nonempty Browser adds 12 bytes per item to the token prefix; do not assume root offset 1620 for arbitrary input.

Buildable@428, ThingClass@432 and DisplayColor@452 still receive member bookmarks when their XML nodes are absent. Their helpers are not nullable token helpers. ReplaceModule@496 and InheritableModule@500 have only a null case in the original root Tokenize body; no non-null implementation was found. They remain rejected.

The six supported enums use exact case-sensitive lookup and preserve original unknown-name fallbacks:

| Attribute | Omitted default | Unknown input |
| --- | --- | --- |
| RadarPriority | INVALID=-1 | COMMANDO=2 |
| EditorSorting | NONE=0 | AUDIO=8 |
| ProductionQueueType | INVALID=-1 | AIRCRAFT=4 |
| BuildPlacementTypeFlag | INVALID=0 | BLOCKED=4 |
| UnitCategory | NONE=-1 | AIRCRAFT=3 |
| WeaponCategory | NONE=-1 | BLAST=1 |

Typed IDs at 116,148,152,156,160,164,376 lowercase the string then call FastHashFn(length,text,length). The four original type-specific helpers are identical after type-name normalization: PlayerTemplate@74056, LogicCommandSet@69003, PackedTextureImage@74102, HealthBarTemplate@74148. CrowdResponse@176, UnitTypeIcon@360 and UnitIntro@392 use the decimal suffix after the first backslash; their original IValue helpers start at 65853, 65900 and 65923. The existing resource module preserves this behavior. Tokenized output increments reference indices and has empty ImportsData, including index -1 wrapping to zero; it is not a normal untokenized import stream.

The duration's omitted default `31000` becomes **155**, not 31000: the original scale bytes `0A D7 A3 3B` represent 0.004999999888241291f. Its override is now restored. Original IL at RVA 0x21b8 explicitly performs conv.r4 after atof, again after multiplication, and again after ceil before conv.r8/conv.u4. The recovered x86 conversion preserves the low uint32 word of the signed-int64 conversion; values outside that range (including infinities) produce zero. Adjacent float values around frame boundaries and powers of two, signs, exponents, legacy parsing and overflow are compared directly against the original. In particular, 200.00002 still produces one frame, while 200.00004 produces two; do not infer this boundary using decimal arithmetic.

## Supported child surface

| Children | Root byte offset | Native / Token contract |
| --- | --- | --- |
| DisplayName, DisplayNameTransformed | 396, 400 | Optional 8-byte string objects; member bookmark targets the object, no nested table |
| ArmorSet, LocomotorSet | 412, 420 | Repeated 12-byte elements; count + item table, each item has three member records and sentinel |
| Buildable, ThingClass | 428, 432 | Inline enum; missing defaults 0; unknown input falls back to 1 / 15 |
| EquivalentTo | 444 | Repeated four-byte GameObject typed IDs; lowercase hashing |
| DisplayColor | 452 | Inline ARGB word; byte channels; missing R/G/B=0, A=255 on a present node; omitted node stays zero |
| Flammability | 456 | Optional 16-byte object; Fuel/MaxBurnRate/Decay/Resistance uint32, zero defaults; four member records + sentinel |
| FormationPreviewDecal, FormationPreviewItemDecal | 512, 516 | Optional 16-byte object; inline Texture string and Width/Height floats default 50; three member records + sentinel |
| LiveCameraOffset | 520 | Optional 12-byte x/y/z float object, zero defaults; direct member bookmark |
| UpgradeCameo | 536 | Repeated eight-byte strings; XSD maximum five |
| ShadowInfo | 544 | Optional 60-byte object; 18 fields, packed booleans at 56–59, two texture references, time values and ShadowType enum; 18 records + sentinel |
| VisionInfo | 552 | Optional 40-byte object; ten scalar fields including percentages and an unsigned segment count; ten records + sentinel |
| DisplayUpgrade | 568 | Repeated four-byte UpgradeTemplate typed IDs; XSD maximum two |

Allocation follows original Marshal/schema order regardless of interleaving in XML, preserving order inside repeated lists. Every additional item/nested token table changes the prefix size; no non-default root address is fixed in the implementation. All non-polymorphic nodes permit BAB's unqualified TypeId annotation. Duplicate singleton children, unknown attributes, nested unsupported blocks, foreign namespaces and non-whitespace text in structured nodes fail closed.

Original child evidence: enum node/value helpers 66032/66040 and 74194/74205; ColorTag 13200; optional strings 59358 and 98223; Flammability 47139/66059/95659; Vector3 13257/53290/97853; string/typed lists 51224/51601/74258 and 98092/98151/98397. Set Marshal 49434/49523, list Marshal 65948/65990, item Tokenize 96970/97024, list Tokenize 104104/104131. ArmorSet's 23-bit mask uses fallback bit 12, minus XOR and ALL including padding/stop behavior. LocomotorSet missing Condition remains zero; explicit empty/unknown Condition becomes 12; INVALID is -1.

Visual leaf Marshal methods: ShadowInfo 14189, FormationPreviewDecal 49185, VisionInfo 49243; wrappers allocate 60/16/40 bytes at 52359/66212/66232. Tokenize 77289/96845/96879. The generator extracts their token keys, offsets and order directly from the original source and verifies them against the readable field metadata. ShadowType map/fallback comes from the original PE/helper 52002, not another SDK.

## Current validation

Source `a115ef671b3c4a7a18a11bc720418b4c68e789f4`, successful run [39924338693](https://github.com/wu162/NewSdk/actions/runs/39924338693). Per /Od and /O2: **23,240 fixtures, 69,720 three-buffer comparisons, 224 refusal checks and 16 independent invariance comparisons**. Both modes total 139,440 buffer comparisons. Evidence: `evidence/gameobject-root/validation.json`, `oracle-{Od,O2}.txt` and build/host logs; component DLL is in the run's `recovered-gameobject-component-x86` artifact. Matching primitive regression: [39924338637](https://github.com/wu162/NewSdk/actions/runs/39924338637).

Every mode covers Win32, Xbox360, PlayStation3 and unknown target 99. Existing root cases and all 284 KindOf names remain; added cases cover duration rounding/overflow, every child enum/bit index, resource suffix boundaries, empty/nonempty allocations, list ordering/growth, masks, strings, packed bools, percentages and mixed child groups. Independent checks cover default/root/Token layouts, nested member keys/types/addresses and sentinels, list headers/items, string allocation and scalar defaults. SDK was not run; original GameObject routing is retained. Shared relocation/parsing/resource/enum-bitset modules were not changed.

Original characterization remains separate: 196 fixtures, 588 repeated three-buffer comparisons; source `46b0ec5020b3911ec0b0403a3d60f63426590673`, run [39897995324](https://github.com/wu162/NewSdk/actions/runs/39897995324). `evidence/gameobject/source-layout.json` joins the original source inventory with all four targets' observed default buffers. Root component evidence is under **`evidence/gameobject-root`**; never confuse it with original-only baseline success or SDK integration.

## Next coherent stage (~35 minutes)

Continue with one complete remaining non-polymorphic capability group, rather than one field per CI run. A useful next group is pointer-list and auto-resolve support: BuildFadeInOnCreateList / BuildVariations plus AutoResolveArmor / AutoResolveWeapon / AutoResolveInfo, following their full child dependencies and allocation/Token/endian contracts. GameDependency/ObjectResourceInfo, SkirmishAIInformation, CrusherInfo, ProjectedBuildabilityInfo, Geometry, audio arrays and all polymorphic module groups remain unsupported.

Important input-pipeline finding: StringList is an xs:list of xs:token in Includes/Base.xsd, but the original pointer-list helper 66048 calls Node.GetChildNodes(null), whose implementation 75998 selects element children; it does not use Browser's whitespace-string overload 68660. Characterize the actual BAB-preprocessed representation and original behavior before implementing these nodes; do not silently treat their text as Browser tokens. The remaining original module/ReplaceModule stubs must remain refused until reconstructed.

Keep complete /Od and /O2 four-target three-buffer comparisons and independent/refusal checks. Save incomplete checkpoints on work branches, integrate coherent prepared groups with ordinary fast-forward pushes, preserve concurrent evidence commits, and obtain final successful sourceCommit/runId evidence. No GameObject SDK plugin exists yet: prepare schema-valid mymod inputs and batch real routed/original-free replay validation at a later integration milestone.
