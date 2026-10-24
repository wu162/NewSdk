#include <msclr/marshal_cppstd.h>
#include "XmlNodeAdapter.h"
#include "../Native/Xmlattributes.h"
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;
namespace Recovered {
public ref class LogicCommandSetCompiler abstract sealed {
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        array<Byte>^ result = gcnew array<Byte>(static_cast<int>(source.size()));
        if (result->Length) System::Runtime::InteropServices::Marshal::Copy(
            IntPtr(const_cast<unsigned char*>(source.data())), result, 0, result->Length);
        return result;
    }
    static void Attributes(XmlNode^ node, bool root) {
        for each (XmlAttribute^ a in node->Attributes) {
            if (a->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (root && a->NamespaceURI == String::Empty &&
                (a->Name == "id" || a->Name == "inheritFrom")) continue;
            throw gcnew NotSupportedException("Unsupported LogicCommandSet attribute: " + a->Name);
        }
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        if (namespaces == nullptr) throw gcnew ArgumentNullException("namespaces");
        if (node->NodeType != XmlNodeType::Element || node->LocalName != "LogicCommandSet" ||
            node->NamespaceURI != "uri:ea.com:eala:asset" ||
            namespaces->LookupNamespace("ea") != node->NamespaceURI)
            throw gcnew NotSupportedException("Expected asset LogicCommandSet and matching ea namespace");
        Attributes(node, true);
        for each (XmlNode^ child in node->ChildNodes) {
            if (child->NodeType == XmlNodeType::Comment || child->NodeType == XmlNodeType::Whitespace ||
                child->NodeType == XmlNodeType::SignificantWhitespace) continue;
            if (child->NodeType != XmlNodeType::Element) {
                if (String::IsNullOrWhiteSpace(child->Value)) continue;
                throw gcnew NotSupportedException("LogicCommandSet contains non-element content");
            }
            if (child->LocalName != "Cmd" || child->NamespaceURI != node->NamespaceURI)
                throw gcnew NotSupportedException("Unsupported LogicCommandSet child: " + child->Name);
            Attributes(child, false);
            for each (XmlNode^ part in child->ChildNodes)
                if (part->NodeType == XmlNodeType::Element)
                    throw gcnew NotSupportedException("Cmd must contain only text");
        }
        XmlNodeAdapter^ adapter = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
        std::vector<std::string> commands;
        XPathNodeIterator^ children = adapter->Children("Cmd");
        while (children->MoveNext())
            commands.push_back(marshal_as<std::string>(children->Current->Value));
        Relo::Chunk compiled = Native::CompileXmlattributesLogicCommandSet(commands, bigEndian);
        AssetBuffer^ result = gcnew AssetBuffer();
        result->InstanceData = Copy(compiled.instance);
        result->RelocationData = Copy(compiled.relocations);
        result->ImportsData = Copy(compiled.imports);
        return result;
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 3959844197u)
            throw gcnew NotSupportedException("Expected LogicCommandSet TypeId 3959844197");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
}
