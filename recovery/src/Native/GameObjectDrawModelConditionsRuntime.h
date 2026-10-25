// Complete shared model condition, particle and FX marshalling.
void MarshalDrawParticleSysBone(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {MarshalModuleFields(input,output,drawModelParticleSysBoneFields,data);}
void EndianDrawParticleSysBone(unsigned char* output) {EndianModuleFields(output,drawModelParticleSysBoneFields);}
void MarshalDrawFXEvent(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {MarshalModuleFields(input,output,drawModelFXEventFields,data);}
void EndianDrawFXEvent(unsigned char* output) {EndianModuleFields(output,drawModelFXEventFields);}
template<class F> void MarshalDrawModelList(const GameObjectChild& input,const char* name,unsigned char* output,unsigned offset,unsigned stride,Relo::Tracker& data,F marshal) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(const auto& c:input.children)if(c.name==name)++list->count;
    if(!list->count)return;auto rows=static_cast<unsigned char*>(data.Push(&list->data,stride,list->count));U32 i=0;
    for(const auto& c:input.children)if(c.name==name)marshal(c,rows+stride*i++,data);data.Pop();
}
template<class F> void EndianDrawModelList(unsigned char* output,unsigned offset,unsigned stride,F endian) {
    auto list=reinterpret_cast<NativeList*>(output+offset);for(U32 i=0;i<list->count;++i)endian(static_cast<unsigned char*>(list->data)+stride*i);Swap(&list->count);
}
void MarshalDrawModelCondition(const GameObjectChild& input,unsigned char* output,Relo::Tracker& data) {
    MarshalModuleFields(input,output,drawModelModelConditionStateFields,data);
    MarshalDrawModelList(input,"Model",output,104,8,data,[](const GameObjectChild& c,unsigned char* r,Relo::Tracker& d){MarshalModuleFields(c,r,drawModelScriptedModelDrawModelFields,d);});
    for(const auto& c:input.children)if(c.name=="Material"){auto r=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+112),28,1));MarshalBeamShaderMaterial(c,r,data);data.Pop();break;}
    MarshalDrawModelList(input,"Texture",output,116,8,data,[](const GameObjectChild& c,unsigned char* r,Relo::Tracker& d){MarshalModuleFields(c,r,drawModelReplaceTextureFields,d);});
    const char* bones[]={"WeaponFireFXBone","WeaponRecoilBone","WeaponMuzzleFlash","WeaponLaunchBone"};
    for(unsigned i=0;i<4;++i)MarshalDrawModelList(input,bones[i],output,124+8*i,16,data,[](const GameObjectChild& c,unsigned char* r,Relo::Tracker& d){MarshalModuleFields(c,r,drawModelBoneAttachPointFields,d);});
    MarshalDrawModelList(input,"ParticleSysBone",output,156,36,data,MarshalDrawParticleSysBone);
    MarshalDrawModelList(input,"FXEvent",output,164,28,data,MarshalDrawFXEvent);
    for(const auto& c:input.children)if(c.name=="ShadowInfo"){auto r=static_cast<unsigned char*>(data.Push(reinterpret_cast<void**>(output+172),60,1));MarshalModuleFields(c,r,drawModelShadowInfoFields,data);data.Pop();break;}
    MarshalDrawModelList(input,"Turret",output,176,28,data,[](const GameObjectChild& c,unsigned char* r,Relo::Tracker& d){MarshalModuleFields(c,r,drawModelModelConditionStateTurretFields,d);});
    MarshalDrawModelList(input,"SubObject",output,184,4,data,[](const GameObjectChild& c,unsigned char* r,Relo::Tracker&){U32 v=HashAssetId(c.text);std::memcpy(r,&v,4);});
}
void EndianDrawModelCondition(unsigned char* output) {
    EndianModuleFields(output,drawModelModelConditionStateFields);
    // Model and texture rows contain only references/bools: their list counts alone swap.
    Swap(output+104);auto material=*reinterpret_cast<unsigned char**>(output+112);if(material)EndianBeamShaderMaterial(material);Swap(output+116);
    for(unsigned i=0;i<4;++i)EndianDrawModelList(output,124+8*i,16,[](unsigned char* r){EndianModuleFields(r,drawModelBoneAttachPointFields);});
    EndianDrawModelList(output,156,36,EndianDrawParticleSysBone);EndianDrawModelList(output,164,28,EndianDrawFXEvent);
    auto shadow=*reinterpret_cast<unsigned char**>(output+172);if(shadow)EndianModuleFields(shadow,drawModelShadowInfoFields);
    EndianDrawModelList(output,176,28,[](unsigned char* r){EndianModuleFields(r,drawModelModelConditionStateTurretFields);});
    EndianDrawModelList(output,184,4,[](unsigned char* r){Swap(r);});
}
std::string DrawModelConditionRelativePath(const std::string& path) {
    if(path=="ModelConditionState")return "";
    if(path.compare(0,20,"ModelConditionState/")==0)return path.substr(20);
    return "#invalid";
}
const ModuleFieldSet DrawModelConditionFieldSet(const std::string& path) {
    std::string p=DrawModelConditionRelativePath(path);
    if(p.empty())return {drawModelModelConditionStateFields,sizeof(drawModelModelConditionStateFields)/sizeof(ModuleField)};
    if(p=="Model")return {drawModelScriptedModelDrawModelFields,sizeof(drawModelScriptedModelDrawModelFields)/sizeof(ModuleField)};
    if(p=="Texture")return {drawModelReplaceTextureFields,2};
    if(p=="WeaponFireFXBone"||p=="WeaponRecoilBone"||p=="WeaponMuzzleFlash"||p=="WeaponLaunchBone")return {drawModelBoneAttachPointFields,3};
    if(p=="ParticleSysBone")return {drawModelParticleSysBoneFields,9};if(p=="FXEvent")return {drawModelFXEventFields,6};if(p=="ShadowInfo")return {drawModelShadowInfoFields,sizeof(drawModelShadowInfoFields)/sizeof(ModuleField)};if(p=="Turret")return {drawModelModelConditionStateTurretFields,5};
    if(p=="Material")return {drawBeamFXShaderMaterialFields,3};return {nullptr,0};
}
bool DrawModelConditionAttribute(const std::string& path,const std::string& attribute) {
    auto fs=DrawModelConditionFieldSet(path);for(unsigned i=0;i<fs.count;++i)if(attribute==fs.fields[i].name)return true;return false;
}
unsigned DrawModelConditionNodeKind(const std::string& path) {
    auto p=DrawModelConditionRelativePath(path);
    if(p.empty())return 2;if(p=="Material"||p=="ShadowInfo"||p=="Material/Constants")return 1;
    if(p=="Model"||p=="Texture"||p=="WeaponFireFXBone"||p=="WeaponRecoilBone"||p=="WeaponMuzzleFlash"||p=="WeaponLaunchBone"||p=="ParticleSysBone"||p=="FXEvent"||p=="Turret")return 2;
    if(p=="SubObject")return 3;return 0;
}
