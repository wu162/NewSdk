# Mouse component

Complete Mouse registered global root, component-only and SDK-pending. No UIConfigList or Joypad root is represented by this entry.

Fixed Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`. Original Plugin HandleType9524, GetExtendedTypeInformation case2525492603 with TypeHash1246977355. Original Module Marshal/Endian41477/41513 (`evidence/xml-source/parts/part-0082.cs.txt`→`part-0083.cs.txt`), tracker constructor96615 (`part-0193.cs.txt`) allocates20 bytes. Complete schema `SDK/Schemas/xsd/AssetTypeMouse.xsd`.

| Offset | Complete field | Original default/type |
| ---: | --- | --- |
| 0 | reserved word | zero |
| 4 | DragTolerance | signed base0 integer48 |
| 8 | DragThreshold | TimeTag float seconds from100ms |
| 12 | InGameUIMaxDoubleClickDistance | unsigned base0 integer20 |
| 16 | AptHitRegionMaxDoubleClickDistance | unsigned base0 integer10 |

All four scalar words swap on every non-Win32 platform including99. No pointers, imports or omitted child fields. Explicit empty attributes override defaults to0.

Files: `src/Native/Xmlmouse.{h,cpp}`, `src/Bridge/XmlmouseCompiler.cpp`, `catalog/XmlmouseTypes.json`, `tests/XmlmouseOracle.cs`, `scripts/TestXmlmouse.ps1`.

Full /Od and /O2 four-platform original ProcessInstance plus direct CompileXml compares all three arrays and fixed original DLL fingerprint. Every field covers numeric/overflow/negative/base/exponent/comma/trailing/empty/NUL/multilingual boundaries, Time units/order and legacy D exponent. Independent root20/default bit values use `100f*0.001f` for the original cast-then-float-multiply contract, all four nonzero offsets and signed/unsigned distinctions, present-empty resets and empty relocation/import arrays. Unsupported root/namespace/attribute/qualified metadata/children/text and TypeId mismatch reject explicitly.

Status: complete implementation and matrix prepared; actual Windows results to be recorded by integration. SDK real compilation0.
