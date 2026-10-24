#include "GameObject.h"
#include "ScalarParsing.h"
#include "DataInitializers.h"
#include "ResourceMarshaling.h"
#include "EnumBitset.h"
#include "ObjectFilter.h"
#include "WeaponSlotOrder.h"
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
enum class AIValue { Float, Signed, Boolean, Time, Angle, Reference, Enum, Bits, NullableFloat, NullableSigned, NullableBoolean, NullableReference, NullableBits };
struct AIEnum {const EnumNameValue* names;unsigned count;int unknown;unsigned bits;};
struct AIField {const char* name;unsigned offset;AIValue kind;const char* fallback;const AIEnum* values;};
struct AILayout {unsigned size;const AIField* fields;unsigned count;const LeafToken* tokens;unsigned tokenCount;};
struct ModuleLayout;
struct ModuleToken {U32 name,type,offset;const ModuleLayout* nested;unsigned mode,stride;};
struct ModuleLayout {const ModuleToken* tokens;unsigned count;};
struct ModuleChoice {U32 type;const ModuleLayout* tokens;unsigned size,kind;};
enum class ModuleValue {Float,Percent,Time,String,Reference,NullableReference,Boolean,Enum,Unsigned,Short,NullablePositiveFloat,NullableModelCondition,ModelCondition,AssetId,SignedList,NullableSigned,NullableObjectStatus,ObjectStatus,Duration,StringList,Angle,DeathBits,Signed,NullableFloat,FloatList,ReplaceOptions,NullableTime,NullableBits,FrameSpeed,Bits};
struct ModuleField {const char* name;unsigned offset;ModuleValue kind;const char* fallback;const AIEnum* values;};
struct ModuleFieldSet {const ModuleField* fields;unsigned count;};
struct ModuleChildName {const char* parent;const char* child;};
#include "GameObjectFields.h"
#include "GameObjectSoundSelectors.h"
#include "GameObjectLifecycle.h"
#include "GameObjectDie.h"
#include "GameObjectUpgrade.h"
#include "GameObjectUpgradeRemaining.h"
#include "GameObjectProduction.h"
#include "GameObjectCreate.h"
#include "GameObjectSlaved.h"
#include "GameObjectTechReceiver.h"
#include "GameObjectVision.h"
#include "GameObjectWeaponBehavior.h"
#include "GameObjectBeamState.h"
#include "GameObjectMotion.h"
#include "GameObjectFlight.h"
#include "GameObjectReactive.h"
#include "GameObjectWeaponSet.h"
#include "GameObjectHealing.h"
#include "GameObjectCrate.h"
#include "GameObjectAIUpdate.h"
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
// The four layouts form one complete AI dependency group. They share the
// existing scalar, bitset, resource and ObjectFilter implementations.
bool NullableAI(AIValue kind) {
    return kind==AIValue::NullableFloat || kind==AIValue::NullableSigned || kind==AIValue::NullableBoolean || kind==AIValue::NullableReference || kind==AIValue::NullableBits;
}
void MarshalAI(const GameObjectChild& input,unsigned char* output,unsigned layoutIndex,Relo::Tracker& data) {
    const auto& layout=aiLayouts[layoutIndex];
    for(unsigned i=0;i<layout.count;++i) {
        const auto& f=layout.fields[i];auto found=input.attributes.find(f.name);
        if(found==input.attributes.end() && !f.fallback) continue;
        std::string value=found==input.attributes.end()?f.fallback:found->second;
        auto at=output+f.offset;bool nullable=NullableAI(f.kind);
        if(nullable) at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(at),f.kind==AIValue::NullableBoolean?1:4,1));
        U32 word=0;float number=0;
        switch(f.kind) {
        case AIValue::NullableBoolean:case AIValue::Boolean:*at=ParseBoolean(value)?1:0;break;
        case AIValue::NullableFloat:case AIValue::Float:number=ParseFloat(value);std::memcpy(at,&number,4);break;
        case AIValue::Time:number=InitializeTime(value.c_str());std::memcpy(at,&number,4);break;
        case AIValue::Angle:number=InitializeAngle(value.c_str());std::memcpy(at,&number,4);break;
        case AIValue::NullableSigned:case AIValue::Signed:word=static_cast<U32>(ParseSigned(value));std::memcpy(at,&word,4);break;
        case AIValue::NullableReference:case AIValue::Reference:MarshalReference(data,at,value);break;
        case AIValue::NullableBits:case AIValue::Bits:MarshalEnumBitset(value,f.values->names,f.values->count,f.values->unknown,f.values->bits,reinterpret_cast<U32*>(at));break;
        case AIValue::Enum:
            word=static_cast<U32>(f.values->unknown);
            for(unsigned j=0;j<f.values->count;++j) if(value==f.values->names[j].name) {word=static_cast<U32>(f.values->names[j].value);break;}
            std::memcpy(at,&word,4);break;
        }
        if(nullable) data.Pop();
    }
    auto filter=[&](const GameObjectChild& child,unsigned offset) {
        ObjectFilterInput in;in.attributes=child.attributes;
        for(const auto& thing:child.children) if(thing.name=="IncludeThing") in.includeThings.push_back(thing.text);else in.excludeThings.push_back(thing.text);
        auto target=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(output+offset),112,1));MarshalObjectFilter(in,target,data);data.Pop();
    };
    if(layoutIndex==0) {
        auto list=reinterpret_cast<NativeList*>(output+60);
        for(const auto& child:input.children) if(child.name=="AIContextualPower") ++list->count;
        if(list->count) {
            auto items=static_cast<unsigned char*>(data.Push(&list->data,172,list->count));unsigned index=0;
            for(const auto& child:input.children) if(child.name=="AIContextualPower") MarshalAI(child,items+172*index++,1,data);
            data.Pop();
        }
        const char* names[]={"ConstructionBaseSelectionCriteria","ResourceNodeBaseClosenessTestSelectionCriteria"};
        for(unsigned i=0;i<2;++i) for(const auto& child:input.children) if(child.name==names[i]) {
            auto target=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+68+4*i),20,1));MarshalAI(child,target,2,data);data.Pop();
        }
        for(const auto& child:input.children) if(child.name=="DodgeWithUnits") filter(child,76);
    } else if(layoutIndex==1) {
        for(const auto& child:input.children) if(child.name=="ObjectFilter") filter(child,156);
        for(const auto& child:input.children) if(child.name=="SpecialPowerWeapon") {
            auto target=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+160),28,1));MarshalAI(child,target,3,data);data.Pop();
        }
    }
}
void TokenizeAI(unsigned char* input,unsigned layoutIndex,void** target,Relo::Tracker& procedures) {
    if(!input) return;
    const auto& layout=aiLayouts[layoutIndex];
    auto table=static_cast<Procedure*>(procedures.Push(target,12,layout.tokenCount+1));
    for(unsigned i=0;i<layout.tokenCount;++i) {
        const auto& t=layout.tokens[i];table[i].name=t.name;table[i].type=t.type;
        if(i<layout.count) {
            auto at=input+t.offset;if(NullableAI(layout.fields[i].kind)) at=*reinterpret_cast<unsigned char**>(at);
            if(at) procedures.BookmarkMemberOffset(at,&table[i].offset);
        } else if(layoutIndex==0 && t.offset==60) {
            auto list=reinterpret_cast<NativeList*>(input+60);
            auto items=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,list->count+1));
            items[0].name=1820487795u;items[0].type=1564774360u;procedures.BookmarkMemberOffset(list,&items[0].offset);
            for(U32 j=0;j<list->count;++j) {items[j+1].name=2974916843u;TokenizeAI(static_cast<unsigned char*>(list->data)+172*j,1,&items[j+1].offset,procedures);}
            procedures.Pop();
        } else if(layoutIndex==0 && (t.offset==68 || t.offset==72)) TokenizeAI(*reinterpret_cast<unsigned char**>(input+t.offset),2,&table[i].offset,procedures);
        else if(layoutIndex==1 && t.offset==160) TokenizeAI(*reinterpret_cast<unsigned char**>(input+160),3,&table[i].offset,procedures);
        else {auto filter=*reinterpret_cast<ObjectFilter**>(input+t.offset);if(filter) TokenizeObjectFilter(filter,&table[i].offset,procedures);}
    }
    procedures.Pop();
}
void EndianAI(unsigned char* input,unsigned layoutIndex) {
    if(!input) return;
    const auto& layout=aiLayouts[layoutIndex];
    for(unsigned i=0;i<layout.count;++i) {
        const auto& f=layout.fields[i];auto at=input+f.offset;
        if(NullableAI(f.kind)) at=*reinterpret_cast<unsigned char**>(at);
        if(!at || f.kind==AIValue::Boolean || f.kind==AIValue::NullableBoolean || f.kind==AIValue::Reference || f.kind==AIValue::NullableReference) continue;
        unsigned words=f.values && f.values->bits?(f.values->bits+31)/32:1;
        for(unsigned j=0;j<words;++j) Swap(at+4*j);
    }
    if(layoutIndex==0) {
        auto list=reinterpret_cast<NativeList*>(input+60);
        for(U32 j=0;j<list->count;++j) EndianAI(static_cast<unsigned char*>(list->data)+172*j,1);
        Swap(&list->count);
        for(unsigned offset=68;offset<=72;offset+=4) EndianAI(*reinterpret_cast<unsigned char**>(input+offset),2);
        auto filter=*reinterpret_cast<ObjectFilter**>(input+76);if(filter) EndianObjectFilter(filter);
    } else if(layoutIndex==1) {
        auto filter=*reinterpret_cast<ObjectFilter**>(input+156);if(filter) EndianObjectFilter(filter);
        EndianAI(*reinterpret_cast<unsigned char**>(input+160),3);
    }
}

