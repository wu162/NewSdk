using System.Resources;

namespace Mvp.Xml.XPointer;

internal static class SR
{
	private static ResourceManager resourceManager = new ResourceManager(typeof(SR).FullName, typeof(SR).Module.Assembly);

	public static string InvalidTokenInElementSchemeWhileNumberExpected => GetString("InvalidTokenInElementSchemeWhileNumberExpected");

	public static string ZeroIndexInElementSchemechildSequence => GetString("ZeroIndexInElementSchemechildSequence");

	public static string InvalidTokenInElementSchemeWhileClosingRoundBracketExpected => GetString("InvalidTokenInElementSchemeWhileClosingRoundBracketExpected");

	public static string EmptyElementSchemeXPointer => GetString("EmptyElementSchemeXPointer");

	public static string InvalidTokenInXmlnsSchemeWhileNCNameExpected => GetString("InvalidTokenInXmlnsSchemeWhileNCNameExpected");

	public static string InvalidTokenInXmlnsSchemeWhileEqualsSignExpected => GetString("InvalidTokenInXmlnsSchemeWhileEqualsSignExpected");

	public static string NullXPointer => GetString("NullXPointer");

	public static string CircumflexCharMustBeEscaped => GetString("CircumflexCharMustBeEscaped");

	public static string UnexpectedEndOfSchemeData => GetString("UnexpectedEndOfSchemeData");

	public static string InvalidTokenAfterShorthandPointer => GetString("InvalidTokenAfterShorthandPointer");

	public static string InvalidToken => GetString("InvalidToken");

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
