using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mvp.Xml.Exslt;

[Obsolete("This class has been deprecated. Please use Mvp.Xml.Common.Xsl.MvpXslTransform instead.")]
public class ExsltTransform
{
	private object sync = new object();

	private XslCompiledTransform xslTransform;

	private ExsltFunctionNamespace _supportedFunctions = ExsltFunctionNamespace.All;

	private ExsltMath exsltMath = new ExsltMath();

	private ExsltRandom exsltRandom = new ExsltRandom();

	private ExsltDatesAndTimes exsltDatesAndTimes = new ExsltDatesAndTimes();

	private ExsltRegularExpressions exsltRegularExpressions = new ExsltRegularExpressions();

	private ExsltStrings exsltStrings = new ExsltStrings();

	private ExsltSets exsltSets = new ExsltSets();

	private GDNDatesAndTimes gdnDatesAndTimes = new GDNDatesAndTimes();

	private GDNRegularExpressions gdnRegularExpressions = new GDNRegularExpressions();

	private GDNMath gdnMath = new GDNMath();

	private GDNSets gdnSets = new GDNSets();

	private GDNStrings gdnStrings = new GDNStrings();

	private GDNDynamic gdnDynamic = new GDNDynamic();

	private bool _multiOutput;

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

	public bool MultiOutput
	{
		get
		{
			return _multiOutput;
		}
		set
		{
			_multiOutput = value;
		}
	}

	public XmlWriterSettings OutputSettings => xslTransform.OutputSettings;

	public TempFileCollection TemporaryFiles => xslTransform.TemporaryFiles;

	public ExsltTransform()
	{
		xslTransform = new XslCompiledTransform();
	}

	public ExsltTransform(bool debug)
	{
		xslTransform = new XslCompiledTransform(debug);
	}

	public void Load(IXPathNavigable stylesheet)
	{
		xslTransform.Load(stylesheet);
	}

	public void Load(string stylesheetUri)
	{
		xslTransform.Load(stylesheetUri);
	}

	public void Load(XmlReader stylesheet)
	{
		xslTransform.Load(stylesheet);
	}