U32 ExactStringHash(const std::string& text);
void MarshalSelectorAudio(const GameObjectChild& child,void** slot,Relo::Tracker& data) {
    bool voice=child.name=="AudioArrayVoice";
    auto lists=static_cast<NativeList*>(data.Push(slot,voice?24:8,1));
    const char* names[]={"AudioEntry","ObjectSpecificEntry","NamedEntry"};
    for(unsigned group=0;group<(voice?3u:1u);++group) {
        auto list=lists+group;std::vector<const GameObjectChild*> values;
        for(const auto& entry:child.children) if(!voice || entry.name==names[group]) values.push_back(&entry);
        list->count=static_cast<U32>(values.size());
        if(!list->count) continue;
        unsigned stride=voice?(group==1?16:12):8;
        auto elements=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));
        for(U32 j=0;j<list->count;++j) {
            auto at=elements+stride*j;const auto& attrs=values[j]->attributes;
            if(voice) {
                if(group==2) {auto n=attrs.find("Name");if(n!=attrs.end()) {auto hash=ExactStringHash(n->second);std::memcpy(at+8,&hash,4);}}
                else {auto type=attrs.find("AudioType");if(type!=attrs.end()) {auto word=EnumValue(group==0?"AudioVoiceEntry":"AudioObjectSpecificVoiceEntry",type->second);std::memcpy(at+8,&word,4);}if(group==1) {auto target=attrs.find("TargetObject");if(target!=attrs.end()) {auto hash=HashAssetId(target->second);std::memcpy(at+12,&hash,4);}}}
                auto sound=attrs.find("Sound");if(sound!=attrs.end()) {auto ref=data.Push(reinterpret_cast<void**>(at),4,1);MarshalReference(data,ref,sound->second);data.Pop();}
                auto eva=attrs.find("EvaEvent");MarshalReference(data,at+4,eva==attrs.end()?"":eva->second);
            } else {
                bool sound=child.name=="AudioArraySound";
                auto ref=attrs.find(sound?"Sound":"EvaEvent");if(ref!=attrs.end()) MarshalReference(data,at,ref->second);
                auto type=attrs.find(sound?"AudioType":"EvaType");if(type!=attrs.end()) {auto word=EnumValue(child.name,type->second);std::memcpy(at+4,&word,4);}
            }
        }
        data.Pop();
    }
    data.Pop();
}
void MarshalSelectorPriority(const GameObjectChild& input,unsigned char* at,Relo::Tracker& data) {
    auto value=input.attributes.find("VoicePriorityOverride");if(value==input.attributes.end()) return;
    auto target=static_cast<U32*>(data.Push(reinterpret_cast<void**>(at),4,1));*target=static_cast<U32>(ParseSigned(value->second));data.Pop();
}
void MarshalSelectorAudioChildren(const GameObjectChild& input,unsigned char* output,bool eva,Relo::Tracker& data) {
    const char* names[]={"AudioArrayVoice","AudioArraySound","EvaEventArray"};
    for(unsigned group=eva?2:0;group<(eva?3u:2u);++group)
        for(const auto& child:input.children) if(child.name==names[group]) MarshalSelectorAudio(child,reinterpret_cast<void**>(output+(eva?0:4*group)),data);
}
void MarshalSoundSelector(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    if(kind==13) {
        auto value=input.attributes.find("Chance");float chance=ParseFloat(value==input.attributes.end()?"50":value->second)*0.01f;std::memcpy(output+8,&chance,4);
        MarshalSelectorPriority(input,output+12,data);
        value=input.attributes.find("RerollOnEveryFrame");output[24]=ParseBoolean(value==input.attributes.end()?"false":value->second)?1:0;
        MarshalSelectorAudioChildren(input,output+16,false,data);return;
    }
    auto list=reinterpret_cast<NativeList*>(output+8);list->count=static_cast<U32>(input.children.size());
    if(!list->count) return;
    unsigned stride=kind==14?132:kind==15?148:124;
    auto entries=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));
    for(U32 i=0;i<list->count;++i) {
        const auto& entry=input.children[i];auto at=entries+stride*i;
        const char* names[]={kind==15?"RequiredModelConditions":"RequiredFlags",kind==15?"ConflictingModelConditions":"ExcludedFlags"};
        for(unsigned group=0;group<2;++group) {auto v=entry.attributes.find(names[group]);MarshalModelCondition(v==entry.attributes.end()?"":v->second,reinterpret_cast<U32*>(at+60*group));}
        if(kind!=16) MarshalSelectorPriority(entry,at+120,data);
        MarshalSelectorAudioChildren(entry,at+(kind==16?120:124),kind==16,data);
        if(kind==15) for(unsigned group=0;group<2;++group) {
            auto upgrades=reinterpret_cast<NativeList*>(at+132+8*group);std::vector<const GameObjectChild*> values;
            for(const auto& child:entry.children) if(child.name==(group==0?"RequiredUpgrade":"ConflictingUpgrade")) values.push_back(&child);
            upgrades->count=static_cast<U32>(values.size());
            if(upgrades->count) {auto words=static_cast<U32*>(data.Push(&upgrades->data,4,upgrades->count));for(U32 j=0;j<upgrades->count;++j) words[j]=HashAssetId(values[j]->text);data.Pop();}
        }
    }
    data.Pop();
}
void EndianSelectorAudio(void* input,bool voice) {
    if(!input) return;
    auto lists=static_cast<NativeList*>(input);
    for(unsigned group=0;group<(voice?3u:1u);++group) {
        auto list=lists+group;unsigned stride=voice?(group==1?16:12):8;
        for(U32 j=0;j<list->count;++j) {auto at=static_cast<unsigned char*>(list->data)+stride*j;Swap(at+(voice?8:4));if(voice && group==1) Swap(at+12);}
        Swap(&list->count);
    }
}
void EndianSelectorAudioChildren(unsigned char* input,bool eva) {
    if(eva) EndianSelectorAudio(*reinterpret_cast<void**>(input),false);
    else {EndianSelectorAudio(*reinterpret_cast<void**>(input),true);EndianSelectorAudio(*reinterpret_cast<void**>(input+4),false);}
}
void EndianSoundSelector(unsigned char* output,unsigned kind) {
    if(kind==13) {Swap(output+8);auto priority=*reinterpret_cast<void**>(output+12);if(priority) Swap(priority);EndianSelectorAudioChildren(output+16,false);return;}
    auto list=reinterpret_cast<NativeList*>(output+8);unsigned stride=kind==14?132:kind==15?148:124;
    for(U32 i=0;i<list->count;++i) {
        auto at=static_cast<unsigned char*>(list->data)+stride*i;
        for(unsigned j=0;j<30;++j) Swap(at+4*j);
        if(kind!=16) {auto priority=*reinterpret_cast<void**>(at+120);if(priority) Swap(priority);}
        EndianSelectorAudioChildren(at+(kind==16?120:124),kind==16);
        if(kind==15) for(unsigned group=0;group<2;++group) {auto upgrades=reinterpret_cast<NativeList*>(at+132+8*group);for(U32 j=0;j<upgrades->count;++j) Swap(static_cast<U32*>(upgrades->data)+j);Swap(&upgrades->count);}
    }
    Swap(&list->count);
}

