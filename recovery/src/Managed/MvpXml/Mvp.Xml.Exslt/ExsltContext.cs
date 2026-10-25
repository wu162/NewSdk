using System;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mvp.Xml.Exslt;

public class ExsltContext : XsltContext
{
	private XmlNameTable _nt;

	private ExsltFunctionNamespace _supportedFunctions = ExsltFunctionNamespace.All;

	private ExsltMath exsltMath = new ExsltMath();

	private ExsltDatesAndTimes exsltDatesAndTimes = new ExsltDatesAndTimes();

	private ExsltRegularExpressions exsltRegularExpressions = new ExsltRegularExpressions();

	private ExsltStrings exsltStrings = new ExsltStrings();

	private ExsltSets exsltSets = new ExsltSets();

	private ExsltRandom exsltRandom = new ExsltRandom();

	private GDNDatesAndTimes gdnDatesAndTimes = new GDNDatesAndTimes();

	private GDNRegularExpressions gdnRegularExpressions = new GDNRegularExpressions();

	private GDNMath gdnMath = new GDNMath();

	private GDNSets gdnSets = new GDNSets();

	private GDNStrings gdnStrings = new GDNStrings();

	private GDNDynamic gdnDynamic = new GDNDynamic();

	public ExsltFunctionNamespace SupportedFunctions
	{
		get
		{
			return _supportedFunctions;
		}
		set
		{
			if (Enum.IsDefined(typeof(ExsltFunctionNamespace), value))
			{
				_supportedFunctions = value;
			}
		}
	}

	public override bool Whitespace => true;

	public ExsltContext(XmlNameTable nt)
		: base((NameTable)nt)
	{
		_nt = nt;
		AddExtensionNamespaces();
	}

	public ExsltContext(NameTable nt, ExsltFunctionNamespace supportedFunctions)
		: this(nt)
	{
		SupportedFunctions = supportedFunctions;
	}

