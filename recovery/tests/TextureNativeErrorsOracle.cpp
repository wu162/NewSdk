#include <Windows.h>
#include <cstdint>
#include <cstring>
#include <fstream>
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
typedef const char* (__cdecl *ErrorFn)(std::int32_t);
void Check(bool ok,const char* text){if(!ok)throw std::runtime_error(text);}
void Isolated(){Check(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll")==nullptr,"Original native module loaded in isolated replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file present in isolated replay");}
std::vector<std::int32_t> Codes(){
    std::vector<std::int32_t> out;
    for(int i=-4096;i<=4096;++i)out.push_back(i);
    for(unsigned i=0;i<32;++i){std::uint32_t m=1u<<i;out.push_back(static_cast<std::int32_t>(m));out.push_back(static_cast<std::int32_t>(~m));}
    for(std::uint32_t n:{0x7fffffffu,0x80000000u,0x80000064u,0xffffff9cu})out.push_back(static_cast<std::int32_t>(n));
    std::uint32_t state=0x426cab18u;for(int i=0;i<4096;++i){state=state*1664525u+1013904223u;out.push_back(static_cast<std::int32_t>(state));}
    Check(out.size()==12357,"Incomplete status-code corpus");return out;
}
void Run(ErrorFn original,ErrorFn recovered,const char* path){
    const auto codes=Codes();std::uint64_t bytes=0,aliases=0;
    if(original){
        std::ofstream out(path,std::ios::binary);out.write("TXERR001",8);std::uint32_t count=static_cast<std::uint32_t>(codes.size());out.write(reinterpret_cast<const char*>(&count),4);
        for(auto code:codes){
            const char* expected=original(code);const char* actual=recovered(code);
            Check(expected&&actual,"Null native string");Check(expected==original(code)&&actual==recovered(code),"Native status storage is not static");
            std::uint32_t length=static_cast<std::uint32_t>(std::strlen(expected)+1);Check(length<=256,"Native status length bound");
            Check(std::strlen(actual)+1==length&&std::memcmp(expected,actual,length)==0,"Complete status bytes including spaces/NUL differ");
            out.write(reinterpret_cast<const char*>(&code),4);out.write(reinterpret_cast<const char*>(&length),4);out.write(expected,length);bytes+=length;
        }
        // Pointer aliasing is part of the small native contract too: compare
        // every guarded-table entry and the shared undefined-code string.
        for(int a=-100;a<=1;++a)for(int b=-100;b<=1;++b){Check((original(a)==original(b))==(recovered(a)==recovered(b)),"Static pointer-alias relationship differs");++aliases;}
        Check(out.good(),"Cannot save full original status capture");
    }else{
        Isolated();std::ifstream in(path,std::ios::binary);char magic[8];std::uint32_t count=0;in.read(magic,8);in.read(reinterpret_cast<char*>(&count),4);
        Check(in.good()&&std::memcmp(magic,"TXERR001",8)==0&&count==codes.size(),"Wrong or incomplete original status capture");
        for(auto code:codes){
            std::int32_t saved;std::uint32_t length;in.read(reinterpret_cast<char*>(&saved),4);in.read(reinterpret_cast<char*>(&length),4);Check(in.good()&&saved==code&&length>0&&length<=256,"Corrupt original status record");
            std::vector<char> expected(length);in.read(expected.data(),length);Check(in.good()&&expected.back()==0,"Truncated original status string");
            Isolated();const char* actual=recovered(code);Check(actual&&std::strlen(actual)+1==length&&std::memcmp(actual,expected.data(),length)==0,"Isolated full native status bytes differ");Check(actual==recovered(code),"Isolated status lifetime differs");Isolated();bytes+=length;
        }
        Check(in.peek()==std::char_traits<char>::eof(),"Unexpected capture tail");
    }
    std::cout<<"PASS native PC status "<<(original?"differential":"NoOriginal")<<" records="<<codes.size()<<" aliasComparisons="<<aliases<<" fullStringBytes="<<bytes<<"; complete -100..0 mapping, int32 boundaries and unknown fallback, static lifetime and exact spaces/NUL\n";
}
}
#pragma managed(pop)

int main(array<String^>^ args){
    HMODULE module=nullptr;
    try{
        if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll|isolated recovered.dll capture");
        ErrorFn original=nullptr;
        if(args[0]!="isolated"){
            auto data=File::ReadAllBytes(args[0]);auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(data))->Replace("-","")->ToLowerInvariant();delete sha;
            if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Fixed original DLL changed");
            auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto handle=Marshal::GetHINSTANCE(assembly->ManifestModule);
            if(handle==IntPtr::Zero||handle==IntPtr(-1))throw gcnew Exception("Original module handle");
            auto start=static_cast<unsigned char*>(handle.ToPointer())+0x508d0;
            if(start[0]!=0x8b||start[1]!=0x44||start[2]!=0x24||start[3]!=4)throw gcnew Exception("Original status function guard");
            original=reinterpret_cast<ErrorFn>(start);
        }
        IntPtr dll=Marshal::StringToHGlobalUni(Path::GetFullPath(args[1]));module=LoadLibraryW(static_cast<const wchar_t*>(dll.ToPointer()));Marshal::FreeHGlobal(dll);
        Check(module!=nullptr,"Recovered native status DLL did not load");auto recovered=reinterpret_cast<ErrorFn>(GetProcAddress(module,"TextureGetErrorString"));Check(recovered!=nullptr,"Recovered C ABI export missing");
        IntPtr capture=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Run(original,recovered,static_cast<const char*>(capture.ToPointer()));Marshal::FreeHGlobal(capture);
        FreeLibrary(module);return 0;
    }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}
    if(module)FreeLibrary(module);return 1;
}
