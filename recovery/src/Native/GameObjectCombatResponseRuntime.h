// Included in original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalCombatResponse(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,combatResponseFieldSets[kind-140],data);
    if(kind==141) for(const auto& child:input.children) if(child.name=="AcceptableFriends") MarshalAssistFilter(child,output,32,data);
    if(kind==142) for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {
        auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+40),56,1));
        MarshalModuleFieldSet(child,at,lifecycleFieldSets[6],data);data.Pop();
    }
}
void EndianCombatResponse(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,combatResponseFieldSets[kind-140]);
    if(kind==141) EndianAssistFilter(output,32);
    if(kind==142) {auto status=*reinterpret_cast<unsigned char**>(output+40);if(status) EndianModuleFieldSet(status,lifecycleFieldSets[6]);}
}
