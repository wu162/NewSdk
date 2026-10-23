#include "GameObject.h"
#include "ScalarParsing.h"
#include "DataInitializers.h"
#include "ResourceMarshaling.h"
#include "EnumBitset.h"
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
    std::map<unsigned,NativeList*> lists;
    lists[40]=reinterpret_cast<NativeList*>(root+40);
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
    marshalList("EquivalentTo",444,false);
    U32* flammability=nullptr;float* camera=nullptr;
    for(const auto& child:children) if(child.name=="Flammability") {
        flammability=static_cast<U32*>(data.Push(reinterpret_cast<void**>(root+456),16,1));
        const char* names[]={"Fuel","MaxBurnRate","Decay","Resistance"};
        for(unsigned i=0;i<4;++i) {auto at=child.attributes.find(names[i]);flammability[i]=at==child.attributes.end()?0:ParseUnsigned(at->second);}
        data.Pop();
    }
    for(const auto& child:children) if(child.name=="LiveCameraOffset") {
        camera=static_cast<float*>(data.Push(reinterpret_cast<void**>(root+520),12,1));
        const char* names[]={"x","y","z"};
        for(unsigned i=0;i<3;++i) {auto at=child.attributes.find(names[i]);camera[i]=at==child.attributes.end()?0.0f:ParseFloat(at->second);}
        data.Pop();
    }
    marshalList("UpgradeCameo",536,true);
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
        else if(t.kind==GameObjectTokenKind::List) {
            auto found=lists.find(t.offset);auto list=found==lists.end()?nullptr:found->second;
            const U32 length=list?list->count:0;
            auto count=static_cast<Procedure*>(procedures.Push(&table[i].offset,12,length+1));
            count->name=1820487795u;count->type=1564774360u;
            procedures.BookmarkMemberOffset(root+t.offset,&count->offset);
            for(U32 j=0;j<length;++j) {
                count[j+1].name=2974916843u;count[j+1].type=0;
                const unsigned stride=t.offset==40 || t.offset==536?8:4;
                procedures.BookmarkMemberOffset(static_cast<unsigned char*>(list->data)+stride*j,&count[j+1].offset);
            }
            procedures.Pop();
        }
    }
    if(bigEndian) {
        for(U32 i=0;i<browser.size();++i) Swap(&browserStrings[i].length);
        for(auto name:displayNames) if(name) Swap(&name->length);
        if(flammability) for(unsigned i=0;i<4;++i) Swap(flammability+i);
        if(camera) for(unsigned i=0;i<3;++i) Swap(camera+i);
        for(const auto& entry:lists) if(entry.first!=40) {
            const unsigned stride=entry.first==536?8:4;
            auto bytes=static_cast<unsigned char*>(entry.second->data);
            for(U32 i=0;i<entry.second->count;++i) Swap(bytes+stride*i);
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

