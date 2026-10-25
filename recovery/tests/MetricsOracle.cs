using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Web.Script.Serialization;
using EALA.Metrics;
class MetricsOracle {
    static readonly List<string[]> Rows = new List<string[]>();
    static BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    static string Text(object v) { return v == null ? "<null>" : Convert.ToString(v, CultureInfo.InvariantCulture); }
    static void Case(string name, Func<string> action) {
        try { Rows.Add(new[]{name,"OK:"+action()}); }
        catch(Exception e) { while(e is TargetInvocationException) e=e.InnerException; if(e.Message=="Oracle invariant failed") throw; Rows.Add(new[]{name,"ERR:"+e.GetType().FullName+":"+e.Message}); }
    }
    static void Check(bool value) { if(!value) throw new Exception("Oracle invariant failed"); }
    static Metric Fixed(MetricDescriptor d, params object[] values) {
        DateTime before=DateTime.Now; var m=new Metric(d,values);
        Check(m.TimeStamp>=before && m.TimeStamp<=DateTime.Now);
        typeof(Metric).GetField("m_timestamp",All).SetValue(m,new DateTime(2006,7,8,9,10,11)); return m;
    }
    static string Signature(MethodBase m)
    {
        var parts = new List<string>();
        foreach (var p in m.GetParameters()) parts.Add(p.ParameterType.ToString());
        return m.Name + "(" + string.Join(",", parts.ToArray()) + ")" + (m is MethodInfo ? ":" + ((MethodInfo)m).ReturnType : "");
    }
    static void Contract(Assembly assembly)
    {
        var entries = new List<string>();
        foreach (var type in assembly.GetTypes())
        {
            if (type.Name.StartsWith("<") || type.Namespace == "System.Runtime.CompilerServices") continue;
            entries.Add("type:" + type.FullName + ":" + type.BaseType);
            foreach (var it in type.GetInterfaces()) entries.Add(type.FullName + ":interface:" + it.FullName);
            foreach (var f in type.GetFields(All))
                entries.Add(type.FullName + ":field:" + f.Name + ":" + f.FieldType + ":" + (f.IsLiteral ? Text(f.GetRawConstantValue()) : ""));
            foreach (var m in type.GetMethods(All)) entries.Add(type.FullName + ":method:" + Signature(m));
            foreach (var c in type.GetConstructors(All)) entries.Add(type.FullName + ":ctor:" + Signature(c));
        }
        entries.Sort(StringComparer.Ordinal);
        foreach (string entry in entries) Rows.Add(new[] { "contract", entry });
    }

