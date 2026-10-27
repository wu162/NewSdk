# XmlCompiler FXList recovery

Baseline: ba7a3d6845be5e124d08320cb2c56fda552d9fbe.
Branch: recovery/xmlcompiler-priority (completion work: recovery/xmlcompiler-fxlist-completion).
Latest acceptance (2026-10-24): source `71cbdd7bd27478d0bcb9dc3dbd2f22deac888dd7`, component run [40095358086](https://github.com/wu162/NewSdk/actions/runs/40095358086), /Od and /O2, platforms 0/1/2/99. 52-entry/51-schema-type Reduced and FullMymod SDK acceptance is recorded in [XML_PRIORITY_SDK.md](XML_PRIORITY_SDK.md), run [40095358016](https://github.com/wu162/NewSdk/actions/runs/40095358016). Full replacement of all XmlCompiler types remains outside this acceptance.

## Active interface and exact boundary

Recovered.FXListCompiler exposes
CompileXml(XmlNode,XmlNamespaceManager,bool) and
ProcessInstance(InstanceDeclaration,TargetPlatform).
All successful results preserve AssetBuffer.InstanceData, RelocationData and
ImportsData. The bridge validates FXList TypeId 2254974584 and the asset namespace.
The actual BAB SDK trace also annotates the root with a numeric TypeId.
An optional matching root TypeId (decimal or base-zero hexadecimal) is accepted
as metadata and does not add a serialized word; mismatches and foreign namespace
attributes remain explicitly refused. This correction arose from real SDK run
40090297467, whose original build succeeded and recovered build rejected the
root metadata. New default/nugget fixtures cover decimal/hex metadata on all
four component platforms and retain the full previous matrix.
BAB also annotates every descendant whose schema type is named. The bridge
allows inert bare TypeId metadata on containers, vectors, random variables,
filters and their reference/enum leaves, matching the original marshaler's
ignored metadata. Filter input maps omit this annotation before invoking the
shared helper. Nugget TypeId still controls its original polymorphic dispatch;
the root TypeId guard remains strict. Nested-metadata fixtures cover these paths.

All five root attributes are implemented: PlayEvenIfShrouded, Tailorable,
CullTracking, CullTrackingMin, CullTrackingMax.
NuggetList supports all fifteen original known concrete TypeIds:

| Type | TypeId | Bytes |
| --- | ---: | ---: |
| SoundFXNugget | 503892219 | 48 |
| TerrainScorchFXNugget | 138194854 | 56 |
| ForceFeedbackFXNugget | 638391350 | 48 |
| DynamicDecalFXNugget | 866573550 | 108 |
| ParticleSystemFXNugget | 751572338 | 148 |
| LaserFXNugget | 904475169 | 64 |
| AttachedModelFXNugget | 2263385183 | 56 |
| TintDrawableFXNugget | 2106748504 | 76 |
| RayEffectFXNugget | 2386148390 | 72 |
| EvaEventFXNugget | 2424288100 | 56 |
| FXParticleSysBoneNugget | 3121289073 | 84 |
| ViewShakeFXNugget | 4011332978 | 48 |
| CameraShakerVolumeFXNugget | 3768725989 | 56 |
| FXListAtBonePosFXNugget | 4234415617 | 56 |
| LightPulseFXNugget | 4293352609 | 72 |

Each branch includes its complete derived attributes and children, including
nullable TerrainScorch.RandomRange, ParticleSystem.TargetOffset, its three
random variables, packed ARGB color, vectors, angles, time units, typed hashes,
resource references and absent-versus-empty strings.
Inherited support includes all four 456-bit model-condition pointer fields,
Weather, StopIfPlayed, OnlyIfOnLand, PlayIfSourceIsStealthed and the repeated
SourceMustNotHaveBeenDisabledThisFrameByType enum list.

SecondaryObjectFilter and SourceObjectFilter are supported, including all root
attributes, nullable status bitsets, and repeated IncludeThing/ExcludeThing typed
hash lists. Unknown nugget TypeId uses the original 44-byte base fallback and
writes TypeId zero; known branches retain their concrete TypeId.
Unknown attributes, foreign namespaces,
duplicate singleton containers/children and non-whitespace unexpected text
are refused. This completion is covered by the final matrix cited above.
Valid known enum fields retain the
original unknown-text fallbacks; enum misspellings are not schema errors here.

## Original evidence and important contracts

Original Module blob: 2d2f471c8008b7c2e1f78e39752d0726da395cea.
Verified source chunks/roots, static literal bytes and native PE enum maps:
recovery/xmlcompiler-priority:recovery/evidence/xml-source.
The manifest pins baseline/sourceBlob and byte-identical source reconstruction.
Original DLL SHA256:
d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6.

Plugin.cs blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b:
HandleType<FXList> performs Marshal -> EndianCorrect -> FinalizeTracker.
MakeRelocatable uses absolute offset zero, no Tokenizer procedure header, and
copies all three buffers. Win32 alone is little endian; 1/2/99 use big endian.

Key source lines: FXList Marshal/Endian 28046/28098 (28-byte root);
FXNugget base Marshal/Endian/dispatch 27012/27075/27130;
concrete nugget methods 27254-28300 and corresponding typed allocation wrappers;
Time/Angle initializers and whitespace Tokenize 18262-18499;
Vector2/Vector3/RGBColor 18535/18556/18721;
RandomVariable 19162; ModelCondition parser 83646 and pointer wrapper 61622;
DisabledType list 64324. Enum tables are embedded from original PE entries,
never inferred from XSD order.

Root: reserved@0, time@4, signed min/max@8/12, list count/pointer@16/20,
booleans@24/25. Nugget base: TypeId@0, four optional 60-byte bitsets@4/8/12/16,
Weather@20, unsupported filter pointers@24/28, disabled count/pointer@32/36,
booleans@40/41/42. Exact derived offsets are in the adapter descriptors.

Time searches ms before s, angle searches r before d; atof first rounds to
float, then multiplies in double by the float scale, then rounds to float.
Signed/unsigned integer fields retain their original CRT/base-zero contracts.
RandomVariable absent normally remains zero; ParticleSystemFXNugget.InitialDelay instead starts CONSTANT/Low=-1/High=-1, even when its child is present but omits Low/High. Present missing Type defaults UNIFORM.
Color channels wrap to bytes; DynamicDecal Color packs A:R:G:B into a word.
String absence differs from present empty (which allocates a NUL).
References use the decimal suffix after first backslash; typed IDs hash the
lowercased complete input. Shared ResourceMarshaling/Relocation implement these
proven contracts; this component never clears imports.

Model-condition tokens are whitespace-delimited and case-sensitive except ALL.
Unknown words map to 233 (ABOUT_TO_HIT); INVALID=-1 is ignored.
A negative token XORs its bit, rather than clearing it. ALL (even -ALL) fills
all fifteen words, and its original branch does not advance the input pointer,
so later tokens have no effect. The implementation preserves that behavior.

## Validation

Run recovery/scripts/TestXmleffects.ps1 in a Windows VS2022 x86 devshell.
It builds /Od and /O2 separately and starts fresh .NET Framework CLR4 processes
with legacy activation. For platforms 0/1/2/99, every accepted fixture invokes
original XmlCompiler ProcessInstance and compares all three arrays byte-for-byte.
Direct CompileXml is compared to recovered ProcessInstance too.

Coverage includes all fifteen branch defaults/distinct fields, all original
ModelCondition entries in each of the four bitsets, all relevant enum maps and
unknown fallbacks, numeric/time/angle/string boundaries, list sizes
2/27/28/29/64/257, nullable allocations, signed/unsigned differences, pointer and
import tables, explicit refusals and independent scalar/bit/packed-color layouts.
Prefix navigation compares canonical original declarations with prefixed direct
CompileXml, with wrong TypeId separately refused.

Outputs: recovery/evidence/xml-effects/validation.json containing
succeeded/sourceCommit/runId and per-/Od-/O2 per-platform fixture,
threeArrayComparisons, directArrayComparisons, rejections and independentLayouts
counts. Build/host logs and Od-oracle.txt/O2-oracle.txt are retained.
The script writes succeeded=false before building and on any failure.

## Filter and unknown-base completion evidence (2026-10-24)

Original sourceBlob remains 2d2f471c8008b7c2e1f78e39752d0726da395cea.
`parts/part-0054.cs.txt` FXNugget Marshal line 27012 allocates SecondaryObjectFilter
at +24, then SourceObjectFilter at +28, each 112 bytes. Both follow the four
model-condition bitsets and precede the disabled enum list. The matching endian
method traverses both pointed filters. `parts/part-0052.cs.txt` ObjectFilter
Marshal/Endian lines 26334/26371 match the existing verified ObjectFilter helper:
id/Rule/Relationship/Alignment at 0/4/8/12, KindOf Include/Exclude at 16/52,
nullable seven-word status bitsets at 88/92, typed hash count/pointers at
96/100 and 104/108. The component reuses this helper without modifying it.
PE enum maps in evidence/xml-source/enums pin KindOfType, ObjectStatusType,
ObjectFilterRule/Alignment/Relationship independently of XSD order.

`parts/part-0129.cs.txt`, unknown wrapper line 64552 allocates 44 bytes, calls
base Marshal and writes zero to TypeId. Endian dispatch falls back to the base
method for that zero. The bridge keeps strict validation of unsupported XML
attributes/children while accepting unknown numeric or absent TypeId as base.

New tests retain all earlier fixtures and add filters on every known branch,
unknown/default/parsed-TypeId cases, all source PE KindOf/ObjectStatus entries
with independent bit layout checks on both filters, enum fallbacks, signed
bitset operations, absent/empty values, repeated typed hash lists at
0/1/2/27/28/29/64/257 and structural/namespace rejection boundaries.
The /Od /O2 four-platform full matrix and three-array/direct comparisons remain
mandatory and passed at the final source above. The 52-entry/51-schema-type SDK plugin and temporary-SDK switch are documented in XML_PRIORITY_SDK.md; original SDK DLLs, public common native helpers and GameObject source remain unchanged.

