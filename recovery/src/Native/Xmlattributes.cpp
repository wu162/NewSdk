#include "Xmlattributes.h"
#include "ResourceMarshaling.h"
#include "ScalarParsing.h"
#include "EnumBitset.h"
#include "XmlattributesEnums.h"
#include <cstring>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
namespace {
struct LogicCommandSetData { Relo::U32 reserved, count; Relo::U32* commands; };
static_assert(sizeof(LogicCommandSetData) == 12, "LogicCommandSet requires x86");
Relo::U32 Swap(Relo::U32 x) {
    return (x << 24) | ((x << 8) & 0x00ff0000u) | ((x >> 8) & 0x0000ff00u) | (x >> 24);
}
}
Relo::Chunk CompileXmlattributesLogicCommandSet(const std::vector<std::string>& commands, bool bigEndian) {
    Relo::Tracker tracker;
    LogicCommandSetData* root = nullptr;
    tracker.Push(reinterpret_cast<void**>(&root), sizeof(*root), 1);
    root->count = static_cast<Relo::U32>(commands.size());
    if (root->count) {
        tracker.Push(reinterpret_cast<void**>(&root->commands), 4, root->count);
        for (Relo::U32 i = 0; i < root->count; ++i)
            MarshalReference(tracker, &root->commands[i], commands[i]);
        tracker.Pop();
    }
    if (bigEndian) root->count = Swap(root->count);
    // XmlCompiler FinalizeTracker serializes this tracker directly at offset 0.
    // In particular, import bookmarks are retained and there is no token table.
    return tracker.MakeRelocatable(bigEndian, 0);
}
namespace {
using Relo::U32;
bool Has(const XmlattributesValues& a, const char* name) { return a.find(name) != a.end(); }
std::string Value(const XmlattributesValues& a, const char* name, const char* fallback = "") {
    auto i = a.find(name); return i == a.end() ? fallback : i->second;
}
void Word(void* p, U32 value) { std::memcpy(p, &value, 4); }
void Real(void* p, float value) { std::memcpy(p, &value, 4); }
void SwapAt(void* p) { U32 w; std::memcpy(&w,p,4); Word(p,Swap(w)); }
float Time(const std::string& text) {
    auto end = text.find("ms"); float factor = 0.001f;
    if (end == std::string::npos) { end = text.find('s'); factor = 1.0f; }
    return static_cast<float>(static_cast<double>(ParseFloat(text.substr(0,end))) * static_cast<double>(factor));
}
U32 Enum(const std::string& text, const EnumNameValue* pairs, std::size_t count, U32 fallback) {
    for (std::size_t i=0;i<count;++i) if (text == pairs[i].name) return static_cast<U32>(pairs[i].value);
    return fallback;
}
template <std::size_t N>
void SetEnum(const XmlattributesValues& a, const char* name, void* where, const EnumNameValue (&pairs)[N], U32 fallback) {
    if (Has(a,name)) Word(where,Enum(Value(a,name),pairs,N,fallback));
}
void Reference(const XmlattributesValues& a, const char* name, Relo::Tracker& t, void* where) {
    if (Has(a,name)) MarshalReference(t,where,Value(a,name));
}
void Bits(Relo::Tracker& t, unsigned char* owner, U32 offset, const XmlattributesValues& a,
    const char* name, bool model, bool big) {
    if (!Has(a,name)) return;
    U32* data = static_cast<U32*>(t.Push(reinterpret_cast<void**>(owner+offset),4,model?15:7));
    if (model) MarshalEnumBitset(Value(a,name),XmlattributesModelConditionFlagType,
        sizeof(XmlattributesModelConditionFlagType)/sizeof(XmlattributesModelConditionFlagType[0]),233,456,data);
    else MarshalEnumBitset(Value(a,name),XmlattributesObjectStatusType,
        sizeof(XmlattributesObjectStatusType)/sizeof(XmlattributesObjectStatusType[0]),198,223,data);
    if (big) for (U32 i=0;i<(model?15u:7u);++i) data[i]=Swap(data[i]);
    t.Pop();
}
void HashList(Relo::Tracker& t, unsigned char* owner, U32 offset, const std::vector<std::string>& values, bool big) {
    Word(owner+offset,static_cast<U32>(values.size()));
    if (!values.empty()) {
        U32* data=static_cast<U32*>(t.Push(reinterpret_cast<void**>(owner+offset+4),4,static_cast<U32>(values.size())));
        for (U32 i=0;i<values.size();++i) data[i]=big?Swap(HashAssetId(values[i])):HashAssetId(values[i]);
        t.Pop();
    }
    if (big) SwapAt(owner+offset);
}
void Filter(Relo::Tracker& t, unsigned char* owner, const XmlattributesInput& input, bool big) {
    if (!input.hasFilter) return;
    unsigned char* root=static_cast<unsigned char*>(t.Push(reinterpret_cast<void**>(owner+36),112,1));
    const auto& a=input.filter;
    if(Has(a,"id")) Word(root,HashAssetId(Value(a,"id")));
    Word(root+4,Enum(Value(a,"Rule","NONE"),XmlattributesObjectFilterRule,
        sizeof(XmlattributesObjectFilterRule)/sizeof(XmlattributesObjectFilterRule[0]),1));
    U32 relation=0;
    MarshalEnumBitset(Value(a,"Relationship"),XmlattributesObjectFilterRelationship,
        sizeof(XmlattributesObjectFilterRelationship)/sizeof(XmlattributesObjectFilterRelationship[0]),0,4,&relation);
    Word(root+8,relation);
    Word(root+12,Enum(Value(a,"Alignment","NONE"),XmlattributesObjectFilterAlignment,
        sizeof(XmlattributesObjectFilterAlignment)/sizeof(XmlattributesObjectFilterAlignment[0]),1));
    for(U32 i=0;i<2;++i) {
        U32 words[9]={};
        MarshalEnumBitset(Value(a,i?"Exclude":"Include"),XmlattributesKindOfType,
            sizeof(XmlattributesKindOfType)/sizeof(XmlattributesKindOfType[0]),149,284,words);
        for(U32 j=0;j<9;++j) Word(root+16+36*i+4*j,big?Swap(words[j]):words[j]);
    }
    Bits(t,root,88,a,"StatusBitFlags",false,big);
    Bits(t,root,92,a,"StatusBitFlagsExclude",false,big);
    HashList(t,root,96,input.filterLists[0],big);
    HashList(t,root,104,input.filterLists[1],big);
    if(big) for(U32 offset : {0u,4u,8u,12u}) SwapAt(root+offset);
    t.Pop();
}

}
Relo::Chunk CompileXmlattributesAttributeModifier(const XmlattributesInput& input, bool big) {
    Relo::Tracker tracker; unsigned char* root=nullptr;
    tracker.Push(reinterpret_cast<void**>(&root),60,1);
    const auto& a=input.attributes;
    SetEnum(a,"Category",root+4,XmlattributesAttributeModifierCategoryType,7);
    Real(root+8,Time(Value(a,"Duration","0s")));
    root[56]=ParseBoolean(Value(a,"ReplaceInCategoryIfLongest","false"));
    root[57]=ParseBoolean(Value(a,"IgnoreIfAnticategoryActive","false"));
    U32 disabled=0;
    MarshalEnumBitset(Value(a,"RemoveWhenDisabledByTypes"),XmlattributesDisabledType,
        sizeof(XmlattributesDisabledType)/sizeof(XmlattributesDisabledType[0]),0,12,&disabled);
    Word(root+12,disabled);
    Reference(a,"StartFX",tracker,root+16); Reference(a,"EndFX",tracker,root+20);
    Bits(tracker,root,24,a,"ModelConditionsSet",true,big);
    Bits(tracker,root,28,a,"ModelConditionsClear",true,big);
    Bits(tracker,root,32,a,"ObjectStatusToSet",false,big);
    Word(root+36,ParseUnsigned(Value(a,"StackingLimit","1")));
    SetEnum(a,"ArmorSetType",root+40,XmlattributesArmorSetType,12);
    if (Has(a,"Shader")) {
        void* ref=tracker.Push(reinterpret_cast<void**>(root+44),4,1);
        MarshalReference(tracker,ref,Value(a,"Shader")); tracker.Pop();
    }
    Word(root+48,static_cast<U32>(input.modifiers.size()));
    if (!input.modifiers.empty()) {
        unsigned char* list=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(root+52),8,static_cast<U32>(input.modifiers.size())));
        for (U32 i=0;i<input.modifiers.size();++i) {
            SetEnum(input.modifiers[i],"Type",list+8*i,XmlattributesAttributeType,32);
            const float v=ParseFloat(Value(input.modifiers[i],"Value","0"));
            Real(list+8*i+4, v*0.01f);
            if (big) { SwapAt(list+8*i); SwapAt(list+8*i+4); }
        }
        tracker.Pop();
    }
    if (big) for (U32 offset : {4u,8u,12u,36u,40u,48u}) SwapAt(root+offset);
    return tracker.MakeRelocatable(big,0);
}
Relo::Chunk CompileXmlattributesUpgradeTemplate(const XmlattributesInput& input, bool big) {
    Relo::Tracker tracker; unsigned char* root=nullptr;
    tracker.Push(reinterpret_cast<void**>(&root),140,1);
    const auto& a=input.attributes;
    const char* strings[]={"DisplayName","Description","TypeDescription","AcquireHint","GroupName","IconImage"};
    const U32 stringOffsets[]={4,12,20,28,104,124};
    // Original allocation order places the last two strings after references.
    for (U32 i=0;i<4;++i) if(Has(a,strings[i])) MarshalString(tracker,root+stringOffsets[i],Value(a,strings[i]));
    SetEnum(a,"Type",root+36,XmlattributesUpgradeType,1);
    if (Has(a,"BuildTime")) Real(root+40,Time(Value(a,"BuildTime")));
    if (Has(a,"BuildCost")) Word(root+44,static_cast<U32>(ParseSigned(Value(a,"BuildCost"))));
    const char* references[]={"ResearchSound","ResearchCompleteEvaEvent","UnitSpecificSound","UpgradeFX",
        "LocalPlayerGainsUpgradeEvaEvent","AlliedPlayerGainsUpgradeEvaEvent","EnemyPlayerGainsUpgradeEvaEvent",
        "LocalPlayerLosesUpgradeEvaEvent","AlliedPlayerLosesUpgradeEvaEvent","EnemyPlayerLosesUpgradeEvaEvent",
        "LocalPlayerProductionStartedEvaEvent","LocalPlayerBuildOnHoldEvaEvent","LocalPlayerBuildCancelledEvaEvent"};
    for (U32 i=0;i<13;++i) Reference(a,references[i],tracker,root+48+4*i);
    if (Has(a,"UseObjectTemplateForCostDiscount")) Word(root+100,HashAssetId(Value(a,"UseObjectTemplateForCostDiscount")));
    if (Has(a,"GroupName")) MarshalString(tracker,root+104,Value(a,"GroupName"));
    if (Has(a,"GroupOrder")) Word(root+112,ParseUnsigned(Value(a,"GroupOrder")));
    SetEnum(a,"SkirmishAIHeuristic",root+116,XmlattributesAIUpgradeHeuristicType,2);
    SetEnum(a,"Options",root+120,XmlattributesUpgradeOptions,0);
    if (Has(a,"IconImage")) MarshalString(tracker,root+124,Value(a,"IconImage"));
    root[136]=ParseBoolean(Value(a,"WaypointQueueable","false"));
    if (input.hasDependency) {
        unsigned char* dep=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(root+132),40,1));
        Bits(tracker,dep,0,input.dependency,"RequiredModelConditionsAny",true,big);
        Bits(tracker,dep,4,input.dependency,"ForbiddenModelConditions",true,big);
        Bits(tracker,dep,8,input.dependency,"RequiredObjectStatusAny",false,big);
        for(U32 i=0;i<3;++i) HashList(tracker,dep,12+8*i,input.dependencyLists[i],big);
        Filter(tracker,dep,input,big);
        tracker.Pop();
    }
    if (big) {
        for (U32 offset : {4u,12u,20u,28u,36u,40u,44u,100u,104u,112u,116u,120u,124u}) SwapAt(root+offset);
    }
    return tracker.MakeRelocatable(big,0);
}

}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
