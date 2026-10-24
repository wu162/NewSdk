void MarshalArea(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    unsigned index=kind-238;MarshalModuleFieldSet(input,output,areaFieldSets[index],data);
    if(index==0 || index==3 || index==5) {
        unsigned offset=index==0?80:index==3?20:40;
        for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalAssistFilter(child,output,offset,data);
    }
    if(index==4) for(const auto& child:input.children) if(child.name=="ObjectFilter") MarshalUpgradeFilter(child,reinterpret_cast<ObjectFilter*>(output+32),data);
    if(index==0 || index==2) {
        unsigned offset=index==0?84:20;
        for(const auto& child:input.children) if(child.name=="ObjectStatusValidation") {
            auto value=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+offset),56,1));MarshalModuleFieldSet(child,value,lifecycleFieldSets[6],data);data.Pop();
        }
    }
    if(index==2) for(const auto& child:input.children) {
        if(child.name=="TimeDelayRange") MarshalModuleFields(child,output+24,areaTimeRangeFields,data);
        if(child.name=="RadiusSpreadRange") MarshalModuleFields(child,output+32,areaRealRangeFields,data);
    }
    if(index==5) MarshalWeaponBehavior(input,output,70,data);
}
void EndianArea(unsigned char* output,unsigned kind) {
    unsigned index=kind-238;EndianModuleFieldSet(output,areaFieldSets[index]);
    if(index==0 || index==3 || index==5) EndianAssistFilter(output,index==0?80:index==3?20:40);
    if(index==4) EndianObjectFilter(reinterpret_cast<ObjectFilter*>(output+32));
    if(index==0 || index==2) {auto value=*reinterpret_cast<unsigned char**>(output+(index==0?84:20));if(value) EndianModuleFieldSet(value,lifecycleFieldSets[6]);}
    if(index==2) {EndianModuleFields(output+24,areaTimeRangeFields);EndianModuleFields(output+32,areaRealRangeFields);}
    if(index==5) EndianWeaponBehavior(output,70);
}
