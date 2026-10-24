# XmlCompiler player/settings group (eight roots)

Windows component acceptance passed at source `71cbdd7bd27478d0bcb9dc3dbd2f22deac888dd7`, run `40095358086` (players job `120876360758`). The complete 14-root players/formation/skirmish group passed `/Od` and `/O2` on platforms 0/1/2/99: 44,760 fixtures, 134,280 three-buffer comparisons, and 984 rejection checks across both modes. SDK acceptance and its exact global-root scope are recorded centrally in [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md); standalone GameStatsModeData is component-tested, while SDK coverage uses MultiplayerSettings nesting because the original global schema choice omits it.
Public interfaces are `Recovered::<Type>Compiler::{CompileXml,ProcessInstance}`.
No recovered implementation calls an original compiler.

| Root | TypeId | Root bytes |
|---|---:|---:|
| AIData | 3827723072 | 220 |
| UnitAISettings | 2964891936 | 448 |
| PlayerTemplate | 4063352333 | 188 |
| MultiplayerSettings | 191082224 | 44 |
| AIBuildStateDefinition | 1644972043 | 20 |
| AIStrategicStateName | 2010562192 | 16 |
| MultiplayerColor | 2384988189 | 28 |
| AIPersonalityToAchievementEventMapping | 4086175864 | 12 |

## Fixed original evidence

Original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob
`00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, original DLL SHA256
`d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.
The existing `evidence/xml-source/method-index.txt` and bounded parts hold the evidence.
Marshal roots: AIData41002/part0082, UnitAISettings58989/parts0117-0118,
PlayerTemplate45732/part0091, MultiplayerSettings33043/part0066,
AIStrategicStateName56145/part0112, MultiplayerColor32243/part0064,
achievement mapping41643/part0083. AIBuildStateDefinition has no standalone Marshal:
its original Plugin wrapper explicitly marshals BuildEntry@12, then inherited
AIStateDefinition@0. Tracker constructors: parts0193/0229/0201/0191-0192/0226/0231.

Defaults, full field names and enums come from exact original static bytes, including
long `Optimization_*` names whose MSVC symbol labels are truncated. Names are not
inferred from those labels. Enum maps are existing `enums/*.json`: BodyDamageType,
PlayerTemplateType, FactionType, AIBeaconType, GameStatsMode and AIBuildHeuristics.
Their unknown fallbacks respectively are1,4,0,0,0,2; absence still leaves zero for
nullable/no-default fields. Schemas guide valid SDK examples, not binary semantics.

## Layout and allocation contracts

All roots use a single Tracker without a Tokenizer header, finalized at absolute
zero. Platform0 is little endian and1/2/99 big endian. Reserved root words and
packing padding stay zero. Every accepted fixture compares instance/relocations/imports.

AIData includes all61 attributes, three inline AssetId strings and a byte crater
depth@216; boolean packing is204..215. UnitAISettings includes all42 attributes,
inline456-bit model conditions@160 and complete inline ObjectFilters@220/@332.
Two final flags are444/445; padding446/447 remains zero. Byte input uses strtoul
then low-byte truncation, not signed parsing or saturation.

PlayerTemplate retains string fields, direct strong references, weak typed hashes,
unsigned resource token lists and all11 child groups. Allocation order is original
field order (DisplayNameToken, FactionIconImageName, ResourceModifierValueList), then
RedAlertTemplate pointer, packed colors, InitialUpgrade, StartingUnitInfo,
ResourceModifierObjectFilter, StartingMoney, MoneyCap, AIBeaconSettings, TechRank,
TechProductionBonuses. StartingUnitInfo is16 bytes (reference@0, Vector3@4), money
transactions8, beacon settings12 and production bonuses28. Color is AARRGGBB packed
from each channel's unsigned low byte, with A255 when a present node omits A.

MultiplayerSettings marshals its eleven singleton text elements, then the16-byte
GameStatsModeData list whose AvailableMapName list stores8-byte string headers.
Nested GameStatsModeData inherits BaseAssetType, so its required id is ignored
metadata as well. AIStrategicStateName has imported State@4 and Name string@8;
its wrapper endian-corrects only the string count directly, references go through Tracker.

AIBuildStateDefinition root has inherited heuristic count/items@4/@8 and BuildEntry
count/items@12/@16. Each BuildEntry is56 bytes including nullable ObjectFilter@52.
The list and its filters are allocated before inherited heuristic pointer array and
polymorphic payloads. The minimal Native `MarshalXmlaiHeuristics` API reuses the
existing validated heuristic marshaler in the caller's Tracker. Existing AI roots
continue to construct the same privately owned Tracker; the helper's external
Tracker is never finalized or destroyed by that helper. Each builder endian-corrects
only the fields/filters it created, so inherited data is not swapped twice.

MultiplayerColor stores three packed colors@4/8/12, TooltipName string@16 and
AvailableInMetaGame byte@24; achievement mapping stores personality import@4 and
unsigned AchievementEventId@8. Full source is split into small layout/enum includes;
there are no new owning native static containers or shutdown-time map destructors.

## Validation and boundaries

`TestXmlplayers.ps1` builds /Od and /O2 x86 CLR4 legacy hosts. `XmlplayersOracle.cs`
and three partial cases sweep every root/nested attribute and enum, conversion
boundaries, all model-condition names, imports, typed IDs, null/empty strings,
ANSI Unicode and embedded NUL, inline/pointer filters, all list sizes0/1/2/3/27/28/29/64/257,
all named leaf metadata and explicit unknown/foreign/duplicate rejection. Sixty
existing independently validated AIState heuristic fixtures are replayed inside
BuildState together with its own entries/filters, including nested weighted heuristics.
Independent ABI checks fix exact root sizes/default packing, a424-byte combined
PlayerTemplate allocation and its pointers/import offsets, nested Multiplayer string
allocation and a200-byte mixed BuildEntry/filter/constant-heuristic order.

Bare TypeId is ignored only as structural metadata; actual heuristic dispatch keeps
its numeric TypeId. Unknown attributes/namespaces, structured text leaves, unexpected
children, duplicate singletons and unknown actual polymorphic heuristics are refused.
Inherited heuristic limits remain those of the existing AI component (96).
No schema validation, inheritance, resource resolution or malformed-XML permissiveness
is claimed inside the component. SDK acceptance is recorded in [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md).

`recovery/sdk/XmlplayersProbe.xml` supplies eight legal-source target samples. The
StrategicStateName and achievement mapping reference the already present priority
probe assets `RecoveryXmlPriority_Strategic_Defaults` and
`RecoveryXmlPriority_Personality_Defaults`; keep the original fourteen-type probe
alongside this group when integrating. UnitAI includes both mandatory filters,
PlayerTemplate both required colors, Multiplayer all eleven required scalar children,
BuildState its required Heuristic and BuildEntry, and MultiplayerColor required children.
Full XSD and actual SDK/isolated replay acceptance are recorded in [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md).

The same component now has a separate four-root formation checkpoint; see XML_FORMATIONS.md. All fourteen component roots passed the acceptance run above; see XML_SKIRMISH.md for the final two roots.
