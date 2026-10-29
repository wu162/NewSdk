#include "../../src/TextureCompiler/Native/TextureNativeDeviceApi.h"
#include <Windows.h>
#include <array>
#include <cstring>
#include <fstream>
#include <exception>
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
using namespace Recovered::TextureCompiler::NativeDeviceAbi;
struct Api { CreateFn create; ReleaseFn release; };
void Check(bool c,const char* why){if(!c)throw std::runtime_error(why);}
void Isolated(){Check(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll")==nullptr,"Original module loaded during device replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file in device replay directory");}
std::uint32_t Load(const void* p,std::size_t offset){std::uint32_t n;std::memcpy(&n,static_cast<const unsigned char*>(p)+offset,4);return n;}
void Store(void* p,std::size_t offset,std::uint32_t n){std::memcpy(static_cast<unsigned char*>(p)+offset,&n,4);}
struct Trace {
 std::vector<unsigned char> bytes;
 void number(std::uint32_t n){for(int i=0;i<4;++i)bytes.push_back(static_cast<unsigned char>(n>>(i*8)));}
 void add(const void* p,std::size_t n){auto b=static_cast<const unsigned char*>(p);bytes.insert(bytes.end(),b,b+n);}
};
Api Bind(unsigned char* original,HMODULE dll){
 Api a=original?Api{reinterpret_cast<CreateFn>(original+0x1c10),reinterpret_cast<ReleaseFn>(original+0x1ad0)}:Api{reinterpret_cast<CreateFn>(GetProcAddress(dll,"TextureCreateDevice")),reinterpret_cast<ReleaseFn>(GetProcAddress(dll,"TextureDeviceRelease"))};
 Check(a.create&&a.release,"Missing device ABI exports");return a;
}
void Snapshot(Trace& t,void* device){
 Check((reinterpret_cast<std::uintptr_t>(device)&127)==0,"Device alignment must be 128 bytes");
 auto base=reinterpret_cast<void*>(static_cast<std::uintptr_t>(Load(static_cast<unsigned char*>(device)-4,0)));
 Check(reinterpret_cast<std::uintptr_t>(device)>reinterpret_cast<std::uintptr_t>(base)&&reinterpret_cast<std::uintptr_t>(device)-reinterpret_cast<std::uintptr_t>(base)<=131,"Hidden GlobalAlloc base");
 Check(GlobalSize(base)>=ObjectBytes+128,"Underlying GlobalAlloc size");
 Check(Load(device,0x2a88)==GetCurrentThreadId(),"Device must retain creating thread ID");
 auto commands=reinterpret_cast<void*>(static_cast<std::uintptr_t>(Load(device,0x3864)));
 MEMORY_BASIC_INFORMATION info{};Check(VirtualQuery(commands,&info,sizeof(info))==sizeof(info),"Command allocation query");
 Check(info.AllocationBase==commands&&info.BaseAddress==commands&&info.State==MEM_COMMIT&&info.Protect==PAGE_READWRITE&&info.Type==MEM_PRIVATE&&info.RegionSize>=CommandBytes,"Command storage ownership/protection");
 std::vector<unsigned char> snapshot(static_cast<unsigned char*>(device),static_cast<unsigned char*>(device)+ObjectBytes);
 // Only process addresses and the verified OS thread ID vary across independent
 // factories/processes. Check their semantics above, then archive every other
 // object byte and every byte of the owned logical command allocation.
 Store(snapshot.data(),0x2a88,1);Store(snapshot.data(),0x3864,1);
 t.add(snapshot.data(),snapshot.size());t.add(commands,CommandBytes);
}
void Case(const Api& a,bool isolated,Trace& t,std::uint32_t adapter,std::uint32_t type,std::uint32_t flags,unsigned seed,bool withParameters,bool verifyReleased=true){
 std::array<unsigned char,64> parameters;for(unsigned i=0;i<parameters.size();++i)parameters[i]=static_cast<unsigned char>(seed+i*31);auto before=parameters;
 auto window=reinterpret_cast<void*>(static_cast<std::uintptr_t>(0xa5000000u|seed));
 void* device=reinterpret_cast<void*>(0xdeadbeef);if(isolated)Isolated();
 const auto result=a.create(adapter,type,window,flags,withParameters?parameters.data():nullptr,&device);if(isolated)Isolated();
 Check(result==0&&device&&parameters==before,"Device factory HRESULT/output/ignored parameters");t.number(adapter);t.number(type);t.number(flags);t.number(withParameters?1:0);t.number(result);Snapshot(t,device);
 // Hold multiple references; each non-final release may only decrement +0x3c.
 const auto references=2+(seed%7);Store(device,0x3c,references);
 for(auto count=references;count>1;--count){
  std::array<unsigned char,ObjectBytes> old;std::memcpy(old.data(),device,ObjectBytes);
  if(isolated)Isolated();auto remaining=a.release(device);if(isolated)Isolated();
  Store(old.data(),0x3c,count-1);Check(remaining==count-1&&std::memcmp(device,old.data(),ObjectBytes)==0,"Release reference count or unrelated device bytes changed");t.number(remaining);
 }
 auto commands=reinterpret_cast<void*>(static_cast<std::uintptr_t>(Load(device,0x3864)));
 if(isolated)Isolated();Check(a.release(device)==0,"Final device release return");if(isolated)Isolated();
 if(verifyReleased){MEMORY_BASIC_INFORMATION info{};Check(VirtualQuery(commands,&info,sizeof(info))==sizeof(info)&&info.State==MEM_FREE,"Final release must free command storage");}
 Check(parameters==before,"Device release changed caller input");t.number(0);
}
struct ThreadContext{Api api;bool isolated;Trace trace;std::exception_ptr failure;unsigned seed;};
DWORD WINAPI ThreadCases(void* p){auto& c=*static_cast<ThreadContext*>(p);try{for(unsigned i=0;i<16;++i)Case(c.api,c.isolated,c.trace,i,1,0xffffffffu,c.seed+i,(i&1)!=0,false);}catch(...){c.failure=std::current_exception();}return 0;}
Trace Run(Api a,bool isolated){
 Trace t;t.add("TXDEV001",8);
 const std::uint32_t values[]={0,1,3,4,0x7fffffff,0x80000000,0xfffffffe,0xffffffff};
 unsigned seed=0;
 for(auto adapter:values)for(auto type:values)for(auto flags:values)for(bool parameters:{false,true})Case(a,isolated,t,adapter,type,flags,seed++,parameters);
 // Simultaneously live devices must own distinct allocations. Releasing one
 // must leave all other complete object and command bytes intact.
 std::array<void*,16> devices{};std::array<std::vector<unsigned char>,16> snapshots;
 for(unsigned i=0;i<devices.size();++i){if(isolated)Isolated();Check(a.create(0,1,nullptr,0,nullptr,&devices[i])==0,"Multiple factory HRESULT");if(isolated)Isolated();for(unsigned j=0;j<i;++j)Check(devices[j]!=devices[i]&&Load(devices[j],0x3864)!=Load(devices[i],0x3864),"Shared device allocations");Trace one;Snapshot(one,devices[i]);snapshots[i]=one.bytes;}
 for(unsigned i=0;i<devices.size();++i){Trace one;Snapshot(one,devices[i]);Check(one.bytes==snapshots[i],"Other device lifetime modified storage");t.add(one.bytes.data(),one.bytes.size());if(isolated)Isolated();Check(a.release(devices[i])==0,"Multiple final release");if(isolated)Isolated();}
 // Real OS threads exercise per-device thread ownership without sharing state.
 std::array<ThreadContext,4> contexts;std::array<HANDLE,4> threads{};
 for(unsigned i=0;i<threads.size();++i){contexts[i].api=a;contexts[i].isolated=isolated;contexts[i].seed=i*16;threads[i]=CreateThread(nullptr,0,ThreadCases,&contexts[i],0,nullptr);Check(threads[i]!=nullptr,"Worker creation");}
 Check(WaitForMultipleObjects(static_cast<DWORD>(threads.size()),threads.data(),TRUE,INFINITE)==WAIT_OBJECT_0,"Worker completion");
 for(unsigned i=0;i<threads.size();++i){CloseHandle(threads[i]);if(contexts[i].failure)std::rethrow_exception(contexts[i].failure);t.add(contexts[i].trace.bytes.data(),contexts[i].trace.bytes.size());}
 // A non-final release is a uint32 decrement, including wraparound, even for
 // caller-owned objects. No allocator/release body is reached in these cases.
 for(auto count:{0u,2u,3u,0x7fffffffu,0x80000000u,0xffffffffu}){
  std::array<unsigned char,ObjectBytes> object;object.fill(0xa5);Store(object.data(),0x3c,count);auto expected=object;Store(expected.data(),0x3c,count-1);
  if(isolated)Isolated();auto result=a.release(object.data());if(isolated)Isolated();Check(result==count-1&&object==expected,"Non-final release exact object state/wraparound");t.number(count);t.number(result);t.add(object.data(),object.size());
 }
 return t;
}
void Compare(unsigned char* original,HMODULE dll,const char* path){
 auto actual=Run(Bind(nullptr,dll),original==nullptr);
 if(original){auto expected=Run(Bind(original,nullptr),false);Check(actual.bytes==expected.bytes,"Full device/object/command/state trace differs");std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(expected.bytes.data()),expected.bytes.size());Check(out.good(),"Save device original capture");}
 else{std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing complete device capture");std::vector<unsigned char> expected((std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>());Check(!in.bad()&&actual.bytes==expected,"Complete isolated device replay differs");}
 // Type 2 is an explicit recovered rejection, not a claimed oracle match.
 void* rejected=reinterpret_cast<void*>(0xdeadbeef);auto a=Bind(nullptr,dll);if(!original)Isolated();Check(a.create(0,2,nullptr,0,nullptr,&rejected)==static_cast<std::int32_t>(0x80004001u)&&rejected==nullptr,"Type-2 partial device must be rejected");if(!original)Isolated();
 std::cout<<"PASS native PC device "<<(original?"differential":"NoOriginal")<<" factoryCases=1024 liveDevices=16 workerCases=64 refcountBoundaries=6 fullObjectBytes=21760 commandBytes=4800 traceBytes="<<actual.bytes.size()<<"; type!=2, full state and allocation lifetime; type2/resources/OOM/mixed-mode identity excluded\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 HMODULE dll=nullptr;
 try{
  if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll|isolated recovered.dll capture");unsigned char* original=nullptr;
  if(args[0]!="isolated"){
   auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
   if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original device DLL fingerprint changed");
   auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto handle=Marshal::GetHINSTANCE(assembly->ManifestModule);if(handle==IntPtr::Zero||handle==IntPtr(-1))throw gcnew Exception("Original module handle");original=static_cast<unsigned char*>(handle.ToPointer());
   if(original[0x1c10]!=0xe9||original[0x1ad0]!=0xe9||original[0xe097]!=0x56||original[0x115fe]!=0x56)throw gcnew Exception("Original device entry guards");
  }
  auto name=Marshal::StringToHGlobalUni(Path::GetFullPath(args[1]));dll=LoadLibraryW(static_cast<const wchar_t*>(name.ToPointer()));Marshal::FreeHGlobal(name);Check(dll!=nullptr,"Recovered device DLL load");
  auto path=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Compare(original,dll,static_cast<const char*>(path.ToPointer()));Marshal::FreeHGlobal(path);FreeLibrary(dll);return 0;
 }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}
 if(dll)FreeLibrary(dll);return 1;
}
