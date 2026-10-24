// Handoff snapshot: contains complete adapter context and particle root/module draft.
// Extract FXParticleSystemTemplateCompiler and its helpers into the new component.
// Not part of the active FXList build; enum mappings still need namespaced PE exports.
#if 0
#include <msclr/marshal_cppstd.h>
#include "XmlNodeAdapter.h"
#include "../Native/Xmleffects.h"
#using <BinaryAssetBuilder.Core.dll>
#using <System.Core.dll>
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

    static void Children(XmlNode^ node, array<String^>^ allowed, bool repeated) {
        System::Collections::Generic::HashSet<String^>^ seen = gcnew System::Collections::Generic::HashSet<String^>();
        for each (XmlNode^ child in node->ChildNodes) {
            if (child->NodeType != XmlNodeType::Element) {
                if ((child->NodeType == XmlNodeType::Text || child->NodeType == XmlNodeType::CDATA) &&
                    !String::IsNullOrWhiteSpace(child->Value)) throw gcnew NotSupportedException("Unsupported effects text");
                continue;
            }
            bool found = false;
            if (child->NamespaceURI == node->NamespaceURI)
                for each (String^ name in allowed) if (child->LocalName == name) { found = true; break; }
            if (!found || (!repeated && !seen->Add(child->LocalName)))
                throw gcnew NotSupportedException("Unsupported or duplicate effects child: " + child->Name);
        }
    }
    static XmlNode^ Child(XmlNode^ node, String^ name) {
        for each(XmlNode^ child in node->ChildNodes)
            if(child->NodeType == XmlNodeType::Element && child->LocalName == name && child->NamespaceURI == node->NamespaceURI) return child;
        return nullptr;
    }
    static std::string Attr(XmlNode^ node, String^ name, String^ fallback) {
        XmlNodeAdapter^ a = gcnew XmlNodeAdapter(node->CreateNavigator(), nullptr);
        return marshal_as<std::string>(a->Attribute(name, fallback));
    }
    static void Random(XmlNode^ node, Native::XmleffectsRandomInput& input) {
        if(node == nullptr) return;
        Attributes(node, gcnew array<String^>{"Type","Low","High"});
        Empty(node);
        input.present = true;
        input.type = Attr(node,"Type","UNIFORM");
        input.low = Attr(node,"Low","");
        input.high = Attr(node,"High","");
    }

    static void Field(XmlNode^ node, String^ name, String^ fallback, unsigned int offset,
        Native::XmleffectsFieldKind kind, String^ enumType, Native::XmleffectsNuggetInput& input) {
        XmlNodeAdapter^ a=gcnew XmlNodeAdapter(node->CreateNavigator(),nullptr);
        String^ value=a->Attribute(name,fallback);
        if(value == nullptr) return;
        Native::XmleffectsFieldInput field;
        field.offset=offset; field.kind=kind; field.text=marshal_as<std::string>(value);
        if(enumType != nullptr) field.enumType=marshal_as<std::string>(enumType);
        input.fields.push_back(field);
    }
    static void Vector(XmlNode^ owner, String^ name, unsigned int offset, bool rgb, Native::XmleffectsNuggetInput& input) {
        XmlNode^ child=Child(owner,name);
        if(child == nullptr) return;
        array<String^>^ axes=rgb ? gcnew array<String^>{"R","G","B"} : gcnew array<String^>{"x","y","z"};
        Attributes(child,axes); Empty(child);
        for(int i=0;i<3;++i) Field(child,axes[i],rgb ? nullptr : "0.0",offset+4*i,Native::EffectFloat,nullptr,input);
    }
    static void Nugget(XmlNode^ node, Native::XmleffectsNuggetInput& input) {
        input.typeId=Native::ParseUnsigned(Attr(node,"TypeId","0"));
        System::Collections::Generic::List<String^>^ attrs=gcnew System::Collections::Generic::List<String^>(gcnew array<String^>{"TypeId","StopIfPlayed","Weather","OnlyIfOnLand","PlayIfSourceIsStealthed"});
        System::Collections::Generic::List<String^>^ children=gcnew System::Collections::Generic::List<String^>(gcnew array<String^>{"SourceMustNotHaveBeenDisabledThisFrameByType"});
        switch(input.typeId) {
        case 503892219u:
            input.size=48; attrs->Add("Value");
            Field(node,"Value",nullptr,44,Native::EffectReference,nullptr,input); break;
        case 638391350u:
            input.size=48; attrs->Add("Event");
            Field(node,"Event",nullptr,44,Native::EffectReference,nullptr,input); break;
        case 4011332978u:
            input.size=48; attrs->Add("Type");
            Field(node,"Type",nullptr,44,Native::EffectEnum,"ViewShakeType",input); break;
        case 2424288100u:
            input.size=56; attrs->AddRange(gcnew array<String^>{"EvaEventOwner","EvaEventAlly","EvaEventEnemy"});
            Field(node,"EvaEventOwner","",44,Native::EffectReference,nullptr,input);
            Field(node,"EvaEventAlly","",48,Native::EffectReference,nullptr,input);
            Field(node,"EvaEventEnemy","",52,Native::EffectReference,nullptr,input); break;
        case 4234415617u:
            input.size=56; attrs->AddRange(gcnew array<String^>{"FX","Bone"});
            Field(node,"FX",nullptr,44,Native::EffectReference,nullptr,input);
            Field(node,"Bone",nullptr,48,Native::EffectString,nullptr,input); break;
        case 2263385183u:
            input.size=56; attrs->AddRange(gcnew array<String^>{"Model","RandomlyRotate","ExpireTimer"});
            Field(node,"Model",nullptr,44,Native::EffectReference,nullptr,input);
            Field(node,"RandomlyRotate","false",52,Native::EffectBoolean,nullptr,input);
            Field(node,"ExpireTimer","8s",48,Native::EffectTime,nullptr,input); break;
        case 904475169u:
            input.size=64; attrs->AddRange(gcnew array<String^>{"LaserTemplate","LaserBackwards"}); children->Add("Offset");
            Field(node,"LaserTemplate",nullptr,44,Native::EffectHash,nullptr,input);
            Field(node,"LaserBackwards","false",60,Native::EffectBoolean,nullptr,input);
            Vector(node,"Offset",48,false,input); break;
        case 2386148390u:
            input.size=72; attrs->Add("Thing"); children->AddRange(gcnew array<String^>{"PrimaryOffset","SecondaryOffset"});
            Field(node,"Thing",nullptr,44,Native::EffectHash,nullptr,input);
            Vector(node,"PrimaryOffset",48,false,input); Vector(node,"SecondaryOffset",60,false,input); break;
        case 4293352609u:
            input.size=72; attrs->AddRange(gcnew array<String^>{"Radius","RadiusAsPercentOfObjectSize","IncreaseTime","DecreaseTime"}); children->Add("Color");
            Field(node,"Radius",nullptr,44,Native::EffectFloat,nullptr,input);
            Field(node,"RadiusAsPercentOfObjectSize",nullptr,48,Native::EffectPercentage,nullptr,input);
            Field(node,"IncreaseTime",nullptr,52,Native::EffectUnsigned,nullptr,input);
            Field(node,"DecreaseTime",nullptr,56,Native::EffectUnsigned,nullptr,input);
            Vector(node,"Color",60,true,input); break;
        case 2106748504u:
            input.size=76; attrs->AddRange(gcnew array<String^>{"PreColorTime","PostColorTime","SustainedColorTime","Frequency","Amplitude"}); children->Add("Color");
            Field(node,"PreColorTime","2s",44,Native::EffectTime,nullptr,input);
            Field(node,"PostColorTime","2s",48,Native::EffectTime,nullptr,input);
            Field(node,"SustainedColorTime","1s",52,Native::EffectTime,nullptr,input);
            Field(node,"Frequency","1.0",56,Native::EffectFloat,nullptr,input);
            Field(node,"Amplitude","1.0",60,Native::EffectFloat,nullptr,input);
            Vector(node,"Color",64,true,input); break;
        case 3768725989u:
            input.size=56; attrs->AddRange(gcnew array<String^>{"Radius","Duration","Amplitude"});
            Field(node,"Radius",nullptr,44,Native::EffectFloat,nullptr,input);
            Field(node,"Duration",nullptr,48,Native::EffectTime,nullptr,input);
            Field(node,"Amplitude",nullptr,52,Native::EffectAngle,nullptr,input); break;
        case 138194854u:
            input.size=56; attrs->AddRange(gcnew array<String^>{"Type","Radius"}); children->Add("RandomRange");
            Field(node,"Type","RANDOM",44,Native::EffectEnum,"TerrainScorchType",input);
            Field(node,"Radius",nullptr,48,Native::EffectFloat,nullptr,input);
            if(Child(node,"RandomRange") != nullptr) {
                XmlNode^ range=Child(node,"RandomRange"); Attributes(range,gcnew array<String^>{"Low","High"}); Empty(range);
                input.hasRange=true; input.rangeLow=Attr(range,"Low","SCORCH_1"); input.rangeHigh=Attr(range,"High","SCORCH_4");
            } break;
        default: throw gcnew NotSupportedException("Unrecovered FXNugget TypeId");
        }
        Attributes(node,attrs->ToArray());
        Children(node,children->ToArray(),true);
        // Only the disabled enum list is repeatable. Other scalar children must be unique.
        for each(String^ name in children) if(name != "SourceMustNotHaveBeenDisabledThisFrameByType") {
            int count=0;
            for each(XmlNode^ c in node->ChildNodes) if(c->NodeType==XmlNodeType::Element && c->LocalName==name) ++count;
            if(count>1) throw gcnew NotSupportedException("Duplicate FXNugget child");
        }
        Field(node,"StopIfPlayed",nullptr,40,Native::EffectBoolean,nullptr,input);
        Field(node,"Weather","INVALID",20,Native::EffectEnum,"WeatherType",input);
        Field(node,"OnlyIfOnLand","false",41,Native::EffectBoolean,nullptr,input);
        Field(node,"PlayIfSourceIsStealthed","false",42,Native::EffectBoolean,nullptr,input);
        for each(XmlNode^ child in node->ChildNodes) if(child->NodeType==XmlNodeType::Element && child->LocalName=="SourceMustNotHaveBeenDisabledThisFrameByType") {
            Attributes(child,gcnew array<String^>(0));
            for each(XmlNode^ nested in child->ChildNodes) if(nested->NodeType==XmlNodeType::Element)
                throw gcnew NotSupportedException("Nested disabled enum");
            input.disabled.push_back(marshal_as<std::string>(child->InnerText));
        }
    }

    static void ModuleRandom(XmlNode^ node, String^ name, unsigned int offset, Native::XmleffectsModuleInput& input) {
        XmlNode^ child=Child(node,name);
        if(child == nullptr) return;
        Native::XmleffectsModuleRandom field; field.offset=offset; Random(child,field.input); input.random.push_back(field);
    }
    static void Module(XmlNode^ container, bool velocity, Native::XmleffectsModuleInput& input) {
        if(container==nullptr) return;
        Attributes(container,gcnew array<String^>(0));
        XmlNode^ node=nullptr;
        for each(XmlNode^ child in container->ChildNodes) {
            if(child->NodeType==XmlNodeType::Element) {
                if(child->NamespaceURI!=container->NamespaceURI || node!=nullptr) throw gcnew NotSupportedException("Multiple/foreign particle modules");
                node=child;
            } else if((child->NodeType==XmlNodeType::Text || child->NodeType==XmlNodeType::CDATA) && !String::IsNullOrWhiteSpace(child->Value))
                throw gcnew NotSupportedException("Particle module text");
        }
        if(node==nullptr) return;
        input.present=true;
        input.typeId=Native::ParseUnsigned(Attr(node,"TypeId","0"));
        Native::XmleffectsNuggetInput values;
        if(velocity) {
            switch(input.typeId) {
            case 0x8bb38534u: case 0xb7b54fc2u:
                input.size=16; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Speed"},false);
                ModuleRandom(node,"Speed",4,input); break;
            case 0xcf2fd6dfu:
                input.size=28; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Speed","OtherSpeed"},false);
                ModuleRandom(node,"Speed",4,input); ModuleRandom(node,"OtherSpeed",16,input); break;
            case 0xf1ec3b2eu:
                input.size=28; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Radial","Normal"},false);
                ModuleRandom(node,"Radial",4,input); ModuleRandom(node,"Normal",16,input); break;
            case 0xecf79bc7u:
                input.size=68; Attributes(node,gcnew array<String^>{"TypeId","Shape"}); Children(node,gcnew array<String^>{"Velocity","Amplitude","InitialRadius","LineWidth","LineAngle"},false);
                Field(node,"Shape","CIRCLE",4,Native::EffectEnum,"FXParticleSystem_WavefrontShape",values);
                ModuleRandom(node,"Velocity",8,input); ModuleRandom(node,"Amplitude",20,input); ModuleRandom(node,"InitialRadius",32,input); ModuleRandom(node,"LineWidth",44,input); ModuleRandom(node,"LineAngle",56,input); break;
            case 0xe7528102u:
                input.size=40; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Position"},false);
                if(Child(node,"Position")!=nullptr) {
                    XmlNode^ position=Child(node,"Position"); Attributes(position,gcnew array<String^>(0)); Children(position,gcnew array<String^>{"x","y","z"},false);
                    ModuleRandom(position,"x",4,input); ModuleRandom(position,"y",16,input); ModuleRandom(position,"z",28,input);
                } break;
            default: throw gcnew NotSupportedException("Unrecovered particle Velocity TypeId");
            }
        } else {
            array<String^>^ attrs=nullptr;
            array<String^>^ children=nullptr;
            switch(input.typeId) {
            case 0xd32fc96bu:
                input.size=8; attrs=gcnew array<String^>{"TypeId","IsHollow"}; children=gcnew array<String^>(0); break;
            case 0xedf83b69u:
                input.size=12; attrs=gcnew array<String^>{"TypeId","IsHollow","Radius"}; children=gcnew array<String^>(0);
                Field(node,"Radius",nullptr,8,Native::EffectFloat,nullptr,values); break;
            case 0x4a0b96f4u:
                input.size=20; attrs=gcnew array<String^>{"TypeId","IsHollow"}; children=gcnew array<String^>{"HalfSize"};
                Vector(node,"HalfSize",8,false,values); break;
            case 0x5c8a6f16u:
                input.size=32; attrs=gcnew array<String^>{"TypeId","IsHollow","Radius","RadiusRate","Length"}; children=gcnew array<String^>{"Offset"};
                Field(node,"Radius",nullptr,8,Native::EffectFloat,nullptr,values); Field(node,"RadiusRate",nullptr,12,Native::EffectFloat,nullptr,values); Field(node,"Length",nullptr,16,Native::EffectFloat,nullptr,values);
                Vector(node,"Offset",20,false,values); break;
            case 0x0da35f9cu:
                input.size=32; attrs=gcnew array<String^>{"TypeId","IsHollow"}; children=gcnew array<String^>{"StartPoint","EndPoint"};
                Vector(node,"StartPoint",8,false,values); Vector(node,"EndPoint",20,false,values); break;
            default: throw gcnew NotSupportedException("Unrecovered particle Volume TypeId");
            }
            Attributes(node,attrs); Children(node,children,false);
            Field(node,"IsHollow",nullptr,4,Native::EffectBoolean,nullptr,values);
        }
        input.fields=values.fields;
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
                // Each child is dispatched by its schema-preprocessed numeric TypeId.
                for each(XmlNode^ nugget in child->ChildNodes) {
                    if(nugget->NodeType==XmlNodeType::Element) {
                        if(nugget->NamespaceURI!=node->NamespaceURI) throw gcnew NotSupportedException("Foreign FXNugget namespace");
                    } else if((nugget->NodeType==XmlNodeType::Text || nugget->NodeType==XmlNodeType::CDATA) && !String::IsNullOrWhiteSpace(nugget->Value)) throw gcnew NotSupportedException("FXNugget list text");
                }
            } else if ((child->NodeType == XmlNodeType::Text || child->NodeType == XmlNodeType::CDATA) &&
                       !String::IsNullOrWhiteSpace(child->Value))
                throw gcnew NotSupportedException("Unsupported FXList text");
        }
        XmlNodeAdapter^ adapter = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
        Native::XmleffectsFXListInput input;
        XmlNode^ listNode = XmleffectsXml::Child(node,"NuggetList");
        if(listNode != nullptr) for each(XmlNode^ nugget in listNode->ChildNodes) if(nugget->NodeType==XmlNodeType::Element) {
            Native::XmleffectsNuggetInput item; XmleffectsXml::Nugget(nugget,item); input.nuggets.push_back(item);
        }
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
        if(namespaces == nullptr) throw gcnew ArgumentNullException("namespaces");
        XmleffectsXml::Attributes(node, gcnew array<String^>{"id","Priority","Shader","Type","ParticleTexture","Drawable","SlaveSystem","PerParticleAttachedSystem","SystemLifetime","SortLevel","EmitterSound","IsOneShot","IsGroundAligned","IsEmitAboveGroundOnly","IsParticleUpTowardsEmitter","UseMaximumHeight"});
        XmleffectsXml::Children(node, gcnew array<String^>{"SlavePosOffset","Lifetime","Size","StartSizeRate","BurstDelay","BurstCount","InitialDelay","Alphas","Colors","Wind","Volume","Velocity","Event"}, false);
        Native::XmleffectsParticleInput input;
        input.priority = XmleffectsXml::Attr(node,"Priority","ULTRA_HIGH_ONLY");
        input.shader = XmleffectsXml::Attr(node,"Shader","ADDITIVE");
        input.type = XmleffectsXml::Attr(node,"Type","PARTICLE");
        array<String^>^ refs = {"ParticleTexture","Drawable","EmitterSound"};
        for(int i=0;i<3;++i) input.references[i] = XmleffectsXml::Attr(node,refs[i],"");
        input.typedIds[0] = XmleffectsXml::Attr(node,"SlaveSystem","");
        input.typedIds[1] = XmleffectsXml::Attr(node,"PerParticleAttachedSystem","");
        input.lifetime = XmleffectsXml::Attr(node,"SystemLifetime","0");
        input.sortLevel = XmleffectsXml::Attr(node,"SortLevel","0");
        array<String^>^ flags = {"IsOneShot","IsGroundAligned","IsEmitAboveGroundOnly","IsParticleUpTowardsEmitter","UseMaximumHeight"};
        for(int i=0;i<5;++i) input.flags[i] = XmleffectsXml::Attr(node,flags[i],"false");
        XmlNode^ offset = XmleffectsXml::Child(node,"SlavePosOffset");
        if(offset != nullptr) {
            XmleffectsXml::Attributes(offset,gcnew array<String^>{"x","y","z"}); XmleffectsXml::Empty(offset);
            array<String^>^ axes = {"x","y","z"};
            for(int i=0;i<3;++i) input.offset[i] = XmleffectsXml::Attr(offset,axes[i],"0.0");
        }
        array<String^>^ randomNames = {"Lifetime","Size","StartSizeRate","BurstDelay","BurstCount","InitialDelay"};
        for(int i=0;i<6;++i) XmleffectsXml::Random(XmleffectsXml::Child(node,randomNames[i]),input.random[i]);
        XmlNode^ alphas = XmleffectsXml::Child(node,"Alphas");
        if(alphas != nullptr) {
            input.hasAlphas = true;
            XmleffectsXml::Attributes(alphas,gcnew array<String^>(0));
            XmleffectsXml::Children(alphas,gcnew array<String^>{"Alpha"},true);
            for each(XmlNode^ alpha in alphas->ChildNodes) if(alpha->NodeType == XmlNodeType::Element) {
                XmleffectsXml::Attributes(alpha,gcnew array<String^>{"RelativeAge","Frame","Type","Low","High"});
                XmleffectsXml::Empty(alpha);
                Native::XmleffectsAlphaInput item;
                item.value.present = true;
                item.value.type = XmleffectsXml::Attr(alpha,"Type","UNIFORM");
                item.value.low = XmleffectsXml::Attr(alpha,"Low","");
                item.value.high = XmleffectsXml::Attr(alpha,"High","");
                item.relativeAge = XmleffectsXml::Attr(alpha,"RelativeAge","");
                item.frame = XmleffectsXml::Attr(alpha,"Frame","1000");
                input.alphas.push_back(item);
            }
        }
        XmlNode^ colors = XmleffectsXml::Child(node,"Colors");
        if(colors != nullptr) {
            input.hasColors = true;
            XmleffectsXml::Attributes(colors,gcnew array<String^>{"UseHouseColor","HouseColorSaturation"});
            XmleffectsXml::Children(colors,gcnew array<String^>{"Color","ColorScale"},true);
            input.houseColor = XmleffectsXml::Attr(colors,"UseHouseColor","false");
            input.saturation = XmleffectsXml::Attr(colors,"HouseColorSaturation","1.0");
            bool scaleSeen = false;
            for each(XmlNode^ color in colors->ChildNodes) if(color->NodeType == XmlNodeType::Element) {
                if(color->LocalName == "ColorScale") {
                    if(scaleSeen) throw gcnew NotSupportedException("Duplicate ColorScale");
                    scaleSeen = true; XmleffectsXml::Random(color,input.colorScale); continue;
                }
                XmleffectsXml::Attributes(color,gcnew array<String^>{"RelativeAge","Frame"});
                XmleffectsXml::Children(color,gcnew array<String^>{"Color"},false);
                Native::XmleffectsColorInput item;
                item.relativeAge = XmleffectsXml::Attr(color,"RelativeAge","");
                item.frame = XmleffectsXml::Attr(color,"Frame","1000");
                XmlNode^ rgb = XmleffectsXml::Child(color,"Color");
                if(rgb != nullptr) {
                    XmleffectsXml::Attributes(rgb,gcnew array<String^>{"r","g","b"}); XmleffectsXml::Empty(rgb);
                    array<String^>^ channels={"r","g","b"};
                    for(int i=0;i<3;++i) item.rgb[i]=XmleffectsXml::Attr(rgb,channels[i],"");
                }
                input.colors.push_back(item);
            }
        }
        XmlNode^ wind = XmleffectsXml::Child(node,"Wind");
        if(wind != nullptr) {
            input.hasWind = true;
            array<String^>^ names={"Motion","Strength","FullStrengthDist","ZeroStrengthDist","AngleChangeMin","AngleChangeMax","PingPongStartAngleMin","PingPongStartAngleMax","PingPongEndAngleMin","PingPongEndAngleMax","TurbulenceAmplitude","TurbulenceFrequency"};
            array<String^>^ defaults={"NOT_USED","2.0","75.0","200.0",".15",".45","0.0","0.7853981633974483","5.4977871437821381","6.2831853071795864","0","0"};
            XmleffectsXml::Attributes(wind,names); XmleffectsXml::Empty(wind);
            for(int i=0;i<12;++i) input.wind[i]=XmleffectsXml::Attr(wind,names[i],defaults[i]);
        }
        XmlNode^ events = XmleffectsXml::Child(node,"Event");
        if(events != nullptr) { XmleffectsXml::Attributes(events,gcnew array<String^>(0)); XmleffectsXml::Empty(events); }
        XmleffectsXml::Module(XmleffectsXml::Child(node,"Volume"),false,input.modules[0]);
        XmleffectsXml::Module(XmleffectsXml::Child(node,"Velocity"),true,input.modules[1]);
        return XmleffectsXml::Buffer(Native::CompileXmleffectsParticle(input,bigEndian));
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 686292351u)
            throw gcnew NotSupportedException("Expected FXParticleSystemTemplate TypeId");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
}

#endif
