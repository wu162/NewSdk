using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace BinaryAssetBuilder.Core;

// Opt-in boundary for the content-addressed /bc namespace. Normal Core builds
// retain their existing resolver contract. Observation records actual candidate
// decisions and consumed bytes; it does not by itself certify a complete closure.
public static class CacheInputBoundary
{
	[ThreadStatic] private static string[] _Roots;
	[ThreadStatic] private static ObservationScope _Observation;

	// The owner supplies a stream containing exactly the bytes whose identity it
	// records. Never hash a path here and then let the consumer reopen that path.
	// This scope is deliberately separate from Begin: recording does not remove
	// the conservative root protection or certify native/legacy readers.
	public static IDisposable BeginObservation(Action<string, bool> probe, Func<string, Stream> openRead, Action<string> unknownRead)
	{
		return BeginObservation(probe, openRead, unknownRead, null);
	}

	public static IDisposable BeginObservation(Action<string, bool> probe, Func<string, Stream> openRead, Action<string> unknownRead, Action<string, long, byte[]> leasedRead)
	{
		if (_Observation != null) throw new InvalidOperationException("A cache input observation is already active.");
		if (probe == null || openRead == null || unknownRead == null) throw new ArgumentNullException("Cache input observation callbacks are required.");
		_Observation = new ObservationScope(probe, openRead, unknownRead, leasedRead);
		return _Observation;
	}

	public static bool IsObserving => _Observation != null;

	public static bool FileExists(string path)
	{
		Validate(path);
		bool exists = File.Exists(path);
		if (_Observation != null) _Observation.Probe(Path.GetFullPath(path), exists);
		return exists;
	}

	public static Stream OpenRead(string path)
	{
		Validate(path);
		if (_Observation == null) return File.OpenRead(path);
		Stream input = _Observation.OpenRead(Path.GetFullPath(path));
		if (input == null) throw new IOException("Cache input observer did not provide a stream.");
		if (!input.CanRead)
		{
			input.Dispose();
			throw new IOException("Cache input observer provided an unreadable stream.");
		}
		return input;
	}

	public static byte[] ReadAllBytes(string path)
	{
		Validate(path);
		if (_Observation == null) return File.ReadAllBytes(path);
		using (Stream input = OpenRead(path))
		using (var output = new MemoryStream())
		{
			byte[] buffer = new byte[8192];
			int count;
			while ((count = input.Read(buffer, 0, buffer.Length)) != 0) output.Write(buffer, 0, count);
			return output.ToArray();
		}
	}

	public static XmlReader CreateXmlReader(string path)
	{
		if (_Observation == null) return XmlReader.Create(path);
		Stream input = OpenRead(path);
		try { return XmlReader.Create(input, new XmlReaderSettings { CloseInput = true }, new Uri(Path.GetFullPath(path), UriKind.Absolute).AbsoluteUri); }
		catch { input.Dispose(); throw; }
	}

	// Legacy/output-side or native readers cannot be inferred from the Core
	// file-reference set. Their use makes this observation incomplete until the
	// corresponding reader receives its own certified input contract.
	public static void ReportUnknownRead(string path)
	{
		if (_Observation != null) _Observation.UnknownRead(path);
	}

	// A certified preparer already holds this exact input open across identity
	// generation and backend consumption. Reuse its digest without a second
	// path open or read; the observer still cannot grant cache/worker eligibility.
	public static void ReportLeasedRead(string path, long length, byte[] digest)
	{
		if (_Observation == null) return;
		Validate(path);
		string full = Path.GetFullPath(path);
		if (_Observation.LeasedRead == null)
		{
			_Observation.UnknownRead("leased-input-not-recorded:" + full);
			return;
		}
		if (length < 0 || digest == null || digest.Length != 32)
			throw new IOException("A leased cache input requires a valid length and SHA256 identity.");
		_Observation.LeasedRead(full, length, digest);
	}

	sealed class ObservationScope : IDisposable
	{
		internal readonly Action<string, bool> Probe;
		internal readonly Func<string, Stream> OpenRead;
		internal readonly Action<string> UnknownRead;
		internal readonly Action<string, long, byte[]> LeasedRead;
		readonly int thread = Thread.CurrentThread.ManagedThreadId;
		bool disposed;
		internal ObservationScope(Action<string, bool> probe, Func<string, Stream> openRead, Action<string> unknownRead, Action<string, long, byte[]> leasedRead)
		{ Probe = probe; OpenRead = openRead; UnknownRead = unknownRead; LeasedRead = leasedRead; }
		public void Dispose()
		{
			if (disposed) return;
			if (thread != Thread.CurrentThread.ManagedThreadId || !Object.ReferenceEquals(_Observation, this))
				throw new InvalidOperationException("Cache input observation must close on its owning thread.");
			_Observation = null; disposed = true;
		}
	}

	public static IDisposable Begin(params string[] roots)
	{
		if (_Roots != null) throw new InvalidOperationException("A cache input boundary is already active.");
		if (roots == null || roots.Length == 0) throw new ArgumentException("Cache input roots are required.");
		var normalized = new string[roots.Length];
		for (int index = 0; index < roots.Length; index++)
		{
			normalized[index] = Path.GetFullPath(roots[index]).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			if (!Directory.Exists(normalized[index])) throw new DirectoryNotFoundException(normalized[index]);
			RejectReparseAncestors(normalized[index]);
		}
		_Roots = normalized;
		return new Scope(Thread.CurrentThread.ManagedThreadId);
	}

	public static void Validate(string path)
	{
		if (_Roots == null) return;
		if (string.IsNullOrEmpty(path) || !Path.IsPathRooted(path)) throw new IOException("Build-cache inputs must be absolute physical paths.");
		string full = Path.GetFullPath(path);
		// An ADS is not an enumerated file in the root inventory. Device and
		// extended paths also have normalization rules outside that contract.
		if (full.StartsWith(@"\\?\", StringComparison.Ordinal) || full.StartsWith(@"\\.\", StringComparison.Ordinal) ||
			full.IndexOf(':', full.Length >= 2 && full[1] == ':' ? 2 : 0) >= 0)
			throw new IOException("Build-cache input uses device syntax or an alternate data stream: " + full);
		bool included = false;
		foreach (string root in _Roots)
			if (string.Equals(root, full, StringComparison.OrdinalIgnoreCase) || full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) { included = true; break; }
		if (!included) throw new IOException("Build-cache input is outside the inventoried SDK, MOD and compiler runtime roots: " + full);
		RejectReparseAncestors(full);
	}

	static void RejectReparseAncestors(string path)
	{
		if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new IOException("Build-cache inputs cannot use reparse points.");
		for (var current = new DirectoryInfo(path); current != null; current = current.Parent)
			if (current.Exists && (current.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Build-cache inputs cannot use reparse points.");
	}
	sealed class Scope : IDisposable
	{
		readonly int thread; bool disposed;
		public Scope(int thread) { this.thread = thread; }
		public void Dispose()
		{
			if (disposed) return;
			if (thread != Thread.CurrentThread.ManagedThreadId) throw new InvalidOperationException("Cache input boundary must close on its owning thread.");
			_Roots = null; disposed = true;
		}
	}
}
