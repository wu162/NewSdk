# System settings and graphics inventory components

Complete NetworkSettings, PathfindSettings, ResourceSettings and GraphicsHardware original registered global roots. All remain component-only and SDK-pending.

Fixed original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

## Complete evidence

Original Plugin HandleType GraphicsHardware26188, Pathfind26791, Network27391, Resource28644. Exact TypeId/TypeHash:

| Root | TypeId | TypeHash | Root bytes |
| --- | ---: | ---: | ---: |
| NetworkSettings | 4272453293 | 2097642646 | 8 |
| PathfindSettings | 970663416 | 2629630449 | 64 |
| ResourceSettings | 3279338544 | 1560001831 | 8 |
| GraphicsHardware | 2846460739 | 1118458536 | 12 |

Module tracker constructors110538(Graphics),111084(Pathfind),111630(Network),112722(Resource), in `evidence/xml-source/parts/part-0221.cs.txt` through `part-0225.cs.txt`.

Network Marshal52722 (`part-0105.cs.txt`); Pathfind Marshal/Endian51544/51638 (`part-0103.cs.txt`); Resource inline Plugin field marshaller reads SharedMoneyTransactionCategories at root+4 and calls MoneyTransactionCategory30 helper87496 (`part-0174.cs.txt`). Its unknown token fallback is22, not UNKNOWN0. GraphicsDevice Marshal/Endian50849/50878 (`part-0101.cs.txt`),24-byte list76483/76525 (`part-0152.cs.txt`/`part-0153.cs.txt`), GameLODType direct/nullable helpers76446/76457. Immutable enum maps from `enums/MoneyTransactionCategory.json` and `GameLODType.json`.

Full schemas: Multiplayer/NetworkSettings.xsd, System/PathfindSettings.xsd, System/ResourceSettings.xsd, System/GameLOD.xsd. Complete original Pathfind C strings were decoded from the fixed Module static data, not inferred from truncated mangled symbol names; exact ordered names are preserved in `XmlsystemsettingsPathFields.inc`.

## Full layouts and semantics

NetworkSettings: reserved zero word and signed RunAheadSlack at+4, default30; explicit empty overrides default to0. No omitted fields or pointers.

PathfindSettings: reserved word and all15 unsigned fields at successive offsets4–60. In order: MaxPathfindCellsPerFrame, MaxPathfindCellsPerPhase, MaxCellsFindMeleeEngagementLocation, MaxCellsAdjustDestination, MaxCellsAdjustHordeMeleeDestination, MaxCellsAdjustTargetDestination, MaxCellsAdjustToPossibleDestination, MaxCellsAdjustToMeleeDestination, MaxCellsAdjustToNearestGroundCell, MaxCellsAdjustToNearestValidCell, MaxCellsPatchPath, MaxCellsToExamineTowardsGoal, MaxExtraCollisionResponsePathsPerFrame, MaxExtraMoveAwayFromPathPathsPerFrame, LayerZCloseEnough. Absent all0, base0 unsigned parsing with original saturation/negative handling.

ResourceSettings: reserved word and SharedMoneyTransactionCategories30-bit set at+4. Complete30-word vocabulary, unknown bit22, original signed-token XOR removal, case-insensitive ALL sets all32 bits including padding. No imports.

GraphicsHardware: reserved word, Device count+4, pointer+8. Device rows24 bytes: unsigned VendorId+0, DeviceId+4, Name String8 length+8/pointer+12, Level enum+16, nullable MacLevel pointer+20. Present Name allocates characters/terminator before that row's MacLevel4 block; even present-empty allocates. Missing Name/MacLevel allocate nothing. Level and MacLevel use complete7-value GameLOD map, unknown present CUSTOM5, known UNKNOWN=-1; absent direct remains0 and absent nullable remainsnull. Pointer ordering follows parent block and allocation target, not global address order. No resource imports occur. All scalar words/lengths/child enums swap for every non-Win32 platform.

## Files and matrix

`src/Native/Xmlsystemsettings.{h,cpp}`, `XmlsystemsettingsEnums.inc`, `XmlsystemsettingsPathFields.inc`, `src/Bridge/XmlsystemsettingsCompiler.cpp`, `catalog/XmlsystemsettingsTypes.json`, `tests/XmlsystemsettingsOracle.cs`, `scripts/TestXmlsystemsettings.ps1`.

Original ProcessInstance plus direct CompileXml comparison of all three arrays, /Od and /O2, platforms0/1/2/99; fixed DLL fingerprint. Every field receives complete integer parser boundaries, signed/unsigned saturation/base/empty/trailing/NUL cases. All30 resource bits and7 LODs have independent value assertions, signs/repeated toggles/ALL padding/unknown/case/NUL. Graphics Name covers empty/absent/NUL/long/multilingual strings, nullable MacLevel present unknown/empty and counts0–1024. Independent8/64/8/12 root layouts, every Pathfind offset,104-byte complete three-device ABI, string bytes/targets and relocation ordering are asserted. Unsupported roots/attributes/qualified metadata/namespace/children/text and mismatched TypeId reject explicitly.

Status: complete implementation/matrix prepared; integration records actual Windows results. No SDK real compilation or active routing changes.
