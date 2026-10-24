# Tactical mouse UI component

Implementation checkpoint: component differential pending; SDK real compilation explicitly prohibited for this round. No SDK plugin route or existing82-root SDK catalog is changed.

51 complete roots:20 tactical commands plus31 tactical buttons/tabs/navigation roots. Precise original type IDs/hashes/schemaGlobal are in catalog/XmluitacticalTypes.json. All51 appear in unchanged CnC3Types.xsd global asset choices. Full source-derived layouts are in src/Native/XmluitacticalLayouts.inc, original Module/Plugin/Tracker/Marshal/Endian evidence in evidence/xml-uitactical/source-methods.json. Fixed Module blob2d2f471c8008b7c2e1f78e39752d0726da395cea, Plugin blob00bb9e5df72d07a21204f85f9a9a2f1d9557f00b and original DLL SHA256d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6 remain unchanged. No original compiler is used by recovered code.

## Complete roots and source Tracker sizes

| Root | Size |
| --- | ---: |
| UIMouseTacticalCoOpAIButtonsHelpButton | 36 |
| UIMouseTacticalCommandCommunicateWithAllies | 20 |
| UIMouseTacticalToggleRepairModeButton | 36 |
| UIMouseTacticalAttackMoveButton | 36 |
| UIMouseTacticalCommandPlaceWall | 8 |
| UIMouseTacticalForceAttackMoveButton | 36 |
| UIMouseTacticalCoOpAIButtonsHoldObjectButton | 36 |
| UIMouseTacticalExitContainerButton | 20 |
| UIMouseTacticalPlayerTechStoreButton | 40 |
| UIMouseTacticalCommandMove | 12 |
| UIMouseTacticalCommandSell | 20 |
| UIMouseTacticalUnitIntroButton | 36 |
| UIMouseTacticalCommandSelect | 16 |
| UIMouseTacticalCoOpAIButtonsNormalButton | 36 |
| UIMouseTacticalCommandPlaceRallyPoint | 8 |
| UIMouseTacticalCommandHarvest | 8 |
| UIMouseTacticalCommandToggleSelfRepair | 12 |
| UIMouseTacticalBuildQueueTab | 12 |
| UIMouseTacticalCommandGetCrate | 8 |
| UIMouseTacticalDeactivateMissionHotSpotsButton | 36 |
| UIMouseTacticalCommandForceAttack | 12 |
| UIMouseTacticalPlanningModeButton | 36 |
| UIMouseTacticalCommandMisc | 16 |
| UIMouseTacticalSelectionRefinementTab | 36 |
| UIMouseTacticalCommandForceMove | 8 |
| UIMouseTacticalOpenAdvancedCommandsButton | 36 |
| UIMouseTacticalSetStanceButton | 12 |
| UIMouseTacticalCoOpAIButtonsHoldPositionButton | 36 |
| UIMouseTacticalCommandMineOre | 8 |
| UIMouseTacticalWaypointModeButton | 36 |
| UIMouseTacticalReverseMoveButton | 36 |
| UIMouseTacticalCommandEnter | 20 |
| UIMouseTacticalCoOpAIButtonsAssaultButton | 36 |
| UIMouseTacticalPauseButton | 36 |
| UIMouseTacticalCommandAttack | 32 |
| UIMouseTacticalCommandAttackMove | 12 |
| UIMouseTacticalForceMoveButton | 36 |
| UIMouseTacticalForceAttackButton | 36 |
| UIMouseTacticalCommandPlaceAIBeacon | 12 |
| UIMouseTacticalCommandDock | 8 |
| UIMouseTacticalCommandReverseMove | 8 |
| UIMouseTacticalBuildQueueTypeTab | 12 |
| UIMouseTacticalSellModeButton | 36 |
| UIMouseTacticalStopButton | 36 |
| UIMouseTacticalCommandSetRallyPoint | 8 |
| UIMouseTacticalFastForwardButton | 36 |
| UIMouseTacticalGroupSelection | 12 |
| UIMouseTacticalLookAt | 60 |
| UIMouseTacticalRadarTelestrator | 12 |
| UIMouseTacticalTelestrator | 8 |
| UIMouseTacticalThreatMeter | 36 |

## Layout and allocation contracts

Ordinary fixed buttons are36-byte roots with InGameUISimpleHelpTemplate inline@4. The help structure is32 bytes: four ANSI String count/pointer pairs Title@0, Description@8, TypeDescription@16, Instructions@24. Original source lines47525/47596 establish the complete help/base behavior. Absent help leaves zero bytes; present strings allocate NUL-terminated aligned blocks in title/description/type/instructions order. String count excludes the NUL byte. Empty strings still allocate. Root PlayerTechStoreButton is40 bytes with nullable TechPointAwardedSound reference-pointer@36 allocated before inherited help strings, preserving derived-first Marshal order. ExitContainerButton is20 bytes with HelpTitleFormat String@4 and HelpDescription String@12.

