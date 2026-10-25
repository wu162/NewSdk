using System.Xml;

namespace Mvp.Xml.XInclude;

internal class XIncludeKeywords
{
	private const string s_XIncludeNamespace = "http://www.w3.org/2001/XInclude";

	private const string s_OldXIncludeNamespace = "http://www.w3.org/2003/XInclude";

	private const string s_Include = "include";

	private const string s_Href = "href";

	private const string s_Parse = "parse";

	private const string s_Xml = "xml";

	private const string s_Text = "text";

	private const string s_Xpointer = "xpointer";

	private const string s_Accept = "accept";

	private const string s_AcceptLanguage = "accept-language";

	private const string s_Encoding = "encoding";

	private const string s_Fallback = "fallback";

	private const string s_XmlNamespace = "http://www.w3.org/XML/1998/namespace";

	private const string s_Base = "base";

	private const string s_XmlBase = "xml:base";

	private const string s_Lang = "lang";

	private const string s_XmlLang = "xml:lang";

	private XmlNameTable nameTable;

	private string _XIncludeNamespace;

	private string _OldXIncludeNamespace;

	private string _Include;

	private string _Href;

	private string _Parse;

	private string _Xml;

	private string _Text;

	private string _Xpointer;

	private string _Accept;

	private string _AcceptLanguage;

	private string _Encoding;

	private string _Fallback;

	private string _XmlNamespace;

	private string _Base;

	private string _XmlBase;

	private string _Lang;

	private string _XmlLang;

	public string XIncludeNamespace => _XIncludeNamespace;

	public string OldXIncludeNamespace => _OldXIncludeNamespace;

	public string Include => _Include;

	public string Href => _Href;

	public string Parse => _Parse;

	public string Xml
	{
		get
		{
			if (_Xml == null)
			{
				_Xml = nameTable.Add("xml");
			}
			return _Xml;
		}
	}

	public string Text
	{
		get
		{
			if (_Text == null)
			{
				_Text = nameTable.Add("text");
			}
			return _Text;
		}
	}

	public string Xpointer
	{
		get
		{
			if (_Xpointer == null)
			{
				_Xpointer = nameTable.Add("xpointer");
			}
			return _Xpointer;
		}
	}

	public string Accept
	{
		get
		{
			if (_Accept == null)
			{
				_Accept = nameTable.Add("accept");
			}
			return _Accept;
		}
	}

	public string AcceptLanguage
	{
		get
		{
			if (_AcceptLanguage == null)
			{
				_AcceptLanguage = nameTable.Add("accept-language");
			}
			return _AcceptLanguage;
		}
	}

	public string Encoding
	{
		get
		{
			if (_Encoding == null)
			{
				_Encoding = nameTable.Add("encoding");
			}
			return _Encoding;
		}
	}

	public string Fallback
	{
		get
		{
			if (_Fallback == null)
			{
				_Fallback = nameTable.Add("fallback");
			}
			return _Fallback;
		}
	}

	public string XmlNamespace
	{
		get
		{
			if (_XmlNamespace == null)
			{
				_XmlNamespace = nameTable.Add("http://www.w3.org/XML/1998/namespace");
			}
			return _XmlNamespace;
		}
	}

	public string Base
	{
		get
		{
			if (_Base == null)
			{
				_Base = nameTable.Add("base");
			}
			return _Base;
		}
	}

	public string XmlBase
	{
		get
		{
			if (_XmlBase == null)
			{
				_XmlBase = nameTable.Add("xml:base");
			}
			return _XmlBase;
		}
	}

	public string Lang
	{
		get
		{
			if (_Lang == null)
			{
				_Lang = nameTable.Add("lang");
			}
			return _Lang;
		}
	}

	public string XmlLang
	{
		get
		{
			if (_XmlLang == null)
			{
				_XmlLang = nameTable.Add("xml:lang");
			}
			return _XmlLang;
		}
	}

	public XIncludeKeywords(XmlNameTable nt)
	{
		nameTable = nt;
		_XIncludeNamespace = nameTable.Add("http://www.w3.org/2001/XInclude");
		_OldXIncludeNamespace = nameTable.Add("http://www.w3.org/2003/XInclude");
		_Include = nameTable.Add("include");
		_Href = nameTable.Add("href");
		_Parse = nameTable.Add("parse");
	}

	public static bool Equals(string keyword1, string keyword2)
	{
		return (object)keyword1 == keyword2;
	}
}
