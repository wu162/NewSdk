using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mvp.Xml.Common.XPath;

public class IndexingXPathNavigator : XPathNavigator
{
	private class IndexingXsltContext : XsltContext
	{
		private KeyExtensionFunction keyFuncImpl;

		public override bool Whitespace => true;

		public IndexingXsltContext(XPathNavigatorIndexManager manager, XmlNameTable nt)
			: base(nt as NameTable)
		{
			keyFuncImpl = new KeyExtensionFunction(manager);
		}

		public override int CompareDocument(string baseUri, string nextbaseUri)
		{
			return 0;
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			return true;
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			return null;
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
		{
			if (prefix.Length == 0 && name == "key")
			{
				if (argTypes.Length != 2)
				{
					throw new ArgumentException(SR.GetString(SR.IndexingXPathNavigator_KeyWrongArguments));
				}
				if (argTypes[0] != XPathResultType.String)
				{
					throw new ArgumentException(SR.GetString(SR.IndexingXPathNavigator_KeyArgumentNotString));
				}
				return keyFuncImpl;
			}
			return null;
		}
	}

	private class KeyExtensionFunction : IXsltContextFunction
	{
		private const int args = 2;

		private static readonly XPathResultType[] argTypes = new XPathResultType[2]
		{
			XPathResultType.String,
			XPathResultType.Any
		};

		private XPathNavigatorIndexManager manager;

		public int Minargs => 2;

		public int Maxargs => 2;

		public XPathResultType[] ArgTypes => argTypes;

		public XPathResultType ReturnType => XPathResultType.NodeSet;

		public KeyExtensionFunction(XPathNavigatorIndexManager manager)
		{
			this.manager = manager;
		}

		public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
		{
			return manager.GetNodes((string)args[0], args[1]);
		}
	}

	private class XPathNavigatorIndexManager
	{
		private IDictionary<string, XPathNavigatorIndex> indexes;

		private XPathNavigator nav;

		private bool indexed;

		public void AddKey(XPathNavigator nav, string indexName, KeyDef key)
		{
			indexed = false;
			this.nav = nav;
			if (indexes == null)
			{
				indexes = new Dictionary<string, XPathNavigatorIndex>();
			}
			if (!indexes.TryGetValue(indexName, out var value))
			{
				value = new XPathNavigatorIndex();
				indexes.Add(indexName, value);
			}
			value.AddKey(key);
		}

		public void BuildIndexes()
		{
			XPathNavigator xPathNavigator = nav.Clone();
			xPathNavigator.MoveToRoot();
			XPathNodeIterator xPathNodeIterator = xPathNavigator.SelectDescendants(XPathNodeType.All, matchSelf: true);
			while (xPathNodeIterator.MoveNext())
			{
				if (xPathNodeIterator.Current.NodeType == XPathNodeType.Element)
				{
					if (xPathNodeIterator.Current.MoveToFirstNamespace())
					{
						do
						{
							foreach (XPathNavigatorIndex value in indexes.Values)
							{
								value.MatchNode(xPathNodeIterator.Current);
							}
						}
						while (xPathNodeIterator.Current.MoveToNextNamespace());
						xPathNodeIterator.Current.MoveToParent();
					}
					if (xPathNodeIterator.Current.MoveToFirstAttribute())
					{
						do
						{
							foreach (XPathNavigatorIndex value2 in indexes.Values)
							{
								value2.MatchNode(xPathNodeIterator.Current);
							}
						}
						while (xPathNodeIterator.Current.MoveToNextAttribute());
						xPathNodeIterator.Current.MoveToParent();
					}
				}
				foreach (XPathNavigatorIndex value3 in indexes.Values)
				{
					value3.MatchNode(xPathNodeIterator.Current);
				}
			}
			indexed = true;
		}

		public XPathNodeIterator GetNodes(string indexName, object value)
		{
			if (!indexed)
			{
				BuildIndexes();
			}
			indexes.TryGetValue(indexName, out var value2);
			return value2?.GetNodes(value);
		}
	}

	private class KeyDef
	{
		private string match;

		private string use;

		private XPathExpression matchExpr;

		private XPathExpression useExpr;

		private XPathNavigator nav;

		public string Match
		{
			get
			{
				return match;
			}
			set
			{
				match = value;
			}
		}

		public string Use
		{
			get
			{
				return use;
			}
			set
			{
				use = value;
			}
		}

		public XPathExpression MatchExpr
		{
			get
			{
				if (matchExpr == null)
				{
					matchExpr = nav.Compile(match);
				}
				return matchExpr;
			}
		}

		public XPathExpression UseExpr
		{
			get
			{
				if (useExpr == null)
				{
					useExpr = nav.Compile(use);
				}
				return useExpr;
			}
		}

		public KeyDef(XPathNavigator nav, string match, string use)
		{
			this.nav = nav;
			this.match = match;
			this.use = use;
		}

		public void Compile()
		{
			matchExpr = nav.Compile(match);
			useExpr = nav.Compile(use);
		}
	}

	private class XPathNavigatorIndex
	{
		private List<KeyDef> keys;

