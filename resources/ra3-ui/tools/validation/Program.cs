using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.Collections;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.UiCompiler;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: UiValidation <FLA-file-or-source-root> <output-root> [--structure-only] [--mutate-real <movie-name> | --mutate <movie-name>]");
    return 2;
}
string source = Path.GetFullPath(args[0]), output = Path.GetFullPath(args[1]);
string? mutationMovie = null;
bool structureOnly = false;
bool mutationReal = false;
for (int i = 2; i < args.Length; i++)
{
    if (args[i] == "--mutate" && ++i < args.Length) mutationMovie = args[i];
    else if (args[i] == "--mutate-real" && ++i < args.Length) { mutationMovie = args[i]; mutationReal = true; }
    else if (args[i] == "--structure-only") structureOnly = true;
    else throw new ArgumentException("Unknown argument: " + args[i]);
}
if (structureOnly && mutationMovie != null) throw new ArgumentException("Mutation requires production compilation; omit --structure-only.");
Directory.CreateDirectory(output);
string sourceRoot = File.Exists(source) ? Path.GetDirectoryName(source)! : source;
string[] files = File.Exists(source) ? new[] { source } : Directory.GetFiles(sourceRoot, "*", SearchOption.AllDirectories)
    .Where(p => Path.GetExtension(p).Equals(".fla", StringComparison.OrdinalIgnoreCase))
    .OrderBy(p => p, StringComparer.Ordinal).ToArray();
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
var rows = new List<MovieResult>();
var totalTime = Stopwatch.StartNew();
foreach (string file in files)
{
    string relative = Path.GetRelativePath(sourceRoot, file), movie = Path.GetFileNameWithoutExtension(file);
    string movieOutput = Path.Combine(output, Path.ChangeExtension(relative, null)!);
    Directory.CreateDirectory(movieOutput);
    string stage = Path.Combine(movieOutput, ".source");
    var row = new MovieResult { Movie = movie, Source = file, RelativeSource = relative, FlaSha256 = Hash(File.ReadAllBytes(file)) };
    var watch = Stopwatch.StartNew();
    try
    {
        row.Structure = ReadStructure(file);
        if (structureOnly)
        {
            row.Status = "passed";
            row.CompilationStatus = "not-requested";
        }
        else
        {
        StageMovie(file, stage);
        using var plugin = new Plugin();
        plugin.Initialize(null, TargetPlatform.Win32);
        var first = plugin.CompileFolder(stage, Path.Combine(movieOutput, "baseline", movie));
        VerifyStreams(first);
        row.Assets = first.Assets.Count;
        row.AssetTypes = first.Assets.GroupBy(a => a.TypeName).ToDictionary(g => g.Key, g => g.Count());
        row.Inputs = first.InputFiles.Select(p => Path.GetRelativePath(stage, p)).ToArray();
        row.Streams = Describe(first);
        if (movie.Equals(mutationMovie, StringComparison.OrdinalIgnoreCase))
        {
            string marker = mutationReal ? "RA3_UI_REAL_EXTERNAL_AS_MUTATION_20261101" : "RA3_UI_CONVERSION_MUTATION_PROBE_20261101";
            string scriptRelative = mutationReal ? "scripts/DOMDocument/0001.as" : movie + ".as";
            string script = Path.Combine(stage, scriptRelative.Replace('/', Path.DirectorySeparatorChar));
            if (mutationReal && !File.Exists(script)) throw new FileNotFoundException("No first real timeline AS script for mutation proof.", scriptRelative);
            string original = File.Exists(script) ? File.ReadAllText(script) : "";
            File.WriteAllText(script, original + "\ntrace(\"" + marker + "\");\n", new UTF8Encoding(false));
            string retainedScript = Path.Combine(movieOutput, "mutated", scriptRelative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(retainedScript)!);
            File.Copy(script, retainedScript, true);
            var changed = plugin.CompileFolder(stage, Path.Combine(movieOutput, "mutated", movie));
            VerifyStreams(changed);
            bool binaryChanged = !first.Binary.AsSpan().SequenceEqual(changed.Binary);
            bool markerAbsentFromBaseline = first.Binary.AsSpan().IndexOf(Encoding.UTF8.GetBytes(marker)) < 0;
            bool markerPresent = changed.Binary.AsSpan().IndexOf(Encoding.UTF8.GetBytes(marker)) >= 0;
            bool dependencyRecorded = changed.InputFiles.Contains(script, StringComparer.OrdinalIgnoreCase);
            string changedFlaSha256 = Hash(File.ReadAllBytes(Path.Combine(stage, Path.GetFileName(file))));
            bool flaUnchanged = changedFlaSha256 == row.FlaSha256;
            if (!binaryChanged || !markerAbsentFromBaseline || !markerPresent || !dependencyRecorded || !flaUnchanged)
                throw new InvalidDataException("Mutation proof failed: binaryChanged=" + binaryChanged + ", markerAbsentFromBaseline=" + markerAbsentFromBaseline + ", markerPresent=" + markerPresent + ", dependencyRecorded=" + dependencyRecorded + ", flaUnchanged=" + flaUnchanged);
            row.Mutation = new { Mode = mutationReal ? "edit-existing-included-as" : "add-or-edit-same-name-timeline-as", Script = scriptRelative, Marker = marker,
                BinaryChanged = binaryChanged, MarkerAbsentFromBaseline = markerAbsentFromBaseline, MarkerPresentInFinalBinary = markerPresent, ExternalAsDependencyRecorded = dependencyRecorded,
                FlaUnchanged = flaUnchanged, BeforeFlaSha256 = row.FlaSha256, AfterFlaSha256 = changedFlaSha256, Streams = Describe(changed) };
        }
        row.Status = "passed";
        row.CompilationStatus = "passed";
        }
    }
    catch (Exception ex)
    {
        row.Status = "failed";
        if (row.Structure == null) row.Structure = new { Status = "failed", Error = ex.Message };
        row.CompilationStatus = structureOnly ? "not-requested" : row.Structure is StructureResult ? "failed" : "skipped-structure-failed";
        row.Error = ex.GetType().FullName + ": " + ex.Message;
        row.Exception = ex.ToString();
    }
    finally
    {
        if (Directory.Exists(stage)) Directory.Delete(stage, true);
        row.ElapsedMilliseconds = watch.ElapsedMilliseconds;
    }
    rows.Add(row);
    File.WriteAllText(Path.Combine(movieOutput, "result.json"), JsonSerializer.Serialize(row, options));
    Console.WriteLine(JsonSerializer.Serialize(new { row.Movie, row.RelativeSource, row.Status, row.CompilationStatus, row.Structure, row.Assets, row.ElapsedMilliseconds, row.Error }));
}
var report = new { Source = source, CompilerAssemblySha256 = Hash(File.ReadAllBytes(typeof(Plugin).Assembly.Location)), Runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
    Mode = structureOnly ? "structure-only" : "structure-and-production-compilation", StructurePassed = rows.Count(r => r.Structure is StructureResult),
    Total = rows.Count, Passed = rows.Count(r => r.Status == "passed"), Failed = rows.Count(r => r.Status == "failed"), ElapsedMilliseconds = totalTime.ElapsedMilliseconds,
    RequestedMutation = mutationMovie, RequestedMutationMode = mutationReal ? "edit-existing-included-as" : "same-name-timeline-as", MutationPerformed = rows.Any(r => r.Mutation != null), Movies = rows };
