using System.Resources;

namespace Mvp.Xml.Common;

internal static class SR
{
	private static ResourceManager _resourceManager = new ResourceManager(typeof(SR).FullName, typeof(SR).Module.Assembly);

	public static string XmlNodeListFactory_IHasXmlNodeMissing => GetString("XmlNodeListFactory_IHasXmlNodeMissing");

	public static string IndexingXPathNavigator_KeyWrongArguments => GetString("IndexingXPathNavigator_KeyWrongArguments");

	public static string IndexingXPathNavigator_KeyArgumentNotString => GetString("IndexingXPathNavigator_KeyArgumentNotString");

	public static string XmlFragmentStream_EOF => GetString("XmlFragmentStream_EOF");

	public static string XPathCache_BadSortObject => GetString("XPathCache_BadSortObject");

	public static string XmlDocumentFactory_NotImplementedDOM => GetString("XmlDocumentFactory_NotImplementedDOM");

	public static string XPathNavigatorIterator_CantAddAfterMove => GetString("XPathNavigatorIterator_CantAddAfterMove");

	internal static object GetObject(string key)
	{
		return _resourceManager.GetObject(key);
	}

	internal static string GetString(string key)
	{
		return _resourceManager.GetString(key);
	}

	internal static string GetString(string key, params object[] args)
	{
		return string.Format(GetString(key), args);
	}
}
