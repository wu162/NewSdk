#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using EALA.Metrics;

namespace NewModCompiler.Application
{
    // Experimental in-process backend. CLI enablement and SDK/G3 acceptance are
    // separate gates; this type never launches BAB, a worker, or another EXE.
    public sealed class CoreStreamCompiler : IStreamCompiler
    {
        static readonly object LeaseGate = new object();
        static CoreStreamCompiler leaseOwner;
        readonly TraceWriteHandler traceWriter;
        BuildRequest request;
        string sdkRoot, modRoot, outputRoot, configurationXml;
        int threadId, traceLevel = 3;
        bool initialized, initializing, disposed, compiling;
        BuiltinCompilers compilers;
        VerifierPluginRegistry verifiers;
        PluginDescriptor[] verifierDescriptors;
        public CoreStreamCompiler(TraceWriteHandler traceWriter = null) { this.traceWriter = traceWriter; }
        public string ProviderIdentity { get { return "NewModCompiler.Core.Experimental.Win32.NoCache"; } }

        public void Initialize(BuildRequest request)
        {
            if (disposed) throw new ObjectDisposedException("CoreStreamCompiler");
            if (initialized) throw new InvalidOperationException("The backend is already initialized.");
            if (request == null) throw new ArgumentNullException("request");
            lock (LeaseGate)
            {
                if (disposed) throw new ObjectDisposedException("CoreStreamCompiler");
                if (initialized || initializing) throw new InvalidOperationException("The backend is already initialized or initializing.");
                if (leaseOwner != null) throw new BuildFailure(ExitCode.Environment, "CORE_SESSION_BUSY", "Only one in-process Core session may own the process state.");
                leaseOwner = this;
                initializing = true;
                threadId = Thread.CurrentThread.ManagedThreadId;
            }
            try
            {
                ValidateOptions(request);
                sdkRoot = Path.GetFullPath(request.SdkRoot);
                modRoot = Path.GetFullPath(request.ModRoot);
                outputRoot = Path.GetFullPath(request.OutputRoot);
                if (!Directory.Exists(sdkRoot) || !Directory.Exists(modRoot))
                    throw new BuildFailure(ExitCode.Environment, "CORE_INPUT_ROOT_MISSING", "The SDK and MOD directories must exist.");
                if (Paths.Contains(sdkRoot, outputRoot) || Paths.Contains(outputRoot, sdkRoot) || Paths.Contains(modRoot, outputRoot) || Paths.Contains(outputRoot, modRoot))
                    throw new BuildFailure(ExitCode.Arguments, "CORE_OUTPUT_OVERLAP", "Core output must be separate from SDK and MOD inputs.");
                LoadSdkConfiguration();
                ReadSettings(); // Validate the real serialized contract before accepting the session.
                this.request = request;
                lock (LeaseGate) { initialized = true; initializing = false; }
            }
            catch (Exception error)
            {
                lock (LeaseGate) { initializing = false; ReleaseLease(); }
                if (error is BuildFailure || error is OperationCanceledException) throw;
                throw Failure(ExitCode.Environment, "CORE_INITIALIZE_FAILED", error);
            }
        }

        void LoadSdkConfiguration()
        {
            if (!Directory.Exists(sdkRoot)) throw new BuildFailure(ExitCode.Environment, "CORE_SDK_MISSING", "The SDK directory does not exist.");
            string path = Path.Combine(sdkRoot, "tools", "BinaryAssetBuilder.exe.config");
            if (!File.Exists(path)) throw new BuildFailure(ExitCode.Environment, "CORE_CONFIG_MISSING", "The SDK BinaryAssetBuilder configuration is missing: " + path);
            var configuration = new XmlDocument { XmlResolver = null };
            configuration.Load(path);
            XmlNode section = configuration.SelectSingleNode("/configuration/assetbuilder");
            if (section == null) throw new BuildFailure(ExitCode.Environment, "CORE_CONFIG_MISSING", "The SDK configuration has no assetbuilder section.");
            configurationXml = section.OuterXml;
            if (!File.Exists(Path.Combine(sdkRoot, "Schemas", "xsd", "CnC3Types.xsd")))
                throw new BuildFailure(ExitCode.Environment, "CORE_SCHEMA_MISSING", "The SDK CnC3Types schema is missing.");
        }

