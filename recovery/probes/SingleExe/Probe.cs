using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder.Core;
using EALA.Hash;
#if !ORACLE
using Recovered;
#endif

// Same fixtures run in separate original and unified-link processes. The original
// business assemblies never enter the latter process or its working directory.
public static class SingleExeProbe
{
    const string Ns = "uri:ea.com:eala:asset";
    static readonly List<string> rows = new List<string>();
#if ORACLE
    static IAssetBuilderPlugin plugin;
    static AssetBuffer Compile(XmlDocument xml, XmlNamespaceManager namespaces, int platform)
    {
        plugin.ReInitialize(null, (TargetPlatform)platform);
        var document = new AssetDeclarationDocument();
        Type stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state, namespaces);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        var instance = new InstanceDeclaration(document); instance.XmlNode = xml.DocumentElement;
        return plugin.ProcessInstance(instance);
    }
#else
    static AssetBuffer Compile(XmlDocument xml, XmlNamespaceManager namespaces, int platform)
    { return ArmorCompiler.CompileXml(xml.DocumentElement, namespaces, platform != 0); }
#endif
    static string Bytes(byte[] value) { if (value == null) throw new Exception("null buffer"); return Convert.ToBase64String(value); }
    static uint Word(byte[] value, int offset, bool big)
    { return big ? ((uint)value[offset] << 24) | ((uint)value[offset+1] << 16) | ((uint)value[offset+2] << 8) | value[offset+3] : BitConverter.ToUInt32(value, offset); }
    static void Case(string label, string attrs, string children, int count, int platform)
    {
        var xml = new XmlDocument();
        xml.LoadXml("<ArmorTemplate xmlns='" + Ns + "' id='Probe' " + attrs + ">" + children + "</ArmorTemplate>");
        var namespaces = new XmlNamespaceManager(xml.NameTable); namespaces.AddNamespace("ea", Ns);
        AssetBuffer b = Compile(xml, namespaces, platform);
        int tokenSize = 96 + count * 48; bool big = platform != 0;
        if (b.InstanceData.Length != 128 + 56 * count || Word(b.InstanceData, tokenSize+24, big) != count ||
            Word(b.InstanceData, 8, big) != tokenSize+4 || Word(b.InstanceData, 68, big) != 84 ||
            Word(b.InstanceData, 92, big) != tokenSize+24 || b.RelocationData.Length != (count == 0 ? 0 : 8) || b.ImportsData.Length != 0)
            throw new Exception("independent Armor layout: " + label);
        if (count != 0 && (Word(b.InstanceData, tokenSize+28, big) != tokenSize+32 ||
            Word(b.RelocationData, 0, big) != tokenSize+28 || Word(b.RelocationData, 4, big) != uint.MaxValue))
            throw new Exception("independent Armor relocation: " + label);
        rows.Add("armor\t" + label + "\t" + platform + "\t" + Bytes(b.InstanceData) + "\t" + Bytes(b.RelocationData) + "\t" + Bytes(b.ImportsData));
    }
    static void Fixtures()
    {
        string[] damages = { "FORCE", "CRUSH", "SLASH", "PIERCE", "FLAME", "HEALING", "UNRESISTABLE", "WATER", "PENALTY", "FALLING", "BLAST", "REFLECTED", "PASSENGER", "MAGIC", "UNDEFINED", "LOGICAL_FIRE", "POISON", "LASER", "PLASMA", "FIRE", "ROCKET", "GUN", "CANNON", "GRENADE", "SNIPER", "MINE", "TIBERIUM", "STORM_SHIELD", "MELEE", "AUTO_CANNON", "IMPACT", "EXPLOSIVE", "CONCUSSIVE", "FLAK", "PRISM", "TESLA", "KI", "RADIATION", "ALL" };
        string[] numbers = { "0", "-0", "-25.5%", "33.3333333333", "16777217", "2d1", "2D-1", "1e-7", "", "garbage", " 25.75junk", "25,5", "100%", "1e30", "1e-30" };
        foreach (int platform in new int[] { 0, 1, 2, 99 }) {
            Case("defaults", "", "", 0, platform);
            Case("distinct-fields", "Default='12.25%' DamageScalar='23.5' SideDamageScalar='34.75' RearDamageScalar='45' FlankedPenalty='56.125'", "<Armor Damage='TESLA' Percent='-10'/><Armor Damage='GUN' Percent='225.5'/>", 2, platform);
            Case("partial-defaults", "SideDamageScalar='0' FlankedPenalty='-12.5'", "", 0, platform);
            for (int i=0; i<numbers.Length; ++i) {
                string n=numbers[i]; Case("percent-"+i, "Default='"+n+"' DamageScalar='"+n+"' SideDamageScalar='"+n+"' RearDamageScalar='"+n+"' FlankedPenalty='"+n+"'", "<Armor Damage='CANNON' Percent='"+n+"'/>", 1, platform);
            }
            foreach (string damage in damages) Case("enum-"+damage, "", "<Armor Damage='"+damage+"' Percent='67.125%'/>", 1, platform);
            foreach (string damage in new string[] { "cannon", " CANNON", "CANNON ", "", "UNKNOWN", "炮弹" })
                Case("unknown-"+damage, "", "<Armor Damage='"+damage+"' Percent='15'/>", 1, platform);
            Case("missing-and-duplicate", "", "<Armor/><Armor Damage='FORCE'/><Armor Percent='10'/><Armor Damage='FORCE' Percent='200'/>", 4, platform);
            Case("namespace-filter", "default='1'", "<Armor xmlns='' Percent='1'/><Armor xmlns='other' Percent='2'/><armor Percent='3'/><Other/><e:Armor xmlns:e='"+Ns+"' Damage='GUN' Percent='42'/><Armor><Percent>5</Percent></Armor>", 2, platform);
            foreach (int count in new int[] { 2, 27, 28, 29, 64, 257 }) {
                string children=""; for(int i=0;i<count;++i) children += "<Armor Damage='"+damages[i%damages.Length]+"' Percent='"+(i*7-40).ToString(CultureInfo.InvariantCulture)+"'/>";
                Case("list-"+count, "", children, count, platform);
            }
        }
        foreach (string value in new string[] { "ArmorTemplate", "abc", "中文", "é", "\u00ff", "" }) {
            rows.Add("hash-string\t"+Bytes(System.Text.Encoding.Unicode.GetBytes(value))+"\t"+FastHash.GetHashCode(value)+"\t"+FastHash.GetHashCode(123u,value));
        }
        foreach (int count in new int[] { 1, 2, 3, 4, 7, 32, 257 }) {
            byte[] data=new byte[count]; for(int i=0;i<count;++i)data[i]=(byte)(i*131+255);
            for(int len=0;len<=count;++len) rows.Add("hash-bytes\t"+count+"\t"+len+"\t"+FastHash.GetHashCode(data,len)+"\t"+FastHash.GetHashCode(123u,data,len));
            rows.Add("hash-full\t"+count+"\t"+FastHash.GetHashCode(data)+"\t"+FastHash.GetHashCode(123u,data));
        }
    }
    public static int Main(string[] args)
    {
        try {
#if FULL_FOUNDATION
            if(args.Length>0 && (args[0]=="build" || args[0]=="plan" || args[0]=="doctor" || args[0]=="--help" || args[0]=="-h"))
                return NewModCompiler.Cli.Program.Main(args);
            if(args.Length>0 && args[0]=="--texture-probe") {
                string[] textureArgs=new string[args.Length-1];
                Array.Copy(args,1,textureArgs,0,textureArgs.Length);
                return TextureProbe.Run(textureArgs);
            }
#endif
            CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("de-DE");
#if ORACLE
            plugin=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[1]).GetType("BinaryAssetBuilder.Tokenizer.Plugin"));
            plugin.Initialize(null,TargetPlatform.Win32);
