using System;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using NewModCompiler.Application;
using NewModCompiler.Cli;

namespace NewModCompiler.Tests
{
    internal static class BuildDefaultsTests
    {
        static int checks;
        static void Check(bool value,string message) { ++checks; if(!value) throw new Exception("Build defaults: "+message); }
        static void Put(string path,string text="<AssetDeclaration/>") { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path,text); }
        static void Reject(string code,Action action) { try { action(); throw new Exception("Accepted "+code); } catch(BuildFailure e) { Check(e.Code==code,"Expected "+code+", got "+e.Code); } }
        public static int Run(string root)
        {
            checks=0;
            string sdk=Path.Combine(root,"sdk"),mod=Path.Combine(root,"mod"),output=Path.Combine(root,"out");
            Directory.CreateDirectory(sdk);Directory.CreateDirectory(mod);
            Func<string[],BuildRequest> parse=extra=>Options.Parse(new[]{"plan","--mod",mod,"--sdk-root",sdk,"--out",output}.Concat(extra).ToArray());
            var standard=parse(new string[0]);
            Check(standard.PackageFormat=="big" && standard.IncludeAdditional && !standard.FixNeutralAssets,"Standard package defaults");
            Check(standard.AutoDetectContent && standard.GameVersion=="1.12" && standard.ModVersion=="1.0","Automatic content and identity defaults");
            foreach(string profile in new[]{"standard","development","advanced"}) {
                var request=parse(new[]{"--profile",profile});
                Check(request.PackageFormat==(profile=="development"?"loose":"big"),"Profile output format");
                Check(request.IncludeAdditional==(profile!="development"),"Additional follows format");
                foreach(string key in new[]{"sc","bc","psc","csc"}) Check(request.Options[key].Value=="true","Original SDK cache default "+key);
                foreach(string key in new[]{"fsc","atc"}) Check(request.Options[key].Value=="false","Original SDK cache default "+key);
                foreach(string key in new[]{"ss","pc","slowclean"}) Check(request.Options[key].Value=="true","Legacy script default "+key);
            }
            Check(!parse(new[]{"--package-format","loose"}).IncludeAdditional,"Explicit loose remains usable");
            Check(!parse(new[]{"--include-additional=false"}).IncludeAdditional,"Additional can be disabled");
            Check(parse(new[]{"--profile","development","--package-format","big"}).IncludeAdditional,"Explicit format overrides profile");
            Reject("CLI_PROFILE",()=>parse(new[]{"--profile","unknown"}));
            Reject("PLAN_NO_INPUTS",()=>Planner.Resolve(standard));
            Put(Path.Combine(mod,"Additional","only.ini"),"additional only");
            Reject("PLAN_NO_INPUTS",()=>Planner.Resolve(standard));
            Put(Path.Combine(mod,"Data","AptUI","One.xml"));
            var apt=Planner.Resolve(standard);Check(apt.Tasks.Length==1 && apt.Tasks[0].Content=="apt" && apt.Tasks[0].Reason=="AutomaticContentDiscovery","APT-only MOD without base");
            Put(Path.Combine(mod,"Data","AdditionalMaps","MapMetaData_One.xml"));
            File.Delete(Path.Combine(mod,"Data","AptUI","One.xml"));
            Check(Planner.Resolve(standard).Tasks.Single().Content=="maps","Metadata-only MOD without base");
            Put(Path.Combine(mod,"Data","Mod.xml"));Put(Path.Combine(mod,"Data","AptUI","One.xml"));
            Check(Planner.Resolve(standard).Tasks.Length==3,"Automatic mixed MOD discovery");
            var current=parse(new[]{"--package-content","global-metadata"});var legacy=parse(new[]{"--package-content","maps"});
            Check(current.Content.SequenceEqual(legacy.Content) && Planner.Resolve(current).Tasks.Single().Id=="maps/MapMetaData_One","Metadata alias preserves task IDs and paths");
            Check(parse(new[]{"--package-content","maps,global-metadata"}).Content.Length==1,"Metadata alias deduplication");
            Check(Planner.Resolve(parse(new[]{"--package-content","apt"})).Tasks.Length==1,"Explicit selection stays partial");
            File.Delete(Path.Combine(mod,"Data","Mod.xml"));
            Reject("PLAN_BASE_MISSING",()=>Planner.Resolve(parse(new[]{"--package-content","base"})));
            Reject("PACKAGE_NEUTRAL_CONTENT",()=>Planner.Resolve(parse(new[]{"--fix-neutral-assets=true"})));
            File.Delete(Path.Combine(mod,"Data","AptUI","One.xml"));
            Check(Planner.Resolve(parse(new[]{"--package-content","apt"})).Tasks.Length==0,"Explicit empty optional selection preserves old behavior");
            var preset=new Dictionary<string,object>{{"sdk-root","../sdk"},{"out","../project-out"},{"profile","development"},{"package-content","global-metadata"},{"pc",false}};
            string project=Path.Combine(mod,"newmodcompiler.json");File.WriteAllText(project,new JavaScriptSerializer().Serialize(preset));
            var saved=Options.Parse(new[]{"plan","--mod",mod});
            Check(saved.SdkRoot==sdk && saved.OutputRoot==Path.Combine(root,"project-out") && saved.PackageFormat=="loose","MOD-only invocation uses project paths relative to MOD");
            Check(saved.Options["pc"].Value=="false" && saved.Options["pc"].Source=="project","Advanced compatibility setting in project");
            var overridden=Options.Parse(new[]{"plan","--mod",mod,"--profile","standard","--out",output,"/pc:true"});
            Check(overridden.PackageFormat=="big" && overridden.OutputRoot==output && overridden.Options["pc"].Source=="commandLine","CLI overrides project");
            File.WriteAllText(project,"{\"mod-version\":2.7,\"game-version\":1.12}");
            Check(parse(new string[0]).ModVersion=="2.7", "Existing numeric project versions remain accepted");
            var development=parse(new[]{"--profile","development","--mod-version","3.1"});
            Check(development.PackageFormat=="loose" && development.ModVersion=="3.1" && development.GameVersion=="1.12", "Development can reuse project package identity and CLI version overrides");
            Reject("PACKAGE_IDENTITY_FORMAT",()=>parse(new[]{"--package-format","loose","--mod-version","3.1"}));
            File.WriteAllText(project,"null");Reject("PROJECT_INVALID",()=>parse(new string[0]));
            File.WriteAllText(project,"{\"out\":null}");Reject("PROJECT_OPTION",()=>parse(new string[0]));
            File.Delete(project);
            Check(Options.Parse(new[]{"doctor","--sdk-root",sdk}).Command=="doctor","Doctor has no MOD/package input");
            return checks;
        }
    }
}
