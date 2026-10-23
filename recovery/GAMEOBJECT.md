# GameObject recovery entry

Status: recovered **root component**, not a complete GameObject compiler or SDK plugin. It supports all 88 attributes declared directly by the GameObject XSD and 38 child kinds: the prior 29 non-polymorphic groups, complete SkirmishAIInformation, two empty module markers, and six module containers restricted to the 46 complete known types (31 base-only, seven additional Body, six ClientUpdate, two sound playback) plus unknown fallback (276 other known module types remain rejected). Unsupported children, fields and foreign namespaces remain explicitly rejected. SDK GameObject routing remains original.

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

Buildable@428, ThingClass@432 and DisplayColor@452 still receive member bookmarks when their XML nodes are absent. Their helpers are not nullable token helpers. ReplaceModule@496 and InheritableModule@500 each allocate one zero byte, padded to four by Tracker. Their XSD types are empty. The root Tokenize body creates no pointee bookmark or nested table, so their token offset remains zero even when present. Both empty types are now fully differentially verified; unknown content is rejected.

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
| GameDependency | 404 | Optional 40-byte object; two nullable 60-byte ModelCondition masks, nullable 28-byte ObjectStatus mask, three typed-ID lists and optional full ObjectFilter |
| ObjectResourceInfo | 408 | Optional eight-byte list header; BuildCost is repeated eight-byte MoneyTransaction (Account/Amount uint32), each with two member records + sentinel |
| BuildFadeInOnCreateList, BuildVariations | 436, 440 | Optional eight-byte list headers; XSD-valid text creates an empty list; absent pointer differs from present-empty; non-XSD element-child representation refused |
| AutoResolveArmor, AutoResolveWeapon | 504, 508 | Optional 20-byte objects; resource reference + two ordered UpgradeTemplate ID lists, nested count/item tokens |
| AutoResolveInfo | 548 | Optional 36-byte object; three eight-byte strings, ModuleData ID and two resource references; six member records + sentinel |

Allocation follows original Marshal/schema order regardless of interleaving in XML, preserving order inside repeated lists. Every additional item/nested token table changes the prefix size; no non-default root address is fixed in the implementation. All non-polymorphic nodes permit BAB's unqualified TypeId annotation. Duplicate singleton children, unknown attributes, nested unsupported blocks, foreign namespaces and non-whitespace text in structured nodes fail closed.

Original child evidence: enum node/value helpers 66032/66040 and 74194/74205; ColorTag 13200; optional strings 59358 and 98223; Flammability 47139/66059/95659; Vector3 13257/53290/97853; string/typed lists 51224/51601/74258 and 98092/98151/98397. Set Marshal 49434/49523, list Marshal 65948/65990, item Tokenize 96970/97024, list Tokenize 104104/104131. ArmorSet's 23-bit mask uses fallback bit 12, minus XOR and ALL including padding/stop behavior. LocomotorSet missing Condition remains zero; explicit empty/unknown Condition becomes 12; INVALID is -1.

Visual leaf Marshal methods: ShadowInfo 14189, FormationPreviewDecal 49185, VisionInfo 49243; wrappers allocate 60/16/40 bytes at 52359/66212/66232. Tokenize 77289/96845/96879. The generator extracts their token keys, offsets and order directly from the original source and verifies them against the readable field metadata. ShadowType map/fallback comes from the original PE/helper 52002, not another SDK.

## Newly restored groups (2026-10-23)

All evidence still refers to original Tokenizer module blob `6e40a9ed6c9f961d294d49768d911e6a6f354619`.

