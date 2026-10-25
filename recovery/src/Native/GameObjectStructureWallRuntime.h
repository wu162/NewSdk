// Included in the original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalStructureWall(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,structureWallFieldSets[kind-137],data);
    if(kind==137) for(const auto& child:input.children) if(child.name=="ConnectingObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+100),data);
    if(kind==139) for(const auto& child:input.children) if(child.name=="RestrictedAreaShape") {
        auto shape=output+8;MarshalModuleFieldSet(child,shape,structureWallFieldSets[3],data);
        for(const auto& offset:child.children) if(offset.name=="Offset") {
            auto vector=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(shape+40),12,1));
            MarshalModuleFieldSet(offset,vector,assistFieldSets[6],data);data.Pop();
        }
    }
}
void EndianStructureWall(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,structureWallFieldSets[kind-137]);
    if(kind==137) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+100));
    if(kind==139) {
        auto shape=output+8;EndianModuleFieldSet(shape,structureWallFieldSets[3]);
        auto vector=*reinterpret_cast<unsigned char**>(shape+40);if(vector) EndianModuleFieldSet(vector,assistFieldSets[6]);
    }
}
