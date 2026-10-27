using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.Core;

public class SchemaSet
{
	// Lifetime metrics for this SchemaSet, including reloads. These intervals nest
	// inside the application's schema preparation interval and must not be added to it.
	private readonly Dictionary<string, DocumentProcessor.WorkMeasurement> _PreparationWork = new Dictionary<string, DocumentProcessor.WorkMeasurement>(StringComparer.Ordinal);
	public long SchemaFileReadCount { get; private set; }
	public long SchemaContentBytesRead { get; private set; }
	public long SchemaContentCheckCount { get; private set; }
	public DocumentProcessor.WorkMeasurement[] GetPreparationMetrics()
	{
		var result = new DocumentProcessor.WorkMeasurement[_PreparationWork.Count];
		int index = 0;
		foreach (var item in _PreparationWork.Values)
			result[index++] = new DocumentProcessor.WorkMeasurement(item.stage) { calls = item.calls, failedCalls = item.failedCalls, ticks = item.ticks };
		Array.Sort(result, (left, right) => StringComparer.Ordinal.Compare(left.stage, right.stage));
		return result;
	}
	private void MeasurePreparation(string stage, Action operation)
	{
		if (!_PreparationWork.TryGetValue(stage, out var work)) _PreparationWork.Add(stage, work = new DocumentProcessor.WorkMeasurement(stage));
		work.calls++;
		long start = Stopwatch.GetTimestamp();
		try { operation(); }
		catch { work.failedCalls++; throw; }
		finally { work.ticks += Stopwatch.GetTimestamp() - start; }
	}

	private class SchemaHashTable : SortedDictionary<XmlSchema, uint>
	{
	}

	private class DependencyList
	{
		public string TypeName;

		public List<uint> Children = new List<uint>();

		public IList<DependencyList> FixedUpChildren;

		public Set<uint> GrandChildren;

		public DependencyList(string typeName)
		{
			TypeName = typeName;
		}
	}

	internal enum ReferenceKind { None, Weak, Strong, File, Hashable }

	// Values contain only our own immutable facts, not navigator/DOM state or a
	// previously resolved asset. Keys belong to this one loaded schema generation.
	internal sealed class ReferenceDescriptor
	{
		internal readonly ReferenceKind Kind;
		internal readonly string RefType;
		internal ReferenceDescriptor(ReferenceKind kind, string refType) { Kind = kind; RefType = refType; }
	}

	private struct ReferenceKey : IEquatable<ReferenceKey>
	{
		private readonly XmlSchemaElement element;
		private readonly XmlSchemaAttribute attribute;
		private readonly XmlSchemaType type;
		internal ReferenceKey(IXmlSchemaInfo info) { element = info.SchemaElement; attribute = info.SchemaAttribute; type = info.SchemaType; }
		public bool Equals(ReferenceKey other) => ReferenceEquals(element, other.element) && ReferenceEquals(attribute, other.attribute) && ReferenceEquals(type, other.type);
		public override bool Equals(object value) => value is ReferenceKey other && Equals(other);
		public override int GetHashCode() => (element == null ? 0 : RuntimeHelpers.GetHashCode(element)) ^ (attribute == null ? 0 : RuntimeHelpers.GetHashCode(attribute)) ^ (type == null ? 0 : RuntimeHelpers.GetHashCode(type));
	}

	private readonly bool _UseReferenceDescriptors;
	private Dictionary<ReferenceKey, ReferenceDescriptor> _ReferenceDescriptors;
	private long _ReferenceDescriptorBuildCount;
	private long _ReferenceDescriptorHitCount;

	public long ReferenceDescriptorBuildCount => _ReferenceDescriptorBuildCount;
	public long ReferenceDescriptorHitCount => _ReferenceDescriptorHitCount;

	private static string GetRefTypeName(XmlAttribute[] attributes)
	{
		if (attributes != null)
			foreach (XmlAttribute attribute in attributes)
				if (attribute.NamespaceURI == "uri:ea.com:eala:asset:schema" && attribute.LocalName == "refType") return attribute.Value;
		return null;
	}

