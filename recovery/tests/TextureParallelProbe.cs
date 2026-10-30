using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

// Deliberately isolated from production: the original mixed DLL owns process globals.
// A crash or byte mismatch here must never turn into worker certification.
public static class TextureParallelProbe
{
    static readonly object gate = new object();
    static int active, maximum;
    static readonly List<object> calls = new List<object>();
    static string Hash(byte[] bytes)
    { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(); }
    static void Require(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); }
    static void Fixture(string path, int seed)
    {
        using (var writer = new BinaryWriter(File.Create(path)))
        {
            writer.Write(new byte[] {0,0,2,0,0,0,0,0,0,0,0,0,0,1,0,1,32,40});
            for (int y=0;y<256;y++) for (int x=0;x<256;x++)
                writer.Write(new byte[] {(byte)(x+seed*37),(byte)(y+seed*53),(byte)(x^y^seed*71),255});
        }
    }
    static InstanceDeclaration Declare(string type, string file, string custom, int index)
    {
        string source = System.Security.SecurityElement.Escape(file);
        string text = type == "TerrainTextureAtlas"
            ? "<TerrainTextureAtlas xmlns='uri:ea.com:eala:asset' id='Probe"+index+"' AtlasSize='2048' AllowLossyCompression='false'><Tile TextureID='1' BaseTexture='"+source+"' NormalTexture='"+source+"'><Cell IndexX='0' IndexY='0'/></Tile></TerrainTextureAtlas>"
            : "<"+type+" xmlns='uri:ea.com:eala:asset' id='Probe"+index+"' File='"+source+"' Type='StandardTexture' GenerateMipMaps='true' OutputFormat='DXT5' AllowAutomaticResize='false'/>";
        var xml = new XmlDocument(); xml.LoadXml(text);
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea","uri:ea.com:eala:asset");
        var doc = new AssetDeclarationDocument();
        var current = typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        var state = FormatterServices.GetUninitializedObject(current);
        current.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(doc,state);
        return new InstanceDeclaration(doc) {XmlNode=xml.DocumentElement,CustomDataPath=custom};
    }
    static byte[][] Execute(IAssetBuilderPlugin plugin, string type, string file, string directory, int index)
    {
        string custom=Path.Combine(directory,"payload.cdata"); Directory.CreateDirectory(directory);
        var declaration=Declare(type,file,custom,index);
        var info=plugin.GetExtendedTypeInformation(declaration.Handle.TypeId);
        declaration.Handle.TypeHash=info.TypeHash; declaration.HasCustomData=info.HasCustomData;
        long start=Stopwatch.GetTimestamp();
        lock(gate) {active++; maximum=Math.Max(maximum,active);}
        try {
            AssetBuffer buffer=plugin.ProcessInstance(declaration);
            Require(buffer!=null,"Missing buffer: "+type);
            return new[] {buffer.InstanceData,buffer.RelocationData,buffer.ImportsData,File.Exists(custom)?File.ReadAllBytes(custom):null};
        }
        finally {
            lock(gate) {
                active--;
                calls.Add(new {typeName=type,index,startTicks=start,endTicks=Stopwatch.GetTimestamp(),threadId=Thread.CurrentThread.ManagedThreadId});
            }
        }
    }
    static string[] Save(string directory, byte[][] buffers)
    {
        Directory.CreateDirectory(directory);
        return buffers.Select((bytes,index)=> {
            if(bytes==null) return null;
            File.WriteAllBytes(Path.Combine(directory,index+".bin"),bytes); return Hash(bytes);
        }).ToArray();
    }
    public static int Main(string[] args)
    {
        // DLL, output root, route|mixed, unsafe|locked|isolated, rounds.
        string output=Path.GetFullPath(args[1]); Directory.CreateDirectory(output);
        var result=new Dictionary<string,object> {{"status","started"},{"route",args[2]},{"mode",args[3]},
            {"sourceCommit",Environment.GetEnvironmentVariable("GITHUB_SHA")},{"runId",Environment.GetEnvironmentVariable("GITHUB_RUN_ID")}};
        IAssetBuilderPlugin plugin=null;
        var isolatedPlugins=new List<IAssetBuilderPlugin>();
        var serializer=new JavaScriptSerializer {MaxJsonLength=int.MaxValue};
        try {
            plugin=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(Path.GetFullPath(args[0])).GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));
            plugin.Initialize(null,TargetPlatform.Win32);
            int workers=4,rounds=int.Parse(args[4]);
            string[] routes=args[2]=="mixed"?new[] {"Texture","OnDemandTexture","TerrainTextureAtlas","Texture"}:Enumerable.Repeat(args[2],workers).ToArray();
            var files=new string[workers]; var expected=new byte[workers][][]; var baseline=new List<object>();
            for(int i=0;i<workers;i++) {
                files[i]=Path.Combine(output,"input"+i+".tga"); Fixture(files[i],i);
                expected[i]=Execute(plugin,routes[i],files[i],Path.Combine(output,"serial"+i),i);
                baseline.Add(new {typeName=routes[i],index=i,inputSha256=Hash(File.ReadAllBytes(files[i])),buffers=Save(Path.Combine(output,"reference"+i),expected[i])});
            }
            result["baseline"]=baseline;
            if(args[3]=="isolated") {
                var modules=new List<object>();
                var nativeHandles=new HashSet<IntPtr>();
                nativeHandles.Add(Marshal.GetHINSTANCE(plugin.GetType().Module));
                for(int i=0;i<workers;i++) {
                    string copyDirectory=Path.Combine(output,"module"+i);Directory.CreateDirectory(copyDirectory);
                    string copy=Path.Combine(copyDirectory,Path.GetFileName(args[0]));
                    File.Copy(args[0],copy);
                    var assembly=Assembly.LoadFile(copy);
                    IntPtr handle=Marshal.GetHINSTANCE(assembly.ManifestModule);
                    modules.Add(new {index=i,assembly.Location,nativeHandle=handle.ToInt64(),assembly.FullName});
                    result["isolatedModules"]=modules;
                    Require(handle!=IntPtr.Zero && handle!=new IntPtr(-1) && nativeHandles.Add(handle),"LoadFile did not create independent native storage");
                    var instance=(IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));
                    instance.Initialize(null,TargetPlatform.Win32);isolatedPlugins.Add(instance);
                }
            }
            File.WriteAllText(Path.Combine(output,"result.json"),serializer.Serialize(result));
            calls.Clear(); maximum=0;
            var comparisons=new List<object>();
            for(int round=0;round<rounds;round++) {
                var actual=new byte[workers][][];
                using(var barrier=new Barrier(workers)) {
                    var tasks=new Task[workers];
                    for(int i=0;i<workers;i++) {
                        int index=i,iteration=round;
                        tasks[i]=Task.Factory.StartNew(delegate {
                            Require(barrier.SignalAndWait(30000),"Worker start barrier timed out");
                            string directory=Path.Combine(output,"round"+iteration+"-worker"+index);
                            if(args[3]=="locked") lock(gate) actual[index]=Execute(plugin,routes[index],files[index],directory,index);
                            else actual[index]=Execute(args[3]=="isolated"?isolatedPlugins[index]:plugin,routes[index],files[index],directory,index);
                        },CancellationToken.None,TaskCreationOptions.LongRunning,TaskScheduler.Default);
                    }
                    Task.WaitAll(tasks);
                }
                for(int i=0;i<workers;i++) {
                    string[] hashes=Save(Path.Combine(output,"round"+round+"-worker"+i),actual[i]);
                    bool equal=Enumerable.Range(0,4).All(b=>expected[i][b]==null?actual[i][b]==null:actual[i][b]!=null && expected[i][b].SequenceEqual(actual[i][b]));
                    comparisons.Add(new {round,index=i,typeName=routes[i],equal,buffers=hashes});
                    result["comparisons"]=comparisons;
                    Require(equal,"Concurrent output differs from serial: "+routes[i]+" round="+round+" worker="+i);
                }
            }
            Require(maximum==(args[3]=="locked"?1:workers),"Probe did not exercise requested overlap");
            result["status"]="passed"; return 0;
        }
        catch(Exception error) {result["status"]="failed";result["error"]=error.ToString();Console.Error.WriteLine(error);return 1;}
        finally {
            result["maximumConcurrency"]=maximum;result["calls"]=calls;result["clockFrequency"]=Stopwatch.Frequency;
            // Save evidence before Dispose: native corruption may crash finalization.
            File.WriteAllText(Path.Combine(output,"result.json"),serializer.Serialize(result));
            foreach(var instance in isolatedPlugins) ((IDisposable)instance).Dispose();
            if(plugin!=null) ((IDisposable)plugin).Dispose();
        }
    }
}
