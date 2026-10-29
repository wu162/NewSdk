#include "../../src/TextureCompiler/Native/TextureNativeResourceApi.h"
#include <Windows.h>
#include <array>
#include <cstring>
#include <exception>
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
using namespace Recovered::TextureCompiler::NativeResourceAbi;
struct Api{RefFn add;RefFn release;UnlockFn unlock;};
void Check(bool c,const char* why){if(!c)throw std::runtime_error(why);}
void Isolated(){Check(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll")==nullptr,"Original module loaded during resource replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file in resource replay directory");}
std::uint32_t Load(const void* p,unsigned offset){std::uint32_t n;std::memcpy(&n,static_cast<const unsigned char*>(p)+offset,4);return n;}
void Store(void* p,unsigned offset,std::uint32_t n){std::memcpy(static_cast<unsigned char*>(p)+offset,&n,4);}
std::uint32_t Address(void* p){return static_cast<std::uint32_t>(reinterpret_cast<std::uintptr_t>(p));}
Api Bind(unsigned char* original,HMODULE dll){Api a=original?Api{reinterpret_cast<RefFn>(original+0xc599),reinterpret_cast<RefFn>(original+0x1940),reinterpret_cast<UnlockFn>(original+0x1980)}:Api{reinterpret_cast<RefFn>(GetProcAddress(dll,"TextureResourceAddRef")),reinterpret_cast<RefFn>(GetProcAddress(dll,"TextureResourceRelease")),reinterpret_cast<UnlockFn>(GetProcAddress(dll,"TextureResourceUnlockRect"))};Check(a.add&&a.release&&a.unlock,"Missing resource exports");return a;}
struct Trace{std::vector<unsigned char> bytes;void number(std::uint32_t n){for(int i=0;i<4;++i)bytes.push_back(static_cast<unsigned char>(n>>(8*i)));}void add(const void* p,std::size_t n){auto b=static_cast<const unsigned char*>(p);bytes.insert(bytes.end(),b,b+n);}};
std::uint32_t Reference(const Api& a,bool isolated,void* p,bool add){if(isolated)Isolated();auto result=add?a.add(p):a.release(p);if(isolated)Isolated();return result;}
void* Object(std::uint32_t header,std::uint32_t refs){auto p=GlobalAlloc(GMEM_ZEROINIT,64);Check(p!=nullptr,"Fixture object allocation");Store(p,0,header);Store(p,4,refs);return p;}
void* Storage(){auto p=VirtualAlloc(nullptr,0x2000,MEM_COMMIT,PAGE_READWRITE);Check(p!=nullptr,"Fixture storage allocation");for(unsigned i=0;i<0x2000;++i)static_cast<unsigned char*>(p)[i]=static_cast<unsigned char>(i*17);return p;}
void Freed(void* p){MEMORY_BASIC_INFORMATION info{};Check(VirtualQuery(p,&info,sizeof(info))==sizeof(info)&&info.State==MEM_FREE,"Resource destruction did not release VM");}
void Lifetime(Api a,bool isolated,Trace& t,unsigned type,unsigned variant){
 auto p=Object(type,1);std::array<void*,2> storage{};unsigned owned=0;
 if(type==1||type==2||type==7||type==8){storage[owned++]=Storage();Store(p,0x18,Address(storage[0])|(type==1?(variant&3):0));}
 else if(type==3){storage[owned++]=Storage();if(variant&1)storage[owned++]=Storage();Store(p,0x20,Address(storage[0])|(variant&0xfff));Store(p,0x30,(owned==2?Address(storage[1]):0)|(variant&0xfff));}
 else if(type==6){storage[owned++]=Storage();Store(p,0x20,Address(storage[0]));}
 auto old=std::array<unsigned char,64>{};std::memcpy(old.data(),p,64);
 for(unsigned i=0;i<8;++i){auto n=Reference(a,isolated,p,true);Check(n==i+2,"Resource AddRef count");Store(old.data(),4,n);Check(std::memcmp(p,old.data(),64)==0,"AddRef changed unrelated bytes");t.number(n);}
 for(unsigned n=9;n>1;--n){auto r=Reference(a,isolated,p,false);Check(r==n-1,"Resource nonfinal Release count");Store(old.data(),4,r);Check(std::memcmp(p,old.data(),64)==0,"Release changed unrelated bytes");t.number(r);}
 for(unsigned i=0;i<owned;++i){for(unsigned j=0;j<0x2000;++j)Check(static_cast<unsigned char*>(storage[i])[j]==static_cast<unsigned char>(j*17),"Ref operations changed storage");t.add(storage[i],0x2000);}
 Check(Reference(a,isolated,p,false)==0,"Final Release count");for(unsigned i=0;i<owned;++i)Freed(storage[i]);t.number(type);t.number(variant);t.number(owned);t.number(0);
}
void ParentViews(Api a,bool isolated,Trace& t,unsigned children){
 auto parent=Object(3,1);auto storage=Storage();Store(parent,0x20,Address(storage)|6);std::vector<void*> views;
 for(unsigned i=0;i<children;++i){auto view=Object(0x40000004u,0);Store(view,0x18,Address(parent));views.push_back(view);Check(Reference(a,isolated,view,true)==1,"View first reference");Check(Load(parent,4)==i+2,"View must retain parent on transition 0 to 1");t.number(Load(parent,4));Check(Reference(a,isolated,view,true)==2&&Load(parent,4)==i+2,"Extra view reference must not retain parent twice");Check(Reference(a,isolated,view,false)==1&&Load(parent,4)==i+2,"Nonfinal view release changed parent");}
 Check(Reference(a,isolated,parent,false)==children,"Drop original parent owner");
 for(unsigned i=0;i<children;++i){Check(Reference(a,isolated,views[i],false)==0,"View final release");if(i+1<children){Check(Load(parent,4)==children-i-1,"Final view releases one parent reference");t.number(Load(parent,4));for(unsigned j=0;j<0x2000;++j)Check(static_cast<unsigned char*>(storage)[j]==static_cast<unsigned char>(j*17),"Live sibling view storage changed");}else{Freed(storage);t.number(0);}}
}
void NestedViews(Api a,bool isolated,Trace& t,unsigned depth){
 auto root=Object(3,1);auto storage=Storage();Store(root,0x20,Address(storage)|6);void* parent=root;
 for(unsigned i=0;i<depth;++i){auto child=Object(0x40000004u,0);Store(child,0x18,Address(parent));parent=child;}
 Check(Reference(a,isolated,parent,true)==1&&Load(root,4)==2,"Nested view must retain full ancestor chain");
 Check(Reference(a,isolated,root,false)==1,"Nested root owner release");
 Check(Reference(a,isolated,parent,false)==0,"Nested view final recursive release");Freed(storage);t.number(depth);t.number(0);
}
struct Worker{Api api;void* resource;bool isolated;bool add;std::exception_ptr failure;};
DWORD WINAPI AtomicWorker(void* p){auto& w=*static_cast<Worker*>(p);try{for(unsigned i=0;i<4096;++i)Reference(w.api,w.isolated,w.resource,w.add);}catch(...){w.failure=std::current_exception();}return 0;}
void Atomic(Api a,bool isolated,Trace& t){auto p=Object(5,1);std::array<Worker,4> workers;std::array<HANDLE,4> handles{};
 for(bool adding:{true,false}){for(unsigned i=0;i<4;++i){workers[i]={a,p,isolated,adding,nullptr};handles[i]=CreateThread(nullptr,0,AtomicWorker,&workers[i],0,nullptr);Check(handles[i]!=nullptr,"Atomic worker create");}Check(WaitForMultipleObjects(4,handles.data(),TRUE,INFINITE)==WAIT_OBJECT_0,"Atomic worker join");for(unsigned i=0;i<4;++i){CloseHandle(handles[i]);if(workers[i].failure)std::rethrow_exception(workers[i].failure);}Check(Load(p,4)==(adding?16385u:1u),"Lost atomic reference updates");t.number(Load(p,4));}
 Check(Reference(a,isolated,p,false)==0,"Atomic fixture final release");}
Trace Run(Api a,bool isolated){Trace t;t.add("TXRES001",8);
 // Full old lock-count nibble x low resource type x high flag combinations,
 // secondary storage presence and four ignored level values.
 unsigned unlockCases=0;
 for(unsigned type=0;type<16;++type)for(unsigned locks=0;locks<16;++locks)for(auto high:{0u,0x40000000u,0x80000000u,0xf0000000u})for(bool secondary:{false,true})for(auto level:{0u,1u,31u,0xffffffffu}){
  std::array<unsigned char,64> p;p.fill(0xa5);Store(p.data(),0,high|(locks<<8)|type);Store(p.data(),0x30,secondary?0x12345003:3);auto expected=p;
  const auto before=Load(p.data(),0);Store(expected.data(),0,before-256);if((before&0xf00)==0x100){Store(expected.data(),0x14,0xffff0000u);if(secondary)Store(expected.data(),0x18,0xffff0000u);}
  if(isolated)Isolated();auto result=a.unlock(p.data(),level);if(isolated)Isolated();Check(result==0&&p==expected,"Unlock full header/dirty-range/state bytes");t.number(level);t.number(result);t.add(p.data(),p.size());++unlockCases;
 }
 const std::uint32_t counts[]={0,1,2,3,0x7ffffffe,0x7fffffff,0x80000000,0xfffffffe,0xffffffff};
 for(unsigned type=0;type<16;++type)for(auto high:{0u,0x40000000u,0x80000000u})for(auto count:counts)for(bool add:{false,true}){
  if((type==4&&(high&0x40000000u))||(!add&&count==1))continue;
  std::array<unsigned char,64> p;p.fill(0xa5);Store(p.data(),0,high|type);Store(p.data(),4,count);auto expected=p;auto result=Reference(a,isolated,p.data(),add);auto n=add?count+1:count-1;Store(expected.data(),4,n);Check(result==n&&p==expected,"Reference boundary exact state/wraparound");t.number(count);t.number(add?1:0);t.number(result);t.add(p.data(),p.size());
 }
 for(unsigned type=0;type<=8;++type)for(unsigned variant=0;variant<8;++variant)Lifetime(a,isolated,t,type,variant);
 for(unsigned children=1;children<=16;++children)ParentViews(a,isolated,t,children);
 for(unsigned depth=1;depth<=16;++depth)NestedViews(a,isolated,t,depth);
 Atomic(a,isolated,t);
 return t;
}
void Compare(unsigned char* original,HMODULE dll,const char* path){auto actual=Run(Bind(nullptr,dll),!original);
 if(original){auto expected=Run(Bind(original,nullptr),false);Check(actual.bytes==expected.bytes,"Complete resource lifetime/atomic/unlock trace differs");std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(expected.bytes.data()),expected.bytes.size());Check(out.good(),"Archive complete resource trace");}
 else{std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing resource capture");std::vector<unsigned char> expected((std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>());Check(!in.bad()&&actual.bytes==expected,"Complete isolated resource replay differs");}
 // Explicit unsupported final-destruction rejection must retain full input.
 for(auto header:{9u,0x80000004u}){std::array<unsigned char,64> object{};Store(object.data(),0,header);Store(object.data(),4,1);auto expected=object;if(!original)Isolated();Check(Bind(nullptr,dll).release(object.data())==0xffffffffu&&object==expected,"Unsupported GPU/context final destruction must preserve object");if(!original)Isolated();}
 std::cout<<"PASS native PC resource "<<(original?"differential":"NoOriginal")<<" unlockCases=8192 lifetimeCases=72 parentViewSequences=16 nestedViewSequences=16 atomicOperations=32768 traceBytes="<<actual.bytes.size()<<"; full state/storage/ownership and uint32 boundaries; type9/GPU contexts and LockRect excluded\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 HMODULE dll=nullptr;
 try{
  if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll|isolated recovered.dll capture");unsigned char* original=nullptr;
  if(args[0]!="isolated"){
   auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
   if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original resource DLL fingerprint changed");
   auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto handle=Marshal::GetHINSTANCE(assembly->ManifestModule);if(handle==IntPtr::Zero||handle==IntPtr(-1))throw gcnew Exception("Original module handle");original=static_cast<unsigned char*>(handle.ToPointer());
   if(original[0x1940]!=0xe9||original[0x1980]!=0x8b||original[0xc599]!=0x56)throw gcnew Exception("Original resource entry guards");
  }
  auto name=Marshal::StringToHGlobalUni(Path::GetFullPath(args[1]));dll=LoadLibraryW(static_cast<const wchar_t*>(name.ToPointer()));Marshal::FreeHGlobal(name);Check(dll!=nullptr,"Recovered resource DLL load");
  auto path=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Compare(original,dll,static_cast<const char*>(path.ToPointer()));Marshal::FreeHGlobal(path);FreeLibrary(dll);return 0;
 }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}
 if(dll)FreeLibrary(dll);return 1;
}
