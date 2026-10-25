using System;
using System.IO;
using System.Reflection;
using BinaryAssetBuilder.Core;

// Scope removal is an intentional contract change, not an original-DLL differential.
internal static class AudioExcludedPlatformOracle
{
    private static void Rejected(Type type, object instance, string method, object[] args)
    {
        try { type.GetMethod(method).Invoke(instance, args); }
        catch (TargetInvocationException error) {
            if (error.InnerException is NotSupportedException || error.InnerException is ArgumentOutOfRangeException) return;
            throw;
        }
        throw new Exception("Excluded platform was accepted: " + type.Name + "." + method);
    }
    private static int Main(string[] args)
    {
        try {
            if (args.Length != 1) throw new ArgumentException("Usage: AudioExcludedPlatformOracle recovered.dll");
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.AudioCompiler.dll")))
                throw new Exception("Recovered rejection host must exclude original AudioCompiler DLL");
            Assembly assembly = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            const string ns = "BinaryAssetBuilder.AudioCompiler.";
            TargetPlatform excluded = (TargetPlatform)1;
            Type catalog = assembly.GetType(ns + "AudioCatalog", true);
            Rejected(catalog, null, "Version", new object[] { excluded });
            Rejected(catalog, null, "TypeInformation", new object[] { 0u, excluded });
            Rejected(catalog, null, "PathMusicInformation", new object[] { 2516466693u, excluded });
            Rejected(catalog, null, "AudioFileInformation", new object[] { 376113229u, excluded });
            Type pluginType = assembly.GetType(ns + "Plugin", true);
            using (IDisposable plugin = (IDisposable)Activator.CreateInstance(pluginType)) {
                Rejected(pluginType, plugin, "Initialize", new object[] { null, excluded });
                Rejected(pluginType, plugin, "ReInitialize", new object[] { null, excluded });
            }
            Type compilerType = assembly.GetType(ns + "PcmWaveAudioCompiler", true);
            Rejected(compilerType, Activator.CreateInstance(compilerType), "CompileAudioFile", new object[] { null, excluded });
            Type policyType = assembly.GetType(ns + "AudioEncodingPolicy", true);
            Rejected(policyType, null, "Select", new object[] { 1, "C:\\Audio\\probe.wav", null, null });
            Console.WriteLine("PASS excluded platform: eight catalog/lifecycle/direct-compile/policy rejection contracts; no original comparison or SDK execution");
            return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
