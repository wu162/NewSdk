// IL code: EALA.Metrics
.class private auto ansi '<Module>'
{
} // end of class <Module>

.class interface public auto ansi abstract EALA.Metrics.IMetricsListener
{
	// Methods
	.method public hidebysig newslot abstract virtual 
		instance void Open () cil managed 
	{
	} // end of method IMetricsListener::Open

	.method public hidebysig newslot abstract virtual 
		instance void Close () cil managed 
	{
	} // end of method IMetricsListener::Close

	.method public hidebysig newslot abstract virtual 
		instance void SubmitMetrics (
			class EALA.Metrics.Metric m_
		) cil managed 
	{
	} // end of method IMetricsListener::SubmitMetrics

} // end of class EALA.Metrics.IMetricsListener

.class public auto ansi beforefieldinit EALA.Metrics.ConsoleMetricsListener
	extends [mscorlib]System.Object
	implements EALA.Metrics.IMetricsListener
{
	// Fields
	.field private class [mscorlib]System.IO.TextWriter m_OutputStream
	.field private class [mscorlib]System.Collections.ArrayList m_ConsoleBuffer

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 29 (0x1d)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: newobj instance void [mscorlib]System.Collections.ArrayList::.ctor()
		IL_0006: stfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.ConsoleMetricsListener::m_ConsoleBuffer
		IL_000b: ldarg.0
		IL_000c: call instance void [mscorlib]System.Object::.ctor()
		IL_0011: ldarg.0
		IL_0012: call class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
		IL_0017: stfld class [mscorlib]System.IO.TextWriter EALA.Metrics.ConsoleMetricsListener::m_OutputStream
		IL_001c: ret
	} // end of method ConsoleMetricsListener::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class [mscorlib]System.IO.TextWriter output
		) cil managed 
	{
		// Method begins at RVA 0x206e
		// Header size: 1
		// Code size: 25 (0x19)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: newobj instance void [mscorlib]System.Collections.ArrayList::.ctor()
		IL_0006: stfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.ConsoleMetricsListener::m_ConsoleBuffer
		IL_000b: ldarg.0
		IL_000c: call instance void [mscorlib]System.Object::.ctor()
		IL_0011: ldarg.0
		IL_0012: ldarg.1
		IL_0013: stfld class [mscorlib]System.IO.TextWriter EALA.Metrics.ConsoleMetricsListener::m_OutputStream
		IL_0018: ret
	} // end of method ConsoleMetricsListener::.ctor

	.method public final hidebysig newslot virtual 
		instance void Open () cil managed 
	{
		// Method begins at RVA 0x2088
		// Header size: 1
		// Code size: 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method ConsoleMetricsListener::Open

	.method public final hidebysig newslot virtual 
		instance void Close () cil managed 
	{
		// Method begins at RVA 0x208c
		// Header size: 12
		// Code size: 60 (0x3c)
		.maxstack 1
		.locals init (
			[0] string,
			[1] class [mscorlib]System.Collections.IEnumerator,
			[2] class [mscorlib]System.IDisposable
		)

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.ConsoleMetricsListener::m_ConsoleBuffer
		IL_0006: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.ArrayList::GetEnumerator()
		IL_000b: stloc.1
		.try
		{
			IL_000c: br.s IL_0020
			// loop start (head: IL_0020)
				IL_000e: ldloc.1
				IL_000f: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
				IL_0014: castclass [mscorlib]System.String
				IL_0019: stloc.0
				IL_001a: ldloc.0
				IL_001b: call void [mscorlib]System.Console::WriteLine(string)

				IL_0020: ldloc.1
				IL_0021: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
				IL_0026: brtrue.s IL_000e
			// end loop

			IL_0028: leave.s IL_003b
		} // end .try
		finally
		{
			IL_002a: ldloc.1
			IL_002b: isinst [mscorlib]System.IDisposable
			IL_0030: stloc.2
			IL_0031: ldloc.2
			IL_0032: brfalse.s IL_003a

			IL_0034: ldloc.2
			IL_0035: callvirt instance void [mscorlib]System.IDisposable::Dispose()

			IL_003a: endfinally
		} // end handler

		IL_003b: ret
	} // end of method ConsoleMetricsListener::Close

	.method public final hidebysig newslot virtual 
		instance void SubmitMetrics (
			class EALA.Metrics.Metric m
		) cil managed 
	{
		// Method begins at RVA 0x20e4
		// Header size: 1
		// Code size: 19 (0x13)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.ConsoleMetricsListener::m_ConsoleBuffer
		IL_0006: ldarg.1
		IL_0007: callvirt instance string [mscorlib]System.Object::ToString()
		IL_000c: callvirt instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
		IL_0011: pop
		IL_0012: ret
	} // end of method ConsoleMetricsListener::SubmitMetrics

} // end of class EALA.Metrics.ConsoleMetricsListener

.class public auto ansi beforefieldinit EALA.Metrics.ListMetric
	extends [mscorlib]System.Object
{
	// Fields
	.field private class [mscorlib]System.Collections.ArrayList m_Metrics

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x20f8
		// Header size: 1
		// Code size: 18 (0x12)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: newobj instance void [mscorlib]System.Collections.ArrayList::.ctor()
		IL_0006: stfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.ListMetric::m_Metrics
		IL_000b: ldarg.0
		IL_000c: call instance void [mscorlib]System.Object::.ctor()
		IL_0011: ret
	} // end of method ListMetric::.ctor

	.method public hidebysig 
		instance void AddMetric (
			class EALA.Metrics.Metric m
		) cil managed 
	{
		// Method begins at RVA 0x210b
		// Header size: 1
		// Code size: 14 (0xe)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.ListMetric::m_Metrics
		IL_0006: ldarg.1
		IL_0007: callvirt instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
		IL_000c: pop
		IL_000d: ret
	} // end of method ListMetric::AddMetric

	.method public hidebysig specialname 
		instance class [mscorlib]System.Collections.ArrayList get_Metrics () cil managed 
	{
		// Method begins at RVA 0x211a
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.ListMetric::m_Metrics
		IL_0006: ret
	} // end of method ListMetric::get_Metrics

	// Properties
	.property instance class [mscorlib]System.Collections.ArrayList Metrics()
	{
		.get instance class [mscorlib]System.Collections.ArrayList EALA.Metrics.ListMetric::get_Metrics()
	}

} // end of class EALA.Metrics.ListMetric

.class public auto ansi beforefieldinit EALA.Metrics.Metric
	extends [mscorlib]System.Object
{
	// Fields
	.field private class EALA.Metrics.MetricDescriptor m_descriptor
	.field private object[] m_datalist
	.field private valuetype [mscorlib]System.DateTime m_timestamp
	.field private static string[] _BytePostfix

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class EALA.Metrics.MetricDescriptor md,
			object[] datalist
		) cil managed 
	{
		.param [2]
			.custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = (
				01 00 00 00
			)
		// Method begins at RVA 0x2122
		// Header size: 1
		// Code size: 32 (0x20)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ldarg.0
		IL_0007: call valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
		IL_000c: stfld valuetype [mscorlib]System.DateTime EALA.Metrics.Metric::m_timestamp
		IL_0011: ldarg.0
		IL_0012: ldarg.2
		IL_0013: stfld object[] EALA.Metrics.Metric::m_datalist
		IL_0018: ldarg.0
		IL_0019: ldarg.1
		IL_001a: stfld class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::m_descriptor
		IL_001f: ret
	} // end of method Metric::.ctor

	.method public hidebysig virtual 
		instance string ToString () cil managed 
	{
		// Method begins at RVA 0x2144
		// Header size: 12
		// Code size: 309 (0x135)
		.maxstack 4
		.locals init (
			[0] class [mscorlib]System.Text.StringBuilder,
			[1] object,
			[2] float64,
			[3] string,
			[4] int32,
			[5] valuetype EALA.Metrics.MetricType,
			[6] valuetype [mscorlib]System.TimeSpan,
			[7] string[],
			[8] int32
		)

		IL_0000: newobj instance void [mscorlib]System.Text.StringBuilder::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldstr "[{0:G}] {1}: "
		IL_000c: ldarg.0
		IL_000d: ldfld valuetype [mscorlib]System.DateTime EALA.Metrics.Metric::m_timestamp
		IL_0012: box [mscorlib]System.DateTime
		IL_0017: ldarg.0
		IL_0018: ldfld class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::m_descriptor
		IL_001d: callvirt instance string EALA.Metrics.MetricDescriptor::get_Description()
		IL_0022: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string, object, object)
		IL_0027: pop
		IL_0028: ldarg.0
		IL_0029: ldfld object[] EALA.Metrics.Metric::m_datalist
		IL_002e: ldc.i4.0
		IL_002f: ldelem.ref
		IL_0030: stloc.1
		IL_0031: ldarg.0
		IL_0032: ldfld class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::m_descriptor
		IL_0037: callvirt instance valuetype EALA.Metrics.MetricType EALA.Metrics.MetricDescriptor::get_Type()
		IL_003c: stloc.s 5
		IL_003e: ldloc.s 5
		IL_0040: switch (IL_004f, IL_0072)

		IL_004d: br.s IL_00c9

		IL_004f: ldloc.0
		IL_0050: ldloc.1
		IL_0051: unbox.any [mscorlib]System.Double
		IL_0056: call valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
		IL_005b: stloc.s 6
		IL_005d: ldloca.s 6
		IL_005f: constrained. [mscorlib]System.TimeSpan
		IL_0065: callvirt instance string [mscorlib]System.Object::ToString()
		IL_006a: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
		IL_006f: pop
		IL_0070: br.s IL_00d1

		IL_0072: ldloc.1
		IL_0073: unbox.any [mscorlib]System.Int64
		IL_0078: conv.r8
		IL_0079: stloc.2
		IL_007a: ldsfld string[] EALA.Metrics.Metric::_BytePostfix
		IL_007f: stloc.s 7
		IL_0081: ldc.i4.0
		IL_0082: stloc.s 8
		IL_0084: br.s IL_00bf
		// loop start (head: IL_00bf)
			IL_0086: ldloc.s 7
			IL_0088: ldloc.s 8
			IL_008a: ldelem.ref
			IL_008b: stloc.3
			IL_008c: ldloc.2
			IL_008d: ldc.r8 1024
			IL_0096: bge.un.s IL_00ad

			IL_0098: ldloc.0
			IL_0099: ldstr "{0:n} {1}"
			IL_009e: ldloc.2
			IL_009f: box [mscorlib]System.Double
			IL_00a4: ldloc.3
			IL_00a5: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string, object, object)
			IL_00aa: pop
			IL_00ab: br.s IL_00d1

			IL_00ad: ldloc.2
			IL_00ae: ldc.r8 1024
			IL_00b7: div
			IL_00b8: stloc.2
			IL_00b9: ldloc.s 8
			IL_00bb: ldc.i4.1
			IL_00bc: add
			IL_00bd: stloc.s 8

			IL_00bf: ldloc.s 8
			IL_00c1: ldloc.s 7
			IL_00c3: ldlen
			IL_00c4: conv.i4
			IL_00c5: blt.s IL_0086
		// end loop

		IL_00c7: br.s IL_00d1

		IL_00c9: ldloc.0
		IL_00ca: ldloc.1
		IL_00cb: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
		IL_00d0: pop

		IL_00d1: ldarg.0
		IL_00d2: ldfld object[] EALA.Metrics.Metric::m_datalist
		IL_00d7: ldlen
		IL_00d8: conv.i4
		IL_00d9: ldc.i4.1
		IL_00da: ble.s IL_012e

		IL_00dc: ldloc.0
		IL_00dd: ldstr " [ {0}"
		IL_00e2: ldarg.0
		IL_00e3: ldfld object[] EALA.Metrics.Metric::m_datalist
		IL_00e8: ldc.i4.1
		IL_00e9: ldelem.ref
		IL_00ea: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string, object)
		IL_00ef: pop
		IL_00f0: ldsfld string [mscorlib]System.String::Empty
		IL_00f5: pop
		IL_00f6: ldc.i4.2
		IL_00f7: stloc.s 4
		IL_00f9: br.s IL_0116
		// loop start (head: IL_0116)
			IL_00fb: ldloc.0
			IL_00fc: ldstr ", {0}"
			IL_0101: ldarg.0
			IL_0102: ldfld object[] EALA.Metrics.Metric::m_datalist
			IL_0107: ldloc.s 4
			IL_0109: ldelem.ref
			IL_010a: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::AppendFormat(string, object)
			IL_010f: pop
			IL_0110: ldloc.s 4
			IL_0112: ldc.i4.1
			IL_0113: add
			IL_0114: stloc.s 4

			IL_0116: ldloc.s 4
			IL_0118: ldarg.0
			IL_0119: ldfld object[] EALA.Metrics.Metric::m_datalist
			IL_011e: ldlen
			IL_011f: conv.i4
			IL_0120: blt.s IL_00fb
		// end loop

		IL_0122: ldloc.0
		IL_0123: ldstr "]"
		IL_0128: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
		IL_012d: pop

		IL_012e: ldloc.0
		IL_012f: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0134: ret
	} // end of method Metric::ToString

	.method public hidebysig specialname 
		instance string get_Name () cil managed 
	{
		// Method begins at RVA 0x2285
		// Header size: 1
		// Code size: 12 (0xc)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::m_descriptor
		IL_0006: callvirt instance string EALA.Metrics.MetricDescriptor::get_Name()
		IL_000b: ret
	} // end of method Metric::get_Name

	.method public hidebysig specialname 
		instance class EALA.Metrics.MetricDescriptor get_Descriptor () cil managed 
	{
		// Method begins at RVA 0x2292
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::m_descriptor
		IL_0006: ret
	} // end of method Metric::get_Descriptor

	.method public hidebysig specialname 
		instance valuetype [mscorlib]System.DateTime get_TimeStamp () cil managed 
	{
		// Method begins at RVA 0x229a
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype [mscorlib]System.DateTime EALA.Metrics.Metric::m_timestamp
		IL_0006: ret
	} // end of method Metric::get_TimeStamp

	.method public hidebysig specialname 
		instance object get_Data () cil managed 
	{
		// Method begins at RVA 0x22a2
		// Header size: 1
		// Code size: 9 (0x9)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld object[] EALA.Metrics.Metric::m_datalist
		IL_0006: ldc.i4.0
		IL_0007: ldelem.ref
		IL_0008: ret
	} // end of method Metric::get_Data

	.method public hidebysig specialname 
		instance object[] get_DataList () cil managed 
	{
		// Method begins at RVA 0x22ac
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld object[] EALA.Metrics.Metric::m_datalist
		IL_0006: ret
	} // end of method Metric::get_DataList

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x22b4
		// Header size: 12
		// Code size: 87 (0x57)
		.maxstack 3
		.locals init (
			[0] string[]
		)

		IL_0000: ldc.i4.s 9
		IL_0002: newarr [mscorlib]System.String
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: ldc.i4.0
		IL_000a: ldstr "Bytes"
		IL_000f: stelem.ref
		IL_0010: ldloc.0
		IL_0011: ldc.i4.1
		IL_0012: ldstr "KiB"
		IL_0017: stelem.ref
		IL_0018: ldloc.0
		IL_0019: ldc.i4.2
		IL_001a: ldstr "MiB"
		IL_001f: stelem.ref
		IL_0020: ldloc.0
		IL_0021: ldc.i4.3
		IL_0022: ldstr "GiB"
		IL_0027: stelem.ref
		IL_0028: ldloc.0
		IL_0029: ldc.i4.4
		IL_002a: ldstr "TiB"
		IL_002f: stelem.ref
		IL_0030: ldloc.0
		IL_0031: ldc.i4.5
		IL_0032: ldstr "PiB"
		IL_0037: stelem.ref
		IL_0038: ldloc.0
		IL_0039: ldc.i4.6
		IL_003a: ldstr "EiB"
		IL_003f: stelem.ref
		IL_0040: ldloc.0
		IL_0041: ldc.i4.7
		IL_0042: ldstr "ZiB"
		IL_0047: stelem.ref
		IL_0048: ldloc.0
		IL_0049: ldc.i4.8
		IL_004a: ldstr "YiB"
		IL_004f: stelem.ref
		IL_0050: ldloc.0
		IL_0051: stsfld string[] EALA.Metrics.Metric::_BytePostfix
		IL_0056: ret
	} // end of method Metric::.cctor

	// Properties
	.property instance string Name()
	{
		.get instance string EALA.Metrics.Metric::get_Name()
	}
	.property instance class EALA.Metrics.MetricDescriptor Descriptor()
	{
		.get instance class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::get_Descriptor()
	}
	.property instance valuetype [mscorlib]System.DateTime TimeStamp()
	{
		.get instance valuetype [mscorlib]System.DateTime EALA.Metrics.Metric::get_TimeStamp()
	}
	.property instance object Data()
	{
		.get instance object EALA.Metrics.Metric::get_Data()
	}
	.property instance object[] DataList()
	{
		.get instance object[] EALA.Metrics.Metric::get_DataList()
	}

} // end of class EALA.Metrics.Metric

