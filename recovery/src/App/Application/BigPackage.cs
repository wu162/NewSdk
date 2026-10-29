using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Script.Serialization;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    // Package identity is derived from current accepted streams and Additional.
    // A matching receipt references existing public BIG/SKUDEF files directly.
    public static class BigPackage
    {
        public static void ValidateRequest(BuildRequest request)
        {
            if(request.FixNeutralAssets && (request.PackageFormat!="big" || request.Content==null || !request.Content.Contains("base"))) throw new BuildFailure(ExitCode.Arguments,"PACKAGE_NEUTRAL_CONTENT","Neutral-asset repair requires BIG packaging with base content selected.");
            if(request.IncludeAdditional && request.PackageFormat!="big") throw new BuildFailure(ExitCode.Arguments,"PACKAGE_ADDITIONAL_FORMAT","Additional files require --package-format big.");
            PackageIdentity.Validate(request);
            if(request.Command=="doctor") return;
            if(request.PackageFormat==null || request.PackageFormat=="loose") return;
            if(request.PackageFormat!="big") throw new BuildFailure(ExitCode.Arguments,"CLI_CAPABILITY","Package format must be loose or big.");
            string name=Path.GetFileName(Path.GetFullPath(request.ModRoot).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar));
            if(string.IsNullOrWhiteSpace(name) || name.Any(value=>value<32 || value>126 || value=='"' || value=='\r' || value=='\n'))
                throw new BuildFailure(ExitCode.Arguments,"PACKAGE_NAME_UNSUPPORTED","BIG packaging requires an ASCII MOD directory basename for the game skudef.");
        }
        public static IList<Artifact> Create(BuildRequest request,BuildPlan plan,string staging,IList<Artifact> artifacts,CancellationToken cancellation,Action<string> warning = null)
        {
            ValidateRequest(request);
            if(request.PackageFormat!="big" || plan==null || artifacts==null || artifacts.Count==0) throw new ArgumentException("BIG packaging requires a nonempty current build artifact list.");
            staging=Path.GetFullPath(staging);
            var entries=new List<BigArchiveEntry>();var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var owners=new Dictionary<BuildTask,HashSet<string>>();
            foreach(var task in plan.Tasks) {
                if(task.Content!="base" && task.Content!="apt" && task.Content!="maps") throw new BuildFailure(ExitCode.Arguments,"PACKAGE_CONTENT_UNSUPPORTED","BIG packaging requires base/apt/maps streams.");
                owners.Add(task,new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            }
            foreach(var artifact in artifacts) {
                cancellation.ThrowIfCancellationRequested();Verify(artifact,staging);
                BuildTask owner=null;
                foreach(var task in plan.Tasks) {
                    string root=Path.Combine(staging,task.Content,Path.GetFileNameWithoutExtension(task.InputPath));
                    if(!Paths.Contains(root,artifact.Path)) continue;
                    if(owner!=null) throw new IOException("BIG artifact has ambiguous task ownership.");owner=task;
                }
                if(owner==null) throw new IOException("BIG input is outside this build's stream tasks.");
                string stem=Path.GetFileNameWithoutExtension(owner.InputPath), leaf=Path.GetFileName(artifact.Path), relative;
                string extension=Path.GetExtension(artifact.Path);
                if(new[] {".manifest",".bin",".imp",".relo"}.Contains(extension,StringComparer.OrdinalIgnoreCase)) {
                    if(!string.Equals(leaf,stem+extension,StringComparison.OrdinalIgnoreCase) || !owners[owner].Add(extension)) throw new IOException("BIG stream files must be one exact four-file set per selected task.");
                    relative=stem+extension;
                } else {
                    var directory=new DirectoryInfo(Path.GetDirectoryName(artifact.Path));
                    if(artifact.Kind!="custom-data" || !string.Equals(extension,".cdata",StringComparison.OrdinalIgnoreCase) || !string.Equals(directory.Name,"cdata",StringComparison.OrdinalIgnoreCase) || directory.Parent==null || !string.Equals(directory.Parent.Name,stem,StringComparison.OrdinalIgnoreCase)) throw new IOException("Unexpected BIG custom-data artifact layout.");
                    relative=stem+"\\cdata\\"+leaf;
                }
                string name="Data\\"+(owner.Content=="base" ? "" : (owner.Content=="apt" ? "AptUI" : "AdditionalMaps")+"\\")+relative;
                if(!names.Add(name)) throw new IOException("Case-insensitive BIG archive path collision.");
                entries.Add(new BigArchiveEntry {Name=name,SourcePath=artifact.ContentPath});
            }
            foreach(var pair in owners) if(pair.Value.Count!=4) throw new IOException("BIG packaging requires all four linked files for every selected stream.");
            IList<BigArchiveEntry> additional=request.IncludeAdditional ? AdditionalFiles.Inputs(request.SdkRoot,request.ModRoot,cancellation,warning) : new BigArchiveEntry[0];
            string fingerprint=request.FileIdentities!=null && !request.FixNeutralAssets ? Fingerprint(request,plan,staging,artifacts,additional) : null;
            Artifact[] reused;
            if(fingerprint!=null && Reuse(request,fingerprint,out reused)) {
                if(request.IncludeAdditional) {
                    var planned=new List<string>(entries.Select(entry=>entry.Name));
                    if(plan.Tasks.Any(task=>task.Content=="base")) foreach(string suffix in new[]{"_l","_m"}) foreach(string ext in new[]{".manifest",".bin",".imp",".relo"})planned.Add("Data\\mod"+suffix+ext);
                    foreach(var task in plan.Tasks)if(task.Content=="apt")planned.Add("Data\\AptUI\\"+Path.GetFileNameWithoutExtension(task.InputPath)+".version");
                    AdditionalFiles.Validate(additional,planned,warning);
                }
                return reused;
            }
            // SDK/defaultscript.cs step 5 runs LoD directly when optional asset
            // repair are disabled. Neutral repair is explicit and modifies only
            // a private package copy, never Core/cache outputs.
            foreach(var task in plan.Tasks) if(task.Content=="base") {
                cancellation.ThrowIfCancellationRequested();
                string manifest=artifacts.Single(value=>string.Equals(Path.GetExtension(value.Path),".manifest",StringComparison.OrdinalIgnoreCase) && Paths.Contains(Path.Combine(staging,task.Content,Path.GetFileNameWithoutExtension(task.InputPath)),value.Path)).ContentPath;
                if(request.FixNeutralAssets) {
                    string originalRoot=Path.GetDirectoryName(manifest);
                    manifest=BasePostProcessor.Prepare(manifest,request.SdkRoot,Path.Combine(staging,".package-neutral"),cancellation);
                    string preparedStem=Path.Combine(Path.GetDirectoryName(manifest),"mod");
                    foreach(var entry in entries) if(Paths.Contains(originalRoot,entry.SourcePath)) {
                        string extension=Path.GetExtension(entry.SourcePath);
                        entry.SourcePath=string.Equals(extension,".cdata",StringComparison.OrdinalIgnoreCase)
                            ? Path.Combine(preparedStem,"cdata",Path.GetFileName(entry.SourcePath)) : preparedStem+extension.ToLowerInvariant();
                    }
                }
                // Win32 resolves the existing Mod.manifest case-insensitively; the
                // original script passes lowercase mod.manifest, which is also
                // the exact embedded dependency name in both LoD manifests.
                foreach(string source in LoDStreamBuilder.Create(Path.Combine(Path.GetDirectoryName(manifest),"mod.manifest"),cancellation,Path.Combine(staging,".package-lod"))) {
                    string name="Data\\"+Path.GetFileName(source);
                    if(!names.Add(name)) throw new IOException("Case-insensitive LoD archive path collision.");
                    entries.Add(new BigArchiveEntry {Name=name,SourcePath=source});
                }
            }
            // SDK/defaultscript.cs uses echo-dot plus space redirection for each
            // actual APT stream: measured Windows bytes are space + CRLF.
            // MapMetaData has no such rule. Derive these from current tasks only.
            int versionIndex=0;
            foreach(var task in plan.Tasks) if(task.Content=="apt") {
                cancellation.ThrowIfCancellationRequested();
                string directory=Path.Combine(staging,".package-versions");Directory.CreateDirectory(directory);
                string source=Path.Combine(directory,(versionIndex++).ToString(System.Globalization.CultureInfo.InvariantCulture)+".version");
                using(var file=new FileStream(source,FileMode.CreateNew,FileAccess.Write,FileShare.None)) {file.WriteByte(32);file.WriteByte(13);file.WriteByte(10);}
                string name="Data\\AptUI\\"+Path.GetFileNameWithoutExtension(task.InputPath)+".version";
                if(!names.Add(name)) throw new IOException("Case-insensitive APT version archive path collision.");
                entries.Add(new BigArchiveEntry {Name=name,SourcePath=source});
            }
            string mod=Path.GetFileName(request.ModRoot.TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar));
            string big=Path.Combine(staging,mod+"_"+request.ModVersion+".big"),skudef=Path.Combine(staging,(request.SkudefName ?? mod)+"_"+request.ModVersion+".skudef");
            if(request.IncludeAdditional) AdditionalFiles.Merge(staging,entries,additional,cancellation,warning);
            MakeBig.Write(big,entries,cancellation);
            BigArchiveWriter.WriteSkudef(skudef,Path.GetFileName(big),cancellation,request.GameVersion);
            cancellation.ThrowIfCancellationRequested();
            var result=new[] {Describe(big,"package.big",request.FileIdentities),Describe(skudef,"package.skudef",request.FileIdentities)};
            if(fingerprint!=null)request.FileIdentities.Remember("package|v1",new JavaScriptSerializer().Serialize(new PackageReceipt {Fingerprint=fingerprint,Artifacts=result}));
            return result;
        }
        sealed class PackageReceipt {public PackageReceipt() {} public string Fingerprint {get;set;} public Artifact[] Artifacts {get;set;} }
        static string Fingerprint(BuildRequest request,BuildPlan plan,string staging,IList<Artifact> artifacts,IList<BigArchiveEntry> additional)
        {
            using(var bytes=new MemoryStream())using(var writer=new BinaryWriter(bytes)) {
                writer.Write(typeof(BigPackage).Assembly.ManifestModule.ModuleVersionId.ToString());
                writer.Write(Path.GetFileName(request.ModRoot.TrimEnd('\\','/')));writer.Write(request.ModVersion);writer.Write(request.GameVersion);writer.Write(request.SkudefName??"");writer.Write(request.IncludeAdditional);
                writer.Write(plan.Tasks.Length);foreach(var task in plan.Tasks) {writer.Write(task.Content);writer.Write(Path.GetFileNameWithoutExtension(task.InputPath));}
                writer.Write(artifacts.Count);foreach(var artifact in artifacts.OrderBy(value=>value.Path,StringComparer.Ordinal)) {writer.Write(artifact.Path.Substring(staging.Length+1));writer.Write(artifact.Kind);writer.Write(artifact.Sha256);}
                writer.Write(additional.Count);foreach(var entry in additional) {writer.Write(entry.Name);writer.Write(request.FileIdentities.Hash(entry.SourcePath));}
                writer.Flush();bytes.Position=0;
                using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","");
            }
        }
        static bool Reuse(BuildRequest request,string fingerprint,out Artifact[] artifacts)
        {
            artifacts=null;string receipt;
            if(!request.FileIdentities.TryReceipt("package|v1",out receipt))return false;
            var previous=new JavaScriptSerializer().Deserialize<PackageReceipt>(receipt);
            if(previous.Fingerprint!=fingerprint)return false;
            var result=new List<Artifact>();
            foreach(var artifact in previous.Artifacts) {
                string path=Path.Combine(request.OutputRoot,"published",Path.GetFileName(artifact.Path));
                if(!File.Exists(path) || request.FileIdentities.Hash(path)!=artifact.Sha256)return false;
                result.Add(new Artifact {Path=path,Kind=artifact.Kind,Size=artifact.Size,Sha256=artifact.Sha256,Verified=true,Published=true});
            }
            artifacts=result.ToArray();return true;
        }
        static void Verify(Artifact artifact,string staging)
        {
            if(artifact==null || string.IsNullOrEmpty(artifact.Path) || !Path.IsPathRooted(artifact.Path) || !Paths.Contains(staging,artifact.Path) || (!artifact.Verified && !File.Exists(artifact.Path))) throw new IOException("BIG input must be an accepted current staging file.");
            if(artifact.Verified)return;
            using(DiagnosticDirectory.Acquire(Path.GetDirectoryName(artifact.Path),false)) {
                if((File.GetAttributes(artifact.Path)&(FileAttributes.Directory|FileAttributes.ReparsePoint))!=0 || new FileInfo(artifact.Path).Length!=artifact.Size || Hash(artifact.Path)!=artifact.Sha256) throw new IOException("BIG input changed after compilation.");
            }
        }
        static Artifact Describe(string path,string kind,FileHashCache hashes) { return new Artifact {Path=path,Kind=kind,Size=new FileInfo(path).Length,Sha256=hashes==null ? Hash(path) : hashes.Hash(path,true),Verified=true}; }
        static string Hash(string path) { using(var hash=SHA256.Create()) using(var stream=File.OpenRead(path)) return BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant(); }
    }
}
