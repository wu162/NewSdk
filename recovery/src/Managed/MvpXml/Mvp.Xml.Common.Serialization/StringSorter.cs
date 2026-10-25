using System.Collections.Generic;

namespace Mvp.Xml.Common.Serialization;

public class StringSorter
{
	private List<string> list = new List<string>();

	public void AddString(string s)
	{
		list.Add(s);
	}

	public string[] GetOrderedArray()
	{
		list.Sort();
		return list.ToArray();
	}
}
