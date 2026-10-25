using System.Collections.Generic;
using System.Web.UI;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.Exslt;

public class GDNSets
{
	public bool subset(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		if (nodeset1.Count > 125 || nodeset2.Count > 125)
		{
			return subset2(nodeset1, nodeset2);
		}
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator(nodeset1, removeDuplicates: true);
		XPathNavigatorIterator xPathNavigatorIterator2 = new XPathNavigatorIterator(nodeset2, removeDuplicates: true);
		foreach (XPathNavigator item in xPathNavigatorIterator)
		{
			if (!xPathNavigatorIterator2.Contains(item))
			{
				return false;
			}
		}
		return true;
	}

	public bool subset2(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		List<Pair> arDocs = new List<Pair>();
		List<XPathNavigator> list = new List<XPathNavigator>(nodeset2.Count);
		while (nodeset2.MoveNext())
		{
			list.Add(nodeset2.Current.Clone());
		}
		auxEXSLT.findDocs(list, arDocs);
		while (nodeset1.MoveNext())
		{
			XPathNavigator current = nodeset1.Current;
			if (!auxEXSLT.findNode(list, arDocs, current))
			{
				return false;
			}
		}
		return true;
	}
}
