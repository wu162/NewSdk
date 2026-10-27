using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.Core;

[Serializable]
public class InstanceDeclaration : IXmlSerializable
{
    // Cache declarations own the value identities they snapshot. Preserve the
    // legacy graph for unsupported derived handles rather than slicing their
    // custom equality/serialization behavior into a base InstanceHandle.
    private sealed class HandleIdentityComparer : IEqualityComparer<InstanceHandle>
    {
        public bool Equals(InstanceHandle left, InstanceHandle right) { return Object.ReferenceEquals(left, right); }
        public int GetHashCode(InstanceHandle value) { return RuntimeHelpers.GetHashCode(value); }
    }
    private sealed class HandleGraph
    {
        internal InstanceHandle Handle, Inheritance;
        internal List2<InstanceHandle> Strong, Weak;
    }
    private static bool ExactHandles(List2<InstanceHandle> handles)
    {
        if (handles != null && handles.Count != 0)
            foreach (InstanceHandle handle in handles)
                if ((object)handle != null && handle.GetType() != typeof(InstanceHandle)) return false;
        return true;
    }
    private static HandleGraph SnapshotHandles(InstanceHandle own, InstanceHandle inheritance, List2<InstanceHandle> strong, List2<InstanceHandle> weak)
    {
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableDeclarationHandleSnapshots", out disabled);
        bool copy = !disabled && ((object)own == null || own.GetType() == typeof(InstanceHandle)) &&
            ((object)inheritance == null || inheritance.GetType() == typeof(InstanceHandle)) &&
            (strong == null || strong.GetType() == typeof(List2<InstanceHandle>)) &&
            (weak == null || weak.GetType() == typeof(List2<InstanceHandle>)) && ExactHandles(strong) && ExactHandles(weak);
        var seen = copy ? new Dictionary<InstanceHandle, InstanceHandle>(new HandleIdentityComparer()) : null;
        return new HandleGraph { Handle = SnapshotHandle(own, seen), Inheritance = SnapshotHandle(inheritance, seen),
            Strong = SnapshotHandleList(strong, seen), Weak = SnapshotHandleList(weak, seen) };
    }
    private static InstanceHandle SnapshotHandle(InstanceHandle handle, Dictionary<InstanceHandle, InstanceHandle> seen)
    {
        if (seen == null || (object)handle == null) return handle;
        InstanceHandle copy;
        if (!seen.TryGetValue(handle, out copy))
        {
            // Copies stored identifiers/hashes directly; never hash names again.
            copy = new InstanceHandle(handle); seen.Add(handle, copy);
        }
        return copy;
    }
    private static List2<InstanceHandle> SnapshotHandleList(List2<InstanceHandle> handles, Dictionary<InstanceHandle, InstanceHandle> seen)
    {
        // Saved empty lists are null in the legacy cache representation;
        // FromLast independently restores fresh empty Current collections.
        if (handles == null || handles.Count == 0) return null;
        if (seen == null) return new List2<InstanceHandle>(handles);
        var result = new List2<InstanceHandle>();
        foreach (InstanceHandle handle in handles) result.Add(SnapshotHandle(handle, seen));
        return result;
    }

	[Serializable]
	private class LastState
	{
		public InstanceHandle Handle;

		public uint ProcessingHash;

		public List2<InstanceHandle> ReferencedInstances;

		public List2<InstanceHandle> WeakReferencedInstances;

		public List2<string> ReferencedFiles;

		public InstanceHandle InheritFromHandle;

		public uint InheritFromXmlHash;

		public uint PrevalidationXmlHash;

		public bool IsInheritable;

		public bool HasCustomData;

		public LastState(CurrentState current)
		{
            HandleGraph handles = SnapshotHandles(current.Handle, current.InheritFromHandle, current.ReferencedInstances, current.WeakReferencedInstances);
            Handle = handles.Handle;
            ProcessingHash = current.ProcessingHash;
            ReferencedInstances = current.ReferencedInstances.Count > 0 ? handles.Strong : null;
            WeakReferencedInstances = current.WeakReferencedInstances.Count > 0 ? handles.Weak : null;
			ReferencedFiles = ((current.ReferencedFiles.Count > 0) ? new List2<string>(current.ReferencedFiles) : null);
			IsInheritable = current.IsInheritable;
			HasCustomData = current.HasCustomData;
			InheritFromHandle = handles.Inheritance;
			InheritFromXmlHash = current.InheritFromXmlHash;
			PrevalidationXmlHash = current.PrevalidationXmlHash;
		}

