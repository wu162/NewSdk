#include <msclr/marshal_cppstd.h>
#include <stdexcept>
#include "XmlNodeAdapter.h"
#include "../Native/WeaponTemplate.h"
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;
namespace Recovered {
public ref class WeaponCompiler abstract sealed {
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        auto result = gcnew array<Byte>(static_cast<int>(source.size()));
        if (result->Length) System::Runtime::InteropServices::Marshal::Copy(
            IntPtr(const_cast<unsigned char*>(source.data())), result, 0, result->Length);
        return result;
    }
public:
    // Partial WeaponTemplate entry point. Unsupported structures are rejected.
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        auto adapter = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
        auto children = adapter->Children(nullptr);
        array<String^>^ durationNames = { "PreAttackDelay", "FiringDuration", "CoolDownDelayBetweenShots", "ClipReloadTime" };
        while (children->MoveNext()) {
            auto child = children->Current;
            if (child->NamespaceURI != node->NamespaceURI || Array::IndexOf<String^>(durationNames, child->LocalName) < 0)
                throw gcnew NotSupportedException("WeaponTemplate child structure is not reconstructed: " + child->LocalName);
            auto attributes = child->Clone();
            if (attributes->MoveToFirstAttribute()) do {
                if (attributes->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                if (attributes->NamespaceURI->Length != 0 || (attributes->LocalName != "MinSeconds" && attributes->LocalName != "MaxSeconds"))
                    throw gcnew NotSupportedException("Unsupported RangeDuration attribute: " + attributes->Name);
            } while (attributes->MoveToNextAttribute());
            if (child->SelectChildren(XPathNodeType::Element)->MoveNext())
                throw gcnew NotSupportedException("RangeDuration does not support nested elements.");
        }
        std::map<std::string, Native::WeaponDurationInput> durations;
        for each (String^ name in durationNames) {
            auto child = adapter->Child(name, nullptr);
            if (child == nullptr) continue;
            Native::WeaponDurationInput duration;
            duration.minSeconds = marshal_as<std::string>(child->Attribute("MinSeconds", "0s"));
            duration.maxSeconds = marshal_as<std::string>(child->Attribute("MaxSeconds", "0s"));
            durations[marshal_as<std::string>(name)] = duration;
        }
        std::map<std::string, std::string> attributes;
        for each (XmlAttribute^ attribute in node->Attributes) {
            if (attribute->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attribute->NamespaceURI->Length != 0) throw gcnew NotSupportedException("Namespaced WeaponTemplate attributes are not supported by this component.");
            if (attribute->Name == "id") continue;
            attributes[marshal_as<std::string>(attribute->Name)] = marshal_as<std::string>(attribute->Value);
        }
        try {
            auto chunk = Native::CompileWeapon(attributes, durations, bigEndian);
            auto buffer = gcnew AssetBuffer();
            buffer->InstanceData = Copy(chunk.instance);
            buffer->RelocationData = Copy(chunk.relocations);
            buffer->ImportsData = Copy(chunk.imports);
            return buffer;
        } catch (const std::invalid_argument& error) {
            throw gcnew NotSupportedException(gcnew String(error.what()));
        }
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 2496977262u) throw gcnew NotSupportedException("Only WeaponTemplate is handled by this component.");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
}