        // Read-only environment initialization, not a synthetic MOD build. The
        // unique hash namespace is never created or finalized. Keep the lease
        // until every scope and provider has restored the caller's process state.
        public static void ProbeEnvironment(string sdkDirectory)
        {
            var probe = new CoreStreamCompiler();
            lock (LeaseGate)
            {
                if (leaseOwner != null) throw new BuildFailure(ExitCode.Environment, "CORE_SESSION_BUSY", "A Core session already owns the process state.");
                leaseOwner = probe;
                probe.threadId = Thread.CurrentThread.ManagedThreadId;
                probe.initializing = true;
            }
            Settings previousSettings = null;
            bool previousMetrics = false, capturedState = false;
            IDisposable tracing = null, state = null, report = null, hashes = null;
            Exception failure = null;
            try
            {
                previousSettings = Settings.Current;
                previousMetrics = MetricManager.Enabled;
                capturedState = true;
                probe.sdkRoot = Path.GetFullPath(sdkDirectory);
                probe.LoadSdkConfiguration();
                Settings settings = probe.ReadSettings();
                settings.DataRoot = probe.sdkRoot;
                settings.SchemaPath = Path.Combine(probe.sdkRoot, "Schemas", "xsd", "CnC3Types.xsd");
                settings.TargetPlatform = TargetPlatform.Win32;
                settings.SessionCache = settings.BuildCache = settings.PartialSessionCache = settings.CompressedSessionCache = false;
                settings.GuiMode = settings.Resident = settings.MetricsReporting = settings.OutputAssetReport = settings.OutputStringHashes = false;
                settings.BuildConfigurationName = null;
                Settings.Current = settings;
                settings = SettingsLoader.GetSettingsForConfiguration(null);
                settings.BigEndian = false;
                Settings.Current = settings;
                tracing = Tracer.BeginScope(null,3);
                state = DocumentProcessor.BeginStateScope();
                report = AssetReport.BeginScope();
                string namespaceOnly = Path.Combine(Path.GetTempPath(), "NewModCompiler-doctor-" + Guid.NewGuid().ToString("N"));
                settings.SessionCacheDirectory = namespaceOnly;
                hashes = HashProvider.BeginStringHashScope(namespaceOnly);
                MetricManager.Enabled = false;
                VerifyAntlrRuntime();
                new SchemaSet(settings.StableSort);
                ExpressionEvaluatorWrapper.LoadAssembly();
                XIncludingReaderWrapper.LoadAssembly();
                probe.EnsureProviders(); // Includes the pinned, EXE-bound texture adapter and verifier.
            }
            catch (Exception error)
            {
                failure = error is BuildFailure ? error : Failure(ExitCode.Environment, "CORE_ENVIRONMENT_INVALID", error);
            }
            finally
            {
                try { probe.DisposeProviders(); } catch (Exception error) { AddCleanupFailure(ref failure,error,ExitCode.Environment,"CORE_ENVIRONMENT_CLEANUP_FAILED"); }
                foreach (IDisposable scope in new[] {report,hashes,state,tracing})
                    try { if (scope != null) scope.Dispose(); } catch (Exception error) { AddCleanupFailure(ref failure,error,ExitCode.Environment,"CORE_ENVIRONMENT_CLEANUP_FAILED"); }
                if (capturedState) { Settings.Current = previousSettings; MetricManager.Enabled = previousMetrics; }
                lock (LeaseGate) { probe.disposed = true; probe.initializing = false; probe.ReleaseLease(); }
            }
            if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
        }

