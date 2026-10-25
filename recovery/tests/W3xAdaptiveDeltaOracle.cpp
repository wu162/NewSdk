// x86 CLR4 component oracle; never starts SDK/BAB. Original internal method
// tokens and layouts are from extraction 40162934653-1, SHA fixed by runner.
#include "../src/Native/W3x/AdaptiveDelta.h"
#include <cmath>
#include <cstdint>
#include <cstring>
#include <limits>
#include <stdexcept>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::Reflection;
using namespace Recovered::W3x;

static unsigned frameFixtures, channelFixtures, comparedBytes, nullChannels;
static std::uint32_t Bits(float value) { std::uint32_t b; std::memcpy(&b,&value,4); return b; }
static float Float(std::uint32_t bits) { float f; std::memcpy(&f,&bits,4); return f; }
static std::uint32_t ReadU32(const unsigned char* p) { std::uint32_t n; std::memcpy(&n,p,4); return n; }
static std::uint16_t ReadU16(const unsigned char* p) { std::uint16_t n; std::memcpy(&n,p,2); return n; }
static void WriteU32(unsigned char* p,std::uint32_t n) { std::memcpy(p,&n,4); }
static void* ReadPointer(const unsigned char* p) { void* n; std::memcpy(&n,p,4); return n; }
static MethodInfo^ Resolve(Assembly^ a,int token,int count) {
    MethodInfo^ m=dynamic_cast<MethodInfo^>(a->ManifestModule->ResolveMethod(token));
    if (!m || m->GetParameters()->Length!=count) throw gcnew Exception("Original adaptive delta signature changed");
    return m;
}
static Object^ Invoke(MethodInfo^ m,array<Object^>^ args) {
    try { return m->Invoke(nullptr,args); }
    catch(TargetInvocationException^ e) { throw e->InnerException; }
}
static void Equal(const void* expected,const void* actual,unsigned size,String^ label) {
    const unsigned char* e=static_cast<const unsigned char*>(expected);
    const unsigned char* a=static_cast<const unsigned char*>(actual);
    for(unsigned i=0;i<size;++i) if(e[i]!=a[i]) {
        Console::Error->WriteLine("Mismatch {0} byte={1}",label,i);
        Console::Error->WriteLine("original={0:X2} recovered={1:X2}",static_cast<unsigned>(e[i]),static_cast<unsigned>(a[i]));
        throw gcnew Exception("Adaptive delta original byte differential failed");
    }
    comparedBytes+=size;
}
static void FrameCase(MethodInfo^ original,unsigned filter,unsigned bits,
    const std::array<float,17>& values,float scale,float maxError,unsigned pointers) {
    // Guard both mutable outputs and the original native object. CompressFrame
    // must only read object+28, write exactly17 destination bytes, and optionally
    // write one float/bool through the output pointers.
    unsigned char object[68], objectBefore[68], expected[33], actual[33];
    std::memset(object,0x3c,sizeof(object)); WriteU32(object+8+28,bits);
    std::memcpy(objectBefore,object,sizeof(object));
    std::memset(expected,0xa5,sizeof(expected)); std::memset(actual,0xa5,sizeof(actual));
    float previousExpected[3]={123.5f,-789.25f,456.5f};
    float previousActual[3]={123.5f,-789.25f,456.5f};
    struct GuardedBool { unsigned char before; bool value; unsigned char after; };
    GuardedBool excessExpected, excessActual;
    std::memset(&excessExpected,0,sizeof(excessExpected));
    std::memset(&excessActual,0,sizeof(excessActual));
    excessExpected.before=excessActual.before=0x57;
    excessExpected.after=excessActual.after=0x79;
    excessExpected.value=excessActual.value=((pointers>>2)&1u)!=0;
    std::array<float,17> input=values;
    array<ParameterInfo^>^ p=original->GetParameters();
    array<Object^>^ args=gcnew array<Object^>(8);
    args[0]=Pointer::Box(object+8,p[0]->ParameterType);
    args[1]=Pointer::Box(expected+8,p[1]->ParameterType);
    args[2]=static_cast<int>(filter);
    args[3]=Pointer::Box(input.data(),p[3]->ParameterType);
    args[4]=scale;
    args[5]=Pointer::Box((pointers&1u)?previousExpected+1:nullptr,p[5]->ParameterType);
    args[6]=Pointer::Box((pointers&2u)?&excessExpected.value:nullptr,p[6]->ParameterType);
    args[7]=maxError;
    const float errorExpected=safe_cast<float>(Invoke(original,args));
    AdaptiveDeltaFrame frame;
    // Native implementation writes all17 bytes, so preserve sentinel around it
    // by copying the result into the guarded region only after the invocation.
    const float errorActual=CompressAdaptiveDeltaFrame(frame,filter,values,scale,bits,
        (pointers&1u)?previousActual+1:nullptr,
        (pointers&2u)?&excessActual.value:nullptr,maxError);
    actual[8]=frame.filter;
    for(unsigned i=0;i<16;++i) actual[9+i]=static_cast<unsigned char>(frame.delta[i]);
    try {
        Equal(expected,actual,sizeof(expected),"Frame and canaries");
        Equal(previousExpected,previousActual,sizeof(previousExpected),"Previous and canaries");
        Equal(&excessExpected,&excessActual,sizeof(excessExpected),"Error flag and canaries");
        Equal(&errorExpected,&errorActual,sizeof(float),"Returned error bits");
        Equal(objectBefore,object,sizeof(object),"Read-only original object");
        Equal(input.data(),values.data(),17*4,"Read-only samples");
    } catch(Exception^) {
        Console::Error->WriteLine("filter={0} bits={1} scale={2:X8}",filter,bits,Bits(scale));
        Console::Error->WriteLine("maxError={0:X8} pointers={1}",Bits(maxError),pointers);
        Console::Error->Write("samples=");
        for(unsigned i=0;i<17;++i) Console::Error->Write("{0:X8} ",Bits(values[i]));
        Console::Error->WriteLine();
        throw;
    }
    ++frameFixtures;
}
static void ChannelCase(MethodInfo^ original,MethodInfo^ estimate,
    const std::vector<std::vector<float> >& samples,float maxError) {
    const std::size_t width=samples.empty()?1:samples[0].size();
    // Source EASTL vector size16, SrcFrame size24: index0; value vector4..19;
    // BinaryMove20. Original AdaptiveDelta ignores index and BinaryMove.
    unsigned char source[16]={0}, channel[16]={0}, settings[24]={0};
    std::vector<unsigned char> frames(samples.size()*24u,0);
    std::vector<std::vector<float> > values=samples;
    for(std::size_t i=0;i<samples.size();++i) {
        unsigned char* f=frames.data()+i*24u;
        WriteU32(f,static_cast<std::uint32_t>(i*3u));
        void* begin=values[i].data(); void* end=values[i].data()+width;
        std::memcpy(f+4,&begin,4); std::memcpy(f+8,&end,4); std::memcpy(f+12,&end,4);
        f[20]=static_cast<unsigned char>(i%2u);
    }
    void* begin=frames.empty()?nullptr:frames.data();
    void* end=frames.empty()?nullptr:frames.data()+frames.size();
    std::memcpy(source,&begin,4); std::memcpy(source+4,&end,4); std::memcpy(source+8,&end,4);
    const std::uint32_t type=0x12345678u;
    const std::uint16_t pivot=0x2468u;
    WriteU32(channel+4,type); std::memcpy(channel+12,&pivot,2);
    std::memcpy(settings+12,&maxError,4);
    array<ParameterInfo^>^ p=original->GetParameters();
    array<Object^>^ args=gcnew array<Object^>(3);
    args[0]=Pointer::Box(channel,p[0]->ParameterType);
    args[1]=Pointer::Box(source,p[1]->ParameterType);
    args[2]=Pointer::Box(settings,p[2]->ParameterType);
    Object^ result=Invoke(original,args);
    unsigned char* expected=result ? static_cast<unsigned char*>(Pointer::Unbox(result)) : nullptr;
    AdaptiveDeltaChannel actual;
    const bool success=CompressAdaptiveDelta(type,pivot,samples,maxError,actual);
    if ((expected!=nullptr)!=success) {
        Console::Error->WriteLine("Channel null mismatch width={0} count={1} maxError={2:X8}",
            static_cast<unsigned>(width),static_cast<unsigned>(samples.size()),Bits(maxError));
        throw gcnew Exception("AdaptiveDelta Compress null differential failed");
    }
    ++channelFixtures;
    if (!success) { ++nullChannels; return; }
    const std::uint32_t expectedType=ReadU32(expected);
    const std::uint16_t expectedPivot=ReadU16(expected+4);
    const std::uint32_t expectedScale=ReadU32(expected+8), actualScale=Bits(actual.scale);
    const std::uint32_t expectedBits=ReadU32(expected+28), expectedWidth=ReadU32(expected+48);
    Equal(&expectedType,&actual.type,4,"Channel type"); Equal(&expectedPivot,&actual.pivot,2,"Channel pivot");
    Equal(&expectedScale,&actualScale,4,"Channel scale"); Equal(&expectedBits,&actual.bits,4,"Channel bits");
    if(expectedWidth!=actual.initial.size()) throw gcnew Exception("AdaptiveDelta width mismatch");
    unsigned char* initial=static_cast<unsigned char*>(ReadPointer(expected+12));
    unsigned char* initialEnd=static_cast<unsigned char*>(ReadPointer(expected+16));
    unsigned char* packedFrames=static_cast<unsigned char*>(ReadPointer(expected+32));
    unsigned char* packedEnd=static_cast<unsigned char*>(ReadPointer(expected+36));
    if(static_cast<std::size_t>(initialEnd-initial)!=actual.initial.size()*4u ||
       static_cast<std::size_t>(packedEnd-packedFrames)!=actual.frames.size()*17u)
        throw gcnew Exception("AdaptiveDelta native vector size mismatch");
    Equal(initial,actual.initial.data(),static_cast<unsigned>(actual.initial.size()*4u),"Channel initial values");
    for(std::size_t i=0;i<actual.frames.size();++i) {
        unsigned char frame[17]; frame[0]=actual.frames[i].filter;
        for(unsigned j=0;j<16;++j) frame[1+j]=static_cast<unsigned char>(actual.frames[i].delta[j]);
        Equal(packedFrames+i*17u,frame,17,"Channel packed frame");
    }
    array<Object^>^ estimateArgs=gcnew array<Object^>(1);
    estimateArgs[0]=Pointer::Box(expected,estimate->GetParameters()[0]->ParameterType);
    const std::uint32_t expectedEstimate=safe_cast<unsigned int>(Invoke(estimate,estimateArgs));
    const std::uint32_t actualEstimate=EstimateAdaptiveDeltaSize(actual);
    Equal(&expectedEstimate,&actualEstimate,4,"Original estimate formula");
    // Original owns this small allocation through MSVCR80. The standalone oracle
    // intentionally keeps it for its process lifetime rather than freeing it
    // through the host's unrelated CRT. Bounded fixture count <1MB in total.
}
int main(array<String^>^ args) {
    try {
        if(IntPtr::Size!=4) throw gcnew Exception("Adaptive delta oracle requires x86 CLR");
        if(args->Length!=1) throw gcnew Exception("Usage: W3xAdaptiveDeltaOracle original.dll");
        Assembly^ a=Assembly::LoadFrom(args[0]);
        MethodInfo^ frame=Resolve(a,0x06000011,8);
        MethodInfo^ compress=Resolve(a,0x06000012,3);
        MethodInfo^ estimate=Resolve(a,0x06000014,1);
        const float scales[]={0.0f,1.0f,Float(0x00000001u),Float(0x3e124925u)};
        const float errors[]={0.0f,0.03125f,100.0f};
        // Finite binary boundaries still force original conv.i4 overflow/NaN
        // internally when a zero/subnormal quantization step is selected.
        const std::uint32_t boundary[]={0,0x80000000u,1,0x80000001u,0x007fffffu,0x00800000u,
            0x3effffffu,0x3f000000u,0x3f000001u,0x3f7fffffu,0x3f800000u,0x3f800001u,
            0xbf800000u,0x4affffffu,0x4b000000u,0xcb000000u};
        for(unsigned pattern=0;pattern<12;++pattern) {
            std::array<float,17> values;
            for(unsigned i=0;i<17;++i) {
                if(pattern==0) values[i]=0.0f;
                else if(pattern==1) values[i]=2.0f;
                else if(pattern==2) values[i]=float(i);
                else if(pattern==3) values[i]=-float(i);
                else if(pattern==4) values[i]=(i&1u)?-10.0f:10.0f;
                else if(pattern==5) values[i]=float(i)*0.03125f;
                else values[i]=Float(boundary[(i+pattern-6)%16u]);
            }
            for(unsigned bits=4;bits<=8;bits+=4)
                for(unsigned filter=0;filter<256;++filter)
                    for(unsigned scale=0;scale<4;++scale)
                        for(unsigned error=0;error<3;++error) {
                            // Alternate null output pointers, and both initial
                            // bool values, without multiplying identical work.
                            const unsigned pointers=(pattern+filter+scale+error)&7u;
                            FrameCase(frame,filter,bits,values,scales[scale],errors[error],pointers);
                        }
        }
        const unsigned counts[]={0,1,2,15,16,17,31,32,33};
        for(unsigned width=1;width<=4;++width)
            for(unsigned ci=0;ci<sizeof(counts)/sizeof(counts[0]);++ci)
                for(unsigned pattern=0;pattern<3;++pattern) {
                    std::vector<std::vector<float> > samples(counts[ci],std::vector<float>(width));
                    std::uint32_t random=0x31415926u;
                    for(unsigned i=0;i<counts[ci];++i) for(unsigned j=0;j<width;++j) {
                        random=random*1664525u+1013904223u;
                        samples[i][j]=pattern==0?2.0f:(pattern==1?float(i+j)*0.125f:float(int(random%1000u)-500)/100.0f);
                    }
                    ChannelCase(compress,estimate,samples,0.0f);
                    ChannelCase(compress,estimate,samples,0.03f);
                    ChannelCase(compress,estimate,samples,1.0f);
                }
        Console::WriteLine(String::Format("PASS W3X adaptive delta original differential frameFixtures={0} channelFixtures={1}",frameFixtures,channelFixtures)+
            String::Format(" comparedBytes={0} nullChannels={1}; SDK not run",comparedBytes,nullChannels));
        return 0;
    } catch(Exception^ e) { Console::Error->WriteLine(e); return 1; }
      catch(const std::exception& e) { Console::Error->WriteLine(gcnew String(e.what())); return 1; }
}
