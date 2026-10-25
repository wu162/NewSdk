// IL code: BinaryAssetBuilder.GameDataVerifier
.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private auto ansi beforefieldinit BinaryAssetBuilder.GameDataVerifier.Plugin
	extends [mscorlib]System.Object
	implements [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.IAssetBuilderVerifierPlugin,
	           [BinaryAssetBuilder.Core]BinaryAssetBuilder.IAssetBuilderPluginBase
{
	// Nested Types
	.class nested private auto ansi abstract sealed beforefieldinit GameObjectVerifier
		extends [mscorlib]System.Object
	{
		// Fields
		.field private static class [System.Xml]System.Xml.XmlNamespaceManager _NamespaceManager
		.field private static class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer _Tracer

		// Methods
		.method public hidebysig static 
			bool Verify (
				class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration declaration
			) cil managed 
		{
			// Method begins at RVA 0x20b0
			// Header size: 12
			// Code size: 65 (0x41)
			.maxstack 3

			IL_0000: ldarg.0
			IL_0001: callvirt instance class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration::get_Document()
			IL_0006: callvirt instance class [System.Xml]System.Xml.XmlNamespaceManager [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument::get_NamespaceManager()
			IL_000b: stsfld class [System.Xml]System.Xml.XmlNamespaceManager BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_NamespaceManager
			IL_0010: ldarg.0
			IL_0011: callvirt instance class [System.Xml]System.Xml.XmlNode [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration::get_Node()
			IL_0016: callvirt instance class [System.Xml]System.Xml.XmlAttributeCollection [System.Xml]System.Xml.XmlNode::get_Attributes()
			IL_001b: ldstr "id"
			IL_0020: callvirt instance class [System.Xml]System.Xml.XmlAttribute [System.Xml]System.Xml.XmlAttributeCollection::get_ItemOf(string)
			IL_0025: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Value()
			IL_002a: ldarg.0
			IL_002b: callvirt instance class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration::get_Document()
			IL_0030: callvirt instance string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument::get_SourcePath()
			IL_0035: ldarg.0
			IL_0036: callvirt instance class [System.Xml]System.Xml.XmlNode [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration::get_Node()
			IL_003b: call bool BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::Verify(string, string, class [System.Xml]System.Xml.XmlNode)
			IL_0040: ret
		} // end of method GameObjectVerifier::Verify

		.method private hidebysig static 
			bool Verify (
				string gameObjectId,
				string srcFile,
				class [System.Xml]System.Xml.XmlNode node
			) cil managed 
		{
			// Method begins at RVA 0x2100
			// Header size: 12
			// Code size: 27 (0x1b)
			.maxstack 3
			.locals init (
				[0] bool
			)

			IL_0000: ldc.i4.1
			IL_0001: stloc.0
			IL_0002: ldarg.2
			IL_0003: ldarg.0
			IL_0004: ldarg.1
			IL_0005: call bool BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::VerifyModuleIds(class [System.Xml]System.Xml.XmlNode, string, string)
			IL_000a: stloc.0
			IL_000b: ldarg.2
			IL_000c: ldarg.0
			IL_000d: call bool BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::VerifyDrawModules(class [System.Xml]System.Xml.XmlNode, string)
			IL_0012: brfalse.s IL_0017

			IL_0014: ldloc.0
			IL_0015: br.s IL_0018

			IL_0017: ldc.i4.0

			IL_0018: stloc.0
			IL_0019: ldloc.0
			IL_001a: ret
		} // end of method GameObjectVerifier::Verify

		.method private hidebysig static 
			bool VerifyDrawModules (
				class [System.Xml]System.Xml.XmlNode node,
				string gameObjectId
			) cil managed 
		{
			// Method begins at RVA 0x2128
			// Header size: 12
			// Code size: 128 (0x80)
			.maxstack 3
			.locals init (
				[0] bool,
				[1] class [System.Xml]System.Xml.XmlNodeList,
				[2] class [System.Xml]System.Xml.XmlNode,
				[3] class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>,
				[4] class [mscorlib]System.Collections.IEnumerator,
				[5] class [mscorlib]System.IDisposable
			)

			IL_0000: ldc.i4.1
			IL_0001: stloc.0
			IL_0002: ldarg.0
			IL_0003: ldstr "ea:Draws/*"
			IL_0008: ldsfld class [System.Xml]System.Xml.XmlNamespaceManager BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_NamespaceManager
			IL_000d: callvirt instance class [System.Xml]System.Xml.XmlNodeList [System.Xml]System.Xml.XmlNode::SelectNodes(string, class [System.Xml]System.Xml.XmlNamespaceManager)
			IL_0012: stloc.1
			IL_0013: ldloc.1
			IL_0014: callvirt instance class [mscorlib]System.Collections.IEnumerator [System.Xml]System.Xml.XmlNodeList::GetEnumerator()
			IL_0019: stloc.s 4
			.try
			{
				IL_001b: br.s IL_005e
				// loop start (head: IL_005e)
					IL_001d: ldloc.s 4
					IL_001f: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
					IL_0024: castclass [System.Xml]System.Xml.XmlNode
					IL_0029: stloc.2
					IL_002a: ldloc.2
					IL_002b: ldarg.1
					IL_002c: call bool BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::VerifyDrawModules_DuplicateDefaultStates(class [System.Xml]System.Xml.XmlNode, string)
					IL_0031: brfalse.s IL_0036

					IL_0033: ldloc.0
					IL_0034: br.s IL_0037

					IL_0036: ldc.i4.0

					IL_0037: stloc.0
					IL_0038: newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::.ctor()
					IL_003d: stloc.3
					IL_003e: ldloc.2
					IL_003f: ldarg.1
					IL_0040: ldloca.s 3
					IL_0042: call bool BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::VerifyDrawModules_UniqueConditionsYes(class [System.Xml]System.Xml.XmlNode, string, class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>&)
					IL_0047: brfalse.s IL_004c

					IL_0049: ldloc.0
					IL_004a: br.s IL_004d

					IL_004c: ldc.i4.0

					IL_004d: stloc.0
					IL_004e: ldloc.2
					IL_004f: ldloca.s 3
					IL_0051: ldarg.1
					IL_0052: call bool BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::VerifyDrawModules_StateNameReferences(class [System.Xml]System.Xml.XmlNode, class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>&, string)
					IL_0057: brfalse.s IL_005c

					IL_0059: ldloc.0
					IL_005a: br.s IL_005d

					IL_005c: ldc.i4.0

					IL_005d: stloc.0

					IL_005e: ldloc.s 4
					IL_0060: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_0065: brtrue.s IL_001d
				// end loop

				IL_0067: leave.s IL_007e
			} // end .try
			finally
			{
				IL_0069: ldloc.s 4
				IL_006b: isinst [mscorlib]System.IDisposable
				IL_0070: stloc.s 5
				IL_0072: ldloc.s 5
				IL_0074: brfalse.s IL_007d

				IL_0076: ldloc.s 5
				IL_0078: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_007d: endfinally
			} // end handler

			IL_007e: ldloc.0
			IL_007f: ret
		} // end of method GameObjectVerifier::VerifyDrawModules

		.method private hidebysig static 
			bool VerifyDrawModules_StateNameReferences (
				class [System.Xml]System.Xml.XmlNode mod,
				class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>& stateNameAttributes,
				string gameObjectId
			) cil managed 
		{
			// Method begins at RVA 0x21c4
			// Header size: 12
			// Code size: 301 (0x12d)
			.maxstack 5
			.locals init (
				[0] bool,
				[1] class [System.Xml]System.Xml.XmlNodeList,
				[2] string,
				[3] class [System.Xml]System.Xml.XmlNode,
				[4] string,
				[5] int32,
				[6] int32,
				[7] int32,
				[8] int32,
				[9] string,
				[10] class [mscorlib]System.Collections.IEnumerator,
				[11] object[],
				[12] object[],
				[13] class [mscorlib]System.IDisposable
			)

			IL_0000: ldc.i4.1
			IL_0001: stloc.0
			IL_0002: ldarg.0
			IL_0003: ldstr "ea:AnimationState/ea:Script"
			IL_0008: ldsfld class [System.Xml]System.Xml.XmlNamespaceManager BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_NamespaceManager
			IL_000d: callvirt instance class [System.Xml]System.Xml.XmlNodeList [System.Xml]System.Xml.XmlNode::SelectNodes(string, class [System.Xml]System.Xml.XmlNamespaceManager)
			IL_0012: stloc.1
			IL_0013: ldstr "CurDrawableSetTransitionAnimState"
			IL_0018: stloc.2
			IL_0019: ldloc.1
			IL_001a: callvirt instance class [mscorlib]System.Collections.IEnumerator [System.Xml]System.Xml.XmlNodeList::GetEnumerator()
			IL_001f: stloc.s 10
			.try
			{
				IL_0021: br IL_0108
				// loop start (head: IL_0108)
					IL_0026: ldloc.s 10
					IL_0028: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
					IL_002d: castclass [System.Xml]System.Xml.XmlNode
					IL_0032: stloc.3
					IL_0033: ldloc.3
					IL_0034: callvirt instance class [System.Xml]System.Xml.XmlNode [System.Xml]System.Xml.XmlNode::get_FirstChild()
					IL_0039: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Value()
					IL_003e: stloc.s 4
					IL_0040: ldloc.s 4
					IL_0042: ldloc.2
					IL_0043: callvirt instance bool [mscorlib]System.String::Contains(string)
					IL_0048: brfalse IL_0108

					IL_004d: ldloc.s 4
					IL_004f: ldloc.2
					IL_0050: callvirt instance int32 [mscorlib]System.String::LastIndexOf(string)
					IL_0055: stloc.s 5
					IL_0057: ldloc.s 5
					IL_0059: ldloc.2
					IL_005a: callvirt instance int32 [mscorlib]System.String::get_Length()
					IL_005f: add
					IL_0060: stloc.s 6
					IL_0062: ldloc.s 4
					IL_0064: ldstr "(\""
					IL_0069: ldloc.s 6
					IL_006b: callvirt instance int32 [mscorlib]System.String::IndexOf(string, int32)
					IL_0070: ldc.i4.2
					IL_0071: add
					IL_0072: stloc.s 7
					IL_0074: ldloc.s 4
					IL_0076: ldstr "\")"
					IL_007b: ldloc.s 7
					IL_007d: callvirt instance int32 [mscorlib]System.String::IndexOf(string, int32)
					IL_0082: stloc.s 8
					IL_0084: ldloc.s 4
					IL_0086: ldloc.s 7
					IL_0088: ldloc.s 8
					IL_008a: ldloc.s 7
					IL_008c: sub
					IL_008d: callvirt instance string [mscorlib]System.String::Substring(int32, int32)
					IL_0092: stloc.s 9
					IL_0094: ldarg.1
					IL_0095: ldind.ref
					IL_0096: ldloc.s 9
					IL_0098: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::ContainsKey(!0)
					IL_009d: brfalse.s IL_00d8

					IL_009f: ldarg.1
					IL_00a0: ldind.ref
					IL_00a1: ldloc.s 9
					IL_00a3: callvirt instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::get_Item(!0)
					IL_00a8: ldc.i4.1
					IL_00a9: ble.s IL_0108

					IL_00ab: ldsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
					IL_00b0: ldstr "GameObject {0} has a script which calls {1} with an ambiguous StateName ({2})"
					IL_00b5: ldc.i4.3
					IL_00b6: newarr [mscorlib]System.Object
					IL_00bb: stloc.s 11
					IL_00bd: ldloc.s 11
					IL_00bf: ldc.i4.0
					IL_00c0: ldarg.2
					IL_00c1: stelem.ref
					IL_00c2: ldloc.s 11
					IL_00c4: ldc.i4.1
					IL_00c5: ldloc.2
					IL_00c6: stelem.ref
					IL_00c7: ldloc.s 11
					IL_00c9: ldc.i4.2
					IL_00ca: ldloc.s 9
					IL_00cc: stelem.ref
					IL_00cd: ldloc.s 11
					IL_00cf: callvirt instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::TraceWarning(string, object[])
					IL_00d4: ldc.i4.0
					IL_00d5: stloc.0
					IL_00d6: br.s IL_0108

					IL_00d8: ldsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
					IL_00dd: ldstr "GameObject {0} has a script with an invalid StateName reference to {1}, <Draw> element {2}"
					IL_00e2: ldc.i4.3
					IL_00e3: newarr [mscorlib]System.Object
					IL_00e8: stloc.s 12
					IL_00ea: ldloc.s 12
					IL_00ec: ldc.i4.0
					IL_00ed: ldarg.2
					IL_00ee: stelem.ref
					IL_00ef: ldloc.s 12
					IL_00f1: ldc.i4.1
					IL_00f2: ldloc.s 9
					IL_00f4: stelem.ref
					IL_00f5: ldloc.s 12
					IL_00f7: ldc.i4.2
					IL_00f8: ldarg.0
					IL_00f9: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Name()
					IL_00fe: stelem.ref
					IL_00ff: ldloc.s 12
					IL_0101: callvirt instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::TraceWarning(string, object[])
					IL_0106: ldc.i4.0
					IL_0107: stloc.0

					IL_0108: ldloc.s 10
					IL_010a: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_010f: brtrue IL_0026
				// end loop

				IL_0114: leave.s IL_012b
			} // end .try
			finally
			{
				IL_0116: ldloc.s 10
				IL_0118: isinst [mscorlib]System.IDisposable
				IL_011d: stloc.s 13
				IL_011f: ldloc.s 13
				IL_0121: brfalse.s IL_012a

				IL_0123: ldloc.s 13
				IL_0125: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_012a: endfinally
			} // end handler

			IL_012b: ldloc.0
			IL_012c: ret
		} // end of method GameObjectVerifier::VerifyDrawModules_StateNameReferences

		.method private hidebysig static 
			bool VerifyDrawModules_UniqueConditionsYes (
				class [System.Xml]System.Xml.XmlNode mod,
				string gameObjectId,
				class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>& stateNameAttributes
			) cil managed 
		{
			// Method begins at RVA 0x2310
			// Header size: 12
			// Code size: 536 (0x218)
			.maxstack 5
			.locals init (
				[0] bool,
				[1] class [System.Xml]System.Xml.XmlNodeList,
				[2] class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>,
				[3] class [System.Xml]System.Xml.XmlNode,
				[4] class [System.Xml]System.Xml.XmlAttribute,
				[5] string,
				[6] string[],
				[7] bool,
				[8] string,
				[9] string[],
				[10] bool,
				[11] int32,
				[12] string,
				[13] bool,
				[14] int32,
				[15] class [System.Xml]System.Xml.XmlAttribute,
				[16] string,
				[17] int32,
				[18] class [mscorlib]System.Collections.IEnumerator,
				[19] char[],
				[20] valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string, int32>,
				[21] char[],
				[22] object[],
				[23] object[],
				[24] class [mscorlib]System.IDisposable
			)

			IL_0000: ldc.i4.1
			IL_0001: stloc.0
			IL_0002: ldarg.0
			IL_0003: ldstr "ea:AnimationState"
			IL_0008: ldsfld class [System.Xml]System.Xml.XmlNamespaceManager BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_NamespaceManager
			IL_000d: callvirt instance class [System.Xml]System.Xml.XmlNodeList [System.Xml]System.Xml.XmlNode::SelectNodes(string, class [System.Xml]System.Xml.XmlNamespaceManager)
			IL_0012: stloc.1
			IL_0013: newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::.ctor()
			IL_0018: stloc.2
			IL_0019: ldloc.1
			IL_001a: callvirt instance class [mscorlib]System.Collections.IEnumerator [System.Xml]System.Xml.XmlNodeList::GetEnumerator()
			IL_001f: stloc.s 18
			.try
			{
				IL_0021: br IL_01f3
				// loop start (head: IL_01f3)
					IL_0026: ldloc.s 18
					IL_0028: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
					IL_002d: castclass [System.Xml]System.Xml.XmlNode
					IL_0032: stloc.3
					IL_0033: ldloc.3
					IL_0034: callvirt instance class [System.Xml]System.Xml.XmlAttributeCollection [System.Xml]System.Xml.XmlNode::get_Attributes()
					IL_0039: ldstr "ConditionsYes"
					IL_003e: callvirt instance class [System.Xml]System.Xml.XmlAttribute [System.Xml]System.Xml.XmlAttributeCollection::get_ItemOf(string)
					IL_0043: stloc.s 4
					IL_0045: ldloc.s 4
					IL_0047: brfalse IL_01a2

					IL_004c: ldloc.s 4
					IL_004e: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Value()
					IL_0053: stloc.s 5
					IL_0055: ldloc.2
					IL_0056: ldloc.s 5
					IL_0058: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::ContainsKey(!0)
					IL_005d: brtrue IL_0172

					IL_0062: ldloc.s 5
					IL_0064: ldc.i4.1
					IL_0065: newarr [mscorlib]System.Char
					IL_006a: stloc.s 19
					IL_006c: ldloc.s 19
					IL_006e: ldc.i4.0
					IL_006f: ldc.i4.s 32
					IL_0071: stelem.i2
					IL_0072: ldloc.s 19
					IL_0074: callvirt instance string[] [mscorlib]System.String::Split(char[])
					IL_0079: stloc.s 6
					IL_007b: ldc.i4.0
					IL_007c: stloc.s 7
					IL_007e: ldloc.2
					IL_007f: callvirt instance class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<!0, !1> class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::get_Keys()
					IL_0084: callvirt instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0, !1> class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<string, int32>::GetEnumerator()
					IL_0089: stloc.s 20
					.try
					{
						IL_008b: br IL_0112
						// loop start (head: IL_0112)
							IL_0090: ldloca.s 20
							IL_0092: call instance !0 valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string, int32>::get_Current()
							IL_0097: stloc.s 8
							IL_0099: ldloc.s 8
							IL_009b: ldc.i4.1
							IL_009c: newarr [mscorlib]System.Char
							IL_00a1: stloc.s 21
							IL_00a3: ldloc.s 21
							IL_00a5: ldc.i4.0
							IL_00a6: ldc.i4.s 32
							IL_00a8: stelem.i2
							IL_00a9: ldloc.s 21
							IL_00ab: callvirt instance string[] [mscorlib]System.String::Split(char[])
							IL_00b0: stloc.s 9
							IL_00b2: ldloc.s 9
							IL_00b4: ldlen
							IL_00b5: conv.i4
							IL_00b6: ldloc.s 6
							IL_00b8: ldlen
							IL_00b9: conv.i4
							IL_00ba: bne.un.s IL_0112

							IL_00bc: ldc.i4.1
							IL_00bd: stloc.s 10
							IL_00bf: ldc.i4.0
							IL_00c0: stloc.s 11
							IL_00c2: br.s IL_0103
							// loop start (head: IL_0103)
								IL_00c4: ldloc.s 6
								IL_00c6: ldloc.s 11
								IL_00c8: ldelem.ref
								IL_00c9: stloc.s 12
								IL_00cb: ldc.i4.0
								IL_00cc: stloc.s 13
								IL_00ce: ldc.i4.0
								IL_00cf: stloc.s 14
								IL_00d1: br.s IL_00ec
								// loop start (head: IL_00ec)
									IL_00d3: ldloc.s 12
									IL_00d5: ldloc.s 9
									IL_00d7: ldloc.s 14
									IL_00d9: ldelem.ref
									IL_00da: callvirt instance bool [mscorlib]System.String::Equals(string)
									IL_00df: brfalse.s IL_00e6

									IL_00e1: ldc.i4.1
									IL_00e2: stloc.s 13
									IL_00e4: br.s IL_00f4

									IL_00e6: ldloc.s 14
									IL_00e8: ldc.i4.1
									IL_00e9: add
									IL_00ea: stloc.s 14

									IL_00ec: ldloc.s 14
									IL_00ee: ldloc.s 9
									IL_00f0: ldlen
									IL_00f1: conv.i4
									IL_00f2: blt.s IL_00d3
								// end loop

								IL_00f4: ldloc.s 13
								IL_00f6: brtrue.s IL_00fd

								IL_00f8: ldc.i4.0
								IL_00f9: stloc.s 10
								IL_00fb: br.s IL_010b

								IL_00fd: ldloc.s 11
								IL_00ff: ldc.i4.1
								IL_0100: add
								IL_0101: stloc.s 11

								IL_0103: ldloc.s 11
								IL_0105: ldloc.s 6
								IL_0107: ldlen
								IL_0108: conv.i4
								IL_0109: blt.s IL_00c4
							// end loop

							IL_010b: ldloc.s 10
							IL_010d: brfalse.s IL_0112

							IL_010f: ldc.i4.1
							IL_0110: stloc.s 7

							IL_0112: ldloca.s 20
							IL_0114: call instance bool valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string, int32>::MoveNext()
							IL_0119: brtrue IL_0090
						// end loop

						IL_011e: leave.s IL_012e
					} // end .try
					finally
					{
						IL_0120: ldloca.s 20
						IL_0122: constrained. valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string, int32>
						IL_0128: callvirt instance void [mscorlib]System.IDisposable::Dispose()
						IL_012d: endfinally
					} // end handler

					IL_012e: ldloc.s 7
					IL_0130: brtrue.s IL_0140

					IL_0132: ldloc.2
					IL_0133: ldloc.s 5
					IL_0135: ldloc.s 6
					IL_0137: ldlen
					IL_0138: conv.i4
					IL_0139: callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::Add(!0, !1)
					IL_013e: br.s IL_01a2

					IL_0140: ldsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
					IL_0145: ldstr "GameObject {0} has duplicate ConditionYes entries for {1} in draw element {2}"
					IL_014a: ldc.i4.3
					IL_014b: newarr [mscorlib]System.Object
					IL_0150: stloc.s 22
					IL_0152: ldloc.s 22
					IL_0154: ldc.i4.0
					IL_0155: ldarg.1
					IL_0156: stelem.ref
					IL_0157: ldloc.s 22
					IL_0159: ldc.i4.1
					IL_015a: ldloc.s 5
					IL_015c: stelem.ref
					IL_015d: ldloc.s 22
					IL_015f: ldc.i4.2
					IL_0160: ldarg.0
					IL_0161: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Name()
					IL_0166: stelem.ref
					IL_0167: ldloc.s 22
					IL_0169: callvirt instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::TraceWarning(string, object[])
					IL_016e: ldc.i4.0
					IL_016f: stloc.0
					IL_0170: br.s IL_01a2

					IL_0172: ldsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
					IL_0177: ldstr "GameObject {0} has duplicate ConditionYes entries for {1} in draw element {2}"
					IL_017c: ldc.i4.3
					IL_017d: newarr [mscorlib]System.Object
					IL_0182: stloc.s 23
					IL_0184: ldloc.s 23
					IL_0186: ldc.i4.0
					IL_0187: ldarg.1
					IL_0188: stelem.ref
					IL_0189: ldloc.s 23
					IL_018b: ldc.i4.1
					IL_018c: ldloc.s 5
					IL_018e: stelem.ref
					IL_018f: ldloc.s 23
					IL_0191: ldc.i4.2
					IL_0192: ldarg.0
					IL_0193: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Name()
					IL_0198: stelem.ref
					IL_0199: ldloc.s 23
					IL_019b: callvirt instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::TraceWarning(string, object[])
					IL_01a0: ldc.i4.0
					IL_01a1: stloc.0

					IL_01a2: ldloc.3
					IL_01a3: callvirt instance class [System.Xml]System.Xml.XmlAttributeCollection [System.Xml]System.Xml.XmlNode::get_Attributes()
					IL_01a8: ldstr "StateName"
					IL_01ad: callvirt instance class [System.Xml]System.Xml.XmlAttribute [System.Xml]System.Xml.XmlAttributeCollection::get_ItemOf(string)
					IL_01b2: stloc.s 15
					IL_01b4: ldloc.s 15
					IL_01b6: brfalse.s IL_01f3

					IL_01b8: ldloc.s 15
					IL_01ba: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Value()
					IL_01bf: stloc.s 16
					IL_01c1: ldarg.2
					IL_01c2: ldind.ref
					IL_01c3: ldloc.s 16
					IL_01c5: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::ContainsKey(!0)
					IL_01ca: brfalse.s IL_01e9

					IL_01cc: ldarg.2
					IL_01cd: ldind.ref
					IL_01ce: ldloc.s 16
					IL_01d0: callvirt instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::get_Item(!0)
					IL_01d5: stloc.s 17
					IL_01d7: ldarg.2
					IL_01d8: ldind.ref
					IL_01d9: ldloc.s 16
					IL_01db: ldloc.s 17
					IL_01dd: ldc.i4.1
					IL_01de: add
					IL_01df: dup
					IL_01e0: stloc.s 17
					IL_01e2: callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::set_Item(!0, !1)
					IL_01e7: br.s IL_01f3

					IL_01e9: ldarg.2
					IL_01ea: ldind.ref
					IL_01eb: ldloc.s 16
					IL_01ed: ldc.i4.1
					IL_01ee: callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, int32>::Add(!0, !1)

					IL_01f3: ldloc.s 18
					IL_01f5: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_01fa: brtrue IL_0026
				// end loop

				IL_01ff: leave.s IL_0216
			} // end .try
			finally
			{
				IL_0201: ldloc.s 18
				IL_0203: isinst [mscorlib]System.IDisposable
				IL_0208: stloc.s 24
				IL_020a: ldloc.s 24
				IL_020c: brfalse.s IL_0215

				IL_020e: ldloc.s 24
				IL_0210: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_0215: endfinally
			} // end handler

			IL_0216: ldloc.0
			IL_0217: ret
		} // end of method GameObjectVerifier::VerifyDrawModules_UniqueConditionsYes

		.method private hidebysig static 
			bool VerifyDrawModules_DuplicateDefaultStates (
				class [System.Xml]System.Xml.XmlNode mod,
				string gameObjectId
			) cil managed 
		{
			// Method begins at RVA 0x2568
			// Header size: 12
			// Code size: 82 (0x52)
			.maxstack 5
			.locals init (
				[0] bool,
				[1] class [System.Xml]System.Xml.XmlNodeList,
				[2] object[]
			)

			IL_0000: ldc.i4.1
			IL_0001: stloc.0
			IL_0002: ldarg.0
			IL_0003: ldstr "ea:AnimationState[@ParseCondStateType=\"PARSE_DEFAULT\"]"
			IL_0008: ldsfld class [System.Xml]System.Xml.XmlNamespaceManager BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_NamespaceManager
			IL_000d: callvirt instance class [System.Xml]System.Xml.XmlNodeList [System.Xml]System.Xml.XmlNode::SelectNodes(string, class [System.Xml]System.Xml.XmlNamespaceManager)
			IL_0012: stloc.1
			IL_0013: ldloc.1
			IL_0014: callvirt instance int32 [System.Xml]System.Xml.XmlNodeList::get_Count()
			IL_0019: ldc.i4.1
			IL_001a: ble.s IL_0050

			IL_001c: ldsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
			IL_0021: ldstr "GameObject {0} has {1} default animation states in draw element {2}"
			IL_0026: ldc.i4.3
			IL_0027: newarr [mscorlib]System.Object
			IL_002c: stloc.2
			IL_002d: ldloc.2
			IL_002e: ldc.i4.0
			IL_002f: ldarg.1
			IL_0030: stelem.ref
			IL_0031: ldloc.2
			IL_0032: ldc.i4.1
			IL_0033: ldloc.1
			IL_0034: callvirt instance int32 [System.Xml]System.Xml.XmlNodeList::get_Count()
			IL_0039: box [mscorlib]System.Int32
			IL_003e: stelem.ref
			IL_003f: ldloc.2
			IL_0040: ldc.i4.2
			IL_0041: ldarg.0
			IL_0042: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Name()
			IL_0047: stelem.ref
			IL_0048: ldloc.2
			IL_0049: callvirt instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::TraceWarning(string, object[])
			IL_004e: ldc.i4.0
			IL_004f: stloc.0

			IL_0050: ldloc.0
			IL_0051: ret
		} // end of method GameObjectVerifier::VerifyDrawModules_DuplicateDefaultStates

		.method private hidebysig static 
			bool VerifyModuleIds (
				class [System.Xml]System.Xml.XmlNode node,
				string gameObjectId,
				string srcFile
			) cil managed 
		{
			// Method begins at RVA 0x25c8
			// Header size: 12
			// Code size: 260 (0x104)
			.maxstack 5
			.locals init (
				[0] bool,
				[1] class [System.Xml]System.Xml.XmlNodeList,
				[2] class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Set`1<string>,
				[3] class [System.Xml]System.Xml.XmlNode,
				[4] class [System.Xml]System.Xml.XmlAttribute,
				[5] string,
				[6] class [mscorlib]System.Collections.IEnumerator,
				[7] object[],
				[8] object[],
				[9] class [mscorlib]System.IDisposable
			)

			IL_0000: ldc.i4.1
			IL_0001: stloc.0
			IL_0002: ldarg.0
			IL_0003: ldstr "ea:Behaviors/* | ea:Draws/* | ea:AI/* | ea:Body/*"
			IL_0008: ldsfld class [System.Xml]System.Xml.XmlNamespaceManager BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_NamespaceManager
			IL_000d: callvirt instance class [System.Xml]System.Xml.XmlNodeList [System.Xml]System.Xml.XmlNode::SelectNodes(string, class [System.Xml]System.Xml.XmlNamespaceManager)
			IL_0012: stloc.1
			IL_0013: newobj instance void class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Set`1<string>::.ctor()
			IL_0018: stloc.2
			IL_0019: ldloc.1
			IL_001a: callvirt instance class [mscorlib]System.Collections.IEnumerator [System.Xml]System.Xml.XmlNodeList::GetEnumerator()
			IL_001f: stloc.s 6
			.try
			{
				IL_0021: br IL_00df
				// loop start (head: IL_00df)
					IL_0026: ldloc.s 6
					IL_0028: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
					IL_002d: castclass [System.Xml]System.Xml.XmlNode
					IL_0032: stloc.3
					IL_0033: ldloc.3
					IL_0034: callvirt instance class [System.Xml]System.Xml.XmlAttributeCollection [System.Xml]System.Xml.XmlNode::get_Attributes()
					IL_0039: ldstr "id"
					IL_003e: callvirt instance class [System.Xml]System.Xml.XmlAttribute [System.Xml]System.Xml.XmlAttributeCollection::get_ItemOf(string)
					IL_0043: stloc.s 4
					IL_0045: ldloc.s 4
					IL_0047: brfalse.s IL_005c

					IL_0049: ldloc.s 4
					IL_004b: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Value()
					IL_0050: ldstr ""
					IL_0055: call bool [mscorlib]System.String::op_Equality(string, string)
					IL_005a: brfalse.s IL_008d

					IL_005c: ldsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
					IL_0061: ldstr "GameObject {0} has an unnamed module {1} in file {2}"
					IL_0066: ldc.i4.3
					IL_0067: newarr [mscorlib]System.Object
					IL_006c: stloc.s 7
					IL_006e: ldloc.s 7
					IL_0070: ldc.i4.0
					IL_0071: ldarg.1
					IL_0072: stelem.ref
					IL_0073: ldloc.s 7
					IL_0075: ldc.i4.1
					IL_0076: ldloc.3
					IL_0077: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Name()
					IL_007c: stelem.ref
					IL_007d: ldloc.s 7
					IL_007f: ldc.i4.2
					IL_0080: ldarg.2
					IL_0081: stelem.ref
					IL_0082: ldloc.s 7
					IL_0084: callvirt instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::TraceError(string, object[])
					IL_0089: ldc.i4.0
					IL_008a: stloc.0
					IL_008b: br.s IL_00df

					IL_008d: ldloc.s 4
					IL_008f: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Value()
					IL_0094: callvirt instance string [mscorlib]System.String::ToLower()
					IL_0099: stloc.s 5
					IL_009b: ldloc.2
					IL_009c: ldloc.s 5
					IL_009e: callvirt instance bool class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Set`1<string>::Contains(!0)
					IL_00a3: brfalse.s IL_00d7

					IL_00a5: ldsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
					IL_00aa: ldstr "GameObject {0} has 2 modules with the name {1} in file {2}"
					IL_00af: ldc.i4.3
					IL_00b0: newarr [mscorlib]System.Object
					IL_00b5: stloc.s 8
					IL_00b7: ldloc.s 8
					IL_00b9: ldc.i4.0
					IL_00ba: ldarg.1
					IL_00bb: stelem.ref
					IL_00bc: ldloc.s 8
					IL_00be: ldc.i4.1
					IL_00bf: ldloc.s 4
					IL_00c1: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Value()
					IL_00c6: stelem.ref
					IL_00c7: ldloc.s 8
					IL_00c9: ldc.i4.2
					IL_00ca: ldarg.2
					IL_00cb: stelem.ref
					IL_00cc: ldloc.s 8
					IL_00ce: callvirt instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::TraceError(string, object[])
					IL_00d3: ldc.i4.0
					IL_00d4: stloc.0
					IL_00d5: br.s IL_00df

					IL_00d7: ldloc.2
					IL_00d8: ldloc.s 5
					IL_00da: callvirt instance void class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Set`1<string>::Add(!0)

					IL_00df: ldloc.s 6
					IL_00e1: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_00e6: brtrue IL_0026
				// end loop

				IL_00eb: leave.s IL_0102
			} // end .try
			finally
			{
				IL_00ed: ldloc.s 6
				IL_00ef: isinst [mscorlib]System.IDisposable
				IL_00f4: stloc.s 9
				IL_00f6: ldloc.s 9
				IL_00f8: brfalse.s IL_0101

				IL_00fa: ldloc.s 9
				IL_00fc: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_0101: endfinally
			} // end handler

			IL_0102: ldloc.0
			IL_0103: ret
		} // end of method GameObjectVerifier::VerifyModuleIds

		.method private hidebysig specialname rtspecialname static 
			void .cctor () cil managed 
		{
			// Method begins at RVA 0x26e8
			// Header size: 1
			// Code size: 21 (0x15)
			.maxstack 8

			IL_0000: ldstr "GameDataVerifier"
			IL_0005: ldstr "Verifies that Sage RTS data is logic correct"
			IL_000a: call class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::GetTracer(string, string)
			IL_000f: stsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::_Tracer
			IL_0014: ret
		} // end of method GameObjectVerifier::.cctor

	} // end of class GameObjectVerifier


	// Fields
	.field private static class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer _Tracer

	// Methods
	.method public final hidebysig newslot virtual 
		instance void Initialize (
			object configSection,
			valuetype [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.TargetPlatform platform
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method Plugin::Initialize

	.method public final hidebysig newslot virtual 
		instance void ReInitialize (
			object configSection,
			valuetype [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.TargetPlatform platform
		) cil managed 
	{
		// Method begins at RVA 0x2052
		// Header size: 1
		// Code size: 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method Plugin::ReInitialize

	.method public final hidebysig newslot virtual 
		instance bool VerifyInstance (
			class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration declaration
		) cil managed 
	{
		// Method begins at RVA 0x2054
		// Header size: 12
		// Code size: 47 (0x2f)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: ldarg.1
		IL_0001: callvirt instance class [System.Xml]System.Xml.XmlNode [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration::get_Node()
		IL_0006: brtrue.s IL_000a

		IL_0008: ldc.i4.1
		IL_0009: ret

		IL_000a: ldarg.1
		IL_000b: callvirt instance class [System.Xml]System.Xml.XmlNode [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration::get_Node()
		IL_0010: callvirt instance string [System.Xml]System.Xml.XmlNode::get_Name()
		IL_0015: dup
		IL_0016: stloc.0
		IL_0017: brfalse.s IL_002d

		IL_0019: ldloc.0
		IL_001a: ldstr "GameObject"
		IL_001f: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0024: brfalse.s IL_002d

		IL_0026: ldarg.1
		IL_0027: call bool BinaryAssetBuilder.GameDataVerifier.Plugin/GameObjectVerifier::Verify(class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.InstanceDeclaration)
		IL_002c: ret

		IL_002d: ldc.i4.1
		IL_002e: ret
	} // end of method Plugin::VerifyInstance

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x20a5
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method Plugin::.ctor

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x208f
		// Header size: 1
		// Code size: 21 (0x15)
		.maxstack 8

		IL_0000: ldstr "GameDataVerifier"
		IL_0005: ldstr "Verifies that Sage RTS data is logic correct"
		IL_000a: call class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer::GetTracer(string, string)
		IL_000f: stsfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Tracer BinaryAssetBuilder.GameDataVerifier.Plugin::_Tracer
		IL_0014: ret
	} // end of method Plugin::.cctor

} // end of class BinaryAssetBuilder.GameDataVerifier.Plugin

You are not using the latest version of the tool, please update.
Latest version is '11.1.0.9782' (yours is '9.1.0.7988')
