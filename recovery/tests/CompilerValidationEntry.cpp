// Test-only entry object. Never part of the production source manifest.
#using "Managed.Application.netmodule"
#using <System.Web.Extensions.dll>
static_assert(sizeof(void*) == 4, "Validation requires the production Win32 ABI");
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Collections::Generic;
using namespace System::Security::Cryptography;
using namespace System::Web::Script::Serialization;
namespace NewModCompiler { namespace Validation {
public ref class ValidationEntry abstract sealed {
    static String^ FileHash(String^ path) {
        FileStream^ input = File::OpenRead(path);
        SHA256^ sha = SHA256::Create();
        try { return BitConverter::ToString(sha->ComputeHash(input))->Replace("-", "")->ToLowerInvariant(); }
        finally { delete input; delete sha; }
    }
    static String^ Text(Dictionary<String^, Object^>^ values, String^ key) {
        Object^ value;
        if (values == nullptr || !values->TryGetValue(key, value) || dynamic_cast<String^>(value) == nullptr)
            throw gcnew InvalidOperationException("Loaded texture manifest lacks " + key);
        return safe_cast<String^>(value);
    }
    static void Equal(String^ actual, String^ expected, String^ label) {
        if (!String::Equals(actual, expected, StringComparison::Ordinal))
            throw gcnew InvalidOperationException("Loaded texture identity/audit mismatch: " + label);
    }
    // Only the pinned, statically audited texture boundary may remain external.
    // This validates bytes, metadata and paths without creating another native
    // plugin instance or changing the fixture's lifetime/payload call sequence.
    static void VerifyTexture(Assembly^ own, Assembly^ texture) {
        String^ path = Path::Combine(Path::GetDirectoryName(own->Location), "BinaryAssetBuilder.TextureCompiler.dll");
        if (!String::Equals(Path::GetFullPath(texture->Location), Path::GetFullPath(path), StringComparison::OrdinalIgnoreCase))
            throw gcnew InvalidOperationException("Loaded texture escaped the entry EXE directory");
        Type^ contract = own->GetType("BinaryAssetBuilder.Core.IAssetBuilderPlugin", true);
        Type^ plugin = texture->GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
        if (!contract->IsAssignableFrom(plugin))
            throw gcnew InvalidOperationException("Loaded texture does not implement entry-owned Core contracts");
        Dictionary<String^, Object^>^ manifest = dynamic_cast<Dictionary<String^, Object^>^>(
            (gcnew JavaScriptSerializer())->DeserializeObject(File::ReadAllText(path + ".patch.json")));
        String^ original = "4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e";
        String^ patched = FileHash(path);
        Equal(Text(manifest, "mode"), "redirect", "patch mode");
        Equal(Text(manifest, "assemblyRefToken"), "0x23000005", "Core reference token");
        Equal(Text(manifest, "originalSha256"), original, "fixed original input");
        Equal(Text(manifest, "patchedSha256"), patched, "loaded texture bytes");
        Equal(Text(manifest, "targetExeSha256"), FileHash(own->Location), "current entry EXE bytes");
        Equal(Text(manifest, "targetIdentity"), own->FullName, "current entry EXE identity");
        Object^ auditValue;
        if (!manifest->TryGetValue("protectedPeAudit", auditValue))
            throw gcnew InvalidOperationException("Loaded texture lacks protected PE audit");
        Dictionary<String^, Object^>^ audit = dynamic_cast<Dictionary<String^, Object^>^>(auditValue);
        Equal(Text(audit, "status"), "passed", "protected PE audit status");
        Equal(Text(audit, "mode"), "redirect", "protected PE audit mode");
        Equal(Text(audit, "originalSha256"), original, "protected PE original");
        Equal(Text(audit, "outputSha256"), patched, "protected PE output");
        int references = 0;
        for each (AssemblyName^ reference in texture->GetReferencedAssemblies()) {
            if (reference->FullName == own->FullName) ++references;
            if (reference->Name == "BinaryAssetBuilder.Core")
                throw gcnew InvalidOperationException("Loaded texture still references old Core");
        }
        if (references != 1)
            throw gcnew InvalidOperationException("Loaded texture has no unique current entry reference");
    }
    static void VerifyOwnership() {
        Assembly^ own = Assembly::GetEntryAssembly();
        if (own->GetName()->Name != "NewModCompiler" || own->GetModules()->Length != 1)
            throw gcnew InvalidOperationException("Validation must enter a single physical NewModCompiler EXE");
        for each (String^ name in gcnew array<String^> {
            "BinaryAssetBuilder.Core.AssetBuffer", "Recovered.ArmorCompiler",
            "NewModCompiler.Application.BuiltinCompilers", "Recovered.Compiler.W3x.Plugin" }) {
            if (own->GetType(name, true)->Assembly != own)
                throw gcnew InvalidOperationException("Business type escaped entry module: " + name);
        }
        for each (Assembly^ loaded in AppDomain::CurrentDomain->GetAssemblies()) {
            String^ name = loaded->GetName()->Name;
            if (name == "BinaryAssetBuilder.TextureCompiler") {
                VerifyTexture(own, loaded);
                continue;
            }
            if (name->StartsWith("BinaryAssetBuilder.", StringComparison::Ordinal) ||
                name->StartsWith("Recovered.", StringComparison::Ordinal) ||
                name->StartsWith("EALA.", StringComparison::Ordinal) || name == "Mvp.Xml" ||
                name->StartsWith("Managed.Foundation", StringComparison::Ordinal) ||
                name->StartsWith("Managed.Application", StringComparison::Ordinal))
                throw gcnew InvalidOperationException("Old business assembly loaded: " + loaded->FullName);
        }
    }
public:
    static int Main(array<String^>^ arguments) {
        try {
            if (arguments->Length < 1) throw gcnew ArgumentException("Expected managed fixture EXE path and arguments");
            VerifyOwnership();
            String^ path = Path::GetFullPath(arguments[0]);
            Assembly^ fixture = Assembly::LoadFrom(path);
            MethodInfo^ entry = fixture->EntryPoint;
            if (fixture == Assembly::GetEntryAssembly() || entry == nullptr)
                throw gcnew InvalidOperationException("Expected a separate managed fixture entry point");
            array<String^>^ rest = gcnew array<String^>(arguments->Length - 1);
            Array::Copy(arguments, 1, rest, 0, rest->Length);
            array<Object^>^ invokeArgs = entry->GetParameters()->Length == 0
                ? gcnew array<Object^>(0) : gcnew array<Object^> { rest };
            Object^ result = entry->Invoke(nullptr, invokeArgs);
            VerifyOwnership();
            return entry->ReturnType == Int32::typeid ? safe_cast<int>(result) : 0;
        } catch (Exception^ error) { Console::Error->WriteLine(error); return 1; }
    }
};
}}
