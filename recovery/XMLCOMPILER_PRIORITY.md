# XmlCompiler priority recovery integration

Integration branch: `recovery/xmlcompiler-priority`.
Fixed base: `ba7a3d6845be5e124d08320cb2c56fda552d9fbe` from `recovery/compiler-source`.
Started 2026-10-24, Beijing time. This session has a one-hour implementation and validation budget.

## Scope and branch ownership

| Worker branch | Priority types |
| --- | --- |
| recovery/xmlcompiler-attributes | AttributeModifier, UpgradeTemplate, LogicCommandSet |
| recovery/xmlcompiler-effects | FXList, FXParticleSystemTemplate |
| recovery/xmlcompiler-powers | SpecialPowerTemplate, ObjectCreationList |
| recovery/xmlcompiler-locomotor | LocomotorTemplate |
| recovery/xmlcompiler-ai | AIPersonalityDefinition, AIStrategicStateDefinition, AIMicroManagerData, AIBudgetStateDefinition, AIStateDefinition, AITargetingHeuristic |

GameObject and all Joypad types are excluded. Existing DLLs and SDK routes remain the original baseline. Worker branches own distinct new source/test/script/type-note files; the integration owner adds the common workflow and merges worker histories. No force updates are permitted.

## Validation contract

Each group supplies `recovery/scripts/TestXml<group>.ps1`, with results in `recovery/evidence/xml-<group>`. Windows x86 VS2022 builds must compare original XmlCompiler ProcessInstance against recovered code in both /Od and /O2, for platform values 0, 1, 2 and 99. The three AssetBuffer arrays must match byte-for-byte. Independent layouts, defaults, namespaces, repeated/list inputs, reference allocation, enum boundaries and explicit unsupported-input rejection must be covered where applicable.

Final `validation.json` records `succeeded`, `sourceCommit` and `runId`. Implementation checkpoints and incomplete groups do not count as verified support. Component differential validation does not constitute complete SDK integration or replacement of the original XmlCompiler DLL.

Original XmlCompiler `Plugin.cs` blob: `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; original `-Module-.cs` blob: `2d2f471c8008b7c2e1f78e39752d0726da395cea`. The inspected HandleType/FinalizeTracker path marshals one tracker, applies endian correction, and exports instance, relocation and imports with absolute offset zero. It does not prepend the Tokenizer procedure table.

## Current checkpoint

All five worker branches have been created from the fixed base. Implementation is in progress; no priority type is declared validated at this checkpoint. The integration workflow has been drafted and its YAML parsed locally; it will be committed with the first complete validation batch. See each worker type note for exact implemented and rejected input boundaries.
