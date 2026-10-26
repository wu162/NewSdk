# AudioCompiler integration review

Read-only repository review on2026-10-26, before the current full-Upheaval
result. This document and the static inventory do not certify an SDK run,
combined recovered B0, R0, or final EXE integration.

## Fixed heads and tree identity

| Branch | Reviewed head | Meaning |
|---|---|---|
| recovery/compiler-source | b122a54abe4405516b021e425b8fb9235d914a0e | Common restored compiler baseline |
| recovery/audio-compiler-source | 7545797e543147e4eb59b5d258e47281479f4ef8 | Audio handoff; validated business source2d6468 |
| refactor/compiler-exe-integration | 46b878558c933bd87d320b63ae17658172166f33 | Joint B0 preparation and failed original gate |

The audio source tree is `ee6074a096795aa66b682fcb363e360feabe4d2f`.
Its validated implementation source is
`2d64684742f9b19808d8fd721fbe31a549907c37`; component/new SDK run40244825458
and preceding4488-asset SDK run40243817293 retain their original source scope.

Audio and compiler-source have identical complete SDK trees:
`1f2d71a20ce8bf7d9343c7a2246c10275abe28f9`.
Their complete `recovery/src` trees are also identical:
`c13c492885fc64bfac0f53562360d706252b6489`.
MetadataInventory/catalog/decompiled/w3x trees were separately checked and
match. No unchanged shared compiler needs a new component execution merely
because audio source/docs/evidence are merged.

The refactor branch has a different shared source tree,
`937a7129322ae086bff8b63cf83cb026f6e099f6`: it adds environment-gated Core
tracing. Do not claim that branch is the exact source accepted above.

## Actual EXE state and hard prerequisite

At46b8785 the root has no`src` directory; recursive`recovery/src` inspection
finds the existing Managed library projects, no final EXE project/CLI entry.
EXE_REFACTOR_STATUS explicitly says R0/R1/R2/R3/R4 have not started.

The only joint B0 receipt is
`recovery/evidence/joint-baseline/40191603240-1/validation.json` at46b8785:

- source2bde81cad5faaacd5475167a121484b6a2be13ac;
- `succeeded=false`, `originalBaselinePassed=false`;
- first Original case3 TacticalHUDBuildQueuePage invocation exits-1;
- restored builds/output comparisons are empty.

The recorded command includes`/bc:false /sc:false /ss:false`, unlike the
Studio-derived successful flow's`/ss:true`. The stored receipt proves the
failure stage, not which individual switch caused it. No current audio-only
Upheaval success may be relabeled as this joint B0 passing.

The hard order remains: latest joint B0 success -> R0 same-EXE/linking and
legacy Texture compatibility -> migration/full EXE/CLI -> clean publication
and real-MOD acceptance. Audio now has restored C# business source, so the
final host does not need to keep the original mixed AudioCompiler as its
production codec backend; the unresolved original Texture plugin's Core type
identity remains a separate R0 problem.

## Safe Audio merge after its full-Upheaval gate

1. Collect terminal audio-only Upheaval evidence with exact source, input,
   runId/attempt, actual audio coverage, four outputs and CustomData. Other
   original plugins in that test remain explicitly original.
2. Re-read compiler-source head before merging. Recheck SDK/shared-source
   equality if it moved; preserve all concurrent additions, never force.
3. Preserve compiler-source's generic RESUME body and add a short Audio/Win32
   section linking AUDIO_COMPILER.md and the immutable receipts. Keep the
   Audio-only resume as a separate scoped entry if useful; do not replace the
   shared GameObject/XML/W3X/Managed navigation with an Audio-only document.
4. A temporary integration branch from the audio head can change only this
   generic navigation, then PR into compiler-source. Because the reviewed
   target is an ancestor of audio, business source conflict is not currently
   expected; actual current target/mergeability must still be checked.
5. Mark only Audio's measured gate complete. Keep production SDK/config
   replacement and final EXE flags false until their own operations pass.

Suggested added navigation content: AudioCompiler is Win32-only; use
`src/AudioCompiler`; source2d6468/treeee6074, component/new SDK40244825458 and
older SDK40243817293 retain exact scopes; current full-Upheaval result must be
filled from its real receipt. Link AUDIO_COMPILER.md for restrictions and next
EXE gate. Do not import the stale refactor B0 failure as a success.

## Small next executable preparation: latest joint B0

No directory migration or Core architecture rewrite is required to prepare it.
Use a fresh branch from the post-Audio compiler-source, not an old tree that
omits restored Audio. Selectively port the existing refactor preparation:

