// Test-only entry object. Never part of the production source manifest.
#using "Managed.Application.netmodule"
static_assert(sizeof(void*) == 4, "Validation requires the production Win32 ABI");
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
namespace NewModCompiler { namespace Validation {
public ref class ValidationEntry abstract sealed {
    // Build scripts verify the prepared DLL bytes before the fixture starts.
    // Retain module/contract ownership checks without repeating deployment audits.
    static void VerifyTexture(Assembly^ own, Assembly^ texture) {
        String^ path = Path::Combine(Path::GetDirectoryName(own->Location), "BinaryAssetBuilder.TextureCompiler.dll");
        if (!String::Equals(Path::GetFullPath(texture->Location), Path::GetFullPath(path), StringComparison::OrdinalIgnoreCase))
            throw gcnew InvalidOperationException("Loaded texture escaped the entry EXE directory");
        Type^ contract = own->GetType("BinaryAssetBuilder.Core.IAssetBuilderPlugin", true);
        Type^ plugin = texture->GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
        if (!contract->IsAssignableFrom(plugin))
            throw gcnew InvalidOperationException("Loaded texture does not implement entry-owned Core contracts");
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