		public LastState(XmlReader reader)
		{
			reader.MoveToAttribute("d");
			string[] array = reader.Value.Split(';');
			ProcessingHash = Convert.ToUInt32(array[0]);
			InheritFromXmlHash = Convert.ToUInt32(array[1]);
			PrevalidationXmlHash = Convert.ToUInt32(array[2]);
			IsInheritable = Convert.ToBoolean(array[3]);
			HasCustomData = Convert.ToBoolean(array[4]);
			reader.MoveToElement();
			reader.Read();
			Handle = new InstanceHandle();
			Handle.ReadXml(reader);
			if (reader.Name == "ih")
			{
				InheritFromHandle = new InstanceHandle();
				InheritFromHandle.ReadXml(reader);
			}
			object obj = XmlHelper.ReadStringArrayElement(reader, "rf");
			ReferencedFiles = ((obj == null) ? null : new List2<string>(obj as string[]));
			obj = XmlHelper.ReadCollection(reader, "ri", typeof(InstanceHandle));
			ReferencedInstances = ((obj == null) ? null : new List2<InstanceHandle>(obj as InstanceHandle[]));
			obj = XmlHelper.ReadCollection(reader, "wri", typeof(InstanceHandle));
			WeakReferencedInstances = ((obj == null) ? null : new List2<InstanceHandle>(obj as InstanceHandle[]));
			reader.Read();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("id");
			writer.WriteAttributeString("d", $"{ProcessingHash};{InheritFromXmlHash};{PrevalidationXmlHash};{IsInheritable};{HasCustomData}");
			Handle.WriteXml(writer);
			if (InheritFromHandle != null)
			{
				InheritFromHandle.WriteXml(writer);
			}
			XmlHelper.WriteStringArrayElement(writer, "rf", (ReferencedFiles == null) ? null : ReferencedFiles.ToArray());
			XmlHelper.WriteCollection(writer, "ri", ReferencedInstances);
			XmlHelper.WriteCollection(writer, "wri", WeakReferencedInstances);
			writer.WriteEndElement();
		}
	}

	private class CurrentState
	{
		public AssetDeclarationDocument Document;

		public InstanceHandle Handle;

		public uint ProcessingHash;

		public XmlNode XmlNode;

		public List2<InstanceHandle> ReferencedInstances;

		public List2<InstanceHandle> WeakReferencedInstances;

		public List2<string> ReferencedFiles;

		public bool IsInheritable;

		public bool HasCustomData;

		public string CustomDataPath;

		public InstanceHandle InheritFromHandle;

		public uint InheritFromXmlHash;

		public uint PrevalidationXmlHash;

		public List2<InstanceHandle> ValidatedReferencedInstances;

		public InstanceHandleSet AllDependentInstances;

		public CurrentState(AssetDeclarationDocument document)
		{
			Document = document;
		}

		public void FromLast(LastState last)
		{
            HandleGraph handles = SnapshotHandles(last.Handle, last.InheritFromHandle, last.ReferencedInstances, last.WeakReferencedInstances);
            Handle = handles.Handle;
            ProcessingHash = last.ProcessingHash;
            ReferencedInstances = handles.Strong ?? new List2<InstanceHandle>();
            WeakReferencedInstances = handles.Weak ?? new List2<InstanceHandle>();
			ReferencedFiles = ((last.ReferencedFiles != null) ? new List2<string>(last.ReferencedFiles) : new List2<string>());
			IsInheritable = last.IsInheritable;
			HasCustomData = last.HasCustomData;
			InheritFromHandle = handles.Inheritance;
			InheritFromXmlHash = last.InheritFromXmlHash;
			PrevalidationXmlHash = last.PrevalidationXmlHash;
		}

		public void FromScratch()
		{
			ReferencedInstances = new List2<InstanceHandle>();
			WeakReferencedInstances = new List2<InstanceHandle>();
			ReferencedFiles = new List2<string>();
		}
	}

	private LastState _Last;

	[NonSerialized]
	private CurrentState _Current;

	public List2<InstanceHandle> ValidatedReferencedInstances
	{
		get
		{
			return _Current.ValidatedReferencedInstances;
		}
		set
		{
			_Current.ValidatedReferencedInstances = value;
		}
	}

	public InstanceHandleSet AllDependentInstances
	{
		get
		{
			return _Current.AllDependentInstances;
		}
		set
		{
			_Current.AllDependentInstances = value;
		}
	}

	public List2<InstanceHandle> ReferencedInstances => _Current.ReferencedInstances;

