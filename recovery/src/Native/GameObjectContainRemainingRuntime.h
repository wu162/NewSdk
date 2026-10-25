// Complete remaining Contain family; allocation follows original schema order.
template<std::size_t N>
void MarshalContainRows(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned stride,const ModuleField (&fields)[N],Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& c:input.children) if(c.name==name) ++list->count;
    if(list->count){auto rows=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 i=0;for(const auto& c:input.children) if(c.name==name) MarshalModuleFields(c,rows+stride*i++,fields,data);data.Pop();}
}
void MarshalContainRestVector(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned dims,Relo::Tracker& data) {
    for(const auto& c:input.children) if(c.name==name){auto v=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+offset),4*dims,1));for(unsigned i=0;i<dims;++i){auto found=c.attributes.find(i==0?"x":i==1?"y":"z");float n=found==c.attributes.end()?0.0f:ParseFloat(found->second);std::memcpy(v+4*i,&n,4);}data.Pop();}
}
void MarshalContainRestWords(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,bool asset,Relo::Tracker& data) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& c:input.children) if(c.name==name) ++list->count;
    if(list->count){auto rows=static_cast<U32*>(data.Push(&list->data,4,list->count));U32 i=0;for(const auto& c:input.children) if(c.name==name) rows[i++]=asset?HashAssetId(c.text):static_cast<U32>(ParseSigned(c.text));data.Pop();}
}
void MarshalContainTransport(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,containRestTransportContainModuleDataFields,data);
    MarshalContainRows(input,"InitialPayload",output,296,8,containRestInitialPayloadFields,data);
    MarshalContainRestVector(input,"ThrowOutPassengersVelocity",output,304,3,data);
    MarshalContainRows(input,"UpgradeCreationTrigger",output,308,12,containRestUpgradeCreationFields,data);
    for(const auto& c:input.children) if(c.name=="FadeFilter") MarshalAssistFilter(c,output,316,data);
    MarshalContainOpen(input,output,data);
}
void MarshalContainRemaining(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    if(kind==280){MarshalContainTransport(input,output,data);return;}
    MarshalModuleFieldSet(input,output,containRestFieldSets[kind-278],data);
    if(kind==278){for(const auto& c:input.children) if(c.name=="CanEnterFilter") MarshalAssistFilter(c,output,80,data);return;}
    if(kind==279)return;
    if(kind==281){MarshalContainTransport(input,output,data);return;}
    MarshalContainRestVector(input,"RandomOffset",output,432,2,data);
    auto ranks=reinterpret_cast<NativeList*>(output+436);for(const auto& c:input.children) if(c.name=="RankInfo") ++ranks->count;
    if(ranks->count){auto rows=static_cast<unsigned char*>(data.Push(&ranks->data,16,ranks->count));U32 i=0;for(const auto& c:input.children) if(c.name=="RankInfo"){auto row=rows+16*i++;MarshalModuleFields(c,row,containRestRankInfoTypeFields,data);MarshalContainRows(c,"Position",row,8,16,containRestPositionAndLeaderTypeFields,data);}data.Pop();}
    MarshalContainRestWords(input,"RankThatStopsAdvance",output,444,false,data);
    MarshalContainRestWords(input,"RankToReleaseWhenAttacking",output,452,false,data);
    MarshalContainRestVector(input,"LeaderPosition",output,460,2,data);
    auto banners=reinterpret_cast<NativeList*>(output+464);for(const auto& c:input.children) if(c.name=="BannerCarrierPosition") ++banners->count;
    if(banners->count){auto rows=static_cast<unsigned char*>(data.Push(&banners->data,12,banners->count));U32 i=0;for(const auto& c:input.children) if(c.name=="BannerCarrierPosition"){auto row=rows+12*i++;for(const auto& v:c.children){if(v.name=="UnitType"){U32 id=HashAssetId(v.text);std::memcpy(row,&id,4);}if(v.name=="Pos")for(unsigned j=0;j<2;++j){auto a=v.attributes.find(j==0?"x":"y");float n=a==v.attributes.end()?0.0f:ParseFloat(a->second);std::memcpy(row+4+4*j,&n,4);}}}data.Pop();}
    MarshalContainRestWords(input,"BannerCarriersAllowed",output,472,true,data);
    MarshalContainRestWords(input,"LeadersAllowed",output,480,true,data);
    MarshalSupportReferences(input,"AttributeModifier",output,488,data);
    MarshalContainTransport(input,output,data);
}
template<std::size_t N>
void EndianContainRows(unsigned char* output,unsigned offset,unsigned stride,const ModuleField (&fields)[N]) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(U32 i=0;i<list->count;++i) EndianModuleFields(static_cast<unsigned char*>(list->data)+stride*i,fields);Swap(&list->count);
}
void EndianContainRestWords(unsigned char* output,unsigned offset,bool reference) {
    auto list=reinterpret_cast<NativeList*>(output+offset);if(!reference) for(U32 i=0;i<list->count;++i) Swap(static_cast<U32*>(list->data)+i);Swap(&list->count);
}
void EndianContainRestVector(unsigned char* output,unsigned offset,unsigned dims) {auto v=*reinterpret_cast<U32**>(output+offset);if(v)for(unsigned j=0;j<dims;++j)Swap(v+j);}
void EndianContainTransport(unsigned char* output) {
    EndianModuleFields(output,containRestTransportContainModuleDataFields);
    EndianContainRows(output,296,8,containRestInitialPayloadFields);EndianContainRestVector(output,304,3);EndianContainRows(output,308,12,containRestUpgradeCreationFields);EndianAssistFilter(output,316);EndianContainOpen(output);
}
void EndianContainRemaining(unsigned char* output,unsigned kind) {
    if(kind==280){EndianContainTransport(output);return;}
    EndianModuleFieldSet(output,containRestFieldSets[kind-278]);
    if(kind==278){EndianAssistFilter(output,80);return;}if(kind==279)return;if(kind==281){EndianContainTransport(output);return;}
    EndianContainRestVector(output,432,2);auto ranks=reinterpret_cast<NativeList*>(output+436);for(U32 i=0;i<ranks->count;++i){auto row=static_cast<unsigned char*>(ranks->data)+16*i;EndianModuleFields(row,containRestRankInfoTypeFields);EndianContainRows(row,8,16,containRestPositionAndLeaderTypeFields);}Swap(&ranks->count);
    EndianContainRestWords(output,444,false);EndianContainRestWords(output,452,false);EndianContainRestVector(output,460,2);
    auto banners=reinterpret_cast<NativeList*>(output+464);for(U32 i=0;i<banners->count;++i)for(unsigned j=0;j<3;++j)Swap(static_cast<U32*>(banners->data)+3*i+j);Swap(&banners->count);
    EndianContainRestWords(output,472,false);EndianContainRestWords(output,480,false);EndianContainRestWords(output,488,true);EndianContainTransport(output);
}