const ModuleChoice* FindModule(U32 type) {
    for(const auto& choice:aiUpdateChoices) if(choice.type==type) return &choice;
    for(const auto& choice:crateChoices) if(choice.type==type) return &choice;
    for(const auto& choice:healingChoices) if(choice.type==type) return &choice;
    if(type==weaponSetChoice.type) return &weaponSetChoice;
    for(const auto& choice:reactiveChoices) if(choice.type==type) return &choice;
    for(const auto& choice:flightChoices) if(choice.type==type) return &choice;
    for(const auto& choice:motionChoices) if(choice.type==type) return &choice;
    for(const auto& choice:beamChoices) if(choice.type==type) return &choice;
    for(const auto& choice:weaponChoices) if(choice.type==type) return &choice;
    for(const auto& choice:visionChoices) if(choice.type==type) return &choice;
    if(type==receiverChoice.type) return &receiverChoice;
    if(type==slavedChoice.type) return &slavedChoice;
    for(const auto& choice:createChoices) if(choice.type==type) return &choice;
    for(const auto& choice:productionChoices) if(choice.type==type) return &choice;
    for(const auto& choice:remainingChoices) if(choice.type==type) return &choice;
    for(const auto& choice:upgradeChoices) if(choice.type==type) return &choice;
    for(const auto& choice:dieChoices) if(choice.type==type) return &choice;
    for(const auto& choice:lifecycleChoices) if(choice.type==type) return &choice;
    for(const auto& choice:selectorChoices) if(choice.type==type) return &choice;
    for(const auto& choice:moduleChoices) if(choice.type==type) return &choice;
    return nullptr;
}
void MarshalModuleFieldSet(const GameObjectChild& input,unsigned char* output,const ModuleFieldSet& fields,Relo::Tracker& data) {
    for(unsigned i=0;i<fields.count;++i) {
        const auto& f=fields.fields[i];
        auto found=input.attributes.find(f.name);if(found==input.attributes.end() && !f.fallback) continue;
        std::string value=found==input.attributes.end()?f.fallback:found->second;auto at=output+f.offset;float number=0;U32 word=0;
        switch(f.kind) {
        case ModuleValue::Float:case ModuleValue::Percent:
            number=ParseFloat(value);if(f.kind==ModuleValue::Percent) number*=0.01f;std::memcpy(at,&number,4);break;
        case ModuleValue::FrameSpeed:number=static_cast<float>(static_cast<double>(weaponSecondsPerLogicFrame)*static_cast<double>(ParseFloat(value)));std::memcpy(at,&number,4);break;
        case ModuleValue::Angle:number=InitializeAngle(value.c_str());std::memcpy(at,&number,4);break;
        case ModuleValue::DeathBits:MarshalEnumBitset(value,lifecycleDeathNames,34,33,34,reinterpret_cast<U32*>(at));break;
        case ModuleValue::Duration: {
            const float scaled=static_cast<float>(static_cast<double>(0.005f)*static_cast<double>(ParseFloat(value)));
            const double wide=static_cast<double>(static_cast<float>(std::ceil(static_cast<double>(scaled))));
            word=(wide>=-9223372036854775808.0 && wide<9223372036854775808.0)?static_cast<U32>(static_cast<std::int64_t>(wide)):0u;
            std::memcpy(at,&word,4);break;
        }
        case ModuleValue::NullableSigned:
            {auto target=static_cast<U32*>(data.Push(reinterpret_cast<void**>(at),4,1));*target=static_cast<U32>(ParseSigned(value));data.Pop();}break;
        case ModuleValue::ObjectStatus:MarshalObjectStatus(value,reinterpret_cast<U32*>(at));break;
        case ModuleValue::NullableObjectStatus:
            {auto words=static_cast<U32*>(data.Push(reinterpret_cast<void**>(at),28,1));MarshalObjectStatus(value,words);data.Pop();}break;
        case ModuleValue::AssetId:word=HashAssetId(value);std::memcpy(at,&word,4);break;
        case ModuleValue::ModelCondition:MarshalModelCondition(value,reinterpret_cast<U32*>(at));break;
        case ModuleValue::NullableModelCondition:
            {auto words=static_cast<U32*>(data.Push(reinterpret_cast<void**>(at),60,1));MarshalModelCondition(value,words);data.Pop();}break;
        case ModuleValue::NullableTime:
            {auto target=static_cast<float*>(data.Push(reinterpret_cast<void**>(at),4,1));*target=InitializeTime(value.c_str());data.Pop();}break;
        case ModuleValue::NullableBits:
            {auto target=static_cast<U32*>(data.Push(reinterpret_cast<void**>(at),4,1));MarshalEnumBitset(value,f.values->names,f.values->count,f.values->unknown,f.values->bits,target);data.Pop();}break;
        case ModuleValue::NullableFloat:
            {auto target=static_cast<float*>(data.Push(reinterpret_cast<void**>(at),4,1));*target=ParseFloat(value);data.Pop();}break;
        case ModuleValue::Bits:
        case ModuleValue::ReplaceOptions:MarshalEnumBitset(value,f.values->names,f.values->count,f.values->unknown,f.values->bits,reinterpret_cast<U32*>(at));break;
        case ModuleValue::NullablePositiveFloat:
            {auto target=static_cast<float*>(data.Push(reinterpret_cast<void**>(at),4,1));*target=ParseFloat(value);if(*target<0.0f) throw GameObjectFormatError("Value should be >= 0");data.Pop();}break;
        case ModuleValue::StringList:case ModuleValue::SignedList:case ModuleValue::FloatList: {
            std::vector<std::string> tokens;std::size_t atText=0;
            auto space=[](unsigned char c) {return c==' ' || (c>=9 && c<=13);};
            while(atText<value.size() && value[atText]) {
                while(atText<value.size() && space(static_cast<unsigned char>(value[atText]))) ++atText;
                auto first=atText;while(atText<value.size() && value[atText] && !space(static_cast<unsigned char>(value[atText]))) ++atText;
                if(atText>first) tokens.push_back(value.substr(first,atText-first));
            }
            auto list=reinterpret_cast<NativeList*>(at);list->count=static_cast<U32>(tokens.size());
            if(list->count) {if(f.kind==ModuleValue::StringList) {auto values=static_cast<ReloString*>(data.Push(&list->data,8,list->count));for(U32 j=0;j<list->count;++j) MarshalString(data,values+j,tokens[j]);}else {auto words=static_cast<U32*>(data.Push(&list->data,4,list->count));for(U32 j=0;j<list->count;++j) if(f.kind==ModuleValue::FloatList) {float n=ParseFloat(tokens[j]);std::memcpy(words+j,&n,4);}else words[j]=static_cast<U32>(ParseSigned(tokens[j]));}data.Pop();}
            break;
        }
        case ModuleValue::Signed:word=static_cast<U32>(ParseSigned(value));std::memcpy(at,&word,4);break;
        case ModuleValue::Unsigned:word=ParseUnsigned(value);std::memcpy(at,&word,4);break;
        case ModuleValue::Short:{auto narrow=static_cast<std::uint16_t>(ParseUnsigned(value));std::memcpy(at,&narrow,2);}break;
        case ModuleValue::Time:number=InitializeTime(value.c_str());std::memcpy(at,&number,4);break;
        case ModuleValue::Boolean:*at=ParseBoolean(value)?1:0;break;
        case ModuleValue::String:MarshalString(data,at,value);break;
        case ModuleValue::Reference:MarshalReference(data,at,value);break;
        case ModuleValue::NullableReference:
            {auto reference=data.Push(reinterpret_cast<void**>(at),4,1);MarshalReference(data,reference,value);data.Pop();}break;
        case ModuleValue::Enum:
            word=static_cast<U32>(f.values->unknown);
            for(unsigned j=0;j<f.values->count;++j) if(value==f.values->names[j].name) {word=static_cast<U32>(f.values->names[j].value);break;}
            std::memcpy(at,&word,4);break;
        }
    }
}
template<std::size_t N>
void MarshalModuleFields(const GameObjectChild& input,unsigned char* output,const ModuleField (&fields)[N],Relo::Tracker& data) {
    ModuleFieldSet set={fields,static_cast<unsigned>(N)};MarshalModuleFieldSet(input,output,set,data);
}
U32 ExactStringHash(const std::string& text);
void MarshalClientUpdate(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,moduleFieldSets[kind],data);
    if(kind==9 || kind==10) {
        auto list=reinterpret_cast<NativeList*>(output+(kind==9?12:8));list->count=static_cast<U32>(input.children.size());
        if(list->count) {
            unsigned stride=kind==9?24:124,fields=kind==9?11:12;
            auto entries=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));
            for(U32 i=0;i<list->count;++i) MarshalModuleFieldSet(input.children[i],entries+stride*i,moduleFieldSets[fields],data);
            data.Pop();
        }
    }
    if(kind==8) {
        auto list=reinterpret_cast<NativeList*>(output+16);list->count=static_cast<U32>(input.children.size());
        if(list->count) {auto keys=static_cast<U32*>(data.Push(&list->data,4,list->count));for(U32 i=0;i<list->count;++i) keys[i]=ExactStringHash(input.children[i].text);data.Pop();}
    }
}
void MarshalBody(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    if(kind==2) MarshalModuleFields(input,output,ShieldBodyModuleDataModuleFields,data);
    MarshalModuleFields(input,output,ActiveBodyModuleDataModuleFields,data);
    auto list=reinterpret_cast<NativeList*>(output+64);list->count=static_cast<U32>(input.children.size());
    if(list->count) {
        auto entries=static_cast<unsigned char*>(data.Push(&list->data,12,list->count));
        for(U32 i=0;i<list->count;++i) MarshalModuleFields(input.children[i],entries+12*i,ActiveBodyDamageRangeModifierModuleFields,data);
        data.Pop();
    }
}
void EndianModuleFieldSet(unsigned char* output,const ModuleFieldSet& fields) {
    for(unsigned i=0;i<fields.count;++i) {
        const auto& f=fields.fields[i];auto at=output+f.offset;
        if(f.kind==ModuleValue::StringList) {auto list=reinterpret_cast<NativeList*>(at);for(U32 j=0;j<list->count;++j) Swap(&static_cast<ReloString*>(list->data)[j].length);Swap(&list->count);}
        else if(f.kind==ModuleValue::Bits) {for(unsigned j=0;j<(f.values->bits+31)/32;++j) Swap(at+4*j);}
        else if(f.kind==ModuleValue::DeathBits) {Swap(at);Swap(at+4);}
        else if(f.kind==ModuleValue::NullableObjectStatus) {auto words=*reinterpret_cast<U32**>(at);if(words) for(unsigned j=0;j<7;++j) Swap(words+j);}
        else if(f.kind==ModuleValue::ObjectStatus) {for(unsigned j=0;j<7;++j) Swap(at+4*j);}
        else if(f.kind==ModuleValue::NullableSigned) {auto word=*reinterpret_cast<U32**>(at);if(word) Swap(word);}
        else if(f.kind==ModuleValue::NullableModelCondition) {auto words=*reinterpret_cast<U32**>(at);if(words) for(unsigned j=0;j<15;++j) Swap(words+j);}
        else if(f.kind==ModuleValue::ModelCondition) {for(unsigned j=0;j<15;++j) Swap(at+4*j);}
        else if((f.kind==ModuleValue::NullablePositiveFloat || f.kind==ModuleValue::NullableFloat || f.kind==ModuleValue::NullableTime || f.kind==ModuleValue::NullableBits)) {auto value=*reinterpret_cast<float**>(at);if(value) Swap(value);}
        else if((f.kind==ModuleValue::SignedList || f.kind==ModuleValue::FloatList)) {auto list=reinterpret_cast<NativeList*>(at);for(U32 j=0;j<list->count;++j) Swap(static_cast<U32*>(list->data)+j);Swap(&list->count);}
        else if(f.kind==ModuleValue::Short) std::swap(at[0],at[1]);
        else if(f.kind!=ModuleValue::Boolean && f.kind!=ModuleValue::Reference && f.kind!=ModuleValue::NullableReference) Swap(at);
    }
}
template<std::size_t N>
void EndianModuleFields(unsigned char* output,const ModuleField (&fields)[N]) {
    ModuleFieldSet set={fields,static_cast<unsigned>(N)};EndianModuleFieldSet(output,set);
}
void MarshalLifecycle(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,lifecycleFieldSets[kind-17],data);
    if(kind==21) for(const auto& child:input.children) {
        auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+20),56,1));
        MarshalModuleFieldSet(child,at,lifecycleFieldSets[6],data);data.Pop();
    }
}
void EndianLifecycle(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,lifecycleFieldSets[kind-17]);
    if(kind==21) {auto at=*reinterpret_cast<unsigned char**>(output+20);if(at) EndianModuleFieldSet(at,lifecycleFieldSets[6]);}
}
void MarshalDie(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,dieFieldSets[kind-23],data);
    if(kind==27) for(unsigned group=0;group<2;++group) {
        auto list=reinterpret_cast<NativeList*>(output+56+8*group);std::vector<const GameObjectChild*> values;
        for(const auto& child:input.children) if(child.name==(group==0?"UpgradeRequired":"UpgradeForbidden")) values.push_back(&child);
        list->count=static_cast<U32>(values.size());
        if(list->count) {auto refs=static_cast<U32*>(data.Push(&list->data,4,list->count));for(U32 j=0;j<list->count;++j) MarshalReference(data,refs+j,values[j]->text);data.Pop();}
    }
    if(kind==29) for(const auto& child:input.children) if(child.name=="ObjectFilter") {
        ObjectFilterInput in;in.attributes=child.attributes;
        for(const auto& entry:child.children) if(entry.name=="IncludeThing") in.includeThings.push_back(entry.text);else in.excludeThings.push_back(entry.text);
        auto filter=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(output+68),112,1));MarshalObjectFilter(in,filter,data);data.Pop();
    }
    for(const auto& child:input.children) if(child.name=="DieMuxData") MarshalModuleFieldSet(child,output+8,dieFieldSets[7],data);
}
void EndianDie(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,dieFieldSets[kind-23]);
    if(kind==27) {Swap(output+56);Swap(output+64);}
    if(kind==29) {auto filter=*reinterpret_cast<ObjectFilter**>(output+68);if(filter) EndianObjectFilter(filter);}
    EndianModuleFieldSet(output+8,dieFieldSets[7]);
}
void MarshalWeakChildren(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);std::vector<const GameObjectChild*> entries;
    for(const auto& child:input.children) if(child.name==name) entries.push_back(&child);
    list->count=static_cast<U32>(entries.size());if(!list->count) return;
    auto ids=static_cast<U32*>(data.Push(&list->data,4,list->count));for(U32 i=0;i<list->count;++i) ids[i]=HashAssetId(entries[i]->text);data.Pop();
}
void EndianWeakChildren(unsigned char* output,unsigned offset) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(U32 i=0;i<list->count;++i) Swap(static_cast<U32*>(list->data)+i);Swap(&list->count);
}
void MarshalUpgradeFilter(const GameObjectChild& child,ObjectFilter* target,Relo::Tracker& data) {
    ObjectFilterInput input;input.attributes=child.attributes;for(const auto& entry:child.children) if(entry.name=="IncludeThing") input.includeThings.push_back(entry.text);else input.excludeThings.push_back(entry.text);MarshalObjectFilter(input,target,data);
}
void MarshalUpgradeAnim(const GameObjectChild& input,unsigned char* output,unsigned offset,Relo::Tracker& data) {
    auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+offset),12,1));MarshalModuleFieldSet(input,at,upgradeFieldSets[11],data);data.Pop();
}
void MarshalUpgradeStruct(const GameObjectChild& input,unsigned char* output,unsigned fields,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,remainingFieldSets[fields],data);
    if(fields==14) for(const auto& child:input.children) {U32 id=HashAssetId(child.text);std::memcpy(output+(child.name=="UpgradeTrigger"?0:4),&id,4);}
    if(fields==13) for(const auto& child:input.children) MarshalModuleFieldSet(child,output+60,remainingFieldSets[15],data);
    if(fields==11) {
        auto list=reinterpret_cast<NativeList*>(output+28);for(const auto& child:input.children) if(child.name=="UpgradeTexture") ++list->count;
        if(list->count) {auto at=static_cast<unsigned char*>(data.Push(&list->data,8,list->count));unsigned i=0;for(const auto& child:input.children) if(child.name=="UpgradeTexture") MarshalModuleFieldSet(child,at+8*i++,remainingFieldSets[12],data);data.Pop();}
        for(const auto& child:input.children) if(child.name=="CustomAnimAndDuration") MarshalUpgradeAnim(child,output,36,data);
    }
}
void EndianUpgradeStruct(unsigned char* output,unsigned fields) {
    EndianModuleFieldSet(output,remainingFieldSets[fields]);
    if(fields==14) {Swap(output);Swap(output+4);}
    if(fields==13) EndianModuleFieldSet(output+60,remainingFieldSets[15]);
    if(fields==11) {Swap(output+28);auto anim=*reinterpret_cast<unsigned char**>(output+36);if(anim) EndianModuleFieldSet(anim,upgradeFieldSets[11]);}
}
void MarshalUpgradeStructList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned stride,unsigned fields,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(list->count) {auto at=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));unsigned i=0;for(const auto& child:input.children) if(child.name==name) MarshalUpgradeStruct(child,at+stride*i++,fields,data);data.Pop();}
}
void EndianUpgradeStructList(unsigned char* output,unsigned offset,unsigned stride,unsigned fields) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(U32 i=0;i<list->count;++i) EndianUpgradeStruct(static_cast<unsigned char*>(list->data)+stride*i,fields);Swap(&list->count);
}
void MarshalRemainingUpgrade(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,remainingFieldSets[kind-41],data);
    if(kind==41) {MarshalUpgradeStructList(input,"AvailableSubStructure",output,36,8,14,data);MarshalUpgradeStructList(input,"StructureOffset",output,44,72,13,data);}
    if(kind==43) MarshalWeakChildren(input,"ReplacementTemplate",output,44,data);
    if(kind==44) MarshalUpgradeStructList(input,"Nugget",output,36,44,11,data);
    if(kind==45) for(const auto& child:input.children) if(child.name=="Offset") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+64),12,1));MarshalModuleFieldSet(child,at,remainingFieldSets[15],data);data.Pop();}
    if(kind==46) MarshalWeakChildren(input,"UpgradeToRemove",output,44,data);
    if(kind==49) {for(const auto& child:input.children) if(child.name=="ObjectFilter") {auto at=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(output+52),112,1));MarshalUpgradeFilter(child,at,data);data.Pop();}MarshalWeakChildren(input,"ApplyToUpgrade",output,56,data);}
    if(kind==50) for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+56),data);
}
void EndianRemainingUpgrade(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,remainingFieldSets[kind-41]);
    if(kind==41) {EndianUpgradeStructList(output,36,8,14);EndianUpgradeStructList(output,44,72,13);}
    if(kind==43) EndianWeakChildren(output,44);
    if(kind==44) EndianUpgradeStructList(output,36,44,11);
    if(kind==45) {auto at=*reinterpret_cast<unsigned char**>(output+64);if(at) EndianModuleFieldSet(at,remainingFieldSets[15]);}
    if(kind==46) EndianWeakChildren(output,44);
    if(kind==49) {auto filter=*reinterpret_cast<ObjectFilter**>(output+52);if(filter) EndianObjectFilter(filter);EndianWeakChildren(output,56);}
    if(kind==50) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+56));
}
void MarshalUpgrade(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    if(kind==60) {
        MarshalModuleFieldSet(input,output,receiverFieldSet,data);
        for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+68),data);
    } else if(kind>=41) MarshalRemainingUpgrade(input,output,kind,data);
    if(kind>30 && kind<=40) MarshalModuleFieldSet(input,output,upgradeFieldSets[kind-30],data);
    MarshalModuleFieldSet(input,output,upgradeFieldSets[0],data);
    for(const auto& child:input.children) if(child.name=="CustomAnimAndDuration") {
        auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+8),12,1));
        MarshalModuleFieldSet(child,at,upgradeFieldSets[11],data);data.Pop();
    }
    for(unsigned group=0;group<2;++group) {
        auto list=reinterpret_cast<NativeList*>(output+12+8*group);std::vector<const GameObjectChild*> values;
        for(const auto& child:input.children) if(child.name==(group==0?"TriggeredBy":"ConflictsWith")) values.push_back(&child);
        list->count=static_cast<U32>(values.size());
        if(list->count) {auto ids=static_cast<U32*>(data.Push(&list->data,4,list->count));for(U32 j=0;j<list->count;++j) ids[j]=HashAssetId(values[j]->text);data.Pop();}
    }
}
void EndianUpgrade(unsigned char* output,unsigned kind) {
    if(kind==60) {EndianModuleFieldSet(output,receiverFieldSet);EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+68));}
    else if(kind>=41) EndianRemainingUpgrade(output,kind);
    if(kind>30 && kind<=40) EndianModuleFieldSet(output,upgradeFieldSets[kind-30]);
    auto anim=*reinterpret_cast<unsigned char**>(output+8);if(anim) EndianModuleFieldSet(anim,upgradeFieldSets[11]);
    for(unsigned group=0;group<2;++group) {auto list=reinterpret_cast<NativeList*>(output+12+8*group);for(U32 j=0;j<list->count;++j) Swap(static_cast<U32*>(list->data)+j);Swap(&list->count);}
}
void MarshalProductionList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned stride,unsigned fields,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(!list->count) return;
    auto at=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));unsigned i=0;
    for(const auto& child:input.children) if(child.name==name) {
        auto entry=at+stride*i++;MarshalModuleFieldSet(child,entry,productionFieldSets[fields],data);
        if(fields==4) for(const auto& filter:child.children) MarshalUpgradeFilter(filter,reinterpret_cast<ObjectFilter*>(entry+12),data);
    }
    data.Pop();
}
void EndianProductionList(unsigned char* output,unsigned offset,unsigned stride,unsigned fields) {
    auto list=reinterpret_cast<NativeList*>(output+offset);
    for(U32 i=0;i<list->count;++i) {auto at=static_cast<unsigned char*>(list->data)+stride*i;EndianModuleFieldSet(at,productionFieldSets[fields]);if(fields==4) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(at+12));}
    Swap(&list->count);
}
void MarshalProduction(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,productionFieldSets[kind-52],data);
    if(kind==52) {
        MarshalProductionList(input,"QuantityModifier",output,60,8,3,data);MarshalProductionList(input,"ProductionModifier",output,68,124,4,data);
        for(const auto& child:input.children) if(child.name=="GiveUpgradeToProduced") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+76),8,1));MarshalProductionList(child,"UpgradeTranslate",at,0,8,5,data);data.Pop();}
    } else for(unsigned i=0;i<2;++i) for(const auto& child:input.children) if(child.name==(i==0?"UnitCreatePoint":"NaturalRallyPoint")) {
        unsigned char* at=output+20+12*i;
        if(kind==53) at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+8+4*i),12,1));
        MarshalModuleFieldSet(child,at,remainingFieldSets[15],data);if(kind==53) data.Pop();
    }
}
void EndianProduction(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,productionFieldSets[kind-52]);
    if(kind==52) {EndianProductionList(output,60,8,3);EndianProductionList(output,68,124,4);auto given=*reinterpret_cast<unsigned char**>(output+76);if(given) EndianProductionList(given,0,8,5);}
    else for(unsigned i=0;i<2;++i) {auto at=kind==53?*reinterpret_cast<unsigned char**>(output+8+4*i):output+20+12*i;if(at) EndianModuleFieldSet(at,remainingFieldSets[15]);}
}
void MarshalCreate(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,createFieldSets[kind-55],data);
    if(kind==56) {for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+12),data);MarshalWeakChildren(input,"Upgrade",output,124,data);}
    if(kind==58) {
        auto list=reinterpret_cast<NativeList*>(output+8);for(const auto& child:input.children) ++list->count;
        if(list->count) {auto refs=static_cast<U32*>(data.Push(&list->data,4,list->count));unsigned i=0;for(const auto& child:input.children) MarshalReference(data,refs+i++,child.text);data.Pop();}
    }
}
void EndianCreate(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,createFieldSets[kind-55]);
    if(kind==56) {EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+12));EndianWeakChildren(output,124);}
    if(kind==58) Swap(output+8);
}
void MarshalSlaved(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,slavedFieldSet,data);
    for(const auto& child:input.children) {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+104),12,1));MarshalModuleFieldSet(child,at,remainingFieldSets[15],data);data.Pop();}
}
void EndianSlaved(unsigned char* output) {
    EndianModuleFieldSet(output,slavedFieldSet);auto at=*reinterpret_cast<unsigned char**>(output+104);if(at) EndianModuleFieldSet(at,remainingFieldSets[15]);
}
void MarshalVision(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,visionFieldSets[kind-61],data);
    if(kind==67) {
        for(const auto& child:input.children) if(child.name=="BroadcastObjectFilter") {auto at=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(output+32),112,1));MarshalUpgradeFilter(child,at,data);data.Pop();}
        for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+36),56,1));MarshalModuleFieldSet(child,at,lifecycleFieldSets[6],data);data.Pop();}
    }
}
void EndianVision(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,visionFieldSets[kind-61]);
    if(kind==67) {auto filter=*reinterpret_cast<ObjectFilter**>(output+32);if(filter) EndianObjectFilter(filter);auto validation=*reinterpret_cast<unsigned char**>(output+36);if(validation) EndianModuleFieldSet(validation,lifecycleFieldSets[6]);}
}
void MarshalWeaponBehaviorList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned fields,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(!list->count) return;
    auto at=static_cast<unsigned char*>(data.Push(&list->data,16,list->count));unsigned i=0;
    for(const auto& child:input.children) if(child.name==name) {
        auto entry=at+16*i++;MarshalModuleFieldSet(child,entry,weaponFieldSets[fields],data);
        if(fields==10) for(const auto& offsetNode:child.children) {auto vector=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(entry+8),12,1));MarshalModuleFieldSet(offsetNode,vector,weaponFieldSets[11],data);data.Pop();}
    }
    data.Pop();
}
void EndianWeaponBehaviorList(unsigned char* output,unsigned offset,unsigned fields) {
    auto list=reinterpret_cast<NativeList*>(output+offset);
    for(U32 i=0;i<list->count;++i) {auto entry=static_cast<unsigned char*>(list->data)+16*i;EndianModuleFieldSet(entry,weaponFieldSets[fields]);if(fields==10) {auto vector=*reinterpret_cast<unsigned char**>(entry+8);if(vector) EndianModuleFieldSet(vector,weaponFieldSets[11]);}}
    Swap(&list->count);
}
unsigned WeaponValidationOffset(unsigned kind) {return kind==69?20:kind==70?24:(kind==74 || kind==75)?32:0;}
void MarshalWeaponBehavior(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,weaponFieldSets[kind-69],data);
    if(kind==70) MarshalWeaponBehaviorList(input,"FireWeaponNugget",output,16,10,data);
    if(kind==75) {
        for(const auto& child:input.children) if(child.name=="Color") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+20),12,1));MarshalModuleFieldSet(child,at,weaponFieldSets[8],data);data.Pop();}
        MarshalWeaponBehaviorList(input,"EffectInfo",output,24,9,data);
    }
    auto offset=WeaponValidationOffset(kind);
    if(offset) for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+offset),56,1));MarshalModuleFieldSet(child,at,lifecycleFieldSets[6],data);data.Pop();}
}
void EndianWeaponBehavior(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,weaponFieldSets[kind-69]);
    if(kind==70) EndianWeaponBehaviorList(output,16,10);
    if(kind==75) {auto color=*reinterpret_cast<unsigned char**>(output+20);if(color) EndianModuleFieldSet(color,weaponFieldSets[8]);EndianWeaponBehaviorList(output,24,9);}
    auto offset=WeaponValidationOffset(kind);if(offset) {auto at=*reinterpret_cast<unsigned char**>(output+offset);if(at) EndianModuleFieldSet(at,lifecycleFieldSets[6]);}
}
void MarshalBeamState(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,beamFieldSets[kind-77],data);
    if(kind==79) return;
    if(kind==78) MarshalModuleFieldSet(input,output,beamFieldSets[0],data);
    for(unsigned group=0;group<2;++group) {
        auto list=reinterpret_cast<NativeList*>(output+20+8*group);const char* name=group==0?"LaserEndParticleSystem":"LaserStartParticleSystem";
        for(const auto& child:input.children) if(child.name==name) ++list->count;
        if(list->count) {auto refs=static_cast<U32*>(data.Push(&list->data,4,list->count));unsigned i=0;for(const auto& child:input.children) if(child.name==name) MarshalReference(data,refs+i++,child.text);data.Pop();}
    }
    for(const auto& child:input.children) if(child.name=="EndOffset") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+36),12,1));MarshalModuleFieldSet(child,at,remainingFieldSets[15],data);data.Pop();}
    for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+40),56,1));MarshalModuleFieldSet(child,at,lifecycleFieldSets[6],data);data.Pop();}
}
void EndianBeamState(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,beamFieldSets[kind-77]);if(kind==79) return;
    if(kind==78) EndianModuleFieldSet(output,beamFieldSets[0]);
    Swap(output+20);Swap(output+28);
    auto vector=*reinterpret_cast<unsigned char**>(output+36);if(vector) EndianModuleFieldSet(vector,remainingFieldSets[15]);
    auto validation=*reinterpret_cast<unsigned char**>(output+40);if(validation) EndianModuleFieldSet(validation,lifecycleFieldSets[6]);
}
int MotionMoveIndex(U32 type) {for(unsigned i=0;i<4;++i) if(motionMoveChoices[i].type==type) return static_cast<int>(i);return -1;}
const char* motionShapeNames[]={"Sphere","Capsule","Box","Cylinder","Triangle"};
const unsigned motionShapeSizes[]={12,16,24,20,48};
void MarshalMotion(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,motionFieldSets[kind-80],data);
    if(kind==80) for(const auto& wrapper:input.children) if(wrapper.name=="Movement" && wrapper.children.size()==1) {
        const auto& move=wrapper.children[0];auto id=move.attributes.find("TypeId");U32 type=id==move.attributes.end()?0:ParseUnsigned(id->second);int index=MotionMoveIndex(type);
        auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+24),index<0?4:motionMoveChoices[index].size,1));
        if(index>=0 && index<3) MarshalModuleFieldSet(move,at,motionFieldSets[4+index],data);
        if(index==2) {auto list=reinterpret_cast<NativeList*>(at+16);list->count=static_cast<U32>(move.children.size());if(list->count) {auto refs=static_cast<U32*>(data.Push(&list->data,4,list->count));for(U32 i=0;i<list->count;++i) MarshalReference(data,refs+i,move.children[i].text);data.Pop();}}
        *reinterpret_cast<U32*>(at)=index<0?0:type;data.Pop();
    }
    if(kind==81) for(const auto& volume:input.children) if(volume.name=="Volume") {
        auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+28),76,1));MarshalModuleFieldSet(volume,at,motionFieldSets[7],data);
        for(unsigned group=0;group<5;++group) {
            auto list=reinterpret_cast<NativeList*>(at+32+8*group);for(const auto& shape:volume.children) if(shape.name==motionShapeNames[group]) ++list->count;
            if(!list->count) continue;
            auto shapes=static_cast<unsigned char*>(data.Push(&list->data,motionShapeSizes[group],list->count));unsigned i=0;
            for(const auto& shape:volume.children) if(shape.name==motionShapeNames[group]) {
                auto entry=shapes+motionShapeSizes[group]*i++;MarshalModuleFieldSet(shape,entry,motionFieldSets[8+group],data);
                for(unsigned transform=0;transform<2;++transform) for(const auto& child:shape.children) if(child.name==(transform==0?"Translation":"Rotation")) {auto vector=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(entry+4*transform),transform==0?12:16,1));MarshalModuleFieldSet(child,vector,motionFieldSets[13+transform],data);data.Pop();}
            }
            data.Pop();
        }
        data.Pop();
    }
}
void EndianMotion(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,motionFieldSets[kind-80]);
    if(kind==80) {auto at=*reinterpret_cast<unsigned char**>(output+24);if(at) {int index=MotionMoveIndex(*reinterpret_cast<U32*>(at));if(index>=0 && index<3) EndianModuleFieldSet(at,motionFieldSets[4+index]);if(index==2) Swap(at+16);Swap(at);}}
    if(kind==81) {auto at=*reinterpret_cast<unsigned char**>(output+28);if(at) {
        EndianModuleFieldSet(at,motionFieldSets[7]);
        for(unsigned group=0;group<5;++group) {auto list=reinterpret_cast<NativeList*>(at+32+8*group);
            for(U32 i=0;i<list->count;++i) {auto entry=static_cast<unsigned char*>(list->data)+motionShapeSizes[group]*i;EndianModuleFieldSet(entry,motionFieldSets[8+group]);for(unsigned transform=0;transform<2;++transform) {auto vector=*reinterpret_cast<unsigned char**>(entry+4*transform);if(vector) EndianModuleFieldSet(vector,motionFieldSets[13+transform]);}}
            Swap(&list->count);
        }
    }}
}
void TokenizeMotionMove(U32* input,void** slot,Relo::Tracker& procedures);
void MarshalFlight(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,flightFieldSets[kind-84],data);
    if(kind==86) MarshalModuleFieldSet(input,output,flightFieldSets[1],data);
}
void EndianFlight(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,flightFieldSets[kind-84]);if(kind==86) EndianModuleFieldSet(output,flightFieldSets[1]);
}
void MarshalReactiveList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned fields,unsigned stride,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(!list->count) return;auto at=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));unsigned i=0;for(const auto& child:input.children) if(child.name==name) MarshalModuleFieldSet(child,at+stride*i++,reactiveFieldSets[fields],data);data.Pop();
}
void EndianReactiveList(unsigned char* output,unsigned offset,unsigned fields,unsigned stride) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(U32 i=0;i<list->count;++i) EndianModuleFieldSet(static_cast<unsigned char*>(list->data)+stride*i,reactiveFieldSets[fields]);Swap(&list->count);
}
void MarshalReactive(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,reactiveFieldSets[kind-87],data);
    if(kind==87) MarshalReactiveList(input,"Data",output,8,4,96,data);
    if(kind==90) {
        for(const auto& child:input.children) if(child.name=="WeaponOffset") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+80),12,1));MarshalModuleFieldSet(child,at,remainingFieldSets[15],data);data.Pop();}
        for(const auto& child:input.children) if(child.name=="DieMuxData") MarshalModuleFieldSet(child,output+84,dieFieldSets[7],data);
        MarshalReactiveList(input,"WeaponFireProbability",output,128,5,12,data);
    }
    if(kind==88 || kind==90) MarshalUpgrade(input,output,30,data);
}
void EndianReactive(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,reactiveFieldSets[kind-87]);
    if(kind==87) EndianReactiveList(output,8,4,96);
    if(kind==90) {auto at=*reinterpret_cast<unsigned char**>(output+80);if(at) EndianModuleFieldSet(at,remainingFieldSets[15]);EndianModuleFieldSet(output+84,dieFieldSets[7]);EndianReactiveList(output,128,5,12);}
    if(kind==88 || kind==90) EndianUpgrade(output,30);
}
// SortedListHelper.Sort builds an EASTL pointer permutation, then exchanges
// bookmarks before each 72-byte record swap. XML and import creation order stay
// unchanged. Keep the inverse map as the original ptr3 array does.
void SortWeaponData(unsigned char* output,U32 count,Relo::Tracker& data) {
    std::vector<int> keys(count);for(U32 i=0;i<count;++i) std::memcpy(&keys[i],output+72*i,4);
    auto order=WeaponSlotOrder(keys);std::vector<U32> current(count),position(count);
    for(U32 i=0;i<count;++i) current[i]=position[i]=i;
    for(U32 i=0;i<count;++i) {
        U32 j=position[order[i]];if(i==j) continue;
        auto a=output+72*j,b=output+72*i;
        if(!data.ExchangePointers(a,b)) throw std::logic_error("WeaponData outside current Tracker block");
        unsigned char temporary[72];std::memcpy(temporary,b,72);std::memcpy(b,a,72);std::memcpy(a,temporary,72);
        std::swap(current[i],current[j]);position[current[i]]=i;position[current[j]]=j;
    }
}
void MarshalWeaponSetNode(const GameObjectChild& input,unsigned char* output,unsigned fields,Relo::Tracker& data);
void MarshalWeaponSetList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned fields,unsigned stride,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(!list->count) return;
    auto at=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));unsigned i=0;
    for(const auto& child:input.children) if(child.name==name) MarshalWeaponSetNode(child,at+stride*i++,fields,data);
    if(fields==5) SortWeaponData(at,list->count,data);
    data.Pop();
}
void MarshalWeaponSetNode(const GameObjectChild& input,unsigned char* output,unsigned fields,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,weaponSetFieldSets[fields],data);
    if(fields==0) {
        const char* names[]={"WeaponSlotHardpoint","WeaponSlotTurret","WeaponSlotHierarchicalTurret","WeaponSlotContained"};const unsigned strides[]={28,124,128,28};
        for(unsigned i=0;i<4;++i) MarshalWeaponSetList(input,names[i],output,8+8*i,1+i,strides[i],data);
    }
    if(fields==1) MarshalWeaponSetList(input,"Weapon",output,16,5,72,data);
    if(fields==2) {
        for(const auto& child:input.children) if(child.name=="TurretSettings") MarshalWeaponSetNode(child,output+28,6,data);
        MarshalWeaponSetNode(input,output,1,data);
    }
    if(fields==3) MarshalWeaponSetNode(input,output,2,data);
    if(fields==4) MarshalWeaponSetNode(input,output,1,data);
    if(fields==6) for(const auto& child:input.children) if(child.name=="TurretAITargetChooserData") {
        auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+88),44,1));MarshalWeaponSetNode(child,at,7,data);data.Pop();
    }
    if(fields==7) MarshalWeaponSetNode(input,output,8,data);
    if(fields==8) MarshalWeaponSetList(input,"ObjectVisionRange",output,20,9,120,data);
    if(fields==9) for(const auto& child:input.children) if(child.name=="Filter") {
        ObjectFilterInput filter;filter.attributes=child.attributes;
        for(const auto& thing:child.children) if(thing.name=="IncludeThing") filter.includeThings.push_back(thing.text);else filter.excludeThings.push_back(thing.text);
        MarshalObjectFilter(filter,reinterpret_cast<ObjectFilter*>(output+8),data);
    }
}
void EndianWeaponSetNode(unsigned char* output,unsigned fields) {
    EndianModuleFieldSet(output,weaponSetFieldSets[fields]);
    auto list=[&](unsigned offset,unsigned child,unsigned stride) {
        auto values=reinterpret_cast<NativeList*>(output+offset);for(U32 i=0;i<values->count;++i) EndianWeaponSetNode(static_cast<unsigned char*>(values->data)+stride*i,child);Swap(&values->count);
    };
    if(fields==0) {const unsigned strides[]={28,124,128,28};for(unsigned i=0;i<4;++i) list(8+8*i,1+i,strides[i]);}
    if(fields==1) {Swap(output);list(16,5,72);}
    if(fields==2) {EndianWeaponSetNode(output+28,6);EndianWeaponSetNode(output,1);}
    if(fields==3) EndianWeaponSetNode(output,2);
    if(fields==4) EndianWeaponSetNode(output,1);
    if(fields==6) {auto chooser=*reinterpret_cast<unsigned char**>(output+88);if(chooser) EndianWeaponSetNode(chooser,7);}
    if(fields==7) EndianWeaponSetNode(output,8);
    if(fields==8) list(20,9,120);
    if(fields==9) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+8));
}
void MarshalHealing(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,healingFieldSets[kind-92],data);
    if(kind==92) {
        for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+100),56,1));MarshalModuleFieldSet(child,at,lifecycleFieldSets[6],data);data.Pop();}
        MarshalUpgrade(input,output,30,data);
    }
    if(kind==93) for(const auto& child:input.children) {if(child.name=="PercentOfBuildCostToRebuildInfo") MarshalModuleFieldSet(child,output+16,healingFieldSets[5],data);if(child.name=="SelfAudio") MarshalModuleFieldSet(child,output+32,healingFieldSets[6],data);}
    if(kind==94) for(const auto& child:input.children) if(child.name=="Die") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+28),44,1));MarshalModuleFieldSet(child,at,dieFieldSets[7],data);data.Pop();}
    if(kind==95) {
        for(const auto& child:input.children) if(child.name=="ObjectFilter") {auto at=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(output+112),112,1));MarshalUpgradeFilter(child,at,data);data.Pop();}
        MarshalModuleFieldSet(input,output,healingFieldSets[4],data);
    }
}
void EndianHealing(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,healingFieldSets[kind-92]);
    if(kind==92) {auto at=*reinterpret_cast<unsigned char**>(output+100);if(at) EndianModuleFieldSet(at,lifecycleFieldSets[6]);EndianUpgrade(output,30);}
    if(kind==93) {EndianModuleFieldSet(output+16,healingFieldSets[5]);EndianModuleFieldSet(output+32,healingFieldSets[6]);}
    if(kind==94) {auto at=*reinterpret_cast<unsigned char**>(output+28);if(at) EndianModuleFieldSet(at,dieFieldSets[7]);}
    if(kind==95) {auto at=*reinterpret_cast<ObjectFilter**>(output+112);if(at) EndianObjectFilter(at);EndianModuleFieldSet(output,healingFieldSets[4]);}
}
void MarshalCrate(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned group=kind-96;MarshalModuleFieldSet(input,output,crateFieldSets[group],data);
    if(group>0 && group<7) MarshalModuleFieldSet(input,output,crateFieldSets[0],data);
    if(group==7) {
        auto list=reinterpret_cast<NativeList*>(output+36);for(const auto& child:input.children) if(child.name=="CrateList") ++list->count;
        if(list->count) {auto at=static_cast<U32*>(data.Push(&list->data,4,list->count));unsigned i=0;for(const auto& child:input.children) if(child.name=="CrateList") MarshalReference(data,at+i++,child.text);data.Pop();}
        for(const auto& child:input.children) if(child.name=="BlockingUnits") {auto at=static_cast<ObjectFilter*>(data.Push(reinterpret_cast<void**>(output+44),112,1));MarshalUpgradeFilter(child,at,data);data.Pop();}
    }
}
void EndianCrate(unsigned char* output,unsigned kind) {
    unsigned group=kind-96;EndianModuleFieldSet(output,crateFieldSets[group]);
    if(group>0 && group<7) EndianModuleFieldSet(output,crateFieldSets[0]);
    if(group==7) {Swap(output+36);auto at=*reinterpret_cast<ObjectFilter**>(output+44);if(at) EndianObjectFilter(at);}
}
// Concrete fields precede the AI base, matching original reference/allocation order.
void MarshalAIUpdate(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned group=kind-104;
    if(group) MarshalModuleFieldSet(input,output,aiUpdateFieldSets[group],data);
    MarshalModuleFieldSet(input,output,aiUpdateFieldSets[0],data);
    for(const auto& child:input.children) if(child.name=="UnitAITargetChooserData") {
        auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+68),40,1));
        MarshalModuleFieldSet(child,at,aiUpdateFieldSets[8],data);
        MarshalWeaponSetNode(child,at,8,data);data.Pop();
    }
}
void EndianAIUpdate(unsigned char* output,unsigned kind) {
    unsigned group=kind-104;
    if(group) EndianModuleFieldSet(output,aiUpdateFieldSets[group]);
    EndianModuleFieldSet(output,aiUpdateFieldSets[0]);
    auto chooser=*reinterpret_cast<unsigned char**>(output+68);
    if(chooser) EndianWeaponSetNode(chooser,8);
}
void EndianModule(U32* input) {
    auto choice=FindModule(input[0]);
    if(choice && choice->kind>=104 && choice->kind<=111) EndianAIUpdate(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=96 && choice->kind<=103) EndianCrate(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=92 && choice->kind<=95) EndianHealing(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind==91) EndianWeaponSetNode(reinterpret_cast<unsigned char*>(input),0);
    if(choice && choice->kind && choice->kind<=2) {
        auto bytes=reinterpret_cast<unsigned char*>(input);
        if(choice->kind==2) EndianModuleFields(bytes,ShieldBodyModuleDataModuleFields);
        EndianModuleFields(bytes,ActiveBodyModuleDataModuleFields);
        auto list=reinterpret_cast<NativeList*>(bytes+64);
        for(U32 i=0;i<list->count;++i) {auto at=static_cast<unsigned char*>(list->data)+12*i;Swap(at);Swap(at+4);}
        Swap(&list->count);
    }
    if(choice && choice->kind>=3 && choice->kind<13) {
        auto bytes=reinterpret_cast<unsigned char*>(input);EndianModuleFieldSet(bytes,moduleFieldSets[choice->kind]);
        if(choice->kind==9 || choice->kind==10) {
            auto list=reinterpret_cast<NativeList*>(bytes+(choice->kind==9?12:8));unsigned stride=choice->kind==9?24:124,fields=choice->kind==9?11:12;
            for(U32 j=0;j<list->count;++j) EndianModuleFieldSet(static_cast<unsigned char*>(list->data)+stride*j,moduleFieldSets[fields]);
            Swap(&list->count);
        }
        if(choice->kind==8) {auto list=reinterpret_cast<NativeList*>(bytes+16);for(U32 i=0;i<list->count;++i) Swap(static_cast<U32*>(list->data)+i);Swap(&list->count);}
    }
    if(choice && choice->kind>=13 && choice->kind<=16) EndianSoundSelector(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=17 && choice->kind<=22) EndianLifecycle(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=23 && choice->kind<=29) EndianDie(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=87 && choice->kind<=90) EndianReactive(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=84 && choice->kind<=86) EndianFlight(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=80 && choice->kind<=83) EndianMotion(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=77 && choice->kind<=79) EndianBeamState(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=69 && choice->kind<=76) EndianWeaponBehavior(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=61 && choice->kind<=68) EndianVision(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind==59) EndianSlaved(reinterpret_cast<unsigned char*>(input));
    if(choice && choice->kind>=55 && choice->kind<=58) EndianCreate(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && choice->kind>=52 && choice->kind<=54) EndianProduction(reinterpret_cast<unsigned char*>(input),choice->kind);
    if(choice && ((choice->kind>=30 && choice->kind<=51) || choice->kind==60)) EndianUpgrade(reinterpret_cast<unsigned char*>(input),choice->kind);
    Swap(input);Swap(input+1);
}

