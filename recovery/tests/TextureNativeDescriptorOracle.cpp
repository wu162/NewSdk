#include "../../src/TextureCompiler/Native/TextureNativeDescriptorApi.h"
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
using namespace Recovered::TextureCompiler::NativeDescriptorAbi;
using U=std::uint32_t;
struct Api{LevelFn level;VolumeFn volume;TextureFn texture;};
void Check(bool c,const char* why){if(!c)throw std::runtime_error(why);}
void Isolated(){Check(!GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"),"Original loaded during surface replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file during surface replay");}
Api Bind(unsigned char* original,HMODULE dll){Api a=original?Api{reinterpret_cast<LevelFn>(original+0xdb60),reinterpret_cast<VolumeFn>(original+0xdbf9),reinterpret_cast<TextureFn>(original+0x3df34)}:Api{reinterpret_cast<LevelFn>(GetProcAddress(dll,"TextureGetLevelDesc")),reinterpret_cast<VolumeFn>(GetProcAddress(dll,"TextureGetVolumeLevelDesc")),reinterpret_cast<TextureFn>(GetProcAddress(dll,"TextureXGGetTextureDesc"))};Check(a.level&&a.volume&&a.texture,"Missing native descriptor exports");return a;}
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
template<class T> struct Guarded{U before;T value;U after;Guarded(){std::memset(this,0xa5,sizeof(*this));}void check(){Check(before==0xa5a5a5a5u&&after==0xa5a5a5a5u,"Descriptor output overran ABI size");}};
void Case(Api a,bool isolated,Trace& t,const std::array<U,16>& header,U level,U ordinal){
 auto p=header;Guarded<LevelDesc> surface;Guarded<VolumeDesc> volume;Guarded<TextureDesc> texture;
 if(isolated)Isolated();a.level(p.data(),level,&surface.value);if(isolated)Isolated();Check(p==header,"Level descriptor changed input");surface.check();
 if(isolated)Isolated();a.volume(p.data(),level,&volume.value);if(isolated)Isolated();Check(p==header,"Volume descriptor changed input");volume.check();
 if(isolated)Isolated();a.texture(p.data(),level,&texture.value);if(isolated)Isolated();Check(p==header,"XG descriptor changed input");texture.check();
 t.number(ordinal);t.add(&surface.value,sizeof(LevelDesc));t.add(&volume.value,sizeof(VolumeDesc));t.add(&texture.value,sizeof(TextureDesc));
}
Trace Run(Api a,bool isolated){Trace t;t.add("TXDSC001",8);constexpr U levels[]={0,1,2,3,4,5,7,8};U ordinal=0;
 for(U dimension=0;dimension<4;++dimension)for(U format=0;format<64;++format)for(U size=0;size<8;++size)for(U flags=0;flags<8;++flags)for(U level:levels)Case(a,isolated,t,Header(dimension,format,size,flags),level,ordinal++);
 Check(ordinal==131072,"Descriptor matrix accounting");
 U state=0x4871bcee;
 for(U i=0;i<4096;++i){auto p=Header(i%4,(i/4)%64,(i/256)%8,i%8);for(U offset:{7u,8u,10u,11u,12u}){state=state*1664525u+1013904223u;p[offset]=state;}Case(a,isolated,t,p,i%9,ordinal++);}
 Check(ordinal==135168,"Full descriptor metadata matrix accounting");return t;
}
void Compare(unsigned char* original,HMODULE dll,const char* path){auto expected=original?Run(Bind(original,nullptr),false):Trace{};auto actual=Run(Bind(nullptr,dll),!original);
 if(original){std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(expected.bytes.data()),expected.bytes.size());Check(out.good(),"Archive descriptor original capture");if(actual.bytes!=expected.bytes){std::size_t i=0;while(i<actual.bytes.size()&&i<expected.bytes.size()&&actual.bytes[i]==expected.bytes[i])++i;std::cerr<<"Descriptor first difference byte="<<i<<" case="<<(i>=8?(i-8)/124:0)<<" expected="<<(i<expected.bytes.size()?static_cast<int>(expected.bytes[i]):-1)<<" actual="<<(i<actual.bytes.size()?static_cast<int>(actual.bytes[i]):-1)<<"\n";}Check(actual.bytes==expected.bytes,"Complete native level/volume/XG descriptor bytes differ");}
 else{std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing descriptor capture");std::vector<unsigned char> capture((std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>());Check(!in.bad()&&actual.bytes==capture,"Complete isolated descriptor replay differs");}
 std::cout<<"PASS native PC descriptor "<<(original?"differential":"NoOriginal")<<" headers=135168 outputRecords=405504 formats=64 dimensions=4 metadataCases=4096 traceBytes="<<actual.bytes.size()<<"; full level/volume/XG structure bytes/canaries/input preservation; nontexture resource descriptors excluded\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 HMODULE dll=nullptr;
 try{
  if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll|isolated recovered.dll capture");unsigned char* original=nullptr;
  if(args[0]!="isolated"){
   auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
   if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original descriptor DLL fingerprint changed");
   auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto handle=Marshal::GetHINSTANCE(assembly->ManifestModule);if(handle==IntPtr::Zero||handle==IntPtr(-1))throw gcnew Exception("Original module handle");original=static_cast<unsigned char*>(handle.ToPointer());
   if(original[0xdb60]!=0xe9||original[0xdbf9]!=0xe9||original[0x3df34]!=0x8b)throw gcnew Exception("Original descriptor entry guards");
  }
  auto name=Marshal::StringToHGlobalUni(Path::GetFullPath(args[1]));dll=LoadLibraryW(static_cast<const wchar_t*>(name.ToPointer()));Marshal::FreeHGlobal(name);Check(dll!=nullptr,"Recovered descriptor DLL load");
  auto path=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Compare(original,dll,static_cast<const char*>(path.ToPointer()));Marshal::FreeHGlobal(path);FreeLibrary(dll);return 0;
 }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}
 if(dll)FreeLibrary(dll);return 1;
}