- AutoResolveArmor / Weapon: Marshal 49424 / 49414, wrappers 66192 / 66202 allocate 20 bytes, Tokenize 96949 / 96928, endian wrappers 66505 / 66516. References remain at byte 0; RequiredUpgrade / ExcludedUpgrade lists occupy 4 / 12. A present empty object has root=1692, object=2284 and total InstanceData=2304. List headers allocate even when counts are zero; count tables have no sentinel. Allocation groups required upgrades before excluded upgrades regardless of interleaving in XML.
- AutoResolveInfo: Marshal / endian 49450 / 49483, wrapper 66222 allocates 36 bytes, Tokenize 96991. UnitType@0, Body@8, Armor@12, Weapon@16, Leadership@20, CombatChain@28. Body uses the same lowercase FastHash contract as the ModuleData helper 67672; string lengths and ID are endian-corrected, reference indices are finalized by the tracker. Empty layout: root=1704, object=2296, total=2332. Token metadata is source-derived by GenerateGameObjectFields.py.
- ObjectResourceInfo: wrapper 65937 allocates eight bytes and selects BuildCost elements. MoneyTransaction Marshal 13379, list Marshal / endian 55634 / 55674; both Account and Amount use strtoul base 0 and occupy 0 / 4. Tokenize 96866 delegates to list helper 99639 and item method 76735. Empty layout: root=1656, object=2248, total=2256; each item adds 48 token bytes plus eight data bytes. Refuse unknown/namespaced attributes, nested blocks and non-whitespace structured text; allow BAB TypeId annotations.
- GameDependency: root@404, Marshal / endian 40407 / 40444, wrapper 58600 allocates 40 bytes, Tokenize 77439. Mask pointers at 0 / 4 / 8, typed-ID lists at 12 / 20 / 28, optional ObjectFilter@36. Lists are RequiredObject (GameObject), ForbiddenUpgrade and NeededUpgrade (UpgradeTemplate). Explicit empty masks allocate arrays, omitted masks remain null; Token points directly to allocated words. Mask parsing reuses the unchanged shared implementations, including fallback bits 233 / 198, minus-XOR and ALL padding/stop semantics. Optional ObjectFilter uses the existing 112-byte Marshal/Tokenize/endian implementation. Empty layout: root=1752, object=2344, total=2384. ObjectFilter is now linked by TestGameObject.ps1 and included in this workflow's path filter; shared implementation is unchanged.

### Pointer StringList input contract

Root Marshal 50270 / 50283 calls pointer-list helper 66048, which allocates an eight-byte header and invokes Node.GetChildNodes(null). Node implementation 75998 uses SelectChildren(Element) for a null name (76050); list Marshal 74258 therefore enumerates **elements**, not text tokens. Token pointer wrapper 104168 delegates to the normal string-list count/item helper. This differs from Browser's separate whitespace-string overload.

Actual SDK `Includes/Base.xsd` declares StringList as `xs:list itemType="xs:token"`. BAB Core `AssetDeclarationDocument.ValidateInstances` at 1214–1237 walks existing nodes, processes reference types, and adds TypeId; `HandleReferenceType` at 1324 does not expand StringList into child elements. `InstanceDeclaration.Node` returns the same XmlNode (202), which is passed to the plugin at AssetDeclarationDocument 1107. This is source analysis of the actual input chain, **not a new real SDK integration run**.

The test loads the actual SDK StringList declaration, validates text nodes with .NET XmlDocument, adds the allowed unqualified TypeId annotation and compares them directly against the original plugin. Empty, multiword, whitespace, Unicode, CDATA/comment/PI cases all preserve an explicit empty list. One present list has root=1632, list=2224, total=2232; both have root=1644 and total=2252, with headers in schema order. Original-only synthetic `<Item>` children confirm the helper's nonempty element path; a separate XSD check rejects this representation and the recovered component explicitly refuses it. **Non-XSD nonempty pointer-list inputs are not supported.** No whitespace-token interpretation was invented.

## Historical validation: 23-child checkpoint

