bool DrawMainHasField(const ModuleFieldSet& set,const std::string& attribute){for(unsigned i=0;i<set.count;++i)if(attribute==set.fields[i].name)return true;return false;}
bool DrawModelRootAttribute(unsigned kind,const std::string& attribute){
    if(DrawMainHasField(drawMainW3DScriptedModelDrawModuleDataFieldSet,attribute))return true;
    if(kind==287)return DrawMainHasField(drawMainW3DSpotlightDrawModuleDataFieldSet,attribute);
    if(kind==290)return DrawMainHasField(drawMainW3DTruckDrawModuleDataFieldSet,attribute);
    if(kind==289)return DrawDynamicsRootAttribute(attribute);
    return false;
}
unsigned DrawModelNodeKind(unsigned kind,const std::string& path){
    if(path=="ModelConditionState" || path=="AnimationState" || path=="AttachModel" || path=="EmbedPortal")return 2;
    if(path=="AttachModel/Model")return 1;
    if((kind==288 || kind==290) && path=="TreadTerrainParticles")return 4;
    if(kind==288 && path=="ArticulationBone")return 2;
    if(kind==290 && path=="TreadScrollingMap")return 2;
    auto v=DrawModelConditionNodeKind(path);if(v)return v;
    v=DrawAnimationNodeKind(path);if(v)return v;
    return kind==289?DrawDynamicsNodeKind(path):0;
}
bool DrawModelNodeAttribute(unsigned kind,const std::string& path,const std::string& attribute){
    if(path=="AttachModel")return DrawMainHasField(drawMainScriptedModelDrawAttachModelFieldSet,attribute);
    if(path=="AttachModel/Model")return DrawMainHasField(drawMainAttachModelStructFieldSet,attribute);
    if(path=="EmbedPortal")return DrawMainHasField(drawMainScriptedModelDrawEmbedPortalFieldSet,attribute);
    if(kind==288 && path=="ArticulationBone")return DrawMainHasField(drawMainArticulationBoneFieldSet,attribute);
    if(kind==290 && path=="TreadScrollingMap")return DrawMainHasField(drawMainTreadScrollingMappingFieldSet,attribute);
    return DrawModelConditionAttribute(path,attribute) || DrawAnimationAttribute(path,attribute) || (kind==289 && DrawDynamicsAttribute(path,attribute));
}
