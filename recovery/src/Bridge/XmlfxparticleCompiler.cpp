#include <msclr/marshal_cppstd.h>
#include "XmlNodeAdapter.h"
#include "../Native/Xmlfxparticle.h"
#using <BinaryAssetBuilder.Core.dll>
#using <System.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;
namespace Recovered {
ref class XmlfxparticleXml abstract sealed {
public:
    static void Attributes(XmlNode^ node, array<String^>^ allowed) {
        for each (XmlAttribute^ attr in node->Attributes) {
            if (attr->NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            bool found = false;
            if (attr->NamespaceURI->Length == 0)
                for each (String^ name in allowed) if (attr->LocalName == name) { found = true; break; }
            if (!found) throw gcnew NotSupportedException("Unsupported particle attribute: " + attr->Name);
        }
    }
    static void Root(XmlNode^ node, String^ name) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        if (node->NodeType != XmlNodeType::Element || node->LocalName != name ||
            node->NamespaceURI != "uri:ea.com:eala:asset")
            throw gcnew NotSupportedException("Expected particle root " + name);
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
                    !String::IsNullOrWhiteSpace(child->Value)) throw gcnew NotSupportedException("Unsupported particle text");
                continue;
            }
            bool found = false;
            if (child->NamespaceURI == node->NamespaceURI)
                for each (String^ name in allowed) if (child->LocalName == name) { found = true; break; }
            if (!found || (!repeated && !seen->Add(child->LocalName)))
                throw gcnew NotSupportedException("Unsupported or duplicate particle child: " + child->Name);
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
    static void Random(XmlNode^ node, Native::XmlfxparticleRandomInput& input) {
        if(node == nullptr) return;
        Attributes(node, gcnew array<String^>{"Type","Low","High"});
        Empty(node);
        input.present = true;
        input.type = Attr(node,"Type","UNIFORM");
        input.low = Attr(node,"Low","");
        input.high = Attr(node,"High","");
    }

