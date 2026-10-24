# XmlCompiler effects recovery

Baseline: ba7a3d6845be5e124d08320cb2c56fda552d9fbe.
Branch: recovery/xmlcompiler-effects. Checkpoint: not Windows validated.

Public classes: Recovered.FXListCompiler and Recovered.FXParticleSystemTemplateCompiler.
Both expose CompileXml(XmlNode, XmlNamespaceManager, bool) and
ProcessInstance(InstanceDeclaration, TargetPlatform). All successful outputs are
AssetBuffer with InstanceData, RelocationData, ImportsData.

## Current support boundary

FXList: complete five root attributes PlayEvenIfShrouded, Tailorable,
CullTracking, CullTrackingMin, CullTrackingMax; absent or empty NuggetList.
Unknown attributes, foreign namespaces, duplicate containers, nonempty
NuggetList and non-whitespace content are explicitly refused. This is a complete
root capability, not a complete FXList nugget recovery.
FXParticleSystemTemplate is explicitly refused pending enum-map and module
source evidence. Its public interface is reserved for integration.

## Evidence

Original XmlCompiler Plugin.cs blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b:
both HandleType methods Marshal, EndianCorrect, FinalizeTracker. FinalizeTracker
MakeRelocatable uses offset 0 and copies all three arrays; there is no Tokenizer
procedure table. ProcessInstance TypeIds: FXList 2254974584,
FXParticleSystemTemplate 686292351; all platforms except Win32 use big endian.

Original FXList struct is 28 bytes, FXParticleSystemTemplate 176 bytes.
The 8.4MB Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea
cannot currently be delivered by fetch_blob in this session (Transport closed);
fetch_file returns empty content even with line bounds. No claim of complete
decompiled-source inspection is made.

Independent original IL evidence is available in
recovery/evidence/BinaryAssetBuilder.XmlCompiler/methods.json blob
803d558b4575b2cbc83ccd303bc85ba09940b6b8 and types.json blob
93239ce090c631684e3f619d59473c7de4d0d8e6:
- Marshal(FXList*), token 0x060000E6: bools at 24,25 default false;
  optional time at 4; signed strtol(base=0) at 8,12; NuggetList at 16.
- EndianCorrect(FXList*), token 0x060000E7: swaps 4,8,12 then list@16.
- Tracker ctor<FXList>, token 0x060007F0: root allocation 28 bytes.
- DataInitializers.Initialize time, token 0x0600001A: searches ms before s,
  truncates at the selected unit, atof -> float, double multiplication by
  float scale 0.001f or 1.0f, converts back to float.
- Marshal<bool>, token 0x0600037F: only initial 1/t/T produces true.
- FXList no nonempty list has no serialized pointers or references.

## Validation

Run recovery/scripts/TestXmleffects.ps1 from Windows VS2022 x86 devshell.
It builds /Od and /O2 separately, starts fresh CLR4 legacy activation processes,
and compares the original XmlCompiler ProcessInstance three arrays on
platforms 0/1/2/99. Direct CompileXml is checked against recovered ProcessInstance.
Every fixture asserts independent root length, reserved bytes, list words,
padding, empty imports and relocations; selected fixtures independently check
scalar offsets/endian. Unknown input boundaries are separately rejected.

Evidence output: recovery/evidence/xml-effects/validation.json, with
succeeded/sourceCommit/runId and per-mode per-platform fixture/comparison/refusal
counts, plus Od-oracle.txt and O2-oracle.txt. Until an actual successful
Windows run exists, this checkpoint is unvalidated. SDK integration is untested.FXParticleSystemTemplate has been reassigned. Its substantial unvalidated
root/inline/keyframe/Wind/Volume/Velocity draft is preserved in disabled
XmlfxparticleDraft.h/.cpp and XmlfxparticleDraftCompiler.cpp snapshots for the
new owner to extract. These snapshots are not active code and are not included
by TestXmleffects.ps1. The active bridge contains no particle compiler class.