        static void VerifyAntlrRuntime()
        {
            Assembly runtime = typeof(antlr.LLkParser).Assembly;
            string expected = Path.Combine(Path.GetDirectoryName(typeof(CoreStreamCompiler).Assembly.Location), "antlr.runtime.dll");
            if (runtime.GetName().Name != "antlr.runtime" || !string.Equals(Path.GetFullPath(runtime.Location),Path.GetFullPath(expected),StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("ANTLR must load from the current compiler directory.");
            using (var algorithm = SHA256.Create()) using (var input = File.OpenRead(runtime.Location))
                if (BitConverter.ToString(algorithm.ComputeHash(input)).Replace("-", "").ToLowerInvariant() != "febdc86d6299a550c6ec87050b0cbf5205e821851e75494c699c17ce8ba2c62a")
                    throw new InvalidDataException("ANTLR runtime differs from the pinned compiler input.");
        }

        void ValidateOptions(BuildRequest candidate)
        {
            traceLevel = 3;
            if (candidate.PackageFormat != null && candidate.PackageFormat != "loose")
                throw new BuildFailure(ExitCode.Arguments, "CORE_OPTION_UNSUPPORTED", "Only loose output is available in the experimental backend.");
            var ordinary = new HashSet<string>(new[] { "mod", "sdk-root", "out", "events", "report", "log", "package-content", "package-format" }, StringComparer.OrdinalIgnoreCase);
            var disabled = new HashSet<string>(new[] { "sc", "bc", "csc", "psc", "fsc", "atc", "pc", "oof", "vf", "oar", "osh", "oix", "mr", "slowclean" }, StringComparer.OrdinalIgnoreCase);
            foreach (var option in candidate.Options)
            {
                if (option.Value == null) throw new BuildFailure(ExitCode.Arguments, "CORE_OPTION_INVALID", "A Core option has no effective value: " + option.Key);
                string key = option.Key, value = option.Value.Value;
                if (key == "package-format" && !string.Equals(value, "loose", StringComparison.OrdinalIgnoreCase))
                    throw new BuildFailure(ExitCode.Arguments, "CORE_OPTION_UNSUPPORTED", "Only loose output is available in the experimental backend.");
                if (ordinary.Contains(key)) continue;
                if (disabled.Contains(key) && string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)) continue;
                if (key == "tp" && string.Equals(value, "Win32", StringComparison.OrdinalIgnoreCase)) continue;
                if (key == "ls" && string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)) continue;
                if (key == "el" && value == "0") continue;
                int level;
                if (key == "tl" && int.TryParse(value, out level) && level >= 0 && level <= 9) { traceLevel = level; continue; }
                throw new BuildFailure(ExitCode.Arguments, "CORE_OPTION_UNSUPPORTED", "Experimental Core backend does not support option " + key + "=" + value + ". Cache, precompiled, alternate output, reporting, path/configuration overrides and postprocessing options are not enabled.");
            }
        }

        Settings ReadSettings()
        {
            var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(configurationXml);
            var settings = new SerializedSectionHandler().Create(null, null, xml.DocumentElement) as Settings;
            if (settings == null) throw new BuildFailure(ExitCode.Environment, "CORE_CONFIG_INVALID", "SDK assetbuilder settings could not be read.");
            // Deserialization retains configuration data; old targetType entries
            // must never reach PluginRegistry or Type.GetType.
            settings.Plugins = new PluginDescriptor[0];
            settings.VerifierPlugins = new PluginDescriptor[0];
            return settings;
        }

        Settings StreamSettings(StreamContext context, string staging)
        {
            Settings settings = ReadSettings();
            settings.InputPath = Path.GetFullPath(context.Task.InputPath);
            settings.SchemaPath = Path.Combine(sdkRoot, "Schemas", "xsd", "CnC3Types.xsd");
            settings.DataRoot = sdkRoot;
            settings.OutputDirectory = Path.Combine(staging, "streams");
            settings.IntermediateOutputDirectory = Path.Combine(staging, "intermediate");
            settings.SessionCacheDirectory = Path.Combine(staging, "hashes");
            settings.BuildCacheDirectory = Path.Combine(staging, "cache");
            settings.DefaultDataPaths = Prepend(Path.Combine(modRoot, "Data"), settings.DefaultDataPaths);
            settings.DefaultArtPaths = Prepend(Path.Combine(modRoot, "Art"), settings.DefaultArtPaths);
            settings.DefaultAudioPaths = Prepend(Path.Combine(modRoot, "Audio"), settings.DefaultAudioPaths);
            settings.BuildConfigurationName = null;
            settings.BuildConfigurations = new BuildConfiguration[0];
            settings.TargetPlatform = TargetPlatform.Win32;
            // The experimental backend has no persistent cache implementation;
            // keep compression disabled too, matching the independent /csc:false
            // baseline without suggesting that compressed-cache reuse is supported.
            settings.SessionCache = settings.BuildCache = settings.PartialSessionCache = settings.FreezeSessionCache = settings.CompressedSessionCache = false;
            settings.AlwaysTouchCache = settings.StreamHints = settings.UsePrecompiled = false;
            settings.GuiMode = settings.Resident = settings.PauseOnError = settings.MetricsReporting = false;
            settings.OldOutputFormat = settings.VersionFiles = settings.OutputAssetReport = settings.OutputStringHashes = settings.OutputIntermediateXml = false;
            settings.SingleFile = false; settings.LinkedStreams = true;
            settings.CustomPostfix = ""; settings.StreamPostfix = ""; settings.BasePatchStream = null;
            settings.TraceLevel = traceLevel; settings.ErrorLevel = 0;
            Settings.Current = settings;
            SettingsLoader.PostProcessSettings(Path.Combine(sdkRoot, "tools"));
            return settings;
        }
        static string Prepend(string path, string configured)
        {
            if (path.IndexOf(';') >= 0) throw new BuildFailure(ExitCode.Arguments, "CORE_PATH_UNSUPPORTED", "Semicolons in MOD paths cannot be represented by the legacy path resolver.");
            return path + ";" + configured;
        }

