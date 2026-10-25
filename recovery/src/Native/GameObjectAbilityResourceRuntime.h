void MarshalAbilityResource(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    if(kind==213) {MarshalModuleFields(input,output,abilityResourceTheftFields,data);MarshalSpecialAbility(input,output,data);return;}
    // Original allocates this vector before the entire OCL500/SpecialPower452 parent.
    for(const auto& child:input.children) if(child.name=="FXOffset") {
        auto vector=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+500),12,1));
        MarshalModuleFieldSet(child,vector,assistFieldSets[6],data);data.Pop();
    }
    MarshalPowerCreation(input,output,198,data);
}
void EndianAbilityResource(unsigned char* output,unsigned kind) {
    if(kind==213) {EndianModuleFields(output,abilityResourceTheftFields);EndianSpecialAbility(output);return;}
    auto vector=*reinterpret_cast<unsigned char**>(output+500);if(vector) EndianModuleFieldSet(vector,assistFieldSets[6]);
    EndianPowerCreation(output,198);
}