.class public auto ansi beforefieldinit EALA.Metrics.MetricApplicationData
	extends [mscorlib]System.Object
{
	// Fields
	.field private string m_ApplicationName
	.field private string m_Username
	.field private string m_MachineName

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2318
		// Header size: 12
		// Code size: 70 (0x46)
		.maxstack 2

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ldarg.0
		IL_0007: call class [mscorlib]System.Security.Principal.WindowsIdentity [mscorlib]System.Security.Principal.WindowsIdentity::GetCurrent()
		IL_000c: callvirt instance string [mscorlib]System.Security.Principal.WindowsIdentity::get_Name()
		IL_0011: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0016: stfld string EALA.Metrics.MetricApplicationData::m_Username
		IL_001b: ldarg.0
		IL_001c: call string [mscorlib]System.Environment::get_MachineName()
		IL_0021: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0026: stfld string EALA.Metrics.MetricApplicationData::m_MachineName
		IL_002b: ldarg.0
		IL_002c: call class [System]System.Diagnostics.Process [System]System.Diagnostics.Process::GetCurrentProcess()
		IL_0031: callvirt instance class [System]System.Diagnostics.ProcessModule [System]System.Diagnostics.Process::get_MainModule()
		IL_0036: callvirt instance string [System]System.Diagnostics.ProcessModule::get_FileName()
		IL_003b: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0040: stfld string EALA.Metrics.MetricApplicationData::m_ApplicationName
		IL_0045: ret
	} // end of method MetricApplicationData::.ctor

	.method public hidebysig specialname 
		instance string get_ApplicationName () cil managed 
	{
		// Method begins at RVA 0x236a
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld string EALA.Metrics.MetricApplicationData::m_ApplicationName
		IL_0006: ret
	} // end of method MetricApplicationData::get_ApplicationName

	.method public hidebysig specialname 
		instance void set_ApplicationName (
			string 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2372
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld string EALA.Metrics.MetricApplicationData::m_ApplicationName
		IL_0007: ret
	} // end of method MetricApplicationData::set_ApplicationName

	.method public hidebysig specialname 
		instance string get_UserName () cil managed 
	{
		// Method begins at RVA 0x237b
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld string EALA.Metrics.MetricApplicationData::m_Username
		IL_0006: ret
	} // end of method MetricApplicationData::get_UserName

	.method public hidebysig specialname 
		instance void set_UserName (
			string 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2383
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld string EALA.Metrics.MetricApplicationData::m_Username
		IL_0007: ret
	} // end of method MetricApplicationData::set_UserName

	.method public hidebysig specialname 
		instance string get_MachineName () cil managed 
	{
		// Method begins at RVA 0x238c
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld string EALA.Metrics.MetricApplicationData::m_MachineName
		IL_0006: ret
	} // end of method MetricApplicationData::get_MachineName

	.method public hidebysig specialname 
		instance void set_MachineName (
			string 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2394
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld string EALA.Metrics.MetricApplicationData::m_MachineName
		IL_0007: ret
	} // end of method MetricApplicationData::set_MachineName

	// Properties
	.property instance string ApplicationName()
	{
		.get instance string EALA.Metrics.MetricApplicationData::get_ApplicationName()
		.set instance void EALA.Metrics.MetricApplicationData::set_ApplicationName(string)
	}
	.property instance string UserName()
	{
		.get instance string EALA.Metrics.MetricApplicationData::get_UserName()
		.set instance void EALA.Metrics.MetricApplicationData::set_UserName(string)
	}
	.property instance string MachineName()
	{
		.get instance string EALA.Metrics.MetricApplicationData::get_MachineName()
		.set instance void EALA.Metrics.MetricApplicationData::set_MachineName(string)
	}

} // end of class EALA.Metrics.MetricApplicationData

.class public auto ansi sealed EALA.Metrics.MetricType
	extends [mscorlib]System.Enum
{
	// Fields
	.field public specialname rtspecialname int32 value__
	.field public static literal valuetype EALA.Metrics.MetricType Duration = int32(0)
	.field public static literal valuetype EALA.Metrics.MetricType Size = int32(1)
	.field public static literal valuetype EALA.Metrics.MetricType Count = int32(2)
	.field public static literal valuetype EALA.Metrics.MetricType Ratio = int32(3)
	.field public static literal valuetype EALA.Metrics.MetricType Name = int32(4)
	.field public static literal valuetype EALA.Metrics.MetricType Enabled = int32(5)
	.field public static literal valuetype EALA.Metrics.MetricType Success = int32(6)

} // end of class EALA.Metrics.MetricType

.class public auto ansi beforefieldinit EALA.Metrics.MetricDescriptor
	extends [mscorlib]System.Object
{
	// Fields
	.field private string m_Name
	.field private valuetype EALA.Metrics.MetricType m_Type
	.field private string m_Description

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			string name,
			valuetype EALA.Metrics.MetricType 'type',
			string description
		) cil managed 
	{
		// Method begins at RVA 0x239d
		// Header size: 1
		// Code size: 28 (0x1c)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ldarg.0
		IL_0007: ldarg.1
		IL_0008: stfld string EALA.Metrics.MetricDescriptor::m_Name
		IL_000d: ldarg.0
		IL_000e: ldarg.2
		IL_000f: stfld valuetype EALA.Metrics.MetricType EALA.Metrics.MetricDescriptor::m_Type
		IL_0014: ldarg.0
		IL_0015: ldarg.3
		IL_0016: stfld string EALA.Metrics.MetricDescriptor::m_Description
		IL_001b: ret
	} // end of method MetricDescriptor::.ctor

	.method public hidebysig specialname 
		instance string get_Name () cil managed 
	{
		// Method begins at RVA 0x23ba
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld string EALA.Metrics.MetricDescriptor::m_Name
		IL_0006: ret
	} // end of method MetricDescriptor::get_Name

	.method public hidebysig specialname 
		instance valuetype EALA.Metrics.MetricType get_Type () cil managed 
	{
		// Method begins at RVA 0x23c2
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype EALA.Metrics.MetricType EALA.Metrics.MetricDescriptor::m_Type
		IL_0006: ret
	} // end of method MetricDescriptor::get_Type

	.method public hidebysig specialname 
		instance string get_Description () cil managed 
	{
		// Method begins at RVA 0x23ca
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld string EALA.Metrics.MetricDescriptor::m_Description
		IL_0006: ret
	} // end of method MetricDescriptor::get_Description

	// Properties
	.property instance string Name()
	{
		.get instance string EALA.Metrics.MetricDescriptor::get_Name()
	}
	.property instance valuetype EALA.Metrics.MetricType Type()
	{
		.get instance valuetype EALA.Metrics.MetricType EALA.Metrics.MetricDescriptor::get_Type()
	}
	.property instance string Description()
	{
		.get instance string EALA.Metrics.MetricDescriptor::get_Description()
	}

} // end of class EALA.Metrics.MetricDescriptor