All20 commands preserve their complete cursor references. Attack32 has seven references@4..28; Select16 also has the two ally HotKeySlot references@8/@12. Move/ForceAttack/AttackMove/ToggleSelfRepair have both cursor fields; one-cursor roots are8 bytes. References use BAB decimal suffixes and Tracker writes index+1; bare names do not create imports. No original reference becomes a string or nullable pointer by inference.

CommunicateWithAllies20 has three cursor references@4/8/12 and nullable BeaconAttachableObjectFilter pointer@16. PlaceAIBeacon12 has BeaconTypeSettings list descriptor@4, stride20: Type enum@0, cursor refs@4/8, nullable AttachableObjectFilter@12, boolean CanTargetObjects/CanTargetPosition@16/17 defaulttrue, padding@18/19. Filters are112 bytes, same shared MarshalObjectFilter/EndianObjectFilter in a single owned Tracker, without transplanting blobs. Original Marshal pushes112 bytes only when a filter exists; present-empty is distinct from absence. Adapter strips BAB bare TypeId and normalizes native IValue attribute strings at NUL before shared parsing, reproducing original C-string enum semantics without changing existing shared code.

BuildQueueTab/BuildQueueTypeTab12 each contain QueueTypeSettings list@4, stride36: ProductionQueueType@0 plus inline help@4. SetStanceButton12 contains StanceSettings list@4 with the same36-byte structure and Stance@0. Enter20 has DefaultCursor/ContestCursor@4/8 and InfiltratorEffectTypeSettings list@12, stride8 (Effect@0/Cursor@4). Sell20 has DefaultCursors inline@4, plus FactionCursors list@12, stride12 (Faction@0, inline Cursors@4); cursor pairs are direct references@0/4. Source lists explicitly Push strides36/36/36/8/12/20, never pointer arrays.

Absent enums remain zero unless source supplies defaults. Unknown ProductionQueueType=4, InfiltratorEffectType=1, AIBeaconType=0, FactionType=0, Stance=1. Tables retain exact native case; strings terminate at NUL. Endian swaps numeric words and string counts; Tracker serializes pointers/import values; booleans/padding stay bytewise. Table-driven registry uses unique UiTacticalDetail namespace and InitOnce process-lifetime immutable ownership, preserving mixed-mode linker/normal-exit constraints.

## Full component differential

scripts/TestXmluitactical.ps1 runs/Od and/O2, four platforms0/1/2/99, fixed-original ProcessInstance and direct CompileXml three-array exact comparisons, all51 root defaults/full values, every field/parser/enum member/NUL/ANSI case, all nullable-present/absent/empty states and six list families at counts0/1/2/3/4/15/16/27/28/29/64/257. Both nullable Filter paths additionally cover complete KindOf and ObjectStatus vocabularies, bitset sign/ALL/unknown forms, status-pointer allocation and IncludeThing/ExcludeThing lists with ANSI/NUL hashes. Native layout/enum syntax and source-selector/Tracker-size checks pass locally; Windows results remain authoritative.

Independent ABI checks preserve source-grounded sizes, complete seven-reference Attack field/value/offset set, block/index import ordering, String counts and payload order, derived nullable allocation order, list strides and exact pointers, enum unknown defaults, packed booleans and filter112 allocation. Generic side arrays enforce sentinel/alignment/bounds/unique addresses, never assume global field-address order; original QuickSort equal-key behavior remains covered by exact arrays including duplicate IDs and list boundaries. Unknown root/attrs/children/namespaces, qualified attrs, unexpected text and duplicate singleton children reject explicitly. Unqualified BAB TypeId metadata is accepted.

SDK remains pending by explicit user instruction. Component/combined DLL evidence cannot be presented as SDK support; the existing82-root SDK route is frozen. Next independent UI groups include QuickChatDisplay, GroupSelection/LookAt/Telestrator, not partial subsets of the51 completed roots.


## Navigation addition and local C-string review

GroupSelection12 contains Group list@4, stride16, with Create/AddTo/Select/View HotKeySlot refs@0/4/8/12. LookAt60 has ScrollCursor@4, eight directional/rotate/zoom HotKeySlot refs@8..36, percentages KeyScrollFactor@40/MouseDragScrollFactor@44/MouseEdgeScrollFactor@48 without source fallbacks, and ViewBookmark list@52, stride8 with GoTo/Save refs@0/4. RadarTelestrator12 has Time StrokeSustainDuration@4 default10s and StrokeFadeDuration@8 default2s. Telestrator8 has direct DrawCursor@4. ThreatMeter36 uses the same full help template inline@4. All source selectors, Tracker sizes, list strides and inlined Plugin endian paths are retained in source-methods.json. Full field/list matrices and independent fixed size/offset/import/time/percentage checks cover these five additions.

Local review only: native original IValue values terminate at NUL. This new adapter normalizes all ObjectFilter attributes before invoking the existing shared helper; IncludeThing/ExcludeThing text reaches HashAssetId, which already constructs its input from text.c_str(). All five tactical enums and filter Rule/Alignment/bitset vocabularies receive DOM-constructed valid-NUL-tail, leadingNUL and unknown-NUL-known fixtures. Existing other components have not been audited or changed by this task, and no old-group regression claim follows from this local finding. SDK real compilation remains prohibited and is not run.
