using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.Exslt;

public class ExsltSets
{
	public XPathNodeIterator difference(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		if (nodeset2.Count > 166)
		{
			return difference2(nodeset1, nodeset2);
		}
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator(nodeset1, removeDuplicates: true);
		XPathNavigatorIterator xPathNavigatorIterator2 = new XPathNavigatorIterator(nodeset2);
		for (int i = 0; i < xPathNavigatorIterator.Count; i++)
		{
			XPathNavigator value = xPathNavigatorIterator[i];
			if (xPathNavigatorIterator2.Contains(value))
			{
				xPathNavigatorIterator.RemoveAt(i);
				i--;
			}
		}
		xPathNavigatorIterator.Reset();
		return xPathNavigatorIterator;
	}

	private XPathNodeIterator difference2(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		List<Pair> arDocs = new List<Pair>();
		List<XPathNavigator> list = new List<XPathNavigator>(nodeset2.Count);
		while (nodeset2.MoveNext())
		{
			list.Add(nodeset2.Current.Clone());
		}
		auxEXSLT.findDocs(list, arDocs);
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator();
		while (nodeset1.MoveNext())
		{
			XPathNavigator current = nodeset1.Current;
			if (!auxEXSLT.findNode(list, arDocs, current))
			{
				xPathNavigatorIterator.Add(current.Clone());
			}
		}
		xPathNavigatorIterator.Reset();
		return xPathNavigatorIterator;
	}

	public XPathNodeIterator distinct(XPathNodeIterator nodeset)
	{
		if (nodeset.Count > 15)
		{
			return distinct2(nodeset);
		}
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator();
		while (nodeset.MoveNext())
		{
			if (!xPathNavigatorIterator.ContainsValue(nodeset.Current.Value))
			{
				xPathNavigatorIterator.Add(nodeset.Current.Clone());
			}
		}
		xPathNavigatorIterator.Reset();
		return xPathNavigatorIterator;
	}

	private XPathNodeIterator distinct2(XPathNodeIterator nodeset)
	{
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator();
		Dictionary<string, string> dictionary = new Dictionary<string, string>(nodeset.Count / 3);
		while (nodeset.MoveNext())
		{
			string value = nodeset.Current.Value;
			if (!dictionary.ContainsKey(value))
			{
				dictionary.Add(value, "");
				xPathNavigatorIterator.Add(nodeset.Current.Clone());
			}
		}
		xPathNavigatorIterator.Reset();
		return xPathNavigatorIterator;
	}

	public bool hasSameNode(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		if (nodeset1.Count >= 250 || nodeset2.Count >= 250)
		{
			return hasSameNode2(nodeset1, nodeset2);
		}
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator(nodeset1, removeDuplicates: true);
		XPathNavigatorIterator xPathNavigatorIterator2 = new XPathNavigatorIterator(nodeset2, removeDuplicates: true);
		foreach (XPathNavigator item in xPathNavigatorIterator)
		{
			if (xPathNavigatorIterator2.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	private bool hasSameNode2(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		XPathNodeIterator xPathNodeIterator = ((nodeset1.Count > nodeset2.Count) ? nodeset1 : nodeset2);
		XPathNodeIterator xPathNodeIterator2 = ((nodeset1.Count > nodeset2.Count) ? nodeset2 : nodeset1);
		List<Pair> arDocs = new List<Pair>();
		List<XPathNavigator> list = new List<XPathNavigator>(xPathNodeIterator.Count);
		while (xPathNodeIterator.MoveNext())
		{
			list.Add(xPathNodeIterator.Current.Clone());
		}
		auxEXSLT.findDocs(list, arDocs);
		new XPathNavigatorIterator();
		while (xPathNodeIterator2.MoveNext())
		{
			XPathNavigator current = xPathNodeIterator2.Current;
			if (auxEXSLT.findNode(list, arDocs, current))
			{
				return true;
			}
		}
		return false;
	}

	public bool has_002Dsame_002Dnode(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		return hasSameNode(nodeset1, nodeset2);
	}

	public XPathNodeIterator intersection(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		if (nodeset1.Count >= 500 || nodeset2.Count >= 500)
		{
			return intersection3(nodeset1, nodeset2);
		}
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator(nodeset1, removeDuplicates: true);
		XPathNavigatorIterator xPathNavigatorIterator2 = new XPathNavigatorIterator(nodeset2);
		for (int i = 0; i < xPathNavigatorIterator.Count; i++)
		{
			XPathNavigator value = xPathNavigatorIterator[i];
			if (!xPathNavigatorIterator2.Contains(value))
			{
				xPathNavigatorIterator.RemoveAt(i);
				i--;
			}
		}
		xPathNavigatorIterator.Reset();
		return xPathNavigatorIterator;
	}

	private XPathNodeIterator intersection3(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		XPathNodeIterator xPathNodeIterator = ((nodeset1.Count > nodeset2.Count) ? nodeset1 : nodeset2);
		XPathNodeIterator xPathNodeIterator2 = ((nodeset1.Count > nodeset2.Count) ? nodeset2 : nodeset1);
		List<Pair> arDocs = new List<Pair>();
		List<XPathNavigator> list = new List<XPathNavigator>(xPathNodeIterator.Count);
		while (xPathNodeIterator.MoveNext())
		{
			list.Add(xPathNodeIterator.Current.Clone());
		}
		auxEXSLT.findDocs(list, arDocs);
		XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator();
		while (xPathNodeIterator2.MoveNext())
		{
			XPathNavigator current = xPathNodeIterator2.Current;
			if (auxEXSLT.findNode(list, arDocs, current))
			{
				xPathNavigatorIterator.Add(current.Clone());
			}
		}
		xPathNavigatorIterator.Reset();
		return xPathNavigatorIterator;
	}

	public XPathNodeIterator leading(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		XPathNavigator xPathNavigator = null;
		if (nodeset2.MoveNext())
		{
			xPathNavigator = nodeset2.Current;
			XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator();
			while (nodeset1.MoveNext())
			{
				if (nodeset1.Current.ComparePosition(xPathNavigator) == XmlNodeOrder.Before)
				{
					xPathNavigatorIterator.Add(nodeset1.Current.Clone());
				}
			}
			xPathNavigatorIterator.Reset();
			return xPathNavigatorIterator;
		}
		return nodeset1;
	}

	public XPathNodeIterator trailing(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
	{
		XPathNavigator xPathNavigator = null;
		if (nodeset2.MoveNext())
		{
			xPathNavigator = nodeset2.Current;
			XPathNavigatorIterator xPathNavigatorIterator = new XPathNavigatorIterator();
			while (nodeset1.MoveNext())
			{
				if (nodeset1.Current.ComparePosition(xPathNavigator) == XmlNodeOrder.After)
				{
					xPathNavigatorIterator.Add(nodeset1.Current.Clone());
				}
			}
			xPathNavigatorIterator.Reset();
			return xPathNavigatorIterator;
		}
		return nodeset1;
	}
}
