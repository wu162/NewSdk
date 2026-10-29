using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Xml;

// Read-only diagnostic of the exact accepted production method. No provider,
// compiler initialization, session mutation or application entry point runs.
public static class CompilerCacheIdentityProfile
{
    static void Need(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    static double Time(Action action)
    {
        Stopwatch clock = Stopwatch.StartNew(); action(); clock.Stop();
        return clock.Elapsed.TotalMilliseconds;
    }
    static string Hex(byte[] value) { return BitConverter.ToString(value).Replace("-", "").ToLowerInvariant(); }
    static List<string> Files(string sdk, string runtime)
    {
        List<string> files = new List<string>();
        files.Add(Path.Combine(sdk, "tools", "BinaryAssetBuilder.exe.config"));
        files.AddRange(Directory.GetFiles(Path.Combine(sdk, "Schemas"), "*", SearchOption.AllDirectories));
        foreach (string path in Directory.GetFiles(runtime))
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            if (extension == ".exe" || extension == ".dll" || extension == ".config") files.Add(path);
        }
        files.Sort(StringComparer.Ordinal);
        return files;
    }
    static bool Same(List<string> a, List<string> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++) if (!String.Equals(a[i], b[i], StringComparison.Ordinal)) return false;
        return true;
    }
    static void Property(object target, string name, object value)
    { target.GetType().GetProperty(name).SetValue(target, value, null); }
    static object PreflightRequest(Assembly assembly, string sdk, string mod, string profile)
    {
        object request = Activator.CreateInstance(assembly.GetType("NewModCompiler.Application.BuildRequest", true));
        string output = Path.Combine(Path.GetDirectoryName(profile), "preflight-output");
        Property(request, "Command", "build"); Property(request, "SdkRoot", sdk);
        Property(request, "ModRoot", mod); Property(request, "OutputRoot", output);
        Property(request, "EventsPath", Path.Combine(output, "build.events.jsonl"));
        Property(request, "ReportPath", Path.Combine(output, "build-report.json"));
        Property(request, "LogPath", Path.Combine(output, "build.log"));
        Property(request, "Content", new string[] { "base" }); Property(request, "PackageFormat", "loose");
        Property(request, "Arguments", new string[0]);
        IDictionary options = (IDictionary)request.GetType().GetProperty("Options").GetValue(request, null);
        Type optionType = assembly.GetType("NewModCompiler.Application.EffectiveOption", true);
        Dictionary<string, string> roots = new Dictionary<string, string>(); roots.Add("dr", sdk);
        XmlDocument xml = new XmlDocument(); xml.XmlResolver = null;
        xml.Load(Path.Combine(sdk, "tools", "BinaryAssetBuilder.exe.config"));
        XmlElement section = (XmlElement)xml.SelectSingleNode("/configuration/assetbuilder");
        foreach (string kind in new string[] { "Data", "Art", "Audio" })
        {
            List<string> values = new List<string>(); values.Add(Path.Combine(mod, kind));
            foreach (string path in section.GetAttribute("default" + kind + "Paths").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Need(!String.IsNullOrWhiteSpace(path) && path != "*", "Diagnostic expects concrete ordered SDK search roots");
                values.Add(Path.GetFullPath(Path.Combine(sdk, path)));
            }
            roots.Add(kind.ToLowerInvariant(), String.Join(";", values.ToArray()));
        }
        foreach (KeyValuePair<string, string> root in roots)
        {
            object option = Activator.CreateInstance(optionType);
            Property(option, "Value", root.Value); Property(option, "Source", "commandLine");
            options.Add(root.Key, option);
        }
        return request;
    }
    public static int Main(string[] args)
    {
        if (args.Length != 4) { Console.Error.WriteLine("Expected runtime directory, SDK root, MOD root and profile.json"); return 2; }
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["schemaVersion"] = 1; result["status"] = "started";
        int exit = 1;
        try
        {
            Need(IntPtr.Size == 4, "Diagnostic requires x86 CLR4");
            string runtime = Path.GetFullPath(args[0]), sdk = Path.GetFullPath(args[1]);
            string[] names = Directory.GetFiles(runtime);
            HashSet<string> required = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
                "NewModCompiler.exe", "NewModCompiler.exe.config", "antlr.runtime.dll",
                "BinaryAssetBuilder.TextureCompiler.dll", "BinaryAssetBuilder.TextureCompiler.dll.patch.json" };
            Need(names.Length == 5, "Accepted runtime directory must contain exactly five files");
            foreach (string path in names) Need(required.Remove(Path.GetFileName(path)), "Unexpected runtime file");
            Assembly assembly = null;
            result["assemblyLoadMs"] = Time(delegate { assembly = Assembly.LoadFrom(Path.Combine(runtime, "NewModCompiler.exe")); });
            Type compilerType = assembly.GetType("NewModCompiler.Application.CoreStreamCompiler", true);
            object request = PreflightRequest(assembly, sdk, Path.GetFullPath(args[2]), Path.GetFullPath(args[3]));
            MethodInfo preflight = compilerType.GetMethod("PreflightInputRoots", BindingFlags.Public | BindingFlags.Static);
            result["firstPreflightInputRootsMs"] = Time(delegate { preflight.Invoke(null, new object[] { request }); });
            double[] preflightWarm = new double[5];
            for (int i = 0; i < preflightWarm.Length; i++)
                preflightWarm[i] = Time(delegate { preflight.Invoke(null, new object[] { request }); });
            result["warmPreflightInputRootsMs"] = preflightWarm;
            result["diagnosticOrder"] = "Assembly.LoadFrom; construct shared-root request; preflight first+5warm; identity first+5warm; enumerate/guard/hash components 5each. First means first method invocation in an already loaded helper, not cold process or disk.";
            object compiler = Activator.CreateInstance(compilerType, new object[] { null });
            BindingFlags privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            compilerType.GetField("sdkRoot", privateInstance).SetValue(compiler, sdk);
            Type settingsType = assembly.GetType("BinaryAssetBuilder.Settings", true);
            object settings = Activator.CreateInstance(settingsType);
            settingsType.GetProperty("DataRoot").SetValue(settings, sdk, null);
            MethodInfo identity = compilerType.GetMethod("LegacyCacheIdentity", privateInstance);
            MethodInfo guardMethod = compilerType.GetMethod("RejectReparseAncestors", BindingFlags.Static | BindingFlags.NonPublic);
            Action<string> guard = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), guardMethod);
            Need(identity != null && guard != null, "Expected production diagnostic methods are unavailable");
            string first = null;
            // First invocation deliberately precedes diagnostic enumeration and
            // component passes, so those do not prewarm this first sample.
            result["firstIdentityMs"] = Time(delegate { first = (string)identity.Invoke(compiler, new object[] { settings }); });
            Need(first != null && first.Length == 64, "Production identity did not return a SHA256 digest");
            result["identityDigest"] = first;
            double[] warm = new double[5]; bool stable = true;
            for (int i = 0; i < warm.Length; i++)
            {
                string current = null;
                warm[i] = Time(delegate { current = (string)identity.Invoke(compiler, new object[] { settings }); });
                stable &= String.Equals(first, current, StringComparison.Ordinal);
            }
            result["warmIdentityMs"] = warm; result["identityStable"] = stable;
            Need(stable, "Production identity changed between read-only calls");
            List<string> files = Files(sdk, runtime);
            long totalBytes = 0, schemaBytes = 0, runtimeBytes = 0;
            int schemaCount = 0, runtimeCount = 0;
            string schemas = Path.Combine(sdk, "Schemas") + Path.DirectorySeparatorChar;
            foreach (string path in files)
            {
                long bytes = new FileInfo(path).Length; totalBytes += bytes;
                if (path.StartsWith(schemas, StringComparison.OrdinalIgnoreCase)) { schemaCount++; schemaBytes += bytes; }
                if (String.Equals(Path.GetDirectoryName(path), runtime, StringComparison.OrdinalIgnoreCase)) { runtimeCount++; runtimeBytes += bytes; }
            }
            result["fileCount"] = files.Count; result["fileBytes"] = totalBytes;
            result["schemaFileCount"] = schemaCount; result["schemaFileBytes"] = schemaBytes;
            result["runtimeFileCount"] = runtimeCount; result["runtimeFileBytes"] = runtimeBytes;
            result["sdkConfigurationFileCount"] = 1;
            double[] hashing = new double[5], guarding = new double[5], enumeration = new double[5];
            string hashOnlyDigest = null;
            for (int repeat = 0; repeat < 5; repeat++)
            {
                enumeration[repeat] = Time(delegate { Need(Same(files, Files(sdk, runtime)), "Enumerated production file set changed"); });
                guarding[repeat] = Time(delegate { foreach (string file in files) guard(file); });
                List<byte[]> contentHashes = new List<byte[]>(files.Count);
                hashing[repeat] = Time(delegate {
                    foreach (string file in files)
                        using (SHA256 algorithm = SHA256.Create())
                        using (FileStream input = File.OpenRead(file)) contentHashes.Add(algorithm.ComputeHash(input));
                });
                // Summarize the digests outside the hash-only measurement.
                using (MemoryStream hashes = new MemoryStream())
                {
                    foreach (byte[] value in contentHashes) hashes.Write(value, 0, value.Length);
                    hashes.Position = 0;
                    using (SHA256 algorithm = SHA256.Create()) hashOnlyDigest = Hex(algorithm.ComputeHash(hashes));
                }
            }
            result["enumerateAndOrdinalSortMs"] = enumeration;
            result["guardOnlyMs"] = guarding; result["hashOnlyMs"] = hashing;
            result["hashOnlyDigest"] = hashOnlyDigest;
            result["componentsAreAdditive"] = false;
            result["scope"] = "Exact production PreflightInputRoots and private identity via reflection. Components separately repeat the same file set with production guard delegate; different scheduling/page-cache/JIT conditions mean component timings must not be added or treated as end-to-end compile time. No input/runtime/cache writes.";
            result["status"] = "passed"; exit = 0;
        }
        catch (Exception error) { result["status"] = "failed"; result["error"] = error.ToString(); }
        string json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(result);
        File.WriteAllText(Path.GetFullPath(args[3]), json + Environment.NewLine);
        Console.WriteLine(json);
        return exit;
    }
}
