using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath;

public class XPathIteratorReader : XmlTextReader, IXmlSerializable
{
	private class FakedRootReader : XmlReader
	{
		private string _name;

		private string _namespace;

		private XmlNodeType _nodetype;

		private ReadState _state;

		public override int AttributeCount => 0;

		public override string BaseURI => string.Empty;

		public override int Depth => -1;

		public override bool EOF => _state == ReadState.EndOfFile;

		public override bool HasValue => false;

		public override bool IsDefault => false;

		public override bool IsEmptyElement => false;

		public override string this[string name, string ns] => null;

		public override string this[string name] => null;

		public override string this[int i] => null;

		public override string LocalName => _name;

		public override string Name => _name;

		public override string NamespaceURI => _namespace;

		public override XmlNameTable NameTable => null;

		public override XmlNodeType NodeType
		{
			get
			{
				if (_state != ReadState.Initial)
				{
					return _nodetype;
				}
				return XmlNodeType.None;
			}
		}

		public override string Prefix => string.Empty;

		public override char QuoteChar => '"';

		public override ReadState ReadState => _state;

		public override string Value => string.Empty;

		public override string XmlLang => string.Empty;

		public override XmlSpace XmlSpace => XmlSpace.Default;

		public FakedRootReader(string name, string ns, XmlNodeType nodeType)
		{
			_name = name;
			_namespace = ns;
			_nodetype = nodeType;
			_state = ((nodeType != XmlNodeType.Element) ? ReadState.Interactive : ReadState.Initial);
		}

		public override void Close()
		{
			_state = ReadState.Closed;
		}

		public override string GetAttribute(string name, string ns)
		{
			return null;
		}

		public override string GetAttribute(string name)
		{
			return null;
		}

		public override string GetAttribute(int i)
		{
			return null;
		}

		public override string LookupNamespace(string prefix)
		{
			return null;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return false;
		}

		public override bool MoveToAttribute(string name)
		{
			return false;
		}

		public override void MoveToAttribute(int i)
		{
		}

		public override XmlNodeType MoveToContent()
		{
			if (_state == ReadState.Initial)
			{
				_state = ReadState.Interactive;
			}
			return _nodetype;
		}

		public override bool MoveToElement()
		{
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			return false;
		}

		public override bool Read()
		{
			if (_state == ReadState.Initial)
			{
				_state = ReadState.Interactive;
				return true;
			}
			if (_state == ReadState.Interactive && _nodetype == XmlNodeType.EndElement)
			{
				_state = ReadState.EndOfFile;
				return false;
			}
			return false;
		}

		public override bool ReadAttributeValue()
		{
			return false;
		}

		public override string ReadInnerXml()
		{
			return string.Empty;
		}

		public override string ReadOuterXml()
		{
			return string.Empty;
		}

		public override void ResolveEntity()
		{
		}
	}

	private XmlReader _current;

	private XPathNodeIterator _iterator;

	private XmlQualifiedName _rootname;

	public override int AttributeCount => _current.AttributeCount;

	public override string BaseURI => _current.BaseURI;

	public override int Depth => _current.Depth + 1;

	public override bool EOF
	{
		get
		{
			if (_current.ReadState != ReadState.EndOfFile)
			{
				return _current.ReadState == ReadState.Closed;
			}
			return true;
		}
	}

	public override bool HasValue => _current.HasValue;

	public override bool IsDefault => false;

	public override bool IsEmptyElement => _current.IsEmptyElement;

	public override string this[string name, string ns] => _current[name, ns];

	public override string this[string name] => _current[name, string.Empty];

	public override string this[int i] => _current[i];

	public override string LocalName => _current.LocalName;

	public override string Name => _current.Name;

	public override string NamespaceURI => _current.NamespaceURI;

	public override XmlNameTable NameTable => _current.NameTable;

	public override XmlNodeType NodeType => _current.NodeType;

	public override string Prefix => _current.Prefix;

	public override char QuoteChar => _current.QuoteChar;

	public override ReadState ReadState => _current.ReadState;

	public override string Value => _current.Value;

	public override string XmlLang => _current.XmlLang;

	public override XmlSpace XmlSpace => XmlSpace.Default;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public XPathIteratorReader()
	{
	}

	public XPathIteratorReader(XPathNodeIterator iterator)
		: this(iterator, "root", string.Empty)
	{
	}

	public XPathIteratorReader(XPathNodeIterator iterator, string rootName)
		: this(iterator, rootName, string.Empty)
	{
	}

	public XPathIteratorReader(XPathNodeIterator iterator, string rootName, string ns)
		: base(new StringReader(string.Empty))
	{
		_iterator = iterator.Clone();
		_current = new FakedRootReader(rootName, ns, XmlNodeType.Element);
		_rootname = new XmlQualifiedName(rootName, ns);
	}

	private string Serialize()
	{
		StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture);
		XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
		xmlTextWriter.WriteNode(this, defattr: false);
		stringWriter.Flush();
		return stringWriter.ToString();
	}

	public override void Close()
	{
		_current.Close();
	}

	public override string GetAttribute(string name, string ns)
	{
		return _current.GetAttribute(name, ns);
	}

	public override string GetAttribute(string name)
	{
		return _current.GetAttribute(name);
	}

	public override string GetAttribute(int i)
	{
		return _current.GetAttribute(i);
	}

	public override string LookupNamespace(string prefix)
	{
		return _current.LookupNamespace(prefix);
	}

	public override bool MoveToAttribute(string name, string ns)
	{
		return _current.MoveToAttribute(name, ns);
	}

	public override bool MoveToAttribute(string name)
	{
		return _current.MoveToAttribute(name);
	}

	public override void MoveToAttribute(int i)
	{
		_current.MoveToAttribute(i);
	}

	public override XmlNodeType MoveToContent()
	{
		return base.MoveToContent();
	}

	public override bool MoveToElement()
	{
		return _current.MoveToElement();
	}

	public override bool MoveToFirstAttribute()
	{
		return _current.MoveToFirstAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		return _current.MoveToNextAttribute();
	}

	public override bool Read()
	{
		if (_current.ReadState == ReadState.Closed || _current.ReadState == ReadState.EndOfFile)
		{
			return false;
		}
		bool flag = _current.Read();
		if (!flag)
		{
			if (_iterator.MoveNext())
			{
				_current = new SubtreeXPathNavigator(_iterator.Current).ReadSubtree();
				return _current.Read();
			}
			if (_current is FakedRootReader && _current.NodeType == XmlNodeType.EndElement)
			{
				return false;
			}
			_current = new FakedRootReader(_rootname.Name, _rootname.Namespace, XmlNodeType.EndElement);
			return true;
		}
		return flag;
	}

	public override bool ReadAttributeValue()
	{
		return _current.ReadAttributeValue();
	}

	public override string ReadInnerXml()
	{
		if (Read())
		{
			return Serialize();
		}
		return string.Empty;
	}

	public override string ReadOuterXml()
	{
		if (_current.ReadState != ReadState.Interactive)
		{
			return string.Empty;
		}
		return Serialize();
	}

	public override void ResolveEntity()
	{
	}

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteNode(this, defattr: false);
	}

	public XmlSchema GetSchema()
	{
		return null;
	}

	public void ReadXml(XmlReader reader)
	{
		XPathDocument xPathDocument = new XPathDocument(reader);
		XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
		xPathNavigator.MoveToFirstChild();
		_rootname = new XmlQualifiedName(xPathNavigator.LocalName, xPathNavigator.NamespaceURI);
		_iterator = xPathNavigator.SelectChildren(XPathNodeType.All);
	}
}
