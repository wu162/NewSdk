#include "Xmlfxparticle.h"
#include "XmlfxparticleEnums.h"
#include <cstring>
#ifdef _MANAGED
#pragma managed(push, off)
#endif
namespace Recovered { namespace Native {
namespace {
using Relo::U32;
void Word(void* dst, U32 offset, U32 word) { std::memcpy(static_cast<unsigned char*>(dst)+offset, &word, 4); }
void Float(void* dst, U32 offset, float value) { std::memcpy(static_cast<unsigned char*>(dst)+offset, &value, 4); }
void Swap(void* dst, U32 offset) {
    U32 word; std::memcpy(&word, static_cast<unsigned char*>(dst)+offset, 4);
    Word(dst, offset, (word << 24) | ((word << 8) & 0x00ff0000u) | ((word >> 8) & 0x0000ff00u) | (word >> 24));
}
void Random(void* dst, U32 offset, const XmlfxparticleRandomInput& input) {
    if (!input.present) return;
    Word(dst, offset, XmlfxparticleEnum("DistributionType", input.type));
    Float(dst, offset+4, ParseFloat(input.low)); Float(dst, offset+8, ParseFloat(input.high));
}

void Module(Relo::Tracker& tracker, void** location, const XmlfxparticleModuleInput& input, bool bigEndian) {
    if(!input.present) return;
    unsigned char* target=static_cast<unsigned char*>(tracker.Push(location,input.size,1));
    Word(target,0,input.typeId);
    for(const auto& field:input.fields) {
        if(field.kind==ParticleBoolean) target[field.offset]=ParseBoolean(field.text);
        else if(field.kind==ParticleEnum) Word(target,field.offset,XmlfxparticleEnum(field.enumType,field.text));
        else if(field.kind==ParticleReference) MarshalReference(tracker,target+field.offset,field.text);
        else if(field.kind==ParticlePercentage) Float(target,field.offset,ParsePercentage(field.text));
        else if(field.kind==ParticleSigned) Word(target,field.offset,static_cast<U32>(ParseSigned(field.text)));
        else Float(target,field.offset,ParseFloat(field.text));
    }
    for(const auto& random:input.random) Random(target,random.offset,random.input);
    if(bigEndian) {
        Swap(target,0);
        for(const auto& field:input.fields) if(field.kind!=ParticleBoolean && field.kind!=ParticleReference) Swap(target,field.offset);
        for(const auto& random:input.random) for(U32 i=0;i<12;i+=4) Swap(target,random.offset+i);
    }
    tracker.Pop();
}
struct List { U32 count; void* items; };
static_assert(sizeof(List) == 8, "effects list x86 ABI");
}
Relo::Chunk CompileXmlfxparticleParticle(const XmlfxparticleParticleInput& input, bool bigEndian) {
    Relo::Tracker tracker;
    unsigned char* root = nullptr;
    tracker.Push(reinterpret_cast<void**>(&root), 176, 1);
    Word(root, 4, XmlfxparticleEnum("FXParticleSystem_Priority", input.priority));
    Word(root, 8, XmlfxparticleEnum("FXParticleSystem_ShaderType", input.shader));
    Word(root, 12, XmlfxparticleEnum("FXParticleSystem_Type", input.type));
    MarshalReference(tracker, root+16, input.references[0]);
    MarshalReference(tracker, root+20, input.references[1]);
    Word(root, 24, HashAssetId(input.typedIds[0])); Word(root, 28, HashAssetId(input.typedIds[1]));
    Word(root, 32, ParseUnsigned(input.lifetime)); Word(root, 36, ParseUnsigned(input.sortLevel));
    if(input.hasEmitterSound) {
        void* sound=tracker.Push(reinterpret_cast<void**>(root+40),4,1);
        MarshalReference(tracker,sound,input.references[2]); tracker.Pop();
    }
    for (U32 i=0; i<3; ++i) Float(root, 44+4*i, ParseFloat(input.offset[i]));
    for (U32 i=0; i<6; ++i) Random(root, 56+12*i, input.random[i]);
    for (U32 i=0; i<5; ++i) root[168+i] = ParseBoolean(input.flags[i]);
    if (input.hasAlphas) {
        List* target = static_cast<List*>(tracker.Push(reinterpret_cast<void**>(root+128), 8, 1));
        target->count = static_cast<U32>(input.alphas.size());
        if (target->count) {
            tracker.Push(&target->items, 20, target->count);
            for (U32 i=0; i<target->count; ++i) {
                unsigned char* item = static_cast<unsigned char*>(target->items)+20*i;
                Random(item, 0, input.alphas[i].value);
                Float(item, 12, ParseFloat(input.alphas[i].relativeAge)*0.01f);
                Word(item, 16, ParseUnsigned(input.alphas[i].frame));
                if (bigEndian) for (U32 j=0; j<20; j+=4) Swap(item,j);
            }
            tracker.Pop();
        }
        if (bigEndian) Swap(target,0);
        tracker.Pop();
    }
    if (input.hasColors) {
        unsigned char* target = static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(root+132), 28, 1));
        Float(target, 0, ParseFloat(input.saturation));
        target[24] = ParseBoolean(input.houseColor);
        List* list = reinterpret_cast<List*>(target+4);
        list->count = static_cast<U32>(input.colors.size());
        if (list->count) {
            tracker.Push(&list->items, 20, list->count);
            for (U32 i=0; i<list->count; ++i) {
                unsigned char* item = static_cast<unsigned char*>(list->items)+20*i;
                Float(item,0,ParseFloat(input.colors[i].relativeAge)*0.01f);
                Word(item,4,ParseUnsigned(input.colors[i].frame));
                for (U32 j=0; j<3; ++j) Float(item,8+4*j,ParseFloat(input.colors[i].rgb[j]));
                if (bigEndian) for (U32 j=0; j<20; j+=4) Swap(item,j);
            }
            tracker.Pop();
        }
        Random(target,12,input.colorScale);
        if (bigEndian) { Swap(target,0); Swap(target,4); for(U32 j=12;j<24;j+=4) Swap(target,j); }
        tracker.Pop();
    }
    if (input.hasWind) {
        void* target = tracker.Push(reinterpret_cast<void**>(root+136),48,1);
        Word(target,0,XmlfxparticleEnum("FXParticleSystem_WindMotion",input.wind[0]));
        for(U32 i=1;i<12;++i) Float(target,4*i,ParseFloat(input.wind[i]));
        if(bigEndian) for(U32 i=0;i<48;i+=4) Swap(target,i);
        tracker.Pop();
    }
    const U32 locations[]={140,144,148,152,164};
    for(U32 i=0;i<5;++i) Module(tracker,reinterpret_cast<void**>(root+locations[i]),input.modules[i],bigEndian);
    if (bigEndian) {
        const U32 words[] = {4,8,12,24,28,32,36};
        for (U32 offset : words) Swap(root,offset);
        for (U32 offset=44;offset<128;offset+=4) Swap(root,offset);
    }
    return tracker.MakeRelocatable(bigEndian,0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
