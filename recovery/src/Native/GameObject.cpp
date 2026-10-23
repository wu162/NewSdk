#include "GameObject.h"
#include "ScalarParsing.h"
#include "DataInitializers.h"
#include "ResourceMarshaling.h"
#include "EnumBitset.h"
#include "ObjectFilter.h"
#include <cstring>
#include <stdexcept>
#include <algorithm>
#include <cmath>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
namespace {
using U32=std::uint32_t;
enum class GameObjectTokenKind { Member, List, Null };
struct GameObjectEnum {const char* field; const char* name; int value;};
struct GameObjectEnumFallback {const char* field; int value;};
struct GameObjectToken { U32 name,type,offset; GameObjectTokenKind kind; };
enum class LeafValue { Float, Percent, Unsigned, Boolean, Time, Enum, String, Reference, AssetId };
struct LeafField {const char* name;unsigned offset;LeafValue kind;const char* fallback;};
struct LeafToken {U32 name,type,offset;};
struct LeafLayout {const char* name;unsigned offset,size;const LeafField* fields;unsigned count;const LeafToken* tokens;};
#include "GameObjectFields.h"
struct ReloString {U32 length;void* data;};
static_assert(sizeof(ReloString)==8, "GameObject strings require x86");
struct NativeList {U32 count;void* data;};
static_assert(sizeof(NativeList)==8, "GameObject lists require x86");
struct Procedure { U32 name,type; void* offset; };
static_assert(sizeof(Procedure)==12, "GameObject requires x86");
U32 EnumValue(const std::string& field,const std::string& text) {
    for(const auto& e:enumEntries) if(field==e.field && text==e.name) return static_cast<U32>(e.value);
    for(const auto& e:enumFallbacks) if(field==e.field) return static_cast<U32>(e.value);
    throw std::logic_error("Unknown GameObject enum field");
}
void Swap(void* p) { auto b=static_cast<unsigned char*>(p); std::swap(b[0],b[3]);std::swap(b[1],b[2]); }
}
const GameObjectField* GameObjectFields() { return fields; }
unsigned GameObjectFieldCount() { return sizeof(fields)/sizeof(fields[0]); }
bool GameObjectLeafAttributeSupported(const std::string& name,const std::string& attribute) {
    for(const auto& leaf:leafLayouts) if(name==leaf.name)
        for(unsigned i=0;i<leaf.count;++i) if(attribute==leaf.fields[i].name) return true;
    return false;
}
Relo::Chunk CompileGameObject(const std::map<std::string,std::string>& attributes,const std::vector<GameObjectChild>& children,bool bigEndian) {
    for(const auto& a:attributes) {
        const GameObjectField* match=nullptr;
        for(const auto& f:fields) if(a.first==f.name) {match=&f;break;}
        if(!match)
            throw std::invalid_argument("Unsupported GameObject attribute: "+a.first);
    }
    Relo::Tracker data, procedures;
    unsigned char* root=nullptr;
    data.Push(reinterpret_cast<void**>(&root),592,1);
    // Original Browser Marshal precedes every scalar/string allocation.
    std::vector<std::string> browser;
    auto browserInput=attributes.find("Browser");
    if(browserInput!=attributes.end()) {
        const auto& text=browserInput->second;
        auto space=[](unsigned char c) {return c==' ' || (c>=9 && c<=13);};
        std::size_t at=0;
        while(at<text.size() && text[at]) {
            while(at<text.size() && space(static_cast<unsigned char>(text[at]))) ++at;
            auto first=at;
            while(at<text.size() && text[at] && !space(static_cast<unsigned char>(text[at]))) ++at;
            if(at>first) browser.push_back(text.substr(first,at-first));
        }
    }
    ReloString* browserStrings=nullptr;
    if(!browser.empty()) {
        U32 count=static_cast<U32>(browser.size());std::memcpy(root+40,&count,4);
        browserStrings=static_cast<ReloString*>(data.Push(reinterpret_cast<void**>(root+44),8,count));
        for(U32 i=0;i<count;++i) MarshalString(data,browserStrings+i,browser[i]);
        data.Pop();
    }
    for(const auto& f:fields) {
        if(f.kind==GameObjectValue::StringList) continue;
        auto found=attributes.find(f.name);
        auto at=root+f.offset;
        U32 word=0;float number=0;
        if(f.kind==GameObjectValue::DurationDefault && found==attributes.end()) {
            word=static_cast<U32>(f.defaultValue);std::memcpy(at,&word,4);continue;
        }
        if(found==attributes.end() && !f.fallback) continue;
        std::string text=found==attributes.end()?f.fallback:found->second;
        switch(f.kind) {
        case GameObjectValue::DurationDefault: {
            // Original helper: atof -> float -> scaled float -> ceil -> float
            // -> conv.u4. The x86 CLR conversion truncates through signed int64;
            // out-of-int64 values produce the integer-indefinite low word (0).
            const float value=ParseFloat(text);
            const float scaled=static_cast<float>(static_cast<double>(0.005f)*static_cast<double>(value));
            const float rounded=static_cast<float>(std::ceil(static_cast<double>(scaled)));
            const double wide=static_cast<double>(rounded);
            word=(wide>=-9223372036854775808.0 && wide<9223372036854775808.0)
                ?static_cast<U32>(static_cast<std::int64_t>(wide)):0u;
            break;
        }
        case GameObjectValue::Bits: MarshalEnumBitset(text,kindOfEntries,284,149,284,reinterpret_cast<U32*>(at));continue;
        case GameObjectValue::AssetId: word=HashAssetId(text);break;
        case GameObjectValue::Reference: MarshalReference(data,at,text);continue;
        case GameObjectValue::Enum: word=EnumValue(f.name,text);break;
        case GameObjectValue::String: MarshalString(data,at,text);continue;
        case GameObjectValue::Boolean: *at=ParseBoolean(text)?1:0;continue;
        case GameObjectValue::Byte: *at=static_cast<unsigned char>(ParseUnsigned(text));continue;
        case GameObjectValue::Short: {auto value=static_cast<std::uint16_t>(ParseUnsigned(text));std::memcpy(at,&value,2);continue;}
        case GameObjectValue::Signed: word=static_cast<U32>(ParseSigned(text));break;
        case GameObjectValue::Unsigned: word=ParseUnsigned(text);break;
        case GameObjectValue::Float: number=ParseFloat(text);std::memcpy(&word,&number,4);break;
        case GameObjectValue::Time: number=InitializeTime(text.c_str());std::memcpy(&word,&number,4);break;
        case GameObjectValue::Angle: number=InitializeAngle(text.c_str());std::memcpy(&word,&number,4);break;
        default: throw std::logic_error("Unsupported GameObject value");
        }
        std::memcpy(at,&word,4);
    }
    ReloString* displayNames[2]={nullptr,nullptr};
    // Original allocation order is schema order, independent of XML order.
    for(unsigned i=0;i<2;++i) for(const auto& child:children)
        if(child.name==(i==0?"DisplayName":"DisplayNameTransformed")) {
            displayNames[i]=static_cast<ReloString*>(data.Push(reinterpret_cast<void**>(root+396+4*i),8,1));
            MarshalString(data,displayNames[i],child.text);data.Pop();
        }
    for(const auto& child:children) {
        U32 word=0;unsigned offset=0;
        if(child.name=="Buildable" || child.name=="ThingClass") {
            word=EnumValue(child.name,child.text);offset=child.name=="Buildable"?428:432;
        } else if(child.name=="DisplayColor") {
            auto channel=[&](const char* name,U32 fallback) {
                auto found=child.attributes.find(name);
                return (found==child.attributes.end()?fallback:ParseUnsigned(found->second))&255u;
            };
            word=(channel("A",255)<<24)|(channel("R",0)<<16)|(channel("G",0)<<8)|channel("B",0);offset=452;
        } else continue;
        std::memcpy(root+offset,&word,4);
    }
    unsigned char* dependency=nullptr;
    U32* dependencyMasks[3]={nullptr,nullptr,nullptr};
    ObjectFilter* dependencyFilter=nullptr;
    for(const auto& child:children) if(child.name=="GameDependency") {
        dependency=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+404),40,1));
        const char* names[]={"RequiredModelConditionsAny","ForbiddenModelConditions","RequiredObjectStatusAny"};
        for(unsigned j=0;j<3;++j) {
            auto found=child.attributes.find(names[j]);if(found==child.attributes.end()) continue;
            dependencyMasks[j]=static_cast<U32*>(data.Push(reinterpret_cast<void**>(dependency+4*j),j==2?28:60,1));
            if(j==2) MarshalObjectStatus(found->second,dependencyMasks[j]);else MarshalModelCondition(found->second,dependencyMasks[j]);
            data.Pop();
        }
        const char* groups[]={"RequiredObject","ForbiddenUpgrade","NeededUpgrade"};
        for(unsigned group=0;group<3;++group) {
            std::vector<std::string> values;
            for(const auto& value:child.children) if(value.name==groups[group]) values.push_back(value.text);
            auto list=reinterpret_cast<NativeList*>(dependency+12+8*group);list->count=static_cast<U32>(values.size());
            if(list->count) {
                auto elements=static_cast<U32*>(data.Push(&list->data,4,list->count));
                for(U32 j=0;j<list->count;++j) elements[j]=HashAssetId(values[j]);
                data.Pop();
            }
        }
        for(const auto& value:child.children) if(value.name=="ObjectFilter") {
            ObjectFilterInput input;input.attributes=value.attributes;
            for(const auto& thing:value.children) if(thing.name=="IncludeThing") input.includeThings.push_back(thing.text);else input.excludeThings.push_back(thing.text);
            dependencyFilter=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(dependency+36),112,1));
            MarshalObjectFilter(input,dependencyFilter,data);data.Pop();
        }
        data.Pop();
    }
    NativeList* buildCosts=nullptr;
    for(const auto& child:children) if(child.name=="ObjectResourceInfo") {
        buildCosts=static_cast<NativeList*>(data.Push(reinterpret_cast<void**>(root+408),8,1));
        buildCosts->count=static_cast<U32>(child.children.size());
        if(buildCosts->count) {
            auto elements=static_cast<U32*>(data.Push(&buildCosts->data,8,buildCosts->count));
            for(U32 i=0;i<buildCosts->count;++i) for(unsigned j=0;j<2;++j) {
                auto found=child.children[i].attributes.find(j==0?"Account":"Amount");
                if(found!=child.children[i].attributes.end()) elements[2*i+j]=ParseUnsigned(found->second);
            }
            data.Pop();
        }
        data.Pop();
    }
    std::map<unsigned,NativeList*> lists;
    lists[40]=reinterpret_cast<NativeList*>(root+40);
    auto marshalSets=[&](const char* name,unsigned offset,bool armor) {
        std::vector<const GameObjectChild*> values;
        for(const auto& child:children) if(child.name==name) values.push_back(&child);
        auto list=reinterpret_cast<NativeList*>(root+offset);lists[offset]=list;
        list->count=static_cast<U32>(values.size());
        if(values.empty()) return;
        auto elements=static_cast<unsigned char*>(data.Push(&list->data,12,list->count));
        for(U32 i=0;i<list->count;++i) {
            auto element=elements+12*i;const auto& attrs=values[i]->attributes;
            auto find=[&](const char* key)->const std::string* {auto it=attrs.find(key);return it==attrs.end()?nullptr:&it->second;};
            if(armor) {
                if(auto value=find("Armor")) MarshalReference(data,element,*value);
                if(auto value=find("DamageFX")) MarshalReference(data,element+4,*value);
                if(auto value=find("Conditions")) MarshalEnumBitset(*value,armorSetEntries,23,12,23,reinterpret_cast<U32*>(element+8));
            } else {
                if(auto value=find("Speed")) {auto number=ParseFloat(*value);std::memcpy(element,&number,4);}
                if(auto value=find("Condition")) {auto word=EnumValue("LocomotorSet",*value);std::memcpy(element+4,&word,4);}
                if(auto value=find("Locomotor")) MarshalReference(data,element+8,*value);
            }
        }
        data.Pop();
    };
    marshalSets("ArmorSet",412,true);
    marshalSets("LocomotorSet",420,false);
    auto marshalList=[&](const char* name,unsigned offset,bool strings) {
        std::vector<std::string> values;
        for(const auto& child:children) if(child.name==name) values.push_back(child.text);
        auto list=reinterpret_cast<NativeList*>(root+offset);lists[offset]=list;
        list->count=static_cast<U32>(values.size());
        if(values.empty()) return;
        auto elements=static_cast<unsigned char*>(data.Push(&list->data,strings?8:4,list->count));
        for(U32 i=0;i<list->count;++i) {
            if(strings) MarshalString(data,elements+8*i,values[i]);
            else {auto id=HashAssetId(values[i]);std::memcpy(elements+4*i,&id,4);}
        }
        data.Pop();
    };
    // The original pointer-list overload selects element children, not text
    // tokens. XSD StringList permits text only, so present valid nodes allocate
    // an empty list header even when their text contains several words.
    std::map<unsigned,NativeList*> pointerLists;
    for(unsigned index=0;index<2;++index) for(const auto& child:children)
        if(child.name==(index==0?"BuildFadeInOnCreateList":"BuildVariations")) {
            unsigned offset=436+4*index;
            pointerLists[offset]=static_cast<NativeList*>(data.Push(reinterpret_cast<void**>(root+offset),8,1));
            data.Pop();
        }
    marshalList("EquivalentTo",444,false);
    U32* flammability=nullptr;float* camera=nullptr;
    std::map<unsigned,unsigned char*> leafObjects;
    auto marshalLeaves=[&](unsigned first,unsigned last) {
        for(const auto& leaf:leafLayouts) if(leaf.offset>=first && leaf.offset<=last)
            for(const auto& child:children) if(child.name==leaf.name) {
                auto bytes=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+leaf.offset),leaf.size,1));
                leafObjects[leaf.offset]=bytes;
                for(unsigned i=0;i<leaf.count;++i) {
                    const auto& f=leaf.fields[i];auto found=child.attributes.find(f.name);
                    if(found==child.attributes.end() && !f.fallback) continue;
                    std::string value=found==child.attributes.end()?f.fallback:found->second;
                    auto at=bytes+f.offset;U32 word=0;float number=0;
                    switch(f.kind) {
                    case LeafValue::String: MarshalString(data,at,value);continue;
                    case LeafValue::Reference: MarshalReference(data,at,value);continue;
                    case LeafValue::AssetId: word=HashAssetId(value);break;
                    case LeafValue::Boolean: *at=ParseBoolean(value)?1:0;continue;
                    case LeafValue::Enum: word=EnumValue(leaf.name,value);break;
                    case LeafValue::Unsigned: word=ParseUnsigned(value);break;
                    default:
                        number=f.kind==LeafValue::Time?InitializeTime(value.c_str()):f.kind==LeafValue::Percent?ParsePercentage(value):ParseFloat(value);
                        std::memcpy(&word,&number,4);break;
                    }
                    std::memcpy(at,&word,4);
                }
                data.Pop();
            }
    };
    for(const auto& child:children) if(child.name=="Flammability") {
        flammability=static_cast<U32*>(data.Push(reinterpret_cast<void**>(root+456),16,1));
        const char* names[]={"Fuel","MaxBurnRate","Decay","Resistance"};
        for(unsigned i=0;i<4;++i) {auto at=child.attributes.find(names[i]);flammability[i]=at==child.attributes.end()?0:ParseUnsigned(at->second);}
        data.Pop();
    }
    unsigned char* geometry=nullptr;
    NativeList* geometryShapes=nullptr;NativeList* contactPoints=nullptr;
    auto marshalVector=[&](const GameObjectChild& input,void* output,unsigned dimensions) {
        const char* names[]={"x","y","z"};
        for(unsigned j=0;j<dimensions;++j) {auto found=input.attributes.find(names[j]);float value=found==input.attributes.end()?0.0f:ParseFloat(found->second);std::memcpy(static_cast<unsigned char*>(output)+4*j,&value,4);}
    };
    auto marshalShape=[&](const GameObjectChild& input,unsigned char* output) {
        auto value=[&](const char* name,const char* fallback)->std::string {auto found=input.attributes.find(name);return found==input.attributes.end()?fallback:found->second;};
        U32 type=EnumValue("GeometryShapeType",value("Type","SPHERE")),generation=EnumValue("ContactPointGeneration",value("ContactPointGeneration","NONE"));
        std::memcpy(output,&type,4);std::memcpy(output+4,&generation,4);
        auto name=input.attributes.find("Name");if(name!=input.attributes.end()) MarshalString(data,output+8,name->second);
        output[44]=ParseBoolean(value("Active","true"))?1:0;output[45]=ParseBoolean(value("UsedForHealthBox","true"))?1:0;
        float major=ParseFloat(value("MajorRadius","1.0")),minor=ParseFloat(value("MinorRadius","1.0"));std::memcpy(output+16,&major,4);std::memcpy(output+20,&minor,4);
        auto other=input.attributes.find("Other");if(other!=input.attributes.end()) MarshalString(data,output+24,other->second);
        float height=ParseFloat(value("Height","1.0"));std::memcpy(output+32,&height,4);
        auto angle=input.attributes.find("FrontAngle");if(angle!=input.attributes.end()) {float value=InitializeAngle(angle->second.c_str());std::memcpy(output+36,&value,4);}
        if(!input.children.empty()) {auto vector=data.Push(reinterpret_cast<void**>(output+40),12,1);marshalVector(input.children[0],vector,3);data.Pop();}
    };
    for(const auto& child:children) if(child.name=="Geometry") {
        geometry=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+492),28,1));
        auto small=child.attributes.find("IsSmall");geometry[24]=small!=child.attributes.end() && ParseBoolean(small->second)?1:0;
        for(const auto& c:child.children) if(c.name=="RotationAnchorOffset") {auto vector=data.Push(reinterpret_cast<void**>(geometry),8,1);marshalVector(c,vector,2);data.Pop();}
        for(const auto& c:child.children) if(c.name=="PlacementShape") {auto shape=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(geometry+4),48,1));marshalShape(c,shape);data.Pop();}
        geometryShapes=reinterpret_cast<NativeList*>(geometry+8);contactPoints=reinterpret_cast<NativeList*>(geometry+16);
        for(const auto& c:child.children) if(c.name=="Shape") ++geometryShapes->count;else if(c.name=="ContactPoint") ++contactPoints->count;
        if(geometryShapes->count) {
            auto shapes=static_cast<unsigned char*>(data.Push(&geometryShapes->data,48,geometryShapes->count));U32 index=0;
            for(const auto& c:child.children) if(c.name=="Shape") {marshalShape(c,shapes+48*index);++index;}
            data.Pop();
        }
        if(contactPoints->count) {
            auto points=static_cast<unsigned char*>(data.Push(&contactPoints->data,20,contactPoints->count));U32 index=0;
            for(const auto& c:child.children) if(c.name=="ContactPoint") {
                auto at=points+20*index++;auto label=c.attributes.find("Label");if(label!=c.attributes.end()) MarshalString(data,at,label->second);
                if(!c.children.empty()) marshalVector(c.children[0],at+8,3);
            }
            data.Pop();
        }
        data.Pop();
    }
    std::map<unsigned,unsigned char*> autoObjects;
    for(unsigned index=0;index<2;++index) for(const auto& child:children)
        if(child.name==(index==0?"AutoResolveArmor":"AutoResolveWeapon")) {
            unsigned offset=504+4*index;
            auto bytes=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+offset),20,1));
            autoObjects[offset]=bytes;
            auto ref=child.attributes.find(index==0?"Armor":"Weapon");
            if(ref!=child.attributes.end()) MarshalReference(data,bytes,ref->second);
            for(unsigned group=0;group<2;++group) {
                std::vector<std::string> values;
                for(const auto& value:child.children) if(value.name==(group==0?"RequiredUpgrade":"ExcludedUpgrade")) values.push_back(value.text);
                auto list=reinterpret_cast<NativeList*>(bytes+4+8*group);
                list->count=static_cast<U32>(values.size());
                if(!values.empty()) {
                    auto elements=static_cast<U32*>(data.Push(&list->data,4,list->count));
                    for(U32 i=0;i<list->count;++i) elements[i]=HashAssetId(values[i]);
                    data.Pop();
                }
            }
            data.Pop();
        }
    marshalLeaves(512,516);
    for(const auto& child:children) if(child.name=="LiveCameraOffset") {
        camera=static_cast<float*>(data.Push(reinterpret_cast<void**>(root+520),12,1));
        const char* names[]={"x","y","z"};
        for(unsigned i=0;i<3;++i) {auto at=child.attributes.find(names[i]);camera[i]=at==child.attributes.end()?0.0f:ParseFloat(at->second);}
        data.Pop();
    }
    marshalList("UpgradeCameo",536,true);
    marshalLeaves(544,552);
    unsigned char* crusher=nullptr;
    NativeList* extraLevels=nullptr;
    for(const auto& child:children) if(child.name=="CrusherInfo") {
        crusher=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+556),36,1));
        auto value=[&](const char* name,const char* fallback)->std::string {auto it=child.attributes.find(name);return it==child.attributes.end()?fallback:it->second;};
        auto word=static_cast<U32>(ParseSigned(value("CrushKnockback","0")));std::memcpy(crusher,&word,4);
        const char* floats[]={"CrushZFactor","MinCrushVelocityPercent","CrushDecelerationPercent"};
        for(unsigned j=0;j<3;++j) {auto text=value(floats[j],j==2?"0":"1");float number=j==0?ParseFloat(text):ParsePercentage(text);std::memcpy(crusher+4+4*j,&number,4);}
        auto ref=child.attributes.find("CrushWeapon");if(ref!=child.attributes.end()) MarshalReference(data,crusher+16,ref->second);
        crusher[28]=static_cast<unsigned char>(ParseSigned(value("CrusherLevel","0")));
        crusher[29]=static_cast<unsigned char>(ParseSigned(value("CrushableLevel","127")));
        const char* flags[]={"CrushEqualLevelProps","UseCrushAttack","CrushOnlyWhileCharging","CrushAllies","CrushWhileStationary"};
        for(unsigned j=0;j<5;++j) crusher[30+j]=ParseBoolean(value(flags[j],j==1?"true":"false"))?1:0;
        extraLevels=reinterpret_cast<NativeList*>(crusher+20);extraLevels->count=static_cast<U32>(child.children.size());
        if(extraLevels->count) {
            auto items=static_cast<unsigned char*>(data.Push(&extraLevels->data,8,extraLevels->count));
            for(U32 j=0;j<extraLevels->count;++j) {
                const auto& attrs=child.children[j].attributes;auto at=items+8*j;
                for(unsigned k=0;k<2;++k) {auto found=attrs.find(k==0?"CrusherLevel":"CrushableLevel");at[4+k]=found==attrs.end()?(k==0?0:127):static_cast<unsigned char>(ParseSigned(found->second));}
                auto found=attrs.find("ModelConditionMatch");if(found!=attrs.end()) {auto v=EnumValue("ExtraCrushLevels",found->second);std::memcpy(at,&v,4);}
            }
            data.Pop();
        }
        data.Pop();
    }
    auto projections=reinterpret_cast<NativeList*>(root+560);
    std::vector<const GameObjectChild*> projectionInputs;
    for(const auto& child:children) if(child.name=="ProjectedBuildabilityInfo") projectionInputs.push_back(&child);
    projections->count=static_cast<U32>(projectionInputs.size());
    std::vector<ObjectFilter*> projectionFilters(projections->count,nullptr);
    if(projections->count) {
        auto items=static_cast<unsigned char*>(data.Push(&projections->data,112,projections->count));
        const EnumNameValue placements[]={{"BLOCKED",4},{"INVALID",0},{"MAIN_STRUCTURE",1},{"OTHER_STRUCTURE",2},{"TIBERIUM_FIELD",3}};
        for(U32 j=0;j<projections->count;++j) {
            auto at=items+112*j;const auto& child=*projectionInputs[j];
            auto value=[&](const char* name,const char* fallback)->std::string {auto it=child.attributes.find(name);return it==child.attributes.end()?fallback:it->second;};
            float radius=ParseFloat(value("Radius","0"));std::memcpy(at,&radius,4);
            auto ry=child.attributes.find("RadiusY");if(ry!=child.attributes.end()) {
                auto number=static_cast<float*>(data.Push(reinterpret_cast<void**>(at+4),4,1));*number=ParseFloat(ry->second);data.Pop();
            }
            MarshalEnumBitset(value("BuildPlacementTypes","INVALID"),placements,5,4,5,reinterpret_cast<U32*>(at+8));
            auto status=child.attributes.find("StatusToReject");if(status!=child.attributes.end()) MarshalObjectStatus(status->second,reinterpret_cast<U32*>(at+12));
            MarshalModelCondition(value("ModelConditionsToReject","STRUCTURE_UNPACKING"),reinterpret_cast<U32*>(at+40));
            float height=ParseFloat(value("AllowedBuildabilityHeightVariation","50.0"));std::memcpy(at+100,&height,4);
            at[108]=ParseBoolean(value("PrimaryBuidability","true"))?1:0;
            if(!child.children.empty()) {
                const auto& filter=child.children[0];ObjectFilterInput input;input.attributes=filter.attributes;
                for(const auto& thing:filter.children) if(thing.name=="IncludeThing") input.includeThings.push_back(thing.text);else input.excludeThings.push_back(thing.text);
                projectionFilters[j]=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(at+104),112,1));
                MarshalObjectFilter(input,projectionFilters[j],data);data.Pop();
            }
        }
        data.Pop();
    }
    marshalList("DisplayUpgrade",568,false);
    // Unsupported optional pointers and remaining list counts stay absent.
    // Eight inline empty lists still produce count tables in original token order.
    Procedure* table=nullptr;
    procedures.Push(reinterpret_cast<void**>(&table),12,127);
    for(unsigned i=0;i<126;++i) {
        const auto& t=tokens[i];table[i].name=t.name;table[i].type=t.type;
        if(t.kind==GameObjectTokenKind::Member) procedures.BookmarkMemberOffset(root+t.offset,&table[i].offset);
        else if((t.offset==396 || t.offset==400) && displayNames[(t.offset-396)/4])
            procedures.BookmarkMemberOffset(displayNames[(t.offset-396)/4],&table[i].offset);
        else if(t.offset==520 && camera) procedures.BookmarkMemberOffset(camera,&table[i].offset);
        else if(t.offset==456 && flammability) {
            // Original Tokenize(Flammability), line 95659: four members + sentinel.
            const U32 names[]={2194468989u,1555868979u,1413518551u,2097559226u};
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,5));
            for(unsigned j=0;j<4;++j) {nested[j].name=names[j];nested[j].type=1982953380u;procedures.BookmarkMemberOffset(flammability+j,&nested[j].offset);}
            procedures.Pop();
        }
        else if(t.offset==404 && dependency) {
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,8));
            const U32 keys[]={618545001u,849164020u,2787973588u,850409515u,725359606u,2925224871u,3832456114u};
            const U32 types[]={1742454729u,1742454729u,2814214430u,344905565u,653971399u,653971399u,3832456114u};
            for(unsigned j=0;j<7;++j) {nested[j].name=keys[j];nested[j].type=types[j];}
            for(unsigned j=0;j<3;++j) if(dependencyMasks[j]) procedures.BookmarkMemberOffset(dependencyMasks[j],&nested[j].offset);
            for(unsigned group=0;group<3;++group) {
                auto list=reinterpret_cast<NativeList*>(dependency+12+8*group);
                auto count=static_cast<Procedure*>(procedures.Push(&nested[group+3].offset,12,list->count+1));
                count[0].name=1820487795u;count[0].type=1564774360u;
                procedures.BookmarkMemberOffset(list,&count[0].offset);
                for(U32 j=0;j<list->count;++j) {
                    count[j+1].name=2974916843u;count[j+1].type=0;
                    procedures.BookmarkMemberOffset(static_cast<U32*>(list->data)+j,&count[j+1].offset);
                }
                procedures.Pop();
            }
            if(dependencyFilter) TokenizeObjectFilter(dependencyFilter,&nested[6].offset,procedures);
            procedures.Pop();
        }
        else if(t.offset==408 && buildCosts) {
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,2));
            nested[0].name=4043881713u;nested[0].type=3070636877u;
            auto count=static_cast<Procedure*>(procedures.Push(&nested[0].offset,12,buildCosts->count+1));
            count[0].name=1820487795u;count[0].type=1564774360u;
            procedures.BookmarkMemberOffset(buildCosts,&count[0].offset);
            for(U32 j=0;j<buildCosts->count;++j) {
                count[j+1].name=2974916843u;count[j+1].type=0;
                auto item=static_cast<Procedure*>(procedures.Push(&count[j+1].offset,12,3));
                for(unsigned k=0;k<2;++k) {
                    item[k].name=k==0?78324893u:1343336868u;item[k].type=1982953380u;
                    procedures.BookmarkMemberOffset(static_cast<U32*>(buildCosts->data)+2*j+k,&item[k].offset);
                }
                procedures.Pop();
            }
            procedures.Pop();procedures.Pop();
        }
        else if(pointerLists.count(t.offset)) {
            auto count=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,1));
            count[0].name=1820487795u;count[0].type=1564774360u;
            procedures.BookmarkMemberOffset(pointerLists.at(t.offset),&count[0].offset);
            procedures.Pop();
        }
        else if(autoObjects.count(t.offset)) {
            auto bytes=autoObjects.at(t.offset);
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,4));
            nested[0].name=t.offset==504?735930353u:63188358u;
            nested[0].type=t.offset==504?1636048523u:3494305058u;
            procedures.BookmarkMemberOffset(bytes,&nested[0].offset);
            for(unsigned group=0;group<2;++group) {
                nested[group+1].name=group==0?2138150697u:4283231007u;
                nested[group+1].type=653971399u;
                auto list=reinterpret_cast<NativeList*>(bytes+4+8*group);
                auto count=static_cast<Procedure*>(procedures.Push(&nested[group+1].offset,12,list->count+1));
                count[0].name=1820487795u;count[0].type=1564774360u;
                procedures.BookmarkMemberOffset(list,&count[0].offset);
                for(U32 j=0;j<list->count;++j) {
                    count[j+1].name=2974916843u;count[j+1].type=0;
                    procedures.BookmarkMemberOffset(static_cast<U32*>(list->data)+j,&count[j+1].offset);
                }
                procedures.Pop();
            }
            procedures.Pop();
        }
        else if(t.offset==492 && geometry) {
            auto tokenizeShape=[&](unsigned char* shape,void** destination) {
                auto nested=static_cast<Procedure*>(procedures.Push(destination,12,12));
                for(unsigned j=0;j<11;++j) {nested[j].name=ShapeProc[j].name;nested[j].type=ShapeProc[j].type;
                    if(j==10) {auto vector=*reinterpret_cast<void**>(shape+40);if(vector) procedures.BookmarkMemberOffset(vector,&nested[j].offset);}
                    else procedures.BookmarkMemberOffset(shape+ShapeProc[j].offset,&nested[j].offset);
                }
                procedures.Pop();
            };
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,6));
            for(unsigned j=0;j<5;++j) {nested[j].name=GeometryProc[j].name;nested[j].type=GeometryProc[j].type;}
            procedures.BookmarkMemberOffset(geometry+24,&nested[0].offset);
            auto anchor=*reinterpret_cast<void**>(geometry);if(anchor) procedures.BookmarkMemberOffset(anchor,&nested[1].offset);
            auto placement=*reinterpret_cast<unsigned char**>(geometry+4);if(placement) tokenizeShape(placement,&nested[2].offset);
            for(unsigned group=0;group<2;++group) {
                auto list=group==0?geometryShapes:contactPoints;
                auto count=static_cast<Procedure*>(procedures.Push(&nested[group+3].offset,12,list->count+1));
                count[0].name=1820487795u;count[0].type=1564774360u;procedures.BookmarkMemberOffset(list,&count[0].offset);
                for(U32 j=0;j<list->count;++j) {
                    count[j+1].name=2974916843u;
                    auto bytes=static_cast<unsigned char*>(list->data)+(group==0?48:20)*j;
                    if(group==0) tokenizeShape(bytes,&count[j+1].offset);
                    else {
                        auto item=static_cast<Procedure*>(procedures.Push(&count[j+1].offset,12,3));
                        for(unsigned k=0;k<2;++k) {item[k].name=ContactPointProc[k].name;item[k].type=ContactPointProc[k].type;procedures.BookmarkMemberOffset(bytes+ContactPointProc[k].offset,&item[k].offset);}
                        procedures.Pop();
                    }
                }
                procedures.Pop();
            }
            procedures.Pop();
        }
        else if(t.offset==556 && crusher) {
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,14));
            const U32 keys[]={910974594u,3643225156u,3253443162u,3004652270u,2213912127u,3110872486u,4183563336u,1698114463u,797901100u,4120968022u,3910970335u,3401922681u,4110060450u};
            const U32 types[]={924736962u,924736962u,602296504u,1683514611u,2011428087u,2011428087u,2011428087u,1683514611u,1683514611u,2011428087u,3494305058u,2011428087u,2425485691u};
            const unsigned offsets[]={28,29,0,4,30,31,32,8,12,33,16,34};
            for(unsigned j=0;j<13;++j) {nested[j].name=keys[j];nested[j].type=types[j];if(j<12) procedures.BookmarkMemberOffset(crusher+offsets[j],&nested[j].offset);}
            auto count=static_cast<Procedure*>(procedures.Push(&nested[12].offset,12,extraLevels->count+1));
            count[0].name=1820487795u;count[0].type=1564774360u;procedures.BookmarkMemberOffset(extraLevels,&count[0].offset);
            for(U32 j=0;j<extraLevels->count;++j) {
                count[j+1].name=2974916843u;
                auto item=static_cast<Procedure*>(procedures.Push(&count[j+1].offset,12,4));
                const U32 names[]={910974594u,3643225156u,4049621940u};const U32 types2[]={924736962u,924736962u,359808523u};const unsigned off[]={4,5,0};
                for(unsigned k=0;k<3;++k) {item[k].name=names[k];item[k].type=types2[k];procedures.BookmarkMemberOffset(static_cast<unsigned char*>(extraLevels->data)+8*j+off[k],&item[k].offset);}
                procedures.Pop();
            }
            procedures.Pop();procedures.Pop();
        }
        else if(t.offset==560) {
            auto count=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,projections->count+1));
            count[0].name=1820487795u;count[0].type=1564774360u;procedures.BookmarkMemberOffset(projections,&count[0].offset);
            for(U32 j=0;j<projections->count;++j) {
                count[j+1].name=2974916843u;
                auto item=static_cast<Procedure*>(procedures.Push(&count[j+1].offset,12,9));auto bytes=static_cast<unsigned char*>(projections->data)+112*j;
                const U32 keys[]={1328662213u,3519330798u,3300370196u,1859875231u,2524881596u,1464587907u,789445011u,3484925126u};
                const U32 types[]={1683514611u,1683514611u,4098281327u,2814214430u,1742454729u,1683514611u,2011428087u,3832456114u};
                const unsigned offsets[]={0,4,8,12,40,100,108,104};
                for(unsigned k=0;k<8;++k) {item[k].name=keys[k];item[k].type=types[k];if(k==1) {auto radius=*reinterpret_cast<float**>(bytes+4);if(radius) procedures.BookmarkMemberOffset(radius,&item[k].offset);}else if(k==7) {if(projectionFilters[j]) TokenizeObjectFilter(projectionFilters[j],&item[k].offset,procedures);}else procedures.BookmarkMemberOffset(bytes+offsets[k],&item[k].offset);}
                procedures.Pop();
            }
            procedures.Pop();
        }
        else if(leafObjects.count(t.offset)) {
            for(const auto& leaf:leafLayouts) if(leaf.offset==t.offset) {
                auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,leaf.count+1));
                for(unsigned j=0;j<leaf.count;++j) {
                    nested[j].name=leaf.tokens[j].name;nested[j].type=leaf.tokens[j].type;
                    procedures.BookmarkMemberOffset(leafObjects.at(t.offset)+leaf.tokens[j].offset,&nested[j].offset);
                }
                procedures.Pop();break;
            }
        }
        else if(t.kind==GameObjectTokenKind::List) {
            auto found=lists.find(t.offset);auto list=found==lists.end()?nullptr:found->second;
            const U32 length=list?list->count:0;
            auto count=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,length+1));
            count->name=1820487795u;count->type=1564774360u;
            procedures.BookmarkMemberOffset(root+t.offset,&count->offset);
            for(U32 j=0;j<length;++j) {
                count[j+1].name=2974916843u;count[j+1].type=0;
                if(t.offset==412 || t.offset==420) {
                    // Original item Tokenize methods 96970 / 97024: 3 members + sentinel.
                    const U32 armorNames[]={735930353u,2905958645u,912300027u};
                    const U32 armorTypes[]={887689851u,2593517055u,3985361129u};
                    const U32 locoNames[]={2322663157u,45929675u,728601449u};
                    const U32 locoTypes[]={1683514611u,2220692146u,4275178604u};
                    auto nested=static_cast<Procedure*>(procedures.Push(&count[j+1].offset,12,4));
                    for(unsigned k=0;k<3;++k) {
                        nested[k].name=t.offset==412?armorNames[k]:locoNames[k];nested[k].type=t.offset==412?armorTypes[k]:locoTypes[k];
                        procedures.BookmarkMemberOffset(static_cast<unsigned char*>(list->data)+12*j+4*k,&nested[k].offset);
                    }
                    procedures.Pop();
                } else {
                    const unsigned stride=t.offset==40 || t.offset==536?8:4;
                    procedures.BookmarkMemberOffset(static_cast<unsigned char*>(list->data)+stride*j,&count[j+1].offset);
                }
            }
            procedures.Pop();
        }
    }
    if(bigEndian) {
        if(geometry) {
            auto swapShape=[&](unsigned char* shape) {
                const unsigned offsets[]={0,4,8,16,20,24,32,36};for(auto off:offsets) Swap(shape+off);
                auto vector=*reinterpret_cast<float**>(shape+40);if(vector) for(unsigned j=0;j<3;++j) Swap(vector+j);
            };
            auto anchor=*reinterpret_cast<float**>(geometry);if(anchor) for(unsigned j=0;j<2;++j) Swap(anchor+j);
            auto placement=*reinterpret_cast<unsigned char**>(geometry+4);if(placement) swapShape(placement);
            for(U32 j=0;j<geometryShapes->count;++j) swapShape(static_cast<unsigned char*>(geometryShapes->data)+48*j);
            for(U32 j=0;j<contactPoints->count;++j) {auto at=static_cast<unsigned char*>(contactPoints->data)+20*j;Swap(at);for(unsigned k=0;k<3;++k) Swap(at+8+4*k);}
            Swap(&geometryShapes->count);Swap(&contactPoints->count);
        }
        if(crusher) {
            for(unsigned j=0;j<4;++j) Swap(crusher+4*j);
            for(U32 j=0;j<extraLevels->count;++j) Swap(static_cast<unsigned char*>(extraLevels->data)+8*j);
            Swap(&extraLevels->count);
        }
        for(U32 j=0;j<projections->count;++j) {
            auto at=static_cast<unsigned char*>(projections->data)+112*j;
            auto ry=*reinterpret_cast<float**>(at+4);if(ry) Swap(ry);
            Swap(at);for(unsigned off=8;off<=100;off+=4) Swap(at+off);
            if(projectionFilters[j]) EndianObjectFilter(projectionFilters[j]);
        }
        for(U32 i=0;i<browser.size();++i) Swap(&browserStrings[i].length);
        for(auto name:displayNames) if(name) Swap(&name->length);
        if(flammability) for(unsigned i=0;i<4;++i) Swap(flammability+i);
        if(camera) for(unsigned i=0;i<3;++i) Swap(camera+i);
        if(dependency) {
            for(unsigned j=0;j<3;++j) if(dependencyMasks[j]) for(unsigned k=0;k<(j==2?7u:15u);++k) Swap(dependencyMasks[j]+k);
            for(unsigned group=0;group<3;++group) {
                auto list=reinterpret_cast<NativeList*>(dependency+12+8*group);
                for(U32 j=0;j<list->count;++j) Swap(static_cast<U32*>(list->data)+j);
                Swap(&list->count);
            }
            if(dependencyFilter) EndianObjectFilter(dependencyFilter);
        }
        if(buildCosts) {
            for(U32 i=0;i<buildCosts->count*2;++i) Swap(static_cast<U32*>(buildCosts->data)+i);
            Swap(&buildCosts->count);
        }
        for(const auto& entry:autoObjects) for(unsigned group=0;group<2;++group) {
            auto list=reinterpret_cast<NativeList*>(entry.second+4+8*group);
            for(U32 j=0;j<list->count;++j) Swap(static_cast<U32*>(list->data)+j);
            Swap(&list->count);
        }
        for(const auto& leaf:leafLayouts) if(leafObjects.count(leaf.offset))
            for(unsigned i=0;i<leaf.count;++i) if(leaf.fields[i].kind!=LeafValue::Boolean) Swap(leafObjects.at(leaf.offset)+leaf.fields[i].offset);
        for(const auto& entry:lists) if(entry.first!=40) {
            const unsigned stride=entry.first==412 || entry.first==420?12:entry.first==536?8:4;
            auto bytes=static_cast<unsigned char*>(entry.second->data);
            for(U32 i=0;i<entry.second->count;++i) {
                Swap(bytes+stride*i);
                if(stride==12) {Swap(bytes+stride*i+4);Swap(bytes+stride*i+8);}
            }
        }
        // In this restricted surface all words below 576 are scalars, lengths,
        // zero masks/counts, or pointers overwritten by MakeRelocatable.
        for(unsigned at=4;at<576;at+=4) Swap(root+at);
        std::swap(root[576],root[577]);std::swap(root[578],root[579]);
    }
    auto prefix=procedures.MakeRelocatable(bigEndian,0);
    procedures.FixupMemberOffsets(data,prefix,bigEndian);
    auto result=data.MakeRelocatable(bigEndian,static_cast<U32>(prefix.instance.size()));
    if(bigEndian) for(unsigned at=0;at<prefix.instance.size();at+=12) {Swap(prefix.instance.data()+at);Swap(prefix.instance.data()+at+4);}
    prefix.instance.insert(prefix.instance.end(),result.instance.begin(),result.instance.end());result.instance=std::move(prefix.instance);
    result.imports.clear();return result;
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif



