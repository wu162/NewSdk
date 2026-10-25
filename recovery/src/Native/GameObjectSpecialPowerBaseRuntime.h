// Included in family definition order; original SpecialPower methods 000-005/019.
void MarshalSpecialPowerBase(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    // Both Radar classes marshal their own scalars before the complete parent.
    if(kind!=162) MarshalModuleFieldSet(input,output,specialPowerFieldSets[kind-162],data);
    MarshalModuleFieldSet(input,output,specialPowerFieldSets[0],data);
    const char* names[]={"AttributeModifierAffects","RequirementsFilterMP","RequirementsFilterStrategic"};
    for(unsigned i=0;i<3;++i) for(const auto& child:input.children) if(child.name==names[i])
        MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+96+112*i),data);
    for(const auto& child:input.children) if(child.name=="InitiateSound") {
        auto sound=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+432),8,1));
        MarshalModuleFields(child,sound,specialPowerSoundFields,data);data.Pop();
    }
}
void EndianSpecialPowerBase(unsigned char* output,unsigned kind) {
    if(kind!=162) EndianModuleFieldSet(output,specialPowerFieldSets[kind-162]);
    EndianModuleFieldSet(output,specialPowerFieldSets[0]);
    for(unsigned i=0;i<3;++i) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+96+112*i));
    // Original parent has no SoundOrEvaEvent Endian call. Strong references
    // (including the nested nullable Sound) are finalized by Tracker, not swapped.
}
