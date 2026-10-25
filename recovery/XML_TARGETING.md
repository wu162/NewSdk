# Targeting compare component recovery

Five original registered entry points; SDK integration remains pending. `TargetingBaseCompare` is a registered compiler entry but is not a global CnC3Types asset declaration. The four other roots are global assets. No main Plugin or SDK route is changed.

## Evidence and complete layouts

Fixed original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

Original Plugin `HandleType` bodies are in `decompiled/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder.XmlCompiler/Plugin.cs`, lines 30288, 30386, 30504, 30602 and 30700 respectively. Original tracker constructors are preserved in `evidence/xml-source/parts/part-0228.cs.txt`, at original Module lines 114087, 114178, 114269, 114360 and 114451. Each constructor allocates one zero-initialized root of the exact size below.

| Root | Root bytes | Complete payload |
| --- | ---: | --- |
| TargetingBaseCompare | 4 | Reserved zero word only |
| TargetingDistanceCompare | 8 | Reserved word; float Tolerance at +4, default 30.0 |
| TargetingCombatChainCompare | 4 | Reserved zero word only |
| TargetingInTurretArcCompare | 4 | Reserved zero word only |
| TargetingCompareList | 12 | Reserved word; Compare count at +4 and list pointer at +8 |

The three 4-byte roots are genuine original no-field handlers, not fallback implementations. They immediately finalize the allocated tracker. The exact schema is `SDK/Schemas/xsd/Modules/UnitAITargetChooser.xsd` lines 3–46, including the distance default `30.0` and `Compare` AssetReference elements. The catalog retains original numeric TypeId/TypeHash pairs, read from Plugin `GetExtendedTypeInformation` cases 3600209407, 2359666647, 89767986, 2007776008 and 3188107749.

`Compare` is a list of 4-byte strong references, not an inline list of concrete comparison objects. Original Module list marshaler at line 80471 allocates 4×count in one child block and visits each element text in document order. Original reference marshaler at line 116437 uses the first C-string backslash and decimal `atoi` suffix, registering a reference even for -1. Serialized values are index+1 (thus -1 outputs zero with an import bookmark). Import order is owner block then unsigned reference index, not list order. Empty lists allocate no child block. Empty `Compare` remains a zero list slot without an import. Only Tolerance and list count need explicit endian swapping; the tracker serializes pointers and reference values for every non-Win32 platform, including 99.

## Implementation and validation

- `src/Native/Xmltargeting.h/.cpp`, `src/Bridge/XmltargetingCompiler.cpp`: complete five-root component and exact named ProcessInstance/CompileXml routes.
- `catalog/XmltargetingTypes.json`: five original identifiers, component-only status.
- `tests/XmltargetingOracle.cs`, `scripts/TestXmltargeting.ps1`: original ProcessInstance and direct CompileXml three-array comparison, /Od and /O2, platforms 0/1/2/99. Fixed original DLL fingerprint is asserted before use.

The matrix covers defaults, all root metadata, numeric parser edge cases, signed overflow, exponent/comma/trailing text, embedded NUL, all reference forms and overflow boundaries, empty/mixed/reverse-index lists, 0/1/2/3/15/16/17/27/28/29/64/257/1024 lengths, CDATA/comments and non-English strings. Independent assertions check exact root/list sizes, default float bits and field offsets, stride, pointer targets, index+1, wraparound references, import ordering and sentinels. Unsupported root/attribute/namespace/child/text, invalid reference attributes, nested reference children, root-handle mismatch and null declarations are explicitly rejected. These structural guards intentionally restrict unsupported input; they do not claim the permissive original ignores are restored.

Status: implementation and matrix prepared; Windows component validation must be recorded by the integrating session. No SDK real compilation requested or performed by this group.
