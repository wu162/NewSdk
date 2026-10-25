using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mvp.Xml.Common.XPath;

public class DynamicContext : XsltContext
{
	internal class DynamicVariable : IXsltContextVariable
	{
		private string _name;

		private object _value;

		private XPathResultType _type;

		XPathResultType IXsltContextVariable.VariableType => _type;

		bool IXsltContextVariable.IsLocal => false;

		bool IXsltContextVariable.IsParam => false;

		public DynamicVariable(string name, object value)
		{
			_name = name;
			_value = value;
			if (value is string)
			{
				_type = XPathResultType.String;
				return;
			}
			if (value is bool)
			{
				_type = XPathResultType.Boolean;
				return;
			}
			if (value is XPathNavigator)
			{
				_type = XPathResultType.String;
				return;
			}
			if (value is XPathNodeIterator)
			{
				_type = XPathResultType.NodeSet;
				return;
			}
			if (value is double)
			{
				_type = XPathResultType.Number;
				return;
			}
			if (value is IConvertible)
			{
				try
				{
					_value = Convert.ToDouble(value);
					_type = XPathResultType.Number;
					return;
				}
				catch (FormatException)
				{
					_type = XPathResultType.Any;
					return;
				}
				catch (OverflowException)
				{
					_type = XPathResultType.Any;
					return;
				}
			}
			_type = XPathResultType.Any;
		}

		object IXsltContextVariable.Evaluate(XsltContext context)
		{
			return _value;
		}
	}

	private IDictionary<string, IXsltContextVariable> _variables = new Dictionary<string, IXsltContextVariable>();

	public override bool Whitespace => true;

	public DynamicContext()
		: base(new NameTable())
	{
	}

	public DynamicContext(NameTable table)
		: base(table)
	{
	}

	public DynamicContext(XmlNamespaceManager context)
		: this(context, new NameTable())
	{
	}

	public DynamicContext(XmlNamespaceManager context, NameTable table)
		: base(table)
	{
		object objB = table.Add("http://www.w3.org/XML/1998/namespace");
		object objB2 = table.Add("http://www.w3.org/2000/xmlns/");
		if (context == null)
		{
			return;
		}
		foreach (string item in context)
		{
			string text = context.LookupNamespace(item);
			if (!object.Equals(text, objB) && !object.Equals(text, objB2))
			{
				base.AddNamespace(item, text);
			}
		}
	}

	public override int CompareDocument(string baseUri, string nextbaseUri)
	{
		return string.Compare(baseUri, nextbaseUri, ignoreCase: false, CultureInfo.InvariantCulture);
	}

	public override string LookupNamespace(string prefix)
	{
		string text = NameTable.Get(prefix);
		if (text == null)
		{
			return null;
		}
		return base.LookupNamespace(text);
	}

	public override string LookupPrefix(string uri)
	{
		string text = NameTable.Get(uri);
		if (text == null)
		{
			return null;
		}
		return base.LookupPrefix(text);
	}

	public override bool PreserveWhitespace(XPathNavigator node)
	{
		return true;
	}

	public static XPathExpression Compile(string xpath)
	{
		return new XmlDocument().CreateNavigator().Compile(xpath);
	}

	public void AddVariable(string name, object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		_variables[name] = new DynamicVariable(name, value);
	}

	public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
	{
		return null;
	}

	public override IXsltContextVariable ResolveVariable(string prefix, string name)
	{
		_variables.TryGetValue(name, out var value);
		return value;
	}
}
