// Original order: nugget string, inline AI (only if present), ModuleInfo list;
// the complete ToggleState list precedes the full SpecialAbility parent.
void MarshalAbilityToggleAI(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,abilityToggleAIToggleSpecialPowerInfoFields,data);
    for(const auto& child:input.children) if(child.name=="StateWeapon") {
        auto weapon=data.Push(reinterpret_cast<void**>(output+140),28,1);
        MarshalModuleFields(child,static_cast<unsigned char*>(weapon),abilityToggleAISpecialPowerWeaponDataFields,data);data.Pop();
    }
}
void MarshalAbilityToggle(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,abilityToggleToggleStatusSpecialAbilityUpdateModuleDataFields,data);
    auto list=reinterpret_cast<NativeList*>(output+240);
    for(const auto& child:input.children) if(child.name=="ToggleState") ++list->count;
    if(list->count) {
        auto rows=static_cast<unsigned char*>(data.Push(&list->data,280,list->count));U32 i=0;
        for(const auto& child:input.children) if(child.name=="ToggleState") {
            auto row=rows+280*i++;MarshalModuleFields(child,row,abilityToggleToggleStatusNuggetFields,data);
            for(const auto& ai:child.children) if(ai.name=="SkirmishAiInfo") MarshalAbilityToggleAI(ai,row+124,data);
            auto modules=reinterpret_cast<NativeList*>(row+272);
            for(const auto& module:child.children) if(module.name=="Module") ++modules->count;
            if(modules->count) {
                auto entries=static_cast<unsigned char*>(data.Push(&modules->data,12,modules->count));U32 j=0;
                for(const auto& module:child.children) if(module.name=="Module") MarshalModuleFields(module,entries+12*j++,abilityRoutingSpecialPowerModuleInfoFields,data);
                data.Pop();
            }
        }
        data.Pop();
    }
    MarshalSpecialAbility(input,output,data);
}
void EndianAbilityToggle(unsigned char* output) {
    auto list=reinterpret_cast<NativeList*>(output+240);
    for(U32 i=0;i<list->count;++i) {
        auto row=static_cast<unsigned char*>(list->data)+280*i;EndianModuleFields(row,abilityToggleToggleStatusNuggetFields);
        auto ai=row+124;EndianModuleFields(ai,abilityToggleAIToggleSpecialPowerInfoFields);
        auto weapon=*reinterpret_cast<unsigned char**>(ai+140);if(weapon) EndianModuleFields(weapon,abilityToggleAISpecialPowerWeaponDataFields);
        auto modules=reinterpret_cast<NativeList*>(row+272);
        for(U32 j=0;j<modules->count;++j) EndianModuleFields(static_cast<unsigned char*>(modules->data)+12*j,abilityRoutingSpecialPowerModuleInfoFields);
        Swap(&modules->count);
    }
    Swap(&list->count);EndianSpecialAbility(output);
}
