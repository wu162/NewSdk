using System;
using System.Diagnostics;
using System.IO;

namespace Mvp.Xml.Common.Serialization;

public class PerfCounterManager : IDisposable
{
	internal const string CATEGORY = "Mvp.Xml.XmlSerializerCache";

	internal const string CATEGORY_DESCRIPTION = "Performance counters to instrument the classes from the Mvp.Xml project.";

	internal const string SERIALIZER_HITS_NAME = "Cache Hits";

	internal const string SERIALIZER_HITS_DESCRIPTION = "Number of times instances were retrieved from the serializer cache.";

	internal const string CACHED_INSTANCES_NAME = "Cached Instances";

	internal const string CACHED_INSTANCES_DESCRIPTION = "Number of XmlSerializer instances in the cache.";

	private PerformanceCounter CacheHitCounter;

	private PerformanceCounter InstanceCounter;

	private readonly int HitCounterInstanceCount;

	private readonly int InstanceCounterInstanceCount;

	private static int GlobalHitCounterInstanceCount;

	private static int GlobalInstanceCounterInstanceCount;

	public PerfCounterManager()
	{
		CheckAndCreateCategory();
		CreateInstances();
		HitCounterInstanceCount = GlobalHitCounterInstanceCount++;
		InstanceCounterInstanceCount = GlobalInstanceCounterInstanceCount++;
	}

	private void CheckAndCreateCategory()
	{
		if (!PerformanceCounterCategory.Exists("Mvp.Xml.XmlSerializerCache"))
		{
			CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();
			counterCreationDataCollection.Add(GetCacheHitCounterData());
			counterCreationDataCollection.Add(GetCachedInstancesCounterData());
			PerformanceCounterCategory.Create("Mvp.Xml.XmlSerializerCache", "Performance counters to instrument the classes from the Mvp.Xml project.", PerformanceCounterCategoryType.MultiInstance, counterCreationDataCollection);
		}
	}

	private CounterCreationData GetCacheHitCounterData()
	{
		return new CounterCreationData("Cache Hits", "Number of times instances were retrieved from the serializer cache.", PerformanceCounterType.NumberOfItems64);
	}

	private CounterCreationData GetCachedInstancesCounterData()
	{
		return new CounterCreationData("Cached Instances", "Number of XmlSerializer instances in the cache.", PerformanceCounterType.NumberOfItems32);
	}

	private void CreateInstances()
	{
		CacheHitCounter = new PerformanceCounter("Mvp.Xml.XmlSerializerCache", "Cache Hits", GetCounterInstanceName(HitCounterInstanceCount), readOnly: false);
		InstanceCounter = new PerformanceCounter("Mvp.Xml.XmlSerializerCache", "Cached Instances", GetCounterInstanceName(InstanceCounterInstanceCount), readOnly: false);
	}

	private string GetCounterInstanceName(int index)
	{
		string text = Environment.CommandLine.Split(' ')[0];
		char[] invalidPathChars = Path.GetInvalidPathChars();
		foreach (char oldChar in invalidPathChars)
		{
			text = text.Replace(oldChar, '%');
		}
		text = (text = Path.GetFileNameWithoutExtension(text));
		return $"{text}-{AppDomain.CurrentDomain.FriendlyName}#{index}";
	}

	public void IncrementHitCount()
	{
		CacheHitCounter.Increment();
	}

	public void IncrementInstanceCount()
	{
		InstanceCounter.Increment();
	}

	~PerfCounterManager()
	{
		Dispose(isDisposing: false);
	}

	private void Dispose(bool isDisposing)
	{
		try
		{
			CacheHitCounter.RemoveInstance();
		}
		catch
		{
		}
		try
		{
			InstanceCounter.RemoveInstance();
		}
		catch
		{
		}
	}

	public void Dispose()
	{
		Dispose(isDisposing: true);
		GC.SuppressFinalize(this);
	}
}
