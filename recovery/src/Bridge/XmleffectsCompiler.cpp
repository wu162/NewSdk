#include <msclr/marshal_cppstd.h>
#include "XmlNodeAdapter.h"
#include "../Native/Xmleffects.h"
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;
namespace Recovered {
ref class XmleffectsXml abstract sealed {
public:
    static void Attributes(XmlNode^ node, array<String^>^ allowed) {
        for each (XmlAttribute^ attr in node->Attributes) {
            if (attr->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            bool found = false;
            if (attr->NamespaceURI->Length == 0)
                for each (String^ name in allowed) if (attr->LocalName == name) { found = true; break; }
            if (!found) throw gcnew NotSupportedException("Unsupported effects attribute: " + attr->Name);
        }
    }
    static void Root(XmlNode^ node, String^ name) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        if (node->NodeType != XmlNodeType::Element || node->LocalName != name ||
            node->NamespaceURI != "uri:ea.com:eala:asset")
            throw gcnew NotSupportedException("Expected effects root " + name);
    }
    static array<Byte>^ Copy(const std::vector<unsigned char>& src) {
        array<Byte>^ dst = gcnew array<Byte>(static_cast<int>(src.size()));
        if (dst->Length) System::Runtime::InteropServices::Marshal::Copy(
            IntPtr(const_cast<unsigned char*>(src.data())), dst, 0, dst->Length);
        return dst;
    }
    static AssetBuffer^ Buffer(const Relo::Chunk& chunk) {
        AssetBuffer^ result = gcnew AssetBuffer();
        result->InstanceData = Copy(chunk.instance);
        result->RelocationData = Copy(chunk.relocations);
        result->ImportsData = Copy(chunk.imports);
        return result;
    }
    static void Empty(XmlNode^ node) {
        for each (XmlNode^ child in node->ChildNodes) {
            if (child->NodeType == XmlNodeType::Element ||
                ((child->NodeType == XmlNodeType::Text || child->NodeType == XmlNodeType::CDATA) &&
                !String::IsNullOrWhiteSpace(child->Value)))
                throw gcnew NotSupportedException("Unsupported effects content: " + child->Name);
        }
    }
};
public ref class FXListCompiler abstract sealed {
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        XmleffectsXml::Root(node, "FXList");
        if (namespaces == nullptr) throw gcnew ArgumentNullException("namespaces");
        array<String^>^ allowed = {"id", "PlayEvenIfShrouded", "Tailorable", "CullTracking", "CullTrackingMin", "CullTrackingMax"};
        XmleffectsXml::Attributes(node, allowed);
        bool listSeen = false;
        for each (XmlNode^ child in node->ChildNodes) {
            if (child->NodeType == XmlNodeType::Element) {
                if (child->LocalName != "NuggetList" || child->NamespaceURI != node->NamespaceURI || listSeen)
                    throw gcnew NotSupportedException("Unsupported FXList child: " + child->Name);
                listSeen = true;
                XmleffectsXml::Attributes(child, gcnew array<String^>(0));
                XmleffectsXml::Empty(child);
            } else if ((child->NodeType == XmlNodeType::Text || child->NodeType == XmlNodeType::CDATA) &&
                       !String::IsNullOrWhiteSpace(child->Value))
                throw gcnew NotSupportedException("Unsupported FXList text");
        }
        XmlNodeAdapter^ adapter = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
        Native::XmleffectsFXListInput input;
        input.playEvenIfShrouded = marshal_as<std::string>(adapter->Attribute("PlayEvenIfShrouded", "false"));
        input.tailorable = marshal_as<std::string>(adapter->Attribute("Tailorable", "false"));
        String^ value = adapter->Attribute("CullTracking", nullptr);
        input.hasCullTracking = value != nullptr;
        if (value != nullptr) input.cullTracking = marshal_as<std::string>(value);
        value = adapter->Attribute("CullTrackingMin", nullptr);
        input.hasCullTrackingMin = value != nullptr;
        if (value != nullptr) input.cullTrackingMin = marshal_as<std::string>(value);
        value = adapter->Attribute("CullTrackingMax", nullptr);
        input.hasCullTrackingMax = value != nullptr;
        if (value != nullptr) input.cullTrackingMax = marshal_as<std::string>(value);
        return XmleffectsXml::Buffer(Native::CompileXmleffectsFXList(input, bigEndian));
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 2254974584u)
            throw gcnew NotSupportedException("Expected FXList TypeId");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
public ref class FXParticleSystemTemplateCompiler abstract sealed {
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        XmleffectsXml::Root(node, "FXParticleSystemTemplate");
        throw gcnew NotSupportedException("FXParticleSystemTemplate recovery pending source evidence; no original DLL forwarding.");
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 686292351u)
            throw gcnew NotSupportedException("Expected FXParticleSystemTemplate TypeId");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
}
