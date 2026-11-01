using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using BinaryAssetBuilder.UiCompiler;
using BinaryAssetBuilder.UiCompiler.Projects;
using UiProjectTools;

if (args.Length == 0 || args.Contains("--help"))
{
    Console.WriteLine("Usage: UiProject.Build <project.json> --out <directory> [--movie <name>]");
    return args.Length == 0 ? 2 : 0;
}
string projectFile = Path.GetFullPath(args[0]);
string? output = null, selected = null;
for (int i = 1; i < args.Length; i++)
{
    if (args[i] == "--out" && ++i < args.Length) output = Path.GetFullPath(args[i]);
    else if (args[i] == "--movie" && ++i < args.Length) selected = args[i];
    else throw new ArgumentException("Unknown or incomplete option: " + args[i]);
}
if (output == null) throw new ArgumentException("--out is required; source assets are never used as the default output.");
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
byte[] manifest = File.ReadAllBytes(projectFile);
var originalIssues = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
using (var parsed = JsonDocument.Parse(manifest, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true }))
{
    if (parsed.RootElement.TryGetProperty("formatVersion", out var version) && version.GetInt32() != 1)
        throw new InvalidDataException("Unsupported UI project formatVersion.");
    if (parsed.RootElement.TryGetProperty("movies", out var inputMovies))
        foreach (var movie in inputMovies.EnumerateArray())
            if (movie.TryGetProperty("originalSourceIssues", out var issue) &&
                issue.TryGetProperty("staticLibraryDependencyState", out var state) && state.GetString() != "closed")
                originalIssues.Add(movie.GetProperty("name").GetString()!, issue.Clone());
}
var project = JsonSerializer.Deserialize<UiProjectDefinition>(manifest, options)
    ?? throw new InvalidDataException("Project document is empty.");
string projectDirectory = Path.GetDirectoryName(projectFile)!;
project.MasterFile = Path.GetFullPath(Path.Combine(projectDirectory, project.MasterFile));
project.SourceRoot = Path.GetFullPath(Path.Combine(projectDirectory, project.SourceRoot ?? "."));
var movies = selected == null ? project.Movies : project.Movies.Where(movie => movie.Name == selected).ToList();
if (!movies.Any()) throw new InvalidDataException("No matching movie in the project.");
try { OutputPaths.EnsureWritableOutput(output, projectDirectory); }
catch (InvalidOperationException error) { Console.Error.WriteLine(error.Message); return 2; }
Directory.CreateDirectory(output);
var compiler = new UiProjectCompiler();
var results = new List<object>();
int failed = 0;
IReadOnlyDictionary<string, ModStreams>? all = null;
if (selected == null)
{
    try { all = compiler.CompileAllStreams(project); }
    catch (Exception error)
    {
        Console.Error.WriteLine(error.Message);
        File.WriteAllText(Path.Combine(output, "build-report.json"), JsonSerializer.Serialize(new
        {
            project = projectFile, status = "failed-before-publish", error = error.ToString(),
            note = "No movie output was replaced by this build."
        }, options) + "\n");
        return 1;
    }
}
foreach (var movie in movies)
{
    var timer = Stopwatch.StartNew();
    try
    {
        if (Path.GetFileName(movie.Name) != movie.Name || movie.Name is "." or "..")
            throw new InvalidDataException("A published movie name must be a filename basename.");
        ModStreams streams = all == null ? compiler.CompileStreams(project, movie.Name) : all[movie.Name];
        string basename = Path.Combine(output, movie.Name, movie.Name);
        streams.WriteTo(basename);
        results.Add(new { movie = movie.Name, status = "passed", assets = streams.Assets.Count,
            milliseconds = timer.ElapsedMilliseconds,
            streams = Describe(streams), inputs = streams.InputFiles.Select(path => Path.GetRelativePath(projectDirectory, path)).ToArray(),
            originalSourceIssues = originalIssues.TryGetValue(movie.Name, out var issue) ? (JsonElement?)issue : null });
        Console.WriteLine($"{movie.Name}: {streams.Assets.Count} assets, four streams written");
        if (originalIssues.ContainsKey(movie.Name)) Console.Error.WriteLine(movie.Name + ": original source has unresolved dependencies; see project.json originalSourceIssues.");
    }
    catch (Exception error)
    {
        failed++;
        results.Add(new { movie = movie.Name, status = "failed", error = error.ToString(), milliseconds = timer.ElapsedMilliseconds });
        Console.Error.WriteLine(movie.Name + ": " + error.Message);
    }
}
var report = new { project = projectFile, projectSha256 = Hash(manifest), masterSha256 = Hash(File.ReadAllBytes(project.MasterFile)),
    compilerSha256 = Hash(File.ReadAllBytes(typeof(UiProjectCompiler).Assembly.Location)), runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
    total = results.Count, passed = results.Count - failed, failed, movies = results };
File.WriteAllText(Path.Combine(output, "build-report.json"), JsonSerializer.Serialize(report, options) + "\n");
return failed == 0 ? 0 : 1;

static string Hash(byte[] data) => Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
static object Describe(ModStreams streams) => new
{
    manifest = new { bytes = streams.Manifest.Length, sha256 = Hash(streams.Manifest) },
    bin = new { bytes = streams.Binary.Length, sha256 = Hash(streams.Binary) },
    relo = new { bytes = streams.Relocations.Length, sha256 = Hash(streams.Relocations) },
    imp = new { bytes = streams.Imports.Length, sha256 = Hash(streams.Imports) }
};
