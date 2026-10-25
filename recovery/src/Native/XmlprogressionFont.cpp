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
using N=XmlprogressionNode;using U=Relo::U32;
bool InFont(const std::string& s,const char* list){return (std::string("|")+list+"|").find("|"+s+"|")!=std::string::npos;}
void ValidateFont(const N& n,const char* attrs,const char* children,bool text=false){for(const auto& a:n.attributes)if(!InFont(a.first,attrs))throw std::invalid_argument("Unsupported font attribute");for(const auto& c:n.children)if(!InFont(c.name,children))throw std::invalid_argument("Unsupported font child");if(!text)for(unsigned char c:n.text)if(!std::isspace(c))throw std::invalid_argument("Unexpected font text");}
const N* FontChild(const N& n,const char* name){const N* found=nullptr;for(const auto& c:n.children)if(c.name==name){if(found)throw std::invalid_argument("Duplicate font singleton child");found=&c;}return found;}
void FontWord(void* p,U x){std::memcpy(p,&x,4);}void FontSwap(void* p){U x;std::memcpy(&x,p,4);FontWord(p,(x<<24)|((x<<8)&0xff0000u)|((x>>8)&0xff00u)|(x>>24));}
struct FontBuilder {Relo::Tracker data;bool big;FontBuilder(bool b):big(b){}
 void Word(void* p,U x){FontWord(p,x);if(big)FontSwap(p);}
 void Real(void* p,const std::string& s){float f=ParseFloat(s);std::memcpy(p,&f,4);if(big)FontSwap(p);}
 void Name(const N& n,unsigned char* p){auto a=n.attributes.find("Name");if(a!=n.attributes.end()){MarshalString(data,p,a->second);if(big)FontSwap(p);}}
 void Desc(const N* n,unsigned char* p){if(!n)return;ValidateFont(*n,"Name|Size|Bold","");Name(*n,p);auto a=n->attributes.find("Size");if(a!=n->attributes.end())Word(p+8,ParseUnsigned(a->second));a=n->attributes.find("Bold");if(a!=n->attributes.end())p[12]=ParseBoolean(a->second);}
};
}
Relo::Chunk CompileXmlprogressionFont(const N& input,bool big){FontBuilder b(big);unsigned char* root=nullptr;b.data.Push(reinterpret_cast<void**>(&root),20,1);
 if(input.name=="FontDefaultSetting"){ValidateFont(input,"id|Name","Size|Settings");b.Name(input,root+4);auto size=FontChild(input,"Size");if(size){ValidateFont(*size,"","",true);auto value=b.data.Push(reinterpret_cast<void**>(root+12),4,1);b.Word(value,ParseUnsigned(size->text));b.data.Pop();}auto settings=FontChild(input,"Settings");if(settings){ValidateFont(*settings,"AntiAliased","");auto a=settings->attributes.find("AntiAliased");if(a!=settings->attributes.end())root[16]=ParseBoolean(a->second);}}
 else if(input.name=="FontTranslation"){ValidateFont(input,"id|Name|TranslateX|TranslateY","");b.Name(input,root+4);auto x=input.attributes.find("TranslateX");if(x!=input.attributes.end())b.Real(root+12,x->second);auto y=input.attributes.find("TranslateY");if(y!=input.attributes.end())b.Real(root+16,y->second);}
 else if(input.name=="FontSubstitution"){ValidateFont(input,"id|Name","Substitute");b.Name(input,root+4);U count=static_cast<U>(input.children.size());b.Word(root+12,count);if(count){auto items=static_cast<unsigned char*>(b.data.Push(reinterpret_cast<void**>(root+16),20,count));for(U i=0;i<count;++i){const auto& n=input.children[i];ValidateFont(n,"Size","Font");auto a=n.attributes.find("Size");if(a!=n.attributes.end())b.Word(items+20*i,ParseUnsigned(a->second));b.Desc(FontChild(n,"Font"),items+20*i+4);}b.data.Pop();}}
 else throw std::invalid_argument("Unsupported font root");
 return b.data.MakeRelocatable(big,0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
