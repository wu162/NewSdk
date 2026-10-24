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
The 8.4 MB -Module-.cs blob 2d2f471c8008b7c2e1f78e39752d0726da395cea
currently returns Transport closed through fetch_blob; fetch_file including line ranges
returns empty. Recovery therefore uses the extracted original IL as the primary
method evidence, with schema names and existing recovered enum maps for dependencies.

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
