using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Script.Serialization;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Real sequential IStreamCompiler fixture. Cold comparisons use new backend
// instances in this same process; this is not a cold-process or SDK/G3 receipt.
public static class CompilerBackendLifecycle
{
    static readonly List<string> Tests = new List<string>();
    static readonly BindingFlags PrivateStatic = BindingFlags.Static | BindingFlags.NonPublic;
    static string work, sdk, probe;
    static int checks, traceCalls;
    static void Require(bool value, string message) { ++checks; if (!value) throw new Exception(message); }
    static object Field(Type type, string name) { return type.GetField(name, PrivateStatic).GetValue(null); }
    sealed class Ambient
    {
        readonly Settings settings = Settings.Current;
        readonly object names = Field(typeof(HashProvider), "_SchemaNameToBinName");
        readonly object bins = Field(typeof(HashProvider), "_StringHashBins");
        readonly string directory = HashProvider.GetOutputDirectory(), file = HashProvider.StringHashesFile;
        readonly TraceWriteHandler writer = Tracer.TraceWrite;
        readonly object mask = Field(typeof(Tracer), "_TraceMask");
        public void Unchanged(string label)
        {
            Require(Object.ReferenceEquals(Settings.Current, settings), label + ": Settings.Current leaked");
            Require(Object.ReferenceEquals(Field(typeof(HashProvider), "_SchemaNameToBinName"), names), label + ": schema hash map leaked");
            Require(Object.ReferenceEquals(Field(typeof(HashProvider), "_StringHashBins"), bins), label + ": string hash bins leaked");
            Require(HashProvider.GetOutputDirectory() == directory && HashProvider.StringHashesFile == file, label + ": hash path leaked");
            Require(Object.ReferenceEquals(Tracer.TraceWrite, writer) && Object.Equals(Field(typeof(Tracer), "_TraceMask"), mask), label + ": trace writer/mask leaked");
        }
    }
    static string Write(string path, string text)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, text); return path;
    }
    static string Asset(string body) { return "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>" + body + "</AssetDeclaration>"; }
    static BuildRequest Request(string mod, string output)
    {
        Directory.CreateDirectory(output);
        return new BuildRequest { Command="build", ModRoot=mod, SdkRoot=sdk, OutputRoot=output,
            Content=new[] {"base"}, PackageFormat="loose", Arguments=new string[0] };
    }
    static BuildTask Task(string mod, string name)
    {
        return new BuildTask { Id="base/"+name, Content="base", InputPath=Path.Combine(mod,"Data",name+".xml"),
            OutputName="base/"+name, Reason="LifecycleFixture", Dependencies=new string[0] };
    }
    static StreamContext Context(BuildRequest request, BuildTask task, string staging)
    {
        Directory.CreateDirectory(staging);
        // Application owns these internal setters in production. Reflection only
        // supplies the real context from this separate validation fixture assembly.
        var context = new StreamContext();
        foreach (var pair in new Dictionary<string,object> { {"BuildId","backend-lifecycle"}, {"Request",request}, {"Task",task}, {"StagingDirectory",staging} })
            typeof(StreamContext).GetProperty(pair.Key).GetSetMethod(true).Invoke(context,new[] {pair.Value});
        return context;
    }
    static CoreStreamCompiler Backend() { return new CoreStreamCompiler(delegate(string source, TraceEventType kind, string message) { ++traceCalls; }); }
    static string Hash(string path)
    {
        using (var sha=SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();
    }
    static Dictionary<string,string> Four(IList<Artifact> artifacts, string staging)
    {
        Require(artifacts != null && artifacts.Count != 0, "Backend returned no artifacts");
        var result = new Dictionary<string,string>(StringComparer.Ordinal);
        foreach (Artifact artifact in artifacts)
        {
            Require(artifact != null && Path.IsPathRooted(artifact.Path) && Paths.Contains(staging,artifact.Path), "Artifact escaped staging");
            Require(File.Exists(artifact.Path) && new FileInfo(artifact.Path).Length == artifact.Size, "Artifact size/path mismatch");
            string hash=Hash(artifact.Path); Require(hash==artifact.Sha256.ToLowerInvariant(), "Artifact hash mismatch");
            string extension=Path.GetExtension(artifact.Path);
            if (extension==".manifest" || extension==".bin" || extension==".imp" || extension==".relo")
                result.Add(artifact.Path.Substring(staging.Length),hash);
        }
        Require(result.Count==4, "Expected exactly four linked stream artifacts");
        foreach (string extension in new[] {".manifest",".bin",".imp",".relo"})
            Require(result.Keys.Count(path=>Path.GetExtension(path)==extension)==1,"Missing linked artifact "+extension);
        return result;
    }
    static Dictionary<string,string> Compile(CoreStreamCompiler backend, BuildRequest request, string name, string slot, Ambient ambient)
    {
        string staging=Path.Combine(request.OutputRoot,slot);
        var result=Four(backend.Compile(Context(request,Task(request.ModRoot,name),staging),CancellationToken.None),staging);
        ambient.Unchanged(slot); return result;
    }
    static void Equal(Dictionary<string,string> left, Dictionary<string,string> right, string label)
    {
        Require(left.Count==right.Count,label+": artifact count changed");
        foreach (var pair in left) Require(right.ContainsKey(pair.Key) && right[pair.Key]==pair.Value,label+": bytes changed "+pair.Key);
    }
    static Dictionary<string,string> Files(string root)
    {
        return Directory.GetFiles(root,"*",SearchOption.AllDirectories).ToDictionary(path=>path.Substring(root.Length),Hash,StringComparer.Ordinal);
    }
    static void SameFiles(Dictionary<string,string> expected,string root)
    {
        var actual=Files(root); Require(expected.Count==actual.Count,"Backend wrote into source MOD");
        foreach(var pair in expected) Require(actual.ContainsKey(pair.Key) && actual[pair.Key]==pair.Value,"Backend mutated source MOD");
    }
    static string MakeMod(string name,string variant)
    {
        string root=Path.Combine(work,name);
        Write(Path.Combine(root,"Data/Armor.xml"),variant);
        Write(Path.Combine(root,"Data/Mod.xml"),Asset("<Includes><Include type='all' source='Armor.xml'/></Includes>"));
        Write(Path.Combine(root,"Data/Extra.xml"),variant);
        Write(Path.Combine(root,"Data/Bad.xml"),Asset("<ArmorTemplate id='Broken' inheritFrom='MissingParent'/>"));
        return root;
    }
    static Dictionary<string,string> Cold(BuildRequest request,Ambient ambient)
    {
        using(var backend=Backend()) { backend.Initialize(request); ambient.Unchanged("cold Initialize"); return Compile(backend,request,"Mod","cold",ambient); }
    }
    static void Run(Ambient ambient)
    {
        string a=MakeMod("mod-a",probe), b=MakeMod("mod-b",probe.Replace("Default=\"72.5%\"","Default=\"12.5%\""));
        Require(File.ReadAllText(Path.Combine(a,"Data/Armor.xml"))!=File.ReadAllText(Path.Combine(b,"Data/Armor.xml")),"Variant fixture did not change payload");
        var originalA=Files(a); var originalB=Files(b);
        var requestA=Request(a,Path.Combine(work,"output-a")); var requestB=Request(b,Path.Combine(work,"output-b"));
        Dictionary<string,string> first,second;
        var backend=Backend();
        try
        {
            backend.Initialize(requestA); ambient.Unchanged("Initialize");
            Require(!String.IsNullOrEmpty(backend.ProviderIdentity),"Backend provider identity is missing");
            using(var competitor=Backend())
            {
                bool rejected=false;
                try { competitor.Initialize(requestB); } catch(InvalidOperationException) { rejected=true; } catch(BuildFailure) { rejected=true; }
                Require(rejected,"Concurrent backend lease was accepted"); ambient.Unchanged("Rejected Initialize");
            }
            first=Compile(backend,requestA,"Mod","first",ambient);
            Compile(backend,requestA,"Extra","second-stream",ambient);
            Tests.Add("single-initialize-multiple-real-core-streams-staging-and-ambient-restoration");
            string failed=Path.Combine(requestA.OutputRoot,"bad-stream"); bool failedAsExpected=false;
            try { backend.Compile(Context(requestA,Task(a,"Bad"),failed),CancellationToken.None); }
            catch(BinaryAssetBuilderException e) { failedAsExpected=e.ErrorCode==ErrorCode.InheritFromError; }
            catch(BuildFailure e) { failedAsExpected=e.ExitCode==ExitCode.Compilation && e.Code=="CORE_COMPILE_FAILED" && e.Message.Contains("MissingParent"); }
            Require(failedAsExpected,"Bad inheritance was not a compilation failure"); ambient.Unchanged("Failed Compile");
            Require(!Directory.Exists(failed) || Directory.GetFiles(failed,"*",SearchOption.AllDirectories).Length==0,"Failed stream left partial files");
            Equal(first,Compile(backend,requestA,"Mod","after-failure",ambient),"Failure recovery");
            Tests.Add("bad-inheritance-failure-then-same-backend-legal-stream-byte-equality");
            string cancelled=Path.Combine(requestA.OutputRoot,"cancelled"); bool wasCancelled=false;
            using(var source=new CancellationTokenSource())
            {
                source.Cancel();
                try { backend.Compile(Context(requestA,Task(a,"Mod"),cancelled),source.Token); }
                catch(OperationCanceledException) { wasCancelled=true; }
            }
            Require(wasCancelled,"Already cancelled stream reached success"); ambient.Unchanged("Cancelled Compile");
            Require(!Directory.Exists(cancelled) || Directory.GetFiles(cancelled,"*",SearchOption.AllDirectories).Length==0,"Cancelled stream left files");
            Equal(first,Compile(backend,requestA,"Mod","after-cancel",ambient),"Cancellation recovery");
            Tests.Add("cancel-before-core-then-same-backend-legal-stream-byte-equality");
        }
        finally { backend.Dispose(); backend.Dispose(); }
        ambient.Unchanged("Dispose twice");
        using(var other=Backend()) { other.Initialize(requestB); second=Compile(other,requestB,"Mod","first",ambient); }
        ambient.Unchanged("Second MOD Dispose");
        string bin=first.Keys.Single(path=>Path.GetExtension(path)==".bin");
        Require(second.ContainsKey(bin) && second[bin]!=first[bin],"Different MOD same IDs produced the same payload");
        Equal(first,Cold(requestA,ambient),"MOD A cold-context comparison");
        Equal(second,Cold(requestB,ambient),"MOD B cold-context comparison");
        Tests.Add("two-mods-same-type-id-different-payload-independent-same-process-cold-context-four-file-comparison");
        var invalid=Request(a,Path.Combine(work,"invalid-output")); invalid.SdkRoot=Path.Combine(work,"missing-sdk");
        using(var broken=Backend())
        {
            bool rejected=false;
            try { broken.Initialize(invalid); } catch(BuildFailure) { rejected=true; } catch(DirectoryNotFoundException) { rejected=true; } catch(FileNotFoundException) { rejected=true; }
            Require(rejected,"Invalid SDK initialization succeeded");
        }
        using(var fresh=Backend()) { fresh.Initialize(requestA); Compile(fresh,requestA,"Mod","after-init-failure",ambient); }
        ambient.Unchanged("Failed initialization lease release");
        SameFiles(originalA,a); SameFiles(originalB,b);
        Require(traceCalls>0,"Backend trace bridge was not exercised");
        Tests.Add("dispose-idempotent-and-failed-initialize-releases-lease-source-mods-unchanged");
    }
    public static int Main(string[] args)
    {
        int code=0; object result; Settings previous=Settings.Current;
        var original=new Ambient();
        try
        {
            if(args.Length!=5 || IntPtr.Size!=4) throw new Exception("Expected work result sourceCommit runId SDK (x86)");
            work=Path.GetFullPath(args[0]); sdk=Path.GetFullPath(args[4]); Directory.CreateDirectory(work);
            probe=File.ReadAllText(Path.Combine(sdk,"Mods/mymod/Data/RecoveryArmorProbe.xml"));
            Require(typeof(CoreStreamCompiler).Assembly==typeof(DocumentProcessor).Assembly,"Backend/Core are outside the same product module");
            Settings.Current=new Settings { StringHashBinDescriptors=new[] {new StringHashBinDescriptor {SchemaTypeName="Poid",BinName="POID"}}, SessionCacheDirectory=Path.Combine(work,"ambient"), OutputAssetReport=false };
            using(Tracer.BeginScope(delegate {},9))
            using(HashProvider.BeginStringHashScope(Settings.Current.SessionCacheDirectory))
            {
                HashProvider.RecordHash("POID","BackendFixtureBaselineOnly");
                HashProvider.StringHashesFile="ambient-custom.xml";
                var ambient=new Ambient(); Run(ambient); ambient.Unchanged("All scenarios");
                HashProvider.FinalizeStringHashes();
                string baseline=File.ReadAllText(Path.Combine(Settings.Current.SessionCacheDirectory,"StringHashes.xml"));
                Require(baseline.Contains("BackendFixtureBaselineOnly") && !baseline.Contains("RecoveryArmorProbe"),"Backend polluted caller hash table contents");
            }
            Settings.Current=previous; original.Unchanged("Fixture scopes restored");
            result=new {succeeded=true,sourceCommit=args[2],runId=args[3],checks=checks,tests=Tests.ToArray(),scope="Real in-process backend only; cold comparisons are same-process new-backend contexts; production CLI, cold-process/G3/SDK and persistent-cache acceptance pending"};
            Console.WriteLine("PASS BackendLifecycle: "+Tests.Count+" groups; "+checks+" checks");
        }
        catch(Exception e)
        {
            code=1; result=new {succeeded=false,sourceCommit=args.Length>2?args[2]:null,runId=args.Length>3?args[3]:null,checks=checks,tests=Tests.ToArray(),error=e.ToString()}; Console.Error.WriteLine(e);
        }
        finally { Settings.Current=previous; }
        if(args.Length>1) File.WriteAllText(args[1],new JavaScriptSerializer().Serialize(result));
        return code;
    }
}
