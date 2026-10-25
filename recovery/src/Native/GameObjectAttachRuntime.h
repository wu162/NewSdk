// Derived allocations precede Attach's nullable masks, filter and strong reference list.
void MarshalAttach(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    if(kind==223) {
        MarshalModuleFields(input,output,attachMoneyGainAttachUpdateModuleDataFields,data);
        for(const auto& child:input.children) if(child.name=="MoneyGainObjectStatusValidation") {
            auto value=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+344),56,1));
            MarshalModuleFields(child,value,attachObjectStatusValidationDataTypeFields,data);data.Pop();
        }
    }
    if(kind==224) {
        MarshalModuleFields(input,output,attachBoobyTrapUpdateModuleDataFields,data);
        for(const auto& child:input.children) if(child.name=="PreventEnteringFilter") MarshalAssistFilter(child,output,396,data);
    }
    MarshalModuleFields(input,output,attachAttachUpdateModuleDataFields,data);
    for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalAssistFilter(child,output,324,data);
    auto list=reinterpret_cast<NativeList*>(output+328);
    for(const auto& child:input.children) if(child.name=="ModifierToLeechFromParent") ++list->count;
    if(list->count) {
        auto refs=static_cast<U32*>(data.Push(&list->data,4,list->count));U32 i=0;
        for(const auto& child:input.children) if(child.name=="ModifierToLeechFromParent") MarshalReference(data,refs+i++,child.text);
        data.Pop();
    }
}
void EndianAttach(unsigned char* output,unsigned kind) {
    if(kind==223) {
        EndianModuleFields(output,attachMoneyGainAttachUpdateModuleDataFields);
        auto value=*reinterpret_cast<unsigned char**>(output+344);if(value) EndianModuleFields(value,attachObjectStatusValidationDataTypeFields);
    }
    if(kind==224) {EndianModuleFields(output,attachBoobyTrapUpdateModuleDataFields);EndianAssistFilter(output,396);}
    EndianModuleFields(output,attachAttachUpdateModuleDataFields);EndianAssistFilter(output,324);Swap(output+328);
}