.class public auto ansi EALA.Metrics.MetricManager
	extends [mscorlib]System.Object
{
	// Fields
	.field private static class EALA.Metrics.MetricApplicationData m_AppData
	.field private static class [mscorlib]System.Collections.ArrayList m_Listeners
	.field private static bool m_SessionOpen
	.field private static class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor> _DescriptorMap
	.field private static bool m_Enabled

	// Methods
	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x23d2
		// Header size: 1
		// Code size: 43 (0x2b)
		.maxstack 8

		IL_0000: newobj instance void [mscorlib]System.Collections.ArrayList::.ctor()
		IL_0005: stsfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.MetricManager::m_Listeners
		IL_000a: ldc.i4.0
		IL_000b: stsfld bool EALA.Metrics.MetricManager::m_SessionOpen
		IL_0010: newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor>::.ctor()
		IL_0015: stsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor> EALA.Metrics.MetricManager::_DescriptorMap
		IL_001a: ldc.i4.1
		IL_001b: stsfld bool EALA.Metrics.MetricManager::m_Enabled
		IL_0020: newobj instance void EALA.Metrics.MetricApplicationData::.ctor()
		IL_0025: stsfld class EALA.Metrics.MetricApplicationData EALA.Metrics.MetricManager::m_AppData
		IL_002a: ret
	} // end of method MetricManager::.cctor

	.method public hidebysig static 
		void AddListener (
			class EALA.Metrics.IMetricsListener listener
		) cil managed 
	{
		// Method begins at RVA 0x23fe
		// Header size: 1
		// Code size: 31 (0x1f)
		.maxstack 8

		IL_0000: ldsfld bool EALA.Metrics.MetricManager::m_SessionOpen
		IL_0005: brfalse.s IL_0012

		IL_0007: ldstr "Cannot add a new listener while session is open."
		IL_000c: newobj instance void [mscorlib]System.ArgumentException::.ctor(string)
		IL_0011: throw

		IL_0012: ldsfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.MetricManager::m_Listeners
		IL_0017: ldarg.0
		IL_0018: callvirt instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
		IL_001d: pop
		IL_001e: ret
	} // end of method MetricManager::AddListener

	.method public hidebysig static 
		void OpenSession () cil managed 
	{
		// Method begins at RVA 0x2420
		// Header size: 12
		// Code size: 89 (0x59)
		.maxstack 2
		.locals init (
			[0] class EALA.Metrics.IMetricsListener,
			[1] class [mscorlib]System.Collections.IEnumerator,
			[2] class [mscorlib]System.IDisposable
		)

		IL_0000: ldsfld bool EALA.Metrics.MetricManager::m_SessionOpen
		IL_0005: brfalse.s IL_0012

		IL_0007: ldstr "Session already opened."
		IL_000c: newobj instance void [mscorlib]System.ArgumentException::.ctor(string)
		IL_0011: throw

		IL_0012: ldsfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.MetricManager::m_Listeners
		IL_0017: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.ArrayList::GetEnumerator()
		IL_001c: stloc.1
		.try
		{
			IL_001d: br.s IL_0031
			// loop start (head: IL_0031)
				IL_001f: ldloc.1
				IL_0020: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
				IL_0025: castclass EALA.Metrics.IMetricsListener
				IL_002a: stloc.0
				IL_002b: ldloc.0
				IL_002c: callvirt instance void EALA.Metrics.IMetricsListener::Open()

				IL_0031: ldloc.1
				IL_0032: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
				IL_0037: brtrue.s IL_001f
			// end loop

			IL_0039: leave.s IL_004c
		} // end .try
		finally
		{
			IL_003b: ldloc.1
			IL_003c: isinst [mscorlib]System.IDisposable
			IL_0041: stloc.2
			IL_0042: ldloc.2
			IL_0043: brfalse.s IL_004b

			IL_0045: ldloc.2
			IL_0046: callvirt instance void [mscorlib]System.IDisposable::Dispose()

			IL_004b: endfinally
		} // end handler

		IL_004c: ldc.i4.1
		IL_004d: stsfld bool EALA.Metrics.MetricManager::m_SessionOpen
		IL_0052: ldc.i4.1
		IL_0053: stsfld bool EALA.Metrics.MetricManager::m_Enabled
		IL_0058: ret
	} // end of method MetricManager::OpenSession

	.method public hidebysig specialname static 
		bool get_Enabled () cil managed 
	{
		// Method begins at RVA 0x2498
		// Header size: 1
		// Code size: 6 (0x6)
		.maxstack 8

		IL_0000: ldsfld bool EALA.Metrics.MetricManager::m_Enabled
		IL_0005: ret
	} // end of method MetricManager::get_Enabled

	.method public hidebysig specialname static 
		void set_Enabled (
			bool 'value'
		) cil managed 
	{
		// Method begins at RVA 0x249f
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: stsfld bool EALA.Metrics.MetricManager::m_Enabled
		IL_0006: ret
	} // end of method MetricManager::set_Enabled

	.method public hidebysig static 
		void CloseSession () cil managed 
	{
		// Method begins at RVA 0x24a8
		// Header size: 12
		// Code size: 83 (0x53)
		.maxstack 2
		.locals init (
			[0] class EALA.Metrics.IMetricsListener,
			[1] class [mscorlib]System.Collections.IEnumerator,
			[2] class [mscorlib]System.IDisposable
		)

		IL_0000: ldsfld bool EALA.Metrics.MetricManager::m_SessionOpen
		IL_0005: brtrue.s IL_0012

		IL_0007: ldstr "Session not opened."
		IL_000c: newobj instance void [mscorlib]System.ArgumentException::.ctor(string)
		IL_0011: throw

		IL_0012: ldsfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.MetricManager::m_Listeners
		IL_0017: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.ArrayList::GetEnumerator()
		IL_001c: stloc.1
		.try
		{
			IL_001d: br.s IL_0031
			// loop start (head: IL_0031)
				IL_001f: ldloc.1
				IL_0020: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
				IL_0025: castclass EALA.Metrics.IMetricsListener
				IL_002a: stloc.0
				IL_002b: ldloc.0
				IL_002c: callvirt instance void EALA.Metrics.IMetricsListener::Close()

				IL_0031: ldloc.1
				IL_0032: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
				IL_0037: brtrue.s IL_001f
			// end loop

			IL_0039: leave.s IL_004c
		} // end .try
		finally
		{
			IL_003b: ldloc.1
			IL_003c: isinst [mscorlib]System.IDisposable
			IL_0041: stloc.2
			IL_0042: ldloc.2
			IL_0043: brfalse.s IL_004b

			IL_0045: ldloc.2
			IL_0046: callvirt instance void [mscorlib]System.IDisposable::Dispose()

			IL_004b: endfinally
		} // end handler

		IL_004c: ldc.i4.0
		IL_004d: stsfld bool EALA.Metrics.MetricManager::m_SessionOpen
		IL_0052: ret
	} // end of method MetricManager::CloseSession

	.method public hidebysig static 
		void Submit (
			string descriptorName,
			object[] datalist
		) cil managed 
	{
		.param [2]
			.custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = (
				01 00 00 00
			)
		// Method begins at RVA 0x2518
		// Header size: 12
		// Code size: 30 (0x1e)
		.maxstack 3
		.locals init (
			[0] class EALA.Metrics.MetricDescriptor
		)

		IL_0000: ldnull
		IL_0001: stloc.0
		IL_0002: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor> EALA.Metrics.MetricManager::_DescriptorMap
		IL_0007: ldarg.0
		IL_0008: callvirt instance string [mscorlib]System.String::ToLower()
		IL_000d: ldloca.s 0
		IL_000f: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor>::TryGetValue(!0, !1&)
		IL_0014: brfalse.s IL_001d

		IL_0016: ldloc.0
		IL_0017: ldarg.1
		IL_0018: call void EALA.Metrics.MetricManager::SubmitInternal(class EALA.Metrics.MetricDescriptor, object[])

		IL_001d: ret
	} // end of method MetricManager::Submit

	.method public hidebysig static 
		void Submit (
			class EALA.Metrics.MetricDescriptor descriptor,
			object[] datalist
		) cil managed 
	{
		.param [2]
			.custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = (
				01 00 00 00
			)
		// Method begins at RVA 0x2542
		// Header size: 1
		// Code size: 31 (0x1f)
		.maxstack 8

		IL_0000: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor> EALA.Metrics.MetricManager::_DescriptorMap
		IL_0005: ldarg.0
		IL_0006: callvirt instance string EALA.Metrics.MetricDescriptor::get_Name()
		IL_000b: callvirt instance string [mscorlib]System.String::ToLower()
		IL_0010: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor>::ContainsKey(!0)
		IL_0015: brfalse.s IL_001e

		IL_0017: ldarg.0
		IL_0018: ldarg.1
		IL_0019: call void EALA.Metrics.MetricManager::SubmitInternal(class EALA.Metrics.MetricDescriptor, object[])

		IL_001e: ret
	} // end of method MetricManager::Submit

	.method private hidebysig static 
		void SubmitInternal (
			class EALA.Metrics.MetricDescriptor descriptor,
			object[] samples
		) cil managed 
	{
		.param [2]
			.custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = (
				01 00 00 00
			)
		// Method begins at RVA 0x2564
		// Header size: 12
		// Code size: 286 (0x11e)
		.maxstack 4
		.locals init (
			[0] object,
			[1] object,
			[2] class EALA.Metrics.IMetricsListener,
			[3] valuetype EALA.Metrics.MetricType,
			[4] valuetype [mscorlib]System.TimeSpan,
			[5] class [mscorlib]System.Collections.IEnumerator,
			[6] class [mscorlib]System.IDisposable
		)

		IL_0000: ldsfld bool EALA.Metrics.MetricManager::m_SessionOpen
		IL_0005: brfalse.s IL_000e

		IL_0007: ldsfld bool EALA.Metrics.MetricManager::m_Enabled
		IL_000c: brtrue.s IL_000f

		IL_000e: ret

		IL_000f: ldarg.1
		IL_0010: ldlen
		IL_0011: conv.i4
		IL_0012: brtrue.s IL_0015

		IL_0014: ret

		IL_0015: ldarg.1
		IL_0016: ldc.i4.0
		IL_0017: ldelem.ref
		IL_0018: stloc.0
		IL_0019: ldnull
		IL_001a: stloc.1
		IL_001b: ldarg.0
		IL_001c: callvirt instance valuetype EALA.Metrics.MetricType EALA.Metrics.MetricDescriptor::get_Type()
		IL_0021: stloc.3
		IL_0022: ldloc.3
		IL_0023: switch (IL_0065, IL_0057, IL_0049, IL_00c1, IL_00b8, IL_009c, IL_00aa)

		IL_0044: br IL_00cf

		IL_0049: ldloc.0
		IL_004a: call int32 [mscorlib]System.Convert::ToInt32(object)
		IL_004f: box [mscorlib]System.Int32
		IL_0054: stloc.1
		IL_0055: br.s IL_00d1

		IL_0057: ldloc.0
		IL_0058: call int64 [mscorlib]System.Convert::ToInt64(object)
		IL_005d: box [mscorlib]System.Int64
		IL_0062: stloc.1
		IL_0063: br.s IL_00d1

		IL_0065: ldloc.0
		IL_0066: callvirt instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
		IL_006b: ldtoken [mscorlib]System.TimeSpan
		IL_0070: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_0075: bne.un.s IL_008e

		IL_0077: ldloc.0
		IL_0078: unbox.any [mscorlib]System.TimeSpan
		IL_007d: stloc.s 4
		IL_007f: ldloca.s 4
		IL_0081: call instance float64 [mscorlib]System.TimeSpan::get_TotalSeconds()
		IL_0086: box [mscorlib]System.Double
		IL_008b: stloc.1
		IL_008c: br.s IL_00d1

		IL_008e: ldloc.0
		IL_008f: call float64 [mscorlib]System.Convert::ToDouble(object)
		IL_0094: box [mscorlib]System.Double
		IL_0099: stloc.1
		IL_009a: br.s IL_00d1

		IL_009c: ldloc.0
		IL_009d: call bool [mscorlib]System.Convert::ToBoolean(object)
		IL_00a2: box [mscorlib]System.Boolean
		IL_00a7: stloc.1
		IL_00a8: br.s IL_00d1

		IL_00aa: ldloc.0
		IL_00ab: call bool [mscorlib]System.Convert::ToBoolean(object)
		IL_00b0: box [mscorlib]System.Boolean
		IL_00b5: stloc.1
		IL_00b6: br.s IL_00d1

		IL_00b8: ldloc.0
		IL_00b9: call string [mscorlib]System.Convert::ToString(object)
		IL_00be: stloc.1
		IL_00bf: br.s IL_00d1

		IL_00c1: ldloc.0
		IL_00c2: call float32 [mscorlib]System.Convert::ToSingle(object)
		IL_00c7: box [mscorlib]System.Single
		IL_00cc: stloc.1
		IL_00cd: br.s IL_00d1

		IL_00cf: ldloc.0
		IL_00d0: stloc.1

		IL_00d1: ldarg.1
		IL_00d2: ldc.i4.0
		IL_00d3: ldloc.1
		IL_00d4: stelem.ref
		IL_00d5: ldsfld class [mscorlib]System.Collections.ArrayList EALA.Metrics.MetricManager::m_Listeners
		IL_00da: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.ArrayList::GetEnumerator()
		IL_00df: stloc.s 5
		.try
		{
			IL_00e1: br.s IL_00fd
			// loop start (head: IL_00fd)
				IL_00e3: ldloc.s 5
				IL_00e5: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
				IL_00ea: castclass EALA.Metrics.IMetricsListener
				IL_00ef: stloc.2
				IL_00f0: ldloc.2
				IL_00f1: ldarg.0
				IL_00f2: ldarg.1
				IL_00f3: newobj instance void EALA.Metrics.Metric::.ctor(class EALA.Metrics.MetricDescriptor, object[])
				IL_00f8: callvirt instance void EALA.Metrics.IMetricsListener::SubmitMetrics(class EALA.Metrics.Metric)

				IL_00fd: ldloc.s 5
				IL_00ff: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
				IL_0104: brtrue.s IL_00e3
			// end loop

			IL_0106: leave.s IL_011d
		} // end .try
		finally
		{
			IL_0108: ldloc.s 5
			IL_010a: isinst [mscorlib]System.IDisposable
			IL_010f: stloc.s 6
			IL_0111: ldloc.s 6
			IL_0113: brfalse.s IL_011c

			IL_0115: ldloc.s 6
			IL_0117: callvirt instance void [mscorlib]System.IDisposable::Dispose()

			IL_011c: endfinally
		} // end handler

		IL_011d: ret
	} // end of method MetricManager::SubmitInternal

	.method public hidebysig static 
		class EALA.Metrics.MetricDescriptor GetDescriptor (
			string name,
			valuetype EALA.Metrics.MetricType 'type',
			string description
		) cil managed 
	{
		// Method begins at RVA 0x26a0
		// Header size: 12
		// Code size: 47 (0x2f)
		.maxstack 4
		.locals init (
			[0] string,
			[1] class EALA.Metrics.MetricDescriptor
		)

		IL_0000: ldarg.0
		IL_0001: callvirt instance string [mscorlib]System.String::ToLower()
		IL_0006: stloc.0
		IL_0007: ldnull
		IL_0008: stloc.1
		IL_0009: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor> EALA.Metrics.MetricManager::_DescriptorMap
		IL_000e: ldloc.0
		IL_000f: ldloca.s 1
		IL_0011: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor>::TryGetValue(!0, !1&)
		IL_0016: brtrue.s IL_002d

		IL_0018: ldarg.0
		IL_0019: ldarg.1
		IL_001a: ldarg.2
		IL_001b: newobj instance void EALA.Metrics.MetricDescriptor::.ctor(string, valuetype EALA.Metrics.MetricType, string)
		IL_0020: stloc.1
		IL_0021: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor> EALA.Metrics.MetricManager::_DescriptorMap
		IL_0026: ldloc.0
		IL_0027: ldloc.1
		IL_0028: callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, class EALA.Metrics.MetricDescriptor>::Add(!0, !1)

		IL_002d: ldloc.1
		IL_002e: ret
	} // end of method MetricManager::GetDescriptor

	.method public hidebysig specialname static 
		class EALA.Metrics.MetricApplicationData get_AppData () cil managed 
	{
		// Method begins at RVA 0x26db
		// Header size: 1
		// Code size: 6 (0x6)
		.maxstack 8

		IL_0000: ldsfld class EALA.Metrics.MetricApplicationData EALA.Metrics.MetricManager::m_AppData
		IL_0005: ret
	} // end of method MetricManager::get_AppData

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x26e2
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method MetricManager::.ctor

	// Properties
	.property bool Enabled()
	{
		.get bool EALA.Metrics.MetricManager::get_Enabled()
		.set void EALA.Metrics.MetricManager::set_Enabled(bool)
	}
	.property class EALA.Metrics.MetricApplicationData AppData()
	{
		.get class EALA.Metrics.MetricApplicationData EALA.Metrics.MetricManager::get_AppData()
	}

} // end of class EALA.Metrics.MetricManager

