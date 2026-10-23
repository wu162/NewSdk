# GameObject recovery entry

Status: recovered **root component**, not a complete GameObject compiler or SDK plugin. It supports 87 of the 88 attributes declared directly by the GameObject XSD: scalar values, six strings, six enums, seven typed IDs, three resource references, Browser string list and both 284-bit KindOf masks. All child elements and the `EvaEventSecondDamageFarFromFirstTimeoutMS` override are explicitly rejected. Its omitted duration default is restored. SDK GameObject routing remains original.

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

The duration's omitted default `31000` becomes **155**, not 31000: the original scale bytes `0A D7 A3 3B` represent 0.004999999888241291f. Its override remains rejected pending direct characterization of rounding and out-of-range float-to-uint behavior.

## Current validation

Source `7b958b9df5c31f880465d6fd2df8eec6cb73fa28`, successful run [39920441194](https://github.com/wu162/NewSdk/actions/runs/39920441194). Each /Od and /O2: **12,452 fixtures, 37,356 three-buffer comparisons, 36 refusal checks**; both modes total 74,712 buffer comparisons. Evidence: `evidence/gameobject-root/validation.json` and `oracle-{Od,O2}.txt`. Matching primitive regression: [39920441265](https://github.com/wu162/NewSdk/actions/runs/39920441265).

For each /Od and /O2, tests cover Win32, Xbox360, PlayStation3 and unknown target 99, comparing every byte of InstanceData, RelocationData and ImportsData. The matrix includes numeric parsing boundaries, string padding/encoding, all six enums and unknown/case/whitespace fallbacks, resource hashes and decimal overflow/wrapping, Browser allocation/token growth and mixed strings/resources/masks. Both KindOf fields test every original name with no sign, plus and minus, plus ALL, XOR, unknown and mixed-list cases.

Independent assertions cover the original 16 scalar offsets, default enum/float/duration values, all eight empty-list count tables, root sentinel, narrow ushort/byte/bool layout, string relocation/padding, tokenized reference indices, all KindOf bit positions and ALL padding, Browser entry/token addresses and allocation order. Unsupported fields/children/namespaces continue to fail closed. No source changes were made to the shared relocation, parsing, resource or enum-bitset implementations.

Original characterization remains separate: 196 fixtures, 588 repeated three-buffer comparisons; source `46b0ec5020b3911ec0b0403a3d60f63426590673`, run [39897995324](https://github.com/wu162/NewSdk/actions/runs/39897995324). `evidence/gameobject/source-layout.json` joins the original source inventory with all four targets' observed default buffers. Root component evidence is under **`evidence/gameobject-root`**; never confuse it with original-only baseline success or SDK integration.

## Next coherent stage (~35 minutes)

Use one coherent implementation batch for the remaining duration attribute and the connected child group below. Investigate uncertain behavior with focused original probes, implement the whole planned group and its tests, then run the full affected matrix together. Save unfinished checkpoints on `recovery/work/<batch>`; integrate the prepared batch into `recovery/compiler-source` for the existing complete CI gates. Consolidate diagnosed fixes before rerunning. See EFFICIENCY.md, “大批实现与统一验证”. Do not repeat separate full-CI batches for scalar, enum, resource, list and bitset representations. Current support and prior validation anchors are unchanged.

1. Verify the current root evidence sourceCommit/runId before changing code; reuse the exact successful result while relevant inputs stay unchanged.
2. Characterize the remaining duration override at root+388: original helper 55021, scale 9619. Cover fractional frame boundaries, exponents, signs, huge values, invalid inputs and uint conversion. Preserve both float-rounding points; do not guess from the 155 default.
3. Restore a connected group of child inputs after examining original helpers: Buildable@428, ThingClass@432 and DisplayColor@452 are useful entry points. DisplayColor is a Color4 node passed through the ColorTag helper, not an arbitrary unsigned-text scalar. Follow with the two optional string pointers at 396/400 if time permits. Preserve omitted defaults, duplicate/namespace rejection and exact allocation/token contracts.
4. Keep full /Od and /O2 four-target three-buffer differences, independent layouts and unsupported-input refusal gates. Avoid modifying shared modules when existing verified semantics suffice; assess affected users if shared behavior changes.
5. Batch GameObject SDK integration later, after a coherent child/module surface and schema-valid mymod fixtures are ready. Keep the original SDK route until the actual routed output comparison and original-free replay pass. No real SDK run was triggered in this stage.
