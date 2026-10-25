#include <msclr/marshal_cppstd.h>
#include "XmlNodeAdapter.h"
#include "../Native/Xmlarmyopening.h"
#include <stdexcept>
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;
namespace Recovered {
public ref class XmlarmyopeningCompiler abstract sealed {
    static Native::XmlarmyopeningNode Read(XmlNode^ node,int depth) {
        if(depth>32)throw gcnew NotSupportedException("Army opening nesting exceeds 32.");
        if(node->NodeType!=XmlNodeType::Element || node->NamespaceURI!="uri:ea.com:eala:asset")
            throw gcnew NotSupportedException("Army opening requires the asset namespace.");
        Native::XmlarmyopeningNode result;result.name=marshal_as<std::string>(node->LocalName);
        for each(XmlAttribute^ attr in node->Attributes) {
            if(attr->NamespaceURI=="http://www.w3.org/2000/xmlns/")continue;
            if(attr->NamespaceURI->Length)throw gcnew NotSupportedException("Qualified Army opening attributes are unsupported.");
            result.attributes[marshal_as<std::string>(attr->LocalName)]=marshal_as<std::string>(attr->Value);
        }
        for each(XmlNode^ child in node->ChildNodes) {
            if(child->NodeType==XmlNodeType::Element)result.children.push_back(Read(child,depth+1));
            else if(child->NodeType==XmlNodeType::Text || child->NodeType==XmlNodeType::CDATA || child->NodeType==XmlNodeType::SignificantWhitespace)
                result.value+=marshal_as<std::string>(child->Value);
            else if(child->NodeType!=XmlNodeType::Comment && child->NodeType!=XmlNodeType::Whitespace)
                throw gcnew NotSupportedException("Unsupported Army opening XML node.");
        }
        return result;
    }
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        array<Byte>^ target=gcnew array<Byte>(static_cast<int>(source.size()));
        if(target->Length)Runtime::InteropServices::Marshal::Copy(IntPtr(const_cast<unsigned char*>(source.data())),target,0,target->Length);
        return target;
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node,XmlNamespaceManager^ namespaces,bool bigEndian) {
        if(node==nullptr)throw gcnew ArgumentNullException("node");
        if(namespaces==nullptr || namespaces->LookupNamespace("ea")!="uri:ea.com:eala:asset")throw gcnew NotSupportedException("Army opening requires ea namespace binding.");
        try {
            Relo::Chunk chunk=Native::CompileXmlarmyopening(Read(node,0),bigEndian);
            AssetBuffer^ result=gcnew AssetBuffer();result->InstanceData=Copy(chunk.instance);result->RelocationData=Copy(chunk.relocations);result->ImportsData=Copy(chunk.imports);return result;
        } catch(const std::exception& error) {throw gcnew NotSupportedException(gcnew String(error.what()));}
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration,TargetPlatform platform) {
        if(declaration==nullptr)throw gcnew ArgumentNullException("declaration");
        XmlNode^ node=declaration->Node;
        if(node==nullptr)throw gcnew ArgumentException("Missing Army opening node.");
        unsigned expected=Native::XmlarmyopeningTypeId(marshal_as<std::string>(node->LocalName));
        if(!expected || declaration->Handle->TypeId!=expected)throw gcnew NotSupportedException("Army opening root/TypeId mismatch.");
        return CompileXml(node,declaration->Document->NamespaceManager,platform!=TargetPlatform::Win32);
    }
};
public ref class ArmyDefinitionCompiler abstract sealed {
public:
    static AssetBuffer^ CompileXml(XmlNode^ node,XmlNamespaceManager^ namespaces,bool bigEndian){
        if(node==nullptr)throw gcnew ArgumentNullException("node");
        if(node->LocalName!="ArmyDefinition")throw gcnew NotSupportedException("Expected ArmyDefinition root.");
        return XmlarmyopeningCompiler::CompileXml(node,namespaces,bigEndian);
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration,TargetPlatform platform){
        if(declaration==nullptr)throw gcnew ArgumentNullException("declaration");
        if(declaration->Node==nullptr||declaration->Node->LocalName!="ArmyDefinition")throw gcnew NotSupportedException("Expected ArmyDefinition declaration.");
        return XmlarmyopeningCompiler::ProcessInstance(declaration,platform);
    }
};
public ref class SkirmishOpeningMoveCompiler abstract sealed {
public:
    static AssetBuffer^ CompileXml(XmlNode^ node,XmlNamespaceManager^ namespaces,bool bigEndian){
        if(node==nullptr)throw gcnew ArgumentNullException("node");
        if(node->LocalName!="SkirmishOpeningMove")throw gcnew NotSupportedException("Expected SkirmishOpeningMove root.");
        return XmlarmyopeningCompiler::CompileXml(node,namespaces,bigEndian);
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration,TargetPlatform platform){
        if(declaration==nullptr)throw gcnew ArgumentNullException("declaration");
        if(declaration->Node==nullptr||declaration->Node->LocalName!="SkirmishOpeningMove")throw gcnew NotSupportedException("Expected SkirmishOpeningMove declaration.");
        return XmlarmyopeningCompiler::ProcessInstance(declaration,platform);
    }
};
}
