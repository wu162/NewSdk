#include "../../src/TextureCompiler/Native/TextureNativeCopyApi.h"
#include <Windows.h>
#include <array>
#include <cmath>
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
Api Bind(unsigned char* original,HMODULE dll){Api a{original?reinterpret_cast<CopyFn>(original+0x3ccc0):reinterpret_cast<CopyFn>(GetProcAddress(dll,"TextureCopySurface"))};Check(a.copy!=nullptr,"Missing native conversion export");return a;}
struct Trace{std::vector<unsigned char> bytes;void number(U n){for(int i=0;i<4;++i)bytes.push_back(static_cast<unsigned char>(n>>(8*i)));}void add(const void* p,std::size_t n){auto b=static_cast<const unsigned char*>(p);bytes.insert(bytes.end(),b,b+n);}};
U Address(void* p){return static_cast<U>(reinterpret_cast<std::uintptr_t>(p));}
constexpr U Formats[]={2,3,4,5,6,7,8,9,10,14,15,16,17,24,25,26,41,42,43,45,46,47,48,50,54,55,56,62,27,28,29,30,31,32,36,37,38,57};
U Bytes(U f){switch(f){case 2:case 8:case 9:case 43:return 1;case 3:case 4:case 5:case 10:case 15:case 24:case 27:case 30:case 41:case 45:case 46:case 47:return 2;case 26:case 29:case 32:case 37:return 8;case 38:return 16;case 57:return 12;default:return 4;}}
float HalfValue(U h){return (h&0x7c00)?std::ldexp(1.0f+(h&1023)/1024.0f,static_cast<int>((h>>10)&31)-15):std::ldexp(static_cast<float>(h&1023),-24);}
U Swizzle(U a,U b,U c,U d){return (a<<18)|(b<<21)|(c<<24)|(d<<27);}
void Case(Trace& t,Api a,bool isolated,U ordinal,U sf,U df,U width,U height,U padding,U orientation,U region,float threshold,U exhaustive){
 const U sourceStride=width*Bytes(sf&63)+padding,destinationStride=width*Bytes(df&63)+padding;
 std::vector<unsigned char> source(sourceStride*height+64),destination(destinationStride*height+64);
 for(U i=0;i<source.size();++i)source[i]=static_cast<unsigned char>((i*13+ordinal*17)^(i>>3));
 for(U i=0;i<destination.size();++i)destination[i]=static_cast<unsigned char>(i*31+ordinal*19);
 if(exhaustive==1)for(U i=0;i<65536;++i)for(U c=0;c<Bytes(sf&63)/2;++c){U value=(i+c*7919)&65535;source[32+i*Bytes(sf&63)+c*2]=static_cast<unsigned char>(value);source[33+i*Bytes(sf&63)+c*2]=static_cast<unsigned char>(value>>8);}
 if(exhaustive>=2)for(U i=0;i<65536;++i)for(U c=0;c<Bytes(sf&63)/4;++c){
  U bits;
  if(exhaustive==2){U pattern=(i+c*7919)&65535;bits=((pattern&0x8000)<<16)|(((pattern>>7)&255)<<23)|((pattern&127)*0x10204);}
  else{const U half=i&0x7fff;const float value=HalfValue(half),next=half==0x7fff?131072.0f:HalfValue(half+1);float midpoint=static_cast<float>((static_cast<double>(value)+next)*0.5);std::memcpy(&bits,&midpoint,4);bits+=exhaustive-4;bits|=(i&0x8000)<<16;}
  std::memcpy(source.data()+32+i*Bytes(sf&63)+c*4,&bits,4);
 }
 const auto oldSource=source,oldDestination=destination;
 auto src=source.data()+32+((orientation&1)?sourceStride*(height-1):0);auto dst=destination.data()+32+((orientation&2)?destinationStride*(height-1):0);
 const auto sp=static_cast<std::int32_t>(sourceStride)*((orientation&1)?-1:1),dp=static_cast<std::int32_t>(destinationStride)*((orientation&2)?-1:1);
 Point point{0,0};Rect rect{0,0,static_cast<std::int32_t>(width),static_cast<std::int32_t>(height)};
 if(region==1){rect.left=4;rect.top=1;}
 if(region==2){point={width>1?1:0,height>1?1:0};rect.right=width>1?width-1:0;rect.bottom=height>1?height-1:0;}
 if(region==3){rect.left=width>2?2:0;point={rect.left,0};}
 const auto oldPoint=point;const auto oldRect=rect;
 if(isolated)Isolated();const auto result=a.copy(dst,dp,width,height,df,region?&point:nullptr,src,sp,sf,region?&rect:nullptr,0,threshold);if(isolated)Isolated();
 const auto expectedResult=sf!=df&&orientation?static_cast<std::int32_t>(0x8007000eu):0;
 if(result!=expectedResult){std::cerr<<"conversion HRESULT ordinal="<<ordinal<<" sf="<<std::hex<<sf<<" df="<<df<<" hr="<<result<<std::dec<<"\n";Check(false,"Conversion success domain rejected");}
 Check(source==oldSource&&std::memcmp(&point,&oldPoint,sizeof(point))==0&&std::memcmp(&rect,&oldRect,sizeof(rect))==0,"Conversion changed borrowed input");
 for(U row=0;row<height;++row)for(U byte=0;byte<destinationStride;++byte){
  const U logicalRow=(orientation&2)?height-1-row:row;
  const bool written=result==0&&logicalRow>=static_cast<U>(point.y)&&logicalRow<static_cast<U>(point.y+rect.bottom-rect.top)&&byte>=static_cast<U>(point.x)*Bytes(df&63)&&byte<static_cast<U>(point.x+rect.right-rect.left)*Bytes(df&63);
  if(!written)Check(destination[32+row*destinationStride+byte]==oldDestination[32+row*destinationStride+byte],"Conversion touched padding/outside rectangle");
 }
 Check(std::memcmp(destination.data(),oldDestination.data(),32)==0&&std::memcmp(destination.data()+destination.size()-32,oldDestination.data()+oldDestination.size()-32,32)==0,"Conversion destination guard");
 t.number(ordinal);t.number(sf);t.number(df);t.number(width);t.number(height);t.number(padding);t.number(orientation);t.number(region);U bits;std::memcpy(&bits,&threshold,4);t.number(bits);t.number(result);t.number(source.size());t.number(destination.size());t.add(source.data(),source.size());t.add(destination.data(),destination.size());
}
U Attributes(U format,U state){
 U result=(state==1||state==3||state==5||state==7)?0x20000:0;
 const U uniform=state==2||state==3?1:state==4||state==5?2:0;
 for(U c=0;c<4;++c){U type=uniform;if(state==6)type=c%3;if(state==7)type=(c+2)%3;
  // Format3 alpha has one bit; signed/biased normalization is undefined there.
  if(format==3&&c==3)type=0;
  result|=type<<(9+2*c);
 }
 return result;
}
Trace Run(Api a,bool isolated){Trace t;t.add("TXCEN001",8);U ordinal=0;
 const U maps[]={Swizzle(0,1,2,3),Swizzle(2,1,0,5)};
 for(U sf:Formats)for(U df:Formats)for(U sourceEndian=0;sourceEndian<4;++sourceEndian)for(U destinationEndian=0;destinationEndian<4;++destinationEndian)for(U state=0;state<2;++state)for(U mapping=0;mapping<2;++mapping)for(U region=0;region<2;++region){
  Case(t,a,isolated,ordinal++,sf|(sourceEndian<<6)|Attributes(sf,state?6:0)|maps[mapping],df|(destinationEndian<<6)|Attributes(df,state?7:0)|maps[1-mapping],8,3,mapping?7:0,0,region,0.5f,0);
 }
 Check(ordinal==184832,"Complete 38-by-38 endian/numeric/region matrix");
 for(U sf:{24u,27u})for(U sourceEndian=0;sourceEndian<4;++sourceEndian)for(U destinationEndian=0;destinationEndian<4;++destinationEndian)
  Case(t,a,isolated,ordinal++,sf|(sourceEndian<<6)|maps[0],6|(destinationEndian<<6)|maps[1],256,256,0,0,0,0.5f,1);
 Check(ordinal==184864,"Complete endian16 bit-pattern strips");
 for(U sourceEndian=0;sourceEndian<4;++sourceEndian)for(U destinationEndian=0;destinationEndian<4;++destinationEndian)for(U orientation=0;orientation<4;++orientation)
  Case(t,a,isolated,ordinal++,6|(sourceEndian<<6)|maps[0],50|(destinationEndian<<6)|maps[1],4,2,7,orientation,0,0.5f,0);
 Check(ordinal==184928,"Complete endian negative-stride failure/positive control matrix");return t;
}

