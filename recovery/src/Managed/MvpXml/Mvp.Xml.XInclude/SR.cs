using System.Resources;

namespace Mvp.Xml.XInclude;

internal sealed class SR
{
	private static ResourceManager resourceManager = new ResourceManager(typeof(SR).FullName, typeof(SR).Module.Assembly);

	public static string AttributeOrNamespaceInIncludeLocationError => GetString("AttributeOrNamespaceInIncludeLocationError");

	public static string IntradocumentReferencesNotSupported => GetString("IntradocumentReferencesNotSupported");

	public static string CustomXmlResolverError => GetString("CustomXmlResolverError");

	public static string CustomXmlResolverReturnedNull => GetString("CustomXmlResolverError");

	public static string MalformedXInclusionResult => GetString("MalformedXInclusionResult");

	public static string FragmentIDInHref => GetString("FragmentIDInHref");

	private SR()
	{
	}

	public static object GetObject(string key)
	{
		return resourceManager.GetObject(key);
	}

	public static string GetString(string key)
	{
		return resourceManager.GetString(key);
	}

	internal static string GetString(string key, params object[] args)
	{
		return string.Format(GetString(key), args);
	}
}
