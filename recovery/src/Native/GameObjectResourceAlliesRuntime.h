// Included in original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalResourceAllies(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,resourceAlliesFieldSets[kind-152],data);
    if(kind==152) {MarshalVision(input,output,61,data);return;}
    for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalAssistFilter(child,output,36,data);
}
void EndianResourceAllies(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,resourceAlliesFieldSets[kind-152]);
    if(kind==152) EndianVision(output,61);
    else EndianAssistFilter(output,36);
}
