// Included in original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalStealthVision(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,stealthVisionFieldSets[kind-145],data);
    if(kind==145) {MarshalUpgrade(input,output,30,data);return;}
    for(unsigned group=0;group<2;++group) {
        auto found=input.attributes.find(stealthVisionVoiceNames[group]);if(found==input.attributes.end()) continue;
        auto value=static_cast<U32*>(data.Push(reinterpret_cast<void**>(output+20+4*group),4,1));
        *value=ExactStringHash(found->second);data.Pop();
    }
    for(const auto& child:input.children) if(child.name=="RequiresNearbyObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+28),data);
}
void EndianStealthVision(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,stealthVisionFieldSets[kind-145]);
    if(kind==145) {EndianUpgrade(output,30);return;}
    for(unsigned group=0;group<2;++group) {auto value=*reinterpret_cast<U32**>(output+20+4*group);if(value) Swap(value);}
    EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+28));
}
