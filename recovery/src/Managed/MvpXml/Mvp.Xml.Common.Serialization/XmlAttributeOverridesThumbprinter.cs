using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Mvp.Xml.Common.Serialization;

public static class XmlAttributeOverridesThumbprinter
{
	public static string GetThumbprint(XmlAttributeOverrides overrides)
	{
		return GetClassThumbprint(overrides);
	}

	private static string GetClassThumbprint(XmlAttributeOverrides overrides)
	{
		Hashtable typesHashtable = GetTypesHashtable(overrides);
		StringSorter stringSorter = new StringSorter();
		foreach (Type key in typesHashtable.Keys)
		{
			stringSorter.AddString(key.AssemblyQualifiedName);
		}
		string[] orderedArray = stringSorter.GetOrderedArray();
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = orderedArray;
		foreach (string text in array)
		{
			stringBuilder.AppendFormat(">>{0}>>", text);
			GetTypePrint(text, typesHashtable, stringBuilder);
			stringBuilder.Append("<<");
		}
		return stringBuilder.ToString();
	}

	private static void GetTypePrint(string typeName, Hashtable attributes, StringBuilder printBuilder)
	{
		Type type = Type.GetType(typeName);
		Hashtable hashtable = attributes[type] as Hashtable;
		StringSorter stringSorter = new StringSorter();
		foreach (string key in hashtable.Keys)
		{
			stringSorter.AddString(key);
		}
		string[] orderedArray = stringSorter.GetOrderedArray();
		string[] array = orderedArray;
		foreach (string text in array)
		{
			printBuilder.AppendFormat("**{0}**", text);
			GetXmlAttributesThumbprint(hashtable[text] as XmlAttributes, printBuilder);
			printBuilder.Append("***");
		}
	}

	private static void GetXmlAttributesThumbprint(XmlAttributes atts, StringBuilder printBuilder)
	{
		if (atts != null)
		{
			printBuilder.Append("any");
			printBuilder.Append(":");
			AddXmlAnyElementsPrint(atts.XmlAnyElements, printBuilder);
			printBuilder.Append(":");
			AddXmlArrayPrint(atts.XmlArray, printBuilder);
			printBuilder.Append(":");
			AddXmlArrayItemsPrint(atts.XmlArrayItems, printBuilder);
			printBuilder.Append(":");
			AddXmlAttributePrint(atts.XmlAttribute, printBuilder);
			printBuilder.Append(":");
			AddXmlChoiceIdentifierPrint(atts.XmlChoiceIdentifier, printBuilder);
			printBuilder.Append(":");
			AddXmlDefaultValuePrint(atts.XmlDefaultValue, printBuilder);
			printBuilder.Append(":");
			AddXmlElementsPrint(atts.XmlElements, printBuilder);
			printBuilder.Append(":");
			AddXmlEnumPrint(atts.XmlEnum, printBuilder);
			printBuilder.Append(":");
			AddXmlIgnorePrint(atts.XmlIgnore, printBuilder);
			printBuilder.Append(":");
			AddXmlNamespacePrint(atts.Xmlns, printBuilder);
			printBuilder.Append(":");
			AddXmlRootPrint(atts.XmlRoot, printBuilder);
			printBuilder.Append(":");
			AddXmlTextPrint(atts.XmlText, printBuilder);
			printBuilder.Append(":");
			AddXmlTypePrint(atts.XmlType, printBuilder);
		}
	}

