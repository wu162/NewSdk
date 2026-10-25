using System;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class ExsltRandom
{
	public XPathNodeIterator randomSequence()
	{
		return randomSequenceImpl(1.0, (int)DateTime.Now.Ticks);
	}

	public XPathNodeIterator random_002Dsequence()
	{
		return randomSequence();
	}

	public XPathNodeIterator randomSequence(double number)
	{
		return randomSequenceImpl(number, (int)DateTime.Now.Ticks);
	}

	public XPathNodeIterator random_002Dsequence(double number)
	{
		return randomSequence(number);
	}

	public XPathNodeIterator randomSequence(double number, double seed)
	{
		return randomSequenceImpl(number, (int)(seed % 2147483647.0));
	}

	public XPathNodeIterator random_002Dsequence(double number, double seed)
	{
		return randomSequence(number, seed);
	}

	private XPathNodeIterator randomSequenceImpl(double number, int seed)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<randoms/>");
		if (seed == int.MinValue)
		{
			seed++;
		}
		Random random = new Random(seed);
		if (number < 0.0)
		{
			number = 1.0;
		}
		if (number > 2147483647.0)
		{
			number = 2147483647.0;
		}
		for (int i = 0; i < Convert.ToInt32(number); i++)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("random");
			xmlElement.InnerText = random.NextDouble().ToString();
			xmlDocument.DocumentElement.AppendChild(xmlElement);
		}
		return xmlDocument.CreateNavigator().Select("/randoms/random");
	}
}
