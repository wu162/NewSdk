using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

class GameObjectSdkOracle
{
    const string NS = "uri:ea.com:eala:asset";
    static InstanceDeclaration Declaration(XmlDocument xml) {
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", NS);
        var doc = new AssetDeclarationDocument();
        var stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        var state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state, ns);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(doc, state);
        var declaration = new InstanceDeclaration(doc); declaration.XmlNode = xml.DocumentElement; return declaration;
    }
    static void Equal(byte[] a, byte[] b, string label) {
        if(a.Length != b.Length) throw new Exception(label+" length");
        for(int i=0;i<a.Length;++i) if(a[i]!=b[i]) throw new Exception(label+" byte "+i);
    }
    static bool Contains(byte[] haystack,byte[] needle) {
        for(int i=0;i<=haystack.Length-needle.Length;++i) {int j=0;while(j<needle.Length&&haystack[i+j]==needle[j])++j;if(j==needle.Length)return true;}return false;
    }
    static int Main(string[] args) {
        try {
            if(args[0]=="schema") {
                var schemas=new XmlSchemaSet(); schemas.XmlResolver=new XmlUrlResolver();
                schemas.Add(NS,Path.Combine(args[1],"Schemas/xsd/CnC3Types.xsd")); schemas.Compile();
                foreach(string file in Directory.GetFiles(Path.Combine(args[1],"Mods/recoverydraw/Data"),"*.xml")) {
                    var settings=new XmlReaderSettings(); settings.Schemas=schemas; settings.ValidationType=ValidationType.Schema;
                    settings.ValidationEventHandler+=delegate(object sender,ValidationEventArgs e){throw new Exception(file+": "+e.Message);};
                    using(var r=XmlReader.Create(file,settings))while(r.Read()){}
                }
                var probe=new XmlDocument();probe.Load(Path.Combine(args[1],"Mods/recoverydraw/Data/RecoveryDrawProbe.xml"));
                foreach(string invalidCase in new[]{"missing-state-kind","invalid-animation-mode","invalid-shader-bool"}) {
                    var bad=(XmlDocument)probe.CloneNode(true);var ns=new XmlNamespaceManager(bad.NameTable);ns.AddNamespace("ea",NS);
                    if(invalidCase=="missing-state-kind") ((XmlElement)bad.SelectSingleNode("//ea:ModelConditionState",ns)).RemoveAttribute("ParseCondStateType");
                    if(invalidCase=="invalid-animation-mode") ((XmlElement)bad.SelectSingleNode("//ea:Animation",ns)).SetAttribute("AnimationMode","INVALID_MODE");
                    if(invalidCase=="invalid-shader-bool") bad.SelectSingleNode("//ea:Material/ea:Constants/ea:Bool/ea:Value",ns).InnerText="invalid-bool";
                    var settings=new XmlReaderSettings();settings.Schemas=schemas;settings.ValidationType=ValidationType.Schema;bool rejected=false;
                    settings.ValidationEventHandler+=delegate(object sender,ValidationEventArgs e){if(e.Severity==XmlSeverityType.Error)rejected=true;};
                    using(var r=XmlReader.Create(new StringReader(bad.OuterXml),settings))while(r.Read()){}
                    if(!rejected)throw new Exception("Complete XSD accepted "+invalidCase);
                    Console.WriteLine("PASS XSD rejects "+invalidCase);
                }
                Console.WriteLine("PASS complete XSD validates recoverydraw mod and all seven assets"); return 0;
            }
            bool isolated=args[0]=="isolated";
            if(isolated) foreach(string name in new[]{"BinaryAssetBuilder.Tokenizer.dll","BinaryAssetBuilder.XmlCompiler.dll"})
                if(File.Exists(Path.Combine(Path.GetDirectoryName(args[1]),name)))throw new Exception("Original DLL present "+name);
            var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[1]).GetType(isolated?"Recovered.GameObject.Plugin":"BinaryAssetBuilder.Tokenizer.Plugin"));
            plugin.Initialize(null,TargetPlatform.Win32);
            var seen=new HashSet<string>(); var types=new HashSet<string>();byte[] bin=File.ReadAllBytes(args[3]); int calls=0;
            foreach(string file in Directory.GetFiles(args[2],"*.xml")) {
                var xml=new XmlDocument();xml.Load(file); string id=xml.DocumentElement.GetAttribute("id");
                var actual=plugin.ProcessInstance(Declaration(xml));string stem=Path.Combine(Path.GetDirectoryName(file),Path.GetFileNameWithoutExtension(file));
                byte[] data=File.ReadAllBytes(stem+".bin"); Equal(actual.InstanceData,data,id+" instance");Equal(actual.RelocationData,File.ReadAllBytes(stem+".relo"),id+" relo");Equal(actual.ImportsData,File.ReadAllBytes(stem+".imp"),id+" imp");
                if(xml.DocumentElement.GetAttribute("TypeId")!="2486173485")throw new Exception("Missing BAB GameObject TypeId "+id);
                seen.Add(id);
                if(id=="mytank"&&!Contains(bin,data))throw new Exception("mytank absent from final mod.bin");
                if(id.StartsWith("RecoveryDraw_")) {
                    if(!Contains(bin,data))throw new Exception("Asset absent from final mod.bin "+id);seen.Add(id);
                    var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",NS);
                    foreach(XmlElement draw in xml.SelectNodes("/ea:GameObject/ea:Draws/*",ns)) {if(!draw.HasAttribute("TypeId"))throw new Exception("Missing module TypeId "+id);types.Add(draw.LocalName);}
                    if(id=="RecoveryDraw_Inherited"&&(xml.DocumentElement.HasAttribute("inheritFrom")||xml.DocumentElement.GetAttribute("BuildTime")!="9"))throw new Exception("Inheritance not resolved");
                }
                ++calls;
            }
            if(args[4]=="recoverydraw") {
                foreach(string id in new[]{"ScriptedModelDraw","SpotlightDraw","TankDraw","TruckDraw","DynamicsDraw","Default","Inherited"})
                    if(!seen.Contains("RecoveryDraw_"+id))throw new Exception("Missing recovered SDK call "+id);
                foreach(string type in new[]{"ScriptedModelDraw","SpotlightDraw","TankDraw","TruckDraw","DynamicsDraw"})if(!types.Contains(type))throw new Exception("Missing draw module "+type);
            } else if(!seen.Contains("mytank")) throw new Exception("No recovered SDK call for mytank");
            if(isolated)foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())if(assembly.GetName().Name=="BinaryAssetBuilder.Tokenizer"||assembly.GetName().Name=="BinaryAssetBuilder.XmlCompiler")throw new Exception("Original compiler loaded");
            Console.WriteLine("PASS "+calls+" actual SDK calls, all three buffers equal, isolated="+isolated+", mod="+args[4]); return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
