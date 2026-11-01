using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.XPointer;

public class XPointerReader : XmlReader, IHasXPathNavigator
{
	private XmlReader _reader;

	private XPathNodeIterator _pointedNodes;

	private static readonly IDictionary<string, WeakReference> _cache = new Dictionary<string, WeakReference>();

	public override int AttributeCount => _reader.AttributeCount;

	public override string BaseURI => _reader.BaseURI;

	public override bool HasValue => _reader.HasValue;

	public override bool IsDefault => _reader.IsDefault;

	public override string Name => _reader.Name;

	public override string LocalName => _reader.LocalName;

	public override string NamespaceURI => _reader.NamespaceURI;

	public override XmlNameTable NameTable => _reader.NameTable;

	public override XmlNodeType NodeType => _reader.NodeType;

	public override string Prefix => _reader.Prefix;

	public override char QuoteChar => _reader.QuoteChar;

	public override int Depth => _reader.Depth;

	public override bool EOF => _reader.EOF;

	public override bool IsEmptyElement => _reader.IsEmptyElement;

	public override ReadState ReadState => _reader.ReadState;

	public override string this[int i] => _reader[i];

	public override string this[string name] => _reader[name];

	public override string this[string name, string namespaceURI] => _reader[name, namespaceURI];

	public override string XmlLang => _reader.XmlLang;

	public override XmlSpace XmlSpace => _reader.XmlSpace;

	public override string Value => _reader.Value;

	private void Init(XPathNavigator nav, string xpointer)
	{
		Pointer pointer = XPointerParser.ParseXPointer(xpointer);
		_pointedNodes = pointer.Evaluate(nav);
		_reader = new SubtreeXPathNavigator(_pointedNodes.Current).ReadSubtree();
	}

	private XPathDocument CreateAndCacheDocument(XmlReader r)
	{
		string baseURI = r.BaseURI;
		XPathDocument xPathDocument = new XPathDocument(r, XmlSpace.Preserve);
		r.Close();
		lock (_cache)
		{
			if (!_cache.ContainsKey(baseURI))
			{
				_cache.Add(baseURI, new WeakReference(xPathDocument));
			}
		}
		return xPathDocument;
	}

	public XPointerReader(IXPathNavigable doc, string xpointer)
		: this(doc.CreateNavigator(), xpointer)
	{
	}

	public XPointerReader(XPathNavigator nav, string xpointer)
	{
		Init(nav, xpointer);
	}

	public XPointerReader(string uri, string xpointer)
		: this(new XmlBaseAwareXmlTextReader(uri), xpointer)
	{
	}

	public XPointerReader(string uri, XmlNameTable nt, string xpointer)
		: this(new XmlBaseAwareXmlTextReader(uri, nt), xpointer)
	{
	}

	public XPointerReader(string uri, Stream stream, XmlNameTable nt, string xpointer)
		: this(new XmlBaseAwareXmlTextReader(uri, stream, nt), xpointer)
	{
	}

	public XPointerReader(string uri, Stream stream, string xpointer)
		: this(uri, stream, new NameTable(), xpointer)
	{
	}

	public XPointerReader(XmlReader reader, string xpointer)
	{
		XPathDocument xPathDocument = null;
		lock (_cache)
		{
			if (_cache.TryGetValue(reader.BaseURI, out var value))
				xPathDocument = value.Target as XPathDocument;
		}
		if (xPathDocument != null) reader.Close();
		else xPathDocument = CreateAndCacheDocument(reader);
		Init(xPathDocument.CreateNavigator(), xpointer);
	}

	public XPointerReader(string uri, string content, string xpointer)
	{
		XPathDocument xPathDocument = null;
		lock (_cache)
		{
			if (_cache.TryGetValue(uri, out var value))
				xPathDocument = value.Target as XPathDocument;
		}
		if (xPathDocument == null)
		{
			XmlReader r = new XmlBaseAwareXmlTextReader(uri, new StringReader(content));
			xPathDocument = CreateAndCacheDocument(r);
		}
		Init(xPathDocument.CreateNavigator(), xpointer);
	}

	public override void Close()
	{
		if (_reader != null)
		{
			_reader.Close();
		}
	}

	public override string GetAttribute(int i)
	{
		return _reader.GetAttribute(i);
	}

	public override string GetAttribute(string name)
	{
		return _reader.GetAttribute(name);
	}

	public override string GetAttribute(string name, string namespaceURI)
	{
		return _reader.GetAttribute(name, namespaceURI);
	}

	public override string LookupNamespace(string prefix)
	{
		return _reader.LookupNamespace(prefix);
	}

	public override void MoveToAttribute(int i)
	{
		_reader.MoveToAttribute(i);
	}

	public override bool MoveToAttribute(string name)
	{
		return _reader.MoveToAttribute(name);
	}

	public override bool MoveToAttribute(string name, string ns)
	{
		return _reader.MoveToAttribute(name, ns);
	}

	public override bool MoveToElement()
	{
		return _reader.MoveToElement();
	}

	public override bool MoveToFirstAttribute()
	{
		return _reader.MoveToFirstAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		return _reader.MoveToNextAttribute();
	}

	public override bool ReadAttributeValue()
	{
		return _reader.ReadAttributeValue();
	}

	public override void ResolveEntity()
	{
		_reader.ResolveEntity();
	}

	public override string ReadInnerXml()
	{
		return _reader.ReadInnerXml();
	}

	public override string ReadOuterXml()
	{
		return _reader.ReadOuterXml();
	}

	public override string ReadString()
	{
		return _reader.ReadString();
	}

	public override bool Read()
	{
		if (_reader.Read())
		{
			return true;
		}
		if (_pointedNodes != null && _pointedNodes.MoveNext())
		{
			_reader = new SubtreeXPathNavigator(_pointedNodes.Current).ReadSubtree();
			return _reader.Read();
		}
		return false;
	}

	public XPathNavigator GetNavigator()
	{
		return _pointedNodes.Current.Clone();
	}
}
