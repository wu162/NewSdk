using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath;

public sealed class XPathCache
{
	private static IDictionary<string, XPathExpression> _cache = new Dictionary<string, XPathExpression>();

	private static IDictionary<string, XPathExpression> Cache => _cache;

	private XPathCache()
	{
	}

	private static XPathExpression GetCompiledExpression(string expression, XPathNavigator source)
	{
		if (!Cache.TryGetValue(expression, out var value))
		{
			value = source.Compile(expression);
			Cache[expression] = value;
		}
		return value.Clone();
	}

	private static XmlNamespaceManager PrepareContext(XPathNavigator source, XmlNamespaceManager context, XmlPrefix[] prefixes, XPathVariable[] variables)
	{
		XmlNamespaceManager xmlNamespaceManager = context;
		if (variables != null)
		{
			DynamicContext dynamicContext = ((xmlNamespaceManager == null) ? new DynamicContext() : new DynamicContext(xmlNamespaceManager));
			for (int i = 0; i < variables.Length; i++)
			{
				XPathVariable xPathVariable = variables[i];
				dynamicContext.AddVariable(xPathVariable.Name, xPathVariable.Value);
			}
			xmlNamespaceManager = dynamicContext;
		}
		if (prefixes != null)
		{
			if (xmlNamespaceManager == null)
			{
				xmlNamespaceManager = new XmlNamespaceManager(source.NameTable);
			}
			foreach (XmlPrefix xmlPrefix in prefixes)
			{
				xmlNamespaceManager.AddNamespace(xmlPrefix.Prefix, xmlPrefix.NamespaceURI);
			}
		}
		return xmlNamespaceManager;
	}

	private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
	{
		if (sortExpression is string)
		{
			expression.AddSort(GetCompiledExpression((string)sortExpression, source), order, caseOrder, lang, dataType);
			return;
		}
		if (sortExpression is XPathExpression)
		{
			expression.AddSort(sortExpression, order, caseOrder, lang, dataType);
			return;
		}
		throw new XPathException(SR.XPathCache_BadSortObject, null);
	}

