# Inline object-filter assets

Component-only recovery of three complete independent original Plugin entries. SDK integration and real SDK compilation remain pending.

| Entry | TypeId | TypeHash | Root bytes | Inline filter offset | Global schema asset |
| --- | ---: | ---: | ---: | ---: | --- |
| ObjectFilterAsset | 1151702845 | 1129453918 | 116 | 4 | yes |
| RegisteredObjectSet | 814796096 | 235028480 | 124 | 12 | yes |
| ObjectVisionRangeItem | 772930644 | 1028521956 | 120 | 8 | no |

All three have a reserved zero root word and a complete inline112-byte ObjectFilter; only its optional statuses and IncludeThing/ExcludeThing lists allocate blocks. An absent filter leaves all inline bytes zero; a present-empty filter applies Rule NONE (3). No synthetic pointer is created for the inline filter.

## Original evidence

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; fixed DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Module `xml-source/parts/part-0059.cs.txt` line29557: ObjectFilterAsset's child Filter is marshaled inline at+4. Plugin HandleType around4220 endians that inline ObjectFilter.
- `parts/part-0093.cs.txt` line46553 and46578: RegisteredObjectSet has SetId@4, Type@8 and child ObjectFilter inline@12; Marshal and EndianCorrect cover the complete root.
- `parts/part-0117.cs.txt` line58854: ObjectVisionRangeItem has Range@4 default100, atof→float then multiplied by0.01f, and child Filter inline@8. Plugin HandleType around30860 independently swaps Range and the inline ObjectFilter. It is an original registered entry despite being a nested UnitAITargetChooser schema type.
- `parts/part-0146.cs.txt` line73092/73103: absent enum attrs leave zero; present unknown SetId falls back26, present unknown Type falls back0. EnumLookup FindNVPair at59990 in part0119/0120 is exact case-sensitive C-string comparison. Exact53/2 entry PE tables: `xml-source/enums/RegisteredObjectSetId.json` and `RegisteredObjectSetType.json`.
- Tracker constructors at92065/101074/114542 and decompiled sequential structs prove116/124/120 byte roots. Schema cross-checks: AssetTypeObjectFilter.xsd, AssetTypeRegisteredObjectSet.xsd and Modules/UnitAITargetChooser.xsd.

The existing independent ObjectFilter helper supplies all Rule/Relationship/Alignment,284 KindOf words,223 ObjectStatus words, nullable status blocks and weak thing lists. The adapter retains original IValue C-string truncation, including attr/thing NUL boundaries, and ignores TypeId only as BAB metadata. Unknown/qualified attrs, foreign namespaces, structured text, unsupported children and duplicate filters are explicitly rejected.

## Verification

Files: `Xmlobjectsets.{h,cpp}`, `XmlobjectsetsCompiler.cpp`, `XmlobjectsetsTypes.json`, `XmlobjectsetsOracle.cs`, `TestXmlobjectsets.ps1`. The test script performs x86 /Od and /O2; platforms0/1/2/99; original ProcessInstance versus recovered ProcessInstance and direct CompileXml; all three arrays; independent root, inline-field, suballocation, weak hash, enum table and relocation ABI checks; complete KindOf/Status words on all three roots; enum case/sign/whitespace/NUL/fallback cases; float/percentage boundaries; list growth16/28/32/257/1024 and strict rejection.

Local native preflight passed g++ syntax checking with only the pointer-size assertion removed in a temporary copy. The real source retains x86 checks. Windows original-DLL differential result belongs to the parent integration run; no matrix success or SDK support is claimed before that evidence is available.
