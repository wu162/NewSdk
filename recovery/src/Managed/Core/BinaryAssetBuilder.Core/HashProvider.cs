using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using EALA.Hash;

namespace BinaryAssetBuilder.Core;

public static class HashProvider
{
	private static Tracer _Tracer;

	private static readonly string StringHashesFileDefault;

	public static string StringHashesFile;

	private static IDictionary<string, string> _SchemaNameToBinName;

	private static IDictionary<string, StringHashBin> _StringHashBins;

	private static readonly string InstanceIdTableName;

	private static readonly string TypeIdTableName;

	public static readonly string StringTableAssetName;

	public static readonly string StringBinEnumAttribute;

	private static readonly uint HashProviderVersion;

	private static string _outputDirectory;

    private static readonly object ScopeGate = new object();
    private static StringHashScope _CurrentScope;
    private static bool _PreparingState;

    // Core uses process-wide hash tables. A stream owns them sequentially; nested
    // scopes restore their exact parent tables, never publish output on disposal.
    public static IDisposable BeginStringHashScope(string outputDirectory)
    {
        return PrepareStringHashes(outputDirectory, true);
    }

    private static void EnsureScopeThread()
    {
        if (_CurrentScope != null && _CurrentScope.ThreadId != System.Threading.Thread.CurrentThread.ManagedThreadId)
            throw new InvalidOperationException("String hash scopes may only be used on their owning thread.");
    }

    private sealed class StringHashScope : IDisposable
    {
        internal readonly int ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        private readonly StringHashScope parent = _CurrentScope;
        private readonly IDictionary<string, string> schemaNames = _SchemaNameToBinName;
        private readonly IDictionary<string, StringHashBin> bins = _StringHashBins;
        private readonly string directory = _outputDirectory;
        private readonly string fileName = StringHashesFile;
        private bool disposed;

        public void Dispose()
        {
            lock (ScopeGate)
            {
                if (disposed) return;
                EnsureScopeThread();
                if (_PreparingState) throw new InvalidOperationException("Cannot dispose a string hash scope during initialization.");
                if (!Object.ReferenceEquals(_CurrentScope, this))
                    throw new InvalidOperationException("String hash scopes must be disposed in reverse creation order.");
                _SchemaNameToBinName = schemaNames;
                _StringHashBins = bins;
                _outputDirectory = directory;
                StringHashesFile = fileName;
                _CurrentScope = parent;
                disposed = true;
            }
        }
    }

	public static string GetOutputDirectory()
	{
		return _outputDirectory;
	}

	static HashProvider()
	{
		_Tracer = Tracer.GetTracer("HashProvider", "Centralized facility to produce hashes from strings");
		StringHashesFileDefault = "StringHashes.xml";
		StringHashesFile = StringHashesFileDefault;
		InstanceIdTableName = "INSTANCEID";
		TypeIdTableName = "TYPEID";
		StringTableAssetName = "StringHashTable";
		StringBinEnumAttribute = "StringHashBin";
		HashProviderVersion = GetTextHash(StringTableAssetName) ^ 2;
	}

	public static void RecordHash(XmlSchemaType type, string str)
	{
		string value = null;
		if (_SchemaNameToBinName.TryGetValue(type.Name, out value))
		{
			RecordHash(value, str);
			return;
		}
		throw new BinaryAssetBuilderException(ErrorCode.InternalError, "StringHashBin does not exist for Schema type {0} but hashing was requested", type.Name);
	}

	public static void RecordHash(InstanceHandle handle)
	{
		_StringHashBins[InstanceIdTableName].RecordStringHash(handle.InstanceId, handle.InstanceName);
		_StringHashBins[TypeIdTableName].RecordStringHash(handle.TypeId, handle.TypeName);
	}

	public static void RecordHash(string binName, string str)
	{
		StringHashBin stringHashBin = _StringHashBins[binName];
		stringHashBin.RecordStringHash(GetTextHash(stringHashBin.IsCaseSensitive() ? str : str.ToLower()), str);
	}

    public static void InitializeStringHashes(string outputDir)
    {
        PrepareStringHashes(outputDir, false);
    }

    private static IDisposable PrepareStringHashes(string outputDirectory, bool createScope)
    {
        lock (ScopeGate)
        {
            EnsureScopeThread();
            if (_PreparingState) throw new InvalidOperationException("String hash initialization cannot be concurrent or reentered.");
            _PreparingState = true;
        }
        try
        {
            // IO and trace callbacks run outside ScopeGate. Reentrant or worker
            // initialization is rejected by the marker instead of waiting for
            // a callback which may itself be waiting for that worker.
            var schemaNames = new Dictionary<string, string>();
            var bins = new Dictionary<string, StringHashBin>();
            bins.Add(InstanceIdTableName, new StringHashBin(InstanceIdTableName, caseSensitive: false));
            bins.Add(TypeIdTableName, new StringHashBin(TypeIdTableName, caseSensitive: true));
            foreach (StringHashBinDescriptor descriptor in Settings.Current.StringHashBinDescriptors)
            {
                schemaNames.Add(descriptor.SchemaTypeName, descriptor.BinName);
                bins.Add(descriptor.BinName, new StringHashBin(descriptor.BinName, descriptor.CaseSensitivity));
            }
            LoadPreviousStringHashes(outputDirectory, StringHashesFileDefault, bins);
            lock (ScopeGate)
            {
                // Snapshot only when the complete candidate is ready to install.
                var scope = createScope ? new StringHashScope() : null;
                _outputDirectory = outputDirectory;
                _SchemaNameToBinName = schemaNames;
                _StringHashBins = bins;
                StringHashesFile = StringHashesFileDefault;
                if (scope != null) _CurrentScope = scope;
                return scope;
            }
        }
        finally { lock (ScopeGate) { _PreparingState = false; } }
    }

