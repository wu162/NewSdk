# ForceFeedbackEvent component

Complete non-Joypad-named ForceFeedbackEvent original registered global asset. Component-only; no SDK route/Plugin change. The event is also referenced by FXList and common UI schemas, and this group does not restore any Joypad-named root.

Fixed Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

Exact original Plugin HandleType at line25517 and GetExtendedTypeInformation case474482543 with TypeHash3791548904. Original Module Marshal at50675 and Endian at50711, preserved in `evidence/xml-source/parts/part-0101.cs.txt`; tracker constructor109992 (`part-0219.cs.txt`) allocates20 bytes. Schema is `SDK/Schemas/xsd/AssetTypeForceFeedbackEvent.xsd`, global asset in CnC3Types.xsd1066.

| Offset | Complete field | Original semantics |
| ---: | --- | --- |
| 0 | Reserved word | zero |
| 4 | LeftMotorUse | float(atof) multiplied by0.01f; absent0 |
| 8 | RightMotorUse | same; no percentage clamp |
| 12 | Duration | TimeTag seconds; absent0 |
| 16 | PlayCount | float(atof), default1; present-empty0 |

All four scalar words are endian-swapped on every non-Win32 platform. There are no pointers, imports, lists, or omitted base fields. The original accepts fractional/negative/out-of-range percentages and fractional PlayCount; this is preserved in the component rather than imposing UI assumptions.

Files: `src/Native/Xmlforcefeedback.{h,cpp}`, `src/Bridge/XmlforcefeedbackCompiler.cpp`, `catalog/XmlforcefeedbackTypes.json`, `tests/XmlforcefeedbackOracle.cs`, `scripts/TestXmlforcefeedback.ps1`.

Complete /Od and /O2, platforms0/1/2/99 compare original ProcessInstance plus direct CompileXml across all three arrays and assert original DLL fingerprint. Fixtures cover every field's full numeric parser boundaries, signed zero, exponent/D exponent/overflow/trailing garbage/comma/percent/Time units, embedded NUL and non-English text, both motor cross-product, independent four-field bit/offset/default ABI, present-empty PlayCount, unclamped percentage range and empty side arrays. Unknown/qualified attributes, child/text/namespace/root and TypeId mismatch are explicit rejection boundaries. No SDK real compilation performed. Actual Windows acceptance is recorded by integration.
