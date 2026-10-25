using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Mvp.Xml.XInclude;

internal class TextIncludingReader : XmlReader
{
	private string _encoding;

	private ReadState _state;

	private string _value;

	private Uri _includeLocation;

	private string _accept;

	private string _acceptLanguage;

	private string _href;

	private bool _exposeCDATA;

	public override int AttributeCount => 0;

	public override string BaseURI => _href;

	public override int Depth
	{
		get
		{
			if (_state != ReadState.Interactive)
			{
				return 0;
			}
			return 1;
		}
	}

	public override bool EOF
	{
		get
		{
			if (_state != ReadState.EndOfFile)
			{
				return false;
			}
			return true;
		}
	}

	public override bool HasValue
	{
		get
		{
			if (_state != ReadState.Interactive)
			{
				return false;
			}
			return true;
		}
	}

	public override bool IsDefault => false;

	public override bool IsEmptyElement => false;

	public override string this[int index] => string.Empty;

	public override string this[string qname] => string.Empty;

	public override string this[string localname, string nsuri] => string.Empty;

	public override string LocalName => string.Empty;

	public override string Name => string.Empty;

	public override string NamespaceURI => string.Empty;

	public override XmlNameTable NameTable => null;

	public override XmlNodeType NodeType
	{
		get
		{
			if (_state != ReadState.Interactive)
			{
				return XmlNodeType.None;
			}
			if (!_exposeCDATA)
			{
				return XmlNodeType.Text;
			}
			return XmlNodeType.CDATA;
		}
	}

	public override string Prefix => string.Empty;

	public override char QuoteChar => '"';

	public override ReadState ReadState => _state;

	public override string Value
	{
		get
		{
			if (_state != ReadState.Interactive)
			{
				return string.Empty;
			}
			return _value;
		}
	}

	public override string XmlLang => string.Empty;

	public override XmlSpace XmlSpace => XmlSpace.None;

	public TextIncludingReader(Uri includeLocation, string encoding, string accept, string acceptLanguage, bool exposeCDATA)
	{
		_includeLocation = includeLocation;
		_href = includeLocation.AbsoluteUri;
		_encoding = encoding;
		_state = ReadState.Initial;
		_accept = accept;
		_acceptLanguage = acceptLanguage;
		_exposeCDATA = exposeCDATA;
	}

	public TextIncludingReader(string value, bool exposeCDATA)
	{
		_state = ReadState.Initial;
		_exposeCDATA = exposeCDATA;
		_value = value;
	}

	public override void Close()
	{
		_state = ReadState.Closed;
	}

	public override string GetAttribute(int index)
	{
		throw new ArgumentOutOfRangeException("index", index, "No attributes exposed");
	}

	public override string GetAttribute(string qname)
	{
		return null;
	}

	public override string GetAttribute(string localname, string nsuri)
	{
		return null;
	}

	public override string LookupNamespace(string prefix)
	{
		return null;
	}

	public override void MoveToAttribute(int index)
	{
	}

	public override bool MoveToAttribute(string qname)
	{
		return false;
	}

	public override bool MoveToAttribute(string localname, string nsuri)
	{
		return false;
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

	public override bool ReadAttributeValue()
	{
		return false;
	}

	public override string ReadInnerXml()
	{
		if (_state != ReadState.Interactive)
		{
			return string.Empty;
		}
		return _value;
	}

	public override string ReadOuterXml()
	{
		if (_state != ReadState.Interactive)
		{
			return string.Empty;
		}
		return _value;
	}

	public override string ReadString()
	{
		if (_state != ReadState.Interactive)
		{
			return string.Empty;
		}
		return _value;
	}

	public override void ResolveEntity()
	{
	}

	public override bool Read()
	{
		switch (_state)
		{
		case ReadState.Initial:
			if (_value == null)
			{
				WebResponse response;
				Stream resource = XIncludingReader.GetResource(_includeLocation.AbsoluteUri, _accept, _acceptLanguage, out response);
				try
				{
					string text = response.ContentType.ToLower();
					StreamReader streamReader;
					if (text == "text/xml" || text == "application/xml" || (text.StartsWith("text/") && text.EndsWith("+xml")) || (text.StartsWith("application/") && text.EndsWith("+xml")))
					{
						streamReader = new StreamReader(resource, GetEncodingFromXMLDecl(_href));
					}
					else if (_encoding != null)
					{
						Encoding encoding;
						try
						{
							encoding = Encoding.GetEncoding(_encoding);
						}
						catch (Exception innerException)
						{
							throw new ResourceException(SR.GetString("NotSupportedEncoding", _encoding), innerException);
						}
						streamReader = new StreamReader(resource, encoding);
					}
					else
					{
						streamReader = new StreamReader(resource, Encoding.UTF8);
					}
					_value = streamReader.ReadToEnd();
					TextUtils.CheckForNonXmlChars(_value);
				}
				catch (ResourceException ex)
				{
					throw ex;
				}
				catch (OutOfMemoryException innerException2)
				{
					throw new ResourceException(SR.GetString("OutOfMemoryWhileFetchingResource", _href), innerException2);
				}
				catch (IOException innerException3)
				{
					throw new ResourceException(SR.GetString("IOErrorWhileFetchingResource", _href), innerException3);
				}
			}
			_state = ReadState.Interactive;
			return true;
		case ReadState.Interactive:
			_state = ReadState.EndOfFile;
			return false;
		default:
			return false;
		}
	}

	private Encoding GetEncodingFromXMLDecl(string href)
	{
		XmlTextReader xmlTextReader = new XmlTextReader(href);
		xmlTextReader.ProhibitDtd = false;
		xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
		try
		{
			while (xmlTextReader.Read() && xmlTextReader.Encoding == null)
			{
			}
			Encoding encoding = xmlTextReader.Encoding;
			return (encoding == null) ? Encoding.UTF8 : encoding;
		}
		finally
		{
			xmlTextReader.Close();
		}
	}
}
