using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mvp.Xml.Common.Serialization;

public class XmlSerializerCache : IDisposable
{
	private Dictionary<string, XmlSerializer> Serializers;

	private object SyncRoot;

	private PerfCounterManager stats;

	public event SerializerCacheDelegate NewSerializer;

	public event SerializerCacheDelegate CacheHit;

	public XmlSerializerCache()
	{
		SyncRoot = new object();
		stats = new PerfCounterManager();
		Serializers = new Dictionary<string, XmlSerializer>();
	}

	public XmlSerializer GetSerializer(Type type, string defaultNamespace)
	{
		return GetSerializer(type, null, new Type[0], null, defaultNamespace);
	}

	public XmlSerializer GetSerializer(Type type, XmlRootAttribute root)
	{
		return GetSerializer(type, null, new Type[0], root, null);
	}

	public XmlSerializer GetSerializer(Type type, XmlAttributeOverrides overrides)
	{
		return GetSerializer(type, overrides, new Type[0], null, null);
	}

	public XmlSerializer GetSerializer(Type type, Type[] types)
	{
		return GetSerializer(type, null, types, null, null);
	}

	public XmlSerializer GetSerializer(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, string defaultNamespace)
	{
		string key = CacheKeyFactory.MakeKey(type, overrides, types, root, defaultNamespace);
		XmlSerializer xmlSerializer = null;
		if (!Serializers.ContainsKey(key))
		{
			lock (SyncRoot)
			{
				if (!Serializers.ContainsKey(key))
				{
					stats.IncrementInstanceCount();
					xmlSerializer = new XmlSerializer(type, overrides, types, root, defaultNamespace);
					Serializers.Add(key, xmlSerializer);
					if (this.NewSerializer != null)
					{
						this.NewSerializer(type, overrides, types, root, defaultNamespace);
					}
				}
			}
		}
		else
		{
			xmlSerializer = Serializers[key];
			stats.IncrementHitCount();
			if (this.CacheHit != null)
			{
				this.CacheHit(type, overrides, types, root, defaultNamespace);
			}
		}
		return xmlSerializer;
	}

	~XmlSerializerCache()
	{
		Dispose(isDisposing: false);
	}

	private void Dispose(bool isDisposing)
	{
		if (isDisposing)
		{
			stats.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(isDisposing: true);
		GC.SuppressFinalize(this);
	}
}
