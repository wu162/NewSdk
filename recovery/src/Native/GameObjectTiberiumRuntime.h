// Included in original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalTiberium(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,tiberiumFieldSets[kind-147],data);
    if(kind==147) {MarshalDie(input,output,23,data);return;}
    if(kind==148) for(const auto& child:input.children) if(child.name=="RadarColor") {
        U32 channels[4];for(unsigned i=0;i<4;++i) {auto found=child.attributes.find(tiberiumColorNames[i]);channels[i]=ParseUnsigned(found==child.attributes.end()?tiberiumColorDefaults[i]:found->second)&255u;}
        U32 color=(channels[3]<<24)|(channels[0]<<16)|(channels[1]<<8)|channels[2];std::memcpy(output+28,&color,4);
    }
    if(kind==149) {
        const char* names[]={"DamageFilter","HealFilter"};
        for(unsigned group=0;group<2;++group) for(const auto& child:input.children) if(child.name==names[group]) MarshalAssistFilter(child,output,64+4*group,data);
    }
}
void EndianTiberium(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,tiberiumFieldSets[kind-147]);
    if(kind==147) EndianDie(output,23);
    if(kind==148) Swap(output+28);
    if(kind==149) {EndianAssistFilter(output,64);EndianAssistFilter(output,68);}
}
