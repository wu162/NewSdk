using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using BinaryAssetBuilder.Core;

// Separate original/rebuilt processes prevent assembly identity unification.
class BuildInfrastructureContract
{
    static readonly List<string> Rows = new List<string>();
    static readonly BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    static string Text(object x) { return x == null ? "<null>" : Convert.ToString(x, CultureInfo.InvariantCulture); }
    static string Signature(MethodBase m) {
        var ps = new List<string>();
        foreach (var p in m.GetParameters()) ps.Add(p.ParameterType.ToString() + ":" + p.Attributes);
        return m.Name + "(" + String.Join(",", ps.ToArray()) + ")" + (m is MethodInfo ? ":" + ((MethodInfo)m).ReturnType : "");
    }
    static void Contract(Assembly a) {
        Rows.Add("assembly:" + a.GetName().Name + ":" + a.GetName().Version);
        foreach (Type t in a.GetTypes()) {
            if (t.Name.StartsWith("<")) continue;
            // Roslyn sets BeforeFieldInit on interfaces; no interface has static state here.
            TypeAttributes attrs=t.Attributes;
            if(t.IsInterface && t.GetFields(All).Length==0) attrs &= ~TypeAttributes.BeforeFieldInit;
            Rows.Add("type:" + t.FullName + ":" + t.BaseType + ":" + attrs);
            foreach (Type i in t.GetInterfaces()) Rows.Add(t.FullName + ":interface:" + i);
            foreach (FieldInfo f in t.GetFields(All)) {
                if (f.Name.StartsWith("<")) continue;
                Rows.Add(t.FullName + ":field:" + f.Name + ":" + f.FieldType + ":" + f.Attributes + ":" + (f.IsLiteral ? Text(f.GetRawConstantValue()) : ""));
            }
            foreach (MethodInfo m in t.GetMethods(All)) {
                if (!m.Name.StartsWith("<")) Rows.Add(t.FullName + ":method:" + Signature(m) + ":" + m.Attributes);
            }
            foreach (ConstructorInfo c in t.GetConstructors(All)) {
                // Original IL is only ldsflda _TheNop; initobj Nop; ret (12 bytes).
                // Roslyn omits this redundant zero initializer; the C# initializer remains in Set.cs.
                if(t.FullName=="BinaryAssetBuilder.Core.Set`1" && c.IsStatic) {
                    byte[] il=c.GetMethodBody().GetILAsByteArray();
                    if(il.Length!=12 || il[0]!=0x7f || il[5]!=0xfe || il[6]!=0x15 || il[11]!=0x2a)
                        throw new InvalidOperationException("Unexpected Set<T> initializer; re-audit normalization");
                    continue;
                }
                Rows.Add(t.FullName + ":ctor:" + Signature(c) + ":" + c.Attributes);
            }
        }
        foreach (string name in a.GetManifestResourceNames()) {
            using (Stream s = a.GetManifestResourceStream(name))
            using (var hash = System.Security.Cryptography.SHA256.Create())
                Rows.Add("resource:" + name + ":" + Convert.ToBase64String(hash.ComputeHash(s)));
        }
    }
    static void Case(string name, Func<string> f) {
        try { Rows.Add("case:" + name + ":" + f()); }
        catch (Exception e) { while(e is TargetInvocationException) e=e.InnerException; Rows.Add("case:" + name + ":ERR:" + e.GetType().FullName + ":" + e.Message); }
    }
    static string Xml(IXmlSerializable x) {
        var sw = new StringWriter(CultureInfo.InvariantCulture);
        using (var w=XmlWriter.Create(sw, new XmlWriterSettings { OmitXmlDeclaration=true })) { x.WriteXml(w); }
        return sw.ToString();
    }
    static void CoreCases() {
        foreach (string culture in new[] { "en-US", "de-DE", "tr-TR" }) {
            System.Threading.Thread.CurrentThread.CurrentCulture=new CultureInfo(culture);
            foreach (string name in new[] { "", "Object", "object", "I", "i", "a:b", "汉字", null }) {
                string n=name;
                Case(culture+":handle:"+Text(n), delegate {
                    var a=new InstanceHandle("GameObject", n); var b=new InstanceHandle(a);
                    a.TypeHash=uint.MaxValue; a.InstanceHash=123;
                    string xml=Xml(a); var c=new InstanceHandle();
                    using(var r=XmlReader.Create(new StringReader(xml))) { r.MoveToContent(); c.ReadXml(r); }
                    return a.Name+"|"+a.FullName+"|"+a.FileBase+"|"+a.GetHashCode()+"|"+a.Equals(b)+"|"+a.CompareTo(c)+"|"+xml;
                });
                Case(culture+":single:"+Text(n), delegate { var a=new InstanceHandle(n); return a.FullName; });
                Case(culture+":definition:"+Text(n), delegate {
                    var d=new Definition { Name=n, OriginalValue="original", EvaluatedValue="value", IsOverride=true };
                    var set=new DefinitionSet(); set.Add(n,d);
                    var copy=new DefinitionSet(); copy.AddDefinitions(set);
                    return copy.GetEvaluatedValue(n)+"|"+d.ToString()+"|"+Xml(d);
                });
            }
        }
    }
    static void CollectionCases() {
        for(int seed=0;seed<20;seed++) {
            int sample=seed;
            Case("core:list:"+seed,delegate {
                var r=new Random(sample); var list=new List2<int>();
                for(int i=0;i<80;i++) list.Add(r.Next(-20,21));
                list.InsertRange(5,new[]{100,200,300}); list.RemoveRange(7,3);
                list.Reverse(2,15); list.Sort();
                var parts=new List<string>();
                foreach(int n in list) parts.Add(n.ToString(CultureInfo.InvariantCulture));
                parts.Add("search="+list.BinarySearch(3));parts.Add("find="+list.FindIndex(delegate(int x){return x>0;}));
                parts.Add("remove="+list.RemoveAll(delegate(int x){return x<0;}));parts.Add("count="+list.Count);
                var e=list.GetEnumerator();e.MoveNext();list.Add(88);
                try{e.MoveNext();parts.Add("enumerator:accepted");}catch(Exception ex){parts.Add("enumerator:"+ex.GetType().FullName);}
                return String.Join(",",parts.ToArray());
            });
        }
        for(int seed=0;seed<20;seed++) {
            int sample=seed;
            Case("core:set:"+seed,delegate {
                var r=new Random(sample); var set=new Set<int>();
                for(int i=0;i<60;i++) set.Add(r.Next(-10,11));
                var copy=new Set<int>(set); set.AddRange(new[]{100,100,200});set.Remove(0);
                var result=new List<string>();foreach(int n in set) result.Add(n.ToString());
                result.Add("contains="+set.Contains(100));result.Add("copy="+copy.Count);result.Add("hash="+set.GetHashCode());
                result.Add("empty="+Set<int>.Empty.Count);copy.Clear();result.Add("cleared="+copy.Count);
                return String.Join(",",result.ToArray());
            });
        }
        foreach(int index in new[]{-1,0,1,10,int.MaxValue}) {
            int i=index;
            Case("core:list:index:"+i,delegate {return new List2<int>()[i].ToString();});
        }
    }
    static void BuildCases() {
        Assembly a=Assembly.Load("EALABuild");
        Type paths=a.GetType("EALABuild.StaticPaths",true);
        foreach(MethodInfo m in paths.GetMethods(BindingFlags.Public|BindingFlags.Static)) {
            if(m.GetParameters().Length!=1 || m.GetParameters()[0].ParameterType!=typeof(string) || m.ReturnType!=typeof(string)) continue;
            foreach(string name in new[] { "", "mymod", "My Mod", "a/b", "../edge", null }) {
                MethodInfo method=m; string n=name;
                Case("build:path:"+m.Name+":"+Text(n),delegate{return Text(method.Invoke(null,new object[]{n}));});
            }
        }
        foreach(Type t in a.GetTypes()) {
            if(t.Namespace!="EALABuild" || !t.Name.EndsWith("Arguments")) continue;
            foreach(ConstructorInfo c in t.GetConstructors()) {
                ConstructorInfo ctor=c; var ps=c.GetParameters(); var vals=new object[ps.Length]; bool valid=true;
                for(int i=0;i<ps.Length;i++) {
                    if(ps[i].ParameterType==typeof(string)) vals[i]="test-value";
                    else if(ps[i].ParameterType==typeof(bool)) vals[i]=true;
                    else if(ps[i].ParameterType==typeof(string[])) vals[i]=new[]{"one","two"};
                    else {valid=false; break;}
                }
                if(!valid) continue;
                Type type=t; object[] values=vals;
                Case("build:args:"+t.FullName+":"+Signature(c),delegate{
                    object instance=ctor.Invoke(values); var rows=new List<string>();
                    foreach(PropertyInfo prop in type.GetProperties(BindingFlags.Public|BindingFlags.Instance)) {
                        if(prop.CanRead && prop.GetIndexParameters().Length==0) rows.Add(prop.Name+"="+Text(prop.GetValue(instance,null)));
                        if(prop.CanWrite && prop.PropertyType==typeof(bool)) {prop.SetValue(instance,false,null);rows.Add(prop.Name+"-set="+Text(prop.GetValue(instance,null)));}
                    }
                    rows.Sort(StringComparer.Ordinal);return String.Join("|",rows.ToArray());
                });
            }
        }
    }
    static int Main(string[] args) {
        foreach (string name in new[] { "BinaryAssetBuilder.Core", "BinaryAssetBuilder.Remote", "EALABuild" }) Contract(Assembly.Load(name));
        CoreCases(); CollectionCases(); BuildCases(); Rows.Sort(StringComparer.Ordinal); File.WriteAllLines(args[0],Rows.ToArray());
        Console.WriteLine("PASS records="+Rows.Count); return 0;
    }
}
