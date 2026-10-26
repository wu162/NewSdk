using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using NewModCompiler.Application;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Cli
{
    public static class Options
    {
        static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            {"od","out"}, {"scd","session-cache-directory"}, {"scp","session-cache-directory"},
            {"bcd","build-cache-directory"}, {"bcp","build-cache-directory"}
        };
        static readonly HashSet<string> Values = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "mod","sdk-root","out","events","report","log","package-content","package-format",
            "sp","dr","art","audio","data","iod","session-cache-directory","build-cache-directory",
            "bcn","tp","bps","cpf","tl","el"
        };
        static readonly HashSet<string> Booleans = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "include-additional","sc","bc","csc","psc","fsc","atc","slowclean","pc","ls","ss","oof","vf","oar","osh","oix","mr"
        };
        static readonly HashSet<string> Unsupported = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "gui","res","pers","mp","poe","sf","sh"
        };
        static BuildFailure Bad(string code, string message) { return new BuildFailure(ExitCode.Arguments, code, message); }
        static void Set(IDictionary<string, EffectiveOption> options, string key, string value, string source)
        {
            EffectiveOption old;
            if (options.TryGetValue(key, out old) && old.Source == source && !string.Equals(old.Value, value, StringComparison.Ordinal))
                throw Bad("CLI_CONFLICT", "Conflicting values for " + key + ": " + old.Value + " / " + value);
            options[key] = new EffectiveOption { Value = value, Source = source };
        }
        static string Canonical(string key) { string canonical; return Aliases.TryGetValue(key, out canonical) ? canonical : key.ToLowerInvariant(); }
        static string NormalizeValue(string key, string value)
        {
            if (Booleans.Contains(key)) {
                bool parsed;
                if (!bool.TryParse(value, out parsed)) throw Bad("CLI_BOOLEAN", key + " expects true or false.");
                return parsed ? "true" : "false";
            }
            if (key == "tp") {
                if (!string.Equals(value, "Win32", StringComparison.OrdinalIgnoreCase)) throw Bad("CLI_PLATFORM", "Only Win32 is supported.");
                return "Win32";
            }
            if (key == "package-format") {
                value = value.ToLowerInvariant();
                if (value != "loose" && value != "big") throw Bad("CLI_CAPABILITY", "Package format must be loose or big; BIG supports base/apt/maps.");
            }
            if (key == "package-content") {
                var content = value.Split(',').Select(x => x.Trim().ToLowerInvariant()).ToArray();
                if (content.Length == 0 || content.Any(x => x != "base" && x != "apt" && x != "maps")) throw Bad("CLI_CONTENT", "Content must be a comma separated selection of base,apt,maps.");
                return string.Join(",", new [] {"base","apt","maps"}.Where(x => content.Contains(x)));
            }
            if (key == "tl" || key == "el") {
                int number; int maximum = key == "tl" ? 9 : 1;
                if (!int.TryParse(value, out number) || number < 0 || number > maximum) throw Bad("CLI_RANGE", key + " must be in range 0.." + maximum);
                return number.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return value;
        }
        public static BuildRequest Parse(string[] args)
        {
            if (args.Length == 0) throw Bad("CLI_COMMAND", "Expected build, plan, or doctor. Use --help for syntax.");
            var request = new BuildRequest { Command = args[0].ToLowerInvariant(), Arguments = args };
            if (request.Command != "build" && request.Command != "plan" && request.Command != "doctor") throw Bad("CLI_COMMAND", "Unknown command: " + args[0]);
            var commandLine = new SortedDictionary<string, EffectiveOption>(StringComparer.Ordinal);
            for (int i = 1; i < args.Length; ++i) {
                string arg = args[i], key, value; int split;
                bool legacy = arg.StartsWith("/", StringComparison.Ordinal);
                if (arg.StartsWith("--", StringComparison.Ordinal)) { arg = arg.Substring(2); split = arg.IndexOf('='); }
                else if (legacy) { arg = arg.Substring(1); split = arg.IndexOf(':'); }
                else throw Bad("CLI_POSITIONAL", "Unexpected argument: " + arg);
                key = Canonical(split >= 0 ? arg.Substring(0, split) : arg);
                if (Unsupported.Contains(key)) throw Bad("CLI_UNSUPPORTED", "Legacy option /" + key + " is unsupported by this nonresident command line host.");
                if (!Values.Contains(key) && !Booleans.Contains(key)) throw Bad("CLI_UNKNOWN", "Unknown option: " + key);
                if (split >= 0) value = arg.Substring(split + 1);
                else if (Booleans.Contains(key)) value = "true";
                else { if (++i >= args.Length) throw Bad("CLI_VALUE", "Missing value for " + key); value = args[i]; }
                if (string.IsNullOrWhiteSpace(value)) throw Bad("CLI_VALUE", "Missing value for " + key);
                value = NormalizeValue(key, value);
                // Resolve equivalent path aliases before conflict checking.
                if (IsPath(key) && value != "-") {
                    if (key=="out" || key=="events" || key=="report" || key=="log") {
                        try { DiagnosticDirectory.ValidateSyntax(value); }
                        catch (IOException e) { throw Bad(key=="out" ? "CLI_OUTPUT_PATH" : "CLI_LOG_PATH",e.Message); }
                    }
                    value = FullPath(value);
                }
                Set(commandLine, key, value, "commandLine");
            }
            if (request.Command != "doctor" && !commandLine.ContainsKey("mod")) throw Bad("CLI_MOD", "--mod must name an existing MOD directory.");
            if (commandLine.ContainsKey("mod")) {
                request.ModRoot = commandLine["mod"].Value;
                if (!Directory.Exists(request.ModRoot)) throw Bad("CLI_MOD", "MOD directory does not exist: " + request.ModRoot);
                string preset = Path.Combine(request.ModRoot, "newmodcompiler.json");
                if (File.Exists(preset)) LoadPreset(request.Options, preset);
            }
            foreach (var pair in commandLine) request.Options[pair.Key] = pair.Value;
            Default(request,"include-additional","false"); Default(request, "package-content", "base,apt,maps"); Default(request, "package-format", "loose"); Default(request,"tp","Win32");
            if (!request.Options.ContainsKey("sdk-root")) throw Bad("CLI_SDK", "--sdk-root is required; it is independent of --mod.");
            request.SdkRoot = FullPath(request.Options["sdk-root"].Value);
            if (!Directory.Exists(request.SdkRoot)) throw new BuildFailure(ExitCode.Environment, "SDK_MISSING", "SDK root does not exist: " + request.SdkRoot);
            if (request.Command != "doctor") {
                if (!request.Options.ContainsKey("out")) throw Bad("CLI_OUT", "--out (or /od) is required; output must be separate from inputs.");
                request.OutputRoot = FullPath(request.Options["out"].Value);
                if (Paths.Contains(request.ModRoot, request.OutputRoot) || Paths.Contains(request.OutputRoot, request.ModRoot) || Paths.Contains(request.SdkRoot, request.OutputRoot) || Paths.Contains(request.OutputRoot, request.SdkRoot))
                    throw Bad("CLI_OUTPUT_OVERLAP", "Output must not overlap the MOD or SDK input directories.");
                try {
                    if (File.Exists(request.OutputRoot)) throw Bad("CLI_OUTPUT_PATH", "Output must name a directory, not an existing file.");
                    string output=DiagnosticDirectory.ResolveOutput(request.OutputRoot);
                    string mod=DiagnosticDirectory.ResolveInputDirectory(request.ModRoot), sdk=DiagnosticDirectory.ResolveInputDirectory(request.SdkRoot);
                    if (Paths.Contains(mod,output) || Paths.Contains(output,mod) || Paths.Contains(sdk,output) || Paths.Contains(output,sdk))
                        throw Bad("CLI_OUTPUT_OVERLAP", "Output physically overlaps the MOD or SDK inputs.");
                } catch (IOException e) { throw Bad("CLI_OUTPUT_PATH",e.Message); }
                request.EventsPath = ResolveDiagnostic(request, "events", "build.events.jsonl");
                request.ReportPath = ResolveDiagnostic(request, "report", "build-report.json");
                request.LogPath = ResolveDiagnostic(request, "log", "build.log");
                string[] diagnostics=new [] {request.EventsPath,request.ReportPath,request.LogPath};
                string[] physical=diagnostics.Select(DiagnosticDirectory.ResolveOutput).ToArray();
                string staging=DiagnosticDirectory.ResolveOutput(Path.Combine(request.OutputRoot,".build"));
                string published=DiagnosticDirectory.ResolveOutput(Path.Combine(request.OutputRoot,"published"));
                for (int i=0;i<physical.Length;++i) {
                    if (Paths.Contains(staging,physical[i]) || Paths.Contains(physical[i],staging) ||
                        Paths.Contains(published,physical[i]) || Paths.Contains(physical[i],published))
                        throw Bad("CLI_LOG_COLLISION","Diagnostic files must not overlap staging or publication directories.");
                    for (int j=0;j<i;++j)
                        if (Paths.Contains(physical[i],physical[j]) || Paths.Contains(physical[j],physical[i]))
                            throw Bad("CLI_LOG_COLLISION","Diagnostic paths must be distinct files, without ancestor/descendant overlap.");
                }
            }
            request.Content = request.Options["package-content"].Value.Split(','); request.PackageFormat = request.Options["package-format"].Value; request.IncludeAdditional = request.Options["include-additional"].Value=="true";
            BigPackage.ValidateRequest(request);
            return request;
        }
        static void Default(BuildRequest request, string key, string value) { if (!request.Options.ContainsKey(key)) request.Options[key] = new EffectiveOption { Value = value, Source = "default" }; }
        static string ResolveDiagnostic(BuildRequest request, string key, string fallback)
        {
            Default(request, key, Path.Combine(request.OutputRoot, fallback));
            string path = request.Options[key].Value;
            if (path == "-") throw Bad("CLI_STDOUT_EVENTS", "Streaming to stdout is unavailable until native output isolation is verified.");
            path = FullPath(path);
            if (Paths.Contains(request.ModRoot, path) || Paths.Contains(request.SdkRoot, path)) throw Bad("CLI_LOG_INPUT", "Diagnostic output must not overwrite inputs.");
            try {
                if (Directory.Exists(path)) throw Bad("CLI_LOG_PATH", "Diagnostic output must name a file, not an existing directory.");
                string physical=DiagnosticDirectory.ResolveOutput(path);
                if (Paths.Contains(DiagnosticDirectory.ResolveInputDirectory(request.ModRoot),physical) || Paths.Contains(DiagnosticDirectory.ResolveInputDirectory(request.SdkRoot),physical))
                    throw Bad("CLI_LOG_INPUT","Diagnostic output physically overlaps inputs.");
            } catch (IOException e) { throw Bad("CLI_LOG_PATH",e.Message); }
            return path;
        }
        static bool IsPath(string key) { return new [] {"mod","sdk-root","out","events","report","log"}.Contains(key); }
        static string FullPath(string value) { try { return Path.GetFullPath(value); } catch (Exception e) { throw Bad("CLI_PATH", e.Message); } }
        static void LoadPreset(IDictionary<string, EffectiveOption> options, string file)
        {
            try {
                var values = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(file));
                foreach (var pair in values) {
                    string key = Canonical(pair.Key);
                    if (!new [] {"package-content","package-format","include-additional","tp"}.Contains(key)) throw Bad("PROJECT_OPTION", "Unsupported project key: " + pair.Key);
                    Set(options, key, NormalizeValue(key, Convert.ToString(pair.Value, System.Globalization.CultureInfo.InvariantCulture)), "project");
                }
            } catch (BuildFailure) { throw; } catch (Exception e) { throw Bad("PROJECT_INVALID", "Invalid project preset: " + e.Message); }
        }