        void EnsureProviders()
        {
            try
            {
                if (compilers == null)
                {
                    BuiltinCompilers candidate = BuiltinCompilers.CreateWithTexture(TargetPlatform.Win32);
                    try
                    {
                        // The old SDK XML handler was #all, so Core reads its global
                        // hash/version via DefaultPlugin, not via a type route. Keep
                        // those exact metadata semantics without reopening fallback
                        // compilation for unknown types in the new explicit registry.
                        // Original XmlCompiler/Plugin.cs:GetAllTypesHash returns
                        // 1424942948; GetVersionNumber returns 4 for Win32. DoBuildData
                        // uses the registry initialized from the SDK #all descriptor.
                        IAssetBuilderPlugin xml = candidate.Registry.GetPlugin(InstanceHandle.GetTypeId("AttributeModifier"));
                        candidate.Registry.DefaultPlugin = new DefaultMetadataPlugin(candidate.Registry.DefaultPlugin, xml);
                        compilers = candidate;
                    }
                    catch (Exception primary)
                    {
                        Exception failure = primary;
                        try { candidate.Dispose(); } catch (Exception cleanup) { AddCleanupFailure(ref failure, cleanup, ExitCode.Internal, "CORE_PROVIDER_CLEANUP_FAILED"); }
                        ExceptionDispatchInfo.Capture(failure).Throw();
                        throw;
                    }
                }
                else compilers.ReInitialize(TargetPlatform.Win32);
                if (verifiers == null)
                {
                    verifierDescriptors = new[] { PluginDescriptor.CreateBuiltin("#all", false, () => new BinaryAssetBuilder.GameDataVerifier.Plugin()) };
                    verifiers = new VerifierPluginRegistry(verifierDescriptors, TargetPlatform.Win32);
                }
                else verifiers.ReInitialize(verifierDescriptors, TargetPlatform.Win32);
            }
            catch (BuildFailure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception error) { throw Failure(ExitCode.Environment, "CORE_PROVIDER_INITIALIZE_FAILED", error); }
        }

        sealed class DefaultMetadataPlugin : IAssetBuilderPlugin
        {
            readonly IAssetBuilderPlugin rejection, metadata;
            public DefaultMetadataPlugin(IAssetBuilderPlugin rejection, IAssetBuilderPlugin metadata)
            {
                if (!(metadata is Recovered.XmlPriority.Plugin)) throw new InvalidOperationException("The default metadata provider must be the built-in XML compiler.");
                this.rejection = rejection; this.metadata = metadata;
            }
            // BuiltinCompilers owns initialization and disposal of the actual
            // XML instance. This view must not initialize it a second time.
            public void Initialize(object configuration, TargetPlatform platform) { }
            public void ReInitialize(object configuration, TargetPlatform platform) { }
            public uint GetAllTypesHash() { return metadata.GetAllTypesHash(); }
            public uint GetVersionNumber() { return metadata.GetVersionNumber(); }
            public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId) { return rejection.GetExtendedTypeInformation(typeId); }
            public AssetBuffer ProcessInstance(InstanceDeclaration instance) { return rejection.ProcessInstance(instance); }
        }

