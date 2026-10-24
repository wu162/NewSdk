# UIConfigList: complete dependency investigation, not implemented

This is an evidence handoff only. UIConfigList is **not** added to any component catalog, bridge, native implementation, success count or SDK route. Mouse is implemented independently in XML_MOUSE.md.

Fixed Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

## Exact entry and common contracts

Original Plugin HandleType9121, GetExtendedTypeInformation case3072160547, TypeHash3852040349. Global asset schema AssetTypeUIConfigList.xsd206, CnC3Types.xsd871. Module tracker constructor96251 allocates12 bytes.

Root12: reserved word0; UIDynamicConfig count+4, pointer+8. Original outer list69960/Endian70002 allocates one16-byte UIDynamicConfig array.

UIDynamicConfig Marshal40726: Name raw **case-sensitive** FastHashInternal.StringHash at+0, Type enum+4, ComponentList count+8/pointer+12. Name length is first-NUL C-string length; do not use lowercase HashAssetId. Type direct enum helper69924 maps INGAME0/SHELL1/BOOTUP2/LOADSCREEN3; present unknown fallbackBOOTUP2, absent remains0. ComponentList singleton is read by name, then every direct element child becomes one UIBaseComponent pointer slot. An absent ComponentList allocates nothing. Exact enumeration map is retained in evidence/xml-source/enums/UIConfigType.json.

Component pointer list85207 allocates4 bytes per slot; dispatch reads numeric BAB TypeId with base0 strtoul, not the XML tag name. Original dispatch40272 and endian dispatch40502 contain **70 concrete branches**, not a small set of currently used shell screens. The complete exact TypeId/size/helper line/marshal line inventory is `evidence/xml-uiconfiglist/contract-index.json`; every helper's written runtimeTypeId has been checked against its dispatch case. Known8-byte branches that call only UIBaseComponent are genuine original base-only types, not empty fallbacks.

UIBaseComponent Marshal33312: runtimeTypeId+0, signed Priority+4. The polymorphic helper fills the runtimeTypeId only after marshaling and popping its block. Original unknown helper69143 separately allocates8 bytes, marshals base Priority and sets runtimeTypeId0; this exact original fallback must not be confused with recovering an unknown concrete component.

## Why the full root is deferred

The full branch set includes approximately49 specialized Marshal methods (about2909 lines before their endian methods and nested helper dependencies). Large complete dependency groups include:

| Internal component | Bytes | Marshal lines |
| --- | ---: | ---: |
| UIComponentOptionsSettingsGraphics | 212 | 387 |
| UIComponentLobby | 264 | 267 |
| UIComponentOptionsSettingsGeneral | 36 | 157 |
| UIComponentObjectSelector | 116 | 136 |
| UIComponentInGameText | 168 | 133 |
| UIComponentShellMultiplayer | 72 | 128 |
| UIComponentGameInfo | 120 | 100 |
| UIComponentPcSaveLoad | 40 | 99 |

There are additional string/reference lists, RGB/coordinate and UI control option dependencies. Some embedded component names contain Joypad; these are nested dependencies of UIConfigList, not additional independently registered Joypad roots. The active user scope excludes Joypad-named roots. If implementing UIConfigList, resolve its internal component boundary explicitly and retain all supported nested layouts; do not count UIConfigList complete after implementing only Name/Type or an empty ComponentList.

## Next complete implementation batch

1. Use contract-index.json to visit all70 concrete helpers and their complete Marshal+Endian dependencies; read mangled C-string static bytes to recover complete names/defaults and exact enum maps, not schema-only assumptions.
2. Implement root/UIDynamicConfig/pointer-list allocation ownership, raw hash and all70 concrete component layouts with complete fields and defaults. Preserve pointer/import ordering by owner block then target/index; confirm polymorphic runtime ID endian handling.
3. Test every concrete branch by numeric TypeId with nonempty fields and independent layout assertions, including all inherited Priority values. Cover nested dependency fields, defaults vs explicit-empty, unknown enums/flags, resource suffix edge cases, NUL, list growth and allocation sorting. Include multi-config mixed-component samples to verify owner-block ordering across nested references.
4. Full /Od and /O2 original ProcessInstance/direct CompileXml three-array differential on platforms0/1/2/99, independent ABI and rejection boundaries. Only after this succeeds add the one root to component-only catalog and combined link; SDK remains separately pending when prohibited.

No SDK real compilation is part of this investigation. There is no partial UIConfigList implementation to accidentally treat as recovered support.