.class public auto ansi beforefieldinit EALA.Metrics.MetricTimer
	extends [mscorlib]System.Object
	implements [mscorlib]System.IDisposable
{
	// Fields
	.field private valuetype [mscorlib]System.DateTime m_StartTime
	.field private string m_UserData
	.field private string m_DescriptorName

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			string descriptorName,
			string userData
		) cil managed 
	{
		// Method begins at RVA 0x26ea
		// Header size: 1
		// Code size: 32 (0x20)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
		IL_0006: stfld valuetype [mscorlib]System.DateTime EALA.Metrics.MetricTimer::m_StartTime
		IL_000b: ldarg.0
		IL_000c: call instance void [mscorlib]System.Object::.ctor()
		IL_0011: ldarg.0
		IL_0012: ldarg.1
		IL_0013: stfld string EALA.Metrics.MetricTimer::m_DescriptorName
		IL_0018: ldarg.0
		IL_0019: ldarg.2
		IL_001a: stfld string EALA.Metrics.MetricTimer::m_UserData
		IL_001f: ret
	} // end of method MetricTimer::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class EALA.Metrics.MetricDescriptor descriptor,
			string userData
		) cil managed 
	{
		// Method begins at RVA 0x270b
		// Header size: 1
		// Code size: 14 (0xe)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: callvirt instance string EALA.Metrics.MetricDescriptor::get_Name()
		IL_0007: ldarg.2
		IL_0008: call instance void EALA.Metrics.MetricTimer::.ctor(string, string)
		IL_000d: ret
	} // end of method MetricTimer::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			string descriptorName
		) cil managed 
	{
		// Method begins at RVA 0x271a
		// Header size: 1
		// Code size: 9 (0x9)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldnull
		IL_0003: call instance void EALA.Metrics.MetricTimer::.ctor(string, string)
		IL_0008: ret
	} // end of method MetricTimer::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			class EALA.Metrics.MetricDescriptor descriptor
		) cil managed 
	{
		// Method begins at RVA 0x2724
		// Header size: 1
		// Code size: 14 (0xe)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: callvirt instance string EALA.Metrics.MetricDescriptor::get_Name()
		IL_0007: ldnull
		IL_0008: call instance void EALA.Metrics.MetricTimer::.ctor(string, string)
		IL_000d: ret
	} // end of method MetricTimer::.ctor

	.method public final hidebysig newslot virtual 
		instance void Dispose () cil managed 
	{
		// Method begins at RVA 0x2734
		// Header size: 12
		// Code size: 97 (0x61)
		.maxstack 4
		.locals init (
			[0] valuetype [mscorlib]System.TimeSpan,
			[1] object[],
			[2] object[]
		)

		IL_0000: call valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
		IL_0005: ldarg.0
		IL_0006: ldfld valuetype [mscorlib]System.DateTime EALA.Metrics.MetricTimer::m_StartTime
		IL_000b: call valuetype [mscorlib]System.TimeSpan [mscorlib]System.DateTime::op_Subtraction(valuetype [mscorlib]System.DateTime, valuetype [mscorlib]System.DateTime)
		IL_0010: stloc.0
		IL_0011: ldarg.0
		IL_0012: ldfld string EALA.Metrics.MetricTimer::m_UserData
		IL_0017: call bool [mscorlib]System.String::IsNullOrEmpty(string)
		IL_001c: brtrue.s IL_0044

		IL_001e: ldarg.0
		IL_001f: ldfld string EALA.Metrics.MetricTimer::m_DescriptorName
		IL_0024: ldc.i4.2
		IL_0025: newarr [mscorlib]System.Object
		IL_002a: stloc.1
		IL_002b: ldloc.1
		IL_002c: ldc.i4.0
		IL_002d: ldloc.0
		IL_002e: box [mscorlib]System.TimeSpan
		IL_0033: stelem.ref
		IL_0034: ldloc.1
		IL_0035: ldc.i4.1
		IL_0036: ldarg.0
		IL_0037: ldfld string EALA.Metrics.MetricTimer::m_UserData
		IL_003c: stelem.ref
		IL_003d: ldloc.1
		IL_003e: call void EALA.Metrics.MetricManager::Submit(string, object[])
		IL_0043: ret

		IL_0044: ldarg.0
		IL_0045: ldfld string EALA.Metrics.MetricTimer::m_DescriptorName
		IL_004a: ldc.i4.1
		IL_004b: newarr [mscorlib]System.Object
		IL_0050: stloc.2
		IL_0051: ldloc.2
		IL_0052: ldc.i4.0
		IL_0053: ldloc.0
		IL_0054: box [mscorlib]System.TimeSpan
		IL_0059: stelem.ref
		IL_005a: ldloc.2
		IL_005b: call void EALA.Metrics.MetricManager::Submit(string, object[])
		IL_0060: ret
	} // end of method MetricTimer::Dispose

} // end of class EALA.Metrics.MetricTimer

.class public auto ansi beforefieldinit EALA.Metrics.ReportMetrics
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x27a1
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method ReportMetrics::.ctor

	.method public hidebysig 
		instance void AddMetric (
			class EALA.Metrics.Metric m
		) cil managed 
	{
		// Method begins at RVA 0x27a9
		// Header size: 1
		// Code size: 12 (0xc)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0006: call void [mscorlib]System.Console::WriteLine(string)
		IL_000b: ret
	} // end of method ReportMetrics::AddMetric

} // end of class EALA.Metrics.ReportMetrics

