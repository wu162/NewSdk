using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.UiCompiler;
using BinaryAssetBuilder.UiCompiler.Projects;

if (args.Length < 3)
{
    Console.Error.WriteLine("Usage: UiProject.Validation <project.json> <old-converted-root> <output-root> [--mutations] [--flacomdoc <jar>]");
    return 2;
}
string manifest = Path.GetFullPath(args[0]), oldRoot = Path.GetFullPath(args[1]), outputRoot = Path.GetFullPath(args[2]);
bool mutations = args.Contains("--mutations");
string? flacomdoc = null;
for (int i = 3; i < args.Length; i++)
    if (args[i] == "--flacomdoc" && ++i < args.Length) flacomdoc = Path.GetFullPath(args[i]);
var json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true, WriteIndented = true };
var project = JsonSerializer.Deserialize<UiProjectDefinition>(File.ReadAllText(manifest), json) ?? throw new InvalidDataException("No project definition.");
string projectDirectory = Path.GetDirectoryName(manifest)!;
project.MasterFile = Path.GetFullPath(Path.Combine(projectDirectory, project.MasterFile));
project.SourceRoot = Path.GetFullPath(Path.Combine(projectDirectory, project.SourceRoot));
if (Inside(outputRoot, project.SourceRoot) || Inside(outputRoot, oldRoot))
    throw new InvalidDataException("Validation output must be outside both source roots.");