void MarshalModule(const GameObjectChild& input,void** slot,Relo::Tracker& data) {
    auto found=input.attributes.find("TypeId");U32 type=found==input.attributes.end()?0:ParseUnsigned(found->second);
    if(!FindModule(type)) for(auto known:knownModuleTypes) if(known==type) throw std::invalid_argument("Unsupported polymorphic GameObject module TypeId");
    auto choice=FindModule(type);
    auto target=static_cast<U32*>(data.Push(slot,choice?choice->size:8,1));
    if(choice && choice->kind && choice->kind<=2) MarshalBody(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=3 && choice->kind<13) MarshalClientUpdate(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=13 && choice->kind<=16) MarshalSoundSelector(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=17 && choice->kind<=22) MarshalLifecycle(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=23 && choice->kind<=29) MarshalDie(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=104 && choice->kind<=111) MarshalAIUpdate(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=96 && choice->kind<=103) MarshalCrate(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=92 && choice->kind<=95) MarshalHealing(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind==91) MarshalWeaponSetNode(input,reinterpret_cast<unsigned char*>(target),0,data);
    if(choice && choice->kind>=87 && choice->kind<=90) MarshalReactive(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=84 && choice->kind<=86) MarshalFlight(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=80 && choice->kind<=83) MarshalMotion(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=77 && choice->kind<=79) MarshalBeamState(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=69 && choice->kind<=76) MarshalWeaponBehavior(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=61 && choice->kind<=68) MarshalVision(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind==59) MarshalSlaved(input,reinterpret_cast<unsigned char*>(target),data);
    if(choice && choice->kind>=55 && choice->kind<=58) MarshalCreate(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && choice->kind>=52 && choice->kind<=54) MarshalProduction(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    if(choice && ((choice->kind>=30 && choice->kind<=51) || choice->kind==60)) MarshalUpgrade(input,reinterpret_cast<unsigned char*>(target),choice->kind,data);
    target[0]=choice?type:0;
    auto id=input.attributes.find("id");if(id!=input.attributes.end()) target[1]=HashAssetId(id->second);
    data.Pop();
}
void TokenizeModuleTable(U32* input,const ModuleLayout& layout,void** slot,Relo::Tracker& procedures) {
    auto table=static_cast<Procedure*>(procedures.Push(slot,12,layout.count+1));
    for(unsigned i=0;i<layout.count;++i) {
        const auto& t=layout.tokens[i];table[i].name=t.name;table[i].type=t.type;
        if(t.mode==1 || t.mode==3 || t.mode==6) {
            auto list=reinterpret_cast<NativeList*>(reinterpret_cast<unsigned char*>(input)+t.offset);
            auto count=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,list->count+1));
            count[0].name=1820487795u;count[0].type=1564774360u;procedures.BookmarkMemberOffset(list,&count[0].offset);
            for(U32 j=0;j<list->count;++j) {
                count[j+1].name=2974916843u;
                if(t.mode==1) TokenizeModuleTable(reinterpret_cast<U32*>(static_cast<unsigned char*>(list->data)+t.stride*j),*t.nested,&count[j+1].offset,procedures);
                else procedures.BookmarkMemberOffset(static_cast<unsigned char*>(list->data)+(t.mode==6?8:4)*j,&count[j+1].offset);
            }
            procedures.Pop();
        } else if(t.mode==9) {
            TokenizeMotionMove(*reinterpret_cast<U32**>(reinterpret_cast<unsigned char*>(input)+t.offset),&table[i].offset,procedures);
        } else if(t.mode==8) {
            TokenizeObjectFilter(reinterpret_cast<ObjectFilter*>(reinterpret_cast<unsigned char*>(input)+t.offset),&table[i].offset,procedures);
        } else if(t.mode==7) {
            auto filter=*reinterpret_cast<ObjectFilter**>(reinterpret_cast<unsigned char*>(input)+t.offset);if(filter) TokenizeObjectFilter(filter,&table[i].offset,procedures);
        } else if(t.mode==4) {
            auto pointee=*reinterpret_cast<U32**>(reinterpret_cast<unsigned char*>(input)+t.offset);if(pointee) TokenizeModuleTable(pointee,*t.nested,&table[i].offset,procedures);
        } else if(t.mode==2) {
            auto pointee=*reinterpret_cast<void**>(reinterpret_cast<unsigned char*>(input)+t.offset);if(pointee) procedures.BookmarkMemberOffset(pointee,&table[i].offset);
        } else if(t.nested) TokenizeModuleTable(reinterpret_cast<U32*>(reinterpret_cast<unsigned char*>(input)+t.offset),*t.nested,&table[i].offset,procedures);
        else procedures.BookmarkMemberOffset(reinterpret_cast<unsigned char*>(input)+t.offset,&table[i].offset);
    }
    procedures.Pop();
}
void TokenizeMotionMove(U32* input,void** slot,Relo::Tracker& procedures) {
    if(!input) return;auto wrapper=static_cast<Procedure*>(procedures.Push(slot,12,1));wrapper->name=2;wrapper->type=input[0];int index=MotionMoveIndex(input[0]);
    if(index>=0) TokenizeModuleTable(input,*motionMoveChoices[index].tokens,&wrapper->offset,procedures);procedures.Pop();
}
void TokenizeModule(U32* input,void** slot,Relo::Tracker& procedures) {
    if(!input) return;
    // Polymorphic wrapper has exactly one record, without a sentinel.
    auto wrapper=static_cast<Procedure*>(procedures.Push(slot,12,1));wrapper->name=2;wrapper->type=input[0];
    if(auto choice=FindModule(input[0])) TokenizeModuleTable(input,*choice->tokens,&wrapper->offset,procedures);
    procedures.Pop();
}
void MarshalModuleList(const GameObjectChild& input,NativeList* list,Relo::Tracker& data) {
    list->count=static_cast<U32>(input.children.size());
    if(!list->count) return;
    auto slots=static_cast<U32**>(data.Push(&list->data,4,list->count));
    for(U32 i=0;i<list->count;++i) MarshalModule(input.children[i],reinterpret_cast<void**>(slots+i),data);
    data.Pop();
}
void TokenizeModuleList(NativeList* list,void** slot,Relo::Tracker& procedures) {
    auto table=static_cast<Procedure*>(procedures.Push(slot,12,list->count+1));
    table[0].name=1820487795u;table[0].type=1564774360u;procedures.BookmarkMemberOffset(list,&table[0].offset);
    for(U32 i=0;i<list->count;++i) {table[i+1].name=2974916843u;TokenizeModule(static_cast<U32**>(list->data)[i],&table[i+1].offset,procedures);}
    procedures.Pop();
}