        public IList<Artifact> Compile(StreamContext context, CancellationToken cancellation)
        {
            CheckOwner();
            if (compiling) throw new InvalidOperationException("Core compilation cannot be reentered.");
            if (context == null || context.Task == null || !Object.ReferenceEquals(context.Request, request))
                throw new BuildFailure(ExitCode.Arguments, "CORE_CONTEXT_INVALID", "Stream context must belong to the initialized request.");
            cancellation.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(context.StagingDirectory) || !Path.IsPathRooted(context.StagingDirectory))
                throw new BuildFailure(ExitCode.Arguments, "CORE_STAGING_INVALID", "The stream workspace must be an absolute directory.");
            string staging = Path.GetFullPath(context.StagingDirectory);
            if (!Paths.Contains(outputRoot, staging) || Paths.Contains(staging, outputRoot) || Paths.Contains(sdkRoot, staging) || Paths.Contains(modRoot, staging))
                throw new BuildFailure(ExitCode.Output, "CORE_STAGING_INVALID", "The stream workspace must be beneath the isolated output root.");
            RejectReparseAncestors(staging);
            if (Directory.Exists(staging) && Directory.EnumerateFileSystemEntries(staging).Any())
                throw new BuildFailure(ExitCode.Output, "CORE_STAGING_NOT_EMPTY", "The stream workspace must be new or empty.");
            if (string.IsNullOrEmpty(context.Task.InputPath) || !Path.IsPathRooted(context.Task.InputPath) || !Paths.Contains(modRoot, context.Task.InputPath) || !File.Exists(context.Task.InputPath) || !string.Equals(Path.GetExtension(context.Task.InputPath), ".xml", StringComparison.OrdinalIgnoreCase))
                throw new BuildFailure(ExitCode.Arguments, "CORE_STREAM_INPUT_INVALID", "The experimental backend requires an existing XML stream entry.");
            Settings previousSettings = Settings.Current;
            bool previousMetrics = MetricManager.Enabled;
            IDisposable hashes = null, tracing = null, report = null, processorState = null;
            Exception failure = null;
            IList<Artifact> artifacts = null;
            compiling = true;
            try
            {
                Directory.CreateDirectory(staging);
                tracing = Tracer.BeginScope(traceWriter, traceLevel);
                MetricManager.Enabled = false; // No listeners or legacy telemetry are installed.
                Settings settings = StreamSettings(context, staging);
                processorState = DocumentProcessor.BeginStateScope();
                report = AssetReport.BeginScope();
                EnsureProviders();
                var schemas = new SchemaSet(settings.StableSort);
                hashes = HashProvider.BeginStringHashScope(settings.SessionCacheDirectory);
                var cache = new SessionCache(); cache.InitializeCache(new List<string>());
                var processor = new DocumentProcessor(settings, compilers.Registry, verifiers) { Cache = cache, SchemaSet = schemas };
                bool success;
                processor.ProcessDocument(settings.InputPath, true, false, out success);
                if (!success) throw new BuildFailure(ExitCode.Compilation, "CORE_COMPILE_FAILED", "Core did not complete the stream.");
                cancellation.ThrowIfCancellationRequested(); // Cancellation is cooperative at complete stream boundaries.
                artifacts = CollectArtifacts(settings.OutputDirectory);
                cancellation.ThrowIfCancellationRequested();
            }
            catch (Exception error)
            {
                failure = error is BuildFailure || error is OperationCanceledException ? error :
                    Failure(ExitCode.Compilation, "CORE_COMPILE_FAILED", error);
            }
            finally
            {
                var cleanup = new List<Exception>();
                try { if (report != null) report.Dispose(); } catch (Exception error) { cleanup.Add(error); }
                try { if (hashes != null) hashes.Dispose(); } catch (Exception error) { cleanup.Add(error); }
                try { if (processorState != null) processorState.Dispose(); } catch (Exception error) { cleanup.Add(error); }
                try { if (tracing != null) tracing.Dispose(); } catch (Exception error) { cleanup.Add(error); }
                Settings.Current = previousSettings;
                MetricManager.Enabled = previousMetrics;
                compiling = false;
                foreach (Exception error in cleanup) AddCleanupFailure(ref failure, error, ExitCode.Internal, "CORE_CLEANUP_FAILED");
                if (failure != null)
                {
                    try { if (Directory.Exists(staging)) Directory.Delete(staging, true); }
                    catch (Exception error) { AddCleanupFailure(ref failure, error, ExitCode.Output, "CORE_STAGING_CLEANUP_FAILED"); }
                }
            }
            if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
            return artifacts;
        }

        static BuildFailure Failure(ExitCode exitCode, string code, Exception error)
        {
            var failure = new BuildFailure(exitCode, code, error.ToString());
            failure.Data["OriginalException"] = error;
            return failure;
        }
        static void AddCleanupFailure(ref Exception primary, Exception secondary, ExitCode exitCode, string code)
        {
            if (primary == null) { primary = Failure(exitCode, code, secondary); return; }
            // Preserve cancellation, the original diagnostic code and its stack.
            // The caller can report secondary failures without losing the cause.
            var failures = primary.Data["CoreCleanupExceptions"] as List<Exception>;
            if (failures == null) { failures = new List<Exception>(); primary.Data["CoreCleanupExceptions"] = failures; }
            failures.Add(secondary);
        }

        static void RejectReparseAncestors(string path)
        {
            for (var directory = new DirectoryInfo(path); directory != null; directory = directory.Parent)
                if (directory.Exists && (directory.Attributes & FileAttributes.ReparsePoint) != 0)
                    throw new BuildFailure(ExitCode.Output, "CORE_STAGING_REPARSE", "The stream workspace cannot use reparse points.");
        }
        static IEnumerable<string> SafeFiles(string directory)
        {
            foreach (string entry in Directory.GetFileSystemEntries(directory).OrderBy(x => x, StringComparer.Ordinal))
            {
                FileAttributes attributes = File.GetAttributes(entry);
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new BuildFailure(ExitCode.Output, "CORE_OUTPUT_REPARSE", "Core output contains a reparse point.");
                if ((attributes & FileAttributes.Directory) != 0) { foreach (string file in SafeFiles(entry)) yield return file; }
                else yield return entry;
            }
        }
        static IList<Artifact> CollectArtifacts(string output)
        {
            var files = SafeFiles(output).ToArray();
            var result = new List<Artifact>();
            string[] manifests = files.Where(x => x.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (manifests.Length == 0) throw new BuildFailure(ExitCode.Output, "CORE_OUTPUT_MISSING", "Core produced no stream manifest.");
            var accepted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string manifest in manifests)
            {
                string stem = manifest.Substring(0, manifest.Length - ".manifest".Length);
                foreach (string extension in new[] { ".manifest", ".bin", ".imp", ".relo" })
                {
                    string file = stem + extension;
                    if (!File.Exists(file)) throw new BuildFailure(ExitCode.Output, "CORE_OUTPUT_INCOMPLETE", "Core output is missing " + file);
                    accepted.Add(file);
                }
            }
            foreach (string file in files)
            {
                string relative = file.Substring(output.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                bool custom = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Any(part => string.Equals(part, "cdata", StringComparison.OrdinalIgnoreCase));
                if (!accepted.Contains(file) && !custom) throw new BuildFailure(ExitCode.Output, "CORE_OUTPUT_UNEXPECTED", "Unexpected file in completed stream output: " + file);
                string sha;
                using (var algorithm = SHA256.Create()) using (var input = File.OpenRead(file)) sha = BitConverter.ToString(algorithm.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
                result.Add(new Artifact { Path = Path.GetFullPath(file), Kind = custom ? "custom-data" : "stream" + Path.GetExtension(file), Size = new FileInfo(file).Length, Sha256 = sha });
            }
            return result;
        }

        void CheckOwner()
        {
            lock (LeaseGate)
            {
                if (disposed) throw new ObjectDisposedException("CoreStreamCompiler");
                if (!initialized) throw new InvalidOperationException("The backend has not been initialized.");
                if (!Object.ReferenceEquals(leaseOwner, this) || threadId != Thread.CurrentThread.ManagedThreadId)
                    throw new InvalidOperationException("The Core session must be used by its owning thread.");
            }
        }
        void ReleaseLease() { lock (LeaseGate) { if (Object.ReferenceEquals(leaseOwner, this)) leaseOwner = null; } }
        public void Dispose()
        {
            lock (LeaseGate)
            {
                if (disposed) return;
                if ((initialized || initializing) && threadId != Thread.CurrentThread.ManagedThreadId)
                    throw new InvalidOperationException("The Core session must be disposed by its owning thread.");
                if (compiling || initializing) throw new InvalidOperationException("Cannot dispose a running or initializing Core session.");
                disposed = true;
            }
            try { DisposeProviders(); }
            finally { ReleaseLease(); }
        }
        void DisposeProviders()
        {
            var failures = new List<Exception>();
            if (verifierDescriptors != null) foreach (PluginDescriptor descriptor in verifierDescriptors)
                try { var owned = descriptor.Plugin as IDisposable; if (owned != null) owned.Dispose(); } catch (Exception error) { failures.Add(error); }
            try { if (compilers != null) compilers.Dispose(); } catch (Exception error) { failures.Add(error); }
            if (failures.Count != 0) throw new AggregateException("Core provider cleanup failed.", failures);
        }
    }
}
#endif
