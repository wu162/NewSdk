using System;

namespace Mvp.Xml.Exslt;

[Flags]
public enum ExsltFunctionNamespace
{
	None = 0,
	DatesAndTimes = 1,
	Math = 2,
	RegularExpressions = 4,
	Sets = 8,
	Strings = 0x10,
	GDNDatesAndTimes = 0x20,
	GDNSets = 0x40,
	GDNMath = 0x80,
	GDNRegularExpressions = 0x100,
	GDNStrings = 0x200,
	Random = 0x400,
	GDNDynamic = 0x808,
	AllExslt = 0x41F,
	All = 0xFFF
}
