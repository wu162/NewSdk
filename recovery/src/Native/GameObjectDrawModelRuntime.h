// Full Scripted model root and concrete Spotlight/Tank/Truck paths.
template<class Marshal>
void MarshalDrawMainList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned stride,Relo::Tracker& data,Marshal marshal) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& c:input.children)if(c.name==name)++list->count;
    if(list->count){auto rows=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 i=0;for(const auto& c:input.children)if(c.name==name)marshal(c,rows+stride*i++,data);data.Pop();}
}
template<class Endian>
void EndianDrawMainList(unsigned char* output,unsigned offset,unsigned stride,Endian endian) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(U32 i=0;i<list->count;++i)endian(static_cast<unsigned char*>(list->data)+stride*i);Swap(&list->count);
}
void MarshalDrawMainAttach(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,drawMainScriptedModelDrawAttachModelFieldSet,data);
    for(const auto& c:input.children)if(c.name=="Model"){MarshalModuleFieldSet(c,output+68,drawMainAttachModelStructFieldSet,data);break;}
}
void EndianDrawMainAttach(unsigned char* output){EndianModuleFieldSet(output,drawMainScriptedModelDrawAttachModelFieldSet);EndianModuleFieldSet(output+68,drawMainAttachModelStructFieldSet);}
void MarshalDrawMainPortal(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data){MarshalModuleFieldSet(input,output,drawMainScriptedModelDrawEmbedPortalFieldSet,data);}
void EndianDrawMainPortal(unsigned char* output){EndianModuleFieldSet(output,drawMainScriptedModelDrawEmbedPortalFieldSet);}
void MarshalDrawScriptedModel(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,drawMainW3DScriptedModelDrawModuleDataFieldSet,data);
    MarshalDrawMainList(input,"ModelConditionState",output,164,196,data,MarshalDrawModelCondition);
    MarshalDrawMainList(input,"AnimationState",output,172,140,data,MarshalDrawAnimationState);
    MarshalDrawMainList(input,"AttachModel",output,180,80,data,MarshalDrawMainAttach);
    MarshalDrawMainList(input,"EmbedPortal",output,188,12,data,MarshalDrawMainPortal);
}
void EndianDrawScriptedModel(unsigned char* output) {
    EndianModuleFieldSet(output,drawMainW3DScriptedModelDrawModuleDataFieldSet);
    EndianDrawMainList(output,164,196,EndianDrawModelCondition);EndianDrawMainList(output,172,140,EndianDrawAnimationState);
    EndianDrawMainList(output,180,80,EndianDrawMainAttach);EndianDrawMainList(output,188,12,EndianDrawMainPortal);
}
void MarshalDrawMainArticulation(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data){MarshalModuleFieldSet(input,output,drawMainArticulationBoneFieldSet,data);}
void EndianDrawMainArticulation(unsigned char* output){EndianModuleFieldSet(output,drawMainArticulationBoneFieldSet);}
void MarshalDrawMainTread(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data){MarshalModuleFieldSet(input,output,drawMainTreadScrollingMappingFieldSet,data);}
void EndianDrawMainTread(unsigned char* output){EndianModuleFieldSet(output,drawMainTreadScrollingMappingFieldSet);}
void MarshalDrawModel(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data){
    if(kind==287)MarshalModuleFieldSet(input,output,drawMainW3DSpotlightDrawModuleDataFieldSet,data);
    if(kind==290)MarshalModuleFieldSet(input,output,drawMainW3DTruckDrawModuleDataFieldSet,data);
    if(kind==288 || kind==290){for(const auto& c:input.children)if(c.name=="TreadTerrainParticles"){auto ref=data.Push(reinterpret_cast<void**>(output+(kind==288?216:384)),4,1);MarshalReference(data,ref,c.text);data.Pop();break;}}
    if(kind==288)MarshalDrawMainList(input,"ArticulationBone",output,220,16,data,MarshalDrawMainArticulation);
    if(kind==290)MarshalDrawMainList(input,"TreadScrollingMap",output,388,28,data,MarshalDrawMainTread);
    if(kind==289)MarshalDrawDynamics(input,output,data);
    MarshalDrawScriptedModel(input,output,data);
}
void EndianDrawModel(unsigned char* output,unsigned kind){
    if(kind==287)EndianModuleFieldSet(output,drawMainW3DSpotlightDrawModuleDataFieldSet);
    if(kind==290)EndianModuleFieldSet(output,drawMainW3DTruckDrawModuleDataFieldSet);
    if(kind==288)EndianDrawMainList(output,220,16,EndianDrawMainArticulation);
    if(kind==290)EndianDrawMainList(output,388,28,EndianDrawMainTread);
    if(kind==289)EndianDrawDynamics(output);
    EndianDrawScriptedModel(output);
}
