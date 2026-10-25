// Included in original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalStatusEffects(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,statusEffectsFieldSets[kind-143],data);
    if(kind!=143) return;
    const char* names[]={"StartColor","EndColor"};
    for(unsigned group=0;group<2;++group) for(const auto& child:input.children) if(child.name==names[group]) {
        auto color=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+32+4*group),12,1));
        MarshalModuleFieldSet(child,color,statusEffectsFieldSets[2],data);data.Pop();
    }
}
void EndianStatusEffects(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,statusEffectsFieldSets[kind-143]);
    if(kind!=143) return;
    for(unsigned group=0;group<2;++group) {auto color=*reinterpret_cast<unsigned char**>(output+32+4*group);if(color) EndianModuleFieldSet(color,statusEffectsFieldSets[2]);}
}