    private static void LoadPreviousStringHashes(string outputDirectory, string fileName, IDictionary<string, StringHashBin> bins)
    {
        string path = Path.Combine(outputDirectory, fileName);
        if (!File.Exists(path))
        {
            path = Path.Combine(outputDirectory, StringHashesFileDefault);
            if (!File.Exists(path)) return;
        }
        CacheInputBoundary.ReportUnknownRead(path);
        _Tracer.TraceInfo("Loading previous string hash file from {0}", path);
        XmlDocument document = new XmlDocument();
        document.Load(path);
        try
        {
            foreach (XmlNode child in document.DocumentElement.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element || !child.Name.Equals(StringTableAssetName)) continue;
                XmlElement element = (XmlElement)child;
                StringHashBin bin;
                if (!bins.TryGetValue(element.Attributes[StringBinEnumAttribute].Value, out bin)) continue;
                if (Convert.ToUInt32(element.Attributes["Version"].Value) == HashProviderVersion)
                    bin.ReadStringHashTable(element);
                else
                    _Tracer.TraceInfo("Old string hash file is out of date.  If you encounter missing strings in the game (such as missing id's), please delete the session cache file in {0} and rebuild", Settings.Current.SessionCacheDirectory);
            }
        }
        catch (Exception exception)
        {
            _Tracer.TraceWarning("Did not successfully initialize previous string hashes, exception message: {0}", exception.ToString());
        }
    }

	public static void FinalizeStringHashes()
	{
		if (!Directory.Exists(_outputDirectory))
		{
			Directory.CreateDirectory(_outputDirectory);
		}
		string outputFileName = Path.Combine(_outputDirectory, StringHashesFileDefault);
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.CloseOutput = true;
		xmlWriterSettings.Indent = true;
		Stream output = null;
		XmlWriter xmlWriter = null;
		Exception failure = null;
		try
		{
			// Own the stream separately so a failing writer Close cannot retain its handle.
			output = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
			xmlWriter = XmlWriter.Create(output, xmlWriterSettings);
			xmlWriter.WriteStartElement("AssetDeclaration", "uri:ea.com:eala:asset");
			if (Settings.Current.OutputAssetReport)
			{
				AssetReport.WriteAssetReportInclude(xmlWriter);
			}
			foreach (KeyValuePair<string, StringHashBin> stringHashBin in _StringHashBins)
			{
				xmlWriter.WriteStartElement(StringTableAssetName);
				xmlWriter.WriteAttributeString("Version", HashProviderVersion.ToString());
				stringHashBin.Value.WriteStringHashTable(xmlWriter);
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
		}
		catch (Exception error) { failure = error; }
		finally
		{
			try { if (xmlWriter != null) xmlWriter.Close(); }
			catch (Exception error) { if (failure == null) failure = error; }
			try { if (output != null) output.Dispose(); }
			catch (Exception error) { if (failure == null) failure = error; }
		}
		if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
	}

	public static uint GetTypeHash(Type type)
	{
		return GetTypeHash((uint)type.FullName.Length, type);
	}

	public static uint GetTypeHash(uint combine, Type type)
	{
		return FastHash.GetHashCode(FastHash.GetHashCode(combine, type.FullName), type.Module.ModuleVersionId.ToByteArray());
	}

	public static uint GetCaseInsensitiveSymbolHash(string symbol)
	{
		return FastHash.GetHashCode(symbol.ToLower());
	}

	public static uint GetCaseSensitiveSymbolHash(string symbol)
	{
		return FastHash.GetHashCode(symbol);
	}

	public static uint GetTextHash(string text)
	{
		return FastHash.GetHashCode(text);
	}

	public static uint GetTextHash(uint hash, string text)
	{
		return FastHash.GetHashCode(hash, text);
	}

	public static uint GetDataHash(byte[] data)
	{
		return FastHash.GetHashCode(data);
	}

	public static uint GetDataHash(uint hash, byte[] data)
	{
		return FastHash.GetHashCode(hash, data);
	}

	public static uint GetXmlHash(uint hash, ref XmlNode node)
	{
		HashingWriter hashingWriter = new HashingWriter(hash);
		XmlWriter xmlWriter = XmlWriter.Create(hashingWriter);
		node.WriteTo(xmlWriter);
		xmlWriter.Flush();
		hashingWriter.Flush();
		return hashingWriter.GetFinalHash();
	}

	public static uint GetXmlHash(ref XmlNode node)
	{
		return GetXmlHash(0u, ref node);
	}
}
