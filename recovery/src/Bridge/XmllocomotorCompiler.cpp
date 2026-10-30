#include <msclr/marshal_cppstd.h>
#include "XmlNodeAdapter.h"
#include "../Native/Xmllocomotor.h"
#include <exception>
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;

namespace Recovered {
public ref class LocomotorTemplateCompiler abstract sealed {
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        array<Byte>^ result = gcnew array<Byte>(static_cast<int>(source.size()));
        if (result->Length) System::Runtime::InteropServices::Marshal::Copy(
            IntPtr(const_cast<unsigned char*>(source.data())), result, 0, result->Length);
        return result;
    }
    static bool NamespaceAttribute(XmlAttribute^ attribute) {
        return attribute->NamespaceURI == "http://www.w3.org/2000/xmlns/";
    }
    static void Attributes(XmlNode^ node, bool root) {

    }
    static void Read(XmlNodeAdapter^ node, const Native::Xmllocomotor::Field* fields,
        unsigned count, std::map<std::string,std::string>& values) {
        for (unsigned i = 0; i < count; ++i) {
            String^ value = node->Attribute(gcnew String(fields[i].name), nullptr);
            if (value != nullptr) values[fields[i].name] = marshal_as<std::string>(value);
        }
    }
    static void Children(XmlNode^ node, bool root) {
        bool bounce = false, jet = false;
        for each (XmlNode^ child in node->ChildNodes) {
            if (child->NodeType != XmlNodeType::Element) {

                continue;
            }
            ;
            if (child->LocalName == "JetLocomotorData" && !jet) {
                jet = true; Attributes(child, false); Children(child, false);

            } else if (child->LocalName == "BounceKickTerrainMap" && !bounce) {
                bounce = true;

                Children(child, false);
            }
        }
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");

        Attributes(node, true); Children(node, true);
        try {
            Native::Xmllocomotor::Input input;
            XmlNodeAdapter^ adapter = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
            Read(adapter, Native::Xmllocomotor::Fields, Native::Xmllocomotor::FieldCount, input.attributes);
            XmlNodeAdapter^ bounce = adapter->Child("BounceKickTerrainMap", nullptr);
            if (bounce != nullptr) { input.hasBounce = true; input.bounce = marshal_as<std::string>(bounce->Value()); }
            XmlNodeAdapter^ jet = adapter->Child("JetLocomotorData", nullptr);
            if (jet != nullptr) {
                input.hasJet = true;
                Read(jet, Native::Xmllocomotor::JetFields, Native::Xmllocomotor::JetFieldCount, input.jetAttributes);
            }
            Relo::Chunk result = Native::Xmllocomotor::Compile(input, bigEndian);
            AssetBuffer^ buffer = gcnew AssetBuffer();
            buffer->InstanceData = Copy(result.instance);
            buffer->RelocationData = Copy(result.relocations);
            buffer->ImportsData = Copy(result.imports);
            return buffer;
        } catch (const std::exception& e) { throw gcnew ArgumentException(gcnew String(e.what())); }
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 3972178387u)
            throw gcnew NotSupportedException("Only LocomotorTemplate is reconstructed by this component");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
}