	private ReferenceDescriptor DescribeReference(IXmlSchemaInfo info)
	{
		XmlSchemaType type = info.SchemaType;
		ReferenceKind kind;
		if (XmlSchemaType.IsDerivedFrom(type, _XmlWeakReferenceType, XmlSchemaDerivationMethod.None)) kind = ReferenceKind.Weak;
		else if (XmlSchemaType.IsDerivedFrom(type, _XmlAssetReferenceType, XmlSchemaDerivationMethod.None)) kind = ReferenceKind.Strong;
		else if (XmlSchemaType.IsDerivedFrom(type, _XmlFileReferenceType, XmlSchemaDerivationMethod.None)) kind = ReferenceKind.File;
		else if (IsHashableType(type)) kind = ReferenceKind.Hashable;
		else kind = ReferenceKind.None;
		string refType = null;
		if (kind == ReferenceKind.Weak || kind == ReferenceKind.Strong)
		{
			if (info.SchemaElement != null) refType = GetRefTypeName(info.SchemaElement.UnhandledAttributes);
			else if (info.SchemaAttribute != null) refType = GetRefTypeName(info.SchemaAttribute.UnhandledAttributes);
			if (refType == null)
			{
				refType = GetRefTypeName(type.UnhandledAttributes);
				if (refType == null && type.DerivedBy == XmlSchemaDerivationMethod.Extension)
					refType = GetRefTypeName(type.BaseXmlSchemaType.UnhandledAttributes);
			}
		}
		return new ReferenceDescriptor(kind, refType);
	}

	internal ReferenceDescriptor GetReferenceDescriptor(IXmlSchemaInfo info)
	{
		if (_LoadedSchemaPath == null) throw new InvalidOperationException("Schema reference descriptions require a successfully loaded schema generation.");
		var key = new ReferenceKey(info);
		if (_UseReferenceDescriptors && _ReferenceDescriptors.TryGetValue(key, out ReferenceDescriptor descriptor))
		{
			_ReferenceDescriptorHitCount++;
			return descriptor;
		}
		descriptor = DescribeReference(info);
		_ReferenceDescriptorBuildCount++;
		if (_UseReferenceDescriptors) _ReferenceDescriptors.Add(key, descriptor);
		return descriptor;
	}

	public const string XmlNamespace = "uri:ea.com:eala:asset";

	private XmlSchemaSet _Schemas;

	private XmlSchemaType _XmlBaseAssetType;

	private XmlSchemaType _XmlBaseInheritableAsset;

	private XmlSchemaType _XmlAssetReferenceType;

	private XmlSchemaType _XmlWeakReferenceType;

	private XmlSchemaType _XmlFileReferenceType;

	private IDictionary<XmlSchemaType, bool> _HashableTypes;

	private IDictionary<string, DateTime> _SchemaPaths;

	private IDictionary<string, string> _SchemaContentHashes;

	private string _LoadedSchemaPath;

	private string _CurrentSchema = "<unknown>";

	private IDictionary<string, StringCollection> _InheritanceMap;

	private IDictionary<uint, int> _AssetDependencies;

	private IDictionary<uint, DependencyList> _DependencyTree;

	private Set<uint> _CircularCheck = new Set<uint>();

	public XmlSchemaSet Schemas => _Schemas;

	public XmlSchemaType XmlBaseAssetType => _XmlBaseAssetType;

	public XmlSchemaType XmlBaseInheritableAsset => _XmlBaseInheritableAsset;

	public XmlSchemaType XmlAssetReferenceType => _XmlAssetReferenceType;

	public XmlSchemaType XmlWeakReferenceType => _XmlWeakReferenceType;

	public XmlSchemaType XmlFileReferenceType => _XmlFileReferenceType;

	public IDictionary<uint, int> AssetDependencies => _AssetDependencies;

	public bool IsHashableType(XmlSchemaType type)
	{
		return _HashableTypes.ContainsKey(type);
	}

	private void ReadSchema(string baseDirectory, string schemaFile)
	{
		schemaFile = (Path.IsPathRooted(schemaFile) ? Path.GetFullPath(schemaFile) : Path.GetFullPath(Path.Combine(baseDirectory, schemaFile)));
		CacheInputBoundary.Validate(schemaFile);
		_CurrentSchema = schemaFile;
		if (_SchemaPaths.ContainsKey(schemaFile)) return;
		byte[] contents = File.ReadAllBytes(schemaFile);
		SchemaFileReadCount++;
		SchemaContentBytesRead += contents.LongLength;
		_SchemaPaths.Add(schemaFile, File.GetLastWriteTime(schemaFile));
		using (var algorithm = SHA256.Create())
			_SchemaContentHashes.Add(schemaFile, Convert.ToBase64String(algorithm.ComputeHash(contents)));
		XmlSchema xmlSchema;
		using (var input = new MemoryStream(contents, false))
		using (var reader = new StreamReader(input))
			xmlSchema = XmlSchema.Read(reader, null);
		foreach (XmlSchemaInclude include in xmlSchema.Includes)
		{
			ReadSchema(Path.GetDirectoryName(schemaFile), include.SchemaLocation);
		}
		xmlSchema.Includes.Clear();
		_Schemas.Add(xmlSchema);
	}

