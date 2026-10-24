void MarshalContainOpen(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,containOpenContainModuleDataFields,data);
    for(const auto& child:input.children) if(child.name=="PassengerFilter") MarshalAssistFilter(child,output,104,data);
    for(const auto& child:input.children) if(child.name=="DieMuxData") {auto d=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+108),44,1));MarshalModuleFieldSet(child,d,dieFieldSets[7],data);data.Pop();}
    auto passengers=reinterpret_cast<NativeList*>(output+112);for(const auto& child:input.children) if(child.name=="PassengerData") ++passengers->count;
    if(passengers->count){auto rows=static_cast<unsigned char*>(data.Push(&passengers->data,128,passengers->count));U32 i=0;for(const auto& child:input.children) if(child.name=="PassengerData"){auto row=rows+128*i++;MarshalModuleFields(child,row,containPassengerDataTypeFields,data);for(const auto& f:child.children) if(f.name=="Filter") MarshalUpgradeFilter(f,reinterpret_cast<ObjectFilter*>(row+12),data);}data.Pop();}
    MarshalSupportReferences(input,"ModifierToGiveOnExit",output,120,data);
    auto members=reinterpret_cast<NativeList*>(output+128);for(const auto& child:input.children) if(child.name=="MemberTemplateStatusInfo") ++members->count;
    if(members->count){auto rows=static_cast<unsigned char*>(data.Push(&members->data,8,members->count));U32 i=0;for(const auto& child:input.children) if(child.name=="MemberTemplateStatusInfo") MarshalModuleFields(child,rows+8*i++,containMemberTemplateStatusDataFields,data);data.Pop();}
}
void EndianContainOpen(unsigned char* output) {
    EndianModuleFields(output,containOpenContainModuleDataFields);EndianAssistFilter(output,104);auto d=*reinterpret_cast<unsigned char**>(output+108);if(d) EndianModuleFieldSet(d,dieFieldSets[7]);
    auto p=reinterpret_cast<NativeList*>(output+112);for(U32 i=0;i<p->count;++i){auto row=static_cast<unsigned char*>(p->data)+128*i;EndianModuleFields(row,containPassengerDataTypeFields);EndianObjectFilter(reinterpret_cast<ObjectFilter*>(row+12));}Swap(&p->count);Swap(output+120);
    auto m=reinterpret_cast<NativeList*>(output+128);for(U32 i=0;i<m->count;++i) EndianModuleFields(static_cast<unsigned char*>(m->data)+8*i,containMemberTemplateStatusDataFields);Swap(&m->count);
}
void MarshalContainCore(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned index=kind-270;if(index==0){MarshalContainOpen(input,output,data);return;}
    MarshalModuleFieldSet(input,output,containFieldSets[index],data);
    if(index==1) for(const auto& child:input.children) if(child.name=="InitialRoster") MarshalModuleFields(child,output+148,containInitialRosterFields,data);
    if(index==2){const char* names[]={"EntryOffset","EntryPosition","ExitOffset"};for(unsigned i=0;i<3;++i) for(const auto& child:input.children) if(child.name==names[i]){auto v=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+164+4*i),12,1));MarshalModuleFields(child,v,controlVector3Fields,data);data.Pop();}}
    if(index==7) for(const auto& child:input.children) if(child.name=="CanAlwaysEnterObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+212),data);
    MarshalContainCore(input,output,270+containParents[index],data);
}
void EndianContainCore(unsigned char* output,unsigned kind) {
    unsigned index=kind-270;if(index==0){EndianContainOpen(output);return;}
    EndianModuleFieldSet(output,containFieldSets[index]);
    if(index==1) EndianModuleFields(output+148,containInitialRosterFields);
    if(index==2) for(unsigned i=0;i<3;++i){auto v=*reinterpret_cast<unsigned char**>(output+164+4*i);if(v) EndianModuleFields(v,controlVector3Fields);}
    if(index==7) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+212));
    EndianContainCore(output,270+containParents[index]);
}