void Compare(unsigned char* original,HMODULE dll,const char* path){auto expected=original?Run(Bind(original,nullptr),false):Trace{};auto actual=Run(Bind(nullptr,dll),!original);
 if(original){std::ofstream out(path,std::ios::binary);out.write(reinterpret_cast<const char*>(expected.bytes.data()),expected.bytes.size());Check(out.good(),"Archive copy original capture");if(actual.bytes!=expected.bytes){std::ofstream failed(std::string(path)+".actual",std::ios::binary);failed.write(reinterpret_cast<const char*>(actual.bytes.data()),actual.bytes.size());std::size_t offset=0;while(offset<actual.bytes.size()&&offset<expected.bytes.size()&&actual.bytes[offset]==expected.bytes[offset])++offset;std::cerr<<"first difference offset="<<offset<<" expected="<<(offset<expected.bytes.size()?static_cast<int>(expected.bytes[offset]):-1)<<" actual="<<(offset<actual.bytes.size()?static_cast<int>(actual.bytes[offset]):-1)<<"\n";Check(false,"Complete conversion source/output trace differs");}}
 else{std::ifstream in(path,std::ios::binary);Check(in.is_open(),"Missing conversion capture");std::vector<unsigned char> capture((std::istreambuf_iterator<char>(in)),std::istreambuf_iterator<char>());Check(!in.bad()&&actual.bytes==capture,"Complete isolated conversion replay differs");}
 auto api=Bind(nullptr,dll);unsigned char source[64]={1,2,3,4},destination[64]={9,8,7,6};auto before=std::array<unsigned char,64>{};std::memcpy(before.data(),destination,64);
 const U unsupported[]={0u,1u,13u,18u,63u,33u,6u|0x100u,6u|0x600u,3u|0x8000u,6u|Swizzle(6,1,2,3)};
 for(U sf:unsupported){if(!original)Isolated();Check(api.copy(destination,8,1,1,26|Swizzle(0,1,2,3),nullptr,source,8,sf,nullptr,0,1)==static_cast<std::int32_t>(0x80004001u)&&std::memcmp(before.data(),destination,64)==0,"Unsupported conversion must preserve output");if(!original)Isolated();}
 // Candidate rejection boundaries are not invoked on the undefined original domain.
 for(U bits:{0x7fc00000u,0x7f800000u,0xff800000u,0xbf800000u,0x3f800001u}){
  float threshold;std::memcpy(&threshold,&bits,4);if(!original)Isolated();Check(api.copy(destination,8,1,1,0x1a20001a,nullptr,source,8,0x1a200006,nullptr,0,threshold)==static_cast<std::int32_t>(0x80004001u)&&std::memcmp(before.data(),destination,64)==0,"Invalid conversion threshold changed output");if(!original)Isolated();
 }
 for(Rect rect: {Rect{-1,0,1,1},Rect{0,-1,1,1},Rect{0,0,2,1},Rect{1,0,0,1}}){
  if(!original)Isolated();Check(api.copy(destination,8,1,1,0x1a20001a,nullptr,source,8,0x1a200006,&rect,0,0.5f)==static_cast<std::int32_t>(0x80004001u)&&std::memcmp(before.data(),destination,64)==0,"Invalid conversion rectangle changed output");if(!original)Isolated();
 }
 for(U sf:{0x1a200082u,0x1a200006u}){U df=sf==0x1a200006u?0x1a200082u:0x1a200006u;
  if(!original)Isolated();Check(api.copy(destination,8,1,1,df,nullptr,source,8,sf,nullptr,0,0.5f)==static_cast<std::int32_t>(0x80004001u)&&std::memcmp(before.data(),destination,64)==0,"Incomplete endian unit must be explicitly rejected");if(!original)Isolated();
 }
 std::cout<<"PASS native PC endian conversion "<<(original?"differential":"NoOriginal")<<" cases=184928 formatPairs=1444 endianPairs=16 numericStates=2 mappings=2 regions=2 exhaustiveStrips=32 exhaustivePixels=2097152 traceBytes="<<actual.bytes.size()<<"; full source/output/padding/guards, thresholds and isolated replay\n";
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
