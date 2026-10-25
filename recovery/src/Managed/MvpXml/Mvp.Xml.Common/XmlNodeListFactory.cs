using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Common;

public sealed class XmlNodeListFactory
{
	private class XmlNodeListIterator : XmlNodeList
	{
		private class XmlNodeListEnumerator : IEnumerator
		{
			private XmlNodeListIterator _iterator;

			private int _position = -1;

			object IEnumerator.Current => _iterator[_position];

			public XmlNodeListEnumerator(XmlNodeListIterator iterator)
			{
				_iterator = iterator;
			}

			void IEnumerator.Reset()
			{
				_position = -1;
			}

			bool IEnumerator.MoveNext()
			{
				_position++;
				_iterator.ReadTo(_position);
				if (_iterator.Done && _position >= _iterator.CurrentPosition)
				{
					return false;
				}
				return true;
			}
		}

		private XPathNodeIterator _iterator;

		private IList<XmlNode> _nodes = new List<XmlNode>();

		private bool _done;

		public override int Count
		{
			get
			{
				if (!_done)
				{
					ReadToEnd();
				}
				return _nodes.Count;
			}
		}

		private bool Done => _done;

		private int CurrentPosition => _nodes.Count;

		public XmlNodeListIterator(XPathNodeIterator iterator)
		{
			_iterator = iterator.Clone();
		}

		public override IEnumerator GetEnumerator()
		{
			return new XmlNodeListEnumerator(this);
		}

		public override XmlNode Item(int index)
		{
			if (index >= _nodes.Count)
			{
				ReadTo(index);
			}
			if (index >= _nodes.Count || index < 0)
			{
				return null;
			}
			return _nodes[index];
		}

		private void ReadToEnd()
		{
			while (_iterator.MoveNext())
			{
				if (!(_iterator.Current is IHasXmlNode hasXmlNode))
				{
					throw new ArgumentException(SR.XmlNodeListFactory_IHasXmlNodeMissing);
				}
				_nodes.Add(hasXmlNode.GetNode());
			}
			_done = true;
		}

		private void ReadTo(int to)
		{
			while (_nodes.Count <= to)
			{
				if (_iterator.MoveNext())
				{
					if (!(_iterator.Current is IHasXmlNode hasXmlNode))
					{
						throw new ArgumentException(SR.XmlNodeListFactory_IHasXmlNodeMissing);
					}
					_nodes.Add(hasXmlNode.GetNode());
					continue;
				}
				_done = true;
				break;
			}
		}
	}

	private XmlNodeListFactory()
	{
	}

	public static XmlNodeList CreateNodeList(XPathNodeIterator iterator)
	{
		return new XmlNodeListIterator(iterator);
	}
}
