// Preserve own String8/filter/list allocations and reference order before SpecialAbility240.
void MarshalAbilityState(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,abilityStateFieldSets[kind-204],data);
    if(kind==206 || kind==207) for(const auto& child:input.children) if(child.name==(kind==206?"AttributeModifierAffects":"AcceptanceFilter")) MarshalAssistFilter(child,output,252,data);
    if(kind==209) MarshalWeakChildren(input,"ReplacementTemplate",output,252,data);
    if(kind==210) MarshalWeakChildren(input,"StructureTemplate",output,248,data);
    MarshalSpecialAbility(input,output,data);
}
void EndianAbilityState(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,abilityStateFieldSets[kind-204]);
    if(kind==206 || kind==207) EndianAssistFilter(output,252);
    if(kind==209) EndianWeakChildren(output,252);
    if(kind==210) EndianWeakChildren(output,248);
    EndianSpecialAbility(output);
}
