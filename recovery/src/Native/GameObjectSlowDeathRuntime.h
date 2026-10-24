void MarshalSlowContact(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,slowContactPointFields,data);
    for(const auto& child:input.children) if(child.name=="Pos") MarshalModuleFields(child,output+8,slowVector3Fields,data);
}
void EndianSlowContact(unsigned char* output) {EndianModuleFields(output,slowContactPointFields);EndianModuleFields(output+8,slowVector3Fields);}
void MarshalSlowGeometry(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,slowGeometryInfoFields,data);
    auto list=reinterpret_cast<NativeList*>(output+16);for(const auto& child:input.children) if(child.name=="Shapes") ++list->count;
    if(list->count) {
        auto rows=static_cast<unsigned char*>(data.Push(&list->data,32,list->count));U32 i=0;
        for(const auto& child:input.children) if(child.name=="Shapes") {auto row=rows+32*i++;MarshalModuleFields(child,row,slowGeometryShapeFields,data);for(const auto& v:child.children) if(v.name=="Offset") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(row+24),12,1));MarshalModuleFields(v,at,slowVector3Fields,data);data.Pop();}}
        data.Pop();
    }
    const char* names[]={"RotationAnchorOffset","Center","HighestContactPoint","InnermostContactPoint"};
    for(unsigned i=0;i<4;++i) for(const auto& child:input.children) if(child.name==names[i]) {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+24+4*i),i==0?8:12,1));if(i==0) MarshalModuleFields(child,at,slowVector2Fields,data);else MarshalModuleFields(child,at,slowVector3Fields,data);data.Pop();}
    for(const auto& child:input.children) if(child.name=="ContactPoints") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+40),20,1));MarshalSlowContact(child,at,data);data.Pop();}
}
void EndianSlowGeometry(unsigned char* output) {
    EndianModuleFields(output,slowGeometryInfoFields);auto list=reinterpret_cast<NativeList*>(output+16);
    for(U32 i=0;i<list->count;++i) {auto row=static_cast<unsigned char*>(list->data)+32*i;EndianModuleFields(row,slowGeometryShapeFields);auto v=*reinterpret_cast<unsigned char**>(row+24);if(v) EndianModuleFields(v,slowVector3Fields);}Swap(&list->count);
    for(unsigned i=0;i<4;++i) {auto v=*reinterpret_cast<unsigned char**>(output+24+4*i);if(v) {if(i==0) EndianModuleFields(v,slowVector2Fields);else EndianModuleFields(v,slowVector3Fields);}}
    auto c=*reinterpret_cast<unsigned char**>(output+40);if(c) EndianSlowContact(c);
}
void MarshalSlowPhaseLists(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    const char* names[]={"FX","OCL","Weapon","Sound"};
    for(unsigned g=0;g<4;++g) {
        auto list=reinterpret_cast<NativeList*>(output+76+8*g);for(const auto& child:input.children) if(child.name==names[g]) ++list->count;
        if(!list->count) continue;
        auto rows=static_cast<unsigned char*>(data.Push(&list->data,12,list->count));U32 i=0;
        for(const auto& child:input.children) if(child.name==names[g]) {auto row=rows+12*i++;if(g==3) MarshalModuleFields(child,row,slowSlowDeathSoundTypeFields,data);else {MarshalSupportReferences(child,names[g],row,4,data);MarshalModuleFields(child,row,slowSlowDeathBaseTypeFields,data);}}
        data.Pop();
    }
}
void EndianSlowPhaseLists(unsigned char* output) {
    for(unsigned g=0;g<4;++g) {auto list=reinterpret_cast<NativeList*>(output+76+8*g);for(U32 i=0;i<list->count;++i){auto row=static_cast<unsigned char*>(list->data)+12*i;if(g==3) EndianModuleFields(row,slowSlowDeathSoundTypeFields);else {EndianModuleFields(row,slowSlowDeathBaseTypeFields);Swap(row+4);}}Swap(&list->count);}
}
void MarshalSlowBase(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,slowSlowDeathBehaviorModuleDataFields,data);MarshalSlowPhaseLists(input,output,data);
    for(const auto& child:input.children) if(child.name=="DieMuxData") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+108),44,1));MarshalModuleFieldSet(child,at,dieFieldSets[7],data);data.Pop();}
}
void EndianSlowBase(unsigned char* output) {EndianModuleFields(output,slowSlowDeathBehaviorModuleDataFields);EndianSlowPhaseLists(output);auto d=*reinterpret_cast<unsigned char**>(output+108);if(d) EndianModuleFieldSet(d,dieFieldSets[7]);}
void MarshalSlowDeath(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned index=kind-265;
    if(index==1 || index==2 || index==4) MarshalModuleFieldSet(input,output,slowFieldSets[index],data);
    if(index==1) {
        for(const auto& child:input.children) if(child.name=="ClearanceGeometry") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+132),48,1));MarshalSlowGeometry(child,at,data);data.Pop();}
        for(const auto& child:input.children) if(child.name=="ClearanceGeometryOffset") {auto at=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+136),12,1));MarshalModuleFields(child,at,slowVector3Fields,data);data.Pop();}
    }
    if(index<4) MarshalSlowBase(input,output,data);
    if(index==4) {
        auto list=reinterpret_cast<NativeList*>(output+64);for(const auto& child:input.children) if(child.name=="Item") ++list->count;
        if(list->count){auto rows=static_cast<unsigned char*>(data.Push(&list->data,20,list->count));U32 i=0;for(const auto& child:input.children) if(child.name=="Item") MarshalModuleFields(child,rows+20*i++,slowBuildingDestructionItemFields,data);data.Pop();}
        MarshalDie(input,output,23,data);
    }
}
void EndianSlowDeath(unsigned char* output,unsigned kind) {
    unsigned index=kind-265;if(index==1 || index==2 || index==4) EndianModuleFieldSet(output,slowFieldSets[index]);
    if(index==1){auto g=*reinterpret_cast<unsigned char**>(output+132);if(g) EndianSlowGeometry(g);auto v=*reinterpret_cast<unsigned char**>(output+136);if(v) EndianModuleFields(v,slowVector3Fields);}
    if(index<4) EndianSlowBase(output);
    if(index==4){auto list=reinterpret_cast<NativeList*>(output+64);for(U32 i=0;i<list->count;++i) EndianModuleFields(static_cast<unsigned char*>(list->data)+20*i,slowBuildingDestructionItemFields);Swap(&list->count);EndianDie(output,23);}
}
