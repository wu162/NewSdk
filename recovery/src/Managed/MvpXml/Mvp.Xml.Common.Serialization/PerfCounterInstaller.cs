using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace Mvp.Xml.Common.Serialization;

[RunInstaller(true)]
public class PerfCounterInstaller : Installer
{
	private Container components;

	public PerfCounterInstaller()
	{
		InitializeComponent();
		try
		{
			PerformanceCounterInstaller performanceCounterInstaller = new PerformanceCounterInstaller
			{
				CategoryName = "Mvp.Xml.XmlSerializerCache"
			};
			CounterCreationData value = new CounterCreationData
			{
				CounterName = "Cache Hits",
				CounterHelp = "Number of times instances were retrieved from the serializer cache.",
				CounterType = PerformanceCounterType.NumberOfItems64
			};
			CounterCreationData value2 = new CounterCreationData
			{
				CounterName = "Cached Instances",
				CounterHelp = "Number of XmlSerializer instances in the cache.",
				CounterType = PerformanceCounterType.NumberOfItems64
			};
			performanceCounterInstaller.Counters.Add(value);
			performanceCounterInstaller.Counters.Add(value2);
			base.Installers.Add(performanceCounterInstaller);
		}
		catch (Exception)
		{
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		components = new Container();
	}
}
