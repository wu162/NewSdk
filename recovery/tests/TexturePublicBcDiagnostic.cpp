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
using CreateDds=std::int32_t (__stdcall *)(const void*,U,U,float,U,void**);
Trace LogicalPixels(Api api,void* resource){
 Trace t;const U levels=((Load(resource,0x2c)>>6)&15)+1;
 for(U level=0;level<levels;++level){
  ::Recovered::TextureCompiler::NativeDescriptorAbi::LevelDesc desc{};api.level(resource,level,&desc);
  ::Recovered::TextureCompiler::NativeSurfaceAbi::LockedRect locked{};Check(api.lock(resource,level,&locked,nullptr,1)==0,"BC actual-consumer lock");
  for(U mask:{~0x1c0u,0xffffffffu}){
   const U pitch=desc.width*4+7;std::vector<unsigned char> rgba(std::size_t(pitch)*desc.height+64,0xa5);
   Check(api.copy(rgba.data()+32,pitch,desc.width,desc.height,0x1a200006,nullptr,locked.bits,locked.pitch,desc.format&mask,nullptr,0x12,0)==0,"BC actual masked/physical LoadImageA copy");
   for(U i=0;i<32;++i)Check(rgba[i]==0xa5&&rgba[rgba.size()-1-i]==0xa5,"BC actual-copy guard");
   for(U y=0;y<desc.height;++y)for(U x=desc.width*4;x<pitch;++x)Check(rgba[32+std::size_t(y)*pitch+x]==0xa5,"BC actual-copy row padding");
   t.number(static_cast<U>(rgba.size()));t.add(rgba.data(),rgba.size());
  }
  Check(api.unlock(resource,level)==0,"BC actual-consumer unlock");
 }
 return t;
}
void Diagnostic(unsigned char* original,const std::string& libs,const std::string& prefix){
 failurePath=prefix;std::vector<HMODULE> modules;const auto baseline=Original(original);auto recovered=Recovered(libs,modules,false);
 auto codec=LoadLibraryA((libs+"/Recovered.TexturePublicBc.dll").c_str());Check(codec!=nullptr,"Public BC candidate load");
 auto decode=reinterpret_cast<DecodeBlock>(GetProcAddress(codec,"TexturePublicBcDecodeBlock"));
 auto encode=reinterpret_cast<EncodeBlock>(GetProcAddress(codec,"TexturePublicBcEncodeBlock"));
 auto create=reinterpret_cast<CreateDds>(GetProcAddress(codec,"TexturePublicBcCreateFromDds"));Check(decode&&encode&&create,"Public BC ABI exports");
 recovered.copy=reinterpret_cast<::Recovered::TextureCompiler::NativeCopyAbi::CopyFn>(GetProcAddress(codec,"TexturePublicBcCopySurface"));Check(recovered.copy!=nullptr,"Public compressed actual-consumer copy");
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
 void* device=nullptr;Check(baseline.device(0,1,nullptr,0,nullptr,&device)==0&&device,"Diagnostic original device");
 for(auto shape:{std::array<U,2>{4,4},std::array<U,2>{4,8},std::array<U,2>{8,4},std::array<U,2>{8,8},std::array<U,2>{16,32},std::array<U,2>{32,16},std::array<U,2>{64,64}})for(U format:{1u,2u,3u})for(U pattern:{0u,1u,2u,31u}){
  U levels=1;for(U n=std::max(shape[0],shape[1]);n>1;n>>=1)++levels;
  for(U provided:{1u,levels}){
   current="bc-dds-"+std::to_string(shape[0])+"x"+std::to_string(shape[1])+"-f"+std::to_string(format)+"-p"+std::to_string(pattern)+"-levels"+std::to_string(provided);
   const auto bytes=Dds(shape[0],shape[1],format,provided,pattern);const auto path=prefix+"-input.dds";Save(path,bytes);void* p=nullptr;
   Check(baseline.load(device,path.c_str(),&p)==0&&p,"Original compressed DDS");auto expected=LogicalPixels(baseline,p);{const auto storage=Consume(baseline,p,false);expected.add(storage.data.data(),storage.data.size());}p=nullptr;
   Check(baseline.load(device,path.c_str(),&p)==0&&p,"Original repeat compressed DDS");auto repeated=LogicalPixels(baseline,p);{const auto storage=Consume(baseline,p,false);repeated.add(storage.data.data(),storage.data.size());}if(expected.data!=repeated.data)++unstable;
   Trace t;t.number(static_cast<U>(bytes.size()));t.add(bytes.data(),bytes.size());t.number(static_cast<U>(expected.data.size()));t.add(expected.data.data(),expected.data.size());t.add(repeated.data.data(),repeated.data.size());
   for(U edge=0;edge<2;++edge)for(U policy=0;policy<8;++policy){p=nullptr;const auto status=create(bytes.data(),static_cast<U>(bytes.size()),policy<<16,0,edge,&p);Check(status==0&&p,"Candidate full DDS resource");auto output=LogicalPixels(recovered,p);{const auto storage=Consume(recovered,p,false);output.add(storage.data.data(),storage.data.size());}if(output.data!=expected.data)++ddsMismatch[edge*8+policy];t.number(edge);t.number(policy<<16);t.number(static_cast<U>(output.data.size()));t.add(output.data.data(),output.data.size());}
   Check(Read(path)==bytes,"DDS diagnostic source changed");record(t);++ddsCases;
  }
 }
 Check(baseline.deviceRelease(device)==0,"Diagnostic original device release");Check(blockCases==1536&&ddsCases==168,"Complete diagnostic corpus");capture.close();
 std::cout<<"PROBE public D3DX BC blockCases="<<blockCases<<" ddsCases="<<ddsCases<<" decodeBitMismatches="<<decodeMismatch<<" originalUnstable="<<unstable<<"; diagnostic only, no production acceptance or NoOriginal claim\n";
 for(U policy=0;policy<8;++policy)std::cout<<"POLICY flags="<<std::hex<<(policy<<16)<<std::dec<<" encodeMismatch="<<encodeMismatch[policy]<<" ddsWrapMismatch="<<ddsMismatch[policy]<<" ddsClampMismatch="<<ddsMismatch[8+policy]<<"\n";
 FreeLibrary(codec);for(auto m:modules)FreeLibrary(m);
}
}
#pragma managed(pop)
int main(array<String^>^ args){
 try{
  if(IntPtr::Size!=4||args->Length!=3)throw gcnew Exception("Usage: x86 original.dll native-dir prefix");
  auto sha=SHA256::Create();auto digest=BitConverter::ToString(sha->ComputeHash(File::ReadAllBytes(args[0])))->Replace("-","")->ToLowerInvariant();delete sha;
  if(digest!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw gcnew Exception("Original fingerprint");
  auto assembly=Assembly::LoadFrom(Path::GetFullPath(args[0]));auto h=Marshal::GetHINSTANCE(assembly->ManifestModule);if(h==IntPtr::Zero||h==IntPtr(-1))throw gcnew Exception("Original module handle");
  auto libs=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[1]));auto prefix=Marshal::StringToHGlobalAnsi(Path::GetFullPath(args[2]));Diagnostic(static_cast<unsigned char*>(h.ToPointer()),static_cast<const char*>(libs.ToPointer()),static_cast<const char*>(prefix.ToPointer()));Marshal::FreeHGlobal(libs);Marshal::FreeHGlobal(prefix);return 0;
 }catch(const std::exception& e){std::cerr<<current<<": "<<e.what()<<"\n";}catch(Exception^ e){Console::Error->WriteLine(e);}return 1;
}