File.WriteAllText(Path.Combine(output, "report.json"), JsonSerializer.Serialize(report, options));
Console.WriteLine(JsonSerializer.Serialize(new { report.Total, report.Passed, report.Failed, report.ElapsedMilliseconds, report.MutationPerformed }));
return report.Failed > 0 || report.Total == 0 || mutationMovie != null && !report.MutationPerformed ? 1 : 0;

static string Hash(byte[] value) => Convert.ToHexString(SHA256.HashData(value)).ToLowerInvariant();
static StructureResult ReadStructure(string file)
{
    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    var type = typeof(Plugin).Assembly.GetType("BinaryAssetBuilder.UiCompiler.Fla.FlaDocument", true)!;
    object document;
    try { document = type.GetMethod("Read", flags, null, new[] { typeof(byte[]) }, null)!.Invoke(null, new object[] { File.ReadAllBytes(file) })!; }
    catch (TargetInvocationException ex) when (ex.InnerException != null) { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw(); throw; }
    int CountProperty(string name) => ((IEnumerable)type.GetProperty(name, flags)!.GetValue(document)!).Cast<object>().Count();
    var pages = (IDictionary)type.GetField("Pages", flags)!.GetValue(document)!;
    var payloads = (IDictionary)type.GetField("Payloads", flags)!.GetValue(document)!;
    var contents = type.GetField("Contents", flags)!.GetValue(document)!;
    var recordType = contents.GetType();
    bool Complete(object record) => (bool)recordType.GetField("Complete", flags)!.GetValue(record)!;
    bool allComplete = Complete(contents) && pages.Values.Cast<object>().All(Complete) && payloads.Values.Cast<object>().All(Complete);
    if (!allComplete) throw new InvalidDataException("FLA parser returned incomplete records.");
    object? Get(object record, string key) => recordType.GetMethod("get", flags)!.Invoke(record, new object[] { key });
    long Number(object record, string key) => (long)recordType.GetMethod("Number", flags)!.Invoke(record, new object[] { key, 0L })!;
    string Text(object record, string key) => (string)recordType.GetMethod("Text", flags)!.Invoke(record, new object[] { key, "" })!;
    int staticText = 0, dynamicText = 0, importedLibraryItems = 0;
    var fonts = new SortedSet<string>(StringComparer.Ordinal);
    var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
    void Visit(object? value)
    {
        if (value == null || value is string || value is byte[] || !visited.Add(value)) return;
        if (recordType.IsInstanceOfType(value))
        {
            if ((string)recordType.GetField("ClassName", flags)!.GetValue(value)! == "CPicText")
            {
                if ((Number(value, "textFlags") & 1) != 0) dynamicText++;
                else
                {
                    staticText++;
                    if (Get(value, "defaultStyle") is object defaultStyle) fonts.Add(Text(defaultStyle, "fontFamily"));
                    if (Get(value, "runs") is IEnumerable runs)
                        foreach (object run in runs) if (Get(run, "style") is object style) fonts.Add(Text(style, "fontFamily"));
                }
            }
            Visit(recordType.GetField("Fields", flags)!.GetValue(value));
        }
        else if (value is IDictionary dictionary) foreach (object? child in dictionary.Values) Visit(child);
        else if (value is IEnumerable sequence) foreach (object? child in sequence) Visit(child);
    }
    Visit(contents);
    foreach (object page in pages.Values) Visit(page);
    foreach (object descriptor in (IEnumerable)type.GetProperty("Library", flags)!.GetValue(document)!)
    {
        object? metadata = Get(descriptor, "libraryMetadata");
        object? link = metadata == null ? Get(descriptor, "linkage") : Get(metadata, "linkage");
        if (link != null && ((Number(link, "flags") & 2) != 0 || Text(link, "url").Length != 0)) importedLibraryItems++;
    }
    IEnumerable<object> Children(object record) => Get(record, "children") is IEnumerable sequence ? sequence.Cast<object>() : Enumerable.Empty<object>();
    int[] sceneTimelineFrames = ((IEnumerable)type.GetProperty("Scenes", flags)!.GetValue(document)!).Cast<object>().Select(descriptor =>
    {
        object page = pages[Text(descriptor, "streamName")]!;
        return Math.Max(1, Children(page).Select(layer => Children(layer).Sum(frame => checked((int)(long)recordType.GetMethod("Number", flags)!.Invoke(frame, new object[] { "duration", 1L })!))).DefaultIfEmpty(1).Max());
    }).ToArray();
    return new StructureResult("passed", Number(contents, "revision"), CountProperty("Scenes"), CountProperty("Library"), CountProperty("Media"),
        pages.Count, payloads.Count, sceneTimelineFrames, sceneTimelineFrames.Sum(), Number(contents, "frameRateWord") / 65536.0,
        allComplete, staticText, dynamicText, fonts.Where(f => f.Length > 0).ToArray(), importedLibraryItems);
}
static Dictionary<string, object> Describe(ModStreams streams) => new()
{
    ["manifest"] = new { Size = streams.Manifest.Length, Sha256 = Hash(streams.Manifest) },
    ["bin"] = new { Size = streams.Binary.Length, Sha256 = Hash(streams.Binary) },
    ["relo"] = new { Size = streams.Relocations.Length, Sha256 = Hash(streams.Relocations) },
    ["imp"] = new { Size = streams.Imports.Length, Sha256 = Hash(streams.Imports) }
};
static void VerifyStreams(ModStreams streams)
{
    if (streams.Manifest.Length < 48 || BitConverter.ToUInt16(streams.Manifest, 2) != 6)
        throw new InvalidDataException("Expected SAGE manifest version 6.");
    uint checksum = BitConverter.ToUInt32(streams.Manifest, 4);
    foreach (var bytes in new[] { streams.Binary, streams.Relocations, streams.Imports })
        if (bytes.Length < 4 || BitConverter.ToUInt32(bytes, 0) != checksum)
            throw new InvalidDataException("Final stream linkage checksum does not match manifest.");
    if (BitConverter.ToInt32(streams.Manifest, 12) != streams.Assets.Count || streams.Assets.Count < 3)
        throw new InvalidDataException("Final manifest asset count does not match compiled assets.");
}
static void StageMovie(string fla, string stage)
{
    if (Directory.Exists(stage)) Directory.Delete(stage, true);
    Directory.CreateDirectory(stage);
    foreach (string entry in Directory.EnumerateFileSystemEntries(Path.GetDirectoryName(fla)!))
    {
        string target = Path.Combine(stage, Path.GetFileName(entry));
        if (Directory.Exists(entry)) CopyTree(entry, target);
        else if (!Path.GetExtension(entry).Equals(".fla", StringComparison.OrdinalIgnoreCase) || entry == fla)
            File.Copy(entry, target);
    }
    static void CopyTree(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (string file in Directory.EnumerateFiles(source)) File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
        foreach (string directory in Directory.EnumerateDirectories(source)) CopyTree(directory, Path.Combine(target, Path.GetFileName(directory)));
    }
}

sealed class MovieResult
{
    public string Movie { get; set; } = "";
    public string Source { get; set; } = "";
    public string RelativeSource { get; set; } = "";
    public string FlaSha256 { get; set; } = "";
    public string Status { get; set; } = "";
    public string CompilationStatus { get; set; } = "";
    public object? Structure { get; set; }
    public int Assets { get; set; }
    public Dictionary<string, int>? AssetTypes { get; set; }
    public string[]? Inputs { get; set; }
    public Dictionary<string, object>? Streams { get; set; }
    public object? Mutation { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public string? Error { get; set; }
    public string? Exception { get; set; }
}
sealed record StructureResult(string Status, long Revision, int Scenes, int LibraryItems, int MediaItems, int ParsedPages, int ParsedMediaPayloads,
    int[] SceneTimelineFrames, int MainTimelineFrames, double FramesPerSecond,
    bool AllReferencedStreamsFullyConsumed, int StaticTextFieldsPresent, int DynamicTextFieldsPresent, string[] PotentialStaticFonts, int RuntimeImportedLibraryItems);
