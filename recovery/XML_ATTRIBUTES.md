# XmlCompiler attributes group

Baseline: ba7a3d6845be5e124d08320cb2c56fda552d9fbe.
Branch: recovery/xmlcompiler-attributes. Status: implementation checkpoint, Windows differential NOT yet run.

## Current component
Public Recovered::LogicCommandSetCompiler static methods:
- AssetBuffer^ CompileXml(XmlNode^, XmlNamespaceManager^, bool bigEndian)
- AssetBuffer^ ProcessInstance(InstanceDeclaration^, TargetPlatform)

The bridge checks TypeId 3959844197 and the asset namespace; only Cmd text children and metadata id/inheritFrom are accepted. Unknown attributes, foreign/unknown children, nested Cmd elements, wrong type, and non-whitespace root text are explicitly rejected. BAB preprocessing must resolve references; no resource resolver is invented.

LogicCommandSet is a 12-byte root, reserved word @0, list count @4, pointer @8. Cmd entries are four-byte asset references. Root/list allocation and reference imports use the existing recovered Tracker and ResourceMarshaling implementations. FinalizeTracker preserves all three arrays. There is no Tokenizer procedure table and imports are never cleared.

## Evidence
At fixed baseline:
- recovery/decompiled/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder.XmlCompiler/Plugin.cs, blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b: ProcessInstance case 3959844197; HandleType<SageBinaryData::LogicCommandSet> invokes Marshal<List<AssetReference<LogicCommand>>> on root+4, endian-corrects the count, and FinalizeTracker; FinalizeTracker calls MakeRelocatable with absolute offset 0 and copies InstanceData, RelocationData, ImportsData.
- Same Plugin.cs: AttributeModifier TypeId 3319822471; UpgradeTemplate TypeId 3786401627; both call their Marshal and EndianCorrect directly before FinalizeTracker, without Tokenizer.
- SageBinaryData/LogicCommandSet.cs: StructLayout Size=12.
- SDK/Schemas/xsd/AssetTypeLogicCommandSet.xsd: no own root attributes; unbounded Cmd LogicCommandRef children.
- Existing recovery/src/Native/ResourceMarshaling.cpp: MarshalReference distinguishes unresolved names from backslash decimal indices; no backslash leaves zero, resolved references create import bookmarks.
- Existing recovery/src/Native/Relocation.cpp: aligned zero initialization; 32-bit pointer/reference serialization and sentinel arrays.

## Validation entry
Run recovery/scripts/TestXmlattributes.ps1 in Windows x86 VS2022 developer shell.
It builds /Od and /O2 separately and launches fresh x86 oracle processes against SDK/tools/BinaryAssetBuilder.XmlCompiler.dll. Each mode covers platforms 0/1/2/99, empty/nonempty lists, unresolved and resolved references, duplicate indices, numeric boundaries, and list sizes around the Tracker sorting threshold. Each fixture compares all three arrays and independently checks root/list layout, endian words, relocation pointer/sentinel, and import membership/sentinel.
Logs: recovery/evidence/xml-attributes/Od-oracle.txt and O2-oracle.txt, with platform fixture/comparison/rejection/layout summaries.
Final structured result: recovery/evidence/xml-attributes/validation.json with succeeded/sourceCommit/runId; initialized false and finalized even on failure.

## Open boundaries
AttributeModifier (60 bytes) and UpgradeTemplate (140 bytes) are NOT implemented. Full -Module-.cs blob 2d2f471c8008b7c2e1f78e39752d0726da395cea repeatedly fails fetch_blob with Transport closed in this session; fetch_file incorrectly returns success with empty content for this large file. Plugin.cs was successfully cached through fetch_blob. Full Marshal/list/reference method evidence is still required before claiming complete LogicCommandSet recovery. Its implementation reuses established common list/reference semantics but awaits the original Windows oracle, including empty-list allocation and all import ordering.
No real SDK integration or substitute DLL routing has been performed. This checkpoint must not be counted as a validated recovered group.
