#include "../../src/TextureCompiler/Native/TextureNativeCopyApi.h"
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
using namespace Recovered::TextureCompiler::NativeCopyAbi;
using U=std::uint32_t;
struct Api{CopyFn copy;};
void Check(bool c,const char* why){if(!c)throw std::runtime_error(why);}
void Isolated(){Check(!GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"),"Original loaded during surface replay");Check(GetFileAttributesA("BinaryAssetBuilder.TextureCompiler.dll")==INVALID_FILE_ATTRIBUTES,"Original file during surface replay");}
Api Bind(unsigned char* original,HMODULE dll){Api a{original?reinterpret_cast<CopyFn>(original+0x3ccc0):reinterpret_cast<CopyFn>(GetProcAddress(dll,"TextureCopySurfaceIdentity"))};Check(a.copy!=nullptr,"Missing native identity-copy export");return a;}
struct Trace{std::vector<unsigned char> bytes;void number(U n){for(int i=0;i<4;++i)bytes.push_back(static_cast<unsigned char>(n>>(8*i)));}void add(const void* p,std::size_t n){auto b=static_cast<const unsigned char*>(p);bytes.insert(bytes.end(),b,b+n);}};
U Address(void* p){return static_cast<U>(reinterpret_cast<std::uintptr_t>(p));}
constexpr U Bits[]={1,1,8,16,16,16,32,32,8,8,16,16,16,32,32,16,32,32,4,8,8,64,32,32,16,32,64,16,32,64,16,32,64,32,64,128,32,64,128,8,16,16,32,8,8,16,16,16,32,8,32,4,8,8,32,32,32,96,4,4,4,4,32,32};
void Blocks(U f,U& x,U& y){if(f==11||f==12||f==40){x=2;y=1;}else if(f==39||f==44){x=4;y=1;}else if((f>=18&&f<=20)||f==49||(f>=51&&f<=53)||(f>=58&&f<=61)){x=y=4;}else{x=y=1;}}
Trace Run(Api a,bool isolated){Trace t;t.add("TXCPY001",8);U ordinal=0;
 constexpr U widths[]={1,4,7,17},heights[]={1,2,5,13},flagsList[]={0,3,12,0xffffffff},formatHigh[]={0,0x80000000,0x20000,0x1a220000},floatBits[]={0,0x3f800000,0x7fc00000,0xff800000};
 for(U format=0;format<64;++format)for(U size=0;size<4;++size)for(U padding:{0u,7u})for(U orientation=0;orientation<4;++orientation)for(U flag:flagsList)for(U region=0;region<4;++region){
  const U width=widths[size],height=heights[size];U bx,by;Blocks(format,bx,by);const U bytes=Bits[format]*bx*by/8;const U stride=((width+bx-1)/bx)*(bytes?bytes:1)+padding,rows=(height+by-1)/by;
  std::vector<unsigned char> source(stride*rows+64),destination(stride*rows+64);
  for(U i=0;i<source.size();++i){source[i]=static_cast<unsigned char>(i*13+ordinal*17);destination[i]=static_cast<unsigned char>(i*31+ordinal*19);}
  auto oldSource=source,oldDestination=destination;
  auto src=source.data()+32+((orientation&1)?stride*(rows-1):0);auto dst=destination.data()+32+((orientation&2)?stride*(rows-1):0);
  auto sourcePitch=static_cast<std::int32_t>(stride)*((orientation&1)?-1:1),destinationPitch=static_cast<std::int32_t>(stride)*((orientation&2)?-1:1);
  Point point{0,0};Rect rect{0,0,static_cast<std::int32_t>(width),static_cast<std::int32_t>(height)};
  if(region==1||region==3){rect.left=width>=2*bx?static_cast<std::int32_t>(bx):0;rect.top=height>=2*by?static_cast<std::int32_t>(by):0;if(region==3)point={rect.left,rect.top};}
  if(region==2){point={width>=2*bx?static_cast<std::int32_t>(bx):0,height>=2*by?static_cast<std::int32_t>(by):0};rect.right=width>=2*bx?static_cast<std::int32_t>((width-bx)/bx*bx):0;rect.bottom=height>=2*by?static_cast<std::int32_t>((height-by)/by*by):0;}
  auto oldPoint=point;auto oldRect=rect;float threshold;U bits=floatBits[(ordinal/4)%4];std::memcpy(&threshold,&bits,4);const U fullFormat=format|formatHigh[region];
  if(isolated)Isolated();auto result=a.copy(dst,destinationPitch,width,height,fullFormat,region?&point:nullptr,src,sourcePitch,fullFormat,region?&rect:nullptr,flag,threshold);if(isolated)Isolated();
  Check(result==(format<2?static_cast<std::int32_t>(0x80004001u):0),"Identity path HRESULT");Check(source==oldSource&&std::memcmp(&point,&oldPoint,sizeof(point))==0&&std::memcmp(&rect,&oldRect,sizeof(rect))==0,"Copy modified borrowed source/rect/point");
  Check(std::memcmp(destination.data(),oldDestination.data(),32)==0&&std::memcmp(destination.data()+destination.size()-32,oldDestination.data()+oldDestination.size()-32,32)==0,"Copy changed destination guards");if(format<2)Check(destination==oldDestination,"Unsupported one-bit format changed destination");
  t.number(ordinal++);t.number(static_cast<U>(result));t.number(static_cast<U>(source.size()));t.number(static_cast<U>(destination.size()));t.add(source.data(),source.size());t.add(destination.data(),destination.size());
 }
 Check(ordinal==32768,"Complete identity-copy matrix");return t;
}
void Compare(unsigned char* original,HMODULE dll,const char* path){auto expected=original?Run(Bind(original,nullptr),false):Trace{};auto actual=Run(Bind(nullptr,dll),!original);
 if(original){std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(expected.bytes.data()),expected.bytes.size());Check(out.good(),"Archive copy original capture");Check(actual.bytes==expected.bytes,"Complete native identity-copy source/destination/padding/guards trace differs");}
 else{std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing identity-copy capture");std::vector<unsigned char> capture((std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>());Check(!in.bad()&&actual.bytes==capture,"Complete isolated identity-copy replay differs");}
 auto a=Bind(nullptr,dll);unsigned char source[64]={1,2,3,4},destination[64]={9,8,7,6};auto before=std::array<unsigned char,64>{};std::memcpy(before.data(),destination,64);if(!original)Isolated();Check(a.copy(destination,4,1,1,6,nullptr,source,4,7,nullptr,0,1)==static_cast<std::int32_t>(0x80004001u)&&std::memcmp(before.data(),destination,64)==0,"Conversion path must be explicitly unsupported and preserve output");if(!original)Isolated();
 std::cout<<"PASS native PC identity copy "<<(original?"differential":"NoOriginal")<<" cases=32768 formats=64 sizes=4 padding=2 orientations=4 flagSets=4 regionSets=4 traceBytes="<<actual.bytes.size()<<"; full source/destination/padding/guards, negative strides and ignored float bits; conversion/filter/unaligned/overlap paths excluded\n";
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 HMODULE dll=nullptr;
 try{
  if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll|isolated recovered.dll capture");unsigned char* original=nullptr;
  if(args[0]!="isolated"){
   auto sha=SHA256::Create();auto hash=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
   if(hash!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original copy DLL fingerprint changed");
   auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto handle=Marshal::GetHINSTANCE(assembly->ManifestModule);if(handle==IntPtr::Zero||handle==IntPtr(-1))throw gcnew Exception("Original module handle");original=static_cast<unsigned char*>(handle.ToPointer());
   if(original[0x3ccc0]!=0x55||original[0x3cef0]!=0x33)throw gcnew Exception("Original copy entry guards");
  }
  auto name=Marshal::StringToHGlobalUni(Path::GetFullPath(args[1]));dll=LoadLibraryW(static_cast<const wchar_t*>(name.ToPointer()));Marshal::FreeHGlobal(name);Check(dll!=nullptr,"Recovered copy DLL load");
  auto path=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Compare(original,dll,static_cast<const char*>(path.ToPointer()));Marshal::FreeHGlobal(path);FreeLibrary(dll);return 0;
 }catch(Exception^ e){Console::Error->WriteLine(e);}catch(const std::exception& e){std::cerr<<e.what()<<"\n";}
 if(dll)FreeLibrary(dll);return 1;
}