Directory.CreateDirectory(outputRoot);
var compiler = new UiProjectCompiler();
var baseline = new Dictionary<string, ModStreams>(StringComparer.Ordinal);
var comparisons = new List<object>();
bool ok = true;
foreach (var movie in project.Movies)
{
    var row = new Dictionary<string, object?> { ["movie"] = movie.Name };
    try
    {
        using var oldCompiler = new Plugin(); oldCompiler.Initialize(null, TargetPlatform.Win32);
        string oldFla = Path.Combine(oldRoot, movie.Name, movie.Name + ".fla");
        ModStreams old = oldCompiler.CompileMod(oldFla, movie.Name);
        ModStreams current = compiler.CompileStreams(project, movie.Name);
        baseline.Add(movie.Name, current);
        var streams = Compare(old, current);
        bool same = streams.Values.All(x => x);
        row["status"] = same ? "passed" : "different";
        row["sameStreams"] = streams;
        row["oldStreams"] = Describe(old);
        row["projectStreams"] = Describe(current);
        row["oldAssets"] = old.Assets.Count;
        row["projectAssets"] = current.Assets.Count;
        row["inputs"] = current.InputFiles.Select(p => Path.GetRelativePath(project.SourceRoot, p)).ToArray();
        if (!same)
        {
            row["firstDifferences"] = Parts(old).ToDictionary(kv => kv.Key,
                kv => FirstDifference(kv.Value, Parts(current)[kv.Key]));
            row["assetDifferences"] = AssetDifferences(old, current);
        }
        current.WriteTo(Path.Combine(outputRoot, "baseline", movie.Name));
        old.WriteTo(Path.Combine(outputRoot, "old", movie.Name));
        ok &= same;
    }
    catch (Exception error)
    {
        row["status"] = "failed"; row["error"] = error.ToString(); ok = false;
    }
    comparisons.Add(row);
    Console.WriteLine(JsonSerializer.Serialize(row, json));
}
var mutationResults = new List<object>();
if (mutations && ok)
{
    var users = new Dictionary<string, List<string>>(StringComparer.Ordinal);
    foreach (var movie in project.Movies)
        foreach (string source in movie.Sources)
        {
            if (!users.TryGetValue(source, out var names)) users.Add(source, names = new());
            names.Add(movie.Name);
        }
    bool PlainScript(string relative)
    {
        string text = File.ReadAllText(Path.Combine(project.SourceRoot, relative));
        string masked = Regex.Replace(text, @"/\*[\s\S]*?\*/|//[^\r\n]*", "");
        return !Regex.IsMatch(masked, @"\b(?:class|interface)\s+[\w.$]+") && text.Length > 30;
    }
    string shared = users.Where(p => p.Value.Count > 1 && PlainScript(p.Key))
        .OrderByDescending(p => p.Key.Contains("GetHelpBoxPos", StringComparison.Ordinal)).ThenBy(p => p.Key, StringComparer.Ordinal).First().Key;
    string privateSource = users.Where(p => p.Value.Count == 1 && p.Value[0] == "TacticalHUDMainCommandBar" && PlainScript(p.Key))
        .OrderByDescending(p => p.Key.Contains("GetSelectionRefinementPage", StringComparison.Ordinal)).ThenBy(p => p.Key, StringComparer.Ordinal).First().Key;
    mutationResults.Add(EditScript("shared-as", shared, users[shared]));
    mutationResults.Add(EditScript("private-as", privateSource, users[privateSource]));
    if (flacomdoc != null) mutationResults.Add(EditSharedShape());

    object EditScript(string mode, string relative, IReadOnlyCollection<string> expectedUsers)
    {
        UiProjectDefinition staged = Stage(project, Path.Combine(outputRoot, mode, "source"));
        string file = Path.Combine(staged.SourceRoot, relative), marker = "UI_PROJECT_" + mode.Replace('-', '_').ToUpperInvariant() + "_PROBE";
        string code = File.ReadAllText(file), statement = "trace(\"" + marker + "\");";
        if (Regex.IsMatch(code, @"^\s*on(?:ClipEvent)?\s*\("))
        {
            int brace = code.IndexOf('{'); if (brace < 0) throw new InvalidDataException("No event body.");
            code = code.Insert(brace + 1, statement);
        }
        else code += "\n" + statement + "\n";
        File.WriteAllText(file, code, new UTF8Encoding(false));
        var outcomes = new List<object>(); bool passed = true;
        foreach (var movie in staged.Movies)
        {
            var changed = compiler.CompileStreams(staged, movie.Name);
            bool expected = expectedUsers.Contains(movie.Name), different = !baseline[movie.Name].Binary.SequenceEqual(changed.Binary);
            bool markerPresent = changed.Binary.AsSpan().IndexOf(Encoding.UTF8.GetBytes(marker)) >= 0;
            bool dependency = changed.InputFiles.Contains(Path.GetFullPath(file), StringComparer.OrdinalIgnoreCase);
            passed &= different == expected && markerPresent == expected && dependency == expected;
            outcomes.Add(new { movie = movie.Name, expected, binaryChanged = different, markerPresent, sourceDependencyRecorded = dependency, streams = Describe(changed) });
            changed.WriteTo(Path.Combine(outputRoot, mode, "streams", movie.Name));
        }
        bool masterUnchanged = Hash(File.ReadAllBytes(project.MasterFile)) == Hash(File.ReadAllBytes(staged.MasterFile));
        passed &= masterUnchanged; ok &= passed;
        return new { mode, source = relative, expectedUsers, passed, masterFlaUnchanged = masterUnchanged, outcomes };
    }

    object EditSharedShape()
    {
        UiProjectDefinition staged = Stage(project, Path.Combine(outputRoot, "shared-shape", "source"));
        string authoring = Path.Combine(Path.GetDirectoryName(project.MasterFile)!, "Authoring");
        string stagedAuthoring = Path.Combine(Path.GetDirectoryName(staged.MasterFile)!, "Authoring");
        CopyTree(authoring, stagedAuthoring);
        var symbols = Directory.GetFiles(stagedAuthoring, "*.xml", SearchOption.AllDirectories)
            .Select(path => (Path: path, Document: XDocument.Load(path)))
            .Where(p => p.Document.Root?.Name.LocalName == "DOMSymbolItem")
            .ToDictionary(p => p.Document.Root!.Attribute("name")!.Value, StringComparer.Ordinal);
        string graphicRoot = symbols.Keys.Single(n => n.Split('/').Last() == "Common_WhiteRectangle");
        var pending = new Queue<string>(); var seen = new HashSet<string>(StringComparer.Ordinal); pending.Enqueue(graphicRoot);
        string? symbol = null, itemName = null; Match? color = null; string text = "";
        while (pending.Count != 0)
        {
            string name = pending.Dequeue(); if (!seen.Add(name)) continue;
            var source = symbols[name]; string candidate = File.ReadAllText(source.Path);
            var fill = Regex.Match(candidate, "(<SolidColor\\b[^>]*\\bcolor=\")#([0-9a-fA-F]{6})(\")");
            if (fill.Success) { symbol = source.Path; itemName = name; color = fill; text = candidate; break; }
            foreach (string dependency in source.Document.Descendants().Attributes("libraryItemName").Select(a => a.Value))
                if (symbols.ContainsKey(dependency)) pending.Enqueue(dependency);
        }
        if (symbol == null || color == null || itemName == null) throw new InvalidDataException("Shared rectangle graph has no solid-color fill.");
        uint rgb = Convert.ToUInt32(color.Groups[2].Value, 16), replacement = rgb ^ 0x00ff00u;
        text = text.Substring(0, color.Groups[2].Index) + replacement.ToString("x6") + text.Substring(color.Groups[2].Index + 6);
        File.WriteAllText(symbol, text, new UTF8Encoding(false));
        var start = new ProcessStartInfo("java") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        foreach (string argument in new[] { "-Xmx1536m", "-jar", flacomdoc!, "convert", "--format", "MX2004", Path.Combine(stagedAuthoring, "Tactical.xfl"), staged.MasterFile }) start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEndAsync(); var stderr = process.StandardError.ReadToEndAsync(); process.WaitForExit();
        string log = stdout.GetAwaiter().GetResult(), error = stderr.GetAwaiter().GetResult();
        File.WriteAllText(Path.Combine(outputRoot, "shared-shape", "flacomdoc.log"), log + error);
        if (process.ExitCode != 0) throw new InvalidDataException("Shared graphic re-export failed.");
        string[] expectedUsers = staged.Movies.Where(m => m.LibraryOrder.Contains(itemName)).Select(m => m.Name).ToArray();
        if (expectedUsers.Length < 2) throw new InvalidDataException("Shared graphic must have multiple users.");
        var outcomes = new List<object>(); bool passed = true;
        foreach (var movie in staged.Movies)
        {
            var changed = compiler.CompileStreams(staged, movie.Name);
            bool expected = expectedUsers.Contains(movie.Name), different = !baseline[movie.Name].Binary.SequenceEqual(changed.Binary);
            passed &= different == expected;
            outcomes.Add(new { movie = movie.Name, expected, binaryChanged = different, streams = Describe(changed) });
            changed.WriteTo(Path.Combine(outputRoot, "shared-shape", "streams", movie.Name));
        }
        ok &= passed;
        return new { mode = "shared-shape", graphicRoot, changedItem = itemName, originalColor = rgb.ToString("x6"), replacementColor = replacement.ToString("x6"), expectedUsers, passed, outcomes };
    }
}
var report = new { project = manifest, masterFlaSha256 = Hash(File.ReadAllBytes(project.MasterFile)), compilerAssemblySha256 = Hash(File.ReadAllBytes(typeof(Plugin).Assembly.Location)),
    comparisons, mutations = mutationResults, passed = ok };
