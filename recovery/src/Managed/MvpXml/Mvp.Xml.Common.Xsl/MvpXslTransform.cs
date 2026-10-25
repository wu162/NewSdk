using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mvp.Xml.Exslt;

namespace Mvp.Xml.Common.Xsl;

public class MvpXslTransform : IXmlTransform
{
	private XslCompiledTransform compiledTransform;

	private object sync = new object();

	private ExsltFunctionNamespace supportedFunctions = ExsltFunctionNamespace.All;

	private bool multiOutput;

	private static XmlReaderSettings DefaultReaderSettings;

	public ExsltFunctionNamespace SupportedFunctions
	{
		get
		{
			return supportedFunctions;
		}
		set
		{
			if (Enum.IsDefined(typeof(ExsltFunctionNamespace), value))
			{
				supportedFunctions = value;
			}
		}
	}

	public bool MultiOutput
	{
		get
		{
			return multiOutput;
		}
		set
		{
			multiOutput = value;
		}
	}

	public TempFileCollection TemporaryFiles => compiledTransform.TemporaryFiles;

	public XmlWriterSettings OutputSettings
	{
		get
		{
			if (compiledTransform != null)
			{
				return compiledTransform.OutputSettings;
			}
			return new XmlWriterSettings();
		}
	}

	public MvpXslTransform()
	{
		compiledTransform = new XslCompiledTransform();
	}

	public MvpXslTransform(bool debug)
	{
		compiledTransform = new XslCompiledTransform(debug);
	}

	public void Load(IXPathNavigable stylesheet)
	{
		compiledTransform.Load(stylesheet);
	}

	public void Load(string stylesheetUri)
	{
		compiledTransform.Load(stylesheetUri);
	}

	public void Load(XmlReader stylesheet)
	{
		compiledTransform.Load(stylesheet);
	}

