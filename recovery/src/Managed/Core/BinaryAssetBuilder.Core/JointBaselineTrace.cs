using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.Core;

// Opt-in B0 evidence at the production call sites. Trace failures intentionally
// fail the diagnostic run instead of silently producing incomplete evidence.
internal static class JointBaselineTrace
{
	private static readonly string Root = Environment.GetEnvironmentVariable("RA3_JOINT_BASELINE_TRACE");
	private static readonly object Sync = new object();
	private static readonly Dictionary<string, bool> SeenAssemblies = new Dictionary<string, bool>();
	private static string DirectoryPath;

	private static bool Enabled => !string.IsNullOrEmpty(Root);

	private static void Initialize()
	{
		if (DirectoryPath != null) return;
		DirectoryPath = Path.Combine(Root, Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
		Directory.CreateDirectory(DirectoryPath);
		Write("assemblies.tsv", "name", "fullname", "location", "sha256");
		Write("calls.tsv", "type", "id", "pluginType", "pluginAssembly", "binlen", "relolen", "implen");
		Write("xml.tsv", "path", "reader");
		Write("xinclude.tsv", "path", "sourceUri", "targetPath");
		Write("verifiers.tsv", "type", "id", "pluginType", "pluginAssembly");
	}

	private static void Write(string file, params string[] fields)
	{
		string[] escaped = new string[fields.Length];
		for (int i = 0; i < fields.Length; i++)
		{
			escaped[i] = (fields[i] ?? "").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");
		}
		using (StreamWriter writer = new StreamWriter(Path.Combine(DirectoryPath, file), true, new UTF8Encoding(false)))
		{
			writer.WriteLine(string.Join("\t", escaped));
		}
	}

	private static void SnapshotAssemblies()
	{
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			string location = assembly is AssemblyBuilder ? "" : assembly.Location;
			string key = assembly.FullName + "\t" + location;
			if (SeenAssemblies.ContainsKey(key)) continue;
			string hash = "";
			if (!string.IsNullOrEmpty(location))
			{
				using (FileStream stream = File.OpenRead(location))
				using (SHA256Managed algorithm = new SHA256Managed())
				{
					hash = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
				}
			}
			Write("assemblies.tsv", assembly.GetName().Name, assembly.FullName, location, hash);
			SeenAssemblies.Add(key, true);
		}
	}

	internal static void XmlLoaded(string path, XmlReader reader, FileNameXmlResolver resolver)
	{
		if (!Enabled) return;
		lock (Sync)
		{
			Initialize();
			Write("xml.tsv", path, reader.GetType().AssemblyQualifiedName);
			foreach (PathMapItem item in resolver.Paths)
			{
				Write("xinclude.tsv", path, item.SourceUri, item.TargetPath);
			}
			SnapshotAssemblies();
		}
	}

	private static string Length(byte[] data)
	{
		return data == null ? "null" : data.Length.ToString(CultureInfo.InvariantCulture);
	}

	internal static void Processed(InstanceDeclaration instance, IAssetBuilderPlugin plugin, AssetBuffer buffer)
	{
		if (!Enabled) return;
		lock (Sync)
		{
			Initialize();
			Type type = plugin.GetType();
			Write("calls.tsv", instance.Handle.TypeName, instance.Handle.InstanceName, type.FullName, type.Assembly.FullName,
				buffer == null ? "null" : Length(buffer.InstanceData),
				buffer == null ? "null" : Length(buffer.RelocationData),
				buffer == null ? "null" : Length(buffer.ImportsData));
			SnapshotAssemblies();
		}
	}

	internal static void Verified(InstanceDeclaration instance, IAssetBuilderVerifierPlugin plugin)
	{
		if (!Enabled) return;
		lock (Sync)
		{
			Initialize();
			Type type = plugin.GetType();
			Write("verifiers.tsv", instance.Handle.TypeName, instance.Handle.InstanceName, type.FullName, type.Assembly.FullName);
			SnapshotAssemblies();
		}
	}
}
