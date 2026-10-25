using System;
using System.IO;
using Mono.Cecil;

// C# cannot spell the original EXSLT hyphenated CLR names. Rename definitions,
// not method bodies: Cecil updates all references while preserving public keys.
static class Program {
    static int Main(string[] args) {
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
