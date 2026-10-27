# Health bar component recovery

Complete HealthBarTemplate and HealthBarSettings original compiler entries, with the full ObjectMapping/ObjectFilter dependency. Both are global schema assets; component-only, SDK integration pending.

## Original evidence

Fixed Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Original Plugin HandleType methods at lines 25304 and 25413 in `decompiled/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder.XmlCompiler/Plugin.cs`; GetExtendedTypeInformation cases 1507490649/3919360587 give TypeHash 2857307522/4130115945.
- Original Module tracker constructors 109810/109901 are preserved in `evidence/xml-source/parts/part-0219.cs.txt`; exact root allocations are 8 and 16 bytes.
- Module Marshal HealthBarTemplate/HealthBarObjectMapping at 50637/50651 (`parts/part-0101.cs.txt`), mapping list and endian routines 76312/76354 (`parts/part-0152.cs.txt`). Strong HealthBar reference routine at 76298 follows first-backslash/atoi/AddReference contract.
- Full ObjectFilter original Marshal/Endian routines at 26334/26371 (`parts/part-0052.cs.txt`), reused through the already recovered `ObjectFilter.cpp` without changing shared semantics. This adapter truncates string values at the first NUL to honor original IValue C strings, including enum attributes.
- Full schema `SDK/Schemas/xsd/AssetTypeHealthBarTemplate.xsd`; both global assets appear in CnC3Types.xsd lines 1063–1064.

## Complete binary contract

| Root | Bytes | Fields |
| --- | ---: | --- |
| HealthBarTemplate | 8 | Reserved zero word; uint32 Length at +4, absent remains zero, original strtoul base 0 |
| HealthBarSettings | 16 | Reserved word; DefaultHealthBar strong reference +4; ObjectMapping count +8, pointer +12 |

Each mapping is exactly 116 bytes: HealthBar strong reference at +0, **inline** 112-byte ObjectFilter at +4. There is no nullable filter pointer. An absent filter remains zero; a present empty filter applies Rule=NONE(3), empty Relationship and Alignment=NONE(0). Mapping arrays allocate as one block, then each filter's StatusBitFlags(28), StatusBitFlagsExclude(28), IncludeThing(4×count), ExcludeThing(4×count), in that order. IncludeThing/ExcludeThing are case-folded TypedAssetId hashes, not resource imports. Only HealthBar fields are strong index+1 references.

ObjectFilter layout relative to its start: id/rule/relationship/alignment at 0/4/8/12; nine include words at 16; nine exclude words at 52; optional status pointers at 88/92; include count/pointer at 96/100; exclude count/pointer at 104/108. Root length, mapping count, inline filter words, optional statuses and thing hashes receive original endian conversion; tracker handles pointers/imports. All non-Win32 platform values use big endian.

## Files and tests

`src/Native/Xmlhealthbars.{h,cpp}`, `src/Bridge/XmlhealthbarsCompiler.cpp`, `catalog/XmlhealthbarsTypes.json`, `tests/XmlhealthbarsOracle.cs`, `scripts/TestXmlhealthbars.ps1`.

Complete /Od and /O2 matrix on 0/1/2/99 compares original ProcessInstance plus direct CompileXml across all three arrays. Fixed original DLL fingerprint is asserted. Independent ABI checks assert root sizes, unsigned Length offset/bits, 116-byte list stride, inline absent/present defaults, 200-byte full allocation, KindOf bit locations, status bits/targets, thing counts/targets, owner-block import ordering and pointer sentinels. Boundary matrix includes all 284 KindOf and 223 ObjectStatus words, positive/negative tokens and NUL truncation, all Rule/Alignment/Relationship enums and unknown fallbacks, unsigned overflow/base/autodetection/trailing text, all strong-reference edge forms, thing list growth, and 1024 mappings. Unsupported unknown/qualified attributes, namespace mismatch, nested payload, duplicate filter, unknown roots and mismatched TypeId fail explicitly.

Status: implementation/tests prepared; integrating session records actual Windows validation. No SDK real compilation, SDK route or main Plugin changes are part of this group.