// Original AudioVoiceReferentialEntry hashes Name without asset-ID lowercasing.
U32 ExactStringHash(const std::string& text) {
    U32 size=static_cast<U32>(std::strlen(text.c_str())),hash=size;if(!size) return 0;
    auto p=reinterpret_cast<const unsigned char*>(text.data());
    auto shortWord=[](const unsigned char* b)->U32 {return b[0]|(U32(b[1])<<8);};
    auto signedByte=[](unsigned char b)->U32 {return static_cast<U32>(static_cast<std::int8_t>(b));};
    for(U32 n=size/4;n;--n,p+=4) {hash+=shortWord(p);hash^=(shortWord(p+2)^(hash<<5))<<11;hash+=hash>>11;}
    switch(size&3) {
    case 3:hash+=shortWord(p);hash^=((signedByte(p[2])<<2)^hash)<<16;hash+=hash>>11;break;
    case 2:hash+=shortWord(p);hash^=hash<<11;hash+=hash>>17;break;
    case 1:hash+=signedByte(*p);hash^=hash<<10;hash+=hash>>1;break;
    }
    hash^=hash<<3;hash+=hash>>5;hash^=hash<<2;hash+=hash>>15;return (hash<<10)^hash;
}
}
const GameObjectField* GameObjectFields() { return fields; }
unsigned GameObjectFieldCount() { return sizeof(fields)/sizeof(fields[0]); }
bool GameObjectLeafAttributeSupported(const std::string& name,const std::string& attribute) {
    for(const auto& leaf:leafLayouts) if(name==leaf.name)
        for(unsigned i=0;i<leaf.count;++i) if(attribute==leaf.fields[i].name) return true;
    return false;
}
bool GameObjectAIAttributeSupported(const std::string& name,const std::string& attribute) {
    unsigned index=name=="SkirmishAIInformation"?0:name=="AIContextualPower"?1:name=="SpecialPowerWeapon"?3:2;
    if(index==1 && attribute=="id") return true;
    const auto& layout=aiLayouts[index];for(unsigned i=0;i<layout.count;++i) if(attribute==layout.fields[i].name) return true;
    return false;
}
unsigned GameObjectSoundSelectorKind(const std::string& typeId) {
    auto choice=FindModule(ParseUnsigned(typeId));return choice && choice->kind>=13 && choice->kind<=16?choice->kind:0;
}
bool GameObjectModuleAttributeSupported(const std::string& typeId,const std::string& attribute) {
    auto choice=FindModule(ParseUnsigned(typeId));if(!choice || !choice->kind) return false;
    if(choice->kind>=104 && choice->kind<=111) {
        for(const auto& set:{aiUpdateFieldSets[choice->kind-104],aiUpdateFieldSets[0]}) for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;
        return false;
    }
    if(choice->kind>=96 && choice->kind<=103) {
        unsigned group=choice->kind-96;const auto& own=crateFieldSets[group];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;
        if(group>0 && group<7) for(unsigned i=0;i<crateFieldSets[0].count;++i) if(attribute==crateFieldSets[0].fields[i].name) return true;return false;
    }
    if(choice->kind>=92 && choice->kind<=95) {
        const auto& own=healingFieldSets[choice->kind-92];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;
        const auto& base=choice->kind==92?upgradeFieldSets[0]:healingFieldSets[4];if(choice->kind==92 || choice->kind==95) for(unsigned i=0;i<base.count;++i) if(attribute==base.fields[i].name) return true;return false;
    }
    if(choice->kind==91) return false;
    if(choice->kind>=87) {const auto& set=reactiveFieldSets[choice->kind-87];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;if(choice->kind==88 || choice->kind==90) for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind>=84) {const auto& set=flightFieldSets[choice->kind-84];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;if(choice->kind==86) for(unsigned i=0;i<flightFieldSets[1].count;++i) if(attribute==flightFieldSets[1].fields[i].name) return true;return false;}
    if(choice->kind>=80) {const auto& set=motionFieldSets[choice->kind-80];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=77) {const auto& set=beamFieldSets[choice->kind-77];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;if(choice->kind==78) for(unsigned i=0;i<beamFieldSets[0].count;++i) if(attribute==beamFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind>=69) {const auto& set=weaponFieldSets[choice->kind-69];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=61) {const auto& set=visionFieldSets[choice->kind-61];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind==60) {for(unsigned i=0;i<receiverFieldSet.count;++i) if(attribute==receiverFieldSet.fields[i].name) return true;for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind==59) {for(unsigned i=0;i<slavedFieldSet.count;++i) if(attribute==slavedFieldSet.fields[i].name) return true;return false;}
    if(choice->kind>=55) {const auto& set=createFieldSets[choice->kind-55];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=52) {const auto& set=productionFieldSets[choice->kind-52];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=30) {const auto& own=choice->kind>=41?remainingFieldSets[choice->kind-41]:upgradeFieldSets[choice->kind-30];for(unsigned i=0;i<own.count;++i) if(attribute==own.fields[i].name) return true;for(unsigned i=0;i<upgradeFieldSets[0].count;++i) if(attribute==upgradeFieldSets[0].fields[i].name) return true;return false;}
    if(choice->kind>=23) {const auto& set=dieFieldSets[choice->kind-23];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=17) {const auto& set=lifecycleFieldSets[choice->kind-17];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=13) return choice->kind==13 && (attribute=="Chance" || attribute=="VoicePriorityOverride" || attribute=="RerollOnEveryFrame");
    const auto& set=moduleFieldSets[choice->kind];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;
    if(choice->kind==2) for(const auto& f:ActiveBodyModuleDataModuleFields) if(attribute==f.name) return true;
    return false;
}
bool GameObjectModuleNestedAttributeSupported(const std::string& typeId,const std::string& name,const std::string& attribute) {
    auto choice=FindModule(ParseUnsigned(typeId));if(!choice) return false;
    if(choice->kind>=30 && choice->kind<=51 && name=="CustomAnimAndDuration") {const auto& set=upgradeFieldSets[11];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind>=23 && choice->kind<=29 && name=="DieMuxData") {const auto& set=dieFieldSets[7];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(choice->kind==21 && name=="ObjectStatusValidation") return attribute=="ForbiddenStatus" || attribute=="RequiredStatus";
    unsigned fields=choice->kind==9 && name=="Sound"?11:choice->kind==10 && name=="ModelConditionSound"?12:0;
    const auto& set=moduleFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;
    return false;
}
bool GameObjectModuleNestedSupported(const std::string& typeId,const std::string& name) {
    auto choice=FindModule(ParseUnsigned(typeId));return choice && ((choice->kind>=1 && choice->kind<=2 && name=="DamageRangeModifier") || (choice->kind==8 && name=="Key") || (choice->kind==9 && name=="Sound") || (choice->kind==10 && name=="ModelConditionSound") || (choice->kind==21 && name=="ObjectStatusValidation") || (choice->kind>=23 && choice->kind<=29 && name=="DieMuxData") || (choice->kind==27 && (name=="UpgradeRequired" || name=="UpgradeForbidden")) || (choice->kind==29 && name=="ObjectFilter") || (choice->kind>=30 && choice->kind<=51 && (name=="CustomAnimAndDuration" || name=="TriggeredBy" || name=="ConflictsWith")));
}
unsigned GameObjectRecursiveModuleKind(const std::string& typeId) {auto c=FindModule(ParseUnsigned(typeId));return c && c->kind>=41?c->kind:0;}
unsigned GameObjectModuleNodeKind(unsigned kind,const std::string& path) {
    if(kind>=104 && kind<=111) {
        if(path=="UnitAITargetChooserData" || path=="UnitAITargetChooserData/ObjectVisionRange/Filter") return 1;
        if(path=="UnitAITargetChooserData/ObjectVisionRange") return 2;
        if(path=="UnitAITargetChooserData/ObjectVisionRange/Filter/IncludeThing" || path=="UnitAITargetChooserData/ObjectVisionRange/Filter/ExcludeThing") return 3;
        return 0;
    }
    if(kind>=96 && kind<=103) {
        if(kind==103) {if(path=="CrateList" || path=="BlockingUnits/IncludeThing" || path=="BlockingUnits/ExcludeThing") return 3;if(path=="BlockingUnits") return 1;}return 0;
    }
    if(kind>=92 && kind<=95) {
        if(kind==92) {if(path=="ObjectStatusValidation" || path=="CustomAnimAndDuration") return 1;if(path=="TriggeredBy" || path=="ConflictsWith") return 3;}
        if(kind==93 && (path=="PercentOfBuildCostToRebuildInfo" || path=="SelfAudio")) return 1;
        if(kind==94 && path=="Die") return 1;
        if(kind==95) {if(path=="ObjectFilter") return 1;if(path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;}
        return 0;
    }
    if(kind==91) {
        for(const char* slot:{"WeaponSlotHardpoint","WeaponSlotTurret","WeaponSlotHierarchicalTurret","WeaponSlotContained"}) {
            std::string base=slot;if(path==base || path==base+"/Weapon") return 2;
            if(base=="WeaponSlotTurret" || base=="WeaponSlotHierarchicalTurret") {
                std::string settings=base+"/TurretSettings",chooser=settings+"/TurretAITargetChooserData",vision=chooser+"/ObjectVisionRange";
                if(path==settings || path==chooser || path==vision+"/Filter") return 1;
                if(path==vision) return 2;
                if(path==vision+"/Filter/IncludeThing" || path==vision+"/Filter/ExcludeThing") return 3;
            }
        }return 0;
    }
    if(kind>=87) {if(kind==87) return path=="Data"?2:0;if(kind==88 || kind==90) {if(path=="CustomAnimAndDuration") return 1;if(path=="TriggeredBy" || path=="ConflictsWith") return 3;}if(kind==90) {if(path=="WeaponOffset" || path=="DieMuxData") return 1;if(path=="WeaponFireProbability") return 2;}return 0;}
    if(kind>=84) return 0;
    if(kind>=80) {
        if(kind==80) return path=="Movement"?1:0;
        if(kind==81) {if(path=="Volume") return 1;for(const auto& shape:motionShapeNames) {std::string base=std::string("Volume/")+shape;if(path==base) return 2;if(path==base+"/Translation" || path==base+"/Rotation") return 1;}}
        return 0;
    }
    if(kind>=77) {if(kind==79) return 0;if(path=="EndOffset" || path=="ObjectStatusValidation") return 1;if(path=="LaserEndParticleSystem" || path=="LaserStartParticleSystem") return 3;return 0;}
    if(kind>=69) {if(WeaponValidationOffset(kind) && path=="ObjectStatusValidation") return 1;if(kind==70) {if(path=="FireWeaponNugget") return 2;if(path=="FireWeaponNugget/Offset") return 1;}if(kind==75) {if(path=="Color") return 1;if(path=="EffectInfo") return 2;}return 0;}
    if(kind>=61) {if(kind==67) {if(path=="BroadcastObjectFilter" || path=="ObjectStatusValidation") return 1;if(path=="BroadcastObjectFilter/IncludeThing" || path=="BroadcastObjectFilter/ExcludeThing") return 3;}return 0;}
    if(kind==60) {if(path=="ObjectFilter" || path=="CustomAnimAndDuration") return 1;if(path=="TriggeredBy" || path=="ConflictsWith" || path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;return 0;}
    if(kind==59) return path=="GuardPosOffset"?1:0;
    if(kind>=55) {if(kind==56 && path=="ObjectFilter") return 1;if((kind==56 && path=="Upgrade") || (kind==58 && path=="SpecialPowerTemplate")) return 3;return 0;}
    if(kind>=52) {
        if(kind==52) {if(path=="QuantityModifier" || path=="ProductionModifier" || path=="GiveUpgradeToProduced/UpgradeTranslate") return 2;if(path=="GiveUpgradeToProduced" || path=="ProductionModifier/ModifierFilter") return 1;}
        if((kind==53 || kind==54) && (path=="UnitCreatePoint" || path=="NaturalRallyPoint")) return 1;
        return 0;
    }
    if(path=="CustomAnimAndDuration") return 1;
    if(path=="TriggeredBy" || path=="ConflictsWith") return 3;
    if(kind==41) {if(path=="AvailableSubStructure" || path=="StructureOffset") return 2;if(path=="AvailableSubStructure/UpgradeTrigger" || path=="AvailableSubStructure/ObjectTemplate") return 4;if(path=="StructureOffset/Offset") return 1;}
    if(kind==43 && path=="ReplacementTemplate") return 3;
    if(kind==44) {if(path=="Nugget" || path=="Nugget/UpgradeTexture") return 2;if(path=="Nugget/CustomAnimAndDuration") return 1;}
    if(kind==45 && path=="Offset") return 1;
    if(kind==46 && path=="UpgradeToRemove") return 3;
    if(kind==49 || kind==50) {if(path=="ObjectFilter") return 1;if(path=="ObjectFilter/IncludeThing" || path=="ObjectFilter/ExcludeThing") return 3;}
    if(kind==49 && path=="ApplyToUpgrade") return 3;
    return 0;
}
bool GameObjectMotionMoveAttribute(const std::string& typeId,const std::string& attribute) {
    int index=MotionMoveIndex(ParseUnsigned(typeId));if(index<0 || index==3) return false;const auto& set=motionFieldSets[4+index];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
}
bool GameObjectMotionMovePaths(const std::string& typeId) {return ParseUnsigned(typeId)==3012512132u;}
bool GameObjectModuleNodeAttribute(unsigned kind,const std::string& path,const std::string& attribute) {
    if(kind>=104 && kind<=111) {
        if(path=="UnitAITargetChooserData") {
            if(attribute=="TargetingCompareList") return true;
            for(unsigned i=0;i<weaponSetFieldSets[8].count;++i) if(attribute==weaponSetFieldSets[8].fields[i].name) return true;
        }
        if(path=="UnitAITargetChooserData/ObjectVisionRange") return attribute=="Range";
        return false;
    }
    if(kind>=96 && kind<=103) return false;
    if(kind>=92 && kind<=95) {
        const ModuleFieldSet* set=nullptr;
        if(kind==92) {if(path=="ObjectStatusValidation") set=&lifecycleFieldSets[6];if(path=="CustomAnimAndDuration") set=&upgradeFieldSets[11];}
        if(kind==93) {if(path=="PercentOfBuildCostToRebuildInfo") set=&healingFieldSets[5];if(path=="SelfAudio") set=&healingFieldSets[6];}
        if(kind==94 && path=="Die") set=&dieFieldSets[7];
        if(set) for(unsigned i=0;i<set->count;++i) if(attribute==set->fields[i].name) return true;return false;
    }
    if(kind==91) {
        unsigned fields=99;auto slash=path.find('/');auto slot=path.substr(0,slash);auto rest=slash==std::string::npos?"":path.substr(slash);
        if(rest.empty()) {
            if(attribute=="ParentID" && slot=="WeaponSlotHierarchicalTurret") return true;fields=1;
        }else if(rest=="/Weapon") fields=5;
        else if(rest=="/TurretSettings") fields=6;
        else if(rest=="/TurretSettings/TurretAITargetChooserData") {
            for(const auto& set:{weaponSetFieldSets[7],weaponSetFieldSets[8]}) for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
        }else if(rest=="/TurretSettings/TurretAITargetChooserData/ObjectVisionRange") fields=9;
        if(fields==99) return false;const auto& set=weaponSetFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
    }
    if(kind>=87) {const ModuleFieldSet* set=nullptr;if(kind==87 && path=="Data") set=&reactiveFieldSets[4];if((kind==88 || kind==90) && path=="CustomAnimAndDuration") set=&upgradeFieldSets[11];if(kind==90) {if(path=="WeaponOffset") set=&remainingFieldSets[15];if(path=="DieMuxData") set=&dieFieldSets[7];if(path=="WeaponFireProbability") set=&reactiveFieldSets[5];}if(set) for(unsigned i=0;i<set->count;++i) if(attribute==set->fields[i].name) return true;return false;}
    if(kind>=84) return false;
    if(kind>=80) {
        if(kind!=81) return false;unsigned fields=99;if(path=="Volume") fields=7;
        for(unsigned i=0;i<5;++i) {std::string base=std::string("Volume/")+motionShapeNames[i];if(path==base) fields=8+i;if(path==base+"/Translation") fields=13;if(path==base+"/Rotation") fields=14;}
        if(fields==99) return false;const auto& set=motionFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;
    }
    if(kind>=77) {if(path=="EndOffset") return attribute=="x" || attribute=="y" || attribute=="z";return path=="ObjectStatusValidation" && (attribute=="RequiredStatus" || attribute=="ForbiddenStatus");}
    if(kind>=69) {if(path=="ObjectStatusValidation") return attribute=="RequiredStatus" || attribute=="ForbiddenStatus";unsigned fields=path=="Color"?8:path=="EffectInfo"?9:path=="FireWeaponNugget"?10:path=="FireWeaponNugget/Offset"?11:99;if(fields==99) return false;const auto& set=weaponFieldSets[fields];for(unsigned i=0;i<set.count;++i) if(attribute==set.fields[i].name) return true;return false;}
    if(kind>=61) return kind==67 && path=="ObjectStatusValidation" && (attribute=="RequiredStatus" || attribute=="ForbiddenStatus");
    if(kind==60) {if(path=="CustomAnimAndDuration") for(unsigned i=0;i<upgradeFieldSets[11].count;++i) if(attribute==upgradeFieldSets[11].fields[i].name) return true;return false;}
    if(kind==59) return path=="GuardPosOffset" && (attribute=="x" || attribute=="y" || attribute=="z");
    if(kind>=55) return false;
    if(kind>=52) {
        unsigned fields=path=="QuantityModifier"?3:path=="ProductionModifier"?4:path=="GiveUpgradeToProduced/UpgradeTranslate"?5:99;
        if(path=="UnitCreatePoint" || path=="NaturalRallyPoint") {for(unsigned i=0;i<remainingFieldSets[15].count;++i) if(attribute==remainingFieldSets[15].fields[i].name) return true;return false;}
        if(fields==99) return false;for(unsigned i=0;i<productionFieldSets[fields].count;++i) if(attribute==productionFieldSets[fields].fields[i].name) return true;return false;
    }
    if(path=="ObjectFilter") return false;
    unsigned index=path=="Nugget"?11:path=="Nugget/UpgradeTexture"?12:path=="StructureOffset"?13:(path=="Offset" || path=="StructureOffset/Offset")?15:99;
    if(path=="CustomAnimAndDuration" || path=="Nugget/CustomAnimAndDuration") {for(unsigned i=0;i<upgradeFieldSets[11].count;++i) if(attribute==upgradeFieldSets[11].fields[i].name) return true;return false;}
    if(index==99) return false;for(unsigned i=0;i<remainingFieldSets[index].count;++i) if(attribute==remainingFieldSets[index].fields[i].name) return true;return false;
}
bool GameObjectModuleChildSupported(const std::string& parent,const std::string& child) {
    for(const auto& entry:moduleChildNames) if(parent==entry.parent && child==entry.child) return true;
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
    unsigned char* skirmishAI=nullptr;
    for(const auto& child:children) if(child.name=="SkirmishAIInformation") {
        skirmishAI=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+460),88,1));
        MarshalAI(child,skirmishAI,0,data);data.Pop();
    }
    std::map<unsigned,NativeList*> moduleLists;
    const char* moduleContainers[]={"Draws","Behaviors","AI","ClientUpdates","ClientBehaviors"};
    const unsigned moduleOffsets[]={464,468,472,484,488};
    U32* bodyModule=nullptr;
    // Schema order places Body after AI and before the two client lists.
    for(unsigned group=0;group<5;++group) {
        if(group==3) for(const auto& child:children) if(child.name=="Body" && child.children.size()==1) MarshalModule(child.children[0],reinterpret_cast<void**>(root+480),data);
        for(const auto& child:children) if(child.name==moduleContainers[group]) {
            auto list=group==2?reinterpret_cast<NativeList*>(root+472):static_cast<NativeList*>(data.Push(reinterpret_cast<void**>(root+moduleOffsets[group]),8,1));
            moduleLists[moduleOffsets[group]]=list;MarshalModuleList(child,list,data);if(group!=2) data.Pop();
        }
    }
    bodyModule=*reinterpret_cast<U32**>(root+480);
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
    // Original 66174/66183: both schema types are empty one-byte objects.
    // Root Tokenize 97735/97742 creates no pointee bookmark or child table.
    for(unsigned i=0;i<2;++i) for(const auto& child:children)
        if(child.name==(i==0?"ReplaceModule":"InheritableModule")) {
            data.Push(reinterpret_cast<void**>(root+496+4*i),1,1);data.Pop();
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
    NativeList* voiceLists=nullptr;
    for(const auto& child:children) if(child.name=="AudioArrayVoice") {
        voiceLists=static_cast<NativeList*>(data.Push(reinterpret_cast<void**>(root+524),24,1));
        const char* names[]={"AudioEntry","ObjectSpecificEntry","NamedEntry"};
        for(unsigned group=0;group<3;++group) {
            auto list=voiceLists+group;std::vector<const GameObjectChild*> values;
            for(const auto& entry:child.children) if(entry.name==names[group]) values.push_back(&entry);
            list->count=static_cast<U32>(values.size());
            if(list->count) {
                unsigned stride=group==1?16:12;auto elements=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));
                for(U32 j=0;j<list->count;++j) {
                    auto at=elements+stride*j;const auto& attrs=values[j]->attributes;
                    if(group==2) {auto name=attrs.find("Name");if(name!=attrs.end()) {auto hash=ExactStringHash(name->second);std::memcpy(at+8,&hash,4);}}
                    else {auto type=attrs.find("AudioType");if(type!=attrs.end()) {auto v=EnumValue(group==0?"AudioVoiceEntry":"AudioObjectSpecificVoiceEntry",type->second);std::memcpy(at+8,&v,4);}if(group==1) {auto target=attrs.find("TargetObject");if(target!=attrs.end()) {auto hash=HashAssetId(target->second);std::memcpy(at+12,&hash,4);}}}
                    auto sound=attrs.find("Sound");if(sound!=attrs.end()) {auto ref=data.Push(reinterpret_cast<void**>(at),4,1);MarshalReference(data,ref,sound->second);data.Pop();}
                    auto eva=attrs.find("EvaEvent");MarshalReference(data,at+4,eva==attrs.end()?"":eva->second);
                }
                data.Pop();
            }
        }
        data.Pop();
    }
    NativeList* audioArrays[2]={nullptr,nullptr};
    for(unsigned group=0;group<2;++group) for(const auto& child:children) if(child.name==(group==0?"AudioArraySound":"EvaEventArray")) {
        auto list=static_cast<NativeList*>(data.Push(reinterpret_cast<void**>(root+528+4*group),8,1));audioArrays[group]=list;
        list->count=static_cast<U32>(child.children.size());
        if(list->count) {
            auto elements=static_cast<U32*>(data.Push(&list->data,8,list->count));
            for(U32 j=0;j<list->count;++j) {
                const auto& attributes=child.children[j].attributes;
                auto ref=attributes.find(group==0?"Sound":"EvaEvent");if(ref!=attributes.end()) MarshalReference(data,elements+2*j,ref->second);
                auto type=attributes.find(group==0?"AudioType":"EvaType");if(type!=attributes.end()) elements[2*j+1]=EnumValue(child.name,type->second);
            }
            data.Pop();
        }
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
    if(!moduleLists.count(472)) moduleLists[472]=reinterpret_cast<NativeList*>(root+472);
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
        else if(moduleLists.count(t.offset)) TokenizeModuleList(moduleLists.at(t.offset),&table[i].offset,procedures);
        else if(t.offset==480 && bodyModule) TokenizeModule(bodyModule,&table[i].offset,procedures);
        else if(t.offset==460 && skirmishAI) TokenizeAI(skirmishAI,0,&table[i].offset,procedures);
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
        else if(t.offset==524 && voiceLists) {
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,4));
            for(unsigned group=0;group<3;++group) {
                nested[group].name=AudioArrayVoiceProc[group].name;nested[group].type=AudioArrayVoiceProc[group].type;auto list=voiceLists+group;
                auto count=static_cast<Procedure*>(procedures.Push(&nested[group].offset,12,list->count+1));
                count[0].name=1820487795u;count[0].type=1564774360u;procedures.BookmarkMemberOffset(list,&count[0].offset);
                for(U32 j=0;j<list->count;++j) {
                    count[j+1].name=2974916843u;unsigned members=group==1?3:2;auto proc=group==0?AudioVoiceEntryProc:group==1?AudioObjectSpecificVoiceEntryProc:AudioVoiceReferentialEntryProc;
                    auto item=static_cast<Procedure*>(procedures.Push(&count[j+1].offset,12,members+1));auto bytes=static_cast<unsigned char*>(list->data)+(group==1?16:12)*j;
                    for(unsigned k=0;k<members;++k) {item[k].name=proc[k].name;item[k].type=proc[k].type;if(k) procedures.BookmarkMemberOffset(bytes+proc[k].offset,&item[k].offset);}
                    auto base=static_cast<Procedure*>(procedures.Push(&item[0].offset,12,3));
                    for(unsigned k=0;k<2;++k) {base[k].name=SoundOrEvaEventProc[k].name;base[k].type=SoundOrEvaEventProc[k].type;}
                    auto sound=*reinterpret_cast<void**>(bytes);if(sound) procedures.BookmarkMemberOffset(sound,&base[0].offset);
                    procedures.BookmarkMemberOffset(bytes+4,&base[1].offset);procedures.Pop();procedures.Pop();
                }
                procedures.Pop();
            }
            procedures.Pop();
        }
        else if((t.offset==528 || t.offset==532) && audioArrays[(t.offset-528)/4]) {
            unsigned group=(t.offset-528)/4;auto list=audioArrays[group];auto proc=group==0?AudioArraySoundProc:EvaEventArrayProc;auto entryProc=group==0?AudioSoundEntryProc:EvaEventEntryProc;
            auto nested=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,2));nested[0].name=proc[0].name;nested[0].type=proc[0].type;
            auto count=static_cast<Procedure*>(procedures.Push(&nested[0].offset,12,list->count+1));
            count[0].name=1820487795u;count[0].type=1564774360u;procedures.BookmarkMemberOffset(list,&count[0].offset);
            for(U32 j=0;j<list->count;++j) {
                count[j+1].name=2974916843u;auto item=static_cast<Procedure*>(procedures.Push(&count[j+1].offset,12,3));
                for(unsigned k=0;k<2;++k) {item[k].name=entryProc[k].name;item[k].type=entryProc[k].type;procedures.BookmarkMemberOffset(static_cast<U32*>(list->data)+2*j+k,&item[k].offset);}
                procedures.Pop();
            }
            procedures.Pop();procedures.Pop();
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
        EndianAI(skirmishAI,0);
        if(bodyModule) EndianModule(bodyModule);
        for(const auto& entry:moduleLists) {
            auto list=entry.second;
            for(U32 j=0;j<list->count;++j) {auto module=static_cast<U32**>(list->data)[j];EndianModule(module);}
            if(entry.first!=472) Swap(&list->count);
        }
        if(voiceLists) for(unsigned group=0;group<3;++group) {
            auto list=voiceLists+group;unsigned stride=group==1?16:12;
            for(U32 j=0;j<list->count;++j) {auto at=static_cast<unsigned char*>(list->data)+stride*j;Swap(at+8);if(group==1) Swap(at+12);}
            Swap(&list->count);
        }
        for(auto list:audioArrays) if(list) {for(U32 j=0;j<list->count;++j) Swap(static_cast<U32*>(list->data)+2*j+1);Swap(&list->count);}
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






