using System;
using System.Collections.Generic;

namespace BinaryAssetBuilder.Core;

[Serializable]
public class InstanceSet : KeyedCollection2<InstanceHandle, InstanceDeclaration>
{
	protected override InstanceHandle GetKeyForItem(InstanceDeclaration item)
	{
		return item.Handle;
	}

	public bool TryGetValue(InstanceHandle key, out InstanceDeclaration value)
	{
		if (base.Dictionary == null)
		{
			value = null;
			return false;
		}
		return base.Dictionary.TryGetValue(key, out value);
	}

    // Inspect the actual legacy lookup table, not a reconstructed table from
    // live declaration.Handle values: keys may intentionally differ from them.
    internal bool TryGetSymbolTable(out SortedDictionary<InstanceHandle, InstanceDeclaration> table)
    {
        table = null;
        if (GetType() != typeof(InstanceSet) || Items.GetType() != typeof(List2<InstanceDeclaration>)) return false;
        if (Dictionary == null) return Count == 0;
        table = Dictionary as SortedDictionary<InstanceHandle, InstanceDeclaration>;
        return table != null && table.GetType() == typeof(SortedDictionary<InstanceHandle, InstanceDeclaration>) &&
            Object.ReferenceEquals(table.Comparer, Comparer<InstanceHandle>.Default);
    }

	public InstanceDeclaration[] ToArray()
	{
		if (base.Count == 0)
		{
			return null;
		}
		List2<InstanceDeclaration> list = new List2<InstanceDeclaration>();
		foreach (InstanceDeclaration item in base.Items)
		{
			list.Add(item);
		}
		return list.ToArray();
	}

	public InstanceSet()
	{
	}

	public InstanceSet(AssetDeclarationDocument document, IEnumerable<InstanceDeclaration> instances)
	{
		if (instances == null)
		{
			return;
		}
		foreach (InstanceDeclaration instance in instances)
		{
			instance.Initialize(document);
			Add(instance);
		}
	}

	public bool TryAdd(InstanceDeclaration declaration)
	{
		if (Contains(declaration.Handle))
		{
			return false;
		}
		Add(declaration);
		return true;
	}

	public void Add(InstanceSet other)
	{
		foreach (InstanceDeclaration item in other)
		{
			TryAdd(item);
		}
	}

	public void Add(IList<InstanceDeclaration> other)
	{
		foreach (InstanceDeclaration item in other)
		{
			TryAdd(item);
		}
	}
}