		private IDictionary<string, List<XPathNavigator>> index;

		public XPathNavigatorIndex()
		{
			keys = new List<KeyDef>();
			index = new Dictionary<string, List<XPathNavigator>>();
		}

		public void AddKey(KeyDef key)
		{
			keys.Add(key);
		}

		public XPathNodeIterator GetNodes(object keyValue)
		{
			List<XPathNavigator> value = null;
			if (keyValue is XPathNodeIterator)
			{
				XPathNodeIterator xPathNodeIterator = keyValue as XPathNodeIterator;
				while (xPathNodeIterator.MoveNext())
				{
					if (index.TryGetValue(xPathNodeIterator.Current.Value, out var value2))
					{
						if (value == null)
						{
							value = new List<XPathNavigator>();
						}
						value.AddRange(value2);
					}
				}
			}
			else
			{
				index.TryGetValue(keyValue.ToString(), out value);
			}
			if (value == null)
			{
				value = new List<XPathNavigator>(0);
			}
			return new XPathNavigatorIterator(value);
		}

		public void MatchNode(XPathNavigator node)
		{
			foreach (KeyDef key in keys)
			{
				if (!node.Matches(key.MatchExpr))
				{
					continue;
				}
				object obj = node.Evaluate(key.UseExpr);
				if (obj is XPathNodeIterator)
				{
					XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)obj;
					while (xPathNodeIterator.MoveNext())
					{
						AddNodeToIndex(node, xPathNodeIterator.Current.Value);
					}
				}
				else
				{
					AddNodeToIndex(node, obj.ToString());
				}
			}
		}

		private void AddNodeToIndex(XPathNavigator node, string key)
		{
			if (!index.TryGetValue(key, out var value))
			{
				value = new List<XPathNavigator>();
				index.Add(key, value);
			}
			value.Add(node.Clone());
		}
	}

	private XPathNavigator nav;

	private XPathNavigatorIndexManager manager;

	public override XPathNodeType NodeType => nav.NodeType;

	public override string LocalName => nav.LocalName;

	public override string Name => nav.Name;

	public override string NamespaceURI => nav.NamespaceURI;

	public override string Prefix => nav.Prefix;

	public override string Value => nav.Value;

	public override string BaseURI => nav.BaseURI;

	public override bool IsEmptyElement => nav.IsEmptyElement;

	public override string XmlLang => nav.XmlLang;

	public override XmlNameTable NameTable => nav.NameTable;

	public override bool HasAttributes => nav.HasAttributes;

	public override bool HasChildren => nav.HasChildren;

	public IndexingXPathNavigator(XPathNavigator navigator)
	{
		nav = navigator;
		manager = new XPathNavigatorIndexManager();
	}

	public void BuildIndexes()
	{
		manager.BuildIndexes();
	}

	public virtual void AddKey(string keyName, string match, string use)
	{
		KeyDef key = new KeyDef(nav, match, use);
		manager.AddKey(nav, keyName, key);
	}

	public override XPathExpression Compile(string xpath)
	{
		XPathExpression xPathExpression = base.Compile(xpath);
		xPathExpression.SetContext(new IndexingXsltContext(manager, nav.NameTable));
		return xPathExpression;
	}

	public override XPathNodeIterator Select(string xpath)
	{
		XPathExpression expr = Compile(xpath);
		return base.Select(expr);
	}

	public override XPathNavigator Clone()
	{
		return new IndexingXPathNavigator(nav.Clone());
	}

	public override string GetAttribute(string localName, string namespaceURI)
	{
		return nav.GetAttribute(localName, namespaceURI);
	}

	public override bool MoveToAttribute(string localName, string namespaceURI)
	{
		return nav.MoveToAttribute(localName, namespaceURI);
	}

	public override bool MoveToFirstAttribute()
	{
		return nav.MoveToFirstAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		return nav.MoveToNextAttribute();
	}

	public override string GetNamespace(string localname)
	{
		return nav.GetNamespace(localname);
	}

	public override bool MoveToNamespace(string @namespace)
	{
		return nav.MoveToNamespace(@namespace);
	}

	public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
	{
		return nav.MoveToFirstNamespace(namespaceScope);
	}

	public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
	{
		return nav.MoveToNextNamespace(namespaceScope);
	}

	public override bool MoveToNext()
	{
		return nav.MoveToNext();
	}

	public override bool MoveToPrevious()
	{
		return nav.MoveToPrevious();
	}

	public override bool MoveToFirst()
	{
		return nav.MoveToFirst();
	}

	public override bool MoveToFirstChild()
	{
		return nav.MoveToFirstChild();
	}

	public override bool MoveToParent()
	{
		return nav.MoveToParent();
	}

	public override void MoveToRoot()
	{
		nav.MoveToRoot();
	}

	public override bool MoveTo(XPathNavigator other)
	{
		return nav.MoveTo(other);
	}

	public override bool MoveToId(string id)
	{
		return nav.MoveToId(id);
	}

	public override bool IsSamePosition(XPathNavigator other)
	{
		return nav.IsSamePosition(other);
	}
}
