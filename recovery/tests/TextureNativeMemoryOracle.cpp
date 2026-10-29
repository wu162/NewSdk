#include "../../src/TextureCompiler/Native/TextureNativeMemoryApi.h"
#include <Windows.h>
#include <cstring>
#include <fstream>
#include <iterator>
#include <type_traits>
#include <iostream>
#include <stdexcept>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Cryptography;
#pragma managed(push,off)
namespace {
using namespace Recovered::TextureCompiler::NativeAbi;
using NativeBuffer=Recovered::TextureCompiler::NativeAbi::Buffer;
static_assert(sizeof(NativeBuffer)==12,"Original native object layout requires x86");
struct Api {AllocateFn alloc;FreeFn free;ConstructFn construct;ResetFn reset;AppendFn append;DeleteFn destroy;ReverseFn reverse;CreateFn create;ReleaseFn release;};
void Check(bool c,const char* why){if(!c)throw std::runtime_error(why);}
void Isolated(){Check(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll")==nullptr,"Original module loaded during isolated native memory replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file in isolated directory");}
Api Bind(unsigned char* original,HMODULE dll){
    Api a;
    if(original){a={reinterpret_cast<AllocateFn>(original+0x4e74c),reinterpret_cast<FreeFn>(original+0x4e758),reinterpret_cast<ConstructFn>(original+0x1c20),reinterpret_cast<ResetFn>(original+0x56b0),reinterpret_cast<AppendFn>(original+0x1a00),reinterpret_cast<DeleteFn>(original+0x1e90),reinterpret_cast<ReverseFn>(original+0x5350),reinterpret_cast<CreateFn>(original+0x1c00),reinterpret_cast<ReleaseFn>(original+0x1ac0)};}
    else{
        a={reinterpret_cast<AllocateFn>(GetProcAddress(dll,"TextureAllocate")),reinterpret_cast<FreeFn>(GetProcAddress(dll,"TextureFree")),reinterpret_cast<ConstructFn>(GetProcAddress(dll,"TextureBufferConstruct")),reinterpret_cast<ResetFn>(GetProcAddress(dll,"TextureBufferReset")),reinterpret_cast<AppendFn>(GetProcAddress(dll,"TextureBufferAppend")),reinterpret_cast<DeleteFn>(GetProcAddress(dll,"TextureBufferDelete")),reinterpret_cast<ReverseFn>(GetProcAddress(dll,"TextureReverseEndianness")),reinterpret_cast<CreateFn>(GetProcAddress(dll,"TextureDirect3DCreate9")),reinterpret_cast<ReleaseFn>(GetProcAddress(dll,"TextureDirect3DRelease"))};
    }
    Check(a.alloc&&a.free&&a.construct&&a.reset&&a.append&&a.destroy&&a.reverse&&a.create&&a.release,"Missing native ABI export");return a;
}
struct Trace {
    std::vector<unsigned char> data;
    void number(std::uint32_t n){for(int i=0;i<4;++i)data.push_back(static_cast<unsigned char>(n>>(i*8)));}
    void buffer(const NativeBuffer& b){Check(b.size<=b.capacity&&b.size<1048576,"NativeBuffer bounds");number(b.size);number(b.capacity);if(b.size){Check(b.data!=nullptr,"Missing buffer storage");data.insert(data.end(),b.data,b.data+b.size);}}
};
template<class F> auto Call(bool isolated,F f)->decltype(f()){
    if(isolated)Isolated();
    if constexpr(std::is_void<decltype(f())>::value){f();if(isolated)Isolated();}
    else{auto value=f();if(isolated)Isolated();return value;}
}
#define API(expression) Call(isolated,[&](){return(expression);})
Trace Run(const Api& a,bool isolated){
    Trace t;
    for(std::uint32_t initial:{1u,2u,3u,7u,16u,31u,64u,255u,1024u,65536u}){
        if(isolated)Isolated();NativeBuffer b;Check(API(a.construct(&b,initial))==&b,"Thiscall constructor return/stack");Check(b.size==0&&b.capacity==initial&&b.data,"Constructor fields");t.buffer(b);
        for(unsigned round=0;round<4;++round){
            for(std::uint32_t count:{0u,1u,2u,3u,7u,15u,31u,64u,128u,513u,4097u,0u,17u}){
                std::vector<unsigned char> source(count?count:1);for(std::uint32_t i=0;i<count;++i)source[i]=static_cast<unsigned char>(i*13+count*17+round*19);auto before=source;
                const auto size=b.size;const auto capacity=b.capacity;auto pointer=b.data;if(isolated)Isolated();API(a.append(&b,source.data(),count));if(isolated)Isolated();
                Check(source==before&&b.size==size+count,"Append/source ownership");if(b.size<=capacity)Check(b.data==pointer,"Unexpected allocation without growth");t.buffer(b);
            }
            auto pointer=b.data;auto capacity=b.capacity;API(a.reset(&b));Check(b.data==pointer&&b.capacity==capacity&&b.size==0,"Reset storage retention");t.buffer(b);
        }
        Check(API(a.destroy(&b,0))==&b&&!b.data&&!b.size&&!b.capacity,"Thiscall destructor body");Check(API(a.destroy(&b,0))==&b,"Repeated cleared-buffer destructor");t.buffer(b);if(isolated)Isolated();
    }
    for(std::uint32_t flags=0;flags<8;++flags){
        auto b=static_cast<NativeBuffer*>(API(a.alloc(sizeof(NativeBuffer))));Check(b,"Object allocator");Check(API(a.construct(b,1))==b,"Allocated-object constructor");unsigned char source[64];for(unsigned i=0;i<64;++i)source[i]=static_cast<unsigned char>(i*29);API(a.append(b,source,64));t.buffer(*b);
        Check(API(a.destroy(b,flags))==b,"Deleting-destructor return");if(!(flags&1)){Check(!b->data&&!b->size&&!b->capacity,"Nondeleting flag state");API(a.free(b));}t.number(flags);if(isolated)Isolated();
    }
    for(std::uint32_t n=0;n<65536;++n){for(std::uint32_t value:{n,n<<16,(n<<8)^0xa50000ffu,(n<<16)|n}){if(isolated)Isolated();t.number(API(a.reverse(value)));if(isolated)Isolated();}}
    for(std::uint32_t version:{0u,1u,32u,0xffffffffu}){Check(API(a.create(version))==reinterpret_cast<void*>(1),"Stdcall facade return/stack");t.number(1);}
    t.number(API(a.release()));if(isolated)Isolated();return t;
}
void Compare(unsigned char* original,HMODULE dll,const char* path){
    auto recovered=Run(Bind(nullptr,dll),original==nullptr);
    if(original){auto expected=Run(Bind(original,nullptr),false);Check(recovered.data==expected.data,"Full native ABI state/payload/endian result differs");std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(expected.data.data()),expected.data.size());Check(out.good(),"Cannot archive original native trace");}
    else{std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing native trace");std::vector<unsigned char> expected((std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>());Check(!in.bad()&&recovered.data==expected,"Complete isolated native ABI replay differs");}
    std::cout<<"PASS native PC memory "<<(original?"differential":"NoOriginal")<<" buffers=10 appendOperations=520 deletionFlags=8 endianValues=262144 traceBytes="<<recovered.data.size()<<"; original x86 layout/thiscall/callee cleanup, growth/reset/payload, ownership and constant facades; invalid pointers/OOM/full devices excluded\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
    HMODULE dll=nullptr;
    try{
        if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll|isolated recovered.dll trace");unsigned char* original=nullptr;
        if(args[0]!="isolated"){
            auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
            if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original memory ABI DLL fingerprint changed");auto a=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto handle=Marshal::GetHINSTANCE(a->ManifestModule);if(handle==IntPtr::Zero||handle==IntPtr(-1))throw gcnew Exception("Original module handle");original=static_cast<unsigned char*>(handle.ToPointer());
            if(original[0x4e74c]!=0xff||original[0x4e74d]!=0x25||original[0x4e758]!=0xff||original[0x4e759]!=0x25)throw gcnew Exception("Original allocator thunk guards");
        }
        auto name=Marshal::StringToHGlobalUni(Path::GetFullPath(args[1]));dll=LoadLibraryW(static_cast<const wchar_t*>(name.ToPointer()));Marshal::FreeHGlobal(name);Check(dll,"Recovered native memory DLL did not load");
        auto trace=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Compare(original,dll,static_cast<const char*>(trace.ToPointer()));Marshal::FreeHGlobal(trace);FreeLibrary(dll);return 0;
    }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}if(dll)FreeLibrary(dll);return 1;
}
