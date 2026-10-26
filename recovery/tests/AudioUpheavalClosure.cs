using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

// Test-only input audit. Uses the SDK's original path resolver and XInclude
// reader; never calls the audio compiler or derives expectations from traces.
internal static class AudioUpheavalClosure
{
    private const string Ns = "uri:ea.com:eala:asset";
    private sealed class Work { internal string Path; internal bool Required; }
    private sealed class Audio {
        internal string Type, Id;
        internal bool Required;
        internal readonly HashSet<string> Sources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
    private static readonly HashSet<string> AudioTypes = new HashSet<string>(new[] {
        "AudioFile", "AudioFileMP3Passthrough", "PathMusicMap", "PathMusicTrack", "PathMusicEvent" });
    private static string sdk, output;
    private static string Relative(string path) { return path.Substring(sdk.Length + 1).Replace('\\', '/'); }
    private static string Resolve(string parent, string source)
    {
        string path = FileNameResolver.ResolvePath(Path.GetDirectoryName(parent), source.Trim().ToLowerInvariant());
        if (String.IsNullOrEmpty(path) || !File.Exists(path)) throw new Exception("Missing include: " + source + " in " + Relative(parent));
        path = Path.GetFullPath(path);
        if (!path.StartsWith(sdk + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new Exception("Include escapes SDK: " + path);
        return path;
    }
    private static void InventoryRoots()
    {
        // Raw XML inventory only: XInclude fragments are valid inventory rows,
        // not independent compile inputs. The actual closure below still uses
        // the original XInclude reader and applies its own strict root gate.
        var files = new List<string>();
        foreach (string directory in new[] { Path.Combine(sdk, "Art"), Path.Combine(sdk, "Mods/Upheaval") })
            files.AddRange(Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Where(p =>
                String.Equals(Path.GetExtension(p), ".xml", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Path.GetExtension(p), ".w3x", StringComparison.OrdinalIgnoreCase)));
        var rows = new List<object>(); var errors = new List<object>(); var unusual = new List<object>();
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (string file in files.OrderBy(p => p, StringComparer.Ordinal)) {
            string name = null, ns = null, parseError = null; int audio = 0, includes = 0;
            try {
                using (XmlReader reader = XmlReader.Create(file)) while (reader.Read()) {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (reader.Depth == 0) { name = reader.LocalName; ns = reader.NamespaceURI; }
                    if (AudioTypes.Contains(reader.LocalName)) audio++;
                    if (reader.LocalName == "Include" || reader.LocalName == "include") includes++;
                }
                if (name == null) throw new Exception("No root element");
            } catch (Exception error) { parseError = error.Message; }
            var row = new { path = Relative(file), root = name, namespaceUri = ns, audioElements = audio, includeElements = includes, parseError = parseError };
            rows.Add(row);
            if (parseError != null) errors.Add(row);
            else {
                string key = ns + "\t" + name; int count; counts.TryGetValue(key, out count); counts[key] = count + 1;
                if (name != "AssetDeclaration" || (ns.Length != 0 && ns != Ns)) unusual.Add(row);
            }
        }
        var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
        File.WriteAllText(Path.Combine(output, "xml-root-inventory.json"), serializer.Serialize(rows));
        File.WriteAllText(Path.Combine(output, "xml-root-inventory-summary.json"), serializer.Serialize(new {
            files = rows.Count, rootsByNamespaceAndName = counts, otherRoots = unusual, parseErrors = errors,
            semantics = "Raw SDK Art and complete pinned MOD XML/W3X inventory; fragment roots and unreferenced parse errors do not determine compile closure"
        }));
        Console.WriteLine("XML inventory: " + rows.Count + " files, " + counts.Count + " root/namespace shapes, " + errors.Count + " parse errors");
    }
    private static void Configure()
    {
        // Same no-configuration case3/4/5 command paths. SettingsLoader applies
        // the original normalization and clears Postfix for the default config.
        Environment.CurrentDirectory = sdk;
        Settings.Current = new Settings();
        Settings.Current.DataRoot = sdk;
        Settings.Current.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        Settings.Current.InputPath = Path.Combine(sdk, "Mods/Upheaval/Data/Mod.xml");
        Settings.Current.OutputDirectory = Path.Combine(sdk, "builtmods");
        Settings.Current.IntermediateOutputDirectory = Settings.Current.OutputDirectory;
        Settings.Current.DefaultDataPaths = @".;.\Mods\Upheaval\Data;.\Mods;.\SageXml";
        Settings.Current.DefaultArtPaths = @".;.\Mods\Upheaval\Art;.\Mods;.\Art";
        Settings.Current.DefaultAudioPaths = @".;.\Mods\Upheaval\Audio;.\Mods;.\Audio";
        Settings.Current.TargetPlatform = TargetPlatform.Win32;
        Settings.Current.SessionCache = false;
        Settings.Current.BuildCache = false;
        Settings.Current.LinkedStreams = true;
        SettingsLoader.PostProcessSettings(sdk);
        XIncludingReaderWrapper.LoadAssembly();
    }
    private static int Main(string[] args)
    {
        try {
            if (args.Length != 2 || IntPtr.Size != 4) throw new Exception("Usage (x86): AudioUpheavalClosure sdk output");
            sdk = Path.GetFullPath(args[0]).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            output = Path.GetFullPath(args[1]); Directory.CreateDirectory(output);
            Configure();
            InventoryRoots();
            string data = Path.Combine(sdk, "Mods/Upheaval/Data");
            var roots = new List<string>();
            roots.AddRange(Directory.GetFiles(Path.Combine(data, "AptUI"), "*.xml").OrderBy(x => x, StringComparer.Ordinal));
            roots.AddRange(Directory.GetFiles(Path.Combine(data, "AdditionalMaps"), "mapmetadata_*.xml").OrderBy(x => x, StringComparer.Ordinal));
            roots.Add(Path.Combine(data, "Mod.xml"));
            if (roots.Count != 5) throw new Exception("Pinned case3/4/5 must have exactly five input XMLs");
            var queue = new Queue<Work>(); foreach (string root in roots) queue.Enqueue(new Work { Path = root, Required = true });
            var visited = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var documents = new Dictionary<string, XmlDocument>(StringComparer.OrdinalIgnoreCase);
            var assets = new Dictionary<string, Audio>(StringComparer.Ordinal);
            var edges = new List<object>(); var xincludes = new List<object>();
            var emptyDeclarations = new List<string>();
            var kindCounts = new Dictionary<string, int>(); var aliasCounts = new Dictionary<string, int>();
            int unsupportedAudioDeclarations = 0;
            while (queue.Count > 0) {
                Work work = queue.Dequeue(); string path = Path.GetFullPath(work.Path);
                int mode = work.Required ? 2 : 1, modes; visited.TryGetValue(path, out modes);
                if ((modes & mode) != 0) continue;
                visited[path] = modes | mode;
                XmlDocument doc;
                if (!documents.TryGetValue(path, out doc)) {
                    doc = new XmlDocument();
                    FileNameXmlResolver resolver = FileNameResolver.GetXmlResolver(Path.GetDirectoryName(path));
                    using (XmlReader source = XmlReader.Create(path))
                    using (XmlReader expanded = XIncludingReaderWrapper.GetReader(source, resolver)) doc.Load(expanded);
                    // The pinned SDK contains empty, namespace-free AssetDeclaration
                    // placeholders (principally W3D art). Original Core gathers
                    // only /ea:AssetDeclaration/* and gives these zero instances;
                    // it does not inject an asset namespace. Accept only the
                    // exact empty shape, never discard an unknown populated root.
                    XmlElement documentRoot = doc.DocumentElement;
                    bool emptyPlaceholder = documentRoot != null && documentRoot.Name == "AssetDeclaration" &&
                        documentRoot.NamespaceURI.Length == 0 && documentRoot.Attributes.Count == 0 && !documentRoot.HasChildNodes;
                    bool assetDeclaration = documentRoot != null && documentRoot.LocalName == "AssetDeclaration" && documentRoot.NamespaceURI == Ns;
                    if (!assetDeclaration && !emptyPlaceholder)
                        throw new Exception("Included XML root requires explicit audit: " + Relative(path) + " root=" +
                            (documentRoot == null ? "<absent>" : documentRoot.Name + " namespace=" + documentRoot.NamespaceURI));
                    if (emptyPlaceholder) emptyDeclarations.Add(Relative(path));
                    foreach (PathMapItem map in resolver.Paths)
                        xincludes.Add(new { document = Relative(path), source = map.SourceUri, physical = Relative(map.TargetPath) });
                    documents.Add(path, doc);
                }
                foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
                    XmlElement asset = node as XmlElement;
                    if (asset == null || asset.NamespaceURI != Ns || !AudioTypes.Contains(asset.LocalName)) continue;
                    string id = asset.GetAttribute("id"); if (id.Length == 0) throw new Exception("Audio id absent: " + Relative(path));
                    // Do not guess whether an abstract/override/template is an
                    // output asset. This fixed input is audited to contain none.
                    if (asset.HasAttribute("abstract") || asset.HasAttribute("inheritFrom") || asset.HasAttribute("replaces")) {
                        unsupportedAudioDeclarations++; throw new Exception("Audio template/override requires explicit audit: " + Relative(path) + ":" + id);
                    }
                    string key = asset.LocalName + "\t" + id; Audio record;
                    if (!assets.TryGetValue(key, out record)) {
                        record = new Audio { Type = asset.LocalName, Id = id }; assets.Add(key, record);
                    }
                    record.Required |= work.Required; record.Sources.Add(Relative(path));
                }
                var ns = new XmlNamespaceManager(doc.NameTable); ns.AddNamespace("ea", Ns);
                foreach (XmlElement include in doc.SelectNodes("/ea:AssetDeclaration/ea:Includes/child::*", ns)) {
                    InclusionType kind = (InclusionType)Enum.Parse(typeof(InclusionType), include.GetAttribute("type"), true);
                    string logical = include.GetAttribute("source");
                    if (!Enum.IsDefined(typeof(InclusionType), kind)) throw new Exception("Unknown include kind: " + kind);
                    string key = kind.ToString(); int count; kindCounts.TryGetValue(key, out count); kindCounts[key] = count + 1;
                    string alias = logical.Contains(":") ? logical.Substring(0, logical.IndexOf(':')).ToLowerInvariant() : "relative";
                    aliasCounts.TryGetValue(alias, out count); aliasCounts[alias] = count + 1;
                    string next = Resolve(path, logical);
                    bool required = work.Required && kind == InclusionType.All;
                    edges.Add(new { document = Relative(path), kind = key, source = logical, physical = Relative(next), required = required });
                    if (kind == InclusionType.Reference) continue;
                    // Instance contributes tentative inheritance/reference sources,
                    // not all output assets. A later All visit promotes the path.
                    queue.Enqueue(new Work { Path = next, Required = required });
                }
            }
            Func<Audio, object> row = a => new { type = a.Type, id = a.Id, sources = a.Sources.OrderBy(x => x, StringComparer.Ordinal).ToArray() };
            object[] expected = assets.Values.Where(a => a.Required).OrderBy(a => a.Type, StringComparer.Ordinal).ThenBy(a => a.Id, StringComparer.Ordinal).Select(row).ToArray();
            object[] tentative = assets.Values.Where(a => !a.Required).OrderBy(a => a.Type, StringComparer.Ordinal).ThenBy(a => a.Id, StringComparer.Ordinal).Select(row).ToArray();
            if (expected.Length == 0) throw new Exception("No required audio declarations in pinned input");
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            File.WriteAllText(Path.Combine(output, "expected-audio.json"), serializer.Serialize(expected));
            File.WriteAllText(Path.Combine(output, "tentative-audio.json"), serializer.Serialize(tentative));
            File.WriteAllText(Path.Combine(output, "include-graph.json"), serializer.Serialize(edges));
            File.WriteAllText(Path.Combine(output, "xinclude-graph.json"), serializer.Serialize(xincludes));
            File.WriteAllText(Path.Combine(output, "empty-asset-placeholders.json"), serializer.Serialize(emptyDeclarations.OrderBy(x => x, StringComparer.Ordinal).ToArray()));
            File.WriteAllLines(Path.Combine(output, "compiled-xml-closure.txt"), documents.Keys.Select(Relative).OrderBy(x => x, StringComparer.Ordinal));
            File.WriteAllText(Path.Combine(output, "closure-summary.json"), serializer.Serialize(new {
                requiredAudio = expected.Length, tentativeAudio = tentative.Length, roots = roots.Select(Relative).ToArray(),
                parsedDocuments = documents.Count, emptyNamespaceAssetPlaceholders = emptyDeclarations.Count, includes = kindCounts, aliases = aliasCounts, xincludePaths = xincludes.Count,
                unsupportedAudioDeclarations = unsupportedAudioDeclarations, settingsData = Settings.Current.DataPaths,
                settingsArt = Settings.Current.ArtPaths, settingsAudio = Settings.Current.AudioPaths,
                semantics = "Original Core resolver/XInclude; all+Self required, instance tentative, reference external; path mode promotion allowed"
            }));
            Console.WriteLine("PASS original Core closure: " + documents.Count + " XML documents, " + expected.Length + " required audio, " + tentative.Length + " tentative audio");
            return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
