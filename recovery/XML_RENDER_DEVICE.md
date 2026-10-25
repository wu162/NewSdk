# W3D device and scorch settings components

Complete W3DDeviceSettings and ScorchMarkSettings registered global assets. Component-only; SDK integration pending. No Joypad root, main Plugin or SDK route changes.

Original fixed Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

## Evidence

Plugin HandleType W3DDeviceSettings27191/ScorchMarkSettings31709; GetExtendedTypeInformation cases1469055344/2383722691, TypeHash2638871790/1770851834. Root tracker constructors111448/115270 allocate168/12 bytes (`evidence/xml-source/parts/part-0222.cs.txt`, `part-0230.cs.txt`). W3D Marshal/Endian52372/52477 (`part-0104.cs.txt`). Scorch row list/Endian77120/77168, inner Scorch enum list/Endian77033/77085 (`part-0154.cs.txt`). String8 list marshaller63091 (`part-0126.cs.txt`). Inline ObjectFilter Marshal/Endian26334/26371 reuse existing ObjectFilter.cpp without changing it. Original decoded enum maps are `enums/{ShroudStatusType,TerrainClassType,TerrainScorchType}.json`.

Full schemas: `SDK/Schemas/xsd/System/W3DDeviceSettings.xsd`, `System/ScorchMarkSettings.xsd`. All fields and nested dependencies are restored.

## Complete layouts

W3DDeviceSettings root168:

| Offset | Complete field / original default |
| ---: | --- |
| 0 | reserved0 |
| 4 | signed MaxTerrainTracks100 |
| 8 | float OpacityOfSimpleMergeDecals35% |
| 12 | float OccludedColorLuminanceScale0.5 |
| 16/20/24 | signed MaxRoadSegments4000 / MaxRoadVertex5000 / MaxRoadIndex10000 |
| 28 | DefaultShroudStatusFOGGED0; unknown present valueCLEAR1 |
| 32/36/40 | inline ShroudColor RGB float channels, absent0 |
| 44 | inline112-byte TreeFadeObjectFilter; absent zero, present empty RuleNONE3 |
| 156/160 | StandardPublicBone count / String8 list pointer |
| 164/165/166/167 | byte UseSimpleHordeDecals false / FogAlpha63 / ShroudAlpha0 / ClearAlpha255 |

Byte alphas cast original base0 unsigned parsing to8 bits, without clamping. Boolean and alpha bytes stay in place on both endian modes. Scalar words and RGB channels endian-swap, inline filter uses its original endian routine, list/string lengths swap and tracker handles all pointers. Filter dynamic allocations occur before bone String8 list and their character data even if XML order differs. Bone text and filter IValues follow first-NUL C strings. A present-empty bone still allocates a terminator.

ScorchMarkSettings root12: reserved0, outer count+4, pointer+8. Outer rows are12 bytes: TerrainClass+0, inner Scorch count+4, pointer+8. Inner list elements are4-byte TerrainScorch enum values. Missing TerrainClass stays0; present unknown mapsCliff3. Unknown/empty Scorch text mapsRANDOM(-1). All list lengths/scalar enum words swap for every non-Win32 platform; no imports occur in either root.

## Implementation and matrix

`src/Native/Xmlrenderdevice.{h,cpp}`, `XmlrenderdeviceEnums.inc`, `src/Bridge/XmlrenderdeviceCompiler.cpp`, `catalog/XmlrenderdeviceTypes.json`, `tests/XmlrenderdeviceOracle.cs`, `scripts/TestXmlrenderdevice.ps1`.

Full original ProcessInstance plus direct CompileXml three-array matrix, /Od and /O2, platforms0/1/2/99, original DLL fingerprint. Covers all scalar fields, signed/unsigned/float/percentage/bool/byte boundaries, all RGB channels, all three exact enum maps plus defaults/fallbacks/case/NUL, full284 KindOf and223 ObjectStatus dictionaries/signs/NUL, all ObjectFilter fields/thing lists, multilingual/empty/NUL/long bone strings, nested scorch lists and counts0–1024. Independent defaults, scalar and byte offsets,168/12 roots,264-byte complete device,60-byte nested scorch ABI, inline filter/status/thing offsets, string lengths/targets/ASCII terminators, pointer sorting and empty imports are asserted. Unknown/qualified attrs, namespace, duplicate singleton, nested attributes/children/text and TypeId mismatch reject explicitly.

Implementation and matrix prepared; integrating session records actual Windows acceptance. SDK real compilation0.
