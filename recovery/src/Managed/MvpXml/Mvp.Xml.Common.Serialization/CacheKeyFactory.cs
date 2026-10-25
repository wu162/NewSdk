using System;
using System.Text;
using System.Xml.Serialization;

namespace Mvp.Xml.Common.Serialization;

public static class CacheKeyFactory
{
	public static string MakeKey(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, string defaultNamespace)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(type.FullName);
		stringBuilder.Append("??");
		stringBuilder.Append(SignatureExtractor.GetOverridesSignature(overrides));
		stringBuilder.Append("??");
		stringBuilder.Append(SignatureExtractor.GetTypeArraySignature(types));
		stringBuilder.Append("??");
		stringBuilder.Append(SignatureExtractor.GetXmlRootSignature(root));
		stringBuilder.Append("??");
		stringBuilder.Append(SignatureExtractor.GetDefaultNamespaceSignature(defaultNamespace));
		return stringBuilder.ToString();
	}
}
