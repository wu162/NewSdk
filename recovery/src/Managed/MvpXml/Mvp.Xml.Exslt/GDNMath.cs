using System;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class GDNMath
{
	public double avg(XPathNodeIterator iterator)
	{
		double num = 0.0;
		int count = iterator.Count;
		if (count == 0)
		{
			return double.NaN;
		}
		try
		{
			while (iterator.MoveNext())
			{
				num += XmlConvert.ToDouble(iterator.Current.Value);
			}
		}
		catch (FormatException)
		{
			return double.NaN;
		}
		return num / (double)count;
	}
}
