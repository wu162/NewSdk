void MarshalObjectControlList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned stride,unsigned group,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& child:input.children) if(child.name==name) ++list->count;
    if(!list->count) return;
    auto rows=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 i=0;
    for(const auto& child:input.children) if(child.name==name) {
        auto row=rows+stride*i++;
        if(group==0) MarshalModuleFields(child,row,controlUpgradeMapFields,data);
        if(group==1) MarshalModuleFields(child,row,controlSqueezeFlingParticleDataTypeFields,data);
        if(group==2) {for(const auto& filter:child.children) if(filter.name=="ObjectFilter") MarshalUpgradeFilter(filter,reinterpret_cast<ObjectFilter*>(row),data);MarshalSupportReferences(child,"UpgradeSelf",row,112,data);}
    }
    data.Pop();
}
void EndianObjectControlList(unsigned char* output,unsigned offset,unsigned stride,unsigned group) {
    auto list=reinterpret_cast<NativeList*>(output+offset);
    for(U32 i=0;i<list->count;++i) {auto row=static_cast<unsigned char*>(list->data)+stride*i;if(group==0) EndianModuleFields(row,controlUpgradeMapFields);if(group==1) EndianModuleFields(row,controlSqueezeFlingParticleDataTypeFields);if(group==2) {EndianObjectFilter(reinterpret_cast<ObjectFilter*>(row));Swap(row+112);}}
    Swap(&list->count);
}
void MarshalObjectControl(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned index=kind-252;MarshalModuleFieldSet(input,output,controlFieldSets[index],data);
    if(index==1 || index==12) for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+(index==1?28:16)),data);
    if(index==2) {
        for(const auto& child:input.children) if(child.name=="InstaKillObjectFilter") MarshalAssistFilter(child,output,48,data);
        for(const auto& child:input.children) if(child.name=="CollidesWithOnTeleport") MarshalAssistFilter(child,output,52,data);
    }
    if(index==3) {MarshalObjectControlList(input,"UpgradeMap",output,24,8,0,data);MarshalVision(input,output,61,data);}
    if(index==4) {
        for(const auto& child:input.children) if(child.name=="ScatterSettings") MarshalModuleFields(child,output+12,controlScatterDamageFields,data);
        for(const auto& child:input.children) if(child.name=="Filter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+20),data);
    }
    if(index==6) MarshalBeamState(input,output,77,data);
    if(index==7) {
        for(const auto& child:input.children) if(child.name=="Filter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+40),data);
        for(const auto& child:input.children) if(child.name=="KillFilter") MarshalAssistFilter(child,output,152,data);
        MarshalObjectControlList(input,"ParticleData",output,156,20,1,data);
    }
    if(index==8) for(const auto& child:input.children) {if(child.name=="LeftOffset") MarshalModuleFields(child,output+12,controlVector3Fields,data);if(child.name=="RightOffset") MarshalModuleFields(child,output+24,controlVector3Fields,data);}
    if(index==9 || index==10) {
        const char* name=index==9?"ObjectStatusValidation":"SelfValidation";unsigned offset=index==9?36:12;
        for(const auto& child:input.children) if(child.name==name) {auto value=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+offset),56,1));MarshalModuleFieldSet(child,value,lifecycleFieldSets[6],data);data.Pop();}
        if(index==9) for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+40),data);
        if(index==10) MarshalObjectControlList(input,"ObjectEffectTuple",output,16,120,2,data);
    }
    if(index==11) for(const auto& child:input.children) if(child.name=="ConnectionShadowInfo") {auto value=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+20),60,1));MarshalModuleFields(child,value,controlShadowInfoFields,data);data.Pop();}
}
void EndianObjectControl(unsigned char* output,unsigned kind) {
    unsigned index=kind-252;EndianModuleFieldSet(output,controlFieldSets[index]);
    if(index==1 || index==12) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+(index==1?28:16)));
    if(index==2) {EndianAssistFilter(output,48);EndianAssistFilter(output,52);}
    if(index==3) {EndianObjectControlList(output,24,8,0);EndianVision(output,61);}
    if(index==4) {EndianModuleFields(output+12,controlScatterDamageFields);EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+20));}
    if(index==6) EndianBeamState(output,77);
    if(index==7) {EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+40));EndianAssistFilter(output,152);EndianObjectControlList(output,156,20,1);}
    if(index==8) {EndianModuleFields(output+12,controlVector3Fields);EndianModuleFields(output+24,controlVector3Fields);}
    if(index==9 || index==10) {auto value=*reinterpret_cast<unsigned char**>(output+(index==9?36:12));if(value) EndianModuleFieldSet(value,lifecycleFieldSets[6]);if(index==9) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+40));if(index==10) EndianObjectControlList(output,16,120,2);}
    if(index==11) {auto value=*reinterpret_cast<unsigned char**>(output+20);if(value) EndianModuleFields(value,controlShadowInfoFields);}
}
