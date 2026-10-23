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
    static void NoStructuredText(XPathNavigator^ node) {
        auto contents=node->SelectChildren(XPathNodeType::All);
        while(contents->MoveNext()) if((contents->Current->NodeType==XPathNodeType::Text || contents->Current->NodeType==XPathNodeType::SignificantWhitespace) && contents->Current->Value->Trim()->Length!=0)
            throw gcnew NotSupportedException("Unexpected text in GameDependency structure.");
    }
    static Native::GameObjectChild ReadDependencyFilter(XPathNavigator^ node) {
        Native::GameObjectChild result;result.name="ObjectFilter";
        array<String^>^ allowed={"id","Rule","Relationship","Alignment","Include","Exclude","StatusBitFlags","StatusBitFlagsExclude"};
        auto a=node->Clone();
        if(a->MoveToFirstAttribute()) do {
            if(a->NamespaceURI=="http://www.w3.org/2000/xmlns/" || (a->NamespaceURI->Length==0 && a->LocalName=="TypeId")) continue;
            if(a->NamespaceURI->Length!=0 || Array::IndexOf<String^>(allowed,a->LocalName)<0) throw gcnew NotSupportedException("Unsupported dependency filter attribute.");
            result.attributes[marshal_as<std::string>(a->LocalName)]=marshal_as<std::string>(a->Value);
        } while(a->MoveToNextAttribute());
        auto nodes=node->SelectChildren(XPathNodeType::Element);
        while(nodes->MoveNext()) {
            auto child=nodes->Current;
            if(child->NamespaceURI!=node->NamespaceURI || (child->LocalName!="IncludeThing" && child->LocalName!="ExcludeThing") || child->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("Unsupported dependency filter child.");
            auto attrs=child->Clone();if(attrs->MoveToFirstAttribute()) do {
                if(attrs->NamespaceURI!="http://www.w3.org/2000/xmlns/" && !(attrs->NamespaceURI->Length==0 && attrs->LocalName=="TypeId")) throw gcnew NotSupportedException("Unsupported filter thing attribute.");
            } while(attrs->MoveToNextAttribute());
            Native::GameObjectChild thing;thing.name=marshal_as<std::string>(child->LocalName);thing.text=marshal_as<std::string>(child->Value);result.children.push_back(thing);
        }
        NoStructuredText(node);return result;
    }
    static Native::GameObjectChild ReadDependency(XPathNavigator^ node) {
        Native::GameObjectChild result;result.name="GameDependency";
        array<String^>^ allowed={"RequiredModelConditionsAny","ForbiddenModelConditions","RequiredObjectStatusAny"};
        auto a=node->Clone();
        if(a->MoveToFirstAttribute()) do {
            if(a->NamespaceURI=="http://www.w3.org/2000/xmlns/" || (a->NamespaceURI->Length==0 && a->LocalName=="TypeId")) continue;
            if(a->NamespaceURI->Length!=0 || Array::IndexOf<String^>(allowed,a->LocalName)<0) throw gcnew NotSupportedException("Unsupported GameDependency attribute.");
            result.attributes[marshal_as<std::string>(a->LocalName)]=marshal_as<std::string>(a->Value);
        } while(a->MoveToNextAttribute());
        bool filterSeen=false;auto nodes=node->SelectChildren(XPathNodeType::Element);
        while(nodes->MoveNext()) {
            auto child=nodes->Current;
            if(child->NamespaceURI!=node->NamespaceURI) throw gcnew NotSupportedException("Foreign GameDependency child.");
            if(child->LocalName=="ObjectFilter") {
                if(filterSeen) throw gcnew NotSupportedException("Duplicate dependency ObjectFilter.");filterSeen=true;result.children.push_back(ReadDependencyFilter(child));continue;
            }
            if((child->LocalName!="RequiredObject" && child->LocalName!="ForbiddenUpgrade" && child->LocalName!="NeededUpgrade") || child->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("Unsupported GameDependency child.");
            auto attrs=child->Clone();if(attrs->MoveToFirstAttribute()) do {
                if(attrs->NamespaceURI!="http://www.w3.org/2000/xmlns/" && !(attrs->NamespaceURI->Length==0 && attrs->LocalName=="TypeId")) throw gcnew NotSupportedException("Unsupported dependency list attribute.");
            } while(attrs->MoveToNextAttribute());
            Native::GameObjectChild value;value.name=marshal_as<std::string>(child->LocalName);value.text=marshal_as<std::string>(child->Value);result.children.push_back(value);
        }
        NoStructuredText(node);return result;
    }
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
                (name!="Buildable" && name!="ThingClass" && name!="DisplayColor" && name!="DisplayName" && name!="DisplayNameTransformed" && name!="EquivalentTo" && name!="UpgradeCameo" && name!="DisplayUpgrade" && name!="Flammability" && name!="LiveCameraOffset" && name!="ArmorSet" && name!="LocomotorSet" && name!="FormationPreviewDecal" && name!="FormationPreviewItemDecal" && name!="ShadowInfo" && name!="VisionInfo" && name!="AutoResolveArmor" && name!="AutoResolveWeapon" && name!="AutoResolveInfo" && name!="BuildFadeInOnCreateList" && name!="BuildVariations" && name!="ObjectResourceInfo" && name!="GameDependency"))
                throw gcnew NotSupportedException("Unsupported GameObject child.");
            bool repeated=name=="ArmorSet" || name=="LocomotorSet" || name=="EquivalentTo" || name=="UpgradeCameo" || name=="DisplayUpgrade";
            if(!repeated && !seen->Add(name)) throw gcnew NotSupportedException("Duplicate GameObject child.");
            if(name=="GameDependency") {children.push_back(ReadDependency(child));continue;}
            bool autoSet=name=="AutoResolveArmor" || name=="AutoResolveWeapon";
            bool resourceInfo=name=="ObjectResourceInfo";
            if(!autoSet && !resourceInfo && child->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("Nested GameObject child content.");
            Native::GameObjectChild item;item.name=marshal_as<std::string>(name);item.text=marshal_as<std::string>(child->Value);
            if(autoSet || resourceInfo) {
                auto values=child->SelectChildren(XPathNodeType::Element);
                while(values->MoveNext()) {
                    auto value=values->Current;
                    if(value->NamespaceURI!="uri:ea.com:eala:asset" || (resourceInfo?value->LocalName!="BuildCost":(value->LocalName!="RequiredUpgrade" && value->LocalName!="ExcludedUpgrade")) || value->SelectChildren(XPathNodeType::Element)->MoveNext())
                        throw gcnew NotSupportedException("Unsupported auto-resolve upgrade child.");
                    Native::GameObjectChild upgrade;upgrade.name=marshal_as<std::string>(value->LocalName);upgrade.text=marshal_as<std::string>(value->Value);
                    auto va=value->Clone();
                    if(va->MoveToFirstAttribute()) do {
                        if(va->NamespaceURI=="http://www.w3.org/2000/xmlns/") continue;
                        if(va->NamespaceURI->Length!=0 || (va->LocalName!="TypeId" && !(resourceInfo && (va->LocalName=="Account" || va->LocalName=="Amount")))) throw gcnew NotSupportedException("Unsupported nested list attribute.");
                        if(va->LocalName!="TypeId") upgrade.attributes[marshal_as<std::string>(va->LocalName)]=marshal_as<std::string>(va->Value);
                    } while(va->MoveToNextAttribute());
                    if(resourceInfo && value->Value->Trim()->Length!=0) throw gcnew NotSupportedException("Unexpected text in BuildCost.");
                    item.children.push_back(upgrade);
                }
                auto content=child->SelectChildren(XPathNodeType::All);
                while(content->MoveNext()) if((content->Current->NodeType==XPathNodeType::Text || content->Current->NodeType==XPathNodeType::SignificantWhitespace) && content->Current->Value->Trim()->Length!=0)
                    throw gcnew NotSupportedException("Unexpected text in auto-resolve set.");
            }
            auto a=child->Clone();
            if(a->MoveToFirstAttribute()) do {
                if(a->NamespaceURI=="http://www.w3.org/2000/xmlns/") continue;
                if(a->NamespaceURI->Length!=0) throw gcnew NotSupportedException("Namespaced GameObject child attribute.");
                if(a->LocalName=="TypeId") continue;
                bool allowed=(name=="AutoResolveArmor" && a->LocalName=="Armor") || (name=="AutoResolveWeapon" && a->LocalName=="Weapon") || (name=="DisplayColor" && (a->LocalName=="R" || a->LocalName=="G" || a->LocalName=="B" || a->LocalName=="A")) ||
                    (name=="Flammability" && (a->LocalName=="Fuel" || a->LocalName=="MaxBurnRate" || a->LocalName=="Decay" || a->LocalName=="Resistance")) ||
                    (name=="LiveCameraOffset" && (a->LocalName=="x" || a->LocalName=="y" || a->LocalName=="z")) ||
                    (name=="ArmorSet" && (a->LocalName=="Armor" || a->LocalName=="DamageFX" || a->LocalName=="Conditions")) ||
                    (name=="LocomotorSet" && (a->LocalName=="Speed" || a->LocalName=="Condition" || a->LocalName=="Locomotor"));
                allowed=allowed || Native::GameObjectLeafAttributeSupported(item.name,marshal_as<std::string>(a->LocalName));
                if(!allowed)
                    throw gcnew NotSupportedException("Unsupported GameObject child attribute.");
                item.attributes[marshal_as<std::string>(a->LocalName)]=marshal_as<std::string>(a->Value);
            } while(a->MoveToNextAttribute());
            if((name=="DisplayColor" || name=="Flammability" || name=="LiveCameraOffset" || name=="ArmorSet" || name=="LocomotorSet" || name=="FormationPreviewDecal" || name=="FormationPreviewItemDecal" || name=="ShadowInfo" || name=="VisionInfo" || name=="AutoResolveInfo") && child->Value->Trim()->Length!=0) throw gcnew NotSupportedException("Unexpected text in structured GameObject child.");
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


