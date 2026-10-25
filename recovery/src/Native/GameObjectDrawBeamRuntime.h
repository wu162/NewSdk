unsigned BeamShaderKind(U32 type) {return type==2750958653u?1:type==2300057986u?2:type==2777716390u?3:type==3741900581u?4:0;}
const ModuleLayout* BeamShaderLayout(unsigned kind) {return kind==1?&beamFXShaderConstantBool:kind==2?&beamFXShaderConstantInt:kind==3?&beamFXShaderConstantTexture:kind==4?&beamFXShaderConstantFloat:nullptr;}
void MarshalBeamShaderConstant(const GameObjectChild& input,void** slot,Relo::Tracker& data) {
    auto t=input.attributes.find("TypeId");U32 type=t==input.attributes.end()?0:ParseUnsigned(t->second);unsigned kind=BeamShaderKind(type);
    auto row=static_cast<unsigned char*>(data.Push(slot,kind==4?20:kind?16:12,1));
    if(kind==4){auto list=reinterpret_cast<NativeList*>(row+12);for(const auto& v:input.children) if(v.name=="Value") ++list->count;if(list->count){auto values=static_cast<float*>(data.Push(&list->data,4,list->count));U32 i=0;for(const auto& v:input.children) if(v.name=="Value") values[i++]=ParseFloat(v.text);data.Pop();}}
    else if(kind)for(const auto& v:input.children) if(v.name=="Value"){if(kind==1)row[12]=ParseBoolean(v.text)?1:0;else if(kind==2){U32 word=static_cast<U32>(ParseSigned(v.text));std::memcpy(row+12,&word,4);}else MarshalReference(data,row+12,v.text);}
    MarshalModuleFields(input,row,beamFXShaderConstantFields,data);*reinterpret_cast<U32*>(row)=kind?type:0;data.Pop();
}
void MarshalBeamShaderMaterial(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,beamFXShaderMaterialFields,data);
    for(const auto& c:input.children) if(c.name=="Constants"){auto list=reinterpret_cast<NativeList*>(output+16);list->count=static_cast<U32>(c.children.size());if(list->count){auto slots=static_cast<void**>(data.Push(&list->data,4,list->count));for(U32 i=0;i<list->count;++i)MarshalBeamShaderConstant(c.children[i],slots+i,data);data.Pop();}}
}
void MarshalDrawBeam(const GameObjectChild& input,unsigned char* output,unsigned kind,Relo::Tracker& data) {
    MarshalModuleFieldSet(input,output,beamFieldSets[kind-283],data);
    if(kind==284){for(const char* n:{"HeadColor","TailColor"})for(const auto& c:input.children)if(c.name==n)MarshalModuleFields(c,output+(c.name=="HeadColor"?72:88),beamColor4fFields,data);}
    else for(const auto& c:input.children)if(c.name=="FXShader")MarshalBeamShaderMaterial(c,output+(kind==283?76:40),data);
    if(kind!=285)for(const auto& c:input.children)if(c.name=="ObjectStatusValidation"){auto row=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+104),56,1));MarshalModuleFieldSet(c,row,lifecycleFieldSets[6],data);data.Pop();}
    if(kind==285)for(const auto& c:input.children)if(c.name=="ParticleSystems"){auto row=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+68),8,1));MarshalSupportReferences(c,"ParticleSys",row,0,data);data.Pop();}
}
void EndianBeamShaderConstant(unsigned char* row) {
    unsigned kind=BeamShaderKind(*reinterpret_cast<U32*>(row));if(kind==2)Swap(row+12);if(kind==4){auto list=reinterpret_cast<NativeList*>(row+12);for(U32 i=0;i<list->count;++i)Swap(static_cast<U32*>(list->data)+i);Swap(&list->count);}Swap(row);Swap(row+4);
}
void EndianBeamShaderMaterial(unsigned char* output) {
    EndianModuleFields(output,beamFXShaderMaterialFields);auto list=reinterpret_cast<NativeList*>(output+16);for(U32 i=0;i<list->count;++i)EndianBeamShaderConstant(static_cast<unsigned char**>(list->data)[i]);Swap(&list->count);
}
void EndianDrawBeam(unsigned char* output,unsigned kind) {
    EndianModuleFieldSet(output,beamFieldSets[kind-283]);
    if(kind==284){EndianModuleFields(output+72,beamColor4fFields);EndianModuleFields(output+88,beamColor4fFields);}else EndianBeamShaderMaterial(output+(kind==283?76:40));
    if(kind!=285){auto v=*reinterpret_cast<unsigned char**>(output+104);if(v)EndianModuleFieldSet(v,lifecycleFieldSets[6]);}
    if(kind==285){auto v=*reinterpret_cast<unsigned char**>(output+68);if(v)Swap(v);}
}
