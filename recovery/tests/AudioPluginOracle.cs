using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

// A process-isolated original/candidate oracle. Payloads and custom data are recorded in full.
internal static class AudioPluginOracle
{
    private static string root;
    private static IAssetBuilderPlugin plugin;
    private static StreamWriter writer;
    private static int count;
    private static string Esc(string value) { return System.Security.SecurityElement.Escape(value); }
    private static string Data(byte[] bytes) { return bytes == null ? "<null>" : Convert.ToBase64String(bytes); }
    private static InstanceDeclaration Declaration(string xmlText, string custom)
    {
        var xml = new XmlDocument(); xml.LoadXml(xmlText);
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        var document = new AssetDeclarationDocument();
        var current = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(current);
        current.GetField("NamespaceManager").SetValue(state, ns);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        var declaration = new InstanceDeclaration(document); declaration.XmlNode = xml.DocumentElement; declaration.CustomDataPath = custom;
        return declaration;
    }
    private static string Element(string type, string attributes)
    {
        return "<" + type + " xmlns='uri:ea.com:eala:asset' id='Fixture' " + attributes + "/>";
    }
    private static void Run(string label, int platform, string xml, bool direct)
    {
        plugin.Initialize(null, (TargetPlatform)platform);
        string custom = Path.Combine(root, "custom.bin");
        if (File.Exists(custom)) File.Delete(custom);
        string result;
        try
        {
            var declaration = Declaration(xml, custom);
            AssetBuffer buffer;
            if (direct)
            {
                string method = "Process" + declaration.XmlNode.LocalName + "Instance";
                buffer = (AssetBuffer)plugin.GetType().GetMethod(method).Invoke(plugin, new object[] { declaration });
            }
            else buffer = plugin.ProcessInstance(declaration);
            result = buffer == null ? "NULL" : "BUFFER:" + Data(buffer.InstanceData) + ":" + Data(buffer.RelocationData) + ":" + Data(buffer.ImportsData);
        }
        catch (Exception ex)
        {
            while (ex is TargetInvocationException && ex.InnerException != null) ex = ex.InnerException;
            result = "ERROR:" + ex.GetType().FullName + ":" + Convert.ToBase64String(Encoding.UTF8.GetBytes(ex.Message));
        }
        result += ":CUSTOM:" + (File.Exists(custom) ? Data(File.ReadAllBytes(custom)) : "<absent>");
        writer.WriteLine(label + ":" + platform + ":" + (direct ? "direct" : "dispatch") + ":" + result); writer.Flush(); count++;
    }
    private static void Wav(string path, int channels, int frames)
    {
        using (var stream = File.Create(path)) using (var b = new BinaryWriter(stream))
        {
            b.Write(Encoding.ASCII.GetBytes("RIFF")); b.Write(36 + channels * frames * 2); b.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
            b.Write(16); b.Write((short)1); b.Write((short)channels); b.Write(22050); b.Write(22050 * channels * 2);
            b.Write((short)(channels * 2)); b.Write((short)16); b.Write(Encoding.ASCII.GetBytes("data")); b.Write(channels * frames * 2);
            short[] samples = { 0, 1, -1, 32767, -32768, 0x1234 };
            for (int i = 0; i < channels * frames; i++) b.Write(samples[i % samples.Length]);
        }
    }
    private static void NativeProbe()
    {
        foreach (int channels in new int[] { 1, 2 }) foreach (int frames in new int[] { 3, 20001 })
        {
            string wave = Path.Combine(root, "pcm-" + channels + "-" + frames + ".wav"); Wav(wave, channels, frames);
            foreach (int platform in new int[] { 0, 1 }) foreach (bool streamed in new bool[] { false, true })
            {
                string attributes = "File='" + Esc(wave) + "' PCCompression='NONE' XenonCompression='NONE' PCQuality='75' XenonQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "' IsStreamedOnXenon='" + streamed.ToString().ToLowerInvariant() + "'";
                Run("pcm-" + channels + "-" + frames + "-" + streamed, platform, Element("AudioFile", attributes), false);
            }
        }
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 5) throw new ArgumentException("Usage: AudioPluginOracle assembly.dll original|recovered fixtures output.txt business|pathmusic|pcm");
            if (IntPtr.Size != 4) throw new Exception("x86 CLR required");
            bool original = args[1] == "original";
            string originalName = "BinaryAssetBuilder.AudioCompiler";
            if (!original && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalName + ".dll"))) throw new Exception("Recovered host contains original target DLL");
            var assembly = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            plugin = (IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.AudioCompiler.Plugin", true));
            root = Path.GetFullPath(args[2]); Directory.CreateDirectory(root);
            using (writer = new StreamWriter(args[3], false, new UTF8Encoding(false)))
            {
                if (args[4] == "pcm") NativeProbe();
                else
                {
                    string map = Path.Combine(root, "map.mpf"), track = Path.Combine(root, "track.mus"), header = Path.Combine(root, "headers.h");
                    File.WriteAllBytes(map, new byte[] { 0, 1, 2, 0xff, 0xfe, 0x55, 0x80 });
                    File.WriteAllBytes(track, new byte[] { 0x50, 0x41, 0x54, 0x48, 0, 0xff, 0x34 });
                    File.WriteAllText(header, "#define PATH_EVENT_Fixture 0x12345678\r\n#define PATH_TRACK_Fixture 0x0012D687\r\n");
                    string mono = Path.Combine(root, "mono.mp3"), stereo = Path.Combine(root, "stereo.mp3");
                    File.WriteAllBytes(mono, Convert.FromBase64String(AudioMp3Fixtures.Mono)); File.WriteAllBytes(stereo, Convert.FromBase64String(AudioMp3Fixtures.Stereo));
                    foreach (int platform in new int[] { 0, 1, 2, 99 }) foreach (bool direct in new bool[] { false, true })
                    {
                        Run("map", platform, Element("PathMusicMap", "File='" + Esc(map) + "'"), direct);
                        Run("map-missing", platform, Element("PathMusicMap", "File='" + Esc(map + ".missing") + "'"), direct);
                        foreach (string type in new string[] { "MAIN", "OVERLAY_0" })
                            Run("track-" + type, platform, Element("PathMusicTrack", "File='" + Esc(track) + "' Type='" + type + "' PathfinderTrackHeader='" + Esc(header) + "'"), direct);
                        foreach (bool xfer in new bool[] { false, true })
                        {
                            Run("event-" + xfer, platform, Element("PathMusicEvent", "PathfinderEventHeader='" + Esc(header) + "' IsXferable='" + xfer.ToString().ToLowerInvariant() + "'"), direct);
                            Run("event-reference-" + xfer, platform, Element("PathMusicEvent", "PathfinderEventHeader='" + Esc(header) + "' IsXferable='" + xfer.ToString().ToLowerInvariant() + "' XferAlternateEvent='OtherEvent'"), direct);
                        }
                        Run("event-missing", platform, Element("PathMusicEvent", "PathfinderEventHeader='" + Esc(header + ".missing") + "' IsXferable='true'"), direct);
                        if (args[4] != "pathmusic") {
                        foreach (string file in new string[] { mono, stereo }) foreach (bool streamed in new bool[] { false, true })
                        {
                            Run("mp3-" + Path.GetFileName(file) + "-" + streamed, platform, Element("AudioFileMP3Passthrough", "File='" + Esc(file) + "' IsStreamed='" + streamed.ToString().ToLowerInvariant() + "'"), direct);
                            Run("mp3-subtitle-" + Path.GetFileName(file) + "-" + streamed, platform, Element("AudioFileMP3Passthrough", "File='" + Esc(file) + "' IsStreamed='" + streamed.ToString().ToLowerInvariant() + "' SubtitleStringName='ExplicitSubtitle'"), direct);
                        }
                        Run("mp3-not-mp3", platform, Element("AudioFileMP3Passthrough", "File='" + Esc(track) + "' IsStreamed='true'"), direct);
                        }
                    }
                }
            }
            ((IDisposable)plugin).Dispose();
            if (!original && AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == originalName)) throw new Exception("Original target assembly loaded in recovered host");
            Console.WriteLine("PASS Audio plugin oracle: " + count + " recorded cases; scope=" + args[4] + "; output comparison follows separately"); return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
