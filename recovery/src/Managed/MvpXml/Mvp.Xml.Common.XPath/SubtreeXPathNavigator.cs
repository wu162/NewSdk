using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath;

public class SubtreeXPathNavigator : XPathNavigator
{
	private XPathNavigator _navigator;

	private XPathNavigator _root;

	private bool _atroot = true;

	private bool _fragment;

	private bool AtRoot => _atroot;

	private bool IsTop => _navigator.IsSamePosition(_root);

	public override string BaseURI
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.BaseURI;
			}
			return string.Empty;
		}
	}

	public override bool HasAttributes
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.HasAttributes;
			}
			return false;
		}
	}

	public override bool HasChildren
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.HasChildren;
			}
			return true;
		}
	}

	public override bool IsEmptyElement
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.IsEmptyElement;
			}
			return false;
		}
	}

	public override string LocalName
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.LocalName;
			}
			return string.Empty;
		}
	}

	public override string Name
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.Name;
			}
			return string.Empty;
		}
	}

	public override string NamespaceURI
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.NamespaceURI;
			}
			return string.Empty;
		}
	}

	public override XmlNameTable NameTable => _navigator.NameTable;

	public override XPathNodeType NodeType
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.NodeType;
			}
			return XPathNodeType.Root;
		}
	}

	public override string Prefix
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.Prefix;
			}
			return string.Empty;
		}
	}

	public override string Value
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.Value;
			}
			return string.Empty;
		}
	}

	public override string XmlLang
	{
		get
		{
			if (!AtRoot)
			{
				return _navigator.XmlLang;
			}
			return string.Empty;
		}
	}

	public SubtreeXPathNavigator(XPathNavigator navigator)
		: this(navigator, enableFragment: false)
	{
	}

	public SubtreeXPathNavigator(XPathNavigator navigator, bool enableFragment)
	{
		_navigator = navigator.Clone();
		_root = navigator.Clone();
		_fragment = enableFragment;
	}

	private SubtreeXPathNavigator(XPathNavigator root, XPathNavigator current, bool atRoot, bool enableFragment)
	{
		_root = root.Clone();
		_navigator = current.Clone();
		_atroot = atRoot;
		_fragment = enableFragment;
	}

	public override XPathNavigator Clone()
	{
		return new SubtreeXPathNavigator(_root, _navigator, _atroot, _fragment);
	}

	public override bool IsSamePosition(XPathNavigator other)
	{
		if (other == null || !(other is SubtreeXPathNavigator))
		{
			return false;
		}
		SubtreeXPathNavigator subtreeXPathNavigator = (SubtreeXPathNavigator)other;
		if (subtreeXPathNavigator._atroot == _atroot && subtreeXPathNavigator._navigator.IsSamePosition(_navigator))
		{
			return subtreeXPathNavigator._root.IsSamePosition(_root);
		}
		return false;
	}

	public override bool MoveToId(string id)
	{
		return _navigator.MoveToId(id);
	}

	public override bool MoveTo(XPathNavigator other)
	{
		if (other == null || !(other is SubtreeXPathNavigator))
		{
			return false;
		}
		return _navigator.MoveTo(((SubtreeXPathNavigator)other)._navigator);
	}

	public override bool MoveToFirst()
	{
		if (AtRoot)
		{
			return false;
		}
		if (IsTop)
		{
			if (!_fragment)
			{
				return false;
			}
			if (_root.MoveToFirst())
			{
				_navigator.MoveToFirst();
				return true;
			}
		}
		return _navigator.MoveToNext();
	}

	public override bool MoveToFirstChild()
	{
		if (AtRoot)
		{
			_atroot = false;
			return true;
		}
		return _navigator.MoveToFirstChild();
	}

	public override bool MoveToNext()
	{
		if (AtRoot)
		{
			return false;
		}
		if (IsTop)
		{
			if (!_fragment)
			{
				return false;
			}
			if (_root.MoveToNext())
			{
				_navigator.MoveToNext();
				return true;
			}
		}
		return _navigator.MoveToNext();
	}

	public override bool MoveToParent()
	{
		if (AtRoot)
		{
			return false;
		}
		if (IsTop)
		{
			_atroot = true;
			return true;
		}
		return _navigator.MoveToParent();
	}

	public override bool MoveToPrevious()
	{
		if (AtRoot)
		{
			return false;
		}
		if (IsTop)
		{
			if (!_fragment)
			{
				return false;
			}
			if (_root.MoveToPrevious())
			{
				_navigator.MoveToPrevious();
				return true;
			}
		}
		return _navigator.MoveToPrevious();
	}

	public override void MoveToRoot()
	{
		_navigator = _root.Clone();
		_atroot = true;
	}

	public override string GetAttribute(string localName, string namespaceURI)
	{
		if (!AtRoot)
		{
			return _navigator.GetAttribute(localName, namespaceURI);
		}
		return string.Empty;
	}

	public override bool MoveToAttribute(string localName, string namespaceURI)
	{
		if (!AtRoot)
		{
			return _navigator.MoveToAttribute(localName, namespaceURI);
		}
		return false;
	}

	public override bool MoveToFirstAttribute()
	{
		if (!AtRoot)
		{
			return _navigator.MoveToFirstAttribute();
		}
		return false;
	}

	public override bool MoveToNextAttribute()
	{
		if (!AtRoot)
		{
			return _navigator.MoveToNextAttribute();
		}
		return false;
	}

	public override string GetNamespace(string localName)
	{
		if (!AtRoot)
		{
			return _navigator.GetNamespace(localName);
		}
		return string.Empty;
	}

	public override bool MoveToNamespace(string @namespace)
	{
		if (!AtRoot)
		{
			return _navigator.MoveToNamespace(@namespace);
		}
		return false;
	}

	public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
	{
		if (!AtRoot)
		{
			return _navigator.MoveToFirstNamespace(namespaceScope);
		}
		return false;
	}

	public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
	{
		if (!AtRoot)
		{
			return _navigator.MoveToNextNamespace(namespaceScope);
		}
		return false;
	}
}
