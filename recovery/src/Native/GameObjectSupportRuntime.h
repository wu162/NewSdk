void MarshalSupportReferences(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,Relo::Tracker& data,bool floats=false) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& c:input.children) if(c.name==name) ++list->count;
    if(!list->count) return;
    auto words=static_cast<U32*>(data.Push(&list->data,4,list->count));U32 i=0;
    for(const auto& c:input.children) if(c.name==name) {if(floats) {float value=ParseFloat(c.text);std::memcpy(words+i,&value,4);}else MarshalReference(data,words+i,c.text);++i;}data.Pop();
}
void MarshalSupport(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned index=kind-244;MarshalModuleFieldSet(input,output,supportFieldSets[index],data);
    if(index==2) MarshalSupportReferences(input,"DependentObjectRadius",output,16,data,true);
    if(index==3) MarshalSupportReferences(input,"Modifier",output,48,data);
    for(const auto& child:input.children) {
        if((index==1 || index==4) && child.name=="ObjectFilter") MarshalAssistFilter(child,output,index==1?36:92,data);
    }
    if(index==3) {
        for(const auto& child:input.children) if(child.name=="AllowFilter") MarshalAssistFilter(child,output,56,data);
        for(const auto& child:input.children) if(child.name=="ContainerFilter") MarshalAssistFilter(child,output,60,data);
        for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {auto value=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+64),56,1));MarshalModuleFieldSet(child,value,lifecycleFieldSets[6],data);data.Pop();}
    }
    if(index==5 || index==7) {
        const char* name=index==5?"Queries":"Event";unsigned offset=index==5?56:52,stride=index==5?12:24;auto list=reinterpret_cast<NativeList*>(output+offset);
        for(const auto& child:input.children) if(child.name==name) ++list->count;
        if(list->count) {
            auto rows=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 i=0;
            for(const auto& child:input.children) if(child.name==name) {auto row=rows+stride*i++;if(index==5) {MarshalModuleFields(child,row,supportQueryFields,data);for(const auto& f:child.children) if(f.name=="ObjectFilter") MarshalAssistFilter(f,row,8,data);}else MarshalModuleFields(child,row,supportFXListBehaviorEventFields,data);}
            data.Pop();
        }
    }
    if(index==6) {
        const char* names[]={"TauntAndPointExcluded","AfraidOf","AlwaysAfraidOf","PointAt"};
        for(unsigned i=0;i<4;++i) for(const auto& child:input.children) if(child.name==names[i]) MarshalAssistFilter(child,output,28+4*i,data);
        MarshalSupportReferences(input,"AddEmotion",output,44,data);
    }
    if(index==0 || index==4) MarshalUpgrade(input,output,30,data);
    if(index==7) MarshalDie(input,output,23,data);
}
void EndianSupport(unsigned char* output,unsigned kind) {
    unsigned index=kind-244;EndianModuleFieldSet(output,supportFieldSets[index]);
    if(index==1 || index==4) EndianAssistFilter(output,index==1?36:92);
    if(index==2) EndianWeakChildren(output,16);
    if(index==3) {Swap(output+48);EndianAssistFilter(output,56);EndianAssistFilter(output,60);auto value=*reinterpret_cast<unsigned char**>(output+64);if(value) EndianModuleFieldSet(value,lifecycleFieldSets[6]);}
    if(index==5 || index==7) {
        auto list=reinterpret_cast<NativeList*>(output+(index==5?56:52));unsigned stride=index==5?12:24;
        for(U32 i=0;i<list->count;++i) {auto row=static_cast<unsigned char*>(list->data)+stride*i;if(index==5) {EndianModuleFields(row,supportQueryFields);EndianAssistFilter(row,8);}else EndianModuleFields(row,supportFXListBehaviorEventFields);}
        Swap(&list->count);
    }
    if(index==6) {for(unsigned i=0;i<4;++i) EndianAssistFilter(output,28+4*i);Swap(output+44);}
    if(index==0 || index==4) EndianUpgrade(output,30);
    if(index==7) EndianDie(output,23);
}
