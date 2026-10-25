// All own allocations precede the complete SpecialPower parent, as in original.
void MarshalSpecialPowerDerived(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned index=kind-165;MarshalModuleFieldSet(input,output,specialDerivedFieldSets[index],data);
    const auto& children=specialDerivedChildren[index];
    if(children.filter) for(const auto& child:input.children) if(child.name==children.filter)
        MarshalAssistFilter(child,output,children.filterOffset,data);
    if(children.strongList) {
        auto list=reinterpret_cast<NativeList*>(output+children.listOffset);
        for(const auto& child:input.children) if(child.name==children.strongList) ++list->count;
        if(list->count) {
            auto references=static_cast<U32*>(data.Push(&list->data,4,list->count));U32 at=0;
            for(const auto& child:input.children) if(child.name==children.strongList) MarshalReference(data,references+at++,child.text);
            data.Pop();
        }
    }
    MarshalSpecialPowerBase(input,output,162,data);
}
void EndianSpecialPowerDerived(unsigned char* output,unsigned kind) {
    unsigned index=kind-165;EndianModuleFieldSet(output,specialDerivedFieldSets[index]);
    const auto& children=specialDerivedChildren[index];
    if(children.filter) EndianAssistFilter(output,children.filterOffset);
    if(children.strongList) Swap(output+children.listOffset); // count only; strong references belong to Tracker
    EndianSpecialPowerBase(output,162);
}