    static void Field(XmlNode^ node, String^ name, String^ fallback, unsigned int offset,
        Native::XmlfxparticleFieldKind kind, String^ enumType, Native::XmlfxparticleNuggetInput& input) {
        XmlNodeAdapter^ a=gcnew XmlNodeAdapter(node->CreateNavigator(),nullptr);
        String^ value=a->Attribute(name,fallback);
        if(value == nullptr) return;
        Native::XmlfxparticleFieldInput field;
        field.offset=offset; field.kind=kind; field.text=marshal_as<std::string>(value);
        if(enumType != nullptr) field.enumType=marshal_as<std::string>(enumType);
        input.fields.push_back(field);
    }
    static void Vector(XmlNode^ owner, String^ name, unsigned int offset, bool rgb, Native::XmlfxparticleNuggetInput& input) {
        XmlNode^ child=Child(owner,name);
        if(child == nullptr) return;
        array<String^>^ axes=rgb ? gcnew array<String^>{"R","G","B"} : gcnew array<String^>{"x","y","z"};
        Attributes(child,axes); Empty(child);
        String^ fallback = "0.0"; if(rgb) fallback = nullptr;
        for(int i=0;i<3;++i) Field(child,axes[i],fallback,offset+4*i,Native::ParticleFloat,nullptr,input);
    }
    static void ModuleRandom(XmlNode^ node, String^ name, unsigned int offset, Native::XmlfxparticleModuleInput& input) {
        XmlNode^ child=Child(node,name);
        if(child == nullptr) return;
        Native::XmlfxparticleModuleRandom field; field.offset=offset; Random(child,field.input); input.random.push_back(field);
    }
    static void Module(XmlNode^ container, Native::XmlfxparticleModuleInput& input) {
        if(container==nullptr) return;
        Attributes(container,gcnew array<String^>(0));
        XmlNode^ node=nullptr;
        for each(XmlNode^ child in container->ChildNodes) {
            if(child->NodeType==XmlNodeType::Element) {
                if(child->NamespaceURI!=container->NamespaceURI || node!=nullptr) throw gcnew NotSupportedException("Multiple/foreign particle modules");
                node=child;
            } else if((child->NodeType==XmlNodeType::Text || child->NodeType==XmlNodeType::CDATA) && !String::IsNullOrWhiteSpace(child->Value)) throw gcnew NotSupportedException("Module text");
        }
        if(node==nullptr) return;
        input.present=true; input.typeId=Native::ParseUnsigned(Attr(node,"TypeId","0"));
        Native::XmlfxparticleNuggetInput values;
        switch(input.typeId) {
        case 877888681u: { // FXParticlePhysicsBase
            input.size=4; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{},false);
            break;
        }
        case 3201797789u: { // FXParticleDrawBase
            input.size=4; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{},false);
            break;
        }
        case 3809157760u: { // FXParticleDrawStreak
            input.size=4; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{},false);
            break;
        }
        case 2268785311u: { // FXParticleEmissionVelocityBase
            input.size=4; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{},false);
            break;
        }
        case 1903056374u: { // FXParticleUpdateBase
            input.size=4; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{},false);
            break;
        }
        case 612354849u: { // FXParticleEmissionVolumeBase
            input.size=8; Attributes(node,gcnew array<String^>{"TypeId","IsHollow"}); Children(node,gcnew array<String^>{},false);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 3543124331u: { // FXParticleEmissionVolumePoint
            input.size=8; Attributes(node,gcnew array<String^>{"TypeId","IsHollow"}); Children(node,gcnew array<String^>{},false);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 1423381183u: { // FXParticleEmissionVolumeLightning
            input.size=8; Attributes(node,gcnew array<String^>{"TypeId","IsHollow"}); Children(node,gcnew array<String^>{},false);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 1373300932u: { // FXParticleDrawLightSource
            input.size=8; Attributes(node,gcnew array<String^>{"TypeId","MaximumIntensityRadiusRatio"}); Children(node,gcnew array<String^>{},false);
            Field(node,"MaximumIntensityRadiusRatio","0.0",4,Native::ParticleFloat,nullptr,values);
            break;
        }
        case 1865622437u: { // FXParticleDrawTrail
            input.size=24; Attributes(node,gcnew array<String^>{"TypeId","TrailLife","UTile","VTile","UScrollRate","VScrollRate"}); Children(node,gcnew array<String^>{},false);
            Field(node,"TrailLife","30",4,Native::ParticleSigned,nullptr,values);
            Field(node,"UTile","1.0",8,Native::ParticleFloat,nullptr,values);
            Field(node,"VTile","1.0",12,Native::ParticleFloat,nullptr,values);
            Field(node,"UScrollRate","0.0",16,Native::ParticleFloat,nullptr,values);
            Field(node,"VScrollRate","0.0",20,Native::ParticleFloat,nullptr,values);
            break;
        }
        case 2381754661u: { // FXParticleDrawSwarm
            input.size=24; Attributes(node,gcnew array<String^>{"TypeId","OpaqueSpeed","TransparentSpeed","SpeedStretchAmount","AttractStrength","EnvironmentTexture"}); Children(node,gcnew array<String^>{},false);
            Field(node,"OpaqueSpeed","0",4,Native::ParticleFloat,nullptr,values);
            Field(node,"TransparentSpeed","100.0",8,Native::ParticleFloat,nullptr,values);
            Field(node,"SpeedStretchAmount","1.0",12,Native::ParticleFloat,nullptr,values);
            Field(node,"AttractStrength","0.0",16,Native::ParticleFloat,nullptr,values);
            Field(node,"EnvironmentTexture",nullptr,20,Native::ParticleReference,nullptr,values);
            break;
        }
        case 3110487039u: { // FXParticleDefaultPhysics
            input.size=36; Attributes(node,gcnew array<String^>{"TypeId","Gravity","Swirly","ParticlesAttachToBone"}); Children(node,gcnew array<String^>{"VelocityDamping","DriftVelocity"},false);
            Field(node,"Gravity",nullptr,4,Native::ParticleFloat,nullptr,values);
            Field(node,"Swirly",nullptr,32,Native::ParticleBoolean,nullptr,values);
            Field(node,"ParticlesAttachToBone",nullptr,33,Native::ParticleBoolean,nullptr,values);
            ModuleRandom(node,"VelocityDamping",8,input);
            Vector(node,"DriftVelocity",20,false,values);
            break;
        }
        case 1976375475u: { // FXParticleSwarmPhysics
            input.size=8; Attributes(node,gcnew array<String^>{"TypeId","AttractStrength"}); Children(node,gcnew array<String^>{},false);
            Field(node,"AttractStrength","0.0",4,Native::ParticleFloat,nullptr,values);
            break;
        }
        case 3992468329u: { // FXParticleEmissionVolumeSphere
            input.size=12; Attributes(node,gcnew array<String^>{"TypeId","Radius","IsHollow"}); Children(node,gcnew array<String^>{},false);
            Field(node,"Radius",nullptr,8,Native::ParticleFloat,nullptr,values);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 1242273524u: { // FXParticleEmissionVolumeBox
            input.size=20; Attributes(node,gcnew array<String^>{"TypeId","IsHollow"}); Children(node,gcnew array<String^>{"HalfSize"},false);
            Vector(node,"HalfSize",8,false,values);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 1552576278u: { // FXParticleEmissionVolumeCylinder
            input.size=32; Attributes(node,gcnew array<String^>{"TypeId","Radius","RadiusRate","Length","IsHollow"}); Children(node,gcnew array<String^>{"Offset"},false);
            Field(node,"Radius",nullptr,8,Native::ParticleFloat,nullptr,values);
            Field(node,"RadiusRate",nullptr,12,Native::ParticleFloat,nullptr,values);
            Field(node,"Length",nullptr,16,Native::ParticleFloat,nullptr,values);
            Vector(node,"Offset",20,false,values);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 795809322u: { // FXParticleEmissionVolumeSpline
            input.size=32; Attributes(node,gcnew array<String^>{"TypeId","IsHollow"}); Children(node,gcnew array<String^>{"StartPoint","EndPoint"},false);
            Vector(node,"StartPoint",8,false,values);
            Vector(node,"EndPoint",20,false,values);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 228810652u: { // FXParticleEmissionVolumeLine
            input.size=32; Attributes(node,gcnew array<String^>{"TypeId","IsHollow"}); Children(node,gcnew array<String^>{"StartPoint","EndPoint"},false);
            Vector(node,"StartPoint",8,false,values);
            Vector(node,"EndPoint",20,false,values);
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 3772792373u: { // FXParticleEmissionVolumeTerrainFire
            input.size=48; Attributes(node,gcnew array<String^>{"TypeId","CellEmissionChance","IsHollow"}); Children(node,gcnew array<String^>{"Offset"},false);
            Field(node,"CellEmissionChance",nullptr,8,Native::ParticleFloat,nullptr,values);
            if(Child(node,"Offset")!=nullptr) {
                XmlNode^ coord=Child(node,"Offset"); Attributes(coord,gcnew array<String^>(0)); Children(coord,gcnew array<String^>{"x","y","z"},false);
                ModuleRandom(coord,"x",12,input); ModuleRandom(coord,"y",24,input); ModuleRandom(coord,"z",36,input);
            }
            Field(node,"IsHollow",nullptr,4,Native::ParticleBoolean,nullptr,values);
            break;
        }
        case 2343798068u: { // FXParticleEmissionVelocitySphere
            input.size=16; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Speed"},false);
            ModuleRandom(node,"Speed",4,input);
            break;
        }
        case 3082112962u: { // FXParticleEmissionVelocityHemisphere
            input.size=16; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Speed"},false);
            ModuleRandom(node,"Speed",4,input);
            break;
        }
        case 3476018911u: { // FXParticleEmissionVelocityOutward
            input.size=28; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Speed","OtherSpeed"},false);
            ModuleRandom(node,"Speed",4,input);
            ModuleRandom(node,"OtherSpeed",16,input);
            break;
        }
        case 4058790702u: { // FXParticleEmissionVelocityCylinder
            input.size=28; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Radial","Normal"},false);
            ModuleRandom(node,"Radial",4,input);
            ModuleRandom(node,"Normal",16,input);
            break;
        }
        case 3880943874u: { // FXParticleEmissionVelocityOrtho
            input.size=40; Attributes(node,gcnew array<String^>{"TypeId"}); Children(node,gcnew array<String^>{"Position"},false);
            if(Child(node,"Position")!=nullptr) {
                XmlNode^ coord=Child(node,"Position"); Attributes(coord,gcnew array<String^>(0)); Children(coord,gcnew array<String^>{"x","y","z"},false);
                ModuleRandom(coord,"x",4,input); ModuleRandom(coord,"y",16,input); ModuleRandom(coord,"z",28,input);
            }
            break;
        }
        case 3975650247u: { // FXParticleEmissionVelocityWavefront
            input.size=68; Attributes(node,gcnew array<String^>{"TypeId","Shape"}); Children(node,gcnew array<String^>{"Velocity","Amplitude","InitialRadius","LineWidth","LineAngle"},false);
            Field(node,"Shape","CIRCLE",4,Native::ParticleEnum,"FXParticleSystem_WavefrontShape",values);
            ModuleRandom(node,"Velocity",8,input);
            ModuleRandom(node,"Amplitude",20,input);
            ModuleRandom(node,"InitialRadius",32,input);
            ModuleRandom(node,"LineWidth",44,input);
            ModuleRandom(node,"LineAngle",56,input);
            break;
        }
        case 2693838876u: { throw gcnew NotSupportedException("Unrecovered FXParticleEventBase"); }
        case 2933826200u: { throw gcnew NotSupportedException("Unrecovered FXParticleUpdateDefault"); }
        case 234581578u: { throw gcnew NotSupportedException("Unrecovered FXParticleUpdateRenderObject"); }
        case 2248017887u: { throw gcnew NotSupportedException("Unrecovered FXParticleDrawGpu"); }
        case 3036920908u: { throw gcnew NotSupportedException("Unrecovered FXParticleDrawLightning"); }
        case 3408220251u: { throw gcnew NotSupportedException("Unrecovered FXParticleDrawRenderObject"); }
        case 1075723135u: { throw gcnew NotSupportedException("Unrecovered FXParticleEventCollision"); }
        case 3465979181u: { throw gcnew NotSupportedException("Unrecovered FXParticleEventLife"); }
        default: {
            // Original MarshalUnknownPolymorphicType allocates four zero bytes.
            Attributes(node,gcnew array<String^>{"TypeId"}); Empty(node);
            input.size=4; input.typeId=0; break;
        }
        }
        input.fields=values.fields;
    }
    static void Empty(XmlNode^ node) {
        for each (XmlNode^ child in node->ChildNodes) {
            if (child->NodeType == XmlNodeType::Element ||
                ((child->NodeType == XmlNodeType::Text || child->NodeType == XmlNodeType::CDATA) &&
                !String::IsNullOrWhiteSpace(child->Value)))
                throw gcnew NotSupportedException("Unsupported particle content: " + child->Name);
        }
    }
};
public ref class FXParticleSystemTemplateCompiler abstract sealed {
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, bool bigEndian) {
        XmlfxparticleXml::Root(node, "FXParticleSystemTemplate");
        if(namespaces == nullptr) throw gcnew ArgumentNullException("namespaces");
        XmlfxparticleXml::Attributes(node, gcnew array<String^>{"id","inheritFrom","Priority","Shader","Type","ParticleTexture","Drawable","SlaveSystem","PerParticleAttachedSystem","SystemLifetime","SortLevel","EmitterSound","IsOneShot","IsGroundAligned","IsEmitAboveGroundOnly","IsParticleUpTowardsEmitter","UseMaximumHeight"});
        XmlfxparticleXml::Children(node, gcnew array<String^>{"SlavePosOffset","Lifetime","Size","StartSizeRate","BurstDelay","BurstCount","InitialDelay","Alphas","Colors","Wind","Physics","Draw","Volume","Velocity","Event","Update"}, false);
        Native::XmlfxparticleParticleInput input;
        input.priority = XmlfxparticleXml::Attr(node,"Priority","ULTRA_HIGH_ONLY");
        input.shader = XmlfxparticleXml::Attr(node,"Shader","ADDITIVE");
        input.type = XmlfxparticleXml::Attr(node,"Type","PARTICLE");
        array<String^>^ refs = {"ParticleTexture","Drawable","EmitterSound"};
        for(int i=0;i<3;++i) input.references[i] = XmlfxparticleXml::Attr(node,refs[i],"");
        input.hasEmitterSound=node->Attributes["EmitterSound"] != nullptr;
        input.typedIds[0] = XmlfxparticleXml::Attr(node,"SlaveSystem","");
        input.typedIds[1] = XmlfxparticleXml::Attr(node,"PerParticleAttachedSystem","");
        input.lifetime = XmlfxparticleXml::Attr(node,"SystemLifetime","0");
        input.sortLevel = XmlfxparticleXml::Attr(node,"SortLevel","0");
        array<String^>^ flags = {"IsOneShot","IsGroundAligned","IsEmitAboveGroundOnly","IsParticleUpTowardsEmitter","UseMaximumHeight"};
        for(int i=0;i<5;++i) input.flags[i] = XmlfxparticleXml::Attr(node,flags[i],"false");
        XmlNode^ offset = XmlfxparticleXml::Child(node,"SlavePosOffset");
        if(offset != nullptr) {
            XmlfxparticleXml::Attributes(offset,gcnew array<String^>{"x","y","z"}); XmlfxparticleXml::Empty(offset);
            array<String^>^ axes = {"x","y","z"};
            for(int i=0;i<3;++i) input.offset[i] = XmlfxparticleXml::Attr(offset,axes[i],"0.0");
        }
        array<String^>^ randomNames = {"Lifetime","Size","StartSizeRate","BurstDelay","BurstCount","InitialDelay"};
        for(int i=0;i<6;++i) XmlfxparticleXml::Random(XmlfxparticleXml::Child(node,randomNames[i]),input.random[i]);
        XmlNode^ alphas = XmlfxparticleXml::Child(node,"Alphas");
        if(alphas != nullptr) {
            input.hasAlphas = true;
            XmlfxparticleXml::Attributes(alphas,gcnew array<String^>(0));
            XmlfxparticleXml::Children(alphas,gcnew array<String^>{"Alpha"},true);
            for each(XmlNode^ alpha in alphas->ChildNodes) if(alpha->NodeType == XmlNodeType::Element) {
                XmlfxparticleXml::Attributes(alpha,gcnew array<String^>{"RelativeAge","Frame","Type","Low","High"});
                XmlfxparticleXml::Empty(alpha);
                Native::XmlfxparticleAlphaInput item;
                item.value.present = true;
                item.value.type = XmlfxparticleXml::Attr(alpha,"Type","UNIFORM");
                item.value.low = XmlfxparticleXml::Attr(alpha,"Low","");
                item.value.high = XmlfxparticleXml::Attr(alpha,"High","");
                item.relativeAge = XmlfxparticleXml::Attr(alpha,"RelativeAge","");
                item.frame = XmlfxparticleXml::Attr(alpha,"Frame","1000");
                input.alphas.push_back(item);
            }
        }
        XmlNode^ colors = XmlfxparticleXml::Child(node,"Colors");
        if(colors != nullptr) {
            input.hasColors = true;
            XmlfxparticleXml::Attributes(colors,gcnew array<String^>{"UseHouseColor","HouseColorSaturation"});
            XmlfxparticleXml::Children(colors,gcnew array<String^>{"Color","ColorScale"},true);
            input.houseColor = XmlfxparticleXml::Attr(colors,"UseHouseColor","false");
            input.saturation = XmlfxparticleXml::Attr(colors,"HouseColorSaturation","1.0");
            bool scaleSeen = false;
            for each(XmlNode^ color in colors->ChildNodes) if(color->NodeType == XmlNodeType::Element) {
                if(color->LocalName == "ColorScale") {
                    if(scaleSeen) throw gcnew NotSupportedException("Duplicate ColorScale");
                    scaleSeen = true; XmlfxparticleXml::Random(color,input.colorScale); continue;
                }
                XmlfxparticleXml::Attributes(color,gcnew array<String^>{"RelativeAge","Frame"});
                XmlfxparticleXml::Children(color,gcnew array<String^>{"Color"},false);
                Native::XmlfxparticleColorInput item;
                item.relativeAge = XmlfxparticleXml::Attr(color,"RelativeAge","");
                item.frame = XmlfxparticleXml::Attr(color,"Frame","1000");
                XmlNode^ rgb = XmlfxparticleXml::Child(color,"Color");
                if(rgb != nullptr) {
                    XmlfxparticleXml::Attributes(rgb,gcnew array<String^>{"r","g","b"}); XmlfxparticleXml::Empty(rgb);
                    array<String^>^ channels={"r","g","b"};
                    for(int i=0;i<3;++i) item.rgb[i]=XmlfxparticleXml::Attr(rgb,channels[i],"");
                }
                input.colors.push_back(item);
            }
        }
        XmlNode^ wind = XmlfxparticleXml::Child(node,"Wind");
        if(wind != nullptr) {
            input.hasWind = true;
            array<String^>^ names={"Motion","Strength","FullStrengthDist","ZeroStrengthDist","AngleChangeMin","AngleChangeMax","PingPongStartAngleMin","PingPongStartAngleMax","PingPongEndAngleMin","PingPongEndAngleMax","TurbulenceAmplitude","TurbulenceFrequency"};
            array<String^>^ defaults={"NOT_USED","2.0","75.0","200.0",".15",".45","0.0","0.7853981633974483","5.4977871437821381","6.2831853071795864","0","0"};
            XmlfxparticleXml::Attributes(wind,names); XmlfxparticleXml::Empty(wind);
            for(int i=0;i<12;++i) input.wind[i]=XmlfxparticleXml::Attr(wind,names[i],defaults[i]);
        }
        XmlNode^ events = XmlfxparticleXml::Child(node,"Event");
        if(events != nullptr) { XmlfxparticleXml::Attributes(events,gcnew array<String^>(0)); XmlfxparticleXml::Empty(events); }
        array<String^>^ moduleNames={"Physics","Draw","Volume","Velocity","Update"};
        for(int i=0;i<5;++i) XmlfxparticleXml::Module(XmlfxparticleXml::Child(node,moduleNames[i]),input.modules[i]);
        return XmlfxparticleXml::Buffer(Native::CompileXmlfxparticleParticle(input,bigEndian));
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration, TargetPlatform platform) {
        if (declaration == nullptr) throw gcnew ArgumentNullException("declaration");
        if (declaration->Handle->TypeId != 686292351u)
            throw gcnew NotSupportedException("Expected FXParticleSystemTemplate TypeId");
        return CompileXml(declaration->Node, declaration->Document->NamespaceManager, platform != TargetPlatform::Win32);
    }
};
}