	private static void AddXmlAnyElementsPrint(XmlAnyElementAttributes atts, StringBuilder printBuilder)
	{
		if (atts != null)
		{
			foreach (XmlAnyElementAttribute att in atts)
			{
				printBuilder.Append("anyatt");
				printBuilder.Append("/");
				printBuilder.Append(att.Name);
				printBuilder.Append("/");
				printBuilder.Append(att.Namespace);
				printBuilder.Append("::");
			}
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlArrayPrint(XmlArrayAttribute att, StringBuilder printBuilder)
	{
		if (att != null)
		{
			printBuilder.Append(att.ElementName);
			printBuilder.Append("/");
			printBuilder.Append(att.Form);
			printBuilder.Append("/");
			printBuilder.Append(att.IsNullable);
			printBuilder.Append("/");
			printBuilder.Append(att.Namespace);
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlArrayItemsPrint(XmlArrayItemAttributes atts, StringBuilder printBuilder)
	{
		if (atts != null)
		{
			foreach (XmlArrayItemAttribute att in atts)
			{
				printBuilder.Append(att.DataType);
				printBuilder.Append("/");
				printBuilder.Append(att.ElementName);
				printBuilder.Append("/");
				printBuilder.Append(att.Form);
				printBuilder.Append("/");
				printBuilder.Append(att.IsNullable);
				printBuilder.Append("/");
				printBuilder.Append(att.Namespace);
				printBuilder.Append("/");
				printBuilder.Append(att.NestingLevel);
				if (att.Type != null)
				{
					printBuilder.Append("/");
					printBuilder.Append(att.Type.AssemblyQualifiedName);
				}
				printBuilder.Append("::");
			}
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlAttributePrint(XmlAttributeAttribute att, StringBuilder printBuilder)
	{
		if (att != null)
		{
			printBuilder.Append(att.AttributeName);
			printBuilder.Append("/");
			printBuilder.Append(att.DataType);
			printBuilder.Append("/");
			printBuilder.Append(att.Form);
			printBuilder.Append("/");
			printBuilder.Append(att.Namespace);
			printBuilder.Append("/");
			if (att.Type != null)
			{
				printBuilder.Append(att.Type.AssemblyQualifiedName);
			}
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlChoiceIdentifierPrint(XmlChoiceIdentifierAttribute att, StringBuilder printBuilder)
	{
		if (att != null)
		{
			printBuilder.Append(att.MemberName);
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlDefaultValuePrint(object defaultValue, StringBuilder printBuilder)
	{
		if (defaultValue != null)
		{
			printBuilder.Append(defaultValue);
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlElementsPrint(XmlElementAttributes atts, StringBuilder printBuilder)
	{
		if (atts != null)
		{
			foreach (XmlElementAttribute att in atts)
			{
				printBuilder.Append(att.DataType);
				printBuilder.Append("/");
				printBuilder.Append(att.ElementName);
				printBuilder.Append("/");
				printBuilder.Append(att.Form);
				printBuilder.Append("/");
				printBuilder.Append(att.IsNullable);
				printBuilder.Append("/");
				printBuilder.Append(att.Namespace);
				printBuilder.Append("/");
				if (att.Type != null)
				{
					printBuilder.Append(att.Type.AssemblyQualifiedName);
				}
				printBuilder.Append("::");
			}
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlEnumPrint(XmlEnumAttribute att, StringBuilder printBuilder)
	{
		if (att != null)
		{
			printBuilder.Append(att.Name);
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlIgnorePrint(bool ignore, StringBuilder printBuilder)
	{
		printBuilder.Append(ignore);
		printBuilder.Append("%%");
	}

	private static void AddXmlNamespacePrint(bool Xmlns, StringBuilder printBuilder)
	{
		printBuilder.Append(Xmlns);
		printBuilder.Append("%%");
	}

	internal static void AddXmlRootPrint(XmlRootAttribute root, StringBuilder printBuilder)
	{
		if (root != null)
		{
			printBuilder.Append(root.DataType);
			printBuilder.Append("/");
			printBuilder.Append(root.ElementName);
			printBuilder.Append("/");
			printBuilder.Append(root.IsNullable);
			printBuilder.Append("/");
			printBuilder.Append(root.Namespace);
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlTextPrint(XmlTextAttribute att, StringBuilder printBuilder)
	{
		if (att != null)
		{
			printBuilder.Append(att.DataType);
			printBuilder.Append("/");
			if (att.Type != null)
			{
				printBuilder.Append(att.Type.AssemblyQualifiedName);
			}
		}
		printBuilder.Append("%%");
	}

	private static void AddXmlTypePrint(XmlTypeAttribute att, StringBuilder printBuilder)
	{
		if (att != null)
		{
			printBuilder.Append(att.IncludeInSchema);
			printBuilder.Append("/");
			printBuilder.Append(att.Namespace);
			printBuilder.Append("/");
			printBuilder.Append(att.TypeName);
		}
		printBuilder.Append("%%");
	}

	private static Hashtable GetTypesHashtable(XmlAttributeOverrides overrides)
	{
		return GetHashtable(overrides, "types");
	}

	private static Hashtable GetHashtable(XmlAttributeOverrides overrides, string fieldName)
	{
		FieldInfo field = typeof(XmlAttributeOverrides).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
		if (field == null)
		{
			throw new ArgumentException("XmlAttributeOverrides does not conform to expected structure");
		}
		return field.GetValue(overrides) as Hashtable;
	}
}
