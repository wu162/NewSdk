using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.XInclude;

public class MvpXmlOracle {
    static readonly List<string[]> Rows=new List<string[]>();
    static readonly BindingFlags All=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static|BindingFlags.DeclaredOnly;
    static void Check(bool ok) { if(!ok) throw new InvalidOperationException("Oracle invariant failed"); }
    static void Case(string name,Func<string> f) {
        try { Rows.Add(new[]{name,"OK:"+f()}); }
        catch(Exception e) { while(e is TargetInvocationException) e=e.InnerException; if(e.Message=="Oracle invariant failed") throw; Rows.Add(new[]{name,"ERR:"+e.GetType().FullName+":"+e.Message}); }
    }
    static string Sig(MethodBase m) {
        var p=new List<string>(); foreach(var x in m.GetParameters()) p.Add(x.ParameterType+":"+x.Attributes);
        return m.Name+"("+String.Join(",",p.ToArray())+")"+(m is MethodInfo?":"+((MethodInfo)m).ReturnType:"");
    }
    static void Contract() {
        Assembly a=typeof(XIncludingReader).Assembly;
        Rows.Add(new[]{"assembly",a.GetName().FullName});
        var entries=new List<string>();
        foreach(Type t in a.GetTypes()) {
            if(t.Name.StartsWith("<")||t.Namespace=="System.Runtime.CompilerServices") continue;
            entries.Add("type:"+t.FullName+":"+t.BaseType);
            foreach(Type i in t.GetInterfaces()) entries.Add(t.FullName+":interface:"+i.FullName);
            foreach(FieldInfo f in t.GetFields(All)) entries.Add(t.FullName+":field:"+f.Name+":"+f.FieldType+":"+(f.IsLiteral?Convert.ToString(f.GetRawConstantValue(),CultureInfo.InvariantCulture):""));
            foreach(MethodInfo m in t.GetMethods(All)) entries.Add(t.FullName+":method:"+Sig(m));
            foreach(ConstructorInfo c in t.GetConstructors(All)) entries.Add(t.FullName+":ctor:"+Sig(c));
        }
        foreach(string n in a.GetManifestResourceNames()) {
            entries.Add("resource:"+n);
            using(Stream s=a.GetManifestResourceStream(n)) using(var rr=new System.Resources.ResourceReader(s))
                foreach(System.Collections.DictionaryEntry e in rr) entries.Add("resource-value:"+n+":"+e.Key+":"+e.Value.GetType().FullName+":"+e.Value);
        }
        entries.Sort(StringComparer.Ordinal); foreach(string e in entries) Rows.Add(new[]{"contract",e});
    }
    class Resolver:BinaryAssetBuilder.Core.FileNameXmlResolver {
        public Resolver():base("C:/fixture") {}
        public readonly Dictionary<string,byte[]> Files=new Dictionary<string,byte[]>();
        public List<string> Calls=new List<string>();
        public override ICredentials Credentials {set{}}
        public void Add(string name,string data) { Files["file:///fixture/"+name]=Encoding.UTF8.GetBytes(data); }
        public override object GetEntity(Uri uri,string role,Type type) {
            Calls.Add(uri.AbsoluteUri); byte[] data;
            if(!Files.TryGetValue(uri.AbsoluteUri,out data)) throw new FileNotFoundException("fixture missing: "+uri.AbsoluteUri);
            return new MemoryStream(data,false);
        }
    }
    static string Read(XmlReader r) {
        var b=new StringBuilder(); int count=0;
        while(r.Read()) {
            Check(++count<2000);
            b.Append(r.NodeType).Append(':').Append(r.Depth).Append(':').Append(r.Name).Append(':').Append(r.Value).Append(':').Append(r.BaseURI).Append(':').Append(r.XmlLang).Append(':').Append(r.IsEmptyElement);
            if(r.HasAttributes) {
                if(r.MoveToFirstAttribute()) do {b.Append('|').Append(r.Name).Append('=').Append(r.Value);} while(r.MoveToNextAttribute());
                r.MoveToElement();
            }
            b.Append('\n');
        }
        b.Append("EOF:").Append(r.EOF).Append(':').Append(r.ReadState); return b.ToString();
    }
    static string Include(string body,bool wrapper) {
        var res=new Resolver();
        res.Add("one.xml","<one xml:lang='zh'><a id='A'>汉字</a><b>2</b></one>");
        res.Add("nested.xml","<nested xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='one.xml'/></nested>");
        res.Add("text.txt","A<&汉字\r\nZ");
        res.Add("cycle.xml","<cycle xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='cycle.xml'/></cycle>");
        res.Add("broken.xml","<broken>");
        string xml="<root xmlns:xi='http://www.w3.org/2001/XInclude' xml:lang='en'>"+body+"</root>";
        using(XmlReader input=XmlReader.Create(new StringReader(xml),new XmlReaderSettings(),"file:///fixture/root.xml")) {
            XmlReader r;
            if(wrapper) {
                BinaryAssetBuilder.Core.XIncludingReaderWrapper.LoadAssembly();
                r=BinaryAssetBuilder.Core.XIncludingReaderWrapper.GetReader(input,res);
                Check(r!=null);
            } else {var xi=new XIncludingReader(input);xi.XmlResolver=res;r=xi;}
            using(r) {string result=Read(r);return result+"\nresolve:"+String.Join(",",res.Calls.ToArray());}
        }
    }
    static void Includes() {
        string[] bodies={
            "<xi:include href='one.xml'/>",
            "<xi:include href='nested.xml'/>",
            "<xi:include href='text.txt' parse='text' encoding='utf-8'/>",
            "<xi:include href='one.xml' xpointer='element(/1/2)'/>",
            "<xi:include href='one.xml' xpointer='xpointer(/one/a)'/>",
            "<xi:include href='missing.xml'><xi:fallback><fallback>yes</fallback></xi:fallback></xi:include>",
            "<xi:include href='missing.xml'><xi:fallback/></xi:include>",
            "<xi:include href='missing.xml'/>",
            "<xi:include href='cycle.xml'/>",
            "<xi:include href='broken.xml'/>",
            "<xi:include href='one.xml' parse='invalid'/>",
            "<xi:include href='one.xml#fragment'/>",
            "<xi:include/>",
            "<xi:include href='text.txt' parse='text' encoding='not-an-encoding'/>",
            "<xi:fallback/>",
            "<xi:unknown/>",
            "<xi:include href='missing.xml'><xi:fallback/><xi:fallback/></xi:include>",
            "<xi:include href='missing.xml'><xi:include href='one.xml'/></xi:include>",
            "<xi:include href='one.xml' xpointer='element(/1/99)'><xi:fallback><empty/></xi:fallback></xi:include>",
            "<xi:include href='one.xml'/><xi:include href='one.xml'/>",
            "<!--before--><xi:include href='one.xml'/><?p after?>"
        };
        foreach(string culture in new[]{"en-US","de-DE","tr-TR"}) {
            Thread.CurrentThread.CurrentCulture=new CultureInfo(culture);
            for(int i=0;i<bodies.Length;i++) {int k=i;Case(culture+":include:"+i,delegate {return Include(bodies[k],false);});}
        }
        Thread.CurrentThread.CurrentCulture=CultureInfo.InvariantCulture;
        // These must actually expand data; matching exception-only transcripts cannot pass.
        Check(Include(bodies[0],false).Contains("汉字"));
        Check(Include(bodies[1],false).Contains("nested"));
        Check(Include(bodies[2],false).Contains("A<&汉字"));
        Check(Include(bodies[5],false).Contains("fallback"));
        Case("core-reflection-route",delegate {string s=Include(bodies[0],true);Check(s.Contains("汉字"));return s;});
    }
    static int Main(string[] args) {
        try {Contract();Includes();Extras();File.WriteAllText(args[0],new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(Rows),new UTF8Encoding(false));Console.WriteLine("PASS "+Rows.Count+" Mvp.Xml contract/resource/behavior rows");return 0;}
        catch(Exception e) {Console.Error.WriteLine(e);return 1;}
    }
    static string Nodes(XPathNodeIterator it) {
        var s=new StringBuilder();while(it.MoveNext()) s.Append(it.Current.OuterXml).Append('|');return s.ToString();
    }
    static XPathNavigator Nav() {return new XPathDocument(new StringReader("<r xmlns:p='urn:p'><n>3</n><n>1</n><n>3</n><p:n>7</p:n></r>")).CreateNavigator();}
    public class Data {public string Name;public int Number;}
    static void Extras() {
        foreach(string culture in new[]{"en-US","de-DE","tr-TR"}) {
            Thread.CurrentThread.CurrentCulture=new CultureInfo(culture);
            foreach(string pointer in new[]{"element(/1)","element(/1/2)","xpointer(/r/n)","xpath1(/r/n[1])","xmlns(p=urn:p) xpointer(/r/p:n)","element(/1/99)","element(/0)","element()","xpointer(","",null,"unknown(foo) element(/1/1)"}) {
                string p=pointer;Case(culture+":pointer:"+p,delegate{return Nodes(Mvp.Xml.XPointer.Pointer.Compile(p).Evaluate(Nav()));});
            }
            foreach(string expression in new[]{"count(/r/n)","sum(/r/n)","/r/n","$v + 2","count(/r/p:n)","bad(","/r/n[$v]"}) {
                string e=expression;Case(culture+":xpath:"+e,delegate {
                    object result=Mvp.Xml.Common.XPath.XPathCache.Evaluate(e,Nav(),new[]{new Mvp.Xml.Common.XmlPrefix("p","urn:p")},new Mvp.Xml.Common.XPath.XPathVariable("v",2));
                    return result is XPathNodeIterator?Nodes((XPathNodeIterator)result):Convert.ToString(result,CultureInfo.InvariantCulture);
                });
            }
            Case(culture+":xpath-sort",delegate{return Nodes(Mvp.Xml.Common.XPath.XPathCache.SelectSorted("/r/n",Nav(),".",XmlSortOrder.Descending,XmlCaseOrder.None,"",XmlDataType.Number));});
            foreach(string value in new[]{"","a b\tc","a,b,,c","汉字<&","Iıİi"}) {
                string v=value;
                Case(culture+":strings:"+v,delegate{var f=new Mvp.Xml.Exslt.ExsltStrings();return Nodes(f.tokenize(v," ,\t"))+":"+Nodes(f.split(v,","))+":"+f.align(v,"..........","center")+":"+f.padding(7,v.Length==0?"x":v)+":"+f.decodeUri(f.encodeUri(v,true));});
            }
            foreach(double number in new[]{-4.0,-1.0,0.0,1.0,4.0,double.NaN,double.PositiveInfinity}) {
                double n=number;Case(culture+":math:"+n.ToString(CultureInfo.InvariantCulture),delegate{var f=new Mvp.Xml.Exslt.ExsltMath();return f.abs(n).ToString("R",CultureInfo.InvariantCulture)+":"+f.sqrt(n).ToString("R",CultureInfo.InvariantCulture)+":"+f.power(n,2).ToString("R",CultureInfo.InvariantCulture);});
            }
            Case(culture+":sets",delegate{var f=new Mvp.Xml.Exslt.ExsltSets();return Nodes(f.distinct(Nav().Select("/r/n")))+":"+Nodes(f.difference(Nav().Select("/r/n"),Nav().Select("/r/n[1]")));});
            Case(culture+":math-nodes",delegate{var f=new Mvp.Xml.Exslt.ExsltMath();return f.min(Nav().Select("/r/n"))+":"+f.max(Nav().Select("/r/n"))+":"+Nodes(f.highest(Nav().Select("/r/n")));});
            foreach(string date in new[]{"2000-02-29T12:34:56Z","2006-03-01","bad",""}) {
                string d=date;Case(culture+":date:"+d,delegate{var f=new Mvp.Xml.Exslt.ExsltDatesAndTimes();return f.date(d)+":"+f.time(d)+":"+f.year(d)+":"+f.leapYear(d);});
            }
        }
        Thread.CurrentThread.CurrentCulture=CultureInfo.InvariantCulture;
        Case("iterator-clone",delegate {
            var i=new Mvp.Xml.Common.XPath.XPathNavigatorIterator(Nav().Select("/r/n"));
            Check(i.Count==3);i.MoveNext();var clone=i.Clone();string s=Nodes(clone);i.Reset();i.RemoveAt(1);return s+":"+Nodes(i);
        });
        foreach(bool debug in new[]{false,true}) {bool d=debug;Case("xslt:"+d,delegate{
            var t=new Mvp.Xml.Common.Xsl.MvpXslTransform(d);
            string style="<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:math='http://exslt.org/math'><xsl:output omit-xml-declaration='yes'/><xsl:template match='/'><out><xsl:value-of select='math:max(/r/n)'/></out></xsl:template></xsl:stylesheet>";
            using(var r=XmlReader.Create(new StringReader(style))) t.Load(r);
            var w=new StringWriter(CultureInfo.InvariantCulture);
            t.Transform(new Mvp.Xml.Common.Xsl.XmlInput(Nav()),null,new Mvp.Xml.Common.Xsl.XmlOutput(w));
            Check(w.ToString().Contains(">3</out>"));return w.ToString();
        });}
        Case("serializer-cache",delegate{
            using(var c=new Mvp.Xml.Common.Serialization.XmlSerializerCache()) {
                var a=c.GetSerializer(typeof(Data),(string)null);var b=c.GetSerializer(typeof(Data),(string)null);Check(Object.ReferenceEquals(a,b));
                var w=new StringWriter(CultureInfo.InvariantCulture);a.Serialize(w,new Data{Name="汉字",Number=42});
                var data=(Data)b.Deserialize(new StringReader(w.ToString()));Check(data.Number==42);return w.ToString()+":"+data.Name;
            }
        });
    }

}
