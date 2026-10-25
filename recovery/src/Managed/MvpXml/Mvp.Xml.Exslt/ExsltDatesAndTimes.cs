using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class ExsltDatesAndTimes
{
	private class ExsltDateTimeFactory
	{
		public static ExsltDateTime ParseDateTime(string d)
		{
			try
			{
				return ParseDate(d);
			}
			catch (FormatException)
			{
			}
			try
			{
				return new TimeTZ(d);
			}
			catch (FormatException)
			{
			}
			try
			{
				return new MonthDay(d);
			}
			catch (FormatException)
			{
			}
			try
			{
				return new Month(d);
			}
			catch (FormatException)
			{
			}
			return new Day(d);
		}

		public static ExsltDateTime ParseDate(string d)
		{
			try
			{
				return new DateTimeTZ(d);
			}
			catch (FormatException)
			{
			}
			try
			{
				return new DateTZ(d);
			}
			catch (FormatException)
			{
			}
			try
			{
				return new YearMonth(d);
			}
			catch (FormatException)
			{
			}
			return new YearTZ(d);
		}
	}

	internal abstract class ExsltDateTime
	{
		public DateTime d;

		public TimeSpan ts = new TimeSpan(TimeSpan.MinValue.Ticks);

		protected CultureInfo ci = new CultureInfo("en-US");

		protected abstract string[] expectedFormats { get; }

		protected abstract string outputFormat { get; }

		public ExsltDateTime()
		{
			d = DateTime.Now;
			TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
			ts = currentTimeZone.GetUtcOffset(d);
		}

		public ExsltDateTime(string inS)
		{
			string text = inS.Trim();
			d = DateTime.ParseExact(text, expectedFormats, ci, DateTimeStyles.AdjustToUniversal);
			if (text.EndsWith("Z"))
			{
				ts = new TimeSpan(0, 0, 0);
			}
			else
			{
				if (text.Length <= 6)
				{
					return;
				}
				string text2 = text.Substring(text.Length - 6, 6);
				if (text2[3] != ':')
				{
					return;
				}
				try
				{
					int num = int.Parse(text2.Substring(0, 3));
					int num2 = int.Parse(text2.Substring(4, 2));
					if (num < 0)
					{
						num2 = -num2;
					}
					ts = new TimeSpan(num, num2, 0);
					d = d.Add(ts);
				}
				catch (Exception)
				{
				}
			}
		}

		public ExsltDateTime(ExsltDateTime inS)
		{
			d = inS.d;
			ts = inS.ts;
		}

		public bool HasTimeZone()
		{
			return TimeSpan.MinValue.Ticks != ts.Ticks;
		}

		public DateTime ToUniversalTime()
		{
			if (!HasTimeZone())
			{
				return d;
			}
			return d.Subtract(ts);
		}

		public override string ToString()
		{
			return ToString(outputFormat);
		}

		public string ToString(string of)
		{
			StringBuilder stringBuilder = new StringBuilder("");
			stringBuilder.Append(d.ToString(of));
			stringBuilder.Append(GetTimeZone());
			return stringBuilder.ToString();
		}

		public string GetTimeZone()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (HasTimeZone())
			{
				if (ts.Hours == 0 && ts.Minutes == 0)
				{
					stringBuilder.Append('Z');
				}
				else if (ts.Hours >= 0 && ts.Minutes >= 0)
				{
					stringBuilder.Append('+');
					stringBuilder.Append(ts.Hours.ToString().PadLeft(2, '0'));
					stringBuilder.Append(':');
					stringBuilder.Append(ts.Minutes.ToString().PadLeft(2, '0'));
				}
				else
				{
					stringBuilder.Append('-');
					stringBuilder.Append((-ts.Hours).ToString().PadLeft(2, '0'));
					stringBuilder.Append(':');
					stringBuilder.Append((-ts.Minutes).ToString().PadLeft(2, '0'));
				}
			}
			return stringBuilder.ToString();
		}

		public string GetGMTOffsetTimeZone()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (HasTimeZone())
			{
				stringBuilder.Append("GMT");
				if (ts.Hours != 0 || ts.Minutes != 0)
				{
					stringBuilder.Append(GetTimeZone());
				}
			}
			return stringBuilder.ToString();
		}

		public string Get822TimeZone()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (HasTimeZone())
			{
				if (ts.Hours == 0 && ts.Minutes == 0)
				{
					stringBuilder.Append("GMT");
				}
				else if (ts.Hours >= 0 && ts.Minutes >= 0)
				{
					stringBuilder.Append('+');
					stringBuilder.Append(ts.Hours.ToString().PadLeft(2, '0'));
					stringBuilder.Append(ts.Minutes.ToString().PadLeft(2, '0'));
				}
				else
				{
					stringBuilder.Append('-');
					stringBuilder.Append((-ts.Hours).ToString().PadLeft(2, '0'));
					stringBuilder.Append((-ts.Minutes).ToString().PadLeft(2, '0'));
				}
			}
			return stringBuilder.ToString();
		}
	}

	internal class TimeTZ : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[27]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f", "HH:mm:sszzz", "HH:mm:ssZ", "HH:mm:ss"
		};

		protected override string outputFormat => "HH:mm:ss";

		public TimeTZ(string inS)
			: base(inS)
		{
		}

		public TimeTZ()
		{
		}
	}

	internal class MonthDay : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[26]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f", "yyyy-MM-dd", "--MM-dd"
		};

		protected override string outputFormat => "--MM-dd";

		public MonthDay()
		{
		}

		public MonthDay(string inS)
			: base(inS)
		{
		}

		public MonthDay(ExsltDateTime inS)
			: base(inS)
		{
		}
	}

	internal class Month : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[27]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f", "yyyy-MM-dd", "yyyy-MM", "--MM--"
		};

		protected override string outputFormat => "--MM--";

		public Month()
		{
		}

		public Month(string inS)
			: base(inS)
		{
		}

		public Month(ExsltDateTime inS)
			: base(inS)
		{
		}
	}

	internal class Day : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[27]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f", "yyyy-MM-dd", "---dd", "--MM-dd"
		};

		protected override string outputFormat => "---dd";

		public Day()
		{
		}

		public Day(string inS)
			: base(inS)
		{
		}

		public Day(ExsltDateTime inS)
			: base(inS)
		{
		}
	}

	internal class DateTimeTZ : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[24]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f"
		};

		protected override string outputFormat => "yyyy-MM-dd\"T\"HH:mm:ss";

		public DateTimeTZ()
		{
		}

		public DateTimeTZ(string inS)
			: base(inS)
		{
		}

		public DateTimeTZ(ExsltDateTime inS)
			: base(inS)
		{
		}
	}

	internal class DateTZ : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[27]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f", "yyyy-MM-ddzzz", "yyyy-MM-ddZ", "yyyy-MM-dd"
		};

		protected override string outputFormat => "yyyy-MM-dd";

		public DateTZ()
		{
		}

		public DateTZ(string inS)
			: base(inS)
		{
		}

		public DateTZ(ExsltDateTime inS)
			: base(inS)
		{
		}
	}

	internal class YearMonth : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[26]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f", "yyyy-MM-dd", "yyyy-MM"
		};

		protected override string outputFormat => "yyyy-MM";

		public YearMonth()
		{
		}

		public YearMonth(string inS)
			: base(inS)
		{
		}

		public YearMonth(ExsltDateTime inS)
			: base(inS)
		{
		}
	}

	internal class YearTZ : ExsltDateTime
	{
		protected override string[] expectedFormats => new string[27]
		{
			"yyyy-MM-dd\"T\"HH:mm:sszzz", "yyyy-MM-dd\"T\"HH:mm:ssZ", "yyyy-MM-dd\"T\"HH:mm:ss", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",
			"yyyy-MM-dd\"T\"HH:mm:ss.fffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fffff", "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ", "yyyy-MM-dd\"T\"HH:mm:ss.ffff", "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fffZ", "yyyy-MM-dd\"T\"HH:mm:ss.fff", "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz", "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",
			"yyyy-MM-dd\"T\"HH:mm:ss.ff", "yyyy-MM-dd\"T\"HH:mm:ss.fzzz", "yyyy-MM-dd\"T\"HH:mm:ss.fZ", "yyyy-MM-dd\"T\"HH:mm:ss.f", "yyyy-MM-dd", "yyyy-MM", "yyyy"
		};

		protected override string outputFormat => "yyyy";

		public YearTZ()
		{
		}

		public YearTZ(string inS)
			: base(inS)
		{
		}

		public YearTZ(ExsltDateTime inS)
			: base(inS)
		{
		}
	}

	private CultureInfo ci = new CultureInfo("en-US");

	private string[] dayAbbrevs = new string[7] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

	private string[] dayNames = new string[7] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

	private string[] monthAbbrevs = new string[12]
	{
		"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct",
		"Nov", "Dec"
	};

	private string[] monthNames = new string[12]
	{
		"January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
		"November", "December"
	};

	public string dateTime()
	{
		DateTimeTZ dtz = new DateTimeTZ();
		return dateTimeImpl(dtz);
	}

	public string date_002Dtime()
	{
		return dateTime();
	}

	public string dateTime(string s)
	{
		try
		{
			DateTimeTZ dtz = new DateTimeTZ(s);
			return dateTimeImpl(dtz);
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string date_002Dtime(string d)
	{
		return dateTime(d);
	}

	internal string dateTimeImpl(DateTimeTZ dtz)
	{
		return dtz.ToString();
	}

	public string date()
	{
		DateTZ dateTZ = new DateTZ();
		return dateTZ.ToString();
	}

	public string date(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return dateTZ.ToString();
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string time()
	{
		TimeTZ timeTZ = new TimeTZ();
		return timeTZ.ToString();
	}

	public string time(string d)
	{
		try
		{
			TimeTZ timeTZ = new TimeTZ(d);
			return timeTZ.ToString();
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public double year()
	{
		return DateTime.Now.Year;
	}

	public double year(string d)
	{
		try
		{
			YearTZ yearTZ = new YearTZ(d);
			return yearTZ.d.Year;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	private static bool IsLeapYear(int year)
	{
		try
		{
			return CultureInfo.CurrentCulture.Calendar.IsLeapYear(year);
		}
		catch
		{
			return false;
		}
	}

	public bool leapYear()
	{
		return IsLeapYear((int)year());
	}

	public bool leap_002Dyear()
	{
		return leapYear();
	}

	public bool leapYear(string d)
	{
		double num = year(d);
		if (num == double.NaN)
		{
			return false;
		}
		return IsLeapYear((int)num);
	}

	public bool leap_002Dyear(string d)
	{
		return leapYear(d);
	}

	public double monthInYear()
	{
		return DateTime.Now.Month;
	}

	public double month_002Din_002Dyear()
	{
		return monthInYear();
	}

	public double monthInYear(string d)
	{
		try
		{
			Month month = new Month(d);
			return month.d.Month;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double month_002Din_002Dyear(string d)
	{
		return monthInYear(d);
	}

	private double weekInYear(DateTime d)
	{
		return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
	}

	public double weekInYear()
	{
		return weekInYear(DateTime.Now);
	}

	public double week_002Din_002Dyear()
	{
		return weekInYear();
	}

	public double weekInYear(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return weekInYear(dateTZ.d);
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double week_002Din_002Dyear(string d)
	{
		return weekInYear(d);
	}

	private double weekInMonth(DateTime d)
	{
		double num = ((d.DayOfWeek == DayOfWeek.Sunday) ? 7.0 : ((double)d.DayOfWeek));
		return Math.Ceiling(((double)d.Day - num) / 7.0) + 1.0;
	}

	public double weekInMonth()
	{
		return weekInMonth(DateTime.Now);
	}

	public double week_002Din_002Dmonth()
	{
		return weekInMonth();
	}

	public double weekInMonth(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return weekInMonth(dateTZ.d);
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double week_002Din_002Dmonth(string d)
	{
		return weekInMonth(d);
	}

	public double dayInYear()
	{
		return DateTime.Now.DayOfYear;
	}

	public double day_002Din_002Dyear()
	{
		return dayInYear();
	}

	public double dayInYear(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return dateTZ.d.DayOfYear;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double day_002Din_002Dyear(string d)
	{
		return dayInYear(d);
	}

	public double dayInWeek()
	{
		return (double)(DateTime.Now.DayOfWeek + 1);
	}

	public double day_002Din_002Dweek()
	{
		return dayInWeek();
	}

	public double dayInWeek(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return (double)(dateTZ.d.DayOfWeek + 1);
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double day_002Din_002Dweek(string d)
	{
		return dayInWeek(d);
	}

	public double dayInMonth()
	{
		return DateTime.Now.Day;
	}

	public double day_002Din_002Dmonth()
	{
		return dayInMonth();
	}

	public double dayInMonth(string d)
	{
		try
		{
			Day day = new Day(d);
			return day.d.Day;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double day_002Din_002Dmonth(string d)
	{
		return dayInMonth(d);
	}

	private double dayOfWeekInMonth(int day)
	{
		return (day - 1) / 7 + 1;
	}

	public double dayOfWeekInMonth()
	{
		return dayOfWeekInMonth(DateTime.Now.Day);
	}

	public double day_002Dof_002Dweek_002Din_002Dmonth()
	{
		return dayOfWeekInMonth();
	}

	public double dayOfWeekInMonth(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return dayOfWeekInMonth(dateTZ.d.Day);
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double day_002Dof_002Dweek_002Din_002Dmonth(string d)
	{
		return dayOfWeekInMonth(d);
	}

	public double hourInDay()
	{
		return DateTime.Now.Hour;
	}

	public double hour_002Din_002Dday()
	{
		return hourInDay();
	}

	public double hourInDay(string d)
	{
		try
		{
			TimeTZ timeTZ = new TimeTZ(d);
			return timeTZ.d.Hour;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double hour_002Din_002Dday(string d)
	{
		return hourInDay(d);
	}

	public double minuteInHour()
	{
		return DateTime.Now.Minute;
	}

	public double minute_002Din_002Dhour()
	{
		return minuteInHour();
	}

	public double minuteInHour(string d)
	{
		try
		{
			TimeTZ timeTZ = new TimeTZ(d);
			return timeTZ.d.Minute;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double minute_002Din_002Dhour(string d)
	{
		return minuteInHour(d);
	}

	public double secondInMinute()
	{
		return DateTime.Now.Second;
	}

	public double second_002Din_002Dminute()
	{
		return secondInMinute();
	}

	public double secondInMinute(string d)
	{
		try
		{
			TimeTZ timeTZ = new TimeTZ(d);
			return timeTZ.d.Second;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public double second_002Din_002Dminute(string d)
	{
		return secondInMinute(d);
	}

	private string dayName(int dow)
	{
		if (dow < 0 || dow >= dayNames.Length)
		{
			return string.Empty;
		}
		return dayNames[dow];
	}

	public string dayName()
	{
		return dayName((int)DateTime.Now.DayOfWeek);
	}

	public string day_002Dname()
	{
		return dayName();
	}

	public string dayName(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return dayName((int)dateTZ.d.DayOfWeek);
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string day_002Dname(string d)
	{
		return dayName(d);
	}

	private string dayAbbreviation(int dow)
	{
		if (dow < 0 || dow >= dayAbbrevs.Length)
		{
			return string.Empty;
		}
		return dayAbbrevs[dow];
	}

	public string dayAbbreviation()
	{
		return dayAbbreviation((int)DateTime.Now.DayOfWeek);
	}

	public string day_002Dabbreviation()
	{
		return dayAbbreviation();
	}

	public string dayAbbreviation(string d)
	{
		try
		{
			DateTZ dateTZ = new DateTZ(d);
			return dayAbbreviation((int)dateTZ.d.DayOfWeek);
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string day_002Dabbreviation(string d)
	{
		return dayAbbreviation(d);
	}

	private string monthName(int month)
	{
		if (month < 1 || month > monthNames.Length)
		{
			return string.Empty;
		}
		return monthNames[month - 1];
	}

	public string monthName()
	{
		return monthName((int)monthInYear());
	}

	public string month_002Dname()
	{
		return monthName();
	}

	public string monthName(string d)
	{
		double num = monthInYear(d);
		if (num == double.NaN)
		{
			return "";
		}
		return monthName((int)num);
	}

	public string month_002Dname(string d)
	{
		return monthName(d);
	}

	private string monthAbbreviation(int month)
	{
		if (month < 1 || month > monthAbbrevs.Length)
		{
			return string.Empty;
		}
		return monthAbbrevs[month - 1];
	}

	public string monthAbbreviation()
	{
		return monthAbbreviation((int)monthInYear());
	}

	public string month_002Dabbreviation()
	{
		return monthAbbreviation();
	}

	public string monthAbbreviation(string d)
	{
		double num = monthInYear(d);
		if (num == double.NaN)
		{
			return "";
		}
		return monthAbbreviation((int)num);
	}

	public string month_002Dabbreviation(string d)
	{
		return monthAbbreviation(d);
	}

	public string formatDate(string d, string format)
	{
		try
		{
			ExsltDateTime exsltDateTime = ExsltDateTimeFactory.ParseDateTime(d);
			StringBuilder stringBuilder = new StringBuilder("");
			int i = 0;
			while (i < format.Length)
			{
				int num = i;
				switch (format[i])
				{
				case 'G':
					for (; i < format.Length && format[i] == 'G'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						if (exsltDateTime.d.Year < 0)
						{
							stringBuilder.Append("BC");
						}
						else
						{
							stringBuilder.Append("AD");
						}
					}
					break;
				case 'y':
					for (; i < format.Length && format[i] == 'y'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						if (i - num == 2)
						{
							stringBuilder.Append((exsltDateTime.d.Year % 100).ToString().PadLeft(i - num, '0'));
						}
						else
						{
							stringBuilder.Append(exsltDateTime.d.Year.ToString().PadLeft(i - num, '0'));
						}
					}
					break;
				case 'M':
					for (; i < format.Length && format[i] == 'M'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(Month)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(MonthDay)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						if (i - num <= 2)
						{
							stringBuilder.Append(exsltDateTime.d.Month.ToString().PadLeft(i - num, '0'));
						}
						else if (i - num == 3)
						{
							stringBuilder.Append(monthAbbreviation(exsltDateTime.d.Month));
						}
						else
						{
							stringBuilder.Append(monthName(exsltDateTime.d.Month));
						}
					}
					break;
				case 'd':
					for (; i < format.Length && format[i] == 'd'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(MonthDay)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(Day)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(exsltDateTime.d.Day.ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'h':
					for (; i < format.Length && format[i] == 'h'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						int num2 = exsltDateTime.d.Hour % 12;
						if (num2 == 0)
						{
							num2 = 12;
						}
						stringBuilder.Append(num2.ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'H':
					for (; i < format.Length && format[i] == 'H'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(exsltDateTime.d.Hour.ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'm':
					for (; i < format.Length && format[i] == 'm'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(exsltDateTime.d.Minute.ToString().PadLeft(i - num, '0'));
					}
					break;
				case 's':
					for (; i < format.Length && format[i] == 's'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(exsltDateTime.d.Second.ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'S':
					for (; i < format.Length && format[i] == 'S'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(exsltDateTime.d.Millisecond.ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'E':
					for (; i < format.Length && format[i] == 'E'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						if (i - num <= 3)
						{
							stringBuilder.Append(dayAbbreviation((int)exsltDateTime.d.DayOfWeek));
						}
						else
						{
							stringBuilder.Append(dayName((int)exsltDateTime.d.DayOfWeek));
						}
					}
					break;
				case 'D':
					for (; i < format.Length && format[i] == 'D'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(exsltDateTime.d.DayOfYear.ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'F':
					for (; i < format.Length && format[i] == 'F'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(MonthDay)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(Day)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(dayOfWeekInMonth(exsltDateTime.d.Day).ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'w':
					for (; i < format.Length && format[i] == 'w'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(weekInYear(exsltDateTime.d));
					}
					break;
				case 'W':
					for (; i < format.Length && format[i] == 'W'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append(weekInMonth(exsltDateTime.d));
					}
					break;
				case 'a':
					for (; i < format.Length && format[i] == 'a'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						if (exsltDateTime.d.Hour < 12)
						{
							stringBuilder.Append("AM");
						}
						else
						{
							stringBuilder.Append("PM");
						}
					}
					break;
				case 'k':
					for (; i < format.Length && format[i] == 'k'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append((exsltDateTime.d.Hour + 1).ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'K':
					for (; i < format.Length && format[i] == 'K'; i++)
					{
					}
					if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(TimeTZ)) || object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)))
					{
						stringBuilder.Append((exsltDateTime.d.Hour % 12).ToString().PadLeft(i - num, '0'));
					}
					break;
				case 'z':
					for (; i < format.Length && format[i] == 'z'; i++)
					{
					}
					stringBuilder.Append(exsltDateTime.GetGMTOffsetTimeZone());
					break;
				case 'Z':
					for (; i < format.Length && format[i] == 'Z'; i++)
					{
					}
					stringBuilder.Append(exsltDateTime.Get822TimeZone());
					break;
				case '\'':
					if (i < format.Length && format[i + 1] == '\'')
					{
						for (i++; i < format.Length && format[i] == '\''; i++)
						{
						}
						stringBuilder.Append("'");
						break;
					}
					i++;
					while (i < format.Length && format[i] != '\'' && i <= format.Length)
					{
						stringBuilder.Append(format.Substring(i++, 1));
					}
					if (i >= format.Length)
					{
						return "";
					}
					i++;
					break;
				default:
					stringBuilder.Append(format[i]);
					i++;
					break;
				}
			}
			return stringBuilder.ToString();
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string format_002Ddate(string d, string format)
	{
		return formatDate(d, format);
	}

	public string parseDate(string d, string format)
	{
		try
		{
			DateTime dateTime = DateTime.ParseExact(d, format, CultureInfo.CurrentCulture);
			return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.RoundtripKind);
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string parse_002Ddate(string d, string format)
	{
		return parseDate(d, format);
	}

	public string difference(string start, string end)
	{
		try
		{
			ExsltDateTime exsltDateTime = ExsltDateTimeFactory.ParseDate(start);
			ExsltDateTime exsltDateTime2 = ExsltDateTimeFactory.ParseDate(end);
			if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearTZ)) || object.ReferenceEquals(exsltDateTime2.GetType(), typeof(YearTZ)))
			{
				StringBuilder stringBuilder = new StringBuilder("");
				int num = exsltDateTime2.d.Year - exsltDateTime.d.Year;
				if (num < 0)
				{
					stringBuilder.Append('-');
				}
				stringBuilder.Append('P');
				stringBuilder.Append(Math.Abs(num));
				stringBuilder.Append('Y');
				return stringBuilder.ToString();
			}
			if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(YearMonth)) || object.ReferenceEquals(exsltDateTime2.GetType(), typeof(YearMonth)))
			{
				StringBuilder stringBuilder2 = new StringBuilder("");
				int num2 = exsltDateTime2.d.Year - exsltDateTime.d.Year;
				int num3 = exsltDateTime2.d.Month - exsltDateTime.d.Month;
				if (num2 > 0 && Math.Sign(num3) == -1)
				{
					num2--;
					num3 += 12;
				}
				else if (num2 < 0 && Math.Sign(num3) == 1)
				{
					num2++;
					num3 -= 12;
				}
				if (num2 < 0 || (num2 == 0 && num3 < 0))
				{
					stringBuilder2.Append('-');
				}
				stringBuilder2.Append('P');
				if (num2 != 0)
				{
					stringBuilder2.Append(Math.Abs(num2));
					stringBuilder2.Append('Y');
				}
				stringBuilder2.Append(Math.Abs(num3));
				stringBuilder2.Append('M');
				return stringBuilder2.ToString();
			}
			if (object.ReferenceEquals(exsltDateTime.GetType(), typeof(DateTZ)) || object.ReferenceEquals(exsltDateTime2.GetType(), typeof(DateTZ)))
			{
				exsltDateTime = new DateTZ(exsltDateTime.d.ToString("yyyy-MM-dd"));
				exsltDateTime2 = new DateTZ(exsltDateTime2.d.ToString("yyyy-MM-dd"));
			}
			TimeSpan timeSpan = exsltDateTime2.d.Subtract(exsltDateTime.d);
			return XmlConvert.ToString(timeSpan);
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string add(string datetime, string duration)
	{
		try
		{
			ExsltDateTime exsltDateTime = ExsltDateTimeFactory.ParseDate(datetime);
			Regex regex = new Regex("^(-)?P(?=\\d+|(?:T\\d+))(?:(\\d+)Y)?(?:(\\d+)M)?(?:(\\d+)D)?(?=T\\d+)?T?(?:(\\d+)H)?(?:(\\d+)M)?(?:(\\d+)S)?$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Match match = regex.Match(duration);
			int num = 1;
			int value = 0;
			int months = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			if (match.Success)
			{
				if (CultureInfo.InvariantCulture.CompareInfo.Compare(match.Groups[1].Value, "-") == 0)
				{
					num = -1;
				}
				if (match.Groups[2].Length > 0)
				{
					value = num * int.Parse(match.Groups[2].Value);
				}
				if (match.Groups[3].Length > 0)
				{
					months = num * int.Parse(match.Groups[3].Value);
				}
				if (match.Groups[4].Length > 0)
				{
					num2 = num * int.Parse(match.Groups[4].Value);
				}
				if (match.Groups[5].Length > 0)
				{
					num3 = num * int.Parse(match.Groups[5].Value);
				}
				if (match.Groups[6].Length > 0)
				{
					num4 = num * int.Parse(match.Groups[6].Value);
				}
				if (match.Groups[7].Length > 0)
				{
					num5 = num * int.Parse(match.Groups[7].Value);
				}
				exsltDateTime.d = exsltDateTime.d.AddYears(value);
				exsltDateTime.d = exsltDateTime.d.AddMonths(months);
				exsltDateTime.d = exsltDateTime.d.AddDays(num2);
				exsltDateTime.d = exsltDateTime.d.AddHours(num3);
				exsltDateTime.d = exsltDateTime.d.AddMinutes(num4);
				exsltDateTime.d = exsltDateTime.d.AddSeconds(num5);
				return exsltDateTime.ToString();
			}
			return "";
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string addDuration(string duration1, string duration2)
	{
		try
		{
			TimeSpan timeSpan = XmlConvert.ToTimeSpan(duration1);
			TimeSpan ts = XmlConvert.ToTimeSpan(duration2);
			return XmlConvert.ToString(timeSpan.Add(ts));
		}
		catch (FormatException)
		{
			return "";
		}
	}

	public string add_002Dduration(string duration1, string duration2)
	{
		return addDuration(duration1, duration2);
	}

	private double seconds(ExsltDateTime d)
	{
		DateTime value = new DateTime(1970, 1, 1, 0, 0, 0, 0, CultureInfo.InvariantCulture.Calendar);
		return d.ToUniversalTime().Subtract(value).TotalSeconds;
	}

	public double seconds()
	{
		return seconds(new DateTimeTZ());
	}

	public double seconds(string datetime)
	{
		try
		{
			return seconds(ExsltDateTimeFactory.ParseDate(datetime));
		}
		catch (FormatException)
		{
		}
		try
		{
			return XmlConvert.ToTimeSpan(datetime).TotalSeconds;
		}
		catch (FormatException)
		{
			return double.NaN;
		}
	}

	public string sum(XPathNodeIterator iterator)
	{
		TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0);
		if (iterator.Count == 0)
		{
			return "";
		}
		try
		{
			while (iterator.MoveNext())
			{
				timeSpan = XmlConvert.ToTimeSpan(iterator.Current.Value).Add(timeSpan);
			}
		}
		catch (FormatException)
		{
			return "";
		}
		return XmlConvert.ToString(timeSpan);
	}

	public string duration()
	{
		return duration(seconds());
	}

	public string duration(double seconds)
	{
		return XmlConvert.ToString(TimeSpan.FromSeconds(seconds));
	}
}
