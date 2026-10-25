using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath;

public class SingletonXPathNodeIterator : XPathNodeIterator
{
	private XPathNavigator navigator;

	private int position;

	public override int Count => 1;

	public override XPathNavigator Current => navigator;

	public override int CurrentPosition => position;

	public SingletonXPathNodeIterator(XPathNavigator nav)
	{
		navigator = nav;
	}

	public override XPathNodeIterator Clone()
	{
		return new SingletonXPathNodeIterator(navigator.Clone());
	}

	public override bool MoveNext()
	{
		if (position == 0)
		{
			position = 1;
			return true;
		}
		return false;
	}
}