	public void Load(IXPathNavigable stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
	{
		xslTransform.Load(stylesheet, settings, stylesheetResolver);
	}

	public void Load(string stylesheetUri, XsltSettings settings, XmlResolver stylesheetResolver)
	{
		xslTransform.Load(stylesheetUri, settings, stylesheetResolver);
	}

	public void Load(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
	{
		xslTransform.Load(stylesheet, settings, stylesheetResolver);
	}

	public void Transform(IXPathNavigable input, XmlWriter results)
	{
		xslTransform.Transform(input, AddExsltExtensionObjects(null), results);
	}

	public void Transform(string inputUri, string resultsFile)
	{
		using FileStream fileStream = File.OpenWrite(resultsFile);
		if (_multiOutput)
		{
			xslTransform.Transform(new XPathDocument(inputUri), AddExsltExtensionObjects(null), new MultiXmlTextWriter(fileStream, OutputSettings.Encoding));
		}
		else
		{
			xslTransform.Transform(new XPathDocument(inputUri), AddExsltExtensionObjects(null), fileStream);
		}
	}

	public void Transform(string inputUri, XmlWriter results)
	{
		xslTransform.Transform(inputUri, AddExsltExtensionObjects(null), results);
	}

	public void Transform(XmlReader input, XmlWriter results)
	{
		xslTransform.Transform(input, AddExsltExtensionObjects(null), results);
	}

	public void Transform(IXPathNavigable input, XsltArgumentList arguments, Stream results)
	{
		if (_multiOutput)
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), new MultiXmlTextWriter(results, OutputSettings.Encoding));
		}
		else
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), results);
		}
	}

	public void Transform(IXPathNavigable input, XsltArgumentList arguments, TextWriter results)
	{
		if (_multiOutput)
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), new MultiXmlTextWriter(results));
		}
		else
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), results);
		}
	}

	public void Transform(IXPathNavigable input, XsltArgumentList arguments, XmlWriter results)
	{
		xslTransform.Transform(input, AddExsltExtensionObjects(arguments), results);
	}

	public void Transform(string inputUri, XsltArgumentList arguments, Stream results)
	{
		if (_multiOutput)
		{
			xslTransform.Transform(inputUri, AddExsltExtensionObjects(arguments), new MultiXmlTextWriter(results, OutputSettings.Encoding));
		}
		else
		{
			xslTransform.Transform(inputUri, AddExsltExtensionObjects(arguments), results);
		}
	}

	public void Transform(string inputUri, XsltArgumentList arguments, TextWriter results)
	{
		if (_multiOutput)
		{
			xslTransform.Transform(inputUri, AddExsltExtensionObjects(arguments), new MultiXmlTextWriter(results));
		}
		else
		{
			xslTransform.Transform(inputUri, AddExsltExtensionObjects(arguments), results);
		}
	}

	public void Transform(string inputUri, XsltArgumentList arguments, XmlWriter results)
	{
		xslTransform.Transform(inputUri, AddExsltExtensionObjects(arguments), results);
	}

	public void Transform(XmlReader input, XsltArgumentList arguments, Stream results)
	{
		if (_multiOutput)
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), new MultiXmlTextWriter(results, OutputSettings.Encoding));
		}
		else
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), results);
		}
	}

	public void Transform(XmlReader input, XsltArgumentList arguments, TextWriter results)
	{
		if (_multiOutput)
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), new MultiXmlTextWriter(results));
		}
		else
		{
			xslTransform.Transform(input, AddExsltExtensionObjects(arguments), results);
		}
	}

	public void Transform(XmlReader input, XsltArgumentList arguments, XmlWriter results)
	{
		xslTransform.Transform(input, AddExsltExtensionObjects(arguments), results);
	}

	public void Transform(XmlReader input, XsltArgumentList arguments, XmlWriter results, XmlResolver documentResolver)
	{
		xslTransform.Transform(input, AddExsltExtensionObjects(arguments), results, documentResolver);
	}

	private XsltArgumentList AddExsltExtensionObjects(XsltArgumentList list)
	{
		if (list == null)
		{
			list = new XsltArgumentList();
		}
		lock (sync)
		{
			list.RemoveExtensionObject("http://exslt.org/math");
			list.RemoveExtensionObject("http://exslt.org/random");
			list.RemoveExtensionObject("http://exslt.org/dates-and-times");
			list.RemoveExtensionObject("http://exslt.org/regular-expressions");
			list.RemoveExtensionObject("http://exslt.org/strings");
			list.RemoveExtensionObject("http://exslt.org/sets");
			list.RemoveExtensionObject("http://gotdotnet.com/exslt/dates-and-times");
			list.RemoveExtensionObject("http://gotdotnet.com/exslt/math");
			list.RemoveExtensionObject("http://gotdotnet.com/exslt/regular-expressions");
			list.RemoveExtensionObject("http://gotdotnet.com/exslt/sets");
			list.RemoveExtensionObject("http://gotdotnet.com/exslt/strings");
			list.RemoveExtensionObject("http://gotdotnet.com/exslt/dynamic");
			if ((SupportedFunctions & ExsltFunctionNamespace.Math) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/math", exsltMath);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.Random) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/random", exsltRandom);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.DatesAndTimes) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/dates-and-times", exsltDatesAndTimes);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.RegularExpressions) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/regular-expressions", exsltRegularExpressions);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.Strings) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/strings", exsltStrings);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.Sets) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/sets", exsltSets);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNDatesAndTimes) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/dates-and-times", gdnDatesAndTimes);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNMath) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/math", gdnMath);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNRegularExpressions) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/regular-expressions", gdnRegularExpressions);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNSets) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/sets", gdnSets);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNStrings) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/strings", gdnStrings);
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNDynamic) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/dynamic", gdnDynamic);
			}
		}
		return list;
	}
}
