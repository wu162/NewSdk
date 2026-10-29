using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    // SDK/defaultscript.cs step 8: SDK/additional, then MOD/additional,
    // recursive File.Copy(..., true) into the built MOD root. Step 9's
    // MakeBig -x:*.asset excludes asset files from the final archive.
    public static class AdditionalFiles
    {
        sealed class Input { public string Name, Path; }
        public static void Merge(string sdkRoot,string modRoot,string staging,IList<BigArchiveEntry> entries,CancellationToken cancellation,Action<string> warning = null)
        {
            if(entries==null) throw new ArgumentNullException("entries");
            cancellation.ThrowIfCancellationRequested();
            var files=new Dictionary<string,Input>(StringComparer.OrdinalIgnoreCase);
            var directories=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach(string root in new[]{sdkRoot,modRoot}) {
                string additional=Path.Combine(Path.GetFullPath(root),"Additional");
                if(Directory.Exists(additional)) using(DiagnosticDirectory.Acquire(additional,false)) Discover(additional,"",files,directories,cancellation,warning);
                else if(File.Exists(additional)) throw new IOException("Additional input must be a directory.");
            }
            if(files.Count==0 && directories.Count==0) return;
            var names=new HashSet<string>(entries.Select(entry=>Canonical(entry.Name)),StringComparer.OrdinalIgnoreCase);
            foreach(var file in files.Values) names.Add(file.Name);
            // CopyFolders cannot copy a directory over a file, or a file over
            // a directory. Include empty source directories in this check.
            foreach(string name in names) {
                if(directories.Contains(name)) throw new IOException("Additional file/directory collision: "+name);
                for(int split=name.LastIndexOf('\\');split>=0;split=name.LastIndexOf('\\',split-1)) {
                    if(names.Contains(name.Substring(0,split))) throw new IOException("Additional file/directory collision: "+name);
                    if(split==0) break;
                }
            }
            string snapshots=Path.Combine(Path.GetFullPath(staging),".package-additional");
            if(File.Exists(snapshots) || Directory.Exists(snapshots)) throw new IOException("Additional staging directory already exists.");
            var replacements=new Dictionary<string,BigArchiveEntry>(StringComparer.OrdinalIgnoreCase);
            var buffer=new byte[64*1024];
            int index=0;
            foreach(var file in files.Values.OrderBy(value=>value.Name,StringComparer.Ordinal)) {
                cancellation.ThrowIfCancellationRequested();
                if(string.Equals(Path.GetExtension(file.Name),".asset",StringComparison.OrdinalIgnoreCase)) continue;
                if(index==0)Directory.CreateDirectory(snapshots);
                string target=Path.Combine(snapshots,(index++).ToString(System.Globalization.CultureInfo.InvariantCulture));
                using(var source=new FileStream(file.Path,FileMode.Open,FileAccess.Read,FileShare.Read))
                using(var destination=new FileStream(target,FileMode.CreateNew,FileAccess.Write,FileShare.None)) {
                    int read;
                    while((read=source.Read(buffer,0,buffer.Length))!=0) {cancellation.ThrowIfCancellationRequested();destination.Write(buffer,0,read);}
                }
                replacements.Add(file.Name,new BigArchiveEntry{Name=file.Name,SourcePath=target});
            }
            cancellation.ThrowIfCancellationRequested();
            // Switch only after the entire current input set has been copied
            // and checked; no source directory or stale output is reused.
            for(int i=0;i<entries.Count;i++) {
                BigArchiveEntry replacement;
                string name=Canonical(entries[i].Name);
                if(replacements.TryGetValue(name,out replacement)) {if(warning!=null) warning("Additional overwrites compiled package member: "+name);entries[i]=replacement;replacements.Remove(name);}
            }
            foreach(var item in replacements.OrderBy(pair=>pair.Key,StringComparer.Ordinal)) entries.Add(item.Value);
        }
        static void Discover(string directory,string relative,IDictionary<string,Input> files,ISet<string> directories,CancellationToken cancellation,Action<string> warning)
        {
            cancellation.ThrowIfCancellationRequested();
            if(relative.Length!=0) {
                if((File.GetAttributes(directory)&FileAttributes.ReparsePoint)!=0)throw new IOException("Additional input directory cannot be a reparse point.");
                directories.Add(Canonical(relative));
            }
            foreach(string file in Directory.GetFiles(directory).OrderBy(value=>value,StringComparer.Ordinal)) {
                cancellation.ThrowIfCancellationRequested();
                if((File.GetAttributes(file)&FileAttributes.ReparsePoint)!=0) throw new IOException("Additional input cannot be a reparse point.");
                string name=Canonical(relative+Path.GetFileName(file));
                Input old;
                if(warning!=null && files.TryGetValue(name,out old) && !string.Equals(Path.GetExtension(name),".asset",StringComparison.OrdinalIgnoreCase))
                    warning("Additional overwrite: " + name + " (" + old.Path + " -> " + file + ")");
                files[name]=new Input{Name=name,Path=file};
            }
            foreach(string child in Directory.GetDirectories(directory).OrderBy(value=>value,StringComparer.Ordinal))
                Discover(child,relative+Path.GetFileName(child)+"\\",files,directories,cancellation,warning);
        }
        static string Canonical(string name) { return name.Replace('/','\\').TrimEnd('\\').ToLowerInvariant(); }
    }
}
