#include "Xmlredalert.h"
#include "ScalarParsing.h"
#include "DataInitializers.h"
#include "ResourceMarshaling.h"
#include <cstring>
#include <stdexcept>
#ifdef _MANAGED
#pragma managed(push,off)
#endif
namespace Recovered { namespace Native { namespace RedAlertDetail {
using Relo::U32;using Node=XmlredalertNode;
void Check(bool ok,const char* s){if(!ok)throw std::invalid_argument(s);}
bool In(const std::string& n,const char* list){return(std::string("|")+list+"|").find("|"+n+"|")!=std::string::npos;}
void Validate(const Node& n,const char* attrs,const char* children,bool root=false,bool text=false){
 Check(text||n.value.find_first_not_of(" \t\r\n")==std::string::npos,"Unexpected red alert text.");
 for(const auto& a:n.attributes)Check(a.first=="TypeId"||(root&&(a.first=="id"||a.first=="inheritFrom"))||In(a.first,attrs),"Unknown red alert attribute.");
 for(const auto& c:n.children)Check(In(c.name,children),"Unknown red alert child.");
}
std::string Value(const Node& n,const char* a,const char* fallback=""){auto i=n.attributes.find(a);return i==n.attributes.end()?fallback:i->second;}
void Swap(void* p){U32 v;std::memcpy(&v,p,4);v=(v<<24)|((v<<8)&0xff0000u)|((v>>8)&0xff00u)|(v>>24);std::memcpy(p,&v,4);}
}
unsigned int XmlredalertTypeId(const std::string& name){return name=="RedAlertTemplate"?4141306833u:0;}
Relo::Chunk CompileXmlredalert(const XmlredalertNode& input,bool bigEndian){
 using namespace RedAlertDetail;static_assert(sizeof(void*)==4,"Red alert requires x86.");Check(XmlredalertTypeId(input.name)!=0,"Unsupported red alert root.");Validate(input,"OrangeAreaThreatThreshold|RedAreaThreatThreshold|MaxAllowedThreat|YellowAreaThreatDecayPerSecond|OrangeAreaThreatDecayPerSecond|RedAreaThreatDecayPerSecond|RedAlertBuildingDamagePerSecond|RedAlertUpdatedBuildingDamagePerSecond|KillThreatMultiplier|DeathThreatMultiplier|YellowAlertTechPointMultiplier|OrangeAlertTechPointMultiplier|RedAlertTechPointMultiplier|DisableNonRepairability|RedAlertDisableStructureBuilding|RedAlertShowAllUnitsAndStructures|EvaEventToPlayOnRedAlertInitiateOwner|EvaEventToPlayOnRedAlertInitiateAlly|EvaEventToPlayOnRedAlertInitiateEnemy|RedAlertBuildingDamageIncreaseTime","YellowAreaEffect|OrangeAreaEffect|RedAreaEffect|RedAlertEffect",true);Relo::Tracker t;unsigned char* root=nullptr;t.Push(reinterpret_cast<void**>(&root),92,1);
 auto word=[&](unsigned off,U32 v){std::memcpy(root+off,&v,4);if(bigEndian)Swap(root+off);};auto real=[&](unsigned off,float v){std::memcpy(root+off,&v,4);if(bigEndian)Swap(root+off);};
 word(4,ParseUnsigned(Value(input,"OrangeAreaThreatThreshold","0")));
 word(8,ParseUnsigned(Value(input,"RedAreaThreatThreshold","0")));
 word(12,ParseUnsigned(Value(input,"MaxAllowedThreat","0")));
 word(24,ParseUnsigned(Value(input,"YellowAreaThreatDecayPerSecond","5")));
 word(28,ParseUnsigned(Value(input,"OrangeAreaThreatDecayPerSecond","5")));
 word(32,ParseUnsigned(Value(input,"RedAreaThreatDecayPerSecond","5")));
 word(36,ParseUnsigned(Value(input,"RedAlertBuildingDamagePerSecond","0")));
 word(40,ParseUnsigned(Value(input,"RedAlertUpdatedBuildingDamagePerSecond","0")));
 real(16,ParseFloat(Value(input,"KillThreatMultiplier","1.0")));
 real(20,ParseFloat(Value(input,"DeathThreatMultiplier","1.0")));
 real(60,ParseFloat(Value(input,"YellowAlertTechPointMultiplier","1.0")));
 real(64,ParseFloat(Value(input,"OrangeAlertTechPointMultiplier","1.0")));
 real(68,ParseFloat(Value(input,"RedAlertTechPointMultiplier","1.0")));
 root[88]=ParseBoolean(Value(input,"DisableNonRepairability","false"));
 root[89]=ParseBoolean(Value(input,"RedAlertDisableStructureBuilding","true"));
 root[90]=ParseBoolean(Value(input,"RedAlertShowAllUnitsAndStructures","true"));
 {auto a=input.attributes.find("EvaEventToPlayOnRedAlertInitiateOwner");if(a!=input.attributes.end())MarshalReference(t,root+48,a->second);}
 {auto a=input.attributes.find("EvaEventToPlayOnRedAlertInitiateAlly");if(a!=input.attributes.end())MarshalReference(t,root+52,a->second);}
 {auto a=input.attributes.find("EvaEventToPlayOnRedAlertInitiateEnemy");if(a!=input.attributes.end())MarshalReference(t,root+56,a->second);}
 real(44,InitializeTime(Value(input,"RedAlertBuildingDamageIncreaseTime","0s").c_str()));
 const char* names[]={"YellowAreaEffect","OrangeAreaEffect","RedAreaEffect","RedAlertEffect"};for(unsigned i=0;i<4;++i){const Node* child=nullptr;for(const auto& c:input.children)if(c.name==names[i]){Check(!child,"Duplicate red alert effect.");child=&c;}if(child){Validate(*child,"","",false,true);auto ref=t.Push(reinterpret_cast<void**>(root+72+4*i),4,1);MarshalReference(t,ref,child->value);t.Pop();}}
 return t.MakeRelocatable(bigEndian,0);
}
}}
#ifdef _MANAGED
#pragma managed(pop)
#endif
