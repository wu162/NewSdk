// Included inside the diagnostic's unmanaged anonymous namespace. Reuse its
// complete resource/VM/ownership trace; do not load an original module here.
struct BcCursor {
 const std::vector<unsigned char>& bytes;std::size_t at=0;
 const unsigned char* take(std::size_t n){Check(n<=bytes.size()-at,"Truncated BC replay record");auto p=bytes.data()+at;at+=n;return p;}
 U number(){return Load(take(4),0);}
 void end(){Check(at==bytes.size(),"BC replay trailing fields");}
};
void ReplayPublicBc(const std::string& directory,const std::string& path){
 std::vector<HMODULE> modules;auto api=Recovered(directory,modules,true);Isolation(api);
 auto codec=LoadLibraryA((directory+"/Recovered.TexturePublicBc.dll").c_str());Check(codec!=nullptr,"Isolated public BC load");
 auto decode=reinterpret_cast<DecodeBlock>(GetProcAddress(codec,"TexturePublicBcDecodeBlock"));
 auto encode=reinterpret_cast<EncodeBlock>(GetProcAddress(codec,"TexturePublicBcEncodeBlock"));
 auto decodeDds=reinterpret_cast<DecodeImage>(GetProcAddress(codec,"TexturePublicBcDecodeDdsImage"));
 auto encodeDds=reinterpret_cast<EncodeImage>(GetProcAddress(codec,"TexturePublicBcEncodeDdsImage"));
 auto resize=reinterpret_cast<ResizeDds>(GetProcAddress(codec,"TexturePublicBcDecodeResizeDdsImage"));
 auto load=reinterpret_cast<NativeLoaderAbi::LoadFn>(GetProcAddress(codec,"TexturePublicBcCreateTextureFromFileA"));
 api.copy=reinterpret_cast<NativeCopyAbi::CopyFn>(GetProcAddress(codec,"TexturePublicBcCopySurface"));
 Check(decode&&encode&&decodeDds&&encodeDds&&load&&resize&&api.copy,"Isolated BC exports");
 void* device=nullptr;Check(api.device(0,1,nullptr,0,nullptr,&device)==0&&device,"Isolated BC device");Isolation(api);
 std::ifstream capture(path,std::ios::binary);Check(capture.is_open(),"Missing BC original capture");
 auto number=[&](){U n=0;capture.read(reinterpret_cast<char*>(&n),4);Check(capture.good(),"Truncated BC capture number");return n;};
 U blocks=0,fileBlocks=0,files=0,diagnosticPolicies=0,floatRows=0;
 const std::string inputPath=path+"-replay.dds";
 while(capture.peek()!=std::char_traits<char>::eof()){
  U n=number();Check(n<512,"BC replay name bound");current.assign(n,'\0');capture.read(&current[0],n);Check(capture.good(),"Truncated BC capture label");
  n=number();Check(n<0x10000000,"BC replay record bound");std::vector<unsigned char> payload(n);capture.read(reinterpret_cast<char*>(payload.data()),n);Check(capture.good(),"Truncated BC capture payload");BcCursor c{payload};
  if(current.rfind("bc-block-",0)==0||current.rfind("bc-file-block-",0)==0){
   const bool file=current.rfind("bc-file-block-",0)==0;const U format=c.number(),size=format==18?8:16;Check(format>=18&&format<=20,"BC replay format");
   auto block=c.take(size),expected=c.take(256),repeat=c.take(256),saved=c.take(256),pixels=c.take(256);
   Check(!std::memcmp(expected,repeat,256),"Original BC decode was unstable");
   std::array<float,64> actual{};Isolation(api);
   auto status=file?decodeDds(block,size,4,4,format,actual.data(),64):decode(block,format,actual.data());Isolation(api);
   Check(status==0&&!std::memcmp(actual.data(),expected,256)&&!std::memcmp(saved,expected,256),"Isolated complete BC decode differs from original");
   auto plain=c.take(size),plainRepeat=c.take(size);Check(!std::memcmp(plain,plainRepeat,size),"Original BC encode was unstable");
   const unsigned char* dither=nullptr;if(file){dither=c.take(size);auto again=c.take(size);Check(!std::memcmp(dither,again,size),"Original dither encode was unstable");}
   std::array<float,64> input{};std::memcpy(input.data(),pixels,256);
   for(U policy=0;policy<8;++policy){
    const U flags=c.number();Check(flags==policy<<16,"BC replay policy order");auto previous=c.take(size);std::array<unsigned char,16> output{};Isolation(api);
    status=file?encodeDds(output.data(),size,4,4,format,input.data(),64,flags,0.5f,0):encode(output.data(),format,input.data(),flags,0);Isolation(api);
    Check(status==0&&!std::memcmp(output.data(),previous,size),"Isolated BC candidate policy changed");
    if(policy==0)Check(!std::memcmp(output.data(),plain,size),"Isolated fixed plain BC encoding differs from original");
    if(file&&policy==3)Check(!std::memcmp(output.data(),dither,size),"Isolated fixed dither BC encoding differs from original");
    Check(!std::memcmp(input.data(),pixels,256),"Isolated BC encoder changed input");
   }
   c.end();if(file)++fileBlocks;else ++blocks;
  }else if(current.rfind("bc-filter-float-",0)==0){
   const U size=c.number();auto source=c.take(size);const U tw=c.number(),th=c.number(),sw=Load(source,16),sh=Load(source,12),fourcc=Load(source,84),format=fourcc==0x31545844?18:fourcc==0x33545844?19:20;
   const std::size_t bytes=std::size_t(tw)*th*16;auto expected=c.take(bytes),saved=c.take(bytes);c.end();Check(!std::memcmp(expected,saved,bytes),"DDS float filter diagnostic has an unresolved difference");
   std::vector<float> output(std::size_t(tw)*th*4);Isolation(api);Check(resize(source+128,((sw+3)/4)*(format==18?8:16),sw,sh,format,output.data(),tw*16,tw,th)==0,"Isolated float file consumer");Isolation(api);Check(!std::memcmp(expected,output.data(),bytes),"Isolated full float file filter differs from original");++floatRows;
  }else if(current.rfind("bc-default-dds-",0)==0){
   const U size=c.number();auto source=c.take(size);const U expectedSize=c.number();auto expected=c.take(expectedSize),repeat=c.take(expectedSize);const U savedSize=c.number();auto saved=c.take(savedSize);c.end();
   Check(!std::memcmp(expected,repeat,expectedSize),"Original default DDS was unstable");Check(savedSize==expectedSize&&!std::memcmp(saved,expected,expectedSize),"Default DDS diagnostic has an unresolved difference");
   std::vector<unsigned char> input(source,source+size);Save(inputPath,input);void* resource=nullptr;Isolation(api);auto status=load(device,inputPath.c_str(),&resource);Isolation(api);Check(status==0&&resource,"Isolated default DDS load");
   auto output=LogicalPixels(api,resource);{auto storage=Consume(api,resource,false);output.add(storage.data.data(),storage.data.size());}Isolation(api);
   Check(output.data.size()==expectedSize&&!std::memcmp(output.data.data(),expected,expectedSize),"Isolated complete DDS resource/consumer differs from original");Check(Read(inputPath)==input,"Isolated loader changed source");++files;
  }else if(current.rfind("bc-dds-",0)==0){
   // The old policy sweep remains diagnostic. Its complete records are read
   // and bounded, but no arbitrary policy is claimed as the default loader.
   const U size=c.number();c.take(size);const U expectedSize=c.number();c.take(std::size_t(expectedSize)*2);
   for(U i=0;i<16;++i){Check(c.number()==i/8&&c.number()==(i%8)<<16,"DDS diagnostic policy order");const U count=c.number();c.take(count);}c.end();++diagnosticPolicies;
  }else Check(false,"Unknown BC capture record");
 }
 Check(capture.eof()&&blocks==1536&&fileBlocks==18168&&files==1572&&floatRows==360&&diagnosticPolicies==168,"Incomplete BC replay/EOF");
 current="isolated-bc-rejections";
 for(U variant=0;variant<8;++variant){
  auto input=Dds(4,4,3,3,2);if(variant==0)input.resize(127);else if(variant==1)input.pop_back();else if(variant==2)Store(input.data(),84,0x30315844);else if(variant==3)Store(input.data(),112,0x200);else if(variant==4)Store(input.data(),24,2);else if(variant==5)Store(input.data(),16,0);else if(variant==6)Store(input.data(),12,0);else Store(input.data(),28,16);
  Save(inputPath,input);void* output=reinterpret_cast<void*>(0x13579);Isolation(api);Check(load(device,inputPath.c_str(),&output)<0&&output==reinterpret_cast<void*>(0x13579),"Isolated DDS failure ownership/output");Isolation(api);Check(Read(inputPath)==input,"Rejected DDS changed input");
 }
 DeleteFileA(inputPath.c_str());void* output=reinterpret_cast<void*>(0x13579);Isolation(api);Check(load(device,inputPath.c_str(),&output)==static_cast<std::int32_t>(0x88760b59u)&&output==reinterpret_cast<void*>(0x13579),"Isolated missing-file output");Isolation(api);
 Check(api.deviceRelease(device)==0,"Isolated BC device release");Isolation(api);FreeLibrary(codec);for(auto m:modules)FreeLibrary(m);
 std::cout<<"PASS public BC NoOriginal blocks="<<blocks<<" fileBlocks="<<fileBlocks<<" defaultFiles="<<files<<" rejected=9; complete original floats/blocks/resource/VM/mips/actual RGBA/ownership and EOF\n";
}
