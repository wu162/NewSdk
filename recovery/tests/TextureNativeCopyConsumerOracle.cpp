#include "../../src/TextureCompiler/Native/TextureNativeDeviceApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeResourceApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeSurfaceApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeDescriptorApi.h"
#include "../../src/TextureCompiler/Native/TextureNativeCopyApi.h"
#include <Windows.h>
#include <algorithm>
#include <array>
#include <cstring>
#include <fstream>
#include <iostream>
#include <iterator>
#include <stdexcept>
#include <string>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Cryptography;
#pragma managed(push,off)
namespace {
using namespace Recovered::TextureCompiler;
using U=std::uint32_t;
struct Api{NativeDeviceAbi::CreateFn create;NativeDeviceAbi::ReleaseFn deviceRelease;NativeResourceAbi::RefFn add,release;NativeResourceAbi::UnlockFn unlock;NativeSurfaceAbi::LayoutFn layout;NativeSurfaceAbi::LockFn lock;NativeDescriptorAbi::LevelFn level;NativeDescriptorAbi::VolumeFn volume;NativeDescriptorAbi::TextureFn texture;NativeCopyAbi::CopyFn copy;};
void Check(bool c,const char* why){if(!c)throw std::runtime_error(why);}
void Isolated(){Check(!GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"),"Original loaded during consumer replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file during consumer replay");}
U Address(const void* p){return static_cast<U>(reinterpret_cast<std::uintptr_t>(p));}
U Load(const void* p,unsigned n){U x;std::memcpy(&x,static_cast<const unsigned char*>(p)+n,4);return x;}
void Store(void* p,unsigned n,U x){std::memcpy(static_cast<unsigned char*>(p)+n,&x,4);}
Api Original(unsigned char* p){return {reinterpret_cast<NativeDeviceAbi::CreateFn>(p+0x1c10),reinterpret_cast<NativeDeviceAbi::ReleaseFn>(p+0x1ad0),reinterpret_cast<NativeResourceAbi::RefFn>(p+0xc599),reinterpret_cast<NativeResourceAbi::RefFn>(p+0x1940),reinterpret_cast<NativeResourceAbi::UnlockFn>(p+0x1980),reinterpret_cast<NativeSurfaceAbi::LayoutFn>(p+0xd24b),reinterpret_cast<NativeSurfaceAbi::LockFn>(p+0x1950),reinterpret_cast<NativeDescriptorAbi::LevelFn>(p+0xdb60),reinterpret_cast<NativeDescriptorAbi::VolumeFn>(p+0xdbf9),reinterpret_cast<NativeDescriptorAbi::TextureFn>(p+0x3df34),reinterpret_cast<NativeCopyAbi::CopyFn>(p+0x3ccc0)};}
Api Recovered(const std::string& directory,std::vector<HMODULE>& modules){
 auto module=[&](const char* name){auto m=LoadLibraryA((directory+"/Recovered.TextureNative"+name+".dll").c_str());Check(m!=nullptr,"Consumer native DLL load");modules.push_back(m);return m;};
 auto device=module("Device"),resource=module("Resource"),surface=module("Surface"),descriptor=module("Descriptor"),copy=module("Copy");
 Api a={reinterpret_cast<NativeDeviceAbi::CreateFn>(GetProcAddress(device,"TextureCreateDevice")),reinterpret_cast<NativeDeviceAbi::ReleaseFn>(GetProcAddress(device,"TextureDeviceRelease")),reinterpret_cast<NativeResourceAbi::RefFn>(GetProcAddress(resource,"TextureResourceAddRef")),reinterpret_cast<NativeResourceAbi::RefFn>(GetProcAddress(resource,"TextureResourceRelease")),reinterpret_cast<NativeResourceAbi::UnlockFn>(GetProcAddress(resource,"TextureResourceUnlockRect")),reinterpret_cast<NativeSurfaceAbi::LayoutFn>(GetProcAddress(surface,"TextureSurfaceGetLayout")),reinterpret_cast<NativeSurfaceAbi::LockFn>(GetProcAddress(surface,"TextureSurfaceLockRect")),reinterpret_cast<NativeDescriptorAbi::LevelFn>(GetProcAddress(descriptor,"TextureGetLevelDesc")),reinterpret_cast<NativeDescriptorAbi::VolumeFn>(GetProcAddress(descriptor,"TextureGetVolumeLevelDesc")),reinterpret_cast<NativeDescriptorAbi::TextureFn>(GetProcAddress(descriptor,"TextureXGGetTextureDesc")),reinterpret_cast<NativeCopyAbi::CopyFn>(GetProcAddress(copy,"TextureCopySurface"))};Check(a.create&&a.deviceRelease&&a.add&&a.release&&a.unlock&&a.layout&&a.lock&&a.level&&a.volume&&a.texture&&a.copy,"Consumer missing ABI entry");return a;
}
struct Trace{std::vector<unsigned char> bytes;void number(U x){for(int i=0;i<4;++i)bytes.push_back(static_cast<unsigned char>(x>>(8*i)));}void add(const void* p,std::size_t n){auto b=static_cast<const unsigned char*>(p);bytes.insert(bytes.end(),b,b+n);}void string(const std::string& s){number(static_cast<U>(s.size()));add(s.data(),s.size());}};
struct Reader{const std::vector<unsigned char>& bytes;std::size_t offset=0;U number(){Check(offset+4<=bytes.size(),"Truncated consumer fixture word");U n=Load(bytes.data(),static_cast<unsigned>(offset));offset+=4;return n;}std::vector<unsigned char> block(U n){Check(n<=0x8000000&&offset+n<=bytes.size(),"Truncated/oversized consumer fixture");std::vector<unsigned char> result(bytes.begin()+offset,bytes.begin()+offset+n);offset+=n;return result;}std::string string(){auto n=number();Check(n<256,"Consumer fixture name bound");auto b=block(n);return {b.begin(),b.end()};}};
std::vector<unsigned char> Read(const std::string& path){std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing consumer fixture/capture");return {(std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>()};}
void Save(const std::string& path,const Trace& t){std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(t.bytes.data()),t.bytes.size());Check(out.good(),"Cannot save consumer fixture/capture");}
void CaptureRegion(Trace& t,U base){if(!base){t.number(0);return;}MEMORY_BASIC_INFORMATION info{};auto p=reinterpret_cast<void*>(base);Check(VirtualQuery(p,&info,sizeof(info))==sizeof(info)&&info.BaseAddress==p&&info.AllocationBase==p&&info.State==MEM_COMMIT&&info.RegionSize<=0x8000000,"Original texture owned allocation");t.number(static_cast<U>(info.RegionSize));t.add(p,info.RegionSize);}
void Capture(unsigned char* original,const std::string& directory,const std::string& path){
 auto a=Original(original);void* device=nullptr;Check(a.create(0,1,nullptr,0,nullptr,&device)==0&&device,"Original fixture device");
 using Loader=std::int32_t (__stdcall *)(void*,const char*,void**);auto load=reinterpret_cast<Loader>(original+0x13c82);
 std::vector<std::string> files;for(auto pattern:{"*.dds","*.tga"}){WIN32_FIND_DATAA data{};auto h=FindFirstFileA((directory+"/"+pattern).c_str(),&data);if(h!=INVALID_HANDLE_VALUE){do{if(!(data.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY))files.push_back(data.cFileName);}while(FindNextFileA(h,&data));FindClose(h);}}
 std::sort(files.begin(),files.end());Check(files.size()==12,"All 12 real representatives required");Trace t;t.add("TXFIX001",8);t.number(static_cast<U>(files.size()));
 for(const auto& name:files){void* texture=nullptr;auto result=load(device,(directory+"/"+name).c_str(),&texture);t.string(name);t.number(static_cast<U>(result));
  if(result<0){Check(name=="LURomania_vol.dds"&&!texture,"Unexpected real 2D fixture load failure");continue;}
  Check(texture&&(Load(texture,0)&15)==3&&Load(texture,4)==1,"Original texture resource state");
  std::array<unsigned char,52> header;std::memcpy(header.data(),texture,52);U primary=Load(texture,0x20)&~0xfffu,secondary=Load(texture,0x30)&~0xfffu;
  Check(primary&&secondary!=primary,"Texture primary/mip allocation ownership");Store(header.data(),0x20,(Load(texture,0x20)&0xfff)|0x10000000u);Store(header.data(),0x30,(Load(texture,0x30)&0xfff)|(secondary?0x20000000u:0));t.add(header.data(),header.size());CaptureRegion(t,primary);CaptureRegion(t,secondary);Check(a.release(texture)==0,"Original captured texture release");
 }
 Check(a.deviceRelease(device)==0,"Original fixture device release");Save(path,t);
}
struct Owned{
 void* object=nullptr;void* primary=nullptr;void* secondary=nullptr;U primarySize=0,secondarySize=0;
 void init(const std::vector<unsigned char>& h,const std::vector<unsigned char>& p,const std::vector<unsigned char>& s){object=GlobalAlloc(GMEM_ZEROINIT,52);Check(object&&h.size()==52,"Replay object storage");std::memcpy(object,h.data(),52);primarySize=static_cast<U>(p.size());secondarySize=static_cast<U>(s.size());primary=VirtualAlloc(nullptr,primarySize,MEM_COMMIT,PAGE_READWRITE);Check(primary!=nullptr,"Replay primary storage");std::memcpy(primary,p.data(),p.size());if(secondarySize){secondary=VirtualAlloc(nullptr,secondarySize,MEM_COMMIT,PAGE_READWRITE);Check(secondary!=nullptr,"Replay mip storage");std::memcpy(secondary,s.data(),s.size());}Store(object,0x20,(Load(object,0x20)&0xfff)|Address(primary));Store(object,0x30,(Load(object,0x30)&0xfff)|Address(secondary));}
 U normalize(const void* p)const{U n=Address(p);if(n>=Address(primary)&&n-Address(primary)<primarySize)return 0x10000000u+n-Address(primary);if(secondary&&n>=Address(secondary)&&n-Address(secondary)<secondarySize)return 0x20000000u+n-Address(secondary);throw std::runtime_error("Consumer layout/lock pointer outside owned allocations");}
 void state(Trace& t)const{std::array<unsigned char,52> h;std::memcpy(h.data(),object,52);Store(h.data(),0x20,(Load(object,0x20)&0xfff)|0x10000000u);Store(h.data(),0x30,(Load(object,0x30)&0xfff)|(secondary?0x20000000u:0));t.add(h.data(),h.size());}
};
Trace Run(Api a,bool isolated,const std::vector<unsigned char>& fixtures){Reader r{fixtures};Check(r.block(8)==std::vector<unsigned char>({'T','X','F','I','X','0','0','1'}),"Consumer fixture magic");const auto count=r.number();Check(count==12,"Complete consumer fixture count");Trace t;t.add("TXCNS001",8);t.number(count);U copyCalls=0,copyPixels=0;void* device=nullptr;if(isolated)Isolated();Check(a.create(0,1,nullptr,0,nullptr,&device)==0&&device,"Recovered consumer device");if(isolated)Isolated();
 for(U index=0;index<count;++index){const auto name=r.string();const U status=r.number();t.string(name);t.number(status);if(static_cast<std::int32_t>(status)<0)continue;
  auto header=r.block(52);auto p=r.block(r.number());auto s=r.block(r.number());Owned resource;resource.init(header,p,s);const auto levels=((Load(resource.object,0x2c)>>6)&15)+1;t.number(levels);
  if(isolated)Isolated();Check(a.add(resource.object)==2&&a.release(resource.object)==1,"Actual texture reference ownership");if(isolated)Isolated();resource.state(t);
  for(U level=0;level<levels;++level){
   NativeDescriptorAbi::LevelDesc desc{};NativeDescriptorAbi::VolumeDesc volume{};NativeDescriptorAbi::TextureDesc texture{};
   if(isolated)Isolated();a.level(resource.object,level,&desc);if(isolated)Isolated();a.volume(resource.object,level,&volume);if(isolated)Isolated();a.texture(resource.object,level,&texture);if(isolated)Isolated();t.add(&desc,sizeof(desc));t.add(&volume,sizeof(volume));t.add(&texture,sizeof(texture));
   std::array<U,7> layout{};void* address=nullptr;if(isolated)Isolated();a.layout(resource.object,0,level,&layout[0],&layout[1],&layout[2],&layout[3],&layout[4],&layout[5],&layout[6],&address);if(isolated)Isolated();t.add(layout.data(),sizeof(layout));t.number(resource.normalize(address));
   for(U mode=0;mode<3;++mode){NativeSurfaceAbi::LockedRect locked{};NativeSurfaceAbi::Rect rect{desc.width>=8?4:0,desc.height>=8?4:0,static_cast<std::int32_t>(desc.width),static_cast<std::int32_t>(desc.height)};if(isolated)Isolated();Check(a.lock(resource.object,level,&locked,mode==1?&rect:nullptr,mode==2?1:mode==1?0x10:0)==0,"Actual texture LockRect result");if(isolated)Isolated();t.number(static_cast<U>(locked.pitch));t.number(resource.normalize(locked.bits));resource.state(t);
    if((desc.format&63)==6){
     const U width=desc.width-(mode==1?rect.left:0),height=desc.height-(mode==1?rect.top:0),pitch=width*4+7;
     std::vector<unsigned char> converted(pitch*height+64,0xa5);
     if(isolated)Isolated();auto result=a.copy(converted.data()+32,pitch,width,height,0x1a200006,nullptr,locked.bits,locked.pitch,desc.format,nullptr,0,0.5f);if(isolated)Isolated();Check(result==0,"Actual PC LockRect endian/swizzle copy");
     for(U i=0;i<32;++i)Check(converted[i]==0xa5&&converted[converted.size()-32+i]==0xa5,"Actual copy destination guards");
     for(U row=0;row<height;++row)for(U byte=width*4;byte<pitch;++byte)Check(converted[32+row*pitch+byte]==0xa5,"Actual copy destination padding");
     t.number(width);t.number(height);t.number(desc.format);t.number(converted.size());t.add(converted.data(),converted.size());++copyCalls;copyPixels+=width*height;
    }

    if(isolated)Isolated();Check(a.unlock(resource.object,level)==0,"Actual texture UnlockRect");if(isolated)Isolated();resource.state(t);
   }
  }
  Check(std::memcmp(resource.primary,p.data(),p.size())==0&&(!s.size()||std::memcmp(resource.secondary,s.data(),s.size())==0),"Lock/descriptor modified actual source allocation");t.add(resource.primary,p.size());if(s.size())t.add(resource.secondary,s.size());
  if(isolated)Isolated();Check(a.release(resource.object)==0,"Actual texture final resource release");if(isolated)Isolated();MEMORY_BASIC_INFORMATION info{};Check(VirtualQuery(resource.primary,&info,sizeof(info))==sizeof(info)&&info.State==MEM_FREE,"Actual texture primary release");if(resource.secondary)Check(VirtualQuery(resource.secondary,&info,sizeof(info))==sizeof(info)&&info.State==MEM_FREE,"Actual texture mip release");
 }
 Check(copyCalls==120,"All40 raw mip levels/three locked views must execute actual conversion");t.number(copyCalls);t.number(copyPixels);Check(r.offset==fixtures.size(),"Trailing consumer fixture data");if(isolated)Isolated();Check(a.deviceRelease(device)==0,"Consumer final device release");if(isolated)Isolated();return t;
}
void Compare(unsigned char* original,const std::string& libs,const std::string& images,const std::string& prefix){if(original)Capture(original,images,prefix+".fixtures");auto fixtures=Read(prefix+".fixtures");std::vector<HMODULE> modules;auto actual=Run(Recovered(libs,modules),!original,fixtures);
 if(original){auto expected=Run(Original(original),false,fixtures);Save(prefix+".capture",expected);Check(actual.bytes==expected.bytes,"Complete actual PC texture native consumer trace differs");}
 else Check(actual.bytes==Read(prefix+".capture"),"Complete isolated actual PC consumer replay differs");
 for(auto m:modules)FreeLibrary(m);std::cout<<"PASS native PC consumer "<<(original?"differential":"NoOriginal")<<" realSourceFixtures=12 traceBytes="<<actual.bytes.size()<<"; all stored native allocations/levels/headers, three locks per level, descriptors/refcount/final release; five native libraries; actual raw endian/swizzle copies=120; full conversion output/padding/guards, capture loader remains oracle-only; no recovered image-loader/production-package claim\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 try{
  if(IntPtr::Size!=4||args->Length!=4)throw gcnew Exception("Usage: x86 original.dll|isolated native-dir image-dir capture-prefix");unsigned char* original=nullptr;
  if(args[0]!="isolated"){
   auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original consumer DLL fingerprint");auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto h=Marshal::GetHINSTANCE(assembly->ManifestModule);if(h==IntPtr::Zero||h==IntPtr(-1))throw gcnew Exception("Original module handle");original=static_cast<unsigned char*>(h.ToPointer());
  }
  auto libs=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[1]));auto images=Marshal::StringToHGlobalAnsi(args[2]);auto prefix=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[3]));Compare(original,static_cast<const char*>(libs.ToPointer()),static_cast<const char*>(images.ToPointer()),static_cast<const char*>(prefix.ToPointer()));Marshal::FreeHGlobal(libs);Marshal::FreeHGlobal(images);Marshal::FreeHGlobal(prefix);return 0;
 }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}return 1;
}
