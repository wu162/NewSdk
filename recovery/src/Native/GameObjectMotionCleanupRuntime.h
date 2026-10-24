// Included in original family definition order by GameObjectModuleFamiliesRuntime.h.
void MarshalMotionCleanup(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,motionCleanupFieldSets[kind-159],data);
    if(kind==160) for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+12),data);
    if(kind==161) {
        for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {
            auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+44),56,1));
            MarshalModuleFieldSet(child,at,lifecycleFieldSets[6],data);data.Pop();
        }
        for(const auto& child:input.children) if(child.name=="KeepAliveObjectFilter") MarshalAssistFilter(child,output,48,data);
    }
}
void EndianMotionCleanup(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,motionCleanupFieldSets[kind-159]);
    if(kind==160) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+12));
    if(kind==161) {
        auto at=*reinterpret_cast<unsigned char**>(output+44);if(at) EndianModuleFieldSet(at,lifecycleFieldSets[6]);
        EndianAssistFilter(output,48);
    }
}
