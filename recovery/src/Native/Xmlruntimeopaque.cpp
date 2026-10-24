#include "Xmlruntimeopaque.h"
#include <cstring>
#include <stdexcept>
#ifdef _MANAGED
#pragma managed(push,off)
#endif
namespace Recovered { namespace Native { namespace RuntimeOpaqueDetail {
using Relo::U32;using Node=XmlruntimeopaqueNode;
void Check(bool ok,const char* s){if(!ok)throw std::invalid_argument(s);}
bool In(const std::string& n,const char* list){return(std::string("|")+list+"|").find("|"+n+"|")!=std::string::npos;}
void Validate(const Node& n,const char* attrs,const char* children,bool root=false,bool text=false){Check(text||n.value.find_first_not_of(" \t\r\n")==std::string::npos,"Unexpected opaque runtime text.");for(const auto& a:n.attributes)Check(a.first=="TypeId"||(root&&(a.first=="id"||a.first=="inheritFrom"))||In(a.first,attrs),"Unknown opaque runtime attribute.");for(const auto& c:n.children)Check(In(c.name,children),"Unknown opaque runtime child.");}
const Node* Child(const Node& n,const char* name){const Node* r=nullptr;for(const auto& c:n.children)if(c.name==name){Check(!r,"Duplicate opaque runtime singleton.");r=&c;}return r;}
std::string Value(const Node& n,const char* a,const char* d=""){auto i=n.attributes.find(a);return i==n.attributes.end()?d:i->second;}
void Swap(void* p){U32 v;std::memcpy(&v,p,4);v=(v<<24)|((v<<8)&0xff0000u)|((v>>8)&0xff00u)|(v>>24);std::memcpy(p,&v,4);}
}
unsigned int XmlruntimeopaqueTypeId(const std::string& name){
 if(name=="BaseRenderAssetType")return 2989459401u;
 if(name=="AudioFileRuntime")return 584092973u;
 return 0;
}
Relo::Chunk CompileXmlruntimeopaque(const XmlruntimeopaqueNode& input,bool bigEndian){
 using namespace RuntimeOpaqueDetail;static_assert(sizeof(void*)==4,"Opaque runtime requires x86.");Check(XmlruntimeopaqueTypeId(input.name)!=0,"Unsupported opaque runtime root.");bool audio=input.name=="AudioFileRuntime";Validate(input,audio?"SubtitleStringName|NumberOfChannels|NumberOfSamples|SampleRate|HeaderData|HeaderDataSize":"","",true,true);Relo::Tracker t;unsigned char* root=nullptr;t.Push(reinterpret_cast<void**>(&root),audio?32:4,1);return t.MakeRelocatable(bigEndian,0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
