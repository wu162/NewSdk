namespace Mvp.Xml.Common.XPath;

public struct XPathVariable
{
	private string _name;

	private object _value;

	public string Name => _name;

	public object Value => _value;

	public XPathVariable(string name, object value)
	{
		_name = name;
		_value = value;
	}

	public override bool Equals(object obj)
	{
		if (Name == ((XPathVariable)obj).Name)
		{
			return Value == ((XPathVariable)obj).Value;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Name + "." + Value.GetHashCode()).GetHashCode();
	}

	public static bool operator ==(XPathVariable a, XPathVariable b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(XPathVariable a, XPathVariable b)
	{
		return !a.Equals(b);
	}
}
