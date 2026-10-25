using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath;

public class EmptyXPathNodeIterator : XPathNodeIterator
{
	public static EmptyXPathNodeIterator Instance = new EmptyXPathNodeIterator();

	public override int Count => 0;

	public override XPathNavigator Current => null;

	public override int CurrentPosition => 0;

	private EmptyXPathNodeIterator()
	{
	}

	public override XPathNodeIterator Clone()
	{
		return this;
	}

	public override bool MoveNext()
	{
		return false;
	}
}
