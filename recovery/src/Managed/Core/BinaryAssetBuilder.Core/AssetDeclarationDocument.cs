using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using BinaryAssetBuilder.Utility;
using EALA.Hash;

namespace BinaryAssetBuilder.Core;

[Serializable]
public class AssetDeclarationDocument : IXmlSerializable
{
	private class XmlNodeWithMetaData
	{
		public XmlNode Node;

		public int LineNumber;

		public XmlNodeWithMetaData(XmlNode node, int line)
		{
			Node = node;
			LineNumber = line;
		}

		public bool Equals(XmlNodeWithMetaData other)
		{
			return Node.Equals(other.Node);
		}
	}

	private class DependencyComparer : IComparer<InstanceDeclaration>
	{
        private readonly AssetDeclarationDocument _Owner;
        public DependencyComparer() { }
        public DependencyComparer(AssetDeclarationDocument owner) { _Owner = owner; }
        private InstanceHandleSet Dependencies(InstanceDeclaration instance)
        {
            return _Owner == null ? instance.AllDependentInstances : _Owner.GetDependentInstances(instance);
        }
		public int Compare(InstanceDeclaration left, InstanceDeclaration right)
		{
			if (left == null || right == null)
			{
				throw new ArgumentNullException();
			}
            InstanceHandleSet rightDependencies = Dependencies(right), leftDependencies = Dependencies(left);
            if (rightDependencies.Count != leftDependencies.Count)
            {
                return leftDependencies.Count.CompareTo(rightDependencies.Count);
            }
            bool flag = rightDependencies != null && rightDependencies.Contains(left.Handle);
            bool flag2 = leftDependencies != null && leftDependencies.Contains(right.Handle);
			if (flag)
			{
				if (flag2)
				{
					throw new BinaryAssetBuilderException(ErrorCode.CircularDependency, $"Illegal circular dependency found between {left} and {right}.  This should have been caught during parsing.");
				}
				return -1;
			}
			if (flag2)
			{
				return 1;
			}
			return string.Compare(left.Handle.TypeName, right.Handle.TypeName);
		}
	}

	private class TypeDepCompare : IComparer<InstanceDeclaration>
	{
		private IDictionary<uint, int> _TypeDependencies;

		public int Compare(InstanceDeclaration left, InstanceDeclaration right)
		{
			int num = _TypeDependencies[left.Handle.TypeId].CompareTo(_TypeDependencies[right.Handle.TypeId]);
			if (num == 0)
			{
				int num2 = string.Compare(left.Handle.TypeName, right.Handle.TypeName);
				if (num2 == 0)
				{
					return string.Compare(left.Handle.InstanceName, right.Handle.InstanceName);
				}
				return num2;
			}
			return num;
		}

		public TypeDepCompare(IDictionary<uint, int> typeDependencies)
		{
			_TypeDependencies = typeDependencies;
		}
	}

    private static bool DocumentSnapshotsEnabled()
    {
        bool disabled;
        AppContext.TryGetSwitch("NewSdk.Compiler.DisableDocumentSnapshots", out disabled);
        return !disabled;
    }

    private static bool CanSnapshotDeclarations(List2<InstanceDeclaration> instances)
    {
        if (instances == null) return true;
        if (instances.GetType() != typeof(List2<InstanceDeclaration>)) return false;
        foreach (InstanceDeclaration instance in instances)
            if (instance == null || instance.GetType() != typeof(InstanceDeclaration)) return false;
        return true;
    }

    private static List2<InstanceDeclaration> SnapshotDeclarations(List2<InstanceDeclaration> instances, bool fromCurrent = false)
    {
        if (instances == null) return null;
        var copies = new List2<InstanceDeclaration>();
        foreach (InstanceDeclaration instance in instances) copies.Add(instance.SnapshotCachedDeclaration(fromCurrent));
        return copies;
    }

	[Serializable]
	private class LastState
	{
        // Runtime provenance: derived declaration/list behavior remains legacy
        // across a restore/recapture, rather than silently slicing custom types.
        [NonSerialized] internal bool LegacyDeclarations;
        [NonSerialized] internal bool LegacyMetadata;
        // Positive runtime provenance only: copied exact values or XML-created
        // records. Unknown BinaryFormatter state never authorizes sharing.
        [NonSerialized] internal bool OwnedMetadata;
        [NonSerialized] internal bool SharedMetadata;

		public uint DocumentHash;

		public uint DependentFileHash;

		public List2<string> DependentFiles;

		public uint IncludePathHash;

		public List2<InclusionItem> InclusionItems;

		public List2<OutputAsset> OutputAssets;

		public List2<Definition> SelfDefines;

		public List2<InstanceDeclaration> SelfInstances;

		public List2<DefinitionPair> UsedDefines;

		public List2<string> StreamHints;

		public LastState(CurrentState current, bool fromCurrent = false)
		{
			DocumentHash = current.DocumentHash;
			IncludePathHash = current.IncludePathHash;
			DependentFileHash = current.DependentFileHash;
			DependentFiles = ((current.DependentFiles.Count > 0) ? new List2<string>(current.DependentFiles) : null);
			SelfInstances = ((current.SelfInstances.Count > 0) ? new List2<InstanceDeclaration>(current.SelfInstances) : null);
            LegacyDeclarations = !DocumentSnapshotsEnabled() || current.LegacyDeclarations ||
                current.SelfInstances.GetType() != typeof(InstanceSet) || !CanSnapshotDeclarations(SelfInstances);
            if (!LegacyDeclarations) SelfInstances = SnapshotDeclarations(SelfInstances, fromCurrent);
			List2<DefinitionPair> list = new List2<DefinitionPair>();
			foreach (KeyValuePair<string, string> usedDefine in current.UsedDefines)
			{
				list.Add(new DefinitionPair
				{
					Name = usedDefine.Key,
					EvaluatedValue = usedDefine.Value
				});
			}
            UsedDefines = list.Count > 0 ? list : null;
			StreamHints = ((current.StreamHints.Count > 0) ? new List2<string>(current.StreamHints) : null);
            CompilerDocumentMetadataSnapshot metadata = CompilerDocumentMetadataSnapshot.Capture(
                current.InclusionItems.Count > 0 ? current.InclusionItems : null,
                current.OutputAssets.Count > 0 ? current.OutputAssets : null,
                current.SelfDefines.Count > 0 ? current.SelfDefines : null, UsedDefines, current.LegacyMetadata);
            LegacyMetadata = metadata.Legacy;
            OwnedMetadata = !metadata.Legacy;
            InclusionItems = metadata.Inclusions; OutputAssets = metadata.Outputs;
            SelfDefines = metadata.Definitions; UsedDefines = metadata.Used;
		}

        internal LastState(LastState last)
        {
            DocumentHash = last.DocumentHash;
            DependentFileHash = last.DependentFileHash;
            IncludePathHash = last.IncludePathHash;
            DependentFiles = last.DependentFiles == null ? null : new List2<string>(last.DependentFiles);
            StreamHints = last.StreamHints == null ? null : new List2<string>(last.StreamHints);
            LegacyDeclarations = last.LegacyDeclarations || !CanSnapshotDeclarations(last.SelfInstances);
            SelfInstances = LegacyDeclarations ? last.SelfInstances : SnapshotDeclarations(last.SelfInstances);
            bool deepCopy;
            AppContext.TryGetSwitch("NewSdk.Compiler.DisableSharedDocumentMetadata", out deepCopy);
            if (!deepCopy && last.OwnedMetadata && !last.LegacyMetadata)
            {
                // Only private saved records are shared. FromLast still clones
                // every mutable record before binding it to a live document.
                last.SharedMetadata = SharedMetadata = true;
                OwnedMetadata = true;
                InclusionItems = last.InclusionItems; OutputAssets = last.OutputAssets;
                SelfDefines = last.SelfDefines; UsedDefines = last.UsedDefines;
            }
            else
            {
                CompilerDocumentMetadataSnapshot metadata = CompilerDocumentMetadataSnapshot.Capture(
                    last.InclusionItems, last.OutputAssets, last.SelfDefines, last.UsedDefines, last.LegacyMetadata);
                LegacyMetadata = metadata.Legacy;
                OwnedMetadata = !metadata.Legacy;
                InclusionItems = metadata.Inclusions; OutputAssets = metadata.Outputs;
                SelfDefines = metadata.Definitions; UsedDefines = metadata.Used;
            }
        }

        internal LastState DetachMetadata()
        {
            // Leave declaration and non-metadata lifecycle behavior untouched.
            // This owner is detached before a dynamic legacy restore can expose
            // its formerly private saved records to public Current collections.
            var result = (LastState)MemberwiseClone();
            CompilerDocumentMetadataSnapshot metadata = CompilerDocumentMetadataSnapshot.Capture(
                InclusionItems, OutputAssets, SelfDefines, UsedDefines, false, true);
            result.InclusionItems = metadata.Inclusions; result.OutputAssets = metadata.Outputs;
            result.SelfDefines = metadata.Definitions; result.UsedDefines = metadata.Used;
            result.LegacyMetadata = metadata.Legacy;
            result.SharedMetadata = false;
            result.OwnedMetadata = !metadata.Legacy;
            return result;
        }

		public LastState(XmlReader reader)
		{
			reader.MoveToAttribute("d");
			string[] array = reader.Value.Split(';');
			DocumentHash = Convert.ToUInt32(array[0]);
			DependentFileHash = Convert.ToUInt32(array[1]);
			IncludePathHash = Convert.ToUInt32(array[2]);
			reader.Read();
			if (reader.IsStartElement())
			{
				ReadOldStrings(reader);
				object obj = XmlHelper.ReadStringArrayElement(reader, "sf");
				DependentFiles = ((obj == null) ? null : new List2<string>(obj as string[]));
				obj = XmlHelper.ReadStringArrayElement(reader, "dsf");
				StreamHints = ((obj == null) ? null : new List2<string>(obj as string[]));
				obj = XmlHelper.ReadCollection(reader, "iic", typeof(InclusionItem));
				InclusionItems = ((obj == null) ? null : new List2<InclusionItem>(obj as InclusionItem[]));
				obj = XmlHelper.ReadCollection(reader, "oac", typeof(OutputAsset));
				OutputAssets = ((obj == null) ? null : new List2<OutputAsset>(obj as OutputAsset[]));
				obj = XmlHelper.ReadCollection(reader, "sdc", typeof(Definition));
				SelfDefines = ((obj == null) ? null : new List2<Definition>(obj as Definition[]));
				obj = XmlHelper.ReadCollection(reader, "sic", typeof(InstanceDeclaration));
				SelfInstances = ((obj == null) ? null : new List2<InstanceDeclaration>(obj as InstanceDeclaration[]));
				obj = XmlHelper.ReadCollection(reader, "udc", typeof(DefinitionPair));
				UsedDefines = ((obj == null) ? null : new List2<DefinitionPair>(obj as DefinitionPair[]));
				reader.Read();
			}
            OwnedMetadata = true;
		}

		private void ReadOldStrings(XmlReader reader)
		{
			string[] array = XmlHelper.ReadStringArrayElement(reader, "s");
			string[] array2 = XmlHelper.ReadStringArrayElement(reader, "p");
			if (array != null)
			{
				string[] array3 = array;
				foreach (string str in array3)
				{
					HashProvider.RecordHash("STRINGHASH", str);
				}
			}
			if (array2 != null)
			{
				string[] array4 = array2;
				foreach (string str2 in array4)
				{
					HashProvider.RecordHash("POID", str2);
				}
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("ad");
			writer.WriteAttributeString("d", $"{DocumentHash};{DependentFileHash};{IncludePathHash}");
			XmlHelper.WriteStringArrayElement(writer, "sf", (DependentFiles == null) ? null : DependentFiles.ToArray());
			XmlHelper.WriteStringArrayElement(writer, "dsf", (StreamHints == null) ? null : StreamHints.ToArray());
			XmlHelper.WriteCollection(writer, "iic", InclusionItems);
			XmlHelper.WriteCollection(writer, "oac", OutputAssets);
			XmlHelper.WriteCollection(writer, "sdc", SelfDefines);
			XmlHelper.WriteCollection(writer, "sic", SelfInstances);
			XmlHelper.WriteCollection(writer, "udc", UsedDefines);
			writer.WriteEndElement();
		}
	}

