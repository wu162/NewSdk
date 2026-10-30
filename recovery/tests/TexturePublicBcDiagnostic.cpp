// Reuse the already audited native ownership/descriptor/VM consumer, while
// retaining this new public-codec comparison as diagnostic-only evidence.
#define main ExistingNativeLoaderOracleMain
#include "TextureNativeLoaderOracle.cpp"
#undef main
#include <iomanip>
#pragma managed(push,off)
namespace {
using DecodeBlock=std::int32_t (__stdcall *)(const void*,U,float*);
using EncodeBlock=std::int32_t (__stdcall *)(void*,U,const float*,U,float);
using DecodeImage=std::int32_t (__stdcall *)(const void*,std::int32_t,U,U,U,float*,std::int32_t);
using EncodeImage=std::int32_t (__stdcall *)(void*,std::int32_t,U,U,U,const float*,std::int32_t,U,float,U);
using ResizeDds=std::int32_t (__stdcall *)(const void*,std::int32_t,U,U,U,float*,std::int32_t,U,U);
using SurfaceView=void* (__stdcall *)(void*,U);
// RVA1210e: dst, dstPalette, dstRect, srcMemory, srcFormat,
// srcPitch, srcPalette, srcRect, srcTiled, srcWidth, srcHeight, filter, key.
// Unlike XGCopySurface, the source pointer precedes its format here.
using SurfaceMemory=std::int32_t (__stdcall *)(void*,const void*,const void*,const void*,U,U,const void*,const NativeCopyAbi::Rect*,U,U,U,U,U);
using DdsDecode=std::int32_t (__stdcall *)(float*,const void*);
using DdsEncode=std::int32_t (__stdcall *)(void*,const float*,U);
using CreateDds=std::int32_t (__stdcall *)(const void*,U,U,float,U,void**);
Trace LogicalPixels(Api api,void* resource){
 Trace t;const U levels=((Load(resource,0x2c)>>6)&15)+1;
 for(U level=0;level<levels;++level){
  ::Recovered::TextureCompiler::NativeDescriptorAbi::LevelDesc desc{};api.level(resource,level,&desc);
  ::Recovered::TextureCompiler::NativeSurfaceAbi::LockedRect locked{};Check(api.lock(resource,level,&locked,nullptr,1)==0,"BC actual-consumer lock");
  for(U mask:{~0x1c0u,0xffffffffu}){
   Isolation(api);const U pitch=desc.width*4+7;std::vector<unsigned char> rgba(std::size_t(pitch)*desc.height+64,0xa5);
   Check(api.copy(rgba.data()+32,pitch,desc.width,desc.height,0x1a200006,nullptr,locked.bits,locked.pitch,desc.format&mask,nullptr,0x12,0)==0,"BC actual masked/physical LoadImageA copy");Isolation(api);
   for(U i=0;i<32;++i)Check(rgba[i]==0xa5&&rgba[rgba.size()-1-i]==0xa5,"BC actual-copy guard");
   for(U y=0;y<desc.height;++y)for(U x=desc.width*4;x<pitch;++x)Check(rgba[32+std::size_t(y)*pitch+x]==0xa5,"BC actual-copy row padding");
   t.number(static_cast<U>(rgba.size()));t.add(rgba.data(),rgba.size());
  }
  Check(api.unlock(resource,level)==0,"BC actual-consumer unlock");Isolation(api);
 }
 return t;
}
void Diagnostic(unsigned char* original,const std::string& libs,const std::string& prefix){
 failurePath=prefix;std::vector<HMODULE> modules;const auto baseline=Original(original);auto recovered=Recovered(libs,modules,false);
 auto codec=LoadLibraryA((libs+"/Recovered.TexturePublicBc.dll").c_str());Check(codec!=nullptr,"Public BC candidate load");
 auto decode=reinterpret_cast<DecodeBlock>(GetProcAddress(codec,"TexturePublicBcDecodeBlock"));
 auto encode=reinterpret_cast<EncodeBlock>(GetProcAddress(codec,"TexturePublicBcEncodeBlock"));
 auto loadDds=reinterpret_cast<NativeLoaderAbi::LoadFn>(GetProcAddress(codec,"TexturePublicBcCreateTextureFromFileA"));
 auto create=reinterpret_cast<CreateDds>(GetProcAddress(codec,"TexturePublicBcCreateFromDds"));Check(decode&&encode&&create&&loadDds,"Public BC ABI exports");
 recovered.copy=reinterpret_cast<::Recovered::TextureCompiler::NativeCopyAbi::CopyFn>(GetProcAddress(codec,"TexturePublicBcCopySurface"));Check(recovered.copy!=nullptr,"Public compressed actual-consumer copy");
 auto resizeDds=reinterpret_cast<ResizeDds>(GetProcAddress(codec,"TexturePublicBcDecodeResizeDdsImage"));
 auto surfaceView=reinterpret_cast<SurfaceView>(original+0xdad0);
 auto surfaceMemory=reinterpret_cast<SurfaceMemory>(original+0x1210e);
 auto decodeDds=reinterpret_cast<DecodeImage>(GetProcAddress(codec,"TexturePublicBcDecodeDdsImage"));
 auto encodeDds=reinterpret_cast<EncodeImage>(GetProcAddress(codec,"TexturePublicBcEncodeDdsImage"));Check(decodeDds&&encodeDds,"D3DX file codec interfaces");
 std::ofstream capture(prefix+".capture",std::ios::binary);Check(capture.is_open(),"Diagnostic capture open");
 auto record=[&](const Trace& t){Trace head;head.name(current);head.number(static_cast<U>(t.data.size()));capture.write(reinterpret_cast<const char*>(head.data.data()),head.data.size());capture.write(reinterpret_cast<const char*>(t.data.data()),t.data.size());Check(capture.good(),"Diagnostic capture write");};
 U decodeMismatch=0,blockCases=0,ddsCases=0,unstable=0;std::array<U,8> encodeMismatch{};std::array<U,16> ddsMismatch{};
 U random=0x72a54ec1;auto next=[&](){random=random*1664525u+1013904223u;return random;};
 for(U format:{18u,19u,20u})for(U sample=0;sample<512;++sample){
  current="bc-block-f"+std::to_string(format)+"-"+std::to_string(sample);const U size=format==18?8:16;
  std::array<unsigned char,16> block;for(auto& byte:block)byte=static_cast<unsigned char>(sample==0?0:sample==1?255:next()>>24);
  std::array<float,64> a{},b{},actual{},pixels{};for(U i=0;i<64;++i)pixels[i]=sample<8?static_cast<float>((sample*37)&255)/255.f:static_cast<float>(next()&255)/255.f;
  Check(baseline.copy(a.data(),64,4,4,0x1a22aa26,nullptr,block.data(),size,0x1a200000|format,nullptr,0x12,0)==0,"Original compressed-to-float");
  Check(baseline.copy(b.data(),64,4,4,0x1a22aa26,nullptr,block.data(),size,0x1a200000|format,nullptr,0x12,0)==0,"Original repeat compressed-to-float");
  Check(decode(block.data(),format,actual.data())==0,"Candidate decode");Trace t;t.number(format);t.add(block.data(),size);t.add(a.data(),sizeof(a));t.add(b.data(),sizeof(b));t.add(actual.data(),sizeof(actual));
  if(a!=b)++unstable;if(std::memcmp(a.data(),actual.data(),sizeof(a)))++decodeMismatch;
  std::array<unsigned char,16> encoded{},repeat{};
  Check(baseline.copy(encoded.data(),size,4,4,0x1a200000|format,nullptr,pixels.data(),64,0x1a22aa26,nullptr,0x12,0)==0,"Original float-to-compressed");
  Check(baseline.copy(repeat.data(),size,4,4,0x1a200000|format,nullptr,pixels.data(),64,0x1a22aa26,nullptr,0x12,0)==0,"Original repeat float-to-compressed");
  if(std::memcmp(encoded.data(),repeat.data(),size))++unstable;t.add(pixels.data(),sizeof(pixels));t.add(encoded.data(),size);t.add(repeat.data(),size);
  for(U policy=0;policy<8;++policy){std::array<unsigned char,16> output{};Check(encode(output.data(),format,pixels.data(),policy<<16,0)==0,"Candidate encode");if(std::memcmp(encoded.data(),output.data(),size))++encodeMismatch[policy];t.number(policy<<16);t.add(output.data(),size);}
  record(t);++blockCases;
 }
 // Compare the distinct D3DX file codec, including its premultiply wrapper,
 // against the actual original entry points. Keep all full floats/blocks and
 // every fixed policy, independent of the existing XG contract above.
 U ddsDecodeMismatch=0,ddsBlockCases=0;std::array<U,8> ddsEncodeMismatch{},ddsDitherMismatch{};
 for(U format:{18u,19u,20u})for(U sample=0;sample<512;++sample){
  current="bc-file-block-f"+std::to_string(format)+"-"+std::to_string(sample);const U size=format==18?8:16;
  std::array<unsigned char,16> block;for(auto& byte:block)byte=static_cast<unsigned char>(sample==0?0:sample==1?255:next()>>24);
  std::array<float,64> a{},b{},actual{},pixels{};for(U i=0;i<64;++i)pixels[i]=sample<8?static_cast<float>((sample*37)&255)/255.f:static_cast<float>(next()&255)/255.f;
  const U drva=format==18?0x29895:format==19?0x2a4f4:0x2a507,erva=format==18?0x29cee:format==19?0x2a51a:0x2a532;
  const auto originalDecode=reinterpret_cast<DdsDecode>(original+drva);const auto originalEncode=reinterpret_cast<DdsEncode>(original+erva);
  Check(originalDecode(a.data(),block.data())==0&&originalDecode(b.data(),block.data())==0,"Original D3DX file block decode");
  Check(decodeDds(block.data(),size,4,4,format,actual.data(),64)==0,"Candidate file decode");
  Trace t;t.number(format);t.add(block.data(),size);t.add(a.data(),sizeof(a));t.add(b.data(),sizeof(b));t.add(actual.data(),sizeof(actual));t.add(pixels.data(),sizeof(pixels));
  if(std::memcmp(a.data(),b.data(),sizeof(a)))++unstable;if(std::memcmp(a.data(),actual.data(),sizeof(a)))++ddsDecodeMismatch;
  std::array<unsigned char,16> encoded{},dither{},repeat{},repeatDither{};
  Check(originalEncode(encoded.data(),pixels.data(),0)==0&&originalEncode(repeat.data(),pixels.data(),0)==0,"Original D3DX file encode");
  Check(originalEncode(dither.data(),pixels.data(),1)==0&&originalEncode(repeatDither.data(),pixels.data(),1)==0,"Original D3DX file dither encode");
  if(std::memcmp(encoded.data(),repeat.data(),size)||std::memcmp(dither.data(),repeatDither.data(),size))++unstable;
  t.add(encoded.data(),size);t.add(repeat.data(),size);t.add(dither.data(),size);t.add(repeatDither.data(),size);
  for(U policy=0;policy<8;++policy){std::array<unsigned char,16> output{};Check(encodeDds(output.data(),size,4,4,format,pixels.data(),64,policy<<16,0.5f,0)==0,"Candidate file encode");if(std::memcmp(encoded.data(),output.data(),size))++ddsEncodeMismatch[policy];if(std::memcmp(dither.data(),output.data(),size))++ddsDitherMismatch[policy];t.number(policy<<16);t.add(output.data(),size);}
  record(t);++ddsBlockCases;
 }
 std::cout<<"PROBE completed file blocks="<<ddsBlockCases<<" decode="<<ddsDecodeMismatch<<" encode0="<<ddsEncodeMismatch[0]<<" dither30000="<<ddsDitherMismatch[3]<<std::endl;
 void* device=nullptr;Check(baseline.device(0,1,nullptr,0,nullptr,&device)==0&&device,"Diagnostic original device");
 for(auto shape:{std::array<U,2>{4,4},std::array<U,2>{4,8},std::array<U,2>{8,4},std::array<U,2>{8,8},std::array<U,2>{16,32},std::array<U,2>{32,16},std::array<U,2>{64,64}})for(U format:{1u,2u,3u})for(U pattern:{0u,1u,2u,31u}){
  U levels=1;for(U n=std::max(shape[0],shape[1]);n>1;n>>=1)++levels;
  for(U provided:{1u,levels}){
   current="bc-dds-"+std::to_string(shape[0])+"x"+std::to_string(shape[1])+"-f"+std::to_string(format)+"-p"+std::to_string(pattern)+"-levels"+std::to_string(provided);
   const auto bytes=Dds(shape[0],shape[1],format,provided,pattern);const auto path=prefix+"-input.dds";Save(path,bytes);void* p=nullptr;
   Check(baseline.load(device,path.c_str(),&p)==0&&p,"Original compressed DDS");auto expected=LogicalPixels(baseline,p);{const auto storage=Consume(baseline,p,false);expected.add(storage.data.data(),storage.data.size());}p=nullptr;
   Check(baseline.load(device,path.c_str(),&p)==0&&p,"Original repeat compressed DDS");auto repeated=LogicalPixels(baseline,p);{const auto storage=Consume(baseline,p,false);repeated.add(storage.data.data(),storage.data.size());}if(expected.data!=repeated.data)++unstable;
   Trace t;t.number(static_cast<U>(bytes.size()));t.add(bytes.data(),bytes.size());t.number(static_cast<U>(expected.data.size()));t.add(expected.data.data(),expected.data.size());t.add(repeated.data.data(),repeated.data.size());
   for(U edge=0;edge<2;++edge)for(U policy=0;policy<8;++policy){p=nullptr;const auto status=create(bytes.data(),static_cast<U>(bytes.size()),policy<<16,0.5f,edge,&p);Check(status==0&&p,"Candidate full DDS resource");auto output=LogicalPixels(recovered,p);{const auto storage=Consume(recovered,p,false);output.add(storage.data.data(),storage.data.size());}if(output.data!=expected.data)++ddsMismatch[edge*8+policy];t.number(edge);t.number(policy<<16);t.number(static_cast<U>(output.data.size()));t.add(output.data.data(),output.data.size());}
   Check(Read(path)==bytes,"DDS diagnostic source changed");record(t);++ddsCases;
  }
 }
 // Actual default file entry, all supplied-level counts, small bases and
 // NPOT. Full original repeats and every physical/logical mip/VM byte remain
 // captured; this remains diagnostic until the complete fixed policy agrees.
 void* recoveredDevice=nullptr;Check(recovered.device(0,1,nullptr,0,nullptr,&recoveredDevice)==0&&recoveredDevice,"Default DDS recovered device");
 U defaultCases=0,defaultMismatch=0,floatCases=0,floatMismatch=0;
 const std::array<U,2> shapes[]={{{1,1}},{{1,8}},{{8,1}},{{2,4}},{{4,2}},{{2,2}},{{3,1}},{{1,3}},{{3,2}},{{2,3}},{{3,3}},{{5,3}},{{3,5}},{{7,9}},{{9,7}},{{17,35}},{{35,17}},{{31,65}},{{65,31}},{{127,5}},{{5,127}},{{129,1}},{{1,129}},{{4,4}},{{4,8}},{{8,4}},{{8,8}},{{16,32}},{{32,16}},{{64,64}}};
 for(auto shape:shapes)for(U format:{1u,2u,3u})for(U pattern:{0u,1u,2u,31u}){
  U levels=1;for(U n=std::max(shape[0],shape[1]);n>1;n>>=1)++levels;
  for(U provided=1;provided<=levels;++provided){
   current="bc-default-dds-"+std::to_string(shape[0])+"x"+std::to_string(shape[1])+"-f"+std::to_string(format)+"-p"+std::to_string(pattern)+"-levels"+std::to_string(provided);
   const auto bytes=Dds(shape[0],shape[1],format,provided,pattern);const auto path=prefix+"-input.dds";Save(path,bytes);void* p=nullptr;
   if(provided==1){
    U tw=4,th=4;while(tw<shape[0])tw<<=1;while(th<shape[1])th<<=1;
    // FileEx's format negotiation silently substitutes a supported device
    // format. Bypass that negotiation with the already audited CPU resource
    // constructor, then call the SAME surface-memory entry used by file IO.
    const U nativeFormat=format==1?18:format==2?19:20,pitch=((shape[0]+3)/4)*(format==1?8:16);
    p=Construct(baseline,tw,th,1,0x1a22aa26);Check(p!=nullptr,"Original explicit float CPU resource");
    auto surface=surfaceView(p,0);Check(surface!=nullptr,"Original float surface view");NativeCopyAbi::Rect sourceRect{0,0,static_cast<std::int32_t>(shape[0]),static_cast<std::int32_t>(shape[1])};
    const auto floatStatus=surfaceMemory(surface,nullptr,nullptr,bytes.data()+128,0x1a200000|nativeFormat,pitch,nullptr,&sourceRect,0,shape[0],shape[1],0x80004,0);
    if(floatStatus!=0)std::cerr<<"Original surface-memory HRESULT="<<std::hex<<static_cast<U>(floatStatus)<<std::dec<<std::endl;
    Check(floatStatus==0,"Original compressed-to-float surface consumer");
    Check(baseline.release(surface)==0,"Original float surface view release");NativeDescriptorAbi::LevelDesc floatDesc{};baseline.level(p,0,&floatDesc);Check((floatDesc.format&63)==38&&floatDesc.width==tw&&floatDesc.height==th,"Original float format and dimensions must remain explicit");
    std::vector<float> expectedFloat(std::size_t(tw)*th*4),actualFloat(expectedFloat.size());
    NativeSurfaceAbi::LockedRect locked{};Check(baseline.lock(p,0,&locked,nullptr,1)==0,"Original float file lock");Check(locked.pitch>=static_cast<std::int32_t>(tw*16),"Original float pitch");
    for(U y=0;y<th;++y)std::memcpy(expectedFloat.data()+std::size_t(y)*tw*4,static_cast<const unsigned char*>(locked.bits)+std::size_t(y)*locked.pitch,tw*16);
    Check(baseline.unlock(p,0)==0,"Original float file unlock");auto storage=Owned(p);Release(baseline,p,storage);p=nullptr;
    Check(resizeDds&&resizeDds(bytes.data()+128,pitch,shape[0],shape[1],nativeFormat,actualFloat.data(),tw*16,tw,th)==0,"Candidate decoded/resized float consumer");
    if(std::memcmp(expectedFloat.data(),actualFloat.data(),expectedFloat.size()*4))++floatMismatch;
    auto label=current;current="bc-filter-float-"+label.substr(15);Trace f;f.number(static_cast<U>(bytes.size()));f.add(bytes.data(),bytes.size());f.number(tw);f.number(th);f.add(expectedFloat.data(),expectedFloat.size()*4);f.add(actualFloat.data(),actualFloat.size()*4);record(f);current=label;++floatCases;
   }
   Check(baseline.load(device,path.c_str(),&p)==0&&p,"Original default compressed DDS");auto expected=LogicalPixels(baseline,p);{const auto storage=Consume(baseline,p,false);expected.add(storage.data.data(),storage.data.size());}p=nullptr;
   Check(baseline.load(device,path.c_str(),&p)==0&&p,"Original repeat default compressed DDS");auto repeated=LogicalPixels(baseline,p);{const auto storage=Consume(baseline,p,false);repeated.add(storage.data.data(),storage.data.size());}if(expected.data!=repeated.data)++unstable;p=nullptr;
   Check(loadDds(recoveredDevice,path.c_str(),&p)==0&&p,"Candidate default DDS file entry");auto output=LogicalPixels(recovered,p);{const auto storage=Consume(recovered,p,false);output.add(storage.data.data(),storage.data.size());}if(output.data!=expected.data)++defaultMismatch;
   Trace t;t.number(static_cast<U>(bytes.size()));t.add(bytes.data(),bytes.size());t.number(static_cast<U>(expected.data.size()));t.add(expected.data.data(),expected.data.size());t.add(repeated.data.data(),repeated.data.size());t.number(static_cast<U>(output.data.size()));t.add(output.data.data(),output.data.size());record(t);++defaultCases;
   Check(Read(path)==bytes,"Default DDS source changed");
  }
 }
 Check(recovered.deviceRelease(recoveredDevice)==0,"Default DDS recovered device release");
 std::cout<<"PROBE float DDS filter cases="<<floatCases<<" mismatches="<<floatMismatch<<std::endl;
 std::cout<<"PROBE default DDS cases="<<defaultCases<<" mismatches="<<defaultMismatch<<"\n";
 Check(baseline.deviceRelease(device)==0,"Diagnostic original device release");Check(blockCases==1536&&ddsBlockCases==1536&&ddsCases==168&&defaultCases==1572&&floatCases==360,"Complete diagnostic corpus");capture.close();
 std::cout<<"PROBE D3DX file blockCases="<<ddsBlockCases<<" decodeBitMismatches="<<ddsDecodeMismatch<<"\n";
 for(U policy=0;policy<8;++policy)std::cout<<"FILE_POLICY flags="<<std::hex<<(policy<<16)<<std::dec<<" encodeMismatch="<<ddsEncodeMismatch[policy]<<" ditherMismatch="<<ddsDitherMismatch[policy]<<"\n";
 std::cout<<"PROBE public D3DX BC blockCases="<<blockCases<<" ddsCases="<<ddsCases<<" decodeBitMismatches="<<decodeMismatch<<" originalUnstable="<<unstable<<"; diagnostic only, no production acceptance or NoOriginal claim\n";
 for(U policy=0;policy<8;++policy)std::cout<<"POLICY flags="<<std::hex<<(policy<<16)<<std::dec<<" encodeMismatch="<<encodeMismatch[policy]<<" ddsWrapMismatch="<<ddsMismatch[policy]<<" ddsClampMismatch="<<ddsMismatch[8+policy]<<"\n";
 FreeLibrary(codec);for(auto m:modules)FreeLibrary(m);
}
#include "TexturePublicBcReplay.h"
}
#pragma managed(pop)
int main(array<String^>^ args){
 try{
  if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll native-dir prefix");
  if(args[0]=="--no-original"){
   auto directory=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[1]));auto path=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));
   ReplayPublicBc(static_cast<const char*>(directory.ToPointer()),static_cast<const char*>(path.ToPointer()));Marshal::FreeHGlobal(directory);Marshal::FreeHGlobal(path);return 0;
  }
  auto sha=SHA256::Create();auto digest=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
  if(digest!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original fingerprint");
  auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto h=Marshal::GetHINSTANCE(assembly->ManifestModule);if(h==IntPtr::Zero||h==IntPtr(-1))throw gcnew Exception("Original module handle");
  auto libs=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[1]));auto prefix=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Diagnostic(static_cast<unsigned char*>(h.ToPointer()),static_cast<const char*>(libs.ToPointer()),static_cast<const char*>(prefix.ToPointer()));Marshal::FreeHGlobal(libs);Marshal::FreeHGlobal(prefix);return 0;
 }catch(const std::exception& e){std::cerr<<current<<": "<<e.what()<<"\n";}catch(Exception^ e){Console::Error->WriteLine(e);}return 1;
}
