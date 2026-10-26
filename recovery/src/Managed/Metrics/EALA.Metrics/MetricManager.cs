using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace EALA.Metrics;

public class MetricManager
{
	private static MetricApplicationData m_AppData;

	private static ArrayList m_Listeners;

	private static bool m_SessionOpen;

	private static readonly object SessionLock = new object();

	private static bool m_Transitioning;

	private static bool m_Dispatching;

	private static Dictionary<string, MetricDescriptor> _DescriptorMap;

	private static bool m_Enabled;

	public static bool Enabled
	{
		get
		{
			return m_Enabled;
		}
		set
		{
			m_Enabled = value;
		}
	}

	public static MetricApplicationData AppData => m_AppData;

	static MetricManager()
	{
		m_Listeners = new ArrayList();
		m_SessionOpen = false;
		_DescriptorMap = new Dictionary<string, MetricDescriptor>();
		m_Enabled = true;
		m_AppData = new MetricApplicationData();
	}

	public static void AddListener(IMetricsListener listener)
	{
		lock (SessionLock)
		{
			CheckListenerMutation();
			if (listener == null) throw new ArgumentNullException(nameof(listener));
			m_Listeners.Add(listener);
		}
	}

	public static bool RemoveListener(IMetricsListener listener)
	{
		lock (SessionLock)
		{
			CheckListenerMutation();
			int index = m_Listeners.IndexOf(listener);
			if (index < 0) return false;
			m_Listeners.RemoveAt(index);
			return true;
		}
	}

	private static void CheckListenerMutation()
	{
		if (m_SessionOpen || m_Transitioning || m_Dispatching)
			throw new ArgumentException("Cannot change listeners while session is open or changing state.");
	}

	public static void OpenSession()
	{
		IMetricsListener[] listeners;
		lock (SessionLock)
		{
			if (m_SessionOpen || m_Transitioning || m_Dispatching)
				throw new ArgumentException("Session already opened or changing state.");
			listeners = (IMetricsListener[])m_Listeners.ToArray(typeof(IMetricsListener));
			m_Transitioning = true;
		}
		int attempted = 0;
		try
		{
			foreach (IMetricsListener listener in listeners)
			{
				attempted++;
				listener.Open();
			}
			lock (SessionLock) { m_SessionOpen = true; m_Enabled = true; }
		}
		catch
		{
			// An Open callback may acquire resources before throwing. Include
			// that listener in rollback, and preserve its original exception.
			for (int i = attempted - 1; i >= 0; i--)
			{
				try { listeners[i].Close(); }
				catch { }
			}
			throw;
		}
		finally { lock (SessionLock) { m_Transitioning = false; } }
	}

	public static void CloseSession()
	{
		IMetricsListener[] listeners;
		lock (SessionLock)
		{
			if (!m_SessionOpen || m_Transitioning || m_Dispatching)
				throw new ArgumentException("Session not opened or changing state.");
			listeners = (IMetricsListener[])m_Listeners.ToArray(typeof(IMetricsListener));
			m_Transitioning = true;
		}
		Exception failure = null;
		try
		{
			foreach (IMetricsListener listener in listeners)
			{
				try { listener.Close(); }
				catch (Exception e) { if (failure == null) failure = e; }
			}
		}
		finally { lock (SessionLock) { m_SessionOpen = false; m_Transitioning = false; } }
		if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
	}

	public static void Submit(string descriptorName, params object[] datalist)
	{
		MetricDescriptor value = null;
		if (_DescriptorMap.TryGetValue(descriptorName.ToLower(), out value))
		{
			SubmitInternal(value, datalist);
		}
	}

	public static void Submit(MetricDescriptor descriptor, params object[] datalist)
	{
		if (_DescriptorMap.ContainsKey(descriptor.Name.ToLower()))
		{
			SubmitInternal(descriptor, datalist);
		}
	}

	private static void SubmitInternal(MetricDescriptor descriptor, params object[] samples)
	{
		IMetricsListener[] listeners;
		lock (SessionLock)
		{
			if (!m_SessionOpen || m_Transitioning || !m_Enabled || samples.Length == 0) return;
			if (m_Dispatching) throw new InvalidOperationException("Metric listener submission cannot reenter itself.");
			listeners = (IMetricsListener[])m_Listeners.ToArray(typeof(IMetricsListener));
			m_Dispatching = true;
		}
		try
		{
			object obj = samples[0];
			samples[0] = descriptor.Type switch
			{
				MetricType.Count => Convert.ToInt32(obj),
				MetricType.Size => Convert.ToInt64(obj),
				MetricType.Duration => (obj.GetType() != typeof(TimeSpan)) ? ((object)Convert.ToDouble(obj)) : ((object)((TimeSpan)obj).TotalSeconds),
				MetricType.Enabled => Convert.ToBoolean(obj),
				MetricType.Success => Convert.ToBoolean(obj),
				MetricType.Name => Convert.ToString(obj),
				MetricType.Ratio => Convert.ToSingle(obj),
				_ => obj,
			};
			foreach (IMetricsListener listener in listeners)
				listener.SubmitMetrics(new Metric(descriptor, samples));
		}
		finally { lock (SessionLock) { m_Dispatching = false; } }
	}

	public static MetricDescriptor GetDescriptor(string name, MetricType type, string description)
	{
		string key = name.ToLower();
		MetricDescriptor value = null;
		if (!_DescriptorMap.TryGetValue(key, out value))
		{
			value = new MetricDescriptor(name, type, description);
			_DescriptorMap.Add(key, value);
		}
		return value;
	}
}
