#include <msclr/marshal_cppstd.h>
#include <stdexcept>
#include "XmlNodeAdapter.h"
#include "../Native/WeaponTemplate.h"
#include "../Native/EnumBitset.h"
#include "../Native/ScalarParsing.h"
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
    // BAB ValidateInstances annotates every named schema element, including
    // non-polymorphic helpers and the root. Only nugget dispatch consumes TypeId.
    static bool IsTypeAnnotation(XPathNavigator^ attribute) {
        return attribute->NamespaceURI->Length==0 && attribute->LocalName=="TypeId";
    }
    static Native::ObjectFilterInput ReadFilter(XPathNavigator^ node) {
        Native::ObjectFilterInput result;
        array<String^>^ allowed = { "id", "Rule", "Relationship", "Alignment", "Include", "Exclude", "StatusBitFlags", "StatusBitFlagsExclude" };
        auto attrs = node->Clone();
        if (attrs->MoveToFirstAttribute()) do {
            if (attrs->NamespaceURI == "http://www.w3.org/2000/xmlns/" || IsTypeAnnotation(attrs)) continue;
            if (attrs->NamespaceURI->Length != 0 || Array::IndexOf<String^>(allowed, attrs->LocalName) < 0)
                throw gcnew NotSupportedException("Unsupported ObjectFilter attribute: "+attrs->Name);
            result.attributes[marshal_as<std::string>(attrs->LocalName)] = marshal_as<std::string>(attrs->Value);
        } while (attrs->MoveToNextAttribute());
        auto children = node->SelectChildren(XPathNodeType::Element);
        while (children->MoveNext()) {
            auto child = children->Current;
            if (child->NamespaceURI != node->NamespaceURI || (child->LocalName != "IncludeThing" && child->LocalName != "ExcludeThing"))
                throw gcnew NotSupportedException("Unsupported ObjectFilter child: "+child->LocalName);
            auto attr = child->Clone();
            if (attr->MoveToFirstAttribute()) do {
                if (attr->NamespaceURI != "http://www.w3.org/2000/xmlns/" && !IsTypeAnnotation(attr)) throw gcnew NotSupportedException("ObjectFilter thing attributes are not supported.");
            } while (attr->MoveToNextAttribute());
            if (child->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("Nested ObjectFilter thing elements are not supported.");
            auto value = marshal_as<std::string>(child->Value);
            if (child->LocalName == "IncludeThing") result.includeThings.push_back(value); else result.excludeThings.push_back(value);
        }
        return result;
    }
    static std::string ReadUpgrade(XPathNavigator^ node) {
        auto attrs=node->Clone();
        if(attrs->MoveToFirstAttribute()) do {
            if(attrs->NamespaceURI != "http://www.w3.org/2000/xmlns/" && !IsTypeAnnotation(attrs)) throw gcnew NotSupportedException("Upgrade list element attributes are unsupported.");
        } while(attrs->MoveToNextAttribute());
        if(node->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("Nested upgrade elements are unsupported.");
        return marshal_as<std::string>(node->Value);
    }
    static Native::NuggetScalarInput ReadScalar(XPathNavigator^ node) {
        Native::NuggetScalarInput result;auto attrs=node->Clone();
        if(attrs->MoveToFirstAttribute()) do {
            if(attrs->NamespaceURI == "http://www.w3.org/2000/xmlns/" || IsTypeAnnotation(attrs)) continue;
            if(attrs->NamespaceURI->Length != 0 || attrs->LocalName != "Scalar") throw gcnew NotSupportedException("Unsupported DamageScalarDetails attribute: "+attrs->Name);
            result.scalar=marshal_as<std::string>(attrs->Value);
        } while(attrs->MoveToNextAttribute());
        auto children=node->SelectChildren(XPathNodeType::Element);
        while(children->MoveNext()) {
            auto child=children->Current;
            if(child->NamespaceURI != node->NamespaceURI) throw gcnew NotSupportedException("Namespaced DamageScalarDetails child is unsupported.");
            if(child->LocalName == "Filter") {
                auto filter=ReadFilter(child);if(!result.hasFilter) {result.filter=filter;result.hasFilter=true;}
            } else if(child->LocalName == "RequiredUpgrade") result.requiredUpgrades.push_back(ReadUpgrade(child));
            else throw gcnew NotSupportedException("Unsupported DamageScalarDetails child: "+child->Name);
        }
        return result;
    }
    static std::map<std::string,std::string> ReadProjectileAttributes(XPathNavigator^ node) {
        std::map<std::string,std::string> result;auto attrs=node->Clone();
        if(attrs->MoveToFirstAttribute()) do {
            if(attrs->NamespaceURI == "http://www.w3.org/2000/xmlns/" || IsTypeAnnotation(attrs)) continue;
            if(attrs->NamespaceURI->Length != 0) throw gcnew NotSupportedException("Namespaced projectile attributes are unsupported.");
            result[marshal_as<std::string>(attrs->LocalName)]=marshal_as<std::string>(attrs->Value);
        } while(attrs->MoveToNextAttribute());
        if(node->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("Nested projectile helper children are unsupported.");
        return result;
    }
    static Native::WeaponNuggetInput ReadNugget(XPathNavigator^ node) {
        Native::WeaponNuggetInput result;auto attrs=node->Clone();
        if(attrs->MoveToFirstAttribute()) do {
            if(attrs->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if(attrs->NamespaceURI->Length != 0) throw gcnew NotSupportedException("Namespaced nugget attributes are unsupported.");
            if(attrs->LocalName == "id") continue; // pipeline-only, not emitted
            if(attrs->LocalName == "TypeId") result.typeId=Native::ParseUnsigned(marshal_as<std::string>(attrs->Value));
            else result.attributes[marshal_as<std::string>(attrs->LocalName)]=marshal_as<std::string>(attrs->Value);
        } while(attrs->MoveToNextAttribute());
        auto children=node->SelectChildren(XPathNodeType::Element);
        while(children->MoveNext()) {
            auto child=children->Current;
            if(child->NamespaceURI != node->NamespaceURI) throw gcnew NotSupportedException("Namespaced nugget child is unsupported.");
            if(child->LocalName == "SpecialObjectFilter") {
                auto filter=ReadFilter(child);if(!result.hasSpecialFilter) {result.specialFilter=filter;result.hasSpecialFilter=true;}
            } else if(child->LocalName == "RequiredUpgrade") result.requiredUpgrades.push_back(ReadUpgrade(child));
            else if(child->LocalName == "ForbiddenUpgrade") result.forbiddenUpgrades.push_back(ReadUpgrade(child));
            else if(child->LocalName == "AttackOffset") {
                auto value=ReadProjectileAttributes(child);if(!result.hasAttackOffset) {result.attackOffset=value;result.hasAttackOffset=true;}
            } else if(child->LocalName == "SpecificBarrelOverride") {
                auto value=ReadUpgrade(child);if(!result.hasBarrel) {result.barrel=value;result.hasBarrel=true;}
            } else if(child->LocalName == "VeterancyProjectiles") {
                Native::VeteranProjectileInput value;value.attributes=ReadProjectileAttributes(child);result.veterans.push_back(value);
            }
            else if(child->LocalName == "DamageScalarDetails") result.damageScalars.push_back(ReadScalar(child));
            else if(child->LocalName == "SpawnTemplate") {
                auto value=ReadUpgrade(child);if(!result.hasSpawnTemplate) {result.spawnTemplate=value;result.hasSpawnTemplate=true;}
            } else if(child->LocalName == "Filter") {
                auto value=ReadFilter(child);if(!result.hasTargetFilter) {result.targetFilter=value;result.hasTargetFilter=true;}
            } else if(child->LocalName == "DamageObjectFilter") {
                auto value=ReadFilter(child);if(!result.hasDamageFilter) {result.damageFilter=value;result.hasDamageFilter=true;}
            } else if(child->LocalName == "KillObjectFilter") {
                auto value=ReadFilter(child);if(!result.hasKillFilter) {result.killFilter=value;result.hasKillFilter=true;}
            } else if(child->LocalName == "Color") {
                auto value=ReadProjectileAttributes(child);
                for(const auto& attr:value) if(attr.first!="R" && attr.first!="G" && attr.first!="B")
                    throw gcnew NotSupportedException("Unsupported RGBColor attribute.");
                if(!result.hasColor) {result.color=value;result.hasColor=true;}
            }
            else throw gcnew NotSupportedException("Unrecovered nugget child: "+child->Name);
        }
        return result;
    }
    static Native::WeaponScatterInput ReadScatter(XPathNavigator^ node) {
        Native::WeaponScatterInput result;
        auto attrs = node->Clone();
        if (attrs->MoveToFirstAttribute()) do {
            if (attrs->NamespaceURI == "http://www.w3.org/2000/xmlns/" || IsTypeAnnotation(attrs)) continue;
            if (attrs->NamespaceURI->Length != 0 || (attrs->LocalName != "Radius" && attrs->LocalName != "RequiredModelConditions"))
                throw gcnew NotSupportedException("Unsupported ScatterRadiusVsType attribute: "+attrs->Name);
            if (attrs->LocalName == "Radius") result.radius = marshal_as<std::string>(attrs->Value);
            else result.modelConditions = marshal_as<std::string>(attrs->Value);
        } while (attrs->MoveToNextAttribute());
        auto children = node->SelectChildren(XPathNodeType::Element);
        while (children->MoveNext()) {
            auto child = children->Current;
            if (child->NamespaceURI != node->NamespaceURI || child->LocalName != "Filter")
                throw gcnew NotSupportedException("Unsupported ScatterRadiusVsType child: "+child->Name);
            auto filter = ReadFilter(child);
            if (!result.hasFilter) { result.filter = filter; result.hasFilter = true; }
        }
        return result;
    }
public:
    static array<UInt32>^ MarshalModelCondition(String^ text, array<UInt32>^ initial) {
        if (text == nullptr) throw gcnew ArgumentNullException("text");
        if (initial == nullptr || initial->Length != 15) throw gcnew ArgumentException("ModelCondition requires fifteen words.", "initial");
        std::uint32_t words[15];
        for (int i = 0; i < 15; ++i) words[i] = initial[i];
        Native::MarshalModelCondition(marshal_as<std::string>(text), words);
        auto result = gcnew array<UInt32>(15);
        for (int i = 0; i < 15; ++i) result[i] = words[i];
        return result;
    }
    // Direct inline helper, including accumulation into a previously populated mask.
    static array<UInt32>^ MarshalObjectStatus(String^ text, array<UInt32>^ initial) {
        if (text == nullptr) throw gcnew ArgumentNullException("text");
        if (initial == nullptr || initial->Length != 7) throw gcnew ArgumentException("ObjectStatus requires seven words.", "initial");
        std::uint32_t words[7];
        for (int i = 0; i < 7; ++i) words[i] = initial[i];
        Native::MarshalObjectStatus(marshal_as<std::string>(text), words);
        auto result = gcnew array<UInt32>(7);
        for (int i = 0; i < 7; ++i) result[i] = words[i];
        return result;
    }
    // Partial WeaponTemplate entry point. Unsupported structures are rejected.
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        auto adapter = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
        auto children = adapter->Children(nullptr);
        array<String^>^ durationNames = { "PreAttackDelay", "FiringDuration", "CoolDownDelayBetweenShots", "ClipReloadTime" };
        array<String^>^ soundNames = { "OverrideVoiceAttackSound", "OverrideVoiceEnterStateAttackSound" };
        array<String^>^ hintAttributes = { "IsAntiGarrisonWeapon", "MaxSpeedOfTarget", "UseLongLockOnTimeCode", "UseAsWarheadForDamageCalculations" };
        std::map<std::string, Native::ObjectFilterInput> filters;
        std::vector<Native::WeaponScatterInput> scatter;
        std::vector<Native::WeaponNuggetInput> nuggets;bool readNuggets=false;
        while (children->MoveNext()) {
            auto child = children->Current;
            if(child->LocalName == "Nuggets" && child->NamespaceURI == node->NamespaceURI) {
                auto attrs=child->Clone();
                if(attrs->MoveToFirstAttribute()) do {
                    if(attrs->NamespaceURI != "http://www.w3.org/2000/xmlns/" && !IsTypeAnnotation(attrs)) throw gcnew NotSupportedException("Nuggets container attributes are unsupported.");
                } while(attrs->MoveToNextAttribute());
                std::vector<Native::WeaponNuggetInput> entries;
                auto list=child->SelectChildren(XPathNodeType::Element);
                while(list->MoveNext()) {
                    if(list->Current->NamespaceURI != node->NamespaceURI) throw gcnew NotSupportedException("Namespaced nugget elements are unsupported.");
                    entries.push_back(ReadNugget(list->Current));
                }
                if(!readNuggets) {nuggets=entries;readNuggets=true;}
                continue;
            }
            if (child->LocalName == "ScatterRadiusVsType" && child->NamespaceURI == node->NamespaceURI) {
                scatter.push_back(ReadScatter(child)); continue;
            }
            bool filter = child->LocalName == "SurpriseAttackObjectFilter" || child->LocalName == "CombinedAttackObjectFilter" || child->LocalName == "HitStoredObjectFilter";
            if (filter && child->NamespaceURI == node->NamespaceURI) {
                auto input = ReadFilter(child); auto name = marshal_as<std::string>(child->LocalName);
                if (!filters.count(name)) filters[name] = input;
                continue;
            }
            bool duration = Array::IndexOf<String^>(durationNames, child->LocalName) >= 0;
            bool sound = Array::IndexOf<String^>(soundNames, child->LocalName) >= 0;
            bool hint = child->LocalName == "WeaponAiHintInfo";
            bool modifier = child->LocalName == "IncompatibleAttributeModifier";
            if (child->NamespaceURI != node->NamespaceURI || (!duration && !sound && !hint && !modifier))
                throw gcnew NotSupportedException("WeaponTemplate child structure is not reconstructed: " + child->LocalName);
            auto attributes = child->Clone();
            if (attributes->MoveToFirstAttribute()) do {
                if (attributes->NamespaceURI == "http://www.w3.org/2000/xmlns/" || IsTypeAnnotation(attributes)) continue;
                bool known = !modifier && (duration ? (attributes->LocalName == "MinSeconds" || attributes->LocalName == "MaxSeconds") :
                    sound ? (attributes->LocalName == "Sound" || attributes->LocalName == "EvaEvent") : Array::IndexOf<String^>(hintAttributes, attributes->LocalName) >= 0);
                if (attributes->NamespaceURI->Length != 0 || !known)
                    throw gcnew NotSupportedException("Unsupported WeaponTemplate child attribute: " + attributes->Name);
            } while (attributes->MoveToNextAttribute());
            if (child->SelectChildren(XPathNodeType::Element)->MoveNext())
                throw gcnew NotSupportedException("WeaponTemplate optional block does not support nested elements.");
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
        std::map<std::string, Native::WeaponOptionalInput> optionalBlocks;
        array<String^>^ blockNames = { "OverrideVoiceAttackSound", "OverrideVoiceEnterStateAttackSound", "WeaponAiHintInfo" };
        for each (String^ name in blockNames) {
            auto child = adapter->Child(name, nullptr);
            if (child == nullptr) continue;
            Native::WeaponOptionalInput input;
            array<String^>^ names = name == "WeaponAiHintInfo" ? hintAttributes : gcnew array<String^> { "Sound", "EvaEvent" };
            for each (String^ attrName in names) {
                String^ value = child->Attribute(attrName, nullptr);
                if (value != nullptr) input[marshal_as<std::string>(attrName)] = marshal_as<std::string>(value);
            }
            optionalBlocks[marshal_as<std::string>(name)] = input;
        }
        std::vector<std::string> incompatibleModifiers;
        auto modifierList = adapter->Children("IncompatibleAttributeModifier");
        while (modifierList->MoveNext()) incompatibleModifiers.push_back(marshal_as<std::string>(modifierList->Current->Value));
        std::map<std::string, std::string> attributes;
        for each (XmlAttribute^ attribute in node->Attributes) {
            if (attribute->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attribute->NamespaceURI->Length != 0) throw gcnew NotSupportedException("Namespaced WeaponTemplate attributes are not supported by this component.");
            if (attribute->Name == "id" || attribute->Name == "TypeId") continue;
            attributes[marshal_as<std::string>(attribute->Name)] = marshal_as<std::string>(attribute->Value);
        }
        try {
            auto chunk = Native::CompileWeapon(attributes, durations, optionalBlocks, incompatibleModifiers, filters, scatter, nuggets, bigEndian);
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



