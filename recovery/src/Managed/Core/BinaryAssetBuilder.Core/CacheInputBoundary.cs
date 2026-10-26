using System;
using System.IO;
using System.Threading;

namespace BinaryAssetBuilder.Core;

// Opt-in boundary for the content-addressed /bc namespace. Normal Core builds
// retain their existing resolver contract. Cached builds must inventory every
// candidate root, not merely the path that happened to win a search this time.
public static class CacheInputBoundary
{
	[ThreadStatic] private static string[] _Roots;

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
