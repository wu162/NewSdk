# Unit ability and player power button templates

Complete UnitAbilityButtonTemplate and PlayerPowerButtonTemplate original global roots, with every registered Data polymorphic branch. Component-only; SDK integration remains pending. These two roots do not introduce Joypad-named roots or change shared tactical UI code.

Fixed original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

## Complete source evidence

Root UnitAbilityButtonTemplate Marshal26310, PlayerPowerButtonTemplate32219; exact root allocations12 checked in their original tracker constructors. Plugin GetExtendedTypeInformation cases3517887782/4044322798 give TypeHash2961656853/2789252562. Complete schemas AssetTypeUnitAbilityButtonTemplate.xsd, AssetTypePlayerPowerButtonTemplate.xsd and their UnitAbilityButtons/PlayerPowerButtons includes.

Unit numeric Data dispatch26180 has14 concrete branches. Player dispatch32185 has3 concrete targeted branches plus the original base Power fallback. Complete IDs, sizes, original helper lines and semantic kinds are retained in `evidence/xml-abilitybuttons/contract-index.json` and the immutable native `XmlabilitybuttonsLayouts.inc`; none of these nested variants is counted as an additional registered root.

Original helpers63407–63575 cover all Unit variants. Targeted Unit Marshal/Endian26072/26108, MultiplePowers Unit26137/26148; Player targeted31992/32028, multiple-targets32057/32092, multiple-powers32142/32153; Player polymorphic helpers/fallback66635. Single-polymorphic wrappers63745/66646 only process a wrapper with exactly one element child. Outline enum63216 uses exact5-value map and unknownHOVER_ATTACK3. Strong reference/list helpers63174–63407 use first-backslash signed atoi suffixes, owner-block bookmarks and index+1.

## Root and full Data layouts

Both roots12: reserved word, LogicCommand(Unit) or SpecialPower(Player) strong reference+4, Data pointer+8. Data wrapper contains one polymorphic child; its numeric BAB TypeId chooses the original branch, independently of the element label. Root/default absent or empty Data stays null. Unsupported wrapper multiplicity is explicitly rejected rather than silently ignoring multiple payloads.

| Data kind | Bytes | Complete payload after runtimeTypeId+0 |
| --- | ---: | --- |
| Empty Unit base/build-unit | 4 | no fields |
| Single-state Unit and original base Power | 8 | StateData strong reference+4 |
| Toggle Unit | 12 | StateData reference count+4/pointer+8 |
| Targeted Unit/Player | 28 | StateData+4; ValidTargetCursor+8(default Arrow); InvalidTargetCursor+12(default Invalid); RadiusCursor+16; ValidTargetOutline+20(default HOVER_ATTACK3); nullable RGB tint pointer+24 |
| MultiplePowers targeted Unit/Player | 52 | Targeted fields, AdditionalSpecialPowerTemplate list+28, AdditionalRadiusCursor+36, AdditionalValidTargetCursor+44 |
| MultipleTargets targeted Player | 52 | Targeted fields, weak TargetMarkerTemplate hash+28, weak TargetMarkerFXList hash+32, unsigned TargetCount+36(default1), real MinTargetDistance+40(default0), AdditionalRadiusCursor list+44 |

Each listed reference list is an8-byte count/pointer header with4-byte strong references. TargetMarker hashes are lowercase TypedAssetId values, not imports. Tint is nullable12-byte RGB float channels, distinct from packed ARGB. MultiplePowers allocates all three derived lists **before** inherited tint; MultipleTargets allocates its radius list before tint, regardless of XML order. StateData is processed after the targeted fields in the original method. Default raw cursor names lack a BAB numeric suffix and therefore create no import bookmarks at component level. All scalar words/runtime IDs/list counts/RGB channels swap for every non-Win32 platform including99; tracker serializes pointers and imports.

Unknown numeric Unit Data uses the original4-byte runtime0 fallback; unknown Player Data uses the original8-byte runtime0/StateData fallback. This known original fallback is documented and tested without counting any unknown concrete type as recovered support. Actual recognized branches are all14 Unit and3 Player branches, with every field/dependency restored.

## Files and full matrix

`src/Native/Xmlabilitybuttons.{h,cpp}`, `XmlabilitybuttonsLayouts.inc`, `src/Bridge/XmlabilitybuttonsCompiler.cpp`, `catalog/XmlabilitybuttonsTypes.json`, `tests/XmlabilitybuttonsOracle.cs`, `scripts/TestXmlabilitybuttons.ps1`.

Full original ProcessInstance plus direct CompileXml comparison of all three arrays, /Od and /O2, platforms0/1/2/99, original DLL fingerprint. Every17 recognized branch has independent runtimeID/size/complete-default word assertions. All root/state/cursor/list references receive signed overflow/negative/missing suffix/trailing/multilingual/NUL boundary matrices; all outline values/fallbacks/NUL; every RGB channel's numeric boundaries; TargetCount/MinTargetDistance full scalar corpus; marker case-folded weak hashes; all reference lists lengths0–1024 and reverse-index bookmark ordering. Root/default/base Power fallback, empty wrapper, nullable empty tint and nested metadata are covered.

Independent92-byte full MultiplePowers samples for both roots pin all scalar/list/tint offsets, derived-before-base allocation, RGB bits, root/data/list owner import grouping and non-monotonic relocation order `[8,44,52,60,36]`. Independent84-byte MultipleTargets sample pins both weak hashes' non-import behavior, target count/distance and allocation order. Unknown/qualified attrs, namespace, duplicate singleton, illegal wrapper count, RGB-vs-ARGB attributes, nested text/children and root-handle mismatch explicitly reject.

Status: complete code/matrix prepared and frozen for the integrating session's actual Windows acceptance. SDK real compilation0. Shared helpers, existing tactical source and all SDK routes are unchanged.
