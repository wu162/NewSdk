using System;
using System.IO;
using Mono.Cecil;

// C# cannot spell the original EXSLT hyphenated CLR names. Rename definitions,
// not method bodies: Cecil updates all references while preserving public keys.
static class Program {
    static int CheckCore(string path) {
        using(var a=AssemblyDefinition.ReadAssembly(path)) {
            bool reference=false,ctor=false,resolver=false;
            foreach(var r in a.MainModule.AssemblyReferences)
                if(r.Name=="Mvp.Xml") {if(r.PublicKeyToken.Length!=0) throw new Exception("Unexpected signed Mvp.Xml reference");reference=true;}
            foreach(var t in a.MainModule.Types) foreach(var m in t.Methods) {
                if(!m.HasBody) continue;
                bool loader=t.FullName=="BinaryAssetBuilder.Core.AssetDeclarationDocument" && m.Name=="LoadXml";
                bool processor=t.FullName=="BinaryAssetBuilder.Core.DocumentProcessor" && m.Name=="ProcessDocument";
                foreach(var i in m.Body.Instructions) {
                    var call=i.Operand as MethodReference;if(call==null) continue;
                    if((loader||processor) && call.DeclaringType.FullName=="BinaryAssetBuilder.Core.XIncludingReaderWrapper") throw new Exception("Production path still uses optional wrapper");
                    if(loader && call.DeclaringType.FullName=="Mvp.Xml.XInclude.XIncludingReader") {
                        if(call.Name==".ctor") ctor=true;
                        if(call.Name=="set_XmlResolver") resolver=true;
                    }
                }
            }
            if(!reference||!ctor||!resolver) throw new Exception("Missing compile-time Mvp.Xml constructor/resolver calls");
            Console.WriteLine("PASS Core.LoadXml directly calls XIncludingReader constructor and XmlResolver; no optional wrapper on production path");
        }
        return 0;
    }
    static int Main(string[] args) {
        if(args[0]=="--check-core") return CheckCore(args[1]);
        string path=Path.GetFullPath(args[0]);
        bool symbols=File.Exists(Path.ChangeExtension(path,".pdb"));
        int count=0;
        using(var assembly=AssemblyDefinition.ReadAssembly(path,new ReaderParameters{InMemory=true,ReadSymbols=symbols})) {
            foreach(var type in assembly.MainModule.Types)
                foreach(var method in type.Methods)
                    if(method.Name.Contains("_002D")) {method.Name=method.Name.Replace("_002D","-");count++;}
            int aliases=0;
            foreach(var type in assembly.MainModule.Types)
                foreach(var method in type.Methods) if(method.Name.Contains("-")) aliases++;
            if(aliases!=51) throw new InvalidOperationException("Expected all 51 original EXSLT aliases, found "+aliases);
            if(count!=0) assembly.Write(path,new WriterParameters{WriteSymbols=symbols});
        }
        Console.WriteLine("Restored "+count+" EXSLT CLR method names");return 0;
    }
}
