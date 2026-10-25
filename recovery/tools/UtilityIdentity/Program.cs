using Mono.Cecil;
// C++ template names and public signatures involving internal native structs
// are valid CLI metadata but cannot be expressed directly by the C# compiler.
using var module = ModuleDefinition.ReadModule(args[0], new ReaderParameters { InMemory = true });
using var original = ModuleDefinition.ReadModule(args[1]);
foreach (var type in module.Types) {
    if (type.Name.StartsWith("StructWrapper_003CAssetStream_003A_003A")) {
        string name = type.Name.Replace("StructWrapper_003CAssetStream_003A_003A", "").Replace("_003E", "");
        var source = original.Types.Single(t => t.Name == "StructWrapper<AssetStream::" + name + ">");
        type.Name = source.Name;
    }
    var match = original.Types.FirstOrDefault(t => t.FullName == type.FullName);
    if (match == null) continue;
    type.Attributes = match.Attributes;
    foreach (var method in type.Methods.Where(m => m.Name == "Dispose" && m.Parameters.Count == 0)) {
        var source = match.Methods.Single(m => m.Name == "Dispose" && m.Parameters.Count == 0);
        method.Attributes = source.Attributes;
    }
}
module.Write(args[0]);
Console.WriteLine("Restored C++/CLI template type identities and native struct visibility");
