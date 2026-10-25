using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BinaryAssetBuilder.Core;

// Executes one side per process so the recovered host cannot bind to the original DLL.
// This tests the metadata catalog only; native initialization and audio payloads are outside this scope.
internal static class AudioMetadataOracle
{
    private static readonly uint[] Known = { 376113229u, 364756738u, 2516466693u, 2590317961u, 2802565590u };
    private static string Encode(object value)
    {
        if (value == null) return "<null>";
        string text = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
    }
    private static string Invoke(Func<object> action)
    {
        try
        {
            object value = action();
            var info = value as ExtendedTypeInformation;
            if (info == null) return "VALUE:" + Encode(value);
            return "INFO:" + String.Join(",", typeof(ExtendedTypeInformation).GetFields().OrderBy(f => f.Name)
                .Select(f => f.Name + "=" + Encode(f.GetValue(info))).ToArray());
        }
        catch (Exception ex)
        {
            while (ex is TargetInvocationException && ex.InnerException != null) ex = ex.InnerException;
            var property = ex.GetType().GetProperty("ErrorCode");
            return "ERROR:" + ex.GetType().FullName + ":" + Encode(ex.Message) + ":" + Encode(property == null ? null : property.GetValue(ex, null));
        }
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 3 || (args[1] != "original" && args[1] != "catalog"))
                throw new ArgumentException("Usage: AudioMetadataOracle assembly.dll original|catalog output.txt");
            if (IntPtr.Size != 4) throw new Exception("x86 host is required");
            bool original = args[1] == "original";
            if (!original && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.AudioCompiler.dll")))
                throw new Exception("Recovered host must not contain the original AudioCompiler DLL");
            var assembly = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            var type = assembly.GetType("BinaryAssetBuilder.AudioCompiler." + (original ? "Plugin" : "AudioCatalog"), true);
            object plugin = original ? Activator.CreateInstance(type) : null;
            int records = 0;
            using (var writer = new StreamWriter(args[2], false, new System.Text.UTF8Encoding(false)))
            {
                Func<string, object[], object> call = (name, values) => type.GetMethod(name).Invoke(plugin, values);
                writer.WriteLine("all:" + Invoke(() => original ? call("GetAllTypesHash", new object[0]) : type.GetField("AllTypesHash").GetValue(null))); records++;
                foreach (int platform in new int[] { 0 })
                {
                    var target = (TargetPlatform)platform;
                    if (original) call("Initialize", new object[] { null, target });
                    writer.WriteLine("version:" + platform + ":" + Invoke(() => original ? call("GetVersionNumber", new object[0]) : call("Version", new object[] { target }))); records++;
                    var ids = Known.Concat(new uint[] { 0u, UInt32.MaxValue }).Concat(Enumerable.Range(0, 4096).Select(i => unchecked((uint)i * 2654435761u)));
                    foreach (uint id in ids)
                    {
                        foreach (string method in new string[] { "GetExtendedTypeInformation", "GetAudioFileExtendedTypeInformation", "GetPathMusicExtendedTypeInformation" })
                        {
                            string recovered = method == "GetExtendedTypeInformation" ? "TypeInformation" : method == "GetAudioFileExtendedTypeInformation" ? "AudioFileInformation" : "PathMusicInformation";
                            writer.WriteLine(platform + ":" + method + ":" + id + ":" + Invoke(() => original ? call(method, new object[] { id }) : call(recovered, new object[] { id, target }))); records++;
                        }
                    }
                    // Each query must produce an independent mutable result.
                    object first = original ? call("GetExtendedTypeInformation", new object[] { Known[0] }) : call("TypeInformation", new object[] { Known[0], target });
                    object second = original ? call("GetExtendedTypeInformation", new object[] { Known[0] }) : call("TypeInformation", new object[] { Known[0], target });
                    if (Object.ReferenceEquals(first, second)) throw new Exception("Shared mutable metadata result");
                }
            }
            if (original) ((IDisposable)plugin).Dispose();
            if (!original && AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "BinaryAssetBuilder.AudioCompiler"))
                throw new Exception("Original AudioCompiler assembly loaded in recovered host");
            Console.WriteLine("PASS Audio metadata: {0} records; native handlers and SDK not tested", records);
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
