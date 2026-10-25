void MarshalAbilityFinish(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned index=kind-216;MarshalModuleFieldSet(input,output,abilityFinishFieldSets[index],data);
    if(index==1 || index==3) {
        auto list=reinterpret_cast<NativeList*>(output+(index==1?248:252));const char* name=index==1?"ReplaceObjectNugget":"TerrainData";
        for(const auto& child:input.children) if(child.name==name) ++list->count;
        if(list->count) {
            unsigned stride=index==1?12:8;auto rows=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 i=0;
            for(const auto& child:input.children) if(child.name==name) {
                auto row=rows+stride*i++;
                if(index==3) MarshalModuleFields(child,row,abilityFinishSpecialDisguiseTerrainDataFields,data);
                else {
                    for(const auto& filter:child.children) if(filter.name=="TargetObjectFilter") MarshalAssistFilter(filter,row,0,data);
                    MarshalWeakChildren(child,"ReplacementObject",row,4,data);
                }
            }
            data.Pop();
        }
    }
    if(index==2) {
        for(const auto& child:input.children) if(child.name=="PositionOffset") {
            auto vector=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+140),12,1));MarshalModuleFieldSet(child,vector,remainingFieldSets[15],data);data.Pop();
        }
        for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+144),data);
        for(const auto& child:input.children) if(child.name=="IgnoreInsideToInsideCheck") MarshalAssistFilter(child,output,256,data);
    } else MarshalSpecialAbility(input,output,data);
}
void EndianAbilityFinish(unsigned char* output,unsigned kind) {
    unsigned index=kind-216;EndianModuleFieldSet(output,abilityFinishFieldSets[index]);
    if(index==1 || index==3) {
        auto list=reinterpret_cast<NativeList*>(output+(index==1?248:252));
        for(U32 i=0;i<list->count;++i) {
            auto row=static_cast<unsigned char*>(list->data)+(index==1?12:8)*i;
            if(index==3) EndianModuleFields(row,abilityFinishSpecialDisguiseTerrainDataFields);
            else {EndianAssistFilter(row,0);EndianWeakChildren(row,4);}
        }
        Swap(&list->count);
    }
    if(index==2) {
        auto vector=*reinterpret_cast<unsigned char**>(output+140);if(vector) EndianModuleFieldSet(vector,remainingFieldSets[15]);
        EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+144));EndianAssistFilter(output,256);
    } else EndianSpecialAbility(output);
}
