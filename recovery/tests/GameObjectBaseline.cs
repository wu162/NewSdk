using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

// Characterization of the original DLL, not a recovered GameObject compiler.
class GameObjectBaseline
{
    static InstanceDeclaration Declaration(string attributes) {
        var xml=new XmlDocument();xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='OriginalBaseline' "+attributes+"/>");
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea","uri:ea.com:eala:asset");
        var doc=new AssetDeclarationDocument();var st=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        var state=FormatterServices.GetUninitializedObject(st);st.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);
        var declaration=new InstanceDeclaration(doc);declaration.XmlNode=xml.DocumentElement;return declaration;
    }
    static uint Word(byte[] b,int at,bool big) {return big?((uint)b[at]<<24)|((uint)b[at+1]<<16)|((uint)b[at+2]<<8)|b[at+3]:BitConverter.ToUInt32(b,at);}
    static void Equal(byte[] a,byte[] b) {if(a.Length!=b.Length)throw new Exception("Nondeterministic original length");for(int i=0;i<a.Length;++i)if(a[i]!=b[i])throw new Exception("Nondeterministic original byte: "+i);}
    static int Main(string[] args) {
        try {
            Directory.CreateDirectory(args[1]);
            var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[0]).GetType("BinaryAssetBuilder.Tokenizer.Plugin"));
            string[] names={"FenceWidth","FenceXOffset","RemoveTerrainRadius","EmotionRange","FactoryExitWidth","FactoryExtraBibWidth","BountyValue","BuildTime","EnergyProduction","EnergyBonus","VoicePriority","Scale","HealthBoxHeightOffset","LiveCameraFXPitch","FormationWidth","CampnessValueRadius"};
            int[] offsets={84,88,92,96,108,112,132,136,140,144,168,188,192,196,200,184};
            bool[] signed={false,false,false,false,false,false,true,false,true,true,true,false,false,false,true,false};
            var schema=new XmlDocument();schema.Load(args[2]);
            var symbols=new Dictionary<uint,string>();
            foreach(XmlElement element in schema.SelectNodes("//*[@name]")) {
                string name=element.GetAttribute("name");uint hash=HashProvider.GetCaseSensitiveSymbolHash(name);
                if(symbols.ContainsKey(hash) && symbols[hash]!=name) throw new Exception("Schema symbol hash collision");
                symbols[hash]=name;
            }
            int fixtures=0;
            foreach(TargetPlatform platform in new[] {TargetPlatform.Win32,TargetPlatform.Xbox360,TargetPlatform.PlayStation3,(TargetPlatform)99}) {
                plugin.Initialize(null,platform);bool big=platform!=TargetPlatform.Win32;
                var meta=plugin.GetExtendedTypeInformation(2486173485u);
                if(meta.TypeHash!=3415017856u || meta.TypeName!="GameObject" || !meta.Tokenized)throw new Exception("Original GameObject metadata changed");
                for(int field=-1;field<names.Length;++field) foreach(int value in field<0?new[] {0}:new[] {-17,0,23}) {
                    string attr=field<0?"":names[field]+"='"+value+"'";
                    var input=Declaration(attr);var buffer=plugin.ProcessInstance(input);var repeat=plugin.ProcessInstance(input);
                    Equal(buffer.InstanceData,repeat.InstanceData);Equal(buffer.RelocationData,repeat.RelocationData);Equal(buffer.ImportsData,repeat.ImportsData);
                    // Token[0] is KindOf, bookmarked at native root+4. Native root is 592 bytes.
                    int root=checked((int)Word(buffer.InstanceData,8,big))-4;
                    if(root<127*12 || root+592>buffer.InstanceData.Length)throw new Exception("Original GameObject root bounds");
                    if(field<0) for(int i=0;i<127;++i) {
                        uint name=Word(buffer.InstanceData,12*i,big),type=Word(buffer.InstanceData,12*i+4,big),target=Word(buffer.InstanceData,12*i+8,big);
                        string resolved;symbols.TryGetValue(name,out resolved);
                        File.AppendAllText(Path.Combine(args[1],"root-tokens.tsv"),((int)platform)+"\t"+i+"\t"+name+"\t"+type+"\t"+target+"\t"+(resolved??"<unresolved>")+Environment.NewLine);
                    }
                    if(field>=0) {
                        uint expected=signed[field]?unchecked((uint)value):BitConverter.ToUInt32(BitConverter.GetBytes((float)value),0);
                        if(Word(buffer.InstanceData,root+offsets[field],big)!=expected)throw new Exception("Original scalar binding: "+attr);
                    }
                    string stem=Path.Combine(args[1],((int)platform)+"-"+(field<0?"defaults":names[field])+"-"+value);
                    File.WriteAllBytes(stem+".bin",buffer.InstanceData);File.WriteAllBytes(stem+".relo",buffer.RelocationData);File.WriteAllBytes(stem+".imp",buffer.ImportsData);
                    File.AppendAllText(Path.Combine(args[1],"layout.tsv"),((int)platform)+"\t"+attr+"\t"+root+"\t"+buffer.InstanceData.Length+"\t"+buffer.RelocationData.Length+"\t"+buffer.ImportsData.Length+Environment.NewLine);
                    ++fixtures;
                }
            }
            Console.WriteLine("PASS original GameObject characterization: fixtures="+fixtures+" repeatedThreeBufferComparisons="+(fixtures*3)+"; 592-byte root, 127 root tokens, 16 scalar bindings, four targets. No recovered GameObject implementation.");return 0;
        } catch(Exception error) {Console.Error.WriteLine(error);return 1;}
    }
}
