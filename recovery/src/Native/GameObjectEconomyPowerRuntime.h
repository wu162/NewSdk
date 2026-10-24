// Included in the original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalEconomyPower(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,economyPowerFieldSets[kind-133],data);
    if(kind!=135) return;
    // Three independent lists are allocated in schema order, irrespective of XML child order.
    const char* names[]={"InitialCaptureBonus","Deposit","ResourceCap"};
    for(unsigned group=0;group<3;++group) {
        auto list=reinterpret_cast<NativeList*>(output+20+8*group);
        for(const auto& child:input.children) if(child.name==names[group]) ++list->count;
        if(!list->count) continue;
        auto rows=static_cast<unsigned char*>(data.Push(&list->data,8,list->count));unsigned i=0;
        for(const auto& child:input.children) if(child.name==names[group]) MarshalModuleFieldSet(child,rows+8*i++,economyPowerFieldSets[4],data);
        data.Pop();
    }
}
void EndianEconomyPower(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,economyPowerFieldSets[kind-133]);
    if(kind!=135) return;
    for(unsigned group=0;group<3;++group) {
        auto list=reinterpret_cast<NativeList*>(output+20+8*group);
        for(U32 i=0;i<list->count;++i) EndianModuleFieldSet(static_cast<unsigned char*>(list->data)+8*i,economyPowerFieldSets[4]);
        Swap(&list->count);
    }
}