	private class CurrentState
	{
        internal bool LegacyDeclarations;
        internal bool LegacyMetadata;
		public uint DocumentHash;

		public string SourcePath;

		public string SourceDirectory;

		public string SourcePathFromRoot;

		public string LogicalSourcePath;

		public uint DependentFileHash;

		public uint IncludePathHash;

		public uint OutputChecksum;

		public bool IsLoaded;

		public bool Processing;

		public string ChangeReason;

		public bool Changed;

		public DocumentState State;

		public List2<Definition> SelfDefines;

		public InstanceSet SelfInstances;

		public List2<string> DependentFiles;

		public List2<InclusionItem> InclusionItems;

		public IDictionary<string, string> UsedDefines;

        // Values visible before local overrides are installed in this pass.
        // Only previous external reads are needed when validating a restored document.
        public IDictionary<string, string> CachedDefineInputs;
        public long DefineScopeEpoch;
        public string DefineReadStage;
        public string DefineReadSite;


		public bool VerificationErrors;

		public FileHashItem HashItem;

		public List2<InstanceDeclaration> OutputInstances = new List2<InstanceDeclaration>();

		public DefinitionSet AllDefines = new DefinitionSet();

		public InstanceSet Instances = new InstanceSet();

		public InstanceSet AllInstances = new InstanceSet();

		public InstanceSet TentativeInstances = new InstanceSet();

		public InstanceSet ReferenceInstances = new InstanceSet();

		public StringDictionary Tags = new StringDictionary();

		public XmlDocument XmlDocument;

		public XmlNamespaceManager NamespaceManager;

		public DocumentProcessor DocumentProcessor;

		public List2<OutputAsset> OutputAssets = new List2<OutputAsset>();

		public List2<string> StreamHints;

		public LinkedList<XmlNodeWithMetaData> NodeSourceInfoSet;

		public IDictionary<InstanceHandle, InstanceDeclaration> OutputInstanceSet;
        public bool UseStreamBindingResults;
        public Dictionary<InstanceDeclaration, StreamBindingState> StreamBindings;
        public StreamBindingResult BindingSnapshot;

		public XmlReader XmlReader;

		public string ConfigurationName;

		public List2<OutputAsset> LastOutputAssets;

		public uint LastDocumentHash;

		public uint LastDependentFileHash;

		public uint LastIncludePathHash;

		public CurrentState()
		{
		}

		public CurrentState(DocumentProcessor documentProcessor, FileHashItem hashItem, string logicalSourcePath, string configuration)
		{
			DocumentProcessor = documentProcessor;
			HashItem = hashItem;
			SourcePath = hashItem.Path;
			LogicalSourcePath = logicalSourcePath;
			SourceDirectory = Path.GetDirectoryName(SourcePath);
			DocumentHash = HashItem.Hash;
			ConfigurationName = configuration;
			string dataRoot = FileNameResolver.GetDataRoot(SourcePath);
			if (!string.IsNullOrEmpty(dataRoot))
			{
				string text = FileNameResolver.GetSafeRelativePath(dataRoot, SourcePath);
				string directoryName = Path.GetDirectoryName(text);
				SourcePathFromRoot = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(text));
				LogicalSourcePath = "DATA:" + text.Replace('\\', '/');
			}
			else
			{
				SourcePathFromRoot = Path.GetFileNameWithoutExtension(SourcePath);
			}
		}

		public void FromLast(AssetDeclarationDocument document, LastState last)
		{
            CompilerDocumentMetadataSnapshot metadata = CompilerDocumentMetadataSnapshot.Capture(
                last.InclusionItems, last.OutputAssets, last.SelfDefines, last.UsedDefines, last.LegacyMetadata);
            LegacyMetadata = metadata.Legacy;
            InclusionItems = metadata.Inclusions ?? new List2<InclusionItem>();
			DependentFiles = ((last.DependentFiles != null) ? new List2<string>(last.DependentFiles) : new List2<string>());
			StreamHints = ((last.StreamHints != null) ? new List2<string>(last.StreamHints) : new List2<string>());
			UsedDefines = new SortedDictionary<string, string>();
			if (metadata.Used != null)
			{
				foreach (DefinitionPair usedDefine in metadata.Used)
				{
					UsedDefines[usedDefine.Name] = usedDefine.EvaluatedValue;
				}
			}
            SelfDefines = metadata.Definitions ?? new List2<Definition>();
			foreach (Definition selfDefine in SelfDefines)
			{
				selfDefine.Document = document;
			}
            LegacyDeclarations = last.LegacyDeclarations || !CanSnapshotDeclarations(last.SelfInstances);
            List2<InstanceDeclaration> declarations = DocumentSnapshotsEnabled() && !LegacyDeclarations
                ? SnapshotDeclarations(last.SelfInstances) : last.SelfInstances;
            SelfInstances = declarations != null ? new InstanceSet(document, declarations) : new InstanceSet();
			Changed = last.DocumentHash != DocumentHash;
            LastOutputAssets = metadata.Legacy ? last.OutputAssets : metadata.Outputs;
			LastDocumentHash = last.DocumentHash;
			LastIncludePathHash = last.IncludePathHash;
			LastDependentFileHash = last.DependentFileHash;
			State = DocumentState.Shallow;
		}

		public void FromScratch()
		{
            LegacyDeclarations = false; LegacyMetadata = false;
			InclusionItems = new List2<InclusionItem>();
			DependentFiles = new List2<string>();
			UsedDefines = new SortedDictionary<string, string>();
			SelfDefines = new List2<Definition>();
			SelfInstances = new InstanceSet();
			StreamHints = new List2<string>();
			State = DocumentState.None;
		}
	}

	private enum LoadType
	{
		InplaceLoad,
		FromScratch
	}

	private static Tracer _Tracer = Tracer.GetTracer("DocumentProcessor", "Provides XML processing functionality");

	private LastState _Last;

	[NonSerialized]
	private CurrentState _Current;

	[NonSerialized]
	private bool _processing;

    [NonSerialized] private CompilerSymbolLookup _SymbolLookup;
    internal object DefineLookupOwner => _Current;
    [NonSerialized] private CompilerDefineLookup _DefineLookup;
    [NonSerialized] private CompilerDefineLookup.Work _DefineLookupWork;
    public object GetDefineLookupMetrics() => _DefineLookupWork == null ? null : _DefineLookupWork.Snapshot();
    public bool TryGetDefinition(string name, out Definition definition)
    {
        return _DefineLookup == null ? AllDefines.TryGetValue(name, out definition) : _DefineLookup.TryGetValue(name, out definition);
    }

    [NonSerialized] private CompilerSymbolLookup.Work _SymbolLookupWork;
    public object GetSymbolLookupMetrics() => _SymbolLookupWork == null ? null : _SymbolLookupWork.Snapshot();

	[NonSerialized]
	public bool ReloadedForInheritance;

	public FileHashItem HashItem => _Current.HashItem;

	public List2<OutputAsset> LastOutputAssets => _Current.LastOutputAssets;

	public string SourcePathFromRoot => _Current.SourcePathFromRoot;

	public string SourcePath => _Current.SourcePath;

    internal string ObservationSourcePath => _Current == null ? null : _Current.SourcePath;

    public DocumentProcessor ExpressionProgramOwner => _Current == null ? null : _Current.DocumentProcessor;

	public InstanceSet SelfInstances => _Current.SelfInstances;

	public List2<Definition> SelfDefines => _Current.SelfDefines;

	public List2<InclusionItem> InclusionItems => _Current.InclusionItems;

	public string LogicalSourcePath => _Current.LogicalSourcePath;

	public uint OutputChecksum => _Current.OutputChecksum;

	public bool IsLoaded => _Current.IsLoaded;

	public DefinitionSet AllDefines => _Current.AllDefines;

	public IDictionary<string, string> UsedDefines => _Current.UsedDefines;

	public List2<InstanceDeclaration> OutputInstances => _Current.OutputInstances;

	public InstanceSet Instances => _Current.Instances;

	public InstanceSet AllInstances => _Current.AllInstances;

	public InstanceSet TentativeInstances => _Current.TentativeInstances;

	public InstanceSet ReferenceInstances => _Current.ReferenceInstances;

	public DocumentState State
	{
		get
		{
			if (_Current != null)
			{
				return _Current.State;
			}
			return DocumentState.None;
		}
		set
		{
			_Current.State = value;
		}
	}

	public StringDictionary Tags => _Current.Tags;

	public XmlDocument XmlDocument => _Current.XmlDocument;

	public XmlNamespaceManager NamespaceManager => _Current.NamespaceManager;

	public string SourceDirectory => _Current.SourceDirectory;

	public bool VerificationErrors => _Current.VerificationErrors;

	public List2<string> StreamHints => _Last.StreamHints;

	public bool Processing
	{
		get
		{
			return _processing;
		}
		set
		{
			_processing = value;
		}
	}

	public void WriteXml(XmlWriter writer)
	{
		if (_Last != null)
		{
			_Last.WriteXml(writer);
		}
	}

	public void ReadXml(XmlReader reader)
	{
		_Last = new LastState(reader);
	}

	public XmlSchema GetSchema()
	{
		return null;
	}

	public void SetChanged(string reason)
	{
		_Current.ChangeReason = ((reason == null && _Current.ChangeReason == null) ? string.Empty : reason);
		_Current.Changed = true;
	}

	public void Reset()
	{
		_Current.XmlDocument = null;
		if (ReloadedForInheritance)
		{
			State = DocumentState.Complete;
			ReloadedForInheritance = false;
		}
	}

	public AssetDeclarationDocument()
	{
	}

	public AssetDeclarationDocument(DocumentProcessor documentProcessor, FileHashItem hashItem, string logicalPath, string configuration)
	{
		Open(documentProcessor, hashItem, logicalPath, configuration);
	}

	public void Reinitialize(OutputManager outputManager)
	{
		if (_Current.HashItem == null)
		{
			throw new BinaryAssetBuilderException(ErrorCode.DependencyCacheFailure, "File hashing information for cached document missing: {0}", _Current.SourcePath);
		}
		string text = null;
		if (_Current.LastDocumentHash != _Current.DocumentHash)
		{
			text = "content changed";
		}
		else
		{
			UpdateDocumentHashes();
			if (_Current.LastDependentFileHash != _Current.DependentFileHash)
			{
				text = "dependent file changed";
			}
			else
			{
				foreach (InstanceDeclaration selfInstance in _Current.SelfInstances)
				{
					BinaryAsset binaryAsset = outputManager.GetBinaryAsset(selfInstance, isOutputAsset: false);
					if (binaryAsset != null && binaryAsset.GetLocation(AssetLocation.All, AssetLocationOption.None) == AssetLocation.None)
					{
						text = "previous output missing";
						break;
					}
					ExtendedTypeInformation extendedTypeInformation = _Current.DocumentProcessor.Plugins.GetExtendedTypeInformation(selfInstance.Handle.TypeId);
					if (selfInstance.Handle.TypeHash != extendedTypeInformation.TypeHash)
					{
						text = "type hash changed";
						break;
					}
					if (selfInstance.ProcessingHash != extendedTypeInformation.ProcessingHash)
					{
						text = "plugin output changed";
						break;
					}
				}
			}
		}
		if (text != null)
		{
			Load(text);
		}
		else if (_Current.LastIncludePathHash != _Current.IncludePathHash)
		{
			foreach (InclusionItem inclusionItem in _Current.InclusionItems)
			{
				inclusionItem.PhysicalPath = FileNameResolver.ResolvePath(SourceDirectory, inclusionItem.LogicalPath).ToLower();
			}
		}
		_Current.StreamHints.Clear();
	}

