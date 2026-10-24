// Own fields and nullable vector precede the complete SpecialAbility parent.
void MarshalAbilityMovement(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,abilityMovementFieldSets[kind-200],data);
    if(kind==201) for(const auto& child:input.children) if(child.name=="EjectionBoneOffset") {
        auto vector=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+272),12,1));
        MarshalModuleFieldSet(child,vector,assistFieldSets[6],data);data.Pop();
    }
    MarshalSpecialAbility(input,output,data);
}
void EndianAbilityMovement(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,abilityMovementFieldSets[kind-200]);
    if(kind==201) {auto vector=*reinterpret_cast<unsigned char**>(output+272);if(vector) EndianModuleFieldSet(vector,assistFieldSets[6]);}
    EndianSpecialAbility(output);
}
