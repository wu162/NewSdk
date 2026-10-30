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
public:
    // Core validates the document before plugin dispatch.
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        if (namespaces == nullptr) throw gcnew ArgumentNullException("namespaces");
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
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager,
            platform != TargetPlatform::Win32);
    }
};
namespace {
Native::XmlattributesValues XmlattributesRead(XmlNode^ node, array<String^>^ allowed, bool metadata) {
    Native::XmlattributesValues result;
    for each (XmlAttribute^ a in node->Attributes) {
        if (a->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
        // BAB schema-type metadata must not enter native attribute/filter value maps.
        if (a->NamespaceURI == String::Empty && a->Name == "TypeId") continue;
        if (metadata && a->NamespaceURI == String::Empty && (a->Name == "id" || a->Name == "inheritFrom")) continue;

        result[marshal_as<std::string>(a->Name)] = marshal_as<std::string>(a->Value);
    }
    return result;
}
System::Collections::Generic::List<XmlNode^>^ XmlattributesChildren(XmlNode^ node, array<String^>^ allowed) {
    auto result=gcnew System::Collections::Generic::List<XmlNode^>();
    for each (XmlNode^ c in node->ChildNodes) {
        if (c->NodeType == XmlNodeType::Comment || c->NodeType == XmlNodeType::Whitespace ||
            c->NodeType == XmlNodeType::SignificantWhitespace) continue;
        if (c->NodeType != XmlNodeType::Element) {
            if(String::IsNullOrWhiteSpace(c->Value)) continue;

        }
        ;

        result->Add(c);
    }
    return result;
}
void XmlattributesRoot(XmlNode^ node, XmlNamespaceManager^ ns, String^ name) {
    if(node == nullptr) throw gcnew ArgumentNullException("node");

}
array<Byte>^ XmlattributesCopy(const std::vector<unsigned char>& source) {
    auto result=gcnew array<Byte>(static_cast<int>(source.size()));
    if(result->Length) System::Runtime::InteropServices::Marshal::Copy(
        IntPtr(const_cast<unsigned char*>(source.data())),result,0,result->Length);
    return result;
}
AssetBuffer^ XmlattributesBuffer(const Relo::Chunk& source) {
    auto result=gcnew AssetBuffer();
    result->InstanceData=XmlattributesCopy(source.instance);
    result->RelocationData=XmlattributesCopy(source.relocations);
    result->ImportsData=XmlattributesCopy(source.imports);
    return result;
}
}
public ref class AttributeModifierCompiler abstract sealed {
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ ns, bool big) {
        XmlattributesRoot(node,ns,"AttributeModifier");
        Native::XmlattributesInput input;
        input.attributes=XmlattributesRead(node,gcnew array<String^>{
            "Category","Duration","ReplaceInCategoryIfLongest","IgnoreIfAnticategoryActive","RemoveWhenDisabledByTypes",
            "StartFX","EndFX","ModelConditionsSet","ModelConditionsClear","ObjectStatusToSet","StackingLimit","ArmorSetType","Shader"},true);
        for each(XmlNode^ c in XmlattributesChildren(node,gcnew array<String^>{"Modifier"})) {
            auto item=XmlattributesRead(c,gcnew array<String^>{"Type","Value"},false);
            XmlattributesChildren(c,gcnew array<String^>(0));
            input.modifiers.push_back(item);
        }
        return XmlattributesBuffer(Native::CompileXmlattributesAttributeModifier(input,big));
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if(declaration == nullptr) throw gcnew ArgumentNullException("declaration");

        return CompileXml(declaration->Node,declaration->Document->NamespaceManager,platform != TargetPlatform::Win32);
    }
};
public ref class UpgradeTemplateCompiler abstract sealed {
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ ns, bool big) {
        XmlattributesRoot(node,ns,"UpgradeTemplate");
        Native::XmlattributesInput input;
        input.attributes=XmlattributesRead(node,gcnew array<String^>{
            "DisplayName","Description","TypeDescription","AcquireHint","Type","BuildTime","BuildCost","ResearchSound",
            "ResearchCompleteEvaEvent","UnitSpecificSound","UpgradeFX","LocalPlayerGainsUpgradeEvaEvent",
            "AlliedPlayerGainsUpgradeEvaEvent","EnemyPlayerGainsUpgradeEvaEvent","LocalPlayerLosesUpgradeEvaEvent",
            "AlliedPlayerLosesUpgradeEvaEvent","EnemyPlayerLosesUpgradeEvaEvent","LocalPlayerProductionStartedEvaEvent",
            "LocalPlayerBuildOnHoldEvaEvent","LocalPlayerBuildCancelledEvaEvent","UseObjectTemplateForCostDiscount",
            "GroupName","GroupOrder","SkirmishAIHeuristic","Options","IconImage","WaypointQueueable"},true);
        for each(XmlNode^ c in XmlattributesChildren(node,gcnew array<String^>{"GameDependency"})) {

            input.hasDependency=true;
            input.dependency=XmlattributesRead(c,gcnew array<String^>{"RequiredModelConditionsAny","ForbiddenModelConditions","RequiredObjectStatusAny"},false);
            for each(XmlNode^ item in XmlattributesChildren(c,gcnew array<String^>{"RequiredObject","ForbiddenUpgrade","NeededUpgrade","ObjectFilter"})) {
                if(item->LocalName == "ObjectFilter") {

                    input.hasFilter=true;
                    input.filter=XmlattributesRead(item,gcnew array<String^>{"id","Rule","Relationship","Alignment","Include","Exclude","StatusBitFlags","StatusBitFlagsExclude"},false);
                    for each(XmlNode^ entry in XmlattributesChildren(item,gcnew array<String^>{"IncludeThing","ExcludeThing"})) {
                        XmlattributesRead(entry,gcnew array<String^>(0),false);

                        input.filterLists[entry->LocalName == "IncludeThing"?0:1].push_back(marshal_as<std::string>(entry->InnerText));
                    }
                    continue;
                }
                XmlattributesRead(item,gcnew array<String^>(0),false);

                int index=item->LocalName == "RequiredObject"?0:item->LocalName == "ForbiddenUpgrade"?1:2;
                input.dependencyLists[index].push_back(marshal_as<std::string>(item->InnerText));
            }
        }
        return XmlattributesBuffer(Native::CompileXmlattributesUpgradeTemplate(input,big));
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if(declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if(declaration->Handle->TypeId != 3786401627u) throw gcnew NotSupportedException("Expected UpgradeTemplate TypeId 3786401627");
        return CompileXml(declaration->Node,declaration->Document->NamespaceManager,platform != TargetPlatform::Win32);
    }
};

}
