using System;
using System.IO;
using System.Linq;
using NewModCompiler.Application;
using NewModCompiler.Cli;

namespace NewModCompiler.Tests
{
    internal static class JobsOptionsTests
    {
        static int checks;
        static void Check(bool value, string message) { ++checks; if (!value) throw new Exception("Jobs options: " + message); }
        static void Reject(string code, Action action)
        {
            try { action(); throw new Exception("Accepted " + code); }
            catch (BuildFailure error) { Check(error.ExitCode == ExitCode.Arguments && error.Code == code, "Expected " + code + ", got " + error.Code); }
        }
        public static int Run(string root)
        {
            checks = 0;
            string sdk = Path.Combine(root, "sdk"), mod = Path.Combine(root, "mod"), output = Path.Combine(root, "out");
            Directory.CreateDirectory(sdk); Directory.CreateDirectory(mod);
            Func<string[], BuildRequest> parse = extra => Options.Parse(new[] { "plan", "--mod", mod, "--sdk-root", sdk, "--out", output }.Concat(extra).ToArray());
            var defaults = parse(new string[0]);
            Check(defaults.RequestedJobs == 1 && defaults.Options["jobs"].Value == "1" && defaults.Options["jobs"].Source == "default", "Default is one requested job with provenance");
            Check(new BuildRequest().RequestedJobs == 1, "Programmatic request defaults to one job");
            Check(!defaults.Options.ContainsKey("bc") && !defaults.Options.ContainsKey("sc"), "Jobs default does not enable persistent caches");
            foreach (string profile in new[] { "standard", "development", "advanced" }) {
                var selected = parse(new[] { "--profile", profile, "--jobs=3" });
                Check(selected.RequestedJobs == 3 && selected.Options["jobs"].Source == "commandLine", "Profile preserves explicit jobs");
            }
            foreach (string session in new[] { "true", "false" }) foreach (string assets in new[] { "true", "false" }) foreach (string jobs in new[] { "1", "2" }) {
                string[] arguments = { "/sc:" + session, "/bc:" + assets, "--jobs=" + jobs };
                if (session == "true" && jobs == "2") Reject("CORE_SESSION_PARALLEL_UNSUPPORTED", () => parse(arguments));
                else {
                    var selected = parse(arguments);
                    Check(selected.RequestedJobs == int.Parse(jobs) && selected.Options["sc"].Value == session && selected.Options["bc"].Value == assets, "Mode matrix preserves effective cache switches and jobs");
                }
            }
            Check(parse(new[] { "--jobs", "0002" }).RequestedJobs == 2 && parse(new[] { "--jobs=0002" }).Options["jobs"].Value == "2", "Equivalent positive values normalize before conflict checking");
            Check(parse(new[] { "--jobs=02", "--jobs=2" }).RequestedJobs == 2, "Equivalent repeated jobs are accepted");
            Check(parse(new[] { "--jobs=2147483647" }).RequestedJobs == int.MaxValue, "Large requested jobs are represented without allocating workers in parsing");
            foreach (string invalid in new[] { "0", "-1", "+1", "1.0", "1e2", " 2", "2 ", "2147483648", "999999999999999999999999", "true" })
                Reject("CLI_RANGE", () => parse(new[] { "--jobs=" + invalid }));
            Reject("CLI_VALUE", () => parse(new[] { "--jobs=" }));
            Reject("CLI_VALUE", () => parse(new[] { "--jobs" }));
            Reject("CLI_CONFLICT", () => parse(new[] { "--jobs=2", "--jobs=3" }));
            string project = Path.Combine(mod, "newmodcompiler.json");
            File.WriteAllText(project, "{\"jobs\":3}");
            try {
                var saved = parse(new string[0]);
                Check(saved.RequestedJobs == 3 && saved.Options["jobs"].Value == "3" && saved.Options["jobs"].Source == "project", "Numeric project jobs are honored");
                var overridden = parse(new[] { "--jobs=1", "/sc:true" });
                Check(overridden.RequestedJobs == 1 && overridden.Options["jobs"].Source == "commandLine", "CLI overrides project jobs before session compatibility check");
                Reject("CORE_SESSION_PARALLEL_UNSUPPORTED", () => parse(new[] { "/sc:true" }));
                File.WriteAllText(project, "{\"jobs\":\"0004\"}");
                Check(parse(new string[0]).RequestedJobs == 4, "String project jobs use the same normalization");
                File.WriteAllText(project, "{\"jobs\":0}"); Reject("CLI_RANGE", () => parse(new string[0]));
                File.WriteAllText(project, "{\"jobs\":1.5}"); Reject("CLI_RANGE", () => parse(new string[0]));
                File.WriteAllText(project, "{\"jobs\":null}"); Reject("PROJECT_OPTION", () => parse(new string[0]));
            } finally { File.Delete(project); }
            Check(Options.Help.Contains("--jobs=N") && Options.Help.Contains("/sc:true") && Options.AdvancedHelp.Contains("Both caches default off"), "Help exposes jobs, session restriction and unchanged cache defaults");
            return checks;
        }
    }
}