	private void AddExtensionNamespaces()
	{
		RemoveNamespace("math", "http://exslt.org/math");
		RemoveNamespace("date", "http://exslt.org/dates-and-times");
		RemoveNamespace("regexp", "http://exslt.org/regular-expressions");
		RemoveNamespace("str", "http://exslt.org/strings");
		RemoveNamespace("set", "http://exslt.org/sets");
		RemoveNamespace("random", "http://exslt.org/random");
		RemoveNamespace("date2", "http://gotdotnet.com/exslt/dates-and-times");
		RemoveNamespace("math2", "http://gotdotnet.com/exslt/math");
		RemoveNamespace("regexp2", "http://gotdotnet.com/exslt/regular-expressions");
		RemoveNamespace("set2", "http://gotdotnet.com/exslt/sets");
		RemoveNamespace("str2", "http://gotdotnet.com/exslt/strings");
		RemoveNamespace("dyn2", "http://gotdotnet.com/exslt/dynamic");
		if ((SupportedFunctions & ExsltFunctionNamespace.Math) > ExsltFunctionNamespace.None)
		{
			AddNamespace("math", "http://exslt.org/math");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.DatesAndTimes) > ExsltFunctionNamespace.None)
		{
			AddNamespace("date", "http://exslt.org/dates-and-times");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.RegularExpressions) > ExsltFunctionNamespace.None)
		{
			AddNamespace("regexp", "http://exslt.org/regular-expressions");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.Strings) > ExsltFunctionNamespace.None)
		{
			AddNamespace("str", "http://exslt.org/strings");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.Sets) > ExsltFunctionNamespace.None)
		{
			AddNamespace("set", "http://exslt.org/sets");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.Random) > ExsltFunctionNamespace.None)
		{
			AddNamespace("random", "http://exslt.org/random");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.GDNDatesAndTimes) > ExsltFunctionNamespace.None)
		{
			AddNamespace("date2", "http://gotdotnet.com/exslt/dates-and-times");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.GDNMath) > ExsltFunctionNamespace.None)
		{
			AddNamespace("math2", "http://gotdotnet.com/exslt/math");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.GDNRegularExpressions) > ExsltFunctionNamespace.None)
		{
			AddNamespace("regexp2", "http://gotdotnet.com/exslt/regular-expressions");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.GDNSets) > ExsltFunctionNamespace.None)
		{
			AddNamespace("set2", "http://gotdotnet.com/exslt/sets");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.GDNStrings) > ExsltFunctionNamespace.None)
		{
			AddNamespace("str2", "http://gotdotnet.com/exslt/strings");
		}
		if ((SupportedFunctions & ExsltFunctionNamespace.GDNDynamic) > ExsltFunctionNamespace.None)
		{
			AddNamespace("dyn2", "http://gotdotnet.com/exslt/dynamic");
		}
	}

	public override int CompareDocument(string baseUri, string nextbaseUri)
	{
		return 0;
	}

	public override bool PreserveWhitespace(XPathNavigator node)
	{
		return true;
	}

	public override IXsltContextVariable ResolveVariable(string prefix, string name)
	{
		return null;
	}

	public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
	{
		return LookupNamespace(_nt.Get(prefix)) switch
		{
			"http://exslt.org/dates-and-times" => GetExtensionFunctionImplementation(exsltDatesAndTimes, name, argTypes), 
			"http://exslt.org/math" => GetExtensionFunctionImplementation(exsltMath, name, argTypes), 
			"http://exslt.org/regular-expressions" => GetExtensionFunctionImplementation(exsltRegularExpressions, name, argTypes), 
			"http://exslt.org/sets" => GetExtensionFunctionImplementation(exsltSets, name, argTypes), 
			"http://exslt.org/strings" => GetExtensionFunctionImplementation(exsltStrings, name, argTypes), 
			"http://exslt.org/random" => GetExtensionFunctionImplementation(exsltRandom, name, argTypes), 
			"http://gotdotnet.com/exslt/dates-and-times" => GetExtensionFunctionImplementation(gdnDatesAndTimes, name, argTypes), 
			"http://gotdotnet.com/exslt/math" => GetExtensionFunctionImplementation(gdnMath, name, argTypes), 
			"http://gotdotnet.com/exslt/regular-expressions" => GetExtensionFunctionImplementation(gdnRegularExpressions, name, argTypes), 
			"http://gotdotnet.com/exslt/sets" => GetExtensionFunctionImplementation(gdnSets, name, argTypes), 
			"http://gotdotnet.com/exslt/strings" => GetExtensionFunctionImplementation(gdnStrings, name, argTypes), 
			"http://gotdotnet.com/exslt/dynamic" => GetExtensionFunctionImplementation(gdnDynamic, name, argTypes), 
			_ => throw new XPathException($"Unrecognized extension function namespace: prefix='{prefix}', namespace URI='{LookupNamespace(_nt.Get(prefix))}'", null), 
		};
	}

	private ExsltContextFunction GetExtensionFunctionImplementation(object obj, string name, XPathResultType[] argTypes)
	{
		MethodInfo[] methods = obj.GetType().GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (!(methodInfo.Name == name))
			{
				continue;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != argTypes.Length)
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < parameters.Length; j++)
			{
				ParameterInfo parameterInfo = parameters[j];
				XPathResultType xPathResultType = ConvertToXPathType(parameterInfo.ParameterType);
				if (xPathResultType != XPathResultType.Any && xPathResultType != argTypes[j])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return new ExsltContextFunction(methodInfo, argTypes, obj);
			}
		}
		throw new XPathException("Extension function not found: " + name, null);
	}

	public static XPathResultType ConvertToXPathType(Type type)
	{
		switch (Type.GetTypeCode(type))
		{
		case TypeCode.Boolean:
			return XPathResultType.Boolean;
		case TypeCode.String:
			return XPathResultType.String;
		case TypeCode.Object:
			if (typeof(IXPathNavigable).IsAssignableFrom(type) || typeof(XPathNavigator).IsAssignableFrom(type))
			{
				return XPathResultType.String;
			}
			if (typeof(XPathNodeIterator).IsAssignableFrom(type))
			{
				return XPathResultType.NodeSet;
			}
			return XPathResultType.Any;
		case TypeCode.Empty:
		case TypeCode.DBNull:
		case TypeCode.DateTime:
			return XPathResultType.Error;
		default:
			return XPathResultType.Number;
		}
	}
}
