// Lists contain concrete values, not polymorphic module pointers.
void MarshalAbilityRouting(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+240);const char* name=kind==211?"SpecialPowerModules":"IntersectionFlagsFilter";
    for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(list->count) {
        unsigned stride=kind==211?12:128;auto entries=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 i=0;
        for(const auto& child:input.children) if(child.name==name) {
            auto entry=entries+stride*i++;
            if(kind==211) MarshalModuleFields(child,entry,abilityRoutingSpecialPowerModuleInfoFields,data);
            else MarshalModuleFields(child,entry,abilityRoutingKindOfAndStatusAndModelConditionTypeFields,data);
        }
        data.Pop();
    }
    MarshalSpecialAbility(input,output,data);
}
void EndianAbilityRouting(unsigned char* output,unsigned kind) {
    auto list=reinterpret_cast<NativeList*>(output+240);unsigned stride=kind==211?12:128;
    for(U32 i=0;i<list->count;++i) {
        auto entry=static_cast<unsigned char*>(list->data)+stride*i;
        if(kind==211) EndianModuleFields(entry,abilityRoutingSpecialPowerModuleInfoFields);
        else EndianModuleFields(entry,abilityRoutingKindOfAndStatusAndModelConditionTypeFields);
    }
    Swap(&list->count);EndianSpecialAbility(output);
}
