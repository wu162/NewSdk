namespace Mvp.Xml.XInclude;

internal enum XIncludingReaderState
{
	Default,
	ExposingXmlBaseAttr,
	ExposingXmlBaseAttrValue,
	ExposingXmlLangAttr,
	ExposingXmlLangAttrValue
}