	public void ReloadIfRequired(InstanceHandleSet requiredOverrideSources)
	{
		string text = null;
		InstanceHandleSet instanceHandleSet = new InstanceHandleSet();
		foreach (InstanceHandle requiredOverrideSource in requiredOverrideSources)
		{
			if (_Current.SelfInstances.Contains(requiredOverrideSource))
			{
				instanceHandleSet.Add(requiredOverrideSource);
			}
		}
		if (instanceHandleSet.Count > 0)
		{
			if (text == null)
			{
				text = "inheritFrom target changed";
			}
			foreach (InstanceHandle item in instanceHandleSet)
			{
				requiredOverrideSources.Remove(item);
			}
		}
		if (text != null)
		{
			if (State == DocumentState.Complete && !IsLoaded)
			{
				PrevalidatedLoad(text);
			}
			else
			{
				Load(text);
			}
		}
	}

	public void UpdateOutputAssets(OutputManager outputManager)
	{
		foreach (BinaryAsset value in outputManager.Assets.Values)
		{
			if (value.IsOutputAsset)
			{
				_Current.OutputAssets.Add(new OutputAsset(value));
			}
		}
	}

	private void UpdateDocumentHashes()
	{
		if (_Current.DependentFiles.Count == 0)
		{
			_Current.DependentFileHash = 0u;
		}
		else
		{
			using MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			foreach (string dependentFile in _Current.DependentFiles)
			{
				FileHashItem item = null;
				if (TryGetFileHashItem(dependentFile, out item))
				{
					binaryWriter.Write(HashProvider.GetTextHash(item.Path.ToLower()));
					binaryWriter.Write(item.Hash);
				}
			}
			byte[] array = memoryStream.ToArray();
			if (array.Length > 0)
			{
				_Current.DependentFileHash = FastHash.GetHashCode(array);
			}
			else
			{
				_Current.DependentFileHash = 0u;
			}
		}
		if (_Current.InclusionItems.Count == 0)
		{
			_Current.IncludePathHash = 0u;
			return;
		}
		_Current.IncludePathHash = (uint)_Current.InclusionItems.Count;
		foreach (InclusionItem inclusionItem in _Current.InclusionItems)
		{
			FileHashItem item2 = null;
			if (TryGetFileHashItem(inclusionItem.LogicalPath, out item2))
			{
				_Current.IncludePathHash = FastHash.GetHashCode(_Current.IncludePathHash, item2.Path.ToLower());
			}
		}
	}

	public void LoadXml(bool updateDependentFiles)
	{
		if (_Current.XmlDocument != null)
		{
			throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Document {0} attempted to load twice.", _Current.SourcePath);
		}
		_Current.DocumentProcessor.RecordXmlParse();
		_Current.DocumentProcessor.MeasureWork("rawParse", () => LoadXmlCore(updateDependentFiles));
	}

	private void LoadXmlCore(bool updateDependentFiles)
	{
		CacheInputBoundary.Validate(_Current.SourcePath);
		FileNameXmlResolver xmlResolver = FileNameResolver.GetXmlResolver(SourceDirectory);
		XmlReader sourceReader = null, xmlReader = null;
        XmlDocument document = null;
        bool subscribed = false;
        Exception primary = null;
		try
		{
            if (!File.Exists(_Current.SourcePath))
            {
                string s = "<?xml version='1.0' encoding='UTF-8'?>\n<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\n</AssetDeclaration>";
                sourceReader = XmlReader.Create(new StringReader(s));
            }
            else
            {
                PhysicalInputSnapshot.Entry sealedSource;
                if (PhysicalInputSnapshot.TryCapture(_Current.SourcePath, out sealedSource))
                {
                    Stream input = sealedSource.OpenRead();
                    try { sourceReader = XmlReader.Create(input, new XmlReaderSettings { CloseInput = true }, new Uri(_Current.SourcePath, UriKind.Absolute).AbsoluteUri); }
                    catch (Exception creationFailure)
                    {
                        try { input.Dispose(); }
                        catch (Exception cleanup) { try { creationFailure.Data["Core.XmlReaderCleanupFailure"] = cleanup; } catch { } }
                        throw;
                    }
                }
                else sourceReader = XmlReader.Create(_Current.SourcePath);
            }
            _Current.XmlReader = sourceReader;
            document = _Current.XmlDocument = new XmlDocument();
            xmlReader = new Mvp.Xml.XInclude.XIncludingReader(sourceReader) { XmlResolver = xmlResolver };
            _Current.NodeSourceInfoSet = new LinkedList<XmlNodeWithMetaData>();
            // Keep the original parsing-only error wrappers. Reader creation
            // failures retain their existing exception type and ordering.
            try
			{
                if (_Current.LogicalSourcePath.EndsWith(".xml"))
                {
                    document.NodeInserted += NodeInsertedHandler;
                    subscribed = true;
                }
                document.Load(xmlReader);
			}
            catch (XmlSchemaValidationException innerException)
			{
				throw new BinaryAssetBuilderException(innerException, ErrorCode.SchemaValidation);
			}
            catch (XmlException innerException)
            {
                throw new BinaryAssetBuilderException(innerException, ErrorCode.XmlFormattingError);
            }
		}
        catch (Exception error) { primary = error; throw; }
		finally
		{
            // Retain local owners: callbacks can replace _Current or its DOM.
            // Closing the base alone cannot close an active nested include.
            if (subscribed) document.NodeInserted -= NodeInsertedHandler;
            Exception cleanup = null;
            try { if (xmlReader != null) xmlReader.Close(); } catch (Exception error) { cleanup = error; }
            try { if (sourceReader != null && sourceReader.ReadState != ReadState.Closed) sourceReader.Close(); }
            catch (Exception error) { if (cleanup == null) cleanup = error; }
            if (cleanup != null)
            {
                if (primary == null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(cleanup).Throw();
                else try { primary.Data["Core.XmlReaderCleanupFailure"] = cleanup; } catch { }
            }
		}
		if (updateDependentFiles)
		{
			foreach (PathMapItem path in xmlResolver.Paths)
			{
				_Current.DependentFiles.Add(path.SourceUri.ToLower());
			}
		}
		_Current.NamespaceManager = new XmlNamespaceManager(xmlReader.NameTable);
		_Current.NamespaceManager.AddNamespace("ea", "uri:ea.com:eala:asset");
		_Current.NamespaceManager.AddNamespace("ms", "urn:schemas-microsoft-com:xslt");
	}

	public void InplaceLoad(string reason)
	{
		InternalLoad(reason, LoadType.InplaceLoad);
	}

	private void Load(string reason)
	{
		_Current.FromScratch();
		InternalLoad(reason, LoadType.FromScratch);
	}

	private void InternalLoad(string reason, LoadType type)
	{
		SetChanged(reason);
		if (State == DocumentState.Shallow)
		{
			_Tracer.TraceInfo("Reloading 'file://{0}'. Reason: {1}", SourcePath, _Current.ChangeReason);
		}
		else
		{
			_Tracer.TraceInfo("Loading 'file://{0}'.", SourcePath);
		}
		LoadXml(type == LoadType.FromScratch);
		GatherUnvalidatedTags();
		GatherDefines();
		if (type != LoadType.InplaceLoad)
		{
			GatherUnvalidatedIncludes();
		}
		GatherUnvalidatedInstances();
		_Current.State = DocumentState.Loaded;
		_Current.IsLoaded = true;
	}

	private void PrevalidatedLoad(string reason)
	{
		if (_Current.State != DocumentState.Complete)
		{
			throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Prevalidation load for {0} was called because of '{1}' when document has not been previously validated", SourcePath, reason);
		}
		LoadXml(updateDependentFiles: false);
		GatherUnvalidatedTags();
		GatherDefines();
		GatherUnvalidatedIncludes();
		GatherUnvalidatedInstances();
		_Current.State = DocumentState.Loaded;
		_Current.IsLoaded = true;
		ReloadedForInheritance = true;
	}

	private void NodeInsertedHandler(object sender, XmlNodeChangedEventArgs e)
	{
		IXmlLineInfo xmlLineInfo = _Current.XmlReader as IXmlLineInfo;
		if (xmlLineInfo.HasLineInfo())
		{
			_Current.NodeSourceInfoSet.AddFirst(new XmlNodeWithMetaData(e.Node, xmlLineInfo.LineNumber));
		}
	}

	public void FromLastHack()
	{
		if (_Current == null)
		{
			PrepareMetadataForLegacyExposure();
			_Current = new CurrentState();
			_Current.FromLast(this, _Last);
		}
	}

	public void Open(DocumentProcessor documentProcessor, FileHashItem hashItem, string logicalPath, string configuration)
	{
		if (_Current == null || _Current.State != DocumentState.Complete)
		{
			_Current = new CurrentState(documentProcessor, hashItem, logicalPath, configuration);
			if (_Last != null)
			{
				PrepareMetadataForLegacyExposure();
				_Current.FromLast(this, _Last);
			}
			else
			{
				Load("New Document");
			}
		}
	}

	public void ResetState()
	{
		_Current = null;
	}

	public override string ToString()
	{
		return $"{Path.GetFileName(SourcePath)} {State}";
	}

	public void MakeCacheable()
	{
		if (_Current == null || _Last != null)
		{
			return;
		}
		_Last = new LastState(_Current);
		foreach (InstanceDeclaration selfInstance in _Current.SelfInstances)
		{
			selfInstance.MakeCacheable();
		}
	}

	public void MakeComplete()
	{
		_Current.State = DocumentState.Complete;
		foreach (InstanceDeclaration selfInstance in SelfInstances)
		{
			selfInstance.MakeComplete();
		}
		_Last = null;
	}

	public void AddStreamsHints(string[] streams)
	{
		foreach (string item in streams)
		{
			if (!_Current.StreamHints.Contains(item))
			{
				_Current.StreamHints.Add(item);
			}
		}
	}

	public void Validate()
	{
		_Current.XmlDocument.Schemas.Add(_Current.DocumentProcessor.SchemaSet.Schemas);
		_Current.XmlDocument.Validate(HandleValidationEvents);
		_Current.NodeSourceInfoSet.Clear();
		_Current.NodeSourceInfoSet = null;
		ValidateInstances();
		UpdateDocumentHashes();
	}

	private bool TryGetFileHashItem(string logicalPath, out FileHashItem item)
	{
		string path = FileNameResolver.ResolvePath(SourceDirectory, logicalPath);
		return _Current.DocumentProcessor.Cache.TryGetFile(path, _Current.ConfigurationName, Settings.Current.TargetPlatform, out item);
	}

	public void HandleValidationEvents(object sender, ValidationEventArgs e)
	{
		if (e.Exception is XmlSchemaValidationException ex)
		{
			XmlNode node = ex.SourceObject as XmlNode;
			LinkedListNode<XmlNodeWithMetaData> linkedListNode = _Current.NodeSourceInfoSet.Find(new XmlNodeWithMetaData(node, 0));
			if (linkedListNode != null && linkedListNode.Value != null)
			{
				throw new BinaryAssetBuilderException(ErrorCode.SchemaValidation, "XML validation error encountered in '{0}' near line {1}:\n   {2}", SourcePath, linkedListNode.Value.LineNumber, e.Message);
			}
		}
		throw new BinaryAssetBuilderException(e.Exception, ErrorCode.SchemaValidation, "XML validation error encountered in '{0}'", SourcePath);
	}

	public void EvaluateDefinitions()
	{
        _Current.CachedDefineInputs = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var used in UsedDefines)
            _Current.CachedDefineInputs[used.Key] = AllDefines.GetEvaluatedValue(used.Key);
		IExpressionEvaluator evaluator = ExpressionEvaluatorWrapper.GetEvaluator(this);
		foreach (Definition selfDefine in SelfDefines)
		{
			if (selfDefine.OriginalValue != null)
			{
				selfDefine.EvaluatedValue = null;
                string oldStage = _Current.DefineReadStage, oldSite = _Current.DefineReadSite;
                _Current.DefineReadStage = "definitions";
                _Current.DefineReadSite = DependencyObservationLog.IsActive ? "Define:" + selfDefine.Name : null;
                try { evaluator.EvaluateDefinition(selfDefine); }
                finally { _Current.DefineReadStage = oldStage; _Current.DefineReadSite = oldSite; }
			}
			Definition value = null;
			if (AllDefines.TryGetValue(selfDefine.Name, out value) && selfDefine.Document != value.Document && !selfDefine.IsOverride)
			{
				throw new BinaryAssetBuilderException(ErrorCode.DuplicateDefine, "Definition {0} defined in {1} is already defined in {2}", selfDefine.Name, selfDefine.Document.SourcePath, value.Document.SourcePath);
			}
			AllDefines[selfDefine.Name] = selfDefine;
            _Current.DefineScopeEpoch++;
		}
	}

