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
        if ((label.StartsWith("map") || label.StartsWith("track") || label.StartsWith("event")) && !result.StartsWith("BUFFER:"))
            throw new Exception("Required PathMusic fixture failed: " + label + " platform=" + platform + " result=" + result);
        if ((label.StartsWith("pcm-") || label.StartsWith("xas-") || label.StartsWith("default-")) && (!result.StartsWith("BUFFER:") || !File.Exists(custom)))
            throw new Exception("Required AudioFile fixture failed: " + label + " platform=" + platform + " result=" + result);
        bool validMp3 = label.StartsWith("mp3-mono.") || label.StartsWith("mp3-stereo.") || label.StartsWith("mp3-subtitle-");
        if (validMp3)
        {
            if (!result.StartsWith("BUFFER:") || !File.Exists(custom)) throw new Exception("Required valid MP3 fixture failed: " + label + " result=" + result);
            var source = new XmlDocument(); source.LoadXml(xml);
            byte[] expected = File.ReadAllBytes(source.DocumentElement.Attributes["File"].Value);
            if (!expected.SequenceEqual(File.ReadAllBytes(custom))) throw new Exception("MP3 custom data did not preserve the exact input bytes: " + label);
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
    private unsafe delegate bool HeaderParser(sbyte* prefix, sbyte* line, sbyte* name, int* found);
    private static unsafe void ParseProbe()
    {
        var method = plugin.GetType().GetMethod("ParsePathMusicHeaderLine");
        var parse = (HeaderParser)Delegate.CreateDelegate(typeof(HeaderParser), plugin, method);
        string[] cases = {
            "#define PATH_EVENT_Fixture 0x12345678", "#define PATH_EVENT_Fixture 0x0",
            "#define PATH_EVENT_Fixture 0x7fffffff", "#define PATH_EVENT_Fixture 0x80000000",
            "#define PATH_EVENT_Fixture 0xffffffff", "#define PATH_EVENT_Fixture 0x123456789",
            "#define PATH_EVENT_Fixture 12345678", "#define PATH_EVENT_Fixture 0X1234",
            "#define PATH_EVENT_Fixture 0x", "#define PATH_EVENT_Fixture 0xNOTHEX",
            "#define PATH_EVENT_Fixture 0x12junk", "#define PATH_EVENT_Fixture\t0x1234",
            "#define PATH_EVENT_Fixture  0x1234", "prefix #define PATH_EVENT_Fixture 0x3456",
            "#define PATH_EVENT_Other 0x12", "#define PATH_TRACK_Fixture 0x12",
            "#DEFINE PATH_EVENT_Fixture 0x12", "#define PATH_EVENT_fixture 0x12",
            "#define PATH_EVENT_FixtureSuffix 0x12", "#define PATH_EVENT_ 0x12", "", "#define",
            "// 0xA #define PATH_EVENT_Fixture 0xBB"
        };
        byte[] prefix = Encoding.ASCII.GetBytes("PATH_EVENT_\0"), name = Encoding.ASCII.GetBytes("Fixture\0");
        for (int i = 0; i < cases.Length; ++i)
        {
            byte[] line = Encoding.ASCII.GetBytes(cases[i] + "\0");
            int value = unchecked((int)0xa1b2c3d4u); bool result;
            fixed (byte* p = prefix) fixed (byte* l = line) fixed (byte* n = name)
                result = parse((sbyte*)p, (sbyte*)l, (sbyte*)n, &value);
            writer.WriteLine("parse:" + i + ":" + result + ":" + value); count++;
        }
    }
    private static void NativeProbe(bool xas)
    {
        foreach (int channels in new int[] { 1, 2 }) foreach (int frames in (xas ? new int[] {128,129,4096} : new int[] { 3, 20001 }))
        {
            string wave = Path.Combine(root, "pcm-" + channels + "-" + frames + ".wav"); Wav(wave, channels, frames);
            foreach (int platform in (xas ? new int[] {0} : new int[] { 0, 1 })) foreach (bool streamed in new bool[] { false, true })
            {
                string attributes = "File='" + Esc(wave) + "' PCCompression='"+(xas?"XAS":"NONE")+"' XenonCompression='NONE' PCQuality='75' XenonQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "' IsStreamedOnXenon='" + streamed.ToString().ToLowerInvariant() + "'";
                Run((xas?"xas-":"pcm-") + channels + "-" + frames + "-" + streamed, platform, Element("AudioFile", attributes), false);
            }
        }
    }
    private static void DefaultProbe()
    {
        foreach (string directory in new string[] { "sounds", "music" })
        {
            string folder=Path.Combine(root,directory); Directory.CreateDirectory(folder);
            string wave=Path.Combine(folder,"default.wav"); Wav(wave,directory=="sounds"?2:1,257);
            foreach(bool direct in new bool[]{false,true})
                Run("default-"+directory,0,Element("AudioFile","File='"+Esc(wave)+"' PCQuality='75' XenonQuality='75' PS3Quality='75'"),direct);
        }
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 5) throw new ArgumentException("Usage: AudioPluginOracle assembly.dll original|recovered fixtures output.txt business|pathmusic|pcm|xas|defaults");
            if (IntPtr.Size != 4) throw new Exception("x86 CLR required");
            bool original = args[1] == "original";
            string originalName = "BinaryAssetBuilder.AudioCompiler";
            if (!original && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalName + ".dll"))) throw new Exception("Recovered host contains original target DLL");
            var assembly = Assembly.LoadFrom(Path.GetFullPath(args[0]));
            plugin = (IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.AudioCompiler.Plugin", true));
            root = Path.GetFullPath(args[2]); Directory.CreateDirectory(root);
            using (writer = new StreamWriter(args[3], false, new UTF8Encoding(false)))
            {
                if (args[4] == "pcm" || args[4] == "xas") NativeProbe(args[4] == "xas");
                else if (args[4] == "defaults") DefaultProbe();
                else
                {
                    ParseProbe();
                    string map = Path.Combine(root, "map.mpf"), track = Path.Combine(root, "track.mus"), header = Path.Combine(root, "headers.h");
                    File.WriteAllBytes(map, new byte[] { 0, 1, 2, 0xff, 0xfe, 0x55, 0x80 });
                    File.WriteAllBytes(track, new byte[] { 0x50, 0x41, 0x54, 0x48, 0, 0xff, 0x34 });
                    File.WriteAllText(header, "#define PATH_EVENT_Fixture 0x12345678\r\n#define PATH_TRACK_Fixture 0x0012D687\r\n");
                    string mono = Path.Combine(root, "mono.mp3"), stereo = Path.Combine(root, "stereo.mp3");
                    File.WriteAllBytes(mono, Convert.FromBase64String(AudioMp3Fixtures.Mono)); File.WriteAllBytes(stereo, Convert.FromBase64String(AudioMp3Fixtures.Stereo));
                    byte[] monoBytes = Convert.FromBase64String(AudioMp3Fixtures.Mono);
                    byte[] id3 = new byte[monoBytes.Length + 10]; Encoding.ASCII.GetBytes("ID3").CopyTo(id3,0); id3[3]=4; monoBytes.CopyTo(id3,10);
                    File.WriteAllBytes(Path.Combine(root,"id3.mp3"),id3);
                    File.WriteAllBytes(Path.Combine(root,"empty.mp3"),new byte[0]);
                    File.WriteAllBytes(Path.Combine(root,"truncated.mp3"),monoBytes.Take(100).ToArray());
                    byte[] invalid = (byte[])monoBytes.Clone(); invalid[0]=0; File.WriteAllBytes(Path.Combine(root,"invalid-first.mp3"),invalid);
                    File.WriteAllBytes(Path.Combine(root,"double.mp3"),monoBytes.Concat(monoBytes).ToArray());
                    Wav(Path.Combine(root,"wave.mp3"),1,3);
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
                        foreach(string edge in new string[]{"id3.mp3","empty.mp3","truncated.mp3","invalid-first.mp3","double.mp3","wave.mp3"})
                            Run("mp3-edge-"+edge,platform,Element("AudioFileMP3Passthrough","File='"+Esc(Path.Combine(root,edge))+"' IsStreamed='true'"),direct);
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
