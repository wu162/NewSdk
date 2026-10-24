#include "Xmlprogression.h"
#include "ScalarParsing.h"
#include "ResourceMarshaling.h"
#include <cstring>
#include <cctype>
#include <stdexcept>
#ifdef _MANAGED
#pragma managed(push,off)
#endif
namespace Recovered { namespace Native {
namespace {
using U=Relo::U32;
bool Allowed(const std::string& n,const char* list){return (std::string("|")+list+"|").find("|"+n+"|")!=std::string::npos;}
void CheckNode(const XmlprogressionNode& n,const char* attrs,const char* children){for(const auto& a:n.attributes)if(!Allowed(a.first,attrs))throw std::invalid_argument("Unsupported UICommandButton attribute");for(const auto& c:n.children)if(!Allowed(c.name,children))throw std::invalid_argument("Unsupported UICommandButton child");for(unsigned char c:n.text)if(!std::isspace(c))throw std::invalid_argument("Unexpected UICommandButton text");}
void Store(void* p,U x){std::memcpy(p,&x,4);}void Swap(void* p){U x;std::memcpy(&x,p,4);Store(p,(x<<24)|((x<<8)&0xff0000u)|((x>>8)&0xff00u)|(x>>24));}
std::string Attr(const XmlprogressionNode& n,const char* name,const char* fallback=""){auto a=n.attributes.find(name);return a==n.attributes.end()?std::string(fallback):a->second;}
U Faction(const std::string& s){if(s=="Soviet")return 1;if(s=="Japan")return 2;if(s=="Neutral")return 3;if(s=="RANDOM")return ~0u;return 0;}
}
Relo::Chunk CompileXmlprogressionUICommandButton(const XmlprogressionNode& input,bool big){
 CheckNode(input,"id","FactionSettings");Relo::Tracker data;unsigned char* root=nullptr;data.Push(reinterpret_cast<void**>(&root),12,1);U count=static_cast<U>(input.children.size());Store(root+4,count);
 if(count){auto items=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(root+8),12,count));for(U i=0;i<count;++i){const auto& n=input.children[i];CheckNode(n,"Faction|LockedImage","ImageTintColor");unsigned char* at=items+12*i;Store(at,Faction(Attr(n,"Faction")));if(n.attributes.count("LockedImage"))MarshalReference(data,at+4,Attr(n,"LockedImage"));if(n.children.size()>1)throw std::invalid_argument("Duplicate ImageTintColor");if(!n.children.empty()){const auto& color=n.children[0];CheckNode(color,"R|G|B|A","");U r=ParseUnsigned(Attr(color,"R","0"))&255u,g=ParseUnsigned(Attr(color,"G","0"))&255u,b=ParseUnsigned(Attr(color,"B","0"))&255u,a=ParseUnsigned(Attr(color,"A","255"))&255u;Store(at+8,(a<<24)|(r<<16)|(g<<8)|b);}if(big){Swap(at);Swap(at+8);}}data.Pop();}
 if(big)Swap(root+4);return data.MakeRelocatable(big,0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
