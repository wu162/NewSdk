using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder;

[XmlType("plugin")]
public class PluginDescriptor
{
	private IAssetBuilderPluginBase _Plugin;
#if NEWSdk_MONOLITHIC
    private Func<IAssetBuilderPluginBase> _Factory;

    // Factory injection is explicit: old targetType strings cannot load old
    // compiler assemblies into the production process.
    public static PluginDescriptor CreateBuiltin(string assetTypes, bool useBuildCache, Func<IAssetBuilderPluginBase> factory)
    {
        if(factory == null) throw new ArgumentNullException(nameof(factory));
        return new PluginDescriptor { Enabled=true, AssetTypes=assetTypes, UseBuildCache=useBuildCache, _Factory=factory };
    }
#endif

	private List<uint> _HandledTypes = new List<uint>();

	private string _AssetTypes;

	private bool _Enabled;

	private string _TargetType;

	private bool _UseBuildCache;

	private string _ConfigSection;

	[XmlIgnore]
	public IAssetBuilderPluginBase Plugin => _Plugin;

	[XmlIgnore]
	public List<uint> HandledTypes => _HandledTypes;

	[XmlAttribute("assetTypes")]
	public string AssetTypes
	{
		get
		{
			return _AssetTypes;
		}
		set
		{
			_AssetTypes = value;
		}
	}

	[XmlAttribute("enabled")]
	public bool Enabled
	{
		get
		{
			return _Enabled;
		}
		set
		{
			_Enabled = value;
		}
	}

	[XmlAttribute("targetType")]
	public string QualifiedName
	{
		get
		{
			return _TargetType;
		}
		set
		{
			_TargetType = value;
		}
	}

	[XmlAttribute("useBuildCache")]
	public bool UseBuildCache
	{
		get
		{
			return _UseBuildCache;
		}
		set
		{
			_UseBuildCache = value;
		}
	}

	[XmlAttribute("configSection")]
	public string ConfigSection
	{
		get
		{
			return _ConfigSection;
		}
		set
		{
			_ConfigSection = value;
		}
	}

	public void Initialize(TargetPlatform platform)
	{
		if (!Enabled)
		{
			return;
		}
#if NEWSdk_MONOLITHIC
        if (_Factory == null) throw new ApplicationException("A built-in factory is required for plugin '" + _TargetType + "'. Legacy targetType loading is unsupported.");
        _Plugin = _Factory();
#else
		Type type = Type.GetType(_TargetType);
		if (type == null)
		{
			throw new ApplicationException($"'{_TargetType}' not found");
		}
		_Plugin = Activator.CreateInstance(type) as IAssetBuilderPluginBase;
#endif
		if (_Plugin == null)
		{
			throw new ApplicationException($"'{_TargetType}' does not implement IAssetBuilderPluginBase");
		}
		if (!string.IsNullOrEmpty(_AssetTypes) && !_AssetTypes.Equals("#all"))
		{
			string[] array = _AssetTypes.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text in array2)
			{
				_HandledTypes.Add(InstanceHandle.GetTypeId(text.Trim()));
			}
		}
		object configObject = null;
		_Plugin.Initialize(configObject, platform);
	}

	public void ReInitialize(TargetPlatform platform)
	{
		if (Enabled)
		{
			if (_Plugin == null)
			{
				Initialize(platform);
				return;
			}
			object configObject = null;
			_Plugin.ReInitialize(configObject, platform);
		}
	}
}