	private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context)
	{
		XPathExpression xPathExpression;
		if (sortExpression is string)
		{
			xPathExpression = GetCompiledExpression((string)sortExpression, source);
		}
		else
		{
			if (!(sortExpression is XPathExpression))
			{
				throw new XPathException(SR.XPathCache_BadSortObject, null);
			}
			xPathExpression = (XPathExpression)sortExpression;
		}
		xPathExpression.SetContext(context);
		expression.AddSort(xPathExpression, order, caseOrder, lang, dataType);
	}

	private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, IComparer comparer)
	{
		if (sortExpression is string)
		{
			expression.AddSort(GetCompiledExpression((string)sortExpression, source), comparer);
			return;
		}
		if (sortExpression is XPathExpression)
		{
			expression.AddSort(sortExpression, comparer);
			return;
		}
		throw new XPathException(SR.XPathCache_BadSortObject, null);
	}

	private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, IComparer comparer, XmlNamespaceManager context)
	{
		XPathExpression xPathExpression;
		if (sortExpression is string)
		{
			xPathExpression = GetCompiledExpression((string)sortExpression, source);
		}
		else
		{
			if (!(sortExpression is XPathExpression))
			{
				throw new XPathException(SR.XPathCache_BadSortObject, null);
			}
			xPathExpression = (XPathExpression)sortExpression;
		}
		xPathExpression.SetContext(context);
		expression.AddSort(xPathExpression, comparer);
	}

	public static object Evaluate(string expression, XPathNavigator source)
	{
		return source.Evaluate(GetCompiledExpression(expression, source));
	}

	public static object Evaluate(string expression, XPathNavigator source, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, null, variables));
		return source.Evaluate(compiledExpression);
	}

	public static object Evaluate(string expression, XPathNavigator source, XmlNamespaceManager context)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(context);
		return source.Evaluate(compiledExpression);
	}

	public static object Evaluate(string expression, XPathNavigator source, params XmlPrefix[] prefixes)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, prefixes, null));
		return source.Evaluate(compiledExpression);
	}

	public static object Evaluate(string expression, XPathNavigator source, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, context, null, variables));
		return source.Evaluate(compiledExpression);
	}

	public static object Evaluate(string expression, XPathNavigator source, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, prefixes, variables));
		return source.Evaluate(compiledExpression);
	}

	public static XPathNodeIterator Select(string expression, XPathNavigator source)
	{
		return source.Select(GetCompiledExpression(expression, source));
	}

	public static XPathNodeIterator Select(string expression, XPathNavigator source, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, null, variables));
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator Select(string expression, XPathNavigator source, XmlNamespaceManager context)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(context);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator Select(string expression, XPathNavigator source, params XmlPrefix[] prefixes)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, prefixes, null));
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator Select(string expression, XPathNavigator source, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, context, null, variables));
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator Select(string expression, XPathNavigator source, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, prefixes, variables));
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		PrepareSort(compiledExpression, source, sortExpression, comparer);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, null, variables));
		PrepareSort(compiledExpression, source, sortExpression, comparer);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(PrepareContext(source, null, null, variables));
		PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		compiledExpression.SetContext(context);
		PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, params XmlPrefix[] prefixes)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		XmlNamespaceManager context = PrepareContext(source, null, prefixes, null);
		compiledExpression.SetContext(context);
		PrepareSort(compiledExpression, source, sortExpression, comparer, context);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XmlPrefix[] prefixes)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		XmlNamespaceManager context = PrepareContext(source, null, prefixes, null);
		compiledExpression.SetContext(context);
		PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		XmlNamespaceManager context2 = PrepareContext(source, context, null, variables);
		compiledExpression.SetContext(context2);
		PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context2);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		XmlNamespaceManager context2 = PrepareContext(source, context, null, variables);
		compiledExpression.SetContext(context2);
		PrepareSort(compiledExpression, source, sortExpression, comparer, context2);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		XmlNamespaceManager context = PrepareContext(source, null, prefixes, variables);
		compiledExpression.SetContext(context);
		PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context);
		return source.Select(compiledExpression);
	}

	public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		XPathExpression compiledExpression = GetCompiledExpression(expression, source);
		XmlNamespaceManager context = PrepareContext(source, null, prefixes, variables);
		compiledExpression.SetContext(context);
		PrepareSort(compiledExpression, source, sortExpression, comparer, context);
		return source.Select(compiledExpression);
	}

	public static XmlNodeList SelectNodes(string expression, XmlNode source)
	{
		XPathNodeIterator iterator = Select(expression, source.CreateNavigator());
		return XmlNodeListFactory.CreateNodeList(iterator);
	}

	public static XmlNodeList SelectNodes(string expression, XmlNode source, params XPathVariable[] variables)
	{
		XPathNodeIterator iterator = Select(expression, source.CreateNavigator(), variables);
		return XmlNodeListFactory.CreateNodeList(iterator);
	}

	public static XmlNodeList SelectNodes(string expression, XmlNode source, XmlNamespaceManager context)
	{
		XPathNodeIterator iterator = Select(expression, source.CreateNavigator(), context);
		return XmlNodeListFactory.CreateNodeList(iterator);
	}

	public static XmlNodeList SelectNodes(string expression, XmlNode source, params XmlPrefix[] prefixes)
	{
		XPathNodeIterator iterator = Select(expression, source.CreateNavigator(), prefixes);
		return XmlNodeListFactory.CreateNodeList(iterator);
	}

	public static XmlNodeList SelectNodes(string expression, XmlNode source, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		XPathNodeIterator iterator = Select(expression, source.CreateNavigator(), context, variables);
		return XmlNodeListFactory.CreateNodeList(iterator);
	}

	public static XmlNodeList SelectNodes(string expression, XmlNode source, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		XPathNodeIterator iterator = Select(expression, source.CreateNavigator(), prefixes, variables);
		return XmlNodeListFactory.CreateNodeList(iterator);
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, params XPathVariable[] variables)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, variables));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XPathVariable[] variables)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, variables));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, context));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, params XmlPrefix[] prefixes)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, prefixes));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XmlPrefix[] prefixes)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, prefixes));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, context, variables));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, context, variables));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, prefixes, variables));
	}

	public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		return XmlNodeListFactory.CreateNodeList(SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, prefixes, variables));
	}

	public static XmlNode SelectSingleNode(string expression, XmlNode source)
	{
		IEnumerator enumerator = SelectNodes(expression, source).GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				return (XmlNode)enumerator.Current;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public static XmlNode SelectSingleNode(string expression, XmlNode source, params XPathVariable[] variables)
	{
		IEnumerator enumerator = SelectNodes(expression, source, variables).GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				return (XmlNode)enumerator.Current;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public static XmlNode SelectSingleNode(string expression, XmlNode source, XmlNamespaceManager context)
	{
		IEnumerator enumerator = SelectNodes(expression, source, context).GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				return (XmlNode)enumerator.Current;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public static XmlNode SelectSingleNode(string expression, XmlNode source, params XmlPrefix[] prefixes)
	{
		IEnumerator enumerator = SelectNodes(expression, source, prefixes).GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				return (XmlNode)enumerator.Current;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public static XmlNode SelectSingleNode(string expression, XmlNode source, XmlNamespaceManager context, params XPathVariable[] variables)
	{
		IEnumerator enumerator = SelectNodes(expression, source, context, variables).GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				return (XmlNode)enumerator.Current;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public static XmlNode SelectSingleNode(string expression, XmlNode source, XmlPrefix[] prefixes, params XPathVariable[] variables)
	{
		IEnumerator enumerator = SelectNodes(expression, source, prefixes, variables).GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				return (XmlNode)enumerator.Current;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}
}
