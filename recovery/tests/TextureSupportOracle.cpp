// x86 CLR4 component differential against the original native function bodies.
// The fixed RVAs are guarded by the exact original input SHA256 in Main.
#include "../../src/TextureCompiler/Native/TextureSupport.h"
#include <cstdint>
#include <cstring>
#include <iostream>
#include <stdexcept>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Cryptography;

#pragma managed(push, off)
namespace {
using namespace Recovered::TextureCompiler;
struct OriginalBuffer { unsigned char* data; std::uint32_t size, capacity; };
static_assert(sizeof(OriginalBuffer)==12,"Original Texture SimpleBuffer requires x86");
typedef OriginalBuffer* (__thiscall *ConstructFn)(OriginalBuffer*, std::uint32_t);
typedef void (__thiscall *ResetFn)(OriginalBuffer*);
typedef void (__thiscall *AppendFn)(OriginalBuffer*, const void*, std::uint32_t);
typedef void* (__thiscall *DestroyFn)(OriginalBuffer*, std::uint32_t);
typedef std::uint32_t (__cdecl *EndianFn)(std::uint32_t);
typedef void* (__stdcall *CreateFn)(std::uint32_t);
typedef std::uint32_t (__cdecl *ReleaseFn)();
void Check(bool value,const char* what) { if(!value) throw std::runtime_error(what); }
void NativeOracle(unsigned char* base) {
    ConstructFn construct=reinterpret_cast<ConstructFn>(base+0x1c20);
    ResetFn reset=reinterpret_cast<ResetFn>(base+0x56b0);
    AppendFn append=reinterpret_cast<AppendFn>(base+0x1a00);
    DestroyFn destroy=reinterpret_cast<DestroyFn>(base+0x1e90);
    EndianFn reverse=reinterpret_cast<EndianFn>(base+0x5350);
    CreateFn create=reinterpret_cast<CreateFn>(base+0x1c00);
    ReleaseFn release=reinterpret_cast<ReleaseFn>(base+0x1ac0);
    std::uint64_t bytes=0, checks=0;
    const std::uint32_t capacities[]={1,2,3,7,16,31,64,255,1024,65536};
    const std::uint32_t counts[]={0,1,2,3,7,15,31,64,128,513,4097,0,17};
    for(std::uint32_t initial : capacities) {
        OriginalBuffer original; SimpleBuffer recovered;
        Check(construct(&original,initial)==&original,"constructor return");
        ConstructBuffer(&recovered,initial);
        Check(original.size==0 && original.capacity==initial,"constructor fields");
        Check(original.data && recovered.data,"constructor allocated storage");
        for(unsigned round=0;round<4;++round) {
            for(std::uint32_t count : counts) {
                std::vector<unsigned char> source(count ? count : 1);
                for(std::uint32_t i=0;i<count;++i) source[i]=static_cast<unsigned char>((i*13+count*17+round*19)&255);
                const std::vector<unsigned char> before=source;
                append(&original,source.data(),count);
                AppendBuffer(&recovered,source.data(),count);
                Check(original.size==recovered.size,"append size");
                Check(original.capacity==recovered.capacity,"append capacity doubling");
                Check(source==before,"append source mutation");
                Check(!original.size || std::memcmp(original.data,recovered.data,original.size)==0,"append payload");
                bytes+=original.size;checks+=4;
            }
            unsigned char* originalPointer=original.data;
            unsigned char* recoveredPointer=recovered.data;
            std::uint32_t capacity=original.capacity;
            reset(&original);ResetBuffer(&recovered);
            Check(!original.size && !recovered.size,"reset size");
            Check(original.capacity==capacity && recovered.capacity==capacity,"reset capacity");
            Check(original.data==originalPointer && recovered.data==recoveredPointer,"reset allocation retention");
            Check(std::memcmp(original.data,recovered.data,capacity<256 ? capacity:256)==0,"reset preserved payload");
            checks+=4;
        }
        Check(destroy(&original,0)==&original,"destructor return");
        DestroyBuffer(&recovered);
        Check(!original.data && !original.size && !original.capacity,"destructor fields");
        Check(!recovered.data && !recovered.size && !recovered.capacity,"recovered destructor fields");
        Check(destroy(&original,0)==&original,"repeated destructor return");
        DestroyBuffer(&recovered);checks+=4;
    }
    // Exhaust every 16-bit value in both halves and both low/high byte positions.
    for(std::uint32_t n=0;n<65536;++n) {
        const std::uint32_t values[]={n,n<<16,(n<<8)^0xa50000ffU,(n<<16)|n};
        for(std::uint32_t value : values) {
            Check(reverse(value)==ReverseEndianness(value),"endian native differential");
            ++checks;
        }
    }
    for(std::uint32_t sdk : {0u,1u,32u,0xffffffffu}) {
        Check(create(sdk)==Direct3DCreate9(sdk),"Direct3DCreate9 constant facade");++checks;
    }
    Check(release()==Direct3DRelease(),"Direct3D.Release constant facade");++checks;
    std::cout << "PASS TextureSupport original native differential: checks=" << checks
              << " payloadBytes=" << bytes << "\n";
}
}
#pragma managed(pop)

int main(array<String^>^ args) {
    try {
        if(IntPtr::Size!=4 || args->Length!=1) throw gcnew Exception("Usage: x86 TextureSupportOracle original.dll");
        String^ input=Path::GetFullPath(args[0]);
        SHA256^ sha=SHA256::Create();
        String^ actual=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(input)))->Replace("-","")->ToLowerInvariant();
        delete sha;
        if(actual!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")
            throw gcnew Exception("Original TextureCompiler SHA256 changed; native RVAs must be reviewed");
        Assembly^ original=Assembly::LoadFrom(input);
        IntPtr module=Marshal::GetHINSTANCE(original->ManifestModule);
        if(module==IntPtr::Zero || module==IntPtr(-1)) throw gcnew Exception("Original native module handle unavailable");
        NativeOracle(static_cast<unsigned char*>(module.ToPointer()));
        return 0;
    }
    catch(Exception^ e) { Console::Error->WriteLine(e);return 1; }
    catch(const std::exception& e) { std::cerr << e.what() << "\n";return 1; }
}
