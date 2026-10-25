# W3D XML component assets

Four complete original registered XmlCompiler entry points, component-only and SDK pending. W3DAnimationRuntime is a nonglobal schema runtime type with a real independent Plugin entry; the other three are global CnC3Types assets. No W3D custom compiler, renderer, GameObject source or SDK route is changed.

| Entry | TypeId | TypeHash | Root bytes | Complete scope |
| --- | ---: | ---: | ---: | --- |
| W3DHierarchy | 1641540160 | 3985956449 | 12 | Pivot100 list@4 |
| W3DContainer | 4042295058 | 1637720895 | 16 | Hierarchy strongref@4, SubObject16 list@8 |
| W3DAnimationRuntime | 3534264507 | 3595590881 | 24 | Hierarchy strongref@4, NumFrames unsigned@8, FrameRate unsigned@12, Channels pointer list@16 |
| W3DCollisionBox | 3810008068 | 1848047257 | 32 | Center Vector3@4, Extent Vector3@16, JoypadPickingOnly byte@28 |

Pivot100: typed lowercase Name hash@0, signed Parent@4, uppercaseXYZ Translation12@8, XYZW Quaternion16@20, FixupMatrix64@36. Matrix field Mrowcol is column-major at4*(4*col+row); absent fields remain zero, no synthetic identity defaults.

SubObject16: BoneIndex unsigned@0, SubObjectID String8@4 with getter defaultempty (therefore even absent SubObjectID allocates padded4bytes), RenderObject strongref@12. The original single-polymorphic helper creates a reference only when the wrapper has exactly one child; zero or multiple children yield no reference. The local tag name is not used by that helper.

## Complete animation channel dispatch

All channel child names are accepted as the original pointer-list enumerates all children. Numeric TypeId2012836613 selects Delta44,1624821609 selects Timecoded36, every other/missing TypeId selects the explicit original BaseRuntime20 method, canonicalizing tag0 and preserving all base fields. This is an independently recovered original base branch, not an unknown root fallback.

BaseRuntime20: tag@0, AnimationChannelType enum@4, unsigned NumFrames@8, VectorLen@12, ushort Pivot@16, zero padding@18. Presentunknown Type falls backOrientation3. Delta44 adds Scale float@20, Init float list count@24/pointer@28, NumBits unsigned@32, CompressedValues byte list count@36/pointer@40. Init is a whitespace-tokenized C-string attribute and allocates before compressed bytes. Timecoded36 adds FrameAndBinaryFlag ushort list@20 and Values float list@28; its frame list allocates first regardless of XML order. Byte/ushort values truncate CRT unsigned results; shorts swap two bytes per element, bytes and padding do not swap. All lists are4byte allocation aligned.

## Original evidence

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Part0040 lines20340/20421: uppercaseXYZ Vector3 and XYZW Quaternion Marshal. Part0041 lines20670/20699 BaseRuntime Marshal/Endian,20855 SubObject,20891/20940 Delta Marshal/Endian,20973 Timecoded continuing part0042.
- Part0042 lines21022 CollisionBox,21062 channel pointer dispatch,21093 root AnimationRuntime,21136/21161 channel/root endian,21184 FixupMatrix,21285 Pivot and21353 Matrix endian; Pivot endian continues in part0043 at21501. All matrix16 and channel primitive/list fields were read from these complete original methods.
- Part0122 lines61150 AnimationChannelType unknownfallback3;61198 single-polymorphic exact-one-child reference;61299 Delta44 constructor;61307 explicit BaseRuntime20 fallback constructor;61329 list endian;61421/61463 SubObject16 lists. Timecoded allocates36 directly in root pointer dispatcher.
- Part0164 lines82300/82340 byte/ushort list Marshal;82380 full channel pointer-list;82420 Pivot typed Name lowercases C-string then FastHash. Complete five-word PE table `xml-source/enums/AnimationChannelType.json` is retained.
- Original Plugin Container HandleType around2049 directly routes Hierarchy and SubObject list; Hierarchy around2150 routes Pivot100 list. Root tracker constructors90154/90245/90336/90427 and decompiled structures prove all root/helper sizes.
- Schema cross-check: AssetTypeW3D.xsd. Runtime registered entry is not a global schema declaration; comments are not counted as global assets. JoypadPickingOnly is a complete field of the non-Joypad-named CollisionBox root, not recovery of an excluded Joypad root.

## Verification

`Xmlw3d.{h,cpp}`, public routes `Xmlw3dCompiler.cpp`, catalog `Xmlw3dTypes.json`, oracle `Xmlw3dOracle.cs`, runner `TestXmlw3d.ps1`.

Full x86 /Od and /O2, platforms0/1/2/99, original ProcessInstance versus restored ProcessInstance/direct CompileXml, all three exact arrays. Oracle covers all root/nested fields, all16 Matrix attrs, all vector/quaternion axes, complete PE enum/case/sign/NUL/fallback, CRT signed/unsigned/ushort/byte/float boundaries, string versus weak hash semantics, whitespace Init, typed Delta/Timecoded and original Base dispatch, single/multiple RenderObject behavior, complete pointer/subobject/pivot and numeric lists to1024, strict unknown/qualified/foreign/text/duplicate refusals and fixed independent ABI for12/16/24/32 roots,100/64/44/36/20/16 helpers, scalar/byte/short padding, source allocation ordering and complete ownerblock relocation/import arrays.

Local g++ syntax preflight passes with only the pointer-size assertion removed from a temporary copy. Production source retains x86 enforcement. Parent Windows integration supplies final original-DLL evidence; no unrun matrix or SDK support is claimed here.

## First matrix host correction

Run40142137629 reached the polymorphic TypeId NUL boundary but failed in the test host's `LoadXml`, before testing that sample, because ChannelSet interpolated the raw TypeId into XML source. ChannelSet now parses a fixed safe Channels/Node skeleton and assigns TypeId via DOM SetAttribute; every NUL sample is preserved byte-for-byte in the attribute value. Other boundary values already use DOM attributes or numeric Value elements. No Native source, sample, comparison, refusal or ABI assertion was removed.