- `.github/workflows/joint-sdk-baseline.yml`;
- `TestJointSdkBaseline.ps1`, `BuildJointManagedBaseline.ps1`,
  `AssertJointSdkEvidence.ps1`;
- the`BuildW3xPlugin.ps1 -DependencyDirectory` support;
- `JointBaselineTrace.cs` and the three environment-gated call sites in
  Core's AssetDeclarationDocument. This is tracing, not an EXE migration.

Before running, repair/audit the entire batch together:

- keep fixed full input068643219fded09a05fb7b75c6507b769c430316 and its194
  versioned.asset files; preserve original-first gating;
- use the measured Studio command semantics, notably`/ss:true`; do not keep
  extra`/bc /sc` switches without verifying their intended behavior;
- require Windows/x86 VS C++/CLI (`cl.exe /clr /MD /EHa /std:c++17 /O2
  /fp:precise`), SDK8, .NET2/net472 references, and existing metadata repair
  tools for Utility/MvpXml;
- after original passes, build the eight managed targets Utility/Metrics/
  ExpressionEval/GameDataVerifier/MvpXml/Core/Remote/EALABuild and the recovered
  XML/GameObject/Weapon/Armor/W3X bridges against the same recovered Core;
- add current Audio source. A temporary generated audio project can retain the
  same source/compiler settings while referencing this batch's recovered Core,
  rather than the standalone project's original-SDK HintPath. Record that
  temporary build definition and input file hashes; do not silently overwrite
  the published standalone project or claim an EXE exists;
- install the recovered Audio route and remove the original target DLL in the
  recovered test SDK. Update AssertJoint's current AudioFile expectation from
  BinaryAssetBuilder.AudioCompiler to the actually built recovered assembly;
  require path/hash and actual calls. Preserve all existing other-module,
  MvpXml/XInclude and actual XML/root coverage assertions;
- compare the20 four-output products plus all relevant CustomData and actual
  Audio buffers; retain the audio adapter if Core's general trace does not
  record those three buffers. Preserve input/config/tool fingerprints and
  original-first failure evidence.

Canonical launch once the repaired complete batch is reviewable:
`./recovery/scripts/TestJointSdkBaseline.ps1 -TimeoutSeconds 1200` under the
x86 developer environment.
This remains unexecuted preparation here. R0 must not begin on the old failed
receipt. A full joint build/run may exceed the time left after this round's
Audio acceptance; saving a concrete preparation is preferable to claiming R0.

## Audio source input inventory for R0

[exe-inputs-current.json](exe-inputs-current.json), generated by
`recovery/scripts/InspectAudioExeInputs.py`, checks all21 tracked Audio source
/project blobs and recomputes their Git tree. It inventories20 C# files:
19 business files plus one assembly-attribute source. Their bytes match the
reviewed remote tree exactly. The current standalone project is net472,
LangVersion10, unsafe enabled, and explicitly references only the original
SDK Core; it is still a validation library, not the final host.

Manual source review finds nine Core type names at the integration boundary:
IAssetBuilderPlugin, InstanceDeclaration, AssetBuffer, TargetPlatform,
ExtendedTypeInformation, Tracer, HashProvider, BinaryAssetBuilderException,
ErrorCode. Pattern scanning finds no native codec imports, Assembly.Load, or
embedded resource lookup in these20 files. This is a static observation, not
compiled metadata/native import/runtime-load certification.

AssemblyInfo's title/version attributes must be consolidated with host-owned
assembly metadata. They cannot all be blindly compiled beside the host's
attributes; that metadata handling does not justify dropping business source.
The manifest records proposed destinations only. No source was moved, no
`.props` facade was added, and no recovery DLL/worker was presented as an EXE.

## Jonwil source relevance to this stage

`sagetools` supplies closely related SIMEX/MPEG/WAVE/resampler and XAS codec
source; the reviewed XAS predictor constants/interleave and header-only MPEG
info shape match the recovered algorithm family. It is useful for focused
cross-audits and new feature investigation. It cannot supply a NewSdk Core
identity/CLI/linker architecture or replace the original-DLL/SDK gate.
`ealayer3compilerplugin` itself references Core and P/Invokes audio.dll, so
adopting that architecture would not satisfy the no-restored-business-DLL
EXE requirement. Do not replace already accepted C# solely to consume these
repositories; any proposed change needs actual original behavior evidence and
its affected complete test group.
