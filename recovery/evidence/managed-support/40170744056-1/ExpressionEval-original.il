// IL code: BinaryAssetBuilder.ExpressionEval
.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator
	extends [mscorlib]System.Object
	implements [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.IExpressionEvaluator
{
	// Fields
	.field private class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument _Document

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument document
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 14 (0xe)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ldarg.0
		IL_0007: ldarg.1
		IL_0008: stfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator::_Document
		IL_000d: ret
	} // end of method ExpressionEvaluator::.ctor

	.method public hidebysig 
		instance string LookupDefine (
			string name
		) cil managed 
	{
		// Method begins at RVA 0x2060
		// Header size: 12
		// Code size: 110 (0x6e)
		.maxstack 5
		.locals init (
			[0] class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition,
			[1] object[]
		)

		IL_0000: ldarg.1
		IL_0001: ldc.i4.0
		IL_0002: ldc.i4.1
		IL_0003: callvirt instance string [mscorlib]System.String::Remove(int32, int32)
		IL_0008: starg.s name
		IL_000a: ldnull
		IL_000b: stloc.0
		IL_000c: ldarg.0
		IL_000d: ldfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator::_Document
		IL_0012: callvirt instance class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.DefinitionSet [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument::get_AllDefines()
		IL_0017: ldarg.1
		IL_0018: ldloca.s 0
		IL_001a: callvirt instance bool class [System]System.Collections.Generic.SortedDictionary`2<string, class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition>::TryGetValue(!0, !1&)
		IL_001f: brfalse.s IL_0055

		IL_0021: ldloc.0
		IL_0022: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::EvaluatedValue
		IL_0027: brfalse.s IL_0055

		IL_0029: ldloc.0
		IL_002a: ldfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::Document
		IL_002f: ldarg.0
		IL_0030: ldfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator::_Document
		IL_0035: beq.s IL_004e

		IL_0037: ldarg.0
		IL_0038: ldfld class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator::_Document
		IL_003d: callvirt instance class [mscorlib]System.Collections.Generic.IDictionary`2<string, string> [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.AssetDeclarationDocument::get_UsedDefines()
		IL_0042: ldarg.1
		IL_0043: ldloc.0
		IL_0044: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::EvaluatedValue
		IL_0049: callvirt instance void class [mscorlib]System.Collections.Generic.IDictionary`2<string, string>::set_Item(!0, !1)

		IL_004e: ldloc.0
		IL_004f: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::EvaluatedValue
		IL_0054: ret

		IL_0055: ldc.i4.s 18
		IL_0057: ldstr "Unknown define {0}"
		IL_005c: ldc.i4.1
		IL_005d: newarr [mscorlib]System.Object
		IL_0062: stloc.1
		IL_0063: ldloc.1
		IL_0064: ldc.i4.0
		IL_0065: ldarg.1
		IL_0066: stelem.ref
		IL_0067: ldloc.1
		IL_0068: newobj instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.BinaryAssetBuilderException::.ctor(valuetype [BinaryAssetBuilder.Core]BinaryAssetBuilder.ErrorCode, string, object[])
		IL_006d: throw
	} // end of method ExpressionEvaluator::LookupDefine

	.method public final hidebysig newslot virtual 
		instance string Evaluate (
			string expression
		) cil managed 
	{
		// Method begins at RVA 0x20dc
		// Header size: 12
		// Code size: 38 (0x26)
		.maxstack 2
		.locals init (
			[0] class ExpressionLexer,
			[1] class ExpressionParser
		)

		IL_0000: ldarg.1
		IL_0001: newobj instance void [mscorlib]System.IO.StringReader::.ctor(string)
		IL_0006: newobj instance void ExpressionLexer::.ctor(class [mscorlib]System.IO.TextReader)
		IL_000b: stloc.0
		IL_000c: ldloc.0
		IL_000d: newobj instance void ExpressionParser::.ctor(class [antlr.runtime]antlr.TokenStream)
		IL_0012: stloc.1
		IL_0013: ldloc.1
		IL_0014: ldarg.0
		IL_0015: callvirt instance void ExpressionParser::SetEvaluator(class BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator)
		IL_001a: ldloc.1
		IL_001b: callvirt instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::expr()
		IL_0020: callvirt instance string BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsString()
		IL_0025: ret
	} // end of method ExpressionEvaluator::Evaluate

	.method public final hidebysig newslot virtual 
		instance void EvaluateDefinition (
			class [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition definition
		) cil managed 
	{
		// Method begins at RVA 0x2110
		// Header size: 12
		// Code size: 109 (0x6d)
		.maxstack 2
		.locals init (
			[0] class ExpressionLexer,
			[1] class ExpressionParser
		)

		IL_0000: ldarg.1
		IL_0001: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::OriginalValue
		IL_0006: brtrue.s IL_0009

		IL_0008: ret

		IL_0009: ldarg.1
		IL_000a: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::OriginalValue
		IL_000f: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_0014: ldc.i4.0
		IL_0015: ble.s IL_0059

		IL_0017: ldarg.1
		IL_0018: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::OriginalValue
		IL_001d: ldc.i4.0
		IL_001e: callvirt instance char [mscorlib]System.String::get_Chars(int32)
		IL_0023: ldc.i4.s 61
		IL_0025: bne.un.s IL_0059

		IL_0027: ldarg.1
		IL_0028: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::OriginalValue
		IL_002d: newobj instance void [mscorlib]System.IO.StringReader::.ctor(string)
		IL_0032: newobj instance void ExpressionLexer::.ctor(class [mscorlib]System.IO.TextReader)
		IL_0037: stloc.0
		IL_0038: ldloc.0
		IL_0039: newobj instance void ExpressionParser::.ctor(class [antlr.runtime]antlr.TokenStream)
		IL_003e: stloc.1
		IL_003f: ldloc.1
		IL_0040: ldarg.0
		IL_0041: callvirt instance void ExpressionParser::SetEvaluator(class BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator)
		IL_0046: ldarg.1
		IL_0047: ldloc.1
		IL_0048: callvirt instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::expr()
		IL_004d: callvirt instance string BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsString()
		IL_0052: stfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::EvaluatedValue
		IL_0057: br.s IL_0065

		IL_0059: ldarg.1
		IL_005a: ldarg.1
		IL_005b: ldfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::OriginalValue
		IL_0060: stfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::EvaluatedValue

		IL_0065: ldarg.1
		IL_0066: ldnull
		IL_0067: stfld string [BinaryAssetBuilder.Core]BinaryAssetBuilder.Core.Definition::OriginalValue
		IL_006c: ret
	} // end of method ExpressionEvaluator::EvaluateDefinition

} // end of class BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator

.class public auto ansi beforefieldinit ExpressionLexer
	extends [antlr.runtime]antlr.CharScanner
	implements [antlr.runtime]antlr.TokenStream
{
	// Fields
	.field public static literal int32 EOF = int32(1)
	.field public static literal int32 NULL_TREE_LOOKAHEAD = int32(3)
	.field public static literal int32 EQ = int32(4)
	.field public static literal int32 OR = int32(5)
	.field public static literal int32 AND = int32(6)
	.field public static literal int32 NE = int32(7)
	.field public static literal int32 LS = int32(8)
	.field public static literal int32 GT = int32(9)
	.field public static literal int32 LE = int32(10)
	.field public static literal int32 GE = int32(11)
	.field public static literal int32 PLUS = int32(12)
	.field public static literal int32 MINUS = int32(13)
	.field public static literal int32 MUL = int32(14)
	.field public static literal int32 DIV = int32(15)
	.field public static literal int32 MOD = int32(16)
	.field public static literal int32 LPAREN = int32(17)
	.field public static literal int32 RPAREN = int32(18)
	.field public static literal int32 INT = int32(19)
	.field public static literal int32 FLOAT = int32(20)
	.field public static literal int32 STRING = int32(21)
	.field public static literal int32 STRING2 = int32(22)
	.field public static literal int32 DEFINE = int32(23)
	.field public static literal int32 ROUND = int32(24)
	.field public static literal int32 WS = int32(25)
	.field public static literal int32 DIGIT = int32(26)
	.field public static literal int32 LETTER = int32(27)
	.field public static literal int32 NUMBER = int32(28)
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_0_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_1_

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [mscorlib]System.IO.Stream ins
		) cil managed 
	{
		// Method begins at RVA 0x2189
		// Header size: 1
		// Code size: 13 (0xd)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: newobj instance void [antlr.runtime]antlr.ByteBuffer::.ctor(class [mscorlib]System.IO.Stream)
		IL_0007: call instance void ExpressionLexer::.ctor(class [antlr.runtime]antlr.InputBuffer)
		IL_000c: ret
	} // end of method ExpressionLexer::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [mscorlib]System.IO.TextReader r
		) cil managed 
	{
		// Method begins at RVA 0x2197
		// Header size: 1
		// Code size: 13 (0xd)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: newobj instance void [antlr.runtime]antlr.CharBuffer::.ctor(class [mscorlib]System.IO.TextReader)
		IL_0007: call instance void ExpressionLexer::.ctor(class [antlr.runtime]antlr.InputBuffer)
		IL_000c: ret
	} // end of method ExpressionLexer::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [antlr.runtime]antlr.InputBuffer ib
		) cil managed 
	{
		// Method begins at RVA 0x21a5
		// Header size: 1
		// Code size: 13 (0xd)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: newobj instance void [antlr.runtime]antlr.LexerSharedInputState::.ctor(class [antlr.runtime]antlr.InputBuffer)
		IL_0007: call instance void ExpressionLexer::.ctor(class [antlr.runtime]antlr.LexerSharedInputState)
		IL_000c: ret
	} // end of method ExpressionLexer::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [antlr.runtime]antlr.LexerSharedInputState state
		) cil managed 
	{
		// Method begins at RVA 0x21b3
		// Header size: 1
		// Code size: 14 (0xe)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance void [antlr.runtime]antlr.CharScanner::.ctor(class [antlr.runtime]antlr.LexerSharedInputState)
		IL_0007: ldarg.0
		IL_0008: call instance void ExpressionLexer::initialize()
		IL_000d: ret
	} // end of method ExpressionLexer::.ctor

	.method private hidebysig 
		instance void initialize () cil managed 
	{
		// Method begins at RVA 0x21c2
		// Header size: 1
		// Code size: 39 (0x27)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldc.i4.1
		IL_0002: stfld bool [antlr.runtime]antlr.CharScanner::caseSensitiveLiterals
		IL_0007: ldarg.0
		IL_0008: ldc.i4.1
		IL_0009: callvirt instance void [antlr.runtime]antlr.CharScanner::setCaseSensitive(bool)
		IL_000e: ldarg.0
		IL_000f: ldc.i4.s 100
		IL_0011: ldc.r4 0.4
		IL_0016: ldnull
		IL_0017: ldsfld class [mscorlib]System.Collections.Comparer [mscorlib]System.Collections.Comparer::Default
		IL_001c: newobj instance void [mscorlib]System.Collections.Hashtable::.ctor(int32, float32, class [mscorlib]System.Collections.IHashCodeProvider, class [mscorlib]System.Collections.IComparer)
		IL_0021: stfld class [mscorlib]System.Collections.Hashtable [antlr.runtime]antlr.CharScanner::literals
		IL_0026: ret
	} // end of method ExpressionLexer::initialize

	.method public hidebysig virtual 
		instance class [antlr.runtime]antlr.IToken nextToken () cil managed 
	{
		// Method begins at RVA 0x21ec
		// Header size: 12
		// Code size: 874 (0x36a)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.RecognitionException,
			[2] class [antlr.runtime]antlr.CharStreamException,
			[3] class [antlr.runtime]antlr.IToken,
			[4] char
		)

		IL_0000: ldc.i4.0
		IL_0001: stloc.0
		IL_0002: ldarg.0
		IL_0003: callvirt instance void [antlr.runtime]antlr.CharScanner::resetText()
		.try
		{
			.try
			{
				IL_0008: ldarg.0
				IL_0009: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_000e: stloc.s 4
				IL_0010: ldloc.s 4
				IL_0012: ldc.i4.s 97
				IL_0014: bgt IL_0102

				IL_0019: ldloc.s 4
				IL_001b: ldc.i4.s 9
				IL_001d: sub
				IL_001e: switch (IL_0116, IL_0116, IL_0259, IL_0259, IL_0116, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0259, IL_0116, IL_0188, IL_0220, IL_0259, IL_0246, IL_01e7, IL_0259, IL_0233, IL_0129, IL_013c, IL_01c1, IL_019b, IL_0259, IL_01ae, IL_020d, IL_01d4, IL_020d, IL_020d, IL_020d, IL_020d, IL_020d, IL_020d, IL_020d, IL_020d, IL_020d, IL_020d, IL_0259, IL_0259, IL_0259, IL_0175)

				IL_00f7: ldloc.s 4
				IL_00f9: ldc.i4.s 97
				IL_00fb: beq.s IL_0162

				IL_00fd: br IL_0259

				IL_0102: ldloc.s 4
				IL_0104: ldc.i4.s 111
				IL_0106: beq.s IL_014f

				IL_0108: ldloc.s 4
				IL_010a: ldc.i4.s 114
				IL_010c: beq IL_01fa

				IL_0111: br IL_0259

				IL_0116: ldarg.0
				IL_0117: ldc.i4.1
				IL_0118: call instance void ExpressionLexer::mWS(bool)
				IL_011d: ldarg.0
				IL_011e: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0123: pop
				IL_0124: br IL_0304

				IL_0129: ldarg.0
				IL_012a: ldc.i4.1
				IL_012b: call instance void ExpressionLexer::mLPAREN(bool)
				IL_0130: ldarg.0
				IL_0131: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0136: pop
				IL_0137: br IL_0304

				IL_013c: ldarg.0
				IL_013d: ldc.i4.1
				IL_013e: call instance void ExpressionLexer::mRPAREN(bool)
				IL_0143: ldarg.0
				IL_0144: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0149: pop
				IL_014a: br IL_0304

				IL_014f: ldarg.0
				IL_0150: ldc.i4.1
				IL_0151: call instance void ExpressionLexer::mOR(bool)
				IL_0156: ldarg.0
				IL_0157: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_015c: pop
				IL_015d: br IL_0304

				IL_0162: ldarg.0
				IL_0163: ldc.i4.1
				IL_0164: call instance void ExpressionLexer::mAND(bool)
				IL_0169: ldarg.0
				IL_016a: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_016f: pop
				IL_0170: br IL_0304

				IL_0175: ldarg.0
				IL_0176: ldc.i4.1
				IL_0177: call instance void ExpressionLexer::mEQ(bool)
				IL_017c: ldarg.0
				IL_017d: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0182: pop
				IL_0183: br IL_0304

				IL_0188: ldarg.0
				IL_0189: ldc.i4.1
				IL_018a: call instance void ExpressionLexer::mNE(bool)
				IL_018f: ldarg.0
				IL_0190: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0195: pop
				IL_0196: br IL_0304

				IL_019b: ldarg.0
				IL_019c: ldc.i4.1
				IL_019d: call instance void ExpressionLexer::mPLUS(bool)
				IL_01a2: ldarg.0
				IL_01a3: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_01a8: pop
				IL_01a9: br IL_0304

				IL_01ae: ldarg.0
				IL_01af: ldc.i4.1
				IL_01b0: call instance void ExpressionLexer::mMINUS(bool)
				IL_01b5: ldarg.0
				IL_01b6: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_01bb: pop
				IL_01bc: br IL_0304

				IL_01c1: ldarg.0
				IL_01c2: ldc.i4.1
				IL_01c3: call instance void ExpressionLexer::mMUL(bool)
				IL_01c8: ldarg.0
				IL_01c9: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_01ce: pop
				IL_01cf: br IL_0304

				IL_01d4: ldarg.0
				IL_01d5: ldc.i4.1
				IL_01d6: call instance void ExpressionLexer::mDIV(bool)
				IL_01db: ldarg.0
				IL_01dc: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_01e1: pop
				IL_01e2: br IL_0304

				IL_01e7: ldarg.0
				IL_01e8: ldc.i4.1
				IL_01e9: call instance void ExpressionLexer::mMOD(bool)
				IL_01ee: ldarg.0
				IL_01ef: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_01f4: pop
				IL_01f5: br IL_0304

				IL_01fa: ldarg.0
				IL_01fb: ldc.i4.1
				IL_01fc: call instance void ExpressionLexer::mROUND(bool)
				IL_0201: ldarg.0
				IL_0202: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0207: pop
				IL_0208: br IL_0304

				IL_020d: ldarg.0
				IL_020e: ldc.i4.1
				IL_020f: call instance void ExpressionLexer::mNUMBER(bool)
				IL_0214: ldarg.0
				IL_0215: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_021a: pop
				IL_021b: br IL_0304

				IL_0220: ldarg.0
				IL_0221: ldc.i4.1
				IL_0222: call instance void ExpressionLexer::mSTRING(bool)
				IL_0227: ldarg.0
				IL_0228: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_022d: pop
				IL_022e: br IL_0304

				IL_0233: ldarg.0
				IL_0234: ldc.i4.1
				IL_0235: call instance void ExpressionLexer::mSTRING2(bool)
				IL_023a: ldarg.0
				IL_023b: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0240: pop
				IL_0241: br IL_0304

				IL_0246: ldarg.0
				IL_0247: ldc.i4.1
				IL_0248: call instance void ExpressionLexer::mDEFINE(bool)
				IL_024d: ldarg.0
				IL_024e: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0253: pop
				IL_0254: br IL_0304

				IL_0259: ldarg.0
				IL_025a: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_025f: ldc.i4.s 60
				IL_0261: bne.un.s IL_0276

				IL_0263: ldarg.0
				IL_0264: ldc.i4.1
				IL_0265: call instance void ExpressionLexer::mLE(bool)
				IL_026a: ldarg.0
				IL_026b: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0270: pop
				IL_0271: br IL_0304

				IL_0276: ldarg.0
				IL_0277: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_027c: ldc.i4.s 60
				IL_027e: bne.un.s IL_0290

				IL_0280: ldarg.0
				IL_0281: ldc.i4.1
				IL_0282: call instance void ExpressionLexer::mLS(bool)
				IL_0287: ldarg.0
				IL_0288: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_028d: pop
				IL_028e: br.s IL_0304

				IL_0290: ldarg.0
				IL_0291: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_0296: ldc.i4.s 62
				IL_0298: bne.un.s IL_02aa

				IL_029a: ldarg.0
				IL_029b: ldc.i4.1
				IL_029c: call instance void ExpressionLexer::mGE(bool)
				IL_02a1: ldarg.0
				IL_02a2: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_02a7: pop
				IL_02a8: br.s IL_0304

				IL_02aa: ldarg.0
				IL_02ab: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_02b0: ldc.i4.s 62
				IL_02b2: bne.un.s IL_02c4

				IL_02b4: ldarg.0
				IL_02b5: ldc.i4.1
				IL_02b6: call instance void ExpressionLexer::mGT(bool)
				IL_02bb: ldarg.0
				IL_02bc: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_02c1: pop
				IL_02c2: br.s IL_0304

				IL_02c4: ldarg.0
				IL_02c5: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_02ca: ldsfld char [antlr.runtime]antlr.CharScanner::EOF_CHAR
				IL_02cf: bne.un.s IL_02e6

				IL_02d1: ldarg.0
				IL_02d2: callvirt instance void [antlr.runtime]antlr.CharScanner::uponEOF()
				IL_02d7: ldarg.0
				IL_02d8: ldarg.0
				IL_02d9: ldc.i4.1
				IL_02da: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
				IL_02df: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_02e4: br.s IL_0304

				IL_02e6: ldarg.0
				IL_02e7: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_02ec: ldarg.0
				IL_02ed: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
				IL_02f2: ldarg.0
				IL_02f3: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
				IL_02f8: ldarg.0
				IL_02f9: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
				IL_02fe: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
				IL_0303: throw

				IL_0304: ldarg.0
				IL_0305: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_030a: brtrue.s IL_0311

				IL_030c: leave IL_0000

				IL_0311: ldarg.0
				IL_0312: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0317: callvirt instance int32 [antlr.runtime]antlr.IToken::get_Type()
				IL_031c: stloc.0
				IL_031d: ldarg.0
				IL_031e: ldloc.0
				IL_031f: callvirt instance int32 [antlr.runtime]antlr.CharScanner::testLiteralsTable(int32)
				IL_0324: stloc.0
				IL_0325: ldarg.0
				IL_0326: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_032b: ldloc.0
				IL_032c: callvirt instance void [antlr.runtime]antlr.IToken::set_Type(int32)
				IL_0331: ldarg.0
				IL_0332: ldfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
				IL_0337: stloc.3
				IL_0338: leave.s IL_0368
			} // end .try
			catch [antlr.runtime]antlr.RecognitionException
			{
				IL_033a: stloc.1
				IL_033b: ldloc.1
				IL_033c: newobj instance void [antlr.runtime]antlr.TokenStreamRecognitionException::.ctor(class [antlr.runtime]antlr.RecognitionException)
				IL_0341: throw
			} // end handler
		} // end .try
		catch [antlr.runtime]antlr.CharStreamException
		{
			IL_0342: stloc.2
			IL_0343: ldloc.2
			IL_0344: isinst [antlr.runtime]antlr.CharStreamIOException
			IL_0349: brfalse.s IL_035c

			IL_034b: ldloc.2
			IL_034c: castclass [antlr.runtime]antlr.CharStreamIOException
			IL_0351: ldfld class [mscorlib]System.IO.IOException [antlr.runtime]antlr.CharStreamIOException::io
			IL_0356: newobj instance void [antlr.runtime]antlr.TokenStreamIOException::.ctor(class [mscorlib]System.IO.IOException)
			IL_035b: throw

			IL_035c: ldloc.2
			IL_035d: callvirt instance string [mscorlib]System.Exception::get_Message()
			IL_0362: newobj instance void [antlr.runtime]antlr.TokenStreamException::.ctor(string)
			IL_0367: throw
		} // end handler

		IL_0368: ldloc.3
		IL_0369: ret
	} // end of method ExpressionLexer::nextToken

	.method public hidebysig 
		instance void mWS (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2598
		// Header size: 12
		// Code size: 208 (0xd0)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32,
			[3] char
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 25
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_0017: stloc.3
		IL_0018: ldloc.3
		IL_0019: ldc.i4.s 9
		IL_001b: sub
		IL_001c: switch (IL_0044, IL_004e, IL_0062, IL_0062, IL_0058)

		IL_0035: ldloc.3
		IL_0036: ldc.i4.s 32
		IL_0038: bne.un.s IL_0062

		IL_003a: ldarg.0
		IL_003b: ldc.i4.s 32
		IL_003d: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0042: br.s IL_0080

		IL_0044: ldarg.0
		IL_0045: ldc.i4.s 9
		IL_0047: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_004c: br.s IL_0080

		IL_004e: ldarg.0
		IL_004f: ldc.i4.s 10
		IL_0051: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0056: br.s IL_0080

		IL_0058: ldarg.0
		IL_0059: ldc.i4.s 13
		IL_005b: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0060: br.s IL_0080

		IL_0062: ldarg.0
		IL_0063: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_0068: ldarg.0
		IL_0069: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
		IL_006e: ldarg.0
		IL_006f: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
		IL_0074: ldarg.0
		IL_0075: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
		IL_007a: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
		IL_007f: throw

		IL_0080: ldarg.0
		IL_0081: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_0086: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_008b: brtrue.s IL_0093

		IL_008d: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0092: stloc.0

		IL_0093: ldarg.1
		IL_0094: brfalse.s IL_00c8

		IL_0096: ldloc.1
		IL_0097: brtrue.s IL_00c8

		IL_0099: ldloc.0
		IL_009a: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_009f: beq.s IL_00c8

		IL_00a1: ldarg.0
		IL_00a2: ldloc.0
		IL_00a3: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_00a8: stloc.1
		IL_00a9: ldloc.1
		IL_00aa: ldarg.0
		IL_00ab: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_00b0: ldloc.2
		IL_00b1: ldarg.0
		IL_00b2: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_00b7: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_00bc: ldloc.2
		IL_00bd: sub
		IL_00be: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_00c3: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_00c8: ldarg.0
		IL_00c9: ldloc.1
		IL_00ca: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_00cf: ret
	} // end of method ExpressionLexer::mWS

	.method public hidebysig 
		instance void mLPAREN (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2674
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 17
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 40
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mLPAREN

	.method public hidebysig 
		instance void mRPAREN (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x26d8
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 18
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 41
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mRPAREN

	.method public hidebysig 
		instance void mOR (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x273c
		// Header size: 12
		// Code size: 88 (0x58)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.5
		IL_000f: stloc.0
		IL_0010: ldarg.0
		IL_0011: ldstr "or"
		IL_0016: callvirt instance void [antlr.runtime]antlr.CharScanner::match(string)
		IL_001b: ldarg.1
		IL_001c: brfalse.s IL_0050

		IL_001e: ldloc.1
		IL_001f: brtrue.s IL_0050

		IL_0021: ldloc.0
		IL_0022: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0027: beq.s IL_0050

		IL_0029: ldarg.0
		IL_002a: ldloc.0
		IL_002b: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0030: stloc.1
		IL_0031: ldloc.1
		IL_0032: ldarg.0
		IL_0033: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0038: ldloc.2
		IL_0039: ldarg.0
		IL_003a: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003f: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0044: ldloc.2
		IL_0045: sub
		IL_0046: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_004b: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0050: ldarg.0
		IL_0051: ldloc.1
		IL_0052: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0057: ret
	} // end of method ExpressionLexer::mOR

	.method public hidebysig 
		instance void mAND (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x27a0
		// Header size: 12
		// Code size: 88 (0x58)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.6
		IL_000f: stloc.0
		IL_0010: ldarg.0
		IL_0011: ldstr "and"
		IL_0016: callvirt instance void [antlr.runtime]antlr.CharScanner::match(string)
		IL_001b: ldarg.1
		IL_001c: brfalse.s IL_0050

		IL_001e: ldloc.1
		IL_001f: brtrue.s IL_0050

		IL_0021: ldloc.0
		IL_0022: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0027: beq.s IL_0050

		IL_0029: ldarg.0
		IL_002a: ldloc.0
		IL_002b: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0030: stloc.1
		IL_0031: ldloc.1
		IL_0032: ldarg.0
		IL_0033: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0038: ldloc.2
		IL_0039: ldarg.0
		IL_003a: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003f: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0044: ldloc.2
		IL_0045: sub
		IL_0046: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_004b: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0050: ldarg.0
		IL_0051: ldloc.1
		IL_0052: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0057: ret
	} // end of method ExpressionLexer::mAND

	.method public hidebysig 
		instance void mEQ (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2804
		// Header size: 12
		// Code size: 85 (0x55)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.4
		IL_000f: stloc.0
		IL_0010: ldarg.0
		IL_0011: ldc.i4.s 61
		IL_0013: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0018: ldarg.1
		IL_0019: brfalse.s IL_004d

		IL_001b: ldloc.1
		IL_001c: brtrue.s IL_004d

		IL_001e: ldloc.0
		IL_001f: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0024: beq.s IL_004d

		IL_0026: ldarg.0
		IL_0027: ldloc.0
		IL_0028: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002d: stloc.1
		IL_002e: ldloc.1
		IL_002f: ldarg.0
		IL_0030: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0035: ldloc.2
		IL_0036: ldarg.0
		IL_0037: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003c: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0041: ldloc.2
		IL_0042: sub
		IL_0043: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0048: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004d: ldarg.0
		IL_004e: ldloc.1
		IL_004f: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0054: ret
	} // end of method ExpressionLexer::mEQ

	.method public hidebysig 
		instance void mNE (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2868
		// Header size: 12
		// Code size: 88 (0x58)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.7
		IL_000f: stloc.0
		IL_0010: ldarg.0
		IL_0011: ldstr "!="
		IL_0016: callvirt instance void [antlr.runtime]antlr.CharScanner::match(string)
		IL_001b: ldarg.1
		IL_001c: brfalse.s IL_0050

		IL_001e: ldloc.1
		IL_001f: brtrue.s IL_0050

		IL_0021: ldloc.0
		IL_0022: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0027: beq.s IL_0050

		IL_0029: ldarg.0
		IL_002a: ldloc.0
		IL_002b: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0030: stloc.1
		IL_0031: ldloc.1
		IL_0032: ldarg.0
		IL_0033: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0038: ldloc.2
		IL_0039: ldarg.0
		IL_003a: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003f: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0044: ldloc.2
		IL_0045: sub
		IL_0046: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_004b: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0050: ldarg.0
		IL_0051: ldloc.1
		IL_0052: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0057: ret
	} // end of method ExpressionLexer::mNE

	.method public hidebysig 
		instance void mLE (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x28cc
		// Header size: 12
		// Code size: 89 (0x59)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 10
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldstr "<="
		IL_0017: callvirt instance void [antlr.runtime]antlr.CharScanner::match(string)
		IL_001c: ldarg.1
		IL_001d: brfalse.s IL_0051

		IL_001f: ldloc.1
		IL_0020: brtrue.s IL_0051

		IL_0022: ldloc.0
		IL_0023: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0028: beq.s IL_0051

		IL_002a: ldarg.0
		IL_002b: ldloc.0
		IL_002c: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0031: stloc.1
		IL_0032: ldloc.1
		IL_0033: ldarg.0
		IL_0034: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0039: ldloc.2
		IL_003a: ldarg.0
		IL_003b: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0040: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0045: ldloc.2
		IL_0046: sub
		IL_0047: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_004c: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0051: ldarg.0
		IL_0052: ldloc.1
		IL_0053: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0058: ret
	} // end of method ExpressionLexer::mLE

	.method public hidebysig 
		instance void mLS (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2934
		// Header size: 12
		// Code size: 85 (0x55)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.8
		IL_000f: stloc.0
		IL_0010: ldarg.0
		IL_0011: ldc.i4.s 60
		IL_0013: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0018: ldarg.1
		IL_0019: brfalse.s IL_004d

		IL_001b: ldloc.1
		IL_001c: brtrue.s IL_004d

		IL_001e: ldloc.0
		IL_001f: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0024: beq.s IL_004d

		IL_0026: ldarg.0
		IL_0027: ldloc.0
		IL_0028: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002d: stloc.1
		IL_002e: ldloc.1
		IL_002f: ldarg.0
		IL_0030: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0035: ldloc.2
		IL_0036: ldarg.0
		IL_0037: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003c: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0041: ldloc.2
		IL_0042: sub
		IL_0043: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0048: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004d: ldarg.0
		IL_004e: ldloc.1
		IL_004f: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0054: ret
	} // end of method ExpressionLexer::mLS

	.method public hidebysig 
		instance void mGE (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2998
		// Header size: 12
		// Code size: 89 (0x59)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 11
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldstr ">="
		IL_0017: callvirt instance void [antlr.runtime]antlr.CharScanner::match(string)
		IL_001c: ldarg.1
		IL_001d: brfalse.s IL_0051

		IL_001f: ldloc.1
		IL_0020: brtrue.s IL_0051

		IL_0022: ldloc.0
		IL_0023: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0028: beq.s IL_0051

		IL_002a: ldarg.0
		IL_002b: ldloc.0
		IL_002c: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0031: stloc.1
		IL_0032: ldloc.1
		IL_0033: ldarg.0
		IL_0034: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0039: ldloc.2
		IL_003a: ldarg.0
		IL_003b: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0040: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0045: ldloc.2
		IL_0046: sub
		IL_0047: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_004c: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0051: ldarg.0
		IL_0052: ldloc.1
		IL_0053: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0058: ret
	} // end of method ExpressionLexer::mGE

	.method public hidebysig 
		instance void mGT (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2a00
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 9
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 62
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mGT

	.method public hidebysig 
		instance void mPLUS (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2a64
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 12
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 43
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mPLUS

	.method public hidebysig 
		instance void mMINUS (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2ac8
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 13
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 45
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mMINUS

	.method public hidebysig 
		instance void mMUL (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2b2c
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 14
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 42
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mMUL

	.method public hidebysig 
		instance void mDIV (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2b90
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 15
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 47
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mDIV

	.method public hidebysig 
		instance void mMOD (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2bf4
		// Header size: 12
		// Code size: 86 (0x56)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 16
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 37
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.1
		IL_001a: brfalse.s IL_004e

		IL_001c: ldloc.1
		IL_001d: brtrue.s IL_004e

		IL_001f: ldloc.0
		IL_0020: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0025: beq.s IL_004e

		IL_0027: ldarg.0
		IL_0028: ldloc.0
		IL_0029: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0036: ldloc.2
		IL_0037: ldarg.0
		IL_0038: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0042: ldloc.2
		IL_0043: sub
		IL_0044: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0049: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_004e: ldarg.0
		IL_004f: ldloc.1
		IL_0050: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0055: ret
	} // end of method ExpressionLexer::mMOD

	.method public hidebysig 
		instance void mROUND (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2c58
		// Header size: 12
		// Code size: 89 (0x59)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 24
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldstr "round"
		IL_0017: callvirt instance void [antlr.runtime]antlr.CharScanner::match(string)
		IL_001c: ldarg.1
		IL_001d: brfalse.s IL_0051

		IL_001f: ldloc.1
		IL_0020: brtrue.s IL_0051

		IL_0022: ldloc.0
		IL_0023: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0028: beq.s IL_0051

		IL_002a: ldarg.0
		IL_002b: ldloc.0
		IL_002c: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0031: stloc.1
		IL_0032: ldloc.1
		IL_0033: ldarg.0
		IL_0034: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0039: ldloc.2
		IL_003a: ldarg.0
		IL_003b: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0040: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0045: ldloc.2
		IL_0046: sub
		IL_0047: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_004c: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0051: ldarg.0
		IL_0052: ldloc.1
		IL_0053: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0058: ret
	} // end of method ExpressionLexer::mROUND

	.method family hidebysig 
		instance void mDIGIT (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2cc0
		// Header size: 12
		// Code size: 88 (0x58)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 26
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 48
		IL_0014: ldc.i4.s 57
		IL_0016: callvirt instance void [antlr.runtime]antlr.CharScanner::matchRange(char, char)
		IL_001b: ldarg.1
		IL_001c: brfalse.s IL_0050

		IL_001e: ldloc.1
		IL_001f: brtrue.s IL_0050

		IL_0021: ldloc.0
		IL_0022: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0027: beq.s IL_0050

		IL_0029: ldarg.0
		IL_002a: ldloc.0
		IL_002b: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0030: stloc.1
		IL_0031: ldloc.1
		IL_0032: ldarg.0
		IL_0033: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0038: ldloc.2
		IL_0039: ldarg.0
		IL_003a: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_003f: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0044: ldloc.2
		IL_0045: sub
		IL_0046: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_004b: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0050: ldarg.0
		IL_0051: ldloc.1
		IL_0052: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0057: ret
	} // end of method ExpressionLexer::mDIGIT

	.method family hidebysig 
		instance void mLETTER (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2d24
		// Header size: 12
		// Code size: 392 (0x188)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32,
			[3] char
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 27
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_0017: stloc.3
		IL_0018: ldloc.3
		IL_0019: ldc.i4.s 65
		IL_001b: sub
		IL_001c: switch (IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_0117, IL_012d, IL_012d, IL_012d, IL_012d, IL_0123, IL_012d, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b, IL_010b)

		IL_0109: br.s IL_012d

		IL_010b: ldarg.0
		IL_010c: ldc.i4.s 97
		IL_010e: ldc.i4.s 122
		IL_0110: callvirt instance void [antlr.runtime]antlr.CharScanner::matchRange(char, char)
		IL_0115: br.s IL_014b

		IL_0117: ldarg.0
		IL_0118: ldc.i4.s 65
		IL_011a: ldc.i4.s 90
		IL_011c: callvirt instance void [antlr.runtime]antlr.CharScanner::matchRange(char, char)
		IL_0121: br.s IL_014b

		IL_0123: ldarg.0
		IL_0124: ldc.i4.s 95
		IL_0126: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_012b: br.s IL_014b

		IL_012d: ldarg.0
		IL_012e: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_0133: ldarg.0
		IL_0134: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
		IL_0139: ldarg.0
		IL_013a: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
		IL_013f: ldarg.0
		IL_0140: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
		IL_0145: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
		IL_014a: throw

		IL_014b: ldarg.1
		IL_014c: brfalse.s IL_0180

		IL_014e: ldloc.1
		IL_014f: brtrue.s IL_0180

		IL_0151: ldloc.0
		IL_0152: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0157: beq.s IL_0180

		IL_0159: ldarg.0
		IL_015a: ldloc.0
		IL_015b: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0160: stloc.1
		IL_0161: ldloc.1
		IL_0162: ldarg.0
		IL_0163: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0168: ldloc.2
		IL_0169: ldarg.0
		IL_016a: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_016f: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0174: ldloc.2
		IL_0175: sub
		IL_0176: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_017b: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0180: ldarg.0
		IL_0181: ldloc.1
		IL_0182: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0187: ret
	} // end of method ExpressionLexer::mLETTER

	.method public hidebysig 
		instance void mNUMBER (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x2eb8
		// Header size: 12
		// Code size: 820 (0x334)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32,
			[3] bool,
			[4] int32,
			[5] int32,
			[6] bool,
			[7] int32,
			[8] int32,
			[9] int32,
			[10] bool,
			[11] int32,
			[12] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 28
		IL_0010: stloc.0
		IL_0011: ldc.i4.0
		IL_0012: stloc.3
		IL_0013: ldarg.0
		IL_0014: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_0019: ldc.i4.s 46
		IL_001b: bne.un.s IL_006b

		IL_001d: ldarg.0
		IL_001e: callvirt instance int32 [antlr.runtime]antlr.CharScanner::mark()
		IL_0023: stloc.s 4
		IL_0025: ldc.i4.1
		IL_0026: stloc.3
		IL_0027: ldarg.0
		IL_0028: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_002d: dup
		IL_002e: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_0033: ldc.i4.1
		IL_0034: add
		IL_0035: stfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		.try
		{
			IL_003a: ldarg.0
			IL_003b: ldc.i4.s 46
			IL_003d: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
			IL_0042: ldarg.0
			IL_0043: ldc.i4.0
			IL_0044: call instance void ExpressionLexer::mDIGIT(bool)
			IL_0049: leave.s IL_0050
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_004b: pop
			IL_004c: ldc.i4.0
			IL_004d: stloc.3
			IL_004e: leave.s IL_0050
		} // end handler

		IL_0050: ldarg.0
		IL_0051: ldloc.s 4
		IL_0053: callvirt instance void [antlr.runtime]antlr.CharScanner::rewind(int32)
		IL_0058: ldarg.0
		IL_0059: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_005e: dup
		IL_005f: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_0064: ldc.i4.1
		IL_0065: sub
		IL_0066: stfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing

		IL_006b: ldloc.3
		IL_006c: brfalse.s IL_00d9

		IL_006e: ldarg.0
		IL_006f: ldc.i4.s 46
		IL_0071: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0076: ldc.i4.0
		IL_0077: stloc.s 5
		// loop start (head: IL_0079)
			IL_0079: ldarg.0
			IL_007a: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_007f: ldc.i4.s 48
			IL_0081: blt.s IL_0096

			IL_0083: ldarg.0
			IL_0084: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_0089: ldc.i4.s 57
			IL_008b: bgt.s IL_0096

			IL_008d: ldarg.0
			IL_008e: ldc.i4.0
			IL_008f: call instance void ExpressionLexer::mDIGIT(bool)
			IL_0094: br.s IL_00b9

			IL_0096: ldloc.s 5
			IL_0098: ldc.i4.1
			IL_0099: bge.s IL_00c1

			IL_009b: ldarg.0
			IL_009c: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_00a1: ldarg.0
			IL_00a2: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
			IL_00a7: ldarg.0
			IL_00a8: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
			IL_00ad: ldarg.0
			IL_00ae: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
			IL_00b3: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
			IL_00b8: throw

			IL_00b9: ldloc.s 5
			IL_00bb: ldc.i4.1
			IL_00bc: add
			IL_00bd: stloc.s 5
			IL_00bf: br.s IL_0079
		// end loop

		IL_00c1: ldarg.0
		IL_00c2: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_00c7: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_00cc: brtrue IL_02f7

		IL_00d1: ldc.i4.s 20
		IL_00d3: stloc.0
		IL_00d4: br IL_02f7

		IL_00d9: ldc.i4.0
		IL_00da: stloc.s 6
		IL_00dc: ldarg.0
		IL_00dd: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_00e2: ldc.i4.s 48
		IL_00e4: blt IL_018a

		IL_00e9: ldarg.0
		IL_00ea: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_00ef: ldc.i4.s 57
		IL_00f1: bgt IL_018a

		IL_00f6: ldarg.0
		IL_00f7: callvirt instance int32 [antlr.runtime]antlr.CharScanner::mark()
		IL_00fc: stloc.s 7
		IL_00fe: ldc.i4.1
		IL_00ff: stloc.s 6
		IL_0101: ldarg.0
		IL_0102: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_0107: dup
		IL_0108: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_010d: ldc.i4.1
		IL_010e: add
		IL_010f: stfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		.try
		{
			IL_0114: ldc.i4.0
			IL_0115: stloc.s 8
			// loop start (head: IL_0117)
				IL_0117: ldarg.0
				IL_0118: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_011d: ldc.i4.s 48
				IL_011f: blt.s IL_0134

				IL_0121: ldarg.0
				IL_0122: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_0127: ldc.i4.s 57
				IL_0129: bgt.s IL_0134

				IL_012b: ldarg.0
				IL_012c: ldc.i4.0
				IL_012d: call instance void ExpressionLexer::mDIGIT(bool)
				IL_0132: br.s IL_0157

				IL_0134: ldloc.s 8
				IL_0136: ldc.i4.1
				IL_0137: bge.s IL_015f

				IL_0139: ldarg.0
				IL_013a: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
				IL_013f: ldarg.0
				IL_0140: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
				IL_0145: ldarg.0
				IL_0146: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
				IL_014b: ldarg.0
				IL_014c: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
				IL_0151: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
				IL_0156: throw

				IL_0157: ldloc.s 8
				IL_0159: ldc.i4.1
				IL_015a: add
				IL_015b: stloc.s 8
				IL_015d: br.s IL_0117
			// end loop

			IL_015f: ldarg.0
			IL_0160: ldc.i4.s 46
			IL_0162: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
			IL_0167: leave.s IL_016f
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_0169: pop
			IL_016a: ldc.i4.0
			IL_016b: stloc.s 6
			IL_016d: leave.s IL_016f
		} // end handler

		IL_016f: ldarg.0
		IL_0170: ldloc.s 7
		IL_0172: callvirt instance void [antlr.runtime]antlr.CharScanner::rewind(int32)
		IL_0177: ldarg.0
		IL_0178: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_017d: dup
		IL_017e: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_0183: ldc.i4.1
		IL_0184: sub
		IL_0185: stfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing

		IL_018a: ldloc.s 6
		IL_018c: brfalse IL_0219

		IL_0191: ldc.i4.0
		IL_0192: stloc.s 9
		// loop start (head: IL_0194)
			IL_0194: ldarg.0
			IL_0195: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_019a: ldc.i4.s 48
			IL_019c: blt.s IL_01b1

			IL_019e: ldarg.0
			IL_019f: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_01a4: ldc.i4.s 57
			IL_01a6: bgt.s IL_01b1

			IL_01a8: ldarg.0
			IL_01a9: ldc.i4.0
			IL_01aa: call instance void ExpressionLexer::mDIGIT(bool)
			IL_01af: br.s IL_01d4

			IL_01b1: ldloc.s 9
			IL_01b3: ldc.i4.1
			IL_01b4: bge.s IL_01dc

			IL_01b6: ldarg.0
			IL_01b7: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_01bc: ldarg.0
			IL_01bd: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
			IL_01c2: ldarg.0
			IL_01c3: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
			IL_01c8: ldarg.0
			IL_01c9: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
			IL_01ce: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
			IL_01d3: throw

			IL_01d4: ldloc.s 9
			IL_01d6: ldc.i4.1
			IL_01d7: add
			IL_01d8: stloc.s 9
			IL_01da: br.s IL_0194
		// end loop

		IL_01dc: ldarg.0
		IL_01dd: ldc.i4.s 46
		IL_01df: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		// loop start (head: IL_01e4)
			IL_01e4: ldarg.0
			IL_01e5: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_01ea: ldc.i4.s 48
			IL_01ec: blt.s IL_0201

			IL_01ee: ldarg.0
			IL_01ef: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_01f4: ldc.i4.s 57
			IL_01f6: bgt.s IL_0201

			IL_01f8: ldarg.0
			IL_01f9: ldc.i4.0
			IL_01fa: call instance void ExpressionLexer::mDIGIT(bool)
			IL_01ff: br.s IL_01e4
		// end loop

		IL_0201: ldarg.0
		IL_0202: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_0207: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_020c: brtrue IL_02f7

		IL_0211: ldc.i4.s 20
		IL_0213: stloc.0
		IL_0214: br IL_02f7

		IL_0219: ldc.i4.0
		IL_021a: stloc.s 10
		IL_021c: ldarg.0
		IL_021d: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_0222: ldc.i4.s 48
		IL_0224: blt.s IL_0278

		IL_0226: ldarg.0
		IL_0227: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_022c: ldc.i4.s 57
		IL_022e: bgt.s IL_0278

		IL_0230: ldarg.0
		IL_0231: callvirt instance int32 [antlr.runtime]antlr.CharScanner::mark()
		IL_0236: stloc.s 11
		IL_0238: ldc.i4.1
		IL_0239: stloc.s 10
		IL_023b: ldarg.0
		IL_023c: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_0241: dup
		IL_0242: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_0247: ldc.i4.1
		IL_0248: add
		IL_0249: stfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		.try
		{
			IL_024e: ldarg.0
			IL_024f: ldc.i4.0
			IL_0250: call instance void ExpressionLexer::mDIGIT(bool)
			IL_0255: leave.s IL_025d
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_0257: pop
			IL_0258: ldc.i4.0
			IL_0259: stloc.s 10
			IL_025b: leave.s IL_025d
		} // end handler

		IL_025d: ldarg.0
		IL_025e: ldloc.s 11
		IL_0260: callvirt instance void [antlr.runtime]antlr.CharScanner::rewind(int32)
		IL_0265: ldarg.0
		IL_0266: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_026b: dup
		IL_026c: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_0271: ldc.i4.1
		IL_0272: sub
		IL_0273: stfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing

		IL_0278: ldloc.s 10
		IL_027a: brfalse.s IL_02d9

		IL_027c: ldc.i4.0
		IL_027d: stloc.s 12
		// loop start (head: IL_027f)
			IL_027f: ldarg.0
			IL_0280: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_0285: ldc.i4.s 48
			IL_0287: blt.s IL_029c

			IL_0289: ldarg.0
			IL_028a: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_028f: ldc.i4.s 57
			IL_0291: bgt.s IL_029c

			IL_0293: ldarg.0
			IL_0294: ldc.i4.0
			IL_0295: call instance void ExpressionLexer::mDIGIT(bool)
			IL_029a: br.s IL_02bf

			IL_029c: ldloc.s 12
			IL_029e: ldc.i4.1
			IL_029f: bge.s IL_02c7

			IL_02a1: ldarg.0
			IL_02a2: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_02a7: ldarg.0
			IL_02a8: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
			IL_02ad: ldarg.0
			IL_02ae: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
			IL_02b3: ldarg.0
			IL_02b4: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
			IL_02b9: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
			IL_02be: throw

			IL_02bf: ldloc.s 12
			IL_02c1: ldc.i4.1
			IL_02c2: add
			IL_02c3: stloc.s 12
			IL_02c5: br.s IL_027f
		// end loop

		IL_02c7: ldarg.0
		IL_02c8: ldfld class [antlr.runtime]antlr.LexerSharedInputState [antlr.runtime]antlr.CharScanner::inputState
		IL_02cd: ldfld int32 [antlr.runtime]antlr.LexerSharedInputState::guessing
		IL_02d2: brtrue.s IL_02f7

		IL_02d4: ldc.i4.s 19
		IL_02d6: stloc.0
		IL_02d7: br.s IL_02f7

		IL_02d9: ldarg.0
		IL_02da: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
		IL_02df: ldarg.0
		IL_02e0: callvirt instance string [antlr.runtime]antlr.CharScanner::getFilename()
		IL_02e5: ldarg.0
		IL_02e6: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getLine()
		IL_02eb: ldarg.0
		IL_02ec: callvirt instance int32 [antlr.runtime]antlr.CharScanner::getColumn()
		IL_02f1: newobj instance void [antlr.runtime]antlr.NoViableAltForCharException::.ctor(char, string, int32, int32)
		IL_02f6: throw

		IL_02f7: ldarg.1
		IL_02f8: brfalse.s IL_032c

		IL_02fa: ldloc.1
		IL_02fb: brtrue.s IL_032c

		IL_02fd: ldloc.0
		IL_02fe: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0303: beq.s IL_032c

		IL_0305: ldarg.0
		IL_0306: ldloc.0
		IL_0307: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_030c: stloc.1
		IL_030d: ldloc.1
		IL_030e: ldarg.0
		IL_030f: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0314: ldloc.2
		IL_0315: ldarg.0
		IL_0316: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_031b: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0320: ldloc.2
		IL_0321: sub
		IL_0322: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0327: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_032c: ldarg.0
		IL_032d: ldloc.1
		IL_032e: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_0333: ret
	} // end of method ExpressionLexer::mNUMBER

	.method public hidebysig 
		instance void mSTRING (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x3220
		// Header size: 12
		// Code size: 125 (0x7d)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 21
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 34
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		// loop start (head: IL_0019)
			IL_0019: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionLexer::tokenSet_0_
			IL_001e: ldarg.0
			IL_001f: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_0024: callvirt instance bool [antlr.runtime]antlr.collections.impl.BitSet::member(int32)
			IL_0029: brfalse.s IL_0038

			IL_002b: ldarg.0
			IL_002c: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionLexer::tokenSet_0_
			IL_0031: callvirt instance void [antlr.runtime]antlr.CharScanner::match(class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_0036: br.s IL_0019
		// end loop

		IL_0038: ldarg.0
		IL_0039: ldc.i4.s 34
		IL_003b: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0040: ldarg.1
		IL_0041: brfalse.s IL_0075

		IL_0043: ldloc.1
		IL_0044: brtrue.s IL_0075

		IL_0046: ldloc.0
		IL_0047: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_004c: beq.s IL_0075

		IL_004e: ldarg.0
		IL_004f: ldloc.0
		IL_0050: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0055: stloc.1
		IL_0056: ldloc.1
		IL_0057: ldarg.0
		IL_0058: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_005d: ldloc.2
		IL_005e: ldarg.0
		IL_005f: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0064: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0069: ldloc.2
		IL_006a: sub
		IL_006b: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0070: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0075: ldarg.0
		IL_0076: ldloc.1
		IL_0077: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_007c: ret
	} // end of method ExpressionLexer::mSTRING

	.method public hidebysig 
		instance void mSTRING2 (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x32ac
		// Header size: 12
		// Code size: 125 (0x7d)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 22
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 39
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		// loop start (head: IL_0019)
			IL_0019: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionLexer::tokenSet_1_
			IL_001e: ldarg.0
			IL_001f: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_0024: callvirt instance bool [antlr.runtime]antlr.collections.impl.BitSet::member(int32)
			IL_0029: brfalse.s IL_0038

			IL_002b: ldarg.0
			IL_002c: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionLexer::tokenSet_1_
			IL_0031: callvirt instance void [antlr.runtime]antlr.CharScanner::match(class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_0036: br.s IL_0019
		// end loop

		IL_0038: ldarg.0
		IL_0039: ldc.i4.s 39
		IL_003b: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0040: ldarg.1
		IL_0041: brfalse.s IL_0075

		IL_0043: ldloc.1
		IL_0044: brtrue.s IL_0075

		IL_0046: ldloc.0
		IL_0047: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_004c: beq.s IL_0075

		IL_004e: ldarg.0
		IL_004f: ldloc.0
		IL_0050: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_0055: stloc.1
		IL_0056: ldloc.1
		IL_0057: ldarg.0
		IL_0058: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_005d: ldloc.2
		IL_005e: ldarg.0
		IL_005f: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0064: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_0069: ldloc.2
		IL_006a: sub
		IL_006b: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_0070: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_0075: ldarg.0
		IL_0076: ldloc.1
		IL_0077: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_007c: ret
	} // end of method ExpressionLexer::mSTRING2

	.method public hidebysig 
		instance void mDEFINE (
			bool _createToken
		) cil managed 
	{
		// Method begins at RVA 0x3338
		// Header size: 12
		// Code size: 435 (0x1b3)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class [antlr.runtime]antlr.IToken,
			[2] int32,
			[3] char
		)

		IL_0000: ldnull
		IL_0001: stloc.1
		IL_0002: ldarg.0
		IL_0003: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0008: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_000d: stloc.2
		IL_000e: ldc.i4.s 23
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldc.i4.s 36
		IL_0014: callvirt instance void [antlr.runtime]antlr.CharScanner::match(char)
		IL_0019: ldarg.0
		IL_001a: ldc.i4.0
		IL_001b: call instance void ExpressionLexer::mLETTER(bool)
		// loop start (head: IL_0020)
			IL_0020: ldarg.0
			IL_0021: ldfld char [antlr.runtime]antlr.CharScanner::cached_LA1
			IL_0026: stloc.3
			IL_0027: ldloc.3
			IL_0028: ldc.i4.s 48
			IL_002a: sub
			IL_002b: switch (IL_016a, IL_016a, IL_016a, IL_016a, IL_016a, IL_016a, IL_016a, IL_016a, IL_016a, IL_016a, IL_0176, IL_0176, IL_0176, IL_0176, IL_0176, IL_0176, IL_0176, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_0176, IL_0176, IL_0176, IL_0176, IL_015e, IL_0176, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e, IL_015e)

			IL_015c: br.s IL_0176

			IL_015e: ldarg.0
			IL_015f: ldc.i4.0
			IL_0160: call instance void ExpressionLexer::mLETTER(bool)
			IL_0165: br IL_0020

			IL_016a: ldarg.0
			IL_016b: ldc.i4.0
			IL_016c: call instance void ExpressionLexer::mDIGIT(bool)
			IL_0171: br IL_0020
		// end loop

		IL_0176: ldarg.1
		IL_0177: brfalse.s IL_01ab

		IL_0179: ldloc.1
		IL_017a: brtrue.s IL_01ab

		IL_017c: ldloc.0
		IL_017d: ldsfld int32 [antlr.runtime]antlr.Token::SKIP
		IL_0182: beq.s IL_01ab

		IL_0184: ldarg.0
		IL_0185: ldloc.0
		IL_0186: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::makeToken(int32)
		IL_018b: stloc.1
		IL_018c: ldloc.1
		IL_018d: ldarg.0
		IL_018e: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_0193: ldloc.2
		IL_0194: ldarg.0
		IL_0195: ldfld class [mscorlib]System.Text.StringBuilder [antlr.runtime]antlr.CharScanner::text
		IL_019a: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
		IL_019f: ldloc.2
		IL_01a0: sub
		IL_01a1: callvirt instance string [mscorlib]System.Text.StringBuilder::ToString(int32, int32)
		IL_01a6: callvirt instance void [antlr.runtime]antlr.IToken::setText(string)

		IL_01ab: ldarg.0
		IL_01ac: ldloc.1
		IL_01ad: stfld class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.CharScanner::returnToken_
		IL_01b2: ret
	} // end of method ExpressionLexer::mDEFINE

	.method private hidebysig static 
		int64[] mk_tokenSet_0_ () cil managed 
	{
		// Method begins at RVA 0x34f8
		// Header size: 12
		// Code size: 28 (0x1c)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.4
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i8 -17179869185
		IL_0012: stelem.i8
		IL_0013: ldloc.1
		IL_0014: ldc.i4.1
		IL_0015: ldc.i4.m1
		IL_0016: conv.i8
		IL_0017: stelem.i8
		IL_0018: ldloc.1
		IL_0019: stloc.0
		IL_001a: ldloc.0
		IL_001b: ret
	} // end of method ExpressionLexer::mk_tokenSet_0_

	.method private hidebysig static 
		int64[] mk_tokenSet_1_ () cil managed 
	{
		// Method begins at RVA 0x3520
		// Header size: 12
		// Code size: 28 (0x1c)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.4
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i8 -549755813889
		IL_0012: stelem.i8
		IL_0013: ldloc.1
		IL_0014: ldc.i4.1
		IL_0015: ldc.i4.m1
		IL_0016: conv.i8
		IL_0017: stelem.i8
		IL_0018: ldloc.1
		IL_0019: stloc.0
		IL_001a: ldloc.0
		IL_001b: ret
	} // end of method ExpressionLexer::mk_tokenSet_1_

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x3548
		// Header size: 1
		// Code size: 31 (0x1f)
		.maxstack 8

		IL_0000: call int64[] ExpressionLexer::mk_tokenSet_0_()
		IL_0005: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_000a: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionLexer::tokenSet_0_
		IL_000f: call int64[] ExpressionLexer::mk_tokenSet_1_()
		IL_0014: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_0019: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionLexer::tokenSet_1_
		IL_001e: ret
	} // end of method ExpressionLexer::.cctor

} // end of class ExpressionLexer

.class public auto ansi beforefieldinit ExpressionParser
	extends [antlr.runtime]antlr.LLkParser
{
	// Fields
	.field public static literal int32 EOF = int32(1)
	.field public static literal int32 NULL_TREE_LOOKAHEAD = int32(3)
	.field public static literal int32 EQ = int32(4)
	.field public static literal int32 OR = int32(5)
	.field public static literal int32 AND = int32(6)
	.field public static literal int32 NE = int32(7)
	.field public static literal int32 LS = int32(8)
	.field public static literal int32 GT = int32(9)
	.field public static literal int32 LE = int32(10)
	.field public static literal int32 GE = int32(11)
	.field public static literal int32 PLUS = int32(12)
	.field public static literal int32 MINUS = int32(13)
	.field public static literal int32 MUL = int32(14)
	.field public static literal int32 DIV = int32(15)
	.field public static literal int32 MOD = int32(16)
	.field public static literal int32 LPAREN = int32(17)
	.field public static literal int32 RPAREN = int32(18)
	.field public static literal int32 INT = int32(19)
	.field public static literal int32 FLOAT = int32(20)
	.field public static literal int32 STRING = int32(21)
	.field public static literal int32 STRING2 = int32(22)
	.field public static literal int32 DEFINE = int32(23)
	.field public static literal int32 ROUND = int32(24)
	.field public static literal int32 WS = int32(25)
	.field public static literal int32 DIGIT = int32(26)
	.field public static literal int32 LETTER = int32(27)
	.field public static literal int32 NUMBER = int32(28)
	.field private class BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator _Eval
	.field public static initonly string[] tokenNames_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_0_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_1_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_2_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_3_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_4_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_5_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_6_
	.field public static initonly class [antlr.runtime]antlr.collections.impl.BitSet tokenSet_7_

	// Methods
	.method public hidebysig 
		instance void SetEvaluator (
			class BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator e
		) cil managed 
	{
		// Method begins at RVA 0x3568
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld class BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator ExpressionParser::_Eval
		IL_0007: ret
	} // end of method ExpressionParser::SetEvaluator

	.method public hidebysig virtual 
		instance void reportError (
			class [antlr.runtime]antlr.RecognitionException ex
		) cil managed 
	{
		// Method begins at RVA 0x3574
		// Header size: 12
		// Code size: 30 (0x1e)
		.maxstack 5
		.locals init (
			[0] object[]
		)

		IL_0000: ldc.i4.s 18
		IL_0002: ldstr "Expression error: {0}"
		IL_0007: ldc.i4.1
		IL_0008: newarr [mscorlib]System.Object
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: ldc.i4.0
		IL_0010: ldarg.1
		IL_0011: callvirt instance string [mscorlib]System.Exception::get_Message()
		IL_0016: stelem.ref
		IL_0017: ldloc.0
		IL_0018: newobj instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.BinaryAssetBuilderException::.ctor(valuetype [BinaryAssetBuilder.Core]BinaryAssetBuilder.ErrorCode, string, object[])
		IL_001d: throw
	} // end of method ExpressionParser::reportError

	.method family hidebysig 
		instance void initialize () cil managed 
	{
		// Method begins at RVA 0x359e
		// Header size: 1
		// Code size: 12 (0xc)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldsfld string[] ExpressionParser::tokenNames_
		IL_0006: stfld string[] [antlr.runtime]antlr.Parser::tokenNames
		IL_000b: ret
	} // end of method ExpressionParser::initialize

	.method family hidebysig specialname rtspecialname 
		instance void .ctor (
			class [antlr.runtime]antlr.TokenBuffer tokenBuf,
			int32 k
		) cil managed 
	{
		// Method begins at RVA 0x35ab
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.2
		IL_0003: call instance void [antlr.runtime]antlr.LLkParser::.ctor(class [antlr.runtime]antlr.TokenBuffer, int32)
		IL_0008: ldarg.0
		IL_0009: call instance void ExpressionParser::initialize()
		IL_000e: ret
	} // end of method ExpressionParser::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [antlr.runtime]antlr.TokenBuffer tokenBuf
		) cil managed 
	{
		// Method begins at RVA 0x35bb
		// Header size: 1
		// Code size: 9 (0x9)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldc.i4.1
		IL_0003: call instance void ExpressionParser::.ctor(class [antlr.runtime]antlr.TokenBuffer, int32)
		IL_0008: ret
	} // end of method ExpressionParser::.ctor

	.method family hidebysig specialname rtspecialname 
		instance void .ctor (
			class [antlr.runtime]antlr.TokenStream lexer,
			int32 k
		) cil managed 
	{
		// Method begins at RVA 0x35c5
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.2
		IL_0003: call instance void [antlr.runtime]antlr.LLkParser::.ctor(class [antlr.runtime]antlr.TokenStream, int32)
		IL_0008: ldarg.0
		IL_0009: call instance void ExpressionParser::initialize()
		IL_000e: ret
	} // end of method ExpressionParser::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [antlr.runtime]antlr.TokenStream lexer
		) cil managed 
	{
		// Method begins at RVA 0x35d5
		// Header size: 1
		// Code size: 9 (0x9)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldc.i4.1
		IL_0003: call instance void ExpressionParser::.ctor(class [antlr.runtime]antlr.TokenStream, int32)
		IL_0008: ret
	} // end of method ExpressionParser::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [antlr.runtime]antlr.ParserSharedInputState state
		) cil managed 
	{
		// Method begins at RVA 0x35df
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldc.i4.1
		IL_0003: call instance void [antlr.runtime]antlr.LLkParser::.ctor(class [antlr.runtime]antlr.ParserSharedInputState, int32)
		IL_0008: ldarg.0
		IL_0009: call instance void ExpressionParser::initialize()
		IL_000e: ret
	} // end of method ExpressionParser::.ctor

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue expr () cil managed 
	{
		// Method begins at RVA 0x35f0
		// Header size: 12
		// Code size: 49 (0x31)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class [antlr.runtime]antlr.RecognitionException
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: ldc.i4.4
			IL_0004: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0009: ldarg.0
			IL_000a: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::orExpr()
			IL_000f: stloc.0
			IL_0010: ldarg.0
			IL_0011: ldc.i4.1
			IL_0012: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0017: leave.s IL_002f
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_0019: stloc.1
			IL_001a: ldarg.0
			IL_001b: ldloc.1
			IL_001c: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_0021: ldarg.0
			IL_0022: ldloc.1
			IL_0023: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_0_
			IL_0028: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_002d: leave.s IL_002f
		} // end handler

		IL_002f: ldloc.0
		IL_0030: ret
	} // end of method ExpressionParser::expr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue orExpr () cil managed 
	{
		// Method begins at RVA 0x3640
		// Header size: 12
		// Code size: 68 (0x44)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[2] class [antlr.runtime]antlr.RecognitionException
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::andExpr()
			IL_0008: stloc.0
			// loop start (head: IL_0009)
				IL_0009: ldarg.0
				IL_000a: ldc.i4.1
				IL_000b: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
				IL_0010: ldc.i4.5
				IL_0011: bne.un.s IL_002a

				IL_0013: ldarg.0
				IL_0014: ldc.i4.5
				IL_0015: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_001a: ldarg.0
				IL_001b: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::andExpr()
				IL_0020: stloc.1
				IL_0021: ldloc.0
				IL_0022: ldloc.1
				IL_0023: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::LogicalOr(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0028: br.s IL_0009
			// end loop

			IL_002a: leave.s IL_0042
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_002c: stloc.2
			IL_002d: ldarg.0
			IL_002e: ldloc.2
			IL_002f: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_0034: ldarg.0
			IL_0035: ldloc.2
			IL_0036: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_1_
			IL_003b: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_0040: leave.s IL_0042
		} // end handler

		IL_0042: ldloc.0
		IL_0043: ret
	} // end of method ExpressionParser::orExpr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue andExpr () cil managed 
	{
		// Method begins at RVA 0x36a0
		// Header size: 12
		// Code size: 68 (0x44)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[2] class [antlr.runtime]antlr.RecognitionException
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::eqExpr()
			IL_0008: stloc.0
			// loop start (head: IL_0009)
				IL_0009: ldarg.0
				IL_000a: ldc.i4.1
				IL_000b: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
				IL_0010: ldc.i4.6
				IL_0011: bne.un.s IL_002a

				IL_0013: ldarg.0
				IL_0014: ldc.i4.6
				IL_0015: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_001a: ldarg.0
				IL_001b: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::eqExpr()
				IL_0020: stloc.1
				IL_0021: ldloc.0
				IL_0022: ldloc.1
				IL_0023: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::LogicalAnd(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0028: br.s IL_0009
			// end loop

			IL_002a: leave.s IL_0042
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_002c: stloc.2
			IL_002d: ldarg.0
			IL_002e: ldloc.2
			IL_002f: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_0034: ldarg.0
			IL_0035: ldloc.2
			IL_0036: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_2_
			IL_003b: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_0040: leave.s IL_0042
		} // end handler

		IL_0042: ldloc.0
		IL_0043: ret
	} // end of method ExpressionParser::andExpr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue eqExpr () cil managed 
	{
		// Method begins at RVA 0x3700
		// Header size: 12
		// Code size: 105 (0x69)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[2] class [antlr.runtime]antlr.RecognitionException,
			[3] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::relExpr()
			IL_0008: stloc.0
			// loop start (head: IL_0009)
				IL_0009: ldarg.0
				IL_000a: ldc.i4.1
				IL_000b: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
				IL_0010: stloc.3
				IL_0011: ldloc.3
				IL_0012: ldc.i4.4
				IL_0013: beq.s IL_001b

				IL_0015: ldloc.3
				IL_0016: ldc.i4.7
				IL_0017: beq.s IL_0032

				IL_0019: br.s IL_004f

				IL_001b: ldarg.0
				IL_001c: ldc.i4.4
				IL_001d: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0022: ldarg.0
				IL_0023: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::relExpr()
				IL_0028: stloc.1
				IL_0029: ldloc.0
				IL_002a: ldloc.1
				IL_002b: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::IsEqual(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0030: br.s IL_0009

				IL_0032: ldarg.0
				IL_0033: ldc.i4.7
				IL_0034: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0039: ldarg.0
				IL_003a: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::relExpr()
				IL_003f: stloc.1
				IL_0040: ldloc.0
				IL_0041: ldloc.1
				IL_0042: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::IsEqual(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0047: ldloc.0
				IL_0048: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Not()
				IL_004d: br.s IL_0009
			// end loop

			IL_004f: leave.s IL_0067
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_0051: stloc.2
			IL_0052: ldarg.0
			IL_0053: ldloc.2
			IL_0054: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_0059: ldarg.0
			IL_005a: ldloc.2
			IL_005b: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_3_
			IL_0060: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_0065: leave.s IL_0067
		} // end handler

		IL_0067: ldloc.0
		IL_0068: ret
	} // end of method ExpressionParser::eqExpr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue relExpr () cil managed 
	{
		// Method begins at RVA 0x3788
		// Header size: 12
		// Code size: 179 (0xb3)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[2] class [antlr.runtime]antlr.RecognitionException,
			[3] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::addExpr()
			IL_0008: stloc.0
			// loop start (head: IL_0009)
				IL_0009: ldarg.0
				IL_000a: ldc.i4.1
				IL_000b: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
				IL_0010: stloc.3
				IL_0011: ldloc.3
				IL_0012: ldc.i4.8
				IL_0013: sub
				IL_0014: switch (IL_002b, IL_0042, IL_005a, IL_0078)

				IL_0029: br.s IL_0099

				IL_002b: ldarg.0
				IL_002c: ldc.i4.8
				IL_002d: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0032: ldarg.0
				IL_0033: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::addExpr()
				IL_0038: stloc.1
				IL_0039: ldloc.0
				IL_003a: ldloc.1
				IL_003b: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::IsLessThan(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0040: br.s IL_0009

				IL_0042: ldarg.0
				IL_0043: ldc.i4.s 9
				IL_0045: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_004a: ldarg.0
				IL_004b: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::addExpr()
				IL_0050: stloc.1
				IL_0051: ldloc.0
				IL_0052: ldloc.1
				IL_0053: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::IsGreaterThan(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0058: br.s IL_0009

				IL_005a: ldarg.0
				IL_005b: ldc.i4.s 10
				IL_005d: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0062: ldarg.0
				IL_0063: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::addExpr()
				IL_0068: stloc.1
				IL_0069: ldloc.0
				IL_006a: ldloc.1
				IL_006b: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::IsGreaterThan(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0070: ldloc.0
				IL_0071: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Not()
				IL_0076: br.s IL_0009

				IL_0078: ldarg.0
				IL_0079: ldc.i4.s 11
				IL_007b: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0080: ldarg.0
				IL_0081: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::addExpr()
				IL_0086: stloc.1
				IL_0087: ldloc.0
				IL_0088: ldloc.1
				IL_0089: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::IsLessThan(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_008e: ldloc.0
				IL_008f: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Not()
				IL_0094: br IL_0009
			// end loop

			IL_0099: leave.s IL_00b1
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_009b: stloc.2
			IL_009c: ldarg.0
			IL_009d: ldloc.2
			IL_009e: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_00a3: ldarg.0
			IL_00a4: ldloc.2
			IL_00a5: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_4_
			IL_00aa: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_00af: leave.s IL_00b1
		} // end handler

		IL_00b1: ldloc.0
		IL_00b2: ret
	} // end of method ExpressionParser::relExpr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue addExpr () cil managed 
	{
		// Method begins at RVA 0x3858
		// Header size: 12
		// Code size: 110 (0x6e)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[2] class [antlr.runtime]antlr.RecognitionException,
			[3] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::mulExpr()
			IL_0008: stloc.0
			// loop start (head: IL_0009)
				IL_0009: ldarg.0
				IL_000a: ldc.i4.1
				IL_000b: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
				IL_0010: stloc.3
				IL_0011: ldloc.3
				IL_0012: ldc.i4.s 12
				IL_0014: sub
				IL_0015: switch (IL_0024, IL_003c)

				IL_0022: br.s IL_0054

				IL_0024: ldarg.0
				IL_0025: ldc.i4.s 12
				IL_0027: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_002c: ldarg.0
				IL_002d: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::mulExpr()
				IL_0032: stloc.1
				IL_0033: ldloc.0
				IL_0034: ldloc.1
				IL_0035: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Add(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_003a: br.s IL_0009

				IL_003c: ldarg.0
				IL_003d: ldc.i4.s 13
				IL_003f: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0044: ldarg.0
				IL_0045: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::mulExpr()
				IL_004a: stloc.1
				IL_004b: ldloc.0
				IL_004c: ldloc.1
				IL_004d: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Subtract(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0052: br.s IL_0009
			// end loop

			IL_0054: leave.s IL_006c
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_0056: stloc.2
			IL_0057: ldarg.0
			IL_0058: ldloc.2
			IL_0059: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_005e: ldarg.0
			IL_005f: ldloc.2
			IL_0060: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_5_
			IL_0065: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_006a: leave.s IL_006c
		} // end handler

		IL_006c: ldloc.0
		IL_006d: ret
	} // end of method ExpressionParser::addExpr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue mulExpr () cil managed 
	{
		// Method begins at RVA 0x38e4
		// Header size: 12
		// Code size: 138 (0x8a)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[2] class [antlr.runtime]antlr.RecognitionException,
			[3] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::unaryExpr()
			IL_0008: stloc.0
			// loop start (head: IL_0009)
				IL_0009: ldarg.0
				IL_000a: ldc.i4.1
				IL_000b: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
				IL_0010: stloc.3
				IL_0011: ldloc.3
				IL_0012: ldc.i4.s 14
				IL_0014: sub
				IL_0015: switch (IL_0028, IL_0040, IL_0058)

				IL_0026: br.s IL_0070

				IL_0028: ldarg.0
				IL_0029: ldc.i4.s 14
				IL_002b: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0030: ldarg.0
				IL_0031: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::unaryExpr()
				IL_0036: stloc.1
				IL_0037: ldloc.0
				IL_0038: ldloc.1
				IL_0039: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Multiply(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_003e: br.s IL_0009

				IL_0040: ldarg.0
				IL_0041: ldc.i4.s 15
				IL_0043: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0048: ldarg.0
				IL_0049: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::unaryExpr()
				IL_004e: stloc.1
				IL_004f: ldloc.0
				IL_0050: ldloc.1
				IL_0051: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Divide(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_0056: br.s IL_0009

				IL_0058: ldarg.0
				IL_0059: ldc.i4.s 16
				IL_005b: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
				IL_0060: ldarg.0
				IL_0061: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::unaryExpr()
				IL_0066: stloc.1
				IL_0067: ldloc.0
				IL_0068: ldloc.1
				IL_0069: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Modulo(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
				IL_006e: br.s IL_0009
			// end loop

			IL_0070: leave.s IL_0088
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_0072: stloc.2
			IL_0073: ldarg.0
			IL_0074: ldloc.2
			IL_0075: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_007a: ldarg.0
			IL_007b: ldloc.2
			IL_007c: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_6_
			IL_0081: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_0086: leave.s IL_0088
		} // end handler

		IL_0088: ldloc.0
		IL_0089: ret
	} // end of method ExpressionParser::mulExpr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue unaryExpr () cil managed 
	{
		// Method begins at RVA 0x398c
		// Header size: 12
		// Code size: 167 (0xa7)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class [antlr.runtime]antlr.RecognitionException,
			[2] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: ldc.i4.1
			IL_0004: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
			IL_0009: stloc.2
			IL_000a: ldloc.2
			IL_000b: ldc.i4.s 12
			IL_000d: sub
			IL_000e: switch (IL_0049, IL_005a, IL_007a, IL_007a, IL_007a, IL_0071, IL_007a, IL_0071, IL_0071, IL_0071, IL_0071, IL_0071, IL_0071)

			IL_0047: br.s IL_007a

			IL_0049: ldarg.0
			IL_004a: ldc.i4.s 12
			IL_004c: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0051: ldarg.0
			IL_0052: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::unaryExpr()
			IL_0057: stloc.0
			IL_0058: br.s IL_008d

			IL_005a: ldarg.0
			IL_005b: ldc.i4.s 13
			IL_005d: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0062: ldarg.0
			IL_0063: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::unaryExpr()
			IL_0068: stloc.0
			IL_0069: ldloc.0
			IL_006a: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::Negate()
			IL_006f: br.s IL_008d

			IL_0071: ldarg.0
			IL_0072: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::singleValue()
			IL_0077: stloc.0
			IL_0078: br.s IL_008d

			IL_007a: ldarg.0
			IL_007b: ldc.i4.1
			IL_007c: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.Parser::LT(int32)
			IL_0081: ldarg.0
			IL_0082: callvirt instance string [antlr.runtime]antlr.Parser::getFilename()
			IL_0087: newobj instance void [antlr.runtime]antlr.NoViableAltException::.ctor(class [antlr.runtime]antlr.IToken, string)
			IL_008c: throw

			IL_008d: leave.s IL_00a5
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_008f: stloc.1
			IL_0090: ldarg.0
			IL_0091: ldloc.1
			IL_0092: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_0097: ldarg.0
			IL_0098: ldloc.1
			IL_0099: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_7_
			IL_009e: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_00a3: leave.s IL_00a5
		} // end handler

		IL_00a5: ldloc.0
		IL_00a6: ret
	} // end of method ExpressionParser::unaryExpr

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue singleValue () cil managed 
	{
		// Method begins at RVA 0x3a50
		// Header size: 12
		// Code size: 326 (0x146)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class [antlr.runtime]antlr.IToken,
			[2] class [antlr.runtime]antlr.IToken,
			[3] class [antlr.runtime]antlr.IToken,
			[4] class [antlr.runtime]antlr.IToken,
			[5] class [antlr.runtime]antlr.IToken,
			[6] class [antlr.runtime]antlr.RecognitionException,
			[7] int32
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		IL_0002: ldnull
		IL_0003: stloc.1
		IL_0004: ldnull
		IL_0005: stloc.2
		IL_0006: ldnull
		IL_0007: stloc.3
		IL_0008: ldnull
		IL_0009: stloc.s 4
		IL_000b: ldnull
		IL_000c: stloc.s 5
		.try
		{
			IL_000e: ldarg.0
			IL_000f: ldc.i4.1
			IL_0010: callvirt instance int32 [antlr.runtime]antlr.Parser::LA(int32)
			IL_0015: stloc.s 7
			IL_0017: ldloc.s 7
			IL_0019: ldc.i4.s 17
			IL_001b: sub
			IL_001c: switch (IL_0046, IL_0116, IL_0062, IL_0083, IL_00a4, IL_00c2, IL_00e2, IL_010d)

			IL_0041: br IL_0116

			IL_0046: ldarg.0
			IL_0047: ldc.i4.s 17
			IL_0049: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_004e: ldarg.0
			IL_004f: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::orExpr()
			IL_0054: stloc.0
			IL_0055: ldarg.0
			IL_0056: ldc.i4.s 18
			IL_0058: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_005d: br IL_0129

			IL_0062: ldarg.0
			IL_0063: ldc.i4.1
			IL_0064: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.Parser::LT(int32)
			IL_0069: stloc.1
			IL_006a: ldarg.0
			IL_006b: ldc.i4.s 19
			IL_006d: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0072: ldloc.1
			IL_0073: callvirt instance string [antlr.runtime]antlr.IToken::getText()
			IL_0078: call class BinaryAssetBuilder.ExpressionEval.ExpressionValue BinaryAssetBuilder.ExpressionEval.ExpressionValue::NewInteger(string)
			IL_007d: stloc.0
			IL_007e: br IL_0129

			IL_0083: ldarg.0
			IL_0084: ldc.i4.1
			IL_0085: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.Parser::LT(int32)
			IL_008a: stloc.2
			IL_008b: ldarg.0
			IL_008c: ldc.i4.s 20
			IL_008e: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0093: ldloc.2
			IL_0094: callvirt instance string [antlr.runtime]antlr.IToken::getText()
			IL_0099: call class BinaryAssetBuilder.ExpressionEval.ExpressionValue BinaryAssetBuilder.ExpressionEval.ExpressionValue::NewFloat(string)
			IL_009e: stloc.0
			IL_009f: br IL_0129

			IL_00a4: ldarg.0
			IL_00a5: ldc.i4.1
			IL_00a6: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.Parser::LT(int32)
			IL_00ab: stloc.3
			IL_00ac: ldarg.0
			IL_00ad: ldc.i4.s 21
			IL_00af: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_00b4: ldloc.3
			IL_00b5: callvirt instance string [antlr.runtime]antlr.IToken::getText()
			IL_00ba: call class BinaryAssetBuilder.ExpressionEval.ExpressionValue BinaryAssetBuilder.ExpressionEval.ExpressionValue::NewQuotedString(string)
			IL_00bf: stloc.0
			IL_00c0: br.s IL_0129

			IL_00c2: ldarg.0
			IL_00c3: ldc.i4.1
			IL_00c4: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.Parser::LT(int32)
			IL_00c9: stloc.s 4
			IL_00cb: ldarg.0
			IL_00cc: ldc.i4.s 22
			IL_00ce: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_00d3: ldloc.s 4
			IL_00d5: callvirt instance string [antlr.runtime]antlr.IToken::getText()
			IL_00da: call class BinaryAssetBuilder.ExpressionEval.ExpressionValue BinaryAssetBuilder.ExpressionEval.ExpressionValue::NewQuotedString(string)
			IL_00df: stloc.0
			IL_00e0: br.s IL_0129

			IL_00e2: ldarg.0
			IL_00e3: ldc.i4.1
			IL_00e4: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.Parser::LT(int32)
			IL_00e9: stloc.s 5
			IL_00eb: ldarg.0
			IL_00ec: ldc.i4.s 23
			IL_00ee: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_00f3: ldarg.0
			IL_00f4: ldfld class BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator ExpressionParser::_Eval
			IL_00f9: ldloc.s 5
			IL_00fb: callvirt instance string [antlr.runtime]antlr.IToken::getText()
			IL_0100: callvirt instance string BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator::LookupDefine(string)
			IL_0105: call class BinaryAssetBuilder.ExpressionEval.ExpressionValue BinaryAssetBuilder.ExpressionEval.ExpressionValue::NewFromLookup(string)
			IL_010a: stloc.0
			IL_010b: br.s IL_0129

			IL_010d: ldarg.0
			IL_010e: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::regularFunction()
			IL_0113: stloc.0
			IL_0114: br.s IL_0129

			IL_0116: ldarg.0
			IL_0117: ldc.i4.1
			IL_0118: callvirt instance class [antlr.runtime]antlr.IToken [antlr.runtime]antlr.Parser::LT(int32)
			IL_011d: ldarg.0
			IL_011e: callvirt instance string [antlr.runtime]antlr.Parser::getFilename()
			IL_0123: newobj instance void [antlr.runtime]antlr.NoViableAltException::.ctor(class [antlr.runtime]antlr.IToken, string)
			IL_0128: throw

			IL_0129: leave.s IL_0144
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_012b: stloc.s 6
			IL_012d: ldarg.0
			IL_012e: ldloc.s 6
			IL_0130: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_0135: ldarg.0
			IL_0136: ldloc.s 6
			IL_0138: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_7_
			IL_013d: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_0142: leave.s IL_0144
		} // end handler

		IL_0144: ldloc.0
		IL_0145: ret
	} // end of method ExpressionParser::singleValue

	.method public hidebysig 
		instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue regularFunction () cil managed 
	{
		// Method begins at RVA 0x3bc0
		// Header size: 12
		// Code size: 65 (0x41)
		.maxstack 3
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] class [antlr.runtime]antlr.RecognitionException
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldarg.0
			IL_0003: ldc.i4.s 24
			IL_0005: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_000a: ldarg.0
			IL_000b: ldc.i4.s 17
			IL_000d: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0012: ldarg.0
			IL_0013: call instance class BinaryAssetBuilder.ExpressionEval.ExpressionValue ExpressionParser::orExpr()
			IL_0018: stloc.0
			IL_0019: ldarg.0
			IL_001a: ldc.i4.s 18
			IL_001c: callvirt instance void [antlr.runtime]antlr.Parser::match(int32)
			IL_0021: ldloc.0
			IL_0022: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::FuncRound()
			IL_0027: leave.s IL_003f
		} // end .try
		catch [antlr.runtime]antlr.RecognitionException
		{
			IL_0029: stloc.1
			IL_002a: ldarg.0
			IL_002b: ldloc.1
			IL_002c: callvirt instance void [antlr.runtime]antlr.Parser::reportError(class [antlr.runtime]antlr.RecognitionException)
			IL_0031: ldarg.0
			IL_0032: ldloc.1
			IL_0033: ldsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_7_
			IL_0038: callvirt instance void [antlr.runtime]antlr.Parser::recover(class [antlr.runtime]antlr.RecognitionException, class [antlr.runtime]antlr.collections.impl.BitSet)
			IL_003d: leave.s IL_003f
		} // end handler

		IL_003f: ldloc.0
		IL_0040: ret
	} // end of method ExpressionParser::regularFunction

	.method private hidebysig 
		instance void initializeFactory () cil managed 
	{
		// Method begins at RVA 0x3c20
		// Header size: 1
		// Code size: 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method ExpressionParser::initializeFactory

	.method private hidebysig static 
		int64[] mk_tokenSet_0_ () cil managed 
	{
		// Method begins at RVA 0x3c24
		// Header size: 12
		// Code size: 16 (0x10)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4.2
		IL_000a: conv.i8
		IL_000b: stelem.i8
		IL_000c: ldloc.1
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: ret
	} // end of method ExpressionParser::mk_tokenSet_0_

	.method private hidebysig static 
		int64[] mk_tokenSet_1_ () cil managed 
	{
		// Method begins at RVA 0x3c40
		// Header size: 12
		// Code size: 20 (0x14)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4 262146
		IL_000e: conv.i8
		IL_000f: stelem.i8
		IL_0010: ldloc.1
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ret
	} // end of method ExpressionParser::mk_tokenSet_1_

	.method private hidebysig static 
		int64[] mk_tokenSet_2_ () cil managed 
	{
		// Method begins at RVA 0x3c60
		// Header size: 12
		// Code size: 20 (0x14)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4 262178
		IL_000e: conv.i8
		IL_000f: stelem.i8
		IL_0010: ldloc.1
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ret
	} // end of method ExpressionParser::mk_tokenSet_2_

	.method private hidebysig static 
		int64[] mk_tokenSet_3_ () cil managed 
	{
		// Method begins at RVA 0x3c80
		// Header size: 12
		// Code size: 20 (0x14)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4 262242
		IL_000e: conv.i8
		IL_000f: stelem.i8
		IL_0010: ldloc.1
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ret
	} // end of method ExpressionParser::mk_tokenSet_3_

	.method private hidebysig static 
		int64[] mk_tokenSet_4_ () cil managed 
	{
		// Method begins at RVA 0x3ca0
		// Header size: 12
		// Code size: 20 (0x14)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4 262386
		IL_000e: conv.i8
		IL_000f: stelem.i8
		IL_0010: ldloc.1
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ret
	} // end of method ExpressionParser::mk_tokenSet_4_

	.method private hidebysig static 
		int64[] mk_tokenSet_5_ () cil managed 
	{
		// Method begins at RVA 0x3cc0
		// Header size: 12
		// Code size: 20 (0x14)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4 266226
		IL_000e: conv.i8
		IL_000f: stelem.i8
		IL_0010: ldloc.1
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ret
	} // end of method ExpressionParser::mk_tokenSet_5_

	.method private hidebysig static 
		int64[] mk_tokenSet_6_ () cil managed 
	{
		// Method begins at RVA 0x3ce0
		// Header size: 12
		// Code size: 20 (0x14)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4 278514
		IL_000e: conv.i8
		IL_000f: stelem.i8
		IL_0010: ldloc.1
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ret
	} // end of method ExpressionParser::mk_tokenSet_6_

	.method private hidebysig static 
		int64[] mk_tokenSet_7_ () cil managed 
	{
		// Method begins at RVA 0x3d00
		// Header size: 12
		// Code size: 20 (0x14)
		.maxstack 3
		.locals init (
			[0] int64[],
			[1] int64[]
		)

		IL_0000: ldc.i4.2
		IL_0001: newarr [mscorlib]System.Int64
		IL_0006: stloc.1
		IL_0007: ldloc.1
		IL_0008: ldc.i4.0
		IL_0009: ldc.i4 393202
		IL_000e: conv.i8
		IL_000f: stelem.i8
		IL_0010: ldloc.1
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ret
	} // end of method ExpressionParser::mk_tokenSet_7_

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x3d20
		// Header size: 12
		// Code size: 387 (0x183)
		.maxstack 3
		.locals init (
			[0] string[]
		)

		IL_0000: ldc.i4.s 29
		IL_0002: newarr [mscorlib]System.String
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: ldc.i4.0
		IL_000a: ldstr "\"<0>\""
		IL_000f: stelem.ref
		IL_0010: ldloc.0
		IL_0011: ldc.i4.1
		IL_0012: ldstr "\"EOF\""
		IL_0017: stelem.ref
		IL_0018: ldloc.0
		IL_0019: ldc.i4.2
		IL_001a: ldstr "\"<2>\""
		IL_001f: stelem.ref
		IL_0020: ldloc.0
		IL_0021: ldc.i4.3
		IL_0022: ldstr "\"NULL_TREE_LOOKAHEAD\""
		IL_0027: stelem.ref
		IL_0028: ldloc.0
		IL_0029: ldc.i4.4
		IL_002a: ldstr "\"EQ\""
		IL_002f: stelem.ref
		IL_0030: ldloc.0
		IL_0031: ldc.i4.5
		IL_0032: ldstr "\"OR\""
		IL_0037: stelem.ref
		IL_0038: ldloc.0
		IL_0039: ldc.i4.6
		IL_003a: ldstr "\"AND\""
		IL_003f: stelem.ref
		IL_0040: ldloc.0
		IL_0041: ldc.i4.7
		IL_0042: ldstr "\"NE\""
		IL_0047: stelem.ref
		IL_0048: ldloc.0
		IL_0049: ldc.i4.8
		IL_004a: ldstr "\"LS\""
		IL_004f: stelem.ref
		IL_0050: ldloc.0
		IL_0051: ldc.i4.s 9
		IL_0053: ldstr "\"GT\""
		IL_0058: stelem.ref
		IL_0059: ldloc.0
		IL_005a: ldc.i4.s 10
		IL_005c: ldstr "\"LE\""
		IL_0061: stelem.ref
		IL_0062: ldloc.0
		IL_0063: ldc.i4.s 11
		IL_0065: ldstr "\"GE\""
		IL_006a: stelem.ref
		IL_006b: ldloc.0
		IL_006c: ldc.i4.s 12
		IL_006e: ldstr "\"PLUS\""
		IL_0073: stelem.ref
		IL_0074: ldloc.0
		IL_0075: ldc.i4.s 13
		IL_0077: ldstr "\"MINUS\""
		IL_007c: stelem.ref
		IL_007d: ldloc.0
		IL_007e: ldc.i4.s 14
		IL_0080: ldstr "\"MUL\""
		IL_0085: stelem.ref
		IL_0086: ldloc.0
		IL_0087: ldc.i4.s 15
		IL_0089: ldstr "\"DIV\""
		IL_008e: stelem.ref
		IL_008f: ldloc.0
		IL_0090: ldc.i4.s 16
		IL_0092: ldstr "\"MOD\""
		IL_0097: stelem.ref
		IL_0098: ldloc.0
		IL_0099: ldc.i4.s 17
		IL_009b: ldstr "\"LPAREN\""
		IL_00a0: stelem.ref
		IL_00a1: ldloc.0
		IL_00a2: ldc.i4.s 18
		IL_00a4: ldstr "\"RPAREN\""
		IL_00a9: stelem.ref
		IL_00aa: ldloc.0
		IL_00ab: ldc.i4.s 19
		IL_00ad: ldstr "\"INT\""
		IL_00b2: stelem.ref
		IL_00b3: ldloc.0
		IL_00b4: ldc.i4.s 20
		IL_00b6: ldstr "\"FLOAT\""
		IL_00bb: stelem.ref
		IL_00bc: ldloc.0
		IL_00bd: ldc.i4.s 21
		IL_00bf: ldstr "\"STRING\""
		IL_00c4: stelem.ref
		IL_00c5: ldloc.0
		IL_00c6: ldc.i4.s 22
		IL_00c8: ldstr "\"STRING2\""
		IL_00cd: stelem.ref
		IL_00ce: ldloc.0
		IL_00cf: ldc.i4.s 23
		IL_00d1: ldstr "\"DEFINE\""
		IL_00d6: stelem.ref
		IL_00d7: ldloc.0
		IL_00d8: ldc.i4.s 24
		IL_00da: ldstr "\"ROUND\""
		IL_00df: stelem.ref
		IL_00e0: ldloc.0
		IL_00e1: ldc.i4.s 25
		IL_00e3: ldstr "\"WS\""
		IL_00e8: stelem.ref
		IL_00e9: ldloc.0
		IL_00ea: ldc.i4.s 26
		IL_00ec: ldstr "\"DIGIT\""
		IL_00f1: stelem.ref
		IL_00f2: ldloc.0
		IL_00f3: ldc.i4.s 27
		IL_00f5: ldstr "\"LETTER\""
		IL_00fa: stelem.ref
		IL_00fb: ldloc.0
		IL_00fc: ldc.i4.s 28
		IL_00fe: ldstr "\"NUMBER\""
		IL_0103: stelem.ref
		IL_0104: ldloc.0
		IL_0105: stsfld string[] ExpressionParser::tokenNames_
		IL_010a: call int64[] ExpressionParser::mk_tokenSet_0_()
		IL_010f: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_0114: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_0_
		IL_0119: call int64[] ExpressionParser::mk_tokenSet_1_()
		IL_011e: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_0123: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_1_
		IL_0128: call int64[] ExpressionParser::mk_tokenSet_2_()
		IL_012d: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_0132: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_2_
		IL_0137: call int64[] ExpressionParser::mk_tokenSet_3_()
		IL_013c: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_0141: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_3_
		IL_0146: call int64[] ExpressionParser::mk_tokenSet_4_()
		IL_014b: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_0150: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_4_
		IL_0155: call int64[] ExpressionParser::mk_tokenSet_5_()
		IL_015a: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_015f: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_5_
		IL_0164: call int64[] ExpressionParser::mk_tokenSet_6_()
		IL_0169: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_016e: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_6_
		IL_0173: call int64[] ExpressionParser::mk_tokenSet_7_()
		IL_0178: newobj instance void [antlr.runtime]antlr.collections.impl.BitSet::.ctor(int64[])
		IL_017d: stsfld class [antlr.runtime]antlr.collections.impl.BitSet ExpressionParser::tokenSet_7_
		IL_0182: ret
	} // end of method ExpressionParser::.cctor

} // end of class ExpressionParser

.class public auto ansi beforefieldinit ExpressionParserTokenTypes
	extends [mscorlib]System.Object
{
	// Fields
	.field public static literal int32 EOF = int32(1)
	.field public static literal int32 NULL_TREE_LOOKAHEAD = int32(3)
	.field public static literal int32 EQ = int32(4)
	.field public static literal int32 OR = int32(5)
	.field public static literal int32 AND = int32(6)
	.field public static literal int32 NE = int32(7)
	.field public static literal int32 LS = int32(8)
	.field public static literal int32 GT = int32(9)
	.field public static literal int32 LE = int32(10)
	.field public static literal int32 GE = int32(11)
	.field public static literal int32 PLUS = int32(12)
	.field public static literal int32 MINUS = int32(13)
	.field public static literal int32 MUL = int32(14)
	.field public static literal int32 DIV = int32(15)
	.field public static literal int32 MOD = int32(16)
	.field public static literal int32 LPAREN = int32(17)
	.field public static literal int32 RPAREN = int32(18)
	.field public static literal int32 INT = int32(19)
	.field public static literal int32 FLOAT = int32(20)
	.field public static literal int32 STRING = int32(21)
	.field public static literal int32 STRING2 = int32(22)
	.field public static literal int32 DEFINE = int32(23)
	.field public static literal int32 ROUND = int32(24)
	.field public static literal int32 WS = int32(25)
	.field public static literal int32 DIGIT = int32(26)
	.field public static literal int32 LETTER = int32(27)
	.field public static literal int32 NUMBER = int32(28)

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x3eaf
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method ExpressionParserTokenTypes::.ctor

} // end of class ExpressionParserTokenTypes

.class public auto ansi beforefieldinit BinaryAssetBuilder.ExpressionEval.ExpressionValue
	extends [mscorlib]System.Object
{
	// Nested Types
	.class nested private auto ansi sealed ValueType
		extends [mscorlib]System.Enum
	{
		// Fields
		.field public specialname rtspecialname int32 value__
		.field public static literal valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType Integer = int32(0)
		.field public static literal valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType Float = int32(1)
		.field public static literal valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType String = int32(2)

	} // end of class ValueType


	// Fields
	.field private valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType _ValueType
	.field private int32 _Int
	.field private float64 _Float
	.field private string _String
	.field private class [mscorlib]System.Globalization.CultureInfo _CultureInfo

	// Methods
	.method private hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x3eb7
		// Header size: 1
		// Code size: 24 (0x18)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldstr "en-US"
		IL_0006: ldc.i4.0
		IL_0007: newobj instance void [mscorlib]System.Globalization.CultureInfo::.ctor(string, bool)
		IL_000c: stfld class [mscorlib]System.Globalization.CultureInfo BinaryAssetBuilder.ExpressionEval.ExpressionValue::_CultureInfo
		IL_0011: ldarg.0
		IL_0012: call instance void [mscorlib]System.Object::.ctor()
		IL_0017: ret
	} // end of method ExpressionValue::.ctor

	.method private hidebysig static 
		void ThrowError (
			string msg
		) cil managed 
	{
		// Method begins at RVA 0x3ed0
		// Header size: 12
		// Code size: 25 (0x19)
		.maxstack 5
		.locals init (
			[0] object[]
		)

		IL_0000: ldc.i4.s 18
		IL_0002: ldstr "Expression error: {0}"
		IL_0007: ldc.i4.1
		IL_0008: newarr [mscorlib]System.Object
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: ldc.i4.0
		IL_0010: ldarg.0
		IL_0011: stelem.ref
		IL_0012: ldloc.0
		IL_0013: newobj instance void [BinaryAssetBuilder.Core]BinaryAssetBuilder.BinaryAssetBuilderException::.ctor(valuetype [BinaryAssetBuilder.Core]BinaryAssetBuilder.ErrorCode, string, object[])
		IL_0018: throw
	} // end of method ExpressionValue::ThrowError

	.method private hidebysig 
		instance void SetInteger (
			string val
		) cil managed 
	{
		// Method begins at RVA 0x3ef5
		// Header size: 1
		// Code size: 20 (0x14)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldc.i4.0
		IL_0002: stfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: call int32 [mscorlib]System.Convert::ToInt32(string)
		IL_000e: stfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0013: ret
	} // end of method ExpressionValue::SetInteger

	.method private hidebysig 
		instance void SetInteger (
			int32 val
		) cil managed 
	{
		// Method begins at RVA 0x3f0a
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldc.i4.0
		IL_0002: stfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: stfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_000e: ret
	} // end of method ExpressionValue::SetInteger

	.method public hidebysig static 
		class BinaryAssetBuilder.ExpressionEval.ExpressionValue NewInteger (
			string val
		) cil managed 
	{
		// Method begins at RVA 0x3f1c
		// Header size: 12
		// Code size: 15 (0xf)
		.maxstack 2
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue
		)

		IL_0000: newobj instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(string)
		IL_000d: ldloc.0
		IL_000e: ret
	} // end of method ExpressionValue::NewInteger

	.method private hidebysig 
		instance void SetFloat (
			string val
		) cil managed 
	{
		// Method begins at RVA 0x3f37
		// Header size: 1
		// Code size: 20 (0x14)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldc.i4.1
		IL_0002: stfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: call float64 [mscorlib]System.Convert::ToDouble(string)
		IL_000e: stfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_0013: ret
	} // end of method ExpressionValue::SetFloat

	.method private hidebysig 
		instance void SetFloat (
			float64 val
		) cil managed 
	{
		// Method begins at RVA 0x3f4c
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldc.i4.1
		IL_0002: stfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: stfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_000e: ret
	} // end of method ExpressionValue::SetFloat

	.method public hidebysig static 
		class BinaryAssetBuilder.ExpressionEval.ExpressionValue NewFloat (
			string val
		) cil managed 
	{
		// Method begins at RVA 0x3f5c
		// Header size: 12
		// Code size: 15 (0xf)
		.maxstack 2
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue
		)

		IL_0000: newobj instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetFloat(string)
		IL_000d: ldloc.0
		IL_000e: ret
	} // end of method ExpressionValue::NewFloat

	.method private hidebysig 
		instance void SetString (
			string val
		) cil managed 
	{
		// Method begins at RVA 0x3f77
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldc.i4.2
		IL_0002: stfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: stfld string BinaryAssetBuilder.ExpressionEval.ExpressionValue::_String
		IL_000e: ret
	} // end of method ExpressionValue::SetString

	.method public hidebysig static 
		class BinaryAssetBuilder.ExpressionEval.ExpressionValue NewQuotedString (
			string val
		) cil managed 
	{
		// Method begins at RVA 0x3f88
		// Header size: 12
		// Code size: 29 (0x1d)
		.maxstack 5
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue
		)

		IL_0000: newobj instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: ldc.i4.1
		IL_0009: ldarg.0
		IL_000a: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_000f: ldc.i4.2
		IL_0010: sub
		IL_0011: callvirt instance string [mscorlib]System.String::Substring(int32, int32)
		IL_0016: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetString(string)
		IL_001b: ldloc.0
		IL_001c: ret
	} // end of method ExpressionValue::NewQuotedString

	.method public hidebysig static 
		class BinaryAssetBuilder.ExpressionEval.ExpressionValue NewFromLookup (
			string val
		) cil managed 
	{
		// Method begins at RVA 0x3fb4
		// Header size: 12
		// Code size: 54 (0x36)
		.maxstack 2
		.locals init (
			[0] class BinaryAssetBuilder.ExpressionEval.ExpressionValue,
			[1] int32,
			[2] float32
		)

		IL_0000: newobj instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::.ctor()
		IL_0005: stloc.0
		IL_0006: ldarg.0
		IL_0007: ldloca.s 1
		IL_0009: call bool [mscorlib]System.Int32::TryParse(string, int32&)
		IL_000e: brfalse.s IL_0019

		IL_0010: ldloc.0
		IL_0011: ldloc.1
		IL_0012: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_0017: br.s IL_0034

		IL_0019: ldarg.0
		IL_001a: ldloca.s 2
		IL_001c: call bool [mscorlib]System.Single::TryParse(string, float32&)
		IL_0021: brfalse.s IL_002d

		IL_0023: ldloc.0
		IL_0024: ldloc.2
		IL_0025: conv.r8
		IL_0026: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetFloat(float64)
		IL_002b: br.s IL_0034

		IL_002d: ldloc.0
		IL_002e: ldarg.0
		IL_002f: callvirt instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetString(string)

		IL_0034: ldloc.0
		IL_0035: ret
	} // end of method ExpressionValue::NewFromLookup

	.method public hidebysig 
		instance int32 AsInteger () cil managed 
	{
		// Method begins at RVA 0x3ff8
		// Header size: 12
		// Code size: 64 (0x40)
		.maxstack 1
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_001b, IL_0022, IL_002e)

		IL_0019: br.s IL_003a

		IL_001b: ldarg.0
		IL_001c: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0021: ret

		IL_0022: ldarg.0
		IL_0023: ldfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_0028: call int32 [mscorlib]System.Convert::ToInt32(float64)
		IL_002d: ret

		IL_002e: ldarg.0
		IL_002f: ldfld string BinaryAssetBuilder.ExpressionEval.ExpressionValue::_String
		IL_0034: call int32 [mscorlib]System.Convert::ToInt32(string)
		IL_0039: ret

		IL_003a: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_003f: throw
	} // end of method ExpressionValue::AsInteger

	.method public hidebysig 
		instance float64 AsFloat () cil managed 
	{
		// Method begins at RVA 0x4044
		// Header size: 12
		// Code size: 71 (0x47)
		.maxstack 2
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_001b, IL_0023, IL_002a)

		IL_0019: br.s IL_0041

		IL_001b: ldarg.0
		IL_001c: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0021: conv.r8
		IL_0022: ret

		IL_0023: ldarg.0
		IL_0024: ldfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_0029: ret

		IL_002a: ldarg.0
		IL_002b: ldfld string BinaryAssetBuilder.ExpressionEval.ExpressionValue::_String
		IL_0030: ldarg.0
		IL_0031: ldfld class [mscorlib]System.Globalization.CultureInfo BinaryAssetBuilder.ExpressionEval.ExpressionValue::_CultureInfo
		IL_0036: callvirt instance class [mscorlib]System.Globalization.NumberFormatInfo [mscorlib]System.Globalization.CultureInfo::get_NumberFormat()
		IL_003b: call float64 [mscorlib]System.Convert::ToDouble(string, class [mscorlib]System.IFormatProvider)
		IL_0040: ret

		IL_0041: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0046: throw
	} // end of method ExpressionValue::AsFloat

	.method public hidebysig 
		instance string AsString () cil managed 
	{
		// Method begins at RVA 0x4098
		// Header size: 12
		// Code size: 75 (0x4b)
		.maxstack 2
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_001b, IL_0027, IL_003e)

		IL_0019: br.s IL_0045

		IL_001b: ldarg.0
		IL_001c: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0021: call string [mscorlib]System.Convert::ToString(int32)
		IL_0026: ret

		IL_0027: ldarg.0
		IL_0028: ldfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_002d: ldarg.0
		IL_002e: ldfld class [mscorlib]System.Globalization.CultureInfo BinaryAssetBuilder.ExpressionEval.ExpressionValue::_CultureInfo
		IL_0033: callvirt instance class [mscorlib]System.Globalization.NumberFormatInfo [mscorlib]System.Globalization.CultureInfo::get_NumberFormat()
		IL_0038: call string [mscorlib]System.Convert::ToString(float64, class [mscorlib]System.IFormatProvider)
		IL_003d: ret

		IL_003e: ldarg.0
		IL_003f: ldfld string BinaryAssetBuilder.ExpressionEval.ExpressionValue::_String
		IL_0044: ret

		IL_0045: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_004a: throw
	} // end of method ExpressionValue::AsString

	.method private hidebysig 
		instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType PromoteToSameType (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x40ef
		// Header size: 1
		// Code size: 28 (0x1c)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: ldarg.1
		IL_0007: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_000c: bgt.s IL_0015

		IL_000e: ldarg.1
		IL_000f: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0014: ret

		IL_0015: ldarg.0
		IL_0016: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_001b: ret
	} // end of method ExpressionValue::PromoteToSameType

	.method public hidebysig 
		instance void LogicalOr (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x410c
		// Header size: 12
		// Code size: 82 (0x52)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_001b, IL_0036, IL_0041)

		IL_0019: br.s IL_004c

		IL_001b: ldarg.0
		IL_001c: ldarg.0
		IL_001d: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0022: brtrue.s IL_002f

		IL_0024: ldarg.1
		IL_0025: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_002a: brtrue.s IL_002f

		IL_002c: ldc.i4.0
		IL_002d: br.s IL_0030

		IL_002f: ldc.i4.1

		IL_0030: stfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0035: ret

		IL_0036: ldstr "'or' not allowed for floats"
		IL_003b: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0040: ret

		IL_0041: ldstr "'or' not allowed for strings"
		IL_0046: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_004b: ret

		IL_004c: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0051: throw
	} // end of method ExpressionValue::LogicalOr

	.method public hidebysig 
		instance void LogicalAnd (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x416c
		// Header size: 12
		// Code size: 82 (0x52)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_001b, IL_0036, IL_0041)

		IL_0019: br.s IL_004c

		IL_001b: ldarg.0
		IL_001c: ldarg.0
		IL_001d: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0022: brfalse.s IL_002c

		IL_0024: ldarg.1
		IL_0025: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_002a: brtrue.s IL_002f

		IL_002c: ldc.i4.0
		IL_002d: br.s IL_0030

		IL_002f: ldc.i4.1

		IL_0030: stfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0035: ret

		IL_0036: ldstr "'and' not allowed for floats"
		IL_003b: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0040: ret

		IL_0041: ldstr "'and' not allowed for strings"
		IL_0046: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_004b: ret

		IL_004c: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0051: throw
	} // end of method ExpressionValue::LogicalAnd

	.method public hidebysig 
		instance void IsEqual (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x41cc
		// Header size: 12
		// Code size: 114 (0x72)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001c, IL_0035, IL_004e)

		IL_001a: br.s IL_006c

		IL_001c: ldarg.0
		IL_001d: ldarg.0
		IL_001e: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0023: ldarg.1
		IL_0024: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0029: beq.s IL_002e

		IL_002b: ldc.i4.0
		IL_002c: br.s IL_002f

		IL_002e: ldc.i4.1

		IL_002f: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_0034: ret

		IL_0035: ldarg.0
		IL_0036: ldarg.0
		IL_0037: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_003c: ldarg.1
		IL_003d: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0042: beq.s IL_0047

		IL_0044: ldc.i4.0
		IL_0045: br.s IL_0048

		IL_0047: ldc.i4.1

		IL_0048: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_004d: ret

		IL_004e: ldarg.0
		IL_004f: ldarg.0
		IL_0050: call instance string BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsString()
		IL_0055: ldarg.1
		IL_0056: callvirt instance string BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsString()
		IL_005b: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0060: brtrue.s IL_0065

		IL_0062: ldc.i4.0
		IL_0063: br.s IL_0066

		IL_0065: ldc.i4.1

		IL_0066: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_006b: ret

		IL_006c: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0071: throw
	} // end of method ExpressionValue::IsEqual

	.method public hidebysig 
		instance void IsLessThan (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x424c
		// Header size: 12
		// Code size: 95 (0x5f)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001c, IL_0035, IL_004e)

		IL_001a: br.s IL_0059

		IL_001c: ldarg.0
		IL_001d: ldarg.0
		IL_001e: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0023: ldarg.1
		IL_0024: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0029: blt.s IL_002e

		IL_002b: ldc.i4.0
		IL_002c: br.s IL_002f

		IL_002e: ldc.i4.1

		IL_002f: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_0034: ret

		IL_0035: ldarg.0
		IL_0036: ldarg.0
		IL_0037: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_003c: ldarg.1
		IL_003d: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0042: blt.s IL_0047

		IL_0044: ldc.i4.0
		IL_0045: br.s IL_0048

		IL_0047: ldc.i4.1

		IL_0048: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_004d: ret

		IL_004e: ldstr "'<', '>', ... not allowed for strings"
		IL_0053: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0058: ret

		IL_0059: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_005e: throw
	} // end of method ExpressionValue::IsLessThan

	.method public hidebysig 
		instance void IsGreaterThan (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x42b8
		// Header size: 12
		// Code size: 95 (0x5f)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001c, IL_0035, IL_004e)

		IL_001a: br.s IL_0059

		IL_001c: ldarg.0
		IL_001d: ldarg.0
		IL_001e: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0023: ldarg.1
		IL_0024: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0029: bgt.s IL_002e

		IL_002b: ldc.i4.0
		IL_002c: br.s IL_002f

		IL_002e: ldc.i4.1

		IL_002f: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_0034: ret

		IL_0035: ldarg.0
		IL_0036: ldarg.0
		IL_0037: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_003c: ldarg.1
		IL_003d: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0042: bgt.s IL_0047

		IL_0044: ldc.i4.0
		IL_0045: br.s IL_0048

		IL_0047: ldc.i4.1

		IL_0048: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_004d: ret

		IL_004e: ldstr "'<', '>', ... not allowed for strings"
		IL_0053: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0058: ret

		IL_0059: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_005e: throw
	} // end of method ExpressionValue::IsGreaterThan

	.method public hidebysig 
		instance void Not () cil managed 
	{
		// Method begins at RVA 0x4324
		// Header size: 12
		// Code size: 74 (0x4a)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_001b, IL_002e, IL_0039)

		IL_0019: br.s IL_0044

		IL_001b: ldarg.0
		IL_001c: ldarg.0
		IL_001d: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0022: brtrue.s IL_0027

		IL_0024: ldc.i4.1
		IL_0025: br.s IL_0028

		IL_0027: ldc.i4.0

		IL_0028: stfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_002d: ret

		IL_002e: ldstr "'!' not allowed for floats"
		IL_0033: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0038: ret

		IL_0039: ldstr "'!' not allowed for strings"
		IL_003e: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0043: ret

		IL_0044: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0049: throw
	} // end of method ExpressionValue::Not

	.method public hidebysig 
		instance void Negate () cil managed 
	{
		// Method begins at RVA 0x437c
		// Header size: 12
		// Code size: 72 (0x48)
		.maxstack 2
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_001b, IL_0029, IL_0037)

		IL_0019: br.s IL_0042

		IL_001b: ldarg.0
		IL_001c: ldarg.0
		IL_001d: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0022: neg
		IL_0023: stfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0028: ret

		IL_0029: ldarg.0
		IL_002a: ldarg.0
		IL_002b: ldfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_0030: neg
		IL_0031: stfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_0036: ret

		IL_0037: ldstr "'!' not allowed for strings"
		IL_003c: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0041: ret

		IL_0042: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0047: throw
	} // end of method ExpressionValue::Negate

	.method public hidebysig 
		instance void Multiply (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x43d0
		// Header size: 12
		// Code size: 85 (0x55)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001c, IL_0030, IL_0044)

		IL_001a: br.s IL_004f

		IL_001c: ldarg.0
		IL_001d: ldarg.0
		IL_001e: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0023: ldarg.1
		IL_0024: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0029: mul
		IL_002a: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_002f: ret

		IL_0030: ldarg.0
		IL_0031: ldarg.0
		IL_0032: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0037: ldarg.1
		IL_0038: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_003d: mul
		IL_003e: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetFloat(float64)
		IL_0043: ret

		IL_0044: ldstr "'*' not allowed for strings"
		IL_0049: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_004e: ret

		IL_004f: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0054: throw
	} // end of method ExpressionValue::Multiply

	.method public hidebysig 
		instance void Divide (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x4434
		// Header size: 12
		// Code size: 170 (0xaa)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001f, IL_0069, IL_0099)

		IL_001a: br IL_00a4

		IL_001f: ldarg.1
		IL_0020: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0025: brtrue.s IL_0032

		IL_0027: ldstr "Division by zero"
		IL_002c: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0031: ret

		IL_0032: ldarg.0
		IL_0033: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0038: ldarg.1
		IL_0039: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_003e: rem
		IL_003f: brfalse.s IL_0055

		IL_0041: ldarg.0
		IL_0042: ldarg.0
		IL_0043: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0048: ldarg.1
		IL_0049: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_004e: div
		IL_004f: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetFloat(float64)
		IL_0054: ret

		IL_0055: ldarg.0
		IL_0056: ldarg.0
		IL_0057: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_005c: ldarg.1
		IL_005d: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0062: div
		IL_0063: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_0068: ret

		IL_0069: ldarg.1
		IL_006a: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_006f: ldc.r8 0.0
		IL_0078: bne.un.s IL_0085

		IL_007a: ldstr "Division by zero"
		IL_007f: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0084: ret

		IL_0085: ldarg.0
		IL_0086: ldarg.0
		IL_0087: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_008c: ldarg.1
		IL_008d: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0092: div
		IL_0093: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetFloat(float64)
		IL_0098: ret

		IL_0099: ldstr "'/' not allowed for strings"
		IL_009e: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_00a3: ret

		IL_00a4: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_00a9: throw
	} // end of method ExpressionValue::Divide

	.method public hidebysig 
		instance void Modulo (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x44ec
		// Header size: 12
		// Code size: 95 (0x5f)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001c, IL_0043, IL_004e)

		IL_001a: br.s IL_0059

		IL_001c: ldarg.1
		IL_001d: ldfld int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Int
		IL_0022: brtrue.s IL_002f

		IL_0024: ldstr "Division by zero"
		IL_0029: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_002e: ret

		IL_002f: ldarg.0
		IL_0030: ldarg.0
		IL_0031: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0036: ldarg.1
		IL_0037: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_003c: rem
		IL_003d: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_0042: ret

		IL_0043: ldstr "'%' not allowed for floats"
		IL_0048: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_004d: ret

		IL_004e: ldstr "'%' not allowed for strings"
		IL_0053: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0058: ret

		IL_0059: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_005e: throw
	} // end of method ExpressionValue::Modulo

	.method public hidebysig 
		instance void Add (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x4558
		// Header size: 12
		// Code size: 98 (0x62)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001c, IL_0030, IL_0044)

		IL_001a: br.s IL_005c

		IL_001c: ldarg.0
		IL_001d: ldarg.0
		IL_001e: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0023: ldarg.1
		IL_0024: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0029: add
		IL_002a: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_002f: ret

		IL_0030: ldarg.0
		IL_0031: ldarg.0
		IL_0032: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0037: ldarg.1
		IL_0038: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_003d: add
		IL_003e: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetFloat(float64)
		IL_0043: ret

		IL_0044: ldarg.0
		IL_0045: ldarg.0
		IL_0046: call instance string BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsString()
		IL_004b: ldarg.1
		IL_004c: callvirt instance string BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsString()
		IL_0051: call string [mscorlib]System.String::Concat(string, string)
		IL_0056: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetString(string)
		IL_005b: ret

		IL_005c: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0061: throw
	} // end of method ExpressionValue::Add

	.method public hidebysig 
		instance void Subtract (
			class BinaryAssetBuilder.ExpressionEval.ExpressionValue rhs
		) cil managed 
	{
		// Method begins at RVA 0x45c8
		// Header size: 12
		// Code size: 85 (0x55)
		.maxstack 3
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::PromoteToSameType(class BinaryAssetBuilder.ExpressionEval.ExpressionValue)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: switch (IL_001c, IL_0030, IL_0044)

		IL_001a: br.s IL_004f

		IL_001c: ldarg.0
		IL_001d: ldarg.0
		IL_001e: call instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0023: ldarg.1
		IL_0024: callvirt instance int32 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsInteger()
		IL_0029: sub
		IL_002a: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_002f: ret

		IL_0030: ldarg.0
		IL_0031: ldarg.0
		IL_0032: call instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_0037: ldarg.1
		IL_0038: callvirt instance float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::AsFloat()
		IL_003d: sub
		IL_003e: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetFloat(float64)
		IL_0043: ret

		IL_0044: ldstr "'-' not allowed for strings"
		IL_0049: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_004e: ret

		IL_004f: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_0054: throw
	} // end of method ExpressionValue::Subtract

	.method public hidebysig 
		instance void FuncRound () cil managed 
	{
		// Method begins at RVA 0x462c
		// Header size: 12
		// Code size: 64 (0x40)
		.maxstack 2
		.locals init (
			[0] valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType
		)

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype BinaryAssetBuilder.ExpressionEval.ExpressionValue/ValueType BinaryAssetBuilder.ExpressionEval.ExpressionValue::_ValueType
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: switch (IL_003f, IL_001b, IL_002e)

		IL_0019: br.s IL_0039

		IL_001b: ldarg.0
		IL_001c: ldarg.0
		IL_001d: ldfld float64 BinaryAssetBuilder.ExpressionEval.ExpressionValue::_Float
		IL_0022: call float64 [mscorlib]System.Math::Round(float64)
		IL_0027: conv.i4
		IL_0028: call instance void BinaryAssetBuilder.ExpressionEval.ExpressionValue::SetInteger(int32)
		IL_002d: ret

		IL_002e: ldstr "'round' not allowed for strings"
		IL_0033: call void BinaryAssetBuilder.ExpressionEval.ExpressionValue::ThrowError(string)
		IL_0038: ret

		IL_0039: newobj instance void [mscorlib]System.ApplicationException::.ctor()
		IL_003e: throw

		IL_003f: ret
	} // end of method ExpressionValue::FuncRound

} // end of class BinaryAssetBuilder.ExpressionEval.ExpressionValue

You are not using the latest version of the tool, please update.
Latest version is '11.1.0.9782' (yours is '9.1.0.7988')
