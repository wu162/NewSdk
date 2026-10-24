// Root SpecialPowerUpdate is a 24-byte Update, not a 452-byte SpecialPower.
void MarshalPowerRouting(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,powerRoutingFieldSets[kind-188],data);
    if(kind==188) {
        for(const auto& child:input.children) if(child.name=="InitiateSound") {
            auto sound=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+16),8,1));
            MarshalModuleFields(child,sound,specialPowerSoundFields,data);data.Pop();
        }
        return;
    }
    auto list=reinterpret_cast<NativeList*>(output+452);const char* name=kind==189?"Modules":"SpecialPower";
    for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(list->count) {
        unsigned stride=kind==189?16:4;auto entries=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 index=0;
        for(const auto& child:input.children) if(child.name==name) {
            auto entry=entries+stride*index++;
            if(kind==189) {MarshalModuleFields(child,entry,powerRoutingActivateFields,data);MarshalModuleFields(child,entry,powerRoutingInfoFields,data);}
            else MarshalModuleFields(child,entry,powerRoutingDispatchFields,data);
        }
        data.Pop();
    }
    MarshalSpecialPowerBase(input,output,162,data);
}
void EndianPowerRouting(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,powerRoutingFieldSets[kind-188]);
    if(kind==188) return; // original skips nullable sound references, as the parent does
    auto list=reinterpret_cast<NativeList*>(output+452);
    if(kind==189) for(U32 i=0;i<list->count;++i) {
        auto entry=static_cast<unsigned char*>(list->data)+16*i;EndianModuleFields(entry,powerRoutingInfoFields);
    }
    Swap(&list->count);EndianSpecialPowerBase(output,162);
}
