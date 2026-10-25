using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath;

public class XPathNavigatorIterator : XPathNodeIterator
{
	private List<XPathNavigator> _navigators;

	private int _position = -1;

	public XPathNavigator this[int index]
	{
		get
		{
			return _navigators[index];
		}
		set
		{
			_navigators[index] = value;
		}
	}

	public override int Count => _navigators.Count;

	public override XPathNavigator Current
	{
		get
		{
			if (_position != -1)
			{
				return _navigators[_position];
			}
			return null;
		}
	}

	public override int CurrentPosition => _position + 1;

	public XPathNavigatorIterator()
	{
		_navigators = new List<XPathNavigator>();
	}

	public XPathNavigatorIterator(int capacity)
	{
		_navigators = new List<XPathNavigator>(capacity);
	}

	public XPathNavigatorIterator(XPathNavigator navigator)
		: this()
	{
		_navigators.Add(navigator);
	}

	public XPathNavigatorIterator(XPathNodeIterator iterator)
		: this(iterator, removeDuplicates: false)
	{
	}

	public XPathNavigatorIterator(XPathNodeIterator iterator, bool removeDuplicates)
		: this()
	{
		XPathNodeIterator xPathNodeIterator = iterator.Clone();
		while (xPathNodeIterator.MoveNext())
		{
			if (!removeDuplicates || !Contains(xPathNodeIterator.Current))
			{
				Add(xPathNodeIterator.Current.Clone());
			}
		}
	}

	public XPathNavigatorIterator(List<XPathNavigator> navigators)
	{
		_navigators = navigators;
	}

	public void Add(XPathNavigator navigator)
	{
		if (_position != -1)
		{
			throw new InvalidOperationException(SR.XPathNavigatorIterator_CantAddAfterMove);
		}
		_navigators.Add(navigator.Clone());
	}

	public void Add(XPathNodeIterator iterator)
	{
		if (_position != -1)
		{
			throw new InvalidOperationException(SR.XPathNavigatorIterator_CantAddAfterMove);
		}
		while (iterator.MoveNext())
		{
			_navigators.Add(iterator.Current.Clone());
		}
	}

	public void Add(IEnumerable<XPathNavigator> navigators)
	{
		if (_position != -1)
		{
			throw new InvalidOperationException(SR.XPathNavigatorIterator_CantAddAfterMove);
		}
		foreach (XPathNavigator navigator in navigators)
		{
			_navigators.Add(navigator.Clone());
		}
	}

	public bool Contains(XPathNavigator value)
	{
		foreach (XPathNavigator navigator in _navigators)
		{
			if (navigator.IsSamePosition(value))
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsValue(string value)
	{
		foreach (XPathNavigator navigator in _navigators)
		{
			if (navigator.Value.Equals(value))
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		_navigators.RemoveAt(index);
	}

	public void Reset()
	{
		_position = -1;
	}

	public override XPathNodeIterator Clone()
	{
		return new XPathNavigatorIterator(new List<XPathNavigator>(_navigators));
	}

	public override bool MoveNext()
	{
		if (_navigators.Count == 0)
		{
			return false;
		}
		_position++;
		if (_position < _navigators.Count)
		{
			return true;
		}
		return false;
	}
}
