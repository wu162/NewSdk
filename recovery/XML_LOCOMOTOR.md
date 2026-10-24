# XmlCompiler LocomotorTemplate

Status: implementation and differential matrix prepared; Windows validation pending.
This is a component recovery, not an SDK integration or replacement DLL acceptance.

## Public interface

`Recovered::LocomotorTemplateCompiler::CompileXml(XmlNode^, XmlNamespaceManager^, bool)`
and `ProcessInstance(InstanceDeclaration^, TargetPlatform)`; TypeId **3972178387**.
Returns all three AssetBuffer arrays. Platform 0 is little endian; 1, 2 and unknown 99 are big endian.

## Original evidence

Fixed source commit: `ba7a3d6845be5e124d08320cb2c56fda552d9fbe`.
Original Module blob: `2d2f471c8008b7c2e1f78e39752d0726da395cea`;
SHA256 `be379d88db4c9f6de9cb4844bfc86a58d880d7a88a6b28cc76ba1c1bfc0fc6c1`.
Read-only source chunks from `recovery/xmlcompiler-priority`, extraction run **40079342644**:
`recovery/evidence/xml-source/roots/LocomotorTemplate.cs.txt` contains complete Marshal
(lines 24964–25208), EndianCorrect (25210–25461), and Tracker constructor (91155–91244).
Jet methods are in part-0049 (24874 onward), pointer helpers in part-0125;
Surface and Jet option bitsets in part-0166.
The manifest verifies concatenated source chunks against the original blob.

Original method IL in baseline `recovery/evidence/BinaryAssetBuilder.XmlCompiler/methods.json`
was decoded independently: root Marshal **0x060000B0**, endian **0x060000B1**,
Jet Marshal **0x060000AE**, Jet endian **0x060000AF**, root constructor **0x060007E9**.
The generated field table records each root Marshal IL offset.
Root count is **114 attributes**, including all optional attributes.
No offsets or defaults are taken from host compiler struct packing.