	public XmlSchemaType GetXmlType(string typeName)
	{
		try
		{
			return Schemas.GlobalTypes[new XmlQualifiedName(typeName, "uri:ea.com:eala:asset")] as XmlSchemaType;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private void BuildGrandchildren(DependencyList l)
	{
		if (l.FixedUpChildren != null)
		{
			return;
		}
		l.FixedUpChildren = new List<DependencyList>();
		l.GrandChildren = new Set<uint>();
		foreach (uint child in l.Children)
		{
			if (_DependencyTree.TryGetValue(child, out var value))
			{
				l.FixedUpChildren.Add(value);
			}
			l.GrandChildren.Add(child);
		}
		foreach (DependencyList fixedUpChild in l.FixedUpChildren)
		{
			BuildGrandchildren(fixedUpChild);
			if (fixedUpChild == l)
			{
				continue;
			}
			foreach (uint grandChild in fixedUpChild.GrandChildren)
			{
				l.GrandChildren.Add(grandChild);
			}
		}
	}

	private void CountReference(uint typeId, XmlAttribute[] unhandled)
	{
		foreach (XmlAttribute xmlAttribute in unhandled)
		{
			if (xmlAttribute.LocalName == "refType")
			{
				_DependencyTree[typeId].Children.Add(InstanceHandle.GetTypeId(xmlAttribute.Value));
				break;
			}
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaObjectCollection items)
	{
		foreach (XmlSchemaObject item in items)
		{
			if (item is XmlSchemaElement element)
			{
				BuildAssetDependencies(typeId, element);
			}
			if (item is XmlSchemaAttribute simple)
			{
				BuildAssetDependencies(typeId, simple);
			}
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaComplexContentExtension complex)
	{
		if (complex.Attributes != null)
		{
			BuildAssetDependencies(typeId, complex.Attributes);
		}
		if (complex.Particle != null)
		{
			BuildAssetDependencies(typeId, complex.Particle);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaSimpleContentExtension simple)
	{
		if (simple.Attributes != null)
		{
			BuildAssetDependencies(typeId, simple.Attributes);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaContentModel contentModel)
	{
		if (contentModel.Content is XmlSchemaComplexContentExtension complex)
		{
			BuildAssetDependencies(typeId, complex);
		}
		XmlSchemaSimpleContentExtension simple = contentModel.Content as XmlSchemaSimpleContentExtension;
		if (contentModel.Content is XmlSchemaSimpleContentExtension)
		{
			BuildAssetDependencies(typeId, simple);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaParticle particle)
	{
		if (particle is XmlSchemaChoice xmlSchemaChoice)
		{
			BuildAssetDependencies(typeId, xmlSchemaChoice.Items);
		}
		if (particle is XmlSchemaSequence xmlSchemaSequence)
		{
			BuildAssetDependencies(typeId, xmlSchemaSequence.Items);
		}
		if (particle is XmlSchemaAll xmlSchemaAll)
		{
			BuildAssetDependencies(typeId, xmlSchemaAll.Items);
		}
		if (particle is XmlSchemaElement element)
		{
			BuildAssetDependencies(typeId, element);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaComplexType complex)
	{
		if (complex.Attributes != null)
		{
			BuildAssetDependencies(typeId, complex.Attributes);
		}
		if (complex.Particle != null)
		{
			BuildAssetDependencies(typeId, complex.Particle);
		}
		if (complex.ContentModel != null)
		{
			BuildAssetDependencies(typeId, complex.ContentModel);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaAttribute simple)
	{
		bool flag = false;
		if (simple.AttributeSchemaType != null)
		{
			BuildAssetDependencies(typeId, simple.AttributeSchemaType);
			flag = XmlSchemaType.IsDerivedFrom(simple.AttributeSchemaType, _XmlAssetReferenceType, XmlSchemaDerivationMethod.None);
		}
		if (flag && simple.UnhandledAttributes != null)
		{
			CountReference(typeId, simple.UnhandledAttributes);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaElement element)
	{
		bool flag = false;
		if (element.ElementSchemaType != null)
		{
			BuildAssetDependencies(typeId, element.ElementSchemaType);
			flag = XmlSchemaType.IsDerivedFrom(element.ElementSchemaType, _XmlAssetReferenceType, XmlSchemaDerivationMethod.None);
		}
		if (flag && element.UnhandledAttributes != null)
		{
			CountReference(typeId, element.UnhandledAttributes);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaSimpleType simple)
	{
		if (XmlSchemaType.IsDerivedFrom(simple, _XmlAssetReferenceType, XmlSchemaDerivationMethod.None) && simple.UnhandledAttributes != null)
		{
			CountReference(typeId, simple.UnhandledAttributes);
		}
	}

	private void BuildAssetDependencies(uint typeId, XmlSchemaType type)
	{
		uint num = 0u;
		if (type.Name != null)
		{
			num = InstanceHandle.GetTypeId(type.Name);
			if (_CircularCheck.Contains(num))
			{
				return;
			}
			_CircularCheck.Add(num);
		}
		if (type.BaseXmlSchemaType != null)
		{
			BuildAssetDependencies(typeId, type.BaseXmlSchemaType);
		}
		if (type is XmlSchemaComplexType complex)
		{
			BuildAssetDependencies(typeId, complex);
		}
		else if (type is XmlSchemaSimpleType simple)
		{
			BuildAssetDependencies(typeId, simple);
		}
		else if (num != 0)
		{
			_CircularCheck.Remove(num);
		}
	}

	private void LoadSchemas(bool countDependencies)
	{
		// A failed load must not make its partially read inputs appear reusable.
		_LoadedSchemaPath = null;
		_ReferenceDescriptors = new Dictionary<ReferenceKey, ReferenceDescriptor>();
		_ReferenceDescriptorBuildCount = _ReferenceDescriptorHitCount = 0;
		_AssetDependencies = null;
		_CircularCheck = new Set<uint>();
		_SchemaPaths = new SortedDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
		_SchemaContentHashes = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		_Schemas = new XmlSchemaSet();
		_InheritanceMap = new SortedDictionary<string, StringCollection>();
		_DependencyTree = new SortedDictionary<uint, DependencyList>();
		_HashableTypes = new Dictionary<XmlSchemaType, bool>();
		try
		{
			MeasurePreparation("schemaRead", () => ReadSchema(Directory.GetCurrentDirectory(), Settings.Current.SchemaPath));
		}
		catch (Exception innerException)
		{
			throw new BinaryAssetBuilderException(innerException, ErrorCode.ReadingSchema, "Error encountered in schema '{0}'", _CurrentSchema);
		}
		MeasurePreparation("schemaCompile", _Schemas.Compile);
		MeasurePreparation("schemaMetadata", () => BuildSchemaMetadata(countDependencies));
		_LoadedSchemaPath = Path.GetFullPath(Settings.Current.SchemaPath);
	}

	private void BuildSchemaMetadata(bool countDependencies)
	{
		SetupHelperSchemaTypes();
		IDictionary<string, StringCollection> dictionary = new SortedDictionary<string, StringCollection>();
		dictionary.Add("BaseAssetType", new StringCollection());
		foreach (XmlSchemaType value2 in Schemas.GlobalTypes.Values)
		{
			if (!XmlSchemaType.IsDerivedFrom(value2, _XmlBaseAssetType, XmlSchemaDerivationMethod.None))
			{
				continue;
			}
			if (countDependencies)
			{
				uint typeId = InstanceHandle.GetTypeId(value2.Name);
				_DependencyTree[typeId] = new DependencyList(value2.Name);
				BuildAssetDependencies(typeId, value2);
			}
			if (value2.BaseXmlSchemaType != null && !string.IsNullOrEmpty(value2.BaseXmlSchemaType.Name))
			{
				StringCollection value = null;
				if (!dictionary.TryGetValue(value2.BaseXmlSchemaType.Name, out value))
				{
					value = new StringCollection();
					dictionary.Add(value2.BaseXmlSchemaType.Name, value);
				}
				value.Add(value2.Name);
			}
		}
		if (countDependencies)
		{
			_AssetDependencies = new SortedDictionary<uint, int>();
			foreach (KeyValuePair<uint, DependencyList> item in _DependencyTree)
			{
				BuildGrandchildren(item.Value);
				_AssetDependencies[item.Key] = item.Value.GrandChildren.Count;
			}
			_DependencyTree.Clear();
			_DependencyTree = null;
		}
		foreach (string key in dictionary.Keys)
		{
			FlattenInheritanceTree(key, dictionary);
		}
	}

	private void SetupHelperSchemaTypes()
	{
		_XmlBaseAssetType = Schemas.GlobalTypes[new XmlQualifiedName("BaseAssetType", "uri:ea.com:eala:asset")] as XmlSchemaType;
		_XmlBaseInheritableAsset = Schemas.GlobalTypes[new XmlQualifiedName("BaseInheritableAsset", "uri:ea.com:eala:asset")] as XmlSchemaType;
		_XmlAssetReferenceType = Schemas.GlobalTypes[new XmlQualifiedName("AssetReference", "uri:ea.com:eala:asset")] as XmlSchemaType;
		_XmlWeakReferenceType = Schemas.GlobalTypes[new XmlQualifiedName("WeakReference", "uri:ea.com:eala:asset")] as XmlSchemaType;
		_XmlFileReferenceType = Schemas.GlobalTypes[new XmlQualifiedName("FileReference", "uri:ea.com:eala:asset")] as XmlSchemaType;
		SetupHashableTypes();
	}

	private void SetupHashableTypes()
	{
		StringHashBinDescriptor[] stringHashBinDescriptors = Settings.Current.StringHashBinDescriptors;
		foreach (StringHashBinDescriptor stringHashBinDescriptor in stringHashBinDescriptors)
		{
			_HashableTypes.Add(Schemas.GlobalTypes[new XmlQualifiedName(stringHashBinDescriptor.SchemaTypeName, "uri:ea.com:eala:asset")] as XmlSchemaType, value: true);
		}
	}

	public SchemaSet(bool countDependencies)
	{
		AppContext.TryGetSwitch("NewSdk.Compiler.DisableSchemaReferenceDescriptors", out bool disabled);
		_UseReferenceDescriptors = !disabled;
		LoadSchemas(countDependencies);
	}

	public void ReloadIfChanged(bool countDependencies)
	{
		bool changed = false;
		MeasurePreparation("schemaContentCheck", () => changed = CheckSchemaChanged(countDependencies));
		if (changed) LoadSchemas(countDependencies);
	}

	private bool CheckSchemaChanged(bool countDependencies)
	{
		bool changed = countDependencies && _AssetDependencies == null;
		string requested = Path.GetFullPath(Settings.Current.SchemaPath);
		if (!string.Equals(requested, _LoadedSchemaPath, StringComparison.OrdinalIgnoreCase) ||
			_SchemaPaths == null || _SchemaContentHashes == null)
			changed = true;
		if (!changed)
		{
			foreach (KeyValuePair<string, DateTime> schemaPath in _SchemaPaths)
			{
				if (!File.Exists(schemaPath.Key) || File.GetLastWriteTime(schemaPath.Key) != schemaPath.Value)
				{
					changed = true;
					break;
				}
				using (var algorithm = SHA256.Create())
				using (var input = File.OpenRead(schemaPath.Key))
				{
					string content = Convert.ToBase64String(algorithm.ComputeHash(input));
					SchemaContentCheckCount++;
					SchemaContentBytesRead += input.Position;
					if (!string.Equals(content, _SchemaContentHashes[schemaPath.Key], StringComparison.Ordinal))
					{
						changed = true;
						break;
					}
				}
			}
		}
		return changed;
	}

	public StringCollection GetDerivedTypes(string typeName)
	{
		StringCollection value = null;
		_InheritanceMap.TryGetValue(typeName, out value);
		return value;
	}

	private bool FlattenInheritanceTree(string typeName, IDictionary<string, StringCollection> dictionary)
	{
		if (_InheritanceMap.ContainsKey(typeName))
		{
			return true;
		}
		StringCollection value = null;
		if (!dictionary.TryGetValue(typeName, out value))
		{
			return false;
		}
		StringCollection stringCollection = new StringCollection();
		StringEnumerator enumerator = value.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				stringCollection.Add(current);
				bool flag = true;
				if (!_InheritanceMap.ContainsKey(current))
				{
					flag = FlattenInheritanceTree(current, dictionary);
				}
				if (!flag)
				{
					continue;
				}
				StringEnumerator enumerator2 = _InheritanceMap[current].GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.Current;
						stringCollection.Add(current2);
					}
				}
				finally
				{
					if (enumerator2 is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable2)
			{
				disposable2.Dispose();
			}
		}
		_InheritanceMap.Add(typeName, stringCollection);
		return true;
	}
}
