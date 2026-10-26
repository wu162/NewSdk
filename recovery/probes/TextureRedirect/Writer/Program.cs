using System.Security.Cryptography;
using System.Text.Json;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

// Build-time only. Never use ModuleDef.Write for the mixed-mode input.
internal static class Program
{
    const string OriginalHash = "4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e";
    static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 4 || (args[0] != "roundtrip" && args[0] != "redirect"))
                throw new ArgumentException("roundtrip|redirect original.dll output.dll target.exe-or-dash");
            string input = Path.GetFullPath(args[1]), output = Path.GetFullPath(args[2]);
            if (input == output || File.Exists(output)) throw new IOException("Output must be a new, separate file");
            if (Hash(input) != OriginalHash) throw new InvalidDataException("Unrecognized original TextureCompiler SHA-256 (including already patched input)");
            using var module = ModuleDefMD.Load(input);
            if (module.IsILOnly) throw new InvalidDataException("Expected a mixed-mode module");
            var core = module.GetAssemblyRefs().Single(a => a.Name == "BinaryAssetBuilder.Core");
            if (core.MDToken.Raw != 0x23000005 || core.Version.ToString() != "1.0.0.0") throw new InvalidDataException("Unexpected Core reference");
            string oldIdentity = core.FullName;
            string[] originalTypes = module.GetTypeRefs().Select(t => t.MDToken.Raw + ":" + t.FullName).ToArray();
            string[] originalMembers = module.GetMemberRefs().Select(m => m.MDToken.Raw + ":" + m.FullName).ToArray();
            string[] targetTypes = Array.Empty<string>();
            string? targetHash = null;
            string[] targetMembers = Array.Empty<string>();
            if (args[0] == "redirect")
            {
                string targetPath = Path.GetFullPath(args[3]);
                using var target = ModuleDefMD.Load(targetPath);
                if (target.Assembly == null || target.Kind == ModuleKind.Dll || target.Kind == ModuleKind.NetModule)
                    throw new InvalidDataException("Target must be a final EXE assembly");
                var needed = module.GetTypeRefs().Where(t => t.ResolutionScope == core).ToArray();
                var definitions = target.GetTypes().ToDictionary(t => t.FullName);
                foreach (var t in needed)
                    if (!definitions.ContainsKey(t.FullName)) throw new InvalidDataException("EXE is missing compatible type: " + t.FullName);
                var neededMembers = module.GetMemberRefs().Where(m => m.Class is TypeRef t && t.ResolutionScope == core).ToArray();
                foreach (var member in neededMembers)
                {
                    var type = definitions[((TypeRef)member.Class).FullName];
                    bool found = member.IsMethodRef
                        ? type.Methods.Any(m => m.IsPublic && m.FullName == member.FullName)
                        : type.Fields.Any(f => f.IsPublic && f.FullName == member.FullName);
                    if (!found) throw new InvalidDataException("EXE lacks a public member with exact signature: " + member.FullName);
                }
                targetMembers = neededMembers.Select(m => m.FullName).OrderBy(n => n, StringComparer.Ordinal).ToArray();
                targetTypes = needed.Select(t => t.FullName).OrderBy(t => t, StringComparer.Ordinal).ToArray();
                targetHash = Hash(targetPath);
                var identity = target.Assembly;
                core.Name = identity.Name;
                core.Version = identity.Version;
                core.Culture = identity.Culture;
                core.PublicKeyOrToken = identity.PublicKey;
                core.Attributes = identity.Attributes;
                core.Hash = null;
            }
            var options = new NativeModuleWriterOptions(module, false) { KeepExtraPEData = true, KeepWin32Resources = true };
            options.MetadataOptions.Flags |= MetadataFlags.PreserveAll;
            options.PEHeadersOptions.TimeDateStamp = module.Metadata.PEImage.ImageNTHeaders.FileHeader.TimeDateStamp;
            // PreserveAll does not include AssemblyRef RIDs. Pre-seed the table in
            // original order before type/member initialization creates references.
            var originalReferences = module.GetAssemblyRefs().OrderBy(a => a.Rid).ToArray();
            options.WriterEvent += (sender, e) => {
                if (e.Event != ModuleWriterEvent.MDAllocateTypeDefRids) return;
                foreach (var reference in originalReferences)
                    if (e.Writer.Metadata.GetToken(reference).Raw != reference.MDToken.Raw)
                        throw new InvalidDataException("Cannot preserve AssemblyRef token " + reference.MDToken);
            };
            module.NativeWrite(output, options);
            using var rewritten = ModuleDefMD.Load(output);
            if (!originalTypes.SequenceEqual(rewritten.GetTypeRefs().Select(t => t.MDToken.Raw + ":" + t.FullName)))
                throw new InvalidDataException("TypeRef tokens/names/namespaces changed");
            if (!originalMembers.SequenceEqual(rewritten.GetMemberRefs().Select(m => m.MDToken.Raw + ":" + m.FullName)))
                throw new InvalidDataException("MemberRef tokens/signatures changed");
            string newIdentity = rewritten.GetAssemblyRefs().Single(a => a.MDToken.Raw == 0x23000005).FullName;
            if (args[0] == "roundtrip" && newIdentity != oldIdentity) throw new InvalidDataException("Roundtrip changed Core identity");
            File.WriteAllText(output + ".patch.json", JsonSerializer.Serialize(new {
                schemaVersion = 1, tool = "TextureReferencePatch", dnlib = "4.5.0", mode = args[0], originalSha256 = Hash(input), patchedSha256 = Hash(output), targetExeSha256 = targetHash,
                assemblyRefToken = "0x23000005", originalIdentity = oldIdentity, targetIdentity = newIdentity, targetTypeNames = targetTypes, compatibleMemberSignatures = targetMembers, compatibleSignatureSha256 = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(string.Join("\n", targetMembers)))).ToLowerInvariant(),
                status = "written-not-runtime-validated", note = "Type existence is not ABI acceptance. Native PE audit and G2 A/B/C runtime receipt are mandatory."
            }, new JsonSerializerOptions { WriteIndented = true }));
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