#else
            #if EXTERNAL_PRODUCT
            Assembly self=typeof(AssetBuffer).Assembly;
#else
            Assembly self=Assembly.GetExecutingAssembly();
#endif
            foreach(Type type in new Type[] {typeof(AssetBuffer),typeof(FastHash),typeof(ArmorCompiler)})
                if(type.Assembly!=self)throw new Exception("business type outside EXE: "+type.FullName);
            if(self.GetModules().Length!=1)throw new Exception("multifile assembly remains");
            foreach(AssemblyName reference in self.GetReferencedAssemblies()) {
                if(reference.Name.StartsWith("BinaryAssetBuilder.") || reference.Name.StartsWith("Recovered.") || reference.Name.StartsWith("EALA.") || reference.Name=="Mvp.Xml")
                    throw new Exception("old business reference: "+reference.FullName);
            }
            using(Stream schema=self.GetManifestResourceStream("BinaryAssetBuilderProject.xsd")) {
                if(schema==null || schema.Length==0)throw new Exception("Core schema resource missing");
                using(XmlReader reader=XmlReader.Create(schema))while(reader.Read()){}
            }
            bool rejected=false;try{ArmorCompiler.CompileXml(null,null,false);}catch(ArgumentNullException){rejected=true;}
            if(!rejected)throw new Exception("null-node failure path not preserved");
#endif
#if FULL_FOUNDATION
            var actualTypes = new List<string>();
            foreach(Type actualType in self.GetTypes()) actualTypes.Add(actualType.FullName);
            actualTypes.Sort(StringComparer.Ordinal);
            File.WriteAllLines(args[0]+".types",actualTypes.ToArray());
            NewSdk.BuildValidation.ManagedFoundationValidation.Run(true);
#endif
            Fixtures();
            File.WriteAllLines(args[0],rows.ToArray());
#if !ORACLE
            using(StreamWriter resources=new StreamWriter(args[0]+".resources")) {
                foreach(string name in self.GetManifestResourceNames()) {
                    using(Stream resource=self.GetManifestResourceStream(name))
                    using(SHA256 hash=SHA256.Create())
                        resources.WriteLine(name+"\t"+BitConverter.ToString(hash.ComputeHash(resource)).Replace("-", "").ToLowerInvariant());
                }
            }
            using(StreamWriter audit=new StreamWriter(args[0]+".bindings")) {
                foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    audit.WriteLine(assembly.FullName+"\t"+(assembly.IsDynamic ? "<dynamic; no physical file>" : assembly.Location));
                    if(assembly!=self && (assembly.GetName().Name.StartsWith("BinaryAssetBuilder.") || assembly.GetName().Name.StartsWith("Recovered.") || assembly.GetName().Name.StartsWith("EALA.")))throw new Exception("hidden business assembly loaded");
                }
            }
#endif
            Console.WriteLine("PASS unified-link fixture rows="+rows.Count);return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