.class public auto ansi beforefieldinit EALA.Metrics.SqlMetricsListener
	extends [mscorlib]System.Object
	implements EALA.Metrics.IMetricsListener
{
	// Fields
	.field private class EALA.Metrics.SqlMetricsWriter m_smw
	.field private class EALA.Metrics.ListMetric m_Metrics

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x27b6
		// Header size: 1
		// Code size: 18 (0x12)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: newobj instance void EALA.Metrics.ListMetric::.ctor()
		IL_0006: stfld class EALA.Metrics.ListMetric EALA.Metrics.SqlMetricsListener::m_Metrics
		IL_000b: ldarg.0
		IL_000c: call instance void [mscorlib]System.Object::.ctor()
		IL_0011: ret
	} // end of method SqlMetricsListener::.ctor

	.method public final hidebysig newslot virtual 
		instance void Open () cil managed 
	{
		// Method begins at RVA 0x27cc
		// Header size: 12
		// Code size: 150 (0x96)
		.maxstack 9
		.locals init (
			[0] class [mscorlib]System.Reflection.Assembly,
			[1] class [mscorlib]System.Reflection.AssemblyName,
			[2] string,
			[3] string,
			[4] class [System]System.Collections.Specialized.NameValueCollection,
			[5] string,
			[6] string,
			[7] string,
			[8] string,
			[9] string,
			[10] class [mscorlib]System.Exception
		)

		.try
		{
			IL_0000: call class [mscorlib]System.Reflection.Assembly [mscorlib]System.Reflection.Assembly::GetEntryAssembly()
			IL_0005: stloc.0
			IL_0006: ldloc.0
			IL_0007: callvirt instance class [mscorlib]System.Reflection.AssemblyName [mscorlib]System.Reflection.Assembly::GetName()
			IL_000c: stloc.1
			IL_000d: ldloc.1
			IL_000e: callvirt instance string [mscorlib]System.Reflection.AssemblyName::get_Name()
			IL_0013: stloc.2
			IL_0014: ldloc.1
			IL_0015: callvirt instance class [mscorlib]System.Version [mscorlib]System.Reflection.AssemblyName::get_Version()
			IL_001a: callvirt instance string [mscorlib]System.Object::ToString()
			IL_001f: stloc.3
			IL_0020: ldstr "SqlMetricsListener"
			IL_0025: call object [System.Configuration]System.Configuration.ConfigurationManager::GetSection(string)
			IL_002a: castclass [System]System.Collections.Specialized.NameValueCollection
			IL_002f: stloc.s 4
			IL_0031: ldloc.s 4
			IL_0033: ldstr "address"
			IL_0038: callvirt instance string [System]System.Collections.Specialized.NameValueCollection::get_Item(string)
			IL_003d: stloc.s 5
			IL_003f: ldloc.s 4
			IL_0041: ldstr "database"
			IL_0046: callvirt instance string [System]System.Collections.Specialized.NameValueCollection::get_Item(string)
			IL_004b: stloc.s 6
			IL_004d: ldloc.s 4
			IL_004f: ldstr "user"
			IL_0054: callvirt instance string [System]System.Collections.Specialized.NameValueCollection::get_Item(string)
			IL_0059: stloc.s 7
			IL_005b: ldloc.s 4
			IL_005d: ldstr "password"
			IL_0062: callvirt instance string [System]System.Collections.Specialized.NameValueCollection::get_Item(string)
			IL_0067: stloc.s 8
			IL_0069: ldloc.s 4
			IL_006b: ldstr "projectname"
			IL_0070: callvirt instance string [System]System.Collections.Specialized.NameValueCollection::get_Item(string)
			IL_0075: stloc.s 9
			IL_0077: ldarg.0
			IL_0078: ldloc.2
			IL_0079: ldloc.3
			IL_007a: ldloc.s 9
			IL_007c: ldloc.s 5
			IL_007e: ldloc.s 6
			IL_0080: ldloc.s 7
			IL_0082: ldloc.s 8
			IL_0084: newobj instance void EALA.Metrics.SqlMetricsWriter::.ctor(string, string, string, string, string, string, string)
			IL_0089: stfld class EALA.Metrics.SqlMetricsWriter EALA.Metrics.SqlMetricsListener::m_smw
			IL_008e: leave.s IL_0095
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0090: stloc.s 10
			IL_0092: ldloc.s 10
			IL_0094: throw
		} // end handler

		IL_0095: ret
	} // end of method SqlMetricsListener::Open

	.method public final hidebysig newslot virtual 
		instance void Close () cil managed 
	{
		// Method begins at RVA 0x2880
		// Header size: 1
		// Code size: 30 (0x1e)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class EALA.Metrics.SqlMetricsWriter EALA.Metrics.SqlMetricsListener::m_smw
		IL_0006: ldarg.0
		IL_0007: ldfld class EALA.Metrics.ListMetric EALA.Metrics.SqlMetricsListener::m_Metrics
		IL_000c: callvirt instance class [mscorlib]System.Collections.ArrayList EALA.Metrics.ListMetric::get_Metrics()
		IL_0011: callvirt instance void EALA.Metrics.SqlMetricsWriter::Submit(class [mscorlib]System.Collections.ICollection)
		IL_0016: ldarg.0
		IL_0017: ldnull
		IL_0018: stfld class EALA.Metrics.SqlMetricsWriter EALA.Metrics.SqlMetricsListener::m_smw
		IL_001d: ret
	} // end of method SqlMetricsListener::Close

	.method public final hidebysig newslot virtual 
		instance void SubmitMetrics (
			class EALA.Metrics.Metric m
		) cil managed 
	{
		// Method begins at RVA 0x289f
		// Header size: 1
		// Code size: 13 (0xd)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class EALA.Metrics.ListMetric EALA.Metrics.SqlMetricsListener::m_Metrics
		IL_0006: ldarg.1
		IL_0007: callvirt instance void EALA.Metrics.ListMetric::AddMetric(class EALA.Metrics.Metric)
		IL_000c: ret
	} // end of method SqlMetricsListener::SubmitMetrics

} // end of class EALA.Metrics.SqlMetricsListener

