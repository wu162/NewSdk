#include "../../src/TextureCompiler/Native/TextureNativeSurfaceApi.h"
#include <Windows.h>
#include <array>
#include <cstring>
#include <fstream>
#include <iostream>
#include <iterator>
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
using namespace Recovered::TextureCompiler::NativeSurfaceAbi;
using U=std::uint32_t;
struct Api{LayoutFn layout;LockFn lock;};
void Check(bool c,const char* why){if(!c)throw std::runtime_error(why);}
void Isolated(){Check(!GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"),"Original loaded during surface replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file during surface replay");}
Api Bind(unsigned char* original,HMODULE dll){Api a=original?Api{reinterpret_cast<LayoutFn>(original+0xd24b),reinterpret_cast<LockFn>(original+0x1950)}:Api{reinterpret_cast<LayoutFn>(GetProcAddress(dll,"TextureSurfaceGetLayout")),reinterpret_cast<LockFn>(GetProcAddress(dll,"TextureSurfaceLockRect"))};Check(a.layout&&a.lock,"Missing native surface exports");return a;}
struct Trace{std::vector<unsigned char> bytes;void number(U n){for(int i=0;i<4;++i)bytes.push_back(static_cast<unsigned char>(n>>(8*i)));}void add(const void* p,std::size_t n){auto b=static_cast<const unsigned char*>(p);bytes.insert(bytes.end(),b,b+n);}};
U Address(void* p){return static_cast<U>(reinterpret_cast<std::uintptr_t>(p));}
std::array<U,16> Header(U dimension,U format,U size,U flags){
 constexpr U widths[]={1,4,15,16,17,31,64,127};constexpr U depths[]={1,2,3,4,5,7,8,16};
 const U w=widths[size],h=widths[(size*3+1)%8],d=depths[size];std::array<U,16> p{};
 p[0]=3|((size&3)<<8)|((size&1)?0x200000:0);p[1]=1;p[5]=size&1?0x00020001:0xffff0000;p[6]=size&1?0x00030002:0xffff0000;
 p[7]=(((w+31)/32)<<22)|((flags&1)?0x80000000u:0);p[8]=0x10000000u|format;
 if(dimension==1&&(size&1))p[8]|=0x400;
 p[9]=dimension==0?w-1:dimension==2?(w-1)|((h-1)<<11)|((d-1)<<22):(w-1)|((h-1)<<13);
 if(dimension==3||(p[8]&0x400))p[9]|=5u<<26;
 p[10]=(flags&2)?0x80000000u:0;p[11]=(size&1)?0x40:0;
 p[12]=0x20000000u|(dimension<<9)|((flags&4)?0x800:0);
 return p;
}
void Case(Api a,bool isolated,Trace& t,U dimension,U format,U size,U flags,U level,U ordinal){
 auto p=Header(dimension,format,size,flags);auto before=p;std::array<U,7> result;result.fill(0xcccccccc);void* address=reinterpret_cast<void*>(0xcccccccc);
 if(isolated)Isolated();a.layout(p.data(),size%6,level,&result[0],&result[1],&result[2],&result[3],&result[4],&result[5],&result[6],&address);if(isolated)Isolated();Check(p==before,"Layout changed input header");
 t.number(ordinal);t.add(result.data(),sizeof(result));t.number(Address(address));
 constexpr U lockFlags[]={0,1,0x10,0x11,0x20,0x200,0x1000,0xffffffff};
 Rect rect{static_cast<std::int32_t>((size+flags)%9)-1,static_cast<std::int32_t>((format+level)%5)-1,0x1234,0x5678};auto oldRect=rect;
 LockedRect out{static_cast<std::int32_t>(0xcccccccc),reinterpret_cast<void*>(0xcccccccc)};
 if(isolated)Isolated();auto code=a.lock(p.data(),level,&out,(ordinal&1)?&rect:nullptr,lockFlags[(size+flags+level)%8]);if(isolated)Isolated();
 Check(code==0&&std::memcmp(&rect,&oldRect,sizeof(rect))==0,"Public LockRect result/caller input");
 t.number(code);t.add(p.data(),sizeof(p));t.number(static_cast<U>(out.pitch));t.number(Address(out.bits));
}
void ActualStorage(Api a,bool isolated,Trace& t){
 for(U format:{2u,3u,6u,18u,19u,20u,35u,57u})for(U level:{0u,1u}){
  auto primary=static_cast<unsigned char*>(VirtualAlloc(nullptr,0x40000,MEM_COMMIT,PAGE_READWRITE));auto secondary=static_cast<unsigned char*>(VirtualAlloc(nullptr,0x40000,MEM_COMMIT,PAGE_READWRITE));Check(primary&&secondary,"Surface fixture storage allocation");
  for(U i=0;i<0x40000;++i){primary[i]=static_cast<unsigned char>(i*17+format);secondary[i]=static_cast<unsigned char>(i*29+format);}
  auto p=Header(1,format,6,0);p[0]=3;p[8]=Address(primary)|format;p[9]=63|(63<<13);p[11]=0;p[12]=Address(secondary)|(1<<9);
  LockedRect out{};if(isolated)Isolated();Check(a.lock(p.data(),level,&out,nullptr,0)==0,"Owned storage LockRect");if(isolated)Isolated();
  auto base=level?secondary:primary;const U offset=Address(out.bits)-Address(base);Check(offset<0x40000-64&&out.pitch>0,"LockRect pointer outside owned storage");
  for(U i=0;i<64;++i)static_cast<unsigned char*>(out.bits)[i]^=static_cast<unsigned char>(i*13);
  t.number(format);t.number(level);t.number(static_cast<U>(out.pitch));t.number(offset);
  p[8]=(p[8]&0xfff)|0x10000000;p[12]=(p[12]&0xfff)|0x20000000;t.add(p.data(),sizeof(p));
  t.add(primary,0x40000);t.add(secondary,0x40000);VirtualFree(primary,0,MEM_RELEASE);VirtualFree(secondary,0,MEM_RELEASE);
 }
}
Trace Run(Api a,bool isolated){Trace t;t.add("TXSRF001",8);constexpr U levels[]={0,1,2,3,4,5,7,8};U ordinal=0;
 for(U dimension=0;dimension<4;++dimension)for(U format=0;format<64;++format)for(U size=0;size<8;++size)for(U flags=0;flags<8;++flags)for(U level:levels)Case(a,isolated,t,dimension,format,size,flags,level,ordinal++);
 Check(ordinal==131072,"Complete surface matrix accounting");ActualStorage(a,isolated,t);return t;
}
void Compare(unsigned char* original,HMODULE dll,const char* path){
 // Original first: a difference can never be hidden by reusing the candidate's
 // state, output or storage; independent complete runs compare exact records.
 auto expected=original?Run(Bind(original,nullptr),false):Trace{};
 auto actual=Run(Bind(nullptr,dll),!original);
 if(original){
  if(actual.bytes!=expected.bytes){std::size_t i=0;while(i<actual.bytes.size()&&i<expected.bytes.size()&&actual.bytes[i]==expected.bytes[i])++i;std::cerr<<"Surface first difference byte="<<i<<" case="<<(i>=8?(i-8)/112:0)<<" expected="<<(i<expected.bytes.size()?static_cast<int>(expected.bytes[i]):-1)<<" actual="<<(i<actual.bytes.size()?static_cast<int>(actual.bytes[i]):-1)<<"\n";}
  // Save the entire original even on failure, for general-rule diagnosis.
  std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(expected.bytes.data()),expected.bytes.size());Check(out.good(),"Save full surface original trace");Check(actual.bytes==expected.bytes,"Complete surface layout/LockRect/state/storage trace differs");
 }else{std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing surface capture");std::vector<unsigned char> capture((std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>());Check(!in.bad()&&actual.bytes==capture,"Complete isolated surface replay differs");}
 std::cout<<"PASS native PC surface "<<(original?"differential":"NoOriginal")<<" layoutCases=131072 formats=64 dimensions=4 lockCases=131072 actualStorageCases=16 traceBytes="<<actual.bytes.size()<<"; full output/state/storage, mip/packed/tiled/border/array/rect/lock flags; invalid pointers/overflow/nontexture descriptors excluded\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 HMODULE dll=nullptr;
 try{
  if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll|isolated recovered.dll capture");unsigned char* original=nullptr;
  if(args[0]!="isolated"){
   auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
   if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original surface DLL fingerprint changed");
   auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto handle=Marshal::GetHINSTANCE(assembly->ManifestModule);if(handle==IntPtr::Zero||handle==IntPtr(-1))throw gcnew Exception("Original module handle");original=static_cast<unsigned char*>(handle.ToPointer());
   if(original[0xd24b]!=0x55||original[0x1950]!=0x8b||original[0xd70f]!=0x55)throw gcnew Exception("Original surface entry guards");
  }
  auto name=Marshal::StringToHGlobalUni(Path::GetFullPath(args[1]));dll=LoadLibraryW(static_cast<const wchar_t*>(name.ToPointer()));Marshal::FreeHGlobal(name);Check(dll!=nullptr,"Recovered surface DLL load");
  auto path=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Compare(original,dll,static_cast<const char*>(path.ToPointer()));Marshal::FreeHGlobal(path);FreeLibrary(dll);return 0;
 }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}
 if(dll)FreeLibrary(dll);return 1;
}