    class Listener : IMetricsListener {
        public List<string> Events=new List<string>();
        public bool Timer;
        public void Open(){Events.Add("open");}
        public void Close(){Events.Add("close");}
        public void SubmitMetrics(Metric m) {
            if(Timer) { Check(m.Data is double && (double)m.Data>=0); Events.Add(m.Name+":duration:"+m.DataList.Length+":"+(m.DataList.Length>1?Text(m.DataList[1]):"")); }
            else Events.Add(m.Name+":"+(m.Data==null?"null":m.Data.GetType().FullName)+":"+Text(m.Data)+":"+m.DataList.Length);
        }
    }
    static void Reset() {
        typeof(MetricManager).GetField("m_Listeners",All).SetValue(null,new ArrayList());
        typeof(MetricManager).GetField("m_SessionOpen",All).SetValue(null,false);
        typeof(MetricManager).GetField("_DescriptorMap",All).SetValue(null,new Dictionary<string,MetricDescriptor>());
        MetricManager.Enabled=true;
    }
    static void Formatting() {
        object[] vals={null,0,1,-1,1L,1023L,1024L,1048576L,long.MaxValue,long.MinValue,1.25,0.0,-1.5,double.NaN,double.PositiveInfinity,1.25f,true,false,"1.5","bad",TimeSpan.FromSeconds(1.5)};
        foreach(string culture in new[]{"en-US","de-DE","tr-TR"}) {
            Thread.CurrentThread.CurrentCulture=new CultureInfo(culture);
            foreach(MetricType t in new[]{MetricType.Duration,MetricType.Size,MetricType.Count,MetricType.Ratio,MetricType.Name,MetricType.Enabled,MetricType.Success,(MetricType)99}) {
                var d=new MetricDescriptor("Metric.I",t,"Description");
                for(int i=0;i<vals.Length;i++) { object v=vals[i];
                    Case(culture+":format:"+t+":"+i,delegate{var m=Fixed(d,v,"extra",null,7); Check(Object.ReferenceEquals(m.Descriptor,d)); Check(m.Name==d.Name); return m.ToString();});
                }
                Case(culture+":empty:"+t,delegate{return Fixed(d,new object[0]).ToString();});
                Case(culture+":nullarray:"+t,delegate{return Fixed(d,(object[])null).ToString();});
            }
            Case(culture+":nullDescriptor",delegate{return Fixed(null,1).ToString();});
            foreach(MetricType t in new[]{MetricType.Duration,MetricType.Size,MetricType.Count,MetricType.Ratio,MetricType.Name,MetricType.Enabled,MetricType.Success,(MetricType)99}) {
                for(int i=0;i<vals.Length;i++) { object v=vals[i];
                    Case(culture+":submit:"+t+":"+i,delegate{
                        Reset();var listener=new Listener();MetricManager.AddListener(listener);
                        var d=MetricManager.GetDescriptor("Metric.I",t,"description");MetricManager.OpenSession();
                        object[] values={v,"extra"};MetricManager.Submit(d,values);MetricManager.CloseSession();
                        return String.Join("|",listener.Events.ToArray())+":mutated:"+Text(values[0]);
                    });
                }
            }
        }
        Thread.CurrentThread.CurrentCulture=CultureInfo.InvariantCulture;
    }
    static void Sessions() {
        Case("session",delegate{
            Reset();var l=new Listener();MetricManager.AddListener(l);
            var d=MetricManager.GetDescriptor("Metric",MetricType.Count,"first");
            Check(Object.ReferenceEquals(d,MetricManager.GetDescriptor("METRIC",MetricType.Name,"second")));
            MetricManager.Submit(d,1);MetricManager.Enabled=false;MetricManager.OpenSession();Check(MetricManager.Enabled);
            MetricManager.Submit("METRIC",2);MetricManager.Submit("missing",3);MetricManager.Submit(new MetricDescriptor("unregistered",MetricType.Count,""),4);
            MetricManager.Submit(d,new object[0]);MetricManager.Enabled=false;MetricManager.Submit(d,5);MetricManager.Enabled=true;
            MetricManager.Submit(new MetricDescriptor("metric",MetricType.Name,"other"),6);MetricManager.CloseSession();
            MetricManager.Submit(d,7);MetricManager.OpenSession();MetricManager.CloseSession();return String.Join("|",l.Events.ToArray());
        });
        Case("close-unopened",delegate{Reset();MetricManager.CloseSession();return "unexpected";});
        Case("double-open",delegate{Reset();MetricManager.OpenSession();MetricManager.OpenSession();return "unexpected";});
        Case("add-open",delegate{Reset();MetricManager.OpenSession();MetricManager.AddListener(new Listener());return "unexpected";});
        Case("null-listener",delegate{Reset();MetricManager.AddListener(null);MetricManager.OpenSession();return "unexpected";});
        Case("null-name",delegate{Reset();return Text(MetricManager.GetDescriptor(null,MetricType.Count,null));});
        Case("null-submit",delegate{Reset();MetricManager.Submit((MetricDescriptor)null,1);return "unexpected";});
        Case("null-samples",delegate{Reset();var d=MetricManager.GetDescriptor("x",MetricType.Count,"");MetricManager.OpenSession();MetricManager.Submit(d,(object[])null);return "unexpected";});
        Case("application",delegate{var a=new MetricApplicationData();Check(!String.IsNullOrEmpty(a.UserName));Check(a.MachineName==Environment.MachineName);Check(!String.IsNullOrEmpty(a.ApplicationName));a.UserName="u";a.MachineName="m";a.ApplicationName="a";return a.UserName+a.MachineName+a.ApplicationName;});
        for(int kind=0;kind<4;kind++) { int k=kind;Case("timer:"+k,delegate{
            Reset();var l=new Listener();l.Timer=true;MetricManager.AddListener(l);var d=MetricManager.GetDescriptor("timer",MetricType.Duration,"");MetricManager.OpenSession();
            MetricTimer timer=k==0?new MetricTimer(d):k==1?new MetricTimer(d,"data"):k==2?new MetricTimer(d.Name):new MetricTimer(d.Name,"");
            timer.Dispose();timer.Dispose();MetricManager.CloseSession();return String.Join("|",l.Events.ToArray());
        });}
    }
    static void Listeners() {
        var m=Fixed(new MetricDescriptor("x",MetricType.Count,"count"),123);
        Case("list-alias",delegate{var l=new ListMetric();l.AddMetric(m);l.AddMetric(null);var a=l.Metrics;Check(Object.ReferenceEquals(a,l.Metrics));a.Add(m);return Text(a.Count)+":"+Object.ReferenceEquals(a[0],m);});
        Case("console-and-report",delegate{
            TextWriter original=Console.Out;var output=new StringWriter();var supplied=new StringWriter();
            try {Console.SetOut(output);var l=new ConsoleMetricsListener(supplied);l.Open();l.SubmitMetrics(m);l.Close();l.Close();new ReportMetrics().AddMetric(m);return output.ToString()+":supplied:"+supplied.ToString();}
            finally{Console.SetOut(original);}
        });
        Case("console-default",delegate{TextWriter original=Console.Out;var w=new StringWriter();try{Console.SetOut(w);var l=new ConsoleMetricsListener();l.SubmitMetrics(m);l.Close();return w.ToString();}finally{Console.SetOut(original);}});
        Case("sql-missing-config",delegate{new SqlMetricsListener().Open();return "unexpected";});
        Case("sql-close-unopened",delegate{var l=new SqlMetricsListener();l.SubmitMetrics(m);l.Close();return "unexpected";});
        Case("sql-descriptor-population",delegate{
            var w=(SqlMetricsWriter)FormatterServices.GetUninitializedObject(typeof(SqlMetricsWriter));
            typeof(SqlMetricsWriter).GetField("m_MetricDescriptors",All).SetValue(w,new Hashtable());
            typeof(SqlMetricsWriter).GetMethod("PopulateMetricDescriptors",All).Invoke(w,new object[]{new ArrayList{m,m}});
            return Text(((Hashtable)typeof(SqlMetricsWriter).GetField("m_MetricDescriptors",All).GetValue(w)).Count);
        });
    }
    static int Main(string[] args) {
        Contract(typeof(Metric).Assembly);Rows.Add(new[]{"assembly",typeof(Metric).Assembly.FullName});
        Formatting();Sessions();Listeners();
        File.WriteAllText(args[0],new JavaScriptSerializer().Serialize(Rows));Console.WriteLine("PASS rows="+Rows.Count);return 0;
    }
}
