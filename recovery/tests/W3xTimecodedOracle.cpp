// Actual original Compress/Estimate/WriteOut IL, x86 only. No SDK launch.
#include "../src/Native/W3x/Timecoded.h"
#include "../src/Native/Relocation.h"
#include <cstring>
#include <cmath>
#include <stdexcept>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::Reflection;
using namespace Recovered::W3x;
static int channels, serialized;
static long long comparedBytes;
static MethodInfo^ Resolve(Assembly^ a,int token,int count) {
    MethodInfo^ m=dynamic_cast<MethodInfo^>(a->ManifestModule->ResolveMethod(token));
    if(!m || m->GetParameters()->Length!=count) throw gcnew Exception("Original Timecoded signature changed");
    return m;
}
static Object^ P(MethodInfo^ m,int arg,void* p) { return Pointer::Box(p,m->GetParameters()[arg]->ParameterType); }
static Object^ Invoke(MethodInfo^ m,array<Object^>^ args) {
    try { return m->Invoke(nullptr,args); } catch(TargetInvocationException^ e) { throw e->InnerException; }
}
static std::uint32_t U32(const unsigned char* p) {std::uint32_t v;std::memcpy(&v,p,4);return v;}
static void Word(unsigned char* p,std::uint32_t v) {std::memcpy(p,&v,4);}
static void* Ptr(const unsigned char* p) {void* v;std::memcpy(&v,p,4);return v;}
static void Equal(const void* e,const void* a,std::size_t size,String^ part) {
    const unsigned char* expected=static_cast<const unsigned char*>(e);
    const unsigned char* actual=static_cast<const unsigned char*>(a);
    for(std::size_t i=0;i<size;++i) if(expected[i]!=actual[i]) {
        Console::Error->WriteLine("FAIL Timecoded {0} byte={1}",part,static_cast<int>(i));
        Console::Error->WriteLine("original={0:X2} recovered={1:X2}",static_cast<int>(expected[i]),static_cast<int>(actual[i]));
        throw gcnew Exception("Original Timecoded differential failed");
    }
    comparedBytes+=static_cast<long long>(size);
}
static Recovered::Relo::Chunk NativeChunk(const TimecodedChannel& channel,bool big) {
    TimecodedEncoded encoded=EncodeTimecoded(channel,big);
    Recovered::Relo::Tracker t; void* root=0;
    unsigned char* output=static_cast<unsigned char*>(t.Push(&root,36,1));
    std::memcpy(output,encoded.root.data(),36);
    void* values=t.Push(reinterpret_cast<void**>(output+32),4,static_cast<std::uint32_t>(encoded.values.size()/4));
    if(!encoded.values.empty()) std::memcpy(values,encoded.values.data(),encoded.values.size()); t.Pop();
    void* codes=t.Push(reinterpret_cast<void**>(output+24),2,static_cast<std::uint32_t>(encoded.codes.size()/2));
    if(!encoded.codes.empty()) std::memcpy(codes,encoded.codes.data(),encoded.codes.size()); t.Pop();t.Pop();
    return t.MakeRelocatable(big,0);
}
static void WriteCase(Assembly^ a,unsigned char* compressed,const TimecodedChannel& channel,bool big) {
    MethodInfo^ write=Resolve(a,0x06000010,4), ^finalize=Resolve(a,0x06000038,4);
    MethodInfo^ destroy=Resolve(a,0x06000037,1), ^destroyChunk=Resolve(a,0x06000035,1);
    unsigned char tracker[88]={0},chunk[24]={0};void* root=0;
    array<Object^>^ args=gcnew array<Object^>(4);
    args[0]=P(write,0,compressed);args[1]=P(write,1,tracker);args[2]=P(write,2,&big);args[3]=P(write,3,&root);
    Invoke(write,args);
    try {
        args=gcnew array<Object^>(4);
        args[0]=P(finalize,0,tracker);args[1]=P(finalize,1,chunk);args[2]=P(finalize,2,&big);args[3]=static_cast<UInt32>(0);
        Invoke(finalize,args);
        try {
            Recovered::Relo::Chunk native=NativeChunk(channel,big);
            const std::vector<unsigned char>* buffers[]={&native.instance,&native.relocations,&native.imports};
            for(int i=0;i<3;++i) {
                if(U32(chunk+i*8+4)!=buffers[i]->size()) throw gcnew Exception("Timecoded serialized buffer size mismatch");
                Equal(Ptr(chunk+i*8),buffers[i]->data(),buffers[i]->size(),"WriteOut/Finalize three buffers");
            }
            ++serialized;
        } finally {array<Object^>^ d=gcnew array<Object^>(1);d[0]=P(destroyChunk,0,chunk);Invoke(destroyChunk,d);}
    } finally {array<Object^>^ d=gcnew array<Object^>(1);d[0]=P(destroy,0,tracker);Invoke(destroy,d);}
}
static void Test(Assembly^ a,unsigned type,const std::vector<TimecodedFrame>& input,float error,float rate) {
    unsigned char source[16]={0},channel[16]={0},settings[24]={0};
    std::vector<unsigned char> frames(input.size()*24,0);
    std::vector<std::vector<float> > values;
    for(std::size_t i=0;i<input.size();++i) values.push_back(input[i].values);
    for(std::size_t i=0;i<input.size();++i) {
        unsigned char* f=frames.data()+i*24;
        Word(f,input[i].index);void* begin=values[i].data();void* end=values[i].data()+values[i].size();
        std::memcpy(f+4,&begin,4);std::memcpy(f+8,&end,4);std::memcpy(f+12,&end,4);f[20]=input[i].binaryMove?1:0;
    }
    void* begin=frames.empty()?0:frames.data();void* end=frames.empty()?0:frames.data()+frames.size();
    std::memcpy(source,&begin,4);std::memcpy(source+4,&end,4);std::memcpy(source+8,&end,4);
    Word(channel+4,type);std::uint16_t pivot=0xa735;std::memcpy(channel+12,&pivot,2);
    std::memcpy(settings,&error,4);std::memcpy(settings+4,&error,4);std::memcpy(settings+8,&error,4);std::memcpy(settings+16,&rate,4);
    const std::vector<unsigned char> before=frames;
    MethodInfo^ compress=Resolve(a,0x0600000e,3), ^estimate=Resolve(a,0x0600000f,1), ^destroy=Resolve(a,0x0600005b,2);
    array<Object^>^ args=gcnew array<Object^>(3);args[0]=P(compress,0,channel);args[1]=P(compress,1,source);args[2]=P(compress,2,settings);
    unsigned char* expected=static_cast<unsigned char*>(Pointer::Unbox(Invoke(compress,args)));
    if(!expected) throw gcnew Exception("Original Timecoded allocation returned null");
    try {
        TimecodedChannel actual=CompressTimecoded(type,pivot,input,error,error,error,rate);
        Equal(expected,&actual.type,4,"type");Equal(expected+4,&actual.pivot,2,"pivot");
        const unsigned char* first=static_cast<const unsigned char*>(Ptr(expected+8));
        const unsigned char* last=static_cast<const unsigned char*>(Ptr(expected+12));
        std::size_t count=first ? static_cast<std::size_t>(last-first)/24 : 0;
        if(count!=actual.frames.size()) throw gcnew Exception("Timecoded compressed frame count mismatch");
        for(std::size_t i=0;i<count;++i) {
            const unsigned char* f=first+i*24;
            Equal(f,&actual.frames[i].index,4,"frame index");
            unsigned char flag=actual.frames[i].binaryMove?1:0;Equal(f+20,&flag,1,"BinaryMove");
            const unsigned char* valuesBegin=static_cast<const unsigned char*>(Ptr(f+4));
            const unsigned char* valuesEnd=static_cast<const unsigned char*>(Ptr(f+8));
            if(static_cast<std::size_t>(valuesEnd-valuesBegin)!=actual.frames[i].values.size()*4) throw gcnew Exception("Timecoded vector width mismatch");
            Equal(valuesBegin,actual.frames[i].values.data(),actual.frames[i].values.size()*4,"frame values");
        }
        args=gcnew array<Object^>(1);args[0]=P(estimate,0,expected);
        std::uint32_t originalEstimate=safe_cast<UInt32>(Invoke(estimate,args));
        std::uint32_t nativeEstimate=EstimateTimecodedSize(actual);Equal(&originalEstimate,&nativeEstimate,4,"EstimateSize");
        Equal(frames.data(),before.data(),frames.size(),"source raw frames unchanged");
        for(std::size_t i=0;i<input.size();++i) Equal(values[i].data(),input[i].values.data(),values[i].size()*4,"source values unchanged");
        // Original WriteOut dereferences the first frame even when empty.
        // Compare Compress/Estimate for zero frames without invoking that UB.
        if(!actual.frames.empty()) {WriteCase(a,expected,actual,false);WriteCase(a,expected,actual,true);}
        ++channels;
    } finally {
        args=gcnew array<Object^>(2);args[0]=P(destroy,0,expected);args[1]=static_cast<UInt32>(1);Invoke(destroy,args);
    }
}
int main(array<String^>^ args) {
    try {
        if(IntPtr::Size!=4 || args->Length!=1) throw gcnew Exception("Usage: x86 W3xTimecodedOracle original.dll");
        Assembly^ a=Assembly::LoadFrom(args[0]);
        const int counts[]={0,1,2,3,9,33};const float errors[]={0.f,.001f,-1.f,.5f};const float rates[]={0.f,.5f,1.f,2.f};
        for(unsigned type=0;type<5;++type) for(int ci=0;ci<6;++ci)
        for(int pattern=0;pattern<4;++pattern) for(int base=0;base<2;++base)
        for(int ei=0;ei<4;++ei) for(int ri=0;ri<4;++ri) {
            std::vector<TimecodedFrame> input;
            for(int i=0;i<counts[ci];++i) {
                TimecodedFrame f;f.index=static_cast<unsigned>(i*3+(base?32766:0));f.binaryMove=pattern==3 && i%3==1;
                if(type==3) {
                    double angle=pattern==0?0:i*.125;double sign=pattern==2 && i%2 ? -1 : 1;
                    f.values={0.f,static_cast<float>(std::sin(angle)*sign),0.f,static_cast<float>(std::cos(angle)*sign)};
                } else f.values.push_back(pattern==0?.25f:pattern==1?static_cast<float>(i*.125-1):static_cast<float>(std::sin(i*.43)));
                input.push_back(f);
            }
            try {Test(a,type,input,errors[ei],rates[ri]);}
            catch(Exception^) {Console::Error->WriteLine("type={0} count={1} pattern={2}",type,counts[ci],pattern);Console::Error->WriteLine("base={0} error={1} rate={2}",base,errors[ei],rates[ri]);throw;}
        }
        // Finite source words with sNaN/qNaN/infinity after byte swap.
        // Original WriteOut's EndianToPlatform(float) return quiets swapped
        // signaling NaNs; this must be compared through the actual helper.
        const std::uint32_t endianWords[]={0xf6d882ffu,0x0100807fu,0x010080ffu,
            0x0000807fu,0x0100c07fu,0x00000000u,0x00000080u};
        for(unsigned wi=0;wi<sizeof(endianWords)/sizeof(endianWords[0]);++wi) {
            float value;
            std::memcpy(&value,&endianWords[wi],4);
            TimecodedFrame frame={0,std::vector<float>(1,value),false};
            Test(a,0,std::vector<TimecodedFrame>(1,frame),-1.f,1.f);
        }
        Console::WriteLine("PASS W3X Timecoded original differential channels={0} serializedChunks={1} comparedBytes={2}; Compress/Estimate/WriteOut; SDK not run",channels,serialized,comparedBytes);
        return 0;
    }catch(Exception^ e){Console::Error->WriteLine(e);return 1;}
     catch(const std::exception& e){Console::Error->WriteLine(gcnew String(e.what()));return 1;}
}
