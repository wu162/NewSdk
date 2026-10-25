using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

internal static class TextureDdsOracle
{
    static int cases;
    static InstanceDeclaration Declaration(string root, string file, string kind, string format, string cdata)
    {
        var xml = new XmlDocument();
        XmlElement node = xml.CreateElement(root, "uri:ea.com:eala:asset"); xml.AppendChild(node);
        node.SetAttribute("id", "DdsProbe"); node.SetAttribute("File", file);
        node.SetAttribute("Type", kind); node.SetAttribute("OutputFormat", format);
        node.SetAttribute("GenerateMipMaps", "true"); node.SetAttribute("AllowAutomaticResize", "true");
        var manager = new XmlNamespaceManager(xml.NameTable); manager.AddNamespace("ea", "uri:ea.com:eala:asset");
        var document = new AssetDeclarationDocument();
        Type stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state, manager);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        var declaration = new InstanceDeclaration(document); declaration.XmlNode = node;
        declaration.CustomDataPath = cdata;
        declaration.Handle.TypeHash = 0x12345678u; declaration.Handle.InstanceHash = 0x9abcdef0u;
        return declaration;
    }
    static string Bytes(byte[] bytes) { return bytes == null ? "NULL" : BitConverter.ToString(bytes); }
    static void Run(IAssetBuilderPlugin plugin, StringBuilder output, string label, InstanceDeclaration declaration)
    {
        output.Append(label).Append('=');
        try
        {
            AssetBuffer value = plugin.ProcessInstance(declaration);
            if (value == null) output.Append("NULL");
            else output.Append(Bytes(value.InstanceData)).Append('/').Append(Bytes(value.RelocationData)).Append('/').Append(Bytes(value.ImportsData));
        }
        catch (BinaryAssetBuilderException e) { output.Append(e.GetType().FullName).Append('/').Append(e.ErrorCode).Append('/').Append(e.Message); }
        catch (Exception e) { output.Append(e.GetType().FullName); }
        output.Append("/cdata=").Append(File.Exists(declaration.CustomDataPath) ? Bytes(File.ReadAllBytes(declaration.CustomDataPath)) : "MISSING").AppendLine();
        cases++;
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 4) throw new Exception("Usage: TextureDdsOracle plugin.dll pluginType transcript.txt workDirectory");
            if (IntPtr.Size != 4) throw new Exception("x86 host required");
            string path = Path.GetFullPath(args[0]), folder = Path.GetDirectoryName(path);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object s, ResolveEventArgs e)
            { string candidate = Path.Combine(folder, new AssemblyName(e.Name).Name + ".dll"); return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null; };
            IAssetBuilderPlugin plugin = (IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(path).GetType(args[1], true));
            string work = Path.GetFullPath(args[3]); Directory.CreateDirectory(work);
            string file = Path.Combine(work, "fixture.dds"), cdata = Path.Combine(work, "fixture.cdata");
            var output = new StringBuilder();
            foreach (int platform in new int[] { 0, 2, 99 })
            {
                plugin.ReInitialize(null, (TargetPlatform)platform);
                foreach (string root in new string[] { "Texture", "OnDemandTexture" })
                foreach (string kind in new string[] { "StandardTexture", "CubeTexture", "VolumeTexture" })
                foreach (string format in new string[] { "DXT1", "DXT1A", "DXT3", "DXT5", "A8R8G8B8", "X8R8G8B8", "R5G6B5" })
                foreach (int length in new int[] { 1, 2, 3, 4, 127, 128, 129, 1025 })
                foreach (bool existing in new bool[] { false, true })
                {
                    byte[] bytes = new byte[length]; for (int i=0;i<length;i++) bytes[i]=(byte)(i*37+13);
                    File.WriteAllBytes(file, bytes);
                    if (File.Exists(cdata)) File.Delete(cdata);
                    if (existing) { byte[] old = new byte[2048]; for (int i=0;i<old.Length;i++) old[i]=(byte)(i*19); File.WriteAllBytes(cdata,old); }
                    Run(plugin, output, platform+"/"+root+"/"+kind+"/"+format+"/"+length+"/"+existing, Declaration(root,file,kind,format,cdata));
                }
                foreach (string root in new string[] { "Texture", "OnDemandTexture" })
                {
                    if (File.Exists(cdata)) File.Delete(cdata);
                    File.WriteAllBytes(file,new byte[0]);
                    Run(plugin,output,platform+"/"+root+"/empty",Declaration(root,file,"StandardTexture","DXT1",cdata));
                    File.Delete(file);
                    Run(plugin,output,platform+"/"+root+"/missing",Declaration(root,file,"StandardTexture","DXT1",cdata));
                }
                Run(plugin,output,platform+"/unknown",Declaration("UnknownTextureType",file,"StandardTexture","DXT1",cdata));
            }
            File.WriteAllText(args[2], output.ToString());
            Console.WriteLine("PASS DDS oracle: "+cases+" cases; three buffers plus complete cdata; PC/PS3/unknown; preexisting tails");
            GC.SuppressFinalize(plugin);
            return 0;
        }
        catch(Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
