// Own fields precede the complete parent, preserving original reference/allocation order.
void MarshalAbilityWeapon(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,abilityWeaponFieldSets[kind-194],data);
    if(kind==195) MarshalModuleFieldSet(input,output,abilityWeaponFieldSets[0],data);
    MarshalSpecialAbility(input,output,data);
}
void EndianAbilityWeapon(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,abilityWeaponFieldSets[kind-194]);
    if(kind==195) EndianModuleFieldSet(output,abilityWeaponFieldSets[0]);
    EndianSpecialAbility(output);
}