File.WriteAllText(Path.Combine(outputRoot, "report.json"), JsonSerializer.Serialize(report, json));
Console.WriteLine(JsonSerializer.Serialize(new { movies = comparisons.Count, mutations = mutationResults.Count, passed = ok }));
return ok ? 0 : 1;

static Dictionary<string, byte[]> Parts(ModStreams value) => new() { ["manifest"] = value.Manifest, ["bin"] = value.Binary, ["relo"] = value.Relocations, ["imp"] = value.Imports };
static bool Inside(string path, string root)
{
    var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    return path.Equals(root, comparison) || path.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, comparison);
}
static Dictionary<string, bool> Compare(ModStreams a, ModStreams b) => Parts(a).ToDictionary(p => p.Key, p => p.Value.SequenceEqual(Parts(b)[p.Key]));
static string Hash(byte[] data) => Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
static Dictionary<string, object> Describe(ModStreams value) => Parts(value).ToDictionary(p => p.Key, p => (object)new { bytes = p.Value.Length, sha256 = Hash(p.Value) });
static int FirstDifference(byte[] a, byte[] b)
{ int limit = Math.Min(a.Length, b.Length); for (int i = 0; i < limit; i++) if (a[i] != b[i]) return i; return a.Length == b.Length ? -1 : limit; }
static object AssetDifferences(ModStreams a, ModStreams b)
{
    var old = a.Assets.ToDictionary(x => x.TypeName + ":" + x.Name);
    var current = b.Assets.ToDictionary(x => x.TypeName + ":" + x.Name);
    return old.Keys.Union(current.Keys).Where(k => !old.ContainsKey(k) || !current.ContainsKey(k) || !old[k].Buffer.InstanceData.SequenceEqual(current[k].Buffer.InstanceData))
        .Select(k => new { asset = k, oldSha256 = old.TryGetValue(k, out var x) ? Hash(x.Buffer.InstanceData) : null, newSha256 = current.TryGetValue(k, out var y) ? Hash(y.Buffer.InstanceData) : null }).ToArray();
}
static UiProjectDefinition Stage(UiProjectDefinition original, string root)
{
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true };
    var copy = JsonSerializer.Deserialize<UiProjectDefinition>(JsonSerializer.Serialize(original, options), options)!;
    Directory.CreateDirectory(root);
    string masterRelative = Path.GetRelativePath(original.SourceRoot, original.MasterFile);
    string target = Path.Combine(root, masterRelative); Directory.CreateDirectory(Path.GetDirectoryName(target)!); File.Copy(original.MasterFile, target, true);
    foreach (string file in original.Movies.SelectMany(m => m.Sources).Distinct(StringComparer.Ordinal))
    {
        target = Path.Combine(root, file); Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        File.Copy(Path.Combine(original.SourceRoot, file), target, true);
    }
    copy.SourceRoot = root; copy.MasterFile = Path.Combine(root, masterRelative); return copy;
}
static void CopyTree(string source, string target)
{
    Directory.CreateDirectory(target);
    foreach (string file in Directory.GetFiles(source)) File.Copy(file, Path.Combine(target, Path.GetFileName(file)), true);
    foreach (string folder in Directory.GetDirectories(source)) CopyTree(folder, Path.Combine(target, Path.GetFileName(folder)));
}
