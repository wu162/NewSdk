using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Xml;

namespace BinaryAssetBuilder.Core;

public static class AssetReport
{
	private const string DefaultFileName = "AssetReport.xml";

	public static string FileName { get => CurrentState.FileName; set => CurrentState.FileName = value; }

	public static string FileNameFullyQualified { get => CurrentState.Path; set => CurrentState.Path = value; }

	public static readonly string TypeName = "AssetReportTable";

	private static XmlWriter AssetRecordWriter { get => CurrentState.Writer; set => CurrentState.Writer = value; }

	private static Stream AssetRecordStream { get => CurrentState.Stream; set => CurrentState.Stream = value; }

	private static IDictionary<string, bool> RecordedAssets { get => CurrentState.Assets; set => CurrentState.Assets = value; }

	internal sealed class State
	{
		internal readonly object Gate = new object();
		internal XmlWriter Writer;
		internal Stream Stream;
		internal IDictionary<string, bool> Assets;
		internal string FileName = DefaultFileName;
		internal string Path;
		internal ReportScope Scope;
		internal bool Closing;
	}

	private static State CurrentState => CompilationContext.Current.AssetReport;
	private static object ScopeGate => CurrentState.Gate;
	private static ReportScope CurrentScope { get => CurrentState.Scope; set => CurrentState.Scope = value; }
	private static bool Closing { get => CurrentState.Closing; set => CurrentState.Closing = value; }

	// Scopes own one context's sequential stream state. Separate contexts can
	// keep report writers open concurrently without sharing writer or scope state.
	public static IDisposable BeginScope()
	{
		lock (ScopeGate)
		{
			EnsureScopeThread();
			if (Closing) throw new InvalidOperationException("Cannot enter an asset report scope during Close.");
			var scope = new ReportScope();
			AssetRecordWriter = null;
			AssetRecordStream = null;
			RecordedAssets = null;
			FileNameFullyQualified = null;
			FileName = DefaultFileName;
			CurrentScope = scope;
			return scope;
		}
	}

	private static void EnsureScopeThread()
	{
		if (CurrentScope != null && CurrentScope.ThreadId != System.Threading.Thread.CurrentThread.ManagedThreadId)
			throw new InvalidOperationException("Asset report scopes may only be used on their owning thread.");
	}

	internal sealed class ReportScope : IDisposable
	{
		private readonly State state = CurrentState;
		internal readonly int ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		private readonly ReportScope parent = CurrentScope;
		private readonly XmlWriter writer = AssetRecordWriter;
		private readonly Stream stream = AssetRecordStream;
		private readonly IDictionary<string, bool> assets = RecordedAssets;
		private readonly string fileName = FileName;
		private readonly string path = FileNameFullyQualified;
		private bool disposed;

		public void Dispose()
		{
			XmlWriter childWriter;
			Stream childStream;
			lock (state.Gate)
			{
				if (disposed) return;
				if (!Object.ReferenceEquals(CurrentState, state))
					throw new InvalidOperationException("Asset report scopes must be disposed in their creating compilation context.");
				EnsureScopeThread();
				if (Closing) throw new InvalidOperationException("Cannot dispose an asset report scope during Close.");
				if (!Object.ReferenceEquals(CurrentScope, this))
					throw new InvalidOperationException("Asset report scopes must be disposed in reverse creation order.");
				Closing = true;
				childWriter = AssetRecordWriter;
				childStream = AssetRecordStream;
				ClearWriterState();
			}
			try { CloseWriter(childWriter, childStream); }
			finally
			{
				lock (ScopeGate)
				{
					AssetRecordWriter = writer;
					AssetRecordStream = stream;
					RecordedAssets = assets;
					FileName = fileName;
					FileNameFullyQualified = path;
					CurrentScope = parent;
					disposed = true;
					Closing = false;
				}
			}
		}
	}

	private static void ClearWriterState()
	{
		AssetRecordWriter = null;
		AssetRecordStream = null;
		FileNameFullyQualified = null;
		RecordedAssets = null;
	}

