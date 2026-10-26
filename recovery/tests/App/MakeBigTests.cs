using System;
using System.IO;
using System.Linq;
using System.Text;
using NewModCompiler.Recovered.MakeBig;
namespace NewModCompiler.Tests
{
    public static class MakeBigTests
    {
        static int checks;
        static void Check(bool value,string message) {checks++;if(!value)throw new Exception("Recovered MakeBig: "+message);}
        public static int Run(string root)
        {
            checks=0;Directory.CreateDirectory(root);string input=Path.Combine(root,"input");Directory.CreateDirectory(input);
            var parsed=new ParseArguments();parsed.parse(new[]{"--force","-q","--verbose","-x:*.asset","--exclude:*skip?.bin","-o:\""+Path.Combine(root,"first.big")+"\"",input,"-o:"+Path.Combine(root,"second.big"),"--unknown"});
            Check(parsed.Force && parsed.Silent && parsed.Verbose && parsed.Filename==Path.Combine(root,"second.big"),"Original colon, long-option, last-value parser contract differs");
            Check(parsed.ExcludeList.Count==2 && parsed.ExcludeList[0].IsMatch(Path.Combine(input,"foo.asset").ToLower()) && !parsed.ExcludeList[0].IsMatch(Path.Combine(input,"foo.asset.bin").ToLower()),"Wildcard must anchor the entire absolute filename");
            var text=new StringWriter();Check(Program.Run(new[]{"--quiet","--help"},new StringReader(""),text)==0 && text.ToString().Contains("Create a big file"),"Help must precede quiet/missing-input handling");
            text=new StringWriter();Check(Program.Run(new[]{"-q"},new StringReader(""),text)==1 && text.ToString()=="","Quiet missing arguments contract differs");
            string child=Path.Combine(input,"Child");Directory.CreateDirectory(child);File.WriteAllText(Path.Combine(child,"MiXeD.TXT"),"child");File.WriteAllText(Path.Combine(input,"Parent.bin"),"parent");File.WriteAllText(Path.Combine(input,"skip.asset"),"exclude");
            string output=Path.Combine(root,"legacy.big");text=new StringWriter();
            Check(Program.Run(new[]{"-q","-x:*.asset","-o:"+output,input},new StringReader(""),text)==0,"Recovered directory build failed");
            byte[] bytes=File.ReadAllBytes(output);Check(Encoding.ASCII.GetString(bytes,0,4)=="BIG4" && BitConverter.ToUInt32(bytes,4)==bytes.Length,"Recovered archive magic or little-endian length differs");
            string raw=Encoding.UTF8.GetString(bytes);Check(raw.IndexOf("child\\mixed.txt\0",StringComparison.Ordinal)<raw.IndexOf("parent.bin\0",StringComparison.Ordinal) && !raw.Contains("skip.asset"),"Original child-first traversal/lowercase/exclusion differs");
            byte[] before=(byte[])bytes.Clone();Check(Program.Run(new[]{"-q","-o:"+output,input},new StringReader(""),new StringWriter())==1 && File.ReadAllBytes(output).SequenceEqual(before),"Quiet existing output must refuse without mutation");
            string force=Path.Combine(root,"forced.big");File.WriteAllText(force,"old");Check(Program.Run(new[]{"-f","-q","-x:*.asset","-o:"+force,input},new StringReader(""),new StringWriter())==0 && File.ReadAllBytes(force).SequenceEqual(before),"Force overwrite changed archive contents");
            string empty=Path.Combine(root,"empty");Directory.CreateDirectory(empty);string emptyBig=Path.Combine(root,"empty.big");Check(Program.Run(new[]{"-q","-o:"+emptyBig,empty},new StringReader(""),new StringWriter())==0 && new FileInfo(emptyBig).Length==24,"Original empty directory must emit the 24-byte BIG4 archive");
            string source=Path.Combine(input,"Parent.bin"),aliases=Path.Combine(root,"aliases.big");NewModCompiler.Application.MakeBig.Write(aliases,new[]{new NewModCompiler.Application.BigArchiveEntry{Name="Data/a.bin",SourcePath=source},new NewModCompiler.Application.BigArchiveEntry{Name="Data/b.bin",SourcePath=source}});
            Check(File.Exists(aliases),"Production exact-inventory adapter lost same-source aliases");
            using(var stream=new MemoryStream()) {
                var writer=new FixupBinaryWriter(stream);writer.WriteBookmark("little",typeof(uint));writer.EndianFlip=true;writer.WriteBookmark("big",typeof(uint));writer.ResolveBookmark("little",0x11223344u);writer.ResolveBookmark("big",0x11223344u);writer.WriteBookmarks();
                Check(stream.ToArray().SequenceEqual(new byte[]{0x44,0x33,0x22,0x11,0x11,0x22,0x33,0x44}),"Bookmark reservation must retain each field's original endian mode");
            }
            return checks;
        }
    }
}