#if NEWSdk_MONOLITHIC
        const string RuntimeHelp = "Experimental Win32 in-process backend: loose output with independent opt-in /bc:true build-asset and /sc:true session-package caching. Session packages restore documents and intermediate/cdata assets, then regenerate linked outputs; mixed Texture inputs are rejected with /sc:true. Texture asset caching remains disabled. Cached builds require all resolved inputs within SDK, MOD or compiler-runtime roots. Successful nonempty builds replace the published directory under the output root; controlled finalization failures roll back. Empty optional selections preserve previous output. Compressed, partial and frozen session caches, legacy cache-directory overrides, and unsupported precompiled/output options are explicitly rejected. Plan performs discovery only. Doctor checks the environment without compiling a stream. BIG packaging supports base/apt/maps and publishes an ASCII-named MOD_1.0.big plus skudef; base includes the original low/medium LoD streams. --include-additional true snapshots SDK then MOD Additional files into BIG with original overwrite precedence and *.asset exclusion. Optional precompiled asset merge and neutral-asset repair are not enabled.";
#else
        const string RuntimeHelp = "Standalone application skeleton: plan is available; build requires an integrated in-process backend. Legacy options are retained for backend validation, not proof of integrated capability.";
#endif
        public const string Help = "NewModCompiler build|plan --mod <directory> --sdk-root <directory> --out <directory> [--package-content base,apt,maps] [--package-format loose|big] [--include-additional true|false] [--events <file>] [--report <file>]\nNewModCompiler doctor --sdk-root <directory>\n" + RuntimeHelp;
    }
}
