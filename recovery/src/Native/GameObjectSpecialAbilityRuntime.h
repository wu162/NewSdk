// All 45 fields retain original Marshal order; both strings precede nullable Anim12.
void MarshalSpecialAbility(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,abilityFieldSet,data);
    for(const auto& child:input.children) if(child.name=="CustomAnimAndDuration") MarshalUpgradeAnim(child,output,232,data);
}
void EndianSpecialAbility(unsigned char* output) {
    EndianModuleFieldSet(output,abilityFieldSet);
    auto anim=*reinterpret_cast<unsigned char**>(output+232);if(anim) EndianModuleFieldSet(anim,upgradeFieldSets[11]);
}