	public List2<InstanceHandle> WeakReferencedInstances => _Current.WeakReferencedInstances;

	public List2<string> ReferencedFiles => _Current.ReferencedFiles;

	public AssetDeclarationDocument Document => _Current.Document;

	public InstanceHandle Handle => _Current.Handle;

    internal InstanceHandle ObservationHandle => _Current == null ? null : _Current.Handle;

    internal AssetDeclarationDocument ObservationDocument => _Current == null ? null : _Current.Document;

	public uint ProcessingHash
	{
		get
		{
			return _Current.ProcessingHash;
		}
		set
		{
			_Current.ProcessingHash = value;
		}
	}

	public XmlNode Node => _Current.XmlNode;

	public XmlNode XmlNode
	{
		get
		{
			return _Current.XmlNode;
		}
		set
		{
			_Current.XmlNode = value;
			if (value == null)
			{
				return;
			}
			XmlAttribute xmlAttribute = value.Attributes["id"];
			if (xmlAttribute != null)
			{
				_Current.Handle = new InstanceHandle(value.Name, xmlAttribute.Value);
				xmlAttribute = value.Attributes["inheritFrom"];
				if (xmlAttribute != null && xmlAttribute.Value != "")
				{
					if (!xmlAttribute.Value.Contains(":"))
					{
						_Current.InheritFromHandle = new InstanceHandle(value.Name, xmlAttribute.Value);
					}
					else
					{
						_Current.InheritFromHandle = new InstanceHandle(xmlAttribute.Value);
					}
				}
				value.Attributes.Remove(xmlAttribute);
				return;
			}
			throw new BinaryAssetBuilderException(ErrorCode.NoIdAttributeForAsset, "Node of type {0} in file://{1} has no id attribute", value.Name, _Current.Document.SourcePath);
		}
	}

	public bool IsInheritable
	{
		get
		{
			return _Current.IsInheritable;
		}
		set
		{
			_Current.IsInheritable = value;
		}
	}

	public bool HasCustomData
	{
		get
		{
			return _Current.HasCustomData;
		}
		set
		{
			_Current.HasCustomData = value;
		}
	}

	public string CustomDataPath
	{
		get
		{
			return _Current.CustomDataPath;
		}
		set
		{
			_Current.CustomDataPath = value;
		}
	}

	public InstanceHandle InheritFromHandle => _Current.InheritFromHandle;

	public uint InheritFromXmlHash
	{
		get
		{
			return _Current.InheritFromXmlHash;
		}
		set
		{
			_Current.InheritFromXmlHash = value;
		}
	}

	public uint PrevalidationXmlHash
	{
		get
		{
			return _Current.PrevalidationXmlHash;
		}
		set
		{
			_Current.PrevalidationXmlHash = value;
		}
	}

	public XmlSchema GetSchema()
	{
		return null;
	}

	public void WriteXml(XmlWriter writer)
	{
		if (_Last == null)
		{
			MakeCacheable();
		}
		_Last.WriteXml(writer);
	}

	public void ReadXml(XmlReader reader)
	{
		_Last = new LastState(reader);
	}

	public InstanceDeclaration()
	{
	}

	public InstanceDeclaration(AssetDeclarationDocument document)
	{
		Initialize(document);
	}

	public void Initialize(AssetDeclarationDocument document)
	{
		if (_Current == null)
		{
			_Current = new CurrentState(document);
			if (_Last != null)
			{
				_Current.FromLast(_Last);
			}
			else
			{
				_Current.FromScratch();
			}
		}
	}

	public void InitializePrecompiled(Asset asset)
	{
		_Current = new CurrentState(null);
		_Current.FromScratch();
		_Current.Handle = new InstanceHandle(asset.TypeName, asset.InstanceName);
		_Current.Handle.TypeHash = asset.TypeHash;
		_Current.Handle.InstanceHash = asset.InstanceHash;
		_Current.IsInheritable = false;
		_Current.HasCustomData = false;
	}

	public void MakeComplete()
	{
		_Last = null;
	}

	public void MakeCacheable()
	{
		if (_Last == null)
		{
			_Last = new LastState(_Current);
		}
	}

	public override bool Equals(object obj)
	{
		return ((InstanceDeclaration)obj).Handle == Handle;
	}

	public override int GetHashCode()
	{
		return _Current.Handle.GetHashCode();
	}

	public override string ToString()
	{
		return _Current.Handle.ToString();
	}

	public void CacheFromInstance(InstanceDeclaration current)
	{
		_Last = new LastState(current._Current);
	}
}
