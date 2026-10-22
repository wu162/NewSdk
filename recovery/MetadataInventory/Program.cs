using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;

// Read metadata without loading the x86 CLR2 mixed-mode image into a CLR8 process.
if (args.Length != 2) throw new ArgumentException("Usage: MetadataInventory input.dll output-directory");
Directory.CreateDirectory(args[1]);
byte[] image = File.ReadAllBytes(args[0]);
using var stream = new MemoryStream(image);
using var pe = new PEReader(stream);
var md = pe.GetMetadataReader();
string TypeName(TypeDefinitionHandle h) {
    var t = md.GetTypeDefinition(h);
    string ns = md.GetString(t.Namespace), name = md.GetString(t.Name);
    return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
}
var methods = new List<object>();
int il = 0, native = 0, other = 0;
foreach (var th in md.TypeDefinitions) {
    var td = md.GetTypeDefinition(th);
    foreach (var mh in td.GetMethods()) {
        var m = md.GetMethodDefinition(mh);
        var codeType = m.ImplAttributes & MethodImplAttributes.CodeTypeMask;
        string kind = codeType.ToString();
        if (codeType == MethodImplAttributes.IL) il++;
        else if (codeType == MethodImplAttributes.Native) native++;
        else other++;
        string? body = null, error = null;
        int? maxStack = null;
        if (m.RelativeVirtualAddress != 0 && codeType == MethodImplAttributes.IL) {
            try {
                var block = pe.GetMethodBody(m.RelativeVirtualAddress);
                body = Convert.ToHexString(block.GetILBytes() ?? Array.Empty<byte>());
                maxStack = block.MaxStack;
            } catch (Exception ex) { error = ex.Message; }
        }
        methods.Add(new {
            token = $"0x{MetadataTokens.GetToken(mh):X8}", type = TypeName(th),
            name = md.GetString(m.Name), rva = $"0x{m.RelativeVirtualAddress:X8}",
            kind, attributes = m.Attributes.ToString(), impl = m.ImplAttributes.ToString(),
            signature = Convert.ToHexString(md.GetBlobBytes(m.Signature)),
            il = body, maxStack, error
        });
    }
}
var json = new JsonSerializerOptions { WriteIndented = true };
File.WriteAllText(Path.Combine(args[1], "methods.json"), JsonSerializer.Serialize(methods, json));
var types = md.TypeDefinitions.Select(h => {
    var t = md.GetTypeDefinition(h); var layout = t.GetLayout();
    return new { token = $"0x{MetadataTokens.GetToken(h):X8}", name = TypeName(h),
        attributes = t.Attributes.ToString(), size = layout.Size, packing = layout.PackingSize,
        fields = t.GetFields().Select(fh => { var f = md.GetFieldDefinition(fh); return new {
            name = md.GetString(f.Name), attributes = f.Attributes.ToString(),
            offset = f.GetOffset(), signature = Convert.ToHexString(md.GetBlobBytes(f.Signature))
        }; }).ToArray() };
}).ToArray();
File.WriteAllText(Path.Combine(args[1], "types.json"), JsonSerializer.Serialize(types, json));
var a = md.GetAssemblyDefinition();
var summary = new {
    file = Path.GetFileName(args[0]), sha256 = Convert.ToHexString(SHA256.HashData(image)),
    assembly = md.GetString(a.Name), version = a.Version.ToString(),
    runtime = md.MetadataVersion, machine = pe.PEHeaders.CoffHeader.Machine.ToString(),
    imageBase = $"0x{pe.PEHeaders.PEHeader!.ImageBase:X}",
    corFlags = pe.PEHeaders.CorHeader!.Flags.ToString(),
    types = types.Length, methods = methods.Count, il, native, other,
    references = md.AssemblyReferences.Select(h => { var r = md.GetAssemblyReference(h);
        return new { name = md.GetString(r.Name), version = r.Version.ToString() }; }).ToArray()
};
File.WriteAllText(Path.Combine(args[1], "summary.json"), JsonSerializer.Serialize(summary, json));
Console.WriteLine(JsonSerializer.Serialize(summary, json));
