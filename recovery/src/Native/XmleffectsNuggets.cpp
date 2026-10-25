#include "Xmleffects.h"
#include "XmleffectsEnums.h"
#include <cstring>
#include <cctype>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
namespace {
using Relo::U32;
void Word(void* field, U32 value) { std::memcpy(field,&value,4); }
void Float(void* field, float value) { std::memcpy(field,&value,4); }
void Swap(void* field) {
    U32 value; std::memcpy(&value,field,4);
    Word(field,(value<<24)|((value<<8)&0x00ff0000u)|((value>>8)&0x0000ff00u)|(value>>24));
}
float Units(const std::string& text, bool angle) {
    std::size_t cut = text.find(angle ? "r" : "ms");
    float scale = angle ? 1.0f : 0.001f;
    if(cut == std::string::npos) {
        cut = text.find(angle ? "d" : "s");
        scale = angle && cut != std::string::npos ? static_cast<float>(3.14159265358979323846)/180.0f : 1.0f;
    }
    return static_cast<float>(static_cast<double>(ParseFloat(text.substr(0,cut)))*static_cast<double>(scale));
}
void ModelConditions(U32* target, const std::string& text) {
    std::size_t at=0;
    while(at<text.size() && text[at]) {
        while(at<text.size() && std::isspace(static_cast<unsigned char>(text[at]))) ++at;
        if(at==text.size() || !text[at]) break;
        std::size_t end=at;
        while(end<text.size() && text[end] && !std::isspace(static_cast<unsigned char>(text[end]))) ++end;
        std::string token=text.substr(at,end-at);
        bool add=true;
        if(!token.empty() && (token[0]=='+' || token[0]=='-')) {add=token[0]!='-'; token.erase(0,1);}
        std::string upper=token; for(char& ch:upper) if(ch>='a'&&ch<='z') ch=static_cast<char>(ch-'a'+'A');
        // The original ALL branch never advances the token pointer: it consumes
        // the remaining loop count repeating ALL, so following tokens have no effect.
        if(upper=="ALL") {for(U32 i=0;i<15;++i)target[i]=~0u; return;}
        U32 index=XmleffectsEnum("ModelConditionFlagType",token);
        if(index<456) {
            U32 mask=1u<<(index&31);
            if(add) target[index>>5]|=mask; else target[index>>5]^=mask;
        }
        at=end;
    }
}

}
void MarshalXmleffectsNugget(Relo::Tracker& tracker, void** pointer, const XmleffectsNuggetInput& input, bool bigEndian) {
    unsigned char* target = static_cast<unsigned char*>(tracker.Push(pointer,input.size,1));
    // Derived Marshal runs before FXNugget base Marshal in the original wrappers.
    for(const auto& field:input.fields) {
        void* at = target+field.offset;
        switch(field.kind) {
        case EffectFloat: Float(at,ParseFloat(field.text)); break;
        case EffectSigned: Word(at,static_cast<U32>(ParseSigned(field.text))); break;
        case EffectUnsigned: Word(at,ParseUnsigned(field.text)); break;
        case EffectHash: Word(at,HashAssetId(field.text)); break;
        case EffectReference: MarshalReference(tracker,at,field.text); break;
        case EffectString: MarshalString(tracker,at,field.text); break;
        case EffectTime: Float(at,Units(field.text,false)); break;
        case EffectAngle: Float(at,Units(field.text,true)); break;
        case EffectBoolean: *static_cast<unsigned char*>(at)=ParseBoolean(field.text); break;
        case EffectEnum: Word(at,XmleffectsEnum(field.enumType,field.text)); break;
        case EffectPercentage: Float(at,ParseFloat(field.text)*0.01f); break;
        }
    }
    if(input.typeId==751572338u) {
        // Original ParticleSystemFXNugget initializes InitialDelay to
        // CONSTANT / Low=-1 / High=-1 before marshaling its optional child.
        bool lowPresent=false, highPresent=false;
        for(const auto& field:input.fields) {
            if(field.offset==132) lowPresent=true;
            if(field.offset==136) highPresent=true;
        }
        if(!lowPresent) {Float(target+132,-1.0f); if(bigEndian) Swap(target+132);}
        if(!highPresent) {Float(target+136,-1.0f); if(bigEndian) Swap(target+136);}
    }
    if(input.hasPackedColor) {
        U32 red=ParseUnsigned(input.rgba[0])&255u, green=ParseUnsigned(input.rgba[1])&255u;
        U32 blue=ParseUnsigned(input.rgba[2])&255u, alpha=ParseUnsigned(input.rgba[3])&255u;
        Word(target+92,(alpha<<24)|(red<<16)|(green<<8)|blue);
        if(bigEndian) Swap(target+92);
    }
    if(input.hasTargetOffset) {
        unsigned char* offset=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(target+100),12,1));
        for(U32 i=0;i<3;++i) {Float(offset+4*i,ParseFloat(input.targetOffset[i]));if(bigEndian)Swap(offset+4*i);}
        tracker.Pop();
    }
    if(input.hasRange) {
        void* range=tracker.Push(reinterpret_cast<void**>(target+52),8,1);
        Word(range,XmleffectsEnum("TerrainScorchType",input.rangeLow));
        Word(static_cast<unsigned char*>(range)+4,XmleffectsEnum("TerrainScorchType",input.rangeHigh));
        if(bigEndian) {Swap(range);Swap(static_cast<unsigned char*>(range)+4);}
        tracker.Pop();
    }
    for(U32 i=0;i<4;++i) if(input.hasModelConditions[i]) {
        U32* bits=static_cast<U32*>(tracker.Push(reinterpret_cast<void**>(target+4+4*i),60,1));
        ModelConditions(bits,input.modelConditions[i]);
        if(bigEndian) for(U32 j=0;j<15;++j) Swap(bits+j);
        tracker.Pop();
    }
    Word(target+32,static_cast<U32>(input.disabled.size()));
    for(U32 i=0;i<2;++i) if(input.hasFilters[i]) {
        ObjectFilter* filter=static_cast<ObjectFilter*>(tracker.Push(reinterpret_cast<void**>(target+24+4*i),112,1));
        MarshalObjectFilter(input.filters[i],filter,tracker);
        if(bigEndian) EndianObjectFilter(filter);
        tracker.Pop();
    }
    if(!input.disabled.empty()) {
        U32* list=static_cast<U32*>(tracker.Push(reinterpret_cast<void**>(target+36),4,static_cast<U32>(input.disabled.size())));
        for(U32 i=0;i<input.disabled.size();++i) {list[i]=XmleffectsEnum("DisabledType",input.disabled[i]);if(bigEndian)Swap(list+i);}
        tracker.Pop();
    }
    Word(target,input.size==44 ? 0u : input.typeId);
    if(bigEndian) {
        Swap(target); Swap(target+32);
        for(const auto& field:input.fields)
            if(field.kind!=EffectBoolean && field.kind!=EffectReference) Swap(target+field.offset);
    }
    tracker.Pop();
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif

