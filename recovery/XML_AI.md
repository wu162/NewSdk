# XmlCompiler AI source recovery

Branch `recovery/xmlcompiler-ai`, fixed baseline `ba7a3d6845be5e124d08320cb2c56fda552d9fbe`. This checkpoint is **not Windows-validated**. It is a source implementation, with no runtime delegation to the original XmlCompiler.

Public C++/CLI API: `Recovered::XmlaiCompiler::CompileXml(XmlNode^,XmlNamespaceManager^,bool)` and `ProcessInstance(InstanceDeclaration^,TargetPlatform)`. CompileXml routes by the root LocalName; ProcessInstance additionally checks the original TypeId. Platform 0 is little endian; platforms 1/2/99 are big endian. All three AssetBuffer arrays are preserved. There is no Tokenizer procedure-table prefix.

## Evidence

Original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; original Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`. The source fragments and exact PE enum maps on `recovery/xmlcompiler-priority:recovery/evidence/xml-source/` are evidence only; this branch does not merge that moving branch.

Root Marshal/Endian/Tracker methods: AITargetingHeuristic starts at Module line 42242, AIMicroManagerData 54337, AIStateDefinition 56159, AIBudgetStateDefinition 56187, AIStrategicStateDefinition 56604, AIPersonalityDefinition 57965. The original metadata inventory method tokens/signatures and IL independently establish native offsets, defaults, allocation order and heuristic discriminators. XmlaiLayouts.inc records those recovered layouts declaratively. Enum names/values come from original PE enumMap JSON, not XSD ordinal assumptions. The XSD only resolves full spellings of truncated MSVC literal symbols.

Plugin HandleType performs Marshal, EndianCorrect, then FinalizeTracker. FinalizeTracker invokes MakeRelocatable with absolute instance offset zero and copies InstanceData, RelocationData and ImportsData. AI TypeIds: Personality 3604279694, Strategic 607123156, MicroManager 2423780215, Budget 2893598307, State 3239935619, Targeting 926814458. Root byte sizes are 512/64/176/20/12/196 respectively.

BuildingPlacementDistance uses the original EASTL pointer introsort: threshold 28, median-of-three, right partition first, heap fallback, insertion completion (Module 81526, 81607, 116451 onward). The comparer is float less-than promoted to double. Equal keys do not imply stable ordering; signed zero fixtures and lengths 27/28/29 exercise that distinction.

## Implemented scope and boundaries

All six types have their complete root attribute tables. AIMicroManagerData includes TargetBias, SpecificSpecialPowerDelay, nullable RetreatBaseCriteria and IgnoreTargets. AIStateDefinition includes all 29 known heuristic variants, nested weighted combinations, nullable/inline ObjectFilters, enum/bitset fields, optional scalars, strings and weak references. AIBudgetStateDefinition additionally includes Budget/AccountShare, marshaled before inherited state heuristics. AIStrategicStateDefinition includes its root attributes, nullable feedback hashes, TargetHeuristic strong-reference list and inherited heuristics; **Tactic is explicitly rejected**. AITargetingHeuristic includes BaseSelection and the sorted BuildingPlacementDistance list. AIPersonalityDefinition includes all root attributes, nullable DefaultTacticHomeBaseCriteria and inline CountAsStructureForBuildPurposes; **its other child lists and InTroubleSkirmishFeedbackControl are explicitly rejected**.

Unknown attributes, children, roots, wrong namespaces and unknown heuristic TypeIds are rejected. Heuristic nodes require BAB's numeric TypeId; the serializer does not pretend that an unprocessed element name supplies schema preprocessing. Singleton duplicates and mixed text are rejected. Sorted distance nonfinite values and nesting deeper than 96 are explicit additional rejection boundaries. Namespace manager must bind ea to the asset URI. Unsupported input is never silently routed to the original DLL.

Nullable scalar/String/flags allocation sizes follow the recovered ABI. Relo::Tracker manages allocation and pointer/reference relocation, while native scalar words are converted before MakeRelocatable. ObjectFilter uses the existing independent ObjectFilter/EnumBitset implementations; no shared file is modified. Strong references use resolved BAB suffix indices and retain ImportsData; weak asset identifiers are lowercase hashes, while StringHash feedback keys preserve case.

## Validation

Run `recovery/scripts/TestXmlai.ps1` in a Windows VS2022 **x86** developer shell. It builds /Od and /O2, starts fresh CLR4 hosts with legacy v2 activation, checks the original DLL fingerprint and compares original XmlCompiler ProcessInstance against recovered ProcessInstance and CompileXml on platforms 0/1/2/99. The oracle uses Framework v4 csc-compatible syntax.

The script writes `recovery/evidence/xml-ai/validation.json` with succeeded/sourceCommit/runId and per-mode per-platform fixture, three-array comparison, rejection and independent layout counts. Each mode keeps its build and oracle logs. Root ABI, reserved word, scalar offsets, optional pointers, string bytes, heuristic list pointers and discriminators have independent assertions. Tests include every root attribute, boundary parsing, all heuristic defaults, heuristic field mutations, imports, nested lists/filters, sorting thresholds/ties/signed-zero, prefix navigation and explicit rejection boundaries. Prefixed nodes use canonical original ProcessInstance vs direct CompileXml because InstanceDeclaration may include a prefix in the type hash.

No validation.json success is committed speculatively. Actual Windows /Od and /O2 results, and SDK integration, remain pending until the parent agent runs the unified workflow. Component differential success does not establish replacement DLL or SDK integration success.
