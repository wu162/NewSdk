using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath;

public interface IHasXPathNavigator
{
	XPathNavigator GetNavigator();
}