	public static void RecordAsset(InstanceDeclaration instance, BinaryAsset asset)
	{
		if (instance.Handle == null)
		{
			throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Asset report failure");
		}
		if (!(instance.Handle.TypeName == TypeName) && !(instance.Handle.TypeName == "StringHashTable"))
		{
			if (AssetRecordWriter == null)
			{
				InitAssetRecordWriter();
			}
			RecordAsset(instance.Handle.InstanceName, instance.Handle.TypeName, asset.InstanceFileSize, instance.ValidatedReferencedInstances);
		}
	}

	private static void RecordAsset(string id, string type, int size, IList<InstanceHandle> references)
	{
		string key = MakeAssetReportId(type, id);
		if (RecordedAssets.ContainsKey(key))
		{
			return;
		}
		RecordedAssets.Add(key, value: true);
		AssetRecordWriter.WriteStartElement("AssetReport", "uri:ea.com:eala:asset");
		AssetRecordWriter.WriteAttributeString("Id", id.ToLower());
		AssetRecordWriter.WriteAttributeString("Type", type);
		AssetRecordWriter.WriteAttributeString("AssetSize", Convert.ToString(size));
		if (references != null)
		{
			foreach (InstanceHandle reference in references)
			{
				AssetRecordWriter.WriteStartElement("Reference");
				AssetRecordWriter.WriteAttributeString("Id", reference.InstanceName.ToLower());
				AssetRecordWriter.WriteAttributeString("Type", reference.TypeName);
				AssetRecordWriter.WriteEndElement();
			}
		}
		AssetRecordWriter.WriteEndElement();
	}

	private static void InitAssetRecordWriter()
	{
		XmlWriter writer = null;
		Stream output = null;
		FileNameFullyQualified = null;
		RecordedAssets = null;
		try
		{
			if (!Directory.Exists(Settings.Current.SessionCacheDirectory))
				Directory.CreateDirectory(Settings.Current.SessionCacheDirectory);
			string path = Path.Combine(Settings.Current.SessionCacheDirectory, FileName);
			output = new FileStream(path, FileMode.Create);
			output = new BufferedStream(output);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CloseOutput = true;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "\t";
			writer = XmlWriter.Create(output, xmlWriterSettings);
			writer.WriteStartElement("AssetDeclaration", "uri:ea.com:eala:asset");
			writer.WriteStartElement(TypeName);
			writer.WriteAttributeString("id", "TheAssetReportTable");
			RecordedAssets = new SortedDictionary<string, bool>();
			FileNameFullyQualified = path;
			AssetRecordStream = output;
			AssetRecordWriter = writer;
		}
		catch
		{
			try { if (writer != null) writer.Close(); } catch { }
			try { if (output != null) output.Dispose(); } catch { }
			throw;
		}
	}

	public static string MakeAssetReportId(string typeId, string instanceId)
	{
		return (typeId + "." + instanceId + ".AssetReport").ToLower();
	}

	public static void Close()
	{
		XmlWriter writer;
		Stream stream;
		lock (ScopeGate)
		{
			EnsureScopeThread();
			if (Closing) throw new InvalidOperationException("Asset report Close cannot be reentered.");
			Closing = true;
			writer = AssetRecordWriter;
			stream = AssetRecordStream;
			ClearWriterState();
		}
		try { CloseWriter(writer, stream); }
		finally { lock (ScopeGate) { Closing = false; } }
	}

	private static void CloseWriter(XmlWriter writer, Stream stream)
	{
		Exception failure = null;
		try
		{
			if (writer != null)
			{
				writer.WriteEndDocument();
				writer.Flush();
			}
		}
		catch (Exception e) { failure = e; }
		finally
		{
			try { if (writer != null) writer.Close(); }
			catch (Exception e) { if (failure == null) failure = e; }
			try { if (stream != null) stream.Dispose(); }
			catch (Exception e) { if (failure == null) failure = e; }
		}
		if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
	}

	public static void WriteAssetReportInclude(XmlWriter writer)
	{
		writer.WriteStartElement("Includes");
		writer.WriteStartElement("Include");
		writer.WriteAttributeString("type", "all");
		writer.WriteAttributeString("source", FileName);
		writer.WriteEndElement();
		writer.WriteEndElement();
	}
}
