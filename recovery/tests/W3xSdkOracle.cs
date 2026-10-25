using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

// Real SDK call replay. This host never replaces BAB's preprocessing/XSD path.
internal static class W3xSdkOracle
{
    const string Ns="uri:ea.com:eala:asset";
    const string OriginalName="BinaryAssetBuilder.W3XCompiler";
    static void Schema(string sdk,string modName,string expectedPath)
    {
        var schemas=new XmlSchemaSet();schemas.XmlResolver=new XmlUrlResolver();
        schemas.Add(Ns,Path.Combine(sdk,"Schemas/xsd/CnC3Types.xsd"));schemas.Compile();
        var expected=new SortedSet<string>();int files=0;
        foreach(string path in Directory.GetFiles(Path.Combine(sdk,"Mods/"+modName),"*",SearchOption.AllDirectories)) {
            string ext=Path.GetExtension(path).ToLowerInvariant();if(ext!=".xml" && ext!=".w3x") continue;
            var settings=new XmlReaderSettings();settings.Schemas=schemas;settings.ValidationType=ValidationType.Schema;
            settings.ValidationEventHandler+=delegate(object sender,ValidationEventArgs e){throw new Exception(path+": "+e.Message);};
            using(var reader=XmlReader.Create(path,settings)) while(reader.Read()){}
            var xml=new XmlDocument();xml.Load(path);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",Ns);
            // BAB resolves unaliased FileReference values relative to the declaring
            // document (Core.FileNameResolver.ResolvePath), not /art search paths.
            foreach(XmlElement texture in xml.SelectNodes("//ea:Texture[@File]",ns)) {
                string file=texture.GetAttribute("File");
                if(file.IndexOf(':')<0 && !File.Exists(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path),file))))
                    throw new Exception(path+": missing document-relative Texture file: "+file);
            }
            foreach(XmlElement asset in xml.SelectNodes("//ea:W3DAnimation|//ea:W3DMesh",ns)) {
                string key=asset.LocalName+"\t"+asset.GetAttribute("id");
                if(asset.GetAttribute("id").Length==0 || !expected.Add(key)) throw new Exception("Missing/duplicate W3X asset id: "+key);
            }
            ++files;
        }
        int animations=0,meshes=0;foreach(string key in expected){if(key.StartsWith("W3DAnimation\t"))++animations;else++meshes;}
        if(animations==0 || meshes==0) throw new Exception("SDK probe must contain both W3X root asset types");
        File.WriteAllLines(expectedPath,new List<string>(expected).ToArray());
        Console.WriteLine("PASS complete CnC3Types.xsd schema files={0} animationAssets={1} meshAssets={2}",files,animations,meshes);
    }
    static InstanceDeclaration Declaration(XmlDocument xml)
    {
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",Ns);
        var document=new AssetDeclarationDocument();Type type=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(type);type.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        var declaration=new InstanceDeclaration(document);declaration.XmlNode=xml.DocumentElement;return declaration;
    }
    static byte[] Buffer(BinaryReader reader)
    {
        int length=reader.ReadInt32();if(length<0 || length>reader.BaseStream.Length-reader.BaseStream.Position) throw new Exception("Invalid/truncated SDK trace buffer");
        byte[] bytes=reader.ReadBytes(length);if(bytes.Length!=length)throw new Exception("Truncated SDK trace buffer");return bytes;
    }
    static void Equal(byte[] expected,byte[] actual,string label)
    {
        if(actual==null || expected.Length!=actual.Length)throw new Exception(label+" length differs");
        for(int i=0;i<expected.Length;++i)if(expected[i]!=actual[i])throw new Exception(label+" byte differs at "+i);
    }
    static bool Contains(byte[] container,byte[] value)
    {
        if(value.Length==0)return false;
        for(int i=0;i<=container.Length-value.Length;++i){int j=0;while(j<value.Length && container[i+j]==value[j])++j;if(j==value.Length)return true;}return false;
    }
    static string Hash(byte[] bytes){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static void NoOriginalLoaded()
    {
        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())if(assembly.GetName().Name==OriginalName)throw new Exception("Original W3X assembly loaded during isolated replay");
    }
    static void Replay(bool isolated,string pluginFile,string trace,string modFile,string expectedFile,string reportFile)
    {
        if(IntPtr.Size!=4)throw new Exception("x86 SDK replay host required");
        if(isolated){if(File.Exists(Path.Combine(Path.GetDirectoryName(pluginFile),OriginalName+".dll")))throw new Exception("Original W3X DLL present in isolated tools directory");NoOriginalLoaded();}
        string pluginType=isolated?"Recovered.Compiler.W3x.NativePlugin":"BinaryAssetBuilder.W3XCompiler.Plugin";
        var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(pluginFile).GetType(pluginType,true));
        byte[] mod=File.ReadAllBytes(modFile);var expectedIds=new HashSet<string>(File.ReadAllLines(expectedFile));var seen=new HashSet<string>();
        var report=new StringBuilder();int calls=0;
        string[] traces=Directory.GetFiles(trace,"*.fixtures");Array.Sort(traces,StringComparer.Ordinal);
        foreach(string file in traces)using(var reader=new BinaryReader(File.OpenRead(file))) {
            if(reader.ReadString()!="W3X-FIXTURE-1")throw new Exception("Bad SDK trace header: "+file);
            string route=reader.ReadString();if(route!="animation" && route!="mesh")throw new Exception("Bad SDK trace route: "+route);
            while(reader.BaseStream.Position<reader.BaseStream.Length){
                int platform=reader.ReadInt32();string xmlText=reader.ReadString();byte[] data=Buffer(reader),relo=Buffer(reader),imp=Buffer(reader);
                var xml=new XmlDocument();xml.LoadXml(xmlText);string type=route=="animation"?"W3DAnimation":"W3DMesh";
                if(xml.DocumentElement.LocalName!=type || xml.DocumentElement.NamespaceURI!=Ns)throw new Exception("SDK trace type mismatch");
                string key=type+"\t"+xml.DocumentElement.GetAttribute("id");
                plugin.Initialize(null,(TargetPlatform)platform);AssetBuffer actual=plugin.ProcessInstance(Declaration(xml));
                if(actual==null)throw new Exception(key+" returned null");Equal(data,actual.InstanceData,key+" instance");Equal(relo,actual.RelocationData,key+" relocation");Equal(imp,actual.ImportsData,key+" imports");
                if(!Contains(mod,data))throw new Exception(key+" SDK instance data absent from final mod.bin");
                seen.Add(key);++calls;report.Append(key).Append("\t").Append(platform).Append("\t").Append(Hash(data)).Append("\t").Append(Hash(relo)).Append("\t").Append(Hash(imp)).AppendLine();
                Console.WriteLine("PASS SDK actual {0} bytes={1} isolated={2}",key,data.Length,isolated);
            }
        }
        if(calls==0)throw new Exception("No actual W3X SDK calls traced");
        foreach(string key in expectedIds)if(!seen.Contains(key))throw new Exception("Expected asset had no actual W3X SDK call: "+key);
        if(isolated)NoOriginalLoaded();File.WriteAllText(reportFile,report.ToString());
        Console.WriteLine("PASS W3X actual SDK calls={0} expectedAssets={1} threeBufferComparisons={2}; all emitted instances present in mod.bin; isolated={3}",calls,expectedIds.Count,calls*3,isolated);
    }
    static int Main(string[] args)
    {
        try {
            if(args.Length==4 && args[0]=="schema"){Schema(args[1],args[2],args[3]);return 0;}
            if(args.Length!=6 || (args[0]!="buffers" && args[0]!="isolated"))throw new Exception("Usage: W3xSdkOracle schema sdk mod expected | buffers/isolated plugin trace mod.bin expected report");
            Replay(args[0]=="isolated",args[1],args[2],args[3],args[4],args[5]);return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