    private static string ObservationHandleName(InstanceHandle handle)
    {
        return !DependencyObservationLog.IsActive || handle == null ? null : handle.TypeName + ":" + handle.InstanceName;
    }

    // Called at LookupDefine's actual result boundary; do not query AllDefines again.
    public void ObserveDefineRead(string name, Definition definition, string value, bool succeeded)
    {
        DependencyObservationLog.RecordDefineRead(this, _Current.DefineReadStage ?? "directExpression",
            _Current.DefineReadSite, _Current.DefineScopeEpoch, name, definition, value, succeeded);
    }

	public bool ValidateCachedDefines()
	{
		foreach (KeyValuePair<string, string> usedDefine in UsedDefines)
		{
            string currentValue;
            if (_Current.CachedDefineInputs == null || !_Current.CachedDefineInputs.TryGetValue(usedDefine.Key, out currentValue))
                currentValue = AllDefines.GetEvaluatedValue(usedDefine.Key);
            if (usedDefine.Value != currentValue)
			{
				return false;
			}
		}
		return true;
	}

	public bool ValidateInheritFromSources()
	{
		foreach (InstanceDeclaration selfInstance in SelfInstances)
		{
			if (selfInstance.InheritFromHandle == null)
			{
				continue;
			}
			FindLocation location = FindLocation.None;
			InstanceDeclaration instanceDeclaration = FindInstanceObserved(selfInstance.InheritFromHandle, (selfInstance.InheritFromHandle == selfInstance.Handle) ? FindLocation.Self : FindLocation.None, out location, "inheritanceValidation", ObservationHandleName(selfInstance.Handle));
			if (instanceDeclaration == null)
			{
				throw new BinaryAssetBuilderException(ErrorCode.InheritFromError, "Instance {0} attempts to inherit from non-existing instance {1}.", selfInstance, selfInstance.InheritFromHandle);
			}
			switch (location)
			{
			case FindLocation.Tentative:
			{
				bool flag = false;
				foreach (InclusionItem inclusionItem in _Current.InclusionItems)
				{
					if (inclusionItem.Document == instanceDeclaration.Document)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new BinaryAssetBuilderException(ErrorCode.InheritFromError, "Instance {0} attempts to inherit from instance {1} which is not directly included.", selfInstance, selfInstance.InheritFromHandle);
				}
				break;
			}
			default:
				throw new BinaryAssetBuilderException(ErrorCode.InheritFromError, "Instance {0} attempts to inherit from instance {1} which is not included by 'instance'.", selfInstance, selfInstance.InheritFromHandle);
			case FindLocation.Self:
				break;
			}
			if (instanceDeclaration.PrevalidationXmlHash != selfInstance.InheritFromXmlHash)
			{
				return false;
			}
		}
		return true;
	}

	public void ProcessExpressions()
	{
		IExpressionEvaluator evaluator = ExpressionEvaluatorWrapper.GetEvaluator(this);
		if (evaluator == null)
		{
			return;
		}
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableExpressionTraversal", out disabled);
        DocumentProcessor workOwner = _Current.DocumentProcessor;
        var work = new DocumentProcessor.ExpressionTraversalWork();
        bool succeeded = false;
        CompilerDefineLookup previousLookup = _DefineLookup;
        string defineSource = ObservationSourcePath;
        if (previousLookup != null) previousLookup.Reentered();
        var defineWork = new CompilerDefineLookup.Work();
        var defineLookup = new CompilerDefineLookup(this, AllDefines, defineWork, previousLookup != null);
        _DefineLookup = defineLookup;
        try
        {
            foreach (InstanceDeclaration selfInstance in SelfInstances)
            {
                string oldStage = _Current.DefineReadStage, oldSite = _Current.DefineReadSite;
                _Current.DefineReadStage = "expressions";
                string consumer = ObservationHandleName(selfInstance.Handle);
                _Current.DefineReadSite = consumer;
                int ordinal = 0;
                try { ProcessExpressionsInNode(evaluator, selfInstance.XmlNode, consumer, ref ordinal, !disabled, work); }
                finally { _Current.DefineReadStage = oldStage; _Current.DefineReadSite = oldSite; }
            }
            succeeded = true;
        }
        finally
        {
            _DefineLookup = previousLookup;
            defineLookup.Dispose(); _DefineLookupWork = defineWork;
            workOwner?.RecordDefineLookupWork(this, defineSource);
            workOwner?.RecordExpressionTraversal(work, succeeded);
        }
	}

    private static bool StandardExpressionNode(XmlNode node)
    {
        Type type = node.GetType();
        return type == typeof(XmlElement) || type == typeof(XmlText) || type == typeof(XmlCDataSection) ||
            type == typeof(XmlComment) || type == typeof(XmlWhitespace) || type == typeof(XmlSignificantWhitespace) ||
            type == typeof(XmlProcessingInstruction) || type == typeof(XmlEntityReference);
    }

    private static bool SingleTextAttribute(XmlAttribute attribute)
    {
        if (attribute.GetType() != typeof(XmlAttribute)) return false;
        // An exact attribute may still aggregate a caller's derived XmlText or
        // EntityReference graph with observable virtual Value getters.
        XmlNode child = attribute.FirstChild;
        return child == null || (child.GetType() == typeof(XmlText) && child.NextSibling == null);
    }

	private void ProcessExpressionsInNode(IExpressionEvaluator eval, XmlNode node, string consumer, ref int ordinal,
        bool optimize, DocumentProcessor.ExpressionTraversalWork work)
	{
        // Compact preorder position identifies the actual read site without
        // allocating a full ancestor path at every node (or when disabled).
        int nodeOrdinal = ordinal++;
        work.nodes++;
		if (node.Attributes != null)
		{
			foreach (XmlAttribute attribute in node.Attributes)
			{
                if (optimize && SingleTextAttribute(attribute))
                {
                    work.singleReadAttributes++;
                    string value = attribute.Value;
                    if (value.Length > 0 && value[0] == '=')
                    {
                        if (DependencyObservationLog.IsActive)
                            _Current.DefineReadSite = consumer + "/" + node.Name + "[" + nodeOrdinal + "]/@" + attribute.Name;
                        attribute.Value = eval.Evaluate(value);
                    }
                    continue;
                }
                work.legacyAttributes++;
				if (attribute.Value.Length > 0 && attribute.Value[0] == '=')
				{
                    if (DependencyObservationLog.IsActive)
                        _Current.DefineReadSite = consumer + "/" + node.Name + "[" + nodeOrdinal + "]/@" + attribute.Name;
                    attribute.Value = eval.Evaluate(attribute.Value);
				}
			}
		}
        bool standard = optimize && StandardExpressionNode(node);
        if (standard)
        {
            work.singleReadNodes++;
            string value = node.Value;
            if (value != null && value.Length > 0 && value[0] == '=')
            {
                if (DependencyObservationLog.IsActive)
                    _Current.DefineReadSite = consumer + "/" + node.Name + "[" + nodeOrdinal + "]";
                node.Value = eval.Evaluate(value);
            }
        }
		else if (node.Value != null && node.Value.Length > 0 && node.Value[0] == '=')
		{
            if (DependencyObservationLog.IsActive)
                _Current.DefineReadSite = consumer + "/" + node.Name + "[" + nodeOrdinal + "]";
            node.Value = eval.Evaluate(node.Value);
		}
        if (standard)
        {
            work.siblingTraversals++;
            // XmlChildEnumerator.MoveNext reads FirstChild once and then the
            // current node's NextSibling after each body. Keep those live reads,
            // including removal/reorder behavior; never prefetch a successor.
            for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
                ProcessExpressionsInNode(eval, child, consumer, ref ordinal, optimize, work);
            return;
        }
        work.legacyTraversals++;
		if (node.ChildNodes == null)
		{
			return;
		}
        foreach (XmlNode childNode in node.ChildNodes)
        {
            ProcessExpressionsInNode(eval, childNode, consumer, ref ordinal, optimize, work);
        }
	}

    // One invocation owns the execution graph. A legacy XML digest is content
    // metadata, not a visiting/completion flag (zero is a valid digest).
    private sealed class InheritanceExecution
    {
        private sealed class IdentityComparer : IEqualityComparer<InstanceDeclaration>
        {
            public bool Equals(InstanceDeclaration left, InstanceDeclaration right) => Object.ReferenceEquals(left, right);
            public int GetHashCode(InstanceDeclaration value) => RuntimeHelpers.GetHashCode(value);
        }
        internal readonly HashSet<InstanceDeclaration> Active = new HashSet<InstanceDeclaration>(new IdentityComparer());
    }

    private sealed class InheritanceFrame
    {
        internal readonly InstanceDeclaration Instance;
        internal InstanceDeclaration Source;
        internal FindLocation Location;
        internal XmlNode IncludedSource;
        internal bool Entered;
        internal InheritanceFrame(InstanceDeclaration instance) { Instance = instance; }
    }

    private void OverrideInstance(InstanceDeclaration root, InheritanceExecution execution)
    {
        var pending = new Stack<InheritanceFrame>();
        pending.Push(new InheritanceFrame(root));
        try
        {
            while (pending.Count != 0)
            {
                InheritanceFrame frame = pending.Peek();
                InstanceDeclaration instance = frame.Instance;
                if (!frame.Entered)
                {
                    if (!execution.Active.Add(instance))
                        throw new BinaryAssetBuilderException(ErrorCode.CircularDependency,
                            "Illegal circular inheritance dependency found at {0} in document '{1}'.", instance.Handle, SourcePath);
                    instance.BeginPrevalidation();
                    frame.Entered = true;
                    if (instance.InheritFromHandle != null)
                    {
                        frame.Source = FindInstanceObserved(instance.InheritFromHandle,
                            (instance.InheritFromHandle == instance.Handle) ? FindLocation.Self : FindLocation.None,
                            out frame.Location, "inheritance", ObservationHandleName(instance.Handle));
                        InstanceDeclaration source = frame.Source;
                        if (source == null)
                            throw new BinaryAssetBuilderException(ErrorCode.InheritFromError, "Instance {0} attempts to inherit from or override non-existing instance {1}", instance.Handle, instance.InheritFromHandle);
                        if (!source.IsInheritable && frame.Location != FindLocation.Self)
                            throw new BinaryAssetBuilderException(ErrorCode.InheritFromError, "Instance {1} cannot be overriden because it is not of type BaseInheritableAsset", instance.Handle, instance.InheritFromHandle);
                        switch (frame.Location)
                        {
                            case FindLocation.Tentative:
                                foreach (InclusionItem inclusion in _Current.InclusionItems)
                                {
                                    if (inclusion.Document == source.Document)
                                    {
                                        frame.IncludedSource = source.XmlNode;
                                        break;
                                    }
                                }
                                break;
                            case FindLocation.Self:
                                if (execution.Active.Contains(source) || !source.HasPrevalidationResult)
                                {
                                    // Resume this exact lookup after its source, without
                                    // pre-resolving siblings or changing error order.
                                    pending.Push(new InheritanceFrame(source));
                                    continue;
                                }
                                break;
                            case FindLocation.None:
                                throw new BinaryAssetBuilderException(ErrorCode.InheritFromError, "Instance {0} attempts to inherit from an instance {1} which could not be found.", instance.Handle, instance.InheritFromHandle);
                            default:
                                throw new BinaryAssetBuilderException(ErrorCode.InheritFromError, "Instance {0}\n  in document '{1}'\n  attempts to inherit from instance {2}\n  from document '{3}'\n  which does not appear to be included by 'instance'.", instance.Handle, instance.Document.SourcePath, source, source.Document.SourcePath);
                        }
                    }
                }
                if (frame.Source != null)
                {
                    // Same-document sources are read after DFS, at precisely the
                    // old recursive return boundary. Included processed DOM is
                    // retained with its original SchemaInfo/defaults/source.
                    XmlNode sourceNode = frame.Location == FindLocation.Self ? frame.Source.XmlNode : frame.IncludedSource;
                    if (sourceNode == null)
                        throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Instance {0} attempts to inherit from instance {1} but source XML is missing.", instance.Handle, instance.InheritFromHandle);
                    DependencyObservationLog.RecordInheritance(this, instance, frame.Source, frame.Location,
                        frame.Location == FindLocation.Self ? "SameDocumentPrevalidation" : "IncludedDocumentProcessed");
                    XmlNode merged = NodeJoiner.OverrideWithWork(_Current.DocumentProcessor.SchemaSet.Schemas,
                        XmlDocument, sourceNode, instance.XmlNode, _Current.DocumentProcessor);
                    XmlNode parent = instance.XmlNode.ParentNode;
                    parent.RemoveChild(instance.XmlNode);
                    parent.AppendChild(merged);
                    instance.XmlNode = merged;
                    instance.InheritFromXmlHash = frame.Source.PrevalidationXmlHash;
                }
                XmlNode node = instance.XmlNode;
                long revision = instance.PrevalidationRevision;
                instance.PrevalidationXmlHash = HashProvider.GetXmlHash(ref node);
                instance.CompletePrevalidation(revision);
                execution.Active.Remove(instance);
                pending.Pop();
            }
        }
        catch
        {
            // A failed or reentrant partial merge is never a completed result.
            foreach (InheritanceFrame frame in pending) execution.Active.Remove(frame.Instance);
            throw;
        }
    }

