# GameObject recovery entry

Status: recovered **root component**, not a complete GameObject compiler or SDK plugin. It supports all 88 attributes declared directly by the GameObject XSD and 23 non-polymorphic child types listed below. Unsupported children, fields and foreign namespaces remain explicitly rejected. SDK GameObject routing remains original.

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

## Current validation

Source `074d267b2835be89ade3944c1cbe6d2f94b3793a`, successful run [39928834132](https://github.com/wu162/NewSdk/actions/runs/39928834132). Per /Od and /O2: **34,052 fixtures, 102,156 three-buffer comparisons, 492 refusal checks and 124 invariance comparisons**. Both modes total 204,312 buffer comparisons. Win32, Xbox360, PlayStation3 and unknown 99 are retained. Evidence: `evidence/gameobject-root/validation.json` and both oracle/build/host logs. The run's `recovered-gameobject-component-x86` artifact contains the component DLL.

Full matrix retains all prior 88-root/KindOf/visual/collection cases and adds all seven new child types and their complete valid dependency groups. GameDependency tests all 457 ModelCondition names and 223 ObjectStatus names, signed/repeated flags, nullable masks and padding, typed-list growth/order, nested ObjectFilter, TypeId and refusal cases. AutoResolve covers references, all string/ID fields, two nested list families and independent token/layout assertions. Resource tests cover base-0 unsigned parsing, overflow, repeated items and nested token sentinels. Pointer StringLists include actual schema validation, direct original text behavior, separate original nonempty characterization and schema-invalid rejection.

Earlier coherent checkpoints: AutoResolve source `8650678284fd171f0dc076732bf12d0944e2d1ef`, run 39926787685, per mode 24,120 fixtures / 72,360 comparisons / 336 refusals / 32 invariance. StringList + resource lists source `93d16a98c24cde4ac9d2e00d26c6d76c96ff7015`, run 39927770965, per mode 24,412 / 73,236 / 420 / 120. These were complete capability groups, not individual fields. Original characterization remains under `evidence/gameobject`; it is not SDK integration or the current component matrix.

Primitive regression: [39928834050](https://github.com/wu162/NewSdk/actions/runs/39928834050), success, matching source. Real SDK runs this stage: **zero**. GameObject SDK routing remains original; no formal GameObject plugin or complete replacement DLL exists yet. Relocation, scalar/resource, shared enum-bitset and ObjectFilter implementation files were not changed.

## Next coherent stage (~35 minutes)

Continue complete remaining non-polymorphic groups: CrusherInfo with ExtraCrushLevels, and ProjectedBuildabilityInfo with its full mask/AllowedObjectFilter dependencies. Do not split each scalar or enum into its own full CI. SkirmishAIInformation requires its nullable overrides, AIContextualPower and base-selection substructures as a later complete group. Geometry, audio arrays and all polymorphic module groups remain unsupported; ReplaceModule/InheritableModule nonempty Token paths remain unresolved and refused.

Keep /Od, /O2, four-target three-buffer comparisons, independent layouts and explicit refusal coverage. Checkpoints go to work branches; integrate complete groups with ordinary fast-forward updates, preserving Actions evidence. Keep SDK batching: prepare schema-valid mymod inputs and formal routed/original-free replay only at a later GameObject integration milestone. Component success does not replace SDK validation.
