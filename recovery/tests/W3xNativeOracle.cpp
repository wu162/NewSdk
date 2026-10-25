// x86 CLR4 component differential. Calls the original DLL's actual internal IL
// methods by their immutable metadata tokens; no SDK/BAB executable is launched.
#include "../src/Native/W3x/MeshConverters.h"
#include <cstdint>
#include <cstring>
#include <cmath>
#include <stdexcept>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::Reflection;
using namespace Recovered::W3x;

static int fixtures, comparisons;
static MethodInfo^ Method(Assembly^ a, int token) {
    MethodInfo^ m = dynamic_cast<MethodInfo^>(a->ManifestModule->ResolveMethod(token));
    if (!m || m->GetParameters()->Length != 6)
        throw gcnew Exception("Original converter signature changed");
    return m;
}
static void Original(MethodInfo^ m, int count, void* src, int srcPitch,
                     void* dst, int dstPitch, void* context) {
    array<ParameterInfo^>^ p = m->GetParameters();
    array<Object^>^ args = gcnew array<Object^>(6);
    args[0]=count; args[1]=Pointer::Box(src,p[1]->ParameterType); args[2]=srcPitch;
    args[3]=Pointer::Box(dst,p[3]->ParameterType); args[4]=dstPitch;
    args[5]=Pointer::Box(context,p[5]->ParameterType);
    try { m->Invoke(nullptr,args); }
    catch (TargetInvocationException^ e) { throw e->InnerException; }
}
static float Float(std::uint32_t bits) { float v; std::memcpy(&v,&bits,4); return v; }
static void Test(Assembly^ assembly, int converter, const float* values,
                 int count, int sourcePitch, int destinationPitch, bool big, int alignment=0) {
    static const int tokens[]={0x060000c2,0x060000c3,0x060000c4,0x060000c5,
                               0x060000c6,0x060000c7,0x060000c8,0x060000d2};
    MeshConversion kind=static_cast<MeshConversion>(converter);
    // CopyDirect<Vector3>'s BE assertion is confirmed by original IL; its native
    // exception ABI is deliberately outside this dynamic converter differential.
    if (kind==MeshConversion::CopyVector3 && big) return;
    unsigned char source[160], expected[160], actual[160], context[24];
    unsigned char sourceBefore[160]; bool written[160]={false};
    const int sourceBase=(sourcePitch<0 ? 96 : 16)+alignment;
    const int destinationBase=(destinationPitch<0 ? 96 : 16)+alignment;
    const int widths[]={8,4,4,8,1,1,4,12};
    std::memset(source,0x35,sizeof(source)); std::memset(expected,0xa7,sizeof(expected));
    std::memset(actual,0xa7,sizeof(actual)); std::memset(context,0,sizeof(context));
    std::uint16_t inverse[256];
    for (int i=0;i<256;++i) inverse[i]=static_cast<std::uint16_t>((i*37+261)&65535);
    std::uint16_t* first=inverse; std::uint16_t* end=inverse+256;
    std::memcpy(context,&first,4); std::memcpy(context+4,&end,4); std::memcpy(context+8,&end,4);
    bool* endian=&big; std::memcpy(context+20,&endian,4);
    for (int i=0;i<(count ? count : 1);++i) {
        unsigned char* at=source+sourceBase+(sourcePitch ? i*sourcePitch : 0);
        std::memcpy(at,values,16);
        if (kind==MeshConversion::ConvertBlendIndex) {
            std::uint16_t index=static_cast<std::uint16_t>((i*71+static_cast<unsigned>(values[0]))&255);
            std::memcpy(at,&index,2);
        }
    }
    for (int i=0;i<count;++i) for (int j=0;j<widths[converter];++j)
        written[destinationBase+i*destinationPitch+j]=true;
    std::memcpy(sourceBefore,source,sizeof(source));
    ConverterContext c; c.bigEndian=big; c.inverseBoneMapping=inverse; c.inverseBoneMappingCount=256;
    Original(Method(assembly,tokens[converter]),count,source+sourceBase,sourcePitch,
             expected+destinationBase,destinationPitch,context);
    if (std::memcmp(source,sourceBefore,sizeof(source)))
        throw gcnew Exception("Original converter mutated source data");
    for (int i=0;i<160;++i) if (!written[i] && expected[i]!=0xa7)
        throw gcnew Exception("Original converter wrote outside declared destination widths");
    ConvertMeshStream(kind,count,source+sourceBase,sourcePitch,actual+destinationBase,destinationPitch,c);
    if (std::memcmp(source,sourceBefore,sizeof(source)))
        throw gcnew Exception("Recovered converter mutated source data");
    for (int i=0;i<160;++i) if (actual[i]!=expected[i]) {
        Console::WriteLine("FAIL converter={0} count={1} srcPitch={2}",converter,count,sourcePitch);
        Console::WriteLine("dstPitch={0} BE={1} byte={2}",destinationPitch,big,i);
        Console::WriteLine("original={0:X2} recovered={1:X2}",static_cast<int>(expected[i]),static_cast<int>(actual[i]));
        Console::WriteLine("input bits {0:X8} {1:X8}",
            BitConverter::ToUInt32(BitConverter::GetBytes(values[0]),0),
            BitConverter::ToUInt32(BitConverter::GetBytes(values[1]),0));
        Console::WriteLine("input bits {0:X8} {1:X8}",
            BitConverter::ToUInt32(BitConverter::GetBytes(values[2]),0),
            BitConverter::ToUInt32(BitConverter::GetBytes(values[3]),0));
        throw gcnew Exception("Original converter byte differential failed");
    }
    ++fixtures; comparisons+=160;
}
int main(array<String^>^ args) {
    try {
        if (IntPtr::Size!=4) throw gcnew Exception("x86 CLR host required");
        if (args->Length!=1) throw gcnew Exception("Usage: W3xNativeOracle original.dll");
        Assembly^ original=Assembly::LoadFrom(args[0]);
        // Preserve raw signaling-NaN source bits: Float(bits)'s float return
        // can itself quiet an x87 NaN before it reaches the tested converter.
        const std::uint32_t raw[][4]={{0x7f800001U,0x3f000000U,0,0},
            {0xff800001U,0x3f000000U,0,0},{0x7fa12345U,0x3f000000U,0,0},
            {0x3f000000U,0x7f800001U,0,0},{0x3f000000U,0xff800001U,0,0}};
        for(int i=0;i<5;++i) for(int be=0;be<2;++be) {
            float values[4];std::memcpy(values,raw[i],sizeof(values));
            Test(original,0,values,1,16,16,be!=0);
        }
        // All float exponent/sign families and mantissa endpoints now reach
        // BOTH implementations. These used to be original-only PROBE records.
        const std::uint32_t mantissas[]={0,1,0x003fffffU,0x007fffffU};
        for(unsigned exponent=0;exponent<256;++exponent)
            for(unsigned sign=0;sign<2;++sign)
                for(unsigned mantissa=0;mantissa<4;++mantissa) {
                    std::uint32_t rawBits[4];
                    for(unsigned component=0;component<4;++component)
                        rawBits[component]=(sign<<31)|(exponent<<23)|mantissas[(mantissa+component)%4];
                    float values[4];std::memcpy(values,rawBits,sizeof(values));
                    for(int be=0;be<2;++be) {
                        Test(original,2,values,3,20,20,be!=0,1);
                        Test(original,5,values,3,20,20,be!=0,1);
                        Test(original,6,values,3,20,20,be!=0,1);
                    }
                }
        // Float neighbors of binary64 arithmetic/conversion boundaries:
        // transformed signed64 limits, former signed32/u32 guard limits,
        // and the loss of +0.5 at binary64's 53-bit precision boundary.
        const std::uint32_t weightBoundaries[]={
            0x5b00807fU,0x5b008080U,0x5b008081U,0x5b008082U,0x5b008083U,
            0xdb00807fU,0xdb008080U,0xdb008081U,0xdb008082U,0xdb008083U,
            0xcb00807fU,0xcb008080U,0xcb008081U,0xcb008082U,0xcb008083U,
            0x4b80807fU,0x4b808080U,0x4b808081U,0x4b808082U,0x4b808083U,
            0x5600807fU,0x56008080U,0x56008081U,0x56008082U,0x56008083U};
        for(unsigned i=0;i<sizeof(weightBoundaries)/sizeof(weightBoundaries[0]);++i) {
            std::uint32_t rawBits[4]={weightBoundaries[i],0,0,0};
            float values[4];std::memcpy(values,rawBits,sizeof(values));
            Test(original,5,values,3,0,20,false);
            Test(original,5,values,3,-20,-20,true,1);
        }
        const std::uint32_t bits[]={0,0x80000000,1,0x80000001,0x007fffff,0x00800000,
            0x33000000,0x33800000,0x387fffff,0x38800000,0x3effffff,0x3f000000,
            0x3f000001,0x3f7fffff,0x3f800000,0x3f800001,0xbf000000,0xbf800000,
            0x477fefff,0x477ff000,0x47800000,0xc7800000,0x4b7fffff,0xcb000000,
            0x7f7fffff,0xff7fffff,0x7f800000,0xff800000,0x7fc00000,0xffc00000,
            0x7fc12345,0x7f800001,0x08800000};
        for (int converter=0;converter<8;++converter)
            for (int be=0;be<2;++be) {
                for (int seed=0;seed<static_cast<int>(sizeof(bits)/sizeof(bits[0]));++seed) {
                    float v[4]; for (int j=0;j<4;++j) v[j]=Float(bits[(seed+j)%(sizeof(bits)/sizeof(bits[0]))]);
                    // Avoid UB in the integer-only index fixture generator.
                    if (converter==4) for (int j=0;j<4;++j) v[j]=static_cast<float>(seed+j);
                    Test(original,converter,v,1,16,16,be!=0);
                    Test(original,converter,v,3,20,20,be!=0);
                    Test(original,converter,v,3,0,20,be!=0);
                    Test(original,converter,v,3,20,0,be!=0);
                    Test(original,converter,v,3,-20,-20,be!=0);
                    Test(original,converter,v,3,20,20,be!=0,1);
                }
                std::uint32_t random=0x1729ab31U;
                for (int seed=0;seed<512;++seed) {
                    float v[4]; for (int j=0;j<4;++j) {
                        random=random*1664525U+1013904223U;
                        v[j]=converter==4 ? static_cast<float>(random&255U) : Float(random);
                    }
                    Test(original,converter,v,3,20,20,be!=0);
                }
                float zero[4]={0,0,0,0}; Test(original,converter,zero,0,0,0,be!=0);
            }
        Console::WriteLine("PASS W3X converter original differential fixtures={0} comparedBytes={1} allFormer728BoundaryProbesIncluded=true; SDK not run",fixtures,comparisons);
        return 0;
    } catch (Exception^ e) { Console::Error->WriteLine(e); return 1; }
      catch (const std::exception& e) { Console::Error->WriteLine(gcnew String(e.what())); return 1; }
}