	public void MergeInstances()
	{
		Instances.Clear();
		Instances.Add(SelfInstances);
		Instances.Add(AllInstances);
	}

	public void ProcessInstances(OutputManager outputManager, ref int instancesCompiledCount, ref int instancesCopiedFromCacheCount)
	{
		if (ReloadedForInheritance)
		{
			return;
		}
		foreach (InstanceDeclaration selfInstance in SelfInstances)
		{
			ExtendedTypeInformation extendedTypeInformation = _Current.DocumentProcessor.Plugins.GetExtendedTypeInformation(selfInstance.Handle.TypeId);
			selfInstance.HasCustomData = extendedTypeInformation.HasCustomData;
			BinaryAsset binaryAsset = outputManager.GetBinaryAsset(selfInstance, isOutputAsset: true);
			if (binaryAsset != null)
			{
				if (binaryAsset.GetLocation(AssetLocation.All, AssetLocationOption.None) == AssetLocation.None)
				{
					CompileInstance(binaryAsset, selfInstance);
				}
				VerifyInstance(binaryAsset, selfInstance);
				AssetLocation assetLocation = binaryAsset.Commit();
				CountCommitSource(assetLocation, ref instancesCompiledCount, ref instancesCopiedFromCacheCount);
				if (assetLocation == AssetLocation.Cache)
					_Current.DocumentProcessor.RecordBuildCacheUse(true);
				else if (assetLocation == AssetLocation.Memory && extendedTypeInformation.UseBuildCache && Settings.Current.BuildCache)
					_Current.DocumentProcessor.RecordBuildCacheUse(false);
				if (assetLocation != AssetLocation.BasePatchStream)
				{
					_Current.DocumentProcessor.AddLastWrittenAsset(binaryAsset);
				}
				if (!selfInstance.IsInheritable)
				{
					selfInstance.XmlNode = null;
				}
			}
		}
	}

	private void CountCommitSource(AssetLocation commitSource, ref int instancesCompiledCount, ref int instancesCopiedFromCacheCount)
	{
		switch (commitSource)
		{
		case AssetLocation.Memory:
			instancesCompiledCount++;
			break;
		case AssetLocation.Local:
			instancesCopiedFromCacheCount++;
			break;
		case AssetLocation.Cache:
			instancesCopiedFromCacheCount++;
			break;
		}
	}

	private void VerifyInstance(BinaryAsset asset, InstanceDeclaration declaration)
	{
		if (_Current.IsLoaded)
		{
			IAssetBuilderVerifierPlugin plugin = _Current.DocumentProcessor.VerifierPlugins.GetPlugin(asset.Instance.Handle.TypeId);
			if (!plugin.VerifyInstance(declaration))
			{
				_Current.VerificationErrors = true;
				throw new BinaryAssetBuilderException(ErrorCode.GameDataVerification, "FATAL: An asset failed the Game Data Verification step.  See previous output.");
			}
		}
	}

	private void CompileInstance(BinaryAsset asset, InstanceDeclaration declaration)
	{
		_Tracer.Message("{0}: Compiling {1}", Path.GetFileName(_Current.SourcePath), asset.Instance.ToString());
		if (asset.Instance.XmlNode == null)
		{
			throw new BinaryAssetBuilderException(ErrorCode.DependencyCacheFailure, "Need to compile instance {0} but XML is not loaded. This is a bug. Please notify cnc3technicalarchitecture@ea.com.", asset.Instance);
		}
		if (declaration.HasCustomData)
		{
			declaration.CustomDataPath = Path.Combine(asset.CustomDataOutputDirectory, asset.FileBase + ".cdata");
			if (!Directory.Exists(asset.CustomDataOutputDirectory))
			{
				Directory.CreateDirectory(asset.CustomDataOutputDirectory);
			}
		}
		DateTime now = DateTime.Now;
		IAssetBuilderPlugin plugin = _Current.DocumentProcessor.Plugins.GetPlugin(asset.Instance.Handle.TypeId);
		_Current.DocumentProcessor.RecordInstanceCompile();
		asset.Buffer = plugin.ProcessInstance(asset.Instance);
		_Current.DocumentProcessor.AddCompileTime(asset.Instance.Handle, DateTime.Now - now);
	}

    public void ProcessOverrides()
    {
        InheritanceExecution execution = null;
        foreach (InstanceDeclaration selfInstance in SelfInstances)
        {
            if (selfInstance.InheritFromHandle == null)
            {
                // The common non-inheriting path needs neither a graph nor DFS
                // frames. It still hashes on every legacy top-level visit.
                selfInstance.BeginPrevalidation();
                XmlNode node = selfInstance.XmlNode;
                long revision = selfInstance.PrevalidationRevision;
                selfInstance.PrevalidationXmlHash = HashProvider.GetXmlHash(ref node);
                selfInstance.CompletePrevalidation(revision);
                continue;
            }
            // Preserve the legacy top-level merge/hash/DOM event order, even
            // when this declaration was recursively completed by an earlier
            // sibling. Only recursive source readiness uses execution states.
            if (execution == null) execution = new InheritanceExecution();
            OverrideInstance(selfInstance, execution);
        }
    }

	private void GatherUnvalidatedTags()
	{
		Tags.Clear();
		XPathNavigator xPathNavigator = _Current.XmlDocument.CreateNavigator();
		XPathNodeIterator xPathNodeIterator = xPathNavigator.Evaluate("/ea:AssetDeclaration/ea:Tags/child::*", _Current.NamespaceManager) as XPathNodeIterator;
		foreach (XPathNavigator item in xPathNodeIterator)
		{
			_Current.Tags.Add(item.GetAttribute("name", ""), item.GetAttribute("value", ""));
		}
	}

	private void GatherDefines()
	{
		SelfDefines.Clear();
		XPathNavigator xPathNavigator = _Current.XmlDocument.CreateNavigator();
		XPathNodeIterator xPathNodeIterator = xPathNavigator.Evaluate("/ea:AssetDeclaration/ea:Defines/child::*", _Current.NamespaceManager) as XPathNodeIterator;
		foreach (XPathNavigator item in xPathNodeIterator)
		{
			Definition definition = new Definition();
			definition.Document = this;
			definition.OriginalValue = item.GetAttribute("value", "");
			definition.Name = item.GetAttribute("name", "");
			definition.IsOverride = item.GetAttribute("override", "") == "true";
			_Current.SelfDefines.Add(definition);
		}
	}

	private void GatherUnvalidatedIncludes()
	{
		InclusionItems.Clear();
		XPathNavigator xPathNavigator = _Current.XmlDocument.CreateNavigator();
		XPathNodeIterator xPathNodeIterator = xPathNavigator.Evaluate("/ea:AssetDeclaration/ea:Includes/child::*", _Current.NamespaceManager) as XPathNodeIterator;
		foreach (XPathNavigator item in xPathNodeIterator)
		{
			string text = item.GetAttribute("source", "").Trim().ToLower();
			string physicalPath = FileNameResolver.ResolvePath(SourceDirectory, text).ToLower();
			InclusionItem inclusionItem = new InclusionItem(text, physicalPath, (InclusionType)Enum.Parse(typeof(InclusionType), item.GetAttribute("type", ""), ignoreCase: true));
			_Current.InclusionItems.Add(inclusionItem);
			if (inclusionItem.Type == InclusionType.Instance)
			{
				_Current.DependentFiles.Add(text);
			}
		}
	}

	private void GatherUnvalidatedInstances()
	{
		SelfInstances.Clear();
		XmlNodeList xmlNodeList = _Current.XmlDocument.SelectNodes("/ea:AssetDeclaration/child::*", _Current.NamespaceManager);
		foreach (XmlNode item in xmlNodeList)
		{
			if (item.Name == "Includes" || item.Name == "Tags" || item.Name == "Defines")
			{
				continue;
			}
			InstanceDeclaration instanceDeclaration = new InstanceDeclaration(this);
			instanceDeclaration.XmlNode = item;
			if (!SelfInstances.TryAdd(instanceDeclaration))
			{
				InstanceDeclaration instanceDeclaration2 = SelfInstances[instanceDeclaration.Handle];
				if (instanceDeclaration.Handle.InstanceName == instanceDeclaration2.Handle.InstanceName)
				{
					throw new BinaryAssetBuilderException(ErrorCode.DuplicateInstance, "Duplicate Instance: {0}, in {1} and {2}", instanceDeclaration, instanceDeclaration.Document.SourcePath, instanceDeclaration2.Document.SourcePath);
				}
				throw new BinaryAssetBuilderException(ErrorCode.DuplicateInstance, "Duplicate Instance: {0} (other is {1})", instanceDeclaration, instanceDeclaration2);
			}
		}
	}

	private void ValidateInstances()
	{
		StringCollection derivedTypes = _Current.DocumentProcessor.SchemaSet.GetDerivedTypes("BaseInheritableAsset");
		foreach (InstanceDeclaration selfInstance in _Current.SelfInstances)
		{
			ExtendedTypeInformation extendedTypeInformation = _Current.DocumentProcessor.Plugins.GetExtendedTypeInformation(selfInstance.Handle.TypeId);
			uint textHash = HashProvider.GetTextHash(extendedTypeInformation.ProcessingHash, 11u.ToString());
			selfInstance.Handle.TypeHash = extendedTypeInformation.TypeHash;
			XmlNode node = selfInstance.XmlNode;
			selfInstance.Handle.InstanceHash = HashProvider.GetXmlHash(textHash, ref node);
			selfInstance.ProcessingHash = extendedTypeInformation.ProcessingHash;
			if (derivedTypes != null)
			{
				selfInstance.IsInheritable = derivedTypes.Contains(selfInstance.XmlNode.SchemaInfo.SchemaType.Name);
			}
			if (selfInstance.Handle.TypeName != selfInstance.XmlNode.SchemaInfo.SchemaType.Name)
			{
				throw new BinaryAssetBuilderException(ErrorCode.SchemaValidation, "Type name and element name do not match for {0}.", selfInstance.Handle);
			}
			if (selfInstance.Handle.TypeHash == 0)
			{
				_Tracer.TraceWarning("No type hash found for type {0}.", selfInstance.Handle.TypeName);
			}
			if (Settings.Current.OutputIntermediateXml)
			{
				OutputIntermediateXml(selfInstance);
			}
			XPathNavigator xPathNavigator = selfInstance.XmlNode.CreateNavigator();
			XPathNodeIterator xPathNodeIterator = xPathNavigator.SelectDescendants("", "uri:ea.com:eala:asset", matchSelf: true);
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			foreach (XPathNavigator item2 in xPathNodeIterator)
			{
				HandleReferenceType(selfInstance, item2, binaryWriter);
				if (item2.HasAttributes)
				{
					item2.MoveToFirstAttribute();
					do
					{
						HandleReferenceType(selfInstance, item2, binaryWriter);
					}
					while (item2.MoveToNextAttribute());
					item2.MoveToParent();
				}
				if (item2.SchemaInfo != null && item2.SchemaInfo.SchemaType != null && item2.SchemaInfo.SchemaType.Name != null)
				{
					item2.CreateAttribute("", "TypeId", "", HashProvider.GetCaseSensitiveSymbolHash(item2.SchemaInfo.SchemaType.Name).ToString());
				}
			}
			foreach (string referencedFile in selfInstance.ReferencedFiles)
			{
				FileHashItem item = null;
				TryGetFileHashItem(referencedFile, out item);
				binaryWriter.Write(item.Hash);
			}
			if (memoryStream.Length > 0)
			{
				selfInstance.Handle.InstanceHash ^= FastHash.GetHashCode(memoryStream.GetBuffer());
			}
		}
	}

