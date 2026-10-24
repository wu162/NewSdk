# Formation assistant XmlCompiler group

Checkpoint: implementation and complete component matrices ready; Windows acceptance pending.
This extends the player/settings component from eight to twelve public root compilers.

| Root | TypeId | TypeHash | Root bytes |
|---|---:|---:|---:|
| FormationAssistantUnitDefinition | 838130914 | 1672393125 | 24 |
| FormationAssistantTemplate | 2912385462 | 156475083 | 12 |
| FormationSelectionTemplate | 3374519264 | 87133761 | 16 |
| FormationAssistant | 1963196508 | 1061339997 | 40 |

Original source Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`:
Marshal46808/46830/46855 and endian46938 in part0093; Tracker constructors
101347/101438/101529/101620 in parts0202-0203. Original Plugin blob
`00bb9e5df72d07a21204f85f9a9a2f1d9557f00b` proves Template has no standalone
Marshal: wrapper15227 directly marshals Row list at4 and endian-corrects it.
The Row/Cell list algorithms73312..73490 in part0146 allocate contiguous8-byte
headers before recursively marshaling children. The original DLL fingerprint and
platform/endian contract are the same fixed values as XML_PLAYERS.md.

UnitDefinition allocates IncludeObjectFilter112-byte contiguous list at4, then
ExcludeObjectFilter list at12, then optional4-byte PreviewObject weak typed hash
pointer at20. Every filter uses the unchanged full ObjectFilter helper, including
nullable status sets and include/exclude weak-reference lists. PreviewObject absent
means null; present empty means an allocated zero hash.

Template's Row list header is4/8; each8-byte Row stores its Cell list at0/4;
each8-byte Cell stores Definition reference list at0/4. Every nested list's whole
header block is allocated before its elements' inner blocks. Definition imports
retain the exact final absolute field offsets. No native owning static containers
were added; private types are scoped in PlayersDetail for combined CLR linking.

Selection stores template strong reference@4, plain float MaxDragLength@8 and
unsigned MaxUnitsSelected@12; its wrapper swaps only direct scalar words while
Tracker handles imported reference endian. Assistant has weak typed hashes@4/8/12/16,
plain floats RowPadding/ColumnPadding/ActivateDragDistance/ActivateTime@20/24/28/32,
and nullable ObjectFilter@36. ActivateTime is a plain float in the actual Marshal,
not a TimeTag conversion. All absent scalar attributes remain zero.

The existing TestXmlplayers.ps1 still executes the whole eight-root matrix plus
XmlplayersFormationCases.cs in /Od and /O2 on0/1/2/99. Added cases cover every scalar,
weak hash, strong reference, all filter attributes/lists and null/empty pointers;
rows/cells/definitions and filter counts0/1/2/3/27/28/29/64/257; full BAB metadata;
unknown/foreign/duplicate/structured-leaf refusal. Independent ABI fixes364-byte
UnitDefinition filter blocks before Preview, a48-byte1-row/2-cell/3-reference grid
with exact import offsets36/40/44, and direct Assistant/Selection scalar/filter offsets.
Three arrays always compare byte for byte against the original DLL, retaining its
actual bookmark ordering. The former assumption that offsets must be sorted was
corrected to unique in-range locations; exact original-array comparison stays intact.

`recovery/sdk/XmlformationsProbe.xml` has four schema-source samples with complete
required children. Template references the UnitDefinition in that same probe and
Selection references Template; no unrelated strong-reference dependencies are added.
Full XSD, SDK profiles and isolated replay remain separate gates. Component guards
keep the same malformed/unknown-XML boundary as the player group. No whole SDK or
whole XmlCompiler replacement is claimed by this checkpoint.
