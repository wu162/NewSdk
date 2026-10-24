# XmlCompiler powers recovery

Baseline: ba7a3d6845be5e124d08320cb2c56fda552d9fbe.
Branch: recovery/xmlcompiler-powers. Status: first implementation, **Windows validation pending**.

Public classes: Recovered.SpecialPowerTemplateCompiler and Recovered.ObjectCreationListCompiler.
Both expose CompileXml(XmlNode, XmlNamespaceManager, bool) and
ProcessInstance(InstanceDeclaration, TargetPlatform), returning AssetBuffer with all three arrays.
TypeIds 2178440954 and 3899542881 were checked in original Plugin.ProcessInstance.

Original XmlCompiler Plugin.HandleType<T> calls Marshal, EndianCorrect and
FinalizeTracker. FinalizeTracker calls MakeRelocatable with absolute offset zero;
there is no Tokenizer procedure table. Imports are retained.

Evidence: original Plugin.cs blob 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b;
methods.json blob 803d558b4575b2cbc83ccd303bc85ba09940b6b8;
types.json blob 93239ce090c631684e3f619d59473c7de4d0d8e6;
SDK/Schemas/xsd/AssetTypeSpecialPowerTemplate.xsd and AssetTypeObjectCreationList.xsd.
The original Module is now read through the verified source export on
recovery/xmlcompiler-priority, sourceBlob 2d2f471c8008b7c2e1f78e39752d0726da395cea,
manifest SHA256 be379d88db4c9f6de9cb4844bfc86a58d880d7a88a6b28cc76ba1c1bfc0fc6c1.
Root methods were verified in roots/SpecialPowerTemplate.cs.txt and
roots/ObjectCreationList.cs.txt; nugget bodies in parts 0057-0059,
GameDependency in part 0061, VectorHack in part 0037, nullable string/hash
helpers in parts 0120/0129/0130/0131/0169.
All local enum names/values were compared with the precise original-PE enum JSON
exports (including all 457 ModelCondition and 223 ObjectStatus entries).
The original DLL SHA256 is d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6;
the standalone validation script enforces this fingerprint.

IL tokens: power Marshal 0600011B / endian 0600011C, OCL 060000F9 / 060000FA,
nuggets Attack 060000EF, Strafe 060000F1, Fling 060000F3, Parabolic 060000F5,
Create 060000F7 and adjacent endian methods. OCL lists 06000487..0600048C.
Power root is 552 bytes: strings @4/@12, target @20, flags @28, filters
@312/@424, dependency pointer @536, forbidden tech list @540, terminal bool @548.
OCL root is 56: flags @4, six count/pointer pairs @8..@48. Nugget sizes
248/16/8/52/32/48; lists are serialized in schema group order, preserving order
within each group. Default values come from the original Marshal IL.

The implementation contains all root attributes, six OCL nugget groups,
ObjectFilter and GameDependency. Until original-DLL differential tests pass,
none of these is claimed accepted or SDK-integrated. Unknown attributes,
children, foreign namespaces, mixed structured text, duplicate singleton nodes
and structured reference values are explicitly rejected. Enumeration unknown
values retain the original fallback rather than being treated as unknown fields.
Base metadata accepted here is id and TypeId only; inheritance must already
be resolved by BAB. No original DLL is called from the recovered compiler.

Validation entry: recovery/scripts/TestXmlpowers.ps1, x86 VS2022 devshell.
Evidence: recovery/evidence/xml-powers/validation.json, sourceCommit/runId,
separate Od/O2 logs. SDK integration and runtime execution remain pending.

OCL has six independent typed lists selected by EA-qualified element name.
The original root/list/nugget methods do not read TypeId or dispatch polymorphically.
TypeId is therefore ignored as upstream metadata; fixtures include missing, zero,
known/mismatched, unknown numeric and nonnumeric values. This differs from the
GameObject module dispatcher and does not require a new polymorphic implementation.

Implementation includes schema-driven sweeps of every root/nugget attribute,
every relevant enum word, malformed scalar/reference inputs, all six typed lists,
nested vector/range/filter/dependency nodes, list sizes 1/2/27/28/29/64/257,
independent root/string/import/list/hash layouts and explicit rejection fixtures.
The oracle calls original XmlCompiler.ProcessInstance for every differential fixture.
