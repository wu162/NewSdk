using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using NewModCompiler.Application;

// Executed by the independent production-object host, in both x86 configurations.
internal static class CompilerUnicodeSettings
{
    const string Ns = "uri:ea.com:eala:asset";
    static readonly List<string> Tests = new List<string>();
    static int checks;
    static string work;
    static void Require(bool value, string message) { ++checks; if (!value) throw new Exception(message); }
    static Settings SettingsForPaths()
    {
        return new Settings {
            DataRoot=work, SchemaPath=work, InputPath=Path.Combine(work,"root.xml"),
            OutputDirectory=Path.Combine(work,"out"), BuildCache=false, SessionCache=false,
            DefaultArtPaths=work, DefaultDataPaths=work, DefaultAudioPaths=work,
            TargetPlatform=TargetPlatform.Win32
        };
    }
    static void SetPaths(Settings settings, string kind, string value)
    {
        switch (kind)
        {
            case "ArtPaths": settings.DefaultArtPaths=value; break;
            case "DataPaths": settings.DefaultDataPaths=value; break;
            case "AudioPaths": settings.DefaultAudioPaths=value; break;
            case "MonitorPaths": settings.MonitorPaths=value; break;
            default: throw new Exception("Unknown test path kind");
        }
    }
    static void InvalidPaths()
    {
        foreach (string kind in new[] { "ArtPaths", "DataPaths", "AudioPaths", "MonitorPaths" })
        foreach (string input in new[] { null, "", ";", ";;", " ", " ; ", "one;\t;two", new string('a',300), "bad\0path" })
        {
            // null MonitorPaths is intentionally the absent optional setting.
            if (kind=="MonitorPaths" && input==null) continue;
            Settings settings=SettingsForPaths(); SetPaths(settings,kind,input); Settings.Current=settings;
            try { SettingsLoader.PostProcessSettings(work); }
            catch (BinaryAssetBuilderException e)
            {
                Require(e.ErrorCode==ErrorCode.InvalidArgument,"Wrong error code for "+kind);
                Require(e.Message.Contains(kind),"Configuration error omitted the setting name");
                Require(e.Message.Contains(input ?? "<null>"),"Configuration error omitted the original input");
                continue;
            }
            throw new Exception("Invalid search paths accepted: "+kind+" '"+input+"'");
        }
        Tests.Add("all-search-kinds-reject-null-empty-separator-whitespace-invalid-and-MAX_PATH-inputs");
        Settings overridden=SettingsForPaths();
        overridden.BuildConfigurationName="Override";
        overridden.BuildConfigurations=new[] { new BuildConfiguration { Name="Override", DataPaths=";" } };
        Settings.Current=overridden;
        try { SettingsLoader.PostProcessSettings(work); }
        catch (BinaryAssetBuilderException e)
        {
            Require(e.ErrorCode==ErrorCode.InvalidArgument && e.Message.Contains("DataPaths") && e.Message.Contains(";"),
                "Selected-configuration empty paths did not preserve a clear error");
            Tests.Add("selected-build-configuration-empty-search-set-rejected"); return;
        }
        throw new Exception("Empty selected-configuration search set accepted");
    }
    static void ValidPaths()
    {
        Settings settings=SettingsForPaths();
        settings.DefaultDataPaths=";"+Path.Combine(work,"资料 空格")+";relative/../子目录;;";
        settings.DefaultArtPaths="*;"+work;
        settings.MonitorPaths="monitor;*";
        Settings.Current=settings; SettingsLoader.PostProcessSettings(work);
        Require(settings.DataPaths.Length==2,"Valid semicolon path filtering changed");
        Require(settings.DataPaths[0]==Path.Combine(work,"资料 空格"),"Unicode search path corrupted");
        Require(settings.DataPaths[1]==Path.Combine(work,"子目录"),"Relative path canonicalization changed");
        Require(settings.ArtPaths.Length==2 && settings.ArtPaths[0]=="*" && settings.ArtPaths[1]==work,"Wildcard art search rule changed");
        Require(settings.AudioPaths.Length==1 && settings.AudioPaths[0]==work,"Default audio path changed");
        Require(settings.ProcessedMonitorPaths.Length==2 && settings.ProcessedMonitorPaths[0]==Path.Combine(work,"monitor") &&
            settings.ProcessedMonitorPaths[1]=="*","Monitor path processing changed");
        settings=SettingsForPaths(); Settings.Current=settings; SettingsLoader.PostProcessSettings(work);
        Require(settings.ProcessedMonitorPaths==null,"Absent optional monitor setting must remain optional");
        Tests.Add("valid-unicode-relative-wildcard-and-semicolon-search-paths-preserved");
    }
    static void XInclude(string name, string alias = null)
    {
        string root=Path.Combine(work,name); string includeDirectory=Path.Combine(root,"资源 目录");
        Directory.CreateDirectory(includeDirectory);
        string main=Path.Combine(root,"根 文件.xml"), included=Path.Combine(includeDirectory,"定义 文件.xml");
        string href=(alias==null ? "" : alias+":")+"资源 目录/定义 文件.xml";
        Settings.Current.DataPaths=new[] { root };
        Settings.Current.ArtPaths=new[] { root };
        Settings.Current.AudioPaths=new[] { root };
        File.WriteAllText(main,"<AssetDeclaration xmlns='"+Ns+"' xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='"+href+"'/></AssetDeclaration>");
        File.WriteAllText(included,"<Defines xmlns='"+Ns+"'><Define name='UNICODE_INCLUDE' value='42'/></Defines>");
        var resolver=new FileNameXmlResolver(root);
        using (var stream=(Stream)resolver.GetEntity(new Uri(alias==null ? included : href,UriKind.Absolute),null,typeof(Stream)))
        using (var reader=new StreamReader(stream))
            Require(reader.ReadToEnd().Contains("UNICODE_INCLUDE"),"Real resolver did not read the included file");
        Require(resolver.Paths.Count==1,"Resolver path map count incorrect");
        string fileRelative=".\\资源 目录\\定义 文件.xml";
        string relative=alias==null ? fileRelative : href;
        Require(resolver.Paths[0].SourceUri==relative,"Unicode relative SourceUri corrupted: "+resolver.Paths[0].SourceUri);
        Require(resolver.Paths[0].TargetPath==included,"Resolver target path corrupted");
        // No production caller currently uses this second shell wrapper. Exercise
        // its Unicode contract directly without claiming production-call coverage.
        Require(ShPath.RelativePathTo(root,true,included,false)==fileRelative,"Standalone shell relative path wrapper lost Unicode");
        Require(ShPath.RelativePathTo("C:\\one",true,"D:\\two.xml",false)==null,"Standalone shell cross-drive failure contract changed");

        // Bind the isolated loader state to its real per-stream processor. The
        // loader records actual XML parsing on this owner, just as production does.
        var processor=new DocumentProcessor(Settings.Current,null,null);
        Require(processor.XmlParseCount==0,"Fresh Unicode loader processor inherited an earlier parse count");
        var document=new AssetDeclarationDocument();
        Type stateType=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=Activator.CreateInstance(stateType,true);
        stateType.GetField("DocumentProcessor").SetValue(state,processor);
        stateType.GetField("SourcePath").SetValue(state,main);
        stateType.GetField("SourceDirectory").SetValue(state,root);
        stateType.GetField("LogicalSourcePath").SetValue(state,"根 文件.xml");
        FieldInfo dependencies=stateType.GetField("DependentFiles");
        dependencies.SetValue(state,Activator.CreateInstance(dependencies.FieldType));
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(document,state);
        document.LoadXml(true);
        Require(processor.XmlParseCount==1,"Real XML loader did not record exactly one parse on its fresh processor");
        XmlNode define=document.XmlDocument.SelectSingleNode("//*[local-name()='Define' and @name='UNICODE_INCLUDE']");
        Require(define!=null && define.Attributes["value"].Value=="42","Production XML loader did not expand XInclude");
        var names=new List<string>();
        foreach (object dependency in (IEnumerable)dependencies.GetValue(state)) names.Add(Convert.ToString(dependency));
        Require(names.Count==1 && names[0]==relative.ToLower(),"Production dependency path lost Unicode characters");
        string resolved=FileNameResolver.ResolvePath(root,names[0]);
        Require(String.Equals(Path.GetFullPath(resolved),included,StringComparison.OrdinalIgnoreCase) && File.Exists(resolved),
            "Recorded Unicode dependency cannot resolve back to its actual file");
        // LoadXml must close the real readers even when they contain an include.
        using (File.Open(main,FileMode.Open,FileAccess.ReadWrite,FileShare.None)) { }
        using (File.Open(included,FileMode.Open,FileAccess.ReadWrite,FileShare.None)) { }
        Tests.Add("real-file-XInclude-source-map-dependent-file-and-reader-release-"+name);
    }
    public static int Main(string[] args)
    {
        Settings original=Settings.Current; object result; int code=0;
        try
        {
            if (args.Length!=5 || IntPtr.Size!=4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            work=Path.GetFullPath(args[0]); Directory.CreateDirectory(work);
            Require(typeof(AssetDeclarationDocument).Assembly==typeof(BuiltinCompilers).Assembly,"Core is outside the production object assembly");
            InvalidPaths(); ValidPaths(); XInclude("ascii-root"); XInclude("中文 根");
            // Alias hrefs do not contain URI metacharacters. Their resolved local
            // search roots do: URI construction must preserve the literal name.
            foreach(string alias in new[] { "data", "art", "audio" })
                XInclude("别名 # %23 根-"+alias,alias);
            result=new { succeeded=true,sourceCommit=args[2],runId=args[3],checks=checks,tests=Tests.ToArray(),longPathsValidated=false,
                scope="Targeted production Core Unicode XInclude and search configuration paths; no full SDK or CLI acceptance" };
            Console.WriteLine("PASS UnicodeSettings: "+Tests.Count+" groups; "+checks+" checks");
        }
        catch (Exception e)
        {
            code=1; result=new { succeeded=false,sourceCommit=args.Length>2?args[2]:null,runId=args.Length>3?args[3]:null,
                checks=checks,tests=Tests.ToArray(),error=e.ToString() }; Console.Error.WriteLine(e);
        }
        finally { Settings.Current=original; }
        if(args.Length>1) File.WriteAllText(args[1],new JavaScriptSerializer().Serialize(result));
        return code;
    }
}
