// Included in original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalWallEnclose(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,wallEncloseFieldSets[kind-150],data);
    if(kind==150) MarshalWeakChildren(input,"SegmentPattern",output,48,data);
    if(kind==151) for(const auto& child:input.children) if(child.name=="Filter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+28),data);
}
void EndianWallEnclose(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,wallEncloseFieldSets[kind-150]);
    if(kind==150) EndianWeakChildren(output,48);
    if(kind==151) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+28));
}