	public void Load(IXPathNavigable stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
	{
		compiledTransform.Load(stylesheet, settings, stylesheetResolver);
	}

	public void Load(string stylesheetUri, XsltSettings settings, XmlResolver stylesheetResolver)
	{
		compiledTransform.Load(stylesheetUri, settings, stylesheetResolver);
	}

	public void Load(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
	{
		compiledTransform.Load(stylesheet, settings, stylesheetResolver);
	}

	public void Transform(XmlInput input, XsltArgumentList arguments, XmlOutput output)
	{
		if (input == null)
		{
			throw new ArgumentNullException("defaltDocument");
		}
		XmlWriter xmlWriter = output.destination as XmlWriter;
		bool flag = false;
		if (xmlWriter == null)
		{
			flag = true;
			if (output.destination is TextWriter textWriter)
			{
				xmlWriter = ((!multiOutput) ? XmlWriter.Create(textWriter, compiledTransform.OutputSettings) : new MultiXmlTextWriter(textWriter));
			}
			else if (output.destination is Stream stream)
			{
				xmlWriter = ((!multiOutput) ? XmlWriter.Create(stream, compiledTransform.OutputSettings) : new MultiXmlTextWriter(stream, compiledTransform.OutputSettings.Encoding));
			}
			else
			{
				if (!(output.destination is string text))
				{
					throw new Exception("Unexpected XmlOutput");
				}
				if (multiOutput)
				{
					xmlWriter = new MultiXmlTextWriter(text, compiledTransform.OutputSettings.Encoding);
				}
				else
				{
					XmlWriterSettings xmlWriterSettings = compiledTransform.OutputSettings.Clone();
					xmlWriterSettings.CloseOutput = true;
					xmlWriter = XmlWriter.Create(text, xmlWriterSettings);
				}
			}
		}
		try
		{
			TransformToWriter(input, arguments, xmlWriter);
		}
		finally
		{
			if (flag)
			{
				xmlWriter.Close();
			}
		}
	}

	public XmlReader Transform(XmlInput input, XsltArgumentList arguments)
	{
		XslReader xslReader = new XslReader(compiledTransform);
		xslReader.StartTransform(input, AddExsltExtensionObjects(arguments));
		return xslReader;
	}

	static MvpXslTransform()
	{
		DefaultReaderSettings = new XmlReaderSettings();
		DefaultReaderSettings.ProhibitDtd = false;
	}

	private XmlReaderSettings GetReaderSettings(XmlInput defaultDocument)
	{
		if (defaultDocument.resolver is DefaultXmlResolver)
		{
			return DefaultReaderSettings;
		}
		XmlReaderSettings xmlReaderSettings = DefaultReaderSettings.Clone();
		xmlReaderSettings.XmlResolver = defaultDocument.resolver;
		return xmlReaderSettings;
	}

	private void TransformToWriter(XmlInput defaultDocument, XsltArgumentList xsltArgs, XmlWriter xmlWriter)
	{
		XsltArgumentList xsltArgumentList = AddExsltExtensionObjects(xsltArgs);
		if (defaultDocument.source is XmlReader input)
		{
			compiledTransform.Transform(input, xsltArgumentList, xmlWriter, defaultDocument.resolver);
			return;
		}
		if (defaultDocument.source is IXPathNavigable iXPathNavigable)
		{
			if (defaultDocument.resolver is DefaultXmlResolver)
			{
				compiledTransform.Transform(iXPathNavigable, xsltArgumentList, xmlWriter);
			}
			else
			{
				TransformIXPathNavigable(iXPathNavigable, xsltArgumentList, xmlWriter, defaultDocument.resolver);
			}
			return;
		}
		if (defaultDocument.source is string inputUri)
		{
			using XmlReader input2 = XmlReader.Create(inputUri, GetReaderSettings(defaultDocument));
			compiledTransform.Transform(input2, xsltArgumentList, xmlWriter, defaultDocument.resolver);
			return;
		}
		if (defaultDocument.source is Stream input3)
		{
			using XmlReader input4 = XmlReader.Create(input3, GetReaderSettings(defaultDocument));
			compiledTransform.Transform(input4, xsltArgumentList, xmlWriter, defaultDocument.resolver);
			return;
		}
		if (defaultDocument.source is TextReader input5)
		{
			using XmlReader input6 = XmlReader.Create(input5, GetReaderSettings(defaultDocument));
			compiledTransform.Transform(input6, xsltArgumentList, xmlWriter, defaultDocument.resolver);
			return;
		}
		throw new Exception("Unexpected XmlInput");
	}

	private void TransformIXPathNavigable(IXPathNavigable nav, XsltArgumentList args, XmlWriter xmlWriter, XmlResolver resolver)
	{
		object value = compiledTransform.GetType().GetField("command", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(compiledTransform);
		MethodInfo method = value.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public, null, new Type[4]
		{
			typeof(IXPathNavigable),
			typeof(XmlResolver),
			typeof(XsltArgumentList),
			typeof(XmlWriter)
		}, null);
		method.Invoke(value, new object[4]
		{
			nav,
			resolver,
			AddExsltExtensionObjects(args),
			xmlWriter
		});
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
				list.AddExtensionObject("http://exslt.org/math", new ExsltMath());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.Random) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/random", new ExsltRandom());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.DatesAndTimes) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/dates-and-times", new ExsltDatesAndTimes());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.RegularExpressions) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/regular-expressions", new ExsltRegularExpressions());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.Strings) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/strings", new ExsltStrings());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.Sets) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://exslt.org/sets", new ExsltSets());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNDatesAndTimes) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/dates-and-times", new GDNDatesAndTimes());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNMath) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/math", new GDNMath());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNRegularExpressions) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/regular-expressions", new GDNRegularExpressions());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNSets) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/sets", new GDNSets());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNStrings) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/strings", new GDNStrings());
			}
			if ((SupportedFunctions & ExsltFunctionNamespace.GDNDynamic) > ExsltFunctionNamespace.None)
			{
				list.AddExtensionObject("http://gotdotnet.com/exslt/dynamic", new GDNDynamic());
			}
		}
		return list;
	}
}
