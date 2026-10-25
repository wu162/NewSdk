using System.Reflection;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mvp.Xml.Exslt;

internal class ExsltContextFunction : IXsltContextFunction
{
	private MethodInfo _method;

	private XPathResultType[] _argTypes;

	private object _ownerObj;

	public int Minargs => _argTypes.Length;

	public int Maxargs => _argTypes.Length;

	public XPathResultType[] ArgTypes => _argTypes;

	public XPathResultType ReturnType => ExsltContext.ConvertToXPathType(_method.ReturnType);

	public ExsltContextFunction(MethodInfo mi, XPathResultType[] argTypes, object owner)
	{
		_method = mi;
		_argTypes = argTypes;
		_ownerObj = owner;
	}

	public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
	{
		return _method.Invoke(_ownerObj, args);
	}
}
