# CommandSet component

Complete original registered CommandSet entry, TypeId2584759047, TypeHash4174095999, nonglobal schema type. CnC3Types.xsd includes AssetTypeCommandSet.xsd but has no global CommandSet asset element; the independent original Plugin entry is still real. Component only, SDK pending; no main Plugin route or SDK compilation changed.

Root24: reserved word@0, Name String8@4, InitialVisible signed32@12 default32, CommandButton list count@16/pointer@20. Each CommandButton16 has Name String8@0, unsigned Index@8, nullable ClientRandomVariable pointer@12. ClientRandomVariable12 has DistributionType enum@0, Low float@4, High float@8. Missing InitialDelay leaves null; presentempty allocates12 with UNIFORM1 and zero floats. Presentunknown Type falls back CONSTANT0.

## Evidence

Original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

Module part0043 lines21725/21770/21797: complete CommandButton Marshal/EndianCorrect and CommandSet Marshal/EndianCorrect. Root Name allocates before full16byte CommandButton array; each button then allocates its Name before optional12byte InitialDelay. This allocation order is retained even with alternative attribute order.

Part0038 lines19162/19184/19231 proves complete RandomVariable/ClientRandomVariable Marshal and EndianCorrect. Part0120 line60160 proves6entry DistributionType table with fallback0; exact PE evidence is `xml-source/enums/DistributionType.json`. The enum is case-sensitive C-string lookup. Declarations and root tracker constructor prove24/16/12 sizes. Schema cross-check: AssetTypeCommandSet.xsd. The component marshal does not enforce XSD max32; test coverage includes33/257/1024 buttons, preserving actual original behavior.

Strings truncate at NUL and allocate presentempty padded bytes. Signed/unsigned parsing uses existing proven CRT adapters. Unknown/qualified attrs, unknown or foreign children, structured text and duplicate InitialDelay explicitly reject. Name is a string, never an asset import reference.

## Validation entry points

`Xmlcommandset.{h,cpp}`, `XmlcommandsetCompiler.cpp`, `XmlcommandsetTypes.json`, `XmlcommandsetOracle.cs`, `TestXmlcommandset.ps1`.

Runner retains x86 /Od and /O2, platforms0/1/2/99, original ProcessInstance versus restored ProcessInstance/direct CompileXml, exact three-buffer comparison. Complete Oracle covers every field; absent/present defaults; all six enum words and case/whitespace/sign/NUL/unknown; signed/unsigned overflow, numeric float and string boundaries; names up to65535; button list growth across16/28/32 through1024; all structured rejection boundaries; independent fixed ABI checks for24/16/12 layouts, Name/Index/Delay offsets, nullable default1 versus fallback0, strings, source allocation ordering and ownerblock relocation arrays.

Local native g++ syntax check passed after removing only the x86 pointer-size assertion from a temporary copy. Production source retains x86 enforcement. Windows original-DLL matrix evidence belongs to parent integration; no unrun component or SDK acceptance is claimed here.

## First matrix ABI correction

Run40140206386 exposed an independent default-zero assertion that excluded byte12 only. On big-endian platforms, InitialVisible32 is stored at byte15, so that zero assertion incorrectly covered part of the nonzero word. The assertion now excludes the entire word12..15 while retaining an independent Word(offset12,platformEndian)==32 assertion for all four bytes and continuing to assert every other byte is zero. Original/recovered arrays had already compared equal; no native code or sample/differential coverage was removed.
