using System;
using System.Collections.Generic;
using System.IO;

namespace NewModCompiler.Application
{
    public enum ExitCode { Success = 0, Arguments = 2, Environment = 3, Compilation = 4, Internal = 5, Cancelled = 6, Output = 7 }
    public sealed class BuildFailure : Exception
    {
        public readonly ExitCode ExitCode; public readonly string Code;
        public BuildFailure(ExitCode exitCode, string code, string message) : base(message) { ExitCode = exitCode; Code = code; }
    }
    public sealed class EffectiveOption
    {
        public string Value { get; set; }
        public string Source { get; set; }
    }
    public sealed class BuildRequest
    {
        public string PerformanceLevel { get; set; }
        NewModCompiler.Diagnostics.BuildPerformance performance;
        internal NewModCompiler.Diagnostics.BuildPerformance Performance { get { return performance ?? (performance = new NewModCompiler.Diagnostics.BuildPerformance(PerformanceLevel)); } }
        internal FileHashCache FileIdentities { get; set; }
#if NEWSdk_MONOLITHIC
        internal CoreInputPreparation InputPreparation { get; set; }
#endif
        public string Command { get; set; }
        public string ModRoot { get; set; }
        public string SdkRoot { get; set; }
        public string OutputRoot { get; set; }
        public string EventsPath { get; set; }
        public string ReportPath { get; set; }
        public string LogPath { get; set; }
        public bool AutoDetectContent { get; set; }
        public string[] Content { get; set; }
        public string PackageFormat { get; set; }
        public string ModVersion { get; set; }
        public string GameVersion { get; set; }
        public string SkudefName { get; set; }
        public bool IncludeAdditional { get; set; }
        public bool FixNeutralAssets { get; set; }
        public string[] Arguments { get; set; }
        public IDictionary<string, EffectiveOption> Options { get; private set; }
        public BuildRequest() { PerformanceLevel="off"; ModVersion="1.0"; GameVersion="1.12"; Options = new SortedDictionary<string, EffectiveOption>(StringComparer.Ordinal); }
    }
    public sealed class BuildTask
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string InputPath { get; set; }
        public string OutputName { get; set; }
        public string Reason { get; set; }
        public string[] Dependencies { get; set; }
    }
    public sealed class SkippedContent
    {
        public string Content { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
    public interface IDependencyPlanner
    {
        IEnumerable<BuildTask> Expand(BuildRequest request, IList<BuildTask> selectedRoots);
    }
    public sealed class BuildPlan
    {
        public BuildTask[] Tasks { get; set; }
        public SkippedContent[] Skipped { get; set; }
        public string PlanKind { get; set; }
        public string DependencyPolicy { get { return "Default is stream discovery only. Core owns include/reference closure. An explicit dependency planner may add stream dependencies, which are validated, deduplicated, and topologically ordered."; } }
    }
    public sealed class Artifact
    {
        public string Path { get; set; }
        public string Kind { get; set; }
        public long Size { get; set; }
        public string Sha256 { get; set; }
        internal string BackingPath { get; set; }
        internal string ContentPath { get { return BackingPath ?? Path; } }
        internal bool Verified { get; set; }
    }
    public sealed class StreamContext
    {
        public string BuildId { get; internal set; }
        public BuildTask Task { get; internal set; }
        public BuildRequest Request { get; internal set; }
        public string StagingDirectory { get; internal set; }
        internal bool CacheManagedBySession { get; set; }
    }
    // The implementation must run Core preprocessing and compilation in this process.
    // Ownership is per session, with fresh stream state per Compile invocation.
    public interface IStreamCompiler : IDisposable
    {
        string ProviderIdentity { get; }
        void Initialize(BuildRequest request);
        IList<Artifact> Compile(StreamContext context, System.Threading.CancellationToken cancellation);
    }
    // Optional participant owned by BuildSession after provider disposal. Commit
    // must remain reversible until all required output channels have closed.
    public interface IBuildCacheCompiler
    {
        object CacheReport { get; }
        void CommitCache();
        void RollbackCache();
        // Cleanup only, idempotent and non-throwing. No new cache writes here.
        void CompleteCache();
    }
    public interface IInputRootsCompiler
    {
        object InputRootsReport { get; }
    }
    public static class Paths
    {
        public static bool Contains(string root, string path)
        {
            root = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            path = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return string.Equals(root, path, StringComparison.OrdinalIgnoreCase) || path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }
    }
}
