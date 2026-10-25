#include "Xmlaptdata.h"
#include "ScalarParsing.h"
#include <cstdio>
#include <cstring>
#include <stdexcept>
#ifdef _MANAGED
#pragma managed(push,off)
#endif
namespace Recovered { namespace Native { namespace AptDataDetail {
using Relo::U32;using Node=XmlaptdataNode;
void Check(bool ok,const char* s){if(!ok)throw std::invalid_argument(s);}
bool In(const std::string& n,const char* list){return(std::string("|")+list+"|").find("|"+n+"|")!=std::string::npos;}
void Validate(const Node& n,const char* attrs,const char* children,bool root=false,bool text=false){Check(text||n.value.find_first_not_of(" \t\r\n")==std::string::npos,"Unexpected APT data text.");for(const auto& a:n.attributes)Check(a.first=="TypeId"||(root&&(a.first=="id"||a.first=="inheritFrom"))||In(a.first,attrs),"Unknown APT data attribute.");for(const auto& c:n.children)Check(In(c.name,children),"Unknown APT data child.");}
const Node* Child(const Node& n,const char* name){const Node* r=nullptr;for(const auto& c:n.children)if(c.name==name){Check(!r,"Duplicate APT data singleton.");r=&c;}return r;}
std::string Value(const Node& n,const char* a,const char* d=""){auto i=n.attributes.find(a);return i==n.attributes.end()?d:i->second;}
void Swap(void* p){U32 v;std::memcpy(&v,p,4);v=(v<<24)|((v<<8)&0xff0000u)|((v>>8)&0xff00u)|(v>>24);std::memcpy(p,&v,4);}
}
unsigned int XmlaptdataTypeId(const std::string& name){
 if(name=="AptAptData")return 1816258268u;
 if(name=="AptConstData")return 983359664u;
 if(name=="AptDatData")return 1110676961u;
 if(name=="AptGeometryData")return 1034587168u;
 return 0;
}
Relo::Chunk CompileXmlaptdata(const XmlaptdataNode& input,bool bigEndian){
 using namespace AptDataDetail;static_assert(sizeof(void*)==4,"APT data requires x86.");Check(XmlaptdataTypeId(input.name)!=0,"Unsupported APT data root.");bool geometry=input.name=="AptGeometryData";Validate(input,geometry?"File|AptID":"File","",true);Relo::Tracker t;unsigned char* root=nullptr;t.Push(reinterpret_cast<void**>(&root),geometry?16:12,1);
 auto file=input.attributes.find("File");if(file!=input.attributes.end()){std::FILE* stream=std::fopen(file->second.c_str(),"rb");if(stream){if(std::fseek(stream,0,SEEK_END)!=0){std::fclose(stream);throw std::invalid_argument("APT DataBlob requires a seekable file.");}long size=std::ftell(stream);if(size<0){std::fclose(stream);throw std::invalid_argument("APT DataBlob file length failed.");}U32 count=static_cast<U32>(size);std::memcpy(root+8,&count,4);void* bytes=nullptr;try{bytes=t.Push(reinterpret_cast<void**>(root+4),1,count);}catch(...){std::fclose(stream);throw;}std::fseek(stream,0,SEEK_SET);if(std::fread(bytes,1,count,stream)!=count){count=0;std::memcpy(root+8,&count,4);}std::fclose(stream);t.Pop();}}
 if(geometry){U32 id=static_cast<U32>(ParseSigned(Value(input,"AptID")));std::memcpy(root+12,&id,4);if(bigEndian)Swap(root+12);}if(bigEndian)Swap(root+8);return t.MakeRelocatable(bigEndian,0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
