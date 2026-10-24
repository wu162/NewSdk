#include "Xmleffects.h"
#include "XmleffectsEnums.h"
#include <cstring>
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
}
void MarshalXmleffectsNugget(Relo::Tracker& tracker, void** pointer, const XmleffectsNuggetInput& input, bool bigEndian) {
    unsigned char* target = static_cast<unsigned char*>(tracker.Push(pointer,input.size,1));
    // Derived Marshal runs before FXNugget base Marshal in the original wrappers.
    for(const auto& field:input.fields) {
        void* at = target+field.offset;
        switch(field.kind) {
        case EffectFloat: Float(at,ParseFloat(field.text)); break;
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
    if(input.hasRange) {
        void* range=tracker.Push(reinterpret_cast<void**>(target+52),8,1);
        Word(range,XmleffectsEnum("TerrainScorchType",input.rangeLow));
        Word(static_cast<unsigned char*>(range)+4,XmleffectsEnum("TerrainScorchType",input.rangeHigh));
        if(bigEndian) {Swap(range);Swap(static_cast<unsigned char*>(range)+4);}
        tracker.Pop();
    }
    Word(target+32,static_cast<U32>(input.disabled.size()));
    if(!input.disabled.empty()) {
        U32* list=static_cast<U32*>(tracker.Push(reinterpret_cast<void**>(target+36),4,static_cast<U32>(input.disabled.size())));
        for(U32 i=0;i<input.disabled.size();++i) {list[i]=XmleffectsEnum("DisabledType",input.disabled[i]);if(bigEndian)Swap(list+i);}
        tracker.Pop();
    }
    Word(target,input.typeId);
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