Source `074d267b2835be89ade3944c1cbe6d2f94b3793a`, successful run [39928834132](https://github.com/wu162/NewSdk/actions/runs/39928834132). Per /Od and /O2: **34,052 fixtures, 102,156 three-buffer comparisons, 492 refusal checks and 124 invariance comparisons**. Both modes total 204,312 buffer comparisons. Win32, Xbox360, PlayStation3 and unknown 99 are retained. Evidence: `evidence/gameobject-root/validation.json` and both oracle/build/host logs. The run's `recovered-gameobject-component-x86` artifact contains the component DLL.

Full matrix retains all prior 88-root/KindOf/visual/collection cases and adds all seven new child types and their complete valid dependency groups. GameDependency tests all 457 ModelCondition names and 223 ObjectStatus names, signed/repeated flags, nullable masks and padding, typed-list growth/order, nested ObjectFilter, TypeId and refusal cases. AutoResolve covers references, all string/ID fields, two nested list families and independent token/layout assertions. Resource tests cover base-0 unsigned parsing, overflow, repeated items and nested token sentinels. Pointer StringLists include actual schema validation, direct original text behavior, separate original nonempty characterization and schema-invalid rejection.

Earlier coherent checkpoints: AutoResolve source `8650678284fd171f0dc076732bf12d0944e2d1ef`, run 39926787685, per mode 24,120 fixtures / 72,360 comparisons / 336 refusals / 32 invariance. StringList + resource lists source `93d16a98c24cde4ac9d2e00d26c6d76c96ff7015`, run 39927770965, per mode 24,412 / 73,236 / 420 / 120. These were complete capability groups, not individual fields. Original characterization remains under `evidence/gameobject`; it is not SDK integration or the current component matrix.

Primitive regression: [39928834050](https://github.com/wu162/NewSdk/actions/runs/39928834050), success, matching source. Real SDK runs this stage: **zero**. GameObject SDK routing remains original; no formal GameObject plugin or complete replacement DLL exists yet. Relocation, scalar/resource, shared enum-bitset and ObjectFilter implementation files were not changed.

## Next coherent stage (~45 minutes)

Continue SkirmishAIInformation with its complete nullable overrides, AIContextualPower/AISpecialPowerInfo, AISpecialPowerWeaponData, both AIBaseSelectionCriteria and ObjectFilter dependencies. Source-only layout/method investigation is in `GAMEOBJECT_AI.md`; it is not implemented or differentially validated. The Crusher, ProjectedBuildability, Geometry and all three audio/EVA groups below are now validated. All polymorphic module groups and nonempty ReplaceModule/InheritableModule paths remain unsupported and refused. Current exact validation anchors are in RESUME.md and the last section below.

Keep /Od, /O2, four-target three-buffer comparisons, independent layouts and explicit refusal coverage. Save meaningful checkpoints promptly and preserve concurrent commits and Actions evidence. Keep SDK batching: prepare schema-valid mymod inputs and formal routed/original-free replay only at a later GameObject integration milestone. Component success does not replace SDK validation.


## Crusher and projected buildability (2026-10-23)

Original Tokenizer module blob remains `6e40a9ed6c9f961d294d49768d911e6a6f354619`. CrusherInfo Marshal/endian 49538/49594, pointer wrapper 66242 (36 bytes at root+556), Tokenize 97045. Twelve attributes occupy scalar words0/4/8/12, resource reference16, signed byte levels28/29 and booleans30–34. MinCrushVelocityPercent default is **1 percent (0.01f)**, not 100 percent. ExtraCrushLevels header20, eight-byte item: ModelConditionMatch enum0 (omitted zero; explicit unknown233), signed byte levels4/5. Marshal 47117/list65637, endian65677, Tokenize95638. Full scalar enum table is PE/source-derived; 457 names tested. Empty Crusher root1800/object2392/total2428; each extra level adds60 token and8 data bytes.

ProjectedBuildabilityInfo Marshal/endian49635/49700, list wrapper66252 uses112-byte stride at root+560; Tokenize97106/list104375. Radius0, optional allocated float pointer4, placement mask8, ObjectStatus words12–36, ModelCondition words40–96, height100, ObjectFilter pointer104, boolean108. Defaults: Radius0, RadiusY null, placement INVALID bit0, absent status zero, model STRUCTURE_UNPACKING, height50, `PrimaryBuidability` true (original misspelling). BuildPlacementType fallback4, bit count5, XOR/ALL semantics reuse unchanged helper. Every223 status and457 model names and symbols tested. Empty item root1740/object2332/total2444; repeated lists and nested full filter supported.

Validated source `4a8989d232432330a230cdcb58f0e7bbd7e6462e`, run39935810403 success: per mode42,704 fixtures /128,112 comparisons /572 refusals /128 invariance checks, both modes and all four platforms; baseline39935810494 success. SDK not run.

## Raw Geometry complete group — Windows validated

This is GameObject Geometry at root+492, **not GeometryInfo**. Original Marshal37595/pointer66164 allocates28 bytes; Tokenize77105. IsSmall byte24; nullable Vector2 anchor@0; optional48-byte PlacementShape@4; Shape list@8; ContactPoint list@16. Allocate anchor, placement, shape list and item allocations, then contact list and item allocations in schema order regardless of XML order.

Shape Marshal37207/endian37284/Tokenize76939: Type enum0 (defaultSPHERE0, unknownBOX1), ContactPointGeneration4 (defaultNONE0, unknownINFANTRY3), Name string8, Major/MinorRadius16/20 default1, Other string24, Height32 default1, optional angle36 (absent0), nullableVector3 offset40, Active/UsedForHealthBox booleans44/45 defaulttrue. ContactPoint Marshal37060/Tokenize76875 uses20-byte stride: Label string0, inlineVector3 Pos8. Geometry/Shape/ContactPoint Token keys/types are generated directly from original methods and source fingerprint checked.

Empty Geometry direct component input is characterized (root1716/object2308/total2336); XSD requires at least oneShape. Empty Shape defaults and independent stride/offset assertions, every enum spelling, scalar/string/vector boundaries, optional pointers, long/repeated/interleaved lists, TypeId and structural refusals are included. These raw component cases do not claim XSD-complete MOD or SDK integration. Validated source140963c2f2be6c1091ce506c5130205b77fe072b, component39955059001 and baseline39955059019 success. Per mode43,624 fixtures/130,872 comparisons/652 refusals/132 invariance checks, both optimization modes/four platforms. SDK not run.

## Audio/EVA complete group — Windows validated

AudioArraySound root528 ->8-byte header, eight-byte entries (reference0, enum4): original list54192 / wrapper54580, itemMarshal18493, Token77155/77170; enum37 names, unknown31, omitted0. EvaEventArray root532 ->8-byte header, eight-byte entries (reference0, enum4): list54757/wrapper54832, itemMarshal18928, Token77259/77276; enum30 names, unknown11, omitted0. Empty arrays root1656/object2248/total2256; each entry adds48Token+8data bytes.

AudioArrayVoice root524 ->24-byte header with AudioEntry/ObjectSpecificEntry/NamedEntry lists at0/8/16. Original Marshal18543; lists54310/54358/54400 use12/16/12-byte stride. SoundOrEvaEvent base0/4: nullable allocated reference pointerSound0 (present empty still allocates4bytes) and inlineEvaEvent reference4 (absent uses empty fallback). Original13587; all three items embed a nested base procedure table. AudioType@8:67 names/fallback32 for ordinary voice,6 names/fallback0 for object-specific voice. TargetObject@12 lowercases as typed assetID; NamedEntry Name@8 **does not lowercase** before FastHashFn(length,text,length) (Marshal18502). Local ExactStringHash preserves signed-byte tail behavior; no shared helpers modified. All four enums source/PE-derived; procedure keys/types generated directly from original methods.

EmptyVoice root1704/object2296/total2320. Per entry token/data deltas: ordinary84/12, object-specific96/16, named84/12, plus4bytes for each presentSound. Complete tests include all140 enum names, missing/empty/unknown, reference index edges, Name length/hash/case/high bytes, typedTargetObject case folding, every repeated list, schema allocation order, TypeId, structural refusals and mixedGeometry/Crusher/Projection samples. Validated source d27f0b932b4855d708cf0f09095c0b50026251e4; component39956084622 and baseline39956084413 success. Per mode44,980 fixtures/134,940 comparisons/796 refusals/136 invariance checks; total269,880 buffer comparisons across both modes, four platforms including99. SDK remains original route.

Next complete group source investigation: `GAMEOBJECT_AI.md`; not implemented or differentially validated yet.

## 2026-10-23 complete AI/common-module batch

Source `afac552209d4880e883f2d37189c896a1823e4a6`, GameObject run https://github.com/wu162/NewSdk/actions/runs/39961644190 success; primitive regression run39961644145 success. Each /Od and /O2:69,744 fixtures,209,232 three-buffer comparisons,8,264 refusals,204 invariance comparisons; targets0/1/2/99. Both modes total418,464 buffer comparisons. This final matrix includes all prior GameObject groups plus complete SkirmishAIInformation, empty ReplaceModule/InheritableModule, all six module containers and all31 common eight-byte module types. Other291 known Marshal types remain rejected. SDK not run, original routing retained.

See GAMEOBJECT_AI.md, GAMEOBJECT_MODULES.md and next-group source investigation GAMEOBJECT_BODY.md. No shared scalar/relocation/resource/bitset/ObjectFilter implementation changed.
