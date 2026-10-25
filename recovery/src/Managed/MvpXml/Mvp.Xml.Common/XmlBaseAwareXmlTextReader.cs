using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Mvp.Xml.Common;

public class XmlBaseAwareXmlTextReader : XmlTextReader
{
	private XmlBaseState _state = new XmlBaseState();

	private Stack<XmlBaseState> _states;

	public override string BaseURI
	{
		get
		{
			if (!(_state.BaseUri == null))
			{
				return _state.BaseUri.AbsoluteUri;
			}
			return "";
		}
	}

	public XmlBaseAwareXmlTextReader(string uri)
		: base(uri)
	{
		_state.BaseUri = new Uri(base.BaseURI);
	}

	public XmlBaseAwareXmlTextReader(string uri, XmlNameTable nt)
		: base(uri, nt)
	{
		_state.BaseUri = new Uri(base.BaseURI);
	}

	public XmlBaseAwareXmlTextReader(TextReader reader)
		: base(reader)
	{
	}

	public XmlBaseAwareXmlTextReader(string uri, TextReader reader)
		: base(uri, reader)
	{
		_state.BaseUri = new Uri(base.BaseURI);
	}

	public XmlBaseAwareXmlTextReader(TextReader reader, XmlNameTable nt)
		: base(reader, nt)
	{
	}

	public XmlBaseAwareXmlTextReader(string uri, TextReader reader, XmlNameTable nt)
		: base(uri, reader, nt)
	{
		_state.BaseUri = new Uri(base.BaseURI);
	}

	public XmlBaseAwareXmlTextReader(Stream stream)
		: base(stream)
	{
	}

	public XmlBaseAwareXmlTextReader(string uri, Stream stream)
		: base(uri, stream)
	{
		_state.BaseUri = new Uri(base.BaseURI);
	}

	public XmlBaseAwareXmlTextReader(Stream stream, XmlNameTable nt)
		: base(stream, nt)
	{
	}

	public XmlBaseAwareXmlTextReader(string uri, Stream stream, XmlNameTable nt)
		: base(uri, stream, nt)
	{
		_state.BaseUri = new Uri(base.BaseURI);
	}

	public override bool Read()
	{
		bool flag = base.Read();
		if (flag)
		{
			if (base.NodeType == XmlNodeType.Element && base.HasAttributes)
			{
				string attribute = GetAttribute("xml:base");
				if (attribute == null)
				{
					return flag;
				}
				Uri uri = null;
				uri = ((!(_state.BaseUri == null)) ? new Uri(_state.BaseUri, attribute) : new Uri(attribute));
				if (_states == null)
				{
					_states = new Stack<XmlBaseState>();
				}
				_states.Push(_state);
				_state = new XmlBaseState(uri, base.Depth);
			}
			else if (base.NodeType == XmlNodeType.EndElement && base.Depth == _state.Depth && _states != null && _states.Count > 0)
			{
				_state = _states.Pop();
			}
		}
		return flag;
	}
}
