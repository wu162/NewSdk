using System;
using System.Text;
using System.Xml.Serialization;

namespace Mvp.Xml.Common.Serialization;

public static class SignatureExtractor
{
	public static string GetDefaultNamespaceSignature(string defaultNamespace)
	{
		return defaultNamespace;
	}

	public static string GetXmlRootSignature(XmlRootAttribute root)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XmlAttributeOverridesThumbprinter.AddXmlRootPrint(root, stringBuilder);
		return stringBuilder.ToString();
	}

	public static string GetOverridesSignature(XmlAttributeOverrides overrides)
	{
		string result = null;
		if (overrides != null)
		{
			result = XmlAttributeOverridesThumbprinter.GetThumbprint(overrides);
		}
		return result;
	}

	public static string GetTypeArraySignature(Type[] types)
	{
		string result = null;
		if (types != null && types.Length > 0)
		{
			new StringBuilder();
			StringSorter stringSorter = new StringSorter();
			foreach (Type type in types)
			{
				stringSorter.AddString(type.AssemblyQualifiedName);
			}
			result = string.Join(":", stringSorter.GetOrderedArray());
		}
		return result;
	}
}
