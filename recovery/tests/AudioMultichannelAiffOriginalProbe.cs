using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

// One bounded original ProcessInstance invocation per process. This is evidence
// discovery, never candidate parity or SDK acceptance.
internal static class AudioMultichannelAiffOriginalProbe
{
    [DllImport("kernel32.dll")] private static extern uint SetErrorMode(uint mode);
    private static string Data(byte[] bytes) { return bytes == null ? null : Convert.ToBase64String(bytes); }
    private static void BE(BinaryWriter writer, uint value, int bytes)
    { for (int i = bytes - 1; i >= 0; --i) writer.Write((byte)(value >> (i * 8))); }
    private static void Aiff(string path, string container, int channels, int frames, int bits, string signal)
    {
        int payload = channels * frames * (bits / 8);
        using (var file = File.Create(path)) using (var writer = new BinaryWriter(file)) {
            writer.Write(Encoding.ASCII.GetBytes("FORM")); BE(writer, 0, 4);
            writer.Write(Encoding.ASCII.GetBytes(container));
            writer.Write(Encoding.ASCII.GetBytes("COMM")); BE(writer, container == "AIFC" ? 24u : 18u, 4);
            BE(writer, (uint)channels, 2); BE(writer, (uint)frames, 4); BE(writer, (uint)bits, 2);
            writer.Write(new byte[] { 0x40,0x0d,0xac,0x44,0,0,0,0,0,0 }); // 22050Hz extended80
            if (container == "AIFC") { writer.Write(Encoding.ASCII.GetBytes("NONE")); writer.Write((ushort)0); }
            writer.Write(Encoding.ASCII.GetBytes("SSND")); BE(writer, (uint)(payload + 8), 4);
            BE(writer, 0, 4); BE(writer, 0, 4); // offset/blockSize
            for (int frame = 0; frame < frames; ++frame) for (int channel = 0; channel < channels; ++channel) {
                short sample = (short)(signal == "constant" ? (channel + 1) * 2048 :
                    (frame == channel * 11 + 1 ? 12000 + channel * 512 : 0));
                if (bits == 8) writer.Write(unchecked((byte)(sbyte)(sample >> 8)));
                else if (bits == 16) BE(writer, unchecked((ushort)sample), 2);
                else BE(writer, unchecked((uint)((sample << 8) | ((frame + channel) & 255))), 3);
            }
            if ((payload & 1) != 0) writer.Write((byte)0);
            writer.Flush(); long size = file.Length; file.Position = 4; BE(writer, checked((uint)(size - 8)), 4);
        }
    }
    private static InstanceDeclaration Declaration(string path, string custom, string codec, bool streamed)
    {
        var xml = new XmlDocument();
        xml.LoadXml("<AudioFile xmlns='uri:ea.com:eala:asset' id='MultichannelProbe' File='" +
            System.Security.SecurityElement.Escape(path) + "' PCCompression='" + codec +
            "' PCQuality='75' IsStreamedOnPC='" + streamed.ToString().ToLowerInvariant() + "'/>");
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        var document = new AssetDeclarationDocument();
        Type current = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(current);
        current.GetField("NamespaceManager").SetValue(state, ns);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        return new InstanceDeclaration(document) { XmlNode = xml.DocumentElement, CustomDataPath = custom };
    }
    private static int Main(string[] args)
    {
        SetErrorMode(0x0001 | 0x0002); // Suppress dialogs; parent records crashes/timeouts.
        if (args.Length != 10 || IntPtr.Size != 4) return 2;
        string assembly = Path.GetFullPath(args[0]), directory = Path.GetFullPath(args[1]), output = Path.GetFullPath(args[2]);
        int channels = Int32.Parse(args[3]), frames = Int32.Parse(args[4]), bits = Int32.Parse(args[7]); string codec = args[5], signal = args[8], container = args[9]; bool streamed = Boolean.Parse(args[6]);
        if ((channels != 1 && channels != 2 && channels != 4 && channels != 6) || frames != 129 || (container != "AIFF" && container != "AIFC") || (bits != 8 && bits != 16 && bits != 24) || (signal != "constant" && signal != "impulse") || (codec != "NONE" && codec != "XAS")) return 2;
        Directory.CreateDirectory(directory); Environment.CurrentDirectory = directory;
        string wave = Path.Combine(directory, container == "AIFF" ? "input.aiff" : "input.aifc"), custom = Path.Combine(directory, "output.cdata");
        Aiff(wave, container, channels, frames, bits, signal); if (File.Exists(custom)) File.Delete(custom);
        var result = new Dictionary<string, object> { { "channels", channels }, { "frames", frames }, { "codec", codec }, { "streamed", streamed }, { "platform", "Win32" }, { "bits", bits }, { "signal", signal }, { "container", container }, { "compression", "NONE" } };
        using (var sha = System.Security.Cryptography.SHA256.Create())
            result["inputSha256"] = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(wave))).Replace("-", "").ToLowerInvariant();
        IAssetBuilderPlugin plugin = null;
        try {
            plugin = (IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(assembly).GetType("BinaryAssetBuilder.AudioCompiler.Plugin", true));
            plugin.Initialize(null, TargetPlatform.Win32);
            AssetBuffer buffer = plugin.ProcessInstance(Declaration(wave, custom, codec, streamed));
            result["status"] = buffer == null ? "NULL" : "BUFFER";
            if (buffer != null) {
                result["instance"] = Data(buffer.InstanceData); result["relocation"] = Data(buffer.RelocationData); result["imports"] = Data(buffer.ImportsData);
                if (buffer.InstanceData != null && buffer.InstanceData.Length >= 29) {
                    result["outputChannels"] = buffer.InstanceData[28];
                    result["outputFrames"] = BitConverter.ToUInt32(buffer.InstanceData, 12);
                    result["outputSampleRate"] = BitConverter.ToUInt32(buffer.InstanceData, 16);
                }
            }
        } catch (Exception error) {
            while (error is TargetInvocationException && error.InnerException != null) error = error.InnerException;
            result["status"] = "ERROR"; result["errorType"] = error.GetType().FullName; result["error"] = error.Message;
        } finally {
            result["customPresent"] = File.Exists(custom);
            result["custom"] = File.Exists(custom) ? Data(File.ReadAllBytes(custom)) : null;
            // Persist a managed outcome before native Dispose; parent separately
            // records a failing exit so a teardown crash cannot count as success.
            File.WriteAllText(output, new JavaScriptSerializer().Serialize(result));
            if (plugin != null) ((IDisposable)plugin).Dispose();
        }
        return 0;
    }
}