	private void OutputIntermediateXml(InstanceDeclaration instance)
	{
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.NewLineOnAttributes = true;
		xmlWriterSettings.Encoding = Encoding.UTF8;
		xmlWriterSettings.Indent = true;
		xmlWriterSettings.IndentChars = "\t";
		string text = Path.Combine(Settings.Current.IntermediateOutputDirectory, "FinalXml_" + Settings.Current.TargetPlatform);
		string text2 = Path.Combine(instance.Handle.TypeName, instance.Handle.InstanceName);
		Directory.CreateDirectory(text);
		Directory.CreateDirectory(Path.Combine(text, instance.Handle.TypeName));
		string outputFileName = Path.Combine(text, text2 + Path.GetExtension(LogicalSourcePath));
		try
		{
			XmlWriter xmlWriter = XmlWriter.Create(outputFileName, xmlWriterSettings);
			XmlElement xmlElement = ReconstructAssetDeclaration(instance);
			xmlElement.WriteTo(xmlWriter);
			xmlWriter.Close();
		}
		catch (Exception ex)
		{
			_Tracer.TraceWarning("Warning: Unable to output {0}, reason: {1}", text2, ex.Message);
		}
	}

	private XmlElement ReconstructAssetDeclaration(InstanceDeclaration instance)
	{
		XmlElement xmlElement = XmlDocument.CreateElement("AssetDeclaration", Settings.Current.AssetNamespace);
		XmlElement xmlElement2 = XmlDocument.CreateElement("Tags", Settings.Current.AssetNamespace);
		XmlElement xmlElement3 = XmlDocument.CreateElement("Tag", Settings.Current.AssetNamespace);
		XmlAttribute xmlAttribute = XmlDocument.CreateAttribute("name");
		xmlAttribute.Value = "SourceXml";
		XmlAttribute xmlAttribute2 = XmlDocument.CreateAttribute("tag");
		xmlAttribute2.Value = LogicalSourcePath;
		xmlElement3.Attributes.Append(xmlAttribute);
		xmlElement3.Attributes.Append(xmlAttribute2);
		xmlElement2.AppendChild(xmlElement3);
		xmlElement.AppendChild(xmlElement2);
		xmlElement.AppendChild(instance.XmlNode);
		return xmlElement;
	}

	public void RecordStringHashes()
	{
		foreach (InstanceDeclaration selfInstance in _Current.SelfInstances)
		{
			HashProvider.RecordHash(selfInstance.Handle);
			foreach (InstanceHandle referencedInstance in selfInstance.ReferencedInstances)
			{
				HashProvider.RecordHash(referencedInstance);
			}
			foreach (InstanceHandle weakReferencedInstance in selfInstance.WeakReferencedInstances)
			{
				HashProvider.RecordHash(weakReferencedInstance);
			}
		}
	}

	private void HandleReferenceType(InstanceDeclaration instance, XPathNavigator navigator, BinaryWriter refTypeIds)
	{
		if (navigator.SchemaInfo == null || navigator.SchemaInfo.SchemaType == null)
		{
			throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Element {0} in instance {1} doesn't have a schema type.", navigator.Name, instance);
		}
		SchemaSet.ReferenceDescriptor descriptor = _Current.DocumentProcessor.SchemaSet.GetReferenceDescriptor(navigator.SchemaInfo);
		if (descriptor.Kind == SchemaSet.ReferenceKind.Weak || descriptor.Kind == SchemaSet.ReferenceKind.Strong)
		{
			HandleAssetReferenceType(navigator, ref instance, refTypeIds, descriptor.Kind == SchemaSet.ReferenceKind.Strong, descriptor.RefType);
		}
		else if (descriptor.Kind == SchemaSet.ReferenceKind.File)
		{
			HandleFileReferenceType(navigator, ref instance);
		}
		else if (descriptor.Kind == SchemaSet.ReferenceKind.Hashable)
		{
			HashProvider.RecordHash(navigator.SchemaInfo.SchemaType, navigator.Value);
		}
	}

	private void HandleFileReferenceType(XPathNavigator navigator, ref InstanceDeclaration instance)
	{
		string text = navigator.Value.Trim().ToLower();
		FileHashItem item = null;
		TryGetFileHashItem(text, out item);
		string value = item.Path.ToLower();
		_Current.DependentFiles.Add(text);
		navigator.SetValue(value);
		instance.ReferencedFiles.Add(text);
	}

	private void HandleAssetReferenceType(XPathNavigator navigator, ref InstanceDeclaration instance, BinaryWriter refTypeIds, bool isAssetRef, string refType)
	{
		string[] array = navigator.Value.Trim().Split('\\');
		string text = array[0];
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		InstanceHandle instanceHandle = new InstanceHandle(text);
		string text2 = refType;
		if (text2 == null)
		{
			throw new BinaryAssetBuilderException(ErrorCode.ReferencingError, "Asset reference to '{0}' in '{1}' does not have type (xas:refType missing in schema).", text, instance);
		}
		refTypeIds.Write(HashProvider.GetCaseSensitiveSymbolHash(text2));
		if (instanceHandle.TypeId == 0)
		{
			instanceHandle.TypeName = text2;
		}
		else if (isAssetRef)
		{
			XmlSchemaType xmlType = _Current.DocumentProcessor.SchemaSet.GetXmlType(instanceHandle.TypeName);
			XmlSchemaType xmlType2 = _Current.DocumentProcessor.SchemaSet.GetXmlType(text2);
			if (xmlType2 == null)
			{
				throw new BinaryAssetBuilderException(ErrorCode.ReferencingError, "Unable to establish schema type of underlying reference type '{0}'. Make sure it is defined and included in the schema set.", text2);
			}
			if (xmlType == null)
			{
				throw new BinaryAssetBuilderException(ErrorCode.ReferencingError, "Unable to establish schema type of referenced instance '{0}'. Make sure it is defined and included in the schema set.", instanceHandle.Name);
			}
			if (!XmlSchemaType.IsDerivedFrom(xmlType, xmlType2, XmlSchemaDerivationMethod.None))
			{
				throw new BinaryAssetBuilderException(ErrorCode.ReferencingError, "Type of instance '{0}' referenced from '{1}' does not appear to be equal to or derived from required reference type '{2}'.", instanceHandle.Name, instance.Handle.Name, text2);
			}
		}
		if (isAssetRef)
		{
			instance.ReferencedInstances.Add(instanceHandle);
			navigator.SetValue($"{text}\\{instance.ReferencedInstances.Count - 1}");
		}
		else
		{
			instance.WeakReferencedInstances.Add(instanceHandle);
		}
	}

    public InstanceDeclaration FindInstance(InstanceHandle handle, FindLocation skipLocation, out FindLocation location)
    {
        return FindInstanceObserved(handle, skipLocation, out location, "directSymbolLookup", null);
    }
    private bool TryFindObserved(InstanceSet instances, InstanceHandle handle, FindLocation candidateLocation,
        long query, long parentQuery, string stage, string site, out InstanceDeclaration value)
    {
        bool found = _SymbolLookup == null ? instances.TryGetValue(handle, out value) : _SymbolLookup.TryGetValue(instances, handle, out value);
        DependencyObservationLog.RecordSymbol("symbolCandidate", query, this, stage, site, handle, candidateLocation, value, found, null, parentQuery);
        return found;
    }
    private InstanceDeclaration FindInstanceObserved(InstanceHandle handle, FindLocation skipLocation, out FindLocation location, string stage, string site, long parentQuery = 0)
    {
        long query = DependencyObservationLog.BeginSymbolLookup();
        location = FindLocation.None;
        InstanceDeclaration value = null;
        try
        {
            if (_Current.SelfInstances != null && skipLocation != FindLocation.Self && TryFindObserved(_Current.SelfInstances, handle, FindLocation.Self, query, parentQuery, stage, site, out value))
                location = FindLocation.Self;
            else if (_Current.AllInstances != null && skipLocation != FindLocation.All && TryFindObserved(_Current.AllInstances, handle, FindLocation.All, query, parentQuery, stage, site, out value))
                location = FindLocation.All;
            else if (_Current.TentativeInstances != null && skipLocation != FindLocation.Tentative && TryFindObserved(_Current.TentativeInstances, handle, FindLocation.Tentative, query, parentQuery, stage, site, out value))
                location = FindLocation.Tentative;
            else if (_Current.ReferenceInstances != null && skipLocation != FindLocation.External && TryFindObserved(_Current.ReferenceInstances, handle, FindLocation.External, query, parentQuery, stage, site, out value))
                location = FindLocation.External;
            DependencyObservationLog.RecordSymbol("symbolLookup", query, this, stage, site, handle, location, value, location != FindLocation.None, null, parentQuery);
            return location == FindLocation.None ? null : value;
        }
        catch (Exception error)
        {
            var buildError = error as BinaryAssetBuilderException;
            DependencyObservationLog.RecordSymbol("symbolLookup", query, this, stage, site, handle, location, null, false,
                buildError == null ? error.GetType().FullName : buildError.ErrorCode.ToString(), parentQuery);
            throw;
        }
    }

	private InstanceDeclaration ResolveReference(InstanceDeclaration parentInstance, InstanceHandle referenceHandle, out FindLocation location, bool weak)
	{
        string stage = weak ? "weakReference" : "strongReference", site = ObservationHandleName(parentInstance.Handle);
        long query = DependencyObservationLog.BeginSymbolSelection();
        location = FindLocation.None;
        InstanceDeclaration instanceDeclaration = null;
        try
        {
        instanceDeclaration = FindInstanceObserved(referenceHandle, FindLocation.None, out location, stage + ":exact", site, query);
		if (instanceDeclaration == null)
		{
			InstanceHandle instanceHandle = new InstanceHandle(referenceHandle.TypeName, referenceHandle.InstanceName);
			// Schema's public derived-name view and its culture comparisons
            // remain legacy. A miss crossing that boundary ends this index.
            _SymbolLookup?.ExternalBoundary();
            StringCollection derivedTypes = _Current.DocumentProcessor.SchemaSet.GetDerivedTypes(referenceHandle.TypeName);
			if (derivedTypes != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				StringEnumerator enumerator = derivedTypes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						instanceHandle.TypeName = current;
						FindLocation location2;
						InstanceDeclaration instanceDeclaration2 = FindInstanceObserved(instanceHandle, FindLocation.None, out location2, stage + ":derived", site, query);
						if (instanceDeclaration2 != null)
						{
							instanceDeclaration = instanceDeclaration2;
							location = location2;
							if (num > 0)
							{
								stringBuilder.AppendFormat(", ");
							}
							stringBuilder.AppendFormat("'{0}'", instanceHandle.Name);
							num++;
						}
					}
				}
				finally
				{
					if (enumerator is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
				if (num > 1)
				{
					throw new BinaryAssetBuilderException(ErrorCode.ReferencingError, "Reference to instance '{0}' from '{1}' in 'file://{2}' is ambiguous. Possible matches: {3}.", referenceHandle.Name, parentInstance.Handle.Name, parentInstance.Document.SourcePath, stringBuilder);
				}
			}
		}
        DependencyObservationLog.RecordSymbol("symbolSelection", query, this, stage, site, referenceHandle, location, instanceDeclaration, instanceDeclaration != null);
        return instanceDeclaration;
        }
        catch (Exception error)
        {
            var buildError = error as BinaryAssetBuilderException;
            DependencyObservationLog.RecordSymbol("symbolSelection", query, this, stage, site, referenceHandle, location, instanceDeclaration, false,
                buildError == null ? error.GetType().FullName : buildError.ErrorCode.ToString());
            throw;
        }
    }

