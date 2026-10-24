# XmlCompiler attributes group

Fixed baseline ba7a3d6845be5e124d08320cb2c56fda552d9fbe; branch recovery/xmlcompiler-attributes.
Latest acceptance (2026-10-24): source `71cbdd7bd27478d0bcb9dc3dbd2f22deac888dd7`, component run [40095358086](https://github.com/wu162/NewSdk/actions/runs/40095358086), /Od and /O2, platforms 0/1/2/99. 52-entry/51-schema-type Reduced and FullMymod SDK acceptance is recorded in [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md), run [40095358016](https://github.com/wu162/NewSdk/actions/runs/40095358016). Full replacement of all XmlCompiler types remains outside this acceptance.
LogicCommandSet first checkpoint dd7a532 had also passed at source3b075e8/run40079501172; final acceptance uses the expanded matrix below.

## Interfaces
Recovered::LogicCommandSetCompiler, Recovered::AttributeModifierCompiler and Recovered::UpgradeTemplateCompiler each expose:
- AssetBuffer^ CompileXml(XmlNode^, XmlNamespaceManager^, bool bigEndian)
- AssetBuffer^ ProcessInstance(InstanceDeclaration^, TargetPlatform)

TypeIds are 3959844197, 3319822471, 3786401627 respectively. Endian condition is platform != Win32, including unknown platform 99.
XmlCompiler directly finalizes its data tracker at absolute offset zero. All three AssetBuffer arrays are preserved; there is no Tokenizer header.

## Implemented contracts
LogicCommandSet: 12-byte root, reserved @0, Cmd count @4/pointer @8; 4-byte reference list with imports.
AttributeModifier: 60-byte root. Category @4; Duration seconds @8 (default 0s); DisabledType bitset @12; StartFX/EndFX references @16/@20; nullable ModelConditionsSet/Clear 60-byte bitsets @24/@28; nullable ObjectStatusToSet 28-byte bitset @32; unsigned StackingLimit @36 (default 1); ArmorSetType @40; nullable Shader reference allocation @44; Modifier count @48/pointer @52, 8-byte items (AttributeType @0, float Value*0.01f @4); bool flags @56/@57 default false.
UpgradeTemplate: 140-byte root. Strings @4/12/20/28; UpgradeType @36; BuildTime seconds @40; signed BuildCost @44; 13 strong references @48..96; GameObject typed ID hash @100; GroupName string @104; unsigned GroupOrder @112; heuristic @116; Options single enum @120; IconImage string @124; nullable GameDependency @132; bool WaypointQueueable @136.
GameDependency: 40 bytes. Three nullable model/model/status bitsets @0/4/8; RequiredObject/ForbiddenUpgrade/NeededUpgrade typed-ID hash lists @12/20/28; nullable complete ObjectFilter @36.
ObjectFilter: 112 bytes. id hash @0; Rule enum @4 (default NONE); Relationship bitset @8; Alignment @12 (default NONE); inline KindOf Include/Exclude masks @16/@52 (9 words each); nullable ObjectStatus masks @88/@92; typed-ID IncludeThing/ExcludeThing lists @96/@104.

Absent nullable attributes retain null; present empty attributes allocate zeroed masks, references or strings. Lists preserve XML order and use Tracker relocations. Strong references require BAB's preprocessed backslash decimal form; unresolved names retain zero and no import. Typed IDs hash lowercase text rather than creating imports. Scalar/time parsing reuses established legacy scalar semantics; time suffix lookup tries lowercase ms before lowercase s. Modifier percentage uses float multiplication, matching its own original IL.

## Original evidence
Original source blob 2d2f471c8008b7c2e1f78e39752d0726da395cea; original DLL SHA256 is recorded by the coordinator's xml-source manifest.
Coordinator evidence at ref recovery/xmlcompiler-priority, recovery/evidence/xml-source (read-only, not merged into this branch):
- roots/AttributeModifier.cs.txt: Marshal lines21612-21654, Endian and 60-byte root constructor.
- roots/UpgradeTemplate.cs.txt: Marshal lines31002-31112, Endian and 140-byte root constructor.
- roots/LogicCommandSet.cs.txt: 12-byte root constructor. Plugin.cs HandleType directly marshals Cmd list.
- parts/part-0043.cs.txt: AttributeModifierListType item Marshal line21597.
- parts/part-0052.cs.txt: ObjectFilter Marshal line26334 and Endian line26371.
- parts/part-0061.cs.txt: GameDependencyType Marshal line30525.
- parts/part-0165.cs.txt: nullable Shader reference helper line82631.
- enums/*.json: exact original PE name/value tables, copied as readable XmlattributesEnums.h constants. Includes AttributeType, AttributeModifierCategoryType, ArmorSetType, DisabledType, UpgradeType, UpgradeOptions, AIUpgradeHeuristicType, ObjectFilterRule/Alignment/Relationship, KindOfType, ModelConditionFlagType, ObjectStatusType.
- method-index.txt contains exact original literal bytes; names longer than MSVC symbol display were decoded from data(hex), including LocalPlayerProductionStartedEvaEvent, LocalPlayerBuildCancelledEvaEvent and UseObjectTemplateForCostDiscount.
Plugin.cs blob00bb9e5df72d07a21204f85f9a9a2f1d9557f00b independently verifies dispatch and FinalizeTracker copies all three arrays.
Original IL in fixed-baseline recovery/evidence/BinaryAssetBuilder.XmlCompiler/methods.json independently verifies tokens 0x06000070/71 (Attribute), 0x0600011D/1E (Upgrade), 0x060005A6 (Cmd list), 0x06000443 (reference), 0x06000119/1A (dependency), 0x060000C6/C7 (filter), enum fallback constants and pointer sizes.

## Validation
Run recovery/scripts/TestXmlattributes.ps1 in Windows x86 VS2022 developer shell. The script builds /Od and /O2 separately, compiles an old-syntax x86 .NET Framework C# host, configures CLR4 legacy activation, and launches fresh processes against original XmlCompiler.dll.
Every mode tests platforms 0/1/2/99, compares all three arrays for every fixture, and checks independent root offsets, endian words, string/pointer allocations, list counts, nullable masks, reference imports, all 13 exact PE enum maps and fallback behavior. It includes every model/status/KindOf entry; full root and nested combinations; empty vs absent; integer/time boundaries; unknown input rejection and canonical-original vs prefixed-direct navigation checks. Prefixed declarations are not passed as canonical original TypeIds.
Output recovery/evidence/xml-attributes/validation.json contains succeeded/sourceCommit/runId, mode exit codes and four platform summaries per mode. Individual logs are Od-oracle.txt/O2-oracle.txt and mode build/host logs. Failure initializes/finalizes succeeded=false.
Final evidence was inspected in recovery/xmlcompiler-priority at recovery/evidence/xml-attributes/validation.json, Od-oracle.txt, O2-oracle.txt. validation.json succeeded=true; sourceCommit=977313a17647051df333e223f7a6c45f732bdc63; runId=40080474059. Both modes have exitCode=0 and four platform summaries (0/1/2/99). Actual attributes job120832813604 and whole run completed/success; collector job succeeded.

| Mode | Fixtures | Three-array byte comparisons | Rejections | Independent layouts |
| --- | ---: | ---: | ---: | ---: |
| /Od | 4716 | 14148 | 116 | 4560 |
| /O2 | 4716 | 14148 | 116 | 4560 |

Each platform in each mode:1179 fixtures,3537 array comparisons,29 rejections,1140 independent layouts. Across both modes:9432 fixtures,28296 array comparisons,232 rejections,9120 layouts.
Original DLL SHA256=d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6, asserted by the script and preserved in validation.json.
52-entry/51-schema-type formal plugin routing and the current full shipped mymod input passed SDK parity; see XML_PRIORITY_SDK.md. All-type replacement DLL acceptance remains outside this scope.

## Explicit input boundary
Only asset-namespace roots with matching ea namespace manager and matching ProcessInstance TypeId are routed. Unknown root/nested attributes, unknown/foreign children, nested text-list elements, duplicate singleton GameDependency/ObjectFilter, and non-whitespace structure text are rejected instead of silently dropped. Root metadata id/inheritFrom is accepted as already handled by BAB. Arbitrary schema/preprocessing/inheritance resolution is outside this component. No original DLL is called by recovered compiler implementation; the original is loaded only by the differential oracle.

## Scope and efficiency
Only group-exclusive files changed; no shared runtime, GameObject, Joypad, workflow or RESUME modifications. Big-file results were retained in JS store; methods/IL and source parts were reused rather than repeatedly reading the unavailable full module. No quantitative quota saving is claimed.
