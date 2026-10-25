using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class GDNDatesAndTimes : ExsltDatesAndTimes
{
	public string avg(XPathNodeIterator iterator)
	{
		TimeSpan ts = new TimeSpan(0, 0, 0, 0);
		int count = iterator.Count;
		if (count == 0)
		{
			return "";
		}
		try
		{
			while (iterator.MoveNext())
			{
				ts = XmlConvert.ToTimeSpan(iterator.Current.Value).Add(ts);
			}
		}
		catch (FormatException)
		{
			return "";
		}
		return duration(ts.TotalSeconds / (double)count);
	}

	public string min(XPathNodeIterator iterator)
	{
		if (iterator.Count == 0)
		{
			return "";
		}
		TimeSpan timeSpan;
		try
		{
			iterator.MoveNext();
			timeSpan = XmlConvert.ToTimeSpan(iterator.Current.Value);
			while (iterator.MoveNext())
			{
				TimeSpan timeSpan2 = XmlConvert.ToTimeSpan(iterator.Current.Value);
				timeSpan = ((timeSpan2 < timeSpan) ? timeSpan2 : timeSpan);
			}
		}
		catch (FormatException)
		{
			return "";
		}
		return XmlConvert.ToString(timeSpan);
	}

	public string max(XPathNodeIterator iterator)
	{
		if (iterator.Count == 0)
		{
			return "";
		}
		TimeSpan timeSpan;
		try
		{
			iterator.MoveNext();
			timeSpan = XmlConvert.ToTimeSpan(iterator.Current.Value);
			while (iterator.MoveNext())
			{
				TimeSpan timeSpan2 = XmlConvert.ToTimeSpan(iterator.Current.Value);
				timeSpan = ((timeSpan2 > timeSpan) ? timeSpan2 : timeSpan);
			}
		}
		catch (FormatException)
		{
			return "";
		}
		return XmlConvert.ToString(timeSpan);
	}

	public string day_002Dabbreviation(string d, string c)
	{
		return dayAbbreviation(d, c);
	}

	public new string day_002Dabbreviation(string c)
	{
		return dayAbbreviation(c);
	}

	public new string dayAbbreviation(string culture)
	{
		try
		{
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetAbbreviatedDayName(DateTime.Now.DayOfWeek);
		}
		catch (Exception)
		{
			return "";
		}
	}

	public string dayAbbreviation(string d, string culture)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetAbbreviatedDayName(dateTZ.d.DayOfWeek);
		}
		catch (Exception)
		{
			return "";
		}
	}

	public string day_002Dname(string d, string c)
	{
		return dayName(d, c);
	}

	public new string day_002Dname(string c)
	{
		return dayName(c);
	}

	public string dayName(string d, string culture)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetDayName(dateTZ.d.DayOfWeek);
		}
		catch (Exception)
		{
			return "";
		}
	}

	public new string dayName(string culture)
	{
		try
		{
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
		}
		catch (Exception)
		{
			return "";
		}
	}

	public string month_002Dabbreviation(string d, string c)
	{
		return monthAbbreviation(d, c);
	}

	public new string month_002Dabbreviation(string c)
	{
		return monthAbbreviation(c);
	}

	public new string monthAbbreviation(string culture)
	{
		try
		{
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetAbbreviatedMonthName(DateTime.Now.Month);
		}
		catch (Exception)
		{
			return "";
		}
	}

	public string monthAbbreviation(string d, string culture)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetAbbreviatedMonthName(dateTZ.d.Month);
		}
		catch (Exception)
		{
			return "";
		}
	}

	public string month_002Dname(string d, string c)
	{
		return monthName(d, c);
	}

	public new string month_002Dname(string c)
	{
		return monthName(c);
	}

	public string monthName(string d, string culture)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetMonthName(dateTZ.d.Month);
		}
		catch (Exception)
		{
			return "";
		}
	}

	public new string monthName(string culture)
	{
		try
		{
			CultureInfo cultureInfo = new CultureInfo(culture);
			return cultureInfo.DateTimeFormat.GetMonthName(DateTime.Now.Month);
		}
		catch (Exception)
		{
			return "";
		}
	}
}