.class public auto ansi beforefieldinit EALA.Metrics.SqlMetricsWriter
	extends [mscorlib]System.Object
{
	// Fields
	.field private int32 m_SessionId
	.field private class [mscorlib]System.Collections.Hashtable m_MetricDescriptors
	.field private string m_UserName
	.field private string m_MachineName
	.field private string m_BuildInfo
	.field private static string m_SqlParameters

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			string applicationName,
			string applicationDescription,
			string applicationVersion,
			string projectName,
			string projectDescription,
			string databaseServer,
			string databaseName,
			string databaseUser,
			string databasePassword
		) cil managed 
	{
		// Method begins at RVA 0x28b0
		// Header size: 12
		// Code size: 82 (0x52)
		.maxstack 10

		IL_0000: ldarg.0
		IL_0001: newobj instance void [mscorlib]System.Collections.Hashtable::.ctor()
		IL_0006: stfld class [mscorlib]System.Collections.Hashtable EALA.Metrics.SqlMetricsWriter::m_MetricDescriptors
		IL_000b: ldarg.0
		IL_000c: call class [mscorlib]System.Security.Principal.WindowsIdentity [mscorlib]System.Security.Principal.WindowsIdentity::GetCurrent()
		IL_0011: callvirt instance string [mscorlib]System.Security.Principal.WindowsIdentity::get_Name()
		IL_0016: callvirt instance string [mscorlib]System.Object::ToString()
		IL_001b: stfld string EALA.Metrics.SqlMetricsWriter::m_UserName
		IL_0020: ldarg.0
		IL_0021: call string [mscorlib]System.Environment::get_MachineName()
		IL_0026: stfld string EALA.Metrics.SqlMetricsWriter::m_MachineName
		IL_002b: ldarg.0
		IL_002c: ldstr ""
		IL_0031: stfld string EALA.Metrics.SqlMetricsWriter::m_BuildInfo
		IL_0036: ldarg.0
		IL_0037: call instance void [mscorlib]System.Object::.ctor()
		IL_003c: ldarg.0
		IL_003d: ldarg.1
		IL_003e: ldarg.2
		IL_003f: ldarg.3
		IL_0040: ldarg.s projectName
		IL_0042: ldarg.s projectDescription
		IL_0044: ldarg.s databaseServer
		IL_0046: ldarg.s databaseName
		IL_0048: ldarg.s databaseUser
		IL_004a: ldarg.s databasePassword
		IL_004c: call instance void EALA.Metrics.SqlMetricsWriter::SetUpDatabase(string, string, string, string, string, string, string, string, string)
		IL_0051: ret
	} // end of method SqlMetricsWriter::.ctor

	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			string applicationName,
			string applicationVersion,
			string projectName,
			string databaseServer,
			string databaseName,
			string databaseUser,
			string databasePassword
		) cil managed 
	{
		// Method begins at RVA 0x2910
		// Header size: 12
		// Code size: 88 (0x58)
		.maxstack 10

		IL_0000: ldarg.0
		IL_0001: newobj instance void [mscorlib]System.Collections.Hashtable::.ctor()
		IL_0006: stfld class [mscorlib]System.Collections.Hashtable EALA.Metrics.SqlMetricsWriter::m_MetricDescriptors
		IL_000b: ldarg.0
		IL_000c: call class [mscorlib]System.Security.Principal.WindowsIdentity [mscorlib]System.Security.Principal.WindowsIdentity::GetCurrent()
		IL_0011: callvirt instance string [mscorlib]System.Security.Principal.WindowsIdentity::get_Name()
		IL_0016: callvirt instance string [mscorlib]System.Object::ToString()
		IL_001b: stfld string EALA.Metrics.SqlMetricsWriter::m_UserName
		IL_0020: ldarg.0
		IL_0021: call string [mscorlib]System.Environment::get_MachineName()
		IL_0026: stfld string EALA.Metrics.SqlMetricsWriter::m_MachineName
		IL_002b: ldarg.0
		IL_002c: ldstr ""
		IL_0031: stfld string EALA.Metrics.SqlMetricsWriter::m_BuildInfo
		IL_0036: ldarg.0
		IL_0037: call instance void [mscorlib]System.Object::.ctor()
		IL_003c: ldarg.0
		IL_003d: ldarg.1
		IL_003e: ldstr ""
		IL_0043: ldarg.2
		IL_0044: ldarg.3
		IL_0045: ldstr ""
		IL_004a: ldarg.s databaseServer
		IL_004c: ldarg.s databaseName
		IL_004e: ldarg.s databaseUser
		IL_0050: ldarg.s databasePassword
		IL_0052: call instance void EALA.Metrics.SqlMetricsWriter::SetUpDatabase(string, string, string, string, string, string, string, string, string)
		IL_0057: ret
	} // end of method SqlMetricsWriter::.ctor

	.method private hidebysig 
		instance void SetUpDatabase (
			string applicationName,
			string applicationDescription,
			string applicationVersion,
			string projectName,
			string projectDescription,
			string databaseServer,
			string databaseName,
			string databaseUser,
			string databasePassword
		) cil managed 
	{
		// Method begins at RVA 0x2974
		// Header size: 12
		// Code size: 81 (0x51)
		.maxstack 10
		.locals init (
			[0] object[]
		)

		IL_0000: ldstr "data source=\"{0}\";initial catalog={1};user id={2};pwd={3}"
		IL_0005: ldc.i4.4
		IL_0006: newarr [mscorlib]System.Object
		IL_000b: stloc.0
		IL_000c: ldloc.0
		IL_000d: ldc.i4.0
		IL_000e: ldarg.s databaseServer
		IL_0010: stelem.ref
		IL_0011: ldloc.0
		IL_0012: ldc.i4.1
		IL_0013: ldarg.s databaseName
		IL_0015: stelem.ref
		IL_0016: ldloc.0
		IL_0017: ldc.i4.2
		IL_0018: ldarg.s databaseUser
		IL_001a: stelem.ref
		IL_001b: ldloc.0
		IL_001c: ldc.i4.3
		IL_001d: ldarg.s databasePassword
		IL_001f: stelem.ref
		IL_0020: ldloc.0
		IL_0021: call string [mscorlib]System.String::Format(string, object[])
		IL_0026: stsfld string EALA.Metrics.SqlMetricsWriter::m_SqlParameters
		IL_002b: ldarg.0
		IL_002c: ldarg.0
		IL_002d: ldarg.1
		IL_002e: ldarg.3
		IL_002f: ldarg.2
		IL_0030: ldarg.s projectName
		IL_0032: ldarg.s projectDescription
		IL_0034: ldarg.0
		IL_0035: ldfld string EALA.Metrics.SqlMetricsWriter::m_UserName
		IL_003a: ldarg.0
		IL_003b: ldfld string EALA.Metrics.SqlMetricsWriter::m_MachineName
		IL_0040: ldarg.0
		IL_0041: ldfld string EALA.Metrics.SqlMetricsWriter::m_BuildInfo
		IL_0046: call instance int32 EALA.Metrics.SqlMetricsWriter::CreateSession(string, string, string, string, string, string, string, string)
		IL_004b: stfld int32 EALA.Metrics.SqlMetricsWriter::m_SessionId
		IL_0050: ret
	} // end of method SqlMetricsWriter::SetUpDatabase

	.method private hidebysig 
		instance int32 CreateSession (
			string applicationName,
			string applicationVersion,
			string applicationDescription,
			string projectName,
			string projectDescription,
			string username,
			string machineName,
			string buildInfo
		) cil managed 
	{
		// Method begins at RVA 0x29d4
		// Header size: 12
		// Code size: 284 (0x11c)
		.maxstack 3
		.locals init (
			[0] int32,
			[1] class [System.Data]System.Data.SqlClient.SqlConnection,
			[2] string,
			[3] class [System.Data]System.Data.SqlClient.SqlCommand,
			[4] class [System.Data]System.Data.SqlClient.SqlParameter,
			[5] class [mscorlib]System.Exception
		)

		IL_0000: ldc.i4.0
		IL_0001: stloc.0
		.try
		{
			IL_0002: ldsfld string EALA.Metrics.SqlMetricsWriter::m_SqlParameters
			IL_0007: newobj instance void [System.Data]System.Data.SqlClient.SqlConnection::.ctor(string)
			IL_000c: stloc.1
			.try
			{
				IL_000d: ldloc.1
				IL_000e: callvirt instance void [System.Data]System.Data.Common.DbConnection::Open()
				IL_0013: ldstr "InsertSession"
				IL_0018: stloc.2
				IL_0019: ldloc.2
				IL_001a: ldloc.1
				IL_001b: newobj instance void [System.Data]System.Data.SqlClient.SqlCommand::.ctor(string, class [System.Data]System.Data.SqlClient.SqlConnection)
				IL_0020: stloc.3
				IL_0021: ldloc.3
				IL_0022: ldc.i4.4
				IL_0023: callvirt instance void [System.Data]System.Data.Common.DbCommand::set_CommandType(valuetype [System.Data]System.Data.CommandType)
				IL_0028: ldloc.3
				IL_0029: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_002e: ldstr "@ApplicationName"
				IL_0033: ldarg.1
				IL_0034: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_0039: pop
				IL_003a: ldloc.3
				IL_003b: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_0040: ldstr "@ApplicationVersion"
				IL_0045: ldarg.2
				IL_0046: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_004b: pop
				IL_004c: ldarg.3
				IL_004d: callvirt instance int32 [mscorlib]System.String::get_Length()
				IL_0052: ldc.i4.0
				IL_0053: ble.s IL_0067

				IL_0055: ldloc.3
				IL_0056: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_005b: ldstr "@ApplicationDescription"
				IL_0060: ldarg.3
				IL_0061: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_0066: pop

				IL_0067: ldloc.3
				IL_0068: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_006d: ldstr "@ProjectName"
				IL_0072: ldarg.s projectName
				IL_0074: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_0079: pop
				IL_007a: ldarg.s projectDescription
				IL_007c: callvirt instance int32 [mscorlib]System.String::get_Length()
				IL_0081: ldc.i4.0
				IL_0082: ble.s IL_0097

				IL_0084: ldloc.3
				IL_0085: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_008a: ldstr "@ProjectDescription"
				IL_008f: ldarg.s projectDescription
				IL_0091: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_0096: pop

				IL_0097: ldloc.3
				IL_0098: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_009d: ldstr "@Username"
				IL_00a2: ldarg.s username
				IL_00a4: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_00a9: pop
				IL_00aa: ldloc.3
				IL_00ab: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_00b0: ldstr "@MachineName"
				IL_00b5: ldarg.s machineName
				IL_00b7: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_00bc: pop
				IL_00bd: ldloc.3
				IL_00be: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_00c3: ldstr "BuildInfo"
				IL_00c8: ldarg.s buildInfo
				IL_00ca: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
				IL_00cf: pop
				IL_00d0: ldstr "@SessionId"
				IL_00d5: ldc.i4.8
				IL_00d6: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor(string, valuetype [System.Data]System.Data.SqlDbType)
				IL_00db: stloc.s 4
				IL_00dd: ldloc.s 4
				IL_00df: ldc.i4.2
				IL_00e0: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Direction(valuetype [System.Data]System.Data.ParameterDirection)
				IL_00e5: ldloc.3
				IL_00e6: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
				IL_00eb: ldloc.s 4
				IL_00ed: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
				IL_00f2: pop
				IL_00f3: ldloc.3
				IL_00f4: callvirt instance int32 [System.Data]System.Data.Common.DbCommand::ExecuteNonQuery()
				IL_00f9: pop
				IL_00fa: ldloc.s 4
				IL_00fc: callvirt instance object [System.Data]System.Data.Common.DbParameter::get_Value()
				IL_0101: unbox.any [mscorlib]System.Int32
				IL_0106: stloc.0
				IL_0107: leave.s IL_0113
			} // end .try
			finally
			{
				IL_0109: ldloc.1
				IL_010a: brfalse.s IL_0112

				IL_010c: ldloc.1
				IL_010d: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_0112: endfinally
			} // end handler

			IL_0113: leave.s IL_011a
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0115: stloc.s 5
			IL_0117: ldloc.s 5
			IL_0119: throw
		} // end handler

		IL_011a: ldloc.0
		IL_011b: ret
	} // end of method SqlMetricsWriter::CreateSession

	.method private hidebysig 
		instance void UpdateMetricDescriptors () cil managed 
	{
		// Method begins at RVA 0x2b30
		// Header size: 12
		// Code size: 346 (0x15a)
		.maxstack 3
		.locals init (
			[0] class [System.Data]System.Data.SqlClient.SqlConnection,
			[1] string,
			[2] class [mscorlib]System.Collections.ArrayList,
			[3] class EALA.Metrics.MetricDescriptor,
			[4] class [System.Data]System.Data.SqlClient.SqlCommand,
			[5] class [System.Data]System.Data.SqlClient.SqlParameter,
			[6] class [mscorlib]System.Exception,
			[7] class [mscorlib]System.Collections.IEnumerator,
			[8] class [mscorlib]System.IDisposable
		)

		.try
		{
			IL_0000: ldsfld string EALA.Metrics.SqlMetricsWriter::m_SqlParameters
			IL_0005: newobj instance void [System.Data]System.Data.SqlClient.SqlConnection::.ctor(string)
			IL_000a: stloc.0
			.try
			{
				IL_000b: ldloc.0
				IL_000c: callvirt instance void [System.Data]System.Data.Common.DbConnection::Open()
				IL_0011: ldstr "RetrieveMetricNameId"
				IL_0016: stloc.1
				IL_0017: ldarg.0
				IL_0018: ldfld class [mscorlib]System.Collections.Hashtable EALA.Metrics.SqlMetricsWriter::m_MetricDescriptors
				IL_001d: callvirt instance class [mscorlib]System.Collections.ICollection [mscorlib]System.Collections.Hashtable::get_Keys()
				IL_0022: newobj instance void [mscorlib]System.Collections.ArrayList::.ctor(class [mscorlib]System.Collections.ICollection)
				IL_0027: stloc.2
				IL_0028: ldloc.2
				IL_0029: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.ArrayList::GetEnumerator()
				IL_002e: stloc.s 7
				.try
				{
					IL_0030: br IL_0123
					// loop start (head: IL_0123)
						IL_0035: ldloc.s 7
						IL_0037: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
						IL_003c: castclass EALA.Metrics.MetricDescriptor
						IL_0041: stloc.3
						IL_0042: ldloc.1
						IL_0043: ldloc.0
						IL_0044: newobj instance void [System.Data]System.Data.SqlClient.SqlCommand::.ctor(string, class [System.Data]System.Data.SqlClient.SqlConnection)
						IL_0049: stloc.s 4
						IL_004b: ldloc.s 4
						IL_004d: ldc.i4.4
						IL_004e: callvirt instance void [System.Data]System.Data.Common.DbCommand::set_CommandType(valuetype [System.Data]System.Data.CommandType)
						IL_0053: ldloc.s 4
						IL_0055: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_005a: ldstr "@MetricName"
						IL_005f: ldloc.3
						IL_0060: callvirt instance string EALA.Metrics.MetricDescriptor::get_Name()
						IL_0065: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
						IL_006a: pop
						IL_006b: ldloc.s 4
						IL_006d: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_0072: ldstr "@MetricType"
						IL_0077: ldloc.3
						IL_0078: callvirt instance valuetype EALA.Metrics.MetricType EALA.Metrics.MetricDescriptor::get_Type()
						IL_007d: box EALA.Metrics.MetricType
						IL_0082: callvirt instance string [mscorlib]System.Object::ToString()
						IL_0087: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
						IL_008c: pop
						IL_008d: ldloc.s 4
						IL_008f: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_0094: ldstr "@MetricTypeEnumId"
						IL_0099: ldloc.3
						IL_009a: callvirt instance valuetype EALA.Metrics.MetricType EALA.Metrics.MetricDescriptor::get_Type()
						IL_009f: box EALA.Metrics.MetricType
						IL_00a4: callvirt instance int32 [mscorlib]System.Object::GetHashCode()
						IL_00a9: box [mscorlib]System.Int32
						IL_00ae: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
						IL_00b3: pop
						IL_00b4: ldloc.3
						IL_00b5: callvirt instance string EALA.Metrics.MetricDescriptor::get_Description()
						IL_00ba: callvirt instance int32 [mscorlib]System.String::get_Length()
						IL_00bf: ldc.i4.0
						IL_00c0: ble.s IL_00da

						IL_00c2: ldloc.s 4
						IL_00c4: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_00c9: ldstr "@MetricDescription"
						IL_00ce: ldloc.3
						IL_00cf: callvirt instance string EALA.Metrics.MetricDescriptor::get_Description()
						IL_00d4: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
						IL_00d9: pop

						IL_00da: ldstr "@MetricNameId"
						IL_00df: ldc.i4.8
						IL_00e0: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor(string, valuetype [System.Data]System.Data.SqlDbType)
						IL_00e5: stloc.s 5
						IL_00e7: ldloc.s 5
						IL_00e9: ldc.i4.2
						IL_00ea: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Direction(valuetype [System.Data]System.Data.ParameterDirection)
						IL_00ef: ldloc.s 4
						IL_00f1: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_00f6: ldloc.s 5
						IL_00f8: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
						IL_00fd: pop
						IL_00fe: ldloc.s 4
						IL_0100: callvirt instance int32 [System.Data]System.Data.Common.DbCommand::ExecuteNonQuery()
						IL_0105: pop
						IL_0106: ldarg.0
						IL_0107: ldfld class [mscorlib]System.Collections.Hashtable EALA.Metrics.SqlMetricsWriter::m_MetricDescriptors
						IL_010c: ldloc.3
						IL_010d: ldloc.s 5
						IL_010f: callvirt instance object [System.Data]System.Data.Common.DbParameter::get_Value()
						IL_0114: unbox.any [mscorlib]System.Int32
						IL_0119: box [mscorlib]System.Int32
						IL_011e: callvirt instance void [mscorlib]System.Collections.Hashtable::set_Item(object, object)

						IL_0123: ldloc.s 7
						IL_0125: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
						IL_012a: brtrue IL_0035
					// end loop

					IL_012f: leave.s IL_0146
				} // end .try
				finally
				{
					IL_0131: ldloc.s 7
					IL_0133: isinst [mscorlib]System.IDisposable
					IL_0138: stloc.s 8
					IL_013a: ldloc.s 8
					IL_013c: brfalse.s IL_0145

					IL_013e: ldloc.s 8
					IL_0140: callvirt instance void [mscorlib]System.IDisposable::Dispose()

					IL_0145: endfinally
				} // end handler

				IL_0146: leave.s IL_0152
			} // end .try
			finally
			{
				IL_0148: ldloc.0
				IL_0149: brfalse.s IL_0151

				IL_014b: ldloc.0
				IL_014c: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_0151: endfinally
			} // end handler

			IL_0152: leave.s IL_0159
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0154: stloc.s 6
			IL_0156: ldloc.s 6
			IL_0158: throw
		} // end handler

		IL_0159: ret
	} // end of method SqlMetricsWriter::UpdateMetricDescriptors

	.method private hidebysig 
		instance void PopulateMetricDescriptors (
			class [mscorlib]System.Collections.ICollection metrics
		) cil managed 
	{
		// Method begins at RVA 0x2ce4
		// Header size: 12
		// Code size: 67 (0x43)
		.maxstack 3
		.locals init (
			[0] class EALA.Metrics.Metric,
			[1] class [mscorlib]System.Collections.IEnumerator,
			[2] class [mscorlib]System.IDisposable
		)

		IL_0000: ldarg.1
		IL_0001: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
		IL_0006: stloc.1
		.try
		{
			IL_0007: br.s IL_0027
			// loop start (head: IL_0027)
				IL_0009: ldloc.1
				IL_000a: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
				IL_000f: castclass EALA.Metrics.Metric
				IL_0014: stloc.0
				IL_0015: ldarg.0
				IL_0016: ldfld class [mscorlib]System.Collections.Hashtable EALA.Metrics.SqlMetricsWriter::m_MetricDescriptors
				IL_001b: ldloc.0
				IL_001c: callvirt instance class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::get_Descriptor()
				IL_0021: ldnull
				IL_0022: callvirt instance void [mscorlib]System.Collections.Hashtable::set_Item(object, object)

				IL_0027: ldloc.1
				IL_0028: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
				IL_002d: brtrue.s IL_0009
			// end loop

			IL_002f: leave.s IL_0042
		} // end .try
		finally
		{
			IL_0031: ldloc.1
			IL_0032: isinst [mscorlib]System.IDisposable
			IL_0037: stloc.2
			IL_0038: ldloc.2
			IL_0039: brfalse.s IL_0041

			IL_003b: ldloc.2
			IL_003c: callvirt instance void [mscorlib]System.IDisposable::Dispose()

			IL_0041: endfinally
		} // end handler

		IL_0042: ret
	} // end of method SqlMetricsWriter::PopulateMetricDescriptors

	.method private hidebysig 
		instance void LookUpMetricDescriptors (
			class [mscorlib]System.Collections.ICollection metrics
		) cil managed 
	{
		// Method begins at RVA 0x2d44
		// Header size: 1
		// Code size: 14 (0xe)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance void EALA.Metrics.SqlMetricsWriter::PopulateMetricDescriptors(class [mscorlib]System.Collections.ICollection)
		IL_0007: ldarg.0
		IL_0008: call instance void EALA.Metrics.SqlMetricsWriter::UpdateMetricDescriptors()
		IL_000d: ret
	} // end of method SqlMetricsWriter::LookUpMetricDescriptors

	.method private hidebysig 
		instance void InsertMetricValueIntoDatabase (
			class EALA.Metrics.Metric m,
			int32 metricEntryId
		) cil managed 
	{
		// Method begins at RVA 0x2d54
		// Header size: 12
		// Code size: 679 (0x2a7)
		.maxstack 3
		.locals init (
			[0] class [System.Data]System.Data.SqlClient.SqlConnection,
			[1] string,
			[2] bool,
			[3] object,
			[4] class [System.Data]System.Data.SqlClient.SqlCommand,
			[5] class [mscorlib]System.Type,
			[6] class [System.Data]System.Data.SqlClient.SqlParameter,
			[7] class [System.Data]System.Data.SqlClient.SqlParameter,
			[8] class [System.Data]System.Data.SqlClient.SqlParameter,
			[9] class [System.Data]System.Data.SqlClient.SqlParameter,
			[10] class [System.Data]System.Data.SqlClient.SqlParameter,
			[11] class [mscorlib]System.Exception,
			[12] object[],
			[13] int32,
			[14] int32,
			[15] float32,
			[16] float64,
			[17] valuetype [mscorlib]System.TimeSpan,
			[18] float64,
			[19] int64
		)

		.try
		{
			IL_0000: ldsfld string EALA.Metrics.SqlMetricsWriter::m_SqlParameters
			IL_0005: newobj instance void [System.Data]System.Data.SqlClient.SqlConnection::.ctor(string)
			IL_000a: stloc.0
			.try
			{
				IL_000b: ldloc.0
				IL_000c: callvirt instance void [System.Data]System.Data.Common.DbConnection::Open()
				IL_0011: ldstr "InsertMetricValueOnly"
				IL_0016: stloc.1
				IL_0017: ldc.i4.0
				IL_0018: stloc.2
				IL_0019: ldarg.1
				IL_001a: callvirt instance object[] EALA.Metrics.Metric::get_DataList()
				IL_001f: stloc.s 12
				IL_0021: ldc.i4.0
				IL_0022: stloc.s 13
				IL_0024: br IL_0288
				// loop start (head: IL_0288)
					IL_0029: ldloc.s 12
					IL_002b: ldloc.s 13
					IL_002d: ldelem.ref
					IL_002e: stloc.3
					IL_002f: ldloc.2
					IL_0030: brtrue.s IL_0039

					IL_0032: ldc.i4.1
					IL_0033: stloc.2
					IL_0034: br IL_0282

					IL_0039: ldloc.1
					IL_003a: ldloc.0
					IL_003b: newobj instance void [System.Data]System.Data.SqlClient.SqlCommand::.ctor(string, class [System.Data]System.Data.SqlClient.SqlConnection)
					IL_0040: stloc.s 4
					IL_0042: ldloc.s 4
					IL_0044: ldc.i4.4
					IL_0045: callvirt instance void [System.Data]System.Data.Common.DbCommand::set_CommandType(valuetype [System.Data]System.Data.CommandType)
					IL_004a: ldloc.3
					IL_004b: callvirt instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
					IL_0050: stloc.s 5
					IL_0052: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
					IL_0057: stloc.s 6
					IL_0059: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
					IL_005e: stloc.s 7
					IL_0060: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
					IL_0065: stloc.s 8
					IL_0067: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
					IL_006c: stloc.s 9
					IL_006e: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
					IL_0073: stloc.s 10
					IL_0075: ldloc.s 6
					IL_0077: ldstr "@IntValue"
					IL_007c: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
					IL_0081: ldloc.s 7
					IL_0083: ldstr "@StringValue"
					IL_0088: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
					IL_008d: ldloc.s 8
					IL_008f: ldstr "@FloatValue"
					IL_0094: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
					IL_0099: ldloc.s 9
					IL_009b: ldstr "@DtValue"
					IL_00a0: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
					IL_00a5: ldloc.s 10
					IL_00a7: ldstr "@LongValue"
					IL_00ac: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
					IL_00b1: ldloc.s 6
					IL_00b3: ldstr "0"
					IL_00b8: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_00bd: ldloc.s 7
					IL_00bf: ldsfld string [mscorlib]System.String::Empty
					IL_00c4: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_00c9: ldloc.s 8
					IL_00cb: ldstr "0"
					IL_00d0: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_00d5: ldloc.s 9
					IL_00d7: ldstr "1/1/1753 12:00:00 AM"
					IL_00dc: call valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::Parse(string)
					IL_00e1: box [mscorlib]System.DateTime
					IL_00e6: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_00eb: ldloc.s 10
					IL_00ed: ldstr "0"
					IL_00f2: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_00f7: ldloc.s 5
					IL_00f9: ldtoken [mscorlib]System.Int32
					IL_00fe: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
					IL_0103: bne.un.s IL_0120

					IL_0105: ldloc.s 6
					IL_0107: ldloc.3
					IL_0108: unbox.any [mscorlib]System.Int32
					IL_010d: stloc.s 14
					IL_010f: ldloca.s 14
					IL_0111: call instance string [mscorlib]System.Int32::ToString()
					IL_0116: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_011b: br IL_0217

					IL_0120: ldloc.s 5
					IL_0122: ldtoken [mscorlib]System.Boolean
					IL_0127: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
					IL_012c: bne.un.s IL_0158

					IL_012e: ldloc.3
					IL_012f: unbox.any [mscorlib]System.Boolean
					IL_0134: brfalse.s IL_0147

					IL_0136: ldloc.s 6
					IL_0138: ldstr "1"
					IL_013d: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_0142: br IL_0217

					IL_0147: ldloc.s 6
					IL_0149: ldstr "0"
					IL_014e: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_0153: br IL_0217

					IL_0158: ldloc.s 5
					IL_015a: ldtoken [mscorlib]System.String
					IL_015f: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
					IL_0164: bne.un.s IL_0178

					IL_0166: ldloc.s 7
					IL_0168: ldloc.3
					IL_0169: castclass [mscorlib]System.String
					IL_016e: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_0173: br IL_0217

					IL_0178: ldloc.s 5
					IL_017a: ldtoken [mscorlib]System.Single
					IL_017f: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
					IL_0184: bne.un.s IL_019e

					IL_0186: ldloc.s 8
					IL_0188: ldloc.3
					IL_0189: unbox.any [mscorlib]System.Single
					IL_018e: stloc.s 15
					IL_0190: ldloca.s 15
					IL_0192: call instance string [mscorlib]System.Single::ToString()
					IL_0197: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_019c: br.s IL_0217

					IL_019e: ldloc.s 5
					IL_01a0: ldtoken [mscorlib]System.Double
					IL_01a5: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
					IL_01aa: bne.un.s IL_01c4

					IL_01ac: ldloc.s 8
					IL_01ae: ldloc.3
					IL_01af: unbox.any [mscorlib]System.Double
					IL_01b4: stloc.s 16
					IL_01b6: ldloca.s 16
					IL_01b8: call instance string [mscorlib]System.Double::ToString()
					IL_01bd: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_01c2: br.s IL_0217

					IL_01c4: ldloc.s 5
					IL_01c6: ldtoken [mscorlib]System.TimeSpan
					IL_01cb: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
					IL_01d0: bne.un.s IL_01f3

					IL_01d2: ldloc.s 8
					IL_01d4: ldloc.3
					IL_01d5: unbox.any [mscorlib]System.TimeSpan
					IL_01da: stloc.s 17
					IL_01dc: ldloca.s 17
					IL_01de: call instance float64 [mscorlib]System.TimeSpan::get_TotalSeconds()
					IL_01e3: stloc.s 18
					IL_01e5: ldloca.s 18
					IL_01e7: call instance string [mscorlib]System.Double::ToString()
					IL_01ec: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
					IL_01f1: br.s IL_0217

					IL_01f3: ldloc.s 5
					IL_01f5: ldtoken [mscorlib]System.Int64
					IL_01fa: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
					IL_01ff: bne.un.s IL_0217

					IL_0201: ldloc.s 10
					IL_0203: ldloc.3
					IL_0204: unbox.any [mscorlib]System.Int64
					IL_0209: stloc.s 19
					IL_020b: ldloca.s 19
					IL_020d: call instance string [mscorlib]System.Int64::ToString()
					IL_0212: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)

					IL_0217: ldloc.s 4
					IL_0219: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
					IL_021e: ldloc.s 6
					IL_0220: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
					IL_0225: pop
					IL_0226: ldloc.s 4
					IL_0228: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
					IL_022d: ldloc.s 7
					IL_022f: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
					IL_0234: pop
					IL_0235: ldloc.s 4
					IL_0237: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
					IL_023c: ldloc.s 8
					IL_023e: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
					IL_0243: pop
					IL_0244: ldloc.s 4
					IL_0246: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
					IL_024b: ldloc.s 9
					IL_024d: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
					IL_0252: pop
					IL_0253: ldloc.s 4
					IL_0255: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
					IL_025a: ldloc.s 10
					IL_025c: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
					IL_0261: pop
					IL_0262: ldloc.s 4
					IL_0264: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
					IL_0269: ldstr "MetricEntryId"
					IL_026e: ldarg.2
					IL_026f: box [mscorlib]System.Int32
					IL_0274: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
					IL_0279: pop
					IL_027a: ldloc.s 4
					IL_027c: callvirt instance int32 [System.Data]System.Data.Common.DbCommand::ExecuteNonQuery()
					IL_0281: pop

					IL_0282: ldloc.s 13
					IL_0284: ldc.i4.1
					IL_0285: add
					IL_0286: stloc.s 13

					IL_0288: ldloc.s 13
					IL_028a: ldloc.s 12
					IL_028c: ldlen
					IL_028d: conv.i4
					IL_028e: blt IL_0029
				// end loop

				IL_0293: leave.s IL_029f
			} // end .try
			finally
			{
				IL_0295: ldloc.0
				IL_0296: brfalse.s IL_029e

				IL_0298: ldloc.0
				IL_0299: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_029e: endfinally
			} // end handler

			IL_029f: leave.s IL_02a6
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_02a1: stloc.s 11
			IL_02a3: ldloc.s 11
			IL_02a5: throw
		} // end handler

		IL_02a6: ret
	} // end of method SqlMetricsWriter::InsertMetricValueIntoDatabase

	.method private hidebysig 
		instance void InsertMetricsIntoDatabase (
			class [mscorlib]System.Collections.ICollection metrics
		) cil managed 
	{
		// Method begins at RVA 0x303c
		// Header size: 12
		// Code size: 859 (0x35b)
		.maxstack 4
		.locals init (
			[0] class [System.Data]System.Data.SqlClient.SqlConnection,
			[1] string,
			[2] class EALA.Metrics.Metric,
			[3] class [System.Data]System.Data.SqlClient.SqlCommand,
			[4] class [mscorlib]System.Type,
			[5] class [System.Data]System.Data.SqlClient.SqlParameter,
			[6] class [System.Data]System.Data.SqlClient.SqlParameter,
			[7] class [System.Data]System.Data.SqlClient.SqlParameter,
			[8] class [System.Data]System.Data.SqlClient.SqlParameter,
			[9] class [System.Data]System.Data.SqlClient.SqlParameter,
			[10] class [System.Data]System.Data.SqlClient.SqlParameter,
			[11] int32,
			[12] class [mscorlib]System.Exception,
			[13] class [mscorlib]System.Collections.IEnumerator,
			[14] int32,
			[15] float32,
			[16] float64,
			[17] valuetype [mscorlib]System.TimeSpan,
			[18] float64,
			[19] int64,
			[20] class [mscorlib]System.IDisposable
		)

		.try
		{
			IL_0000: ldsfld string EALA.Metrics.SqlMetricsWriter::m_SqlParameters
			IL_0005: newobj instance void [System.Data]System.Data.SqlClient.SqlConnection::.ctor(string)
			IL_000a: stloc.0
			.try
			{
				IL_000b: ldloc.0
				IL_000c: callvirt instance void [System.Data]System.Data.Common.DbConnection::Open()
				IL_0011: ldstr "InsertMetricValue"
				IL_0016: stloc.1
				IL_0017: ldarg.1
				IL_0018: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
				IL_001d: stloc.s 13
				.try
				{
					IL_001f: br IL_0324
					// loop start (head: IL_0324)
						IL_0024: ldloc.s 13
						IL_0026: callvirt instance object [mscorlib]System.Collections.IEnumerator::get_Current()
						IL_002b: castclass EALA.Metrics.Metric
						IL_0030: stloc.2
						IL_0031: ldloc.1
						IL_0032: ldloc.0
						IL_0033: newobj instance void [System.Data]System.Data.SqlClient.SqlCommand::.ctor(string, class [System.Data]System.Data.SqlClient.SqlConnection)
						IL_0038: stloc.3
						IL_0039: ldloc.3
						IL_003a: ldc.i4.4
						IL_003b: callvirt instance void [System.Data]System.Data.Common.DbCommand::set_CommandType(valuetype [System.Data]System.Data.CommandType)
						IL_0040: ldloc.2
						IL_0041: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_0046: callvirt instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
						IL_004b: stloc.s 4
						IL_004d: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
						IL_0052: stloc.s 5
						IL_0054: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
						IL_0059: stloc.s 6
						IL_005b: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
						IL_0060: stloc.s 7
						IL_0062: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
						IL_0067: stloc.s 8
						IL_0069: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor()
						IL_006e: stloc.s 9
						IL_0070: ldloc.s 5
						IL_0072: ldstr "@IntValue"
						IL_0077: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
						IL_007c: ldloc.s 6
						IL_007e: ldstr "@StringValue"
						IL_0083: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
						IL_0088: ldloc.s 7
						IL_008a: ldstr "@FloatValue"
						IL_008f: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
						IL_0094: ldloc.s 8
						IL_0096: ldstr "@DtValue"
						IL_009b: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
						IL_00a0: ldloc.s 9
						IL_00a2: ldstr "@LongValue"
						IL_00a7: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_ParameterName(string)
						IL_00ac: ldloc.s 5
						IL_00ae: ldstr "0"
						IL_00b3: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_00b8: ldloc.s 6
						IL_00ba: ldsfld string [mscorlib]System.String::Empty
						IL_00bf: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_00c4: ldloc.s 7
						IL_00c6: ldstr "0"
						IL_00cb: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_00d0: ldloc.s 8
						IL_00d2: ldstr "1/1/1753 12:00:00 AM"
						IL_00d7: call valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::Parse(string)
						IL_00dc: box [mscorlib]System.DateTime
						IL_00e1: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_00e6: ldloc.s 9
						IL_00e8: ldstr "0"
						IL_00ed: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_00f2: ldloc.s 4
						IL_00f4: ldtoken [mscorlib]System.Int32
						IL_00f9: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
						IL_00fe: bne.un.s IL_0120

						IL_0100: ldloc.s 5
						IL_0102: ldloc.2
						IL_0103: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_0108: unbox.any [mscorlib]System.Int32
						IL_010d: stloc.s 14
						IL_010f: ldloca.s 14
						IL_0111: call instance string [mscorlib]System.Int32::ToString()
						IL_0116: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_011b: br IL_0238

						IL_0120: ldloc.s 4
						IL_0122: ldtoken [mscorlib]System.Boolean
						IL_0127: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
						IL_012c: bne.un.s IL_015d

						IL_012e: ldloc.2
						IL_012f: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_0134: unbox.any [mscorlib]System.Boolean
						IL_0139: brfalse.s IL_014c

						IL_013b: ldloc.s 5
						IL_013d: ldstr "1"
						IL_0142: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_0147: br IL_0238

						IL_014c: ldloc.s 5
						IL_014e: ldstr "0"
						IL_0153: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_0158: br IL_0238

						IL_015d: ldloc.s 4
						IL_015f: ldtoken [mscorlib]System.String
						IL_0164: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
						IL_0169: bne.un.s IL_0182

						IL_016b: ldloc.s 6
						IL_016d: ldloc.2
						IL_016e: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_0173: castclass [mscorlib]System.String
						IL_0178: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_017d: br IL_0238

						IL_0182: ldloc.s 4
						IL_0184: ldtoken [mscorlib]System.Single
						IL_0189: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
						IL_018e: bne.un.s IL_01b0

						IL_0190: ldloc.s 7
						IL_0192: ldloc.2
						IL_0193: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_0198: unbox.any [mscorlib]System.Single
						IL_019d: stloc.s 15
						IL_019f: ldloca.s 15
						IL_01a1: call instance string [mscorlib]System.Single::ToString()
						IL_01a6: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_01ab: br IL_0238

						IL_01b0: ldloc.s 4
						IL_01b2: ldtoken [mscorlib]System.Double
						IL_01b7: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
						IL_01bc: bne.un.s IL_01db

						IL_01be: ldloc.s 7
						IL_01c0: ldloc.2
						IL_01c1: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_01c6: unbox.any [mscorlib]System.Double
						IL_01cb: stloc.s 16
						IL_01cd: ldloca.s 16
						IL_01cf: call instance string [mscorlib]System.Double::ToString()
						IL_01d4: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_01d9: br.s IL_0238

						IL_01db: ldloc.s 4
						IL_01dd: ldtoken [mscorlib]System.TimeSpan
						IL_01e2: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
						IL_01e7: bne.un.s IL_020f

						IL_01e9: ldloc.s 7
						IL_01eb: ldloc.2
						IL_01ec: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_01f1: unbox.any [mscorlib]System.TimeSpan
						IL_01f6: stloc.s 17
						IL_01f8: ldloca.s 17
						IL_01fa: call instance float64 [mscorlib]System.TimeSpan::get_TotalSeconds()
						IL_01ff: stloc.s 18
						IL_0201: ldloca.s 18
						IL_0203: call instance string [mscorlib]System.Double::ToString()
						IL_0208: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)
						IL_020d: br.s IL_0238

						IL_020f: ldloc.s 4
						IL_0211: ldtoken [mscorlib]System.Int64
						IL_0216: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
						IL_021b: bne.un.s IL_0238

						IL_021d: ldloc.s 9
						IL_021f: ldloc.2
						IL_0220: callvirt instance object EALA.Metrics.Metric::get_Data()
						IL_0225: unbox.any [mscorlib]System.Int64
						IL_022a: stloc.s 19
						IL_022c: ldloca.s 19
						IL_022e: call instance string [mscorlib]System.Int64::ToString()
						IL_0233: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Value(object)

						IL_0238: ldloc.3
						IL_0239: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_023e: ldloc.s 5
						IL_0240: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
						IL_0245: pop
						IL_0246: ldloc.3
						IL_0247: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_024c: ldloc.s 6
						IL_024e: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
						IL_0253: pop
						IL_0254: ldloc.3
						IL_0255: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_025a: ldloc.s 7
						IL_025c: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
						IL_0261: pop
						IL_0262: ldloc.3
						IL_0263: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_0268: ldloc.s 8
						IL_026a: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
						IL_026f: pop
						IL_0270: ldloc.3
						IL_0271: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_0276: ldloc.s 9
						IL_0278: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
						IL_027d: pop
						IL_027e: ldloc.3
						IL_027f: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_0284: ldstr "@MetricNameId"
						IL_0289: ldarg.0
						IL_028a: ldfld class [mscorlib]System.Collections.Hashtable EALA.Metrics.SqlMetricsWriter::m_MetricDescriptors
						IL_028f: ldloc.2
						IL_0290: callvirt instance class EALA.Metrics.MetricDescriptor EALA.Metrics.Metric::get_Descriptor()
						IL_0295: callvirt instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
						IL_029a: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
						IL_029f: pop
						IL_02a0: ldloc.3
						IL_02a1: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_02a6: ldstr "@SessionId"
						IL_02ab: ldarg.0
						IL_02ac: ldfld int32 EALA.Metrics.SqlMetricsWriter::m_SessionId
						IL_02b1: box [mscorlib]System.Int32
						IL_02b6: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
						IL_02bb: pop
						IL_02bc: ldloc.3
						IL_02bd: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_02c2: ldstr "@Timestamp"
						IL_02c7: ldloc.2
						IL_02c8: callvirt instance valuetype [mscorlib]System.DateTime EALA.Metrics.Metric::get_TimeStamp()
						IL_02cd: box [mscorlib]System.DateTime
						IL_02d2: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::AddWithValue(string, object)
						IL_02d7: pop
						IL_02d8: ldstr "@MetricEntryId"
						IL_02dd: ldc.i4.8
						IL_02de: newobj instance void [System.Data]System.Data.SqlClient.SqlParameter::.ctor(string, valuetype [System.Data]System.Data.SqlDbType)
						IL_02e3: stloc.s 10
						IL_02e5: ldloc.s 10
						IL_02e7: ldc.i4.2
						IL_02e8: callvirt instance void [System.Data]System.Data.Common.DbParameter::set_Direction(valuetype [System.Data]System.Data.ParameterDirection)
						IL_02ed: ldloc.3
						IL_02ee: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameterCollection [System.Data]System.Data.SqlClient.SqlCommand::get_Parameters()
						IL_02f3: ldloc.s 10
						IL_02f5: callvirt instance class [System.Data]System.Data.SqlClient.SqlParameter [System.Data]System.Data.SqlClient.SqlParameterCollection::Add(class [System.Data]System.Data.SqlClient.SqlParameter)
						IL_02fa: pop
						IL_02fb: ldloc.3
						IL_02fc: callvirt instance int32 [System.Data]System.Data.Common.DbCommand::ExecuteNonQuery()
						IL_0301: pop
						IL_0302: ldloc.s 10
						IL_0304: callvirt instance object [System.Data]System.Data.Common.DbParameter::get_Value()
						IL_0309: unbox.any [mscorlib]System.Int32
						IL_030e: stloc.s 11
						IL_0310: ldloc.2
						IL_0311: callvirt instance object[] EALA.Metrics.Metric::get_DataList()
						IL_0316: ldlen
						IL_0317: conv.i4
						IL_0318: ldc.i4.1
						IL_0319: ble.s IL_0324

						IL_031b: ldarg.0
						IL_031c: ldloc.2
						IL_031d: ldloc.s 11
						IL_031f: call instance void EALA.Metrics.SqlMetricsWriter::InsertMetricValueIntoDatabase(class EALA.Metrics.Metric, int32)

						IL_0324: ldloc.s 13
						IL_0326: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
						IL_032b: brtrue IL_0024
					// end loop

					IL_0330: leave.s IL_0347
				} // end .try
				finally
				{
					IL_0332: ldloc.s 13
					IL_0334: isinst [mscorlib]System.IDisposable
					IL_0339: stloc.s 20
					IL_033b: ldloc.s 20
					IL_033d: brfalse.s IL_0346

					IL_033f: ldloc.s 20
					IL_0341: callvirt instance void [mscorlib]System.IDisposable::Dispose()

					IL_0346: endfinally
				} // end handler

				IL_0347: leave.s IL_0353
			} // end .try
			finally
			{
				IL_0349: ldloc.0
				IL_034a: brfalse.s IL_0352

				IL_034c: ldloc.0
				IL_034d: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_0352: endfinally
			} // end handler

			IL_0353: leave.s IL_035a
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0355: stloc.s 12
			IL_0357: ldloc.s 12
			IL_0359: throw
		} // end handler

		IL_035a: ret
	} // end of method SqlMetricsWriter::InsertMetricsIntoDatabase

	.method public hidebysig 
		instance void Submit (
			class [mscorlib]System.Collections.ICollection metrics
		) cil managed 
	{
		// Method begins at RVA 0x33f0
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance void EALA.Metrics.SqlMetricsWriter::LookUpMetricDescriptors(class [mscorlib]System.Collections.ICollection)
		IL_0007: ldarg.0
		IL_0008: ldarg.1
		IL_0009: call instance void EALA.Metrics.SqlMetricsWriter::InsertMetricsIntoDatabase(class [mscorlib]System.Collections.ICollection)
		IL_000e: ret
	} // end of method SqlMetricsWriter::Submit

} // end of class EALA.Metrics.SqlMetricsWriter

You are not using the latest version of the tool, please update.
Latest version is '11.1.0.9782' (yours is '9.1.0.7988')