Plugin.cs blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`:
HandleType at lines 3125–3214 performs **Marshal → EndianCorrect → FinalizeTracker**.
FinalizeTracker lines 37 onward calls MakeRelocatable(endian, **0**) and copies all three arrays.
The dispatch case at 38167 uses 3972178387.
There is **no Tokenizer table or prefix** and imports must be retained.

Enum maps in the original Module contain sorted-name/value pairs:
Appearance line 12298 (10), Surface 12308 (11), LocoZ 17364 (13), LocoF 17394 (13),
Jet option 12258 (1). Their stored values agree with the explicit tables used here;
the differential matrix exercises every name.
All 48 pairs have also been checked directly against the exported PE enums/*.json.
The shared ModelCondition (457, including INVALID=-1) and ObjectStatus (223) tables
have been compared in full against XmlCompiler PE maps; all pairs match.
Unknown enum fallbacks from complete IL: LocoZ=4, Appearance=1, LocoF=10,
Surface bit=3 and Jet option bit=0. Enum lookup is case sensitive.
Shared ModelCondition/ObjectStatus helpers preserve the original ALL/sign/padding behavior.

## Layout and complete supported surface

The root is **396 bytes**: reserved zero @0; Surface bitset @4; numeric/enum fields @8–104;
nullable ActiveModelConditions/ActiveObjectStatus/EnteringModelConditions pointers @108/112/116;
numeric fields @120–344; direct WaterToAirTransitionFX reference @348;
WaterSurfaceHeightOffset @352; nullable ForbiddenObjectStatus @356;
nullable BounceKickTerrainMap pointer @360; nullable JetLocomotorData pointer @364;
25 packed boolean bytes @368–392, three trailing zero pad bytes @393–395.
All 114 root names, exact defaults, kinds and offsets are in `XmllocomotorFields.inc`,
including the shipped spelling `BurniningDeath*` and `Aeleron*`.

Present empty ModelCondition attributes allocate 60 bytes; ObjectStatus allocates 28.
Absent attributes allocate nothing. Allocation follows Marshal order: ActiveModel,
ActiveObject, EnteringModel, ForbiddenObject, Bounce, Jet. Relocation entries are the
root pointer field offsets, terminated by FFFFFFFF only when nonempty.
Bounce is a nullable 4-byte reference block, even when its text is empty.
Jet is a nullable 20-byte block: Options @0, four float distances @4/8/12/16.
Absent Jet attributes remain zero. This type has no dynamic list fields.

References use shared MarshalReference: the first backslash suffix is a decimal signed
index; an imported value becomes index+1 in instance data, with its absolute field
offset in ImportsData and a final FFFFFFFF. Both direct and nested references are preserved.
No resource name is resolved or invented; BAB is expected to have resolved the input.

Float, signed, bool and percentage conversions reuse the existing recovered helpers.
Time and angle helpers reproduce ordered case-sensitive substring unit recognition,
truncate at the matched unit, parse float, then double multiply by the float factor,
and round to float. Time checks ms before s; angle checks r before d.
Endian correction swaps scalar words and pointed flag words/Jet words; packed booleans
stay bytes; Tracker handles pointer and import words.

## Explicit rejection boundary

The component accepts the asset-namespace LocomotorTemplate root, id/inheritFrom metadata,
all known unqualified root attributes, one JetLocomotorData, and one BounceKickTerrainMap.
It rejects unknown or namespaced attributes, unknown/foreign/nested child elements,
duplicate singleton children, unexpected root or Jet text, wrong types, and null inputs.
These are explicit component guards: original Marshal may silently ignore unknown XML.
Numeric garbage and unknown enum tokens retain original conversions/fallbacks and are
covered by differential tests rather than incorrectly rejected.
BAB preprocessing, XSD validation and inheritance resolution remain outside this component.

Core InstanceDeclaration.XmlNode setter (baseline InstanceDeclaration.cs, line 220) uses
node.Name, so a prefixed root gets a different TypeId. Original XmlCompiler ProcessInstance
has an explicit default returning null. The host asserts that behavior and recovered wrong-TypeId
rejection. Prefix navigation is separately checked with canonical original ProcessInstance
versus prefixed recovered CompileXml, retaining all three array and independent layout assertions.

## Validation

Run in a **VS2022 x86 developer shell**:
`powershell -File recovery/scripts/TestXmllocomotor.ps1`.
The independent script builds /Od and /O2 with /fp:precise, a Framework v4 x86 C# host
and CLR4 legacy activation config, verifies original DLL SHA256
\`d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6\`, and invokes the original XmlCompiler ProcessInstance
for every accepted fixture on platforms **0/1/2/99**.

Coverage: every root attribute, every enum/ModelCondition/ObjectStatus name, scalar
lexical/unit boundaries, optional-empty allocation, both resource forms, all Jet fields,
combined fields, child order, namespaces, long token streams, explicit refusal guards,
and independent root/block/pointer/import/sentinel/padding layouts. Every accepted
fixture compares InstanceData, RelocationData and ImportsData byte for byte.
The old-C# host contains no interpolation, local functions, using declarations or patterns.

Outputs: `recovery/evidence/xml-locomotor/validation.json` with succeeded/sourceCommit/runId,
per-mode result JSON, /Od and /O2 build/host/oracle logs. Each oracle log retains four
platform rows with fixture, three-array comparison, rejection and independent layout counts.
The JSON starts failed and succeeds only after both complete matrices pass.

First matrix run 40079870444 (integrated source a89a743b) built successfully and reached
the prefix-routing fixture. The test now distinguishes declaration routing from navigator
namespace behavior; it does not weaken or remove buffer comparisons.

Pending: final Windows x86 matrix, final sourceCommit/runId
verification, and future SDK integration. No passing Windows/SDK claim is made by this checkpoint.
