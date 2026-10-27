using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using NewModCompiler.Application;
using NewModCompiler.Cli;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Tests
{
    internal static class BigPackageTests
    {
        static int checks;
        static void Check(bool value,string message) {++checks;if(!value)throw new Exception("BIG package: "+message);}
        static void Put(string path,string value){Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,value);}
        sealed class Backend:IStreamCompiler
        {
            public bool Extra;
            public string ProviderIdentity {get{return "TEST_CURRENT_BIG_STREAMS";}}
            public void Initialize(BuildRequest request){}
            public void Dispose(){}
            public IList<Artifact> Compile(StreamContext context,CancellationToken token)
            {
                string stem=Path.GetFileNameWithoutExtension(context.Task.InputPath);
                string prefix=Path.Combine(context.StagingDirectory,"streams","mods","fixture","data",context.Task.Content,stem);
                var result=new List<Artifact>();
                foreach(string extension in new[]{".manifest",".bin",".imp",".relo"}) {
                    string path=prefix+extension;
                    if(context.Task.Content=="base") {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        byte[] bytes=new byte[extension==".manifest"?48:4];
                        if(extension==".manifest") {bytes[1]=1;bytes[2]=6;}
                        // The original stream checksum is copied to LoD payloads.
                        int checksum=extension==".manifest"?4:0;bytes[checksum]=17;bytes[checksum+1]=34;bytes[checksum+2]=51;bytes[checksum+3]=68;
                        File.WriteAllBytes(path,bytes);
                    } else Put(path,context.Task.Content+extension);
                    result.Add(new Artifact{Path=path,Kind="stream"+extension});
                }
                if(context.Task.Content=="apt") {
                    string path=Path.Combine(prefix,"cdata","00000000.11111111.22222222.33333333.cdata");Put(path,"custom");result.Add(new Artifact{Path=path,Kind="custom-data"});
                }
                // A stale unlisted file must never enter the archive.
                Put(Path.Combine(context.StagingDirectory,"stale.asset"),"unlisted old data");
                if(Extra) {string path=prefix+".asset";Put(path,"unexpected");result.Add(new Artifact{Path=path,Kind="unexpected"});}
                return result;
            }
        }
        sealed class Sink:IBuildEventSink
        {
            public string Mode;public BuildRequest Request;public CancellationTokenSource Cancellation;
            public void Write(IDictionary<string,object> row) {
                string type=(string)row["eventType"];
                if(Mode=="cancel" && type=="StageStarted" && (string)row["stageId"]=="package") Cancellation.Cancel();
                if(Mode=="report" && type=="StageCompleted" && (string)row["stageId"]=="package") Directory.CreateDirectory(Request.ReportPath);
                if(Mode=="terminal" && type=="BuildFinished")throw new IOException("terminal");
            }
            public void Dispose(){if(Mode=="close")throw new IOException("close");}
        }
        static ExitCode Run(BuildRequest request,string mode,CancellationTokenSource cancellation) {
            var sink=new Sink{Request=request,Mode=mode,Cancellation=cancellation};
            using(var events=new EventDispatcher(Guid.NewGuid().ToString("N"),new IBuildEventSink[]{sink}))
                return new BuildSession(request,new Backend{Extra=mode=="unexpected"},events).Run(cancellation.Token);
        }
        static uint BigEndian(BinaryReader reader){byte[] b=reader.ReadBytes(4);return ((uint)b[0]<<24)|((uint)b[1]<<16)|((uint)b[2]<<8)|b[3];}
        static string[] Names(string path) {
            using(var reader=new BinaryReader(File.OpenRead(path))) {
                Check(Encoding.ASCII.GetString(reader.ReadBytes(4))=="BIG4","Wrong archive header");reader.ReadUInt32();uint count=BigEndian(reader);BigEndian(reader);
                var result=new List<string>();
                for(uint i=0;i<count;i++){
                    uint offset=BigEndian(reader),size=BigEndian(reader);var name=new List<byte>();byte b;while((b=reader.ReadByte())!=0)name.Add(b);
                    string text=Encoding.ASCII.GetString(name.ToArray());result.Add(text);
                    if(text.StartsWith("data\\mod_",StringComparison.Ordinal)) {
                        long position=reader.BaseStream.Position;reader.BaseStream.Position=offset;
                        byte[] payload=reader.ReadBytes(checked((int)size));
                        if(text.EndsWith(".manifest",StringComparison.Ordinal)) {
                            Check(payload.Length==62 && payload[36]==14 && Encoding.ASCII.GetString(payload,48,14)=="\x02mod.manifest\0","Base LoD embedded dependency must preserve the original lowercase script argument");
                        } else Check(payload.SequenceEqual(new byte[]{17,34,51,68}),"Base LoD payload did not preserve the source checksum");
                        reader.BaseStream.Position=position;
                    }
                    if(text.EndsWith(".version",StringComparison.Ordinal)) {
                        long position=reader.BaseStream.Position;reader.BaseStream.Position=offset;
                        Check(size==3 && reader.ReadByte()==32 && reader.ReadByte()==13 && reader.ReadByte()==10,"APT version does not preserve the measured old echo-dot space/CRLF bytes");reader.BaseStream.Position=position;
                    }
                }
                return result.ToArray();
            }
        }
        public static int Run(string root,string sdk) {
            checks=0;string mod=Path.Combine(root,"package-mod");
            Put(Path.Combine(mod,"Data","AptUI","One.xml"),"<AssetDeclaration/>");Put(Path.Combine(mod,"Data","AdditionalMaps","MapMetaData_One.xml"),"<AssetDeclaration/>");
            foreach(string mode in new[]{"success","terminal","close","report","cancel","unexpected"}) {
                string output=Path.Combine(root,mode);var request=Options.Parse(new[]{"build","--mod",mod,"--sdk-root",sdk,"--out",output,"--package-content","apt,maps","--package-format","big"});
                string previous=Path.Combine(output,"published","previous.txt");Put(previous,"previous");
                using(var cancellation=new CancellationTokenSource()) {
                    ExitCode code=Run(request,mode,cancellation);
                    Check(code==(mode=="success"?ExitCode.Success:mode=="cancel"?ExitCode.Cancelled:ExitCode.Output),"Wrong packaging/finalization outcome for "+mode);
                }
                if(mode!="success") {
                    Check(File.ReadAllText(previous)=="previous","Failed BIG build replaced previous publication");
                    if(Directory.Exists(request.ReportPath))Directory.Delete(request.ReportPath);
                    using(var retry=new CancellationTokenSource())Check(Run(request,"success",retry)==ExitCode.Success,"Same-path BIG retry failed after "+mode);
                }
                string published=Path.Combine(output,"published"),big=Path.Combine(published,"package-mod_1.0.big"),sku=Path.Combine(published,"package-mod_1.0.skudef");
                Check(Directory.GetFiles(published,"*",SearchOption.AllDirectories).Length==2 && File.Exists(big) && File.Exists(sku),"BIG publication is not the exact two-file set");
                Check(!File.Exists(previous),"Successful BIG replacement retained old files");
                string[] names=Names(big);
                Check(names.Length==10 && names.Distinct(StringComparer.OrdinalIgnoreCase).Count()==10,"BIG duplicated/lost current artifacts or included stale files");
                foreach(string extension in new[]{"manifest","bin","imp","relo"}) {
                    Check(names.Contains("data\\aptui\\one."+extension),"APT game path incorrect");
                    Check(names.Contains("data\\additionalmaps\\mapmetadata_one."+extension),"Map game path incorrect");
                }
                Check(names.Contains("data\\aptui\\one\\cdata\\00000000.11111111.22222222.33333333.cdata"),"Stream CustomData game path incorrect");
                Check(names.Contains("data\\aptui\\one.version") && !names.Contains("data\\additionalmaps\\mapmetadata_one.version"),"APT-only version-file rule differs from the old SDK script");
                Check(File.ReadAllText(sku)=="mod-game 1.12\r\nadd-big package-mod_1.0.big\r\n","skudef does not reference the current BIG");
                var report=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(File.ReadAllText(request.ReportPath));
                Check(new JavaScriptSerializer().Serialize(report["artifacts"]).Contains("package.big") && ((System.Collections.ICollection)report["artifacts"]).Count==2,"Report did not switch to the final package allowlist");
            }
            Put(Path.Combine(mod,"Data","Mod.xml"),"<AssetDeclaration/>");
            foreach(string selection in new[]{"base","base,apt,maps"}) {
                var request=Options.Parse(new[]{"build","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(root,"lod-"+selection.Replace(',','-')),"--package-content",selection,"--package-format","big"});
                using(var cancellation=new CancellationTokenSource())Check(Run(request,"success",cancellation)==ExitCode.Success,"Base LoD packaging failed");
                string[] names=Names(Path.Combine(request.OutputRoot,"published","package-mod_1.0.big"));
                Check(names.Length==(selection=="base"?12:22),"Base LoD archive has an incomplete or stale member set");
                foreach(string suffix in new[]{"","_l","_m"})foreach(string extension in new[]{".manifest",".bin",".imp",".relo"})
                    Check(names.Contains("data\\mod"+suffix+extension),"Base/LoD member layout differs");
                Check(!names.Contains("data\\mod.version"),"Base acquired an unsupported APT-only version marker");
            }
            var versioned=Options.Parse(new[]{"build","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(root,"versioned"),"--package-content","apt,maps","--package-format","big","--mod-version","2.7-beta","--game-version","1.13","--skudef-name","Launcher"});
            using(var cancellation=new CancellationTokenSource())Check(Run(versioned,"success",cancellation)==ExitCode.Success,"Custom package identity failed");
            string vp=Path.Combine(versioned.OutputRoot,"published");
            Check(Directory.GetFiles(vp).Length==2 && File.Exists(Path.Combine(vp,"package-mod_2.7-beta.big")),"MOD version did not name the exact BIG");
            Check(File.ReadAllText(Path.Combine(vp,"Launcher_2.7-beta.skudef"))=="mod-game 1.13\r\nadd-big package-mod_2.7-beta.big\r\n","Custom SKUDEF name/game version/BIG reference differs");
            string project=Path.Combine(mod,"newmodcompiler.json");
            File.WriteAllText(project,"{\"package-format\":\"big\",\"mod-version\":\"3.0\",\"game-version\":\"1.12\",\"skudef-name\":\"PresetLauncher\"}");
            var preset=Options.Parse(new[]{"plan","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(root,"version-preset"),"--mod-version","4.0"});
            Check(preset.ModVersion=="4.0" && preset.Options["mod-version"].Source=="commandLine" && preset.SkudefName=="PresetLauncher" && preset.Options["skudef-name"].Source=="project","Identity preset/CLI precedence differs");
            File.Delete(project);
            foreach(var pair in new[]{new[]{"--mod-version","../escape"},new[]{"--game-version","1.12\r\nadd-big evil.big"},new[]{"--skudef-name","../escape"}}) {
                try {Options.Parse(new[]{"plan","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(root,"identity-invalid"),"--package-format","big"}.Concat(pair).ToArray());throw new Exception("Invalid identity accepted");}
                catch(BuildFailure failure){Check(failure.Code=="PACKAGE_IDENTITY_INVALID","Invalid identity diagnostic differs");}
            }
            try {Options.Parse(new[]{"plan","--mod",mod,"--sdk-root",sdk,"--out",Path.Combine(root,"identity-loose"),"--mod-version","2.0"});throw new Exception("Loose package identity silently ignored");}
            catch(BuildFailure failure){Check(failure.Code=="PACKAGE_IDENTITY_FORMAT","Loose identity diagnostic differs");}
            string unicode=Path.Combine(root,"中文MOD");Directory.CreateDirectory(unicode);
            try {Options.Parse(new[]{"build","--mod",unicode,"--sdk-root",sdk,"--out",Path.Combine(root,"unicode-big"),"--package-content","apt","--package-format","big"});throw new Exception("Unicode BIG filename accepted without game evidence");}
            catch(BuildFailure failure){Check(failure.Code=="PACKAGE_NAME_UNSUPPORTED","BIG ASCII filename boundary lost its explicit diagnostic");}
            string empty=Path.Combine(root,"empty-mod");Directory.CreateDirectory(empty);
            var optional=Options.Parse(new[]{"build","--mod",empty,"--sdk-root",sdk,"--out",Path.Combine(root,"empty-output"),"--package-content","apt,maps","--package-format","big"});
            string retained=Path.Combine(optional.OutputRoot,"published","previous.txt");Put(retained,"previous");
            using(var cancellation=new CancellationTokenSource())Check(Run(optional,"success",cancellation)==ExitCode.Success,"Empty optional BIG selection failed");
            Check(File.ReadAllText(retained)=="previous" && Directory.GetFiles(Path.GetDirectoryName(retained)).Length==1,"Empty BIG selection replaced prior publication with an empty package");
            return checks;
        }
    }
}
