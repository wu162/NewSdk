#include <msclr/marshal_cppstd.h>
#include <stdexcept>
#include "XmlNodeAdapter.h"
#include "../Native/GameObject.h"
#using <BinaryAssetBuilder.Core.dll>
#using <System.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;
namespace Recovered {
public ref class GameObjectCompiler abstract sealed {
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        auto result=gcnew array<Byte>(static_cast<int>(source.size()));
        if(result->Length) System::Runtime::InteropServices::Marshal::Copy(IntPtr(const_cast<unsigned char*>(source.data())),result,0,result->Length);
        return result;
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node,XmlNamespaceManager^ namespaces,bool bigEndian) {
        if(node==nullptr) throw gcnew ArgumentNullException("node");
        if(node->LocalName!="GameObject" || node->NamespaceURI!="uri:ea.com:eala:asset") throw gcnew NotSupportedException("Expected GameObject asset element.");
        auto nav=node->CreateNavigator();auto attrs=nav->Clone();std::map<std::string,std::string> input;
        if(attrs->MoveToFirstAttribute()) do {
            if(attrs->NamespaceURI=="http://www.w3.org/2000/xmlns/") continue;
            if(attrs->NamespaceURI->Length!=0) throw gcnew NotSupportedException("Namespaced GameObject attribute.");
            if(attrs->LocalName=="id" || attrs->LocalName=="TypeId") continue;
            input[marshal_as<std::string>(attrs->LocalName)]=marshal_as<std::string>(attrs->Value);
        } while(attrs->MoveToNextAttribute());
        std::vector<Native::GameObjectChild> children;
        auto nodes=nav->SelectChildren(XPathNodeType::Element);
        auto seen=gcnew System::Collections::Generic::HashSet<String^>();
        while(nodes->MoveNext()) {
            auto child=nodes->Current;
            auto name=child->LocalName;
            if(child->NamespaceURI!="uri:ea.com:eala:asset" ||
                (name!="Buildable" && name!="ThingClass" && name!="DisplayColor" && name!="DisplayName" && name!="DisplayNameTransformed" && name!="EquivalentTo" && name!="UpgradeCameo" && name!="DisplayUpgrade" && name!="Flammability" && name!="LiveCameraOffset"))
                throw gcnew NotSupportedException("Unsupported GameObject child.");
            bool repeated=name=="EquivalentTo" || name=="UpgradeCameo" || name=="DisplayUpgrade";
            if(!repeated && !seen->Add(name)) throw gcnew NotSupportedException("Duplicate GameObject child.");
            if(child->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("Nested GameObject child content.");
            Native::GameObjectChild item;item.name=marshal_as<std::string>(name);item.text=marshal_as<std::string>(child->Value);
            auto a=child->Clone();
            if(a->MoveToFirstAttribute()) do {
                if(a->NamespaceURI=="http://www.w3.org/2000/xmlns/") continue;
                if(a->NamespaceURI->Length!=0) throw gcnew NotSupportedException("Namespaced GameObject child attribute.");
                if(a->LocalName=="TypeId") continue;
                bool allowed=(name=="DisplayColor" && (a->LocalName=="R" || a->LocalName=="G" || a->LocalName=="B" || a->LocalName=="A")) ||
                    (name=="Flammability" && (a->LocalName=="Fuel" || a->LocalName=="MaxBurnRate" || a->LocalName=="Decay" || a->LocalName=="Resistance")) ||
                    (name=="LiveCameraOffset" && (a->LocalName=="x" || a->LocalName=="y" || a->LocalName=="z"));
                if(!allowed)
                    throw gcnew NotSupportedException("Unsupported GameObject child attribute.");
                item.attributes[marshal_as<std::string>(a->LocalName)]=marshal_as<std::string>(a->Value);
            } while(a->MoveToNextAttribute());
            if((name=="DisplayColor" || name=="Flammability" || name=="LiveCameraOffset") && child->Value->Trim()->Length!=0) throw gcnew NotSupportedException("DisplayColor text content.");
            children.push_back(item);
            if(name=="UpgradeCameo" || name=="DisplayUpgrade") {
                unsigned count=0;for(const auto& c:children) if(c.name==item.name) ++count;
                if(count>(name=="UpgradeCameo"?5u:2u)) throw gcnew NotSupportedException("GameObject list exceeds XSD maximum.");
            }
        }
        try {
            auto compiled=Native::CompileGameObject(input,children,bigEndian);auto buffer=gcnew AssetBuffer();
            buffer->InstanceData=Copy(compiled.instance);buffer->RelocationData=Copy(compiled.relocations);buffer->ImportsData=Copy(compiled.imports);return buffer;
        } catch(const std::exception& error) {throw gcnew NotSupportedException(gcnew String(error.what()));}
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration,TargetPlatform platform) {
        if(declaration==nullptr) throw gcnew ArgumentNullException("declaration");
        if(declaration->Handle->TypeId!=2486173485u) throw gcnew NotSupportedException("Expected GameObject type.");
        return CompileXml(declaration->Node,declaration->Document->NamespaceManager,platform!=TargetPlatform::Win32);
    }
};
}