    private sealed class DeclarationReferenceComparer : IEqualityComparer<InstanceDeclaration>
    {
        public bool Equals(InstanceDeclaration left, InstanceDeclaration right) { return Object.ReferenceEquals(left, right); }
        public int GetHashCode(InstanceDeclaration value) { return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value); }
    }
    private void BeginStreamBindings()
    {
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableStreamBindingResults", out disabled);
        _Current.UseStreamBindingResults = !disabled;
        _Current.BindingSnapshot = null;
        _Current.StreamBindings = disabled ? null : new Dictionary<InstanceDeclaration, StreamBindingState>(new DeclarationReferenceComparer());
    }
    private StreamBindingState GetBindingState(InstanceDeclaration instance)
    {
        StreamBindingState result;
        return _Current.StreamBindings != null && _Current.StreamBindings.TryGetValue(instance, out result) ? result : null;
    }
    internal IEnumerable<InstanceHandle> GetValidatedReferences(InstanceDeclaration instance)
    {
        if (!_Current.UseStreamBindingResults) return instance.ValidatedReferencedInstances;
        StreamBindingState binding = GetBindingState(instance);
        return binding == null ? null : binding.References;
    }
    internal InstanceHandleSet GetDependentInstances(InstanceDeclaration instance)
    {
        if (!_Current.UseStreamBindingResults) return instance.AllDependentInstances;
        StreamBindingState binding = GetBindingState(instance);
        return binding == null ? null : binding.Dependencies;
    }
    public StreamBindingResult GetStreamBindings()
    {
        return _Current == null ? null : _Current.BindingSnapshot;
    }
    private void PublishStreamBindings()
    {
        var assets = new List<StreamBindingResult.AssetBinding>();
        foreach (InstanceDeclaration instance in _Current.OutputInstances)
            assets.Add(new StreamBindingResult.AssetBinding(instance, GetValidatedReferences(instance), GetDependentInstances(instance)));
        _Current.BindingSnapshot = new StreamBindingResult(_Current.UseStreamBindingResults, _Current.SourcePath, assets);
    }

	private void AddOutputInstance(InstanceDeclaration instance)
	{
		if (instance.Handle.TypeHash == 0 || _Current.OutputInstanceSet.ContainsKey(instance.Handle))
		{
			return;
		}
		_Current.OutputInstanceSet.Add(instance.Handle, instance);
        // OutputInstanceSet still handles cycles and the canonical first
        // declaration. All mutable binding work now belongs to this root.
        List2<InstanceHandle> references;
        InstanceHandleSet dependencies;
        if (_Current.UseStreamBindingResults)
        {
            var binding = new StreamBindingState(); _Current.StreamBindings.Add(instance, binding);
            references = binding.References; dependencies = binding.Dependencies;
        }
        else
        {
            references = instance.ValidatedReferencedInstances = new List2<InstanceHandle>();
            dependencies = instance.AllDependentInstances = new InstanceHandleSet();
        }
		foreach (InstanceHandle referencedInstance in instance.ReferencedInstances)
		{
			FindLocation location2 = FindLocation.None;
			InstanceDeclaration instanceDeclaration2 = ResolveReference(instance, referencedInstance, out location2, false);
			if (instanceDeclaration2 != null)
			{
				if (location2 != FindLocation.External)
				{
					dependencies.TryAdd(instanceDeclaration2.Handle);
					AddOutputInstance(instanceDeclaration2);
                    InstanceHandleSet targetDependencies = GetDependentInstances(instanceDeclaration2);
                    if (targetDependencies != null)
                    {
                        foreach (InstanceHandle allDependentInstance in targetDependencies)
						{
							dependencies.TryAdd(allDependentInstance);
						}
					}
				}
				references.Add(instanceDeclaration2.Handle);
			}
			else
			{
				if (Settings.Current.ErrorLevel > 0)
				{
					throw new BinaryAssetBuilderException(ErrorCode.UnknownReference, "Unknown referenced asset: {0}", referencedInstance);
				}
				_SymbolLookup?.ExternalBoundary();
                if (_Current.DocumentProcessor.MissingReferences.TryAdd(referencedInstance))
				{
					_Tracer.TraceWarning("Unknown asset '{0}' referenced from '{1}' in 'file://{2}'", referencedInstance.Name, instance.Handle.Name, instance.Document.SourcePath);
				}
				references.Add(referencedInstance);
			}
		}
		foreach (InstanceHandle weakReferencedInstance in instance.WeakReferencedInstances)
		{
			FindLocation location3 = FindLocation.None;
			InstanceDeclaration instanceDeclaration3 = ResolveReference(instance, weakReferencedInstance, out location3, true);
			if (instanceDeclaration3 != null && location3 == FindLocation.Tentative)
			{
				AddOutputInstance(instanceDeclaration3);
				references.Add(instanceDeclaration3.Handle);
			}
		}
		foreach (string referencedFile in instance.ReferencedFiles)
		{
			FileHashItem item = null;
			// A public ISessionCache can execute arbitrary consumer code.
            _SymbolLookup?.ExternalBoundary();
            if (!instance.Document.TryGetFileHashItem(referencedFile, out item))
			{
				if (Settings.Current.ErrorLevel > 0)
				{
					throw new BinaryAssetBuilderException(ErrorCode.FileNotFound, "Referenced file not found: {0}", referencedFile);
				}
				_Tracer.TraceWarning("Referenced file not found: {0}", referencedFile);
			}
		}
	}

	public void StableSort(OutputManager outputManager)
	{
		_Tracer.TraceInfo("Stable sorting assets");
		List2<InstanceDeclaration> finalList = new List2<InstanceDeclaration>();
		if (outputManager.BasePatchStreamManifest != null)
		{
			_Tracer.TraceInfo("Merging base stream assets from {0}", outputManager.BasePatchStream);
            DocumentProcessor metricOwner = _Current.DocumentProcessor; string metricSource = _Current.SourcePath;
            try { StableSort_InitializeFromBaseManifest(ref outputManager, ref _Current.OutputInstances, ref finalList); }
            finally { if (metricOwner != null) metricOwner.RecordBaseManifestWork(this, metricSource); }
		}
		_Tracer.TraceInfo("Sorting by schema dependencies");
        // The last trace may replace the processor's SchemaSet. Admission must
        // inspect the exact map already captured by this comparer, not a later
        // property value that the insertion loop will never consult.
        IDictionary<uint, int> typeDependencies = _Current.DocumentProcessor.SchemaSet.AssetDependencies;
		TypeDepCompare typeDepCompare = new TypeDepCompare(typeDependencies);
		_Current.OutputInstances.Sort(typeDepCompare);
		_Tracer.TraceInfo("Sorting by asset dependencies");
        DocumentProcessor membershipMetricOwner = _Current.DocumentProcessor; string membershipMetricSource = _Current.SourcePath;
        _StableSortMembershipWork = new CompilerStableSortMembership.Work();
        try
        {
            bool safeOwner = GetType() == typeof(AssetDeclarationDocument) && _Current.UseStreamBindingResults &&
                _Current.StreamBindings != null && _Current.StreamBindings.Comparer.GetType() == typeof(DeclarationReferenceComparer);
            CompilerStableSortMembership membership = CompilerStableSortMembership.Create(safeOwner, _Current.StreamBindings,
                _Current.OutputInstances, finalList, typeDependencies, _StableSortMembershipWork);
            foreach (InstanceDeclaration outputInstance in _Current.OutputInstances)
            {
                int index = StableSort_CalcInsertPositionCore(finalList, outputInstance, typeDepCompare, this, membership);
                finalList.Insert(index, outputInstance);
            }
            _Current.OutputInstances = finalList;
        }
        finally
        {
            if (membershipMetricOwner != null) membershipMetricOwner.RecordStableSortMembershipWork(this, membershipMetricSource);
        }
	}

    [NonSerialized] private CompilerStableSortMembership.Work _StableSortMembershipWork;
    public object GetStableSortMembershipMetrics()
    {
        return _StableSortMembershipWork == null ? null : _StableSortMembershipWork.Snapshot();
    }

    private sealed class BaseManifestWork
    {
        internal long Calls, IndexedCalls, LinearCalls, SmallGroupCalls, IndexEntries, BaseLookups, LinearCandidateChecks,
            LinearRemovalChecks, IndexLookups, RankSteps, Reused, Patched, Misses, CallbackFallbacks, MutationFallbacks, UnsafeFallbacks;
    }
    [NonSerialized] private BaseManifestWork _BaseManifestWork;
    public object GetBaseManifestWorkMetrics()
    {
        var work = _BaseManifestWork ?? new BaseManifestWork();
        return new { calls = work.Calls, indexedCalls = work.IndexedCalls, linearCalls = work.LinearCalls, smallGroupCalls = work.SmallGroupCalls,
            indexEntries = work.IndexEntries, baseLookups = work.BaseLookups, linearCandidateChecks = work.LinearCandidateChecks,
            linearRemovalChecks = work.LinearRemovalChecks, indexLookups = work.IndexLookups, rankSteps = work.RankSteps,
            reused = work.Reused, patched = work.Patched, misses = work.Misses, callbackFallbacks = work.CallbackFallbacks,
            mutationFallbacks = work.MutationFallbacks, unsafeFallbacks = work.UnsafeFallbacks };
    }
    private struct BaseManifestCandidate
    {
        internal readonly InstanceDeclaration Instance;
        internal readonly int Slot;
        internal BaseManifestCandidate(InstanceDeclaration instance, int slot) { Instance = instance; Slot = slot; }
    }
    private sealed class BaseManifestIndex
    {
        internal readonly Dictionary<ulong, Queue<BaseManifestCandidate>> Candidates = new Dictionary<ulong, Queue<BaseManifestCandidate>>();
        internal readonly int[] Alive;
        internal int ExpectedVersion, ExpectedCount;
        internal readonly List2<InstanceDeclaration> Source;
        internal readonly long CallbackVersion;
        private BaseManifestIndex(List2<InstanceDeclaration> source)
        {
            Source = source; ExpectedCount = source.Count;
            Alive = new int[source.Count + 1]; ExpectedVersion = source.MutationVersion; CallbackVersion = Tracer.CallbackVersion;
            for (int index = 1; index < Alive.Length; index++) Alive[index] = index & -index;
        }
        internal static BaseManifestIndex Create(List2<InstanceDeclaration> instances, BaseManifestWork work)
        {
            var result = new BaseManifestIndex(instances);
            for (int index = 0; index < instances.Count; index++)
            {
                InstanceDeclaration instance = instances[index];
                // Equals is virtual. Only the ordinary declaration's known key
                // equality permits RemoveAt to replace the old Remove search.
                if (instance == null || instance.GetType() != typeof(InstanceDeclaration)) return null;
                InstanceHandle handle;
                try { handle = instance.Handle; }
                catch (NullReferenceException) { return null; }
                if (handle == null) return null;
                ulong key = Key(handle.TypeId, handle.InstanceId);
                Queue<BaseManifestCandidate> candidates;
                if (!result.Candidates.TryGetValue(key, out candidates)) result.Candidates.Add(key, candidates = new Queue<BaseManifestCandidate>());
                candidates.Enqueue(new BaseManifestCandidate(instance, index)); work.IndexEntries++;
            }
            return result;
        }
        internal int Position(int slot, BaseManifestWork work)
        {
            int result = 0;
            for (int index = slot + 1; index > 0; index -= index & -index) { result += Alive[index]; work.RankSteps++; }
            return result - 1;
        }
        internal void Removed(int slot, int version, int count, BaseManifestWork work)
        {
            for (int index = slot + 1; index < Alive.Length; index += index & -index) { Alive[index]--; work.RankSteps++; }
            ExpectedVersion = version; ExpectedCount = count;
        }
    }
    private static ulong Key(uint typeId, uint instanceId) { return ((ulong)typeId << 32) | instanceId; }
    private static bool IndexStillValid(BaseManifestIndex index, List2<InstanceDeclaration> instances, BaseManifestWork work)
    {
        bool callback = Tracer.CallbackVersion != index.CallbackVersion;
        bool mutation = !Object.ReferenceEquals(instances, index.Source) || instances.Count != index.ExpectedCount || instances.MutationVersion != index.ExpectedVersion;
        if (callback) work.CallbackFallbacks++;
        if (mutation) work.MutationFallbacks++;
        return !callback && !mutation;
    }
    private void StableSort_InitializeFromBaseManifest(ref OutputManager outputManager, ref List2<InstanceDeclaration> outputInstances, ref List2<InstanceDeclaration> finalList)
    {
        BaseManifestWork work = _BaseManifestWork ?? (_BaseManifestWork = new BaseManifestWork()); work.Calls++;
        Manifest basePatchStreamManifest = outputManager.BasePatchStreamManifest;
        Asset[] assets = basePatchStreamManifest.Assets;
        if (assets.Length == 0) return; // Legacy never touches either list here.
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableBaseManifestIndex", out disabled);
        bool small = outputInstances == null || outputInstances.Count < 16 || assets.Length < 8;
        BaseManifestIndex index = null;
        if (!disabled && !small && !Object.ReferenceEquals(outputInstances, finalList) && outputInstances.Count <= 262144)
        {
            index = BaseManifestIndex.Create(outputInstances, work);
            if (index == null) work.UnsafeFallbacks++;
        }
        if (index == null) { work.LinearCalls++; if (small) work.SmallGroupCalls++; }
        else work.IndexedCalls++;
        foreach (Asset baseAsset in assets)
        {
            work.BaseLookups++;
            if (index != null && !IndexStillValid(index, outputInstances, work)) index = null;
            InstanceDeclaration instanceDeclaration; Queue<BaseManifestCandidate> candidates = null; int slot = -1;
            if (index == null) instanceDeclaration = FindInstance(baseAsset, outputInstances, work);
            else if (outputInstances.Count == 0) instanceDeclaration = null;
            else
            {
                work.IndexLookups++;
                if (index.Candidates.TryGetValue(Key(baseAsset.TypeId, baseAsset.InstanceId), out candidates) && candidates.Count != 0)
                {
                    BaseManifestCandidate candidate = candidates.Peek(); slot = candidate.Slot; instanceDeclaration = candidate.Instance;
                }
                else instanceDeclaration = null;
            }
            if (instanceDeclaration == null) { work.Misses++; continue; }
            if (outputManager.GetBinaryAsset(instanceDeclaration, isOutputAsset: false).GetLocation(AssetLocation.BasePatchStream, AssetLocationOption.None) == AssetLocation.BasePatchStream)
            {
                _Tracer.TraceInfo("Using base stream instance {0}:{1}", instanceDeclaration.Handle.TypeName, instanceDeclaration.Handle.InstanceName);
                // A callback can change any handle, even without touching the
                // list version. Permanently return to the old path after one.
                if (index != null && !IndexStillValid(index, outputInstances, work)) index = null;
                finalList.Add(instanceDeclaration);
                if (index == null) RemoveInstanceLegacy(outputInstances, instanceDeclaration, work);
                else
                {
                    int position = index.Position(slot, work);
                    outputInstances.RemoveAt(position); candidates.Dequeue();
                    index.Removed(slot, outputInstances.MutationVersion, outputInstances.Count, work);
                }
                work.Reused++;
            }
            else
            {
                _Tracer.TraceInfo("Using patched instance {0}:{1}", instanceDeclaration.Handle.TypeName, instanceDeclaration.Handle.InstanceName);
                if (index != null && !IndexStillValid(index, outputInstances, work)) index = null;
                work.Patched++;
            }
        }
    }
    private static InstanceDeclaration FindInstance(Asset baseAsset, List2<InstanceDeclaration> instances, BaseManifestWork work)
    {
        foreach (InstanceDeclaration instance in instances)
        {
            work.LinearCandidateChecks++;
            if (baseAsset.TypeId == instance.Handle.TypeId && baseAsset.InstanceId == instance.Handle.InstanceId) return instance;
        }
        return null;
    }
    private static void RemoveInstanceLegacy(List2<InstanceDeclaration> instances, InstanceDeclaration instance, BaseManifestWork work)
    {
        int count = instances.Count;
        for (int index = 0; index < count; index++)
        {
            work.LinearRemovalChecks++;
            if (instances[index].Equals(instance)) { instances.RemoveAt(index); return; }
        }
    }

    private static int StableSort_CalcInsertPosition(List2<InstanceDeclaration> finalList, InstanceDeclaration instance, TypeDepCompare depCompare)
    {
        return StableSort_CalcInsertPositionCore(finalList, instance, depCompare, null);
    }
    private static int StableSort_CalcInsertPositionCore(List2<InstanceDeclaration> finalList, InstanceDeclaration instance, TypeDepCompare depCompare, AssetDeclarationDocument owner, CompilerStableSortMembership membership = null)
    {
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		uint num5 = 0u;
		uint num6 = 0u;
		bool flag = false;
		foreach (InstanceDeclaration final in finalList)
		{
			if (final.Handle.TypeId != num6)
			{
				num3 = num2;
				num6 = final.Handle.TypeId;
			}
            InstanceHandleSet instanceDependencies = owner == null ? instance.AllDependentInstances : owner.GetDependentInstances(instance);
            InstanceHandleSet finalDependencies = owner == null ? final.AllDependentInstances : owner.GetDependentInstances(final);
            if (instanceDependencies != null && (membership == null ? instanceDependencies.Contains(final.Handle) : membership.Contains(instanceDependencies, final.Handle)))
			{
				num = num2 + 1;
				num4 = num;
				if (instance.Handle.TypeId != final.Handle.TypeId)
				{
					flag = false;
					num5 = final.Handle.TypeId;
				}
			}
			else
			{
				if (finalDependencies != null && (membership == null ? finalDependencies.Contains(instance.Handle) : membership.Contains(finalDependencies, instance.Handle)))
				{
					if (num3 > num4 && instance.Handle.TypeId != final.Handle.TypeId)
					{
						num = num3;
					}
					break;
				}
				if (instance.Handle.TypeId == final.Handle.TypeId)
				{
					if (!flag)
					{
						num = num2;
						flag = true;
					}
					if (string.Compare(instance.Handle.InstanceName, final.Handle.InstanceName) > 0)
					{
						num = num2 + 1;
					}
				}
				else if (final.Handle.TypeId == num5)
				{
					num = num2 + 1;
				}
				else
				{
					num5 = 0u;
					if (depCompare.Compare(instance, final) > 0)
					{
						num = num2 + 1;
					}
				}
			}
			num2++;
		}
		return num;
	}

    public void PrepareOutputInstances(OutputManager outputManager)
    {
        BeginStreamBindings();
        try
        {
            PrepareOutputInstancesCore(outputManager);
            PublishStreamBindings();
        }
        catch
        {
            // No partial result can masquerade as a successful binding view.
            _Current.BindingSnapshot = null; _Current.StreamBindings = null;
            throw;
        }
    }
    private void PrepareOutputInstancesCore(OutputManager outputManager)
    {
		_Current.OutputInstanceSet = new SortedDictionary<InstanceHandle, InstanceDeclaration>();
        _Current.OutputInstances.Clear();
        DocumentProcessor symbolOwner = _Current.DocumentProcessor; string symbolSource = _Current.SourcePath;
        _SymbolLookupWork = new CompilerSymbolLookup.Work();
        _SymbolLookup = CompilerSymbolLookup.Create(GetType() == typeof(AssetDeclarationDocument) && _Current.UseStreamBindingResults,
            new[] { Instances, SelfInstances, AllInstances, TentativeInstances, ReferenceInstances }, _SymbolLookupWork);
        try
        {
            foreach (InstanceDeclaration instance in Instances) AddOutputInstance(instance);
        }
        finally
        {
            // Mutable declarations/tables cannot escape through diagnostics.
            _SymbolLookup = null;
            if (symbolOwner != null) symbolOwner.RecordSymbolLookupWork(this, symbolSource);
        }
		foreach (InstanceDeclaration value in _Current.OutputInstanceSet.Values)
		{
			BinaryAsset binaryAsset = outputManager.GetBinaryAsset(value, isOutputAsset: true);
			if (binaryAsset != null)
			{
				AssetLocation location = binaryAsset.GetLocation(AssetLocation.All, AssetLocationOption.None);
				if (location == AssetLocation.None)
				{
					location = binaryAsset.GetLocation(AssetLocation.Local | AssetLocation.Cache, AssetLocationOption.ForceUpdate);
				}
				if (location == AssetLocation.None)
				{
					throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Asset {0} not available.", value);
				}
				if ((location & AssetLocation.Output) == 0)
				{
					binaryAsset.Commit();
				}
				_Current.OutputInstances.Add(value);
				if (Settings.Current.OutputAssetReport)
				{
					AssetReport.RecordAsset(value, binaryAsset, GetValidatedReferences(value));
				}
			}
		}
		if (Settings.Current.StableSort)
		{
			StableSort(outputManager);
		}
		else
		{
			_Current.OutputInstances.Sort(new DependencyComparer(this));
		}
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		foreach (InstanceDeclaration outputInstance in _Current.OutputInstances)
		{
			binaryWriter.Write(outputInstance.Handle.TypeId);
			binaryWriter.Write(outputInstance.Handle.TypeHash);
			binaryWriter.Write(outputInstance.Handle.InstanceId);
			binaryWriter.Write(outputInstance.Handle.InstanceHash);
			binaryWriter.Write(outputInstance.ReferencedInstances.Count);
		}
		if (memoryStream.Length > 0)
		{
			_Current.OutputChecksum = FastHash.GetHashCode(memoryStream.GetBuffer());
		}
		else
		{
			_Current.OutputChecksum = 0u;
		}
		_Current.OutputInstanceSet = null;
	}

    private void PrepareMetadataForLegacyExposure()
    {
        if (_Last == null || DocumentSnapshotsEnabled() || !_Last.OwnedMetadata) return;
        if (_Last.SharedMetadata) _Last = _Last.DetachMetadata();
        // Revoke before exposing any values, including partial restores.
        _Last.OwnedMetadata = false;
    }

	public void CacheFromDocument(AssetDeclarationDocument other)
	{
		if (other._Last != null)
		{
            other.PrepareMetadataForLegacyExposure();
            _Last = DocumentSnapshotsEnabled() ? new LastState(other._Last) : other._Last;
		}
		else
		{
			if (other._Current == null || _Last != null)
			{
				return;
			}
            _Last = new LastState(other._Current, true);
            if (DocumentSnapshotsEnabled() && !_Last.LegacyDeclarations) return;
            List2<InstanceDeclaration> list = new List2<InstanceDeclaration>();
			foreach (InstanceDeclaration selfInstance in other._Current.SelfInstances)
			{
				InstanceDeclaration instanceDeclaration = new InstanceDeclaration();
				instanceDeclaration.CacheFromInstance(selfInstance);
				list.Add(instanceDeclaration);
			}
			_Last.SelfInstances = new List2<InstanceDeclaration>(list);
		}
	}
}
