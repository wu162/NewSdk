using System;
using System.Text;
using System.Xml;

namespace Mvp.Xml.XPointer;

internal class XPointerLexer
{
	public enum LexKind
	{
		NCName = 78,
		QName = 81,
		LRBracket = 40,
		RRBracket = 41,
		Circumflex = 94,
		Number = 100,
		Eq = 61,
		Space = 83,
		Slash = 47,
		EscapedData = 68,
		Eof = 69
	}

	private string _ptr;

	private int _ptrIndex;

	private LexKind _kind;

	private char _currChar;

	private int _number;

	private string _ncname;

	private string _prefix;

	private bool _canBeSchemaName;

	public LexKind Kind => _kind;

	public int Number => _number;

	public string NCName => _ncname;

	public string Prefix => _prefix;

	public bool CanBeSchemaName => _canBeSchemaName;

	private string ParseName()
	{
		int startIndex = _ptrIndex - 1;
		int num = 0;
		while (LexUtils.IsNCNameChar(_currChar))
		{
			NextChar();
			num++;
		}
		return _ptr.Substring(startIndex, num);
	}

	public XPointerLexer(string p)
	{
		if (p == null)
		{
			throw new ArgumentNullException("pointer", SR.NullXPointer);
		}
		_ptr = p;
		NextChar();
	}

	public bool NextChar()
	{
		if (_ptrIndex < _ptr.Length)
		{
			_currChar = _ptr[_ptrIndex++];
			return true;
		}
		_currChar = '\0';
		return false;
	}

	public void SkipWhiteSpace()
	{
		while (LexUtils.IsWhitespace(_currChar))
		{
			NextChar();
		}
	}

	public bool NextLexeme()
	{
		switch (_currChar)
		{
		case '\0':
			_kind = LexKind.Eof;
			return false;
		case '(':
		case ')':
		case '/':
		case '=':
			_kind = (LexKind)Convert.ToInt32(_currChar);
			NextChar();
			break;
		case '^':
			NextChar();
			if (_currChar == '^' || _currChar == '(' || _currChar == ')')
			{
				_kind = LexKind.EscapedData;
				NextChar();
				break;
			}
			throw new XPointerSyntaxException(SR.CircumflexCharMustBeEscaped);
		default:
			if (char.IsDigit(_currChar))
			{
				_kind = LexKind.Number;
				int startIndex = _ptrIndex - 1;
				int num = 0;
				while (char.IsDigit(_currChar))
				{
					NextChar();
					num++;
				}
				_number = XmlConvert.ToInt32(_ptr.Substring(startIndex, num));
			}
			else if (LexUtils.IsStartNameChar(_currChar))
			{
				_kind = LexKind.NCName;
				_prefix = string.Empty;
				_ncname = ParseName();
				if (_currChar == ':')
				{
					NextChar();
					_prefix = _ncname;
					_kind = LexKind.QName;
					if (!LexUtils.IsStartNCNameChar(_currChar))
					{
						throw new XPointerSyntaxException(SR.GetString("InvalidNameToken", _prefix, _currChar));
					}
					_ncname = ParseName();
				}
				_canBeSchemaName = _currChar == '(';
			}
			else if (LexUtils.IsWhitespace(_currChar))
			{
				_kind = LexKind.Space;
				while (LexUtils.IsWhitespace(_currChar))
				{
					NextChar();
				}
			}
			else
			{
				_kind = LexKind.EscapedData;
			}
			break;
		}
		return true;
	}

	public string ParseEscapedData()
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		do
		{
			switch (_currChar)
			{
			case '^':
				if (!NextChar())
				{
					throw new XPointerSyntaxException(SR.UnexpectedEndOfSchemeData);
				}
				if (_currChar == '^' || _currChar == '(' || _currChar == ')')
				{
					stringBuilder.Append(_currChar);
					continue;
				}
				throw new XPointerSyntaxException(SR.CircumflexCharMustBeEscaped);
			case '(':
				num++;
				break;
			case ')':
				if (num-- == 0)
				{
					NextLexeme();
					return stringBuilder.ToString();
				}
				break;
			}
			stringBuilder.Append(_currChar);
		}
		while (NextChar());
		throw new XPointerSyntaxException(SR.UnexpectedEndOfSchemeData);
	}
}
