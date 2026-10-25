using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using U = BinaryAssetBuilder.Utility;

// Run the same executable in separate original/rebuilt directories. No target DLL is
// resolved from the repository after process startup. Avoid malformed native pointers.
class UtilityOracle
{
    static List<string> Rows = new List<string>();
    static string Work, Built;
    static BindingFlags Decl = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    static string N(string s) {
        if(s==null)return "<null>";
        s=s.Replace("_003C","<").Replace("_003E",">").Replace("_003A",":");
        if(Work!=null)s=s.Replace(Work,"<work>");
        if(Built!=null)s=s.Replace(Built,"<builtmods>");
        return s.Replace("\r","\\r").Replace("\n","\\n").Replace("\0","\\0");
    }
    static string V(object o) { return o==null?"<null>":N(Convert.ToString(o,CultureInfo.InvariantCulture)); }
    static void Row(string k,object v) { Rows.Add(k+":"+V(v)); }
    static void Case(string k,Func<string> f) {
        try{Row("case:"+k,f());}catch(Exception e){while(e is TargetInvocationException && e.InnerException!=null)e=e.InnerException;Row("case:"+k,"ERR:"+e.GetType().FullName+":"+e.Message);}
    }
    static string Bytes(byte[] b) { using(var s=SHA256.Create())return b.Length+":"+Convert.ToBase64String(s.ComputeHash(b)); }
    static string Snapshot(object o) {
        var r=new List<string>();
        foreach(var p in o.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance)) {
            if(p.GetIndexParameters().Length!=0 || !p.CanRead)continue;
            if(p.PropertyType==typeof(string) || p.PropertyType.IsPrimitive)r.Add(p.Name+"="+V(p.GetValue(o,null)));
        }
        r.Sort(StringComparer.Ordinal);return String.Join("|",r.ToArray());
    }
    static string Sig(MethodBase m) {
        var a=new List<string>();foreach(var p in m.GetParameters())a.Add(N(p.ParameterType.ToString()));
        return m.Name+"("+String.Join(",",a.ToArray())+")"+(m is MethodInfo?":"+N(((MethodInfo)m).ReturnType.ToString()):"");
    }
    static void Contract() {
        var a=typeof(U.Manifest).Assembly; Row("assembly",a.GetName().Name+":"+a.GetName().Version);
        foreach(var t in a.GetExportedTypes()) {
            if(t.Namespace!="BinaryAssetBuilder.Utility" && t.Namespace!="AssetStream")continue;
            Row("api:type",N(t.FullName)+":"+N(t.BaseType==null?"":t.BaseType.ToString()));
            foreach(var m in t.GetMethods(Decl))Row("api:method",N(t.FullName)+":"+Sig(m)+":"+m.IsStatic);
            foreach(var c in t.GetConstructors(Decl))Row("api:ctor",N(t.FullName)+":"+Sig(c));
            foreach(var f in t.GetFields(Decl))Row("api:field",N(t.FullName)+":"+f.Name+":"+N(f.FieldType.ToString()));
        }
    }
    static string SaveName(U.NameBuffer n) { using(var s=new MemoryStream()){n.SaveToStream(s);return Bytes(s.ToArray());} }
    static string SaveReference(U.ReferencedFileBuffer n) { using(var s=new MemoryStream()){n.SaveToStream(s);return Bytes(s.ToArray());} }
    static string SaveUInt(U.UInt32Buffer n,bool be) { using(var s=new MemoryStream()){n.SaveToStream(s,be);return Bytes(s.ToArray());} }
    static string[] Collision() {
        var seen=new Dictionary<int,string>();uint state=1234567;
        for(int i=0;i<400000;i++) {
            state=unchecked(state*1664525+1013904223);string x=state.ToString("x8")+":"+i.ToString("x");int h=x.GetHashCode();string old;
            if(seen.TryGetValue(h,out old) && old!=x)return new[]{old,x};seen[h]=x;
        }
        throw new Exception("Deterministic collision search found no pair");
    }
    static void Buffers() {
        foreach(string culture in new[]{"en-US","de-DE","tr-TR"}) {
            Thread.CurrentThread.CurrentCulture=new CultureInfo(culture);
            var names=new List<string>(new[]{"","a","A","I","i","ı","İ","汉字","é","e\u0301","\0","x\0y","x","a/b","a\\b",new string('x',16384)});
            for(int i=0;i<1000;i++)names.Add("name-"+i+"-"+(i%23));
            var nb=new U.NameBuffer();var rb=new U.ReferencedFileBuffer();
            for(int i=0;i<names.Count;i++) {
                int j=i;string s=names[i];
                Case(culture+":name:"+j,delegate{return nb.AddName(s)+"|"+nb.AddName(s)+"|"+nb.Length;});
                Case(culture+":reference:"+j,delegate{return rb.AddReference(s,j%2==0)+"|"+rb.AddReference(s,j%2!=0)+"|"+rb.Length;});
            }
            Case(culture+":name:null",delegate{return nb.AddName(null).ToString();});
            Case(culture+":reference:null",delegate{return rb.AddReference(null,false).ToString();});
            Row(culture+":name:bytes",SaveName(nb));Row(culture+":reference:bytes",SaveReference(rb));
            Case(culture+":name:nullstream",delegate{nb.SaveToStream(null);return "accepted";});
            Case(culture+":reference:nullstream",delegate{rb.SaveToStream(null);return "accepted";});
            nb.Dispose();rb.Dispose();nb.Dispose();rb.Dispose();
            Row(culture+":postdispose",nb.AddName("after")+"|"+rb.AddReference("after",true)+"|"+SaveName(nb)+"|"+SaveReference(rb));
        }
        Thread.CurrentThread.CurrentCulture=CultureInfo.InvariantCulture;
        string[] pair=Collision();Row("collision:inputs",pair[0]+"|"+pair[1]);
        using(var n=new U.NameBuffer()) {int a=n.AddName(pair[0]);int b=n.AddName(pair[1]);Row("collision:name",a+"|"+b+"|"+SaveName(n));if(a!=b)throw new Exception("Original hash-only name deduplication lost");}
        using(var n=new U.ReferencedFileBuffer()) {int a=n.AddReference(pair[0],false);int b=n.AddReference(pair[1],true);Row("collision:reference",a+"|"+b+"|"+SaveReference(n));if(a!=b)throw new Exception("Original hash-only reference deduplication lost");}
        using(var n=new U.UInt32Buffer()) {
            Row("uint:empty",SaveUInt(n,false));uint st=42;
            for(int i=0;i<2048;i++){st=unchecked(st*1664525+1013904223);uint val=i<5?new uint[]{0,1,0x80000000,uint.MaxValue,0x12345678}[i]:st;Row("uint:offset:"+i,n.AddValue(val)+"|"+n.Length);}
            Row("uint:little",SaveUInt(n,false));Row("uint:big",SaveUInt(n,true));
            Case("uint:nullstream",delegate{n.SaveToStream(null,true);return "accepted";});
            n.Dispose();Row("uint:postdispose",n.AddValue(17)+"|"+SaveUInt(n,false));
        }
    }
    class ShortStream : MemoryStream {
        int limit;public ShortStream(byte[] b,int max):base(b){limit=max;}
        public override int Read(byte[] b,int o,int c){return base.Read(b,o,Math.Min(c,limit));}
    }
    class FailingStream : MemoryStream {
        public override void Write(byte[] b,int o,int c){throw new IOException("oracle-write-failure");}
    }
    static void Call(object o,string name,params object[] args){o.GetType().GetMethod(name).Invoke(o,args);}
    static byte[] Save(object o,bool be){using(var s=new MemoryStream()){Call(o,"SaveToStream",s,be);return s.ToArray();}}
    static void Headers() {
        foreach(var type in new[]{typeof(U.AssetHeader),typeof(U.AssetEntry),typeof(U.ManifestHeader)}) {
            int size=type==typeof(U.AssetHeader)?32:48;
            for(int seed=0;seed<16;seed++)foreach(bool be in new[]{false,true}) {
                object h=Activator.CreateInstance(type);byte[] raw=new byte[size];new Random(seed+17).NextBytes(raw);Call(h,"LoadFromBuffer",raw,false);
                foreach(var p in type.GetProperties(BindingFlags.Public|BindingFlags.Instance))if(p.CanWrite) {
                    uint v=unchecked((uint)(seed*1234567+0x12345678));object x=p.PropertyType==typeof(bool)?(object)(seed%2==0):p.PropertyType==typeof(ushort)?(object)(ushort)v:p.PropertyType==typeof(uint)?(object)v:(object)unchecked((int)v);p.SetValue(h,x,null);
                }
                string key="header:"+type.Name+":"+seed+":"+be;
                Row(key+":properties",Snapshot(h));byte[] saved=Save(h,be);Row(key+":bytes",Bytes(saved));Row(key+":after",Snapshot(h));
                object h2=Activator.CreateInstance(type);Call(h2,"LoadFromBuffer",saved,be);Row(key+":reload",Snapshot(h2));((IDisposable)h2).Dispose();
                if(type==typeof(U.AssetHeader))foreach(long l in new long[]{-1,0,31,32,33,0x100000020L,long.MaxValue})Row(key+":length:"+l,((U.AssetHeader)h).IsValidFileLength(l));
                Case(key+":write-failure",delegate{Call(h,"SaveToStream",new FailingStream(),be);return "accepted";});Row(key+":after-failure",Snapshot(h));
                ((IDisposable)h).Dispose();
            }
            foreach(int limit in new[]{0,1,7,31,47,48})foreach(bool be in new[]{false,true}) {
                object h=Activator.CreateInstance(type);byte[] raw=new byte[size];for(int i=0;i<size;i++)raw[i]=(byte)(i*13+3);
                using(var stream=new ShortStream(raw,limit)){Call(h,"LoadFromStream",stream,be);Row("short:"+type.Name+":"+limit+":"+be,stream.Position+"|"+Snapshot(h)+"|"+Bytes(Save(h,false)));}((IDisposable)h).Dispose();
            }
            foreach(bool be in new[]{false,true}) {
                object h=Activator.CreateInstance(type);
                Case("header:"+type.Name+":empty:"+be,delegate{Call(h,"LoadFromBuffer",new byte[0],be);return "accepted";});
                Case("header:"+type.Name+":null:"+be,delegate{Call(h,"LoadFromBuffer",null,be);return "accepted";});
                ((IDisposable)h).Dispose();
            }
        }
        Row("manifest:latest",U.ManifestHeader.LatestVersion);
    }
    static void WriteManifest(string path,int version,bool be,bool linked,string reference,bool patch,int assets,bool zeroSizes) {
        byte[] names=Encoding.ASCII.GetBytes("Type:One\0Type:Two\0"),sources=Encoding.ASCII.GetBytes("one.xml\0two.xml\0");
        byte[] references=reference==null?new byte[0]:Encoding.ASCII.GetBytes((patch?"\u0002":"\u0001")+reference+"\0");
        using(var output=File.Create(path))using(var h=new U.ManifestHeader()) {
            h.Version=(ushort)version;h.IsLinked=linked;h.AssetCount=(uint)assets;h.AssetNameBufferSize=(uint)names.Length;h.SourceFileNameBufferSize=(uint)sources.Length;
            h.ReferencedManifestNameBufferSize=(uint)references.Length;h.AssetReferenceBufferSize=(uint)(assets*8);h.MaxInstanceChunkSize=7;h.MaxRelocationChunkSize=11;h.MaxImportsChunkSize=13;h.AllTypesHash=0xfedcba98;h.StreamChecksum=0x12345678;h.TotalInstanceDataSize=(uint)(assets*7);h.SaveToStream(output,be);
            for(int i=0;i<assets;i++)using(var a=new U.AssetEntry()) {
                a.LoadFromBuffer(new byte[48],false);a.TypeId=100;a.InstanceId=(uint)(200+i);a.TypeHash=300;a.InstanceHash=(uint)(400+i);a.NameOffset=i*9;a.SourceFileNameOffset=i*8;a.AssetReferenceOffset=i*8;a.AssetReferenceCount=1;
                a.InstanceDataSize=zeroSizes?0:7;a.RelocationDataSize=zeroSizes?0:11;a.ImportsDataSize=zeroSizes?0:13;a.Tokenized=(uint)i;a.SaveToStream(output,be);
            }
            using(var b=new U.UInt32Buffer()){for(int i=0;i<assets;i++){b.AddValue(100);b.AddValue((uint)(200+i));}b.SaveToStream(output,be);}
            output.Write(references,0,references.Length);output.Write(names,0,names.Length);output.Write(sources,0,sources.Length);
        }
    }
    static string Assets(U.Manifest m) {
        var results=new List<string>();foreach(var a in m.Assets) {
            results.Add(Snapshot(a));foreach(var r in a.ExternalReferences)results.Add("ref:"+r.TypeId+":"+r.InstanceId+":"+r.TypeName+":"+r.InstanceName+":"+r.QualifiedName);
        }
        return String.Join(";",results.ToArray());
    }
    static void Inspect(string key,string path,bool validate) {
        using(var m=new U.Manifest()) {
            bool loaded=m.Load(path,null,validate);Row(key+":loaded",loaded);Row(key+":state",Snapshot(m));if(!loaded)return;
            Row(key+":assets",Assets(m));foreach(var r in m.ReferencedManifests)Row(key+":reference",r.Path+"|"+r.IsPatch);
            if(m.PatchManifest!=null)Row(key+":patch",Snapshot(m.PatchManifest)+"|"+Assets(m.PatchManifest));
            using(var entry=new U.AssetEntry()) {
                entry.LoadFromBuffer(new byte[48],false);
                for(int i=0;i<m.Assets.Length;i++) {
                    var asset=m.Assets[i];entry.TypeId=asset.TypeId;entry.InstanceId=asset.InstanceId;entry.TypeHash=asset.TypeHash;entry.InstanceHash=asset.InstanceHash;
                    Row(key+":equivalent:"+i,U.Manifest.IsEquivalent(entry,asset)+"|"+m.GetBaseStreamPosition(entry));entry.TypeHash^=1;Row(key+":type-mismatch:"+i,U.Manifest.IsEquivalent(entry,asset));entry.InstanceHash^=1;Row(key+":instance-mismatch:"+i,U.Manifest.IsEquivalent(entry,asset));
                }
                entry.InstanceId=uint.MaxValue;Row(key+":absent",m.GetBaseStreamPosition(entry));
            }
            Case(key+":reload",delegate{return m.Reload()+"|"+Snapshot(m)+"|"+Assets(m);});
        }
    }
    static void Manifests() {
        Directory.CreateDirectory(Work);
        using(var m=new U.Manifest()){Row("new:manifest",m.IsLoaded+"|"+m.Reload()+"|"+m.StringFromHash(0)+"|"+m.StringFromHash(uint.MaxValue));Case("manifest:missing",delegate{return m.Load(Path.Combine(Work,"missing.manifest")).ToString();});}
        for(int ver=3;ver<=6;ver++)foreach(bool be in new[]{false,true})foreach(bool linked in new[]{false,true}) {
            string key="manifest:v"+ver+":"+be+":"+linked,path=Path.Combine(Work,"v"+ver+"-"+be+"-"+linked+".manifest");WriteManifest(path,ver,be,linked,"external.manifest",false,2,false);
            Case(key,delegate{Inspect(key,path,false);return "done";});
        }
        string basePath=Path.Combine(Work,"base.manifest"),patchPath=Path.Combine(Work,"patch.manifest");WriteManifest(basePath,6,false,true,null,false,2,false);WriteManifest(patchPath,6,false,true,"base.manifest",true,2,true);
        Case("patch:present",delegate{Inspect("patch:present",patchPath,false);return "done";});
        WriteManifest(patchPath,6,false,false,"missing-base.manifest",true,0,true);Case("patch:missing",delegate{Inspect("patch:missing",patchPath,false);return "done";});
        string hashDir=Path.Combine(Work,"hashes");Directory.CreateDirectory(hashDir);string hashXml=Path.Combine(hashDir,"StringHashes.xml");
        foreach(string xml in new[]{"<root><StringAndHash Hash='100' Text='Type'/><StringAndHash Hash='200' Text='One'/><StringAndHash Hash='100' Text='Ignored'/></root>","<root><StringAndHash Hash='100' Text='Type'/>","<root><StringAndHash Hash='nonnumeric' Text='Bad'/></root>"}) {
            File.WriteAllText(hashXml,xml);using(var m=new U.Manifest())Case("hashxml:"+xml,delegate{m.LoadStringHashes(Path.Combine(hashDir,"nested"));return m.StringFromHash(100)+"|"+m.StringFromHash(200)+"|"+m.StringFromHash(201);});
        }
        string[] files=Directory.GetFiles(Built,"*.manifest",SearchOption.AllDirectories);Array.Sort(files,StringComparer.OrdinalIgnoreCase);
        foreach(string p in files){string file=p;Case("sdk:"+N(file),delegate{Inspect("sdk:"+N(file),file,false);return "done";});}
    }
    static int Main(string[] args) {
        Work=Path.GetFullPath(args[2]);Built=Path.GetFullPath(args[1]);Thread.CurrentThread.CurrentCulture=CultureInfo.InvariantCulture;
        Contract();Buffers();Headers();Manifests();
        foreach(uint error in new uint[]{0,1,2,5,6,87,123,99999,uint.MaxValue}) {uint e=error;Case("win32:error:"+e,delegate{return U.FileSystemUtils.GetErrorString(e);});}
        foreach(char c in new[]{'\0','?','/','1'}){char drive=c;Case("win32:invaliddrive:"+(int)c,delegate{U.FileSystemUtils.FlushVolume(drive);return "unexpected-success";});}
        foreach(string path in new[]{null,"","base.manifest","../base.manifest"})foreach(bool patch in new[]{false,true}) {var r=new U.ReferencedManifest(path,patch);Row("referencedmanifest:"+V(path)+":"+patch,r.Path+"|"+r.IsPatch);}
        var handle=new U.ReferenceHandle();Row("referencehandle:default",handle.TypeName+"|"+handle.InstanceName+"|"+handle.QualifiedName+"|"+handle.TypeId+"|"+handle.InstanceId);
        Rows.Sort(StringComparer.Ordinal);File.WriteAllLines(args[0],Rows.ToArray(),new UTF8Encoding(false));Console.WriteLine("PASS records="+Rows.Count);return 0;
    }
}
