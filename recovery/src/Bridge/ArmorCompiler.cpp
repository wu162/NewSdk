#include "XmlNodeAdapter.h"
#include "../Native/ArmorTemplate.h"
#include <msclr/marshal_cppstd.h>
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;

namespace Recovered {
public ref class ArmorCompiler abstract sealed {
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        array<Byte>^ result = gcnew array<Byte>(static_cast<int>(source.size()));
        if (result->Length) System::Runtime::InteropServices::Marshal::Copy(
            IntPtr(const_cast<unsigned char*>(source.data())), result, 0, result->Length);
        return result;
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        XmlNodeAdapter^ adapter = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
        Native::ArmorInput input;
        array<String^>^ names = { "Default", "DamageScalar", "SideDamageScalar", "RearDamageScalar", "FlankedPenalty" };
        for (int i = 0; i < 5; ++i)
            input.percentages[i] = marshal_as<std::string>(adapter->Attribute(names[i], i == 4 ? "100%" : "100"));
        XPathNodeIterator^ children = adapter->Children("Armor");
        while (children->MoveNext()) {
            XmlNodeAdapter^ child = gcnew XmlNodeAdapter(children->Current->Clone(), namespaces);
            Native::ArmorEntryInput entry;
            String^ damage = child->Attribute("Damage", nullptr);
            String^ percent = child->Attribute("Percent", nullptr);
            entry.hasDamage = damage != nullptr; entry.hasPercent = percent != nullptr;
            if (entry.hasDamage) entry.damage = marshal_as<std::string>(damage);
            if (entry.hasPercent) entry.percent = marshal_as<std::string>(percent);
            input.entries.push_back(entry);
        }
        Relo::Chunk compiled = Native::CompileArmor(input, bigEndian);
        AssetBuffer^ buffer = gcnew AssetBuffer();
        buffer->InstanceData = Copy(compiled.instance);
        buffer->RelocationData = Copy(compiled.relocations);
        buffer->ImportsData = Copy(compiled.imports);
        return buffer;
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 980180622u)
            throw gcnew NotSupportedException("Only ArmorTemplate is reconstructed by this component.");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
}
