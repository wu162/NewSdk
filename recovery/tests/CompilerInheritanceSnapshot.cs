using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Request-equivalent sequential streams share the successful in-memory session
// and SchemaSet, while each stream gets a fresh semantic DocumentProcessor.
internal static class CompilerInheritanceSnapshot
{
    const string Ns = "uri:ea.com:eala:asset";
    const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
    static string sdk, work;
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly Dictionary<string,long> counters = new Dictionary<string,long>();
    static readonly Dictionary<string,double> phaseMilliseconds = new Dictionary<string,double>();
    static readonly FieldInfo DocumentFrames=typeof(DocumentProcessor).GetField("_CurrentDocumentStack",BindingFlags.Static|BindingFlags.NonPublic);
    static readonly string[] ParentPhases={"core.xml.parse","core.xml.expressions","core.xml.overrides","core.xml.schema.validate",
        "core.xml.instances.validate","core.xml.instance.hash","core.xml.instance.references"};
    static double Milliseconds(string key) { double value; return phaseMilliseconds.TryGetValue(key,out value) ? value : 0; }
    static bool InParentDocument()
    {
        // A newly opened root parses before pushing its own frame. Check Count
        // before Peek so the test collector also handles that initial parse.
        var frames=(Stack<string>)DocumentFrames.GetValue(null);
        return frames.Count!=0 && frames.Peek()=="parent.xml:";
    }
    static Dictionary<string,double> ParentSample()
    {
        return ParentPhases.ToDictionary(name=>name,name=>Milliseconds("parent."+name));
    }
    static Dictionary<string,double> ParentDelta(Dictionary<string,double> before)
    {
        return ParentPhases.ToDictionary(name=>name,name=>Milliseconds("parent."+name)-before[name]);
    }
    sealed class Phase : IDisposable
    {
        readonly string name; readonly bool parent; readonly Stopwatch clock=Stopwatch.StartNew();
        internal Phase(string name,bool parent) { this.name=name; this.parent=parent; }
        public void Dispose()
        {
            double milliseconds=clock.Elapsed.TotalMilliseconds;
            phaseMilliseconds[name]=Milliseconds(name)+milliseconds;
            if(parent) phaseMilliseconds["parent."+name]=Milliseconds("parent."+name)+milliseconds;
        }
    }
    static void CopyDirectory(string source,string destination)
    {
        foreach(string file in Directory.GetFiles(source,"*",SearchOption.AllDirectories))
        {
            string target=Path.Combine(destination,file.Substring(source.Length+1));
            Directory.CreateDirectory(Path.GetDirectoryName(target)); File.Copy(file,target);
        }
    }
    static object snapshotEvidence;
    static int retainedNodes, retainedAttributes;
    static void CountRetained(XmlNode node)
    {
        retainedNodes++;
        if(node.Attributes!=null) retainedAttributes+=node.Attributes.Count;
        foreach(XmlNode child in node.ChildNodes) CountRetained(child);
    }
    static void Require(bool value, string message) { ++checks; if (!value) throw new Exception(message); }
    static long Count(string key) { long value; return counters.TryGetValue(key, out value) ? value : 0; }
    static object Snapshot(InstanceDeclaration instance) { return typeof(InstanceDeclaration).GetField("_InheritanceSnapshot", Private).GetValue(instance); }
    static string Asset(string body) { return "<AssetDeclaration xmlns='" + Ns + "'>" + body + "</AssetDeclaration>"; }
    static string Write(string path, string body)
    {
        path=Path.GetFullPath(Path.Combine(work,path)); Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path,Asset(body)); return path;
    }
    static string Include(string type,string source) { return "<Includes><Include type='"+type+"' source='"+source+"'/></Includes>"; }
    static Settings Configure()
    {
        var config=new XmlDocument(); config.Load(Path.Combine(sdk,"tools/BinaryAssetBuilder.exe.config"));
        var settings=(Settings)new SerializedSectionHandler().Create(null,null,config.SelectSingleNode("/configuration/assetbuilder"));
        settings.DataRoot=work; settings.SchemaPath=Path.Combine(sdk,"Schemas/xsd/CnC3Types.xsd");
        settings.InputPath=Path.Combine(work,"streams/first.xml");
        settings.OutputDirectory=Path.Combine(work,"output"); settings.IntermediateOutputDirectory=Path.Combine(work,"intermediate");
        settings.SessionCacheDirectory=Path.Combine(work,"cache");
        settings.DefaultDataPaths=work; settings.DefaultArtPaths=work; settings.DefaultAudioPaths=work;
        settings.TargetPlatform=TargetPlatform.Win32; settings.BuildCache=true; settings.SessionCache=true;
        settings.BuildCacheDirectory=Path.Combine(work,"build-cache");
        settings.LinkedStreams=true; settings.OutputAssetReport=false; settings.OutputStringHashes=false;
        settings.ErrorLevel=1; settings.UsePrecompiled=false; settings.StreamHints=false; settings.StableSort=false;
        settings.MeasurePerformance=delegate(string name) {
            if(!name.StartsWith("core.xml.",StringComparison.Ordinal)) return null;
            bool parent=InParentDocument();
            return parent || name=="core.xml.parse" || name=="core.xml.inheritance.frontend" ? (IDisposable)new Phase(name,parent) : null;
        };
        settings.RecordPerformanceCounter=delegate(string name,long value,bool maximum) {
            long previous=Count(name); counters[name]=maximum ? Math.Max(previous,value) : previous+value;
        };
        Settings.Current=settings; SettingsLoader.PostProcessSettings(work);
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
        Directory.CreateDirectory(settings.SessionCacheDirectory);
        HashProvider.InitializeStringHashes(settings.SessionCacheDirectory);
        return settings;
    }
    static VerifierPluginRegistry Verifiers()
    {
        var verifiers=new VerifierPluginRegistry(new PluginDescriptor[0],TargetPlatform.Win32);
        verifiers.DefaultPlugin=(IAssetBuilderVerifierPlugin)Activator.CreateInstance(
            typeof(DocumentProcessor).Assembly.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin",true),true);
        verifiers.DefaultPlugin.Initialize(null,TargetPlatform.Win32); return verifiers;
    }
    static DocumentProcessor Processor(BuiltinCompilers plugins,SchemaSet schemas,SessionCache cache,VerifierPluginRegistry verifiers=null)
    {
        return new DocumentProcessor(Settings.Current,plugins.Registry,verifiers ?? Verifiers()) {Cache=cache,SchemaSet=schemas};
    }
    static SessionCache Cache(SchemaSet schemas,string name)
    {
        var cache=new SessionCache(); cache.LoadCacheFrom(null,Path.Combine(Settings.Current.SessionCacheDirectory,name+".xml"));
        cache.InitializeCache(new List<string>(),schemas.ContentIdentity); return cache;
    }
    static void Run(DocumentProcessor processor,string path)
    {
        bool success; processor.ProcessDocument(path,true,false,out success); Require(success,"Core did not report success: "+path);
    }
    static AssetDeclarationDocument Document(SessionCache cache,string path)
    {
        AssetDeclarationDocument document; cache.TryGetDocument(path,null,TargetPlatform.Win32,false,out document);
        Require(document!=null,"Expected cached document "+path); return document;
    }
    static Dictionary<string,string> Four(string directory,string stream)
    {
        var result=new Dictionary<string,string>();
        foreach(string path in Directory.GetFiles(directory,stream+".*",SearchOption.AllDirectories))
        {
            string extension=Path.GetExtension(path);
            if(extension!=".manifest" && extension!=".bin" && extension!=".imp" && extension!=".relo") continue;
            using(var sha=SHA256.Create()) result.Add(extension,BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))));
        }
        Require(result.Count==4,"Expected four published stream files for "+stream); return result;
    }
    static void Equal(Dictionary<string,string> expected,Dictionary<string,string> actual)
    {
        Require(expected.Count==actual.Count,"Changed publication inventory");
        foreach(var pair in expected) Require(actual.ContainsKey(pair.Key) && actual[pair.Key]==pair.Value,"Changed publication bytes: "+pair.Key);
    }
    static void Streams(BuiltinCompilers plugins,SchemaSet schemas,Settings settings)
    {
        string parent=Write("streams/parent.xml","<ArmorTemplate id='SnapshotParent' DamageScalar='81.25%'/><ArmorTemplate id='SnapshotUnrelatedSibling' Default='63.25%'/>");
        Write("streams/one.xml",Include("instance","parent.xml")+"<ArmorTemplate id='SnapshotChildOne' inheritFrom='SnapshotParent'/>");
        Write("streams/two.xml",Include("instance","parent.xml")+"<ArmorTemplate id='SnapshotChildTwo' inheritFrom='SnapshotParent'/>");
        string first=Write("streams/first.xml",Include("all","one.xml"));
        string second=Write("streams/second.xml",Include("all","two.xml"));
        var cache=Cache(schemas,"streams"); var one=Processor(plugins,schemas,cache);
        Run(one,first); cache.SaveCache(false);
        string serializedFirst=Path.Combine(settings.SessionCacheDirectory,"first-state.xml");
        File.Copy(cache.CacheFileName,serializedFirst);
        string controlBuildCache=Path.Combine(work,"control-build-cache");
        CopyDirectory(settings.BuildCacheDirectory,controlBuildCache);
        var document=Document(cache,parent); var instance=document.SelfInstances[0];
        XmlNode source=instance.XmlNode;
        Require(Snapshot(instance)!=null && Count("cache.inheritanceSnapshot.publishedInstances")==1,"First inheritance source was not published on demand");
        Require(source.SchemaInfo.SchemaType.Name=="ArmorTemplate","Inheritance snapshot lost its source PSVI");
        XmlAttribute defaultValue=source.Attributes["Default"];
        Require(defaultValue!=null && defaultValue.Value=="100" && defaultValue.SchemaInfo.SchemaType!=null,
            "Inheritance snapshot lost XSD default attribute or attribute PSVI");
        int retainedCharacters=source.OwnerDocument.OuterXml.Length;
        instance.CustomDataPath="previous-stream-only";
        instance.ReferencedInstances.Add(new InstanceHandle("ArmorTemplate","PreviousStreamOnly"));
        long reloads=Count("cache.reload.inheritanceTarget"), avoided=Count("cache.inheritanceSnapshot.reloadAvoided");
        cache.InitializeCache(new List<string>(),schemas.ContentIdentity);
        Require(typeof(InstanceDeclaration).GetField("_Current",Private).GetValue(instance)==null,"Cross-stream initialization retained CurrentState");
        object retainedSnapshot=Snapshot(instance);
        XmlNode retainedNode=(XmlNode)retainedSnapshot.GetType().GetField("Node",Private).GetValue(retainedSnapshot);
        Require(Object.ReferenceEquals(retainedNode,source),"Snapshot retained a copied or different inheritance source");
        CountRetained(retainedNode.OwnerDocument);
        double candidateParseStart=Milliseconds("core.xml.parse"),candidateFrontendStart=Milliseconds("core.xml.inheritance.frontend");
        var candidateParentStart=ParentSample();
        var candidateClock=Stopwatch.StartNew(); var two=Processor(plugins,schemas,cache); Run(two,second); candidateClock.Stop();
        double candidateParseMs=Milliseconds("core.xml.parse")-candidateParseStart;
        double candidateFrontendMs=Milliseconds("core.xml.inheritance.frontend")-candidateFrontendStart;
        var candidateParentMs=ParentDelta(candidateParentStart);
        var restored=Document(cache,parent).SelfInstances[0];
        Require(two.XmlParseCount==2 && two.InstanceCompileCount==1,"Snapshot stream repeated the parent's XML frontend or backend: parse="+two.XmlParseCount+" compile="+two.InstanceCompileCount);
        Require(Count("cache.inheritanceSnapshot.reloadAvoided")==avoided+1 && Count("cache.reload.inheritanceTarget")==reloads,
            "Snapshot reuse did not avoid the inherited parent reload");
        Require(Object.ReferenceEquals(source,restored.XmlNode) && restored.XmlNode.SchemaInfo.SchemaType.Name=="ArmorTemplate" &&
            restored.XmlNode.Attributes["Default"].SchemaInfo.SchemaType!=null,"Restored inheritance source changed node identity/defaults/PSVI");
        Require(restored.CustomDataPath!="previous-stream-only" && !restored.ReferencedInstances.Any(value=>value.InstanceName=="PreviousStreamOnly"),
            "Snapshot reused transient references or CDATA state");
        var expected=Four(settings.OutputDirectory,"second");
        long candidateAvoided=Count("cache.inheritanceSnapshot.reloadAvoided")-avoided;
        long candidatePublished=Count("cache.inheritanceSnapshot.publishedInstances"),candidateDocuments=Count("cache.inheritanceSnapshot.publishedDocuments");
        // Identical input and first-generation backend assets, but loading the
        // serialized metadata deliberately supplies no in-memory source snapshot.
        string output=settings.OutputDirectory,intermediate=settings.IntermediateOutputDirectory,buildCache=settings.BuildCacheDirectory;
        settings.OutputDirectory=Path.Combine(work,"control-output"); settings.IntermediateOutputDirectory=Path.Combine(work,"control-intermediate");
        settings.BuildCacheDirectory=controlBuildCache;
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
        var controlCache=new SessionCache();
        controlCache.LoadCacheFrom(serializedFirst,Path.Combine(settings.SessionCacheDirectory,"control-session.xml"));
        controlCache.InitializeCache(new List<string>(),schemas.ContentIdentity);
        long controlReloadStart=Count("cache.reload.inheritanceTarget");
        double controlParseStart=Milliseconds("core.xml.parse"),controlFrontendStart=Milliseconds("core.xml.inheritance.frontend");
        var controlParentStart=ParentSample();
        var controlClock=Stopwatch.StartNew(); var control=Processor(plugins,schemas,controlCache); Run(control,second); controlClock.Stop();
        double controlParseMs=Milliseconds("core.xml.parse")-controlParseStart;
        double controlFrontendMs=Milliseconds("core.xml.inheritance.frontend")-controlFrontendStart;
        var controlParentMs=ParentDelta(controlParentStart);
        Require(control.XmlParseCount==3 && control.InstanceCompileCount==1 && Count("cache.reload.inheritanceTarget")==controlReloadStart+1,
            "Serialized-session control did not reproduce the one-parent frontend reload: parse="+control.XmlParseCount+" compile="+control.InstanceCompileCount);
        Require(candidateParentMs.Values.All(value=>value==0) && controlParentMs["core.xml.parse"]>0 && controlParentMs["core.xml.schema.validate"]>0,
            "Parent-only XML parse/schema attribution did not match actual reload work");
        Equal(expected,Four(settings.OutputDirectory,"second"));
        snapshotEvidence=new {candidate=new {xmlParse=two.XmlParseCount,instanceCompile=two.InstanceCompileCount,wallMs=candidateClock.Elapsed.TotalMilliseconds,
                xmlParseMs=candidateParseMs,prevalidatedLoadFrontendMs=candidateFrontendMs,parentPhasesMs=candidateParentMs,reloadsAvoided=candidateAvoided},
            serializedSessionControl=new {xmlParse=control.XmlParseCount,instanceCompile=control.InstanceCompileCount,wallMs=controlClock.Elapsed.TotalMilliseconds,
                xmlParseMs=controlParseMs,prevalidatedLoadFrontendMs=controlFrontendMs,parentPhasesMs=controlParentMs,inheritanceTargetReloads=Count("cache.reload.inheritanceTarget")-controlReloadStart},
            publishedInstances=candidatePublished,publishedDocuments=candidateDocuments,retainedDocumentCount=1,retainedNodeCount=retainedNodes,
            retainedAttributeCount=retainedAttributes,retainedDocumentXmlCharacters=retainedCharacters,
            memoryScope="Original source node retains its OwnerDocument; counts describe this tiny fixture, not managed-memory bytes",
            timingScope="One detailed diagnostic pair with identical input and first-generation backend assets; parentPhasesMs uses the parent.xml traversal frame. prevalidatedLoadFrontendMs covers only PrevalidatedLoad, not Shallow Load. Not a stable throughput estimate"};
        settings.OutputDirectory=output; settings.IntermediateOutputDirectory=intermediate; settings.BuildCacheDirectory=buildCache;
        cache.SaveCache(false);
        // A fresh cache has no snapshot and provides the same-input byte oracle.
        settings.OutputDirectory=Path.Combine(work,"cold-output"); settings.IntermediateOutputDirectory=Path.Combine(work,"cold-intermediate");
        Directory.CreateDirectory(settings.OutputDirectory); Directory.CreateDirectory(settings.IntermediateOutputDirectory);
        settings.BuildCache=false;
        Run(Processor(plugins,schemas,Cache(schemas,"cold")),second); Equal(expected,Four(settings.OutputDirectory,"second"));
        settings.BuildCache=true;
        settings.OutputDirectory=output; settings.IntermediateOutputDirectory=intermediate;
        tests.Add("request-sequential-stream-parent-front-end-reload-avoided-original-PSVI-defaults-transient-reset-four-published-byte-oracle");
        // Changed source must rebuild, reject invalid XSD and publish no new snapshot.
        long published=Count("cache.inheritanceSnapshot.publishedInstances");
        Write("streams/parent.xml","<ArmorTemplate id='SnapshotParent' Bogus='1'/>");
        cache.InitializeCache(new List<string>(),schemas.ContentIdentity);
        bool rejected=false;
        try { Run(Processor(plugins,schemas,cache),second); }
        catch(BinaryAssetBuilderException error) { rejected=error.ErrorCode==ErrorCode.SchemaValidation; }
        Require(rejected,"Changed parent source reused an obsolete snapshot instead of rejecting invalid XSD");
        document=Document(cache,parent);
        Require(document.State!=DocumentState.Complete && document.SelfInstances.All(value=>Snapshot(value)==null) &&
            Count("cache.inheritanceSnapshot.publishedInstances")==published,"Rejected parent published an inheritance snapshot");
        tests.Add("changed-parent-discards-old-source-snapshot-full-XSD-rejection-no-new-publication");
    }
    sealed class StopVerifier : IAssetBuilderVerifierPlugin
    {
        readonly bool cancel;
        internal StopVerifier(bool cancel) { this.cancel=cancel; }
        public void Initialize(object config,TargetPlatform platform) { }
        public void ReInitialize(object config,TargetPlatform platform) { }
        public bool VerifyInstance(InstanceDeclaration instance)
        {
            if(cancel) throw new OperationCanceledException("Snapshot cancellation before document completion");
            return false;
        }
    }
    static void Incomplete(BuiltinCompilers plugins,SchemaSet schemas,bool cancel)
    {
        string name=cancel ? "cancelled" : "failed";
        string input=Write(name+"/root.xml","<ArmorTemplate id='SnapshotStopParent'/><ArmorTemplate id='SnapshotStopChild' inheritFrom='SnapshotStopParent'/>");
        var cache=Cache(schemas,name); var verifiers=new VerifierPluginRegistry(new PluginDescriptor[0],TargetPlatform.Win32);
        verifiers.DefaultPlugin=new StopVerifier(cancel); long published=Count("cache.inheritanceSnapshot.publishedInstances"); bool rejected=false;
        try { Run(Processor(plugins,schemas,cache,verifiers),input); }
        catch(OperationCanceledException) { rejected=cancel; }
        catch(BinaryAssetBuilderException error) { rejected=!cancel && error.ErrorCode==ErrorCode.GameDataVerification; }
        Require(rejected,"Pre-completion failure/cancellation did not propagate");
        var document=Document(cache,input);
        Require(document.State!=DocumentState.Complete && document.SelfInstances.All(value=>Snapshot(value)==null) &&
            Count("cache.inheritanceSnapshot.publishedInstances")==published,"Incomplete document published a requested snapshot");
        tests.Add(name+"-after-self-inheritance-request-before-completion-no-snapshot-publication");
    }
    public static int Main(string[] args)
    {
        object result; int code=0; Settings original=Settings.Current;
        try
        {
            if(args.Length!=5 || IntPtr.Size!=4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            work=Path.GetFullPath(args[0]); sdk=Path.GetFullPath(args[4]); Directory.CreateDirectory(work);
            var settings=Configure(); var schemas=new SchemaSet(false);
            using(var plugins=BuiltinCompilers.Create(TargetPlatform.Win32))
            { Streams(plugins,schemas,settings); Incomplete(plugins,schemas,false); Incomplete(plugins,schemas,true); }
            result=new {succeeded=true,sourceCommit=args[2],runId=args[3],checks=checks,tests=tests.ToArray(),counters=counters,snapshot=snapshotEvidence,
                scope="Request-equivalent sequential Core streams, source defaults/PSVI, invalidation, failure/cancellation and raw publication equality; no persisted snapshots"};
            Console.WriteLine("PASS InheritanceSnapshot: "+tests.Count+" groups; "+checks+" checks");
        }
        catch(Exception error)
        {
            code=1; result=new {succeeded=false,sourceCommit=args.Length>2?args[2]:null,runId=args.Length>3?args[3]:null,checks=checks,tests=tests.ToArray(),counters=counters,snapshot=snapshotEvidence,error=error.ToString()};
            Console.Error.WriteLine(error);
        }
        finally { Settings.Current=original; }
        if(args.Length>1) File.WriteAllText(args[1],new JavaScriptSerializer().Serialize(result)); return code;
    }
}
