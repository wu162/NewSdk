using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

internal class auxEXSLT
{
	public static bool findNode(List<XPathNavigator> arNodes1, List<Pair> arDocs, XPathNavigator currNode)
	{
		int num = -1;
		int num2 = -1;
		foreach (Pair arDoc in arDocs)
		{
			switch (arNodes1[(int)arDoc.First].ComparePosition(currNode))
			{
			case XmlNodeOrder.Same:
				return true;
			case XmlNodeOrder.Unknown:
				continue;
			}
			num = (int)arDoc.First;
			num2 = (int)arDoc.Second;
			break;
		}
		if (num == -1)
		{
			return false;
		}
		while (num2 >= num)
		{
			int num3 = (num + num2) / 2;
			switch (arNodes1[num3].ComparePosition(currNode))
			{
			case XmlNodeOrder.Before:
				num = num3 + 1;
				break;
			case XmlNodeOrder.After:
				num2 = num3 - 1;
				break;
			default:
				return true;
			}
		}
		return false;
	}

	public static void findDocs(List<XPathNavigator> arNodes, List<Pair> arDocs)
	{
		int count = arNodes.Count;
		int num = 0;
		while (num < count)
		{
			int num2 = num;
			int num3 = count - 1;
			int num4 = num3;
			int num5 = (num2 + num4) / 2;
			while (num4 > num2)
			{
				if (arNodes[num2].ComparePosition(arNodes[num5]) == XmlNodeOrder.Unknown)
				{
					num4 = num5 - 1;
					if (arNodes[num2].ComparePosition(arNodes[num4]) != XmlNodeOrder.Unknown)
					{
						num3 = num4;
						break;
					}
				}
				else
				{
					if (arNodes[num5].ComparePosition(arNodes[num5 + 1]) == XmlNodeOrder.Unknown)
					{
						num3 = num5;
						break;
					}
					num2 = num5 + 1;
				}
				num5 = (num2 + num4) / 2;
			}
			Pair item = new Pair(num, num3);
			arDocs.Add(item);
			num = num3 + 1;
		}
	}
}
