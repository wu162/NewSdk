using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EALABuild;

public class GUIElementData
{
	private static List<List<string>> stringList = new List<List<string>>();

	private string dataKey;

	private string label;

	private Type controlType;

	public static Type CheckBoxType = typeof(bool);

	public static Type InputBoxType = typeof(string);

	public static Type LabelType = typeof(Label);

	public string DataKey => dataKey;

	public string LabelString
	{
		get
		{
			return label;
		}
		set
		{
			label = value;
		}
	}

	public Type ControlType
	{
		get
		{
			return controlType;
		}
		set
		{
			if (value != LabelType && value != CheckBoxType && value != InputBoxType)
			{
				throw new ArgumentException("Invalid type passed to ControlType");
			}
			controlType = value;
		}
	}

	public GUIElementData(string key, string controlLabel, Type controlTypeData, ScriptIndex scriptIndex)
	{
		if (stringList.Count < (int)(scriptIndex + 1))
		{
			stringList.Add(new List<string>());
			stringList[(int)scriptIndex].Add(key);
		}
		else
		{
			if (stringList[(int)scriptIndex].Contains(key))
			{
				throw new ArgumentException($"Duplicate key specified {key} for index {scriptIndex}");
			}
			stringList[(int)scriptIndex].Add(key);
		}
		dataKey = key;
		LabelString = controlLabel;
		ControlType = controlTypeData;
	}
}
