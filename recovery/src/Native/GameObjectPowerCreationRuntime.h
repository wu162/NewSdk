void MarshalPowerCreation(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,powerCreationFieldSets[kind-198],data);
    if(kind==198) {
        auto pairs=reinterpret_cast<NativeList*>(output+476);
        for(const auto& child:input.children) if(child.name=="UpgradeOCL") ++pairs->count;
        if(pairs->count) {
            auto entries=static_cast<U32*>(data.Push(&pairs->data,4,pairs->count));U32 i=0;
            for(const auto& child:input.children) if(child.name=="UpgradeOCL") {auto value=child.attributes.find("OCL");if(value!=child.attributes.end()) MarshalReference(data,entries+i,value->second);++i;}
            data.Pop();
        }
        for(const auto& child:input.children) if(child.name=="NearestSecondaryObjectFilter") MarshalAssistFilter(child,output,484,data);
        auto upgrades=reinterpret_cast<NativeList*>(output+488);
        for(const auto& child:input.children) if(child.name=="Upgrade") ++upgrades->count;
        if(upgrades->count) {
            auto entries=static_cast<U32*>(data.Push(&upgrades->data,4,upgrades->count));U32 i=0;
            for(const auto& child:input.children) if(child.name=="Upgrade") MarshalReference(data,entries+i++,child.text);
            data.Pop();
        }
    }
    MarshalSpecialPowerBase(input,output,162,data);
}
void EndianPowerCreation(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,powerCreationFieldSets[kind-198]);
    if(kind==198) {Swap(output+476);EndianAssistFilter(output,484);Swap(output+488);}
    EndianSpecialPowerBase(output,162);
}
