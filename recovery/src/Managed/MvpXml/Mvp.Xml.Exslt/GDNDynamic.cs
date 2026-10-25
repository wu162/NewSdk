using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt;

public class GDNDynamic
{
	public object evaluate(XPathNodeIterator contextNode, string expression)
	{
		return evaluate(contextNode, expression, "");
	}

	public object evaluate(XPathNodeIterator contextNode, string expression, string namespaces)
	{
		if (expression == string.Empty || contextNode == null)
		{
			return string.Empty;
		}
		if (contextNode.MoveNext())
		{
			try
			{
				XPathExpression xPathExpression = contextNode.Current.Compile(expression);
				ExsltContext exsltContext = new ExsltContext(contextNode.Current.NameTable);
				XPathNavigator xPathNavigator = contextNode.Current.Clone();
				if (xPathNavigator.NodeType != XPathNodeType.Element)
				{
					xPathNavigator.MoveToParent();
				}
				if (xPathNavigator.MoveToFirstNamespace())
				{
					do
					{
						exsltContext.AddNamespace(xPathNavigator.Name, xPathNavigator.Value);
					}
					while (xPathNavigator.MoveToNextNamespace());
				}
				if (namespaces != string.Empty)
				{
					try
					{
						Regex regex = new Regex("xmlns:(?<p>\\w+)\\s*=\\s*(('(?<n>.+)')|(\"(?<n>.+)\"))\\s*");
						Match match = regex.Match(namespaces);
						while (match.Success)
						{
							try
							{
								exsltContext.AddNamespace(match.Groups["p"].Value, match.Groups["n"].Value);
							}
							catch
							{
							}
							match = match.NextMatch();
						}
					}
					catch
					{
					}
				}
				xPathExpression.SetContext(exsltContext);
				return contextNode.Current.Evaluate(xPathExpression, contextNode);
			}
			catch
			{
				return string.Empty;
			}
		}
		return string.Empty;
	}
}
