using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

class XmlattributesOracle
{
    const string Ns = "uri:ea.com:eala:asset";
    static int fixtures, comparisons, rejections, layouts;
    static InstanceDeclaration Declaration(XmlDocument xml, XmlNamespaceManager ns) {
        var doc = new AssetDeclarationDocument();
        Type t = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(t);
        t.GetField("NamespaceManager").SetValue(state, ns);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc, state);
        var d = new InstanceDeclaration(doc); d.XmlNode = xml.DocumentElement; return d;
    }
    static void Equal(string label, byte[] a, byte[] b) {
        if (a == null || b == null || a.Length != b.Length) throw new Exception(label + " length");
        for (int i=0;i<a.Length;i++) if(a[i]!=b[i]) throw new Exception(label+" byte="+i+" expected="+a[i]+" actual="+b[i]);
        comparisons++;
    }
    static uint Word(byte[] b, int n, bool big) {
        return big ? ((uint)b[n]<<24)|((uint)b[n+1]<<16)|((uint)b[n+2]<<8)|b[n+3] : BitConverter.ToUInt32(b,n);
    }
    static void Require(bool ok, string label) { if(!ok) throw new Exception(label); }
    static string Xml(string attrs, string children) { return "<LogicCommandSet xmlns='"+Ns+"' id='Oracle' "+attrs+">"+children+"</LogicCommandSet>"; }
    static void Check(IAssetBuilderPlugin plugin, TargetPlatform platform, string label, string[] commands, uint?[] expected) {
        string children = "";
        foreach(string s in commands) children += "<Cmd>"+System.Security.SecurityElement.Escape(s)+"</Cmd>";
        var xml = new XmlDocument(); xml.LoadXml(Xml("",children));
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea",Ns);
        var decl = Declaration(xml,ns);
        var original = plugin.ProcessInstance(decl);
        var actual = LogicCommandSetCompiler.ProcessInstance(decl, platform);
        Equal(label+"/instance",original.InstanceData,actual.InstanceData);
        Equal(label+"/relocations",original.RelocationData,actual.RelocationData);
        Equal(label+"/imports",original.ImportsData,actual.ImportsData);
        bool big = platform != TargetPlatform.Win32;
        int n=commands.Length, imports=0;
        Require(actual.InstanceData.Length==12+4*n,label+" root/list size (no Tokenizer header)");
        Require(Word(actual.InstanceData,0,big)==0 && Word(actual.InstanceData,4,big)==n,label+" root/count");
        Require(Word(actual.InstanceData,8,big)==(n==0?0:12),label+" list pointer");
        Require(actual.RelocationData.Length==(n==0?0:8),label+" relocation size");
        if(n!=0) Require(Word(actual.RelocationData,0,big)==8 && Word(actual.RelocationData,4,big)==uint.MaxValue,label+" relocation words");
        var offsets = new HashSet<uint>();
        for(int i=0;i<n;i++) {
            uint value=expected[i].HasValue ? unchecked(expected[i].Value+1) : 0;
            Require(Word(actual.InstanceData,12+4*i,big)==value,label+" reference "+i);
            if(expected[i].HasValue) { imports++; offsets.Add((uint)(12+4*i)); }
        }
        Require(actual.ImportsData.Length==(imports==0?0:(imports+1)*4),label+" import size");
        for(int i=0;i<imports;i++) Require(offsets.Remove(Word(actual.ImportsData,4*i,big)),label+" import offset");
        if(imports!=0) Require(Word(actual.ImportsData,4*imports,big)==uint.MaxValue,label+" import sentinel");
        layouts++; fixtures++;
        Console.WriteLine("PASS platform={0} fixture={1} commands={2} threeArrayComparisons=3 independentLayout=1",(int)platform,label,n);
    }
    static void Reject(string text, TargetPlatform p) {
        var xml = new XmlDocument(); xml.LoadXml(text);
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea",Ns);
        try { LogicCommandSetCompiler.CompileXml(xml.DocumentElement,ns,p!=TargetPlatform.Win32); }
        catch(NotSupportedException) { rejections++; return; }
        throw new Exception("Accepted unsupported input: "+text);
    }
    static int Main(string[] args) {
        try {
            CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("de-DE");
            var asm=Assembly.LoadFrom(args[0]);
            var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(asm.GetType("BinaryAssetBuilder.XmlCompiler.Plugin"));
            foreach(TargetPlatform p in new[]{(TargetPlatform)0,(TargetPlatform)1,(TargetPlatform)2,(TargetPlatform)99}) {
                plugin.Initialize(null,p);
                int f=fixtures,c=comparisons,r=rejections,l=layouts;
                Check(plugin,p,"empty",new string[0],new uint?[0]);
                Check(plugin,p,"plain",new[]{"Command_A","", "  ","A&amp;B"},new uint?[]{null,null,null,null});
                Check(plugin,p,"resources",new[]{"LogicCommand:A\\0","A\\1","B\\17","C\\-1","D\\+2","E\\ 9tail","F\\","G\\junk","H\\2147483648","I\\-2147483649"},new uint?[]{0,1,17,uint.MaxValue,2,9,0,0,2147483647,2147483648});
                Check(plugin,p,"duplicates",new[]{"A\\5","B\\5","C\\0","D\\5","NoSlash","Z\\-2"},new uint?[]{5,5,0,5,null,unchecked((uint)-2)});
                foreach(int count in new[]{1,2,3,27,28,29,31,64,129}) {
                    var commands=new string[count];var expected=new uint?[count];
                    for(int i=0;i<count;i++) { int value=(i*37)%131; commands[i]="Command_"+i+"\\"+value.ToString(CultureInfo.InvariantCulture); expected[i]=(uint)value; }
                    Check(plugin,p,"list-"+count,commands,expected);
                }
                Reject(Xml("Bogus='1'",""),p);
                Reject(Xml("Default='1'",""),p);
                Reject(Xml("","<Unknown/>"),p);
                Reject(Xml("","<Cmd Value='1'/>"),p);
                Reject(Xml("","<Cmd><Nested/></Cmd>"),p);
                Reject(Xml("","<Cmd xmlns='urn:other'>A</Cmd>"),p);
                Reject(Xml("","unexpected text"),p);
                Reject("<LogicCommandSet id='x'/>",p);
                Reject("<UpgradeTemplate xmlns='"+Ns+"' id='x'/>",p);
                var wrong=new XmlDocument();wrong.LoadXml("<ArmorTemplate xmlns='"+Ns+"' id='x'/>");
                var wns=new XmlNamespaceManager(wrong.NameTable);wns.AddNamespace("ea",Ns);
                try { LogicCommandSetCompiler.ProcessInstance(Declaration(wrong,wns),p); throw new Exception("wrong TypeId accepted"); }
                catch(NotSupportedException) { rejections++; }
                Console.WriteLine("SUMMARY platform={0} fixtures={1} threeArrayComparisons={2} rejected={3} independentLayouts={4}",(int)p,fixtures-f,comparisons-c,rejections-r,layouts-l);
            }
            Console.WriteLine("TOTAL fixtures={0} threeArrayComparisons={1} rejected={2} independentLayouts={3}",fixtures,comparisons,rejections,layouts);
            return 0;
        } catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
