using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.Exslt;

public class ExsltMath
{
	public double min(XPathNodeIterator iterator)
	{
		if (iterator.Count == 0)
		{
			return double.NaN;
		}
		try
		{
			iterator.MoveNext();
			double num = XmlConvert.ToDouble(iterator.Current.Value);
			while (iterator.MoveNext())
			{
				double num2 = XmlConvert.ToDouble(iterator.Current.Value);
				num = ((num2 < num) ? num2 : num);
			}
			return num;
		}
		catch
		{
			return double.NaN;
		}
	}

	public double max(XPathNodeIterator iterator)
	{
		if (iterator.Count == 0)
		{
			return double.NaN;
		}
		try
		{
			iterator.MoveNext();
			double num = XmlConvert.ToDouble(iterator.Current.Value);
			while (iterator.MoveNext())
			{
				double num2 = XmlConvert.ToDouble(iterator.Current.Value);
				num = ((num2 > num) ? num2 : num);
			}
			return num;
		}
		catch
		{
			return double.NaN;
		}
	}

	public XPathNodeIterator highest(XPathNodeIterator iterator)
	{
		if (iterator.Count == 0)
		{
			return EmptyXPathNodeIterator.Instance;
		}
		List<XPathNavigator> list = new List<XPathNavigator>();
		try
		{
			iterator.MoveNext();
			double num = XmlConvert.ToDouble(iterator.Current.Value);
			list.Add(iterator.Current.Clone());
			while (iterator.MoveNext())
			{
				double num2 = XmlConvert.ToDouble(iterator.Current.Value);
				if (num2 > num)
				{
					num = num2;
					list.Clear();
					list.Add(iterator.Current.Clone());
				}
				else if (num2 == num)
				{
					list.Add(iterator.Current.Clone());
				}
			}
		}
		catch
		{
			return EmptyXPathNodeIterator.Instance;
		}
		return new XPathNavigatorIterator(list);
	}

	public XPathNodeIterator lowest(XPathNodeIterator iterator)
	{
		if (iterator.Count == 0)
		{
			return EmptyXPathNodeIterator.Instance;
		}
		List<XPathNavigator> list = new List<XPathNavigator>();
		try
		{
			iterator.MoveNext();
			double num = XmlConvert.ToDouble(iterator.Current.Value);
			list.Add(iterator.Current.Clone());
			while (iterator.MoveNext())
			{
				double num2 = XmlConvert.ToDouble(iterator.Current.Value);
				if (num2 < num)
				{
					num = num2;
					list.Clear();
					list.Add(iterator.Current.Clone());
				}
				else if (num2 == num)
				{
					list.Add(iterator.Current.Clone());
				}
			}
		}
		catch
		{
			return EmptyXPathNodeIterator.Instance;
		}
		return new XPathNavigatorIterator(list);
	}

	public double abs(double number)
	{
		return Math.Abs(number);
	}

	public double sqrt(double number)
	{
		if (number < 0.0)
		{
			return 0.0;
		}
		return Math.Sqrt(number);
	}

	public double power(double x, double y)
	{
		return Math.Pow(x, y);
	}

	public double log(double x)
	{
		return Math.Log(x);
	}

	public double constant(string c, double precision)
	{
		return c.ToUpper() switch
		{
			"E" => Math.E, 
			"PI" => Math.PI, 
			"SQRRT2" => Math.Sqrt(2.0), 
			"LN2" => Math.Log(2.0), 
			"LN10" => Math.Log(10.0), 
			"LOG2E" => Math.Log(Math.E, 2.0), 
			"SQRT1_2" => Math.Sqrt(0.5), 
			_ => double.NaN, 
		};
	}

	public double random()
	{
		Random random = new Random((int)DateTime.Now.Ticks);
		return random.NextDouble();
	}

	public double sin(double x)
	{
		return Math.Sin(x);
	}

	public double asin(double x)
	{
		return Math.Asin(x);
	}

	public double cos(double x)
	{
		return Math.Cos(x);
	}

	public double acos(double x)
	{
		return Math.Acos(x);
	}

	public double tan(double x)
	{
		return Math.Tan(x);
	}

	public double atan(double x)
	{
		return Math.Atan(x);
	}

	public double atan2(double x, double y)
	{
		return Math.Atan2(x, y);
	}

	public double exp(double x)
	{
		return Math.Exp(x);
	}
}
